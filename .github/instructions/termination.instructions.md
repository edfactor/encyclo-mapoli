---
applyTo: "src/ui/src/pages/DecemberActivities/Termination/**/*.*"
---

# Termination Feature - Technical Summary

## Overview

The Termination feature is part of the December Activities workflow in the Smart Profit Sharing application. It enables users to search for terminated employees within a date range and manage their profit-sharing account forfeitures by adjusting suggested forfeit amounts. The feature provides inline editing capabilities, bulk operations, and comprehensive filtering options.

**Location**: `src/ui/src/pages/DecemberActivities/Termination/`

**Primary Pattern**: Search-Filter-Grid with inline editable cells and bulk operations

**Key Capabilities**:

- Search terminated employees by date range with fiscal year constraints
- Filter by vested balance with comparison operators (=, <, <=, >, >=)
- Inline editing of suggested forfeit amounts
- Individual and bulk save operations
- Archive mode for completed records
- Read-only mode support
- Unsaved changes protection

---

## Directory Structure

```
Termination/
├── Termination.tsx                              # Main page container (122 lines)
├── TerminationSearchFilter.tsx                  # Search/filter form component (315 lines)
├── TerminationGrid.tsx                          # Data grid component (182 lines)
├── TerminationGridColumns.tsx                   # Main grid columns: PSN, Name (16 lines)
├── TerminationDetailsGridColumns.tsx            # Detail columns with editable fields (168 lines)
├── TerminationHeaderComponent.tsx               # Bulk save header component (23 lines)
├── hooks/
│   ├── useTerminationState.ts                   # State management hook (157 lines)
│   └── useTerminationGrid.ts                    # Grid logic and API integration (616 lines)
├── __test__/
│   ├── Termination.test.tsx                     # Main page tests
│   ├── TerminationSearchFilter.test.tsx         # Filter form tests
│   └── useTerminationState.test.tsx             # State hook tests
└── Claude.md                                     # Existing architecture documentation
```

**Total Lines of Code**: ~1,599 lines (excluding tests and documentation)

---

## File Descriptions

### 1. Termination.tsx (Main Container)

**Purpose**: Top-level orchestrator component that manages the overall page layout and coordinates between filter, grid, and state management.

**Key Responsibilities**:

- Provides `Page` wrapper from smart-ui-library with title and status dropdown
- Fetches fiscal calendar data via `useLazyGetAccountingRangeToCurrent(6)`
- Manages centralized state through `useTerminationState()` custom hook
- Implements unsaved changes guard to prevent navigation with unsaved edits
- Handles error scroll-to-top behavior for validation messages
- Global event listener for `dsmMessage` events to trigger scroll on errors

**State Management**:

```typescript
// Local state
const [isFetching, setIsFetching] = useState(false);

// Custom hook state from useTerminationState()
const { state, actions } = useTerminationState();
// state: { hasUnsavedChanges, initialSearchLoaded, searchParams,
//          resetPageFlag, shouldArchive }
// actions: { handleSearch, handleStatusChange, handleUnsavedChanges,
//            setInitialSearchLoaded, handleArchiveHandled }
```

**Layout Structure**:

```jsx
<Page label="TERMINATIONS" actionNode={<StatusDropdownActionNode />}>
  <ApiMessageAlert commonKey="TerminationSave" />
  <DSMAccordion title="Filter">
    <TerminationSearchFilter
      fiscalData={fiscalData}
      onSearch={actions.handleSearch}
      setInitialSearchLoaded={actions.setInitialSearchLoaded}
      hasUnsavedChanges={state.hasUnsavedChanges}
      isFetching={isFetching}
    />
  </DSMAccordion>
  <TerminationGrid
    initialSearchLoaded={state.initialSearchLoaded}
    setInitialSearchLoaded={actions.setInitialSearchLoaded}
    searchParams={state.searchParams}
    resetPageFlag={state.resetPageFlag}
    onUnsavedChanges={actions.handleUnsavedChanges}
    hasUnsavedChanges={state.hasUnsavedChanges}
    fiscalData={fiscalData}
    shouldArchive={state.shouldArchive}
    onArchiveHandled={actions.handleArchiveHandled}
    onErrorOccurred={scrollToTop}
    onLoadingChange={setIsFetching}
  />
</Page>
```

**Unsaved Changes Guard**:

```typescript
useUnsavedChangesGuard(state.hasUnsavedChanges);
// Prevents navigation when hasUnsavedChanges is true
// Shows browser confirmation dialog: "You have unsaved changes"
```

**Error Scroll Behavior**:

```typescript
useEffect(() => {
  const handleMessageEvent = (event: Event) => {
    const customEvent = event as CustomEvent;
    if (
      customEvent.detail?.key === "TerminationSave" &&
      customEvent.detail?.message?.type === "error"
    ) {
      scrollToTop(); // Smooth scroll to top to show error message
    }
  };
  window.addEventListener("dsmMessage", handleMessageEvent);
  return () => window.removeEventListener("dsmMessage", handleMessageEvent);
}, [scrollToTop]);
```

**Lines**: 122

---

### 2. TerminationSearchFilter.tsx (Filter Form)

**Purpose**: Provides comprehensive search form with date range selection, vested balance filtering, and data quality options.

**Key Features**:

- **React Hook Form** integration with **Yup** validation
- Date range inputs (Begin Date, End Date) constrained to fiscal calendar bounds
- **Vested Balance Filter** with operator dropdown (=, <, <=, >, >=) and amount field
- Checkbox: "Exclude members with: $0 Ending Balance, 100% Vested, or Forfeited"
- Redux integration for persisting search state across sessions
- `DuplicateSsnGuard` integration (warning mode) for data quality checks
- Prevents search when unsaved changes exist

**Validation Schema**:

