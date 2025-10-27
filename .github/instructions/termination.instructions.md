---
applyTo: "src/ui/src/pages/DecemberActivities/Termination/**/*.*"
---
# Termination Component Technical Documentation

## Overview

The Termination feature allows users to process forfeitures for terminated employees during the December year-end process. The implementation follows a master-detail grid pattern with inline editing, bulk operations, comprehensive state management, and financial summary displays.

## Architecture Overview

The Termination feature implements a **Complex Page with Custom Hook** pattern (see `/src/ui/src/pages/CLAUDE.md` for pattern details). The architecture separates concerns into distinct layers:

```
┌─────────────────────────────────────────────────────────┐
│ Termination.tsx (Page Container)                        │
│ - Page wrapper and layout                               │
│ - Status management                                     │
│ - Error message handling                                │
│ - Navigation guard                                      │
└──────────────────┬──────────────────────────────────────┘
                   │
        ┌──────────┴──────────┐
        │                     │
┌───────▼──────────┐  ┌──────▼────────────────────────┐
│ TerminationSearch│  │ TerminationGrid.tsx            │
│ Filter.tsx       │  │ - Master-detail grid display  │
│ - Date range     │  │ - Financial summary totals    │
│ - Validation     │  │ - Expansion/collapse          │
│ - Duplicate SSN  │  │ - Inline editing              │
│   guard          │  │ - Pagination                  │
└──────────────────┘  └───────┬───────────────────────┘
                              │
                  ┌───────────┴──────────┐
                  │                      │
      ┌───────────▼──────────┐  ┌───────▼─────────────┐
      │ useTerminationGrid.ts│  │ Column Definitions  │
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
6. **Financial Summaries**: Displays totals for profit sharing balances and forfeitures

---

## Component Hierarchy

```
Termination (Page Container)
├── Page (from smart-ui-library)
│   ├── label: "TERMINATIONS (008-10)"
│   └── actionNode: StatusDropdownActionNode
├── ApiMessageAlert (Error/success messages)
├── Divider
├── DSMAccordion (Collapsible filter section)
│   └── TerminationSearchFilter
│       ├── DsmDatePicker (Begin Date)
│       ├── DsmDatePicker (End Date)
│       ├── DuplicateSsnGuard (Prerequisite check)
│       └── SearchAndReset (Action buttons)
└── TerminationGrid
    ├── ReportSummary (Total counts)
    ├── TotalsGrid (Financial summaries - 4 grids)
    │   ├── Amount in Profit Sharing
    │   ├── Vested Amount
    │   ├── Total Forfeitures
    │   └── Total Beneficiary Allocations
    ├── DSMGrid (AG Grid wrapper)
    │   ├── Expansion column (►/▼)
    │   ├── Main row columns (badge, name)
    │   └── Detail row columns (profit year, balances, dates, suggested forfeit)
    └── Pagination
```

---

## File Descriptions

### 1. `Termination.tsx`

**Purpose**: Main page container and orchestrator

**Responsibilities**:

- Page layout and structure
- Fiscal calendar data fetching
- Status change handling
- Error message display and scroll-to-top functionality
- Navigation guard for unsaved changes
- Search parameter management

**Key Hooks Used**:

- `useTerminationState()` - State management
- `useLazyGetAccountingRangeToCurrent(6)` - Fetch fiscal calendar data
- `useUnsavedChangesGuard(hasUnsavedChanges)` - Block navigation if unsaved changes

**State Management**:

```typescript
const { state, actions } = useTerminationState();
// state.searchParams - Current search parameters
// state.hasUnsavedChanges - Tracks pending edits
// state.initialSearchLoaded - Whether first search has been performed
// state.resetPageFlag - Triggers pagination reset
// state.shouldArchive - Archive mode flag (set when status changes to Complete)
```

**Key Features**:

#### Error Message Handling with Scroll-to-Top

The page implements a custom event listener to scroll to the top when errors occur:

```typescript
useEffect(() => {
  const handleMessageEvent = (event: Event) => {
    const customEvent = event as CustomEvent;
    if (customEvent.detail?.key === "TerminationSave" && customEvent.detail?.message?.type === "error") {
      scrollToTop();
    }
  };

  window.addEventListener("dsmMessage", handleMessageEvent);

  return () => {
    window.removeEventListener("dsmMessage", handleMessageEvent);
  };
}, [scrollToTop]);
```

**Purpose**: When a save operation fails, the error message appears at the top of the page via `ApiMessageAlert`. The event listener ensures the user sees the error by automatically scrolling to the top.

#### Loading State Display

```typescript
{!isCalendarDataLoaded ? (
  <Grid width={"100%"} container justifyContent="center" padding={4}>
    <CircularProgress />
  </Grid>
) : (
  <>
    <TerminationSearchFilter />
    <TerminationGrid />
  </>
)}
```

**File Location**: `/src/ui/src/pages/DecemberActivities/Termination/Termination.tsx`

---

### 2. `TerminationGrid.tsx`

**Purpose**: Master-detail grid display with financial summaries

**Responsibilities**:

- Grid configuration and rendering
- Master-detail row expansion/collapse
- Column definition merging (main + detail columns)
- Cell styling for detail rows
- Financial summary display (4 totals grids)
- Grid context management
- Pagination

**Key Features**:

#### Financial Summary Totals

Displays four summary grids above the main grid:

```typescript
<div className="sticky top-0 z-10 flex bg-white">
  <TotalsGrid
    displayData={[[numberToCurrency(termination.totalEndingBalance || 0)]]}
    leftColumnHeaders={["Amount in Profit Sharing"]}
    topRowHeaders={[]}
  />
  <TotalsGrid
    displayData={[[numberToCurrency(termination.totalVested || 0)]]}
    leftColumnHeaders={["Vested Amount"]}
    topRowHeaders={[]}
  />
  <TotalsGrid
    displayData={[[numberToCurrency(termination.totalForfeit || 0)]]}
    leftColumnHeaders={["Total Forfeitures"]}
    topRowHeaders={[]}
  />
  <TotalsGrid
    displayData={[[numberToCurrency(termination.totalBeneficiaryAllocation || 0)]]}
    leftColumnHeaders={["Total Beneficiary Allocations"]}
    topRowHeaders={[]}
  />
