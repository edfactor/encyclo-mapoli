# UnForfeit Component Technical Documentation

## Overview

The UnForfeit feature allows users to reverse forfeitures for rehired employees during the December year-end process. The implementation follows a master-detail grid pattern with inline editing, bulk operations, and comprehensive state management.

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Component Hierarchy](#component-hierarchy)
3. [File Descriptions](#file-descriptions)
4. [State Management](#state-management)
5. [Data Flow](#data-flow)
6. [Grid Architecture](#grid-architecture)
7. [Key Features](#key-features)
8. [Shared Utilities](#shared-utilities)
9. [Redux Integration](#redux-integration)
10. [Hooks Reference](#hooks-reference)
11. [Type Definitions](#type-definitions)
12. [Common Patterns](#common-patterns)

---

## Architecture Overview

The UnForfeit feature implements a **Complex Page with Custom Hook** pattern (see `/src/ui/src/pages/CLAUDE.md` for pattern details). The architecture separates concerns into distinct layers:

```
┌─────────────────────────────────────────────────────────┐
│ UnForfeit.tsx (Page Container)                          │
│ - Page wrapper and layout                               │
│ - Status management                                     │
│ - Prerequisites (frozen year check)                     │
└──────────────────┬──────────────────────────────────────┘
                   │
        ┌──────────┴──────────┐
        │                     │
┌───────▼──────────┐  ┌──────▼────────────────────────┐
│ UnForfeitSearch  │  │ UnForfeitGrid.tsx              │
│ Filter.tsx       │  │ - Master-detail grid display  │
│ - Date range     │  │ - Expansion/collapse          │
│ - Filters        │  │ - Inline editing              │
│ - Validation     │  │ - Pagination                  │
└──────────────────┘  └───────┬───────────────────────┘
                              │
                  ┌───────────┴──────────┐
                  │                      │
      ┌───────────▼──────────┐  ┌───────▼─────────────┐
      │ useUnForfeitGrid.ts  │  │ Column Definitions  │
      │ - Business logic     │  │ - Main columns      │
      │ - API integration    │  │ - Detail columns    │
      │ - Selection state    │  │ - Header component  │
      └──────────────────────┘  └─────────────────────┘
```

### Key Architectural Principles

1. **Separation of Concerns**: Business logic in hooks, presentation in components
2. **Shared Utilities**: Common logic extracted to `/src/ui/src/utils/forfeitActivities/`
3. **Type Safety**: Full TypeScript typing throughout
4. **State Management**: Reducer pattern for complex state transitions
5. **Read-Only Support**: Comprehensive read-only mode using `useReadOnlyNavigation`

---

## Component Hierarchy

```
UnForfeit (Page Container)
├── Page (from smart-ui-library)
│   ├── label: "REHIRE FORFEITURES (008-10)"
│   └── actionNode: StatusDropdownActionNode
├── ApiMessageAlert (Error/success messages)
├── FrozenYearWarning (Conditional - shown if year is frozen)
├── DSMAccordion (Collapsible filter section)
│   └── UnForfeitSearchFilter
│       ├── DsmDatePicker (Begin Date)
│       ├── DsmDatePicker (End Date)
│       ├── FormControlLabel (Exclude zero balance checkbox)
│       └── SearchAndReset (Action buttons)
└── UnForfeitGrid
    ├── ReportSummary (Total counts)
    ├── DSMGrid (AG Grid wrapper)
    │   ├── Expansion column (►/▼)
    │   ├── Main row columns (badge, name, SSN, dates, balances)
    │   └── Detail row columns (profit year, hours, wages, forfeiture, suggested unforfeiture)
    └── Pagination
```

---

## File Descriptions

### 1. `UnForfeit.tsx`

**Purpose**: Main page container and orchestrator

**Responsibilities**:

- Page layout and structure
- Fiscal calendar data fetching
- Status change handling
- Frozen year warning display
- Navigation guard for unsaved changes
- Auto-trigger archive search when status changes to "Complete"

**Key Hooks Used**:

- `useUnForfeitState()` - State management
- `useLazyGetAccountingRangeToCurrent(6)` - Fetch fiscal calendar data
- `useDecemberFlowProfitYear()` - Get active profit year
- `useIsProfitYearFrozen(profitYear)` - Check if year is frozen
- `useUnsavedChangesGuard(hasUnsavedChanges)` - Block navigation if unsaved changes

**State Management**:

```typescript
const { state, actions } = useUnForfeitState();
// state.hasUnsavedChanges - Tracks pending edits
// state.initialSearchLoaded - Whether first search has been performed
// state.resetPageFlag - Triggers pagination reset
// state.shouldArchive - Archive mode flag (set when status changes to Complete)
```

**Key Features**:

- Loading state displays `CircularProgress` while fiscal data loads
- Conditional rendering based on `isCalendarDataLoaded`
- Auto-search trigger when `shouldArchive` becomes true (lines 36-40)
- Status change handler passed to `StatusDropdownActionNode`

**File Location**: `/src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeit.tsx`

---

### 2. `UnForfeitGrid.tsx`

**Purpose**: Master-detail grid display with inline editing

**Responsibilities**:

- Grid configuration and rendering
- Master-detail row expansion/collapse
- Column definition merging (main + detail columns)
- Cell styling for detail rows and invalid cells
- Grid context management
- Pagination

**Key Features**:

#### Master-Detail Pattern

The grid displays two types of rows:

1. **Master rows**: Employee summary (badge, name, SSN, hire dates, balances)
2. **Detail rows**: Individual profit year records (year, hours, wages, forfeiture, suggested unforfeiture)

Master rows can be expanded to show detail rows by clicking the expansion column (►/▼).

#### Grid Configuration

```typescript
const columnDefs = useMemo(() => {
  const expansionColumn = {
    field: "isExpandable",
    width: 50,
    cellRenderer: (params) => params.data.isExpandable ? (params.data.isExpanded ? "▼" : "►") : "",
    onCellClicked: (event) => handleRowExpansion(event.data.badgeNumber.toString())
  };

  // Main columns: visible for master rows, hidden for detail rows (unless shared)
  const visibleColumns = mainColumns.map(column => ({
    ...column,
    cellRenderer: (params) => {
      if (params.data?.isDetail) {
        const hideInDetails = !detailColumns.some(col => col.field === column.field);
        if (hideInDetails) return "";
      }
      return /* render value */;
    }
  }));

  // Detail-only columns: only visible for detail rows
  const detailOnlyColumns = detailColumns
    .filter(col => !mainColumns.some(mainCol => mainCol.field === col.field))
    .map(column => ({
      ...column,
      cellRenderer: (params) => !params.data?.isDetail ? "" : /* render value */
    }));

  return [expansionColumn, ...visibleColumns, ...detailOnlyColumns];
}, [mainColumns, detailColumns, handleRowExpansion]);
```

#### Row Styling

```typescript
getRowClass: (params) => (params.data.isDetail ? "detail-row" : "");
```

Detail rows have gray background (`#f5f5f5`) to visually distinguish them from master rows.

#### Dynamic Grid Height

```typescript
const gridMaxHeight = useDynamicGridHeight();
```

The grid height adjusts based on viewport size for optimal display.

#### Read-Only Mode

```typescript
const isReadOnly = useReadOnlyNavigation();
```

All editing, saving, and bulk operations are disabled when in read-only mode.

**File Location**: `/src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeitGrid.tsx`

---

### 3. `UnForfeitSearchFilter.tsx`

**Purpose**: Search and filter form with date range selection

**Responsibilities**:

- Date range validation (begin/end dates within fiscal year)
- Exclude zero balance checkbox
- Form validation using React Hook Form + Yup
- Search parameter management via Redux
- Reset functionality

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
    .test("is-too-early", "Insufficient data for dates before 2024", (value) => new Date(value) > new Date(2024, 1, 1)),
  profitYear: profitYearValidator()
});
```

**Key Features**:

1. **Date Range Selection**:

   - Begin date: Rehire begin date (min: fiscal begin, max: fiscal end)
   - End date: Rehire end date (min: begin date or fiscal begin, max: fiscal end)
   - Both dates use `DsmDatePicker` with `disableFuture`

2. **Exclude Zero Balance Checkbox**:

   - Filters out employees with no current or vested balance
   - Uses `FormControlLabel` with `Checkbox`
   - Has `FormHelperText` spacer for vertical alignment with date pickers

3. **Validation and Submit**:

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

       dispatch(setUnForfeitsQueryParams(updatedData));
       triggerSearch(updatedData);
       if (onSearch) onSearch();
     }
   };
   ```

4. **Reset Functionality**:
   ```typescript
   const handleReset = () => {
     setHasUnsavedChanges(false);
     setInitialSearchLoaded(false);
     dispatch(clearUnForfeitsQueryParams());
     dispatch(clearUnForfeitsDetails());
     reset({
       beginningDate: fiscalData.fiscalBeginDate ? mmDDYYFormat(fiscalData.fiscalBeginDate) : undefined,
       endingDate: fiscalData.fiscalEndDate ? mmDDYYFormat(fiscalData.fiscalEndDate) : undefined,
       excludeZeroBalance: false,
       profitYear: selectedProfitYear,
       pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
     });
     clearErrors();
   };
   ```

**File Location**: `/src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeitSearchFilter.tsx`

---

### 4. `UnForfeitGridColumns.ts`

**Purpose**: Column definitions for master (employee summary) rows

**Columns**:

| Column          | Field                            | Type     | Description                                     |
| --------------- | -------------------------------- | -------- | ----------------------------------------------- |
| Badge Number    | `badgeNumber`                    | Number   | Employee badge number (navigable)               |
| Full Name       | `fullName`                       | String   | Employee full name                              |
| SSN             | `ssn`                            | String   | Social security number (masked)                 |
| Hire Date       | `hireDate`                       | Date     | Original hire date                              |
| Rehired Date    | `reHiredDate`                    | Date     | Date employee was rehired                       |
| Current Balance | `netBalanceLastYear`             | Currency | Current profit sharing balance                  |
| Vested Balance  | `vestedBalanceLastYear`          | Currency | Vested portion of balance                       |
| Store           | `store`                          | Number   | Store number                                    |
| Years           | `companyContributionYears`       | Number   | Years of company contributions                  |
| Enrollment      | `enrollmentId`, `enrollmentName` | String   | Enrollment ID and name formatted as `[id] name` |

**Column Factory Usage**:
The file uses factory functions from `/src/ui/src/utils/gridColumnFactory.ts` for consistency:

- `createBadgeColumn({})` - Badge number with navigation
- `createNameColumn({ field: "fullName" })` - Name column
- `createSSNColumn({})` - Masked SSN
- `createDateColumn(...)` - Date formatting
- `createCurrencyColumn(...)` - Currency formatting
- `createStoreColumn({})` - Store number
- `createCountColumn(...)` - Numeric count

**File Location**: `/src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeitGridColumns.ts`

---

### 5. `UnForfeitProfitDetailGridColumns.tsx`

**Purpose**: Column definitions for detail (profit year) rows with inline editing

**Columns**:

| Column                 | Field                   | Type     | Editable | Description                                     |
| ---------------------- | ----------------------- | -------- | -------- | ----------------------------------------------- |
| Profit Year            | `profitYear`            | Year     | No       | Year of profit sharing record                   |
| Hours                  | `hoursTransactionYear`  | Hours    | No       | Hours worked in transaction year (zeros hidden) |
| Wages                  | `wagesTransactionYear`  | Currency | No       | Wages earned in transaction year (zeros hidden) |
| Forfeiture             | `forfeiture`            | Currency | No       | Amount forfeited                                |
| Suggested Unforfeiture | `suggestedUnforfeiture` | Currency | Yes      | Amount to unforfeit (editable, pinned right)    |
| Remark                 | `remark`                | String   | No       | Comment or note                                 |
| Save Button            | (special)               | Action   | N/A      | Checkbox + Save button (pinned right)           |

**Inline Editing**:

The "Suggested Unforfeiture" column uses custom editor and renderer:

```typescript
{
  headerName: "Suggested Unforfeiture",
  field: "suggestedUnforfeit",
  editable: (params) => params.data.suggestedUnforfeiture != null && !isReadOnly,
  cellEditor: SuggestedForfeitEditor,
  cellRenderer: (params) => {
    return SuggestedForfeitCellRenderer({ ...params, selectedProfitYear }, false, true);
  },
  valueFormatter: (params) => numberToCurrency(params.data.suggestedUnforfeiture)
}
```

**Save Button Column**:

The save button column includes:

1. **Header**: Custom `HeaderComponent` with bulk save checkbox
2. **Cell**: Checkbox (for bulk selection) + Save button (for individual save)

Cell Renderer Logic:

```typescript
cellRenderer: (params: UnForfeitsSaveButtonCellParams) => {
  if (!isTransactionEditable(params, isReadOnly)) {
    return "";
  }

  const rowKey = params.data.profitDetailId;
  const currentValue = params.context?.editedValues?.[rowKey]?.value ?? params.data.suggestedUnforfeiture;
  const isLoading = params.context?.loadingRowIds?.has(params.data.badgeNumber);
  const isDisabled = (currentValue || 0) === 0 || isLoading || isReadOnly;

  return (
    <div>
      <Checkbox
        checked={isSelected}
        disabled={isDisabled}
        onChange={() => {
          if (isSelected) {
            params.removeRowFromSelectedRows(id);
            params.node?.setSelected(false);
          } else {
            params.addRowToSelectedRows(id);
            params.node?.setSelected(true);
          }
        }}
      />
      <IconButton
        onClick={async () => {
          const request: ForfeitureAdjustmentUpdateRequest = {
            badgeNumber: params.data.badgeNumber,
            forfeitureAmount: -(currentValue || 0), // NEGATED for unforfeit
            profitYear: selectedProfitYear,
            offsettingProfitDetailId: params.data.profitDetailId, // Required for tracking
            classAction: false
          };
          await params.onSave(request, employeeName);
        }}
        disabled={isDisabled}>
        {isLoading ? <CircularProgress size={20} /> : <SaveOutlined />}
      </IconButton>
    </div>
  );
}
```

**Read-Only Mode**:
When `isReadOnly` is true:

- Editing is disabled
- Checkboxes and save buttons are disabled
- Tooltips explain "You are in read-only mode and cannot save changes."

**File Location**: `/src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeitProfitDetailGridColumns.tsx`

---

### 6. `UnForfeitHeaderComponent.tsx`

**Purpose**: Custom header component for save button column with bulk save functionality

**Responsibilities**:

- Render bulk save checkbox in column header
- Determine which rows are eligible for bulk save
- Create update payloads for bulk operations
- Check if any saves are in progress

**Key Functions**:

#### `isNodeEligible`

Determines if a row can be included in bulk save:

```typescript
const isNodeEligible = (nodeData, context) => {
  if (!nodeData.isDetail) return false; // Only detail rows

  const baseRowKey = nodeData.profitDetailId;
  const editedValues = context?.editedValues || {};
  const matchingKey = baseRowKey in editedValues ? baseRowKey : undefined;
  const currentValue = matchingKey ? editedValues[matchingKey]?.value : nodeData.suggestedUnforfeiture;

  return (currentValue || 0) !== 0; // Non-zero values only
};
```

#### `createUpdatePayload`

Creates API request payload for a row:

```typescript
const createUpdatePayload = (nodeData, context): ForfeitureAdjustmentUpdateRequest => {
  const baseRowKey = nodeData.profitDetailId;
  const editedValues = context?.editedValues || {};
  const matchingKey = baseRowKey in editedValues ? baseRowKey : undefined;
  const currentValue = matchingKey ? editedValues[matchingKey]?.value : nodeData.suggestedUnforfeiture;

  return {
    badgeNumber: Number(nodeData.badgeNumber),
    profitYear: profitYear, // Active profit year
    forfeitureAmount: -(currentValue || 0), // NEGATED for unforfeit
    offsettingProfitDetailId: nodeData.profitDetailId, // Required for tracking which forfeiture is being reversed
    classAction: false
  };
};
```

**Critical Note**: The `offsettingProfitDetailId` field is **required** for unforfeit operations. It tracks which original forfeiture record is being reversed.

#### `hasSavingInProgress`

Checks if any rows are currently being saved:

```typescript
const hasSavingInProgress = (): boolean => {
  return params.context?.loadingRowIds?.size > 0;
};
```

**Component Rendering**:

```typescript
return (
  <SelectableGridHeader
    {...params}
    isNodeEligible={isNodeEligible}
    createUpdatePayload={createUpdatePayload}
    isBulkSaving={hasSavingInProgress}
    isReadOnly={params.isReadOnly}
  />
);
```

The `SelectableGridHeader` component (from `/src/ui/src/components/SelectableGridHeader.tsx`) handles:

- Rendering the bulk save checkbox
- Selecting/deselecting all eligible rows
- Triggering bulk save operation
- Disabling during save operations

**File Location**: `/src/ui/src/pages/DecemberActivities/UnForfeit/UnForfeitHeaderComponent.tsx`

---

## State Management

### useUnForfeitState Hook

**Purpose**: Centralized state management for UnForfeit page using reducer pattern

**State Structure**:

```typescript
interface UnForfeitState {
  initialSearchLoaded: boolean; // Whether first search has been performed
  resetPageFlag: boolean; // Toggle to trigger pagination reset
  hasUnsavedChanges: boolean; // Tracks pending edits
  shouldBlock: boolean; // Whether to block navigation
  previousStatus: string | null; // Previously selected status
  shouldArchive: boolean; // Archive mode flag
}
```

**Actions**:

| Action Type                 | Payload                                   | Description                                         |
| --------------------------- | ----------------------------------------- | --------------------------------------------------- |
| `SET_INITIAL_SEARCH_LOADED` | `boolean`                                 | Mark whether initial search has been performed      |
| `TOGGLE_RESET_PAGE_FLAG`    | None                                      | Toggle pagination reset flag                        |
| `SET_UNSAVED_CHANGES`       | `boolean`                                 | Set unsaved changes flag                            |
| `SET_SHOULD_BLOCK`          | `boolean`                                 | Set navigation block flag                           |
| `SET_STATUS_CHANGE`         | `{ status: string, statusName?: string }` | Handle status change, trigger archive if "Complete" |
| `SET_ARCHIVE_HANDLED`       | None                                      | Clear archive flag after processing                 |
| `RESET_STATE`               | None                                      | Reset to initial state                              |

**Reducer Logic**:

```typescript
function unForfeitReducer(state: UnForfeitState, action: UnForfeitAction): UnForfeitState {
  switch (action.type) {
    case "SET_STATUS_CHANGE": {
      const { status, statusName } = action.payload;
      const isCompleteLike = (statusName ?? "").toLowerCase().includes("complete");
      const isChangingToComplete = isCompleteLike && state.previousStatus !== status;

      return {
        ...state,
        previousStatus: status,
        shouldArchive: isChangingToComplete // Trigger archive when changing to "Complete"
      };
    }

    case "SET_UNSAVED_CHANGES":
      return {
        ...state,
        hasUnsavedChanges: action.payload,
        shouldBlock: action.payload // Block navigation when unsaved changes exist
      };

    // ... other cases
  }
}
```

**Exported Actions**:

```typescript
const { state, actions } = useUnForfeitState();

actions.setInitialSearchLoaded(true);
actions.handleSearch(); // Toggles resetPageFlag
actions.handleUnsavedChanges(true);
actions.setShouldBlock(true);
actions.handleStatusChange("complete", "Complete");
actions.handleArchiveHandled();
```

**Usage in UnForfeit.tsx**:

```typescript
const { state, actions } = useUnForfeitState();

// Use navigation guard to block navigation when unsaved changes
useUnsavedChangesGuard(state.hasUnsavedChanges);

// Auto-trigger search when status changes to "Complete"
useEffect(() => {
  if (state.shouldArchive) {
    actions.handleSearch();
  }
}, [state.shouldArchive, actions]);
```

**File Location**: `/src/ui/src/hooks/useUnForfeitState.ts`

---

## Data Flow

### Search Flow

```
User clicks "SEARCH"
    ↓
UnForfeitSearchFilter validates form
    ↓
triggerSearch() called (RTK Query lazy query)
    ↓
setUnForfeitsQueryParams() (Redux action)
    ↓
API request to /api/unforfeits
    ↓
Response cached by RTK Query
    ↓
useUnForfeitGrid hook receives data
    ↓
flattenMasterDetailData() transforms data
    ↓
Grid displays master rows
    ↓
User clicks expansion arrow
    ↓
handleRowExpansion() toggles expansion
    ↓
Grid displays detail rows
```

### Save Flow (Individual)

```
User edits "Suggested Unforfeiture" cell
    ↓
Cell editor captures value
    ↓
editedValues context updated with rowKey -> value
    ↓
User clicks save button
    ↓
handleSave() called from useUnForfeitGrid
    ↓
prepareSaveRequest() transforms value (negates for unforfeit)
    ↓
updateForfeiture mutation (RTK Query)
    ↓
API request to /api/forfeiture-adjustments
    ↓
Success:
  - generateSaveSuccessMessage()
  - Remove from editedValues
  - Trigger data refresh
  - Clear row selection
  - Display success message
    ↓
Error:
  - Display error message via ApiMessageAlert
  - Keep edited value
  - Row remains selected
```

### Bulk Save Flow

```
User edits multiple cells
    ↓
Each edit tracked in editedValues context
    ↓
User selects checkboxes for rows to save
    ↓
User clicks bulk save checkbox in header
    ↓
handleBulkSave() called from useUnForfeitGrid
    ↓
Filter selected rows using isNodeEligible()
    ↓
Create payloads using createUpdatePayload()
    ↓
Execute saves sequentially with handleBatchOperations()
    ↓
For each row:
  - Mark as loading (add to loadingRowIds)
  - Call updateForfeiture mutation
  - Handle success/error
  - Remove from loading
    ↓
After all complete:
  - Display summary message (X succeeded, Y failed)
  - Refresh grid data
  - Clear selections
  - Clear edited values
```

### Archive Flow

```
User changes status to "Complete"
    ↓
StatusDropdownActionNode calls handleStatusChange()
    ↓
useUnForfeitState reducer sets shouldArchive = true
    ↓
UnForfeit.tsx useEffect detects shouldArchive
    ↓
actions.handleSearch() called (triggers search)
    ↓
Search executes with current filters + archive flag
    ↓
Grid displays archived data
    ↓
actions.handleArchiveHandled() called
    ↓
shouldArchive reset to false
```

---

## Grid Architecture

### Master-Detail Row Structure

The grid uses a **flattened master-detail structure** where both master and detail rows exist in a single flat array with type indicators:

```typescript
// Master row (employee summary)
{
  badgeNumber: 12345,
  fullName: "John Doe",
  ssn: "123-45-6789",
  hireDate: "2020-01-15",
  reHiredDate: "2023-06-01",
  netBalanceLastYear: 5000.00,
  vestedBalanceLastYear: 3000.00,
  store: 101,
  companyContributionYears: 3,
  enrollmentId: 5,
  enrollmentName: "Standard",
  isExpandable: true,   // Can be expanded
  isExpanded: false,    // Currently collapsed
  isDetail: false       // This is a master row
}

// Detail rows (profit year records - only visible when expanded)
{
  badgeNumber: 12345,
  profitYear: 2021,
  hoursTransactionYear: 2080,
  wagesTransactionYear: 52000.00,
  forfeiture: -1000.00,
  suggestedUnforfeiture: 1000.00,
  remark: "Rehired employee",
  profitDetailId: 98765,  // Unique ID for this profit year record
  isDetail: true,         // This is a detail row
  fullName: "John Doe"    // Copied from master for display
}
```

### Data Transformation

The `flattenMasterDetailData()` utility (from `/src/ui/src/utils/forfeitActivities/gridDataHelpers.ts`) transforms hierarchical data into this flat structure:

```typescript
// Input: Hierarchical structure
const masterData = [
  {
    badgeNumber: 12345,
    fullName: "John Doe",
    profitYearDetails: [
      { profitYear: 2021, forfeiture: -1000.00, ... },
      { profitYear: 2022, forfeiture: -500.00, ... }
    ]
  }
];

// Output: Flat array with type indicators
const flatData = flattenMasterDetailData(
  masterData,
  expandedRows, // Set<string> of expanded badgeNumbers
  {
    activityType: "unforfeit",
    getRowKey: (row) => row.badgeNumber.toString(),
    getDetailRows: (row) => row.profitYearDetails,
    shouldShowDetails: true
  }
);

// Result:
[
  { badgeNumber: 12345, fullName: "John Doe", isExpandable: true, isExpanded: true, isDetail: false },
  { badgeNumber: 12345, profitYear: 2021, forfeiture: -1000.00, isDetail: true, fullName: "John Doe" },
  { badgeNumber: 12345, profitYear: 2022, forfeiture: -500.00, isDetail: true, fullName: "John Doe" }
]
```

### Row Expansion Logic

```typescript
const handleRowExpansion = useCallback((badgeNumber: string) => {
  setExpandedRows((prev) => {
    const newSet = new Set(prev);
    if (newSet.has(badgeNumber)) {
      newSet.delete(badgeNumber); // Collapse
    } else {
      newSet.add(badgeNumber); // Expand
    }
    return newSet;
  });
}, []);
```

When a row is expanded:

1. Badge number added to `expandedRows` Set
2. `flattenMasterDetailData()` re-runs with updated Set
3. Detail rows inserted after master row in flat array
4. Grid re-renders with detail rows visible

### Column Visibility Logic

Columns use custom cell renderers to control visibility:

```typescript
// Main columns: Hide for detail rows (unless shared field)
const visibleColumns = mainColumns.map((column) => ({
  ...column,
  cellRenderer: (params) => {
    if (params.data?.isDetail) {
      const hideInDetails = !detailColumns.some((detailCol) => detailCol.field === column.field);
      if (hideInDetails) {
        return ""; // Hide this field for detail rows
      }
    }
    // Render normally for master rows
    return /* value */;
  }
}));

// Detail-only columns: Hide for master rows
const detailOnlyColumns = detailColumns
  .filter((col) => !mainColumns.some((mainCol) => mainCol.field === col.field))
  .map((column) => ({
    ...column,
    cellRenderer: (params) => {
      if (!params.data?.isDetail) {
        return ""; // Hide for master rows
      }
      return; /* value */ // Show for detail rows
    }
  }));
```

This creates a unified grid where:

- Master rows show employee summary columns
- Detail rows show profit year columns
- Some columns (like badge number) can appear in both

---

## Key Features

### 1. Inline Editing

**Implementation**:

- Uses AG Grid's built-in editing with custom `SuggestedForfeitEditor`
- Edits tracked in grid context: `context.editedValues[rowKey] = { value: newValue }`
- Row key for unforfeit is `profitDetailId` (unique per profit year record)

**Validation**:

- Cannot save zero values
- Cannot edit when read-only
- Original value preserved until save succeeds

**Persistence**:

- Edited values persist across pagination
- Cleared on successful save
- Cleared on reset

### 2. Bulk Operations

**Selection**:

- Checkboxes in save button column
- Bulk checkbox in column header selects/deselects all eligible rows
- Eligibility: `isDetail === true && (currentValue || 0) !== 0`

**Batch Save**:

```typescript
const handleBatchOperations = async (
  requests: ForfeitureAdjustmentUpdateRequest[],
  names: string[],
  batchSize: number = 5
) => {
  const results = { succeeded: [], failed: [] };

  for (let i = 0; i < requests.length; i += batchSize) {
    const batch = requests.slice(i, i + batchSize);
    const batchNames = names.slice(i, i + batchSize);

    const batchResults = await Promise.allSettled(batch.map((req, idx) => updateForfeiture(req).unwrap()));

    batchResults.forEach((result, idx) => {
      if (result.status === "fulfilled") {
        results.succeeded.push(batchNames[idx]);
      } else {
        results.failed.push({ name: batchNames[idx], error: result.reason });
      }
    });
  }

  return results;
};
```

**Progress Indication**:

- `loadingRowIds` Set tracks rows currently being saved
- Save buttons show `CircularProgress` spinner
- Bulk save checkbox disabled during operation

### 3. Value Transformation

**Critical Pattern**: Unforfeiture values are **negated** before sending to API

```typescript
// User enters positive value in UI: 1000.00
// API expects negative value for unforfeit: -1000.00

// Transformation in prepareSaveRequest():
forfeitureAmount: transformForfeitureValue("unforfeit", request.forfeitureAmount);

// transformForfeitureValue() implementation:
export function transformForfeitureValue(activityType: ActivityType, value: number): number {
  if (activityType === "unforfeit") {
    return -(value || 0); // NEGATE for unforfeit
  }
  return value || 0; // No transformation for termination
}
```

**Why?**

- Forfeitures are stored as negative values in the database
- Unforfeitures reverse forfeitures by adding a positive offsetting entry
- The negation converts user-entered positive values to the required format

### 4. Row Keys and Tracking

**UnForfeit uses `profitDetailId` as row key**:

```typescript
// From gridDataHelpers.ts
export function generateRowKey(config: RowKeyConfig, params: RowKeyParams): string {
  if (config.type === "unforfeit") {
    return params.profitDetailId.toString(); // UnForfeit uses profitDetailId
  }
  // Termination uses composite key: `${badgeNumber}-${profitYear}`
}
```

**Why `profitDetailId`?**

- Each profit year record has a unique `profitDetailId`
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

### 5. Read-Only Mode

**Hook**: `useReadOnlyNavigation()`

**Effects when read-only**:

1. **Editing disabled**: `editable` callback returns false
2. **Checkboxes disabled**: Selection checkboxes are disabled
3. **Save buttons disabled**: Individual and bulk save buttons disabled
4. **Tooltips**: "You are in read-only mode and cannot save changes."
5. **Status dropdown disabled**: Cannot change page status

**Implementation**:

```typescript
const isReadOnly = useReadOnlyNavigation();

// In column definitions
editable: (params) => params.data.suggestedUnforfeiture != null && !isReadOnly

// In cell renderer
<IconButton disabled={isDisabled || isReadOnly}>
  <SaveOutlined />
</IconButton>
```

### 6. Unsaved Changes Guard

**Hook**: `useUnsavedChangesGuard(hasUnsavedChanges)`

**Behavior**:

- Blocks navigation when `hasUnsavedChanges === true`
- Shows browser confirmation dialog: "You have unsaved changes. Are you sure you want to leave?"
- Applies to:
  - Route navigation (React Router)
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
handleReset() => setHasUnsavedChanges(false);

// 3. User confirms navigation
// (browser confirmation dialog)
```

### 7. Archive Mode

**Trigger**: Status change to "Complete" (or any status name containing "complete")

**Flow**:

1. User changes status via `StatusDropdownActionNode`
2. `handleStatusChange()` detects "complete" status
3. `shouldArchive` flag set to true
4. `UnForfeit.tsx` useEffect detects flag
5. `actions.handleSearch()` called automatically
6. Search executes with current filters
7. Grid displays results
8. `handleArchiveHandled()` clears flag

**Purpose**: Automatically refresh data when marking page as complete

---

## Shared Utilities

The UnForfeit feature uses shared utilities from `/src/ui/src/utils/forfeitActivities/` to maintain consistency with the Termination feature.

### 1. `gridDataHelpers.ts`

**Functions Used**:

#### `generateRowKey()`

```typescript
const rowKey = generateRowKey(
  { type: "unforfeit" },
  {
    badgeNumber: row.badgeNumber,
    profitYear: row.profitYear,
    profitDetailId: row.profitDetailId
  }
);
// Returns: profitDetailId.toString() for unforfeit
```

#### `transformForfeitureValue()`

```typescript
const transformedValue = transformForfeitureValue("unforfeit", 1000.0);
// Returns: -1000.00 (negated for unforfeit)
```

#### `flattenMasterDetailData()`

```typescript
const flatData = flattenMasterDetailData(masterData, expandedRows, {
  activityType: "unforfeit",
  getRowKey: (row) => row.badgeNumber.toString(),
  getDetailRows: (row) => row.profitYearDetails,
  shouldShowDetails: true
});
// Returns: Flat array with master and detail rows
```

### 2. `saveOperationHelpers.ts`

**Functions Used**:

#### `prepareSaveRequest()`

```typescript
const preparedRequest = prepareSaveRequest({ activityType: "unforfeit", rowKeyConfig: { type: "unforfeit" } }, request);
// Transforms forfeitureAmount by negating
```

#### `generateSaveSuccessMessage()`

```typescript
const message = generateSaveSuccessMessage("unforfeit", "John Doe", -1000.0);
// Returns: "Successfully saved unforfeiture of $1,000.00 for John Doe"
```

#### `extractRowKey()`

```typescript
const rowKey = extractRowKey({ type: "unforfeit" }, rowData, { editedValues: context.editedValues });
// Returns: profitDetailId
```

#### `clearGridSelections()`

```typescript
clearGridSelections(gridRef, selectionState);
// Clears all row selections and resets selection state
```

#### `handleBatchOperations()`

```typescript
const results = await handleBatchOperations(
  requests,
  names,
  async (request, name) => {
    await updateForfeiture(request).unwrap();
    return generateSaveSuccessMessage("unforfeit", name, request.forfeitureAmount);
  },
  5 // batch size
);
// Returns: { succeeded: string[], failed: Array<{name: string, error: any}> }
```

**Benefits**:

- Consistent behavior between Termination and UnForfeit
- DRY (Don't Repeat Yourself) principle
- Single source of truth for business logic
- Easier to maintain and test

---

## Redux Integration

### RTK Query API Endpoints

**Search Endpoint**:

```typescript
const [triggerSearch, { data, isFetching }] = useLazyGetUnForfeitsQuery();

// Execute search
triggerSearch({
  beginningDate: "01/01/2024",
  endingDate: "12/31/2024",
  excludeZeroBalance: true,
  profitYear: 2024,
  pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false }
});

// Response structure
{
  response: {
    results: UnForfeitMasterDto[],
    total: number
  }
}
```

**Update Endpoint**:

```typescript
const [updateForfeiture, { isLoading, error }] = useUpdateForfeitureAdjustmentMutation();

// Execute update
await updateForfeiture({
  badgeNumber: 12345,
  profitYear: 2024,
  forfeitureAmount: -1000.0, // Negated for unforfeit
  offsettingProfitDetailId: 98765,
  classAction: false
}).unwrap();
```

### Redux Slices

**Query Parameters Slice** (`yearsEndSlice.ts`):

```typescript
// Actions
setUnForfeitsQueryParams(params) - Store search parameters
clearUnForfeitsQueryParams() - Clear search parameters
clearUnForfeitsDetails() - Clear search results

// State
unForfeitsQueryParams: {
  beginningDate: string,
  endingDate: string,
  excludeZeroBalance: boolean,
  profitYear: number,
  pagination: { skip, take, sortBy, isSortDescending }
}
```

**Why Store Query Params?**

- Persist search criteria across pagination
- Restore search state when navigating back to page
- Support browser refresh without losing context

---

## Hooks Reference

### 1. `useUnForfeitGrid`

**Purpose**: Main business logic hook for grid operations

**Parameters**:

```typescript
interface UseUnForfeitGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  resetPageFlag: boolean;
  onUnsavedChanges: (hasChanges: boolean) => void;
  hasUnsavedChanges: boolean;
  shouldArchive: boolean;
  onArchiveHandled?: () => void;
  setHasUnsavedChanges: (hasChanges: boolean) => void;
  fiscalCalendarYear: CalendarResponseDto | null;
}
```

**Returns**:

```typescript
{
  pageNumber: number,
  pageSize: number,
  gridData: Array<MasterRow | DetailRow>,
  unForfeits: UnForfeitsResponse,
  selectedProfitYear: number,
  selectionState: {
    selectedRowIds: Set<number>,
    addRowToSelection: (id: number) => void,
    removeRowFromSelection: (id: number) => void,
    clearSelections: () => void
  },
  handleSave: (request: ForfeitureAdjustmentUpdateRequest, name: string) => Promise<void>,
  handleBulkSave: (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => Promise<void>,
  handleRowExpansion: (badgeNumber: string) => void,
  sortEventHandler: (event: SortChangedEvent) => void,
  onGridReady: (params: GridReadyEvent) => void,
  paginationHandlers: {
    setPageNumber: (page: number) => void,
    setPageSize: (size: number) => void
  },
  gridRef: React.MutableRefObject<GridReadyEvent | null>,
  gridContext: {
    editedValues: Record<number, { value: number }>,
    loadingRowIds: Set<string>
  }
}
```

**Key Responsibilities**:

- Data fetching and caching
- Master-detail expansion state
- Inline editing state (`editedValues`)
- Row selection state
- Save operations (individual and bulk)
- Pagination and sorting
- Error handling and user feedback

**File Location**: `/src/ui/src/hooks/useUnForfeitGrid.ts`

### 2. `useUnForfeitState`

**Purpose**: Page-level state management

**Returns**:

```typescript
{
  state: {
    initialSearchLoaded: boolean,
    resetPageFlag: boolean,
    hasUnsavedChanges: boolean,
    shouldBlock: boolean,
    previousStatus: string | null,
    shouldArchive: boolean
  },
  actions: {
    setInitialSearchLoaded: (loaded: boolean) => void,
    handleSearch: () => void,
    handleUnsavedChanges: (hasChanges: boolean) => void,
    setShouldBlock: (shouldBlock: boolean) => void,
    handleStatusChange: (status: string, statusName?: string) => void,
    handleArchiveHandled: () => void
  }
}
```

**File Location**: `/src/ui/src/hooks/useUnForfeitState.ts`

### 3. `useDecemberFlowProfitYear`

**Purpose**: Get active profit year from context

**Returns**: `number` (e.g., 2024)

### 4. `useIsProfitYearFrozen`

**Purpose**: Check if profit year is frozen

**Parameters**: `profitYear: number`

**Returns**: `boolean`

### 5. `useUnsavedChangesGuard`

**Purpose**: Block navigation when unsaved changes exist

**Parameters**: `hasUnsavedChanges: boolean`

**Behavior**: Registers `beforeunload` and React Router block handlers

### 6. `useReadOnlyNavigation`

**Purpose**: Check if current navigation context requires read-only mode

**Returns**: `boolean`

### 7. `useDynamicGridHeight`

**Purpose**: Calculate optimal grid height based on viewport

**Returns**: `string` (e.g., "calc(100vh - 400px)")

---

## Type Definitions

### Request Types

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

interface ForfeitureAdjustmentUpdateRequest {
  badgeNumber: number;
  profitYear: number;
  forfeitureAmount: number; // NEGATED for unforfeit
  offsettingProfitDetailId: number; // Required for unforfeit
  classAction: boolean; // Always false for manual operations
}
```

### Response Types

```typescript
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
  profitDetailId: number; // Unique ID for this profit year record
  profitYear: number;
  hoursTransactionYear: number;
  wagesTransactionYear: number;
  forfeiture: number;
  suggestedUnforfeiture: number;
  remark: string;
}

interface UnForfeitsResponse {
  response: {
    results: UnForfeitMasterDto[];
    total: number;
  };
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
  profitDetailId: number;
  profitYear: number;
  hoursTransactionYear: number;
  wagesTransactionYear: number;
  forfeiture: number;
  suggestedUnforfeiture: number;
  remark: string;
  fullName: string; // Copied from master row
  isDetail: true;
}
```

---

## Common Patterns

### 1. Error Handling

**Pattern**: All errors displayed via `ApiMessageAlert` component with common key

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

**Pattern**: Multiple loading indicators for different operations

```typescript
// Global search loading
const { isFetching } = useLazyGetUnForfeitsQuery();

// Individual row loading
const [loadingRowIds, setLoadingRowIds] = useState<Set<string>>(new Set());

// Mutation loading
const { isLoading } = useUpdateForfeitureAdjustmentMutation();
```

**Display**:

- Search: Disable search button, show spinner
- Individual save: Show CircularProgress in save button
- Bulk save: Disable bulk checkbox, show progress for each row

### 3. Memoization

**Pattern**: Memoize expensive computations and callbacks

```typescript
// Column definitions
const mainColumns = useMemo(() => UnForfeitGridColumns(), []);
const detailColumns = useMemo(
  () => GetProfitDetailColumns(/* ... */),
  [
    /* dependencies */
  ]
);

// Callbacks
const handleSave = useCallback(
  async (request, name) => {
    // ... save logic
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
    <UnForfeitSearchFilter />
    <UnForfeitGrid />
  </>
)}

// Show grid only after search
{initialSearchLoaded && unForfeits?.response && (
  <UnForfeitGrid data={unForfeits.response} />
)}

// Show frozen year warning if applicable
{isFrozen && <FrozenYearWarning profitYear={profitYear} />}
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
  const currentValue = params.context?.editedValues?.[rowKey]?.value;
  const isLoading = params.context?.loadingRowIds?.has(rowKey);
  // ...
}
```