```typescript
yup.object().shape({
  beginningDate: dateStringValidator(2000, 2099, "Beginning Date").required(),
  endingDate: endDateStringAfterStartDateValidator(
    "beginningDate",
    tryddmmyyyyToDate,
    "Ending date must be the same or after the beginning date",
  ).required(),
  forfeitureStatus: yup.string().required(),
  pagination: yup
    .object({
      skip: yup.number().required(),
      take: yup.number().required(),
      sortBy: yup.string().required(),
      isSortDescending: yup.boolean().required(),
    })
    .required(),
  profitYear: profitYearValidator(2015, 2099),
  excludeZeroAndFullyVested: yup.boolean(),
  vestedBalanceValue: yup
    .number()
    .nullable()
    .min(0, "Vested Balance must be 0 or greater")
    .transform((value, originalValue) => (originalValue === "" ? null : value)),
  vestedBalanceOperator: yup
    .number()
    .nullable()
    .when("vestedBalanceValue", {
      is: (val: number | null | undefined) => val !== null && val !== undefined,
      then: (schema) =>
        schema.required("Operator is required when Vested Balance is provided"),
      otherwise: (schema) => schema.nullable(),
    }),
});
```

**Vested Balance Filter UI** (Lines 224-277):

```typescript
<Grid size={{ xs: 12, sm: 6, md: 2.5 }}>
  <InputLabel sx={{ mb: 1 }}>Vested Balance</InputLabel>
  <Box sx={{ display: "flex", gap: 1 }}>
    {/* Operator Dropdown */}
    <TextField select fullWidth value={field.value ?? ""}>
      <MenuItem value="">-</MenuItem>        {/* Empty/no filter */}
      <MenuItem value={0}>=</MenuItem>       {/* Equals */}
      <MenuItem value={1}>&lt;</MenuItem>    {/* Less Than */}
      <MenuItem value={2}>&lt;=</MenuItem>   {/* Less Than Or Equal */}
      <MenuItem value={3}>&gt;</MenuItem>    {/* Greater Than */}
      <MenuItem value={4}>&gt;=</MenuItem>   {/* Greater Than Or Equal */}
    </TextField>

    {/* Amount Field */}
    <TextField
      fullWidth
      type="number"
      placeholder="Amount"
      value={field.value ?? ""}
      inputProps={{ min: 0, step: 1 }}
    />
  </Box>
</Grid>
```

**Default Values**:

```typescript
{
  beginningDate: fiscalData?.fiscalBeginDate || "",
  endingDate: fiscalData?.fiscalEndDate || "",
  forfeitureStatus: "showAll",
  pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false },
  profitYear: selectedProfitYear,
  excludeZeroAndFullyVested: false,
  vestedBalanceValue: null,
  vestedBalanceOperator: null
}
```

**Vested Balance Filter Logic** (Lines 118-122):

```typescript
const validateAndSubmit = async (data: TerminationSearchRequest) => {
  const params = { ...data, profitYear: selectedProfitYear, ... };

  // Only include vested balance fields if both are provided
  if (data.vestedBalanceValue === null || data.vestedBalanceOperator === null) {
    delete params.vestedBalanceValue;
    delete params.vestedBalanceOperator;
  }

  onSearch(params);
  setInitialSearchLoaded(true);
};
```

**Unsaved Changes Protection** (Lines 104-107):

```typescript
if (hasUnsavedChanges) {
  alert("Please save your changes.");
  return;
}
```

**Redux Integration**:

- Reads `termination` state from `yearsEnd` slice for persisted begin/end dates
- Dispatches `clearTermination()` on reset

**Lines**: 315

---

### 3. TerminationGrid.tsx (Data Grid)

**Purpose**: Renders AG Grid with termination data, summary totals, pagination, and coordinates all grid interactions.

**Key Features**:

- Displays four summary totals in `TotalsGrid` components:
  - Amount in Profit Sharing
  - Vested Amount
  - Total Forfeitures
  - Total Beneficiary Allocations
- Report summary section via `ReportSummary` component
- Dynamic grid height calculation via `useDynamicGridHeight()` hook
- Read-only mode detection via `useReadOnlyNavigation()` hook
- Combines main columns (PSN, Name) with 14 detail columns

**Hook Dependencies**:

```typescript
const gridMaxHeight = useDynamicGridHeight(); // Viewport-based height
const isReadOnly = useReadOnlyNavigation(); // Permission check
const {
  pageNumber,
  pageSize,
  gridData,
  isFetching,
  termination,
  selectedProfitYear,
  selectionState,
  handleSave,
  handleBulkSave,
  sortEventHandler,
  onGridReady,
  paginationHandlers,
  gridRef,
  gridContext,
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
  onLoadingChange,
  isReadOnly,
});
```

**Summary Totals** (Lines 120-137):

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

**Grid Configuration** (Lines 139-164):

```typescript
<DSMGrid
  preferenceKey={"TERMINATION"}              // Saves user column preferences
  handleSortChanged={sortEventHandler}
  maxHeight={gridMaxHeight}
  isLoading={isFetching}
  providedOptions={{
    onGridReady: (params) => {
      gridRef.current = params;
      onGridReady(params);
    },
    rowData: gridData,
    columnDefs: columnDefs,                  // Combined main + detail columns
    rowSelection: {
      mode: "multiRow",
      checkboxes: false,                     // Checkboxes in save button column
      headerCheckbox: false,
      enableClickSelection: false
    },
    rowHeight: 40,
    suppressMultiSort: true,
    defaultColDef: { resizable: true },
    context: gridContext                     // Shared state for cell renderers
  }}
/>
```

**Column Assembly** (Lines 88-112):