</div>
```

**Sticky Positioning**: The `sticky top-0 z-10` classes ensure the totals remain visible when scrolling the grid.

**Financial Totals Explained**:

- **Amount in Profit Sharing**: Total ending balance across all terminated employees
- **Vested Amount**: Total vested balance (amount employees are entitled to)
- **Total Forfeitures**: Sum of all forfeitures being processed
- **Total Beneficiary Allocations**: Sum of distributions to beneficiaries

#### Master-Detail Pattern

The grid displays two types of rows:

1. **Master rows**: Employee summary (badge, name)
2. **Detail rows**: Individual profit year records (year, balances, dates, suggested forfeit)

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
const getRowClass = (params: { data: { isDetail: boolean } }) => {
  return params.data.isDetail ? "bg-gray-100" : "";
};
```

Detail rows have gray background (`bg-gray-100` Tailwind class) to visually distinguish them from master rows.

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

**File Location**: `/src/ui/src/pages/DecemberActivities/Termination/TerminationGrid.tsx`

---

### 3. `TerminationSearchFilter.tsx`

**Purpose**: Search and filter form with date range selection and duplicate SSN prerequisite

**Responsibilities**:

- Date range validation (begin/end dates within fiscal year)
- Form validation using React Hook Form + Yup
- Search parameter management
- Reset functionality
- Duplicate SSN prerequisite check via `DuplicateSsnGuard`

**Validation Schema**:

```typescript
const schema = yup.object().shape({
  beginningDate: dateStringValidator(2000, 2099, "Beginning Date").required(),
  endingDate: endDateStringAfterStartDateValidator(
    "beginningDate",
    tryddmmyyyyToDate,
    "Ending date must be the same or after the beginning date"
  ).required(),
  forfeitureStatus: yup.string().required("Forfeiture Status is required"),
  profitYear: profitYearValidator(2015, 2099)
});
```

**Key Features**:

1. **Date Range Selection**:

   - Begin date: Termination begin date (min: fiscal begin, max: fiscal end)
   - End date: Termination end date (min: begin date or fiscal begin, max: fiscal end)
   - Both dates use `DsmDatePicker` with `disableFuture`

2. **Duplicate SSN Guard**:

   ```typescript
   <DuplicateSsnGuard mode="warning">
     {({ prerequisitesComplete }) => (
       <SearchAndReset
         handleReset={handleReset}
         handleSearch={validateAndSearch}
         disabled={!isValid || !prerequisitesComplete}
       />
     )}
   </DuplicateSsnGuard>
   ```

   **Purpose**: The `DuplicateSsnGuard` component checks for duplicate SSNs in the system. If duplicates exist, the search button is disabled and a warning is displayed. This ensures data integrity before processing terminations.

3. **Validation and Submit**:

   ```typescript
   const validateAndSubmit = async (data: TerminationSearchRequest) => {
     if (hasUnsavedChanges) {
       alert("Please save your changes.");
       return;
     }

     const params = {
       ...data,
       profitYear: selectedProfitYear,
       beginningDate: data.beginningDate
         ? mmDDYYFormat(data.beginningDate)
         : mmDDYYFormat(fiscalData?.fiscalBeginDate || ""),
       endingDate: data.endingDate ? mmDDYYFormat(data.endingDate) : mmDDYYFormat(fiscalData?.fiscalEndDate || "")
     };

     onSearch(params);
     setInitialSearchLoaded(true);
   };
   ```

4. **Reset Functionality**:
   ```typescript
   const handleReset = async () => {
     setInitialSearchLoaded(false);
     reset({
       beginningDate: fiscalData ? mmDDYYFormat(fiscalData.fiscalBeginDate) : "",
       endingDate: fiscalData ? mmDDYYFormat(fiscalData.fiscalEndDate) : "",
       forfeitureStatus: "showAll",
       pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false },
       profitYear: selectedProfitYear
     });
     await trigger();
     dispatch(clearTermination());
   };
   ```

**File Location**: `/src/ui/src/pages/DecemberActivities/Termination/TerminationSearchFilter.tsx`

---

### 4. `TerminationGridColumns.tsx`

**Purpose**: Column definitions for master (employee summary) rows

**Columns**:

