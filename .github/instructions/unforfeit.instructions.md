---
applyTo: "src/ui/src/pages/DecemberActivities/Unforfeit/**/*.*"
---
# UnForfeit Feature - Technical Implementation Guide

## Overview

The UnForfeit (Rehire Forfeitures) feature is a December year-end activity that allows users to reverse forfeitures for rehired employees. It implements a master-detail grid pattern with inline editing, bulk save operations, comprehensive state management, and real-time validation.

**Purpose**: Reverse profit-sharing forfeitures for employees who were previously terminated and have been rehired within a specified date range during the year-end closing process.

---

## Architecture Summary

### Component Hierarchy

```
UnForfeit.tsx (Page Container)
├── ApiMessageAlert (Error/success messages)
├── FrozenYearWarning (Conditional - shown if year is frozen)
├── DSMAccordion (Collapsible filter section)
│   └── UnForfeitSearchFilter
│       ├── DsmDatePicker (Rehire Begin Date)
│       ├── DsmDatePicker (Rehire End Date)
│       ├── Checkbox (Exclude zero balance employees)
│       └── SearchAndReset (Action buttons with loading state)
└── UnForfeitGrid
    ├── ReportSummary (Total record counts)
    ├── DSMGrid (AG Grid wrapper)
    │   ├── Expansion column (►/▼)
    │   ├── Main row columns (10 columns - badge, name, SSN, dates, balances, etc.)
    │   └── Detail row columns (6 columns with inline editing)
    └── Pagination
```

### Key Design Patterns

1. **Master-Detail Grid**: Employees (master) expand to show profit year records (detail)
2. **Inline Editing**: Direct cell editing for "Suggested Unforfeiture" amounts
3. **Bulk Operations**: Select multiple rows and save them simultaneously
4. **Shared Components**: Uses refactored shared components from `/src/ui/src/components/ForfeitActivities/`
5. **Auto-Archive**: Automatically refreshes data when status changes to "Complete"
6. **Frozen Year Warning**: Displays warning banner when profit year is frozen
7. **Read-Only Mode**: Comprehensive support for read-only navigation contexts

---

## File Structure

### 1. **UnForfeit.tsx** (Page Container - 100 lines)

**Purpose**: Main orchestrator for the UnForfeit feature

**Key Responsibilities**:

- Page layout and structure using `Page` component from smart-ui-library
- Fiscal calendar data fetching
- Status change handling via `StatusDropdownActionNode`
- Frozen year warning display
- Navigation guard for unsaved changes
- Auto-trigger search when archive mode activated

**State Management**:

```typescript
const { state, actions } = useUnForfeitState();
// state.initialSearchLoaded - Whether first search performed
// state.resetPageFlag - Triggers pagination reset
// state.hasUnsavedChanges - Tracks pending edits
// state.shouldBlock - Whether to block navigation
// state.previousStatus - Previously selected status
// state.shouldArchive - Archive mode flag
```

**Key Hooks Used**:

- `useLazyGetAccountingRangeToCurrent(6)` - Fetches fiscal calendar data
- `useUnForfeitState()` - Page-level state management
- `useIsProfitYearFrozen(profitYear)` - Check if year is frozen
- `useUnsavedChangesGuard()` - Blocks navigation with unsaved changes
- `useDecemberFlowProfitYear()` - Get active profit year

**Auto-Archive Pattern**:

```typescript
useEffect(() => {
  if (state.shouldArchive) {
    actions.handleSearch(); // Auto-trigger search
  }
}, [state.shouldArchive, actions]);
```

**Purpose**: When status changes to "Complete", automatically refresh data without manual search click.

**Frozen Year Warning**:

```typescript
{isFrozen && <FrozenYearWarning profitYear={profitYear} />}
```

**Props Flow**:

```typescript
// To UnForfeitSearchFilter
<UnForfeitSearchFilter
  setInitialSearchLoaded={actions.setInitialSearchLoaded}
  fiscalData={fiscalCalendarYear}
  onSearch={actions.handleSearch}
  hasUnsavedChanges={state.hasUnsavedChanges}
  setHasUnsavedChanges={actions.handleUnsavedChanges}
/>

// To UnForfeitGrid
<UnForfeitGrid
  initialSearchLoaded={state.initialSearchLoaded}
  setInitialSearchLoaded={actions.setInitialSearchLoaded}
  resetPageFlag={state.resetPageFlag}
  onUnsavedChanges={actions.handleUnsavedChanges}
  hasUnsavedChanges={state.hasUnsavedChanges}
  shouldArchive={state.shouldArchive}
  onArchiveHandled={actions.handleArchiveHandled}
  setHasUnsavedChanges={actions.handleUnsavedChanges}
  fiscalCalendarYear={fiscalCalendarYear}
/>
```

