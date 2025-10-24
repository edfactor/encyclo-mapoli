---
applyTo: "src/ui/src/pages/DecemberActivities/Unforfeit/**/*.*"
---
# UnForfeit (Rehire Forfeitures) Page - Technical Documentation

## Overview

The UnForfeit page (labeled "Rehire Forfeitures") is a December Activities workflow page for reversing forfeitures when employees are rehired. It uses the same master-detail grid pattern as the Termination page but with critical differences in data handling, validation rules, and business logic.

### Key Features

- **Rehire-Specific Search**: Date range filter for rehire dates with validation
- **Zero Balance Exclusion**: Optional filter to hide employees with no balances
- **Master-Detail Grid**: Expandable rows showing year-by-year forfeiture history
- **Inline Editing**: "Suggested Unforfeiture" field with negative value handling
- **Individual & Bulk Save**: Save single rows or multiple selected rows
- **Frozen Year Warning**: Alert when working with frozen profit year
- **Read-Only Mode**: Permission-based access control
- **Archive Support**: Handle archived forfeiture data

---

## Architecture Overview

### Component Hierarchy

```
UnForfeit (Page Wrapper)
├── FrozenYearWarning (Conditional alert)
├── StatusDropdownActionNode (Navigation status)
├── ApiMessageAlert (Save messages)
├── UnForfeitSearchFilter (Rehire date range + exclusion checkbox)
│   └── React Hook Form (yup validation)
└── UnForfeitGrid (Master-detail AG Grid)
    ├── useUnForfeitGrid (Custom hook - main logic)
    ├── ReportSummary (Record count)
    ├── DSMGrid (AG Grid wrapper)
    │   ├── Expansion Column (▼/► icons)
    │   ├── Main Columns (Badge, Name, SSN, Dates, Balances)
    │   └── Detail Columns (Year-specific forfeiture data + save controls)
    └── Pagination
```

### File Structure

```
DecemberActivities/UnForfeit/
├── UnForfeit.tsx                           # Main page component
├── UnForfeitSearchFilter.tsx               # Rehire date range filter
├── UnForfeitGrid.tsx                       # Master-detail grid display
├── UnForfeitGridColumns.ts                 # Main row column definitions
├── UnForfeitProfitDetailGridColumns.tsx    # Detail row columns (forfeiture data + controls)
└── UnForfeitHeaderComponent.tsx            # Bulk save header
```

### Custom Hooks Used

```
hooks/
├── useUnForfeitState.ts            # State management (search params, unsaved changes, archive)
├── useUnForfeitGrid.ts             # Grid logic (expansion, editing, save operations)
├── useFiscalCalendarYear.ts        # Fiscal date range fetching
├── useDecemberFlowProfitYear.ts    # Current profit year
├── useIsProfitYearFrozen.ts        # Frozen year checking
├── useUnsavedChangesGuard.ts       # Navigation blocking
├── useReadOnlyNavigation.ts        # Permission checking
└── useDynamicGridHeight.ts         # Responsive grid sizing
```

---

## Key Differences from Termination

### 1. Data Source & Business Logic

**Termination**:
- Employees who have **terminated employment**
- "Suggested Forfeit": Amount to forfeit (positive values)
- Creates forfeitures

**UnForfeit**:
- Employees who have been **rehired** after termination
- "Suggested Unforfeiture": Amount to reverse/restore (positive input, **negated** on save)
- Reverses existing forfeitures

### 2. Search Parameters

**Termination**:
```typescript
{
  beginningDate: "01/01/2025",
  endingDate: "12/31/2025",
  forfeitureStatus: "showAll"
}
```

**UnForfeit**:
```typescript
{
  beginningDate: "01/01/2025",    // Rehire begin date
  endingDate: "12/31/2025",       // Rehire ending date
  excludeZeroBalance: true        // Optional exclusion filter
}
```

### 3. Value Negation

**Critical Difference**: UnForfeit **negates** the value on save:

```typescript
// Termination: Save as-is
forfeitureAmount: currentValue

// UnForfeit: Negate the value
forfeitureAmount: -(currentValue || 0)
```

**Why**: Forfeiture adjustments in the backend are:
- **Positive** = forfeit (reduce balance)
- **Negative** = unforfeit (restore balance)