```typescript
const mainColumns = useMemo(() => GetTerminationColumns(), []);
const detailColumns = useMemo(
  () =>
    GetDetailColumns(
      selectionState.addRowToSelection,
      selectionState.removeRowFromSelection,
      selectedProfitYear,
      handleSave,
      handleBulkSave,
      isReadOnly,
    ),
  [selectionState, selectedProfitYear, handleSave, handleBulkSave, isReadOnly],
);

const columnDefs = useMemo(() => {
  return [...mainColumns, ...detailColumns];
}, [mainColumns, detailColumns]);
```

**Read-Only Cell Refresh** (Lines 79-85):

```typescript
// Refresh grid cells when read-only status changes
// Forces cell renderers to re-read isReadOnly from context
useEffect(() => {
  if (gridRef.current?.api) {
    gridRef.current.api.refreshCells({ force: true });
  }
}, [isReadOnly]);
```

**Pagination** (Lines 166-174):

```typescript
{!!termination && termination.response.results.length > 0 && (
  <Pagination
    pageNumber={pageNumber}
    setPageNumber={paginationHandlers.setPageNumber}
    pageSize={pageSize}
    setPageSize={paginationHandlers.setPageSize}
    recordCount={termination.response.total || 0}
  />
)}
```

**Lines**: 182

---

### 4. TerminationGridColumns.tsx (Main Columns)

**Purpose**: Defines the two primary identification columns displayed at the left of the grid.

**Columns**:

```typescript
export const GetTerminationColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "PSN",
      field: "psn",
      psnSuffix: false, // PSN already combined with suffix
    }),
    createNameColumn({
      field: "name",
    }),
  ];
};
```

**Column Definitions**:

1. **PSN (Profit Sharing Number)**:

   - Factory: `createBadgeColumn()` from `utils/gridColumnFactory`
   - Displays employee/beneficiary identification number
   - No suffix needed (PSN format: `1234567` for employees, `1234567-1` for beneficiaries)

2. **Name**:
   - Factory: `createNameColumn()` from grid factory
   - Displays full name (Last, First format)
   - Left-aligned text

**Purpose of Separation**: Separates main identification columns from detail columns for modularity. Main columns remain visible while scrolling horizontally through detail columns.

**Lines**: 16

---

### 5. TerminationDetailsGridColumns.tsx (Detail Columns)

**Purpose**: Defines 14 detail columns including the editable "Suggested Forfeit" field and save button column.

**Complete Column List**:

1. **Profit Year**: Year column (non-sortable)
2. **Beginning Balance**: Currency column
3. **Beneficiary Allocation**: Currency column
4. **Distribution Amount**: Currency column
5. **Forfeit Amount**: Currency column
6. **Ending Balance**: Currency column
7. **Vested Balance**: Currency column
8. **Vested %**: Percentage column (formatted as `XX%`)
9. **Term Date**: Date column (MM/DD/YYYY format)
10. **YTD PS Hours**: Hours column with comma separators
11. **Age**: Age column (max width 70px)
12. **Forfeited**: Yes/No column
13. **Suggested Forfeit**: **Editable currency column** (pinned right)
14. **Save Button**: Action column with individual/bulk save (pinned right)

**Editable Column - Suggested Forfeit** (Lines 92-129):

```typescript
{
  headerName: "Suggested Forfeit",
  field: "suggestedForfeit",
  colId: "suggestedForfeit",
  minWidth: 150,
  pinned: "right",
  type: "rightAligned",
  resizable: true,
  sortable: false,

  // Cell styling with error highlighting
  cellClass: (params) => {
    if (params.data.suggestedForfeit === null) return "";
    const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
    const hasError = params.context?.editedValues?.[rowKey]?.hasError;
    return hasError ? "bg-blue-50" : "";  // Blue background for validation errors
  },

  // Only editable if suggestedForfeit is not null
  editable: ({ node }) => node.data.suggestedForfeit !== null,

  // Custom editor and renderer
  flex: 1,
  cellEditor: SuggestedForfeitEditor,
  cellRenderer: (params: ICellRendererParams) => {
    // Beneficiaries (PSN > 7 chars) don't show suggested forfeit
    if (params.data.suggestedForfeit === null ||
        params.data.psn.length > MAX_EMPLOYEE_BADGE_LENGTH) {
      return null;
    }
    return SuggestedForfeitCellRenderer(
      { ...params, selectedProfitYear },
      true,   // showEditIcon
      false   // isReadOnly
    );
  },

  // Format display value as currency
  valueFormatter: (params) =>
    params.value !== null ? numberToCurrency(params.value) : "",

  // Get value from edited context or original data
  valueGetter: (params) => {
    const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
    const editedValue = params.context?.editedValues?.[rowKey]?.value;
    return editedValue ?? params.data.suggestedForfeit;
  }
}
```

**Key Mechanisms**:

- **Row Key Pattern**: `${badgeNumber}-${profitYear}` for unique row identification
- **Context-Based Editing**: Uses AG Grid `context` to track in-memory edits (not persisted until save)
- **Error Highlighting**: `bg-blue-50` CSS class for validation errors
- **Null Handling**: Non-editable cells render as empty (null values)
- **Beneficiary Handling**: PSN length > 7 indicates beneficiary (no suggested forfeit)

**Save Button Column** (Lines 131-165):

```typescript
{
  headerName: "Save Button",
  field: "saveButton",
  colId: "saveButton",
  minWidth: 130,
  width: 130,
  pinned: "right",
  lockPinned: true,
  resizable: false,
  sortable: false,
  cellStyle: { backgroundColor: "#E8E8E8" },

  // Bulk save header component
  headerComponent: HeaderComponent,
  headerComponentParams: {
    addRowToSelectedRows,
    removeRowFromSelectedRows,
    onBulkSave,
    isReadOnly
  },

  // Value getter combines current value, loading state, and selection state
  // This triggers cell refresh when any of these change
  valueGetter: (params) => {
    const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
    const editedValue = params.context?.editedValues?.[rowKey]?.value;
    const currentValue = editedValue ?? params.data.suggestedForfeit ?? 0;
    return `${currentValue}-${params.context?.loadingRowIds?.has(params.data.psn)}-${params.node?.isSelected()}`;
  },

  // Cell renderer params for individual save
  cellRendererParams: {
    addRowToSelectedRows,
    removeRowFromSelectedRows,
    onSave
  },

  // Custom cell renderer
  cellRenderer: createSaveButtonCellRenderer({
    activityType: "termination",
    selectedProfitYear,
    isReadOnly
  })
}
```

