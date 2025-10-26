# Page Component Architecture

## Overview

This document provides a detailed technical description of how page components are assembled in the Smart Profit Sharing UI application. The application follows a consistent, modular architecture where pages are composed from reusable building blocks and follow established patterns for state management, data fetching, and user interactions.

## Table of Contents

1. [Directory Structure](#directory-structure)
2. [Page Component Patterns](#page-component-patterns)
3. [Core Building Blocks](#core-building-blocks)
4. [File Organization](#file-organization)
5. [State Management Patterns](#state-management-patterns)
6. [Data Fetching Patterns](#data-fetching-patterns)
7. [Common Components](#common-components)
8. [Navigation Patterns](#navigation-patterns)
9. [Page Assembly Examples](#page-assembly-examples)

---

## Directory Structure

```
src/ui/src/pages/
├── AddDistribution/           # Distribution creation workflow
├── Adjustments/               # Adjustment forms
├── BeneficiaryInquiry/       # Beneficiary search and details
├── DecemberActivities/        # Year-end December operations
│   ├── DemographicBadgesNotInPayprofit/
│   ├── DistributionsAndForfeitures/
│   ├── DuplicateNamesAndBirthdays/
│   ├── DuplicateSSNsOnDemographics/
│   ├── Forfeit/
│   ├── MilitaryContribution/
│   ├── NegativeEtvaForSSNsOnPayprofit/
│   ├── Termination/
│   └── UnForfeit/
├── DistributionInquiry/      # Distribution search and management
├── Documentation/             # Application documentation viewer
├── EditDistribution/          # Distribution editing workflow
├── FiscalClose/               # Fiscal close operations
│   ├── AgeReports/
│   ├── EligibleEmployees/
│   ├── ForfeituresAdjustment/
│   ├── InfoCard.tsx
│   ├── PAY426Reports/
│   ├── PaymasterUpdate/
│   ├── Profall/
│   ├── ProfitShareByStore/
│   ├── ProfitShareEditUpdate/
│   ├── ProfitShareGrossReport/
│   ├── ProfitShareTotals426/
│   ├── QPAY066B/
│   ├── ReprintCertificates/
│   └── YTDWagesExtract/
├── FrozenSummary/            # Frozen data summary views
├── InquiriesAndAdjustments/  # Employee inquiries and adjustments
├── ITOperations/             # IT operations (demographic freeze, etc.)
├── MasterInquiry/            # Master inquiry (employee/beneficiary lookup)
├── Reports/                  # Reporting pages
│   ├── PROF130/
│   │   ├── BalanceByAge/
│   │   ├── BalanceByYears/
│   │   ├── ContributionsByAge/
│   │   ├── DistributionByAge/
│   │   ├── ForfeituresByAge/
│   │   └── VestedAmountsByAge/
│   ├── QPAY066AdHocReports/
│   ├── QPAY600/
│   └── RecentlyTerminated/
├── ProfitShareReport/        # Main profit share report
├── Unauthorized/             # Unauthorized access page
├── ViewDistribution/         # Distribution viewing and disbursement tracking
├── Distributions/            # Distribution management
└── Dev/                      # Development/testing pages
```

### Statistics

- **Total page directories**: 13 (organized thematically)
- **Total nested subdirectories**: 40+ components
- **Total page component files**: ~120+ `.tsx` files across all levels
- **Grid column definition files**: 50+ files

---

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
    <Page
      label={CAPTIONS.PROFALL}
      actionNode={renderActionNode()}>
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
  const [searchData, setSearchData] = useState<DistributionSearchRequest | null>(null);
  const [hasSearched, setHasSearched] = useState(false);
  const [triggerSearch, { data, isFetching }] = useLazySearchDistributionsQuery();
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
    <Page label={CAPTIONS.DISTRIBUTIONS_INQUIRY} actionNode={<StatusDropdownActionNode />}>
      <MissiveAlertProvider>
        <DistributionInquiryContent />
      </MissiveAlertProvider>
    </Page>
  );
};
```

**Key Characteristics**:

- Separates page wrapper (`DistributionInquiry`) from content (`DistributionInquiryContent`)
- Uses `MissiveAlertProvider` context for user messages
- Collapsible filter section using `DSMAccordion`
- Conditional grid display based on `hasSearched` state
- RTK Query lazy hooks for data fetching
- Action buttons (New Entry, Export, etc.)
- Error handling via missive alerts

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
    resetAll
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

**Key Characteristics**:

- Complex state managed via `useReducer` pattern
- Custom hook encapsulates all business logic
- Component focuses purely on rendering
- Multiple conditional sections (member grid, member details, profit details)
- Progressive disclosure pattern (select member → show details → show profit data)
- Memoized component for performance

---

### 4. Tabbed Page

**Pattern**: Multiple tabs/views within a single page.

**Example**: `FrozenSummary.tsx`

```typescript
const FrozenSummary = () => {
  const [selectedTab, setSelectedTab] = useState(0);

  const tabs = ["Summary", "Distributions", "Contributions", "Forfeitures", "Balance"];

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

**Key Characteristics**:

- Material-UI `Tabs` component for navigation
- Conditional rendering based on `selectedTab` state
- Different content for each tab
- Tab state managed locally

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
  const { profitSharingEdit, profitSharingUpdate } = useSelector((state: RootState) => state.yearsEnd);

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
      <Page label="PROFIT SHARE EDIT/UPDATE" actionNode={<StatusDropdownActionNode />}>
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

**Key Characteristics**:

- Uses `PrerequisiteGuard` to ensure required data is loaded
- Multiple RTK Query hooks (mutations and lazy queries)
- Complex state orchestration (Redux + local state)
- Custom hooks for cross-cutting concerns (`useChecksumValidation`, `useFiscalCloseProfitYear`)
- Confirmation modals for critical actions
- Validation alerts
- Multiple nested components (tabs, tables, grids)

---

## Core Building Blocks

### 1. Page Wrapper (`smart-ui-library`)

The `Page` component is the top-level container for all pages.

```typescript
<Page
  label="DISTRIBUTION INQUIRY"        // Page title (displayed in header)
  actionNode={<StatusDropdownActionNode />}  // Optional action area (right side of header)
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

### 3. DSMGrid (AG Grid Wrapper)

Standardized grid component wrapping AG Grid.

```typescript
<DSMGrid
  rowData={data?.results ?? []}
  columnDefs={columnDefs}
  pagination={true}
  paginationPageSize={25}
  onPaginationChanged={handlePaginationChange}
  onSortChanged={handleSortChange}
  loading={isLoading}
/>
```

**Features**:

- Automatic pagination
- Sorting
- Loading states
- Responsive design
- Consistent styling

---

### 4. Search/Filter Components

Pattern: Each page with filters has a dedicated filter component.

**File naming**: `{PageName}SearchFilter.tsx`

**Example**: `DistributionInquirySearchFilter.tsx`

```typescript
interface SearchFilterProps {
  onSearch: (formData: SearchFormData) => void;
  onReset: () => void;
  isLoading: boolean;
}

const DistributionInquirySearchFilter: React.FC<SearchFilterProps> = ({
  onSearch,
  onReset,
  isLoading
}) => {
  const [formData, setFormData] = useState<SearchFormData>(initialFormData);

  const handleSearch = () => {
    onSearch(formData);
  };

  return (
    <Grid container spacing={2}>
      {/* Form fields */}
      <TextField label="Badge Number" value={formData.badgeNumber} onChange={...} />
      <TextField label="SSN" value={formData.ssn} onChange={...} />

      {/* Action buttons */}
      <SearchAndReset onSearch={handleSearch} onReset={onReset} isSearching={isLoading} />
    </Grid>
  );
};
```

**Common Pattern**:

- Local state for form fields
- Callback props for `onSearch` and `onReset`
- `SearchAndReset` button component from `smart-ui-library`
- Validation logic (optional)

---

### 5. Grid Column Definitions

Pattern: Grid columns defined in separate files for reusability and testability.

**File naming**: `{PageName}GridColumns.ts`

**Example**: `ProfallGridColumns.ts`

```typescript
import { ColDef } from "ag-grid-community";
import { createBadgeColumn, createNameColumn, createStoreColumn } from "../../utils/gridColumnFactory";

export const GetProfallGridColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  return [
    createStoreColumn({ minWidth: 80, sortable: true }),
    createBadgeColumn({ headerName: "Badge", navigateFunction: navFunction }),
    createNameColumn({ field: "employeeName", minWidth: 180, sortable: true }),
    {
      headerName: "Department",
      field: "departmentName",
      colId: "departmentName",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    }
  ];
};
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
</MissiveAlertProvider>

// In page content:
const { missiveAlerts, addAlert, clearAlerts } = useMissiveAlerts();

// Display alerts:
{missiveAlerts.length > 0 && <MissiveAlerts />}

// Add alert:
addAlert({
  id: 911,
  severity: "success",
  message: "Distribution Saved Successfully",
  description: "Distribution for John Doe has been saved."
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

## File Organization

### Standard Page Directory Structure

```
PageName/
├── PageName.tsx                  # Main page component
├── PageNameGrid.tsx              # Grid component
├── PageNameGridColumns.ts        # Grid column definitions
├── PageNameSearchFilter.tsx      # Filter/search form component
├── PageNameDetails.tsx           # Detail view component (if applicable)
├── PageNameModal.tsx             # Modal/dialog components
├── hooks/
│   ├── usePageName.ts            # Custom hook encapsulating page logic
│   └── usePageNameReducer.ts    # Reducer for complex state management
└── utils/
    └── PageNameFunctions.ts      # Utility functions specific to page
```

### Example: DistributionInquiry

```
DistributionInquiry/
├── DistributionInquiry.tsx                 # Main page
├── DistributionInquiryGrid.tsx             # Grid
├── DistributionInquiryGridColumns.tsx      # Column definitions
├── DistributionInquirySearchFilter.tsx     # Search filter
├── DistributionActions.tsx                 # Row action menu component
├── NewEntryDialog.tsx                      # Dialog for new entries
└── DeleteDistributionModal.tsx             # Delete confirmation modal
```

### Example: MasterInquiry (Complex)

```
MasterInquiry/
├── MasterInquiry.tsx                       # Main page
├── MasterInquiryMemberGrid.tsx             # Member selection grid
├── MasterInquiryMemberGridColumns.tsx      # Member grid columns
├── MasterInquiryMemberDetails.tsx          # Member details section
├── MasterInquiryDetailsGrid.tsx            # Profit details grid
├── MasterInquiryGridColumns.ts             # Profit grid columns
├── MasterInquirySearchFilter.tsx           # Search filter
├── StandaloneMemberDetails.tsx             # Reusable member details component
├── hooks/
│   ├── useMasterInquiry.ts                 # Main logic hook
│   └── useMasterInquiryReducer.ts          # State reducer
└── utils/
    └── MasterInquiryFunctions.ts           # Utility functions
```

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
      return { ...state, search: { results: action.payload, isLoading: false } };
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
</MissiveAlertProvider>

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

### 5. Pagination & Sorting

```typescript
const handlePaginationChange = async (pageNumber: number, pageSize: number, sortParams: SortParams) => {
  if (searchData) {
    const request = {
      ...searchData,
      skip: pageNumber * pageSize,
      take: pageSize,
      sortBy: sortParams.sortBy,
      isSortDescending: sortParams.isSortDescending
    };
    await triggerSearch(request);
  }
};
```

**Pattern**: Grid pagination/sort triggers new API call with updated parameters.

---

## Common Components

### From `smart-ui-library`

1. **Page** - Top-level page wrapper
2. **DSMAccordion** - Collapsible sections
3. **DSMGrid** - AG Grid wrapper with pagination
4. **SearchAndReset** - Standard search/reset button pair
5. **TotalsGrid** - Grid for displaying summary/totals
6. **SmartModal** - Modal dialog component
7. **Pagination** - Standalone pagination component
8. **ApiMessageAlert** - API error/success message display

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

## Navigation Patterns

### 1. Programmatic Navigation

```typescript
import { useNavigate } from "react-router-dom";

const navigate = useNavigate();

// Navigate to a route
navigate("/distribution-inquiry");

// Navigate with state
navigate("/view-distribution", {
  state: {
    showSuccessMessage: true,
    memberName: "John Doe",
    amount: 5000
  }
});

// Navigate back
navigate(-1);
```

---

### 2. Navigation with URL Parameters

```typescript
// Navigate with params
navigate(`/view-distribution/${distributionId}?memberType=1&profitYear=2024`);

// Read params
const { distributionId } = useParams();
const [searchParams] = useSearchParams();
const memberType = searchParams.get("memberType");
```

---

### 3. Location State (Passing Data Between Pages)

```typescript
// From page A
navigate("/distribution-inquiry", {
  state: {
    showSuccessMessage: true,
    operationType: "added",
    memberName: "John Doe"
  }
});

// In page B
const location = useLocation();
const state = location.state as LocationState | undefined;

useEffect(() => {
  if (state?.showSuccessMessage) {
    addAlert({ message: "Success!", description: state.memberName });
    // Clear state after reading
    window.history.replaceState({}, document.title);
  }
}, [location]);
```

**Use Case**: Showing success messages after navigation (e.g., after saving a form).

---

### 4. Read-Only Navigation Guard

```typescript
import { useReadOnlyNavigation } from "../../hooks/useReadOnlyNavigation";

const isReadOnly = useReadOnlyNavigation();

<Button disabled={isReadOnly} onClick={handleEdit}>
  EDIT
</Button>
```

**Pattern**: Disable actions when user has read-only permissions.

---

## Page Assembly Examples

### Example 1: Simple Report Page

**File**: `QPAY600/QPAY600.tsx`

```typescript
const QPAY600 = () => {
  const profitYear = useFiscalCloseProfitYear();
  const [searchParams, setSearchParams] = useState<QPAY600Request | null>(null);
  const [triggerReport, { data, isFetching }] = useLazyGetQPAY600ReportQuery();

  const handleSearch = (params: QPAY600SearchParams) => {
    const request = { ...params, profitYear };
    setSearchParams(request);
    triggerReport(request);
  };

  return (
    <Page label="QPAY600 REPORT" actionNode={<StatusDropdownActionNode />}>
      <Grid container rowSpacing="24px">
        <Divider />

        <DSMAccordion title="Filter">
          <QPAY600FilterSection onSearch={handleSearch} />
        </DSMAccordion>

        {searchParams && (
          <QPAY600Grid data={data?.results ?? []} isLoading={isFetching} />
        )}
      </Grid>
    </Page>
  );
};
```

**Components**:

- `Page` wrapper with status dropdown
- `DSMAccordion` for filter
- `QPAY600FilterSection` (search form)
- `QPAY600Grid` (results grid)

---

### Example 2: Multi-Step Workflow

**File**: `AddDistribution/AddDistribution.tsx`

```typescript
const AddDistribution = () => {
  const navigate = useNavigate();
  const [step, setStep] = useState<"search" | "form" | "review">("search");
  const [selectedMember, setSelectedMember] = useState<Member | null>(null);
  const [formData, setFormData] = useState<DistributionFormData | null>(null);
  const [createDistribution, { isLoading }] = useCreateDistributionMutation();

  const handleMemberSelected = (member: Member) => {
    setSelectedMember(member);
    setStep("form");
  };

  const handleFormSubmit = (data: DistributionFormData) => {
    setFormData(data);
    setStep("review");
  };

  const handleConfirm = async () => {
    if (selectedMember && formData) {
      await createDistribution({
        memberId: selectedMember.id,
        ...formData
      }).unwrap();

      navigate("/distribution-inquiry", {
        state: {
          showSuccessMessage: true,
          memberName: selectedMember.name,
          amount: formData.amount
        }
      });
    }
  };

  return (
    <Page label="ADD DISTRIBUTION">
      {step === "search" && (
        <MemberSearchForm onMemberSelected={handleMemberSelected} />
      )}
      {step === "form" && (
        <DistributionForm
          member={selectedMember}
          onSubmit={handleFormSubmit}
          onBack={() => setStep("search")}
        />
      )}
      {step === "review" && (
        <DistributionReview
          member={selectedMember}
          data={formData}
          onConfirm={handleConfirm}
          onBack={() => setStep("form")}
          isLoading={isLoading}
        />
      )}
    </Page>
  );
};
```

**Pattern**: Multi-step wizard with state transitions.

---

### Example 3: Page with Tabs

**File**: `PROF130/PROF130.tsx`

```typescript
const PROF130 = () => {
  const [selectedReport, setSelectedReport] = useState("distributions");

  const reports = {
    distributions: <DistributionByAge />,
    contributions: <ContributionsByAge />,
    forfeitures: <ForfeituresByAge />,
    balanceByAge: <BalanceByAge />,
    balanceByYears: <BalanceByYears />,
    vestedAmounts: <VestedAmountsByAge />
  };

  return (
    <Page label="PROF130 REPORTS" actionNode={<StatusDropdownActionNode />}>
      <Grid container rowSpacing="24px">
        <Tabs value={selectedReport} onChange={(e, val) => setSelectedReport(val)}>
          <Tab value="distributions" label="Distributions by Age" />
          <Tab value="contributions" label="Contributions by Age" />
          <Tab value="forfeitures" label="Forfeitures by Age" />
          <Tab value="balanceByAge" label="Balance by Age" />
          <Tab value="balanceByYears" label="Balance by Years" />
          <Tab value="vestedAmounts" label="Vested Amounts by Age" />
        </Tabs>

        <Grid width="100%">
          {reports[selectedReport]}
        </Grid>
      </Grid>
    </Page>
  );
};
```

**Pattern**: Tab-based navigation within a single page.

---