| Column       | Field         | Type   | Description                                       |
| ------------ | ------------- | ------ | ------------------------------------------------- |
| Badge Number | `badgeNumber` | Number | Employee badge number with PSN suffix (navigable) |
| Name         | `name`        | String | Employee full name                                |

**Minimal Master Columns**: Unlike UnForfeit, Termination only shows badge and name in master rows. All other employee details are in the detail rows.

**PSN Suffix**: The badge column includes a PSN (Person Number) suffix option, which displays an additional identifier after the badge number when available.

**Column Factory Usage**:

```typescript
export const GetTerminationColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      psnSuffix: true // Shows additional identifier
    }),
    createNameColumn({
      field: "name"
    })
  ];
};
```

**File Location**: `/src/ui/src/pages/DecemberActivities/Termination/TerminationGridColumns.tsx`

---

### 5. `TerminationDetailsGridColumns.tsx`

**Purpose**: Column definitions for detail (profit year) rows with inline editing

**Columns**:

| Column                 | Field                   | Type     | Editable | Description                                        |
| ---------------------- | ----------------------- | -------- | -------- | -------------------------------------------------- |
| Profit Year            | `profitYear`            | Year     | No       | Year of profit sharing record                      |
| Beginning Balance      | `beginningBalance`      | Currency | No       | Balance at start of year                           |
| Beneficiary Allocation | `beneficiaryAllocation` | Currency | No       | Amount allocated to beneficiaries                  |
| Distribution Amount    | `distributionAmount`    | Currency | No       | Amount distributed during year                     |
| Forfeit Amount         | `forfeit`               | Currency | No       | Amount already forfeited                           |
| Ending Balance         | `endingBalance`         | Currency | No       | Balance at end of year                             |
| Vested Balance         | `vestedBalance`         | Currency | No       | Vested portion of ending balance                   |
| Vested %               | `vestedPercent`         | Percent  | No       | Percentage of balance that is vested               |
| Term Date              | `dateTerm`              | Date     | No       | Termination date                                   |
| YTD PS Hours           | `ytdPsHours`            | Hours    | No       | Year-to-date profit sharing hours                  |
| Age                    | `age`                   | Number   | No       | Employee age at termination                        |
| Forfeited              | `hasForfeited`          | Yes/No   | No       | Whether forfeiture has been processed              |
| Suggested Forfeit      | `suggestedForfeit`      | Currency | Yes      | Amount to forfeit (editable for current year only) |
| Save Button            | (special)               | Action   | N/A      | Checkbox + Save button (pinned right)              |

**Inline Editing**:

The "Suggested Forfeit" column uses custom editor and renderer:

```typescript
{
  headerName: "Suggested Forfeit",
  field: "suggestedForfeit",
  editable: ({ node }) => node.data.isDetail && node.data.profitYear === selectedProfitYear,
  cellEditor: SuggestedForfeitEditor,
  cellRenderer: (params) => SuggestedForfeitCellRenderer(
    { ...params, selectedProfitYear },
    true,  // isTermination = true
    false  // isUnforfeit = false
  ),
  valueFormatter: (params) => numberToCurrency(params.value),
  valueGetter: (params) => {
    if (!params.data.isDetail) return params.data.suggestedForfeit;
    const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
    const editedValue = params.context?.editedValues?.[rowKey]?.value;
    return editedValue ?? params.data.suggestedForfeit ?? 0;
  }
}
```

**Editable Only for Current Year**: Users can only edit forfeitures for the selected profit year (current year). Historical years are read-only.

**Error Highlighting**:

```typescript
cellClass: (params) => {
  if (!params.data.isDetail) return "";
  const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
  const hasError = params.context?.editedValues?.[rowKey]?.hasError;
  return hasError ? "bg-red-50" : "";
};
```

Cells with validation errors are highlighted with a red background.

**Save Button Column**:

The save button column includes:

1. **Header**: Custom `HeaderComponent` with bulk save checkbox
2. **Cell**: Checkbox (for bulk selection) + Save button (for individual save)

Cell Renderer Logic:

```typescript
cellRenderer: (params: SaveButtonCellParams) => {
  if (!params.data.isDetail || params.data.profitYear !== selectedProfitYear) {
    return ""; // Only show for current year detail rows
  }

  const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
  const hasError = params.context?.editedValues?.[rowKey]?.hasError;
  const currentValue = params.context?.editedValues?.[rowKey]?.value ?? params.data.suggestedForfeit;
  const isLoading = params.context?.loadingRowIds?.has(params.data.badgeNumber);
  const isZeroValue = currentValue === 0 || currentValue === null || currentValue === undefined;
  const isDisabled = hasError || isLoading || isZeroValue || isReadOnly;

  return (
    <div>
      <Tooltip title={isZeroValue ? "Forfeit cannot be zero." : isReadOnly ? "You are in read-only mode" : ""}>
        <Checkbox
          checked={isSelected}
          disabled={isDisabled}
          onChange={() => {
            if (isSelected) {
              params.removeRowFromSelectedRows(id);
            } else {
              params.addRowToSelectedRows(id);
            }
            params.node?.setSelected(!isSelected);
          }}
        />
      </Tooltip>
      <IconButton
        onClick={async () => {
          const request: ForfeitureAdjustmentUpdateRequest = {
            badgeNumber: params.data.badgeNumber,
            profitYear: params.data.profitYear,
            forfeitureAmount: currentValue || 0, // NOT negated (only UnForfeit negates)
            classAction: false,
            offsettingProfitDetailId: undefined
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

**Tooltips**: Disabled checkboxes show explanatory tooltips (zero value, read-only mode).

**Read-Only Mode**:
When `isReadOnly` is true:

- Editing is disabled
- Checkboxes and save buttons are disabled
- Tooltips explain "You are in read-only mode and cannot save changes."

**File Location**: `/src/ui/src/pages/DecemberActivities/Termination/TerminationDetailsGridColumns.tsx`

---

### 6. `TerminationHeaderComponent.tsx`

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
  if (!nodeData.isDetail || nodeData.profitYear !== selectedProfitYear) return false;

  const rowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}`;
  const currentValue = context?.editedValues?.[rowKey]?.value ?? nodeData.suggestedForfeit;

  return (currentValue || 0) !== 0; // Non-zero values only
};
```

**Eligibility Criteria**:

1. Must be a detail row (`isDetail === true`)
2. Must be for the current/selected profit year
3. Must have a non-zero forfeit value

#### `createUpdatePayload`

Creates API request payload for a row:

```typescript
const createUpdatePayload = (nodeData, context): ForfeitureAdjustmentUpdateRequest => {
  const rowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}`;
  const currentValue = context?.editedValues?.[rowKey]?.value ?? nodeData.suggestedForfeit;

  return {
    badgeNumber: Number(nodeData.badgeNumber),
    profitYear: nodeData.profitYear,
    forfeitureAmount: currentValue || 0, // Termination does NOT negate (only UnForfeit negates)
    classAction: false
  };
};
```

**Critical Note**: Unlike UnForfeit, Termination does **NOT** negate the forfeiture amount. The value is sent as-is to the API.

**No `offsettingProfitDetailId`**: Termination does not require tracking an offsetting record, so this field is omitted (or set to `undefined`).

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
    onBulkSave={params.onBulkSave}
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

**File Location**: `/src/ui/src/pages/DecemberActivities/Termination/TerminationHeaderComponent.tsx`

---

## State Management

### useTerminationState Hook

**Purpose**: Centralized state management for Termination page using reducer pattern

**State Structure**:

```typescript
interface TerminationState {
  searchParams: TerminationSearchRequest | null; // Current search parameters
  initialSearchLoaded: boolean; // Whether first search has been performed
  hasUnsavedChanges: boolean; // Tracks pending edits
  resetPageFlag: boolean; // Toggle to trigger pagination reset
  currentStatus: string | null; // Currently selected status
  archiveMode: boolean; // Whether archive mode is active
  shouldArchive: boolean; // Archive flag
}
```

**Actions**:

| Action Type                 | Payload                                   | Description                                         |
| --------------------------- | ----------------------------------------- | --------------------------------------------------- |
| `SET_SEARCH_PARAMS`         | `TerminationSearchRequest`                | Set search params and mark search as loaded         |
| `SET_INITIAL_SEARCH_LOADED` | `boolean`                                 | Mark whether initial search has been performed      |
| `SET_UNSAVED_CHANGES`       | `boolean`                                 | Set unsaved changes flag                            |
| `TOGGLE_RESET_PAGE_FLAG`    | None                                      | Toggle pagination reset flag                        |
| `SET_STATUS_CHANGE`         | `{ status: string, statusName?: string }` | Handle status change, trigger archive if "Complete" |
| `SET_ARCHIVE_HANDLED`       | None                                      | Clear archive flag after processing                 |
| `RESET_STATE`               | None                                      | Reset to initial state                              |

**Reducer Logic**:

```typescript
function terminationReducer(state: TerminationState, action: TerminationAction): TerminationState {
  switch (action.type) {
    case "SET_STATUS_CHANGE": {
      const { statusName } = action.payload;
      const isCompleteLike = (statusName ?? "").toLowerCase().includes("complete");
      const isChangingToComplete = isCompleteLike && state.currentStatus !== statusName;

      if (isChangingToComplete) {
        // Entering archive mode
        const updatedSearchParams = state.searchParams ? { ...state.searchParams, archive: true } : null;

        return {
          ...state,
          currentStatus: statusName || null,
          archiveMode: true,
          shouldArchive: true,
          searchParams: updatedSearchParams,
          resetPageFlag: !state.resetPageFlag
        };
      } else {
        // Exiting archive mode
        const shouldResetArchive = !isCompleteLike;
        let updatedSearchParams = state.searchParams;

        if (shouldResetArchive && state.searchParams) {
          // Remove archive flag from search params
          const { archive, ...paramsWithoutArchive } = state.searchParams as TerminationSearchRequest & {
            archive?: boolean;
          };
          updatedSearchParams = paramsWithoutArchive as TerminationSearchRequest;
        }

        return {
          ...state,
          currentStatus: statusName || null,
          archiveMode: shouldResetArchive ? false : state.archiveMode,
          searchParams: shouldResetArchive ? updatedSearchParams : state.searchParams,
          resetPageFlag: shouldResetArchive ? !state.resetPageFlag : state.resetPageFlag
        };
      }
    }

    case "SET_SEARCH_PARAMS":
      return {
        ...state,
        searchParams: action.payload,
        initialSearchLoaded: true,
        resetPageFlag: !state.resetPageFlag
      };

    case "SET_UNSAVED_CHANGES":
      return {
        ...state,
        hasUnsavedChanges: action.payload
      };

    // ... other cases
  }
}
```

