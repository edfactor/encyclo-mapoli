---
applyTo: "src/ui/src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/**/*.*"
---
# Manage Executive Hours and Dollars Technical Documentation

## Overview

The Manage Executive Hours and Dollars feature allows users to search for executives and update their profit-sharing hours and dollar allocations during the December year-end process. The implementation features inline grid editing, a modal for adding executives, pending changes tracking with visual indicators, and comprehensive validation.

## Architecture Overview

The Manage Executive Hours and Dollars feature implements a **Complex Page with Custom Hook** pattern with a unique dual-grid architecture: a main grid for editing and a modal with a separate search grid for adding executives.

```
┌──────────────────────────────────────────────────────────────┐
│ ManageExecutiveHoursAndDollars.tsx (Page Container)          │
│ - Page wrapper with save button in action node              │
│ - Status management                                         │
│ - Read-only mode handling                                   │
│ - MissiveAlertProvider for user messages                    │
└────────────────┬─────────────────────────────────────────────┘
                 │
    ┌────────────┴────────────┐
    │                         │
┌───▼──────────────────┐  ┌──▼─────────────────────────────────┐
│ Search Filter        │  │ ManageExecutiveHoursAndDollarsGrid │
│ - Multiple search    │  │ - Inline editable grid             │
│   criteria           │  │ - Visual change indicators         │
│ - Mutual exclusion   │  │ - Add Executive button             │
│ - Validation         │  │ - Pending changes tracking         │
└──────────────────────┘  └────────────┬───────────────────────┘
                                       │
                          ┌────────────┴────────────┐
                          │                         │
              ┌───────────▼──────────┐  ┌──────────▼────────────┐
              │ SmartModal           │  │ Column Definitions    │
              │ SearchAndAddExecutive│  │ - Full/Mini modes     │
              │ - Modal search filter│  │ - Editable columns    │
              │ - Modal grid         │  │ - Read-only columns   │
              │ - Add to main button │  │                       │
              └──────────────────────┘  └───────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│ useManageExecutiveHoursAndDollars Hook                          │
│ - Dual grid state (main + modal)                               │
│ - Pending changes tracking                                     │
│ - Reducer-based state management                               │
│ - Combined data handling (search results + added executives)   │
└─────────────────────────────────────────────────────────────────┘
```

## Component Hierarchy

```
ManageExecutiveHoursAndDollars (Page Container)
├── MissiveAlertProvider (Error/success message context)
│   └── ManageExecutiveHoursAndDollarsInner
│       ├── Page (from smart-ui-library)
│       │   ├── label: "MANAGE EXECUTIVE HOURS AND DOLLARS (008-10)"
│       │   └── actionNode: Save Button + StatusDropdownActionNode
│       └── ManageExecutiveHoursAndDollarsContent
│           ├── FrozenYearWarning (Conditional)
│           ├── StatusReadOnlyInfo (Conditional)
│           ├── Divider
│           ├── DSMAccordion (Collapsible filter)
│           │   └── ManageExecutiveHoursAndDollarsSearchFilter
│           │       ├── FormLabel + TextField (Profit Year - disabled)
│           │       ├── FormLabel + TextField (Full Name)
│           │       ├── FormLabel + TextField (SSN)
│           │       ├── FormLabel + TextField (Badge Number)
│           │       ├── FormLabel + Checkbox (Has Executive Hours and Dollars)
│           │       ├── FormLabel + Checkbox (Monthly Payroll)
│           │       └── SearchAndReset (Action buttons)
│           └── ManageExecutiveHoursAndDollarsGrid (Conditional on showGrid)
│               ├── ReportSummary (Total counts)
│               ├── RenderAddExecutiveButton
│               ├── DSMGrid (AG Grid wrapper)
│               │   ├── Badge column
│               │   ├── Full Name column
│               │   ├── Store column
│               │   ├── SSN column
│               │   ├── Executive Hours column (editable)
│               │   ├── Executive Dollars column (editable)
│               │   ├── Oracle Hours column (read-only)
│               │   ├── Oracle Dollars column (read-only)
│               │   ├── Pay Frequency column
│               │   └── Employment Status column
│               ├── Pagination
│               └── SmartModal (Add Executive Modal)
│                   └── SearchAndAddExecutive
│                       ├── Page (Modal page wrapper)
│                       │   ├── label: "Add New Executive"
│                       │   └── actionNode: RenderAddButton (Add to Main Grid)
│                       ├── DSMAccordion (Modal filter)
│                       │   └── ManageExecutiveHoursAndDollarsSearchFilter (isModal=true)
│                       │       ├── FormLabel + TextField (Full Name) *
│                       │       ├── FormLabel + TextField (SSN) *
│                       │       └── FormLabel + TextField (Badge Number) *
│                       └── ManageExecutiveHoursAndDollarsGrid (isModal=true)
│                           ├── Instruction text (red)
│                           ├── DSMGrid (with row selection)
│                           │   ├── Badge column
│                           │   ├── Full Name column
│                           │   ├── Executive Hours column (read-only in modal)
│                           │   └── Executive Dollars column (read-only in modal)
│                           └── Pagination
```

**Note**: \* indicates required field in modal

---

## File Descriptions

### 1. `ManageExecutiveHoursAndDollars.tsx`

**Purpose**: Main page container with save button and user message handling

**Responsibilities**:

- Page layout and structure
- Save button rendering in action node
- Status change handling
- Read-only mode enforcement
- User message display via MissiveAlerts
- Component composition

**Key Features**:

#### Save Button in Action Node

Unlike Termination/UnForfeit which save individual rows, this feature has a global save button:

```typescript
const RenderSaveButton = memo(({ hasPendingChanges, onSave, isReadOnly = true }: RenderSaveButtonProps) => {
  const isDisabled = !hasPendingChanges || isReadOnly;
  const readOnlyTooltip = "You are in read-only mode and cannot save changes.";
  const noPendingChangesTooltip = "You must change hours or dollars to save.";

  const saveButton = (
    <Button
      disabled={isDisabled}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<SaveOutlined color={isDisabled ? "disabled" : "primary"} />}
      onClick={isReadOnly ? undefined : onSave}>
      Save
    </Button>
  );

  const tooltipTitle = isReadOnly ? readOnlyTooltip : noPendingChangesTooltip;

  if (isDisabled) {
    return (
      <Tooltip placement="top" title={tooltipTitle}>
        <span>{saveButton}</span>
      </Tooltip>
    );
  } else {
    return saveButton;
  }
});
```

**Disabled Conditions**:

1. No pending changes (`hasPendingChanges === false`)
2. Read-only mode (`isReadOnly === true`)

**Tooltips**:

- Read-only: "You are in read-only mode and cannot save changes."
- No changes: "You must change hours or dollars to save."

#### Action Node Composition

```typescript
const renderActionNode = () => {
  return (
    <Box sx={{ display: "flex", gap: 2 }}>
      <RenderSaveButton
        hasPendingChanges={hasPendingChanges}
        onSave={handleSave}
        isReadOnly={isReadOnly}
      />
      <StatusDropdownActionNode />
    </Box>
  );
};
```

