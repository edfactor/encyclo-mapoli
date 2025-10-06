# Report Cross-Reference Matrix - Summary for Confluence

## Overview

This document provides a comprehensive cross-reference matrix showing which values across different Profit Sharing reports must match for data integrity validation. It serves as the authoritative reference for implementing checksum validation and report reconciliation.

## Key Cross-References Identified

### 1. **Primary Distribution Match (4-Way)**

The most critical reconciliation - distributions must match across **four different reports**:

- **PAY444**: DISTRIB (Totals section)
- **PAY443**: Total Distributions
- **QPAY129**: Distributions
- **QPAY066TA**: Total Disbursements

**All four must equal exactly the same value (format: ddd,ddd,ddd.dd)**

---

### 2. **Total Contributions (2-Way)**

- **PAY444**: CONTRIB (Totals section)
- **PAY443**: Total Contributions

**Note:** ALLOC (allocated transfers) is NOT included in total contributions

---

### 3. **Total Forfeitures (3-Way)**

- **PAY444**: FORFEITS (Totals section)
- **PAY443**: Total Forfeitures
- **QPAY129**: Forfeited Amount

---

### 4. **Total Earnings (2-Way)**

- **PAY444**: EARNINGS (Totals section)
- **PAY443**: Total Earnings

---

### 5. **Beginning/Ending Balances (2-Way Each)**

Both beginning and ending balances must match:
- **PAY444**: Beginning Balance ↔ **PAY443**: Beginning Balance
- **PAY444**: Ending Balance ↔ **PAY443**: Ending Balance

---

### 6. **PAY426 Section Reconciliation**

**PAY426N-9 (Summary Report)** must match:
- **PAY426**: Section totals (wages, hours, points)
- **PAY426N-01 through PAY426N-08, PAY426N-10**: Individual section reports

Each section's wages, hours, and points must roll up correctly to the summary.

---

### 7. **ALLOC Balance (Special Case)**

Internal transfers must net to zero:

**ALLOC + PAID ALLOC = 0.00**

These represent money transferred between accounts and are NOT included in contribution or distribution totals.

---

## Balance Equation

The fundamental accounting equation that must hold:

```
Ending Balance = Beginning Balance 
                 + Contributions
                 + Earnings
                 + Earnings2
                 + Forfeitures
                 - Distributions
                 + Military
```

---

## Military Contributions (Special Handling)

- Military contributions do NOT show in the CONTRIB field
- They ARE included in the ending balance calculation
- Stored separately with PROFIT_CODE = 0 and PROFIT_YEAR = YYYY.1

---

## Implementation Priority

**Priority 1 (Critical):**
1. PAY444.DISTRIB = PAY443 = QPAY129 = QPAY066TA (4-way distributions)
2. PAY444 Balance Equation validation
3. PAY444.CONTRIB = PAY443.CONTRIB
4. PAY444.FORFEITS = PAY443 = QPAY129 (3-way forfeitures)
5. ALLOC + PAID ALLOC = 0

**Priority 2 (Important):**
6. PAY444.EARNINGS = PAY443.EARNINGS
7. Beginning/Ending balance matches
8. PAY426N-9 = sum of PAY426N-## sections

---

## Field Names for API Validation

When using the `ValidateReportFieldsAsync` endpoint:

| Report Value | Field Name |
|--------------|------------|
| Distributions | `TotalDistributions` |
| Contributions | `TotalContributions` |
| Forfeitures | `TotalForfeitures` |
| Earnings | `TotalEarnings` |
| Beginning Balance | `BeginningBalance` |
| Ending Balance | `EndingBalance` |

---

## Detailed Documentation

Two comprehensive documents have been created in the SMART application:

1. **REPORT_CROSSREFERENCE_MATRIX.md** - Complete detailed matrix with:
   - Full report descriptions
   - Validation rules for each cross-reference
   - Implementation guidelines
   - Testing checklist
   - Database schema references

2. **REPORT_CROSSREFERENCE_QUICK.md** - Quick visual reference with:
   - Simplified diagrams
   - Validation priority order
   - Field name mappings
   - Quick tips

Both documents are available in the SMART application under Documentation → Reports section.

---

## Technical Notes

- All monetary values use format `ddd,ddd,ddd.dd` (Decimal with 2 decimal places)
- Use tolerance of ±0.01 for decimal comparisons (rounding)
- Documents include sample API requests for the new checksum validation service
- Cross-references validated during Year-End processing

---

## Related Confluence Pages

- [Profit Sharing - Balance Reports (Matches)](https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31886714)
- [Profit Sharing - How to read PAY444](https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31887292)
- [PAY443 (Profit-sharing Forfeitures and Points Report)](https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/70549709)
- [QPAY129 (Distribution and Forfeiture Amount Report)](https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/51478875)

---

**Created:** October 6, 2025  
**Based on:** Confluence page 31886714 and related report documentation  
**Implementation:** Available in SMART Profit Sharing application
