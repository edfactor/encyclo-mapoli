---
applyTo: "src/ui/src/pages/**/*.*"
paths: "src/ui/src/pages/**/*.*"
---

# Page Component Architecture

## Overview

This document provides a detailed technical description of how page components are assembled in the Smart Profit Sharing UI application. The application follows a consistent, modular architecture where pages are composed from reusable building blocks and follow established patterns for state management, data fetching, and user interactions.

## Page Component Patterns

The application uses several recurring page component patterns based on complexity and functionality.

### 1. Simple Grid Page

**Pattern**: Single grid with optional filters, minimal state.

**Example**: `Profall.tsx`

```typescript
const Profall = () => {
  const [pageNumberReset, setPageNumberReset] = useState(false);

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page label={CAPTIONS.PROFALL} actionNode={renderActionNode()}>
      <Grid container rowSpacing="24px">
        <Grid width="100%">
          <Divider />
        </Grid>
        <Grid width="100%">
          <ProfallGrid
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>
    </Page>
  );
};
```

**Key Characteristics**:

- Wraps content in `<Page>` component from `smart-ui-library`
- Provides `label` (page title) and optional `actionNode` (usually StatusDropdownActionNode)
- Uses Material-UI `Grid` for layout
- Single grid component as primary content
- Minimal local state

---

### 2. Search/Filter Page

**Pattern**: Collapsible filter section with grid showing results.

**Example**: `DistributionInquiry.tsx`

```typescript
const DistributionInquiryContent = () => {
  const dispatch = useDispatch();
  const [searchData, setSearchData] =
    useState<DistributionSearchRequest | null>(null);
  const [hasSearched, setHasSearched] = useState(false);
  const [triggerSearch, { data, isFetching }] =
    useLazySearchDistributionsQuery();
  const { missiveAlerts, addAlert, clearAlerts } = useMissiveAlerts();

  const handleSearch = async (formData: DistributionSearchFormData) => {
    // Map form data to API request
    // Trigger search
    // Handle errors
  };

  return (
    <Grid container rowSpacing="24px">
      {/* Missive alerts */}
      {missiveAlerts.length > 0 && <MissiveAlerts />}

      {/* Action buttons */}
      <Grid width="100%">
        <Button onClick={handleNewEntry}>NEW ENTRY</Button>
        <Button onClick={handleExport}>EXPORT</Button>
      </Grid>

      {/* Collapsible filter accordion */}
      <Grid width="100%">
        <DSMAccordion title="Filter">
          <DistributionInquirySearchFilter
            onSearch={handleSearch}
            onReset={handleReset}
            isLoading={isFetching}
          />
        </DSMAccordion>
      </Grid>

      {/* Results grid (conditional) */}
      {hasSearched && (
        <Grid width="100%">
          <DistributionInquiryGrid
            postReturnData={data?.results ?? null}
            totalRecords={data?.total ?? 0}
            isLoading={isFetching}
            onPaginationChange={handlePaginationChange}
          />
        </Grid>
      )}
    </Grid>
  );
};

const DistributionInquiry = () => {
  return (
    <Page
      label={CAPTIONS.DISTRIBUTIONS_INQUIRY}
      actionNode={<StatusDropdownActionNode />}
    >
      <MissiveAlertProvider>
        <DistributionInquiryContent />
      </MissiveAlertProvider>
    </Page>
  );
};
```

---

### 3. Complex Page with Custom Hook

**Pattern**: Encapsulates complex state and logic in a custom hook, page component focuses on rendering.

**Example**: `MasterInquiry.tsx`

