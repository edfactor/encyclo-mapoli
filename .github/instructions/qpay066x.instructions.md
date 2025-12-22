---
applyTo: "src/ui/src/pages/Reports/QPAY066xAdHocReports/**/*.*"
paths: "src/ui/src/pages/Reports/QPAY066xAdHocReports/**/*.*"
---

# QPAY066x Ad Hoc Reports - Technical Summary

## Overview

The QPAY066x Ad Hoc Reports feature provides a flexible reporting interface for profit sharing breakdown reports across multiple employee categories (terminated, inactive, retired, etc.). This feature is located in `src/ui/src/pages/Reports/QPAY066xAdHocReports/` and follows the standard page component architecture pattern with search/filter functionality, dynamic grid display, and conditional totals.

**Key Capabilities:**

- Six predefined report presets covering different employee scenarios
- Dynamic search filters with optional date ranges based on selected report
- Server-side pagination and sorting
- Conditional summary totals (only when store number specified)
- Export-ready grid data via AG Grid
- Mutually exclusive Badge/Name search fields

---

## File Structure

```
QPAY066xAdHocReports/
├── QPAY066xAdHocReports.tsx              # Main page component
├── QPAY066xAdHocReportsGrid.tsx          # Grid display component
├── QPAY066xAdHocGridColumns.ts           # Column definitions
├── QPAY066xAdHocSearchFilter.tsx         # Filter form component
├── availableQPAY066xReports.ts           # Report preset configurations
└── hooks/
    └── useQPAY066xAdHocReports.ts        # Custom hook for data fetching
```

---

## Component Breakdown

### 1. QPAY066xAdHocReports.tsx

**Purpose**: Main page orchestrator component that manages overall page state and coordinates between filter and grid components.

**Location**: `src/ui/src/pages/Reports/QPAY066xAdHocReports/QPAY066xAdHocReports.tsx`

**Key Responsibilities:**

- Manages search state (storeNumber, badgeNumber, employeeName, storeManagement, dates)
- Handles preset selection and form resets
- Integrates pagination via `useGridPagination` hook
- Clears Redux state before new searches to prevent UI flicker
- Coordinates search execution with custom hook

**State Management:**

```typescript
const [currentPreset, setCurrentPreset] = useState<QPAY066xAdHocReportPreset | null>(null);
const [storeNumber, setStoreNumber] = useState<string>("");
const [badgeNumber, setBadgeNumber] = useState<string>("");
const [employeeName, setEmployeeName] = useState<string>("");
const [storeManagement, setStoreManagement] = useState<boolean>(false);
const [startDate, setStartDate] = useState<string>("");
const [endDate, setEndDate] = useState<string>("");
const [hasSearched, setHasSearched] = useState(false);
```

**Pagination Integration:**

- Uses `useGridPagination` hook with initial settings:
  - `initialPageSize`: 25
  - `initialSortBy`: "badgeNumber"
  - `initialSortDescending`: false
- Pagination changes trigger new API calls with updated skip/take/sort parameters
- Resets pagination to first page on new searches

**Key Implementation Detail - Redux State Clearing:**

```typescript
const handleSearch = () => {
  if (!currentPreset) return;

  // Clear previous grid data before new search (prevents old data flickering)
  dispatch(clearBreakdownByStore());
  dispatch(clearBreakdownByStoreManagement());
  dispatch(clearBreakdownByStoreTotals());

  gridPagination.resetPagination();
  executeSearch({...});
};
```

**Lines Referenced:**

- Line 20: Redux dispatch hook initialization
- Lines 30-53: Grid pagination hook with onPaginationChange callback
- Lines 101-104: Redux clear actions to prevent stale data display
- Line 167: Passes `gridPagination` prop to grid component

---

### 2. QPAY066xAdHocReportsGrid.tsx

**Purpose**: Displays search results in AG Grid with conditional summary totals and pagination controls.

**Location**: `src/ui/src/pages/Reports/QPAY066xAdHocReports/QPAY066xAdHocReportsGrid.tsx`

**Key Responsibilities:**

- Renders grid using `DSMGrid` component wrapper
- Displays conditional summary table (only when storeNumber provided)
- Manages loading states for both grid and summary
- Integrates pagination component
- Dynamically determines data source based on `storeManagement` flag

**Props Interface:**

```typescript
interface QPAY066xAdHocReportsGridProps {
  reportTitle: string;
  isLoading: boolean;
  storeNumber: string;
  gridPagination: ReturnType<typeof useGridPagination>;
}
```

**Data Source Logic:**
The component intelligently selects data from Redux based on the `storeManagement` flag:

```typescript
const breakdownByStoreManagement = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreManagement);
const breakdownByStore = useSelector((state: RootState) => state.yearsEnd.breakdownByStore);
const breakdownByStoreTotals = useSelector((state: RootState) => state.yearsEnd.breakdownByStoreTotals);

const rowData = useMemo(() => {
  // Check both locations - data location depends on storeManagement flag
  if (breakdownByStoreManagement?.response?.results) {
    return breakdownByStoreManagement.response.results;
  }
  if (breakdownByStore?.response?.results) {
    return breakdownByStore.response.results;
  }
  return [];
}, [breakdownByStoreManagement, breakdownByStore]);
```

**Conditional Totals Display:**

```typescript
const showTotals = storeNumber && storeNumber.trim() !== "";

{showTotals && (
  <TableContainer sx={{ mb: 4.5 }}>
    <Table size="small">
      <TableBody>
        <TableRow>
          <TableCell>Amount In Profit Sharing</TableCell>
          <TableCell>Vested Amount</TableCell>
          <TableCell>Total Forfeitures</TableCell>
          <TableCell>Total Loans</TableCell>
          <TableCell>Total Beneficiary Allocations</TableCell>
        </TableRow>
        {/* Summary data rows */}
      </TableBody>
    </Table>
  </TableContainer>
)}
```

**Summary Data Calculation:**

```typescript
const summaryData = useMemo(() => {
  if (breakdownByStoreTotals) {
    return {
      amountInProfitSharing: numberToCurrency(breakdownByStoreTotals.totalBeginningBalances),
      vestedAmount: numberToCurrency(breakdownByStoreTotals.totalVestedBalance),
      totalForfeitures: numberToCurrency(breakdownByStoreTotals.totalForfeitures),
      totalLoans: numberToCurrency(0), // Not provided in API response
      totalBeneficiaryAllocations: numberToCurrency(0) // Not provided in API response
    };
  }
  return {
    /* zero defaults */
  };
}, [breakdownByStoreTotals]);
```

**Total Records Calculation:**
Dynamically selects total count from appropriate Redux slice:

```typescript
const totalRecords = useMemo(() => {
  if (breakdownByStoreManagement?.response?.total) {
    return breakdownByStoreManagement.response.total;
  }
  if (breakdownByStore?.response?.total) {
    return breakdownByStore.response.total;
  }
  return 0;
}, [breakdownByStoreManagement, breakdownByStore]);
```

**Pagination Component:**

```typescript
{rowData && rowData.length > 0 && (
  <Grid size={{ xs: 12 }}>
    <Pagination
      pageNumber={pageNumber}
      setPageNumber={(value: number) => {
        handlePaginationChange(value - 1, pageSize);
      }}
      pageSize={pageSize}
      setPageSize={(value: number) => {
        handlePaginationChange(0, value);
      }}
      recordCount={totalRecords}
    />
  </Grid>
)}
```

**Lines Referenced:**

- Lines 33-54: Total records calculation from Redux
- Lines 56-74: Summary data calculation
- Lines 80-89: Row data selection logic
- Line 43: Conditional totals check
- Lines 175-189: Pagination component integration

---

### 3. QPAY066xAdHocGridColumns.ts

**Purpose**: Defines grid column configuration and TypeScript interface for breakdown report employees.

**Location**: `src/ui/src/pages/Reports/QPAY066xAdHocReports/QPAY066xAdHocGridColumns.ts`

**Key Responsibilities:**

- Provides TypeScript interface for row data (`BreakdownByStoreEmployee`)
- Defines column configuration using grid column factory functions
- Custom computed columns (Age at Termination)

**Employee Data Interface:**

```typescript
export interface BreakdownByStoreEmployee {
  badgeNumber: number;
  beginningBalance: number;
  beneficiaryAllocation: number; // Added in recent enhancement
  certificateSort: number;
  city: string;
  contributions: number;
  dateOfBirth: string;
  departmentId: number;
  distributions: number;
  earnings: number;
  employmentStatusId: string;
  endingBalance: number;
  enrollmentId: number;
  forfeitures: number;
  fullName: string;
  hireDate: string;
  isExecutive: boolean;
  payClassificationId: string;
  payClassificationName: string;
  payFrequencyId: number;
  postalCode: string;
  profitShareHours: number;
  ssn: string;
  state: string;
  storeNumber: number;
  street1: string;
  terminationDate: string;
  vestedAmount: number;
  vestedPercentage: number;
}
```

**Column Configuration:**
Uses factory functions from `utils/gridColumnFactory` for consistency:

```typescript
export const GetQPAY066xAdHocGridColumns = (): ColDef[] => [
  createBadgeColumn({}),
  createNameColumn({ field: "fullName" }),
  createCurrencyColumn({
    headerName: "Beginning Balance",
    field: "beginningBalance",
    minWidth: 130
  }),
  createCurrencyColumn({
    headerName: "Beneficiary Allocation",
    field: "beneficiaryAllocation"
  }),
  createCurrencyColumn({
    headerName: "Distribution Amount",
    field: "distributions"
  }),
  createCurrencyColumn({
    headerName: "Forfeit",
    field: "forfeitures"
  }),
  createCurrencyColumn({
    headerName: "Ending Balance",
    field: "endingBalance"
  }),
  createCurrencyColumn({
    headerName: "Vesting Balance",
    field: "vestedAmount"
  }),
  createDateColumn({
    headerName: "Term Date",
    field: "terminationDate",
    minWidth: 100
  }),
  createHoursColumn({
    headerName: "YTD Hours",
    field: "profitShareHours",
    minWidth: 90
  }),
  createPercentageColumn({
    headerName: "Vested",
    field: "vestedPercentage"
  }),
  createAgeColumn({ field: "dateOfBirth" }),
  createAgeColumn({
    headerName: "Age at Term",
    valueGetter: (params) => {
      const dob = params.data?.["dateOfBirth"];
      const termDate = params.data?.["terminationDate"];
      if (!dob || !termDate) return 0;
      const birthDate = new Date(dob);
      const terminationDate = new Date(termDate);
      let age = terminationDate.getFullYear() - birthDate.getFullYear();
      const m = terminationDate.getMonth() - birthDate.getMonth();
      if (m < 0 || (m === 0 && terminationDate.getDate() < birthDate.getDate())) {
        age--;
      }
      return age;
    }
  })
];
```

**Key Columns:**

- Badge Number (with navigation support)
- Full Name
- Beginning Balance, Beneficiary Allocation, Distributions, Forfeitures, Ending Balance, Vested Amount (all currency-formatted)
- Termination Date (date-formatted)
- YTD Hours (hours-formatted)
- Vested Percentage (percentage-formatted)
- Age (computed from date of birth)
- Age at Termination (custom calculation from DOB and termination date)

**Lines Referenced:**

- Lines 12-42: TypeScript interface definition
- Lines 44-109: Column definitions using factory functions
- Lines 93-108: Custom "Age at Term" calculated column

---

### 4. QPAY066xAdHocSearchFilter.tsx

**Purpose**: Form component for configuring search parameters with dynamic validation based on selected report preset.

**Location**: `src/ui/src/pages/Reports/QPAY066xAdHocReports/QPAY066xAdHocSearchFilter.tsx`

**Key Responsibilities:**

- Preset selection dropdown
- Dynamic form validation based on preset requirements
- Mutually exclusive Badge/Name field enforcement
- Optional date range inputs (conditional based on preset)
- Store number input with validation
- Store management checkbox
- Form submission and reset handling

**Props Interface:**

```typescript
interface QPAY066xAdHocSearchFilterProps {
  presets: QPAY066xAdHocReportPreset[];
  currentPreset: QPAY066xAdHocReportPreset | null;
  onPresetChange: (preset: QPAY066xAdHocReportPreset | null) => void;
  onReset: () => void;
  onStoreNumberChange: (storeNumber: string) => void;
  onBadgeNumberChange: (badgeNumber: string) => void;
  onEmployeeNameChange: (employeeName: string) => void;
  onStoreManagementChange: (storeManagement: boolean) => void;
  onStartDateChange: (startDate: string) => void;
  onEndDateChange: (endDate: string) => void;
  onSearch: () => void;
  isLoading?: boolean;
}
```

**Form Data Structure:**

```typescript
interface QPAY066xAdHocSearchFilterFormData {
  storeNumber: number | null;
  startDate: string;
  endDate: string;
  badgeNumber: string;
  employeeName: string;
  storeManagement: boolean;
}
```

**Dynamic Validation Schema:**
The validation schema adapts based on whether the selected preset requires a date range:

```typescript
const createSchema = (requiresDateRange: boolean) =>
  yup.object().shape({
    storeNumber: yup
      .number()
      .nullable()
      .default(null)
      .test("is-positive", "Store Number must be a positive number", function (value) {
        if (value === null || value === undefined) return true;
        return value > 0;
      }),
    startDate: requiresDateRange
      ? dateStringValidator(2000, 2099, "Start Date").required("Start Date is required")
      : yup.string().default(""),
    endDate: requiresDateRange
      ? endDateStringAfterStartDateValidator(
          "startDate",
          tryddmmyyyyToDate,
          "End Date must be equal to or greater than Start Date"
        ).required("End Date is required")
      : yup.string().default(""),
    badgeNumber: yup
      .string()
      .default("")
      .test("is-valid-badge", "Badge Number must be between 1 and 11 digits", function (value) {
        if (!value || value === "") return true;
        const numValue = Number(value);
        return !isNaN(numValue) && numValue >= 1 && numValue <= 99999999999;
      }),
    employeeName: yup.string().default(""),
    storeManagement: yup.boolean().default(false)
  });

const requiresDateRange = currentPreset?.requiresDateRange || false;

const {
  control,
  handleSubmit,
  reset,
  watch,
  trigger,
  formState: { errors, isValid }
} = useForm<QPAY066xAdHocSearchFilterFormData>({
  resolver: yupResolver(createSchema(requiresDateRange)),
  defaultValues: {
    storeNumber: null,
    startDate: "",
    endDate: "",
    badgeNumber: "",
    employeeName: "",
    storeManagement: false
  }
});
```

