# PS-2424 Follow-Up Investigation Report

## Investigation Overview

**Ticket:** PS-2424 - Fix incorrect vested percentage calculation in Account History report  
**Date:** December 2024  
**Investigator:** AI Assistant  
**Purpose:** Database investigation of specific badge examples showing Account History Report issues

## Background

Following the root cause analysis and code fix implementation for PS-2424, this follow-up investigation examines three specific badge numbers mentioned in the ticket to:

1. Verify the data integrity underlying the reported issues
2. Identify any additional data quality concerns
3. Provide recommendations for data cleanup or additional fixes if needed

## Badges Under Investigation

### Badge 706355: Missing ProfitCode 8 in 2021 Earnings

- **Issue:** Account History Report shows $608.56 for 2021, should be $651.16 (missing $42.60 from PC 8)
- **Expected Findings:** PROFIT_DETAIL records with both ProfitCode 0 and ProfitCode 8 for profit year 2021

### Badge 700580: Withdrawal Duplication Across Years

- **Issue:** 2014 withdrawals appear on all subsequent years in Account History Report
- **Expected Findings:** PROFIT_DETAIL records showing improper distribution sequence or year assignment

### Badge 2109: Zero Vested Balance with 20% Vesting

- **Issue:** Shows 20% vesting percentage but $0 vested balance
- **Expected Findings:** Negative balance offsetting contributions, or data integrity issues in year-over-year aggregation

## SQL Investigation Queries

### Query 1: Badge 706355 - Verify ProfitCode 8 Records

```sql
-- Get all PROFIT_DETAIL records for badge 706355, profit year 2021
SELECT
    d.BADGE_NUMBER,
    d.SSN,
    d.ORACLE_HCM_ID,
    pd.PROFIT_YEAR,
    pd.PROFIT_CODE_ID,
    pd.CONTRIBUTION,
    pd.EARNINGS,
    pd.FORFEITURE,
    pd.FEDERAL_TAXES,
    pd.STATE_TAXES,
    pd.DISTRIBUTION_SEQUENCE,
    pd.PROFIT_YEAR_ITERATION
FROM PROFIT_DETAIL pd
INNER JOIN DEMOGRAPHIC d ON pd.SSN = d.SSN
WHERE d.BADGE_NUMBER = 706355
  AND pd.PROFIT_YEAR = 2021
ORDER BY pd.PROFIT_CODE_ID, pd.PROFIT_YEAR_ITERATION;

-- Expected: Records with PROFIT_CODE_ID = 0 ($608.56) and PROFIT_CODE_ID = 8 ($42.60)
```

### Query 2: Badge 706355 - Historical Earnings Aggregation

```sql
-- Get year-over-year earnings to verify aggregation
SELECT
    d.BADGE_NUMBER,
    pd.PROFIT_YEAR,
    SUM(CASE WHEN pd.PROFIT_CODE_ID IN (0, 8) THEN pd.EARNINGS ELSE 0 END) as TotalEarnings,
    SUM(CASE WHEN pd.PROFIT_CODE_ID = 0 THEN pd.EARNINGS ELSE 0 END) as PC0_Earnings,
    SUM(CASE WHEN pd.PROFIT_CODE_ID = 8 THEN pd.EARNINGS ELSE 0 END) as PC8_Earnings,
    SUM(pd.CONTRIBUTION) as TotalContributions,
    SUM(pd.FORFEITURE) as TotalForfeitures
FROM PROFIT_DETAIL pd
INNER JOIN DEMOGRAPHIC d ON pd.SSN = d.SSN
WHERE d.BADGE_NUMBER = 706355
GROUP BY d.BADGE_NUMBER, pd.PROFIT_YEAR
ORDER BY pd.PROFIT_YEAR;

-- Expected: 2021 shows both PC 0 and PC 8 earnings, total = $651.16
```

### Query 3: Badge 700580 - Withdrawal Pattern Analysis

