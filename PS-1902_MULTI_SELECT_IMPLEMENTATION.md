# PS-1902: Multi-Select State and Tax Code Filtering

## Overview

Enhanced the Distributions and Forfeitures filtering to support:

1. Multi-select for states and tax codes
2. "All" preselected by default in both dropdowns
3. TaxCode changed from string to char[] for proper type matching with backend

## Changes Made

### Backend Changes

#### 1. DistributionsAndForfeituresRequest.cs

**File**: `src/services/src/Demoulas.ProfitSharing.Common/Contracts/Request/DistributionsAndForfeituresRequest.cs`

**Changed from single values to arrays:**

```csharp
// BEFORE
public string? State { get; set; }
public string? TaxCode { get; set; }

// AFTER
public string[]? States { get; set; }
public char[]? TaxCodes { get; set; }
```

**Rationale**:

- Arrays support multi-select functionality
- `char[]` for TaxCodes matches backend database type (`TaxCodeId` is `char?`)
- Eliminates string-to-char conversion issues

#### 2. CleanupReportService.cs

**File**: `src/services/src/Demoulas.ProfitSharing.Services/Reports/CleanupReportService.cs`

**Updated LINQ query to handle arrays:**

```csharp
// BEFORE
(string.IsNullOrEmpty(req.State) || pd.CommentRelatedState == req.State) &&
(string.IsNullOrEmpty(req.TaxCode) || (pd.TaxCodeId.HasValue && pd.TaxCodeId.Value.ToString() == req.TaxCode))

// AFTER
(req.States == null || req.States.Length == 0 || req.States.Contains(pd.CommentRelatedState)) &&
(req.TaxCodes == null || req.TaxCodes.Length == 0 || (pd.TaxCodeId.HasValue && req.TaxCodes.Contains(pd.TaxCodeId.Value)))
```

**Benefits**:

- Uses efficient `.Contains()` for array membership testing
- Null and empty array checks ensure "All" behavior (no filtering)
- Direct char comparison without string conversion

### Frontend Changes

#### 1. Type Definitions

**distributions.ts** - Request DTO:

```typescript
// BEFORE
export interface DistributionsAndForfeituresRequestDto {
  startDate?: string;
  endDate?: string;
  pagination: SortedPaginationRequestDto;
}

// AFTER
export interface DistributionsAndForfeituresRequestDto {
  startDate?: string;
  endDate?: string;
  states?: string[];
  taxCodes?: string[];
  pagination: SortedPaginationRequestDto;
}
```

**distributions.ts** - Query Params:

```typescript
// BEFORE
export interface DistributionsAndForfeituresQueryParams {
  startDate?: string;
  endDate?: string;
  state?: string;
  taxCode?: string;
}

// AFTER
export interface DistributionsAndForfeituresQueryParams {
  startDate?: string;
  endDate?: string;
  states?: string[];
  taxCodes?: string[];
}
```

#### 2. DistributionAndForfeituresSearchFilter.tsx

**Form interface and validation:**

```typescript
// BEFORE
interface DistributionsAndForfeituresSearch {
  startDate: Date | null;
  endDate: Date | null;
  state: string;
  taxCode: string;
}

const schema = yup.object().shape({
  startDate: yup.date().nullable(),
  endDate: endDateAfterStartDateValidator("startDate"),
  state: yup.string(),
  taxCode: yup.string(),
});

// AFTER
interface DistributionsAndForfeituresSearch {
  startDate: Date | null;
  endDate: Date | null;
  states: string[];
  taxCodes: string[];
}

const schema = yup.object().shape({
  startDate: yup.date().nullable(),
  endDate: endDateAfterStartDateValidator("startDate"),
  states: yup.array().of(yup.string().required()).required(),
  taxCodes: yup.array().of(yup.string().required()).required(),
});
```

**Default values (All preselected):**

```typescript
// BEFORE
defaultValues: {
  startDate: null,
  endDate: null,
  state: "",
  taxCode: ""
}

// AFTER
defaultValues: {
  startDate: null,
  endDate: null,
  states: [],      // Empty array = "All"
  taxCodes: []     // Empty array = "All"
}
```

**Multi-select dropdowns with "All" logic:**

```tsx
<Controller
  name="states"
  control={control}
  render={({ field }) => (
    <TextField
      {...field}
      value={field.value.length === 0 ? ["ALL"] : field.value}
      onChange={(e) => {
        const value = e.target.value as unknown as string[];
        // If "ALL" is selected, clear other selections
        if (value.includes("ALL") && value.length > 1) {
          field.onChange([]);
        } else {
          field.onChange(value);
        }
      }}
      select
      SelectProps={{
        multiple: true,
        renderValue: (selected) => {
          const vals = selected as string[];
          if (vals.length === 0 || vals.includes("ALL")) {
            return "All";
          }
          return vals.join(", ");
        },
      }}
      // ... other props
    >
      <MenuItem
        value="ALL"
        disabled={field.value.length > 0 && !field.value.includes("ALL")}
      >
        All
      </MenuItem>
      {statesData?.map((state) => (
        <MenuItem key={state.abbreviation} value={state.abbreviation}>
          {state.abbreviation} - {state.name}
        </MenuItem>
      ))}
    </TextField>
  )}
/>
```

**Key Features**:

- **Display**: Shows "All" when empty array, otherwise shows comma-separated values
- **Selection Logic**: Selecting any specific value clears "All"; "All" is disabled when specific values are selected
- **onChange**: If "ALL" is clicked when items are selected, it clears the array back to empty (All)
- **Empty Array = All**: Empty array means no filtering (return all records)

