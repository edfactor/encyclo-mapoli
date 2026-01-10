# Balance Reports Cross-Reference Matrix

## Overview

This matrix documents the fields and reports that must be cross-referenced to ensure balance accuracy across the Profit Sharing system. Letters (A-Z) indicate which totals should match across different reports.

**Source:** [Profit Sharing - Balance Reports (Matches)](https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31886714/Profit+Sharing+-+Balance+Reports+Matches)

---

## Report Structure

### Primary Reports

- **PAY426** - Report by sections (master/detail view)
- **PAY426N-9** - Summary report (aggregated totals)
- **PAY426N-01 through PAY426N-08, PAY426N-10** - Individual section reports

### Key Principle

**The data on PAY426N-9 should match the section totals on PAY426 and the section reports PAY426N-01 through PAY426N-08 and PAY426N-10.**

---

## Cross-Reference Matrix

### Section 1: Wages and Point Values

| Field/Metric     | PAY426        | PAY426N-9     | PAY426N-01 to N-08, N-10 | Match Letter | Notes                                    |
| ---------------- | ------------- | ------------- | ------------------------ | ------------ | ---------------------------------------- |
| **Total Wages**  | Section Total | Summary Total | Individual Section       | A            | Must match across all three report types |
| **Total Points** | Section Total | Summary Total | Individual Section       | B            | Must match across all three report types |
| **Point Value**  | Calculated    | Calculated    | Calculated               | C            | Derived from Total Wages / Total Points  |

### Section 2: Contributions and Distributions

| Field/Metric            | PAY426        | PAY426N-9     | QPAY129             | Year End          | Match Letter | Notes                         |
| ----------------------- | ------------- | ------------- | ------------------- | ----------------- | ------------ | ----------------------------- |
| **Total Contributions** | Section Total | Summary Total | N/A                 | Beginning Balance | D            | Excludes ALLOC transfers      |
| **Total Distributions** | Section Total | Summary Total | Distribution Amount | Used in YE Calc   | E            | Excludes PAID ALLOC transfers |
| **Forfeited Amount**    | Section Total | Summary Total | Forfeited Amount    | Used in YE Calc   | F            | From QPAY129 to Year End      |

### Section 3: Account Transfers (ALLOC)

| Field/Metric     | PAY426                 | PAY444          | Match Letter | Notes                                  |
| ---------------- | ---------------------- | --------------- | ------------ | -------------------------------------- |
| **ALLOC**        | Money IN to account    | Transfer Credit | G            | NOT included in Total Contribution     |
| **PAID ALLOC**   | Money OUT from account | Transfer Debit  | H            | NOT included in Total Distribution     |
| **Net Transfer** | ALLOC + PAID ALLOC     | Should = 0      | I            | Transfers must net to zero across plan |

**Important:** ALLOC and PAID ALLOC represent internal transfers between accounts within the plan. These amounts must balance to zero when summed across all accounts.

### Section 4: PAY444 Balance Components

| Field/Metric                  | PAY443         | PAY444             | Match Letter | Notes                                 |
| ----------------------------- | -------------- | ------------------ | ------------ | ------------------------------------- |
| **Beginning Balance**         | Prior Year End | Current Year Start | J            | PAY443 end = PAY444 start             |
| **Contributions**             | Year Activity  | Year Activity      | K            | Should match PAY426 contributions     |
| **Distributions**             | Year Activity  | Year Activity      | L            | Should match PAY426 distributions     |
| **ALLOC (Transfer IN)**       | N/A            | Money IN           | M            | Not in contribution total             |
| **PAID ALLOC (Transfer OUT)** | N/A            | Money OUT          | N            | Not in distribution total             |
| **Ending Balance**            | Year End       | Next Year Start    | O            | Becomes next year's beginning balance |

### Section 5: Year-End Process Inputs

| Field/Metric            | Source Report    | Destination          | Match Letter | Notes                        |
| ----------------------- | ---------------- | -------------------- | ------------ | ---------------------------- |
| **Distribution Amount** | QPAY129          | Year End Calculation | P            | Used in profit allocation    |
| **Forfeited Amount**    | QPAY129          | Year End Calculation | Q            | Redistributed in allocation  |
| **Total Wages**         | PAY426/PAY426N-9 | Year End Calculation | R            | Basis for point calculations |
| **Total Points**        | PAY426/PAY426N-9 | Year End Calculation | S            | Basis for distribution       |

---

## Critical Balance Rules

### Rule 1: Section Totals Must Match

```
PAY426 (Section Total) = PAY426N-9 (Summary Total) = Sum(PAY426N-01 to N-08, N-10)
```

### Rule 2: Transfers Must Net to Zero

```
Sum(ALLOC across all accounts) + Sum(PAID ALLOC across all accounts) = 0
```

### Rule 3: Contributions Exclude ALLOC

```
Total Contribution = Employer Contributions + Employee Contributions
Total Contribution ≠ Total Contribution + ALLOC
```

### Rule 4: Distributions Exclude PAID ALLOC

```
Total Distribution = Withdrawals + Other Distributions
Total Distribution ≠ Total Distribution + PAID ALLOC
```

### Rule 5: Balance Equation

```
Ending Balance = Beginning Balance + Contributions + ALLOC - Distributions - PAID ALLOC + Investment Returns/Losses
```

---

## Validation Checklist

### Pre-Year-End Validation

