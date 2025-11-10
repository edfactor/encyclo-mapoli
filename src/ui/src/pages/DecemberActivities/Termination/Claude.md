# Termination Feature - Technical Summary

## Overview

The Termination feature is part of the December Activities workflow in the Smart Profit Sharing application. It provides functionality for managing employee termination records, including searching for terminated employees within a date range and adjusting suggested forfeit amounts for their profit-sharing balances.

**Location**: `src/ui/src/pages/DecemberActivities/Termination/`

**Primary Pattern**: Search-Filter-Grid with inline editable cells and bulk operations

---

## File Structure

```
Termination/
├── Termination.tsx                              # Main page container
├── TerminationSearchFilter.tsx                  # Search/filter form component
├── TerminationGrid.tsx                          # Data grid component
├── TerminationGridColumns.tsx                   # Main grid columns (PSN, Name)
├── TerminationDetailsGridColumns.tsx            # Detail columns with editable fields
├── TerminationHeaderComponent.tsx               # Bulk save header component
├── Claude.md                                    # Existing technical architecture doc
└── __test__/
    ├── Termination.test.tsx                     # Main page tests
    └── TerminationSearchFilter.test.tsx         # Filter form tests
```

---

## Component Files

### 1. Termination.tsx (Main Container)

**Purpose**: Top-level page component that orchestrates the termination workflow.

**Key Responsibilities**:

- Manages page layout using `Page` component from smart-ui-library
- Fetches fiscal calendar data via `useLazyGetAccountingRangeToCurrent(6)` hook
- Provides centralized state management through `useTerminationState()` custom hook
- Implements unsaved changes guard using `useUnsavedChangesGuard()`
- Handles error scroll-to-top behavior for validation messages
- Global event listener for `dsmMessage` events to trigger scroll on errors

**State Management**:

- Local state: `isFetching` - loading indicator
- Custom hook state: `state` and `actions` from `useTerminationState()`
  - `hasUnsavedChanges`: Tracks unsaved grid edits
  - `initialSearchLoaded`: Whether initial search has been performed
  - `searchParams`: Current search parameters
  - `resetPageFlag`: Triggers pagination reset
  - `shouldArchive`: Archive mode flag

**Layout Structure**:

```
<Page label="TERMINATIONS" actionNode={<StatusDropdownActionNode />}>
  <ApiMessageAlert commonKey="TerminationSave" />
  <DSMAccordion title="Filter">
    <TerminationSearchFilter />
  </DSMAccordion>
  <TerminationGrid />
</Page>
```

**Lines of Code**: 122

---

### 2. TerminationSearchFilter.tsx (Filter Form)

**Purpose**: Provides search form with date range selection and filtering options.

**Key Features**:

- **React Hook Form** integration with **Yup** validation schema
- Date range inputs (Begin Date, End Date) constrained to fiscal calendar bounds
- Checkbox option: "Exclude members with; a $0 Ending Balance, 100% Vested, or Forfeited"
- Redux integration for persisting search state across sessions
- `DuplicateSsnGuard` integration (warning mode) for data quality checks
- Prevents search submission when unsaved changes exist

**Validation Schema**:

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

**Form Fields**:

1. Beginning Date (required, MM/DD/YYYY format, constrained to fiscal year)
2. Ending Date (required, must be same or after beginning date)
3. Exclude Zero/Fully Vested checkbox

**Default Values**:

- Begin Date: Fiscal year start date
- End Date: Fiscal year end date
- Forfeiture Status: "showAll"
- Pagination: `{ skip: 0, take: 25, sortBy: "name", isSortDescending: false }`
- Exclude Zero/Fully Vested: false

**Redux Integration**:

- Reads `termination` state from `yearsEnd` slice for persisted values
- Dispatches `clearTermination()` on reset

**Unsaved Changes Handling**:

- Displays browser alert: "Please save your changes." if search attempted with unsaved edits
- Search button disabled when `hasUnsavedChanges` is true

**Lines of Code**: 228

---

### 3. TerminationGrid.tsx (Data Grid)

**Purpose**: Renders AG Grid with termination data, summary totals, and pagination.

**Key Features**:

- Displays summary totals in `TotalsGrid` components:
  - Amount in Profit Sharing
  - Vested Amount
  - Total Forfeitures
  - Total Beneficiary Allocations