**Mutually Exclusive Badge/Name Fields:**
The component enforces mutual exclusivity between Badge Number and Name fields using disabled states:

```typescript
const badgeNumberValue = useWatch({ control, name: "badgeNumber" });
const nameValue = useWatch({ control, name: "employeeName" });

const hasValue = (value: string | number | null | undefined): boolean => {
  if (value === null || value === undefined || value === "") return false;
  if (typeof value === "string" && value.trim() === "") return false;
  return true;
};

const hasBadgeNumber = hasValue(badgeNumberValue);
const hasName = hasValue(nameValue);

const isBadgeNumberDisabled = hasName;
const isNameDisabled = hasBadgeNumber;

const getExclusionHelperText = (fieldName: string, isDisabled: boolean): string => {
  if (!isDisabled) return "";
  if (fieldName === "badgeNumber") {
    return "Disabled: Name field is in use. Press Reset to clear and re-enable.";
  }
  if (fieldName === "employeeName") {
    return "Disabled: Badge field is in use. Press Reset to clear and re-enable.";
  }
  return "";
};
```

**Conditional Date Range Fields:**
Date fields only render when the selected preset requires them:

```typescript
{requiresDateRange && (
  <>
    <Grid size={{ xs: 12, sm: 3 }}>
      <Controller
        name="startDate"
        control={control}
        render={({ field }) => (
          <DSMDatePicker
            id="startDate"
            onChange={(value: Date | null) => {
              const formatted = value ? mmDDYYFormat(value) : "";
              field.onChange(formatted);
              onStartDateChange(formatted);
              trigger("startDate");
              if (value) trigger("endDate");
            }}
            value={field.value ? tryddmmyyyyToDate(field.value) : null}
            required={true}
            label="Start Date"
            disableFuture
            error={errors.startDate?.message}
          />
        )}
      />
    </Grid>
    <Grid size={{ xs: 12, sm: 3 }}>
      <Controller
        name="endDate"
        control={control}
        render={({ field }) => {
          const minDateFromStart = startDateValue ? tryddmmyyyyToDate(startDateValue) : null;
          return (
            <DSMDatePicker
              id="endDate"
              onChange={(value: Date | null) => {
                const formatted = value ? mmDDYYFormat(value) : "";
                field.onChange(formatted);
                onEndDateChange(formatted);
                trigger("endDate");
              }}
              value={field.value ? tryddmmyyyyToDate(field.value) : null}
              required={true}
              label="End Date"
              disableFuture
              error={errors.endDate?.message}
              minDate={minDateFromStart ?? undefined}
            />
          );
        }}
      />
    </Grid>
  </>
)}
```

**Search/Reset Buttons:**

```typescript
<SearchAndReset
  handleReset={handleResetForm}
  handleSearch={handleSubmit(handleFormSubmit)}
  isFetching={isLoading}
  disabled={!currentPreset || isLoading || !isValid}
  searchButtonText="Search"
/>
```

**Lines Referenced:**

- Lines 65-95: Dynamic validation schema creation
- Lines 111-130: Form hook configuration with dynamic schema
- Lines 135-162: Mutually exclusive field logic
- Lines 318-387: Conditional date range rendering

---

### 5. availableQPAY066xReports.ts

**Purpose**: Configuration file defining all available QPAY066x report presets.

**Location**: `src/ui/src/pages/Reports/QPAY066xAdHocReports/availableQPAY066xReports.ts`

**Key Responsibilities:**

- Centralized report preset configuration
- Defines report IDs, names, descriptions, parameters
- Specifies date range requirements per report
- Maps report IDs to API endpoints

**Preset Type Definition:**

```typescript
interface QPAY066xAdHocReportPreset {
  id: string; // Unique report identifier
  name: string; // Display name
  description: string; // Human-readable description
  params: {
    reportId: number; // Numeric ID for backend
  };
  requiresDateRange: boolean; // Whether dates are required
  apiEndpoint: string; // API endpoint path
}
```

**Available Reports:**