```sql
-- Get all PROFIT_DETAIL records for badge 700580 to identify withdrawal duplication
SELECT
    d.BADGE_NUMBER,
    d.SSN,
    pd.PROFIT_YEAR,
    pd.PROFIT_CODE_ID,
    pd.CONTRIBUTION,
    pd.EARNINGS,
    pd.FORFEITURE,
    pd.DISTRIBUTION_SEQUENCE,
    pd.PROFIT_YEAR_ITERATION,
    pd.REMARK
FROM PROFIT_DETAIL pd
INNER JOIN DEMOGRAPHIC d ON pd.SSN = d.SSN
WHERE d.BADGE_NUMBER = 700580
  AND pd.PROFIT_CODE_ID IN (1, 2, 3, 5, 9)  -- Outgoing payment codes
ORDER BY pd.PROFIT_YEAR, pd.DISTRIBUTION_SEQUENCE;

-- Expected: 2014 withdrawal records that may be incorrectly associated with later years
```

### Query 4: Badge 700580 - Year-Over-Year Balance Calculation

```sql
-- Verify proper year-over-year balance calculation
WITH YearlyTotals AS (
    SELECT
        d.BADGE_NUMBER,
        pd.PROFIT_YEAR,
        SUM(CASE WHEN pd.PROFIT_CODE_ID IN (0, 8) THEN pd.CONTRIBUTION + pd.EARNINGS ELSE 0 END) as Incoming,
        SUM(CASE WHEN pd.PROFIT_CODE_ID IN (1, 2, 3, 5, 9) THEN ABS(pd.CONTRIBUTION + pd.EARNINGS + pd.FORFEITURE) ELSE 0 END) as Outgoing
    FROM PROFIT_DETAIL pd
    INNER JOIN DEMOGRAPHIC d ON pd.SSN = d.SSN
    WHERE d.BADGE_NUMBER = 700580
    GROUP BY d.BADGE_NUMBER, pd.PROFIT_YEAR
)
SELECT
    PROFIT_YEAR,
    Incoming,
    Outgoing,
    Incoming - Outgoing as NetChange,
    SUM(Incoming - Outgoing) OVER (ORDER BY PROFIT_YEAR) as RunningBalance
FROM YearlyTotals
ORDER BY PROFIT_YEAR;

-- Expected: Identify which year(s) have the actual 2014 withdrawal recorded
```

### Query 5: Badge 2109 - Balance and Vesting Analysis

```sql
-- Get comprehensive view of badge 2109's profit sharing records
SELECT
    d.BADGE_NUMBER,
    d.SSN,
    pd.PROFIT_YEAR,
    pd.PROFIT_CODE_ID,
    pd.CONTRIBUTION,
    pd.EARNINGS,
    pd.FORFEITURE,
    pd.FEDERAL_TAXES,
    pd.STATE_TAXES,
    pd.PROFIT_YEAR_ITERATION,
    pd.YEARS_OF_SERVICE_CREDIT
FROM PROFIT_DETAIL pd
INNER JOIN DEMOGRAPHIC d ON pd.SSN = d.SSN
WHERE d.BADGE_NUMBER = 2109
ORDER BY pd.PROFIT_YEAR, pd.PROFIT_CODE_ID;

-- Expected: Negative forfeitures or other transactions offsetting contributions
```

### Query 6: Badge 2109 - Vested Balance Calculation

```sql
-- Calculate actual vested balance using same logic as Account History Report
WITH YearlyTotals AS (
    SELECT
        d.BADGE_NUMBER,
        pd.PROFIT_YEAR,
        SUM(CASE WHEN pd.PROFIT_CODE_ID IN (0, 8) THEN pd.CONTRIBUTION + pd.EARNINGS ELSE 0 END) as TotalIncoming,
        SUM(CASE WHEN pd.PROFIT_CODE_ID IN (1, 2, 3, 5, 9) THEN ABS(pd.CONTRIBUTION + pd.EARNINGS + pd.FORFEITURE) ELSE 0 END) as TotalOutgoing,
        MAX(pd.YEARS_OF_SERVICE_CREDIT) as YearsOfService
    FROM PROFIT_DETAIL pd
    INNER JOIN DEMOGRAPHIC d ON pd.SSN = d.SSN
    WHERE d.BADGE_NUMBER = 2109
    GROUP BY d.BADGE_NUMBER, pd.PROFIT_YEAR
),
RunningBalance AS (
    SELECT
        PROFIT_YEAR,
        TotalIncoming,
        TotalOutgoing,
        YearsOfService,
        SUM(TotalIncoming - TotalOutgoing) OVER (ORDER BY PROFIT_YEAR) as CumulativeBalance
    FROM YearlyTotals
),
VestingSchedule AS (
    SELECT
        PROFIT_YEAR,
        CumulativeBalance,
        YearsOfService,
        CASE
            WHEN YearsOfService < 3 THEN 0.0
            WHEN YearsOfService = 3 THEN 0.20
            WHEN YearsOfService = 4 THEN 0.40
            WHEN YearsOfService = 5 THEN 0.60
            WHEN YearsOfService = 6 THEN 0.80
            ELSE 1.0
        END as VestingPercentage
    FROM RunningBalance
)
SELECT
    PROFIT_YEAR,
    CumulativeBalance,
    YearsOfService,
    VestingPercentage,
    CumulativeBalance * VestingPercentage as VestedBalance
FROM VestingSchedule
ORDER BY PROFIT_YEAR DESC
LIMIT 1;

-- Expected: Identify why vested balance is $0 despite 20% vesting
```

