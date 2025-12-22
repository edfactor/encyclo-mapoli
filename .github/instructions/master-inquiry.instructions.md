---
name: "Master Inquiry Page - Technical Documentation"
applyTo: "src/ui/src/pages/InquiriesAndAdjustments/MasterInquiry/**/*.*"
paths: "src/ui/src/pages/InquiriesAndAdjustments/MasterInquiry/**/*.*"
---

# Master Inquiry Page - Technical Documentation

## Overview

The Master Inquiry page is a complex, multi-stage search and detail viewing interface for looking up employees and beneficiaries in the Demoulas Profit Sharing system. It demonstrates advanced patterns including:

- **Progressive disclosure**: Search → Member selection → Member details → Profit details
- **State machine architecture**: Uses reducer pattern with explicit view modes
- **Smart re-rendering**: Extensive use of `React.memo` and custom comparison functions
- **Duplicate request prevention**: Ref-based deduplication for API calls
- **URL parameter handling**: Deep-linking support via badge number in URL
- **Mutual exclusion fields**: SSN, Name, and Badge fields cannot be used simultaneously

This page serves as a reference implementation for complex search/detail workflows in the application.

---

## Architecture Overview

### Component Hierarchy

```
MasterInquiry (Page Wrapper)
└── MissiveAlertProvider (Context for alerts)
    └── MasterInquiryContent (Main content, memoized)
        ├── useMasterInquiry() [Custom Hook - State Management]
        ├── MasterInquirySearchFilter (Search form)
        ├── MasterInquiryMemberGrid (Multiple member results)
        ├── MasterInquiryMemberDetails (Selected member info)
        └── MasterInquiryDetailsGrid (Profit data grid)
```

### File Structure

```
src/ui/src/pages/MasterInquiry/
├── MasterInquiry.tsx                      # Main page component
├── MasterInquirySearchFilter.tsx          # Search form with validation
├── MasterInquiryMemberGrid.tsx            # Grid showing multiple members
├── MasterInquiryMemberGridColumns.tsx     # Column definitions for member grid
├── MasterInquiryMemberDetails.tsx         # Detailed member information
├── MasterInquiryDetailsGrid.tsx           # Grid showing profit details
├── MasterInquiryGridColumns.ts            # Column definitions for profit grid
├── StandaloneMemberDetails.tsx            # Reusable member details component
├── hooks/
│   ├── useMasterInquiry.ts                # Main state management hook
│   ├── useMasterInquiry.test.tsx          # Tests for main hook
│   ├── useMasterInquiryReducer.ts         # Reducer logic and types
│   └── useMasterInquiryReducer.test.tsx   # Tests for reducer
├── utils/
│   ├── MasterInquiryFunctions.ts          # Helper functions (PSN split, etc.)
│   ├── MasterInquiryFunctions.test.ts     # Tests for helpers
│   ├── transformSearchParams.ts           # Form-to-API transformation
│   └── transformSearchParams.test.ts      # Tests for transformation
└── Claude.md                              # Additional documentation
```

---

## State Management

### View Modes (State Machine)

The page operates as a state machine with four distinct modes:

```typescript
export type ViewMode =
  | "idle"
  | "searching"
  | "multipleMembers"
  | "memberDetails";
```

#### State Transitions

```
idle
  ↓ (user submits search)
searching
  ↓ (results received)
  ├─→ multipleMembers (2+ results) ─→ memberDetails (user selects one)
  └─→ memberDetails (1 result - auto-select)
```

#### View Mode Selectors

```typescript
// From useMasterInquiryReducer.ts

export const selectShowMemberGrid = (state: MasterInquiryState): boolean =>
  state.view.mode === "multipleMembers" &&
  Boolean(state.search.results && state.search.results.results.length > 1);

export const selectShowMemberDetails = (state: MasterInquiryState): boolean =>
  state.view.mode === "memberDetails" &&
  Boolean(state.selection.selectedMember);

export const selectShowProfitDetails = (state: MasterInquiryState): boolean =>
  state.view.mode === "memberDetails" &&
  Boolean(state.selection.selectedMember && state.selection.memberProfitData);
```

### Reducer State Shape

```typescript
export interface MasterInquiryState {
  search: {
    params: MasterInquiryRequest | null;
    results: SearchResponse | null;
    isSearching: boolean;
    isManuallySearching: boolean; // User-initiated vs pagination
    isFetchingMembers: boolean; // Separate loading for pagination
    noResultsMessage: string | null;
    error: string | null;
  };

  selection: {
    selectedMember: SelectedMember | null;
    memberDetails: MemberDetails | null;
    memberProfitData: ProfitData | null;
    isFetchingMemberDetails: boolean;
    isFetchingProfitData: boolean;
  };

  view: {
    mode: ViewMode;
  };
}
```

### Key Actions

```typescript
export type MasterInquiryAction =
  // Search actions
  | {
      type: "SEARCH_START";
      payload: { params: MasterInquiryRequest; isManual: boolean };
    }
  | { type: "SEARCH_SUCCESS"; payload: { results: SearchResponse } }
  | { type: "SEARCH_FAILURE"; payload: { error: string } }
  | { type: "SEARCH_RESET" }

  // Pagination actions (separate from search)
  | { type: "MEMBERS_FETCH_START" }
  | { type: "MEMBERS_FETCH_SUCCESS"; payload: { results: SearchResponse } }
  | { type: "MEMBERS_FETCH_FAILURE"; payload: { error: string } }

  // Selection actions
  | { type: "SELECT_MEMBER"; payload: { member: SelectedMember | null } }

  // Member details actions
  | { type: "MEMBER_DETAILS_FETCH_START" }
  | {
      type: "MEMBER_DETAILS_FETCH_SUCCESS";
      payload: { details: MemberDetails };
    }
  | { type: "MEMBER_DETAILS_FETCH_FAILURE" }

  // Profit data actions
  | { type: "PROFIT_DATA_FETCH_START" }
  | { type: "PROFIT_DATA_FETCH_SUCCESS"; payload: { profitData: ProfitData } }
  | { type: "PROFIT_DATA_FETCH_FAILURE" }

  // Utility actions
  | { type: "SET_NO_RESULTS_MESSAGE"; payload: { message: string | null } }
  | { type: "SET_VIEW_MODE"; payload: { mode: ViewMode } }
  | { type: "RESET_ALL" };
```