```typescript
const reports: QPAY066xAdHocReportPreset[] = [
  {
    id: "QPAY066C",
    name: "QPAY066C",
    description: "Terminated managers and associates for all stores with a balance but not vested",
    params: { reportId: 1 },
    requiresDateRange: true,
    apiEndpoint: "/api/yearend/breakdown-by-store/terminated/withcurrentbalance/notvested"
  },
  {
    id: "QPAY066-Inactive",
    name: "QPAY066-Inactive",
    description: "Inactive Employees",
    params: { reportId: 2 },
    requiresDateRange: false,
    apiEndpoint: "/api/yearend/breakdown-by-store/inactive"
  },
  {
    id: "QPAY066-I",
    name: "QPAY066-I",
    description: "Inactive with Vested Balance",
    params: { reportId: 3 },
    requiresDateRange: false,
    apiEndpoint: "/api/yearend/breakdown-by-store/inactive/withvestedbalance"
  },
  {
    id: "QPAY066B",
    name: "QPAY066B",
    description: "Terminated with Beneficiary Allocation",
    params: { reportId: 4 },
    requiresDateRange: true, // Note: Date range is optional in implementation
    apiEndpoint: "/api/yearend/breakdown-by-store/terminated/withbeneficiaryallocation"
  },
  {
    id: "QPAY066W",
    name: "QPAY066W",
    description: "Retired with Balance Activity",
    params: { reportId: 5 },
    requiresDateRange: true, // Note: Date range is optional in implementation
    apiEndpoint: "/api/yearend/breakdown-by-store/retired/withbalanceactivity"
  },
  {
    id: "QPAY066TA",
    name: "QPAY066TA",
    description: "Managers and associates for all stores",
    params: { reportId: 6 },
    requiresDateRange: false,
    apiEndpoint: "/api/yearend/breakdown-by-store"
  }
];
```

**Key Notes:**

- **QPAY066C**: Requires date range for terminated employees with balance but not vested
- **QPAY066-Inactive** and **QPAY066-I**: No date range required
- **QPAY066B** and **QPAY066W**: Marked as requiring date range, but implementation allows optional dates (backend accepts `DateOnly?` parameters)
- **QPAY066TA**: General breakdown report, no date constraints

**Lines Referenced:**

- Lines 1-66: Complete preset configuration array

---

### 6. useQPAY066xAdHocReports.ts

**Purpose**: Custom React hook encapsulating data fetching logic and report orchestration.

**Location**: `src/ui/src/pages/Reports/QPAY066xAdHocReports/hooks/useQPAY066xAdHocReports.ts`

**Key Responsibilities:**

- Manages lazy RTK Query hooks for all report endpoints
- Routes search requests to appropriate API based on report ID
- Aggregates loading states from multiple queries
- Conditionally fetches totals based on storeNumber presence
- Provides report title lookup function

**Search Parameters Interface:**

```typescript
export interface QPAY066xSearchParams {
  reportId: string;
  storeNumber?: number;
  badgeNumber?: number;
  employeeName?: string;
  storeManagement: boolean;
  startDate?: string;
  endDate?: string;
  pagination?: SortedPaginationRequestDto;
}
```

**RTK Query Hooks:**

```typescript
const [fetchQPAY066TA, { isFetching: isFetchingTA }] = useLazyGetBreakdownByStoreQuery();
const [fetchQPAY066Inactive, { isFetching: isFetchingInactive }] = useLazyGetBreakdownByStoreInactiveQuery();
const [fetchQPAY066I, { isFetching: isFetchingI }] = useLazyGetBreakdownByStoreInactiveWithVestedBalanceQuery();
const [fetchQPAY066C, { isFetching: isFetchingC }] = useLazyGetBreakdownByStoreTerminatedBalanceNotVestedQuery();
const [fetchQPAY066B, { isFetching: isFetchingB }] = useLazyGetBreakdownByStoreTerminatedWithBenAllocationsQuery();
const [fetchQPAY066W, { isFetching: isFetchingW }] = useLazyGetBreakdownByStoreRetiredWithBalanceActivityQuery();
const [fetchTotals, { isFetching: isFetchingTotals }] = useLazyGetBreakdownByStoreTotalsQuery();
```

**Aggregated Loading State:**

```typescript
const isLoading = useMemo(
  () =>
    isFetchingTA || isFetchingInactive || isFetchingI || isFetchingC || isFetchingB || isFetchingW || isFetchingTotals,
  [isFetchingTA, isFetchingInactive, isFetchingI, isFetchingC, isFetchingB, isFetchingW, isFetchingTotals]
);
```

**Search Execution Logic:**

