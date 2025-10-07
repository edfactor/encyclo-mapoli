# Quick Report Cross-Reference Guide

**Visual summary of which report values should match for data validation.**

---

## 🎯 Primary Distribution Match (Your Example)

```
PAY444.DISTRIB (ddd,ddd,ddd.dd)
    ↓
    = PAY443.TotalDistributions (ddd,ddd,ddd.dd)
    ↓
    = QPAY129.Distributions (ddd,ddd,ddd.dd)
    ↓
    = QPAY066TA.TotalDisbursements (ddd,ddd,ddd.dd)
```

**All four values MUST match exactly!**

---

## 💰 Critical Financial Reconciliations

### 1. Distributions (4-way match)
```
PAY444 ─────┐
            ├──→ Must all equal
PAY443 ─────┤
            ├──→ Distributions Value
QPAY129 ────┤
            │
QPAY066TA ──┘
```

### 2. Contributions (2-way match)
```
PAY444.CONTRIB ←→ PAY443.TotalContributions
```

### 3. Forfeitures (3-way match)
```
PAY444.FORFEITS ─┬─ Must all equal
                 ├─ PAY443.TotalForfeitures
                 └─ QPAY129.ForfeitedAmount
```

### 4. Earnings (2-way match)
```
PAY444.EARNINGS ←→ PAY443.TotalEarnings
```

### 5. Balances (2-way match each)
```
Beginning: PAY444 ←→ PAY443
Ending:    PAY444 ←→ PAY443
```

---

## 📊 PAY426 Summary ↔ Section Reports

```
PAY426N-9 (Summary)
    ├── Total Wages ──→ Must equal sum of:
    │                   ├─ PAY426N-01 (Section 1)
    │                   ├─ PAY426N-02 (Section 2)
    │                   ├─ PAY426N-03 (Section 3)
    │                   ├─ PAY426N-04 (Section 4)
    │                   ├─ PAY426N-05 (Section 5)
    │                   ├─ PAY426N-06 (Section 6)
    │                   ├─ PAY426N-07 (Section 7)
    │                   ├─ PAY426N-08 (Section 8)
    │                   └─ PAY426N-10 (Section 10)
    │
    ├── Total Hours ───→ Must equal sum of all section hours
    └── Total Points ──→ Must equal sum of all section points
```

---

## ⚖️ Balance Equation (Must Always Hold)

```
Ending Balance = Beginning Balance 
                 + Contributions
                 + Earnings
                 + Earnings2
                 + Forfeitures
                 - Distributions
                 + Military
```

**Applies to:** PAY444 (and should match PAY443)

---

## 🔄 Special Case: ALLOC (Internal Transfers)

```
ALLOC + PAID ALLOC = 0.00

PAY444.ALLOC (money IN) 
    + 
PAY444.PAID_ALLOC (money OUT) 
    = 
0.00 (must net to zero!)
```

**Note:** These are NOT included in contribution or distribution totals.

---

## 👥 Participant Count Matches

```
PAY426.ALL_EMP ──┬──→ PAY426N-9.ALL_EMP
                 └──→ PAY443.TotalParticipants

PAY426.IN_PLAN ───→ PAY426N-9.IN_PLAN
```

---

## 🚨 Validation Priority Order

**Do these checks first (highest risk):**

1. ✅ **PAY444.DISTRIB = PAY443 = QPAY129 = QPAY066TA** (4-way distributions)
2. ✅ **PAY444 Balance Equation** (mathematical integrity)
3. ✅ **PAY444.CONTRIB = PAY443.CONTRIB** (contributions)
4. ✅ **PAY444.FORFEITS = PAY443 = QPAY129** (3-way forfeitures)
5. ✅ **ALLOC + PAID ALLOC = 0** (transfer balance)

**Then validate these (medium risk):**

6. ✅ PAY444.EARNINGS = PAY443.EARNINGS
7. ✅ PAY444 Beginning/Ending Balance = PAY443
8. ✅ PAY426N-9 totals = sum of PAY426N-## sections

---

## 📝 Field Names for API Validation

When using the `ValidateReportFieldsAsync` endpoint, use these field names:

| Report Value | Field Name in API |
|--------------|-------------------|
| Distributions | `TotalDistributions` |
| Contributions | `TotalContributions` |
| Forfeitures | `TotalForfeitures` |
| Earnings | `TotalEarnings` |
| Beginning Balance | `BeginningBalance` |
| Ending Balance | `EndingBalance` |
| ALLOC | `Alloc` |
| PAID ALLOC | `PaidAlloc` |
| Military | `Military` |

---

## 💡 Quick Tips

- **Format:** All monetary values are `ddd,ddd,ddd.dd` (decimal with 2 places)
- **Tolerance:** Use ±0.01 for decimal comparisons (rounding)
- **Military:** Shows separately, NOT in CONTRIB field, but IS in balance
- **ALLOC:** Internal transfers only, NOT in contribution/distribution totals
- **Sections:** PAY426 has sections 1-8 and 10 (no section 9, which is the summary)

---

## 🔗 Related Documents

- **[REPORT_CROSSREFERENCE_MATRIX.md](./REPORT_CROSSREFERENCE_MATRIX.md)** - Complete detailed matrix
- **[Year-End-Testability-And-Acceptance-Criteria.md](./Year-End-Testability-And-Acceptance-Criteria.md)** - Testing requirements
- **[TELEMETRY_GUIDE.md](./TELEMETRY_GUIDE.md)** - How to add telemetry to validation

---

## 📍 Confluence Source

Original documentation: [Profit Sharing - Balance Reports (Matches)](https://demoulas.atlassian.net/wiki/spaces/MAIN/pages/31886714/Profit+Sharing+-+Balance+Reports+Matches)

---

**Last Updated:** October 6, 2025
