# Termination Page - Technical Documentation

## Overview

The Termination page is a December Activities workflow page for managing employee terminations and processing forfeitures for the current profit year. It demonstrates an advanced master-detail grid pattern with inline editing, bulk operations, and real-time validation.

### Key Features

- **Master-Detail Grid**: Expandable rows showing year-by-year profit details
- **Inline Editing**: Editable "Suggested Forfeit" cells with validation
- **Individual & Bulk Save**: Save single rows or select multiple for bulk operations
- **Unsaved Changes Guard**: Navigation protection when edits are pending
- **Read-Only Mode**: Respects user permissions and frozen year status
- **Archive Functionality**: Special mode for handling archived data
- **Real-Time Totals**: Dynamic summary cards showing aggregated amounts
- **Duplicate SSN Prevention**: Validates prerequisites before search

---

## Architecture Overview

### Component Hierarchy

```
Termination (Page Wrapper)
├── StatusDropdownActionNode (Navigation status tracking)
├── ApiMessageAlert (Save success/error messages)
├── TerminationSearchFilter (Date range + forfeiture status)
│   ├── DuplicateSsnGuard (Prerequisite validation)
│   └── React Hook Form (yup validation)
└── TerminationGrid (Master-detail AG Grid)
    ├── useTerminationGrid (Custom hook - main logic)
    ├── ReportSummary (Record count display)
    ├── TotalsGrid × 4 (Sticky summary cards)
    ├── DSMGrid (AG Grid wrapper)
    │   ├── Expansion Column (▼/► icons)
    │   ├── Main Columns (Badge, Name)
    │   └── Detail Columns (Year-specific data + save controls)
    └── Pagination
```

### File Structure

```
DecemberActivities/Termination/
├── Termination.tsx                         # Main page component
├── TerminationSearchFilter.tsx             # Date range filter form
├── TerminationGrid.tsx                     # Master-detail grid display
├── TerminationGridColumns.tsx              # Main row column definitions (Badge, Name)
├── TerminationDetailsGridColumns.tsx       # Detail row columns (profit year data + edit controls)
└── TerminationHeaderComponent.tsx          # Bulk save header with SelectableGridHeader
```

### Custom Hooks Used

```
hooks/
├── useTerminationState.ts          # State management (search params, unsaved changes, archive)
├── useTerminationGrid.ts           # Grid logic (expansion, editing, save operations)
├── useFiscalCalendarYear.ts        # Fiscal date range fetching
├── useDecemberFlowProfitYear.ts    # Current profit year
├── useUnsavedChangesGuard.ts       # Navigation blocking
├── useReadOnlyNavigation.ts        # Permission checking
└── useDynamicGridHeight.ts         # Responsive grid sizing
```

---

## State Management

### useTerminationState Hook

Centralized state management for the entire Termination page.

```typescript
const { state, actions } = useTerminationState();

// State shape
interface TerminationState {
  initialSearchLoaded: boolean; // Has user performed initial search?
  searchParams: TerminationSearchRequest | null; // Last search parameters
  resetPageFlag: boolean; // Trigger pagination reset
  hasUnsavedChanges: boolean; // Prevent navigation
  shouldArchive: boolean; // Archive mode active
}

// Actions
actions.handleSearch(params); // Execute search
actions.setInitialSearchLoaded(true); // Mark search as performed
actions.handleUnsavedChanges(true); // Track edit state
actions.handleStatusChange(); // Navigation status update
actions.handleArchiveHandled(); // Clear archive flag
```

**Key Pattern**: State and actions separated for clean interface. Actions encapsulate all state mutations.

### useTerminationGrid Hook

Manages grid-specific logic: row expansion, editing, save operations, pagination.

```typescript
const {
  pageNumber,
  pageSize,
  gridData, // Flattened main + detail rows

  termination, // API response data
  selectedProfitYear, // Current profit year

  selectionState, // Row selection tracking
  handleSave, // Individual save
  handleBulkSave, // Bulk save
  handleRowExpansion, // Expand/collapse rows
  sortEventHandler, // Grid sorting
  onGridReady, // Grid initialization
  paginationHandlers, // Pagination callbacks
  gridRef, // Grid API reference
  gridContext // Context for cell renderers
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
  onLoadingChange
});
```