UI shows positive "suggested unforfeiture" amounts for clarity, but negates before saving.

### 4. Editable Field Name

**Termination**: `suggestedForfeit`
**UnForfeit**: `suggestedUnforfeiture`

### 5. Profit Detail ID Tracking

**UnForfeit** includes `offsettingProfitDetailId` for tracking which forfeiture is being reversed:

```typescript
{
  badgeNumber: 123456,
  profitYear: 2025,
  forfeitureAmount: -1500,
  offsettingProfitDetailId: 789,  // <-- References original forfeiture
  classAction: false
}
```

### 6. Validation Rules

**UnForfeit** has stricter date validation:

```typescript
endingDate: endDateStringAfterStartDateValidator(/* ... */)
  .required("Ending Date is required")
  .test("is-too-early", "Insufficient data for dates before 2024", function (value) {
    return new Date(value) > new Date(2024, 1, 1);  // <-- Extra validation
  })
```

**Reason**: Historical data limitations for rehire records.

---

## State Management

### useUnForfeitState Hook

Similar to `useTerminationState` but tailored for unforfeit workflow:

```typescript
const { state, actions } = useUnForfeitState();

// State shape
interface UnForfeitState {
  initialSearchLoaded: boolean;
  resetPageFlag: boolean;
  hasUnsavedChanges: boolean;
  shouldArchive: boolean;
}

// Actions
actions.handleSearch();              // Execute search (no params - uses Redux state)
actions.setInitialSearchLoaded(true);
actions.handleUnsavedChanges(true);
actions.handleStatusChange();
actions.handleArchiveHandled();
```

**Key Difference**: `handleSearch` takes no parameters (relies on Redux for search params).

### Auto-Trigger Search on Archive

```typescript
// In UnForfeit.tsx
useEffect(() => {
  if (state.shouldArchive) {
    actions.handleSearch();  // Auto-search when archive mode activated
  }
}, [state.shouldArchive, actions]);
```

**User Flow**: Change navigation status → Archive mode activates → Search runs automatically.

---

## Search Filter

### Filter Component

**File**: `UnForfeitSearchFilter.tsx`

```typescript
const schema = yup.object().shape({
  beginningDate: dateStringValidator(2000, 2099, "Beginning Date")
    .required("Beginning Date is required"),

  endingDate: endDateStringAfterStartDateValidator(
    "beginningDate",
    tryddmmyyyyToDate,
    "Ending date must be the same or after the beginning date"
  )
    .required("Ending Date is required")
    .test("is-too-early", "Insufficient data for dates before 2024", function (value) {
      return new Date(value) > new Date(2024, 1, 1);
    }),

  pagination: yup.object({ skip, take, sortBy, isSortDescending }).required(),
  profitYear: profitYearValidator()
});

const UnForfeitSearchFilter = ({
  fiscalData,
  onSearch,
  hasUnsavedChanges,
  setHasUnsavedChanges
}) => {
  const { control, handleSubmit, formState: { errors, isValid } } = useForm({
    resolver: yupResolver(schema),
    defaultValues: {
      beginningDate: fiscalData.fiscalBeginDate,
      endingDate: fiscalData.fiscalEndDate,
      excludeZeroBalance: false,
      pagination: { skip: 0, take: 25, sortBy: "fullName", isSortDescending: false }
    }
  });

  const validateAndSubmit = (data) => {
    if (hasUnsavedChanges) {
      alert("Please save your changes.");
      return;
    }

    dispatch(setUnForfeitsQueryParams(data));
    triggerSearch(data);
    if (onSearch) onSearch();
  };

  return (
    <form onSubmit={handleSubmit(validateAndSubmit)}>
      <DsmDatePicker
        name="beginningDate"
        control={control}
        label="Rehire Begin Date"
        minDate={fiscalData.fiscalBeginDate}
        maxDate={fiscalData.fiscalEndDate}
      />

      <DsmDatePicker
        name="endingDate"
        control={control}
        label="Rehire Ending Date"
        minDate={beginningDateValue || fiscalData.fiscalBeginDate}
        maxDate={fiscalData.fiscalEndDate}
      />

      <Controller
        name="excludeZeroBalance"
        control={control}
        render={({ field }) => (
          <FormControlLabel
            control={<Checkbox checked={field.value || false} onChange={(e) => field.onChange(e.target.checked)} />}
            label="Exclude employees with no current balance and no vested balance"
          />
        )}
      />

      <SearchAndReset
        handleSearch={handleSubmit(validateAndSubmit)}
        handleReset={handleReset}
        disabled={!isValid || isFetching}
      />
    </form>
  );
};
```

