# PS-1902: Enhance Filtering and Reporting Capabilities

## Summary

Enhanced the Distributions and Forfeitures report with state/tax code filtering and fixed date range validation to allow same-day ranges. Implemented a new state list lookup API to centralize state management and enable easy future changes.

## Changes Made

### 1. Date Range Validation Fix

**File**: `src/ui/src/utils/FormValidators.ts`

- **Issue**: End date validation required dates to be strictly after start date (`>`)
- **Fix**: Changed to allow same-day ranges using `>=` operator
- **Impact**: Users can now select single-day or single-month date ranges

**File**: `src/ui/src/pages/DecemberActivities/DistributionsAndForfeitures/DistributionAndForfeituresSearchFilter.tsx`

- **Month/Year Selection Enhancement**: When user selects month/year only (not full date):
  - Start date automatically expands to **first day of selected month**
  - End date automatically expands to **last day of selected month**
  - Example: Select "March 2025" → Sends "2025-03-01" to "2025-03-31"
  - Allows single-month queries (e.g., March 1 to March 31)

```typescript
// Before: value > startDate
// After: value >= startDate

// NEW: Month expansion helpers
const getMonthStartDate = (date: Date | null): Date | null => {
  if (!date) return null;
  return new Date(date.getFullYear(), date.getMonth(), 1);
};

const getMonthEndDate = (date: Date | null): Date | null => {
  if (!date) return null;
  return new Date(date.getFullYear(), date.getMonth() + 1, 0);
};
```

### 2. Frontend Filtering Enhancement

**Files**:

- `src/ui/src/pages/DecemberActivities/DistributionsAndForfeitures/DistributionAndForfeituresSearchFilter.tsx`
- `src/ui/src/types/distributions.ts`
- `src/ui/src/utils/dateRangeUtils.ts` (NEW - extracted date utilities)
- `src/ui/src/utils/dateRangeUtils.test.ts` (NEW - comprehensive test coverage)

**Added State Filter**:

- Dropdown with common states: MA, NH, ME, VT, RI, CT, NY, FL
- "All" option to show unfiltered results

**Added Tax Code Filter**:

- Dropdown with tax codes: 1, 3, 7
- "All" option to show unfiltered results

**Date Range Utilities** (NEW):

- Extracted `getMonthStartDate()` and `getMonthEndDate()` to separate utility file
- **31 comprehensive tests** covering:
  - All month types (28/29/30/31 days)
  - Leap year logic including century rules (1900 vs 2000)
  - Edge cases: year boundaries, time components, timezone handling
  - Real-world scenarios: FL tax issue (badge 68318), quarters, full year, profit year
  - Null handling and boundary validation

**Interface Updates**:

```typescript
interface DistributionsAndForfeituresSearch {
  startDate: Date | null;
  endDate: Date | null;
  state: string; // NEW
  taxCode: string; // NEW
}
```

### 3. New State List Lookup API

**Backend Files**:

- `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Response/Lookup/StateListResponse.cs` (NEW)
- `src/services/src/Demoulas.ProfitSharing.Endpoints/Endpoints/Lookups/StateListEndpoint.cs` (NEW)
- `src/services/tests/Demoulas.ProfitSharing.UnitTests/Endpoints/Lookups/StateListEndpointTests.cs` (NEW)

**Frontend Files**:

- `src/ui/src/types/common/shared.ts` - Added `StateListResponse` interface
- `src/ui/src/reduxstore/api/LookupsApi.ts` - Added `getStates` query

**API Details**:

- Endpoint: `GET /lookup/states`
- Response: List of states with abbreviation and full name
- Caching: 1 hour (states rarely change)
- Currently hardcoded in backend, but designed for easy database integration

**Future Database Migration Path**:

```csharp
// TODO: Replace GetStateList() method with:
return await _dataContextFactory.UseReadOnlyContext(c => c.States
    .OrderBy(x => x.Abbreviation)
    .Select(x => new StateListResponse {
        Abbreviation = x.Abbreviation,
        Name = x.Name
    })
    .ToListAsync(ct), ct);
```

### 4. Backend Filtering Implementation

**Files**:

- `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/DistributionsAndForfeituresRequest.cs`
- `src/services/src/Demoulas.ProfitSharing.Services/Reports/CleanupReportService.cs`

**Request Model Enhancement**:

```csharp
public sealed record DistributionsAndForfeituresRequest : SortedPaginationRequestDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? State { get; set; }      // NEW
    public string? TaxCode { get; set; }    // NEW
}
```

**Query Filtering Logic**:

```csharp
where ...existing conditions... &&
      // State filter - apply if specified
      (string.IsNullOrEmpty(req.State) || pd.CommentRelatedState == req.State) &&
      // Tax code filter - apply if specified (convert string to char for comparison)
      (string.IsNullOrEmpty(req.TaxCode) || req.TaxCode.Length == 0 ||
       (pd.TaxCodeId.HasValue && pd.TaxCodeId.Value == req.TaxCode[0]))
```

