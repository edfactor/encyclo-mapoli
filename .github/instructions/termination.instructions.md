---
applyTo: "src/ui/src/pages/DecemberActivities/Termination/**/*.*"
paths: "src/ui/src/pages/DecemberActivities/Termination/**/*.*"
---

# Termination

A December year-end activity for processing profit-sharing forfeitures for terminated employees.

## Purpose

Process forfeitures for employees terminated within a specified date range. Users can search by termination date, review forfeiture details, and submit individual or bulk adjustments.

## Key Components

| File                                | Responsibility                                                |
| ----------------------------------- | ------------------------------------------------------------- |
| `Termination.tsx`                   | Page container with status handling and unsaved changes guard |
| `TerminationSearchFilter.tsx`       | Date-based search with `DuplicateSsnGuard` prerequisite       |
| `TerminationGrid.tsx`               | Master-detail AG Grid with inline editing and totals          |
| `TerminationGridColumns.tsx`        | Master row column definitions                                 |
| `TerminationDetailsGridColumns.tsx` | Detail row columns with editable "Suggested Forfeit"          |
| `TerminationHeaderComponent.tsx`    | Bulk save checkbox header                                     |

## State Management

### Custom Hooks

- `useTerminationState()`: Page-level state (search params, unsaved changes, archive mode)
- `useTerminationGrid()`: Grid logic (expansion, pagination, save operations)

### Redux State

- `yearsEndSlice.termination`: Search results and financial totals

### Grid Context

- `editedValues`: Inline edits keyed by `${badgeNumber}-${profitYear}`
- `loadingRowIds`: Rows currently being saved

## API Endpoints

| Operation | RTK Query Hook                                | Endpoint                           |
| --------- | --------------------------------------------- | ---------------------------------- |
| Search    | `useLazyGetTerminationReportQuery()`          | `/api/terminations`                |
| Save      | `useUpdateForfeitureAdjustmentMutation()`     | `/api/forfeiture-adjustments`      |
| Bulk Save | `useUpdateForfeitureAdjustmentBulkMutation()` | `/api/forfeiture-adjustments/bulk` |

## Key Patterns

- Master-detail grid with expandable rows
- Value sent as-is (no negation, unlike UnForfeit)
- Composite row key: `${badgeNumber}-${profitYear}`
- Bulk operations process in batches of 5
- Uses shared components from `/src/ui/src/components/ForfeitActivities/`