**Grid Context Shape**:

```typescript
gridContext = {
  editedValues: Record<string, { value?: number; hasError?: boolean }>,
  loadingRowIds: Set<string>
  // ... other context data
};
```

**Context Usage**: Cell renderers access `params.context` to get edited values and loading states.

---

## Master-Detail Grid Pattern

### Data Structure

**Main Row** (Employee-level):

```typescript
{
  badgeNumber: number;
  psnSuffix?: number;
  name: string;
  isExpandable: true;
  isExpanded: false;
  isDetail: false;
  // ... other employee fields
}
```

**Detail Row** (Year-level):

```typescript
{
  badgeNumber: number; // Parent reference
  profitYear: number; // Unique per detail
  isDetail: true; // Flag for rendering logic
  beginningBalance: number;
  endingBalance: number;
  vestedBalance: number;
  suggestedForfeit: number; // Editable field
  hasForfeited: boolean;
  // ... other year-specific fields
}
```

**Flattened Grid Data**:

```typescript
gridData = [
  { badgeNumber: 123456, name: "Doe, John", isExpandable: true, isExpanded: false, isDetail: false },
  // If expanded:
  { badgeNumber: 123456, profitYear: 2025, isDetail: true, suggestedForfeit: 1500, ... },
  { badgeNumber: 123456, profitYear: 2024, isDetail: true, suggestedForfeit: 0, ... },
  { badgeNumber: 789012, name: "Smith, Jane", isExpandable: true, isExpanded: false, isDetail: false },
  // ...
]
```

### Column Definitions

**Expansion Column** (Always first):

```typescript
const expansionColumn = {
  headerName: "",
  field: "isExpandable",
  width: 50,
  cellRenderer: (params) => {
    if (params.data && !params.data.isDetail && params.data.isExpandable) {
      return params.data.isExpanded ? "▼" : "►";
    }
    return "";
  },
  onCellClicked: (event) => {
    if (event.data && !event.data.isDetail && event.data.isExpandable) {
      handleRowExpansion(event.data.badgeNumber.toString());
    }
  },
  pinned: "left" as const
};
```

**Main Columns** (Visible in all rows):

```typescript
export const GetTerminationColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      psnSuffix: true // Shows PSN suffix if present
    }),
    createNameColumn({
      field: "name"
    })
  ];
};
```

**Detail Columns** (Only visible in detail rows):

```typescript
export const GetDetailColumns = (
  addRowToSelectedRows,
  removeRowFromSelectedRows,
  selectedProfitYear,
  onSave,
  onBulkSave,
  isReadOnly
): ColDef[] => {
  return [
    createYearColumn({ field: "profitYear" }),
    createCurrencyColumn({ field: "beginningBalance" }),
    createCurrencyColumn({ field: "beneficiaryAllocation" }),
    createCurrencyColumn({ field: "distributionAmount" }),
    createCurrencyColumn({ field: "forfeit" }),
    createCurrencyColumn({ field: "endingBalance" }),
    createCurrencyColumn({ field: "vestedBalance" }),
    { field: "vestedPercent", valueFormatter: (p) => `${p.value}%` },
    createDateColumn({ field: "dateTerm" }),
    createHoursColumn({ field: "ytdPsHours" }),
    createAgeColumn({}),
    createYesOrNoColumn({ field: "hasForfeited" }),

    // EDITABLE FIELD
    {
      headerName: "Suggested Forfeit",
      field: "suggestedForfeit",
      pinned: "right",
      editable: ({ node }) => node.data.isDetail && node.data.profitYear === selectedProfitYear,
      cellEditor: SuggestedForfeitEditor,
      cellRenderer: (params) => SuggestedForfeitCellRenderer(params, true, false),
      valueGetter: (params) => {
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
        const editedValue = params.context?.editedValues?.[rowKey]?.value;
        return editedValue ?? params.data.suggestedForfeit ?? 0;
      }
    },

    // SAVE CONTROLS
    {
      headerName: "Save Button",
      field: "saveButton",
      pinned: "right",
      headerComponent: HeaderComponent,
      cellRenderer: SaveButtonCellRenderer
    }
  ];
};
```