The action node contains both the save button and the status dropdown, displayed side-by-side.

#### MissiveAlerts Integration

```typescript
const handleSave = useCallback(async () => {
  try {
    await saveChanges();
    addAlert(EXECUTIVE_HOURS_AND_DOLLARS_MESSAGES.EXECUTIVE_HOURS_SAVED_SUCCESS);
  } catch (_error) {
    addAlert(EXECUTIVE_HOURS_AND_DOLLARS_MESSAGES.EXECUTIVE_HOURS_SAVE_ERROR);
  }
}, [saveChanges, addAlert]);
```

Success and error messages displayed via `MissiveAlertProvider` context.

#### Read-Only Guards

The component uses multiple hooks to enforce read-only mode:

- `useReadOnlyNavigation()` - Check navigation-based read-only state
- `useIsReadOnlyByStatus()` - Check status-based read-only state
- `useIsProfitYearFrozen(profitYear)` - Check if profit year is frozen

**File Location**: `/src/ui/src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/ManageExecutiveHoursAndDollars.tsx`

---

### 2. `ManageExecutiveHoursAndDollarsGrid.tsx`

**Purpose**: Dual-mode grid component (main or modal)

**Responsibilities**:

- Grid rendering with editable cells
- Mutable row data management for inline editing
- Validation (hours ≤ 4000, dollars ≤ 20,000,000)
- Visual change indicators (yellow background for unsaved changes)
- Modal grid with row selection
- Add Executive button rendering
- Pagination handling

**Key Features**:

#### Dual Mode Architecture

The grid operates in two modes controlled by `isModal` prop:

| Mode      | Usage                 | Features                                                     |
| --------- | --------------------- | ------------------------------------------------------------ |
| **Main**  | Primary editing grid  | Inline editing, visual indicators, add button, save tracking |
| **Modal** | Executive search grid | Row selection (checkboxes), read-only, instruction text      |

```typescript
const ManageExecutiveHoursAndDollarsGrid: React.FC<Props> = ({
  isModal = false,
  gridData = null, // Main grid data
  modalResults = null // Modal grid data
  // ... other props
}) => {
  const currentData = isModal ? modalResults : gridData;
  const currentPagination = isModal ? modalGridPagination : mainGridPagination;
  const columnDefs = useMemo(() => GetManageExecutiveHoursAndDollarsColumns(isModal), [isModal]);
  // ...
};
```

#### Mutable Row Data Pattern

To support inline editing without React re-render issues:

```typescript
const [mutableRowData, setMutableRowData] = useState<ExecutiveHoursAndDollars[]>([]);
const isEditingRef = useRef(false);
const dataInitializedRef = useRef(false);

// Initialize mutable row data when we first get data
useEffect(() => {
  if (currentData?.response?.results && !dataInitializedRef.current && !isEditingRef.current) {
    setMutableRowData(currentData.response.results.map((row) => ({ ...row })));
    dataInitializedRef.current = true;
  } else if (!currentData?.response?.results && !isEditingRef.current) {
    setMutableRowData([]);
    dataInitializedRef.current = false;
  }
}, [currentData]);
```

**Purpose**: AG Grid requires mutable row data for inline editing. The original data from API is immutable, so we create a mutable copy. The refs prevent conflicts between editing and data updates.

#### Inline Editing with Validation

```typescript
const processEditedRow = useCallback(
  (event: CellValueChangedEvent) => {
    const rowInQuestion: IRowNode = event.node;

    // Mark that we're editing to prevent data resets
    isEditingRef.current = true;

    // Validate hours ≤ 4000
    if (rowInQuestion.data.hoursExecutive > 4000) {
      const originalRow = currentData?.response.results.find(
        (obj) => obj.badgeNumber === rowInQuestion.data.badgeNumber
      );
      if (originalRow) {
        rowInQuestion.data.hoursExecutive = originalRow.hoursExecutive;
        event.api.refreshCells({ force: true });
        isEditingRef.current = false;
        return; // Reject change
      }
    }

    // Validate dollars ≤ 20,000,000
    if (rowInQuestion.data.incomeExecutive > 20000000) {
      const originalRow = currentData?.response.results.find(
        (obj) => obj.badgeNumber === rowInQuestion.data.badgeNumber
      );
      if (originalRow) {
        rowInQuestion.data.incomeExecutive = originalRow.incomeExecutive;
        event.api.refreshCells({ force: true });
        isEditingRef.current = false;
        return; // Reject change
      }
    }

    // Update the local mutable row data to persist the change
    setMutableRowData((prevData) =>
      prevData.map((row) =>
        row.badgeNumber === rowInQuestion.data.badgeNumber
          ? {
              ...row,
              hoursExecutive: rowInQuestion.data.hoursExecutive,
              incomeExecutive: rowInQuestion.data.incomeExecutive
            }
          : row
      )
    );

    // Update row if not in modal - this tracks changes for saving
    if (!isModal) {
      updateExecutiveRow(
        rowInQuestion.data.badgeNumber,
        rowInQuestion.data.hoursExecutive,
        rowInQuestion.data.incomeExecutive
      );
    }

    // Reset editing flag after a small delay
    setTimeout(() => {
      isEditingRef.current = false;
    }, 100);
  },
  [isModal, currentData, updateExecutiveRow]
);
```

**Validation Logic**:

- **Hours > 4000**: Revert to original value, refresh grid, reject change
- **Dollars > 20,000,000**: Revert to original value, refresh grid, reject change
- **Valid values**: Update mutable data, track for saving (main grid only)

#### Visual Change Indicators

```typescript
<DSMGrid
  providedOptions={{
    // ... other options
    getRowStyle: (params) => {
      // Rows with unsaved changes will have yellow color
      if (!isModal && isRowStagedToSave(params.node.data.badgeNumber)) {
        return { background: "lemonchiffon" };
      } else {
        return { background: "white" };
      }
    }
  }}
/>
```

Rows with pending changes are highlighted with a yellow (`lemonchiffon`) background to visually indicate unsaved work.

#### Add Executive Button (Main Grid)

```typescript
const RenderAddExecutiveButton: React.FC<Props> = ({
  reportReponse,
  isModal,
  onOpenModal,
  isReadOnly = false
}) => {
  const gridAvailable: boolean = reportReponse?.response != null;
  const isDisabled = !gridAvailable || isReadOnly;

  const addButton = (
    <Button
      disabled={isDisabled}
      variant="outlined"
      color="secondary"
      size="medium"
      startIcon={<AddOutlined />}
      onClick={isReadOnly ? undefined : () => onOpenModal()}>
      Add Executive
    </Button>
  );

  if (!isModal && isDisabled) {
    const tooltipTitle = isReadOnly
      ? "You are in read-only mode and cannot add executives."
      : "You can only add an executive to search results.";

    return <Tooltip title={tooltipTitle}><span>{addButton}</span></Tooltip>;
  } else if (!isModal) {
    return addButton;
  } else {
    return null; // Not rendered in modal
  }
};
```

**Disabled Conditions**:

