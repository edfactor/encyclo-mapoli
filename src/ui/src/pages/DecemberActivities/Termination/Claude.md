# Termination Feature - Technical Implementation Guide

## Overview

The Termination feature is a December year-end activity that allows users to process forfeitures for terminated employees. It implements a master-detail grid pattern with inline editing, bulk save operations, comprehensive state management, financial summaries, and real-time validation.

**Purpose**: Process profit-sharing forfeitures for employees who have been terminated within a specified date range during the year-end closing process.

**Location**: `/src/ui/src/pages/DecemberActivities/Termination/`

---

## Architecture Summary

### Component Hierarchy

```
Termination.tsx (Page Container)
├── ApiMessageAlert (Error/success messages with scroll-to-top)
├── DSMAccordion (Collapsible filter section)
│   └── TerminationSearchFilter
│       ├── DsmDatePicker (Begin Date)
│       ├── DsmDatePicker (End Date)
│       ├── DuplicateSsnGuard (Prerequisite validation)
│       └── SearchAndReset (Action buttons with loading state)
└── TerminationGrid
    ├── ReportSummary (Total record counts)
    ├── TotalsGrid × 4 (Financial summaries - sticky header)
    │   ├── Amount in Profit Sharing
    │   ├── Vested Amount
    │   ├── Total Forfeitures
    │   └── Total Beneficiary Allocations
    ├── DSMGrid (AG Grid wrapper)
    │   ├── Expansion column (►/▼)
    │   ├── Main row columns (badge, name)
    │   └── Detail row columns (12 columns with inline editing)
    └── Pagination
```

### Key Design Patterns

1. **Master-Detail Grid**: Employees (master) expand to show profit year records (detail)
2. **Inline Editing**: Direct cell editing for "Suggested Forfeit" amounts
3. **Bulk Operations**: Select multiple rows and save them simultaneously
4. **Shared Components**: Uses refactored shared components from `/src/ui/src/components/ForfeitActivities/`
5. **State Lifting**: Loading states lifted from grid hook to parent for search button feedback
6. **Read-Only Mode**: Comprehensive support for read-only navigation contexts

---

## File Structure

### 1. **Termination.tsx** (Page Container - 117 lines)

**Purpose**: Main orchestrator for the Termination feature

**Key Responsibilities**:

- Page layout and structure using `Page` component from smart-ui-library
- Fiscal calendar data fetching
- Status change handling via `StatusDropdownActionNode`
- Error message display with automatic scroll-to-top
- Navigation guard for unsaved changes
- Loading state management for search button feedback

**State Management**:

```typescript
const { state, actions } = useTerminationState();
// state.searchParams - Current search parameters
// state.hasUnsavedChanges - Tracks pending edits
// state.initialSearchLoaded - Whether first search performed
// state.resetPageFlag - Triggers pagination reset
// state.shouldArchive - Archive mode flag
```

**Key Hooks Used**:

- `useLazyGetAccountingRangeToCurrent(6)` - Fetches fiscal calendar data
- `useTerminationState()` - Page-level state management
- `useUnsavedChangesGuard()` - Blocks navigation with unsaved changes

**Error Scroll-to-Top Pattern**:

```typescript
useEffect(() => {
  const handleMessageEvent = (event: Event) => {
    const customEvent = event as CustomEvent;
    if (customEvent.detail?.key === "TerminationSave" && customEvent.detail?.message?.type === "error") {
      scrollToTop();
    }
  };

  window.addEventListener("dsmMessage", handleMessageEvent);
  return () => window.removeEventListener("dsmMessage", handleMessageEvent);
}, [scrollToTop]);
```

**Loading State Flow**:

- Parent component tracks `isFetching` state via `useState`
- Passes `setIsFetching` to `TerminationGrid` via `onLoadingChange` prop
- Passes `isFetching` to `TerminationSearchFilter` for button state

**Props Flow**:

```typescript
// To TerminationSearchFilter
<TerminationSearchFilter
  fiscalData={fiscalData}
  onSearch={actions.handleSearch}
  setInitialSearchLoaded={actions.setInitialSearchLoaded}
  hasUnsavedChanges={state.hasUnsavedChanges}
  isFetching={isFetching} // Disables search button
/>

// To TerminationGrid
<TerminationGrid
  initialSearchLoaded={state.initialSearchLoaded}
  searchParams={state.searchParams}
  resetPageFlag={state.resetPageFlag}
  onUnsavedChanges={actions.handleUnsavedChanges}
  hasUnsavedChanges={state.hasUnsavedChanges}
  fiscalData={fiscalData}
  shouldArchive={state.shouldArchive}
  onArchiveHandled={actions.handleArchiveHandled}
  onErrorOccurred={scrollToTop}
  onLoadingChange={setIsFetching} // Receives loading state
/>
```

---

### 2. **TerminationSearchFilter.tsx** (Search Form - 203 lines)

**Purpose**: Date-based search filter with validation

**Key Features**:

- Date range selection (begin/end dates within fiscal year)
- Form validation using React Hook Form + Yup
- Duplicate SSN prerequisite check via `DuplicateSsnGuard`
- Loading state feedback on search button