```typescript
const MasterInquiryContent = memo(() => {
  const {
    searchParams,
    searchResults,
    isSearching,
    selectedMember,
    memberDetails,
    memberProfitData,
    showMemberGrid,
    showMemberDetails,
    showProfitDetails,
    executeSearch,
    selectMember,
    resetAll,
  } = useMasterInquiry(); // Custom hook encapsulates all logic

  return (
    <Grid container>
      <DSMAccordion title="Filter">
        <MasterInquirySearchFilter
          onSearch={executeSearch}
          onReset={resetAll}
          isSearching={isSearching}
        />
      </DSMAccordion>

      {showMemberGrid && searchResults && (
        <MasterInquiryMemberGrid
          searchResults={searchResults}
          onMemberSelect={selectMember}
        />
      )}

      {selectedMember && showMemberDetails && (
        <MasterInquiryMemberDetails memberDetails={memberDetails} />
      )}

      {selectedMember && showProfitDetails && (
        <MasterInquiryGrid profitData={memberProfitData} />
      )}
    </Grid>
  );
});

const MasterInquiry = () => {
  return (
    <Page label="MASTER INQUIRY (008-10)">
      <MissiveAlertProvider>
        <MasterInquiryContent />
      </MissiveAlertProvider>
    </Page>
  );
};
```

**Custom Hook Pattern** (`useMasterInquiry.ts`):

```typescript
const useMasterInquiry = () => {
  const [state, dispatch] = useReducer(masterInquiryReducer, initialState);
  const [triggerSearch, { isLoading }] = useLazySearchProfitMasterInquiryQuery();
  const [triggerMemberDetails] = useLazyGetProfitMasterInquiryMemberQuery();
  const { addAlert, clearAlerts } = useMissiveAlerts();

  const executeSearch = useCallback((params) => {
    // Execute search logic
    // Update reducer state
  }, []);

  const selectMember = useCallback((member) => {
    // Fetch member details
    // Fetch profit data
    // Update state
  }, []);

  return {
    // Expose state and actions
    searchResults: state.search.results,
    selectedMember: state.selection.selectedMember,
    executeSearch,
    selectMember
    // ... other state/actions
  };
};
```

---

### 4. Tabbed Page

**Pattern**: Multiple tabs/views within a single page.

**Example**: `FrozenSummary.tsx`

```typescript
const FrozenSummary = () => {
  const [selectedTab, setSelectedTab] = useState(0);

  const tabs = [
    "Summary",
    "Distributions",
    "Contributions",
    "Forfeitures",
    "Balance",
  ];

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setSelectedTab(newValue);
  };

  return (
    <Page label="Frozen Process Summary">
      <Grid container width="100%" rowSpacing="24px">
        <Grid width="100%">
          <Tabs value={selectedTab} onChange={handleTabChange}>
            {tabs.map((tab, index) => (
              <Tab key={index} label={tab} />
            ))}
          </Tabs>
        </Grid>
        <Grid width="100%">
          {selectedTab === 0 ? (
            <CleanUpSummaryCards setSelectedTab={setSelectedTab} />
          ) : (
            <CleanUpSummaryGrids tabIndex={selectedTab} />
          )}
        </Grid>
      </Grid>
    </Page>
  );
};
```

---

### 5. Complex Workflow Page

**Pattern**: Multi-step workflow with forms, confirmations, validations, and side effects.

**Example**: `ProfitShareEditUpdate.tsx`

```typescript
const ProfitShareEditUpdate = () => {
  const dispatch = useDispatch();
  const profitYear = useFiscalCloseProfitYear();
  const [changesApplied, setChangesApplied] = useState(false);
  const [showConfirmation, setShowConfirmation] = useState(false);
  const { profitSharingEdit, profitSharingUpdate } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const [applyMaster] = useGetMasterApplyMutation();
  const [triggerRevert] = useLazyGetMasterRevertQuery();
  const [triggerStatus] = useLazyGetProfitMasterStatusQuery();

  const { checksumValidation } = useChecksumValidation({ profitYear });

  const saveAction = useCallback(async () => {
    // Complex save logic with validation
    // Dispatch Redux actions
    // Show confirmation
    // Refresh checksums
  }, []);

  const revertAction = useCallback(async () => {
    // Revert changes
    // Clear state
    // Show messages
  }, []);

  return (
    <PrerequisiteGuard prerequisites={["FrozenData"]}>
      <Page
        label="PROFIT SHARE EDIT/UPDATE"
        actionNode={<StatusDropdownActionNode />}
      >
        <Grid container rowSpacing="24px">
          {/* Checksum validation alerts */}
          {checksumValidation && <ValidationAlert data={checksumValidation} />}

          {/* Search/filter section */}
          <DSMAccordion title="Filter">
            <ProfitShareEditUpdateSearchFilter onSearch={handleSearch} />
          </DSMAccordion>

          {/* Tabbed grids for Edit vs Update */}
          <ProfitShareEditUpdateTabs />

          {/* Summary tables */}
          <MasterUpdateSummaryTable />

          {/* Action buttons */}
          <Button onClick={saveAction}>APPLY</Button>
          <Button onClick={revertAction}>REVERT</Button>

          {/* Confirmation modal */}
          <ProfitShareEditConfirmation
            open={showConfirmation}
            onConfirm={handleConfirm}
            onCancel={handleCancel}
          />
        </Grid>
      </Page>
    </PrerequisiteGuard>
  );
};
```

