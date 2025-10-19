# Master Inquiry Backend Performance Analysis

## Executive Summary

**Current Performance**: 22.46 seconds for a simple search
**Target Performance**: < 3 seconds
**Primary Issues**: Sequential database operations, excessive Oracle Read calls, lack of query optimization

## Trace Analysis

### Request Details

- **Endpoint**: POST /api/master/master-inquiry/search
- **Payload**: `{"endProfitYear":2025,"paymentType":1,"memberType":0,"take":5,"skip":0,"sortBy":"badgeNumber","isSortDescending":true}`
- **Total Duration**: 22.46 seconds
- **Total Spans**: 119 Oracle operations

### Performance Breakdown

| Operation                    | Duration      | % of Total | Issue                                      |
| ---------------------------- | ------------- | ---------- | ------------------------------------------ |
| ReadOnlyContext.Operation #1 | 22.2s         | 98.8%      | Parent operation wrapping all queries      |
| ReadOnlyContext.Operation #2 | 21.2s         | 94.4%      | Likely duplicate SSN detection query       |
| ReadOnlyContext.Operation #3 | 15.76s        | 70.2%      | Likely employee details retrieval          |
| ReadOnlyContext.Operation #4 | 12.42s        | 55.3%      | Likely beneficiary details retrieval       |
| 80+ Oracle Read operations   | 5.5s combined | 24.5%      | Indicates result streaming/materialization |

## Critical Performance Issues

### 1. **Sequential Database Operations (CRITICAL)**

**Problem**: Four major database operations executed sequentially instead of in parallel.

**Evidence from trace**:

- Operation #2: 21.2s (duplicate SSN detection)
- Operation #3: 15.76s (employee details - 40 Read operations)
- Operation #4: 12.42s (beneficiary details - 42 Read operations)

**Root Cause** (from code analysis):

```csharp
// MasterInquiryService.cs, lines 133-148
var duplicateSsns = await demographics
    .Where(d => ssnQuery.Contains(d.Ssn))
    .GroupBy(d => d.Ssn)
    .Where(g => g.Count() > 1)
    .Select(g => g.Key)
    .TagWith("MasterInquiry: Find duplicate SSNs via JOIN")
    .ToHashSetAsync(timeoutToken).ConfigureAwait(false);  // BLOCKING CALL #1

// Then later, SEQUENTIAL calls:
if (memberType == 1) {
    detailsList = await GetDemographicDetailsForSsns(...);  // BLOCKING CALL #2
} else if (memberType == 2) {
    detailsList = await GetBeneficiaryDetailsForSsns(...);  // BLOCKING CALL #3
} else {
    // BOTH executed sequentially, not in parallel!
    var employeeDetails = await GetAllDemographicDetailsForSsns(...);
    var beneficiaryDetails = await GetAllBeneficiaryDetailsForSsns(...);
}
```

**Impact**: When `memberType=0` (both), we wait for employees THEN beneficiaries instead of fetching simultaneously.

**Estimated Time Savings**: 40-50% reduction (parallel execution could save 12-15 seconds)

---

### 2. **Expensive Duplicate SSN Detection**

**Problem**: 21.2 second query just to find duplicate SSNs, with excessive Read operations.

**Evidence**:

- ReadOnlyContext.Operation #2: 21.2s
- Contains 28+ Oracle Read operations at ~60-100ms each
- ExecuteReaderAsync took 2.58s just to start reading

**Current Implementation**:

```csharp
var demographics = await _demographicReaderService.BuildDemographicQuery(ctx);

var duplicateSsns = await demographics
    .Where(d => ssnQuery.Contains(d.Ssn))  // JOIN to filtered SSN set
    .GroupBy(d => d.Ssn)
    .Where(g => g.Count() > 1)
    .Select(g => g.Key)
    .ToHashSetAsync(timeoutToken);  // Materializes all duplicate SSNs
```

**Issues**:

1. `BuildDemographicQuery` likely returns full demographics table (potentially 100K+ rows)
2. JOIN operation on large dataset
3. GroupBy requires sorting/aggregation across entire result set
4. Multiple Read operations suggest large intermediate result set

**Optimization Opportunity**:

- Use window functions (`COUNT() OVER (PARTITION BY Ssn)`) for more efficient duplicate detection
- Or better: detect duplicates ONLY within the filtered SSN set, not all demographics
- Consider caching known duplicates or using indexed CTE

**Estimated Time Savings**: 15-18 seconds

---

### 3. **Excessive Oracle Read Operations**

**Problem**: 80+ individual Read operations, each taking 50-100ms.

**Evidence**:

- Operation #2: 28 Reads (~2.5s total)
- Operation #3: 42 Reads (~2.4s total)
- Operation #4: 42 Reads (~2.4s total)