**Factory Functions Used**:

- `createYearColumn()` - Year with right alignment
- `createCurrencyColumn()` - Currency with $ formatting
- `createDateColumn()` - Date with MM/DD/YYYY format
- `createHoursColumn()` - Hours with comma separators
- `createAgeColumn()` - Age with max 70px width
- `createYesOrNoColumn()` - Boolean as Yes/No text
- `createSaveButtonCellRenderer()` - Save button with loading states

**Lines**: 168

---

### 6. TerminationHeaderComponent.tsx (Bulk Save Header)

**Purpose**: Renders bulk save controls in the "Save Button" column header, enabling multi-row operations.

**Implementation**: Thin wrapper around `SharedForfeitHeaderComponent` configured for "termination" activity type.

```typescript
export const HeaderComponent: React.FC<HeaderComponentProps> = (params) => {
  return (
    <SharedForfeitHeaderComponent
      {...params}
      config={{ activityType: "termination" }}
    />
  );
};
```

**Props Interface**:

```typescript
interface HeaderComponentProps extends IHeaderParams {
  addRowToSelectedRows: (id: number) => void;
  removeRowFromSelectedRows: (id: number) => void;
  onBulkSave?: (
    requests: ForfeitureAdjustmentUpdateRequest[],
    names: string[],
  ) => Promise<void>;
  isBulkSaving?: boolean;
  isReadOnly?: boolean;
}
```

**Functionality**:

- Displays checkbox for "select all" functionality
- Shows bulk save button when rows are selected
- Displays count of selected rows
- Automatically disabled in read-only mode
- Shares common implementation with other forfeit activities (UnForfeit, etc.)

**Visual Behavior**:

- **No selections**: Shows empty checkbox
- **Some selections**: Shows checkbox with count badge
- **Has selections**: Shows "Save Selected" button

**Lines**: 23

---

### 7. hooks/useTerminationState.ts (State Management Hook)

**Purpose**: Encapsulates all page-level state management using `useReducer` pattern for complex state transitions.

**State Interface**:

```typescript
interface TerminationState {
  searchParams: TerminationSearchRequest | null; // Current search parameters
  initialSearchLoaded: boolean; // Has initial search been performed
  hasUnsavedChanges: boolean; // Are there unsaved grid edits
  resetPageFlag: boolean; // Toggles to trigger pagination reset
  currentStatus: string | null; // Current page status from dropdown
  archiveMode: boolean; // Is archive mode active
  shouldArchive: boolean; // Should next search use archive=true
}
```

**Action Types**:

```typescript
type TerminationAction =
  | { type: "SET_SEARCH_PARAMS"; payload: TerminationSearchRequest }
  | { type: "SET_INITIAL_SEARCH_LOADED"; payload: boolean }
  | { type: "SET_UNSAVED_CHANGES"; payload: boolean }
  | { type: "TOGGLE_RESET_PAGE_FLAG" }
  | {
      type: "SET_STATUS_CHANGE";
      payload: { status: string; statusName?: string };
    }
  | { type: "SET_ARCHIVE_HANDLED" }
  | { type: "RESET_STATE" };
```

**Key Reducer Logic - Status Change** (Lines 61-100):

```typescript
case "SET_STATUS_CHANGE": {
  const { statusName } = action.payload;
  const isCompleteLike = (statusName ?? "").toLowerCase().includes("complete");
  const isChangingToComplete = isCompleteLike && state.currentStatus !== statusName;

  if (isChangingToComplete) {
    // Transitioning to Complete status -> Enable archive mode
    const updatedSearchParams = state.searchParams
      ? { ...state.searchParams, archive: true }
      : null;

    return {
      ...state,
      currentStatus: statusName || null,
      archiveMode: true,
      shouldArchive: true,                   // Flag for grid to trigger archive search
      searchParams: updatedSearchParams,
      resetPageFlag: !state.resetPageFlag    // Reset pagination
    };
  } else {
    // Leaving Complete status -> Disable archive mode
    const shouldResetArchive = !isCompleteLike;
    const wasInArchiveMode = state.archiveMode;
    const isLeavingArchiveMode = wasInArchiveMode && shouldResetArchive;

    let updatedSearchParams = state.searchParams;
    if (shouldResetArchive && state.searchParams) {
      const { ...paramsWithoutArchive } = state.searchParams;
      updatedSearchParams = paramsWithoutArchive as TerminationSearchRequest;
    }

    return {
      ...state,
      currentStatus: statusName || null,
      archiveMode: shouldResetArchive ? false : state.archiveMode,
      searchParams: shouldResetArchive ? updatedSearchParams : state.searchParams,
      resetPageFlag: isLeavingArchiveMode ? !state.resetPageFlag : state.resetPageFlag
    };
  }
}
```

**Exported Actions**:

```typescript
return {
  state,
  actions: {
    handleSearch: (params) => {
      const searchParamsWithArchive = {
        ...params,
        ...(state.archiveMode && { archive: true }),
      };
      dispatch({ type: "SET_SEARCH_PARAMS", payload: searchParamsWithArchive });
    },
    handleUnsavedChanges: (hasChanges) =>
      dispatch({ type: "SET_UNSAVED_CHANGES", payload: hasChanges }),
    handleStatusChange: (newStatus, statusName) =>
      dispatch({
        type: "SET_STATUS_CHANGE",
        payload: { status: newStatus, statusName },
      }),
    handleArchiveHandled: () => dispatch({ type: "SET_ARCHIVE_HANDLED" }),
    setInitialSearchLoaded: (loaded) =>
      dispatch({ type: "SET_INITIAL_SEARCH_LOADED", payload: loaded }),
  },
};
```

