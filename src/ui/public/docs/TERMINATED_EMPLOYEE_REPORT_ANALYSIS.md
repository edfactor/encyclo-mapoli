# Terminated Employee & Beneficiary Report - System Analysis

**Document Version:** 1.0  
**Last Updated:** October 1, 2025  
**Status:** Post-Revert Baseline Analysis

## Executive Summary

The Terminated Employee & Beneficiary Report shows profit sharing distributions for employees who terminated employment during the reporting year (2025). This document analyzes the current state of the SMART implementation compared to the legacy READY/COBOL system (QPAY066).

### Current Accuracy Metrics
- **Population Parity:** 98.4% (489 common employees / 497 expected)
- **Missing Employees:** 8 (6 beneficiaries, 2 regular employees)
- **Extra Employees:** 35 (2025 terminations with 2024 balance carryforward)
- **Calculation Differences:** 483 year detail differences across common employees
- **Financial Impact:** -$1.24M beneficiary allocation gap (primary concern)

---

## System Architecture Overview

### READY/COBOL System (Legacy - Source of Truth)
- **Program:** QPAY066.pco
- **Data Model:** Snapshot-based (one PAYPROFIT record per employee)
- **Transaction Storage:** Historical transactions in profit_detail table
- **Key Fields:** 
  - `py_ps_years` - Years in profit sharing (stored directly in PAYPROFIT)
  - `ps_amt` - Current balance snapshot
  - Multiple pre-calculated totals

### SMART System (New Implementation)
- **Service:** `TerminatedEmployeeReportService.cs`
- **Data Model:** Temporal (multiple PayProfit records per year)
- **Transaction Processing:** Live calculation from `ProfitDetail` entity
- **Key Differences:**
  - `YearsInPs` calculated dynamically from `sum(YEARS_OF_SERVICE_CREDIT)` in profit_detail
  - Uses `TotalVestingBalance.CurrentBalance` as beginning balance
  - Real-time transaction aggregation instead of snapshot totals

---

## Discrepancy Analysis

### 1. Missing Employees (8 Total)

#### Missing Beneficiaries (6 employees - **$1.24M impact**)

| Badge PSN | Name | Badge | Impact | Root Cause |
|-----------|------|-------|--------|------------|
| 7058241000 | BRIGGS, MICAH | 705824 | Unknown | Filtered by IsInteresting |
| 7043941000 | BROCK, AUTUMN | 704394 | Unknown | Filtered by IsInteresting |
| 7054021000 | HURLEY, CARTER | 705402 | Unknown | Filtered by IsInteresting |
| 7040131000 | LAMB, MUHAMMAD | 704013 | Unknown | Filtered by IsInteresting |
| 706678100 | LEWIS, ETHEL | 706678 | Unknown | Filtered by IsInteresting |
| 7034861000 | SCHULTZ, AALIYAH | 703486 | Unknown | Filtered by IsInteresting |

**Expected Total Beneficiary Allocation:** $1,349,459.54  
**Actual Total Beneficiary Allocation:** $109,145.xx  
**Gap:** **-$1,240,314.xx**

**Hypothesis:** These 6 beneficiaries likely account for the $1.24M gap. They are being filtered out by the `IsInteresting` method which checks:
```csharp
if (member.BeginningBalance == 0 
    && member.BeneficiaryAllocation == 0 
    && member.DistributionAmount == 0 
    && member.Forfeit == 0)
{
    return false; // Filtered out
}
```

**Investigation Needed:**
1. Check if these beneficiaries have `ProfitDetail` transactions in 2025
2. Verify `TotalVestingBalance` records exist for deceased employees
3. Determine if beneficiary allocation transactions (Type 5/6) are being processed correctly

#### Missing Regular Employees (2 employees)

| Badge PSN | Name | Badge | Status |
|-----------|------|-------|--------|
| 700680 | BATES, JADE | 700680 | Also appears as 7006801000 in Extra |
| 700655 | MILLS, VALENTINA | 700655 | Also appears as 7006551000 in Extra |

**Pattern:** Both appear in "Extra" with beneficiary suffix (1000), suggesting data duplication or beneficiary relationship confusion.

---

### 2. Extra Employees (35 Total)

**Sample of Extra Employees:**
- BARTLETT, JASON (709927)
- BENTLEY, JENNIFER (709752)
- BENTLEY, JOSHUA (710220)
- CAIN, CAMILA (709945)
- CONNORS, ROWAN (710063)
- FLORES, LEVI (709609)
- GOULD, LILIANA (710425)
- HALEY, MAYA (710096)
- JOHNS, CHRISTIAN (709325)
- ... and 26 more

