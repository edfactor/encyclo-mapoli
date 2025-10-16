# PS-1902: Distributions and Forfeitures GET to POST Conversion

## Overview

Converted the `distributions-and-forfeitures` endpoint from GET to POST to handle larger payloads and fix missing query parameters (state and taxCode).

## Issues Addressed

1. **Missing Query Parameters**: State and taxCode were not being sent to backend
2. **Payload Too Large**: GET request query string was becoming too long
3. **Duplicate API Calls**: Search button was triggering API call twice

## Changes Made

### Frontend Changes

#### 1. YearsEndApi.ts

**File**: `src/ui/src/reduxstore/api/YearsEndApi.ts`

**Change**: Converted GET to POST and added missing state/taxCode parameters

```typescript
// BEFORE
getDistributionsAndForfeitures: builder.query<...>({
  query: (params) => ({
    url: `yearend/distributions-and-forfeitures`,
    method: "GET",
    params: {
      startDate: params.startDate,
      endDate: params.endDate,
      take: params.pagination.take,
      skip: params.pagination.skip,
      sortBy: params.pagination.sortBy,
      isSortDescending: params.pagination.isSortDescending
      // MISSING: state, taxCode
    }
  })
})

// AFTER
getDistributionsAndForfeitures: builder.query<...>({
  query: (params) => ({
    url: `yearend/distributions-and-forfeitures`,
    method: "POST",
    body: {
      startDate: params.startDate,
      endDate: params.endDate,
      state: params.state,           // ADDED
      taxCode: params.taxCode,        // ADDED
      skip: params.pagination.skip,
      take: params.pagination.take,
      sortBy: params.pagination.sortBy,
      isSortDescending: params.pagination.isSortDescending
    }
  })
})
```

#### 2. DistributionAndForfeituresSearchFilter.tsx

**File**: `src/ui/src/pages/DecemberActivities/DistributionsAndForfeitures/DistributionAndForfeituresSearchFilter.tsx`

**Change**: Removed direct `triggerSearch` call to prevent duplicate API calls

```typescript
// BEFORE
triggerSearch({
  ...(data.startDate && { startDate: formatDateOnly(data.startDate) }),
  ...(data.endDate && { endDate: formatDateOnly(data.endDate) }),
  ...(data.state && data.state !== "" && { state: data.state }),
  ...(data.taxCode && data.taxCode !== "" && { taxCode: data.taxCode }),
  pagination: { skip: 0, take: 25, sortBy: "employeeName, date", isSortDescending: false }
}, false);
dispatch(setDistributionsAndForfeituresQueryParams({...}));
setInitialSearchLoaded(true);

// AFTER
// Only dispatch query params and set flag - Grid's useEffect will make the API call
dispatch(setDistributionsAndForfeituresQueryParams({
  startDate: formatDateOnly(data.startDate),
  endDate: formatDateOnly(data.endDate),
  state: data.state || undefined,
  taxCode: data.taxCode || undefined
}));
setInitialSearchLoaded(true);
```

**Reason**: The direct `triggerSearch` call was causing one API call, then `setInitialSearchLoaded(true)` was triggering the Grid's `useEffect` which made a second API call. By removing the direct call, we rely on the Grid's `useEffect` to make the single API call.

#### 3. DistributionAndForfeituresGrid.tsx

**File**: `src/ui/src/pages/DecemberActivities/DistributionsAndForfeitures/DistributionAndForfeituresGrid.tsx`

**Changes**: Added state and taxCode parameters to all API call locations

```typescript
// Added to onSearch callback
const onSearch = useCallback(async () => {
  const request = {
    profitYear: profitYear || 0,
    ...(distributionsAndForfeituresQueryParams?.startDate && {
      startDate: distributionsAndForfeituresQueryParams?.startDate
    }),
    ...(distributionsAndForfeituresQueryParams?.endDate && {
      endDate: distributionsAndForfeituresQueryParams?.endDate
    }),
    ...(distributionsAndForfeituresQueryParams?.state && {      // ADDED
      state: distributionsAndForfeituresQueryParams?.state
    }),
    ...(distributionsAndForfeituresQueryParams?.taxCode && {    // ADDED
      taxCode: distributionsAndForfeituresQueryParams?.taxCode
    }),
    pagination: { ... }
  };
  await triggerSearch(request, false);
}, [
  // ... existing dependencies
  distributionsAndForfeituresQueryParams?.state,    // ADDED
  distributionsAndForfeituresQueryParams?.taxCode,  // ADDED
]);

// Similar changes made to:
// - onPaginationChange callback
// - useEffect pagination reset logic
```

**Reason**: The Grid component is responsible for making API calls when filters change. It needs to include state and taxCode in the request payload and dependency arrays.

### Backend Changes

#### 1. DistributionsAndForfeitureEndpoint.cs

