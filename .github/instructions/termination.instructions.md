---
applyTo: "src/ui/src/pages/DecemberActivities/Termination/**/*.*"
---
# Termination Page Technical Architecture

## Overview

The Termination page is a December Activities feature that allows users to search for and manage employee termination records within a profit-sharing system. The page follows a **Search-Filter-Grid** pattern with editable inline grid cells for managing suggested forfeit amounts.

## Component Hierarchy

```
Termination.tsx (Main Container)
├── TerminationSearchFilter.tsx (Filter Form)
└── TerminationGrid.tsx (Data Grid)
    ├── TerminationGridColumns.tsx (Main columns: PSN, Name)
    ├── TerminationDetailsGridColumns.tsx (Detail columns with editing)
    └── TerminationHeaderComponent.tsx (Bulk action header)
```

## Core Components

### 1. Termination.tsx (Main Container)

**Responsibilities:**

- Page layout and top-level state orchestration
- Fiscal calendar data fetching via `useLazyGetAccountingRangeToCurrent(6)`
- Unsaved changes guard using `useUnsavedChangesGuard(state.hasUnsavedChanges)`
- Error handling with scroll-to-top behavior for validation messages
- Custom state management via `useTerminationState()` hook

**Key State:**

- `isFetching`: Loading state passed to child components
- `state` & `actions` from `useTerminationState()`: Centralized state including `hasUnsavedChanges`, `initialSearchLoaded`, `searchParams`, `resetPageFlag`, `shouldArchive`

**Layout Pattern:**

```typescript
<Page label="TERMINATIONS" actionNode={<StatusDropdownActionNode />}>
  <ApiMessageAlert commonKey="TerminationSave" />
  <DSMAccordion title="Filter">
    <TerminationSearchFilter {...props} />
  </DSMAccordion>
  <TerminationGrid {...props} />
</Page>
```

**Event Listeners:**

- Listens for global `dsmMessage` events to trigger scroll-to-top when errors occur (lines 47-63)

---

### 2. TerminationSearchFilter.tsx (Filter Form)

**Responsibilities:**

- React Hook Form integration with Yup validation schema
- Date range selection (begin/end dates) constrained to fiscal calendar
- Checkbox for "Exclude members with; a $0 Ending Balance, 100% Vested, or Forfeited"
- Redux integration for persisting search state across sessions

**Validation Schema:**

```typescript
yup.object().shape({
  beginningDate: dateStringValidator(2000, 2099, "Beginning Date").required(),
  endingDate: endDateStringAfterStartDateValidator(...).required(),
  forfeitureStatus: yup.string().required(),
  pagination: yup.object({...}).required(),
  profitYear: profitYearValidator(2015, 2099),
  excludeZeroAndFullyVested: yup.boolean()
})
```

**Key Features:**

- Uses `DsmDatePicker` components with fiscal year min/max constraints
- Prevents search when `hasUnsavedChanges` is true (alerts user to save first)
- Resets form to default fiscal dates on reset action
- Integrates `DuplicateSsnGuard` component (warning mode) to check for data quality issues before search

**Redux Integration:**

- Reads `termination` state from `yearsEnd` slice for initial values (line 60)
- Dispatches `clearTermination()` on reset (line 112)

---

### 3. TerminationGrid.tsx (Data Grid)

**Responsibilities:**

- Renders AG Grid with combined main + detail columns
- Displays summary totals (TotalsGrid) for: Amount in Profit Sharing, Vested Amount, Total Forfeitures, Total Beneficiary Allocations
- Pagination controls
- Report summary section
- Read-only mode detection via `useReadOnlyNavigation()`

**Hook Dependencies:**

- `useDynamicGridHeight()`: Calculates optimal grid height based on viewport
- `useTerminationGrid()`: Complex hook encapsulating grid state, data fetching, selection state, and save handlers
- `useReadOnlyNavigation()`: Determines if user has read-only permissions

**Column Assembly:**