**File Location**: `/src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeit.tsx`

---

### 2. **UnForfeitSearchFilter.tsx** (Search Form - 251 lines)

**Purpose**: Date-based search filter with validation and zero-balance exclusion

**Key Features**:

- Date range selection (rehire begin/end dates within fiscal year)
- Form validation using React Hook Form + Yup
- "Exclude zero balance" checkbox filter
- Loading state feedback on search button (built-in via `useLazyGetUnForfeitsQuery`)
- Redux state persistence for search parameters

**Validation Schema**:

```typescript
const schema = yup.object().shape({
  beginningDate: dateStringValidator(2000, 2099, "Beginning Date").required(),
  endingDate: endDateStringAfterStartDateValidator(
    "beginningDate",
    tryddmmyyyyToDate,
    "Ending date must be the same or after the beginning date"
  )
    .required()
    .test("is-too-early", "Insufficient data for dates before 2024", (value) => {
      return new Date(value) > new Date(2024, 1, 1);
    }),
  profitYear: profitYearValidator()
});
```

**Date Validation**: Rejects dates before February 2024 (insufficient historical data).

**Date Range Constraints**:

- Rehire Begin Date: Min = fiscal begin, Max = fiscal end
- Rehire End Date: Min = max(begin date, fiscal begin), Max = fiscal end
- Both dates use `DsmDatePicker` with `disableFuture`

**Exclude Zero Balance Checkbox**:

```typescript
<FormControlLabel
  control={
    <Checkbox
      checked={field.value || false}
      onChange={(e) => field.onChange(e.target.checked)}
    />
  }
  label="Exclude employees with no current or vested balance"
/>
```

**Purpose**: Filter out employees with zero balances to focus on actionable unforfeitures.

**Loading State** (Built-in):

```typescript
const [triggerSearch, { isFetching }] = useLazyGetUnForfeitsQuery();

<SearchAndReset
  handleReset={handleReset}
  handleSearch={validateAndSearch}
  isFetching={isFetching}  // Built into search filter
  disabled={!isValid || isFetching}
/>
```

**Key Difference from Termination**: UnForfeit has built-in loading state because search trigger happens in filter component, not parent.

**Validation and Submit**:

```typescript
const validateAndSubmit = (data: StartAndEndDateRequest) => {
  if (hasUnsavedChanges) {
    alert("Please save your changes.");
    return;
  }

  if (isValid && hasToken) {
    const updatedData = {
      ...data,
      beginningDate: mmDDYYFormat(beginDate),
      endingDate: mmDDYYFormat(endDate),
      profitYear: selectedProfitYear
    };

    dispatch(setUnForfeitsQueryParams(updatedData)); // Redux persistence
    triggerSearch(updatedData); // RTK Query
    if (onSearch) onSearch(); // Notify parent
  }
};
```

**Redux Persistence**:

- Stores search parameters in Redux `yearsEndSlice`
- Allows restoring search state on page refresh
- Enables navigation away and back without losing search

**Reset Functionality**:

- Clears all form fields
- Resets to fiscal year defaults
- Clears Redux search params (`clearUnForfeitsQueryParams`)
- Clears Redux results (`clearUnForfeitsDetails`)
- Marks search as not loaded
- Clears validation errors

**File Location**: `/src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeitSearchFilter.tsx`

---

### 3. **UnForfeitGrid.tsx** (Grid Display - 231 lines)

**Purpose**: Master-detail grid with inline editing

**Key Features**:

- Dynamic grid height calculation
- Master-detail row expansion/collapse
- Read-only mode support
- Inline editing for all detail rows
- Pagination
- Custom CSS for detail row styling

**Custom Styling** (Inline):

```typescript
<style>
  {`
    .detail-row {
      background-color: #f5f5f5;
    }
    .invalid-cell {
      background-color: #fff6f6;
    }
  `}
</style>
```

**Detail rows**: Gray background (`#f5f5f5`)
**Invalid cells**: Light red background (`#fff6f6`)