---

## Core Building Blocks

### 1. Page Wrapper (`smart-ui-library`)

The `Page` component is the top-level container for all pages.

```typescript
<Page
  label="DISTRIBUTION INQUIRY" // Page title (displayed in header)
  actionNode={<StatusDropdownActionNode />} // Optional action area (right side of header)
>
  {/* Page content */}
</Page>
```

**Props**:

- `label`: Page title (string)
- `actionNode`: Optional React node for header actions (typically `StatusDropdownActionNode`)

---

### 2. DSMAccordion (Collapsible Filter)

Used for collapsible filter sections.

```typescript
<DSMAccordion title="Filter">
  <MySearchFilter onSearch={handleSearch} onReset={handleReset} />
</DSMAccordion>
```

**Common Use**: Wrapping search/filter forms to save vertical space.

---

### 3. DSMGrid vs DSMPaginatedGrid

**Use `DSMPaginatedGrid` for server-side paginated data** (recommended for most use cases).
**Use `DSMGrid` only for small, non-paginated datasets** (e.g., static lookups, small inline tables).

#### DSMPaginatedGrid (Recommended for Paginated Data)

Combines grid and pagination into a single component with built-in:

- Integrated pagination controls
- Content-aware or fixed height modes
- Loading state handling
- Sort event handling

```typescript
import { DSMPaginatedGrid, ISortParams } from "smart-ui-library";
import { useGridPagination } from "../../../hooks/useGridPagination";

const MyGrid: React.FC = () => {
  const { pageNumber, pageSize, handlePageNumberChange, handlePageSizeChange, handleSortChange } =
    useGridPagination({
      initialPageSize: 25,
      initialSortBy: "badgeNumber",
      initialSortDescending: false,
      persistenceKey: GRID_KEYS.MY_GRID,
      onPaginationChange: (pageNum, pageSz, sortParams) => {
        // Trigger API call with pagination params
        getData({ skip: pageNum * pageSz, take: pageSz, ...sortParams });
      }
    });

  return (
    <DSMPaginatedGrid
      preferenceKey={GRID_KEYS.MY_GRID}
      isLoading={isFetching}
      onSortChange={(update: ISortParams) => handleSortChange(update)}
      providedOptions={{
        rowData: data?.results || [],
        columnDefs: columnDefs
      }}
      pagination={{
        pageNumber,
        pageSize,
        sortParams: { sortBy: "badgeNumber", isSortDescending: false },
        handlePageNumberChange,  // Pass directly - NO value - 1
        handlePageSizeChange,
        handleSortChange
      }}
      heightConfig={{
        mode: "content-aware",
        maxHeight: 600
      }}
      totalRecords={data?.total ?? 0}
      showPagination={!!data?.results?.length}
    />
  );
};
```

**CRITICAL**: When using `DSMPaginatedGrid`, pass handlers directly (e.g., `handlePageNumberChange,`). Do NOT subtract 1 from the page number - `DSMPaginatedGrid` handles the 1-based to 0-based conversion internally.

#### DSMGrid (For Non-Paginated Data Only)

Use for small datasets that don't require pagination:

```typescript
<DSMGrid
  preferenceKey="SMALL_LOOKUP_GRID"
  isLoading={isLoading}
  maxHeight={300}
  providedOptions={{
    rowData: lookupData ?? [],
    columnDefs: columnDefs
  }}
/>

```

**Grid Column Factory** (`utils/gridColumnFactory.ts`):