1. No search results (`gridAvailable === false`)
2. Read-only mode (`isReadOnly === true`)

#### Modal Grid Configuration

```typescript
{isModal && (
  <div className="px-[24px]">
    <Typography variant="body1" sx={{ color: "#db1532" }}>
      Please select rows then click the add button up top
    </Typography>
  </div>
)}

<DSMGrid
  providedOptions={{
    rowSelection: isModal
      ? {
          mode: "multiRow",
          checkboxes: true,
          headerCheckbox: true,
          enableClickSelection: false
        }
      : undefined,
    onSelectionChanged: (event: SelectionChangedEvent) => {
      if (isModal) {
        const selectedRows = event.api.getSelectedRows();
        selectExecutivesInModal(selectedRows);
      }
    }
  }}
/>
```

Modal grid has:

- Checkboxes for row selection
- Header checkbox for select all
- Red instruction text
- No inline editing (columns not editable in mini mode)

**File Location**: `/src/ui/src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/ManageExecutiveHoursAndDollarsGrid.tsx`

---

### 3. `ManageExecutiveHoursAndDollarsSearchFilter.tsx`

**Purpose**: Search filter with mutual exclusion and dual-mode support

**Responsibilities**:

- Form validation using React Hook Form + Yup
- Search field mutual exclusion (SSN, Badge, Name)
- Dual mode (main vs modal) with different requirements
- Profit year display (disabled field)
- Checkbox filters (Has Executive Hours, Monthly Payroll)
- Reset functionality

**Key Features**:

#### Search Field Mutual Exclusion

Only one of SSN, Badge Number, or Full Name can be active at a time:

```typescript
const [activeField, setActiveField] = useState<"socialSecurity" | "badgeNumber" | "fullNameContains" | null>(null);

useEffect(() => {
  if (socialSecurity && !badgeNumber) {
    setActiveField("socialSecurity");
  } else if (badgeNumber && !socialSecurity) {
    setActiveField("badgeNumber");
  } else if (fullNameContains && fullNameContains !== "") {
    setActiveField("fullNameContains");
  } else if (!socialSecurity && !badgeNumber && !fullNameContains) {
    setActiveField(null);
  }
}, [socialSecurity, badgeNumber, fullNameContains]);
```

Fields are disabled based on `activeField`:

```typescript
<TextField
  disabled={activeField === "socialSecurity" || activeField === "badgeNumber"}
  // ... other props for fullNameContains field
/>

<TextField
  disabled={activeField === "badgeNumber" || activeField === "fullNameContains"}
  // ... other props for socialSecurity field
/>

<TextField
  disabled={activeField === "socialSecurity" || activeField === "fullNameContains"}
  // ... other props for badgeNumber field
/>
```

**Purpose**: Prevents ambiguous searches. User must search by one identifier at a time.

#### Dual Mode: Main vs Modal

The filter behaves differently depending on `isModal` prop:

| Mode      | Profit Year Field | Required Fields                     | Checkboxes |
| --------- | ----------------- | ----------------------------------- | ---------- |
| **Main**  | Visible, disabled | Optional (can search by checkboxes) | Visible    |
| **Modal** | Hidden            | One of SSN/Badge/Name required      | Hidden     |

```typescript
const requiredLabel = isModal && (
  <Typography component="span" color="error" fontWeight="bold">
    *
  </Typography>
);

// Profit Year (main only)
{!isModal && (
  <Grid size={{ xs: 12, sm: 6, md: 3 }}>
    <FormLabel>Profit Year</FormLabel>
    <Controller
      name="profitYear"
      control={control}
      render={({ field }) => (
        <TextField
          {...field}
          fullWidth
          type="number"
          disabled={true}  // Always disabled
        />
      )}
    />
  </Grid>
)}

// Checkboxes (main only)
{!isModal && (
  <>
    <FormLabel>Has Executive Hours and Dollars</FormLabel>
    <Controller name="hasExecutiveHoursAndDollars" control={control} render={...} />

    <FormLabel>Monthly Payroll</FormLabel>
    <Controller name="isMonthlyPayroll" control={control} render={...} />
  </>
)}
```

**Why Different?**

- **Main**: Users can search broadly (e.g., "all executives with hours", "all monthly payroll")
- **Modal**: Users search for specific individual to add (must provide identifier)

#### Validation Schema

```typescript
const validationSchema = yup
  .object({
    profitYear: profitYearValidator(),
    socialSecurity: ssnValidator,
    badgeNumber: badgeNumberStringValidator,
    fullNameContains: yup
      .string()
      .typeError("Full Name must be a string")
      .nullable()
      .transform((value) => value || undefined),
    hasExecutiveHoursAndDollars: yup.boolean().default(false).required(),
    isMonthlyPayroll: yup.boolean().default(false).required()
  })
  .test("at-least-one-required", "At least one field must be provided", (values) =>
    Boolean(
      values.profitYear ||
        values.socialSecurity ||
        values.badgeNumber ||
        values.fullNameContains ||
        values.hasExecutiveHoursAndDollars !== false ||
        values.isMonthlyPayroll !== false
    )
  );
```

**Custom Test**: Ensures at least one search criterion is provided.

#### Field State Tracking

```typescript
const [fieldStates, setFieldStates] = useState({
  socialSecurity: false,
  badgeNumber: false,
  fullNameContains: false,
  hasExecutiveHoursAndDollars: false,
  isMonthlyPayroll: false
});

const oneAddSearchFilterEntered =
  fieldStates.socialSecurity ||
  fieldStates.badgeNumber ||
  fieldStates.fullNameContains ||
  fieldStates.hasExecutiveHoursAndDollars ||
  fieldStates.isMonthlyPayroll;

// Search button disabled if no fields entered
<SearchAndReset
  disabled={!oneAddSearchFilterEntered}
  handleReset={handleReset}
  handleSearch={validateAndSearch}
  isFetching={isSearching}
/>
```

**Purpose**: Disable search button until user enters at least one criterion.

**File Location**: `/src/ui/src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/ManageExecutiveHoursAndDollarsSearchFilter.tsx`

---

### 4. `SearchAndAddExecutive.tsx`

**Purpose**: Modal content for searching and adding executives

**Responsibilities**:

- Modal page wrapper
- Add to Main Grid button rendering
- Filter and grid composition
- Tooltip management for disabled states

**Key Features**:

#### Add to Main Grid Button

```typescript
const RenderAddButton = ({ canAddExecutives, onAddToMainGrid, isReadOnly = true }: Props) => {
  const isDisabled = !canAddExecutives || isReadOnly;

  const addButton = (
    <Button
      disabled={isDisabled}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<AddOutlined />}
      onClick={isReadOnly ? undefined : onAddToMainGrid}>
      Add to Main Grid
    </Button>
  );

  if (isDisabled) {
    const tooltipTitle = isReadOnly
      ? "You are in read-only mode and cannot add executives."
      : "You must select only one row to add.";

    return <Tooltip title={tooltipTitle}><span>{addButton}</span></Tooltip>;
  } else {
    return addButton;
  }
};
```

**Disabled Conditions**:

1. No executives selected (`canAddExecutives === false` when `modalSelectedExecutives.length === 0`)
2. Read-only mode (`isReadOnly === true`)

#### Secure-by-Default Pattern

```typescript
// PS-1623: Secure-by-default, all add actions are read-only unless explicitly overridden.
// QA: Verify add button is disabled unless isReadOnly is false.

const SearchAndAddExecutive = ({
  // ... props
  isReadOnly = true // Secure-by-default
}: SearchAndAddExecutiveProps) => {
  // ...
};
```

**Security Note**: `isReadOnly` defaults to `true`. Callers must explicitly pass `false` to enable adding executives.

#### Modal Composition

```typescript
<Page
  label="Add New Executive"
  actionNode={<RenderAddButton />}>
  <Grid container rowSpacing="24px">
    <Divider />
    <DSMAccordion title="Filter">
      <ManageExecutiveHoursAndDollarsSearchFilter
        onSearch={executeModalSearch}
        onReset={handleReset}
        isSearching={isModalSearching}
        isModal={true}  // Modal mode
      />
    </DSMAccordion>
    <ManageExecutiveHoursAndDollarsGrid
      isModal={true}  // Modal mode
      modalResults={modalResults}
      isSearching={isModalSearching}
      selectExecutivesInModal={selectExecutivesInModal}
      modalGridPagination={modalGridPagination}
    />
  </Grid>
</Page>
```

The modal contains its own filter and grid, both configured for modal mode (`isModal={true}`).

**File Location**: `/src/ui/src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/SearchAndAddExecutive.tsx`

---

### 5. `ManageExecutiveHoursAndDollarsGridColumns.ts`

**Purpose**: Column definitions with dual mode support

**Columns**:

| Column            | Field                | Type     | Editable (Main) | Editable (Modal) | Description                               |
| ----------------- | -------------------- | -------- | --------------- | ---------------- | ----------------------------------------- |
| Badge Number      | `badgeNumber`        | Number   | No              | No               | Employee badge number                     |
| Full Name         | `fullName`           | String   | No              | No               | Employee full name                        |
| Store             | `store`              | Number   | No              | No               | Store number                              |
| SSN               | `ssn`                | String   | No              | No               | Social security number (masked, sortable) |
| Executive Hours   | `hoursExecutive`     | Hours    | **Yes**         | No               | Editable hours allocation                 |
| Executive Dollars | `incomeExecutive`    | Currency | **Yes**         | No               | Editable dollar allocation                |
| Oracle Hours      | `currentHoursYear`   | Hours    | No              | Hidden           | Hours from Oracle (read-only)             |
| Oracle Dollars    | `currentIncomeYear`  | Currency | No              | Hidden           | Dollars from Oracle (read-only)           |
| Pay Frequency     | `payFrequencyId`     | Status   | No              | Hidden           | Monthly/Weekly payroll status             |
| Employment Status | `employmentStatusId` | Status   | No              | Hidden           | Active/Terminated status                  |

**Dual Mode Configuration**:

```typescript
export const GetManageExecutiveHoursAndDollarsColumns = (mini?: boolean): ColDef[] => {
  const columns: ColDef[] = [
    createBadgeColumn({}),
    createNameColumn({ field: "fullName" }),
    createStoreColumn({}),
    createSSNColumn({ sortable: true }), // PS-1829: Backend sorts before masking
    createHoursColumn({
      headerName: "Executive Hours",
      field: "hoursExecutive",
      editable: !mini // Only editable in main grid
    }),
    {
      ...createCurrencyColumn({
        headerName: "Executive Dollars",
        field: "incomeExecutive"
      }),
      editable: !mini // Only editable in main grid
    },
    createHoursColumn({
      headerName: "Oracle Hours",
      field: "currentHoursYear"
    }),
    createCurrencyColumn({
      headerName: "Oracle Dollars",
      field: "currentIncomeYear"
    }),
    createStatusColumn({
      headerName: "Pay Frequency",
      field: "payFrequencyId",
      valueFormatter: (params) => params.data?.payFrequencyName
    }),
    createStatusColumn({
      headerName: "Employment Status",
      field: "employmentStatusId",
      valueFormatter: (params) => params.data?.employmentStatusName
    })
  ];

  // Filter columns for modal (mini mode)
  if (mini) {
    return columns.filter(
      (column) =>
        column.colId === "badgeNumber" ||
        column.colId === "fullName" ||
        column.colId === "ssn" ||
        column.colId === "hoursExecutive" ||
        column.colId === "incomeExecutive"
    );
  }
  return columns;
};
```

**Mini Mode (Modal)**:

- Shows only: Badge, Name, SSN, Executive Hours, Executive Dollars
- Hours and Dollars are read-only (not editable in modal)
- Purpose: Simplified view for selecting executives to add

**Main Mode**:

- Shows all columns
- Executive Hours and Executive Dollars are editable
- Oracle columns provide reference values

**File Location**: `/src/ui/src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/ManageExecutiveHoursAndDollarsGridColumns.ts`

---

## State Management

### useManageExecutiveHoursAndDollars Hook

**Purpose**: Centralized state management using reducer pattern with dual grid support

**State Structure** (via reducer):

```typescript
interface State {
  search: {
    params: ExecutiveHoursAndDollarsRequestDto | null;
    results: PagedReportResponse<ExecutiveHoursAndDollars> | null;
    isSearching: boolean;
    error: string | null;
    initialLoaded: boolean;
  };
  modal: {
    results: PagedReportResponse<ExecutiveHoursAndDollars> | null;
    searchParams: ExecutiveHoursAndDollarsRequestDto | null;
    isSearching: boolean;
    selectedExecutives: ExecutiveHoursAndDollars[];
    isOpen: boolean;
  };
  grid: {
    additionalExecutives: ExecutiveHoursAndDollars[]; // Manually added via modal
    pendingChanges: ExecutiveHoursAndDollarsGrid[]; // Unsaved edits
  };
  view: {
    pageNumberReset: boolean;
  };
}
```

**Actions**:

