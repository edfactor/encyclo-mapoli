---
applyTo: "src/ui/src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/**/*.*"
paths: "src/ui/src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/**/*.*"
---

# Manage Executive Hours and Dollars

## Purpose

Allows users to manage executive hours and dollar allocations during December year-end profit-sharing. Users can:

- Search executives by name, SSN, badge number, or payroll type
- Edit hours (max 4,000) and dollars (max $20,000,000) inline
- Add additional executives via modal search
- Save pending changes in batch

## Key Components

| File                                                | Responsibility                                     |
| --------------------------------------------------- | -------------------------------------------------- |
| `ManageExecutiveHoursAndDollars.tsx`                | Page container with Save button and MissiveAlerts  |
| `ManageExecutiveHoursAndDollarsGrid.tsx`            | Dual-mode AG Grid (main/modal) with inline editing |
| `ManageExecutiveHoursAndDollarsSearchFilter.tsx`    | Search form with mutual exclusion                  |
| `SearchAndAddExecutive.tsx`                         | Modal for searching/selecting executives           |
| `ManageExecutiveHoursAndDollarsGridColumns.ts`      | Column definitions with full/mini modes            |
| `hooks/useManageExecutiveHoursAndDollars.ts`        | Main business logic hook                           |
| `hooks/useManageExecutiveHoursAndDollarsReducer.ts` | Reducer for dual-grid state                        |

## State Management

### Local State (useReducer)

```typescript
{
  search: { params, results, isSearching, error, initialLoaded },
  modal: { isOpen, results, selectedExecutives, isSearching },
  grid: { data, pendingChanges, additionalExecutives, selectedRows },
  view: { mode, pageNumberReset }
}
```

**Key Selectors**: `selectCombinedGridData`, `selectHasPendingChanges`, `selectIsRowStagedToSave`

## API Endpoints

| Hook                                        | Endpoint                                  | Purpose          |
| ------------------------------------------- | ----------------------------------------- | ---------------- |
| `useLazyGetExecutiveHoursAndDollarsQuery`   | GET /yearend/executive-hours-and-dollars  | Main grid search |
| `useLazyGetAdditionalExecutivesQuery`       | GET /yearend/executive-hours-and-dollars  | Modal search     |
| `useUpdateExecutiveHoursAndDollarsMutation` | PUT /yearend/executive-hours-and-dollars/ | Save changes     |

## Key Patterns

- Dual-mode grid (main vs modal with different columns)
- Mutable row data for AG Grid inline editing
- Pending changes tracking with yellow background indicator
- Multiple read-only guards (role, status, frozen year)