**Key Features**:

1. **Rehire Date Labels**: "Rehire Begin Date" vs "Begin Date"
2. **Zero Balance Exclusion**: Checkbox to filter out employees with no balance
3. **Stricter Validation**: Must be after 2024-02-01
4. **Dynamic End Date Min**: End date minimum is the greater of fiscal begin or selected begin date

```typescript
const minDateFromBeginning = beginningDateValue ? tryddmmyyyyToDate(beginningDateValue) : null;
const fiscalMinDate = tryddmmyyyyToDate(fiscalData.fiscalBeginDate);
const effectiveMinDate =
  minDateFromBeginning && fiscalMinDate
    ? minDateFromBeginning > fiscalMinDate
      ? minDateFromBeginning
      : fiscalMinDate
    : (minDateFromBeginning ?? fiscalMinDate ?? undefined);
```

---

## Grid Columns

### Main Columns

**File**: `UnForfeitGridColumns.ts`

```typescript
export const UnForfeitGridColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createNameColumn({ field: "fullName" }),
    createSSNColumn({}),

    createDateColumn({
      headerName: "Hire Date",
      field: "hireDate"
    }),

    createDateColumn({
      headerName: "Rehired Date",
      field: "reHiredDate"              // <-- Key difference
    }),

    createCurrencyColumn({
      headerName: "Current Balance",
      field: "netBalanceLastYear"
    }),

    createCurrencyColumn({
      headerName: "Vested Balance",
      field: "vestedBalanceLastYear"
    }),

    createStoreColumn({}),

    createCountColumn({
      headerName: "Years",
      field: "companyContributionYears"
    }),

    {
      headerName: "Enrollment",
      field: "enrollmentId",
      valueGetter: (params) => {
        const id = params.data?.enrollmentId;
        const name = params.data?.enrollmentName;
        if (!id || !name) return "-";
        return `[${id}] ${name}`;
      }
    }
  ];
};
```

**Key Fields**:
- `reHiredDate`: Date employee was rehired (critical for unforfeit eligibility)
- `netBalanceLastYear`: Current balance (for exclusion filter)
- `vestedBalanceLastYear`: Vested balance (for exclusion filter)

### Detail Columns

**File**: `UnForfeitProfitDetailGridColumns.tsx`