### Cell Renderer Logic for Hybrid Columns

Main columns hide content for detail rows unless the same field exists in detail columns:

```typescript
const visibleColumns = mainColumns.map((column) => {
  return {
    ...column,
    cellRenderer: (params: ICellRendererParams) => {
      // For detail rows, hide this column unless it's also in detailColumns
      if (params.data?.isDetail) {
        const hideInDetails = !detailColumns.some((detailCol) => detailCol.field === (column as ColDef).field);
        if (hideInDetails) {
          return ""; // Hide content for detail rows
        }
      }

      // Use default renderer for main rows or shared fields
      if ((column as ColDef).cellRenderer) {
        return (column as ColDef).cellRenderer(params);
      }
      return params.valueFormatted || params.value;
    }
  };
});
```

Detail-only columns show nothing for main rows:

```typescript
const detailOnlyColumns = detailColumns
  .filter((detailCol) => !mainColumns.some((mainCol) => mainCol.field === detailCol.field))
  .map((column) => ({
    ...column,
    cellRenderer: (params) => {
      if (!params.data?.isDetail) {
        return ""; // Hide for main rows
      }
      // Use default renderer for detail rows
      if ((column as ColDef).cellRenderer) {
        return (column as ColDef).cellRenderer(params);
      }
      return params.valueFormatted || params.value;
    }
  }));
```

**Final Column Array**:

```typescript
const columnDefs = [expansionColumn, ...visibleColumns, ...detailOnlyColumns];
```

---

## Inline Editing Pattern

### Editable Cell Configuration

```typescript
{
  headerName: "Suggested Forfeit",
  field: "suggestedForfeit",
  editable: ({ node }) =>
    node.data.isDetail && node.data.profitYear === selectedProfitYear,
  cellEditor: SuggestedForfeitEditor,  // Custom editor component
  cellRenderer: (params) =>
    SuggestedForfeitCellRenderer(params, true, false),
  valueGetter: (params) => {
    const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
    const editedValue = params.context?.editedValues?.[rowKey]?.value;
    return editedValue ?? params.data.suggestedForfeit ?? 0;
  },
  cellClass: (params) => {
    if (!params.data.isDetail) return "";
    const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
    const hasError = params.context?.editedValues?.[rowKey]?.hasError;
    return hasError ? "bg-red-50" : "";
  }
}
```

**Key Features**:

- **Conditional editability**: Only current profit year rows are editable
- **Custom editor**: `SuggestedForfeitEditor` handles input validation
- **Value getter**: Pulls from `editedValues` context if edited, else original data
- **Error styling**: Red background for invalid values

### Edit Tracking in Context

```typescript
// In useTerminationGrid hook
const [editedValues, setEditedValues] = useState<Record<string, { value?: number; hasError?: boolean }>>({});

// Grid context includes editedValues
const gridContext = useMemo(
  () => ({
    editedValues,
    loadingRowIds
    // ...
  }),
  [editedValues, loadingRowIds]
);

// Editor component calls onValueChange callback
// (Handled by SuggestedForfeitEditor)
```

**Edited Values Structure**:

```typescript
editedValues = {
  "123456-2025": { value: 1500, hasError: false },
  "789012-2025": { value: -100, hasError: true } // Invalid (negative)
};
```

---

## Save Operations

### Individual Save

**Save Button Cell Renderer**:

```typescript
cellRenderer: (params: SaveButtonCellParams) => {
  if (!params.data.isDetail || params.data.profitYear !== selectedProfitYear) {
    return "";
  }

  const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
  const currentValue = params.context?.editedValues?.[rowKey]?.value
    ?? params.data.suggestedForfeit;
  const isLoading = params.context?.loadingRowIds?.has(params.data.badgeNumber);
  const isZeroValue = currentValue === 0 || currentValue == null;
  const isDisabled = hasError || isLoading || isZeroValue || isReadOnly;

  return (
    <div>
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
      <IconButton
        onClick={async () => {
          if (params.onSave) {
            const request: ForfeitureAdjustmentUpdateRequest = {
              badgeNumber: params.data.badgeNumber,
              profitYear: params.data.profitYear,
              forfeitureAmount: currentValue || 0,
              classAction: false
            };
            await params.onSave(request, params.data.fullName);
          }
        }}
        disabled={isDisabled}>
        {isLoading ? <CircularProgress size={20} /> : <SaveOutlined />}
      </IconButton>
    </div>
  );
}
```

