---
applyTo: "src/ui/src/pages/InquiriesAndAdjustments/Reversals/**/*.*"
paths: "src/ui/src/pages/InquiriesAndAdjustments/Reversals/**/*.*"
---

# Reversals Page Technical Documentation

## Overview

The Reversals page allows users to reverse profit sharing transactions. Users search for a member, view their profit details, select eligible transactions, and confirm reversals one by one.

## File Structure

```
Reversals/
├── Reversals.tsx                    # Main page component
├── ReversalsSearchFilter.tsx        # Search form (SSN, Badge, Member Type)
├── ReversalsGrid.tsx                # AG Grid with checkbox selection
├── ReversalsGridColumns.ts          # Column definitions and eligibility logic
├── ReversalCheckboxCellRenderer.tsx # Custom checkbox cell renderer
├── ReversalConfirmationModal.tsx    # Step-through confirmation dialog
├── hooks/
│   └── useReversals.ts              # State management hook
└── REVERSALS.md                     # This file
```

## Component Architecture

### Reversals.tsx

Main page wrapper that composes all child components. Uses `MissiveAlertProvider` for alerts and `ApiMessageAlert` for success/error messages.

### ReversalsSearchFilter.tsx

Search form with three fields:

- **Social Security Number** - Numeric input (max 9 digits)
- **Badge/PSN Number** - Numeric input (max 11 digits)
- SSN and Badge are mutually exclusive (one disables the other)
- **Member Type** - Radio buttons: All, Employees, Beneficiaries
- Auto-switches to "Beneficiaries" when badge length exceeds 6 digits (PSN format)

### ReversalsGrid.tsx

AG Grid displaying profit detail transactions with:

- Custom checkbox column for row selection (managed via React state, not AG Grid selection API)
- "REVERSE" button (green, outlined) showing selection count
- Pagination support
- Selection state stored in `Set<number>` and passed to cells via grid context

Key pattern: Uses `gridApiRef.current.redrawRows()` in a `useEffect` to force checkbox re-render when selection changes.

### ReversalsGridColumns.ts

Defines grid columns and eligibility logic:

**Reversible Profit Codes:** `[1, 3, 5, 6, 9]`

- 1: Outgoing payments - Partial withdrawal
- 3: Outgoing direct payments / rollover payments
- 5: Outgoing XFER beneficiary / QDRO allocation
- 6: Incoming QDRO beneficiary allocation
- 9: Outgoing payment from 100% vesting amount

**Eligibility Rules (`isRowReversible`):**

1. Profit code must be in allowed list
2. Transaction must be within last 2 months
3. January rule: If current month is January, transaction month must be > 1 AND < 12 (blocks January and December transactions)

### ReversalCheckboxCellRenderer.tsx

Custom cell renderer that:

- Shows checkbox for eligible rows
- Shows error icon with tooltip for ineligible rows
- Gets/sets selection state via grid context (`getSelectedIds`, `toggleSelection`)

### ReversalConfirmationModal.tsx

Step-through confirmation dialog:

- Shows one transaction at a time: "Reverse Transaction (1/N)"
- Displays profit detail ID and payment amount
- "YES, REVERSE" (red) and "NO, CANCEL" (green outlined) buttons
- Canceling at any point cancels ALL selected reversals

### hooks/useReversals.ts

Central state management using `useReducer`. Manages:

**State:**

- Search parameters and loading state
- Selected member info
- Profit data results and pagination
- Confirmation modal state (items, current index, processing flag)

**Key Actions:**

- `executeSearch(params)` - Search for member, fetch profit details
- `handlePaginationChange(page, size)` - Re-fetch with new pagination
- `initiateReversal(rows)` - Open confirmation modal with selected rows
- `confirmReversal()` - Advance to next item or execute API call
- `cancelReversal()` - Close modal, show cancellation message
- `resetAll()` - Clear all state

**API Integration:**

- Uses `useLazySearchProfitMasterInquiryQuery` for member search
- Uses `useLazyGetProfitMasterInquiryFilteredDetailsQuery` for profit details
- Uses `useReverseProfitDetailsMutation` for reversal execution

## API Endpoint

**POST** `/api/profitdetails/reversals`

Request body:

```json
{
  "ids": [12345, 67890]
}
```

Error handling includes `onlyNetworkToastErrors: true` to suppress automatic toast errors and manually parse 400 validation errors with detailed messages per profit detail ID.

## Message Display

Uses Redux `messageSlice` with key `"ReversalOperation"` for:

- **Success (green):** Lists each reversed profit detail ID with payment amount
- **Error (red):** Shows validation errors per profit detail ID from API
- **Warning (yellow):** Shown when user cancels, listing all cancelled IDs
