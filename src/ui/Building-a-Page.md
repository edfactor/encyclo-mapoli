# Building a Page in Profit Sharing UI

This guide shows how to build pages in the Profit Sharing UI using the Master Inquiry page as a real example. We follow a **unidirectional data flow** where user interactions flow through validation, API calls, state management, and UI updates.

## End-to-End User Flow

**User Journey**: User visits `/master-inquiry` → enters badge number → hits search → sees results

**Technical Flow**: Router → Page Component → Form Validation → API Call → Redux State → UI Update

## 1. Router Setup

Routes connect URLs to components in `src/components/router/RouterSubAssembly.tsx`:

```tsx
<Route
  path={`${ROUTES.MASTER_INQUIRY}/:badgeNumber?`}
  element={<MasterInquiry />}
/>
```

Constants are defined in `src/constants.ts`:

```tsx
export const ROUTES = {
  MASTER_INQUIRY: "master-inquiry"
} as const;
```

## 2. Page Component Structure

Every page follows this pattern from `src/pages/MasterInquiry/MasterInquiry.tsx`:

```tsx
const MasterInquiry = () => {
  return (
    <Page label="MASTER INQUIRY (008-10)">
      <MissiveAlertProvider>
        <MasterInquiryContent />
      </MissiveAlertProvider>
    </Page>
  );
};

const MasterInquiryContent = memo(() => {
  const { 
    searchResults,
    isSearching,
    executeSearch,
    resetAll
  } = useMasterInquiry(); // Custom hook with all logic

  return (
    <Grid container>
      <DSMAccordion title="Filter">
        <MasterInquirySearchFilter
          onSearch={executeSearch}
          onReset={resetAll}
          isSearching={isSearching}
        />
      </DSMAccordion>
      
      {searchResults && (
        <MasterInquiryGrid data={searchResults} />
      )}
    </Grid>
  );
});
```

**Key Pattern**: 
- `Page` wrapper from smart-ui-library
- Provider for context (alerts, etc.)
- Separated content component using `memo()`
- Custom hook encapsulates all logic

## 3. Form Validation (Real-time)

User types in form → validation runs on every keystroke using Yup schema from `src/pages/MasterInquiry/MasterInquirySearchFilter.tsx`:

```tsx
const schema = yup.object().shape({
  badgeNumber: yup
    .number()
    .typeError("Badge number must be a number")
    .integer("Badge number must be an integer")
    .min(0, "Badge number must be positive")
    .max(99999999999, "Badge number must be 11 digits or less")
    .nullable(),
  endProfitYear: yup
    .number()
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .test("greater-than-start", "End year must be after start year", function (endYear) {
      const startYear = this.parent.startProfitYear;
      return !startYear || !endYear || endYear >= startYear;
    })
    .nullable()
});

// In component
const {
  control,
  formState: { errors },
  watch,
  setValue
} = useForm<MasterInquirySearch>({
  resolver: yupResolver(schema),
  mode: "onChange" // Validates on every change
});
```

## 4. API Integration with RTK Query

User clicks search → API call triggered from `src/reduxstore/api/InquiryApi.ts`:

```tsx
export const InquiryApi = createApi({
  reducerPath: "inquiryApi",
  baseQuery: createDataSourceAwareBaseQuery(),
  endpoints: (builder) => ({
    searchProfitMasterInquiry: builder.query<Paged<EmployeeDetails>, MasterInquiryRequest>({
      query: (params) => ({
        url: "master/master-inquiry/search",
        method: "POST",
        body: {
          badgeNumber: params.badgeNumber,
          ssn: params.ssn,
          name: params.name,
          // ... other search parameters
        }
      })
    })
  })
});

export const { useLazySearchProfitMasterInquiryQuery } = InquiryApi;
```

**Usage in Hook** (`src/pages/MasterInquiry/hooks/useMasterInquiry.ts`):

```tsx
const [triggerSearch, { isLoading: isSearching }] = useLazySearchProfitMasterInquiryQuery();

const executeSearch = useCallback(async (params: MasterInquiryRequest) => {
  try {
    dispatch({ type: "SEARCH_START", payload: { params, isManual: true } });
    clearAlerts();

    const response = await triggerSearch(params).unwrap();
    
    if (response?.results?.length > 0) {
      dispatch({ type: "SEARCH_SUCCESS", payload: { results: response } });
    } else {
      dispatch({ type: "SEARCH_SUCCESS", payload: { results: { results: [], total: 0 } } });
      addAlert(MASTER_INQUIRY_MESSAGES.MEMBER_NOT_FOUND);
    }
  } catch (error) {
    dispatch({ type: "SEARCH_FAILURE", payload: { error: error?.toString() || "Unknown error" } });
    addAlert({
      id: 999,
      severity: "Error", 
      message: "Search Failed",
      description: "The search request failed. Please try again."
    });
  }
}, [triggerSearch, clearAlerts, addAlert]);
```