**Save Handler** (in useTerminationGrid):

```typescript
const handleSave = async (request: ForfeitureAdjustmentUpdateRequest, name: string) => {
  // Add to loading set
  setLoadingRowIds((prev) => new Set(prev).add(request.badgeNumber.toString()));

  try {
    await saveAdjustmentMutation(request).unwrap();

    // Clear edited value for this row
    const rowKey = `${request.badgeNumber}-${request.profitYear}`;
    setEditedValues((prev) => {
      const updated = { ...prev };
      delete updated[rowKey];
      return updated;
    });

    // Show success message
    ApiMessageAlert.setMessage({
      key: "TerminationSave",
      message: {
        type: "success",
        text: `Saved forfeiture for ${name}`
      }
    });

    // Refresh grid data
    await refetchData();
  } catch (error) {
    // Show error message
    ApiMessageAlert.setMessage({
      key: "TerminationSave",
      message: { type: "error", text: error.message }
    });
  } finally {
    // Remove from loading set
    setLoadingRowIds((prev) => {
      const updated = new Set(prev);
      updated.delete(request.badgeNumber.toString());
      return updated;
    });
  }
};
```

### Bulk Save

**Header Component** (`TerminationHeaderComponent.tsx`):

```typescript
export const HeaderComponent: React.FC<HeaderComponentProps> = (params) => {
  const selectedProfitYear = useDecemberFlowProfitYear();

  // Determine which nodes are eligible for bulk save
  const isNodeEligible = (nodeData, context) => {
    if (!nodeData.isDetail || nodeData.profitYear !== selectedProfitYear)
      return false;

    const rowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}`;
    const currentValue = context?.editedValues?.[rowKey]?.value
      ?? nodeData.suggestedForfeit;

    return (currentValue || 0) !== 0;
  };

  // Create save payload for a node
  const createUpdatePayload = (nodeData, context) => {
    const rowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}`;
    const currentValue = context?.editedValues?.[rowKey]?.value
      ?? nodeData.suggestedForfeit;

    return {
      badgeNumber: Number(nodeData.badgeNumber),
      profitYear: nodeData.profitYear,
      forfeitureAmount: -(currentValue || 0),  // Note: negated for unforfeit
      classAction: false
    };
  };

  const hasSavingInProgress = () => {
    return params.context?.loadingRowIds?.size > 0;
  };

  return (
    <SelectableGridHeader
      {...params}
      isNodeEligible={isNodeEligible}
      createUpdatePayload={createUpdatePayload}
      isBulkSaving={hasSavingInProgress}
      isReadOnly={params.isReadOnly}
    />
  );
};
```

**SelectableGridHeader Component** (reusable):

```typescript
<SelectableGridHeader
  // AG Grid header props (from params)
  displayName="Save Button"
  api={params.api}
  columnApi={params.columnApi}
  context={params.context}

  // Custom props
  isNodeEligible={isNodeEligible}           // Filter function
  createUpdatePayload={createUpdatePayload} // Transform function
  onBulkSave={onBulkSave}                   // Save handler
  isBulkSaving={isBulkSaving}               // Loading state
  isReadOnly={isReadOnly}                   // Permission check
/>
```

**Bulk Save Handler** (in useTerminationGrid):

```typescript
const handleBulkSave = async (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => {
  // Add all badges to loading set
  setLoadingRowIds((prev) => {
    const updated = new Set(prev);
    requests.forEach((req) => updated.add(req.badgeNumber.toString()));
    return updated;
  });

  try {
    // API call for bulk save (may be sequential or batched)
    await Promise.all(requests.map((req) => saveAdjustmentMutation(req).unwrap()));

    // Clear edited values for all saved rows
    setEditedValues((prev) => {
      const updated = { ...prev };
      requests.forEach((req) => {
        const rowKey = `${req.badgeNumber}-${req.profitYear}`;
        delete updated[rowKey];
      });
      return updated;
    });

    // Show success message
    ApiMessageAlert.setMessage({
      key: "TerminationSave",
      message: {
        type: "success",
        text: `Saved ${requests.length} forfeitures`
      }
    });

    // Refresh grid
    await refetchData();
  } catch (error) {
    // Error handling
  } finally {
    // Clear loading states
    setLoadingRowIds(new Set());
  }
};
```