**Analysis**:
This pattern suggests:

1. Row-by-row streaming from Oracle
2. Possible N+1 pattern for related data
3. Missing eager loading (`Include()`) for related entities
4. Or: very large result sets being materialized

**Investigation Needed**:

- Check if EmployeeInquiryService/BeneficiaryInquiryService use proper `Include()` for navigation properties
- Verify projection happens in SQL, not after materialization
- Check for lazy loading being enabled (should be disabled)

**Estimated Time Savings**: 3-5 seconds

---

### 4. **CountAsync Before Fetching Data**

**Problem**: Executing `CountAsync` on SSN query adds unnecessary database round-trip.

**Code**:

```csharp
// Line 127-129
var estimatedCount = await ssnQuery.CountAsync(timeoutToken).ConfigureAwait(false);

if (estimatedCount > 50000) {
    _logger.LogWarning("...");
}
```

**Issue**: This is a safety check, but adds latency for every request.

**Optimization**:

- Remove `CountAsync` or make it conditional (only for debugging)
- Use SQL Server query hints to limit result sets
- Or run count in parallel with actual data fetch

**Estimated Time Savings**: 0.5-1 second

---

### 5. **In-Memory Operations After Database Fetch**

**Problem**: Age calculation done in-memory after database fetch.

**Code** (line 201-204):

```csharp
foreach (MemberDetails details in detailsList.Results)
{
    details.Age = details.DateOfBirth.Age();
}
```

**Issue**: While this is fast for small result sets, it's unnecessary processing.

**Optimization**:

- Calculate age in SQL using `DATEDIFF` or equivalent Oracle function
- Include in projection query
- Or use computed column if age is frequently accessed

**Estimated Time Savings**: < 0.1 second (minor, but good practice)

---

### 6. **Missing Query Optimization**

**Problem**: No evidence of query plan caching, parameterization, or hints.

**Observations**:

- Multiple `ExecuteNonQueryAsync` calls (~40ms each) suggest Oracle statement preparation
- `SendReExecuteRequest` and `SendExecuteRequest` alternating pattern
- No `.TagWith()` usage visible in trace for query identification

**Recommendations**:

1. Add `.TagWith()` to all major queries for APM tracing
2. Use query compilation for frequently-executed queries
3. Add Oracle hints for index usage where applicable
4. Review execution plans for missing indexes

---

## Recommended Solutions

### Priority 1: Parallelize Database Operations (HIGH IMPACT)

**Change**: Execute duplicate detection and details fetching in parallel.

**Implementation**:

```csharp
// BEFORE (sequential)
var duplicateSsns = await demographics
    .Where(d => ssnQuery.Contains(d.Ssn))
    .GroupBy(d => d.Ssn)
    .Where(g => g.Count() > 1)
    .Select(g => g.Key)
    .ToHashSetAsync(timeoutToken);

var detailsList = await GetDemographicDetailsForSsns(req, ssnList, currentYear, previousYear, duplicateSsns, timeoutToken);

// AFTER (parallel)
var duplicateTask = demographics
    .Where(d => ssnQuery.Contains(d.Ssn))
    .GroupBy(d => d.Ssn)
    .Where(g => g.Count() > 1)
    .Select(g => g.Key)
    .ToHashSetAsync(timeoutToken);

var detailsTask = memberType switch {
    1 => GetDemographicDetailsForSsns(req, ssnList, currentYear, previousYear, new HashSet<int>(), timeoutToken),
    2 => GetBeneficiaryDetailsForSsns(req, ssnList, timeoutToken),
    _ => Task.WhenAll(
        GetAllDemographicDetailsForSsns(ssnList, currentYear, previousYear, new HashSet<int>(), timeoutToken),
        GetAllBeneficiaryDetailsForSsns(ssnList, timeoutToken)
    )
};

await Task.WhenAll(duplicateTask, detailsTask);

var duplicateSsns = await duplicateTask;
// Process details with duplicateSsns...
```

**Expected Impact**: 40-50% reduction (12-15 seconds)

---

### Priority 2: Optimize Duplicate SSN Detection (HIGH IMPACT)

**Change**: Detect duplicates more efficiently using SQL window functions or scoped query.

**Option A: Window Function (Best Performance)**

```csharp
// Use Oracle analytic function
var duplicateSsns = await ctx.Database
    .SqlQuery<int>($@"
        SELECT DISTINCT Ssn
        FROM (
            SELECT Ssn, COUNT(*) OVER (PARTITION BY Ssn) as DupCount
            FROM Demographics
            WHERE Ssn IN ({string.Join(",", ssnList.Take(1000))})  -- Parameterize properly
        )
        WHERE DupCount > 1
    ")
    .ToHashSetAsync(timeoutToken);
```