```typescript
export const GetProfitDetailColumns = (
  addRowToSelectedRows,
  removeRowFromSelectedRows,
  selectedProfitYear,
  onSave,
  onBulkSave,
  isReadOnly = false
): ColDef[] => {
  return [
    createYearColumn({ field: "profitYear" }),

    createHoursColumn({
      field: "hoursTransactionYear",
      valueGetter: (params) => {
        // Hide zeros (no data for many years)
        const value = params.data?.hoursTransactionYear;
        return value == null || value == 0 ? null : value;
      }
    }),

    createCurrencyColumn({
      headerName: "Wages",
      field: "wagesTransactionYear",
      valueGetter: (params) => {
        // Hide zeros (no data for many years)
        const value = params.data?.wagesTransactionYear;
        return value == null || value == 0 ? null : value;
      }
    }),

    createCurrencyColumn({
      headerName: "Forfeiture",
      field: "forfeiture"
    }),

    // EDITABLE FIELD
    {
      headerName: "Suggested Unforfeiture",
      field: "suggestedUnforfeit",
      pinned: "right",
      editable: (params) =>
        params.data.suggestedUnforfeiture != null && !isReadOnly,
      cellEditor: SuggestedForfeitEditor,
      cellRenderer: (params) =>
        SuggestedForfeitCellRenderer({ ...params, selectedProfitYear }, false, true),
      valueFormatter: (params) => numberToCurrency(params.data.suggestedUnforfeiture)
    },

    createCommentColumn({
      headerName: "Remark",
      field: "remark"
    }),

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

**Key Differences**:
- **Hours/Wages**: Use `valueGetter` to hide zeros (many years have no data)
- **Editable Condition**: `suggestedUnforfeiture != null` (simpler than Termination's year check)
- **Field Name**: `suggestedUnforfeiture` vs `suggestedForfeit`

---

## Save Operations

### Individual Save

**Save Button Cell Renderer**:

```typescript
cellRenderer: (params: UnForfeitsSaveButtonCellParams) => {
  if (!isTransactionEditable(params, isReadOnly)) {
    return "";
  }

  const rowKey = params.data.profitDetailId;  // <-- Uses profitDetailId, not badge-year
  const currentValue = params.context?.editedValues?.[rowKey]?.value
    ?? params.data.suggestedUnforfeiture;
  const isLoading = params.context?.loadingRowIds?.has(params.data.badgeNumber);
  const isDisabled = (currentValue || 0) === 0 || isLoading || isReadOnly;

  return (
    <div>
      <Checkbox /* ... */ />
      <IconButton
        onClick={async () => {
          if (!isReadOnly && params.onSave) {
            const request: ForfeitureAdjustmentUpdateRequest = {
              badgeNumber: params.data.badgeNumber,
              forfeitureAmount: -(currentValue || 0),         // <-- NEGATED
              profitYear: selectedProfitYear,
              offsettingProfitDetailId: params.data.profitDetailId,  // <-- Track source
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

**Critical Differences**:

1. **Row Key**: Uses `profitDetailId` instead of `${badgeNumber}-${profitYear}`
   - **Why**: Forfeiture records have unique IDs, multiple forfeitures can exist per year

2. **Value Negation**: `forfeitureAmount: -(currentValue || 0)`
   - **Why**: Unforfeit = negative forfeiture adjustment

3. **Offsetting Profit Detail ID**: Tracks which forfeiture is being reversed
   - **Why**: Audit trail and data integrity

### Bulk Save Header

**File**: `UnForfeitHeaderComponent.tsx`

```typescript
export const HeaderComponent: React.FC<UnForfeitHeaderComponentProps> = (params) => {
  const profitYear = useDecemberFlowProfitYear();

  const isNodeEligible = (nodeData, context) => {
    if (!nodeData.isDetail) return false;

    // Use profitDetailId as key (not badge-year)
    const baseRowKey = nodeData.profitDetailId;
    const editedValues = context?.editedValues || {};
    const matchingKey = baseRowKey in editedValues ? baseRowKey : undefined;
    const currentValue = matchingKey
      ? editedValues[matchingKey]?.value
      : nodeData.suggestedUnforfeiture;

    return (currentValue || 0) !== 0;
  };

  const createUpdatePayload = (nodeData, context) => {
    const baseRowKey = nodeData.profitDetailId;
    const editedValues = context?.editedValues || {};
    const matchingKey = baseRowKey in editedValues ? baseRowKey : undefined;
    const currentValue = matchingKey
      ? editedValues[matchingKey]?.value
      : nodeData.suggestedUnforfeiture;

    return {
      badgeNumber: Number(nodeData.badgeNumber),
      profitYear: profitYear,
      forfeitureAmount: -(currentValue || 0),    // <-- NEGATED
      classAction: false
    };
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

**Key Points**:
- Uses `profitDetailId` for row tracking
- Negates value in `createUpdatePayload`
- Does NOT use `offsettingProfitDetailId` in bulk operations (backend handles it)

---

## Frozen Year Warning

### Conditional Display

```typescript
// In UnForfeit.tsx
const profitYear = useDecemberFlowProfitYear();
const isFrozen = useIsProfitYearFrozen(profitYear);

return (
  <Page label="Rehire Forfeitures" actionNode={<StatusDropdownActionNode />}>
    {isFrozen && <FrozenYearWarning profitYear={profitYear} />}
    {/* ... rest of page */}
  </Page>
);
```

**FrozenYearWarning Component**:

```typescript
<Alert severity="warning" sx={{ margin: "16px" }}>
  <AlertTitle>Frozen Year Warning</AlertTitle>
  Profit year {profitYear} is frozen. Changes may be restricted or require approval.
</Alert>
```

**User Experience**: Warning appears at top of page when working with frozen year. Does NOT block operations (read-only mode handles that).

---

## API Integration

### Endpoints

**1. Search UnForfeits**:

```typescript
useLazyGetUnForfeitsQuery()

// Request
{
  beginningDate: "01/01/2025",
  endingDate: "12/31/2025",
  excludeZeroBalance: true,
  profitYear: 2025,
  pagination: { skip: 0, take: 25, sortBy: "fullName", isSortDescending: false }
}

// Response
{
  response: {
    results: [
      {
        badgeNumber: 123456,
        fullName: "Doe, John",
        ssn: "***-**-1234",
        hireDate: "2020-01-15",
        reHiredDate: "2025-03-01",      // <-- Rehire date
        netBalanceLastYear: 5000,
        vestedBalanceLastYear: 4000,
        storeNumber: 42,
        companyContributionYears: 3,
        enrollmentId: 1,
        enrollmentName: "Enrolled",
        profitDetails: [
          {
            profitDetailId: 789,        // <-- Unique ID for this forfeiture
            profitYear: 2024,
            hoursTransactionYear: 1200,
            wagesTransactionYear: 35000,
            forfeiture: 1500,
            suggestedUnforfeiture: 1500,  // <-- Amount to reverse
            remark: "Rehired after termination"
          }
        ]
      }
    ],
    total: 45
  }
}
```

**2. Save Forfeiture Adjustment** (Same as Termination):

```typescript
useSaveForfeitureAdjustmentMutation()

// Request
{
  badgeNumber: 123456,
  profitYear: 2025,
  forfeitureAmount: -1500,            // <-- NEGATIVE (unforfeit)
  offsettingProfitDetailId: 789,      // <-- References source forfeiture
  classAction: false
}

// Response
{
  success: true,
  message: "Forfeiture adjustment saved"
}
```

---

## Key Patterns

### 1. Profit Detail ID as Row Key

**UnForfeit**:
```typescript
const rowKey = params.data.profitDetailId;  // Unique ID
```

**Termination**:
```typescript
const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;  // Composite key
```

**Why Different**:
- UnForfeit: Multiple forfeiture records can exist per badge/year (due to multiple terminations/rehires)
- Termination: One record per badge/year (current status)

### 2. Value Negation for UnForfeit

Always negate before saving:

```typescript
forfeitureAmount: -(currentValue || 0)
```

**UI Display**: Positive numbers (easier to understand)
**Backend**: Negative numbers (standard forfeiture adjustment semantics)

### 3. Zero Value Hiding in Hours/Wages

```typescript
valueGetter: (params) => {
  const value = params.data?.hoursTransactionYear;
  return value == null || value == 0 ? null : value;
}
```

**Why**: Historical data often has zeros (missing data). Hiding zeros reduces visual clutter.

### 4. Simplified Editability Check

```typescript
function isTransactionEditable(params, isReadOnly: boolean = false): boolean {
  return params.data.suggestedUnforfeiture != null && !isReadOnly;
}
```

**Why**: Unlike Termination (which restricts to current profit year), UnForfeit allows editing any row where `suggestedUnforfeiture` is present.

---

## Comparison Table: Termination vs UnForfeit

| Feature | Termination | UnForfeit |
|---------|-------------|-----------|
| **Purpose** | Process forfeitures for terminated employees | Reverse forfeitures for rehired employees |
| **Search Criteria** | Termination date range + forfeiture status | Rehire date range + zero balance exclusion |
| **Editable Field** | `suggestedForfeit` | `suggestedUnforfeiture` |
| **Row Key** | `${badgeNumber}-${profitYear}` | `profitDetailId` |
| **Value on Save** | As-is (positive) | Negated (negative) |
| **Offsetting ID** | Not used | `offsettingProfitDetailId` |
| **Editability** | Current profit year only | Any row with suggested value |
| **Date Validation** | >= fiscal begin | >= fiscal begin AND >= 2024-02-01 |
| **Zero Value Display** | Show zeros | Hide zeros (hours/wages) |
| **Warning Display** | None | FrozenYearWarning if year frozen |
| **Main Columns** | Badge, Name | Badge, Name, SSN, Hire Date, **Rehire Date**, Balances |

---