---

## Search Flow

### 1. Search Filter Component

**File**: `MasterInquirySearchFilter.tsx`

#### Key Features

- **React Hook Form** with **yup** validation
- **Mutual exclusion**: SSN, Name, Badge fields disable each other
- **Auto-member-type detection**: Badge length determines Employee vs Beneficiary
- **URL parameter handling**: Processes badge number from route params
- **Redux persistence**: Saves search params to Redux for navigation breadcrumbs

#### Form Schema

```typescript
const schema = yup.object().shape({
  endProfitYear: profitYearNullableValidator.test(
    "greater-than-start",
    "End year must be after start year",
    function (endYear) {
      const startYear = this.parent.startProfitYear;
      return !startYear || !endYear || endYear >= startYear;
    },
  ),
  startProfitMonth: monthValidator,
  endProfitMonth: monthValidator.min(
    yup.ref("startProfitMonth"),
    "End month must be after start month",
  ),
  socialSecurity: ssnValidator,
  name: yup.string().nullable(),
  badgeNumber: badgeNumberOrPSNValidator,
  paymentType: yup
    .string()
    .oneOf(["all", "hardship", "payoffs", "rollovers"])
    .default("all"),
  memberType: yup
    .string()
    .oneOf(["all", "employees", "beneficiaries", "none"])
    .default("all"),
  contribution: positiveNumberValidator("Contribution"),
  earnings: positiveNumberValidator("Earnings"),
  forfeiture: positiveNumberValidator("Forfeiture"),
  payment: positiveNumberValidator("Payment"),
  voids: yup.boolean().default(false),
});
```

#### Mutual Exclusion Logic

```typescript
// Watch the three mutually exclusive fields
const socialSecurityValue = useWatch({ control, name: "socialSecurity" });
const nameValue = useWatch({ control, name: "name" });
const badgeNumberWatchValue = useWatch({ control, name: "badgeNumber" });

// Determine which fields should be disabled
const hasSocialSecurity = hasValue(socialSecurityValue);
const hasName = hasValue(nameValue);
const hasBadgeNumber = hasValue(badgeNumberWatchValue);

const isSocialSecurityDisabled = hasName || hasBadgeNumber;
const isNameDisabled = hasSocialSecurity || hasBadgeNumber;
const isBadgeNumberDisabled = hasSocialSecurity || hasName;
```

**Helper Text Example**:

- If SSN field has value → Name and Badge show: "Disabled: SSN field is in use. Press Reset to clear and re-enable."

#### Badge Number Auto-Detection

```typescript
const handleBadgeNumberChange = useCallback(
  (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const badgeStr = e.target.value;
    let memberType: string;

    if (badgeStr.length === 0) {
      memberType = "all";
    } else if (badgeStr.length >= 8) {
      memberType = "beneficiaries"; // PSN numbers are longer
    } else {
      memberType = "employees"; // Regular badge numbers
    }

    setValue(
      "memberType",
      memberType as "all" | "employees" | "beneficiaries" | "none",
    );
  },
  [setValue],
);
```

#### URL Parameter Handling

```typescript
const { badgeNumber } = useParams<{ badgeNumber: string }>();
const urlSearchProcessedRef = useRef(false);

useEffect(() => {
  if (badgeNumber && !urlSearchProcessedRef.current) {
    urlSearchProcessedRef.current = true;

    const formData = {
      ...schema.getDefault(),
      memberType: determineCorrectMemberType(badgeNumber),
      badgeNumber: Number(badgeNumber),
      endProfitYear: profitYear,
      pagination: {
        skip: 0,
        take: 5,
        sortBy: "badgeNumber",
        isSortDescending: true,
      },
    };

    reset(formData);
    const searchParams = transformSearchParams(formData, profitYear);
    onSearch(searchParams);

    // Remove badge number from URL after consuming it
    navigate(`/${ROUTES.MASTER_INQUIRY}`, { replace: true });
  }
}, [badgeNumber, reset, profitYear, onSearch, navigate]);
```

**Purpose**: Allows deep-linking like `/master-inquiry/123456` which auto-searches for badge 123456.

#### Search Button Enablement

Search button is enabled only when:

1. Form is valid (no validation errors)
2. At least one search criterion is provided (any field has value OR non-default memberType/paymentType)
3. Not currently searching

```typescript
const hasSearchCriteria = useMemo(
  () => {
    const hasFieldValues =
      hasValue(watchedBadgeNumber) ||
      hasValue(watchedStartProfitMonth) ||
      hasValue(watchedEndProfitMonth) ||
      hasValue(watchedSocialSecurity) ||
      hasValue(watchedName) ||
      hasValue(watchedContribution) ||
      hasValue(watchedEarnings) ||
      hasValue(watchedForfeiture) ||
      hasValue(watchedPayment);

    const hasNonDefaultSelections =
      watchedMemberType !== "all" || watchedPaymentType !== "all";

    return hasFieldValues || hasNonDefaultSelections;
  },
  [
    /* dependencies */
  ]
);

// In render
<SearchAndReset
  handleSearch={validateAndSearch}
  handleReset={handleReset}
  isFetching={isSearching}
  disabled={!isValid || isSearching || !hasSearchCriteria}
/>;
```

### 2. Parameter Transformation

**File**: `utils/transformSearchParams.ts`

Transforms form data (UI shape) into API request shape.