---

## Search Filter

### Filter Component

**File**: `TerminationSearchFilter.tsx`

```typescript
const schema = yup.object().shape({
  beginningDate: dateStringValidator(2000, 2099, "Beginning Date")
    .required("Beginning Date is required"),
  endingDate: endDateStringAfterStartDateValidator(
    "beginningDate",
    tryddmmyyyyToDate,
    "Ending date must be the same or after the beginning date"
  ).required("Ending Date is required"),
  forfeitureStatus: yup.string().required("Forfeiture Status is required"),
  pagination: yup.object({ skip, take, sortBy, isSortDescending }).required(),
  profitYear: profitYearValidator(2015, 2099)
});

const TerminationSearchFilter = ({ fiscalData, onSearch, hasUnsavedChanges }) => {
  const { control, handleSubmit, formState: { errors, isValid } } = useForm({
    resolver: yupResolver(schema),
    defaultValues: {
      beginningDate: fiscalData.fiscalBeginDate,
      endingDate: fiscalData.fiscalEndDate,
      forfeitureStatus: "showAll",
      pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false }
    }
  });

  const validateAndSubmit = async (data) => {
    if (hasUnsavedChanges) {
      alert("Please save your changes.");
      return;
    }

    onSearch({
      ...data,
      beginningDate: mmDDYYFormat(data.beginningDate),
      endingDate: mmDDYYFormat(data.endingDate)
    });
  };

  return (
    <form onSubmit={handleSubmit(validateAndSubmit)}>
      <DsmDatePicker name="beginningDate" control={control} />
      <DsmDatePicker name="endingDate" control={control} />

      <DuplicateSsnGuard mode="warning">
        {({ prerequisitesComplete }) => (
          <SearchAndReset
            handleSearch={handleSubmit(validateAndSubmit)}
            handleReset={handleReset}
            disabled={!isValid || !prerequisitesComplete}
          />
        )}
      </DuplicateSsnGuard>
    </form>
  );
};
```

**Key Features**:

- **Date validation**: End date must be >= begin date
- **Fiscal bounds**: Dates constrained to fiscal year range
- **Prerequisite check**: `DuplicateSsnGuard` blocks search if duplicate SSNs exist
- **Unsaved changes**: Alert prevents search if edits are pending

---

## Totals Display

### Sticky Summary Cards

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

**Styling**:

- `sticky top-0 z-10`: Stays visible when scrolling grid
- `flex`: Horizontal layout
- `bg-white`: Opaque background

**Data Source**: API response includes totals in `termination` object.

---

## Row Expansion Logic

### Expansion State Tracking

```typescript
// In useTerminationGrid
const [expandedRows, setExpandedRows] = useState<Set<string>>(new Set());

const handleRowExpansion = useCallback((badgeNumber: string) => {
  setExpandedRows((prev) => {
    const updated = new Set(prev);
    if (updated.has(badgeNumber)) {
      updated.delete(badgeNumber); // Collapse
    } else {
      updated.add(badgeNumber); // Expand
    }
    return updated;
  });
}, []);
```

### Flattening Data for Grid

```typescript
const gridData = useMemo(() => {
  if (!termination?.response?.results) return [];

  return termination.response.results.flatMap((employee) => {
    const mainRow = {
      ...employee,
      isExpandable: true,
      isExpanded: expandedRows.has(employee.badgeNumber.toString()),
      isDetail: false
    };

    // If expanded, include detail rows
    if (expandedRows.has(employee.badgeNumber.toString())) {
      const detailRows = employee.profitDetails.map((detail) => ({
        ...detail,
        badgeNumber: employee.badgeNumber, // Parent reference
        name: employee.name,
        isDetail: true
      }));
      return [mainRow, ...detailRows];
    }

    // Otherwise just main row
    return [mainRow];
  });
}, [termination, expandedRows]);
```