- Report summary section via `ReportSummary` component
- Dynamic grid height calculation via `useDynamicGridHeight()` hook
- Read-only mode detection via `useReadOnlyNavigation()` hook
- Combines main columns (PSN, Name) with detail columns (all data fields)

**Hook Dependencies**:

- `useDynamicGridHeight()`: Calculates optimal grid height based on viewport
- `useReadOnlyNavigation()`: Determines if user has read-only permissions
- `useTerminationGrid()`: Encapsulates grid state, data fetching, selection state, and save handlers

**Grid Configuration**:

```typescript
{
  preferenceKey: "TERMINATION",          // Saves user preferences
  rowSelection: {
    mode: "multiRow",
    checkboxes: false,
    headerCheckbox: false,
    enableClickSelection: false
  },
  rowHeight: 40,
  suppressMultiSort: true,
  defaultColDef: { resizable: true }
}
```

**Column Assembly**:

- Retrieves main columns from `GetTerminationColumns()`
- Retrieves detail columns from `GetDetailColumns()` with callbacks for save operations
- Combines into flat array for AG Grid

**Read-Only Behavior**:

- Refreshes grid cells when read-only status changes (line 79-85)
- Forces cell renderers to re-read `isReadOnly` from context

**Lines of Code**: 182

---

### 4. TerminationGridColumns.tsx (Main Columns)

**Purpose**: Defines the two primary identification columns for the grid.

**Columns**:

1. **PSN (Profit Sharing Number)**:

   - Uses `createBadgeColumn()` factory
   - Header: "PSN"
   - Field: `psn`
   - No suffix needed (PSN already combined)

2. **Name**:
   - Uses `createNameColumn()` factory
   - Field: `name`

**Factory Functions Used**:

- `createBadgeColumn()`: Standardized badge column from `utils/gridColumnFactory`
- `createNameColumn()`: Standardized name column from grid factory

**Purpose**: Separates main columns from detail columns for modularity and reusability.

**Lines of Code**: 16

---

### 5. TerminationDetailsGridColumns.tsx (Detail Columns)

**Purpose**: Defines 14 detail columns including the editable "Suggested Forfeit" field and save button column.

**Column List**:

1. **Profit Year**: Year column (non-sortable)
2. **Beginning Balance**: Currency column
3. **Beneficiary Allocation**: Currency column
4. **Distribution Amount**: Currency column
5. **Forfeit Amount**: Currency column
6. **Ending Balance**: Currency column
7. **Vested Balance**: Currency column
8. **Vested %**: Percentage column (formatted as `XX%`)
9. **Term Date**: Date column
10. **YTD PS Hours**: Hours column
11. **Age**: Age column (max width 70px)
12. **Forfeited**: Yes/No column
13. **Suggested Forfeit**: **Editable currency column** (lines 92-129)
14. **Save Button**: Action column with individual/bulk save (lines 131-165)

**Editable Column - Suggested Forfeit**:

```typescript
{
  field: "suggestedForfeit",
  editable: ({ node }) => node.data.suggestedForfeit !== null,
  cellEditor: SuggestedForfeitEditor,
  cellRenderer: SuggestedForfeitCellRenderer,
  pinned: "right",
  valueGetter: (params) => {
    const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
    return params.context?.editedValues?.[rowKey]?.value ?? params.data.suggestedForfeit;
  },
  cellClass: (params) => {
    const hasError = params.context?.editedValues?.[rowKey]?.hasError;
    return hasError ? "bg-blue-50" : ""; // Blue highlight for errors
  }
}
```

**Key Mechanisms**:

- Uses AG Grid `context` to track edited values in memory (not persisted until save)
- Row key pattern: `${badgeNumber}-${profitYear}` for unique row identification
- Error highlighting via conditional CSS classes (`bg-blue-50` for errors)
- Null values render as empty cells (non-editable)
- Beneficiaries (PSN length > 7) do not show suggested forfeit (line 112)

**Save Button Column**:

- Uses `HeaderComponent` (bulk save header) - line 142
- Uses `createSaveButtonCellRenderer()` factory - line 160
- Pinned to right side, fixed width (130px), locked pinning
- Disabled in read-only mode
- Tracks loading state via `context.loadingRowIds` set
- `valueGetter` combines current value, loading state, and selection state into single string for change detection