```typescript
export const transformSearchParams = (
  data: MasterInquirySearch,
  profitYear: number,
): MasterInquiryRequest => {
  // Split PSN if needed (badge numbers >7 chars are PSN+suffix)
  const { psnSuffix, verifiedBadgeNumber } = splitFullPSN(
    data.badgeNumber?.toString(),
  );

  return {
    pagination: {
      skip: data.pagination?.skip || 0,
      take: data.pagination?.take || 5,
      sortBy: data.pagination?.sortBy || "badgeNumber",
      isSortDescending: data.pagination?.isSortDescending ?? true,
    },
    endProfitYear: data.endProfitYear ?? profitYear,
    ...(!!data.startProfitMonth && { startProfitMonth: data.startProfitMonth }),
    ...(!!data.endProfitMonth && { endProfitMonth: data.endProfitMonth }),
    ...(!!data.socialSecurity && { ssn: Number(data.socialSecurity) }),
    ...(!!data.name && { name: data.name }),
    ...(verifiedBadgeNumber !== undefined && {
      badgeNumber: verifiedBadgeNumber,
    }),
    ...(psnSuffix !== undefined && { psnSuffix }),
    ...(!!data.paymentType && {
      paymentType: paymentTypeGetNumberMap[data.paymentType],
    }),
    ...(!!data.memberType && {
      memberType: memberTypeGetNumberMap[data.memberType],
    }),
    ...(!!data.contribution && { contributionAmount: data.contribution }),
    ...(!!data.earnings && { earningsAmount: data.earnings }),
    ...(!!data.forfeiture && { forfeitureAmount: data.forfeiture }),
    ...(!!data.payment && { paymentAmount: data.payment }),
    _timestamp: Date.now(),
  };
};
```

**Key Transformations**:

- **PSN splitting**: Badge numbers >7 digits split into `badgeNumber` + `psnSuffix`
- **String to number mappings**: `paymentType: "hardship"` → `paymentType: 1`
- **Conditional inclusion**: Only include fields that have values (using spread with conditional)
- **Timestamp**: Adds timestamp to bust caching if needed

### 3. Search Execution (useMasterInquiry Hook)

**File**: `hooks/useMasterInquiry.ts`

```typescript
const executeSearch = useCallback(
  async (params: MasterInquiryRequest) => {
    try {
      // Deduplication: Skip if same params and already searching
      const currentParamsString = JSON.stringify({
        badge: params.badgeNumber,
        ssn: params.ssn,
        name: params.name,
        profitYear: params.profitYear,
        endProfitYear: params.endProfitYear,
        startProfitMonth: params.startProfitMonth,
        endProfitMonth: params.endProfitMonth,
        memberType: params.memberType,
        paymentType: params.paymentType,
      });

      if (
        lastSearchParamsRef.current === currentParamsString &&
        state.search.isSearching
      ) {
        console.log("[useMasterInquiry] Skipping duplicate executeSearch call");
        return;
      }

      lastSearchParamsRef.current = currentParamsString;
      dispatch({ type: "SEARCH_START", payload: { params, isManual: true } });
      clearAlerts();

      // Minimum 300ms loading state (UX improvement - prevents flash)
      const [response] = await Promise.all([
        triggerSearch(params).unwrap(),
        new Promise((resolve) => setTimeout(resolve, 300)),
      ]);

      if (
        response &&
        (Array.isArray(response)
          ? response.length > 0
          : response.results?.length > 0)
      ) {
        const results = Array.isArray(response) ? response : response.results;
        const total = Array.isArray(response)
          ? response.length
          : response.total;

        dispatch({
          type: "SEARCH_SUCCESS",
          payload: { results: { results, total } },
        });
      } else {
        // No results - dispatch success with empty array
        dispatch({
          type: "SEARCH_SUCCESS",
          payload: { results: { results: [], total: 0 } },
        });

        // Show appropriate "not found" message based on search type
        const isSimple = isSimpleSearch(searchFormData);
        const isBeneficiarySearch =
          masterInquiryRequestParams?.memberType === "beneficiaries";

        let alertMessage;
        if (isSimple && isBeneficiarySearch) {
          alertMessage = MASTER_INQUIRY_MESSAGES.BENEFICIARY_NOT_FOUND;
        } else if (isSimple) {
          alertMessage = MASTER_INQUIRY_MESSAGES.MEMBER_NOT_FOUND;
        } else {
          alertMessage = MASTER_INQUIRY_MESSAGES.NO_RESULTS_FOUND;
        }

        addAlert(alertMessage);
      }
    } catch (error) {
      console.error("Search failed:", error);
      dispatch({
        type: "SEARCH_FAILURE",
        payload: { error: error?.toString() || "Unknown error" },
      });
      addAlert({
        id: 999,
        severity: "Error",
        message: "Search Failed",
        description: "The search request failed. Please try again.",
      } as MissiveResponse);
    }
  },
  [triggerSearch, masterInquiryRequestParams, clearAlerts],
);
```

**Key Features**:

- **Deduplication**: Prevents duplicate API calls using ref-based comparison
- **Minimum loading time**: Always shows spinner for at least 300ms (prevents flash for fast queries)
- **Smart error messages**: Different messages for simple searches vs complex searches
- **Type safety**: Handles API responses that may be array or object with `.results` property

---

## Member Selection & Details

### Member Grid (Multiple Results)

**File**: `MasterInquiryMemberGrid.tsx`

Shown when search returns 2+ members.

#### Grid Features

```typescript
<DSMGrid
  preferenceKey="MASTER_INQUIRY_MEMBER_GRID"
  handleSortChanged={handleSortChange}
  isLoading={isLoading}
  providedOptions={{
    rowData: searchResults.results,
    columnDefs: columns,
    context: { onBadgeClick: handleMemberClick },
    onRowClicked: (event) => {
      if (event.data) {
        handleMemberClick(event.data);
      }
    },
  }}
/>
```