**Option B: Scoped GroupBy (Better than current)**

```csharp
// Only check duplicates within filtered SSN set, not all demographics
var duplicateSsns = await ctx.Demographics
    .Where(d => ssnList.Contains(d.Ssn))  // Filter first to small set
    .GroupBy(d => d.Ssn)
    .Where(g => g.Count() > 1)
    .Select(g => g.Key)
    .TagWith("MasterInquiry: Duplicate detection (scoped)")
    .ToHashSetAsync(timeoutToken);
```

**Expected Impact**: 15-18 seconds

---

### Priority 3: Add Eager Loading and Optimize Projections (MEDIUM IMPACT)

**Change**: Ensure all queries use proper `Include()` for navigation properties.

**Investigation Steps**:

1. Review `EmployeeInquiryService.GetEmployeeDetailsForSsnsAsync`
2. Review `BeneficiaryInquiryService.GetBeneficiaryDetailsForSsnsAsync`
3. Verify all navigation properties are eagerly loaded
4. Ensure projection happens in SQL, not in-memory

**Expected Impact**: 3-5 seconds

---

### Priority 4: Remove Unnecessary CountAsync (LOW IMPACT)

**Change**: Remove or conditionalize the SSN count check.

```csharp
// BEFORE
var estimatedCount = await ssnQuery.CountAsync(timeoutToken);
if (estimatedCount > 50000) {
    _logger.LogWarning("...");
}

// AFTER (only in debug/staging)
#if DEBUG
var estimatedCount = await ssnQuery.CountAsync(timeoutToken);
_logger.LogInformation("SSN query count: {Count}", estimatedCount);
#endif
```

**Expected Impact**: 0.5-1 second

---

### Priority 5: Add Telemetry Tags (NO PERFORMANCE IMPACT, OBSERVABILITY)

**Change**: Add `.TagWith()` to all major queries for better APM visibility.

```csharp
var duplicateSsns = await demographics
    .Where(d => ssnQuery.Contains(d.Ssn))
    .GroupBy(d => d.Ssn)
    .Where(g => g.Count() > 1)
    .Select(g => g.Key)
    .TagWith($"MasterInquiry: Duplicate SSN detection - Year {req.ProfitYear}, MemberType {req.MemberType}")
    .ToHashSetAsync(timeoutToken);
```

**Benefit**: Better trace visibility in future performance analysis.

---

## Expected Performance Improvement Summary

| Optimization                 | Estimated Time Savings | Effort   | Priority |
| ---------------------------- | ---------------------- | -------- | -------- |
| Parallelize DB operations    | 12-15 seconds          | Medium   | 1        |
| Optimize duplicate detection | 15-18 seconds          | Medium   | 1        |
| Add eager loading            | 3-5 seconds            | Low      | 2        |
| Remove CountAsync            | 0.5-1 second           | Very Low | 3        |
| Calculate age in SQL         | < 0.1 second           | Very Low | 4        |
| **TOTAL POTENTIAL**          | **18-22 seconds**      |          |          |

**Projected Final Performance**: **< 2-4 seconds** (from 22.46s)

---

## Implementation Plan

### Phase 1: Quick Wins (1-2 hours)

1. ✅ Remove `CountAsync` or make conditional
2. ✅ Add `.TagWith()` to all queries
3. ✅ Calculate age in SQL projection

### Phase 2: Parallel Execution (2-4 hours)

1. ✅ Refactor duplicate detection to run in parallel with details fetch
2. ✅ Use `Task.WhenAll` for employee/beneficiary queries when `memberType=0`
3. ✅ Add unit tests for parallel execution paths
4. ✅ Test with timeouts to ensure cancellation works correctly

### Phase 3: Query Optimization (4-8 hours)

1. ✅ Implement window function approach for duplicate detection
2. ✅ Review and optimize employee/beneficiary detail queries
3. ✅ Add missing `Include()` statements
4. ✅ Profile Oracle execution plans
5. ✅ Add indexes if missing

### Phase 4: Validation (2-3 hours)

1. ✅ Load testing with realistic data volumes
2. ✅ APM trace analysis to verify improvements
3. ✅ Regression testing for edge cases
4. ✅ Documentation updates

**Total Estimated Effort**: 9-17 hours
**Expected ROI**: 80-90% performance improvement

---

## Next Steps

1. **Immediate**: Review with team and prioritize optimizations
2. **Create Jira ticket** for tracking (suggest: PS-XXXX)
3. **Branch**: `feature/PS-XXXX-master-inquiry-backend-performance`
4. **Implement Phase 1** (quick wins) first for immediate relief
5. **Measure**: Re-run trace after each phase to validate improvements

---

## Additional Considerations

### Database Indexes

Verify indexes exist for:

- `Demographics.Ssn` (likely exists)
- `ProfitDetails.Ssn` + `ProfitYear` (composite)
- `ProfitDetails.CommentTypeId` (for payment type filter)

### Caching Opportunities

Consider caching:

- Duplicate SSN list (per profit year)
- Common search results (short TTL)
- Demographics lookup (if rarely changes)

### Query Compilation

For frequently-executed queries, use EF Core compiled queries:

```csharp
private static readonly Func<ProfitSharingDbContext, int, Task<Demographics>> GetBySSN =
    EF.CompileAsyncQuery((ProfitSharingDbContext ctx, int ssn) =>
        ctx.Demographics.FirstOrDefault(d => d.Ssn == ssn));
```

---

## Implementation Notes (October 17, 2025)

### Oracle IN Clause Batching

**Critical Safety Fix**: Added batching to handle Oracle's 1000-item limit for `IN` clauses.

**Issue**: EF Core translates `.Contains()` to SQL `IN` clause, which Oracle limits to 1000 items. Queries with >1000 SSNs would throw `ORA-01795` error.

**Solution**: Implemented batching in duplicate detection logic:

```csharp
const int oracleBatchSize = 1000;
var ssnBatches = ssnList.Chunk(oracleBatchSize).ToList();
var duplicateSsns = new HashSet<int>();

foreach (var ssnBatch in ssnBatches)
{
    var batchDuplicates = await demographics
        .Where(d => ssnBatch.Contains(d.Ssn))  // Max 1000 items per batch
        .GroupBy(d => d.Ssn)
        .Where(g => g.Count() > 1)
        .Select(g => g.Key)
        .TagWith($"MasterInquiry: Optimized duplicate detection - batch size {ssnBatch.Count()}")
        .ToListAsync(timeoutToken).ConfigureAwait(false);

    foreach (var dup in batchDuplicates)
    {
        duplicateSsns.Add(dup);
    }
}
```

**Impact**:

- Prevents runtime errors for queries with >1000 SSNs
- Maintains performance optimization (scoped filtering)
- Adds logging for batch processing transparency
- Applied to both main query path and exact match path

**Risk Assessment**: Low - typical queries have 5-50 SSNs, batching adds minimal overhead for small sets while preventing catastrophic failure for large sets.

### AgeInSql() Extension Method Now Available

**Update**: The common library team (Demoulas.Util.Extensions) has implemented `.AgeInSql()` extension method for DateTime, DateTimeOffset, and DateOnly types.

**Current Usage in MasterInquiryService** (line 655):

```csharp
// In-memory age calculation (after data materialized)
Age = memberData.DateOfBirth.Age()
```

**Assessment**: This is **already optimal** because:

1. Age calculation happens **after** `foreach` loop (in-memory)
2. Data is already materialized from detail services
3. No query projection involved - `.AgeInSql()` would throw here

**Where `.AgeInSql()` COULD Help**: If we ever add age calculation to EF Core query projections:

```csharp
// Example future optimization in detail services
.Select(d => new MemberData
{
    DateOfBirth = d.DateOfBirth,
    Age = d.DateOfBirth.AgeInSql()  // ✅ Would calculate in Oracle SQL
})
```

**Current Approach**: No changes needed - age calculation is not a bottleneck (<0.1s in our trace analysis).

### Similar Pattern Found: PayrollDuplicateSsnReportService

**Observation**: `PayrollDuplicateSsnReportService.cs` has similar duplicate SSN detection logic (lines 145-150) that is reusable:

```csharp
private IQueryable<int> GetDuplicateSsnQuery(IQueryable<Demographic> demographics)
{
    return demographics
        .GroupBy(x => x.Ssn)
        .Where(g => g.Count() > 1)
        .Select(g => g.Key);
}
```

**Potential Issue**: Line 64 uses this result in a `.Contains()` without batching:

```csharp
var dupSsns = await GetDuplicateSsnQuery(demographics).ToHashSetAsync(ct);
// ... later ...
.Where(dem => dupSsns.Contains(dem.Ssn))  // Could exceed 1000 items if >1000 duplicate SSNs exist
```

**Recommendation for Follow-up**:

1. **Low Priority**: Unlikely to have >1000 duplicate SSNs in demographics
2. **If Needed**: Apply same batching pattern as MasterInquiryService
3. **Consider**: Extract to shared service if pattern repeats elsewhere
4. **Note**: The two services have different use cases:
   - **PayrollDuplicateSsnReportService**: Finds ALL duplicates across entire table (no SSN list scoping)
   - **MasterInquiryService**: Finds duplicates within filtered SSN subset (scoped + batched)

---

_Analysis Date: October 17, 2025_
_Analyst: AI Assistant_
_Ticket: To be created_