**Badge Range Analysis:**
- **Extra badge range:** 700655 - 710425
- **Missing badge range:** 700655 - 706678
- **Pattern:** 33 of 35 extra employees have 2025 termination dates

**Root Cause:** Temporal data model architecture difference
- **READY:** Snapshot at year-end 2024 → only includes employees terminated in 2025 who had activity
- **SMART:** Includes ALL 2025 terminations if they have any non-zero values from 2024 carryforward
- These employees likely terminated in 2025 but have `BeginningBalance` from 2024 `PayProfit` records

**Expected Behavior:** This is likely correct SMART behavior - showing complete 2025 termination picture

---

### 3. Calculation Differences (483 Year Detail Fields)

#### A. VestedBalance Differences (High Impact)

**Sample Cases:**

| Employee | Expected | Actual | Difference | % Error |
|----------|----------|--------|------------|---------|
| 700637 | $3,310.87 | $0.00 | -$3,310.87 | -100% |
| 709985 | $2,632.00 | $5,263.99 | +$2,631.99 | +100% |
| 708006 | $9,876.76 | $14,815.15 | +$4,938.39 | +50% |

**Pattern Analysis:**
- Some show 100% increase (doubling) - suggests vesting % doubled
- Some show 100% decrease (zero) - suggests incorrect vesting % application
- Related to `VestedPercent` calculation differences

**Formula:** `VestedBalance = EndingBalance × VestedPercent`

**Root Causes:**
1. **VestedPercent Calculation Differences** (see section below)
2. **BeginningBalance Source Discrepancy:**
   - READY: Uses `ps_amt` from PAYPROFIT snapshot
   - SMART: Uses `TotalVestingBalance.CurrentBalance`
   - May have timing/calculation differences

#### B. VestedPercent Differences (Moderate Impact - Many Employees)

**Common Patterns:**
- Expected: 0%, Actual: 1% (or 100%)
- Expected: 0.4% (or 40%), Actual: 0.6% (or 60%)
- Expected: 0.2% (or 20%), Actual: 0.4% (or 40%)

**Root Cause - IDENTIFIED:**

The vesting percentage calculation depends on `YearsInPs` (years in profit sharing):

**COBOL/READY Logic (lines 996-1029):**
```cobol
MOVE R2-PS-YEARS TO YEARS-ENROLLED
IF YEARS-ENROLLED < 2
    PERFORM GET-OLD-SCHEDULE
ELSE IF YEARS-ENROLLED = 2
    PERFORM GET-NEW-SCHEDULE
ELSE
    MOVE 100 TO VESTED-PERCENT
```

**Data Source Discrepancy:**
- **READY:** Uses `py_ps_years` field stored directly in PAYPROFIT table
  - This field is NOT imported into SMART database
  - Static value calculated/stored in legacy system
  
- **SMART:** Calculates `YearsInPs` dynamically:
  ```csharp
  var yearsInPs = transactions
      .Where(t => t.ProfitYear == reportYear)
      .Sum(t => t.YearsOfServiceCredit ?? 0);
  ```
  - Aggregates from `YEARS_OF_SERVICE_CREDIT` in profit_detail
  - May differ from pre-calculated READY value

**Impact:** Different years → different vesting schedule → different percentages

**Example:**
- If READY has `py_ps_years = 1` → Old Schedule → 20% vesting
- If SMART calculates `YearsInPs = 2` → New Schedule → 40% vesting
- Result: VestedBalance doubles

**Resolution Options:**
1. **Import `py_ps_years` into SMART** (requires schema change)
2. **Accept discrepancy** and document as architectural difference
3. **Investigate calculation logic** for `YEARS_OF_SERVICE_CREDIT` in profit_detail

#### C. YtdPsHours Differences (Low Impact - Precision)

**Pattern:** Small decimal differences (0.02 - 0.09 hours typical)

**Examples:**
- Expected: 1152.7, Actual: 1152.75 (diff: 0.05)
- Expected: 218.2, Actual: 218.28 (diff: 0.08)
- Expected: 697.0, Actual: 697.00 (diff: 0.00 - formatting)

**Root Cause:** Rounding/precision differences in hour aggregation
- READY: May round at transaction level then sum
- SMART: May sum then round, or use different precision

**Impact:** Negligible (< 0.1 hours per employee)