**Interaction**:

- Click anywhere on row → Select member
- Click badge link → Select member (same action)

#### Pagination

```typescript
<Pagination
  rowsPerPageOptions={[5, 10, 50]}
  pageNumber={memberGridPagination.pageNumber}
  setPageNumber={(value: number) => {
    handlePaginationChange(value - 1, memberGridPagination.pageSize);
  }}
  pageSize={memberGridPagination.pageSize}
  setPageSize={(value: number) => {
    handlePaginationChange(0, value); // Reset to page 0 on size change
  }}
  recordCount={searchResults.total}
/>
```

**Default**: 5 rows per page (good for member selection UX - not overwhelming)

#### Member Selection Handler

```typescript
const handleMemberClick = (member: EmployeeDetails) => {
  onMemberSelect({
    memberType: member.isEmployee ? 1 : 2,
    id: Number(member.id),
    ssn: Number(member.ssn),
    badgeNumber: Number(member.badgeNumber),
    psnSuffix: Number(member.psnSuffix),
  });
};
```

**Effect**: Triggers state transition to `memberDetails` view mode.

### Member Details Fetch

**File**: `hooks/useMasterInquiry.ts`

When a member is selected, two automatic fetches occur:

#### 1. Member Details Fetch

```typescript
useEffect(() => {
  const currentMember = state.selection.selectedMember;
  if (!currentMember?.memberType || !currentMember?.id) {
    return;
  }

  // Deduplication: Skip if we just called with same member
  if (
    lastMemberDetailsCallRef.current?.memberType === currentMember.memberType &&
    lastMemberDetailsCallRef.current?.id === currentMember.id
  ) {
    console.log("[useMasterInquiry] Skipping duplicate member details fetch");
    return;
  }

  lastMemberDetailsCallRef.current = {
    memberType: currentMember.memberType,
    id: currentMember.id,
  };

  dispatch({ type: "MEMBER_DETAILS_FETCH_START" });
  triggerMemberDetails({
    memberType: currentMember.memberType,
    id: currentMember.id,
    profitYear: state.search.params?.endProfitYear,
  })
    .unwrap()
    .then((details) => {
      dispatch({ type: "MEMBER_DETAILS_FETCH_SUCCESS", payload: { details } });

      // Process missives (alerts) from response
      if (details.missives && details.missives.length > 0) {
        if (Array.isArray(missives) && missives.length > 0) {
          const localMissives: MissiveResponse[] = details.missives
            .map((id: number) =>
              missives.find((m: MissiveResponse) => m.id === id),
            )
            .filter(Boolean) as MissiveResponse[];

          if (localMissives.length > 0) {
            addAlerts(localMissives);
          }
        }
      }

      // Special alert: If searching "all" but found beneficiary
      if (
        !details.isEmployee &&
        masterInquiryRequestParams?.memberType === "all"
      ) {
        addAlert(MASTER_INQUIRY_MESSAGES.BENEFICIARY_FOUND(details.ssn));
      }
    })
    .catch(() => {
      dispatch({ type: "MEMBER_DETAILS_FETCH_FAILURE" });
    });
}, [
  state.selection.selectedMember,
  state.search.params?.endProfitYear,
  triggerMemberDetails,
]);
```

**Missive Processing**: API returns array of missive IDs (e.g., `[1, 5, 12]`). Hook cross-references with Redux store's missive definitions to get full alert objects, then displays them.

#### 2. Profit Details Fetch

```typescript
useEffect(() => {
  if (!profitFetchDeps.memberType || !profitFetchDeps.id) {
    return;
  }

  // Skip if we just called with same member (pagination changes handled separately)
  if (
    lastProfitDetailsCallRef.current?.memberType ===
      profitFetchDeps.memberType &&
    lastProfitDetailsCallRef.current?.id === profitFetchDeps.id
  ) {
    console.log("[useMasterInquiry] Skipping duplicate profit details fetch");
    return;
  }

  lastProfitDetailsCallRef.current = {
    memberType: profitFetchDeps.memberType,
    id: profitFetchDeps.id,
  };

  dispatch({ type: "PROFIT_DATA_FETCH_START" });
  triggerProfitDetails({
    memberType: profitFetchDeps.memberType,
    id: profitFetchDeps.id,
    skip: profitFetchDeps.pageNumber * profitFetchDeps.pageSize,
    take: profitFetchDeps.pageSize,
    sortBy: profitFetchDeps.sortBy,
    isSortDescending: profitFetchDeps.isSortDescending,
  })
    .unwrap()
    .then((profitData) => {
      dispatch({ type: "PROFIT_DATA_FETCH_SUCCESS", payload: { profitData } });
    })
    .catch(() => {
      dispatch({ type: "PROFIT_DATA_FETCH_FAILURE" });
    });
}, [profitFetchDeps, triggerProfitDetails]);
```

**Dependency Object Pattern**:

```typescript
const profitFetchDeps = useMemo(
  () => ({
    memberType: state.selection.selectedMember?.memberType,
    id: state.selection.selectedMember?.id,
    pageNumber: profitGridPagination.pageNumber,
    pageSize: profitGridPagination.pageSize,
    sortBy: profitGridPagination.sortParams.sortBy,
    isSortDescending: profitGridPagination.sortParams.isSortDescending,
  }),
  [
    /* dependencies */
  ],
);
```

**Why**: Ensures profit details refetch when pagination changes, but deduplicates initial member selection.

### Member Details Component

**File**: `MasterInquiryMemberDetails.tsx`

Displays detailed member information in four columns.

#### Layout Structure