- [ ] PAY426 section totals match PAY426N-9 summary totals
- [ ] PAY426N-01 through PAY426N-08 and PAY426N-10 individual totals sum to PAY426N-9 totals
- [ ] ALLOC + PAID ALLOC = 0 across entire plan
- [ ] PAY444 beginning balance matches prior year PAY443 ending balance

### Year-End Process Validation

- [ ] QPAY129 distribution amount correctly imported to Year End
- [ ] QPAY129 forfeited amount correctly imported to Year End
- [ ] PAY426 wages and points correctly imported to Year End
- [ ] Calculated point values consistent across all reports

### Post-Distribution Validation

- [ ] PAY444 distributions match PAY426 distributions (excluding PAID ALLOC)
- [ ] PAY444 contributions match PAY426 contributions (excluding ALLOC)
- [ ] PAY444 ending balance equation validates
- [ ] Next year PAY443 beginning balance equals current year PAY444 ending balance

---

## Report Cross-Reference Quick Guide

| If validating...          | Check against...                 | Field to compare                 | Expected result |
| ------------------------- | -------------------------------- | -------------------------------- | --------------- |
| PAY426 Total Wages        | PAY426N-9 Total Wages            | Wages                            | Exact match     |
| PAY426N-9 Total Wages     | Sum of PAY426N-01 to N-10 Wages  | Wages                            | Exact match     |
| PAY426 Total Points       | PAY426N-9 Total Points           | Points                           | Exact match     |
| PAY426N-9 Total Points    | Sum of PAY426N-01 to N-10 Points | Points                           | Exact match     |
| QPAY129 Distribution      | Year End Input                   | Distribution Amount              | Exact match     |
| QPAY129 Forfeited         | Year End Input                   | Forfeited Amount                 | Exact match     |
| PAY444 ALLOC              | PAY444 PAID ALLOC                | Net Transfer                     | Sum = 0         |
| PAY444 Total Contribution | PAY426 Total Contribution        | Contributions (excl. ALLOC)      | Exact match     |
| PAY444 Total Distribution | PAY426 Total Distribution        | Distributions (excl. PAID ALLOC) | Exact match     |
| PAY443 Ending Balance     | PAY444 Beginning Balance         | Balance                          | Exact match     |

---

## Common Balance Issues and Resolutions

### Issue 1: Section Totals Don't Match Summary

**Symptoms:** PAY426 section totals ≠ PAY426N-9 summary totals  
**Likely Causes:**

- Data filtering differences between reports
- Timing of report generation (data changed between runs)
- Missing records in section reports

**Resolution Steps:**

1. Verify all section reports (PAY426N-01 to N-10) were generated from same data snapshot
2. Sum individual section totals manually
3. Compare to PAY426N-9 summary
4. Investigate discrepancies by section

### Issue 2: ALLOC/PAID ALLOC Doesn't Net to Zero

**Symptoms:** Sum(ALLOC) + Sum(PAID ALLOC) ≠ 0  
**Likely Causes:**

- Missing transfer transactions
- Incomplete posting of transfer pairs
- Data entry error in transfer amounts

**Resolution Steps:**

1. List all ALLOC transactions
2. List all PAID ALLOC transactions
3. Verify each ALLOC has corresponding PAID ALLOC
4. Verify amounts match exactly (opposite signs)

### Issue 3: Year-End Import Mismatches

**Symptoms:** QPAY129 amounts don't match Year End inputs  
**Likely Causes:**

- Manual entry errors
- Report generated before final adjustments
- Import process failure

**Resolution Steps:**

1. Regenerate QPAY129 with final data
2. Re-import distribution and forfeited amounts
3. Verify import logs for errors
4. Cross-check with PAY426 totals

### Issue 4: Beginning Balance Mismatches

**Symptoms:** PAY444 beginning balance ≠ prior year PAY443 ending balance  
**Likely Causes:**

- Year-end process incomplete
- Manual adjustments not carried forward
- Data archiving issues

**Resolution Steps:**

1. Pull prior year PAY443 report
2. Compare ending balance to current year PAY444 beginning balance
3. Investigate any adjustments made between year-end and year-start
4. Document and correct discrepancies

---

## Implementation Notes for Development

### Database Validation Queries

When implementing automated balance checks, the following queries should be implemented:

```sql
-- Check PAY426 section totals match PAY426N-9 summary
-- Check ALLOC + PAID ALLOC = 0
-- Check PAY444 beginning balance = prior PAY443 ending balance
-- Check contributions match (excluding ALLOC)
-- Check distributions match (excluding PAID ALLOC)
```

### API Endpoints for Balance Validation

Consider implementing these validation endpoints:

- `/api/reports/validate-section-totals/{year}`
- `/api/reports/validate-alloc-transfers/{year}`
- `/api/reports/validate-beginning-balances/{year}`
- `/api/reports/validate-qpay129-import/{year}`

### Telemetry and Monitoring

Track these business metrics:

- `ps_balance_validation_total{validation_type, result}`
- `ps_balance_discrepancy_amount{report_type, field}`
- `ps_alloc_transfer_imbalance{year}`

---

## References

- [How to read PAY444](https://demoulas.atlassian.net/wiki/display/MAIN/Profit+Sharing+-+How+to+read+PAY444)
- [Profit Sharing - Balance Reports (Matches)](https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31886714/Profit+Sharing+-+Balance+Reports+Matches)

---

_Document created: October 12, 2025_  
_Last updated: October 12, 2025_  
_Maintained by: Development Team_