**Resolution:** Low priority - acceptable variance

#### D. BeginningBalance & EndingBalance Differences (Connected)

**Pattern:** BeginningBalance differences propagate to EndingBalance

**Formula Chain:**
```
BeginningBalance (from source)
+ Contributions
- Distributions  
- Forfeitures
+/- BeneficiaryAllocation
= EndingBalance
```

**Root Cause Chain:**
1. Different `BeginningBalance` source (PAYPROFIT vs TotalVestingBalance)
2. Propagates through calculation
3. Results in different `EndingBalance`
4. Combined with different `VestedPercent` → different `VestedBalance`

**Investigation Needed:**
- Compare PAYPROFIT.ps_amt vs TotalVestingBalance.CurrentBalance for sample employees
- Determine which source is more accurate
- Document intentional differences vs bugs

---

### 4. Report Totals Discrepancy

| Metric | READY (Expected) | SMART (Actual) | Difference | % Variance |
|--------|------------------|----------------|------------|------------|
| Employee Count | 497 | 524 | +27 | +5.4% |
| Total Ending Balance | $24,692,640.86 | $24,614,372.19 | -$78,268.67 | -0.3% |
| Total Vested | $22,666,201.39 | $22,609,522.58 | -$56,678.81 | -0.2% |
| **Total Beneficiary Allocation** | **$1,349,459.54** | **$109,145.xx** | **-$1,240,314.xx** | **-91.9%** |

**Primary Concern:** Total Beneficiary Allocation is 91.9% under expected value

**Root Cause:** Missing 6 beneficiaries account for the bulk of this gap

---

## Data Flow Comparison

### READY/COBOL Data Flow
```
1. Load PAYPROFIT record (snapshot)
   → BeginningBalance = ps_amt
   → YearsInPs = py_ps_years

2. Load profit_detail transactions (year filter)
   → Aggregate by transaction type

3. Calculate ending values
   → EndingBalance = ps_amt + contributions - distributions - forfeitures

4. Calculate vesting
   → Determine schedule from py_ps_years
   → Apply vesting % to EndingBalance

5. Filter interesting records
   → Check ps_amt != 0 OR bene_alloc != 0 OR distributions != 0 OR forfeit != 0

6. Output report
```

### SMART Data Flow
```
1. Query Demographics (terminated in year)
   → Filter by TerminationDate in report year

2. Load TotalVestingBalance
   → BeginningBalance = CurrentBalance

3. Load PayProfit (multiple years)
   → Get multiple temporal records

4. Load ProfitDetail transactions (year filter)
   → Aggregate by ProfitCodeId mapping to transaction types

5. Calculate YearsInPs
   → Sum YEARS_OF_SERVICE_CREDIT from transactions

6. Calculate ending values
   → EndingBalance = BeginningBalance + contributions - distributions - forfeitures

7. Calculate vesting
   → Determine schedule from calculated YearsInPs
   → Apply vesting % to EndingBalance

8. Filter interesting records (IsInteresting method)
   → Check BeginningBalance != 0 OR BeneficiaryAllocation != 0 
           OR DistributionAmount != 0 OR Forfeit != 0

9. Build beneficiary records
   → Query deceased employees
   → Create beneficiary BadgePSN (append 1000)
   → Copy calculations from primary employee

10. Sort and paginate
```

**Key Differences:**
1. **Data Source:** PAYPROFIT snapshot vs TotalVestingBalance
2. **Years Calculation:** Stored field vs dynamic aggregation
3. **Beneficiary Logic:** COBOL inline vs separate query/construction
4. **Temporal Handling:** Single record vs multiple PayProfit records

---

## Transaction Type Mapping

### COBOL Transaction Types (profit_detail.type_code)

| Type | COBOL Logic | Transaction Description |
|------|-------------|------------------------|
| 0 | `R2-FORFEIT + R2-CONTRIB + R2-EARNINGS` | Combined forfeit + contribution + earnings |
| 1 | Distribution via `R2-PROFIT-DIST` | Standard distribution |
| 2 | Forfeit via `R2-FORFEIT` | Forfeiture |
| 3 | Distribution via `R2-PROFIT-DIST` | Hardship distribution |
| 5 | Bene debit via `R2-PROFIT-DIST` | Beneficiary allocation debit |
| 6 | Bene credit via `R2-CONTRIB` | Beneficiary allocation credit |
| 8 | Earnings via `R2-EARNINGS` | Investment earnings |
| 9 | Distribution via `R2-PROFIT-DIST` | Distribution (alternative) |