| Action Type                   | Payload                                          | Description                                  |
| ----------------------------- | ------------------------------------------------ | -------------------------------------------- |
| `SEARCH_START`                | `{ params: ExecutiveHoursAndDollarsRequestDto }` | Begin main grid search                       |
| `SEARCH_SUCCESS`              | `{ results: PagedReportResponse }`               | Main grid search succeeded                   |
| `SEARCH_FAILURE`              | `{ error: string }`                              | Main grid search failed                      |
| `SEARCH_RESET`                | None                                             | Clear search results and state               |
| `MODAL_OPEN`                  | None                                             | Open add executive modal                     |
| `MODAL_CLOSE`                 | None                                             | Close modal                                  |
| `MODAL_SEARCH_START`          | `{ params: ExecutiveHoursAndDollarsRequestDto }` | Begin modal grid search                      |
| `MODAL_SEARCH_SUCCESS`        | `{ results: PagedReportResponse }`               | Modal grid search succeeded                  |
| `MODAL_SEARCH_FAILURE`        | `{ error: string }`                              | Modal grid search failed                     |
| `MODAL_SELECT_EXECUTIVES`     | `{ executives: ExecutiveHoursAndDollars[] }`     | Track selected rows in modal                 |
| `MODAL_CLEAR_SELECTION`       | None                                             | Clear modal selection                        |
| `ADD_ADDITIONAL_EXECUTIVES`   | `{ executives: ExecutiveHoursAndDollars[] }`     | Add selected executives to main grid         |
| `CLEAR_ADDITIONAL_EXECUTIVES` | None                                             | Clear manually added executives              |
| `ADD_PENDING_CHANGE`          | `{ change: ExecutiveHoursAndDollarsGrid }`       | Track edited row                             |
| `UPDATE_PENDING_CHANGE`       | `{ change: ExecutiveHoursAndDollarsGrid }`       | Update existing pending change               |
| `REMOVE_PENDING_CHANGE`       | `{ change: ExecutiveHoursAndDollarsGrid }`       | Remove pending change (reverted to original) |
| `CLEAR_PENDING_CHANGES`       | None                                             | Clear all pending changes after save         |
| `SET_INITIAL_LOADED`          | `{ loaded: boolean }`                            | Mark initial search complete                 |
| `SET_PAGE_RESET`              | `{ reset: boolean }`                             | Toggle pagination reset                      |

**Key Reducer Selectors**:

```typescript
// Combine search results + manually added executives
export const selectCombinedGridData = (state: State): PagedReportResponse<ExecutiveHoursAndDollars> | null => {
  if (!state.search.results) return null;

  const combinedResults = [...(state.search.results.response?.results || []), ...state.grid.additionalExecutives];

  return {
    ...state.search.results,
    response: {
      ...state.search.results.response,
      results: combinedResults,
      total: combinedResults.length
    }
  };
};

// Check if any pending changes exist
export const selectHasPendingChanges = (state: State): boolean => {
  return state.grid.pendingChanges.length > 0;
};

// Check if specific row has pending changes
export const selectIsRowStagedToSave = (state: State) => {
  return (badgeNumber: number): boolean => {
    return state.grid.pendingChanges.some((change) =>
      change.executiveHoursAndDollars.some((exec) => exec.badgeNumber === badgeNumber)
    );
  };
};

// Show grid only after search
export const selectShowGrid = (state: State): boolean => {
  return state.search.initialLoaded;
};

// Show modal when opened
export const selectShowModal = (state: State): boolean => {
  return state.modal.isOpen;
};
```

**Hook Returns**:

```typescript
return {
  profitYear,
  searchParams: state.search.params,
  searchResults: state.search.results,
  isSearching: isSearching || state.search.isSearching,
  gridData: combinedGridData, // Search results + added executives
  hasPendingChanges, // True if unsaved edits exist
  showGrid, // True after initial search
  isRowStagedToSave, // Function to check if row has pending changes

  isModalOpen: showModal,
  modalResults: state.modal.results,
  modalSelectedExecutives: state.modal.selectedExecutives,
  isModalSearching: isModalSearching || state.modal.isSearching,

  mainGridPagination,
  modalGridPagination,

  executeSearch, // Main grid search
  executeModalSearch, // Modal grid search
  resetSearch,
  openModal,
  closeModal,
  selectExecutivesInModal,
  addExecutivesToMainGrid,
  updateExecutiveRow, // Track edited cell
  saveChanges, // Save all pending changes
  saveExecutiveHoursAndDollars, // Archive save (status complete)

  initialSearchLoaded: state.search.initialLoaded,
  setInitialSearchLoaded: (loaded) => dispatch({ type: "SET_INITIAL_LOADED", payload: { loaded } }),
  pageNumberReset: state.view.pageNumberReset,
  setPageNumberReset: (reset) => dispatch({ type: "SET_PAGE_RESET", payload: { reset } })
};
```

**File Location**: `/src/ui/src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/hooks/useManageExecutiveHoursAndDollars.ts`

---

## Data Flow

### Search Flow (Main Grid)

```
User enters search criteria
    ↓
ManageExecutiveHoursAndDollarsSearchFilter validates
    ↓
executeSearch() called
    ↓
Reducer: SEARCH_START (store params)
    ↓
triggerSearch (RTK Query lazy query)
    ↓
API request to /api/executive-hours-and-dollars
    ↓
Success:
  - Reducer: SEARCH_SUCCESS (store results)
  - Redux: setExecutiveHoursAndDollarsGridYear (store year)
  - Reducer: CLEAR_ADDITIONAL_EXECUTIVES (reset manual adds)
  - Grid displays results
    ↓
Error:
  - Reducer: SEARCH_FAILURE (store error)
  - Display error message
```

### Add Executive Flow

```
User clicks "Add Executive" button
    ↓
openModal() called
    ↓
Reducer: MODAL_OPEN, MODAL_CLEAR_SELECTION
    ↓
SmartModal opens with SearchAndAddExecutive
    ↓
User searches in modal (executeModalSearch)
    ↓
Reducer: MODAL_SEARCH_START → MODAL_SEARCH_SUCCESS
    ↓
Modal grid displays results with checkboxes
    ↓
User selects row(s) via checkboxes
    ↓
selectExecutivesInModal() called
    ↓
Reducer: MODAL_SELECT_EXECUTIVES (track selection)
    ↓
User clicks "Add to Main Grid" button
    ↓
addExecutivesToMainGrid() called
    ↓
Reducer: ADD_ADDITIONAL_EXECUTIVES (add to additionalExecutives array)
    ↓
Reducer: MODAL_CLOSE
    ↓
Main grid re-renders with combined data (search results + added executives)
    ↓
Added executives appear in main grid
    ↓
User can edit hours/dollars for added executives
```

### Inline Edit Flow

```
User clicks editable cell (Executive Hours or Dollars)
    ↓
Cell enters edit mode (AG Grid)
    ↓
User changes value and presses Enter or clicks away
    ↓
processEditedRow() called (CellValueChangedEvent)
    ↓
isEditingRef.current = true (prevent data conflicts)
    ↓
Validate hours ≤ 4000 and dollars ≤ 20,000,000
    ↓
Valid?
  Yes:
    - Update mutableRowData (local grid state)
    - updateExecutiveRow() called
    - Check if row already has pending change:
      - Yes: Update existing change or remove if reverted to original
      - No: Add new pending change
    - Reducer: ADD_PENDING_CHANGE / UPDATE_PENDING_CHANGE / REMOVE_PENDING_CHANGE
    - Redux: addExecutiveHoursAndDollarsGridRow / updateExecutiveHoursAndDollarsGridRow / removeExecutiveHoursAndDollarsGridRow
    - Row background changes to yellow if pending
    - Save button enables (hasPendingChanges = true)
  No:
    - Revert to original value
    - Refresh grid cells
    - No changes tracked
    ↓
setTimeout(() => isEditingRef.current = false, 100)
```

### Save Flow