```typescript
const mainColumns = GetTerminationColumns(); // PSN + Name
const detailColumns = GetDetailColumns(...); // All detail fields + editable columns
const columnDefs = [...mainColumns, ...detailColumns]; // Flat combined list
```

**Grid Configuration:**

- Row selection enabled (`multiRow`, no checkboxes)
- Preferenc key: `"TERMINATION"` (saves column widths/order to user preferences)
- Row height: 40px
- Suppresses multi-column sorting

---

### 4. TerminationGridColumns.tsx (Main Columns)

**Purpose:** Defines the two primary identification columns.

**Columns:**

1. **PSN (Profit Sharing Number)**: Badge column without suffix (lines 6-10)
2. **Name**: Employee name column (lines 11-13)

**Factory Functions Used:**

- `createBadgeColumn()`: Standardized badge column from grid factory
- `createNameColumn()`: Standardized name column from grid factory

---

### 5. TerminationDetailsGridColumns.tsx (Detail Columns)

**Purpose:** Defines 14+ detail columns including editable "Suggested Forfeit" field and bulk save functionality.

**Columns Overview:**

1. **Profit Year**: Year column
2. **Beginning Balance**: Currency column
3. **Beneficiary Allocation**: Currency column
4. **Distribution Amount**: Currency column
5. **Forfeit Amount**: Currency column
6. **Ending Balance**: Currency column
7. **Vested Balance**: Currency column
8. **Vested %**: Percentage column (formatted as `XX%`)
9. **Term Date**: Date column
10. **YTD PS Hours**: Hours column
11. **Age**: Age column
12. **Forfeited**: Yes/No column
13. **Suggested Forfeit**: **Editable currency column** (lines 92-128)
14. **Save Button**: Action column with individual/bulk save (lines 129-163)

**Editable Column Pattern (Suggested Forfeit):**

```typescript
{
  field: "suggestedForfeit",
  editable: ({ node }) => node.data.suggestedForfeit !== null, // Only editable if not null
  cellEditor: SuggestedForfeitEditor, // Custom editor component
  cellRenderer: SuggestedForfeitCellRenderer, // Custom renderer
  valueGetter: (params) => {
    const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
    return params.context?.editedValues?.[rowKey]?.value ?? params.data.suggestedForfeit;
  },
  cellClass: (params) => {
    const hasError = params.context?.editedValues?.[rowKey]?.hasError;
    return hasError ? "bg-blue-50" : ""; // Highlight errors
  }
}
```

**Key Mechanisms:**

- Uses AG Grid `context` to track edited values in memory (not persisted until save)
- Row key pattern: `${badgeNumber}-${profitYear}` for uniquely identifying rows
- Error highlighting via conditional CSS classes
- Null values render as empty cells (non-editable)

**Save Button Column:**

- Uses `HeaderComponent` (bulk save header) - lines 140
- Uses `createSaveButtonCellRenderer()` factory (individual save) - line 158
- Pinned to right side of grid
- Fixed width (130px)
- Read-only mode disables save buttons
- Loading state tracked via `context.loadingRowIds` set

---

### 6. TerminationHeaderComponent.tsx (Bulk Save Header)

**Purpose:** Renders bulk save controls in the "Save Button" column header.

**Pattern:** Thin wrapper around `SharedForfeitHeaderComponent` configured for "termination" activity type.

```typescript
<SharedForfeitHeaderComponent
  {...params}
  config={{ activityType: "termination" }}
/>
```

**Props Passed Through:**

- `addRowToSelectedRows`: Callback to add row to selection
- `removeRowFromSelectedRows`: Callback to remove row from selection
- `onBulkSave`: Handler for bulk save operation
- `isBulkSaving`: Loading state for bulk operation
- `isReadOnly`: Disables bulk actions in read-only mode

---

## Data Flow

### Search Flow