**Validation Schema**:

```typescript
const schema = yup.object().shape({
  beginningDate: dateStringValidator(2000, 2099, "Beginning Date").required(),
  endingDate: endDateStringAfterStartDateValidator(
    "beginningDate",
    tryddmmyyyyToDate,
    "Ending date must be the same or after the beginning date"
  ).required(),
  forfeitureStatus: yup.string().required(),
  profitYear: profitYearValidator(2015, 2099)
});
```

**Date Range Constraints**:

- Begin Date: Min = fiscal begin, Max = fiscal end
- End Date: Min = max(begin date, fiscal begin), Max = fiscal end
- Both dates use `DsmDatePicker` with `disableFuture`

**Duplicate SSN Guard Pattern**:

```typescript
<DuplicateSsnGuard mode="warning">
  {({ prerequisitesComplete }) => (
    <SearchAndReset
      handleReset={handleReset}
      handleSearch={validateAndSearch}
      isFetching={isFetching}
      disabled={!isValid || !prerequisitesComplete || isFetching}
    />
  )}
</DuplicateSsnGuard>
```

**Purpose**: Ensures no duplicate SSNs exist before allowing search. Critical for data integrity during financial processing.

**Validation and Submit**:

```typescript
const validateAndSubmit = async (data: TerminationSearchRequest) => {
  if (hasUnsavedChanges) {
    alert("Please save your changes.");
    return;
  }

  const params = {
    ...data,
    profitYear: selectedProfitYear,
    beginningDate: mmDDYYFormat(data.beginningDate),
    endingDate: mmDDYYFormat(data.endingDate)
  };

  onSearch(params); // Triggers state update in parent
  setInitialSearchLoaded(true);
};
```

**Reset Functionality**:

- Clears all form fields
- Resets to fiscal year defaults
- Clears Redux termination state
- Marks search as not loaded

---

### 3. **TerminationGrid.tsx** (Grid Display - 252 lines)

**Purpose**: Master-detail grid with financial summaries and inline editing

**Key Features**:

- Dynamic grid height calculation
- Master-detail row expansion/collapse
- Four sticky financial summary grids
- Read-only mode support
- Inline editing for current year only
- Pagination

**Financial Summary Totals** (Sticky Header):

```typescript
<div className="sticky top-0 z-10 flex bg-white">
  <TotalsGrid
    displayData={[[numberToCurrency(termination.totalEndingBalance || 0)]]}
    leftColumnHeaders={["Amount in Profit Sharing"]}
  />
  <TotalsGrid
    displayData={[[numberToCurrency(termination.totalVested || 0)]]}
    leftColumnHeaders={["Vested Amount"]}
  />
  <TotalsGrid
    displayData={[[numberToCurrency(termination.totalForfeit || 0)]]}
    leftColumnHeaders={["Total Forfeitures"]}
  />
  <TotalsGrid
    displayData={[[numberToCurrency(termination.totalBeneficiaryAllocation || 0)]]}
    leftColumnHeaders={["Total Beneficiary Allocations"]}
  />
</div>
```

**Purpose**: Provides real-time financial oversight. Users can verify individual forfeitures sum to displayed totals.

**Hook Integration**:

```typescript
const {
  pageNumber,
  pageSize,
  gridData,
  termination,
  selectedProfitYear,
  selectionState,
  handleSave,
  handleBulkSave,
  handleRowExpansion,
  sortEventHandler,
  onGridReady,
  paginationHandlers,
  gridRef,
  gridContext
} = useTerminationGrid({
  initialSearchLoaded,
  setInitialSearchLoaded,
  searchParams,
  resetPageFlag,
  onUnsavedChanges,
  hasUnsavedChanges,
  fiscalData,
  shouldArchive,
  onArchiveHandled,
  onErrorOccurred,
  onLoadingChange // NEW: Notifies parent of loading state
});
```

**Master-Detail Column Composition**:

```typescript
const columnDefs = useMemo(() => {
  // Expansion column (►/▼)
  const expansionColumn = { ... };

  // Main columns: Hide content for detail rows (unless shared)
  const visibleColumns = mainColumns.map(column => ({
    ...column,
    cellRenderer: (params) => {
      if (params.data?.isDetail) {
        const hideInDetails = !detailColumns.some(
          col => col.field === column.field
        );
        if (hideInDetails) return "";
      }
      return /* render value */;
    }
  }));

  // Detail-only columns: Only visible for detail rows
  const detailOnlyColumns = detailColumns
    .filter(col => !mainColumns.some(mainCol => mainCol.field === col.field))
    .map(column => ({
      ...column,
      cellRenderer: (params) => !params.data?.isDetail ? "" : /* value */
    }));

  return [expansionColumn, ...visibleColumns, ...detailOnlyColumns];
}, [mainColumns, detailColumns, handleRowExpansion]);
```

**Row Styling**:

```typescript
const getRowClass = (params: { data: { isDetail: boolean } }) => {
  return params.data.isDetail ? "bg-gray-100" : "";
};
```

Detail rows have gray background (`bg-gray-100` Tailwind class) for visual distinction.