**Archive Mode Logic**:

- When status changes to "Complete", `archive: true` is added to search params
- Search is re-triggered with archive flag
- When status changes away from "Complete", archive flag is removed
- This allows viewing "archived" (historical) termination data

**Exported Actions**:

```typescript
const { state, actions } = useTerminationState();

actions.handleSearch(searchParams);
actions.handleUnsavedChanges(true);
actions.handleStatusChange("complete", "Complete");
actions.handleArchiveHandled();
actions.setInitialSearchLoaded(true);
```

**Usage in Termination.tsx**:

```typescript
const { state, actions } = useTerminationState();

// Use navigation guard to block navigation when unsaved changes
useUnsavedChangesGuard(state.hasUnsavedChanges);

// Pass search params to grid
<TerminationGrid searchParams={state.searchParams} />
```

**File Location**: `/src/ui/src/hooks/useTerminationState.ts`

---

## Data Flow

### Search Flow

```
User clicks "SEARCH"
    ↓
TerminationSearchFilter validates form
    ↓
actions.handleSearch(params) called
    ↓
useTerminationState updates searchParams and toggles resetPageFlag
    ↓
TerminationGrid receives new searchParams
    ↓
useTerminationGrid detects searchParams change
    ↓
triggerSearch() called (RTK Query lazy query)
    ↓
API request to /api/terminations
    ↓
Response cached by RTK Query
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
User edits "Suggested Forfeit" cell
    ↓
Cell editor captures value
    ↓
editedValues context updated with rowKey -> value
    ↓
User clicks save button
    ↓
handleSave() called from useTerminationGrid
    ↓
prepareSaveRequest() (no negation for termination)
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
  - Scroll to top (via window event listener)
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
handleBulkSave() called from useTerminationGrid
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
useTerminationState reducer:
  - Sets archiveMode = true
  - Sets shouldArchive = true
  - Adds archive: true to searchParams
  - Toggles resetPageFlag
    ↓
TerminationGrid detects searchParams change
    ↓
Search executes with archive flag
    ↓
Grid displays archived data
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

---

## Grid Architecture

### Master-Detail Row Structure

The grid uses a **flattened master-detail structure** where both master and detail rows exist in a single flat array with type indicators:

```typescript
// Master row (employee summary)
{
  badgeNumber: 12345,
  name: "John Doe",
  isExpandable: true,   // Can be expanded
  isExpanded: false,    // Currently collapsed
  isDetail: false       // This is a master row
}

// Detail rows (profit year records - only visible when expanded)
{
  badgeNumber: 12345,
  name: "John Doe",  // Copied from master
  profitYear: 2024,
  beginningBalance: 10000.00,
  beneficiaryAllocation: 0.00,
  distributionAmount: 0.00,
  forfeit: 0.00,
  endingBalance: 10000.00,
  vestedBalance: 8000.00,
  vestedPercent: 80,
  dateTerm: "2024-06-15",
  ytdPsHours: 2080,
  age: 45,
  hasForfeited: false,
  suggestedForfeit: 2000.00,
  isDetail: true      // This is a detail row
}
```

### Data Transformation

The `flattenMasterDetailData()` utility (from `/src/ui/src/utils/forfeitActivities/gridDataHelpers.ts`) transforms hierarchical data into this flat structure:

```typescript
// Input: Hierarchical structure
const masterData = [
  {
    badgeNumber: 12345,
    name: "John Doe",
    profitYears: [
      { profitYear: 2024, beginningBalance: 10000.00, ... },
      { profitYear: 2023, beginningBalance: 9000.00, ... }
    ]
  }
];

// Output: Flat array with type indicators
const flatData = flattenMasterDetailData(
  masterData,
  expandedRows, // Set<string> of expanded badgeNumbers
  {
    activityType: "termination",
    getRowKey: (row) => row.badgeNumber.toString(),
    getDetailRows: (row) => row.profitYears,
    shouldShowDetails: true
  }
);