```
User clicks "Save" button in action node
    ↓
handleSave() called
    ↓
saveChanges() called
    ↓
Check if pendingChanges exist
    ↓
Yes:
  - updateHoursAndDollars mutation (RTK Query)
  - API request with executiveHoursAndDollarsGrid from Redux
  - Success:
    - Reducer: CLEAR_PENDING_CHANGES
    - Redux: clearExecutiveHoursAndDollarsGridRows()
    - All yellow backgrounds removed
    - Save button disables (hasPendingChanges = false)
    - Success message via MissiveAlerts
  - Error:
    - Error message via MissiveAlerts
    - Pending changes remain
    - Yellow backgrounds remain
No:
  - No action (save button already disabled)
```

### Archive Save Flow (Status Complete)

```
User changes status to "Complete"
    ↓
StatusDropdownActionNode triggers status change
    ↓
saveExecutiveHoursAndDollars() called
    ↓
triggerSearch with archive: true flag
    ↓
API archives executive hours and dollars data
    ↓
Complete
```

---

## Grid Architecture

### Mutable Row Data Pattern

**Problem**: AG Grid requires mutable row data for inline editing, but React state is immutable.

**Solution**: Create a mutable copy of the data and manage it separately:

```typescript
// Original data (immutable)
const currentData = isModal ? modalResults : gridData;

// Mutable copy for grid editing
const [mutableRowData, setMutableRowData] = useState<ExecutiveHoursAndDollars[]>([]);

// Initialize on data load
useEffect(() => {
  if (currentData?.response?.results && !dataInitializedRef.current && !isEditingRef.current) {
    setMutableRowData(currentData.response.results.map((row) => ({ ...row })));
    dataInitializedRef.current = true;
  }
}, [currentData]);

// Pass mutable data to grid
<DSMGrid
  providedOptions={{
    rowData: mutableRowData,
    // ...
  }}
/>
```

**Refs for Conflict Prevention**:

- `isEditingRef`: Prevents data updates during active editing
- `dataInitializedRef`: Tracks whether data has been initialized
- `lastDataLengthRef`: Detects when new executives are added

### Combined Data Pattern

The main grid displays **search results + manually added executives**:

```typescript
// Reducer selector
export const selectCombinedGridData = (state: State): PagedReportResponse<ExecutiveHoursAndDollars> | null => {
  if (!state.search.results) return null;

  const combinedResults = [
    ...(state.search.results.response?.results || []), // Search results
    ...state.grid.additionalExecutives // Manually added
  ];

  return {
    ...state.search.results,
    response: {
      ...state.search.results.response,
      results: combinedResults,
      total: combinedResults.length
    }
  };
};
```

**Why?**

- Search results come from API
- Added executives don't exist in backend yet (not saved)
- User can edit both types in the same grid
- All changes are saved together via single save operation

### Pending Changes Tracking

**Data Structure**:

```typescript
interface ExecutiveHoursAndDollarsGrid {
  executiveHoursAndDollars: Array<{
    badgeNumber: number;
    executiveHours: number;
    executiveDollars: number;
  }>;
  profitYear: number | null;
}

// State
grid: {
  pendingChanges: ExecutiveHoursAndDollarsGrid[];  // Array of changes
}
```

**Tracking Logic**:

```typescript
const updateExecutiveRow = useCallback(
  (badgeNumber: number, hours: number, dollars: number) => {
    const rowRecord: ExecutiveHoursAndDollarsGrid = {
      executiveHoursAndDollars: [
        {
          badgeNumber,
          executiveHours: hours,
          executiveDollars: dollars
        }
      ],
      profitYear: profitYear || null
    };

    const isRowStagedToSave = selectIsRowStagedToSave(state);

    if (isRowStagedToSave(badgeNumber)) {
      // Row already has pending changes
      const combinedData = selectCombinedGridData(state);
      const originalRow = combinedData?.response.results.find((obj) => obj.badgeNumber === badgeNumber);

      if (originalRow && hours === originalRow.hoursExecutive && dollars === originalRow.incomeExecutive) {
        // Reverted to original - remove pending change
        dispatch({ type: "REMOVE_PENDING_CHANGE", payload: { change: rowRecord } });
        reduxDispatch(removeExecutiveHoursAndDollarsGridRow(rowRecord));
      } else {
        // Still different from original - update pending change
        dispatch({ type: "UPDATE_PENDING_CHANGE", payload: { change: rowRecord } });
        reduxDispatch(updateExecutiveHoursAndDollarsGridRow(rowRecord));
      }
    } else {
      // First change for this row - add pending change
      dispatch({ type: "ADD_PENDING_CHANGE", payload: { change: rowRecord } });
      reduxDispatch(addExecutiveHoursAndDollarsGridRow(rowRecord));
    }
  },
  [state, profitYear, reduxDispatch]
);
```

**Visual Indicator**:

```typescript
getRowStyle: (params) => {
  if (!isModal && isRowStagedToSave(params.node.data.badgeNumber)) {
    return { background: "lemonchiffon" }; // Yellow background
  } else {
    return { background: "white" };
  }
};
```

Rows with pending changes are highlighted yellow to indicate unsaved work.

---

## Key Features

### 1. Inline Editing with Validation

**Editable Columns**: Executive Hours, Executive Dollars

**Validation Rules**:

- Executive Hours ≤ 4000
- Executive Dollars ≤ 20,000,000

**Validation Enforcement**:

```typescript
if (rowInQuestion.data.hoursExecutive > 4000) {
  // Find original value
  const originalRow = currentData?.response.results.find(...);
  if (originalRow) {
    // Revert to original
    rowInQuestion.data.hoursExecutive = originalRow.hoursExecutive;
    event.api.refreshCells({ force: true });
    isEditingRef.current = false;
    return; // Reject change
  }
}
```

Changes that exceed limits are **silently rejected** by reverting to the original value.

**User Experience**:

- User types new value
- Presses Enter or clicks away
- If invalid: Grid reverts to previous value
- If valid: Cell shows new value, row turns yellow

### 2. Dual Grid Architecture

**Main Grid**:

- Displays search results + manually added executives
- Inline editing enabled
- Visual change indicators (yellow background)
- Pagination for search results
- Add Executive button

**Modal Grid**:

- Separate search and pagination
- Row selection with checkboxes
- Read-only (no inline editing)
- Instruction text for user guidance
- Add to Main Grid button

**Shared Component**: Both grids use `ManageExecutiveHoursAndDollarsGrid` with `isModal` prop to switch modes.

### 3. Pending Changes Tracking

**Tracking**:

- Each edit tracked in `grid.pendingChanges` array
- Redux also stores changes for save operation
- `hasPendingChanges` computed from array length

**Visual Indicators**:

- Rows with pending changes: Yellow background (`lemonchiffon`)
- Save button: Disabled when no pending changes
- Tooltip: "You must change hours or dollars to save."

**Revert Detection**:
If user changes value back to original, the pending change is **removed**:

```typescript
if (originalRow && hours === originalRow.hoursExecutive && dollars === originalRow.incomeExecutive) {
  dispatch({ type: "REMOVE_PENDING_CHANGE", payload: { change: rowRecord } });
  // Row background returns to white
}
```

### 4. Add Executive via Modal

**Flow**:

1. Click "Add Executive" button (main grid)
2. Modal opens with search filter and grid
3. User searches for executive
4. User selects row(s) via checkboxes
5. Click "Add to Main Grid" button
6. Modal closes
7. Selected executives appear in main grid
8. User can edit hours/dollars for added executives

**Why Modal?**

- Prevents cluttering main grid with all employees
- Allows targeted search for specific individual
- Clean separation between browsing and editing

### 5. Search Field Mutual Exclusion

**Rule**: Only one of SSN, Badge Number, or Full Name can be active at a time.

**Implementation**:

- Track `activeField` state
- Disable other fields based on `activeField`
- Clear `activeField` when all three are empty

**Purpose**: Prevents ambiguous searches. User must search by one identifier at a time.

**Checkboxes**: "Has Executive Hours and Dollars" and "Monthly Payroll" can be used independently or with identifier fields.

### 6. Read-Only Mode

**Multiple Guards**:

```typescript
const isReadOnly = useReadOnlyNavigation(); // Navigation-based
const isReadOnlyByStatus = useIsReadOnlyByStatus(); // Status-based
const isFrozen = useIsProfitYearFrozen(profitYear); // Year-based
```

**Effects when read-only**:

1. **Save button disabled**: Cannot save changes
2. **Add Executive button disabled**: Cannot add new executives
3. **Add to Main Grid disabled**: Cannot add from modal
4. **Editing disabled**: Columns not editable (enforced by column config)
5. **Warning displayed**: `FrozenYearWarning` or `StatusReadOnlyInfo` shown

**Tooltips**: All disabled buttons explain reason (read-only mode).

### 7. Dual Mode Search Filter

**Main Filter**:

- Shows Profit Year (disabled)
- Shows all search fields
- Shows checkboxes (Has Executive Hours, Monthly Payroll)
- Optional search criteria (can search by checkboxes alone)

**Modal Filter**:

- Hides Profit Year
- Shows search fields with required asterisk
- Hides checkboxes (not relevant for modal)
- Required: At least one of SSN/Badge/Name

**Why Different?**

- Main: Broad searches allowed
- Modal: Specific individual required

### 8. Oracle Reference Values

**Oracle Columns** (read-only):

- Oracle Hours (`currentHoursYear`)
- Oracle Dollars (`currentIncomeYear`)

**Purpose**: Provide reference values from Oracle HCM system. Users can see source values while editing executive values.

**Display**: Only in main grid (hidden in modal for simplicity).

---

## Redux Integration

### RTK Query API Endpoints

**Main Search Endpoint**:

```typescript
const [triggerSearch, { isLoading: isSearching }] = useLazyGetExecutiveHoursAndDollarsQuery();

// Execute search
await triggerSearch({
  profitYear: 2024,
  badgeNumber: 12345,
  hasExecutiveHoursAndDollars: false,
  isMonthlyPayroll: false,
  pagination: {
    skip: 0,
    take: 25,
    sortBy: "fullName",
    isSortDescending: false
  }
}).unwrap();

// Response structure
{
  response: {
    results: ExecutiveHoursAndDollars[],
    total: number
  }
}
```

**Modal Search Endpoint**:

```typescript
const [triggerModalSearch, { isLoading: isModalSearching }] = useLazyGetAdditionalExecutivesQuery();

// Same request/response structure as main search
```

**Update Endpoint**:

```typescript
const [updateHoursAndDollars] = useUpdateExecutiveHoursAndDollarsMutation();

// Execute update
await updateHoursAndDollars({
  executiveHoursAndDollars: [
    {
      badgeNumber: 12345,
      executiveHours: 2080,
      executiveDollars: 150000
    },
    {
      badgeNumber: 67890,
      executiveHours: 1500,
      executiveDollars: 120000
    }
  ],
  profitYear: 2024
}).unwrap();
```

### Redux Slices

**Executive Hours and Dollars Grid Slice** (`yearsEndSlice.ts`):

```typescript
// Actions
addExecutiveHoursAndDollarsGridRow(row) - Add pending change
updateExecutiveHoursAndDollarsGridRow(row) - Update pending change
removeExecutiveHoursAndDollarsGridRow(row) - Remove pending change
clearExecutiveHoursAndDollarsGridRows() - Clear all pending changes
setExecutiveHoursAndDollarsGridYear(year) - Set profit year

// State
executiveHoursAndDollarsGrid: {
  executiveHoursAndDollars: Array<{
    badgeNumber: number,
    executiveHours: number,
    executiveDollars: number
  }>,
  profitYear: number | null
}
```

**Why Redux?**

- Pending changes need to be accessible by save operation
- Multiple components need access to pending changes
- Survives component unmounts (if user navigates away and back)

---

## Hooks Reference

### 1. `useManageExecutiveHoursAndDollars`

**Purpose**: Main business logic hook for dual-grid management

**Returns**:

```typescript
{
  profitYear: number,
  searchParams: ExecutiveHoursAndDollarsRequestDto | null,
  searchResults: PagedReportResponse<ExecutiveHoursAndDollars> | null,
  isSearching: boolean,
  gridData: PagedReportResponse<ExecutiveHoursAndDollars> | null,  // Combined data
  hasPendingChanges: boolean,
  showGrid: boolean,
  isRowStagedToSave: (badgeNumber: number) => boolean,

  isModalOpen: boolean,
  modalResults: PagedReportResponse<ExecutiveHoursAndDollars> | null,
  modalSelectedExecutives: ExecutiveHoursAndDollars[],
  isModalSearching: boolean,

  mainGridPagination: GridPaginationState & GridPaginationActions,
  modalGridPagination: GridPaginationState & GridPaginationActions,

  executeSearch: (searchForm: ExecutiveSearchForm) => Promise<void>,
  executeModalSearch: (searchForm: ExecutiveSearchForm) => Promise<void>,
  resetSearch: () => void,
  openModal: () => void,
  closeModal: () => void,
  selectExecutivesInModal: (executives: ExecutiveHoursAndDollars[]) => void,
  addExecutivesToMainGrid: () => void,
  updateExecutiveRow: (badge: number, hours: number, dollars: number) => void,
  saveChanges: () => Promise<void>,
  saveExecutiveHoursAndDollars: () => Promise<void>,

  initialSearchLoaded: boolean,
  setInitialSearchLoaded: (loaded: boolean) => void,
  pageNumberReset: boolean,
  setPageNumberReset: (reset: boolean) => void
}
```

**File Location**: `/src/ui/src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/hooks/useManageExecutiveHoursAndDollars.ts`

### 2. `useFiscalCloseProfitYear`

**Purpose**: Get active profit year from context

**Returns**: `number` (e.g., 2024)

### 3. `useReadOnlyNavigation`

**Purpose**: Check if current navigation context requires read-only mode

**Returns**: `boolean`

### 4. `useIsReadOnlyByStatus`

**Purpose**: Check if current page status requires read-only mode

**Returns**: `boolean`

### 5. `useIsProfitYearFrozen`

**Purpose**: Check if profit year is frozen

**Parameters**: `profitYear: number`

**Returns**: `boolean`

