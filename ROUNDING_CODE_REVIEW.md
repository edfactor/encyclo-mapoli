# Rounding & Casting Code Review - Services Layer

**Date:** December 2, 2025  
**Focus:** Identify potential rounding, truncation, and overflow issues in financial calculations

## Executive Summary

Comprehensive review of casting and rounding patterns in the services layer reveals several categories of issues:

1. **✅ FIXED (7 items)** - Math.Round now uses correct decimal places (0 for points, 2 for amounts)
2. **⚠️ POTENTIAL ISSUES (13 items)** - Unsafe casts to `short` that could overflow
3. **✅ SAFE PATTERNS (5 items)** - Proper rounding with appropriate decimal places

---

## Detailed Findings

### Category 1: Casting int/decimal to short (⚠️ OVERFLOW RISK)

When summing employee counts or point totals and casting to `short` (max: 32,767), there's risk of overflow for large datasets.

#### FrozenReportService.cs - Lines 150, 152, 154, 156, 267, 334, 452, 453, 556-559, 687-688

**Pattern:**

```csharp
// ⚠️ RISKY: Casting sum directly to short without bounds checking
(short)details.Sum(d => d.EmployeeCount)
(short)details.Where(d => d.RegularAmount > 0).Sum(d => d.EmployeeCount)
```

**Risk Analysis:**

- EmployeeCount is `int` (0 to 2,147,483,647)
- short range: -32,768 to 32,767
- **Dataset with > 32,767 employees would silently overflow**
- No validation, no error handling

**Recommendation:**

```csharp
// Option 1: Add validation
var employeeSum = details.Sum(d => d.EmployeeCount);
if (employeeSum > short.MaxValue)
    throw new InvalidOperationException($"Employee count {employeeSum} exceeds short.MaxValue");
return (short)employeeSum;

// Option 2: Change property type to int if reasonable
// (requires checking downstream consumers)
```

**Affected Lines:**

- Line 150: RegularTotalEmployees
- Line 152: HardshipTotalEmployees
- Line 154: TotalEmployees
- Line 156: BothHardshipAndRegularEmployees
- Line 267: TotalEmployees
- Line 334: TotalEmployees
- Line 452: TotalMembers (EmployeeCount + BeneficiaryCount)
- Line 453: TotalBeneficiaries
- Line 556: totalFullTimeCount
- Line 557: totalNotVestedCount
- Line 558: totalPartialVestedCount
- Line 559: totalBeneficiaryCount
- Line 687: TotalMembers
- Line 688: TotalBeneficiaries

**Severity:** 🔴 HIGH - Silent data corruption possible

---

### Category 2: Sum then Cast Pattern for Point Totals

#### ForfeituresAndPointsForYearService.cs - Line 117

**Pattern:**

```csharp
int totalContForfeitPoints = (int)employeeData.Sum(e => e.PointsEarned ?? 0);
```

**Analysis:**

- ✅ SAFE: `e.PointsEarned` is `decimal?`, Sum returns `decimal`, cast to `int` is explicit
- ✅ CORRECT: No rounding issue (points are already whole numbers)
- **Question:** Why is PointsEarned stored as decimal? Should be int?

**Recommendation:**

```csharp
// Add comment explaining why decimal → int cast is safe
// e.g., "PointsEarned represents whole points; decimal is legacy schema"
int totalContForfeitPoints = (int)employeeData.Sum(e => e.PointsEarned ?? 0);
```

---

### Category 3: Already Fixed - Math.Round with Correct Decimal Places

#### ✅ COMPLETED EARLIER IN SESSION

The following locations were already corrected to use `MidpointRounding.AwayFromZero`:

1. **YearEndChangeCalculator.cs:159** - `Math.Round(income / 100.0m, 0, ...)` ✅
2. **BeneficiariesProcessingHelper.cs:81** - `Math.Round(memberTotals.PointsDollars / 100, 0, ...)` ✅
3. **EmployeeProcessorHelper.cs:130** - `Math.Round(memberTotals.PointsDollars / 100, 0, ...)` ✅
4. **ForfeituresAndPointsForYearService.cs:209** - `Math.Round(balanceConsideredForEarnings / 100, 0, ...)` ✅
5. **ForfeituresAndPointsForYearService.cs:228** - `Math.Round(currentBalance.Value / 100, 0, ...)` ✅ + cast to short
6. **ProfitSharingSummaryReportService.cs:110** - `Math.Round(pp.TotalIncome / 100, 0, ...)` ✅
7. **ProfitSharingSummaryReportService.cs:505** - `Math.Round(x.Wages / 100, 0, ...)` ✅