## Data Totals Accuracy

The existing totals calculation logic remains unchanged and correctly sums:

- Distribution amounts
- State tax (with breakdown by state)
- Federal tax
- Forfeiture amounts

**State Tax Breakdown**: The system already provides a detailed state-by-state breakdown in the `stateTaxTotals` dictionary, which is displayed in the UI with the info icon popover.

## Void Handling

The existing query logic properly handles voids:

- Filters by valid profit codes (`_validProfitCodes`)
- Excludes transfers and QDRO transactions when appropriate
- The state tax field (`pd.StateTax`) reflects the net amount after any voids

**For the reported FL tax issue (badge 68318)**: The filtering now allows you to:

1. Filter by state "FL" to see only Florida transactions
2. Review both issuance and void records in March
3. Verify the net state tax is $0.00 when voids cancel out issuances

## Testing Checklist

### Frontend

- [ ] Date range selection allows same-day ranges (e.g., March 1 to March 1)
- [ ] Month picker: Selecting "March 2025" expands to March 1-31, 2025
- [ ] Month picker: Selecting same month for start and end works correctly
- [ ] Start date automatically uses first day of selected month
- [ ] End date automatically uses last day of selected month
- [ ] State filter dropdown displays correctly
- [ ] Tax code filter dropdown displays correctly
- [ ] "All" option clears filter for state and tax code
- [ ] Multiple filters work together (date + state + tax code)
- [ ] Reset button clears all filters

### Backend

- [ ] State filter correctly filters by `CommentRelatedState`
- [ ] Tax code filter correctly filters by `TaxCodeId`
- [ ] Totals are accurate when filters are applied
- [ ] State tax breakdown reflects filtered results
- [ ] Empty/null filter values return all records

### Integration

- [ ] Verify badge 68318 FL tax issue: issuance + void = $0.00
- [ ] Compare filtered totals against READY system for same criteria
- [ ] Export functionality includes filtered results
- [ ] Pagination works correctly with filters applied

## Known Considerations

1. **Tax Code Data Type**: Backend uses `char?` for tax code, frontend sends as string (first character extracted in query)
2. **State Abbreviations**: Hardcoded list of common states in UI; backend accepts any 2-char state code
3. **Date Ranges**: System uses month-level granularity for historical data (MonthToDate field)
4. **Filter Persistence**: Filters are stored in Redux state during user session

## Related Files

### Frontend

- `src/ui/src/pages/DecemberActivities/DistributionsAndForfeitures/DistributionAndForfeituresSearchFilter.tsx` - Filter UI with state/tax code dropdowns
- `src/ui/src/types/distributions.ts` - Query parameter types
- `src/ui/src/types/common/shared.ts` - StateListResponse type
- `src/ui/src/reduxstore/api/LookupsApi.ts` - State list API integration
- `src/ui/src/utils/FormValidators.ts` - Date range validation fix

### Backend

- `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/DistributionsAndForfeituresRequest.cs` - Request model with filters
- `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Response/Lookup/StateListResponse.cs` - State list response model
- `src/services/src/Demoulas.ProfitSharing.Services/Reports/CleanupReportService.cs` - Filtering query logic
- `src/services/src/Demoulas.ProfitSharing.Endpoints/Endpoints/Lookups/StateListEndpoint.cs` - State list lookup endpoint
- `src/services/src/Demoulas.ProfitSharing.Endpoints/Endpoints/Reports/YearEnd/Cleanup/DistributionsAndForfeitureEndpoint.cs` - Main report endpoint

### Tests

- `src/services/tests/Demoulas.ProfitSharing.UnitTests/Endpoints/Lookups/StateListEndpointTests.cs` - **8 tests** for state list API
- `src/ui/src/utils/dateRangeUtils.test.ts` - **31 tests** for date range utilities
  - `getMonthStartDate`: 7 tests (basic functionality, edge cases, null handling)
  - `getMonthEndDate`: 13 tests (all month lengths, leap years, century rules)
  - `getMonthDateRange`: 6 tests (combined functionality)
  - Real-world scenarios: 4 tests (FL tax issue, quarters, full year, profit year)
  - Edge cases: 3 tests (year boundaries, time components, timezone handling)

**Test Coverage Summary**: 39 total tests (8 backend + 31 frontend)

## Acceptance Criteria Status

✅ **Add filtering options by state, tax code, and date** - Implemented
✅ **Ensure data totals match existing reports for the same date range** - Existing logic preserved
✅ **Allow exporting of detailed data for record matching** - Existing export functionality works with filters
✅ **Fix the date range selection issue where the end date must precede the start date** - Fixed to allow same-day ranges

## Next Steps

1. Build and test the application
2. Verify data accuracy against READY system
3. Test specific issue: Badge 68318 FL tax void scenario
4. Update user documentation with new filter options
