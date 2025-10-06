# Profit Sharing Report Cross-Reference Matrix

**Purpose:** This matrix documents which values across different Profit Sharing reports should match for data integrity validation. Use this as a reference for implementing checksum validation and report reconciliation.

**Last Updated:** October 6, 2025

---

## Report Overview

| Report Code | Report Name | Purpose |
|-------------|-------------|---------|
| **PAY426** | Profit Sharing Recap (by sections) | Current status of plan members, broken by sections |
| **PAY426N-9** | Summary Report | Summary totals across all sections |
| **PAY426N-01 through PAY426N-08, PAY426N-10** | Section Reports | Individual section breakdowns |
| **PAY443** | Forfeitures and Points Report | Forfeiture details and point calculations |
| **PAY444** | Year-End Balance Report | Detailed account balances with transactions |
| **QPAY129** | Distribution and Forfeiture Amount Report | Distribution and forfeiture totals used in Year End |
| **QPAY066** | Terminated Employee Report | Terminated employee details and balances |
| **QPAY066TA** | Terminated Employee Total Disbursements | Total disbursements for terminated employees |

---

## Cross-Reference Matrix

### 1. Total Distributions (Year-End Processing)

This is the primary cross-check value that appears across multiple reports.

| Report | Field Name | Format | Notes |
|--------|-----------|--------|-------|
| **PAY444** | DISTRIB (Totals section) | ddd,ddd,ddd.dd | Total distributions for the year |
| **PAY443** | Total Distributions | ddd,ddd,ddd.dd | Should match PAY444's DISTRIB exactly |
| **QPAY129** | Distributions | ddd,ddd,ddd.dd | Should match PAY444 and PAY443 |
| **QPAY066TA** | Total Disbursements | ddd,ddd,ddd.dd | Should match the above three reports |

**Validation Rule:**
```
PAY444.DISTRIB = PAY443.TotalDistributions = QPAY129.Distributions = QPAY066TA.TotalDisbursements
```

---

### 2. Total Contributions

| Report | Field Name | Format | Notes |
|--------|-----------|--------|-------|
| **PAY444** | CONTRIB (Totals section) | ddd,ddd,ddd.dd | Total contributions (excludes ALLOC) |
| **PAY443** | Total Contributions | ddd,ddd,ddd.dd | Should match PAY444's CONTRIB |
| **PAY426** | Section Total Contributions | ddd,ddd,ddd.dd | Sum of all section contributions |

**Validation Rule:**
```
PAY444.CONTRIB = PAY443.TotalContributions = SUM(PAY426.SectionContributions)
```

**Important Note:** ALLOC (allocated transfers) is NOT included in total contributions.

---

### 3. Total Forfeitures

| Report | Field Name | Format | Notes |
|--------|-----------|--------|-------|
| **PAY444** | FORFEITS (Totals section) | ddd,ddd,ddd.dd | Total forfeitures for the year |
| **PAY443** | Total Forfeitures | ddd,ddd,ddd.dd | Should match PAY444's FORFEITS |
| **QPAY129** | Forfeited Amount | ddd,ddd,ddd.dd | Should match PAY444 and PAY443 |

**Validation Rule:**
```
PAY444.FORFEITS = PAY443.TotalForfeitures = QPAY129.ForfeitedAmount
```

---

### 4. Total Earnings

| Report | Field Name | Format | Notes |
|--------|-----------|--------|-------|
| **PAY444** | EARNINGS (Totals section) | ddd,ddd,ddd.dd | Investment earnings for the year |
| **PAY443** | Total Earnings | ddd,ddd,ddd.dd | Should match PAY444's EARNINGS |

**Validation Rule:**
```
PAY444.EARNINGS = PAY443.TotalEarnings
```

---

### 5. Beginning Balance

| Report | Field Name | Format | Notes |
|--------|-----------|--------|-------|
| **PAY444** | Beginning Balance (Totals) | ddd,ddd,ddd.dd | Total plan balance at year start |
| **PAY443** | Beginning Balance | ddd,ddd,ddd.dd | Should match PAY444 |

**Validation Rule:**
```
PAY444.BeginningBalance = PAY443.BeginningBalance
```

---

### 6. Ending Balance

| Report | Field Name | Format | Notes |
|--------|-----------|--------|-------|
| **PAY444** | Ending Balance (Totals) | ddd,ddd,ddd.dd | Total plan balance at year end |
| **PAY443** | Ending Balance | ddd,ddd,ddd.dd | Should match PAY444 |

**Validation Rule:**
```
PAY444.EndingBalance = PAY443.EndingBalance
```

**Balance Equation:**
```
Ending Balance = Beginning Balance + CONTRIB + EARNINGS + EARNINGS2 + FORFEITS - DISTRIB + Military
```