// Result:
[
  { badgeNumber: 12345, name: "John Doe", isExpandable: true, isExpanded: true, isDetail: false },
  { badgeNumber: 12345, name: "John Doe", profitYear: 2024, beginningBalance: 10000.00, isDetail: true },
  { badgeNumber: 12345, name: "John Doe", profitYear: 2023, beginningBalance: 9000.00, isDetail: true }
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

- Master rows show only badge and name
- Detail rows show profit year columns
- Badge and name are hidden in detail rows (already shown in master)

---

## Key Features

### 1. Financial Summary Totals

**Display**: Four `TotalsGrid` components above the main grid showing:

- Amount in Profit Sharing (total ending balance)
- Vested Amount (total vested balance)
- Total Forfeitures (sum of forfeitures)
- Total Beneficiary Allocations (sum of distributions to beneficiaries)

**Sticky Positioning**: The totals remain visible when scrolling the grid using `sticky top-0 z-10` classes.

**Data Source**: Values come from the API response's summary fields:

```typescript
{
  totalEndingBalance: 250000.00,
  totalVested: 200000.00,
  totalForfeit: 50000.00,
  totalBeneficiaryAllocation: 10000.00
}
```

**Purpose**: Provides financial oversight and validation. Users can verify that individual forfeitures sum to the displayed totals.

### 2. Inline Editing

**Implementation**:

- Uses AG Grid's built-in editing with custom `SuggestedForfeitEditor`
- Edits tracked in grid context: `context.editedValues[rowKey] = { value: newValue }`
- Row key for termination is composite: `${badgeNumber}-${profitYear}`

**Editable Only for Current Year**:

```typescript
editable: ({ node }) => node.data.isDetail && node.data.profitYear === selectedProfitYear;
```

Users can only edit forfeitures for the selected profit year (current year). Historical years are read-only.

**Validation**:

- Cannot save zero values
- Cannot edit when read-only
- Original value preserved until save succeeds

**Error Highlighting**:

```typescript
cellClass: (params) => {
  if (!params.data.isDetail) return "";
  const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
  const hasError = params.context?.editedValues?.[rowKey]?.hasError;
  return hasError ? "bg-red-50" : "";
};
```

Cells with validation errors are highlighted with a red background (`bg-red-50` Tailwind class).

**Persistence**:

- Edited values persist across pagination
- Cleared on successful save
- Cleared on reset

### 3. Bulk Operations

**Selection**:

- Checkboxes in save button column
- Bulk checkbox in column header selects/deselects all eligible rows
- Eligibility: `isDetail === true && profitYear === selectedProfitYear && (currentValue || 0) !== 0`

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

### 4. Value Transformation

**Critical Pattern**: Termination values are **NOT negated** before sending to API

```typescript
// User enters positive value in UI: 2000.00
// API receives positive value: 2000.00 (no negation)

// Transformation in prepareSaveRequest():
forfeitureAmount: transformForfeitureValue("termination", request.forfeitureAmount);

// transformForfeitureValue() implementation:
export function transformForfeitureValue(activityType: ActivityType, value: number): number {
  if (activityType === "unforfeit") {
    return -(value || 0); // NEGATE for unforfeit
  }
  return value || 0; // No transformation for termination
}
```

**Why No Negation?**

- Forfeitures are stored as positive values representing money leaving the employee's account
- Unforfeitures reverse this by adding a negative offsetting entry
- Terminations create new forfeitures, so values are positive

**Contrast with UnForfeit**: UnForfeit negates values because it reverses existing forfeitures.

### 5. Row Keys and Tracking

**Termination uses composite row key**: `${badgeNumber}-${profitYear}`

```typescript
// From gridDataHelpers.ts
export function generateRowKey(config: RowKeyConfig, params: RowKeyParams): string {
  if (config.type === "unforfeit") {
    return params.profitDetailId.toString(); // UnForfeit uses profitDetailId
  }
  return `${params.badgeNumber}-${params.profitYear}`; // Termination uses composite key
}
```

**Why Composite Key?**

- Each employee can have multiple profit years
- Badge number alone is not unique across years
- Composite key uniquely identifies each detail row

**No `offsettingProfitDetailId`**: Termination does not require tracking an offsetting record (unlike UnForfeit), so this field is omitted.

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
editable: ({ node }) => node.data.isDetail && node.data.profitYear === selectedProfitYear && !isReadOnly

// In cell renderer
<IconButton disabled={isDisabled || isReadOnly}>
  <SaveOutlined />
</IconButton>
```

### 7. Unsaved Changes Guard

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
handleReset() => setInitialSearchLoaded(false);

// 3. User confirms navigation
// (browser confirmation dialog)
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

**Purpose**: View historical termination data when page is marked complete. Allows reviewing past terminations without affecting current processing.

**Exiting Archive Mode**: When status changes away from "Complete", the archive flag is removed and the search re-executes to show current data.

### 9. Duplicate SSN Guard

**Component**: `DuplicateSsnGuard` in TerminationSearchFilter

**Purpose**: Prevent searching when duplicate SSNs exist in the system

**Implementation**:

```typescript
<DuplicateSsnGuard mode="warning">
  {({ prerequisitesComplete }) => (
    <SearchAndReset
      handleReset={handleReset}
      handleSearch={validateAndSearch}
      disabled={!isValid || !prerequisitesComplete}
    />
  )}
</DuplicateSsnGuard>
```

**Behavior**:

- Checks for duplicate SSNs on mount
- Disables search button if duplicates found
- Displays warning message with details
- Must be resolved before terminations can be processed

**Data Integrity**: Ensures clean data before processing financial transactions.

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

  return () => {
    window.removeEventListener("dsmMessage", handleMessageEvent);
  };
}, [scrollToTop]);
```

**Purpose**: When a save operation fails, the error message appears at the top of the page via `ApiMessageAlert`. The event listener ensures the user sees the error by automatically scrolling to the top.

**Event Type**: Uses `dsmMessage` custom event dispatched by the message system.

---

## Shared Utilities

The Termination feature uses shared utilities from `/src/ui/src/utils/forfeitActivities/` to maintain consistency with the UnForfeit feature.

### 1. `gridDataHelpers.ts`

**Functions Used**:

#### `generateRowKey()`

```typescript
const rowKey = generateRowKey(
  { type: "termination" },
  {
    badgeNumber: row.badgeNumber,
    profitYear: row.profitYear
  }
);
// Returns: `${badgeNumber}-${profitYear}` for termination
```

#### `transformForfeitureValue()`

```typescript
const transformedValue = transformForfeitureValue("termination", 2000.0);
// Returns: 2000.00 (no negation for termination)
```

#### `flattenMasterDetailData()`

```typescript
const flatData = flattenMasterDetailData(masterData, expandedRows, {
  activityType: "termination",
  getRowKey: (row) => row.badgeNumber.toString(),
  getDetailRows: (row) => row.profitYears,
  shouldShowDetails: true
});
// Returns: Flat array with master and detail rows
```

### 2. `saveOperationHelpers.ts`

**Functions Used**:

#### `prepareSaveRequest()`

```typescript
const preparedRequest = prepareSaveRequest(
  { activityType: "termination", rowKeyConfig: { type: "termination" } },
  request
);
// Does NOT transform forfeitureAmount (no negation)
```

#### `generateSaveSuccessMessage()`

```typescript
const message = generateSaveSuccessMessage("termination", "John Doe", 2000.0);
// Returns: "Successfully saved forfeiture of $2,000.00 for John Doe"
```

#### `extractRowKey()`

```typescript
const rowKey = extractRowKey({ type: "termination" }, rowData, { editedValues: context.editedValues });
// Returns: `${badgeNumber}-${profitYear}`
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
    return generateSaveSuccessMessage("termination", name, request.forfeitureAmount);
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
const [triggerSearch, { data, isFetching }] = useLazyGetTerminationsQuery();

// Execute search
triggerSearch({
  beginningDate: "01/01/2024",
  endingDate: "12/31/2024",
  forfeitureStatus: "showAll",
  profitYear: 2024,
  archive: false,
  pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false }
});

// Response structure
{
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

**Update Endpoint**:

```typescript
const [updateForfeiture, { isLoading, error }] = useUpdateForfeitureAdjustmentMutation();

// Execute update
await updateForfeiture({
  badgeNumber: 12345,
  profitYear: 2024,
  forfeitureAmount: 2000.0, // NOT negated for termination
  classAction: false
}).unwrap();
```

### Redux Slices

**Termination Slice** (`yearsEndSlice.ts`):

```typescript
// Actions
clearTermination() - Clear termination search results

// State
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

**Why Store Termination Data?**

- Persist search results for grid display
- Store financial summary totals
- Support browser refresh without losing context

---

## Hooks Reference

### 1. `useTerminationGrid`

**Purpose**: Main business logic hook for grid operations

**Parameters**:

```typescript
interface UseTerminationGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  searchParams: TerminationSearchRequest | null;
  resetPageFlag: boolean;
  onUnsavedChanges: (hasChanges: boolean) => void;
  hasUnsavedChanges: boolean;
  fiscalData: CalendarResponseDto | null;
  shouldArchive?: boolean;
  onArchiveHandled?: () => void;
  onErrorOccurred?: () => void;
  onLoadingChange?: (isLoading: boolean) => void;
}
```

**Returns**:

```typescript
{
  pageNumber: number,
  pageSize: number,
  gridData: Array<MasterRow | DetailRow>,
  termination: TerminationResponse,
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
    editedValues: Record<string, { value: number, hasError?: boolean }>,
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

**File Location**: `/src/ui/src/hooks/useTerminationGrid.ts`

### 2. `useTerminationState`

**Purpose**: Page-level state management

**Returns**:

```typescript
{
  state: {
    searchParams: TerminationSearchRequest | null,
    initialSearchLoaded: boolean,
    hasUnsavedChanges: boolean,
    resetPageFlag: boolean,
    currentStatus: string | null,
    archiveMode: boolean,
    shouldArchive: boolean
  },
  actions: {
    handleSearch: (params: TerminationSearchRequest) => void,
    handleUnsavedChanges: (hasChanges: boolean) => void,
    handleStatusChange: (status: string, statusName?: string) => void,
    handleArchiveHandled: () => void,
    setInitialSearchLoaded: (loaded: boolean) => void
  }
}
```

**File Location**: `/src/ui/src/hooks/useTerminationState.ts`

### 3. `useDecemberFlowProfitYear`

**Purpose**: Get active profit year from context

**Returns**: `number` (e.g., 2024)

### 4. `useUnsavedChangesGuard`

**Purpose**: Block navigation when unsaved changes exist

**Parameters**: `hasUnsavedChanges: boolean`

**Behavior**: Registers `beforeunload` and React Router block handlers

### 5. `useReadOnlyNavigation`

**Purpose**: Check if current navigation context requires read-only mode

**Returns**: `boolean`

### 6. `useDynamicGridHeight`

**Purpose**: Calculate optimal grid height based on viewport

**Returns**: `string` (e.g., "calc(100vh - 400px)")

---

## Type Definitions

### Request Types

```typescript
interface TerminationSearchRequest {
  beginningDate: string; // Format: "MM/DD/YYYY"
  endingDate: string; // Format: "MM/DD/YYYY"
  forfeitureStatus: string; // "showAll", "forfeited", "notForfeited"
  profitYear?: number;
  archive?: boolean; // Include archived data
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
  classAction: boolean; // Always false for manual operations
  offsettingProfitDetailId?: number; // Undefined for termination
}
```

### Response Types

```typescript
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
  name: string; // Copied from master row
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

**Pattern**: All errors displayed via `ApiMessageAlert` component with common key

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

**Scroll-to-Top**: Window event listener ensures user sees error message at top of page.

**Benefits**:

- Consistent error display
- Automatic scroll to top
- Dismissible alerts
- Error details preserved

### 2. Loading States

**Pattern**: Multiple loading indicators for different operations

```typescript
// Global search loading
const { isFetching } = useLazyGetTerminationsQuery();

// Individual row loading
const [loadingRowIds, setLoadingRowIds] = useState<Set<string>>(new Set());

// Mutation loading
const { isLoading } = useUpdateForfeitureAdjustmentMutation();
```

**Display**:

- Search: Show CircularProgress while fiscal data loads
- Individual save: Show CircularProgress in save button
- Bulk save: Disable bulk checkbox, show progress for each row

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
    <TerminationSearchFilter />
    <TerminationGrid />
  </>
)}