**File**: `src/services/src/Demoulas.ProfitSharing.Endpoints/Endpoints/Reports/YearEnd/Cleanup/DistributionsAndForfeitureEndpoint.cs`

**Change**: Changed from `Get()` to `Post()`

```csharp
// BEFORE
public override void Configure()
{
    Get("distributions-and-forfeitures");
    // ...
}

// AFTER
public override void Configure()
{
    Post("distributions-and-forfeitures");
    // ...
}
```

#### 2. Request DTO (Already Exists)

**File**: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/DistributionsAndForfeituresRequest.cs`

**No changes needed** - DTO already had State and TaxCode properties:

```csharp
public sealed record DistributionsAndForfeituresRequest : SortedPaginationRequestDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? State { get; set; }      // Already present
    public string? TaxCode { get; set; }    // Already present
}
```

### Test Changes

#### CleanupReportServiceTests.cs

**File**: `src/services/tests/Demoulas.ProfitSharing.UnitTests/Reports/YearEnd/CleanupReportServiceTests.cs`

**Change**: Updated all test calls from `GETAsync` to `POSTAsync`

```csharp
// BEFORE (4 occurrences)
response = await ApiClient
    .GETAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest,
              DistributionsAndForfeitureTotalsResponse>(req);

// AFTER (4 occurrences)
response = await ApiClient
    .POSTAsync<DistributionsAndForfeitureEndpoint, DistributionsAndForfeituresRequest,
               DistributionsAndForfeitureTotalsResponse>(req);
```

## Verification

### Backend Compilation

```bash
cd src/services
dotnet build Demoulas.ProfitSharing.slnx --no-incremental
# ✅ Build succeeded in 106.7s
```

### Frontend Type Checking

```bash
cd src/ui
npx tsc --noEmit
# ✅ No errors
```

## API Contract Changes

### Old Contract (GET)

```
GET /api/yearend/distributions-and-forfeitures?startDate=2025-01-01&endDate=2025-01-31&take=25&skip=0&sortBy=employeeName,date&isSortDescending=false
```

**Issues**:

- Missing state and taxCode in query string
- Query string can get very long with all parameters

### New Contract (POST)

```
POST /api/yearend/distributions-and-forfeitures
Content-Type: application/json

{
  "startDate": "2025-01-01",
  "endDate": "2025-01-31",
  "state": "MA",
  "taxCode": "4",
  "skip": 0,
  "take": 25,
  "sortBy": "employeeName, date",
  "isSortDescending": false
}
```

**Benefits**:

- All parameters sent in request body
- No URL length limitations
- Cleaner API design for complex queries

## Testing Recommendations

1. **Manual Testing**:

   - Test with state filter (e.g., "MA - Massachusetts")
   - Test with tax code filter (e.g., "4 - Death")
   - Test with both filters combined
   - Test with date range filtering
   - Verify only ONE API call is made per search button click
   - Test pagination after search

2. **Backend Tests**:

   - All existing tests continue to pass (changed GET to POST)
   - Run: `dotnet test src/services/tests/Demoulas.ProfitSharing.UnitTests/Demoulas.ProfitSharing.UnitTests.csproj`

3. **Integration Testing**:
   - Verify state and taxCode parameters are properly received by backend
   - Test with missing optional parameters (should still work)
   - Test with all parameters present

## Related Files

### Modified Files

- ✅ `src/ui/src/reduxstore/api/YearsEndApi.ts`
- ✅ `src/ui/src/pages/DecemberActivities/DistributionsAndForfeitures/DistributionAndForfeituresSearchFilter.tsx`
- ✅ `src/services/src/Demoulas.ProfitSharing.Endpoints/Endpoints/Reports/YearEnd/Cleanup/DistributionsAndForfeitureEndpoint.cs`
- ✅ `src/services/tests/Demoulas.ProfitSharing.UnitTests/Reports/YearEnd/CleanupReportServiceTests.cs`

### Unchanged Files (Already Correct)

- ℹ️ `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/DistributionsAndForfeituresRequest.cs` - Already had State and TaxCode properties
- ℹ️ `src/services/src/Demoulas.ProfitSharing.Services/Reports/CleanupReportService.cs` - Already handles State and TaxCode filtering

## Breaking Changes

⚠️ **API Contract Change**: This is a breaking change for any external consumers of the API.

- Clients must now send POST requests instead of GET
- Request parameters must be sent in JSON body instead of query string

However, since this is an internal application with the frontend tightly coupled to the backend, there should be no issues as both are updated together.

## Next Steps

1. **Deploy Changes**: Deploy frontend and backend together to avoid mismatch
2. **Monitor API Calls**: Verify in browser Network tab that only one API call is made per search
3. **Verify Filtering**: Test that state and taxCode filters work correctly
4. **Update Documentation**: If there's API documentation, update to reflect POST instead of GET

## Date

- **Created**: 2025-10-16
- **Jira Ticket**: PS-1902
- **Related Work**: Part of enhancing filtering and reporting capabilities