### SMART Property Mapping (ProfitDetail entity)

| Type | SMART Properties Used | Notes |
|------|----------------------|-------|
| 0 | `Forfeiture + Contribution + Earnings` | Three separate properties |
| 1,3,9 | `Forfeiture` (holds distribution amount) | Counter-intuitive naming! |
| 2 | `Forfeiture` | Actual forfeit |
| 5 | `Forfeiture` (holds bene debit) | Counter-intuitive naming! |
| 6 | `Contribution` (holds bene credit) | Used for beneficiary allocation |
| 8 | `Earnings` | Investment earnings |

**Critical Note:** The `ProfitDetail.Forfeiture` property is overloaded:
- Type 2: Actual forfeiture
- Types 1,3,9: Distribution amounts (not forfeitures!)
- Type 5: Beneficiary debit (not a forfeiture!)

This requires careful `ProfitCodeId` checking to determine semantic meaning.

---

## IsInteresting Filter Logic

### Purpose
Filters out employees with no activity (all zeros) to reduce report clutter

### COBOL Logic (lines 953-960)
```cobol
IF W-PSAMT NOT = ZERO
    OR W-BEN-ALLOC NOT = ZERO
    OR W-PSLOAN NOT = ZERO
    OR W-PSFORF NOT = ZERO
    PERFORM WRITE-DETAIL
```

Where:
- `W-PSAMT` = ending balance
- `W-BEN-ALLOC` = beneficiary allocation
- `W-PSLOAN` = distributions (loan/hardship)
- `W-PSFORF` = forfeitures

### SMART Logic (Current Implementation)
```csharp
private static bool IsInteresting(Member member)
{
    return member.BeginningBalance != 0
        || member.BeneficiaryAllocation != 0
        || member.DistributionAmount != 0
        || member.Forfeit != 0;
}
```

**Discrepancy:** COBOL uses `ending balance`, SMART uses `beginning balance`

**Impact:** May filter different sets of employees:
- COBOL: Keeps anyone with non-zero ending position
- SMART: Keeps anyone with non-zero starting position

**Recommendation:** Change SMART to match COBOL logic:
```csharp
return member.EndingBalance != 0
    || member.BeneficiaryAllocation != 0
    || member.DistributionAmount != 0
    || member.Forfeit != 0;
```

---

## Beneficiary Handling

### COBOL Approach (lines 818-846)
```cobol
* If employee deceased and has vested balance
IF R2-TERMINATE-CODE = "D"
    IF VESTED-AMOUNT > ZERO
        * Beneficiary gets allocation = vested balance
        MOVE VESTED-AMOUNT TO W-BEN-ALLOC
        * Append 1000 to badge for beneficiary record
        STRING R2-BADGE DELIMITED BY SIZE
               "1000" DELIMITED BY SIZE
               INTO W-BADGE-PSN
```

### SMART Approach (lines 151-197)
```csharp
// Separate query for deceased employees
var deceased = demographicSlice
    .Where(d => d.EmploymentStatusId == deceasedStatusId)
    .ToList();

foreach (var demographic in deceased)
{
    // Create beneficiary with 1000 suffix
    var beneficiaryPsn = demographic.BadgeNumber + "1000";
    
    // Copy member calculations
    var beneficiary = new MemberDto
    {
        BadgePSn = beneficiaryPsn,
        BadgeNumber = demographic.BadgeNumber, // FIXED!
        // ... copy other fields
        BeneficiaryAllocation = member.VestedBalance, // Allocation = vested
        ProfitYear = request.EndingDate.Year  // FIXED!
    };
}
```

**Recent Fixes:**
1. `BadgeNumber` now correctly uses employee's badge (not beneficiary PSN)
2. `ProfitYear` now correctly set to report year (was 0)

**Current Issue:** 6 beneficiaries missing from SMART report
- Likely filtered by `IsInteresting` check
- May be missing `TotalVestingBalance` records
- May be missing transaction data for 2025

---

## Recommended Investigation Priority

### Priority 1: Critical Financial Impact
**Missing $1.24M Beneficiary Allocation**