```typescript
const executeSearch = useCallback(
  async (searchParams: QPAY066xSearchParams) => {
    if (!profitYear) {
      console.error("Profit year is not available");
      return;
    }

    const {
      reportId,
      storeNumber,
      badgeNumber,
      employeeName,
      storeManagement,
      startDate,
      endDate,
      pagination = {
        skip: 0,
        take: 255,
        sortBy: "badgeNumber",
        isSortDescending: false
      }
    } = searchParams;

    setCurrentReportId(reportId);

    const baseParams = {
      profitYear,
      storeNumber,
      storeManagement,
      badgeNumber,
      employeeName,
      pagination
    };

    try {
      // Route to the correct API based on report ID
      switch (reportId) {
        case "QPAY066TA":
          await fetchQPAY066TA(baseParams);
          break;
        case "QPAY066-Inactive":
          await fetchQPAY066Inactive(baseParams);
          break;
        case "QPAY066-I":
          await fetchQPAY066I(baseParams);
          break;
        case "QPAY066C":
          // QPAY066C requires startDate and endDate
          if (startDate && endDate) {
            await fetchQPAY066C({ ...baseParams, startDate, endDate });
          }
          break;
        case "QPAY066B":
          // QPAY066B has optional date range
          await fetchQPAY066B({ ...baseParams, startDate, endDate });
          break;
        case "QPAY066W":
          // QPAY066W has optional date range
          await fetchQPAY066W({ ...baseParams, startDate, endDate });
          break;
        default:
          console.error(`Unknown report ID: ${reportId}`);
          return;
      }

      // Fetch totals only if storeNumber is provided
      if (storeNumber) {
        await fetchTotals(baseParams);
      }
    } catch (error) {
      console.error("Error executing search:", error);
    }
  },
  [
    profitYear,
    fetchQPAY066TA,
    fetchQPAY066Inactive,
    fetchQPAY066I,
    fetchQPAY066C,
    fetchQPAY066B,
    fetchQPAY066W,
    fetchTotals
  ]
);
```

**Report Title Lookup:**

```typescript
const getReportTitle = useCallback((reportId: string | null): string => {
  if (!reportId) return "N/A";
  const matchingPreset = reports.find((preset) => preset.id === reportId);
  return matchingPreset ? matchingPreset.description.toUpperCase() : "N/A";
}, []);
```

**Return Value:**

```typescript
return {
  executeSearch, // Function to trigger search
  isLoading, // Aggregated loading state
  currentReportId, // Currently selected report ID
  getReportTitle // Function to get report title by ID
};
```

**Key Implementation Notes:**

1. **React Hook Rules Compliance**: All RTK Query hooks are called unconditionally at the top of the component (lines 43-49), following React hook rules.

2. **Conditional API Calls**: Despite all hooks being called unconditionally, actual API calls are made conditionally based on reportId in the switch statement (lines 101-128).

3. **Optional Date Ranges**: QPAY066B and QPAY066W pass dates to API even when undefined, relying on backend to handle optional `DateOnly?` parameters (lines 117-124).

4. **Conditional Totals Fetch**: Totals API is only called when storeNumber is provided (lines 131-133), optimizing network requests.

5. **Default Pagination**: If pagination is not provided, defaults to skip=0, take=255, sortBy="badgeNumber" (lines 79-84).

**Lines Referenced:**

- Lines 16-25: Search params interface
- Lines 43-49: Lazy query hook declarations
- Lines 52-62: Aggregated loading state
- Lines 64-148: Main executeSearch callback
- Lines 131-133: Conditional totals fetch
- Lines 150-154: Report title lookup function

---

## Data Flow Architecture

### 1. Search Workflow

```
User selects preset
    ↓
QPAY066xAdHocSearchFilter validates form
    ↓
Form data passed to QPAY066xAdHocReports via onSearch callback
    ↓
QPAY066xAdHocReports calls executeSearch from useQPAY066xAdHocReports hook
    ↓
useQPAY066xAdHocReports routes to appropriate RTK Query endpoint based on reportId
    ↓
RTK Query caches response in Redux state:
  - breakdownByStore (for storeManagement=false)
  - breakdownByStoreManagement (for storeManagement=true)
  - breakdownByStoreTotals (if storeNumber provided)
    ↓
QPAY066xAdHocReportsGrid reads from Redux and displays results
```

### 2. Redux State Structure

```typescript
state.yearsEnd = {
  breakdownByStore: {
    response: {
      results: BreakdownByStoreEmployee[],
      total: number
    }
  },
  breakdownByStoreManagement: {
    response: {
      results: BreakdownByStoreEmployee[],
      total: number
    }
  },
  breakdownByStoreTotals: {
    totalBeginningBalances: number,
    totalVestedBalance: number,
    totalForfeitures: number
  }
}
```

