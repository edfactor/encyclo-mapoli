# Frontend Code Patterns - Code Review Guide

This document describes established patterns used throughout the Smart Profit Sharing frontend codebase (`src/ui/src/`). Use this guide when reviewing code to ensure consistency with existing patterns.

---

## Table of Contents

1. [Component Patterns](#1-component-patterns)
2. [State Management Patterns](#2-state-management-patterns)
3. [Hook Patterns](#3-hook-patterns)
4. [API Patterns](#4-api-patterns)
5. [Testing Patterns](#5-testing-patterns)
6. [Form Patterns](#6-form-patterns)
7. [Grid/Table Patterns](#7-gridtable-patterns)
8. [Styling Patterns](#8-styling-patterns)
9. [Type Patterns](#9-type-patterns)
10. [Utility Patterns](#10-utility-patterns)
11. [Navigation Patterns](#11-navigation-patterns)
12. [Common Page Patterns](#12-common-page-patterns)

---

## 1. Component Patterns

### File Organization

- **One component per file**: Each component has its own `.tsx` file
- **Props interface at top**: Define `interface ComponentNameProps { ... }` before the component
- **Functional components only**: No class components; use hooks for state and lifecycle

### Page Component Structure

Pages follow this structure:

```typescript
import { Page } from "smart-ui-library";
import { StatusDropdownActionNode } from "@/components/StatusDropdown";

export const PageName: React.FC = () => {
  const { state, actions } = usePageNameHook();

  return (
    <Page
      label="Page Title"
      actionNode={<StatusDropdownActionNode ... />}
    >
      {/* Filter section */}
      {/* Grid section */}
    </Page>
  );
};
```

### Reusable Components

| Component           | Purpose                                                |
| ------------------- | ------------------------------------------------------ |
| `LabelValueSection` | Display read-only key-value pairs                      |
| `PrerequisiteGuard` | Gate rendering on prerequisites (render-props pattern) |
| `StatusDropdown`    | Status selection with disabled tooltip support         |
| `ColorKey`          | Visual status/category indicators                      |

### Review Checklist

- [ ] Component uses functional style with hooks
- [ ] Props interface is explicitly typed (no `any`)
- [ ] Page components wrap content with `<Page>` from smart-ui-library
- [ ] Complex pages use custom hooks to manage state/logic

---

## 2. State Management Patterns

### Redux Architecture

```
RTK Query (server data) + Redux Slices (UI state)
```

### RTK Query for API Data

**Location**: `reduxstore/api/{Domain}Api.ts`

```typescript
export const DomainApi = createApi({
  reducerPath: "domainApi",
  baseQuery: createDataSourceAwareBaseQuery(),
  keepUnusedDataFor: 0,
  refetchOnMountOrArgChange: true,
  endpoints: (builder) => ({
    getData: builder.query<ResponseType, RequestType>({
      query: (params) => ({
        url: "/endpoint",
        method: "POST",
        body: params,
      }),
      onQueryStarted: async (_, { dispatch, queryFulfilled }) => {
        const { data } = await queryFulfilled;
        dispatch(setData(data)); // Dispatch to Redux slice
      },
    }),
    mutateData: builder.mutation<ResponseType, RequestType>({ ... }),
  }),
});

export const { useGetDataQuery, useLazyGetDataQuery, useMutateDataMutation } = DomainApi;
```

### Redux Slices for UI State

**Location**: `reduxstore/slices/{domain}Slice.ts`

```typescript
export const domainSlice = createSlice({
  name: "domain",
  initialState: { ... },
  reducers: {
    setData: (state, action: PayloadAction<DataType>) => {
      state.data = action.payload;
    },
    clearData: (state) => {
      state.data = null;
    },
  },
});

export const { setData, clearData } = domainSlice.actions;
```

### Naming Conventions

| Pattern       | Example                       |
| ------------- | ----------------------------- |
| Set action    | `setData`, `setSearchResults` |
| Clear action  | `clearData`, `resetState`     |
| Toggle action | `toggleEditMode`              |

### Review Checklist

- [ ] API calls use RTK Query, not raw fetch
- [ ] UI-only state uses Redux slices or local state (not RTK Query)
- [ ] Lazy queries (`useLazyGetDataQuery`) used for search/manual triggers
- [ ] `onQueryStarted` dispatches to slices when needed

---

## 3. Hook Patterns

### Custom Hook Organization

**Location**: `hooks/{hookName}.ts` or `pages/{PageName}/hooks/{hookName}.ts`

### Common Hook Categories

| Category         | Examples                                                | Return Pattern              |
| ---------------- | ------------------------------------------------------- | --------------------------- |
| Data fetching    | `useDecemberFlowProfitYear`, `useFiscalCloseProfitYear` | Single value                |
| State management | `useEditState`, `useTerminationState`                   | `{ state, actions }`        |
| Grid helpers     | `useDynamicGridHeight`, `useGridPagination`             | Object with computed values |
| Permission/nav   | `useIsReadOnlyByStatus`, `useReadOnlyNavigation`        | Boolean or status object    |

### Complex State Hook Pattern

```typescript
interface State {
  hasUnsavedChanges: boolean;
  searchParams: SearchParams | null;
  // ...
}

type Action =
  | { type: "SET_SEARCH_PARAMS"; payload: SearchParams }
  | { type: "RESET" };

const reducer = (state: State, action: Action): State => {
  switch (action.type) {
    case "SET_SEARCH_PARAMS":
      return { ...state, searchParams: action.payload };
    case "RESET":
      return initialState;
  }
};

export const usePageState = () => {
  const [state, dispatch] = useReducer(reducer, initialState);

  const actions = useMemo(
    () => ({
      setSearchParams: (params: SearchParams) =>
        dispatch({ type: "SET_SEARCH_PARAMS", payload: params }),
      reset: () => dispatch({ type: "RESET" }),
    }),
    []
  );

  return { state, actions };
};
```

### Review Checklist

- [ ] Complex logic extracted to custom hooks
- [ ] Hooks use `useCallback` for functions returned to components
- [ ] Dependencies in `useCallback`/`useMemo` are complete
- [ ] Hooks compose other hooks (read from Redux, call API hooks)

---

## 4. API Patterns

### Base Query Configuration

The base query (`createDataSourceAwareBaseQuery()`) provides:

- Bearer token injection from Redux
- Impersonation header support
- Data source header extraction
- Cache-busting headers
- Configurable timeout (default 100s)

### Date Handling

| Direction        | Format                                |
| ---------------- | ------------------------------------- |
| Send to API      | `MM/DD/YYYY` string                   |
| Receive from API | `yyyy-MM-dd` or `yyyy-MM-ddTHH:mm:ss` |
| Display          | `MM/DD/YYYY`                          |

### Pagination Request

```typescript
pagination: {
  skip: number; // 0-based offset
  take: number; // Page size
  sortBy: string; // Column name
  isSortDescending: boolean;
}
```

### Response Patterns

```typescript
// Paged response
interface PagedResponse<T> {
  results: T[];
  total: number;
  dataSource?: string;
}

// Report response
interface PagedReportResponse<T> {
  reportName: string;
  reportDate: string;
  response: RobustlyPaged<T>;
}
```

### Error Suppression

```typescript
query: (params) => ({
  url: "/endpoint",
  body: params,
  meta: {
    suppressAllToastErrors: true,    // Suppress all error toasts
    onlyNetworkToastErrors: true,    // Only show network errors
  },
}),
```

### Review Checklist

- [ ] Dates converted properly between API and display formats
- [ ] Pagination uses standard structure
- [ ] Error handling via metadata flags, not try/catch
- [ ] Long operations use custom timeout

---

## 5. Testing Patterns

### File Organization

| Type            | Location                              |
| --------------- | ------------------------------------- |
| Component tests | `Component.test.tsx` (same directory) |
| Hook tests      | `hooks/__test__/useHook.test.ts`      |
| Test utilities  | `test/utils/`, `test/mocks/`          |

### RTK Query Mocking

```typescript
// Lazy query mock
vi.mocked(useLazyGetData).mockReturnValue([
  vi.fn().mockResolvedValue({ data: mockData }),
  { data: mockData, isLoading: false, isError: false },
]);

// Eager query mock
vi.mocked(useGetData).mockReturnValue({
  data: mockData,
  isLoading: false,
  isError: false,
  refetch: vi.fn(),
});

// Mutation mock
vi.mocked(useCreateData).mockReturnValue([
  vi.fn().mockResolvedValue({ data: mockResult }),
  { isLoading: false, isSuccess: false },
]);
```

### Mock Factory Functions

```typescript
import {
  createRTKQueryLazyMock,
  createRTKQueryMutationMock,
} from "@/test/utils";

const mockLazyQuery = createRTKQueryLazyMock(mockData, { isLoading: false });
const mockMutation = createRTKQueryMutationMock(mockResult);
```

### Test Patterns

```typescript
describe("ComponentName", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should render correctly", async () => {
    const user = userEvent.setup();
    render(<Component />, { wrapper: createMockStoreAndWrapper() });

    await waitFor(() => {
      expect(
        screen.getByRole("button", { name: "Submit" })
      ).toBeInTheDocument();
    });
  });
});
```

### Review Checklist

- [ ] Tests use `userEvent.setup()` for interactions
- [ ] Async assertions wrapped in `waitFor()`
- [ ] Mocks cleared in `beforeEach`
- [ ] Uses accessible queries (`getByRole`, `getByLabelText`)

---

## 6. Form Patterns

### ⚠️ MANDATORY: All Forms Use Yup Validation

**ALL FORMS MUST USE YUP FOR VALIDATION. NO EXCEPTIONS.**

- Custom validation logic MUST be implemented in Yup schemas
- Manual validation (e.g., checking `if (!value)`) is NOT allowed
- All form inputs MUST be wrapped in `Controller` with Yup schema validation
- Direct use of `useState` for form fields is NOT allowed - use React Hook Form

### React Hook Form + Yup Structure

```typescript
import { useForm, Controller } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";

const schema = yup.object({
  badgeNumber: badgeNumberValidator.required("Badge is required"),
  startDate: dateStringValidator(minDate, maxDate, "Start Date"),
});

const {
  control,
  handleSubmit,
  formState: { errors },
} = useForm({
  resolver: yupResolver(schema),
  defaultValues: { badgeNumber: "", startDate: "" },
});

<form onSubmit={handleSubmit(onSubmit)}>
  <Controller
    name="badgeNumber"
    control={control}
    render={({ field }) => (
      <TextField {...field} error={!!errors.badgeNumber} />
    )}
  />
</form>;
```

### Pre-built Validators

**Location**: `utils/FormValidators.ts`

| Validator                                  | Description            |
| ------------------------------------------ | ---------------------- |
| `ssnValidator`                             | Exactly 9 digits       |
| `badgeNumberValidator`                     | 1-7 digits (1-9999999) |
| `psnValidator`                             | 9-11 digits            |
| `profitYearValidator(min, max)`            | Year in range          |
| `dateStringValidator(min, max, fieldName)` | Date validation        |
| `monthValidator`                           | 1-12                   |
| `positiveNumberValidator(fieldName)`       | Positive numbers       |

### Form Layout

```typescript
<DSMAccordion title="Search Filters" defaultExpanded>
  <form onSubmit={handleSubmit(onSearch)}>
    <Grid container rowSpacing="24px" columnSpacing={2}>
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <Controller name="field1" ... />
      </Grid>
    </Grid>
    <SearchAndReset onReset={handleReset} />
  </form>
</DSMAccordion>
```

### Review Checklist

- [ ] **CRITICAL**: Uses React Hook Form with Yup resolver (NOT manual validation)
- [ ] **CRITICAL**: NO `useState` for form field management (must use `useForm`)
- [ ] **CRITICAL**: NO manual validation checks in submit handlers (e.g., `if (!value.trim())`)
- [ ] Reuses pre-built validators from `FormValidators.ts`
- [ ] Form wrapped in `DSMAccordion` for collapsible sections
- [ ] Uses `SearchAndReset` component for action buttons
- [ ] All fields wrapped in `Controller` component

---

## 7. Grid/Table Patterns

### DSMGrid vs DSMPaginatedGrid

**For paginated data**, use `DSMPaginatedGrid` which includes built-in pagination controls:

```typescript
<DSMPaginatedGrid
  preferenceKey={GRID_KEYS.MY_GRID}
  isLoading={isFetching}
  onSortChange={sortEventHandler}
  providedOptions={{
    rowData: data?.results || [],
    columnDefs: columnDefs,
    context: { isReadOnly },
  }}
  pagination={{
    pageNumber,
    pageSize,
    sortParams: { sortBy: "fieldName", isSortDescending: false },
    handlePageNumberChange, // Pass directly - NO value - 1
    handlePageSizeChange,
    handleSortChange,
  }}
  heightConfig={{
    mode: "content-aware",
    maxHeight: gridMaxHeight,
  }}
  totalRecords={data?.total ?? 0}
  showPagination={!!data?.results?.length}
/>
```

**CRITICAL:** DSMPaginatedGrid internally converts from 1-based (UI) to 0-based (API) page numbers. **Never subtract 1** from `handlePageNumberChange` - pass handlers directly.

**For non-paginated data**, use `DSMGrid`:

```typescript
<DSMGrid
  columnDefs={columnDefs}
  rowData={data}
  pagination={false}
  loading={isLoading}
  maxHeight={500}
  rowHeight={40}
  context={{
    editedValues,
    updateEditedValue,
    loadingRowIds,
    isReadOnly,
  }}
/>
```

### Column Definition Factories

**Location**: `utils/gridColumnFactory.ts`

| Factory                    | Purpose                          |
| -------------------------- | -------------------------------- |
| `createBadgeColumn()`      | Badge numbers with optional link |
| `createNameColumn()`       | Employee names (left-aligned)    |
| `createCurrencyColumn()`   | Money values (right-aligned)     |
| `createDateColumn()`       | Dates (MM/DD/YYYY)               |
| `createPercentageColumn()` | Percentages with % suffix        |
| `createYesOrNoColumn()`    | Booleans (Y/N)                   |
| `createHoursColumn()`      | Hours with comma separators      |
| `createStoreColumn()`      | Store numbers                    |

### Column Definition Pattern

**CRITICAL**: Column definitions MUST be in separate files, never inline in components.

**File Location**: `pages/{PageName}/components/{GridName}Columns.tsx` or `components/grids/{GridName}Columns.tsx`

**Pattern**:

```typescript
import { ColDef } from "ag-grid-community";
import {
  createDateColumn,
  createCurrencyColumn,
  createYesOrNoColumn,
} from "@/utils/gridColumnFactory";

interface GridColumnsOptions {
  // Context functions/data needed by cell renderers
  handleAction: (id: number) => void;
  validationErrors?: Record<number, ValidationError>;
}

export const GetMyGridColumns = (options: GridColumnsOptions): ColDef[] => {
  const { handleAction, validationErrors } = options;

  return [
    // Use factory functions for standard column types
    createDateColumn({
      headerName: "Start Date",
      field: "startDate",
      minWidth: 120,
    }),
    createCurrencyColumn({
      headerName: "Amount",
      field: "amount",
      minWidth: 100,
    }),
    createYesOrNoColumn({
      headerName: "Active",
      field: "isActive",
      minWidth: 80,
    }),

    // Custom columns when factories don't apply
    {
      headerName: "Custom Column",
      field: "customField",
      colId: "customField",
      minWidth: 120,
      sortable: true,
      resizable: true,
      headerClass: "right-align",
      cellClass: "right-align",
      valueFormatter: (params) => formatValue(params.value),
      cellRenderer: (params) => {
        // Custom cell renderer logic
        return (
          <CustomCellComponent data={params.data} onAction={handleAction} />
        );
      },
    },
  ];
};
```

**In Component**:

```typescript
import { GetMyGridColumns } from "./MyGridColumns";

const MyComponent = () => {
  const handleAction = useCallback((id: number) => {
    // handle action
  }, []);

  const columnDefs = useMemo(
    () => GetMyGridColumns({ handleAction, validationErrors }),
    [handleAction, validationErrors]
  );

  return <DSMGrid columnDefs={columnDefs} rowData={data} />;
};
```

**Available Factory Functions**:

- `createBadgeColumn()` - Badge numbers with optional Master Inquiry link
- `createNameColumn()` - Employee names (left-aligned)
- `createCurrencyColumn()` - Money values (right-aligned, formatted)
- `createDateColumn()` - Dates (MM/DD/YYYY format)
- `createPercentageColumn()` - Percentages with % suffix
- `createYesOrNoColumn()` - Booleans (renders "Yes"/"No")
- `createHoursColumn()` - Hours with comma separators
- `createStoreColumn()` - Store numbers (right-aligned)
- `createStateColumn()` - State codes (2-letter)
- `createAddressColumn()` - Address fields
- `createZipColumn()` - Zip codes with formatting
- `createCityColumn()` - City names
- `createSSNColumn()` - SSN (properly masked)
- `createAgeColumn()` - Age fields (right-aligned)
- `createPhoneColumn()` - Phone numbers with formatting
- `createPSNColumn()` - PSN numbers
- `createTaxCodeColumn()` - Tax codes with brackets

### Old Pattern (DO NOT USE)

### Old Pattern (DO NOT USE)

**❌ WRONG - Inline column definitions**:

```typescript
const MyComponent = () => {
  const columnDefs: ColDef[] = useMemo(
    () => [
      {
        field: "startDate",
        headerName: "Start Date",
        valueFormatter: (params) => new Date(params.value).toLocaleDateString(),
      },
      // ... more columns inline
    ],
    []
  );

  return <DSMGrid columnDefs={columnDefs} />;
};
```

**Problems with inline definitions:**

1. Makes components too long and hard to read
2. Can't reuse column definitions across grids
3. Doesn't enforce use of factory functions for consistency
4. Harder to test column configurations

### Custom Cell Renderers

```typescript
export const EditableCellRenderer: React.FC<ICellRendererParams> = (props) => {
  const { editedValues, updateEditedValue, isReadOnly } = props.context ?? {};
  const rowKey = generateRowKey(props.data);

  if (isReadOnly) {
    return <span>{props.value}</span>;
  }

  return (
    <input
      value={editedValues?.[rowKey]?.value ?? props.value}
      onChange={(e) => updateEditedValue?.(rowKey, e.target.value, false)}
    />
  );
};
```

### Grid Context Pattern

```typescript
const gridContext = {
  editedValues: Record<string, { value: number; hasError: boolean }>,
  updateEditedValue: (rowKey: string, value: number, hasError: boolean) => void,
  loadingRowIds: Set<string>,
  isReadOnly: boolean,
};
```

### Review Checklist

- [ ] Uses `DSMPaginatedGrid` for paginated data, `DSMGrid` for non-paginated (not raw AG Grid)
- [ ] **MANDATORY: Column definitions in separate `*GridColumns.ts` file (NEVER inline in component)**
- [ ] **MANDATORY: Uses factory functions from `gridColumnFactory.ts` for common column types (dates, currency, yes/no, etc.)**
- [ ] Context passed for editable grids
- [ ] Alignment classes consistent (right-align for numbers, center-align for yes/no)
- [ ] Pagination handlers passed directly to DSMPaginatedGrid (no `value - 1`)

---

## 8. Styling Patterns

### Material-UI Grid Layout

```typescript
<Grid container rowSpacing="24px" columnSpacing={2}>
  <Grid size={{ xs: 12, sm: 6, md: 4 }}>{/* Responsive content */}</Grid>
  <Grid width="100%">{/* Full-width content */}</Grid>
</Grid>
```

### Common MUI Components

| Component    | Usage                         |
| ------------ | ----------------------------- |
| `TextField`  | Text inputs, selects, numbers |
| `Button`     | Actions                       |
| `Tooltip`    | Help text with arrow          |
| `Box`        | Container with `sx` prop      |
| `Typography` | Text styling                  |
| `Divider`    | Visual separators             |

### AG Grid Alignment Classes

| Class          | Usage                  |
| -------------- | ---------------------- |
| `left-align`   | Text columns (default) |
| `center-align` | Status columns         |
| `right-align`  | Number columns         |

### Review Checklist

- [ ] Uses MUI Grid for layout (not raw CSS grid)
- [ ] Numbers right-aligned in grids
- [ ] Consistent spacing values (`rowSpacing="24px"`)
- [ ] Uses `sx` prop for inline styles (not `style`)

---

## 9. Type Patterns

### Type Organization

**Location**: `types/{domain}/{category}.ts`

Examples:

- `types/common/api.ts` - Common API types
- `types/distributions.ts` - Distribution domain
- `types/december-activities/termination.ts` - Feature-specific

### Request/Response DTOs

```typescript
// Request
interface GetDataRequest {
  badgeNumber?: string;
  profitYear: number;
  pagination: PaginationParams;
}

// Response
interface GetDataResponse {
  results: DataItem[];
  total: number;
  dataSource?: string;
}
```

### Const Objects (instead of enums)

```typescript
export const NavigationStatus = {
  NotStarted: 1,
  InProgress: 2,
  OnHold: 3,
  Complete: 4,
} as const;

export type NavigationStatusType =
  (typeof NavigationStatus)[keyof typeof NavigationStatus];
```

### Union Types for State

```typescript
type LoadingState = "idle" | "loading" | "error" | "success";
type ReportType = "distribution" | "contribution" | "forfeiture";
```

### Review Checklist

- [ ] No `any` type (project convention)
- [ ] Types in dedicated `types/` directory
- [ ] Use `as const` objects instead of enums
- [ ] Generics used for reusable types

---

## 10. Utility Patterns

### Date Utilities

**Location**: `utils/dateUtils.ts`

```typescript
import {
  isMaskedDate,
  getMaskedDateDisplay,
  mmDDYYFormat,
} from "@/utils/dateUtils";

// Always check for masked dates
const displayDate = isMaskedDate(date)
  ? getMaskedDateDisplay()
  : mmDDYYFormat(date);
```

### Key Date Functions

| Function                 | Purpose                             |
| ------------------------ | ----------------------------------- |
| `isMaskedDate()`         | Check if date is masked             |
| `getMaskedDateDisplay()` | Get masked display string           |
| `mmDDYYFormat()`         | Format as MM/DD/YYYY                |
| `tryddmmyyyyToDate()`    | Parse with multiple format attempts |

### Number Formatting

```typescript
import { numberToCurrency } from "smart-ui-library";
import { formatNumberWithComma, formatPercentage } from "@/utils";

const currency = numberToCurrency(1234.56); // "$1,234.56"
const formatted = formatNumberWithComma(1234); // "1,234"
const percent = formatPercentage(0.85); // "85%"
```

### Lookup Tables

**Location**: `utils/lookups.ts`

```typescript
const taxCodes: Record<string, string> = {
  "0": "0: Unknown",
  "1": "1: Early distribution",
  // ...
};

export const getTaxCodeLabel = (code: string) => taxCodes[code] ?? "";
```

### Review Checklist

- [ ] Date display checks for masked dates
- [ ] Currency formatting uses `numberToCurrency`
- [ ] Lookup tables are Record types
- [ ] Error utilities used for validation messages

---

## 11. Navigation Patterns

### React Router Navigation

```typescript
const navigate = useNavigate();

// Simple navigation
navigate("/page-name");

// With parameters
navigate(`/page/${id}`);

// With state
navigate("/target", { state: { successMsg: "Saved!" } });

// Back navigation
navigate(-1);
```

### Reading Location State

```typescript
const location = useLocation();
const state = location.state as { successMsg?: string } | undefined;

// Clear state after reading
useEffect(() => {
  if (state?.successMsg) {
    showMessage(state.successMsg);
    window.history.replaceState({}, document.title);
  }
}, [state]);
```

### Guards and Permissions

| Hook                       | Purpose                                 |
| -------------------------- | --------------------------------------- |
| `useReadOnlyNavigation()`  | Detect read-only mode                   |
| `useUnsavedChangesGuard()` | Prevent navigation with unsaved changes |
| `useIsReadOnlyByStatus()`  | Check if page is read-only by status    |

### Review Checklist

- [ ] Uses `useNavigate` hook (not `history`)
- [ ] State passed via navigate, not global state
- [ ] Location state cleared after reading
- [ ] Read-only mode handled via guards/hooks

---

## 12. Common Page Patterns

### Search-Filter-Grid Page

Used in 30+ pages. Structure:

1. Collapsible filter section (`DSMAccordion`)
2. React Hook Form + Yup validation
3. Lazy API query for search
4. Conditional grid display (only after search)
5. Pagination and sorting
6. Optional bulk actions

```typescript
export const SearchPage: React.FC = () => {
  const { handleSubmit, control } = useForm({ resolver: yupResolver(schema) });
  const [triggerSearch, { data, isLoading }] = useLazySearchQuery();
  const [hasSearched, setHasSearched] = useState(false);

  const onSearch = async (formData: FormData) => {
    await triggerSearch(formData);
    setHasSearched(true);
  };

  return (
    <Page label="Search Page">
      <DSMAccordion title="Search Filters" defaultExpanded>
        <form onSubmit={handleSubmit(onSearch)}>
          {/* Form fields */}
          <SearchAndReset onReset={reset} />
        </form>
      </DSMAccordion>

      {hasSearched && (
        <DSMGrid
          columnDefs={columns}
          rowData={data?.results}
          loading={isLoading}
        />
      )}
    </Page>
  );
};
```

### Inline Editable Grid

1. Custom cell editor (currency input with validation)
2. Edit state tracking via context
3. Error highlighting with visual indicators
4. Row-level save + bulk save buttons
5. Navigation prevented during edits

### Detail Row Expansion

1. Select from master grid
2. Fetch and display detail grid
3. Filter or sort detail data
4. Show member details section

### RTK Query + Redux Slice Integration

1. RTK Query fetches data
2. `onQueryStarted` dispatches to Redux slice
3. Components read from Redux slice
4. API cache handled transparently

### Message/Alert System

```typescript
// Wrap page
<MissiveAlertProvider>
  <PageContent />
</MissiveAlertProvider>

// Display alerts
<MissiveAlerts />

// Dispatch message
dispatch(setMessage({ type: "success", text: "Saved!" }));
```

---

## Quick Reference

### File Naming Conventions

| Type         | Convention                  | Example                     |
| ------------ | --------------------------- | --------------------------- |
| Component    | PascalCase                  | `SearchFilter.tsx`          |
| Hook         | camelCase with `use` prefix | `useTerminationState.ts`    |
| Utility      | camelCase                   | `dateUtils.ts`              |
| Types        | PascalCase or domain-based  | `distributions.ts`          |
| Test         | `.test.tsx` suffix          | `Component.test.tsx`        |
| Grid columns | `*GridColumns.ts`           | `TerminationGridColumns.ts` |

### Import Order

```typescript
// 1. React/external libraries
import React, { useState, useCallback } from "react";
import { useForm } from "react-hook-form";

// 2. UI libraries
import { Grid, TextField } from "@mui/material";
import { Page, DSMGrid } from "smart-ui-library";

// 3. Internal imports (absolute)
import { useGetDataQuery } from "@/reduxstore/api/DataApi";
import { formatDate } from "@/utils/dateUtils";

// 4. Relative imports
import { getGridColumns } from "./GridColumns";
import { usePageState } from "./hooks/usePageState";
```

---

## Summary

This codebase follows a highly structured, modular architecture with:

- **RTK Query** for server data
- **Redux slices** for UI state
- **Custom hooks** for logic encapsulation
- **Material-UI + smart-ui-library** for components
- **React Hook Form + Yup** for forms
- **AG Grid (via DSMGrid)** for data tables

When reviewing code, verify it follows these established patterns rather than introducing new approaches.