// Show grid only after search
{initialSearchLoaded && termination?.response && (
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

## Differences from UnForfeit

This section highlights the key differences between Termination and UnForfeit features to help developers understand the distinct patterns.

### 1. Value Transformation

| Feature     | Value Transformation | Reason                                                      |
| ----------- | -------------------- | ----------------------------------------------------------- |
| Termination | **No negation**      | Creates new forfeitures (positive values)                   |
| UnForfeit   | **Negates values**   | Reverses existing forfeitures (negative offsetting entries) |

```typescript
// Termination
forfeitureAmount: currentValue || 0;

// UnForfeit
forfeitureAmount: -(currentValue || 0);
```

### 2. Row Key Strategy

| Feature     | Row Key                                       | Reason                                 |
| ----------- | --------------------------------------------- | -------------------------------------- |
| Termination | **Composite**: `${badgeNumber}-${profitYear}` | Identifies profit year record uniquely |
| UnForfeit   | **Single**: `profitDetailId`                  | Tracks specific forfeiture to reverse  |

### 3. Offsetting Profit Detail ID

| Feature     | `offsettingProfitDetailId` | Reason                                          |
| ----------- | -------------------------- | ----------------------------------------------- |
| Termination | **Omitted / Undefined**    | Not reversing an existing forfeiture            |
| UnForfeit   | **Required**               | Must link to original forfeiture being reversed |

### 4. Master Row Columns

| Feature     | Master Columns                                             | Reason                                   |
| ----------- | ---------------------------------------------------------- | ---------------------------------------- |
| Termination | **Minimal**: Badge, Name only                              | Focus on detail rows with financial data |
| UnForfeit   | **Comprehensive**: Badge, Name, SSN, Dates, Balances, etc. | More context needed at summary level     |

### 5. Financial Summary Totals

| Feature     | Financial Summaries                                                   | Reason                               |
| ----------- | --------------------------------------------------------------------- | ------------------------------------ |
| Termination | **Yes**: 4 totals grids (amounts, vested, forfeitures, beneficiaries) | Financial oversight for terminations |
| UnForfeit   | **No**: Only ReportSummary (counts)                                   | Less financial complexity            |

### 6. Editable Rows

| Feature     | Editable Criteria                                        | Reason                                           |
| ----------- | -------------------------------------------------------- | ------------------------------------------------ |
| Termination | **Detail rows** for **current profit year only**         | Only process current year forfeitures            |
| UnForfeit   | **All detail rows** with non-zero suggested unforfeiture | Can reverse forfeitures from any historical year |

### 7. Search Filter

| Feature     | Additional Filter Components                 | Reason                                  |
| ----------- | -------------------------------------------- | --------------------------------------- |
| Termination | **DuplicateSsnGuard**: Prerequisite check    | Ensure data integrity before processing |
| UnForfeit   | **Checkbox**: Exclude zero balance employees | Filter display for relevant employees   |

### 8. Archive Mode

| Feature     | Archive Mode Implementation                                      | Reason                    |
| ----------- | ---------------------------------------------------------------- | ------------------------- |
| Termination | **Explicit**: `archive` flag in searchParams, managed by reducer | Complex state transitions |
| UnForfeit   | **Simple**: `shouldArchive` flag triggers search                 | Simpler state management  |

### 9. Error Handling

| Feature     | Error Scroll-to-Top                             | Reason                              |
| ----------- | ----------------------------------------------- | ----------------------------------- |
| Termination | **Yes**: Window event listener for `dsmMessage` | Ensure user sees errors at page top |
| UnForfeit   | **Yes**: Same pattern implemented               | Consistent UX across features       |

### 10. State Management

| Feature     | State Hook            | Key State Fields                                   |
| ----------- | --------------------- | -------------------------------------------------- |
| Termination | `useTerminationState` | `searchParams`, `archiveMode`, `currentStatus`     |
| UnForfeit   | `useUnForfeitState`   | `resetPageFlag`, `previousStatus`, `shouldArchive` |

Both use reducer pattern but with different state structures optimized for their specific workflows.

---