- Provides factory functions for common column types
- Ensures consistency across grids
- Common factories: `createBadgeColumn`, `createNameColumn`, `createStoreColumn`, `createStateColumn`, `createZipColumn`, `createAddressColumn`

---

### 6. StatusDropdownActionNode

Standard component for page status tracking.

```typescript
const renderActionNode = () => {
  return <StatusDropdownActionNode />;
};

<Page
  label="MY PAGE"
  actionNode={renderActionNode()}
>
```

**Features**:

- Fetches current navigation status from API
- Allows users to update status (Not Started, In Progress, Completed)
- Automatically disabled in read-only mode
- Integrates with navigation state in Redux

---

### 7. Missive Alerts

Application-wide alert/message system.

```typescript
// In page wrapper:
<MissiveAlertProvider>
  <PageContent />
</MissiveAlertProvider>;

// In page content:
const { missiveAlerts, addAlert, clearAlerts } = useMissiveAlerts();

// Display alerts:
{
  missiveAlerts.length > 0 && <MissiveAlerts />;
}

// Add alert:
addAlert({
  id: 911,
  severity: "success",
  message: "Distribution Saved Successfully",
  description: "Distribution for John Doe has been saved.",
});

// Clear alerts:
clearAlerts();
```

**Predefined Messages** (`MissiveMessages.ts`):

- `DISTRIBUTION_INQUIRY_MESSAGES.MEMBER_NOT_FOUND`
- `DISTRIBUTION_INQUIRY_MESSAGES.SAVE_SUCCESS`
- etc.

---

### 8. Modals/Dialogs

Confirmation dialogs, data entry dialogs, etc.

```typescript
<DeleteDistributionModal
  open={isDeleteDialogOpen}
  distribution={distributionToDelete}
  onConfirm={handleConfirmDelete}
  onCancel={handleCloseDeleteDialog}
  isLoading={isDeleting}
/>
```

**Common Props**:

- `open`: Boolean state
- `onConfirm`: Callback for confirmation
- `onCancel`: Callback for cancellation
- `isLoading`: Loading state during async operations

---

## State Management Patterns

### 1. Local Component State

Simple pages use `useState` for local UI state.

```typescript
const [isDialogOpen, setIsDialogOpen] = useState(false);
const [searchData, setSearchData] = useState<SearchRequest | null>(null);
const [selectedTab, setSelectedTab] = useState(0);
```

**Use When**:

- State is purely UI-related
- State doesn't need to be shared across components
- State doesn't persist across navigation

---

### 2. Redux Global State

Complex state or state shared across pages stored in Redux slices.

```typescript
// Reading from Redux
const { profitSharingEdit, profitSharingUpdate } = useSelector((state: RootState) => state.yearsEnd);
const currentMember = useSelector((state: RootState) => state.distribution.currentMember);

// Dispatching actions
const dispatch = useDispatch();
dispatch(setCurrentMember(memberData));
dispatch(clearProfitSharingUpdate());
```

**Use When**:

- Data needs to be shared across multiple pages
- Data persists across navigation
- Centralized state management is beneficial

---

### 3. RTK Query Cache

Server data automatically cached by RTK Query.

```typescript
// Lazy query (manual trigger)
const [triggerSearch, { data, isFetching, error }] = useLazySearchDistributionsQuery();

// Trigger
await triggerSearch({ badgeNumber: 12345 });

// Mutation
const [deleteDistribution, { isLoading }] = useDeleteDistributionMutation();
await deleteDistribution(distributionId);
```

**Use When**:

- Fetching server data
- Automatic caching and deduplication needed
- Loading/error states needed

---

### 4. useReducer for Complex Local State

Pages with complex, multi-step workflows use `useReducer`.

```typescript
const [state, dispatch] = useReducer(masterInquiryReducer, initialState);

// Reducer
const masterInquiryReducer = (state: State, action: Action): State => {
  switch (action.type) {
    case "SEARCH_START":
      return { ...state, search: { ...state.search, isLoading: true } };
    case "SEARCH_SUCCESS":
      return {
        ...state,
        search: { results: action.payload, isLoading: false }
      };
    case "SELECT_MEMBER":
      return { ...state, selection: { selectedMember: action.payload } };
    default:
      return state;
  }
};
```