---

### 7. PAY426 Section Reconciliation

The PAY426N-9 (summary) should match the section totals from PAY426 and individual section reports.

| Source | Field | Target | Validation |
|--------|-------|--------|-----------|
| **PAY426N-9** | Total Wages | **PAY426** | Sum of all section wages |
| **PAY426N-9** | Total Hours | **PAY426** | Sum of all section hours |
| **PAY426N-9** | Total Points | **PAY426** | Sum of all section points |
| **PAY426N-9** | Section 1 Total | **PAY426N-01** | Section 1 total wages |
| **PAY426N-9** | Section 2 Total | **PAY426N-02** | Section 2 total wages |
| **PAY426N-9** | Section 3 Total | **PAY426N-03** | Section 3 total wages |
| **PAY426N-9** | Section 4 Total | **PAY426N-04** | Section 4 total wages |
| **PAY426N-9** | Section 5 Total | **PAY426N-05** | Section 5 total wages |
| **PAY426N-9** | Section 6 Total | **PAY426N-06** | Section 6 total wages |
| **PAY426N-9** | Section 7 Total | **PAY426N-07** | Section 7 total wages |
| **PAY426N-9** | Section 8 Total | **PAY426N-08** | Section 8 total wages |
| **PAY426N-9** | Section 10 Total | **PAY426N-10** | Section 10 total wages |

**Validation Rules:**
```
PAY426N-9.TotalWages = PAY426.SectionTotalWages
PAY426N-9.TotalWages = SUM(PAY426N-01..08,10.Wages)

PAY426N-9.TotalHours = PAY426.SectionTotalHours
PAY426N-9.TotalHours = SUM(PAY426N-01..08,10.Hours)

PAY426N-9.TotalPoints = PAY426.SectionTotalPoints
PAY426N-9.TotalPoints = SUM(PAY426N-01..08,10.Points)
```

---

### 8. ALLOC and PAID ALLOC (Special Case)

**Important:** These are internal transfers between accounts and should net to zero.

| Report | Field Name | Format | Notes |
|--------|-----------|--------|-------|
| **PAY444** | ALLOC (Totals section) | ddd,ddd,ddd.dd | Money transferred INTO accounts |
| **PAY444** | PAID ALLOC (Totals section) | ddd,ddd,ddd.dd | Money transferred OUT of accounts |

**Validation Rule:**
```
PAY444.ALLOC + PAY444.PAID_ALLOC = 0.00
```

**Note:** ALLOC and PAID ALLOC are NOT included in total contribution or distribution totals because they represent internal transfers.

---

### 9. Military Contributions

| Report | Field Name | Format | Notes |
|--------|-----------|--------|-------|
| **PAY444** | Military (Detail section) | ddd,ddd.dd | Per-employee military contribution |
| **PAY444** | Military (Totals section) | ddd,ddd,ddd.dd | Total military contributions |

**Notes:**
- Military contributions do NOT show in the CONTRIB field
- Military contributions are stored separately in PROFIT_EARN with PROFIT_CODE = 0 and PROFIT_YEAR = YYYY.1
- Military contributions ARE included in the ending balance calculation

---

## Participant Count Cross-References

### 10. Employee Counts

| Report | Field Name | Notes |
|--------|-----------|-------|
| **PAY426** | ALL-EMP | Total employees in plan |
| **PAY426** | NEW-EMP | New employees |
| **PAY426** | EMP<21 | Employees under 21 |
| **PAY426** | IN-PLAN | Employees actively in plan |
| **PAY426N-9** | Employee Totals | Should match PAY426 counts |
| **PAY443** | Total Participants | Should match PAY426 ALL-EMP |

**Validation Rule:**
```
PAY426.ALL_EMP = PAY426N-9.ALL_EMP
PAY426.IN_PLAN = PAY426N-9.IN_PLAN
PAY443.TotalParticipants = PAY426.ALL_EMP
```

---

## Implementation Guidelines for Checksum Validation

### Priority 1: Critical Monetary Reconciliations

These are the most important cross-checks for data integrity:

1. **PAY444.DISTRIB = PAY443.DISTRIB = QPAY129.Distributions = QPAY066TA.TotalDisbursements**
   - Field names: `TotalDistributions`, `Distributions`, `TotalDisbursements`
   - Format: Decimal(15,2)

2. **PAY444.CONTRIB = PAY443.CONTRIB**
   - Field names: `TotalContributions`
   - Format: Decimal(15,2)

3. **PAY444.FORFEITS = PAY443.FORFEITS = QPAY129.ForfeitedAmount**
   - Field names: `TotalForfeitures`, `ForfeitedAmount`
   - Format: Decimal(15,2)