**Archive Mode Flow**:

1. User changes status dropdown to "Complete" → `archiveMode = true`, `shouldArchive = true`
2. Next search automatically includes `archive: true` parameter
3. After search completes → `shouldArchive = false` (via `handleArchiveHandled`)
4. Archived records persist until status changes away from "Complete"

**Lines**: 157

---

### 8. hooks/useTerminationGrid.ts (Grid Logic Hook)

**Purpose**: Encapsulates all grid-related logic including data fetching, pagination, sorting, editing, and save operations.

**Configuration**:

```typescript
interface TerminationGridConfig {
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
  isReadOnly: boolean;
}
```

**API Hooks**:

```typescript
const [triggerSearch, { isFetching }] = useLazyGetTerminationReportQuery();
const [updateForfeitureAdjustmentBulk] =
  useUpdateForfeitureAdjustmentBulkMutation();
const [updateForfeitureAdjustment] = useUpdateForfeitureAdjustmentMutation();
```

**Custom Hooks Integration**:

```typescript
const editState = useEditState(); // Tracks edited values and loading rows
const selectionState = useRowSelection(); // Tracks selected rows for bulk operations
const {
  pageNumber,
  pageSize,
  sortParams,
  handlePaginationChange,
  handleSortChange,
  resetPagination,
} = useGridPagination({
  initialPageSize: 25,
  initialSortBy: "name",
  initialSortDescending: false,
  onPaginationChange: async (pageNum, pageSz, sortPrms) => {
    // Trigger search with updated pagination
  },
});
```

**Grid Data Assembly** (Lines 528-560):

```typescript
const gridData = useMemo(() => {
  if (!termination?.response?.results) return [];

  // Flatten: create one row per employee combining master data with yearDetail
  return termination.response.results
    .map((masterRow) => {
      const badgeNumber = masterRow.psn;

      // Find the yearDetail that matches the selected profit year
      const matchingDetail = masterRow.yearDetails?.find(
        (detail) => detail.profitYear === selectedProfitYear,
      );

      // Skip this employee if no matching year detail found
      if (!matchingDetail) return null;

      return {
        // Master row fields
        psn: masterRow.psn,
        name: masterRow.name,
        badgeNumber,

        // Current year detail fields (spread all fields from matchingDetail)
        ...matchingDetail,

        // Metadata (mark as detail row for shared components)
        isDetail: true,
        parentId: badgeNumber,
        index: 0,
      };
    })
    .filter((row) => row !== null); // Remove null entries
}, [termination, selectedProfitYear]);
```

**Individual Save Handler** (Lines 309-406):

```typescript
const handleSave = useCallback(
  async (request: ForfeitureAdjustmentUpdateRequest, name: string) => {
    const rowId = request.badgeNumber;
    const actualCurrentPage = currentPageNumberRef.current;  // Avoid stale closure

    editState.addLoadingRow(rowId);  // Show loading spinner

    try {
      // Transform request using shared helper
      const transformedRequest = prepareSaveRequest(ACTIVITY_CONFIG, request);
      await updateForfeitureAdjustment(transformedRequest);

      // Remove from edited values
      const rowKey = generateRowKey(ACTIVITY_CONFIG.rowKeyConfig, {
        badgeNumber: request.badgeNumber,
        profitYear: request.profitYear
      });
      editState.removeEditedValue(rowKey);

      // Check for remaining edits
      const remainingEdits = Object.keys(editState.editedValues)
        .filter((key) => key !== rowKey).length > 0;
      onUnsavedChanges(remainingEdits);

      // Generate success message
      const successMessage = generateSaveSuccessMessage(
        ACTIVITY_CONFIG.activityType,
        name || "the selected employee",
        request.forfeitureAmount
      );

      // Refresh grid and show success message after data loads
      if (searchParams) {
        const skip = actualCurrentPage * pageSize;
        const params = createRequest(skip, sortParams.sortBy,
                                      sortParams.isSortDescending,
                                      selectedProfitYear, pageSize);
        if (params) {
          await triggerSearch(params, false);
          handlePaginationChange(actualCurrentPage, pageSize);  // Stay on same page
          setPendingSuccessMessage(successMessage);
        }
      } else {
        // Show message immediately if no search params
        dispatch(setMessage({ ...Messages.TerminationSaveSuccess, ... }));
      }
    } catch (error) {
      console.error("Failed to save forfeiture adjustment:", error);
      const errorMessage = formatApiError(error,
        getErrorMessage(ACTIVITY_CONFIG.activityType, "save"));
      dispatch(setMessage({ key: "TerminationSave", message: {
        message: errorMessage, type: "error"
      }}));
      if (onErrorOccurred) onErrorOccurred();  // Scroll to top
    } finally {
      editState.removeLoadingRow(rowId);
    }
  },
  [updateForfeitureAdjustment, editState, onUnsavedChanges, searchParams,
   pageSize, sortParams, selectedProfitYear, createRequest, triggerSearch,
   handlePaginationChange, dispatch, onErrorOccurred]
);
```

**Bulk Save Handler** (Lines 408-501):