**Use When**:

- State transitions are complex
- Multiple related pieces of state update together
- State logic is testable in isolation

---

### 5. Context Providers

Context for cross-component communication within a page.

```typescript
// Page wrapper
<MissiveAlertProvider>
  <PageContent />
</MissiveAlertProvider>;

// In any child component
const { addAlert, clearAlerts } = useMissiveAlerts();
```

**Common Contexts**:

- `MissiveAlertProvider`: Alert/message system
- Custom contexts for complex pages

---

## Data Fetching Patterns

### 1. Immediate Fetch on Mount

```typescript
const { data, isLoading, error } = useGetDataQuery();

// Data automatically fetched when component mounts
```

**Use When**: Data needed immediately on page load.

---

### 2. Lazy/Manual Fetch

```typescript
const [triggerFetch, { data, isLoading }] = useLazyGetDataQuery();

useEffect(() => {
  if (profitYear) {
    triggerFetch({ profitYear });
  }
}, [profitYear]);
```

**Use When**: Fetch should be triggered by user action or dependent on other data.

---

### 3. Dependent Fetches (Sequential)

```typescript
const selectMember = useCallback(async (member: Member) => {
  dispatch({ type: "SELECT_MEMBER", payload: member });

  // First fetch: member details
  const memberDetails = await triggerMemberDetails({ id: member.id }).unwrap();
  dispatch({ type: "MEMBER_DETAILS_SUCCESS", payload: memberDetails });

  // Second fetch: profit data (depends on member details)
  const profitData = await triggerProfitDetails({
    id: member.id,
    memberType: member.memberType
  }).unwrap();
  dispatch({ type: "PROFIT_DATA_SUCCESS", payload: profitData });
}, []);
```

**Use When**: Second fetch depends on result of first fetch.

---

### 4. Parallel Fetches

```typescript
useEffect(() => {
  Promise.all([triggerStates().unwrap(), triggerTaxCodes().unwrap(), triggerMissives().unwrap()]);
}, []);
```

**Use When**: Multiple independent data sources needed simultaneously.

---

### 5. Pagination & Sorting (Server-Side)

**CRITICAL**: All grids with pagination MUST use server-side pagination and sorting. Never load all data client-side.

#### Complete Setup Pattern

**Parent Component State:**

```typescript
import { SortParams } from "../../../hooks/useGridPagination";

const [pageNumber, setPageNumber] = useState(0); // 0-based for UI
const [pageSize, setPageSize] = useState(10); // Default page size
const [sortBy, setSortBy] = useState<string>("Created"); // Default sort column
const [isSortDescending, setIsSortDescending] = useState<boolean>(true); // Default sort order

const fetchData = useCallback(
  (page: number, size: number, sort?: string, desc?: boolean) => {
    triggerQuery(
      {
        pageNumber: page + 1, // API expects 1-based
        pageSize: size,
        sortBy: sort ?? sortBy,
        isSortDescending: desc ?? isSortDescending
      },
      false
    );
  },
  [triggerQuery, sortBy, isSortDescending]
);

const handlePageChange = (page: number, size: number) => {
  setPageNumber(page);
  setPageSize(size);
  fetchData(page, size);
};

const handleSortChange = (sortParams: SortParams) => {
  setSortBy(sortParams.sortBy);
  setIsSortDescending(sortParams.isSortDescending);
  setPageNumber(0); // Reset to first page on sort
  fetchData(0, pageSize, sortParams.sortBy, sortParams.isSortDescending);
};
```

**Child Grid Component Props:**

```typescript
interface GridProps {
  data?: PaginatedData;
  isLoading: boolean;
  pageNumber: number;
  pageSize: number;
  onPageChange: (page: number, pageSize: number) => void;
  onSortChange: (sortParams: SortParams) => void;
}

const Grid: React.FC<GridProps> = ({
  data,
  isLoading,
  pageNumber,
  pageSize,
  handlePageNumberChange,
  handlePageSizeChange,
  handleSortChange,
}) => {
  const columnDefs = useMemo(() => GetGridColumns(), []);

  return (
    <DSMPaginatedGrid
      preferenceKey="MY_GRID"
      isLoading={isLoading}
      onSortChange={handleSortChange}
      providedOptions={{
        rowData: data?.results || [],
        columnDefs: columnDefs,
      }}
      pagination={{
        pageNumber,
        pageSize,
        sortParams: { sortBy: "Created", isSortDescending: true },
        handlePageNumberChange,  // Pass directly - NO value - 1
        handlePageSizeChange,
        handleSortChange
      }}
      heightConfig={{
        mode: "content-aware",
        maxHeight: 600
      }}
      totalRecords={data?.total ?? 0}
      showPagination={!!data?.results?.length}
    />
  );
};
```