4. **PAY444.EndingBalance = PAY443.EndingBalance**
   - Field names: `EndingBalance`
   - Format: Decimal(15,2)

### Priority 2: Balance Equation Validation

Validate the accounting equation within each report:

```csharp
// PAY444 Balance Equation
EndingBalance = BeginningBalance 
                + Contributions 
                + Earnings 
                + Earnings2 
                + Forfeitures 
                - Distributions 
                + Military;
```

### Priority 3: Section Reconciliation

Validate that summary reports match their detailed breakdowns:

```csharp
// PAY426 Section Reconciliation
PAY426N_9.TotalWages = Sum(PAY426N_01..08_10.Wages);
PAY426N_9.TotalHours = Sum(PAY426N_01..08_10.Hours);
PAY426N_9.TotalPoints = Sum(PAY426N_01..08_10.Points);
```

### Priority 4: Transfer Balance Check

```csharp
// ALLOC Balance Check
ALLOC + PAID_ALLOC = 0.00 (within rounding tolerance)
```

---

## Sample API Request for Validation

Using the newly implemented `ValidateReportFieldsAsync` endpoint:

```json
POST /checksum/validate-fields
{
  "profitYear": 2024,
  "reportType": "PAY444",
  "fields": {
    "TotalDistributions": 12345678.90,
    "TotalContributions": 23456789.01,
    "TotalForfeitures": 345678.12,
    "EndingBalance": 98765432.10,
    "BeginningBalance": 87654321.00,
    "TotalEarnings": 1234567.89
  }
}
```

**Response will show:**
- Which fields match archived checksums
- Which fields indicate data drift
- Per-field validation details

---

## Database Schema References

### Key Tables

- **PROFIT_CONT**: Contributions (PROFIT_CODE = 6 for ALLOC)
- **PROFIT_FORT**: Forfeitures (PROFIT_CODE = 5 & 9 for PAID ALLOC)
- **PROFIT_EARN**: Earnings (PROFIT_CODE = 0 for Military contributions)
- **ReportChecksum**: Archived checksums with KeyFieldsChecksumJson

---

## Testing Checklist

When implementing cross-reference validation:

- [ ] PAY444 distributions match PAY443
- [ ] PAY444 distributions match QPAY129
- [ ] PAY444 distributions match QPAY066TA
- [ ] PAY444 contributions match PAY443
- [ ] PAY444 forfeitures match PAY443
- [ ] PAY444 forfeitures match QPAY129
- [ ] PAY444 earnings match PAY443
- [ ] PAY444 balance equation validates
- [ ] PAY426N-9 totals match PAY426 section totals
- [ ] Each PAY426N-## matches corresponding PAY426 section
- [ ] ALLOC + PAID ALLOC = 0
- [ ] Participant counts match across reports
- [ ] Military contributions properly excluded from CONTRIB
- [ ] Military contributions properly included in balance equation

---

## Related Documentation

- [TELEMETRY_GUIDE.md](./TELEMETRY_GUIDE.md) - Telemetry implementation for validation endpoints
- [VALIDATION_PATTERNS.md](../../../.github/VALIDATION_PATTERNS.md) - Server/client validation patterns
- [DISTRIBUTED_CACHING_PATTERNS.md](../../../.github/DISTRIBUTED_CACHING_PATTERNS.md) - Caching strategies for report data
- [Year-End-Testability-And-Acceptance-Criteria.md](./Year-End-Testability-And-Acceptance-Criteria.md) - Year-end testing requirements

---

## Confluence References

- [Profit Sharing - Balance Reports (Matches)](https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31886714/Profit+Sharing+-+Balance+Reports+Matches)
- [Profit Sharing - How to read PAY444](https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31887292/Profit+Sharing+-+How+to+read+PAY444)
- [PAY443 (Profit-sharing Forfeitures and Points Report)](https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/70549709/PAY443+Profit-sharing+Forfeitures+and+Points+Report)
- [QPAY129 (Distribution and Forfeiture Amount Report)](https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/51478875/QPAY129+Distribution+and+Forfeiture+Amount+Report)
- [QPAY066 (Terminated Employee Report)](https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/51478799/QPAY066+Terminated+Employee+Report)

---

## Notes

- All monetary values use format `ddd,ddd,ddd.dd` (Decimal with 2 decimal places)
- The matching letters (a, b, c, etc.) in the original Confluence document refer to these cross-reference relationships
- When implementing validation, use a tolerance of ±0.01 for decimal comparisons to account for rounding
- ALLOC and PAID ALLOC are special cases that represent internal transfers and must net to zero
- Military contributions have special handling and storage separate from regular contributions