**Hook Integration**:

```typescript
const {
  pageNumber,
  pageSize,
  gridData,
  unForfeits,
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
} = useUnForfeitGrid({
  initialSearchLoaded,
  setInitialSearchLoaded,
  resetPageFlag,
  onUnsavedChanges,
  hasUnsavedChanges,
  shouldArchive,
  onArchiveHandled,
  setHasUnsavedChanges,
  fiscalCalendarYear
});
```

**Master-Detail Column Composition** (Same as Termination):

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

**Grid Context** (Shared State):

```typescript
context: {
  editedValues: Record<number, { value: number }>,  // Key is profitDetailId
  loadingRowIds: Set<string>
}
```

**Key Difference from Termination**: Uses `profitDetailId` as key (number) instead of composite string.

**File Location**: `/src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeitGrid.tsx`

---

### 4. **UnForfeitGridColumns.ts** (Main Columns - 58 lines)

**Purpose**: Column definitions for master (employee summary) rows

**Comprehensive Master Columns** (10 total):

| Column          | Field                            | Type     | Description                |
| --------------- | -------------------------------- | -------- | -------------------------- |
| Badge Number    | `badgeNumber`                    | Number   | Employee badge (navigable) |
| Full Name       | `fullName`                       | String   | Employee full name         |
| SSN             | `ssn`                            | String   | Social security (masked)   |
| Hire Date       | `hireDate`                       | Date     | Original hire date         |
| Rehired Date    | `reHiredDate`                    | Date     | Rehire date                |
| Current Balance | `netBalanceLastYear`             | Currency | Current balance            |
| Vested Balance  | `vestedBalanceLastYear`          | Currency | Vested balance             |
| Store           | `store`                          | Number   | Store number               |
| Years           | `companyContributionYears`       | Count    | Contribution years         |
| Enrollment      | `enrollmentId`, `enrollmentName` | String   | Formatted as `[id] name`   |

**Key Difference from Termination**: Much more comprehensive master columns showing employee context and balances.

**Column Factories** (from `/src/ui/src/utils/gridColumnFactory`):

- `createBadgeColumn()` - Badge number with navigation
- `createNameColumn()` - Employee full name
- `createSSNColumn()` - Masked SSN display
- `createDateColumn()` - Date formatting
- `createCurrencyColumn()` - Currency formatting
- `createStoreColumn()` - Store number
- `createCountColumn()` - Numeric count

**Enrollment Column** (Custom formatter):

```typescript
{
  headerName: "Enrollment",
  field: "enrollmentId",
  colId: "enrollmentId",
  width: 120,
  valueGetter: (params) => {
    const id = params.data?.enrollmentId;
    const name = params.data?.enrollmentName;
    if (!id || !name) return "-";
    return `[${id}] ${name}`;
  }
}
```

**File Location**: `/src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeitGridColumns.ts`

---

### 5. **UnForfeitProfitDetailGridColumns.tsx** (Detail Columns - 108 lines)

**Purpose**: Column definitions for detail (profit year) rows with inline editing

**Column Structure** (7 total columns):

| Column                     | Field                   | Type     | Editable | Description                           |
| -------------------------- | ----------------------- | -------- | -------- | ------------------------------------- |
| Profit Year                | `profitYear`            | Year     | No       | Year of profit record                 |
| Hours                      | `hoursTransactionYear`  | Hours    | No       | Transaction year hours (zeros hidden) |
| Wages                      | `wagesTransactionYear`  | Currency | No       | Transaction year wages (zeros hidden) |
| Forfeiture                 | `forfeiture`            | Currency | No       | Original forfeiture amount            |
| **Suggested Unforfeiture** | `suggestedUnforfeiture` | Currency | **Yes**  | Amount to reverse (editable)          |
| Remark                     | `remark`                | String   | No       | Comment/note                          |
| Save Button                | (special)               | Action   | N/A      | Checkbox + Save button                |

**Zeros Hidden Pattern** (Hours and Wages):

```typescript
valueGetter: (params) => {
  // Do not show zeros, for many years we only have zero (aka no data)
  const value = params.data?.hoursTransactionYear;
  return value == null || value == 0 ? null : value;
};
```

**Purpose**: Historical years often have zero hours/wages (no data available). Hiding zeros improves readability.

**Editable Check Function**:

```typescript
function isTransactionEditable(params, isReadOnly: boolean = false): boolean {
  return params.data.suggestedUnforfeiture != null && !isReadOnly;
}
```

**Key Difference from Termination**: All years are editable (not just current year), as long as they have a suggested unforfeiture value.

**Suggested Unforfeiture Column** (Inline Editing):

```typescript
{
  headerName: "Suggested Unforfeiture",
  field: "suggestedUnforfeit",
  width: 150,
  pinned: "right",
  editable: (params) => isTransactionEditable(params, isReadOnly),
  cellEditor: SuggestedForfeitEditor,
  cellRenderer: (params) => SuggestedForfeitCellRenderer(
    { ...params, selectedProfitYear },
    false,  // isTermination = false
    true    // isUnforfeit = true
  ),
  valueFormatter: (params) => numberToCurrency(params.data.suggestedUnforfeiture)
}
```

**Row Key Pattern**: `profitDetailId` (Unique ID)

- Single numeric ID per profit year record
- Used to track edited values before save
- Different from Termination (which uses composite key)

**Save Button Column** (Refactored):

```typescript
{
  headerName: "Save Button",
  field: "saveButton",
  pinned: "right",
  lockPinned: true,
  headerComponent: HeaderComponent, // Bulk save checkbox
  cellRenderer: createSaveButtonCellRenderer({
    activityType: "unforfeit",
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
- Calls `params.api.refreshCells({ force: true })` on checkbox change (UnForfeit-specific)

**File Location**: `/src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeitProfitDetailGridColumns.tsx`

---

### 6. **UnForfeitHeaderComponent.tsx** (Bulk Save Header - 13 lines)

**Purpose**: Custom header for save button column with bulk save functionality

**Refactored Implementation** (Uses Shared Component):

```typescript
export const HeaderComponent: React.FC<UnForfeitHeaderComponentProps> = (params) => {
  return (
    <SharedForfeitHeaderComponent
      {...params}
      config={{
        activityType: "unforfeit"
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

**Activity-Specific Behavior** (UnForfeit):

- **Row Key**: Single `profitDetailId`
- **Value Transformation**: NEGATED (sent as negative to API)
- **Eligibility**: All detail rows with non-zero values
- **Requires `offsettingProfitDetailId`**: Must link to original forfeiture

**File Location**: `/src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeitHeaderComponent.tsx`

---

## Data Flow

### Search Flow

```
User clicks "SEARCH"
    ↓
UnForfeitSearchFilter validates form (React Hook Form + Yup)
    ↓
triggerSearch() called directly in filter (RTK Query lazy query)
    ↓
dispatch(setUnForfeitsQueryParams(params)) - Store in Redux
    ↓
API request to /api/unforfeits
    ↓
Response cached by RTK Query
    ↓
onSearch() callback notifies parent
    ↓
actions.handleSearch() toggles resetPageFlag
    ↓
UnForfeitGrid receives resetPageFlag change
    ↓
useUnForfeitGrid detects resetPageFlag change
    ↓
Fetches data from Redux selector
    ↓
flattenMasterDetailData() transforms hierarchical data to flat structure
    ↓
Grid displays master rows (badge, name, SSN, dates, balances, etc.)
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

**Key Difference from Termination**: Search happens IN the filter component, not in the grid hook.

### Save Flow (Individual)

```
User edits "Suggested Unforfeiture" cell
    ↓
SuggestedForfeitEditor captures value
    ↓
editedValues context updated: editedValues[profitDetailId] = { value: newValue }
    ↓
onUnsavedChanges(true) - Enables navigation guard
    ↓
Cell re-renders with edited value (from context)
    ↓
User clicks individual save button
    ↓
handleSave() called from useUnForfeitGrid
    ↓
prepareSaveRequest() creates ForfeitureAdjustmentUpdateRequest
    ↓
Value NEGATED (unforfeit sends negative values)
    ↓
offsettingProfitDetailId added (links to original forfeiture)
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
  - Keep edited value in context
  - Row remains selected
  - onUnsavedChanges remains true
```

### Bulk Save Flow

```
User edits multiple cells
    ↓
Each edit tracked in editedValues context (keyed by profitDetailId)
    ↓
User clicks checkboxes for rows to save
    ↓
selectedRowIds Set updated via selectionState
    ↓
User clicks bulk save checkbox in header
    ↓
handleBulkSave() called from useUnForfeitGrid
    ↓
Filter selected rows using isNodeEligible():
  - Must be detail row (isDetail === true)
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
useUnForfeitState reducer:
  - Detects status contains "complete"
  - Sets shouldArchive = true
    ↓
UnForfeit.tsx useEffect detects shouldArchive
    ↓
actions.handleSearch() called automatically (no user click needed)
    ↓
toggles resetPageFlag in reducer
    ↓
UnForfeitGrid detects resetPageFlag change
    ↓
Grid refetches data from Redux
    ↓
Grid displays current data (archive mode is implicit)
    ↓
actions.handleArchiveHandled() called
    ↓
shouldArchive reset to false
```

**Key Difference from Termination**: No explicit archive flag in search params. Archive is implicit in status change.

---

## State Management

### Page-Level State (useUnForfeitState)

**Hook Location**: `/src/ui/src/hooks/useUnForfeitState.ts`

**State Structure**:

```typescript
interface UnForfeitState {
  initialSearchLoaded: boolean;
  resetPageFlag: boolean;
  hasUnsavedChanges: boolean;
  shouldBlock: boolean;
  previousStatus: string | null;
  shouldArchive: boolean;
}
```

**Actions**:

- `setInitialSearchLoaded(loaded)` - Mark search as performed
- `handleSearch()` - Toggle resetPageFlag
- `handleUnsavedChanges(hasChanges)` - Track unsaved edits
- `setShouldBlock(shouldBlock)` - Set navigation block flag
- `handleStatusChange(status, statusName)` - Handle status change, trigger archive
- `handleArchiveHandled()` - Clear archive flag

**Archive Mode Logic**:

```typescript
case "SET_STATUS_CHANGE": {
  const { status, statusName } = action.payload;
  const isCompleteLike = (statusName ?? "").toLowerCase().includes("complete");
  const isChangingToComplete = isCompleteLike && state.previousStatus !== status;

  return {
    ...state,
    previousStatus: status,
    shouldArchive: isChangingToComplete // Trigger archive refresh
  };
}
```

**Simpler than Termination**: No explicit archive flag in search params. Just triggers a refresh.

### Grid State (useUnForfeitGrid)

**Hook Location**: `/src/ui/src/hooks/useUnForfeitGrid.ts`

**Key State Variables**:

```typescript
// Expansion state
const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});

// Message pending state
const [pendingSuccessMessage, setPendingSuccessMessage] = useState<string | null>(null);
const [isPendingBulkMessage, setIsPendingBulkMessage] = useState<boolean>(false);

// RTK Query state
const [triggerSearch, { isFetching }] = useLazyGetUnForfeitsQuery();

// Edit state (from useEditState hook)
editState: {
  editedValues: Record<number, { value: number }>,  // Key is profitDetailId (number)
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

**Composite Hooks Pattern** (Same as Termination):

- `useEditState()` - Inline editing and validation
- `useRowSelection()` - Checkbox selection tracking
- `useGridPagination()` - Pagination and sorting

### Redux State (yearsEndSlice)

**Search Parameters**:

```typescript
unForfeitsQueryParams: {
  beginningDate: string,
  endingDate: string,
  excludeZeroBalance: boolean,
  profitYear: number,
  pagination: { skip, take, sortBy, isSortDescending }
}
```

**Search Results**:

```typescript
unForfeitsDetails: {
  response: {
    results: UnForfeitMasterDto[],
    total: number
  }
}
```

**Actions**:

- `setUnForfeitsQueryParams(params)` - Store search parameters
- `clearUnForfeitsQueryParams()` - Clear search parameters
- `clearUnForfeitsDetails()` - Clear search results

**Why Store in Redux?**

- Persist search parameters across component re-renders
- Restore search state on page refresh
- Share data across components

---

## API Integration

### Search Endpoint

**RTK Query Hook**: `useLazyGetUnForfeitsQuery()`

**Request**:

```typescript
interface StartAndEndDateRequest {
  beginningDate: string; // Format: "MM/DD/YYYY"
  endingDate: string; // Format: "MM/DD/YYYY"
  excludeZeroBalance?: boolean;
  profitYear?: number;
  pagination?: {
    skip: number;
    take: number;
    sortBy: string;
    isSortDescending: boolean;
  };
}
```

**Response**:

```typescript
interface UnForfeitsResponse {
  response: {
    results: UnForfeitMasterDto[];
    total: number;
  };
}

interface UnForfeitMasterDto {
  badgeNumber: number;
  fullName: string;
  ssn: string;
  hireDate: string;
  reHiredDate: string;
  netBalanceLastYear: number;
  vestedBalanceLastYear: number;
  store: number;
  companyContributionYears: number;
  enrollmentId: number;
  enrollmentName: string;
  profitYearDetails: UnForfeitDetailDto[];
}

interface UnForfeitDetailDto {
  profitDetailId: number; // Unique ID for this record
  profitYear: number;
  hoursTransactionYear: number;
  wagesTransactionYear: number;
  forfeiture: number;
  suggestedUnforfeiture: number;
  remark: string;
}
```

### Update Endpoint

**RTK Query Hook**: `useUpdateForfeitureAdjustmentMutation()`

**Request**:

```typescript
interface ForfeitureAdjustmentUpdateRequest {
  badgeNumber: number;
  profitYear: number;
  forfeitureAmount: number; // NEGATED for unforfeit
  offsettingProfitDetailId: number; // Required - links to original forfeiture
  classAction: boolean; // Always false for manual operations
}
```

**Critical**: UnForfeit NEGATES the value and REQUIRES `offsettingProfitDetailId`.

**Response**: Success/error status

### Bulk Update Endpoint

**RTK Query Hook**: `useUpdateForfeitureAdjustmentBulkMutation()`

**Request**: Array of `ForfeitureAdjustmentUpdateRequest`

**Response**: Success/error status

---

## Key Features

### 1. Value Transformation (Critical)

**UnForfeit Pattern**: **Negates values**

```typescript
// User enters: 1000.00
// API receives: -1000.00 (negated)

transformForfeitureValue("unforfeit", 1000.0); // Returns: -1000.00
```

**Why?**

- Forfeitures are stored as negative values in database
- Unforfeitures reverse forfeitures by adding positive offsetting entries
- Negation converts user-entered positive values to required format

**Contrast with Termination**:

```typescript
transformForfeitureValue("termination", 2000.0); // Returns: 2000.00 (no negation)
```

### 2. Row Keys and Tracking

**UnForfeit uses `profitDetailId` as row key**: Single numeric ID

```typescript
generateRowKey(
  { type: "unforfeit" },
  {
    badgeNumber: 12345,
    profitYear: 2021,
    profitDetailId: 98765
  }
);
// Returns: "98765"
```

**Why `profitDetailId`?**

- Each profit year record has unique `profitDetailId`
- Needed to track which specific forfeiture is being reversed
- Sent as `offsettingProfitDetailId` in API request
- Backend uses this to link unforfeit to original forfeiture

**Usage**:

```typescript
// In editedValues context
context.editedValues[profitDetailId] = { value: 1000.00 };

// In API request
{
  badgeNumber: 12345,
  profitYear: 2021,
  forfeitureAmount: -1000.00,
  offsettingProfitDetailId: 98765, // Links to original forfeiture
  classAction: false
}
```

**Contrast with Termination**: Uses composite key `${badgeNumber}-${profitYear}`

### 3. Editable Criteria

**UnForfeit Pattern**: All years with suggested unforfeiture value

```typescript
function isTransactionEditable(params, isReadOnly: boolean = false): boolean {
  return params.data.suggestedUnforfeiture != null && !isReadOnly;
}
```

**Key Difference from Termination**: Can edit any historical year (not just current year).

**Why?**

- Unforfeitures can reverse forfeitures from any past year
- Terminations only create new forfeitures for current year

### 4. Exclude Zero Balance Filter

**Feature**: Checkbox to exclude employees with no balance

**Implementation**:

```typescript
<FormControlLabel
  control={
    <Checkbox
      checked={field.value || false}
      onChange={(e) => field.onChange(e.target.checked)}
    />
  }
  label="Exclude employees with no current or vested balance"
/>
```

**Sent to API**: `excludeZeroBalance: true/false`

**Purpose**: Focus on actionable unforfeitures by hiding employees with no balances.

**Not in Termination**: Termination doesn't have this filter.

### 5. Frozen Year Warning

**Feature**: Warning banner when profit year is frozen

**Implementation**:

```typescript
const isFrozen = useIsProfitYearFrozen(profitYear);

{isFrozen && <FrozenYearWarning profitYear={profitYear} />}
```

**Purpose**: Alert users that year is locked, preventing accidental edits during frozen periods.

**Not in Termination**: Termination doesn't show frozen warning (different business rules).

### 6. Auto-Archive on Status Change

**Feature**: Automatically refreshes data when status changes to "Complete"

**Implementation**:

```typescript
useEffect(() => {
  if (state.shouldArchive) {
    actions.handleSearch(); // Auto-trigger
  }
}, [state.shouldArchive, actions]);
```

**Purpose**: Seamless transition to viewing archived data without manual search.

**Contrast with Termination**: Termination adds explicit archive flag to search params.

### 7. Loading State (Built-in)

**Feature**: Search button disables and shows spinner during fetch

**Implementation**: Built directly into search filter component

```typescript
const [triggerSearch, { isFetching }] = useLazyGetUnForfeitsQuery();

<SearchAndReset
  isFetching={isFetching}
  disabled={!isValid || isFetching}
/>
```

**Key Difference from Termination**: UnForfeit has built-in loading state because search trigger is in filter, not grid.

### 8. Zeros Hidden (Hours/Wages)

**Feature**: Hours and wages columns hide zero values

**Implementation**:

```typescript
valueGetter: (params) => {
  const value = params.data?.hoursTransactionYear;
  return value == null || value == 0 ? null : value;
};
```

**Purpose**: Many historical years have zero hours/wages (no data available). Hiding improves readability.

**Not in Termination**: Termination shows all values.

### 9. Grid Cell Refresh on Checkbox

**Feature**: Forces grid refresh when checkbox state changes

**Implementation**:

```typescript
onChange={() => {
  if (!isReadOnly) {
    if (isSelected) {
      params.removeRowFromSelectedRows(id);
      params.node?.setSelected(false);
    } else {
      params.addRowToSelectedRows(id);
      params.node?.setSelected(true);
    }
    params.api.refreshCells({ force: true }); // Force refresh
  }
}}
```

**Purpose**: Ensures checkbox state syncs properly with AG Grid selection state.

**UnForfeit-Specific**: Termination doesn't need this due to different selection handling.

### 10. Comprehensive Master Columns

**Feature**: 10 master columns showing complete employee context

**Columns**: Badge, Name, SSN, Hire Date, Rehire Date, Current Balance, Vested Balance, Store, Years, Enrollment

**Contrast with Termination**: Termination only shows Badge and Name (minimal master columns).

**Why?**

- UnForfeit needs more context to assess rehire unforfeitures
- Users need balances and dates visible before expanding rows
- Termination focuses on detail rows for financial data

---

## Shared Components

The UnForfeit feature uses shared components from `/src/ui/src/components/ForfeitActivities/` to maintain consistency with Termination.

### 1. SharedForfeitHeaderComponent

**Purpose**: Generic bulk save header for forfeit activities

**Usage in UnForfeit**:

```typescript
<SharedForfeitHeaderComponent
  {...params}
  config={{
    activityType: "unforfeit"
  }}
/>
```

**Activity-Specific Behavior**:

- Row key generation: Single `profitDetailId`
- Value transformation: Negation
- Eligibility: All years, non-zero values
- Requires `offsettingProfitDetailId`

### 2. createSaveButtonCellRenderer

**Purpose**: Factory function for save button cell renderers

**Usage in UnForfeit**:

```typescript
cellRenderer: createSaveButtonCellRenderer({
  activityType: "unforfeit",
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
- Grid refresh on checkbox change (UnForfeit-specific)

### Benefits of Shared Components

1. **Consistency**: Termination and UnForfeit stay in sync
2. **Maintainability**: Changes in one place affect both features
3. **Type Safety**: Shared components fully typed
4. **Testability**: Can unit test utilities independently
5. **DRY Principle**: Eliminated ~148 lines of duplicated code

---

## Differences from Termination

| Aspect                       | UnForfeit                          | Termination                                |
| ---------------------------- | ---------------------------------- | ------------------------------------------ |
| **Value Transformation**     | Negates (sends negative)           | No negation (sends positive)               |
| **Row Key**                  | `profitDetailId` (single numeric)  | `${badgeNumber}-${profitYear}` (composite) |
| **offsettingProfitDetailId** | Required (links to original)       | Omitted (new forfeitures)                  |
| **Editable Criteria**        | All years with non-zero            | Current year only                          |
| **Master Columns**           | 10 columns (comprehensive)         | 2 columns (minimal)                        |
| **Financial Summaries**      | No                                 | Yes (4 totals grids)                       |
| **Frozen Warning**           | Yes                                | No                                         |
| **Exclude Zero Filter**      | Yes                                | No                                         |
| **Loading State**            | Built-in (filter component)        | Lifted to parent                           |
| **Archive Mode**             | Auto-refresh on status change      | Explicit archive flag                      |
| **Zero Hiding**              | Yes (hours/wages)                  | No                                         |
| **Grid Refresh**             | Calls `refreshCells()` on checkbox | Not needed                                 |
| **Search Location**          | In filter component                | In grid hook                               |
| **Redux Persistence**        | Search params + results            | Just results                               |

---

## Type Definitions

### Request Types

```typescript
interface StartAndEndDateRequest {
  beginningDate: string; // "MM/DD/YYYY"
  endingDate: string; // "MM/DD/YYYY"
  excludeZeroBalance?: boolean;
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
  forfeitureAmount: number; // NEGATED for unforfeit
  offsettingProfitDetailId: number; // Required for unforfeit
  classAction: boolean;
}
```

### Response Types

```typescript
interface UnForfeitsResponse {
  response: {
    results: UnForfeitMasterDto[];
    total: number;
  };
}

interface UnForfeitMasterDto {
  badgeNumber: number;
  fullName: string;
  ssn: string;
  hireDate: string;
  reHiredDate: string;
  netBalanceLastYear: number;
  vestedBalanceLastYear: number;
  store: number;
  companyContributionYears: number;
  enrollmentId: number;
  enrollmentName: string;
  profitYearDetails: UnForfeitDetailDto[];
}

interface UnForfeitDetailDto {
  profitDetailId: number;
  profitYear: number;
  hoursTransactionYear: number;
  wagesTransactionYear: number;
  forfeiture: number;
  suggestedUnforfeiture: number;
  remark: string;
}
```

### Grid Types

```typescript
interface MasterRow {
  badgeNumber: number;
  fullName: string;
  ssn: string;
  hireDate: string;
  reHiredDate: string;
  netBalanceLastYear: number;
  vestedBalanceLastYear: number;
  store: number;
  companyContributionYears: number;
  enrollmentId: number;
  enrollmentName: string;
  isExpandable: boolean;
  isExpanded: boolean;
  isDetail: false;
}

interface DetailRow {
  badgeNumber: number;
  fullName: string; // Copied from master
  profitDetailId: number;
  profitYear: number;
  hoursTransactionYear: number;
  wagesTransactionYear: number;
  forfeiture: number;
  suggestedUnforfeiture: number;
  remark: string;
  isDetail: true;
}
```

---

## Common Patterns

### 1. Error Handling

**Pattern**: All errors displayed via `ApiMessageAlert` with common key

```typescript
// In component
<ApiMessageAlert commonKey="UnforfeitSave" />

// In hook
try {
  await updateForfeiture(request).unwrap();
} catch (error) {
  setMessage(dispatch, {
    commonKey: "UnforfeitSave",
    type: "error",
    message: `Failed to save unforfeiture for ${name}`,
    description: error.message || "Unknown error"
  });
}
```

### 2. Loading States

**Pattern**: Multiple loading indicators

```typescript
// Search loading (built-in)
const [triggerSearch, { isFetching }] = useLazyGetUnForfeitsQuery();

// Individual row loading
const [loadingRowIds, setLoadingRowIds] = useState<Set<string>>(new Set());

// Mutation loading
const { isLoading } = useUpdateForfeitureAdjustmentMutation();
```

### 3. Memoization

**Pattern**: Memoize expensive computations

```typescript
const mainColumns = useMemo(() => UnForfeitGridColumns(), []);
const detailColumns = useMemo(
  () => GetProfitDetailColumns(/* ... */),
  [
    /* dependencies */
  ]
);
```

### 4. Conditional Rendering

**Pattern**: Progressive disclosure

```typescript
{!isCalendarDataLoaded ? (
  <CircularProgress />
) : (
  <>
    <UnForfeitSearchFilter />
    <UnForfeitGrid />
  </>
)}

{isFrozen && <FrozenYearWarning profitYear={profitYear} />}
```

---
