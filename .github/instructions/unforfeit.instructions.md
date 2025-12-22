---
applyTo: "src/ui/src/pages/DecemberActivities/UnForfeit/**/*.*"
paths: "src/ui/src/pages/DecemberActivities/UnForfeit/**/*.*"
---

# UnForfeit (Rehire Forfeitures)

## Purpose

Reverses profit-sharing forfeitures for employees who were previously terminated and have been rehired within a specified date range. This allows forfeited amounts to be restored to the employee's account.

## Key Components

| File                                   | Responsibility                                               |
| -------------------------------------- | ------------------------------------------------------------ |
| `UnForfeit.tsx`                        | Page container with frozen year warning and navigation guard |
| `UnForfeitSearchFilter.tsx`            | Date range filter with React Hook Form + Yup                 |
| `UnForfeitGrid.tsx`                    | Master-detail grid with inline editing                       |
| `UnForfeitGridColumns.ts`              | Master row columns (badge, name, SSN, dates, balances)       |
| `UnForfeitProfitDetailGridColumns.tsx` | Detail row columns with editable "Suggested Unforfeiture"    |
| `UnForfeitHeaderComponent.tsx`         | Bulk save header using `SharedForfeitHeaderComponent`        |

## State Management

### Page-Level (`useUnForfeitState` hook)

- `initialSearchLoaded`, `resetPageFlag`, `hasUnsavedChanges`, `shouldArchive`

### Grid State (`useUnForfeitGrid` hook)

- `expandedRows`, `editedValues`, `loadingRowIds`, `selectedRowIds`

### Redux (`yearsEndSlice`)

- `unForfeitsQueryParams`, `unForfeitsDetails`

## API Endpoints

| Hook                                        | Endpoint                              | Purpose                 |
| ------------------------------------------- | ------------------------------------- | ----------------------- |
| `useLazyGetUnForfeitsQuery`                 | GET /api/unforfeits                   | Fetch rehired employees |
| `useUpdateForfeitureAdjustmentMutation`     | POST /api/forfeiture-adjustments      | Save individual         |
| `useUpdateForfeitureAdjustmentBulkMutation` | POST /api/forfeiture-adjustments/bulk | Save multiple           |

## Key Patterns

- **Value Transformation**: Negates values before API (user enters positive, API receives negative)
- **Row Key**: `profitDetailId` (single ID, not composite)
- **Editable Years**: All years with non-null `suggestedUnforfeiture`
- **Auto-Archive**: Refreshes when status changes to "Complete"

## Comparison with Termination

| Aspect                            | UnForfeit        | Termination                    |
| --------------------------------- | ---------------- | ------------------------------ |
| Value transformation              | Negates          | No negation                    |
| Row key                           | `profitDetailId` | `${badgeNumber}-${profitYear}` |
| Requires offsettingProfitDetailId | Yes              | No                             |

See `Claude.md` for detailed implementation documentation.