**Factory Functions Used**:

- `createYearColumn()`
- `createCurrencyColumn()`
- `createDateColumn()`
- `createHoursColumn()`
- `createAgeColumn()`
- `createYesOrNoColumn()`
- `createSaveButtonCellRenderer()`

**Lines of Code**: 168

---

### 6. TerminationHeaderComponent.tsx (Bulk Save Header)

**Purpose**: Renders bulk save controls in the "Save Button" column header.

**Implementation**: Thin wrapper around `SharedForfeitHeaderComponent` configured for "termination" activity type.

```typescript
<SharedForfeitHeaderComponent
  {...params}
  config={{ activityType: "termination" }}
/>
```

**Props Interface**:

```typescript
interface HeaderComponentProps extends IHeaderParams {
  addRowToSelectedRows: (id: number) => void;
  removeRowFromSelectedRows: (id: number) => void;
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => Promise<void>;
  isBulkSaving?: boolean;
  isReadOnly?: boolean;
}
```

**Functionality**:

- Provides bulk selection checkboxes
- Enables bulk save operation for multiple rows
- Automatically disabled in read-only mode
- Shares common forfeit header component with other December activities (UnForfeit, etc.)

**Lines of Code**: 23

---

### 7. Claude.md (Architecture Documentation)

**Purpose**: Comprehensive technical architecture documentation for the Termination page.

**Contents**:

- Component hierarchy diagram
- Detailed component responsibilities
- Data flow diagrams (search flow, edit flow)
- State management patterns (Redux, local state, grid context)
- API integration details
- Dependencies and shared components

**Lines**: 480+ (documentation)

---

## Test Files

### 8. **test**/Termination.test.tsx

**Purpose**: Unit tests for the main Termination page component.

**Test Setup**:

- Mocks `useFiscalCalendarYear` hook to return fiscal date range
- Mocks `useTerminationState` hook to return state and actions
- Mocks `useUnsavedChangesGuard` hook
- Mocks Redux API hooks (`LookupsApi`, `NavigationStatusApi`)
- Uses `createMockStoreAndWrapper` for Redux provider

**Test Coverage** (partial listing from first 100 lines):

- Rendering tests
- Fiscal data loading states
- Search filter integration
- Grid integration
- Error handling behavior

**Lines of Code**: ~200+ (full file not shown)

---

### 9. **test**/TerminationSearchFilter.test.tsx

**Purpose**: Unit tests for the TerminationSearchFilter component.

**Test Setup**:

- Mocks `DsmDatePicker` component
- Mocks `DuplicateSsnGuard` component
- Mocks form validators (`FormValidators`)
- Mocks date utility functions
- Mocks `smart-ui-library` `SearchAndReset` component
- Mocks Redux API hooks

**Mock Strategies**:

- Uses `vi.hoisted()` for API mock functions to avoid hoisting issues
- Provides minimal implementations for date pickers and validators
- Mocks validators to return simple Yup schemas that pass validation

**Test Coverage** (partial listing from first 100 lines):

- Form rendering
- Date input validation
- Search submission
- Reset functionality
- Unsaved changes guard
- Error handling

**Lines of Code**: ~200+ (full file not shown)

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
On error: error message displayed, scroll to top
```

---

## State Management

### Redux State (yearsEnd slice)

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

### Local State (useTerminationState hook)

```typescript
state: {
  hasUnsavedChanges: boolean,
  initialSearchLoaded: boolean,
  searchParams: TerminationSearchRequest | null,
  resetPageFlag: boolean,
  shouldArchive: boolean
}