**Grid Context** (Shared State):

```typescript
context: {
  editedValues: Record<string, { value: number, hasError?: boolean }>,
  loadingRowIds: Set<string>
}
```

Allows cell renderers to access:

- Currently edited values (before save)
- Loading state for individual rows
- Validation errors

---

### 4. **TerminationGridColumns.tsx** (Main Columns - 15 lines)

**Purpose**: Column definitions for master (employee summary) rows

**Minimal Master Columns** (Design Decision):
Unlike UnForfeit, Termination only shows badge and name in master rows. All financial details are in detail rows for cleaner presentation.

```typescript
export const GetTerminationColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      psnSuffix: true // Shows Person Number identifier
    }),
    createNameColumn({
      field: "name"
    })
  ];
};
```

**Column Factories** (from `/src/ui/src/utils/gridColumnFactory`):

- `createBadgeColumn()` - Badge number with PSN suffix, navigation support
- `createNameColumn()` - Employee full name

**PSN Suffix**: Additional person identifier displayed after badge number when available.

---

### 5. **TerminationDetailsGridColumns.tsx** (Detail Columns - 162 lines)

**Purpose**: Column definitions for detail (profit year) rows with inline editing

**Column Structure** (12 total columns):

| Column                 | Field                   | Type     | Editable  | Description               |
| ---------------------- | ----------------------- | -------- | --------- | ------------------------- |
| Profit Year            | `profitYear`            | Year     | No        | Year of profit record     |
| Beginning Balance      | `beginningBalance`      | Currency | No        | Balance at year start     |
| Beneficiary Allocation | `beneficiaryAllocation` | Currency | No        | Beneficiary distributions |
| Distribution Amount    | `distributionAmount`    | Currency | No        | Amount distributed        |
| Forfeit Amount         | `forfeit`               | Currency | No        | Previously forfeited      |
| Ending Balance         | `endingBalance`         | Currency | No        | Balance at year end       |
| Vested Balance         | `vestedBalance`         | Currency | No        | Vested portion            |
| Vested %               | `vestedPercent`         | Percent  | No        | Vested percentage         |
| Term Date              | `dateTerm`              | Date     | No        | Termination date          |
| YTD PS Hours           | `ytdPsHours`            | Hours    | No        | Year-to-date hours        |
| Age                    | `age`                   | Number   | No        | Age at termination        |
| Forfeited              | `hasForfeited`          | Yes/No   | No        | Forfeiture processed      |
| **Suggested Forfeit**  | `suggestedForfeit`      | Currency | **Yes\*** | Forfeit amount (editable) |
| Save Button            | (special)               | Action   | N/A       | Checkbox + Save           |

**\*Editable only for current year**: `node.data.profitYear === selectedProfitYear`

**Suggested Forfeit Column** (Inline Editing):

```typescript
{
  headerName: "Suggested Forfeit",
  field: "suggestedForfeit",
  pinned: "right",
  editable: ({ node }) =>
    node.data.isDetail && node.data.profitYear === selectedProfitYear,
  cellEditor: SuggestedForfeitEditor,
  cellRenderer: (params) => SuggestedForfeitCellRenderer(
    { ...params, selectedProfitYear },
    true,  // isTermination = true
    false  // isUnforfeit = false
  ),
  valueGetter: (params) => {
    if (!params.data.isDetail) return params.data.suggestedForfeit;
    const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
    const editedValue = params.context?.editedValues?.[rowKey]?.value;
    return editedValue ?? params.data.suggestedForfeit ?? 0;
  },
  cellClass: (params) => {
    if (!params.data.isDetail) return "";
    const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
    const hasError = params.context?.editedValues?.[rowKey]?.hasError;
    return hasError ? "bg-red-50" : ""; // Error highlighting
  }
}
```

**Row Key Pattern**: `${badgeNumber}-${profitYear}` (Composite key)

- Unique identifier for each detail row
- Used to track edited values before save
- Different from UnForfeit (which uses `profitDetailId`)

**Save Button Column** (Refactored):

```typescript
{
  headerName: "Save Button",
  field: "saveButton",
  pinned: "right",
  lockPinned: true,
  headerComponent: HeaderComponent, // Bulk save checkbox
  cellRenderer: createSaveButtonCellRenderer({
    activityType: "termination",
    selectedProfitYear,
    isReadOnly
  })
}
```

**Uses Shared Component**: `createSaveButtonCellRenderer` from `/src/ui/src/components/ForfeitActivities/`

**Features**:

- Checkbox for bulk selection
- Individual save button with loading spinner
- Disabled when value is zero, invalid, or read-only
- Tooltips for disabled states

---

### 6. **TerminationHeaderComponent.tsx** (Bulk Save Header - 22 lines)

**Purpose**: Custom header for save button column with bulk save functionality

**Refactored Implementation** (Uses Shared Component):

```typescript
export const HeaderComponent: React.FC<HeaderComponentProps> = (params) => {
  return (
    <SharedForfeitHeaderComponent
      {...params}
      config={{
        activityType: "termination"
      }}
    />
  );
};
```