**Key Point**: Data is stored in different Redux slices based on the `storeManagement` flag. The grid component checks both locations and displays whichever has data.

### 3. Pagination Flow

```
User changes page or page size
    ↓
Pagination component calls handlePaginationChange
    ↓
gridPagination.handlePaginationChange updates internal state
    ↓
onPaginationChange callback in QPAY066xAdHocReports.tsx fires
    ↓
New executeSearch call with updated skip/take/sort parameters
    ↓
API returns new page of results
    ↓
Redux state updates with new data
    ↓
Grid re-renders with new page
```

---

## Key Technical Patterns

### 1. Conditional Rendering Based on Data Presence

The grid component uses conditional rendering to display totals only when a store number is provided:

```typescript
const showTotals = storeNumber && storeNumber.trim() !== "";

{showTotals && (
  <TableContainer>
    {/* Summary table */}
  </TableContainer>
)}
```

### 2. Dynamic Schema Validation

The search filter dynamically adjusts validation requirements based on selected preset:

```typescript
const createSchema = (requiresDateRange: boolean) => yup.object().shape({
  startDate: requiresDateRange
    ? dateStringValidator(...).required("Start Date is required")
    : yup.string().default(""),
  endDate: requiresDateRange
    ? endDateStringAfterStartDateValidator(...).required("End Date is required")
    : yup.string().default("")
});
```

### 3. Mutually Exclusive Fields

Badge and Name fields are mutually exclusive, enforced via disabled states:

```typescript
const badgeNumberValue = useWatch({ control, name: "badgeNumber" });
const nameValue = useWatch({ control, name: "employeeName" });

const isBadgeNumberDisabled = hasValue(nameValue);
const isNameDisabled = hasValue(badgeNumberValue);

<TextField
  disabled={isBadgeNumberDisabled}
  helperText={getExclusionHelperText("badgeNumber", isBadgeNumberDisabled)}
/>
```

### 4. Redux State Clearing

To prevent old data from flickering during new searches, Redux state is explicitly cleared:

```typescript
dispatch(clearBreakdownByStore());
dispatch(clearBreakdownByStoreManagement());
dispatch(clearBreakdownByStoreTotals());
```

### 5. Conditional API Calls

Totals API is only called when store number is provided, optimizing network usage:

```typescript
if (storeNumber) {
  await fetchTotals(baseParams);
}
```

### 6. Smart Redux Data Selection

The grid intelligently selects data from the appropriate Redux slice based on the storeManagement flag:

```typescript
const rowData = useMemo(() => {
  if (breakdownByStoreManagement?.response?.results) {
    return breakdownByStoreManagement.response.results;
  }
  if (breakdownByStore?.response?.results) {
    return breakdownByStore.response.results;
  }
  return [];
}, [breakdownByStoreManagement, breakdownByStore]);
```

---

## Recent Enhancements

### 1. Beneficiary Allocation Field Addition

**Issue**: QPAY066B report (Terminated with Beneficiary Allocation) was not displaying beneficiaryAllocation values correctly.

**Solution**:

- Added `beneficiaryAllocation` field to `BreakdownByStoreEmployee` interface
- Added currency-formatted column to grid
- Fixed backend filter calculation to match SQL formula:
  ```csharp
  .Where(x => x.Sum(r => r.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary.Id
      ? -r.Forfeiture
      : (r.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id ? r.Contribution : 0)) > 0)
  ```

### 2. Conditional Totals Display

**Issue**: Totals section displayed even when not needed (when no store number provided).

**Solution**:

- Added `storeNumber` prop to grid component
- Implemented conditional rendering: `const showTotals = storeNumber && storeNumber.trim() !== "";`
- Modified custom hook to only fetch totals when storeNumber is provided

### 3. Optional Date Range Support

**Issue**: QPAY066B and QPAY066W reports showed blank grids when dates were not provided, even though dates are optional on backend.

**Solution**:

- Removed conditional checks preventing API calls without dates
- Updated API calls to pass `startDate` and `endDate` as undefined when not provided
- Backend `DateOnly?` parameters handle undefined correctly

### 4. Date Format Conversion

**Issue**: Date range searches returned no results due to format mismatch (UI: MM/dd/yyyy, API: yyyy-MM-dd).

**Solution**:

- Created `convertToISODateString` utility function in `dateUtils.ts`
- Applied conversion to three date-based endpoints in `YearsEndApi.ts`:
  - getBreakdownByStoreTerminatedBalanceNotVested
  - getBreakdownByStoreTerminatedWithBenAllocations
  - getBreakdownByStoreRetiredWithBalanceActivity

### 5. Pagination Implementation

**Issue**: Grid lacked pagination controls, making large result sets difficult to navigate.

**Solution**:

- Integrated `useGridPagination` hook with onPaginationChange callback
- Added `Pagination` component to grid
- Calculated `totalRecords` from Redux state for accurate page count
- Reset pagination to first page on new searches

### 6. Redux State Clearing

**Issue**: When changing presets and searching, old report results briefly appeared behind the loading spinner before new results loaded.

**Solution**:

- Added Redux clear action dispatches in `handleSearch` before executing new search:
  ```typescript
  dispatch(clearBreakdownByStore());
  dispatch(clearBreakdownByStoreManagement());
  dispatch(clearBreakdownByStoreTotals());
  ```

---

## API Integration

### RTK Query Endpoints

All endpoints are defined in `src/ui/src/reduxstore/api/YearsEndApi.ts`:

1. **getBreakdownByStore** (QPAY066TA)

   - Endpoint: `GET /api/yearend/breakdown-by-store`
   - Returns: Paginated list of all managers and associates

2. **getBreakdownByStoreInactive** (QPAY066-Inactive)

   - Endpoint: `GET /api/yearend/breakdown-by-store/inactive`
   - Returns: Inactive employees

3. **getBreakdownByStoreInactiveWithVestedBalance** (QPAY066-I)

   - Endpoint: `GET /api/yearend/breakdown-by-store/inactive/withvestedbalance`
   - Returns: Inactive employees with vested balance

4. **getBreakdownByStoreTerminatedBalanceNotVested** (QPAY066C)

   - Endpoint: `GET /api/yearend/breakdown-by-store/terminated/withcurrentbalance/notvested`
   - Returns: Terminated employees with balance but not vested
   - Requires: startDate, endDate

5. **getBreakdownByStoreTerminatedWithBenAllocations** (QPAY066B)

   - Endpoint: `GET /api/yearend/breakdown-by-store/terminated/withbeneficiaryallocation`
   - Returns: Terminated employees with beneficiary allocations
   - Optional: startDate, endDate

6. **getBreakdownByStoreRetiredWithBalanceActivity** (QPAY066W)

   - Endpoint: `GET /api/yearend/breakdown-by-store/retired/withbalanceactivity`
   - Returns: Retired employees with balance activity
   - Optional: startDate, endDate

7. **getBreakdownByStoreTotals**
   - Endpoint: `GET /api/yearend/breakdown-by-store/totals`
   - Returns: Summary totals for a specific store
   - Required: storeNumber

### Request Parameters

All endpoints accept the following parameters:

```typescript
{
  profitYear: number;           // Current profit year
  storeNumber?: number;         // Optional store filter
  storeManagement?: boolean;    // Include managers flag
  badgeNumber?: number;         // Optional badge filter (mutually exclusive with employeeName)
  employeeName?: string;        // Optional name filter (mutually exclusive with badgeNumber)
  startDate?: string;           // Optional start date (yyyy-MM-dd format)
  endDate?: string;             // Optional end date (yyyy-MM-dd format)
  pagination: {
    skip: number;               // Number of records to skip
    take: number;               // Number of records to return
    sortBy: string;             // Field to sort by
    isSortDescending: boolean;  // Sort direction
  }
}
```

### Response Format

All endpoints return a paginated response:

```typescript
{
  results: BreakdownByStoreEmployee[];  // Array of employee records
  total: number;                        // Total count of records (for pagination)
}
```

Totals endpoint returns:

```typescript
{
  totalBeginningBalances: number;
  totalVestedBalance: number;
  totalForfeitures: number;
}
```

---

## Backend Integration Points

### Related Backend Service

**File**: `src/services/src/Demoulas.ProfitSharing.Services/Reports/Breakdown/BreakdownReportService.cs`

**Key Method**: `GetBreakdownByStoreAsync`

This service method:

- Filters employees based on report criteria
- Calculates beneficiaryAllocation using profit code logic
- Applies pagination and sorting
- Returns paged results with total count

**Beneficiary Allocation Calculation** (Lines 478-483):

```csharp
var ssnsWithBeneficiaryAllocation = ctx.ProfitDetails
    .Where(ba => ba.ProfitYear == request.ProfitYear && profitCodes.Contains(ba.ProfitCodeId))
    .GroupBy(x => x.Ssn)
    .Where(x => x.Sum(r => r.ProfitCodeId == ProfitCode.Constants.OutgoingXferBeneficiary.Id
        ? -r.Forfeiture
        : (r.ProfitCodeId == ProfitCode.Constants.IncomingQdroBeneficiary.Id ? r.Contribution : 0)) > 0)
    .Select(ba => ba.Key);
```

**Profit Code Constants**:

- `OutgoingXferBeneficiary.Id = 5`: -forfeiture
- `IncomingQdroBeneficiary.Id = 6`: +contribution

---