```typescript
const handleBulkSave = useCallback(
  async (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => {
    const badgeNumbers = requests.map((request) => request.badgeNumber);
    editState.addLoadingRows(badgeNumbers); // Show loading spinners

    const actualCurrentPage = currentPageNumberRef.current;
    setIsBulkSaveInProgress(true); // Prevent pagination reset

    try {
      // Transform all requests using shared helper
      const transformedRequests = prepareBulkSaveRequests(
        ACTIVITY_CONFIG,
        requests,
      );
      await updateForfeitureAdjustmentBulk(transformedRequests);

      // Generate row keys using shared helper
      const rowKeys = getRowKeysForRequests(ACTIVITY_CONFIG, requests);
      editState.clearEditedValues(rowKeys);
      selectionState.clearSelection();

      // Clear selections in grid
      clearGridSelectionsForBadges(gridRef.current?.api, badgeNumbers);

      // Check for remaining edits
      const remainingEditKeys = Object.keys(editState.editedValues).filter(
        (key) => !rowKeys.includes(key),
      );
      onUnsavedChanges(remainingEditKeys.length > 0);

      // Generate bulk success message
      const employeeNames = names.map((name) => name || "Unknown Employee");
      const bulkSuccessMessage = generateBulkSaveSuccessMessage(
        ACTIVITY_CONFIG.activityType,
        requests.length,
        employeeNames,
      );

      if (searchParams) {
        const skip = actualCurrentPage * pageSize;
        const request = createRequest(
          skip,
          sortParams.sortBy,
          sortParams.isSortDescending,
          selectedProfitYear,
          pageSize,
        );
        if (request) {
          await triggerSearch(request, false);
          handlePaginationChange(actualCurrentPage, pageSize); // Stay on same page
          setPendingSuccessMessage(bulkSuccessMessage);
          setIsPendingBulkMessage(true);
        }
      }
    } catch (error) {
      console.error("Failed to save forfeiture adjustments:", error);
      const errorMessage = formatApiError(
        error,
        getErrorMessage(ACTIVITY_CONFIG.activityType, "bulkSave"),
      );
      dispatch(
        setMessage({
          key: "TerminationSave",
          message: {
            message: errorMessage,
            type: "error",
          },
        }),
      );
      if (onErrorOccurred) onErrorOccurred();
    } finally {
      editState.removeLoadingRows(badgeNumbers);
      setTimeout(() => setIsBulkSaveInProgress(false), 100); // Reset after Redux update
    }
  },
  [
    updateForfeitureAdjustmentBulk,
    editState,
    selectionState,
    onUnsavedChanges,
    searchParams,
    pageSize,
    sortParams,
    selectedProfitYear,
    createRequest,
    triggerSearch,
    handlePaginationChange,
    dispatch,
    onErrorOccurred,
  ],
);
```

**Pagination Handlers** (Lines 562-579):

```typescript
const paginationHandlers = {
  setPageNumber: (value: number) => {
    if (hasUnsavedChanges) {
      alert("Please save your changes.");
      return;
    }
    handlePaginationChange(value - 1, pageSize); // Convert 1-based to 0-based
    setInitialSearchLoaded(true);
  },
  setPageSize: (value: number) => {
    if (hasUnsavedChanges) {
      alert("Please save your changes.");
      return;
    }
    handlePaginationChange(0, value); // Reset to page 0
    setInitialSearchLoaded(true);
  },
};
```

**Grid Context** (Lines 608-613):

```typescript
gridContext: {
  editedValues: editState.editedValues,        // In-memory edits by rowKey
  updateEditedValue: editState.updateEditedValue,
  loadingRowIds: editState.loadingRowIds,      // Set of loading rows
  isReadOnly                                    // Read-only permission flag
}
```

**Returned Interface**:

```typescript
return {
  // State
  pageNumber,
  pageSize,
  sortParams,
  gridData,
  isFetching,
  termination,
  selectedProfitYear,

  // Edit and selection state
  editState,
  selectionState,

  // Handlers
  handleSave,
  handleBulkSave,
  sortEventHandler,
  onGridReady,

  // Pagination
  paginationHandlers,

  // Refs
  gridRef,

  // Grid context
  gridContext,
};
```

**Lines**: 616

---

## Data Flow

### Search Flow

```
1. User fills filter form (dates, vested balance, checkboxes)
   ↓
2. TerminationSearchFilter validates via Yup schema
   ↓
3. onSearch callback → Termination.tsx → actions.handleSearch()
   ↓
4. useTerminationState reducer: SET_SEARCH_PARAMS action
   - Sets searchParams
   - Sets initialSearchLoaded = true
   - Toggles resetPageFlag (triggers pagination reset)
   - Adds archive flag if archiveMode is true
   ↓
5. TerminationGrid.tsx receives searchParams via props
   ↓
6. useTerminationGrid hook detects searchParams change
   ↓
7. useEffect (lines 244-301) calls createRequest() and triggerSearch()
   - Builds FilterableStartAndEndDateRequest with pagination/sort
   - Includes vestedBalanceValue and vestedBalanceOperator if both set
   - Includes archive flag if shouldArchive is true
   ↓
8. RTK Query: useLazyGetTerminationReportQuery() sends API request
   ↓
9. Backend returns TerminationReportResponse
   - response.results: Array of TerminatedEmployee with yearDetails
   - response.total: Total record count
   - totalEndingBalance, totalVested, totalForfeit, totalBeneficiaryAllocation
   ↓
10. Redux slice updates: yearsEnd.termination state
   ↓
11. useTerminationGrid assembles gridData:
    - Flattens results: one row per employee
    - Matches yearDetail to selectedProfitYear
    - Combines master fields (psn, name) with detail fields
   ↓
12. Grid renders with:
    - Summary totals (TotalsGrid components)
    - Flattened row data
    - Pagination controls
```

### Edit Flow (Suggested Forfeit)