```typescript
<Grid container paddingX="24px" width={"100%"}>
  <Grid size={{ xs: 12 }}>
    <Typography variant="h2">Member Details</Typography>
  </Grid>

  <Grid size={{ xs: 12 }}>
    <Grid container spacing={3}>
      <Grid size={{ xs: 12, sm: 6, md: 3 }}>
        <LabelValueSection data={summarySection} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 3 }}>
        <LabelValueSection data={personalSection} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 3 }}>
        <LabelValueSection data={milestoneSection} />
      </Grid>
      <Grid size={{ xs: 12, sm: 6, md: 3 }}>
        <LabelValueSection data={planSection} />
      </Grid>
    </Grid>
  </Grid>

  {/* Missive Alerts */}
  {missiveAlerts.length > 0 && (
    <Grid size={{ xs: 12 }}>
      <MissiveAlerts />
    </Grid>
  )}
</Grid>
```

#### Four Data Sections

**1. Summary Section** (Name, Address, Phone, Store, Enrollment Status)

```typescript
const summarySection = useMemo(() => {
  if (!memberDetails) return [];
  const {
    firstName,
    lastName,
    address,
    addressCity,
    addressState,
    addressZipCode,
    phoneNumber,
    isEmployee,
    workLocation,
    storeNumber,
  } = memberDetails;

  const formattedZip = addressZipCode ? formatZipCode(addressZipCode) : "";
  const cityStateZip =
    [formattedCity, formattedState].filter(Boolean).join(", ") +
    (formattedZip ? ` ${formattedZip}` : "");

  return [
    { label: "Name", value: `${lastName}, ${firstName}` },
    { label: "Address", value: `${address}` },
    { label: "", value: cityStateZip },
    { label: "Phone #", value: formatPhoneNumber(phoneNumber) },
    ...(isEmployee
      ? [{ label: "Work Location", value: workLocation || "N/A" }]
      : []),
    ...(isEmployee
      ? [{ label: "Store", value: storeNumber > 0 ? storeNumber : "N/A" }]
      : []),
    {
      label: "Enrolled",
      value: enrollmentStatus.enrolled.replace(/\s*\(\d+\)/, ""),
    },
    {
      label: "Forfeited",
      value: enrollmentStatus.forfeited.replace(/\s*\(\d+\)/, ""),
    },
  ];
}, [memberDetails, enrollmentStatus]);
```

**2. Personal Section** (Badge/PSN, Department, Class, Gender, DOB, SSN, Allocations)

```typescript
const personalSection = useMemo(() => {
  // Badge vs PSN based on isEmployee
  ...(isEmployee ? [{ label: "Badge", value: viewBadgeLinkRenderer(badgeNumber) }] : []),
  ...(!isEmployee ? [{ label: "PSN", value: viewBadgeLinkRenderer(badgeNumber, psnSuffix) }] : []),

  // Duplicate SSN detection
  if (badgesOfDuplicateSsns && badgesOfDuplicateSsns.length) {
    for (const badge of badgesOfDuplicateSsns) {
      duplicateBadgeLink.push({
        label: "Duplicate SSN with",
        value: viewBadgeLinkRenderer(badge),
        labelColor: "error"  // Red text
      });
    }
  }

  // CRITICAL: DO NOT CALCULATE AGE IN FRONTEND
  // Age must be provided by backend and never calculated in the frontend because:
  // 1. Frontend calculation will be inconsistent with backend (timezone, calculation timing)
  // 2. Age is sensitive data that must be masked for unprivileged users
  // 3. Backend has authoritative date references for age calculation
  const dobDisplay = dateOfBirth ? mmDDYYFormat(dateOfBirth) : "N/A";

  return [
    /* badge/psn fields */,
    { label: "DOB", value: dobDisplay },
    { label: "SSN", value: `${ssnValue}` },
    ...duplicateBadgeLink,
    { label: "Allocation To", value: numberToCurrency(allocationToAmount) }
  ];
}, [memberDetails]);
```

**3. Milestone Section** (Hire Date, Termination, Re-Hire, ETVA)

```typescript
const milestoneSection = useMemo(() => {
  return [
    ...(isEmployee
      ? [
          {
            label: "Hire Date",
            value: hireDate ? mmDDYYFormat(hireDate) : "N/A",
          },
        ]
      : []),
    ...(isEmployee
      ? [
          {
            label: "Full Time Date",
            value: fullTimeDate ? mmDDYYFormat(fullTimeDate) : "N/A",
          },
        ]
      : []),
    ...(isEmployee
      ? [
          {
            label: "Termination Date",
            value: terminationDate ? mmDDYYFormat(terminationDate) : "N/A",
          },
        ]
      : []),
    ...(isEmployee
      ? [{ label: "Termination Reason", value: terminationReason || "N/A" }]
      : []),
    ...(isEmployee
      ? [
          {
            label: "Re-Hire Date",
            value: reHireDate ? mmDDYYFormat(reHireDate) : "N/A",
          },
        ]
      : []),
    { label: "ETVA", value: numberToCurrency(currentEtva) },
    { label: "Allocation From", value: numberToCurrency(allocationFromAmount) },
  ];
}, [memberDetails]);
```

**4. Plan Section** (Balances, Vesting, Contributions)

```typescript
const planSection = useMemo(() => {
  const yearLabel =
    profitYear == new Date().getFullYear() ? "Current" : `End ${profitYear}`;

  // Current vested balance highlighted in bold blue (per PS-1897)
  const formattedCurrentVested =
    currentVestedAmount == null ? (
      "N/A"
    ) : (
      <Typography
        component="span"
        variant="body2"
        sx={{ fontWeight: "bold", color: "#0258A5" }}
      >
        {numberToCurrency(currentVestedAmount)}
      </Typography>
    );

  return [
    {
      label: "Begin Balance",
      value: beginPSAmount == null ? "N/A" : numberToCurrency(beginPSAmount),
    },
    {
      label: "Begin Vested Balance",
      value:
        beginVestedAmount == null ? "N/A" : numberToCurrency(beginVestedAmount),
    },
    {
      label: `${yearLabel} Balance`,
      value:
        currentPSAmount == null ? "N/A" : numberToCurrency(currentPSAmount),
    },
    { label: `${yearLabel} Vested Balance`, value: formattedCurrentVested },
    ...(isEmployee
      ? [
          {
            label: "Profit Sharing Hours",
            value: formatNumberWithComma(yearToDateProfitSharingHours),
          },
        ]
      : []),
    ...(isEmployee ? [{ label: "Years In Plan", value: yearsInPlan }] : []),
    { label: "Vested Percent", value: formatPercentage(percentageVested) },
    {
      label: "Contributions in Last Year",
      value:
        receivedContributionsLastYear == null
          ? "N/A"
          : receivedContributionsLastYear
          ? "Y"
          : "N",
    },
  ];
}, [memberDetails, profitYear]);
```