```
User fills filter form
  ↓
TerminationSearchFilter validates via Yup schema
  ↓
onSearch callback → Termination.tsx → actions.handleSearch()
  ↓
Updates searchParams in useTerminationState
  ↓
TerminationGrid.tsx receives searchParams via props
  ↓
useTerminationGrid hook triggers API call
  ↓
Grid displays results + totals
```

### Edit Flow (Suggested Forfeit)

```
User clicks editable "Suggested Forfeit" cell
  ↓
SuggestedForfeitEditor renders (custom input)
  ↓
User enters new value
  ↓
Value stored in grid context.editedValues[rowKey]
  ↓
Cell re-renders showing edited value (blue highlight if error)
  ↓
User clicks individual "Save" button OR bulk save header
  ↓
onSave or onBulkSave callback triggered
  ↓
API mutation sent (ForfeitureAdjustmentUpdateRequest)
  ↓
On success: grid refreshes, context cleared, success message shown
  ↓
On error: error message displayed, scroll to top
```

---

## State Management

### Redux State (yearsEnd slice)

**Termination-specific state:**

```typescript
termination: {
  startDate: string | null,
  endDate: string | null,
  response: {
    results: TerminationRecord[],
    total: number
  },
  totalEndingBalance: number,
  totalVested: number,
  totalForfeit: number,
  totalBeneficiaryAllocation: number
}
```

**Other yearsEnd state used:**

- `selectedProfitYearForDecemberActivities`: Current profit year
- `unForfeitsQueryParams`: Pagination/sort params

### Local State (useTerminationState hook)

**Provides:**

- `state.hasUnsavedChanges`: Tracks unsaved edits
- `state.initialSearchLoaded`: Tracks if initial search has run
- `state.searchParams`: Current search parameters
- `state.resetPageFlag`: Triggers pagination reset
- `state.shouldArchive`: Flag for archiving records
- `actions.handleSearch()`: Updates search params
- `actions.handleStatusChange()`: Updates page status
- `actions.handleUnsavedChanges()`: Sets unsaved changes flag
- `actions.setInitialSearchLoaded()`: Sets initial search flag
- `actions.handleArchiveHandled()`: Clears archive flag

### Grid Context (AG Grid)

**Stores:**

- `editedValues`: Map of edited cell values keyed by `${badgeNumber}-${profitYear}`
- `loadingRowIds`: Set of PSN IDs currently being saved
- `selectedProfitYear`: Current profit year for validation

## API Integration

**RTK Query Hooks (inferred from useTerminationGrid):**

- `useLazyGetTerminationsQuery()`: Searches terminations by date range
- `useUpdateForfeitureAdjustmentMutation()`: Saves individual forfeit adjustment
- `useBulkUpdateForfeitureAdjustmentMutation()`: Saves multiple forfeit adjustments

**Request/Response Types:**

- `TerminationSearchRequest`: Extends `StartAndEndDateRequest` with `forfeitureStatus`, `archive`, `excludeZeroBalance`, `excludeZeroAndFullyVested`
- `ForfeitureAdjustmentUpdateRequest`: Contains `badgeNumber`, `profitYear`, `suggestedForfeit`, etc.

## Dependencies

**Internal Hooks:**

- `useLazyGetAccountingRangeToCurrent()`: Fetches fiscal calendar dates
- `useTerminationState()`: Custom state management hook
- `useUnsavedChangesGuard()`: Navigation guard for unsaved changes
- `useDynamicGridHeight()`: Calculates grid height
- `useReadOnlyNavigation()`: Read-only mode detection
- `useTerminationGrid()`: Grid state and API integration
- `useDecemberFlowProfitYear()`: Gets current profit year for December activities

**Shared Components:**

- `StatusDropdownActionNode`: Page status dropdown
- `DsmDatePicker`: Date picker component
- `DuplicateSsnGuard`: Data quality guard
- `SuggestedForfeitEditor` / `SuggestedForfeitCellRenderer`: Custom cell editor/renderer
- `SharedForfeitHeaderComponent`: Bulk save header
- `createSaveButtonCellRenderer()`: Save button cell renderer factory

---