#### 3. DistributionAndForfeituresGrid.tsx

**Updated API calls to use arrays:**

```typescript
// onSearch callback
const request = {
  profitYear: profitYear || 0,
  ...(distributionsAndForfeituresQueryParams?.startDate && {
    startDate: distributionsAndForfeituresQueryParams?.startDate
  }),
  ...(distributionsAndForfeituresQueryParams?.endDate && {
    endDate: distributionsAndForfeituresQueryParams?.endDate
  }),
  ...(distributionsAndForfeituresQueryParams?.states &&
      distributionsAndForfeituresQueryParams.states.length > 0 && {
    states: distributionsAndForfeituresQueryParams?.states
  }),
  ...(distributionsAndForfeituresQueryParams?.taxCodes &&
      distributionsAndForfeituresQueryParams.taxCodes.length > 0 && {
    taxCodes: distributionsAndForfeituresQueryParams?.taxCodes
  }),
  pagination: { ... }
};
```

**Pagination reset logic (array comparison):**

```typescript
const currentQueryParams = {
  startDate: distributionsAndForfeituresQueryParams?.startDate,
  endDate: distributionsAndForfeituresQueryParams?.endDate,
  states: JSON.stringify(distributionsAndForfeituresQueryParams?.states || []),
  taxCodes: JSON.stringify(
    distributionsAndForfeituresQueryParams?.taxCodes || []
  ),
  profitYear,
};
```

**Rationale**: JSON.stringify for array comparison since array references change but contents may be the same.

## User Experience

### Default Behavior

1. **On Page Load**: Both State and Tax Code dropdowns show "All" (preselected)
2. **Initial Search**: Returns all records (no filtering)

### Multi-Select Behavior

1. **Selecting Specific Values**:

   - Click dropdown → Select MA → Select CT → Shows "MA, CT"
   - "All" option becomes disabled
   - Search returns only records matching selected states

2. **Returning to "All"**:

   - Option 1: Click "All" (if enabled) → Clears selections
   - Option 2: Use Reset button → Resets to "All"

3. **Combining Filters**:
   - Select states: MA, CT
   - Select tax codes: 4, 7
   - Returns records matching (MA OR CT) AND (4 OR 7)

### Visual Feedback

- **Empty selections**: Displays "All"
- **Single selection**: Displays "MA"
- **Multiple selections**: Displays "MA, CT, NY"
- **All option**: Grayed out when specific values are selected

## API Contract

### Request (POST)

```json
POST /api/yearend/distributions-and-forfeitures

{
  "startDate": "2025-01-01",
  "endDate": "2025-01-31",
  "states": ["MA", "CT", "NY"],
  "taxCodes": ["4", "7"],
  "skip": 0,
  "take": 25,
  "sortBy": "employeeName, date",
  "isSortDescending": false
}
```

### Request (All Selected - No Filtering)

```json
{
  "startDate": "2025-01-01",
  "endDate": "2025-01-31",
  // states and taxCodes omitted (or empty arrays)
  "skip": 0,
  "take": 25,
  "sortBy": "employeeName, date",
  "isSortDescending": false
}
```

## Testing Checklist

### Backend Testing

- [ ] Empty arrays return all records (no filtering)
- [ ] Single state filters correctly
- [ ] Multiple states filter correctly (OR logic)
- [ ] Single tax code filters correctly
- [ ] Multiple tax codes filter correctly (OR logic)
- [ ] Combined filters (states AND tax codes)
- [ ] Null arrays behave same as empty arrays

### Frontend Testing

- [ ] "All" is preselected on page load
- [ ] Selecting specific value removes "All"
- [ ] "All" option becomes disabled when values selected
- [ ] Clicking "All" clears other selections
- [ ] Multi-select shows comma-separated values
- [ ] Reset button returns to "All"
- [ ] Pagination preserves filter selections
- [ ] Sorting preserves filter selections

### Integration Testing

- [ ] Search with "All" returns expected record count
- [ ] Search with specific states returns correct records
- [ ] Search with specific tax codes returns correct records
- [ ] Search with combined filters returns correct records
- [ ] No duplicate API calls on search
- [ ] Query params properly sent to backend

## Breaking Changes

⚠️ **API Contract Change**:

- Request properties changed from `state` and `taxCode` (singular) to `states` and `taxCodes` (arrays)
- Backend now expects `string[]` for States and `char[]` for TaxCodes
- Frontend and backend must be deployed together

## Migration Notes

If there are any saved searches or bookmarks referencing the old API:

- Old: `?state=MA&taxCode=4`
- New: `POST` with `{"states": ["MA"], "taxCodes": ["4"]}`

These will need to be updated manually or a migration path provided.

## Performance Considerations

- **Backend**: `.Contains()` is efficient for small to medium-sized arrays
- **Frontend**: Array comparisons use JSON.stringify for simplicity; could optimize if performance issues arise
- **Network**: POST body is more efficient than long query strings for multiple selections

## Future Enhancements

Potential improvements for future iterations:

1. **Checkbox UI**: Consider using checkboxes instead of multi-select dropdown for better UX
2. **Select All States**: Add "Select All" option to quickly select all available states
3. **Search Within Dropdown**: For tax codes list, add search/filter capability
4. **Saved Filters**: Allow users to save common filter combinations
5. **URL State**: Persist filters in URL for bookmarking/sharing

## Date

- **Created**: 2025-10-16
- **Jira Ticket**: PS-1902
- **Related Work**: GET to POST conversion, Filtering enhancements