```
1. User clicks editable "Suggested Forfeit" cell
   ↓
2. AG Grid invokes SuggestedForfeitEditor (custom cell editor)
   - Renders currency input with validation
   ↓
3. User enters new value (e.g., 5000.00)
   ↓
4. On blur/enter, editor calls context.updateEditedValue()
   ↓
5. useEditState hook updates editState.editedValues:
   {
     "1234567-2024": { value: 5000, hasError: false }
   }
   ↓
6. Cell re-renders with new value from context:
   - valueGetter reads from editedValues[rowKey] first
   - cellClass applies "bg-blue-50" if hasError is true
   - Cell shows edit indicator (pencil icon)
   ↓
7. useTerminationGrid tracks editState.hasAnyEdits
   ↓
8. onUnsavedChanges(true) called → hasUnsavedChanges = true
   ↓
9. Navigation guard prevents page navigation
   Search button shows warning: "Please save your changes."
   ↓
10. User clicks individual "SAVE" button OR selects rows + "SAVE SELECTED"
```

### Individual Save Flow

```
1. User clicks "SAVE" button in save button column
   ↓
2. Cell renderer calls onSave callback with request and name
   ↓
3. useTerminationGrid.handleSave() executes:

   a. Add row to loadingRowIds set
      ↓
   b. Call prepareSaveRequest() to transform request
      ↓
   c. API: updateForfeitureAdjustment(transformedRequest)
      ↓
   d. On success:
      - Generate row key: "badgeNumber-profitYear"
      - Remove from editedValues
      - Check for remaining edits
      - Update hasUnsavedChanges
      - Generate success message
      - Refresh grid data (stay on same page)
      - Show success message after grid loads
      ↓
   e. On error:
      - Format error message
      - Dispatch error message to Redux
      - Call onErrorOccurred() to scroll to top
      ↓
   f. Finally: Remove row from loadingRowIds
```

### Bulk Save Flow

```
1. User clicks checkboxes in save button column to select multiple rows
   ↓
2. Header component shows "SAVE SELECTED (n)" button
   ↓
3. User clicks "SAVE SELECTED"
   ↓
4. Header calls onBulkSave callback with array of requests and names
   ↓
5. useTerminationGrid.handleBulkSave() executes:

   a. Add all badge numbers to loadingRowIds
      Set isBulkSaveInProgress = true (prevents pagination reset)
      ↓
   b. Call prepareBulkSaveRequests() to transform all requests
      ↓
   c. API: updateForfeitureAdjustmentBulk(transformedRequests)
      ↓
   d. On success:
      - Generate row keys for all requests
      - Clear all edited values for these rows
      - Clear selection state
      - Clear grid row selections
      - Check for remaining edits
      - Update hasUnsavedChanges
      - Generate bulk success message (lists all names)
      - Refresh grid data (stay on same page)
      - Show success message after grid loads
      ↓
   e. On error:
      - Format error message
      - Dispatch error message to Redux
      - Call onErrorOccurred() to scroll to top
      ↓
   f. Finally:
      - Remove all rows from loadingRowIds
      - Reset isBulkSaveInProgress (after delay for Redux update)
```

---

## State Management

### Redux State (yearsEnd slice)

```typescript
termination: {
  startDate: string | null,                // Persisted begin date
  endDate: string | null,                  // Persisted end date
  response: {
    results: TerminatedEmployee[],         // Array of employee records
    total: number                          // Total record count for pagination
  },
  totalEndingBalance: number,              // Sum of all ending balances
  totalVested: number,                     // Sum of all vested amounts
  totalForfeit: number,                    // Sum of all forfeitures
  totalBeneficiaryAllocation: number       // Sum of all beneficiary allocations
}
```

### Local State (useTerminationState hook)

```typescript
state: {
  hasUnsavedChanges: boolean,              // Are there unsaved grid edits?
  initialSearchLoaded: boolean,            // Has initial search been performed?
  searchParams: TerminationSearchRequest | null,  // Current search parameters
  resetPageFlag: boolean,                  // Toggles to trigger pagination reset
  currentStatus: string | null,            // Current page status (In Progress, Complete, etc.)
  archiveMode: boolean,                    // Is archive mode currently active?
  shouldArchive: boolean                   // Should next search use archive=true?
}

actions: {
  handleSearch: (params: TerminationSearchRequest) => void,
  handleStatusChange: (status: string, statusName?: string) => void,
  handleUnsavedChanges: (hasChanges: boolean) => void,
  setInitialSearchLoaded: (loaded: boolean) => void,
  handleArchiveHandled: () => void
}
```

### Grid Context (AG Grid)

```typescript
context: {
  editedValues: Record<string, { value: number, hasError: boolean }>,
  // Key: "badgeNumber-profitYear"
  // Value: { value: edited forfeit amount, hasError: validation error flag }

  updateEditedValue: (rowKey: string, value: number, hasError: boolean) => void,

  loadingRowIds: Set<string>,
  // Set of badge numbers currently saving

  isReadOnly: boolean
  // Global read-only permission flag
}
```

### Edit State (useEditState hook)

```typescript
editState: {
  editedValues: Record<string, { value: number, hasError: boolean }>,
  loadingRowIds: Set<string>,
  hasAnyEdits: boolean,                    // Computed: editedValues has entries

  updateEditedValue: (rowKey: string, value: number, hasError: boolean) => void,
  removeEditedValue: (rowKey: string) => void,
  clearEditedValues: (rowKeys: string[]) => void,
  addLoadingRow: (badgeNumber: string) => void,
  removeLoadingRow: (badgeNumber: string) => void,
  addLoadingRows: (badgeNumbers: string[]) => void,
  removeLoadingRows: (badgeNumbers: string[]) => void
}
```

### Selection State (useRowSelection hook)

```typescript
selectionState: {
  selectedRows: Set<string>,               // Set of selected badge numbers

  addRowToSelection: (badgeNumber: string) => void,
  removeRowFromSelection: (badgeNumber: string) => void,
  clearSelection: () => void,
  isRowSelected: (badgeNumber: string) => boolean
}
```