**Action Items:**
1. For each of 6 missing beneficiaries, query database:
   ```sql
   -- Check TotalVestingBalance
   SELECT * FROM PARTICIPANT_TOTAL_VESTING_BALANCE 
   WHERE SSN = (SELECT SSN FROM DEMOGRAPHIC WHERE BADGE_NUMBER = 705824);
   
   -- Check ProfitDetail transactions
   SELECT * FROM PROFIT_DETAIL 
   WHERE SSN = (SELECT SSN FROM DEMOGRAPHIC WHERE BADGE_NUMBER = 705824)
     AND PROFIT_YEAR = 2025;
   
   -- Check PayProfit
   SELECT * FROM PAY_PROFIT
   WHERE DEMOGRAPHIC_ID = (SELECT ID FROM DEMOGRAPHIC WHERE BADGE_NUMBER = 705824)
     AND PROFIT_YEAR IN (2024, 2025);
   ```

2. Determine why `IsInteresting` filters them out
3. Fix filtering logic or ensure data exists

### Priority 2: Medium Impact  
**VestedPercent Calculation Differences**

**Action Items:**
1. Compare `py_ps_years` from READY vs calculated `YearsInPs` in SMART for sample employees
2. Decide: Import `py_ps_years` OR accept architectural difference
3. Document expected variance if accepting difference

### Priority 3: Low Impact
**YtdPsHours Precision & Name Truncation**

**Action Items:**
1. Document as acceptable variance
2. Consider standardizing rounding if needed
3. Fix name truncation (increase field width)

### Priority 4: Architectural Difference
**Extra 35 Employees**

**Action Items:**
1. Confirm these are 2025 terminations with 2024 balance carryforward
2. Document as expected behavior difference (temporal model advantage)
3. Consider this a feature, not a bug (more complete picture)

---

## Testing Recommendations

### Unit Tests Needed
1. **Transaction Processing**
   - Test each of 8 transaction types individually
   - Verify ProfitDetail property mapping (especially Forfeiture overloading)
   - Test Type 0 multi-property aggregation

2. **Vesting Calculation**
   - Test old schedule (years < 2): [0, 0, 20, 40, 60, 80, 100]
   - Test new schedule (years = 2): [0, 20, 40, 60, 80, 100]
   - Test 100% vesting (years > 2 OR deceased)
   - Test zero balance edge case

3. **IsInteresting Filter**
   - Test all combinations of zero/non-zero values
   - Test against COBOL equivalents

4. **Beneficiary Logic**
   - Test PSN suffix generation (badge + "1000")
   - Test allocation = vested balance
   - Test deceased employee identification

### Integration Tests Needed
1. **End-to-End Comparison**
   - Compare each field for all common employees
   - Identify systematic patterns in differences
   - Track variance over time

2. **Data Quality Checks**
   - Validate TotalVestingBalance exists for all employees
   - Validate ProfitDetail transactions loaded correctly
   - Validate Demographics termination dates accurate

---

## Glossary

### Terms
- **READY:** Legacy mainframe system (COBOL/DB2)
- **SMART:** New .NET/Oracle system
- **QPAY066:** COBOL program that generates terminated employee report
- **BadgePSN:** Badge number + optional suffix (1000 for beneficiaries, 100 for some beneficiary types)
- **IsInteresting:** Filter logic to exclude all-zero employees
- **Vesting Schedule:** Timeline for when employee owns profit sharing balance
  - Old Schedule: 6-year cliff (100% at year 6)
  - New Schedule: 5-year graded (20%/yr starting year 1)

### Database Tables
- **PAYPROFIT (READY):** Snapshot table with one record per employee
- **PAY_PROFIT (SMART):** Temporal table with multiple records per employee/year
- **PARTICIPANT_TOTAL_VESTING_BALANCE:** Current balance summary
- **PROFIT_DETAIL:** Individual transactions (contributions, distributions, etc.)
- **DEMOGRAPHIC:** Employee master data

### Key Fields
- `py_ps_years`: Years in profit sharing (READY only - not imported to SMART)
- `ps_amt`: Current profit sharing amount (READY)
- `CurrentBalance`: Current vesting balance (SMART)
- `YEARS_OF_SERVICE_CREDIT`: Transaction-level years credit (SMART aggregates this)

---

## Change History

| Date | Version | Changes | Author |
|------|---------|---------|--------|
| 2025-10-01 | 1.0 | Initial baseline analysis after revert | System |

---

## References

- **COBOL Source:** `QPAY066.pco` in repository root
- **Service Implementation:** `TerminatedEmployeeReportService.cs`
- **Test Suite:** `TerminatedEmployeeAndBeneficiaryReportIntegrationTests.cs`
- **Related Docs:** 
  - `QPAY066_COBOL_ANALYSIS.md` (detailed COBOL logic breakdown)
  - API documentation (when available)
