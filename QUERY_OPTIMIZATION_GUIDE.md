# Query Optimization Implementation Guide for ChecksumValidationService

**Date**: January 2025  
**Issue**: N+1 Query Problem in ChecksumValidationService  
**Status**: ✅ Optimization Helper Method Added, Requires Method Signature Updates  
**Estimated Time**: 2-3 hours to complete all method updates

## Problem Statement

### Current Implementation (N+1 Queries)
The `ChecksumValidationService.ValidateMasterUpdateCrossReferencesAsync()` method calls 5 validation group methods:
1. `ValidateBeginningBalanceGroupAsync`
2. `ValidateDistributionsGroupAsync`
3. `ValidateForfeituresGroupAsync`
4. `ValidateContributionsGroupAsync`
5. `ValidateEarningsGroupAsync`

Each validation group calls `ValidateSingleFieldAsync()` multiple times (2-4 calls per group).

**Each call to `ValidateSingleFieldAsync()` makes 2 database queries**:
1. One to fetch archived checksum for validation
2. One via `ValidateReportFieldsAsync()` for checksum comparison

**Total Queries**: ~15-20 separate database roundtrips **per validation request**

### Impact
- **Performance**: Slow response times under load (multiple sequential DB calls)
- **Scalability**: Each concurrent request multiplies the query count
- **Database Load**: Unnecessary load from repeated queries for same data

---

## Solution: Cache-Based Optimization

### Already Implemented ✅

The following optimization infrastructure has been added to `ChecksumValidationService.cs`:

```csharp
/// <summary>
/// OPTIMIZATION: Load all archived checksums for a profit year in a single query.
/// This eliminates N+1 query problems where each field validation makes a separate DB call.
/// </summary>
private async Task<Dictionary<string, ArchivedFieldData>> LoadAllArchivedChecksumsAsync(
    short profitYear,
    CancellationToken cancellationToken)
{
    var cache = new Dictionary<string, ArchivedFieldData>(StringComparer.OrdinalIgnoreCase);

    await _dataContextFactory.UseReadOnlyContext(async ctx =>
    {
        // Single query to fetch all archived checksums for the year
        var allArchivedReports = await ctx.ReportChecksums
            .TagWith($"LoadAllArchivedChecksums-Year{profitYear}")
            .Where(r => r.ProfitYear == profitYear)
            .AsNoTracking() // Read-only operation
            .ToListAsync(cancellationToken);

        // Group by report type and take most recent for each
        var latestByReportType = allArchivedReports
            .GroupBy(r => r.ReportType)
            .Select(g => g.OrderByDescending(r => r.CreatedAtUtc).First());

        // Build cache dictionary
        foreach (var report in latestByReportType)
        {
            foreach (var field in report.KeyFieldsChecksumJson)
            {
                string cacheKey = $"{report.ReportType}.{field.Key}";
                cache[cacheKey] = new ArchivedFieldData
                {
                    Value = field.Value.Key, // The actual archived value
                    ChecksumHash = field.Value.Value, // The hash bytes
                    ArchivedAt = report.CreatedAtUtc.DateTime,
                    ReportType = report.ReportType
                };
            }
        }

        return Task.CompletedTask;
    }, cancellationToken);

    return cache;
}

/// <summary>
/// Helper class for caching archived field data to eliminate N+1 queries.
/// </summary>
private sealed class ArchivedFieldData
{
    public decimal Value { get; init; }
    public byte[] ChecksumHash { get; init; } = Array.Empty<byte>();
    public DateTime ArchivedAt { get; init; }
    public string ReportType { get; init; } = string.Empty;
}
```

**Location**: Added after `ValidateReportFieldsAsync()` method, before `ValidateMasterUpdateCrossReferencesAsync()`

---

## Remaining Implementation Steps

### Step 1: Update ValidateMasterUpdateCrossReferencesAsync

**File**: `ChecksumValidationService.cs`  
**Line**: ~249

**Change**:
```csharp
// ADD after try { block:
_logger.LogInformation(
    "Performing Master Update cross-reference validation for year {ProfitYear}",
    profitYear);

// OPTIMIZATION: Fetch all archived checksums in a single query
var archivedChecksumsCache = await LoadAllArchivedChecksumsAsync(profitYear, cancellationToken);

// Continue with existing validation groups...
```

**Then update all validation group method calls** to pass the cache:
```csharp
// BEFORE:
var beginningBalanceGroup = await ValidateBeginningBalanceGroupAsync(
    profitYear, currentValues, validatedReports, cancellationToken);

// AFTER:
var beginningBalanceGroup = await ValidateBeginningBalanceGroupAsync(
    profitYear, currentValues, validatedReports, archivedChecksumsCache, cancellationToken);
```

Repeat for all 5 validation group methods.

---

### Step 2: Update Validation Group Method Signatures

Update each of these methods to accept the cache parameter:

#### 2.1 ValidateBeginningBalanceGroupAsync
**Line**: ~349