**Conditional Fields**: Employee-only fields (Hire Date, Store, etc.) use spread operator with conditional arrays:

```typescript
...(isEmployee ? [{ label: "Store", value: storeNumber }] : [])
```

**Result**: Beneficiaries get a condensed view; employees get full employment history.

### Profit Details Grid

**File**: `MasterInquiryDetailsGrid.tsx`

Shows year-by-year profit sharing data for the selected member.

```typescript
<DSMGrid
  preferenceKey={CAPTIONS.MASTER_INQUIRY}
  handleSortChanged={handleSortChange}
  isLoading={!!isLoading}
  maxHeight={gridMaxHeight}
  providedOptions={{
    rowData: profitData.results,
    columnDefs: columnDefs,
    suppressMultiSort: true,
    rowSelection: {
      mode: "multiRow",
      checkboxes: false,
      headerCheckbox: false,
      enableClickSelection: false,
    },
  }}
/>
```

**Pagination**: 25 rows per page (default for detail grids)

**Dynamic Height**: Uses `useDynamicGridHeight()` hook to maximize vertical space.

---

## Performance Optimizations

### 1. Memoization Strategy

All major components use `React.memo` with custom comparison functions:

```typescript
const MasterInquiryContent = memo(() => {
  // Component logic
});

const MasterInquiryMemberGrid = memo(
  ({
    searchResults,
    onMemberSelect,
    memberGridPagination,
    onPaginationChange,
    onSortChange,
    isLoading,
  }) => {
    // Component logic
  },
  (prevProps, nextProps) => {
    // Custom comparison - only re-render if these actually change
    return (
      prevProps.searchResults.results === nextProps.searchResults.results &&
      prevProps.searchResults.total === nextProps.searchResults.total &&
      prevProps.memberGridPagination.pageNumber ===
        nextProps.memberGridPagination.pageNumber &&
      prevProps.memberGridPagination.pageSize ===
        nextProps.memberGridPagination.pageSize &&
      prevProps.memberGridPagination.sortParams ===
        nextProps.memberGridPagination.sortParams &&
      prevProps.isLoading === nextProps.isLoading &&
      prevProps.onMemberSelect === nextProps.onMemberSelect &&
      prevProps.onPaginationChange === nextProps.onPaginationChange &&
      prevProps.onSortChange === nextProps.onSortChange
    );
  },
);
```

**Why**: Prevents unnecessary re-renders when parent state changes but component props haven't actually changed.

### 2. Duplicate Request Prevention

Uses refs to track last API call parameters:

```typescript
const lastSearchParamsRef = useRef<string | null>(null);
const lastMemberDetailsCallRef = useRef<{
  memberType: number;
  id: number;
} | null>(null);
const lastProfitDetailsCallRef = useRef<{
  memberType: number;
  id: number;
} | null>(null);

// In executeSearch
const currentParamsString = JSON.stringify({
  /* search params */
});
if (
  lastSearchParamsRef.current === currentParamsString &&
  state.search.isSearching
) {
  console.log("[useMasterInquiry] Skipping duplicate executeSearch call");
  return;
}
lastSearchParamsRef.current = currentParamsString;
```

**Why**: React 18's concurrent mode can cause double-rendering in development. Refs ensure we don't make duplicate API calls even if the hook runs twice.

### 3. Computed Data with useMemo

Member details sections are computed with `useMemo`:

```typescript
const summarySection = useMemo(() => {
  if (!memberDetails) return [];
  // Compute summary data
  return [
    /* array of label/value pairs */
  ];
}, [memberDetails, enrollmentStatus]);
```

**Why**: Prevents recomputing formatted data on every render. Only recalculates when dependencies change.

### 4. Pagination Object Memoization

```typescript
const profitFetchDeps = useMemo(
  () => ({
    memberType: state.selection.selectedMember?.memberType,
    id: state.selection.selectedMember?.id,
    pageNumber: profitGridPagination.pageNumber,
    pageSize: profitGridPagination.pageSize,
    sortBy: profitGridPagination.sortParams.sortBy,
    isSortDescending: profitGridPagination.sortParams.isSortDescending,
  }),
  [
    /* dependencies */
  ],
);
```

**Why**: Object identity stability. Without this, the dependency object would be recreated on every render, causing the effect to run unnecessarily.

---

## Pagination & Sorting

### Two Independent Pagination Systems

1. **Member Grid Pagination** (search results)
2. **Profit Grid Pagination** (profit details)

Both use the `useGridPagination` hook:

```typescript
const memberGridPagination = useGridPagination({
  initialPageSize: 5,
  initialSortBy: "badgeNumber",
  initialSortDescending: true,
  onPaginationChange: handleMemberGridPaginationChange,
});

const profitGridPagination = useGridPagination({
  initialPageSize: 25,
  initialSortBy: "profitYear",
  initialSortDescending: true,
  onPaginationChange: handleProfitGridPaginationChange,
});
```

### Pagination Change Handlers

#### Member Grid Pagination