actions: {
  handleSearch: (params: TerminationSearchRequest) => void,
  handleStatusChange: (status: string) => void,
  handleUnsavedChanges: (hasChanges: boolean) => void,
  setInitialSearchLoaded: (loaded: boolean) => void,
  handleArchiveHandled: () => void
}
```

### Grid Context (AG Grid)

```typescript
context: {
  editedValues: Record<string, { value: number, hasError: boolean }>,
  loadingRowIds: Set<string>,
  selectedProfitYear: number,
  isReadOnly: boolean
}
```

---

## Key Dependencies

### Custom Hooks

- `useLazyGetAccountingRangeToCurrent()` - Fetches fiscal calendar dates
- `useTerminationState()` - Centralized state management
- `useUnsavedChangesGuard()` - Navigation guard for unsaved changes
- `useDynamicGridHeight()` - Calculates optimal grid height
- `useReadOnlyNavigation()` - Detects read-only mode
- `useTerminationGrid()` - Grid state and API integration
- `useDecemberFlowProfitYear()` - Gets current profit year for December activities

### Shared Components

- `StatusDropdownActionNode` - Page status dropdown
- `DsmDatePicker` - Date picker with fiscal year constraints
- `DuplicateSsnGuard` - Data quality validation guard
- `SuggestedForfeitEditor` / `SuggestedForfeitCellRenderer` - Custom cell editor/renderer
- `SharedForfeitHeaderComponent` - Reusable bulk save header
- `createSaveButtonCellRenderer()` - Save button cell renderer factory

### External Libraries

- `react-hook-form` - Form state management
- `yup` - Form validation schema
- `@hookform/resolvers` - Yup resolver for react-hook-form
- `ag-grid-community` - Grid functionality
- `smart-ui-library` - UI component library (Page, DSMGrid, DSMAccordion, etc.)
- `@mui/material` - Material-UI components (Grid, Button, Checkbox, etc.)

---

## API Integration

**RTK Query Hooks** (inferred from useTerminationGrid):

- `useLazyGetTerminationsQuery()` - Searches termination records by date range
- `useUpdateForfeitureAdjustmentMutation()` - Saves individual forfeit adjustment
- `useBulkUpdateForfeitureAdjustmentMutation()` - Saves multiple forfeit adjustments

**Request Types**:

```typescript
TerminationSearchRequest extends StartAndEndDateRequest {
  forfeitureStatus: string;
  archive?: boolean;
  excludeZeroBalance?: boolean;
  excludeZeroAndFullyVested?: boolean;
}

ForfeitureAdjustmentUpdateRequest {
  badgeNumber: string;
  profitYear: number;
  suggestedForfeit: number;
  // ... other fields
}
```

---

## Design Patterns

1. **Search-Filter-Grid Pattern**: Standard pattern for data exploration pages
2. **Custom Hook State Management**: Encapsulates complex state logic in reusable hooks
3. **Factory Functions**: Standardized column definitions via grid factory functions
4. **Context-Based Editing**: Uses AG Grid context to track in-memory edits before persistence
5. **Bulk Operations**: Shared header component enables multi-row operations
6. **Read-Only Mode**: Centralized read-only detection disables editing across all components
7. **Unsaved Changes Guard**: Prevents navigation and search when unsaved changes exist
8. **Error Highlighting**: Visual feedback (blue background) for validation errors in editable cells

---

## Performance Considerations

- **Dynamic Grid Height**: Automatically adjusts to viewport to prevent unnecessary scrolling
- **Column Memoization**: Uses `useMemo()` to prevent unnecessary column recalculation
- **Conditional Rendering**: Only renders grid after search is performed
- **In-Memory Editing**: Edits stored in grid context until save, reducing API calls
- **Bulk Operations**: Saves multiple rows in single API call
- **Grid Preferences**: Saves column widths/order to user preferences (key: "TERMINATION")

---

## Total Lines of Code

| File                              | Lines       |
| --------------------------------- | ----------- |
| Termination.tsx                   | 122         |
| TerminationSearchFilter.tsx       | 228         |
| TerminationGrid.tsx               | 182         |
| TerminationGridColumns.tsx        | 16          |
| TerminationDetailsGridColumns.tsx | 168         |
| TerminationHeaderComponent.tsx    | 23          |
| Termination.test.tsx              | ~200+       |
| TerminationSearchFilter.test.tsx  | ~200+       |
| **Total**                         | **~1,139+** |

---

## Related Documentation

- **Page Component Architecture**: `src/ui/src/pages/CLAUDE.md`
- **Grid Column Factory**: `src/ui/src/utils/gridColumnFactory.ts`
- **Termination Architecture**: `src/ui/src/pages/DecemberActivities/Termination/Claude.md`
- **Redux Store Configuration**: `src/ui/src/reduxstore/store.ts`
- **Frontend Unit Testing Guide**: `ai-templates/front-end/fe-unit-tests.md`