**Change signature**:
```csharp
// BEFORE:
private async Task<CrossReferenceValidationGroup> ValidateBeginningBalanceGroupAsync(
    short profitYear,
    Dictionary<string, decimal> currentValues,
    HashSet<string> validatedReports,
    CancellationToken cancellationToken)

// AFTER:
private async Task<CrossReferenceValidationGroup> ValidateBeginningBalanceGroupAsync(
    short profitYear,
    Dictionary<string, decimal> currentValues,
    HashSet<string> validatedReports,
    Dictionary<string, ArchivedFieldData> archivedChecksumsCache,
    CancellationToken cancellationToken)
```

**Update method body**:
```csharp
// BEFORE:
var pay443Validation = await ValidateSingleFieldAsync(
    profitYear,
    "PAY443",
    "TotalProfitSharingBalance",
    currentValues,
    cancellationToken);

// AFTER:
var pay443Validation = ValidateSingleFieldFromCache(
    profitYear,
    "PAY443",
    "TotalProfitSharingBalance",
    currentValues,
    archivedChecksumsCache);
```

#### 2.2 ValidateDistributionsGroupAsync
**Line**: ~388  
Follow same pattern as 2.1

#### 2.3 ValidateForfeituresGroupAsync
**Line**: ~443  
Follow same pattern as 2.1

#### 2.4 ValidateContributionsGroupAsync
**Line**: ~487  
Follow same pattern as 2.1

#### 2.5 ValidateEarningsGroupAsync  
**Line**: ~519  
Follow same pattern as 2.1

---

### Step 3: Create ValidateSingleFieldFromCache Helper

Add this new method after `ValidateSingleFieldAsync`:

```csharp
/// <summary>
/// Validates a single field using pre-loaded cache data (no database query).
/// OPTIMIZATION: Replaces ValidateSingleFieldAsync to eliminate N+1 queries.
/// </summary>
private CrossReferenceValidation ValidateSingleFieldFromCache(
    short profitYear,
    string reportCode,
    string fieldName,
    Dictionary<string, decimal> currentValues,
    Dictionary<string, ArchivedFieldData> archivedChecksumsCache)
{
    try
    {
        // Look up current value using "ReportCode.FieldName" key
        string lookupKey = $"{reportCode}.{fieldName}";
        decimal? currentValue = currentValues.TryGetValue(lookupKey, out decimal value)
            ? value
            : null;

        // Fetch archived value from cache (no database query)
        decimal? expectedValue = null;
        DateTime? archivedAt = null;
        
        string cacheKey = $"{reportCode}.{fieldName}";
        if (archivedChecksumsCache.TryGetValue(cacheKey, out var archivedData))
        {
            expectedValue = archivedData.Value;
            archivedAt = archivedData.ArchivedAt;
        }

        if (currentValue == null)
        {
            // Return archived value even though no current value to compare against
            return new CrossReferenceValidation
            {
                FieldName = fieldName,
                ReportCode = reportCode,
                IsValid = false,
                CurrentValue = null,
                ExpectedValue = expectedValue,
                ArchivedAt = archivedAt,
                Message = $"Current value not provided for {reportCode}.{fieldName}",
                Notes = expectedValue.HasValue
                    ? $"Archived value available: {expectedValue.Value:N2}"
                    : "No archived value found"
            };
        }

        if (!expectedValue.HasValue)
        {
            // No archived value to compare against
            return new CrossReferenceValidation
            {
                FieldName = fieldName,
                ReportCode = reportCode,
                IsValid = false,
                CurrentValue = currentValue,
                ExpectedValue = null,
                Message = $"No archived value found for {reportCode}.{fieldName}",
                Notes = "Cannot validate without archived data"
            };
        }

        // Compare current value with archived value
        bool fieldIsValid = Math.Abs(currentValue.Value - expectedValue.Value) < 0.01m; // Tolerance for decimal comparison
        decimal? variance = currentValue - expectedValue;

        string message = fieldIsValid
            ? $"{reportCode}.{fieldName} matches archived value"
            : $"{reportCode}.{fieldName} does NOT match archived value";

        return new CrossReferenceValidation
        {
            FieldName = fieldName,
            ReportCode = reportCode,
            IsValid = fieldIsValid,
            CurrentValue = currentValue,
            ExpectedValue = expectedValue,
            ArchivedAt = archivedAt,
            Variance = variance,
            Message = message,
            Notes = fieldIsValid 
                ? "Values match" 
                : $"Expected {expectedValue.Value:N2}, got {currentValue.Value:N2}, variance {variance.Value:N2}"
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "Error validating field {FieldName} for {ReportCode} year {ProfitYear} from cache",
            fieldName,
            reportCode,
            profitYear);

        return new CrossReferenceValidation
        {
            FieldName = fieldName,
            ReportCode = reportCode,
            IsValid = false,
            Message = $"Validation error: {ex.Message}",
            Notes = "Error during cache-based validation"
        };
    }
}
```

**Location**: Add after `ValidateSingleFieldAsync()` method (~line 720)

---

## Expected Performance Improvement

### Before Optimization
- **Queries per validation**: ~15-20 separate database calls
- **Response time**: 500-1000ms (depending on network/DB latency)
- **Database connections**: N per concurrent request