**Uses**: `SharedForfeitHeaderComponent` from `/src/ui/src/components/ForfeitActivities/`

**Shared Component Handles**:

- Bulk save checkbox rendering
- Row eligibility determination (`isNodeEligible`)
- Payload creation (`createUpdatePayload`)
- Loading state tracking (`hasSavingInProgress`)

**Activity-Specific Behavior** (Termination):

- **Row Key**: Composite `${badgeNumber}-${profitYear}`
- **Value Transformation**: No negation (sent as-is to API)
- **Eligibility**: Only current year detail rows with non-zero values
- **No `offsettingProfitDetailId`**: Not needed for new forfeitures

---

## Data Flow

### Search Flow

```
User clicks "SEARCH"
    ↓
TerminationSearchFilter validates form (React Hook Form + Yup)
    ↓
actions.handleSearch(params) called
    ↓
useTerminationState reducer updates searchParams and toggles resetPageFlag
    ↓
TerminationGrid receives new searchParams via props
    ↓
useTerminationGrid detects searchParams change
    ↓
useEffect triggers triggerSearch() (RTK Query lazy query)
    ↓
API request to /api/terminations
    ↓
Response cached by RTK Query
    ↓
flattenMasterDetailData() transforms hierarchical data to flat structure
    ↓
Grid displays master rows (badge, name)
    ↓
User clicks expansion arrow (►)
    ↓
handleRowExpansion() updates expandedRows state
    ↓
flattenMasterDetailData() re-runs with updated expansion state
    ↓
Detail rows inserted after master row
    ↓
Grid displays detail rows (profit years with gray background)
```

### Save Flow (Individual)

```
User edits "Suggested Forfeit" cell
    ↓
SuggestedForfeitEditor captures value
    ↓
editedValues context updated: editedValues[rowKey] = { value: newValue }
    ↓
onUnsavedChanges(true) - Enables navigation guard
    ↓
Cell re-renders with edited value (from context)
    ↓
User clicks individual save button
    ↓
handleSave() called from useTerminationGrid
    ↓
prepareSaveRequest() creates ForfeitureAdjustmentUpdateRequest
    ↓
Value NOT negated (termination sends positive values)
    ↓
updateForfeiture mutation (RTK Query)
    ↓
API request to /api/forfeiture-adjustments
    ↓
Success:
  - generateSaveSuccessMessage() creates message
  - Remove edited value from context
  - Trigger data refresh (re-fetch)
  - Clear row selection
  - Display success message via ApiMessageAlert
  - onUnsavedChanges(false)
    ↓
Error:
  - Display error message via ApiMessageAlert
  - Scroll to top (via window event listener)
  - Keep edited value in context
  - Row remains selected
  - onUnsavedChanges remains true
```

### Bulk Save Flow

```
User edits multiple cells
    ↓
Each edit tracked in editedValues context
    ↓
User clicks checkboxes for rows to save
    ↓
selectedRowIds Set updated via selectionState
    ↓
User clicks bulk save checkbox in header
    ↓
handleBulkSave() called from useTerminationGrid
    ↓
Filter selected rows using isNodeEligible():
  - Must be detail row (isDetail === true)
  - Must be current year (profitYear === selectedProfitYear)
  - Must have non-zero value
    ↓
Create payloads using createUpdatePayload() for each eligible row
    ↓
Execute saves with handleBatchOperations() (batches of 5)
    ↓
For each row in batch:
  - Mark as loading (add badgeNumber to loadingRowIds)
  - Call updateForfeiture mutation
  - Handle success/error individually
  - Remove from loading set
    ↓
After all complete:
  - Display summary message (X succeeded, Y failed)
  - Refresh grid data
  - Clear all selections
  - Clear all edited values
  - onUnsavedChanges(false)
```

### Archive Flow

```
User changes status to "Complete"
    ↓
StatusDropdownActionNode calls handleStatusChange("complete", "Complete")
    ↓
useTerminationState reducer:
  - Detects status contains "complete"
  - Sets archiveMode = true
  - Sets shouldArchive = true
  - Adds archive: true to searchParams
  - Toggles resetPageFlag
    ↓
TerminationGrid detects searchParams change
    ↓
Search executes with archive flag
    ↓
API returns archived (historical) data
    ↓
Grid displays archived termination records
    ↓
User changes status away from "Complete"
    ↓
useTerminationState reducer:
  - Removes archive flag from searchParams
  - Sets archiveMode = false
  - Toggles resetPageFlag
    ↓
Search re-executes without archive flag
    ↓
Grid displays current (non-archived) data
```

### Loading State Flow (NEW)

```
User clicks "SEARCH" button
    ↓
TerminationSearchFilter triggers validateAndSearch()
    ↓
actions.handleSearch(params) called
    ↓
TerminationGrid receives searchParams update
    ↓
useTerminationGrid triggers triggerSearch()
    ↓
isFetching becomes true (RTK Query state)
    ↓
useEffect in useTerminationGrid detects isFetching change
    ↓
onLoadingChange(true) called
    ↓
setIsFetching(true) in Termination.tsx parent component
    ↓
isFetching prop passed to TerminationSearchFilter
    ↓
SearchAndReset component:
  - Disables search button
  - Shows spinner
    ↓
API request completes
    ↓
isFetching becomes false
    ↓
useEffect triggers onLoadingChange(false)
    ↓
setIsFetching(false)
    ↓
SearchAndReset component:
  - Enables search button
  - Hides spinner
```