**Result**: Flat array suitable for AG Grid, with detail rows interspersed.

---

## Unsaved Changes Protection

### Navigation Guard

```typescript
// In Termination.tsx
const { state, actions } = useTerminationState();
useUnsavedChangesGuard(state.hasUnsavedChanges);
```

**useUnsavedChangesGuard Hook**:

```typescript
export const useUnsavedChangesGuard = (hasUnsavedChanges: boolean) => {
  useEffect(() => {
    const handleBeforeUnload = (e: BeforeUnloadEvent) => {
      if (hasUnsavedChanges) {
        e.preventDefault();
        e.returnValue = ""; // Chrome requires returnValue to be set
      }
    };

    window.addEventListener("beforeunload", handleBeforeUnload);

    return () => {
      window.removeEventListener("beforeunload", handleBeforeUnload);
    };
  }, [hasUnsavedChanges]);

  // React Router navigation blocking
  useBlocker(
    ({ currentLocation, nextLocation }) => hasUnsavedChanges && currentLocation.pathname !== nextLocation.pathname
  );
};
```

**User Experience**:

- Browser refresh/close: Shows browser confirmation dialog
- React Router navigation: Shows modal with "Stay" / "Leave" buttons
- Search button: Disabled with alert message

---

## Read-Only Mode

### Permission Check

```typescript
// In TerminationGrid
const isReadOnly = useReadOnlyNavigation();

// Pass to column definitions
const detailColumns = GetDetailColumns(
  addRowToSelection,
  removeRowFromSelection,
  selectedProfitYear,
  handleSave,
  handleBulkSave,
  isReadOnly // <-- Permission flag
);
```

### Disabling Edit Controls

**Editable Check**:

```typescript
{
  field: "suggestedForfeit",
  editable: ({ node }) =>
    node.data.isDetail &&
    node.data.profitYear === selectedProfitYear &&
    !isReadOnly,  // <-- Blocked in read-only
}
```

**Save Button Disabled**:

```typescript
const isDisabled = hasError || isLoading || isZeroValue || isReadOnly;

<IconButton disabled={isDisabled}>
  <SaveOutlined />
</IconButton>
```

**Tooltip Messaging**:

```typescript
{isReadOnly ? (
  <Tooltip title="You are in read-only mode and cannot save changes.">
    <span>{saveButtonElement}</span>
  </Tooltip>
) : (
  saveButtonElement
)}
```

---

## Archive Mode

### Activation

Archive mode is triggered by status change in navigation:

```typescript
// In Termination.tsx
const renderActionNode = () => {
  return <StatusDropdownActionNode onStatusChange={actions.handleStatusChange} />;
};

// useTerminationState handles archive flag
actions.handleStatusChange = () => {
  // Sets state.shouldArchive = true
};
```

### Behavior

When `shouldArchive` is true:

1. **Search Parameter Modified**:

```typescript
const searchParams = {
  ...state.searchParams,
  archive: state.shouldArchive // Adds archive flag
};
```

2. **API Returns Archived Data**: Backend filters to archived records

3. **Grid Displays Archived Records**: Same UI, different data source

4. **Archive Handled Callback**: Clears flag after successful fetch

```typescript
useEffect(() => {
  if (state.shouldArchive) {
    // Fetch archived data
    triggerSearch({ ...searchParams, archive: true });
    onArchiveHandled(); // Clear flag
  }
}, [state.shouldArchive]);
```

---

## Error Handling

### Scroll to Top on Error

```typescript
const scrollToTop = useCallback(() => {
  window.scrollTo({ top: 0, behavior: "smooth" });
}, []);

// Listen for error messages
useEffect(() => {
  const handleMessageEvent = (event: CustomEvent) => {
    if (event.detail?.key === "TerminationSave" &&
        event.detail?.message?.type === "error") {
      scrollToTop();
    }
  };

  window.addEventListener("dsmMessage", handleMessageEvent);
  return () => window.removeEventListener("dsmMessage", handleMessageEvent);
}, [scrollToTop]);

// Pass error handler to grid
<TerminationGrid
  onErrorOccurred={scrollToTop}
  // ...
/>
```