**RTK Query API Endpoint:**

```typescript
getData: builder.query<
  PaginatedData,
  {
    pageNumber?: number;
    pageSize?: number;
    sortBy?: string;
    isSortDescending?: boolean;
  }
>({
  query: ({ pageNumber = 1, pageSize = 10, sortBy = "Created", isSortDescending = true }) => ({
    url: "my-endpoint",
    method: "GET",
    params: { pageNumber, pageSize, sortBy, isSortDescending }
  })
});
```

#### Common Pagination Mistakes

1. **Missing `handleSortChanged`**: Grid won't trigger API calls on column header clicks
2. **Missing `onSortChange` prop**: Grid component can't pass sort events to parent
3. **Wrong page number base**: UI uses 0-based, API uses 1-based - must convert
4. **Not resetting page on sort**: Should always go to page 0 when sorting changes
5. **Client-side pagination**: Never load all records and paginate in browser
6. **Missing sort state**: Parent must track `sortBy` and `isSortDescending`

**Pattern**: Grid pagination/sort triggers new API call with updated parameters.

---

## Grid Column Factories (MANDATORY)

### Always Use Grid Column Factories

**NEVER manually define column objects**. Always use the grid column factory functions from `src/ui/src/utils/gridColumnFactory.ts`.

#### Why Use Factories?

1. **Consistent rendering**: Badge hyperlinks, date formatting, currency display
2. **Master Inquiry integration**: Badge columns automatically link to Master Inquiry
3. **Accessibility**: Standard tooltips, screen reader support
4. **Maintainability**: Change once, applies everywhere

---

## Common Components

### From `smart-ui-library`

1. **Page** - Top-level page wrapper
2. **DSMAccordion** - Collapsible sections
3. **DSMPaginatedGrid** - AG Grid wrapper with integrated pagination (use for paginated data)
4. **DSMGrid** - AG Grid wrapper without pagination (use for small, non-paginated datasets)
5. **SearchAndReset** - Standard search/reset button pair
6. **TotalsGrid** - Grid for displaying summary/totals
7. **SmartModal** - Modal dialog component
8. **Pagination** - Standalone pagination component (rarely needed - prefer DSMPaginatedGrid)
9. **ApiMessageAlert** - API error/success message display

### From `components/`

1. **StatusDropdownActionNode** - Page status dropdown
2. **MissiveAlerts** - Application alert display
3. **PrerequisiteGuard** - Ensures prerequisites before rendering page
4. **StatusDropdown** - Status dropdown (used by StatusDropdownActionNode)

### Utility Functions

1. **numberToCurrency** - Format numbers as currency
2. **formatNumberWithComma** - Format numbers with commas
3. **setMessage** - Dispatch message to message slice

---

## Route Path Patterns (CRITICAL)

### Route Constants Location

All route paths are defined in `src/ui/src/constants.ts`. When creating a new page:

1. **Add route constant** to constants.ts:

   ```typescript
   export const ORACLE_HCM_DIAGNOSTICS = "oracle-hcm-diagnostics"; // NO leading slash!
   ```

2. **NEVER include path prefixes** in route constants:

   ```typescript
   // ❌ WRONG - includes prefix, will break routing
   export const MY_PAGE = "/it-operations/my-page";

   // ✅ RIGHT - just the page path, no prefix
   export const MY_PAGE = "my-page";
   ```

3. **Use constant in RouterSubAssembly**:
   ```typescript
   <Route
     path={Navigation.Constants.ORACLE_HCM_DIAGNOSTICS}
     element={<LazyOracleHcmDiagnostics />}
   />
   ```