---

## State Management

### Local Component State (Termination.tsx)

```typescript
const [isFetching, setIsFetching] = useState(false);
```

**Purpose**: Track search loading state for button feedback
**Updated by**: `TerminationGrid` via `onLoadingChange` callback
**Used by**: `TerminationSearchFilter` to disable/enable search button

### Page-Level State (useTerminationState)

**Hook Location**: `/src/ui/src/hooks/useTerminationState.ts`

**State Structure**:

```typescript
interface TerminationState {
  searchParams: TerminationSearchRequest | null;
  initialSearchLoaded: boolean;
  hasUnsavedChanges: boolean;
  resetPageFlag: boolean;
  currentStatus: string | null;
  archiveMode: boolean;
  shouldArchive: boolean;
}
```

**Actions**:

- `handleSearch(params)` - Update search params, toggle reset flag
- `handleUnsavedChanges(hasChanges)` - Track unsaved edits
- `handleStatusChange(status, statusName)` - Handle status change, trigger archive
- `handleArchiveHandled()` - Clear archive flag after processing
- `setInitialSearchLoaded(loaded)` - Mark search as performed

**Archive Mode Logic**:

```typescript
case "SET_STATUS_CHANGE": {
  const { statusName } = action.payload;
  const isCompleteLike = (statusName ?? "").toLowerCase().includes("complete");
  const isChangingToComplete = isCompleteLike && state.currentStatus !== statusName;

  if (isChangingToComplete) {
    // Add archive flag to search params
    const updatedSearchParams = state.searchParams
      ? { ...state.searchParams, archive: true }
      : null;

    return {
      ...state,
      currentStatus: statusName || null,
      archiveMode: true,
      shouldArchive: true,
      searchParams: updatedSearchParams,
      resetPageFlag: !state.resetPageFlag
    };
  }
  // ... handle other cases
}
```

### Grid State (useTerminationGrid)

**Hook Location**: `/src/ui/src/hooks/useTerminationGrid.ts`

**Key State Variables**:

```typescript
// Expansion state
const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});

// Message pending state
const [pendingSuccessMessage, setPendingSuccessMessage] = useState<string | null>(null);
const [isPendingBulkMessage, setIsPendingBulkMessage] = useState<boolean>(false);
const [isBulkSaveInProgress, setIsBulkSaveInProgress] = useState<boolean>(false);

// RTK Query state
const [triggerSearch, { isFetching }] = useLazyGetTerminationReportQuery();

// Edit state (from useEditState hook)
editState: {
  editedValues: Record<string, { value: number, hasError?: boolean }>,
  loadingRowIds: Set<string>
}

// Selection state (from useRowSelection hook)
selectionState: {
  selectedRowIds: Set<number>,
  addRowToSelection: (id: number) => void,
  removeRowFromSelection: (id: number) => void,
  clearSelections: () => void
}

// Pagination state (from useGridPagination hook)
{
  pageNumber: number,
  pageSize: number,
  sortParams: SortParams,
  setPageNumber: (page: number) => void,
  setPageSize: (size: number) => void
}
```

**Composite Hooks Pattern**:
The grid hook composes multiple smaller hooks for separation of concerns:

- `useEditState()` - Inline editing and validation
- `useRowSelection()` - Checkbox selection tracking
- `useGridPagination()` - Pagination and sorting

### Redux State (yearsEndSlice)

```typescript
termination: {
  startDate: string,
  endDate: string,
  response: {
    results: TerminationMasterDto[],
    total: number
  },
  totalEndingBalance: number,
  totalVested: number,
  totalForfeit: number,
  totalBeneficiaryAllocation: number
}
```

**Actions**:

- `clearTermination()` - Clear termination search results

**Why Store in Redux?**

- Persist search results across component re-renders
- Support browser refresh without losing context
- Share financial totals across components

---

## API Integration

### Search Endpoint

**RTK Query Hook**: `useLazyGetTerminationReportQuery()`

**Request**:

```typescript
interface TerminationSearchRequest {
  beginningDate: string; // Format: "MM/DD/YYYY"
  endingDate: string; // Format: "MM/DD/YYYY"
  forfeitureStatus: string; // "showAll", "forfeited", "notForfeited"
  profitYear: number;
  archive?: boolean; // Include archived data
  pagination: {
    skip: number;
    take: number;
    sortBy: string;
    isSortDescending: boolean;
  };
}
```

**Response**:

```typescript
interface TerminationResponse {
  response: {
    results: TerminationMasterDto[];
    total: number;
  };
  totalEndingBalance: number; // Sum of all ending balances
  totalVested: number; // Sum of vested balances
  totalForfeit: number; // Sum of forfeitures
  totalBeneficiaryAllocation: number; // Sum of beneficiary distributions
}

interface TerminationMasterDto {
  badgeNumber: number;
  name: string;
  profitYears: TerminationDetailDto[];
}

interface TerminationDetailDto {
  profitYear: number;
  beginningBalance: number;
  beneficiaryAllocation: number;
  distributionAmount: number;
  forfeit: number;
  endingBalance: number;
  vestedBalance: number;
  vestedPercent: number;
  dateTerm: string;
  ytdPsHours: number;
  age: number;
  hasForfeited: boolean;
  suggestedForfeit: number;
}
```

### Update Endpoint

**RTK Query Hook**: `useUpdateForfeitureAdjustmentMutation()`

**Request**:

```typescript
interface ForfeitureAdjustmentUpdateRequest {
  badgeNumber: number;
  profitYear: number;
  forfeitureAmount: number; // NOT negated for termination
  classAction: boolean; // Always false for manual operations
  offsettingProfitDetailId?: number; // Undefined for termination
}
```

**Critical**: Termination does NOT negate the value. The amount is sent as entered by the user.

**Response**: Success/error status

### Bulk Update Endpoint

**RTK Query Hook**: `useUpdateForfeitureAdjustmentBulkMutation()`

**Request**: Array of `ForfeitureAdjustmentUpdateRequest`

**Response**: Success/error status

---

## Key Features

### 1. Financial Summary Totals (Sticky Header)

**Display**: Four `TotalsGrid` components above main grid showing:

- Amount in Profit Sharing (total ending balance)
- Vested Amount (total vested balance)
- Total Forfeitures (sum of forfeitures)
- Total Beneficiary Allocations (sum of distributions to beneficiaries)

**Styling**: `sticky top-0 z-10` - Remains visible when scrolling

**Purpose**:

- Financial oversight and validation
- Users can verify individual forfeitures sum to displayed totals
- Real-time updates as data changes

### 2. Inline Editing (Current Year Only)

**Editable Column**: "Suggested Forfeit"

**Editable Criteria**:

```typescript
editable: ({ node }) => node.data.isDetail && node.data.profitYear === selectedProfitYear;
```

**Implementation**:

- Uses `SuggestedForfeitEditor` component
- Values stored in grid context: `editedValues[rowKey] = { value }`
- `valueGetter` checks context first, then original data
- Changes trigger `onUnsavedChanges(true)`

**Validation**:

- Cannot save zero values
- Cannot edit in read-only mode
- Error highlighting: `bg-red-50` class for cells with errors

**Persistence**:

- Edited values persist across pagination
- Cleared on successful save
- Cleared on reset

### 3. Bulk Operations

**Selection Mechanism**:

- Checkboxes in save button column (detail rows only)
- Bulk checkbox in column header
- Selection tracked in `selectedRowIds` Set

**Eligibility Criteria**:

```typescript
isNodeEligible: (nodeData) => {
  if (!nodeData.isDetail || nodeData.profitYear !== selectedProfitYear) return false;
  const currentValue = getCurrentValue(nodeData, context);
  return (currentValue || 0) !== 0;
};
```

**Batch Processing**:

- Processes in batches of 5 (configurable)
- Sequential execution with `Promise.allSettled()`
- Individual row loading indicators
- Summary message after completion

**Progress Tracking**:

- `loadingRowIds` Set tracks currently saving rows
- Save buttons show `CircularProgress` spinner
- Bulk checkbox disabled during operation

### 4. Value Transformation (Critical)

**Termination Pattern**: **No negation**

```typescript
// User enters: 2000.00
// API receives: 2000.00 (no transformation)

transformForfeitureValue("termination", 2000.0); // Returns: 2000.00
```

**Why?**

- Forfeitures are positive values representing money leaving employee accounts
- UnForfeit negates (reverses) forfeitures with negative offsetting entries
- Termination creates new forfeitures, so values stay positive

**Contrast with UnForfeit**:

```typescript
transformForfeitureValue("unforfeit", 1000.0); // Returns: -1000.00
```

### 5. Row Keys and Tracking

**Termination uses composite row key**: `${badgeNumber}-${profitYear}`

```typescript
generateRowKey(
  { type: "termination" },
  {
    badgeNumber: 12345,
    profitYear: 2024
  }
);
// Returns: "12345-2024"
```

**Why Composite Key?**

- Each employee can have multiple profit years
- Badge number alone not unique across years
- Composite key uniquely identifies each detail row

**No `offsettingProfitDetailId`**: Not needed for new forfeitures (only for reversals)

**Usage**:

```typescript
// In editedValues context
const rowKey = `${badgeNumber}-${profitYear}`;
context.editedValues[rowKey] = { value: 2000.00 };

// In API request
{
  badgeNumber: 12345,
  profitYear: 2024,
  forfeitureAmount: 2000.00,
  classAction: false
  // No offsettingProfitDetailId
}
```

### 6. Read-Only Mode

**Hook**: `useReadOnlyNavigation()`

**Returns**: `boolean` - True if current navigation requires read-only access

**Effects when read-only**:

- ✗ Editing disabled (`editable` returns false)
- ✗ Checkboxes disabled
- ✗ Save buttons disabled
- ✗ Tooltips explain read-only mode
- ✗ Status dropdown disabled