**User Experience**: Error messages appear at top of page (via `ApiMessageAlert`). Auto-scroll ensures visibility.

### Validation Errors

**Cell-Level Errors**:

```typescript
cellClass: (params) => {
  const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
  const hasError = params.context?.editedValues?.[rowKey]?.hasError;
  return hasError ? "bg-red-50" : ""; // Red background
};
```

**Form-Level Errors**:

React Hook Form displays errors inline with form fields.

---

## API Integration

### Endpoints

**1. Search Terminations**:

```typescript
useLazyGetTerminationsQuery()

// Request
{
  beginningDate: "01/01/2025",
  endingDate: "12/31/2025",
  forfeitureStatus: "showAll",
  profitYear: 2025,
  pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false },
  archive?: boolean
}

// Response
{
  response: {
    results: [
      {
        badgeNumber: 123456,
        name: "Doe, John",
        profitDetails: [
          { profitYear: 2025, beginningBalance: 10000, ... },
          { profitYear: 2024, beginningBalance: 9500, ... }
        ]
      }
    ],
    total: 150
  },
  totalEndingBalance: 1500000,
  totalVested: 1200000,
  totalForfeit: 50000,
  totalBeneficiaryAllocation: 25000
}
```

**2. Save Forfeiture Adjustment**:

```typescript
useSaveForfeitureAdjustmentMutation()

// Request
{
  badgeNumber: 123456,
  profitYear: 2025,
  forfeitureAmount: 1500,
  classAction: false,
  offsettingProfitDetailId?: number
}

// Response
{
  success: true,
  message: "Forfeiture adjustment saved"
}
```

---

## Performance Optimizations

### 1. Memoized Column Definitions

```typescript
const mainColumns = useMemo(() => GetTerminationColumns(), []);
const detailColumns = useMemo(
  () =>
    GetDetailColumns(
      addRowToSelection,
      removeRowFromSelection,
      selectedProfitYear,
      handleSave,
      handleBulkSave,
      isReadOnly
    ),
  [addRowToSelection, removeRowFromSelection, selectedProfitYear, handleSave, handleBulkSave, isReadOnly]
);
```

**Why**: Column definitions are expensive to create. Memoization prevents recreation on every render.

### 2. Grid Context Memoization

```typescript
const gridContext = useMemo(
  () => ({
    editedValues,
    loadingRowIds
    // ...
  }),
  [editedValues, loadingRowIds]
);
```

**Why**: Context object identity stability prevents unnecessary cell re-renders.

### 3. Flattened Grid Data

```typescript
const gridData = useMemo(() => {
  // Flatten main + detail rows
  return termination.response.results.flatMap(/* ... */);
}, [termination, expandedRows]);
```

**Why**: AG Grid requires flat array. Pre-computing once per data change is more efficient than computing in renderer.

### 4. Dynamic Grid Height

```typescript
const gridMaxHeight = useDynamicGridHeight();

<DSMGrid maxHeight={gridMaxHeight} />
```

**Why**: Maximizes visible rows without manual scrolling. Adjusts to viewport size.

---

## Common Patterns

### 1. Row Key Generation

Consistent key format for tracking edited values and loading states:

```typescript
const rowKey = `${badgeNumber}-${profitYear}`;
```

### 2. Conditional Rendering in Cells

Check `isDetail` flag to determine which content to show:

```typescript
cellRenderer: (params) => {
  if (params.data?.isDetail) {
    // Detail row content
  } else {
    // Main row content
  }
};
```

### 3. Context-Based Value Retrieval

Prefer edited value from context over original data:

```typescript
valueGetter: (params) => {
  const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
  const editedValue = params.context?.editedValues?.[rowKey]?.value;
  return editedValue ?? params.data.originalValue ?? 0;
};
```

### 4. Loading State Management with Sets

```typescript
const [loadingRowIds, setLoadingRowIds] = useState<Set<string>>(new Set());

// Add
setLoadingRowIds((prev) => new Set(prev).add(badgeNumber));

// Remove
setLoadingRowIds((prev) => {
  const updated = new Set(prev);
  updated.delete(badgeNumber);
  return updated;
});

// Check
const isLoading = loadingRowIds.has(badgeNumber);
```