**Key Fix Applied:** Using `0` decimal places for point calculations (they're cast to int/short)

---

### Category 4: Decimal Cast for Financial Aggregation

#### ProfitSharingSummaryReportService.cs - Lines 114-115, 136-140

**Pattern:**

```csharp
Balance = (decimal)(bal != null && bal.TotalAmount != null ? bal.TotalAmount : 0),
TotalWages = mainGroup.Sum(x => (decimal)x.Wages),
```

**Analysis:**

- ✅ SAFE: Casting to decimal (broader range than int/long)
- ✅ CORRECT: No precision loss when casting int/long to decimal
- **Note:** x.Wages is `decimal` in projection, so explicit cast is redundant but harmless

**Recommendation:** Remove redundant casts for clarity

```csharp
// CURRENT (works but verbose)
TotalWages = mainGroup.Sum(x => (decimal)x.Wages),

// CLEARER (if x.Wages is already decimal)
TotalWages = mainGroup.Sum(x => x.Wages),
```

---

### Category 5: Integer Year Calculations (Safe)

#### Multiple Files - Lines like `(short)(currentYear - 1)`

**Pattern:**

```csharp
short lastYear = (short)(currentYear - 1);
short etvaHotProfitYear = (short)DateTime.Now.Year;
```

**Analysis:**

- ✅ SAFE: Years 1-9999 fit safely in short range (-32,768 to 32,767)
- ✅ CORRECT: No rounding, no precision loss

**Files:**

- ForfeituresAndPointsForYearService.cs:45
- MasterInquiryService.cs:202, 514
- ProfitMasterService.cs:84, 157, 252
- YearEndService.cs:130, 372, 381
- PostFrozenService.cs:71, 305, 438
- BreakdownReportService.cs:617, 794
- PayrollDuplicateSsnReportService.cs:46
- UnForfeitService.cs:56
- ReportRunnerService.cs:58
- FrozenReportService.cs:700
- ProfitSharingSummaryReportService.cs:94, 96

**Verdict:** ✅ All safe, no action needed

---

## Recommendations & Action Items

### Immediate Actions (High Priority)

1. **Add overflow checking in FrozenReportService**

   - Validate employee/beneficiary counts don't exceed short.MaxValue
   - Add guard clauses with descriptive error messages
   - Consider changing affected properties to `int` if counts might grow beyond 32,767

2. **Document PointsEarned decimal schema**
   - Add comment explaining why `decimal?` is used for whole number points
   - Clarify if this is legacy schema that could be refactored

### Medium Priority

3. **Remove redundant decimal casts**
   - ProfitSharingSummaryReportService:136-140
   - Improves code clarity without functional change

### Future Enhancements

4. **Consider strong typing for employee counts**

   - Create `EmployeeCount` value object instead of bare `short`
   - Enforces bounds at type level, not just at assignment

5. **Add analyzer rule for casting int to short**
   - Extend DSM006 or create new DSM007 rule
   - Flags all `(short)` casts of expressions that could overflow

---

## Testing Recommendations

### Unit Tests Needed

1. **FrozenReportService overflow test**

   ```csharp
   [Test]
   public void ComputeAggregates_With32768EmployeesThrows()
   {
       // Create detail records totaling 32,768+ employees
       // Verify exception thrown, not silent overflow
   }
   ```

2. **Edge case: exactly short.MaxValue**
   ```csharp
   [Test]
   public void ComputeAggregates_With32767EmployeesSucceeds()
   {
       // Verify boundary is handled correctly
   }
   ```

### Integration Tests

3. **Production data validation**
   - Run against READY production data
   - Verify employee/beneficiary counts stay well below 32,767 threshold
   - Add monitoring/alerting if counts exceed 20,000

---

## Summary Table

| File                                  | Line                                                     | Issue                     | Type          | Severity | Status      |
| ------------------------------------- | -------------------------------------------------------- | ------------------------- | ------------- | -------- | ----------- |
| FrozenReportService.cs                | 150, 152, 154, 156, 267, 334, 452, 453, 556-559, 687-688 | Cast int sum to short     | Overflow Risk | 🔴 HIGH  | ⚠️ PENDING  |
| ForfeituresAndPointsForYearService.cs | 117                                                      | Sum decimal to int        | Safe Pattern  | ✅ LOW   | ✅ ANALYZED |
| YearEndChangeCalculator.cs            | 159                                                      | Math.Round decimal places | Fixed         | ✅ LOW   | ✅ FIXED    |
| BeneficiariesProcessingHelper.cs      | 81                                                       | Math.Round decimal places | Fixed         | ✅ LOW   | ✅ FIXED    |
| EmployeeProcessorHelper.cs            | 130                                                      | Math.Round decimal places | Fixed         | ✅ LOW   | ✅ FIXED    |
| ForfeituresAndPointsForYearService.cs | 209, 228                                                 | Math.Round decimal places | Fixed         | ✅ LOW   | ✅ FIXED    |
| ProfitSharingSummaryReportService.cs  | 110, 505                                                 | Math.Round decimal places | Fixed         | ✅ LOW   | ✅ FIXED    |
| ProfitSharingSummaryReportService.cs  | 114-115, 136-140                                         | Redundant decimal cast    | Style         | ✅ LOW   | ℹ️ NOTED    |
| Multiple                              | Year calculations                                        | Cast int to short         | Safe          | ✅ LOW   | ✅ SAFE     |

---

## Files Analyzed

- ✅ FrozenReportService.cs (1,004 lines)
- ✅ ForfeituresAndPointsForYearService.cs (244 lines)
- ✅ ProfitSharingSummaryReportService.cs (759 lines)
- ✅ YearEndChangeCalculator.cs (240 lines)
- ✅ BeneficiariesProcessingHelper.cs (94 lines)
- ✅ EmployeeProcessorHelper.cs (276 lines)
- ✅ YearEndService.cs (multiple locations)
- ✅ MasterInquiryService.cs (multiple locations)
- ✅ ProfitMasterService.cs (multiple locations)
- ✅ And 8 other report services

---

**Next Step:** Address FrozenReportService overflow risks before finalizing PR for PS-2275