**Implementation**:

```typescript
const isReadOnly = useReadOnlyNavigation();

// In column definitions
editable: ({ node }) =>
  node.data.isDetail &&
  node.data.profitYear === selectedProfitYear &&
  !isReadOnly

// In cell renderer
<IconButton disabled={isDisabled || isReadOnly}>
  {isLoading ? <CircularProgress /> : <SaveOutlined />}
</IconButton>
```

### 7. Unsaved Changes Guard

**Hook**: `useUnsavedChangesGuard(hasUnsavedChanges)`

**Behavior**:

- Blocks navigation when `hasUnsavedChanges === true`
- Shows browser confirmation: "You have unsaved changes. Are you sure you want to leave?"
- Applies to:
  - React Router navigation
  - Browser back/forward buttons
  - Browser refresh
  - Tab close

**Tracking**:

```typescript
// Set when user edits any value
onUnsavedChanges(true);

// Cleared when:
// 1. Successful save (individual or bulk)
onUnsavedChanges(false);

// 2. User clicks reset
handleReset() => setInitialSearchLoaded(false);

// 3. User confirms navigation via browser dialog
```

### 8. Archive Mode

**Trigger**: Status change to "Complete" (or any status name containing "complete")

**Flow**:

1. User changes status via `StatusDropdownActionNode`
2. `handleStatusChange()` detects "complete" status
3. Reducer adds `archive: true` to searchParams
4. Toggles `resetPageFlag` to trigger search
5. Search executes with archive flag
6. Grid displays archived (historical) data

**Purpose**: View historical termination data when page marked complete. Allows reviewing past terminations without affecting current processing.

**Exiting Archive Mode**: When status changes away from "Complete", archive flag removed and search re-executes.

### 9. Duplicate SSN Guard

**Component**: `DuplicateSsnGuard` in TerminationSearchFilter

**Purpose**: Prevent searching when duplicate SSNs exist in system

**Implementation**:

```typescript
<DuplicateSsnGuard mode="warning">
  {({ prerequisitesComplete }) => (
    <SearchAndReset
      disabled={!isValid || !prerequisitesComplete || isFetching}
    />
  )}
</DuplicateSsnGuard>
```

**Behavior**:

- Checks for duplicate SSNs on mount
- Disables search button if duplicates found
- Displays warning message with details
- Must be resolved before terminations can be processed

**Data Integrity**: Critical safeguard before processing financial transactions

### 10. Error Scroll-to-Top

**Implementation**: Custom window event listener in `Termination.tsx`

```typescript
useEffect(() => {
  const handleMessageEvent = (event: Event) => {
    const customEvent = event as CustomEvent;
    if (customEvent.detail?.key === "TerminationSave" && customEvent.detail?.message?.type === "error") {
      scrollToTop();
    }
  };

  window.addEventListener("dsmMessage", handleMessageEvent);
  return () => window.removeEventListener("dsmMessage", handleMessageEvent);
}, [scrollToTop]);
```

**Purpose**: When save fails, error message appears at top via `ApiMessageAlert`. Listener ensures user sees error by scrolling to top.

**Event Type**: Uses `dsmMessage` custom event dispatched by message system

### 11. Loading State Feedback (NEW)

**Feature**: Search button disables and shows spinner during data fetch

**Implementation**:

- State lifted from `useTerminationGrid` hook to parent component
- Parent tracks `isFetching` via `useState`
- `useEffect` in hook calls `onLoadingChange(isFetching)`
- State passed down to search filter
- `SearchAndReset` component consumes state

**User Experience**:

- Button disabled during search
- Spinner appears on button
- Cannot trigger multiple simultaneous searches
- Matches UnForfeit behavior

---

## Shared Components

The Termination feature uses shared components from `/src/ui/src/components/ForfeitActivities/` to maintain consistency with UnForfeit.

### 1. SharedForfeitHeaderComponent

**Purpose**: Generic bulk save header for forfeit activities

**Usage in Termination**:

```typescript
<SharedForfeitHeaderComponent
  {...params}
  config={{
    activityType: "termination"
  }}
/>
```

**Activity-Specific Behavior**:

- Row key generation: Composite `${badgeNumber}-${profitYear}`
- Value transformation: No negation
- Eligibility: Current year only, non-zero values
- No `offsettingProfitDetailId`

### 2. createSaveButtonCellRenderer

**Purpose**: Factory function for save button cell renderers

**Usage in Termination**:

```typescript
cellRenderer: createSaveButtonCellRenderer({
  activityType: "termination",
  selectedProfitYear,
  isReadOnly
});
```

**Features**:

- Checkbox for bulk selection
- Individual save button with spinner
- Tooltips for disabled states
- Activity-specific payload creation
- Loading state management

### Benefits of Shared Components

1. **Consistency**: Termination and UnForfeit stay in sync
2. **Maintainability**: Changes in one place affect both features
3. **Type Safety**: Shared components fully typed
4. **Testability**: Can unit test utilities independently
5. **DRY Principle**: Eliminated ~148 lines of duplicated code

---

## Type Definitions

### Request Types