### Query 7: Cross-Check PAY_PROFIT for Data Consistency

```sql
-- Verify data exists in PAY_PROFIT for these badges
SELECT
    d.BADGE_NUMBER,
    pp.SSN,
    pp.PROFIT_YEAR,
    pp.BALANCE,
    pp.VESTED_BALANCE,
    pp.VESTED_PERCENTAGE
FROM PAY_PROFIT pp
INNER JOIN DEMOGRAPHIC d ON pp.SSN = d.SSN
WHERE d.BADGE_NUMBER IN (706355, 700580, 2109)
ORDER BY d.BADGE_NUMBER, pp.PROFIT_YEAR DESC;

-- Expected: Compare PAY_PROFIT data with PROFIT_DETAIL aggregations
```

## Investigation Execution Plan

1. **Connect to Oracle Database** using appropriate credentials (QA/Production as needed)
2. **Execute Query 1 & 2** for badge 706355 to verify ProfitCode 8 presence
3. **Execute Query 3 & 4** for badge 700580 to identify withdrawal duplication pattern
4. **Execute Query 5 & 6** for badge 2109 to analyze zero vested balance issue
5. **Execute Query 7** to cross-check PAY_PROFIT consistency
6. **Document findings** with actual data results
7. **Provide recommendations** based on data analysis

## Expected Outcomes

### Badge 706355

- **Data Verification:** Confirm PROFIT_DETAIL contains both PC 0 ($608.56) and PC 8 ($42.60) for 2021
- **Code Fix Impact:** With the fix in ProfitDetailExtensions.AggregateEarnings(), the Account History Report will now correctly show $651.16
- **Recommendation:** No data cleanup needed; code fix resolves the issue

### Badge 700580

- **Data Analysis:** Identify the actual profit year(s) where the 2014 withdrawal is recorded
- **Issue Type:** Likely a display/aggregation issue rather than data corruption
- **Recommendation:** May require additional investigation into DISTRIBUTION_SEQUENCE handling or year assignment logic

### Badge 2109

- **Balance Analysis:** Determine actual balance composition (contributions, earnings, forfeitures)
- **Vesting Calculation:** Verify years of service calculation and vesting percentage logic
- **Recommendation:** May indicate legitimate zero balance (all contributions forfeited) or data quality issue requiring correction

## Next Steps

Once database queries are executed:

1. Document actual query results in this report
2. Compare findings with expected outcomes
3. Identify any additional code fixes needed beyond PS-2424
4. Create follow-up Jira tickets for any newly discovered issues
5. Recommend data cleanup scripts if data integrity issues are found

## Related Tickets

- **PS-2424:** Main ticket - Fix incorrect vested percentage calculation in Account History report
- **PS-2452:** Duplicate ticket - Profit Sharing Account History Report vesting issue

## Acceptance Criteria Validation

After code fix deployment and database investigation:

- [ ] Badge 706355 Account History Report shows $651.16 for 2021 (includes PC 8)
- [ ] Badge 700580 withdrawal pattern analyzed and issue documented/fixed
- [ ] Badge 2109 zero vested balance explained with data evidence
- [ ] No regressions in Master Inquiry or other reports
- [ ] All unit tests pass (17 tests for ProfitDetailExtensions)

---

**Investigation Status:** Ready for database query execution  
**Code Status:** Fix implemented and unit tested (17/17 tests passing)  
**Next Action:** Execute SQL queries and document findings