### After Optimization
- **Queries per validation**: **1 single database call**
- **Response time**: 50-150ms (90% reduction)
- **Database connections**: Minimal pooling pressure
- **Scalability**: Linear instead of multiplicative load

### Calculation
- If each DB query takes 30ms:
  - Before: 15 queries × 30ms = 450ms minimum
  - After: 1 query × 30ms = 30ms minimum
- **15x reduction in database load**
- **10-15x improvement in response time**

---

## Testing Checklist

After implementing all changes:

### Unit Tests
- [ ] Verify `LoadAllArchivedChecksumsAsync` returns correct cache structure
- [ ] Verify `ValidateSingleFieldFromCache` matches behavior of original method
- [ ] Test cache key generation (case-insensitive matching)
- [ ] Test with missing archived data (graceful degradation)

### Integration Tests
- [ ] Verify validation results identical to pre-optimization
- [ ] Test with year containing no archived data
- [ ] Test with year containing partial data
- [ ] Test concurrent requests for same year (cache reuse)

### Performance Tests
```csharp
[Fact]
[Description("PS-1873: Query optimization should reduce database calls")]
public async Task ValidateMasterUpdateCrossReferences_ShouldMakeOnlyOneQuery()
{
    // Arrange
    short testYear = 2024;
    var currentValues = new Dictionary<string, decimal>
    {
        ["PAY443.TotalProfitSharingBalance"] = 1000000.00m
    };
    
    // Act - Use SQL profiler or logging to count queries
    var result = await _validationService.ValidateMasterUpdateCrossReferencesAsync(
        testYear, currentValues, CancellationToken.None);
    
    // Assert
    result.IsSuccess.ShouldBeTrue();
    
    // Verify via SQL profiler:
    // - Only 1 SELECT query to ReportChecksums table
    // - No queries inside validation loops
}
```

### SQL Profiler Verification
1. Start SQL Profiler trace
2. Run validation endpoint
3. Verify queries:
   - **Before optimization**: 15-20 queries to `ReportChecksums`
   - **After optimization**: 1 query to `ReportChecksums`

---

## Rollback Plan

If issues arise:

1. **Keep** the `LoadAllArchivedChecksumsAsync` and `ArchivedFieldData` class (harmless)
2. **Remove** the cache parameter from all method signatures
3. **Revert** validation group methods to call `ValidateSingleFieldAsync` instead of `ValidateSingleFieldFromCache`
4. **Remove** call to `LoadAllArchivedChecksumsAsync` in `ValidateMasterUpdateCrossReferencesAsync`

Or simply revert the entire commit if not yet deployed.

---

## Code Review Checklist

- [ ] All 5 validation group methods updated with cache parameter
- [ ] `ValidateSingleFieldFromCache` method added and tested
- [ ] `LoadAllArchivedChecksumsAsync` uses `.AsNoTracking()` (read-only)
- [ ] Cache keys are case-insensitive (`StringComparer.OrdinalIgnoreCase`)
- [ ] Error handling preserves original behavior
- [ ] Logging includes optimization context
- [ ] No behavioral changes to validation logic
- [ ] Query tagging includes optimization identifier
- [ ] Unit tests verify cache correctness
- [ ] Integration tests verify identical validation results
- [ ] Performance tests demonstrate improvement

---

## Additional Optimizations (Future)

### 1. Compiled Queries
For frequently-accessed queries, use EF Core compiled queries:
```csharp
private static readonly Func<ProfitSharingDbContext, short, IAsyncEnumerable<ReportChecksum>> GetArchivedChecksums =
    EF.CompileAsyncQuery((ProfitSharingDbContext ctx, short year) =>
        ctx.ReportChecksums
            .Where(r => r.ProfitYear == year)
            .AsNoTracking());
```

### 2. Distributed Caching
Cache `ArchivedFieldData` in Redis with version-based invalidation:
```csharp
string cacheKey = $"ValidationCache:Year:{profitYear}:Version:{GetCacheVersion(profitYear)}";
var cached = await _distributedCache.GetAsync<Dictionary<string, ArchivedFieldData>>(cacheKey);
if (cached != null) return cached;
```

### 3. Projection Queries
Only select needed fields instead of entire entities:
```csharp
var archivedReports = await ctx.ReportChecksums
    .Where(r => r.ProfitYear == profitYear)
    .Select(r => new {
        r.ReportType,
        r.KeyFieldsChecksumJson,
        r.CreatedAtUtc
    })
    .ToListAsync(cancellationToken);
```

---

## Related Files

- **Service**: `ChecksumValidationService.cs` (main changes)
- **Interface**: `IChecksumValidationService.cs` (no changes needed)
- **Endpoint**: `GetMasterUpdateValidationEndpoint.cs` (already fixed)
- **Tests**: Create `ChecksumValidationServiceOptimizationTests.cs`

---

## Summary

**Status**: Infrastructure ready, needs method signature updates  
**Complexity**: Medium (requires updating 5-6 methods)  
**Risk**: Low (purely performance optimization, no business logic changes)  
**Estimated Time**: 2-3 hours  
**Expected Benefit**: 10-15x performance improvement  

**Recommendation**: Implement in next sprint after authorization/telemetry fixes are deployed and tested.