```typescript
interface TerminationSearchRequest extends StartAndEndDateRequest {
  forfeitureStatus: string; // "showAll" | "forfeited" | "notForfeited"
  archive?: boolean;
}

interface StartAndEndDateRequest {
  beginningDate: string; // "MM/DD/YYYY"
  endingDate: string; // "MM/DD/YYYY"
  profitYear?: number;
  pagination?: {
    skip: number;
    take: number;
    sortBy: string;
    isSortDescending: boolean;
  };
}

interface ForfeitureAdjustmentUpdateRequest {
  badgeNumber: number;
  profitYear: number;
  forfeitureAmount: number; // NOT negated for termination
  classAction: boolean;
  offsettingProfitDetailId?: number; // Undefined for termination
}
```

### Response Types

```typescript
interface TerminationResponse {
  response: {
    results: TerminationMasterDto[];
    total: number;
  };
  totalEndingBalance: number;
  totalVested: number;
  totalForfeit: number;
  totalBeneficiaryAllocation: number;
}

interface TerminationMasterDto {
  badgeNumber: number;
  name: string;
  profitYears: TerminationDetailDto[];
}

interface TerminationDetailDto {
  profitYear: number;
  beginningBalance: number;
  beneficiaryAllocation: number;
  distributionAmount: number;
  forfeit: number;
  endingBalance: number;
  vestedBalance: number;
  vestedPercent: number;
  dateTerm: string;
  ytdPsHours: number;
  age: number;
  hasForfeited: boolean;
  suggestedForfeit: number;
}
```

### Grid Types

```typescript
interface MasterRow {
  badgeNumber: number;
  name: string;
  isExpandable: boolean;
  isExpanded: boolean;
  isDetail: false;
}

interface DetailRow {
  badgeNumber: number;
  name: string; // Copied from master
  profitYear: number;
  beginningBalance: number;
  beneficiaryAllocation: number;
  distributionAmount: number;
  forfeit: number;
  endingBalance: number;
  vestedBalance: number;
  vestedPercent: number;
  dateTerm: string;
  ytdPsHours: number;
  age: number;
  hasForfeited: boolean;
  suggestedForfeit: number;
  isDetail: true;
}
```

---

## Common Patterns

### 1. Error Handling

**Pattern**: All errors displayed via `ApiMessageAlert` with common key

```typescript
// In component
<ApiMessageAlert commonKey="TerminationSave" />

// In hook
try {
  await updateForfeiture(request).unwrap();
} catch (error) {
  setMessage(dispatch, {
    commonKey: "TerminationSave",
    type: "error",
    message: `Failed to save forfeiture for ${name}`,
    description: error.message || "Unknown error"
  });
}
```

**Scroll-to-Top**: Window event listener ensures user sees error at page top

### 2. Loading States

**Pattern**: Multiple loading indicators for different operations

```typescript
// Global search loading
const [isFetching, setIsFetching] = useState(false);

// Individual row loading
const [loadingRowIds, setLoadingRowIds] = useState<Set<string>>(new Set());

// Mutation loading
const { isLoading } = useUpdateForfeitureAdjustmentMutation();
```

**Display**:

- Search: Disable button, show spinner
- Individual save: Show `CircularProgress` in button
- Bulk save: Disable checkbox, show progress per row

### 3. Memoization

**Pattern**: Memoize expensive computations and callbacks

```typescript
// Column definitions
const mainColumns = useMemo(() => GetTerminationColumns(), []);
const detailColumns = useMemo(
  () => GetDetailColumns(/* ... */),
  [
    /* dependencies */
  ]
);

// Callbacks
const handleSave = useCallback(
  async (request, name) => {
    /* ... */
  },
  [
    /* dependencies */
  ]
);
```

**Benefits**:

- Prevent unnecessary re-renders
- Improve performance with large datasets
- Stable references for child components

### 4. Conditional Rendering

**Pattern**: Progressive disclosure based on state

```typescript
// Wait for fiscal data
{!isCalendarDataLoaded ? (
  <CircularProgress />
) : (
  <>
    <TerminationSearchFilter />
    <TerminationGrid />
  </>
)}

// Show grid only after search
{termination?.response && (
  <TerminationGrid data={termination.response} />
)}
```

**Benefits**:

- Better UX (show loading states)
- Avoid errors from missing data
- Clear visual hierarchy

### 5. Context for Grid State

**Pattern**: Use grid context to share state across cell renderers

```typescript
// Define context
const gridContext = useMemo(() => ({
  editedValues: editedValuesRef.current,
  loadingRowIds: loadingRowIds
}), [loadingRowIds]);

// Pass to grid
<DSMGrid
  providedOptions={{
    context: gridContext,
    // ... other options
  }}
/>

// Access in cell renderer
cellRenderer: (params) => {
  const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
  const currentValue = params.context?.editedValues?.[rowKey]?.value;
  const hasError = params.context?.editedValues?.[rowKey]?.hasError;
  const isLoading = params.context?.loadingRowIds?.has(params.data.badgeNumber);
  // ...
}
```

**Benefits**:

- Share state without prop drilling
- Update cell renderers reactively
- Centralized state management

---