### 6. `useMissiveAlerts`

**Purpose**: Access user message system

**Returns**:

```typescript
{
  missiveAlerts: MissiveAlert[],
  addAlert: (alert: MissiveAlert) => void,
  clearAlerts: () => void
}
```

### 7. `useGridPagination`

**Purpose**: Manage grid pagination state and actions

**Parameters**:

```typescript
{
  initialPageSize: number,
  initialSortBy: string,
  initialSortDescending: boolean,
  onPaginationChange: (pageNumber, pageSize, sortParams) => void
}
```

**Returns**:

```typescript
{
  pageNumber: number,
  pageSize: number,
  sortParams: ISortParams,
  handlePaginationChange: (page, size, sort) => void,
  handleSortChange: (event: SortChangedEvent) => void,
  resetPagination: () => void
}
```

---

## Type Definitions

### Request Types

```typescript
interface ExecutiveHoursAndDollarsRequestDto {
  profitYear: number;
  badgeNumber?: number;
  socialSecurity?: number;
  fullNameContains?: string;
  hasExecutiveHoursAndDollars: boolean;
  isMonthlyPayroll: boolean;
  pagination: {
    skip: number;
    take: number;
    sortBy: string;
    isSortDescending: boolean;
  };
  archive?: boolean; // For status complete archiving
}

interface ExecutiveHoursAndDollarsGrid {
  executiveHoursAndDollars: Array<{
    badgeNumber: number;
    executiveHours: number;
    executiveDollars: number;
  }>;
  profitYear: number | null;
}
```

### Response Types

```typescript
interface ExecutiveHoursAndDollars {
  badgeNumber: number;
  fullName: string;
  store: number;
  ssn: string;
  hoursExecutive: number; // Editable
  incomeExecutive: number; // Editable
  currentHoursYear: number; // Oracle reference
  currentIncomeYear: number; // Oracle reference
  payFrequencyId: number;
  payFrequencyName: string;
  employmentStatusId: number;
  employmentStatusName: string;
}

interface PagedReportResponse<T> {
  response: {
    results: T[];
    total: number;
  };
}
```

---

## Common Patterns

### 1. Error Handling

**Pattern**: All errors displayed via `MissiveAlerts` context

```typescript
const { addAlert } = useMissiveAlerts();

try {
  await saveChanges();
  addAlert(EXECUTIVE_HOURS_AND_DOLLARS_MESSAGES.EXECUTIVE_HOURS_SAVED_SUCCESS);
} catch (_error) {
  addAlert(EXECUTIVE_HOURS_AND_DOLLARS_MESSAGES.EXECUTIVE_HOURS_SAVE_ERROR);
}
```

**Message Definitions**:

```typescript
export const EXECUTIVE_HOURS_AND_DOLLARS_MESSAGES = {
  EXECUTIVE_HOURS_SAVED_SUCCESS: {
    id: 1,
    severity: "success",
    message: "Executive Hours and Dollars Saved",
    description: "Your changes have been successfully saved."
  },
  EXECUTIVE_HOURS_SAVE_ERROR: {
    id: 2,
    severity: "error",
    message: "Save Failed",
    description: "An error occurred while saving executive hours and dollars."
  }
};
```

### 2. Loading States

**Pattern**: Multiple loading indicators for different operations

```typescript
// Main search loading
const { isLoading: isSearching } = useLazyGetExecutiveHoursAndDollarsQuery();

// Modal search loading
const { isLoading: isModalSearching } = useLazyGetAdditionalExecutivesQuery();

// Save loading
const { isLoading: isSaving } = useUpdateExecutiveHoursAndDollarsMutation();
```

**Display**:

- Search: Disable search button, show grid loading spinner
- Modal search: Disable modal search button, show modal grid loading spinner
- Save: Handled by mutation loading state

### 3. Memoization

**Pattern**: Memoize expensive computations

```typescript
const combinedGridData = useMemo(() => selectCombinedGridData(state), [state]);
const hasPendingChanges = useMemo(() => selectHasPendingChanges(state), [state]);
const showGrid = useMemo(() => selectShowGrid(state), [state]);
const isRowStagedToSave = useMemo(() => selectIsRowStagedToSave(state), [state]);

const columnDefs = useMemo(() => GetManageExecutiveHoursAndDollarsColumns(isModal), [isModal]);
```

**Benefits**:

- Prevent unnecessary re-renders
- Improve performance with complex state selectors
- Stable references for child components

### 4. Conditional Rendering

**Pattern**: Progressive disclosure based on state

```typescript
// Show grid only after search
{showGrid && (
  <ManageExecutiveHoursAndDollarsGrid gridData={gridData} />
)}

// Show frozen year warning if applicable
{isFrozen && <FrozenYearWarning profitYear={profitYear} />}

// Show read-only info if status-based read-only
{isReadOnlyByStatus && <StatusReadOnlyInfo />}
```

### 5. Tooltip Wrapping for Disabled Buttons

**Pattern**: Wrap disabled buttons in tooltips to explain why

```typescript
if (isDisabled) {
  const tooltipTitle = isReadOnly
    ? "You are in read-only mode and cannot save changes."
    : "You must change hours or dollars to save.";

  return (
    <Tooltip placement="top" title={tooltipTitle}>
      <span>{saveButton}</span>
    </Tooltip>
  );
} else {
  return saveButton;
}
```

**Why `<span>` wrapper?** MUI Tooltip requires a DOM element child when button is disabled.

---

## Validation Rules

### Search Filter Validation

**At Least One Field Required**:

```typescript
.test("at-least-one-required", "At least one field must be provided", (values) =>
  Boolean(
    values.profitYear ||
      values.socialSecurity ||
      values.badgeNumber ||
      values.fullNameContains ||
      values.hasExecutiveHoursAndDollars !== false ||
      values.isMonthlyPayroll !== false
  )
);
```

**SSN Validator**:

```typescript
ssnValidator: yup
  .string()
  .nullable()
  .matches(/^\d{3}-\d{2}-\d{4}$/, "SSN must be in format XXX-XX-XXXX");
```

**Badge Number Validator**:

```typescript
badgeNumberStringValidator: yup.string().nullable().matches(/^\d+$/, "Badge number must be numeric");
```

**Full Name Validator**:

```typescript
fullNameContains: yup
  .string()
  .typeError("Full Name must be a string")
  .nullable()
  .transform((value) => value || undefined);
```

### Inline Edit Validation

**Hours Validation**:

```typescript
if (rowInQuestion.data.hoursExecutive > 4000) {
  // Revert to original
  rowInQuestion.data.hoursExecutive = originalRow.hoursExecutive;
  event.api.refreshCells({ force: true });
  return; // Reject change
}
```

**Dollars Validation**:

```typescript
if (rowInQuestion.data.incomeExecutive > 20000000) {
  // Revert to original
  rowInQuestion.data.incomeExecutive = originalRow.incomeExecutive;
  event.api.refreshCells({ force: true });
  return; // Reject change
}
```

**Silent Rejection**: Invalid edits are silently rejected by reverting to previous value. No error message is shown to user.

---