```typescript
const handleMemberGridPaginationChange = useCallback(
  (pageNumber: number, pageSize: number, sortParams: SortParams) => {
    const currentSearchParams = searchParamsRef.current;
    if (currentSearchParams) {
      dispatch({ type: "MEMBERS_FETCH_START" });
      triggerSearch({
        ...currentSearchParams,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending,
        },
      })
        .unwrap()
        .then((response: SearchResponse | SearchResponse["results"]) => {
          const results = Array.isArray(response) ? response : response.results;
          const total = Array.isArray(response)
            ? response.length
            : response.total;
          dispatch({
            type: "MEMBERS_FETCH_SUCCESS",
            payload: { results: { results, total } },
          });
        })
        .catch((error: Error) => {
          dispatch({
            type: "MEMBERS_FETCH_FAILURE",
            payload: { error: error?.toString() || "Unknown error" },
          });
        });
    }
  },
  [triggerSearch],
);
```

**Key**: Uses `MEMBERS_FETCH_*` actions (separate from `SEARCH_*`) so pagination doesn't clear selected member.

#### Profit Grid Pagination

```typescript
const handleProfitGridPaginationChange = useCallback(
  (pageNumber: number, pageSize: number, sortParams: SortParams) => {
    const currentSelectedMember = selectedMemberRef.current;
    if (currentSelectedMember?.memberType && currentSelectedMember?.id) {
      triggerProfitDetails({
        memberType: currentSelectedMember.memberType,
        id: currentSelectedMember.id,
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending,
      })
        .unwrap()
        .then((profitData) => {
          dispatch({
            type: "PROFIT_DATA_FETCH_SUCCESS",
            payload: { profitData },
          });
        });
    }
  },
  [triggerProfitDetails],
);
```

**Key**: Uses ref (`selectedMemberRef.current`) to avoid stale closure issues.

---

## Error Handling & User Feedback

### Missive Alert System

**Context Provider**: `MissiveAlertProvider` wraps page content

**Hook**: `useMissiveAlerts()`

```typescript
const { missiveAlerts, addAlert, addAlerts, clearAlerts } = useMissiveAlerts();

// Single alert
addAlert({
  id: 911,
  severity: "Error",
  message: "Member Not Found",
  description: "No member found with the provided badge number.",
});

// Multiple alerts (from API response)
addAlerts(localMissives);

// Clear all alerts
clearAlerts();
```

### Alert Display

```typescript
{
  missiveAlerts.length > 0 && <MissiveAlerts />;
}
```

**Location**: Displays at top of page (in search filter accordion area) and at bottom of member details.

### No Results Messages

Three different messages based on search type:

```typescript
const isSimple = isSimpleSearch(searchFormData);
const isBeneficiarySearch =
  masterInquiryRequestParams?.memberType === "beneficiaries";

let alertMessage;
if (isSimple && isBeneficiarySearch) {
  alertMessage = MASTER_INQUIRY_MESSAGES.BENEFICIARY_NOT_FOUND;
} else if (isSimple) {
  alertMessage = MASTER_INQUIRY_MESSAGES.MEMBER_NOT_FOUND;
} else {
  alertMessage = MASTER_INQUIRY_MESSAGES.NO_RESULTS_FOUND;
}

addAlert(alertMessage);
```

**Simple Search**: Badge, SSN, or Name only (no amount filters)
**Complex Search**: Any amount filters (contribution, earnings, etc.)

---

## Testing

### Test Coverage

```
MasterInquiry/
├── hooks/
│   ├── useMasterInquiry.test.tsx              # Hook integration tests
│   └── useMasterInquiryReducer.test.tsx       # Reducer unit tests
├── utils/
│   ├── MasterInquiryFunctions.test.ts         # Helper function tests
│   └── transformSearchParams.test.ts          # Transformation tests
├── MasterInquiryMemberDetails.test.tsx        # Component tests
├── MasterInquiryMemberGrid.test.tsx           # Grid component tests
└── MasterInquiryMemberGridColumns.test.tsx    # Column definition tests
```

### Key Test Scenarios

**Reducer Tests** (`useMasterInquiryReducer.test.tsx`):

- State transitions for all actions
- View mode logic (idle → searching → multipleMembers → memberDetails)
- Single result auto-selection
- Error handling

**Hook Tests** (`useMasterInquiry.test.tsx`):

- Search execution flow
- Member selection and dependent fetches
- Pagination handlers
- Reset functionality
- Duplicate request prevention

**Transformation Tests** (`transformSearchParams.test.ts`):

- PSN splitting logic
- Conditional field inclusion
- Enum mappings (paymentType, memberType)

**Component Tests**:

- Rendering with various data shapes
- Conditional field display (employee vs beneficiary)
- User interactions (clicks, pagination)

---

## Common Patterns & Idioms

### 1. Conditional Array Spreading

Used extensively for employee-only fields:

```typescript
return [
  { label: "Name", value: fullName },
  ...(isEmployee ? [{ label: "Store", value: storeNumber }] : []),
  ...(isEmployee ? [{ label: "Department", value: department }] : []),
  { label: "SSN", value: ssn },
];
```

**Result**: Clean conditional inclusion without complex JSX logic.

### 2. Ref-Based Mutable State

For values that need to be current but shouldn't trigger re-renders:

```typescript
const searchParamsRef = useRef(state.search.params);

useEffect(() => {
  searchParamsRef.current = state.search.params;
}, [state.search.params]);

// In callback
const currentSearchParams = searchParamsRef.current;
```

**Use Cases**:

- Deduplication keys
- Latest values in closures (avoiding stale closure issues)

### 3. Minimum Loading Time

```typescript
const [response] = await Promise.all([
  triggerSearch(params).unwrap(),
  new Promise((resolve) => setTimeout(resolve, 300)),
]);
```

**Why**: Prevents loading spinner "flash" for very fast responses. Better UX than spinner appearing/disappearing in <100ms.