---

## Key Dependencies

### Custom Hooks

- **useLazyGetAccountingRangeToCurrent(6)** - Fetches fiscal calendar dates (6 months range)
- **useTerminationState()** - Centralized state management with useReducer
- **useTerminationGrid()** - Grid state, API integration, save operations
- **useUnsavedChangesGuard()** - Navigation guard for unsaved changes
- **useDynamicGridHeight()** - Calculates optimal grid height based on viewport
- **useReadOnlyNavigation()** - Detects read-only mode from navigation permissions
- **useDecemberFlowProfitYear()** - Gets current profit year for December activities
- **useEditState()** - Tracks edited values and loading rows
- **useRowSelection()** - Tracks selected rows for bulk operations
- **useGridPagination()** - Pagination and sorting state

### Shared Components

- **StatusDropdownActionNode** - Page status dropdown in header
- **DSMDatePicker** - Date picker with fiscal year constraints
- **DuplicateSsnGuard** - Data quality validation guard (warning mode)
- **SuggestedForfeitEditor** - Custom cell editor for forfeit amounts
- **SuggestedForfeitCellRenderer** - Custom cell renderer with edit icon
- **SharedForfeitHeaderComponent** - Reusable bulk save header (used across forfeit activities)
- **createSaveButtonCellRenderer()** - Save button cell renderer factory

### External Libraries

- **react-hook-form** - Form state management and validation
- **yup** - Form validation schema builder
- **@hookform/resolvers/yup** - Yup resolver for react-hook-form
- **ag-grid-community** - Enterprise-grade data grid
- **smart-ui-library** - UI component library (Page, DSMGrid, DSMAccordion, TotalsGrid, etc.)
- **@mui/material** - Material-UI components (Grid, Button, Checkbox, TextField, MenuItem, etc.)
- **react-redux** - Redux bindings for React
- **@reduxjs/toolkit** - Redux Toolkit with RTK Query

---

## API Integration

### RTK Query Hooks

**Search Query**:

```typescript
useLazyGetTerminationReportQuery();
// Endpoint: GET /api/years-end/termination-report
// Request: FilterableStartAndEndDateRequest & { archive?: boolean }
// Response: TerminationReportResponse
```

**Individual Save Mutation**:

```typescript
useUpdateForfeitureAdjustmentMutation();
// Endpoint: PUT /api/years-end/forfeiture-adjustment
// Request: ForfeitureAdjustmentUpdateRequest
// Response: Success/Error
```

**Bulk Save Mutation**:

```typescript
useUpdateForfeitureAdjustmentBulkMutation();
// Endpoint: PUT /api/years-end/forfeiture-adjustment/bulk
// Request: ForfeitureAdjustmentUpdateRequest[]
// Response: Success/Error
```

### Request Types

**TerminationSearchRequest**:

```typescript
interface TerminationSearchRequest extends StartAndEndDateRequest {
  beginningDate: string; // MM/DD/YYYY format
  endingDate: string; // MM/DD/YYYY format
  profitYear: number;
  forfeitureStatus: string; // "showAll"
  excludeZeroBalance?: boolean;
  excludeZeroAndFullyVested?: boolean;
  vestedBalanceValue?: number | null; // Filter amount
  vestedBalanceOperator?: number | null; // 0=Equals, 1=LessThan, 2=LessThanOrEqual, 3=GreaterThan, 4=GreaterThanOrEqual
  archive?: boolean; // Include archived records
  pagination: {
    skip: number;
    take: number;
    sortBy: string;
    isSortDescending: boolean;
  };
}
```

**ForfeitureAdjustmentUpdateRequest**:

```typescript
interface ForfeitureAdjustmentUpdateRequest {
  badgeNumber: string;
  profitYear: number;
  forfeitureAmount: number; // New suggested forfeit amount
  // ... other fields populated by shared helpers
}
```

### Response Types

**TerminationReportResponse**:

```typescript
interface TerminationReportResponse {
  response: {
    results: TerminatedEmployee[];
    total: number;
  };
  totalEndingBalance: number;
  totalVested: number;
  totalForfeit: number;
  totalBeneficiaryAllocation: number;
}

interface TerminatedEmployee {
  psn: string; // "1234567" or "1234567-1"
  name: string; // "Last, First"
  yearDetails: YearDetail[];
}

interface YearDetail {
  profitYear: number;
  beginningBalance: number;
  beneficiaryAllocation: number;
  distributionAmount: number;
  forfeit: number;
  endingBalance: number;
  vestedBalance: number;
  vestedPercent: number;
  dateTerm: string; // MM/DD/YYYY
  ytdPsHours: number;
  age: number;
  hasForfeited: boolean;
  suggestedForfeit: number | null; // null for beneficiaries
}
```

---

## Design Patterns

### 1. Search-Filter-Grid Pattern

Standard pattern for data exploration pages with collapsible filter section.

### 2. Custom Hook State Management

Encapsulates complex state logic in reusable hooks (`useTerminationState`, `useTerminationGrid`).

### 3. Factory Functions

Standardized column definitions via grid factory functions ensure consistency.

### 4. Context-Based Editing

Uses AG Grid context to track in-memory edits before persistence, avoiding premature API calls.

### 5. Bulk Operations

Shared header component enables multi-row operations with consistent UX.

### 6. Read-Only Mode

Centralized read-only detection disables editing across all components.

### 7. Unsaved Changes Guard

Prevents navigation and search when unsaved changes exist, ensuring data integrity.

### 8. Error Highlighting

Visual feedback (blue background) for validation errors in editable cells.

### 9. Archive Mode Integration

Status dropdown integration automatically filters records based on completion status.

### 10. Shared Utility Helpers

Common functions for row key generation, request transformation, message generation ensure consistency across forfeit activities.

---