## 5. State Management

API response → Redux state → UI update using `useReducer` pattern from `src/pages/MasterInquiry/hooks/useMasterInquiryReducer.ts`:

```tsx
// State shape
interface State {
  search: {
    results: SearchResults | null;
    isLoading: boolean;
    error: string | null;
  };
  selection: {
    selectedMember: SelectedMember | null;
    memberDetails: EmployeeDetails | null;
  };
}

// Typed actions
type Action = 
  | { type: "SEARCH_START"; payload: { params: MasterInquiryRequest } }
  | { type: "SEARCH_SUCCESS"; payload: { results: SearchResults } }
  | { type: "SEARCH_FAILURE"; payload: { error: string } }
  | { type: "SELECT_MEMBER"; payload: { member: SelectedMember } };

// Reducer
const masterInquiryReducer = (state: State, action: Action): State => {
  switch (action.type) {
    case "SEARCH_START":
      return { 
        ...state, 
        search: { ...state.search, isLoading: true, error: null } 
      };
    case "SEARCH_SUCCESS":
      return { 
        ...state, 
        search: { ...state.search, isLoading: false, results: action.payload.results } 
      };
    case "SEARCH_FAILURE":
      return { 
        ...state, 
        search: { ...state.search, isLoading: false, error: action.payload.error } 
      };
    default:
      return state;
  }
};
```

## 6. Data Display with Grids

Results render using AgGrid with column factory from `src/utils/gridColumnFactory.ts`:

```tsx
// Column definitions
const columnDefs = [
  createBadgeColumn({ 
    renderAsLink: true,
    navigateFunction: navigate 
  }),
  createNameColumn({ 
    headerName: "Employee Name",
    field: "employeeName" 
  }),
  createCurrencyColumn({ 
    headerName: "Current Balance", 
    field: "currentBalance" 
  }),
  createDateColumn({ 
    headerName: "Hire Date", 
    field: "hireDate" 
  })
];

// Grid component
<AgGrid
  columnDefs={columnDefs}
  rowData={searchResults.results}
  pagination={true}
  paginationPageSize={25}
  onRowClicked={onMemberSelect}
/>
```

## 7. Pagination & Sorting

Consistent pagination using `src/hooks/useGridPagination.ts`:

```tsx
const memberGridPagination = useGridPagination({
  initialPageSize: 5,
  initialSortBy: "badgeNumber",
  initialSortDescending: true,
  onPaginationChange: handleMemberGridPaginationChange
});

// In component
<MasterInquiryGrid
  data={searchResults}
  pagination={memberGridPagination}
  onPaginationChange={memberGridPagination.handlePaginationChange}
  onSortChange={memberGridPagination.handleSortChange}
/>
```

## Complete User Journey Example

**1. User navigates to Master Inquiry page**
```
URL: /master-inquiry → RouterSubAssembly.tsx renders <MasterInquiry />
```

**2. User enters badge number "1234567"**
```
MasterInquirySearchFilter.tsx → Yup validation runs → No errors
```

**3. User clicks "Search"**
```
executeSearch() called → dispatch("SEARCH_START") → UI shows loading
```

**4. API call made**
```
triggerSearch({ badgeNumber: "1234567" }) → POST to /master/master-inquiry/search
```

**5. Response received**
```
Success: dispatch("SEARCH_SUCCESS") → state.search.results updated
Error: dispatch("SEARCH_FAILURE") → error message shown
```

**6. UI updates**
```
searchResults truthy → MasterInquiryGrid renders with data
```

## Project File Structure

Real structure from Master Inquiry:

```
src/pages/MasterInquiry/
├── MasterInquiry.tsx                    # Main page component
├── MasterInquiryDetailsGrid.tsx         # Data display
├── MasterInquirySearchFilter.tsx        # Form component  
├── MasterInquiryMemberGrid.tsx          # Results grid
├── hooks/
│   ├── useMasterInquiry.ts             # Main logic hook
│   ├── useMasterInquiryReducer.ts      # State management
│   └── useMissiveAlerts.ts             # Alert handling
└── utils/
    ├── MasterInquiryFunctions.ts       # Pure functions
    └── MasterInquiryMessages.ts        # Constants
```

## Best Practices

**State Management**:
- RTK Query for server state (API calls)
- useReducer for complex local state  
- Redux slices for global app state

**Performance**:
- `memo()` on content components
- `useCallback()` for event handlers
- `useMemo()` for expensive computations

**Error Handling**:
- Try-catch in async functions
- User-friendly error messages
- Graceful fallbacks for failed states

**Component Composition**:
- Small, focused components
- Custom hooks for logic
- Smart-ui-library components for consistency

This architecture provides a predictable, maintainable pattern that scales across all pages in the Profit Sharing application.