### 4. Type-Safe Action Dispatching

```typescript
export type MasterInquiryAction =
  | {
      type: "SEARCH_START";
      payload: { params: MasterInquiryRequest; isManual: boolean };
    }
  | { type: "SEARCH_SUCCESS"; payload: { results: SearchResponse } }
  | { type: "SEARCH_FAILURE"; payload: { error: string } };

// TypeScript enforces correct payload shape
dispatch({ type: "SEARCH_SUCCESS", payload: { results } });
```

**Benefit**: Compiler catches typos and missing fields.

### 5. Progressive Disclosure Rendering

```typescript
{
  showMemberGrid && searchResults && !isFetchingMembers && (
    <MasterInquiryMemberGrid {...props} />
  );
}

{
  selectedMember && (
    <Grid sx={{ display: showMemberDetails ? "block" : "none" }}>
      {!isFetchingMemberDetails && memberDetails ? (
        <MasterInquiryMemberDetails {...props} />
      ) : (
        <CircularProgress />
      )}
    </Grid>
  );
}
```

**Pattern**: Render structure always present, use CSS `display: none` to hide. Prevents layout shift during async loads.

---

## API Endpoints

### 1. Search Members

**Hook**: `useLazySearchProfitMasterInquiryQuery()`

**Request**:

```typescript
interface MasterInquiryRequest {
  pagination: {
    skip: number;
    take: number;
    sortBy: string;
    isSortDescending: boolean;
  };
  endProfitYear: number;
  startProfitMonth?: number;
  endProfitMonth?: number;
  ssn?: number;
  name?: string;
  badgeNumber?: number;
  psnSuffix?: number;
  paymentType?: number; // 0=all, 1=hardship, 2=payoffs, 3=rollovers
  memberType?: number; // 0=all, 1=employees, 2=beneficiaries, 3=none
  contributionAmount?: number;
  earningsAmount?: number;
  forfeitureAmount?: number;
  paymentAmount?: number;
  _timestamp?: number;
}
```

**Response**:

```typescript
interface SearchResponse {
  results: EmployeeDetails[];
  total: number;
}
```

### 2. Get Member Details

**Hook**: `useLazyGetProfitMasterInquiryMemberQuery()`

**Request**:

```typescript
{
  memberType: number;  // 1=employee, 2=beneficiary
  id: number;
  profitYear?: number;
}
```

**Response**:

```typescript
interface MemberDetails {
  firstName: string;
  lastName: string;
  badgeNumber: number;
  ssn: string;
  isEmployee: boolean;
  hireDate?: string;
  terminationDate?: string;
  currentPSAmount: number;
  currentVestedAmount: number;
  percentageVested: number;
  missives?: number[]; // Array of missive IDs to display
  badgesOfDuplicateSsns?: number[]; // Alert user to duplicate SSNs
  // ... many more fields
}
```

### 3. Get Profit Details

**Hook**: `useLazyGetProfitMasterInquiryMemberDetailsQuery()`

**Request**:

```typescript
{
  memberType: number;
  id: number;
  skip: number;
  take: number;
  sortBy: string;
  isSortDescending: boolean;
}
```

**Response**:

```typescript
interface ProfitData {
  results: Array<{
    profitYear: number;
    contributions: number;
    distributions: number;
    forfeitures: number;
    earnings: number;
    endBalance: number;
    // ... other yearly data
  }>;
  total: number;
}
```

---

## Redux Store Integration

### State Slices Used

1. **`inquiry` slice** (`reduxstore/slices/inquirySlice`)

   - `masterInquiryRequestParams`: Persists search parameters for breadcrumbs/navigation
   - Actions: `setMasterInquiryRequestParams`, `clearMasterInquiryRequestParams`

2. **`lookups` slice**

   - `missives`: Array of all possible alert definitions
   - Used to cross-reference missive IDs from API responses

3. **`security` slice**
   - User token (not directly used, but required for authenticated API calls)

### Redux Actions Dispatched

```typescript
// Set search params (for navigation breadcrumb)
dispatch(setMasterInquiryRequestParams(data));

// Clear search params
dispatch(clearMasterInquiryRequestParams());
dispatch(clearMasterInquiryData());
dispatch(clearMasterInquiryGroupingData());
```

---

## Critical Security & Data Rules

### DO NOT Calculate Age in Frontend

**CRITICAL VIOLATION**: The following is STRICTLY PROHIBITED:

```typescript
// WRONG: DO NOT DO THIS
const age = Math.floor(
  (Date.now() - new Date(dateOfBirth).getTime()) /
    (1000 * 60 * 60 * 24 * 365.25),
);
const dobDisplay = `${mmDDYYFormat(dateOfBirth)} (${age})`;
```

**Why This Is Critical**:

1. **Backend Inconsistency**: Frontend age calculations will diverge from backend calculations due to:

   - Different timezones
   - Timing differences in calculation
   - Different date references used for calculation
   - This causes data integrity issues and user confusion

2. **Sensitive Data**: Age is classified as sensitive data that must be:

   - Masked for users without elevated privileges
   - Calculated centrally by backend with authority
   - Never exposed via frontend logic where it could be manipulated

3. **Privacy/Security**: Unprivileged users should never see calculated ages; the backend determines what fields are visible based on user role

**Correct Pattern**:

```typescript
// CORRECT: Display DOB only, let backend handle age if needed
const dobDisplay = dateOfBirth ? mmDDYYFormat(dateOfBirth) : "N/A";

return [
  { label: "DOB", value: dobDisplay },
  // If age display is required by business logic, it MUST come from backend
  // in the MemberDetails response, never calculated in frontend
];
```

**If Age Display Is Required**:

- Add an `Age` field to the `MemberDetails` API response
- Backend calculates age with authoritative date references
- Frontend displays the backend-provided value directly
- Access control applied at backend: mask/exclude age for unprivileged users

---
