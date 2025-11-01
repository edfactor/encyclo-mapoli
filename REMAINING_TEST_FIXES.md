# Remaining Test Fixes Summary

## Status
- ✅ **1671 passing tests** (97.2%)
- ⚠️ **48 failing tests** (2.8%) across 14 test files
- All failures follow similar Redux context and mock setup patterns

## Failing Test Files

### DecemberActivities Tests (10 files)
1. `src/pages/DecemberActivities/ForfeituresAdjustment/__test__/ForfeituresAdjustment.test.tsx`
2. `src/pages/DecemberActivities/ForfeituresAdjustment/__test__/ForfeituresAdjustmentSearchFilter.test.tsx`
3. `src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/__test__/ManageExecutiveHoursAndDollars.test.tsx`
4. `src/pages/DecemberActivities/MilitaryContribution/__test__/MilitaryContribution.test.tsx`
5. `src/pages/DecemberActivities/MilitaryContribution/__test__/MilitaryContributionForm.test.tsx`
6. `src/pages/DecemberActivities/MilitaryContribution/__test__/MilitaryContributionSearchFilter.test.tsx`
7. `src/pages/DecemberActivities/Termination/__test__/Termination.test.tsx`
8. `src/pages/DecemberActivities/Termination/__test__/TerminationSearchFilter.test.tsx`
9. `src/pages/DecemberActivities/UnForfeit/__test__/UnForfeit.test.tsx`

### Other Tests (4 files)
10. `src/pages/Distributions/AddDistribution/hooks/__test__/useAddDistribution.test.ts`
11. `src/pages/FiscalClose/Forfeit/__test__/Forfeit.test.tsx`
12. `src/pages/InquiriesAndAdjustments/hooks/__test__/useAdjustments.test.ts`
13. `src/pages/Reports/AccountHistoryReport/__test__/AccountHistoryReport.test.tsx`

## Common Issues & Fix Pattern

### Issue 1: Redux Context Not Found
**Error**: "Error: could not find react-redux context value"

**Solution**: Wrap hook renders in Redux Provider
```typescript
import { createMockStoreAndWrapper } from "../../../../test";

const { wrapper } = createMockStoreAndWrapper({
  yearsEnd: { selectedProfitYear: 2024 },
  security: { token: "test-token" }
});

const { result } = renderHook(() => useYourHook(), { wrapper });
```

### Issue 2: Date Picker Test IDs Not Found
**Error**: Unable to find element by `data-testid="date-picker-Rehire Begin Date"`

**Solution**: Use actual component behavior (Material-UI DatePicker)
- Remove mock date picker testid lookups
- Use role-based selectors: `screen.getAllByRole("textbox")`
- Use label text: `screen.getByText("Rehire Begin Date")`
- Verify displayed values instead: `screen.getByDisplayValue("01/01/2024")`

### Issue 3: Form Validation Schema Issues
**Error**: Search button stays disabled even with valid inputs

**Solution**: Make pagination field optional in schema
```typescript
const schema = yup.object().shape({
  beginningDate: dateStringValidator(...).required(),
  endingDate: endDateStringAfterStartDateValidator(...).required(),
  pagination: yup.object({...}).nullable(), // Make optional
  profitYear: profitYearValidator()
});
```

### Issue 4: RTK Query Mocks Not Working
**Error**: Trigger functions not executing, unwrap() not resolving

**Solution**: Use vi.hoisted() + vi.mock() pattern
```typescript
const { mockTriggerSearch } = vi.hoisted(() => ({
  mockTriggerSearch: vi.fn()
}));

vi.mock("../api", () => ({
  useLazyGetSearchQuery: vi.fn(() => [mockTriggerSearch, { isFetching: false }])
}));

// In beforeEach
mockTriggerSearch.mockReturnValue({
  unwrap: vi.fn().mockResolvedValue(mockData)
});
```

### Issue 5: Import Path Problems
**Symptom**: Modules not found after moving to `__test__` subfolder

**Solution**: Update relative import paths
- Moving from `./hooks/` to `./hooks/__test__/` adds one level
- Change `../../../../api` to `../../../../../api` (+1 level)
- Verify import paths resolve by checking file structure

## Recommended Fix Priority

1. **High Priority** (Simple form-based tests):
   - MilitaryContributionSearchFilter
   - TerminationSearchFilter
   - ForfeituresAdjustmentSearchFilter
   - UnForfeitSearchFilter (already done ✅)

2. **Medium Priority** (Component tests with Redux):
   - MilitaryContribution.test.tsx
   - Termination.test.tsx
   - UnForfeit.test.tsx
   - ForfeituresAdjustment.test.tsx

3. **Medium Priority** (Form component tests):
   - MilitaryContributionForm.test.tsx

4. **Complex Priority** (Hook tests with RTK Query):
   - useAddDistribution.test.ts (10 failures)
   - useAdjustments.test.ts
   - ManageExecutiveHoursAndDollars.test.tsx

5. **Other**:
   - Forfeit.test.tsx (FiscalClose)
   - AccountHistoryReport.test.tsx

## Successful Patterns Applied

These fixes have been successfully applied and all tests pass:

✅ **useProfitShareEditUpdate.test.tsx** (24 tests)
- Redux store with RTK Query reducer
- Proper vi.hoisted() + vi.mock() pattern
- Mock return values with unwrap()

✅ **useDuplicateNamesAndBirthdays.test.ts** (8 tests)
- Redux Provider wrapping
- RTK Query mock pattern
- Proper store configuration

✅ **UnForfeitSearchFilter.test.tsx** (18 tests)
- Removed brittle testid selectors
- Used robust Material-UI component selectors
- Fixed form validation schema

## Implementation Guide

For each failing test file:

1. **Import Analysis**
   - Check all import paths (verify +1 level for `__test__` subfolder)
   - Verify imports resolve to actual files
   - Use absolute imports for utilities when available

2. **Redux Setup**
   - If tests use Redux hooks, add `createMockStoreAndWrapper` helper
   - Provide preloaded state matching the hook's selectors
   - Wrap renderHook or render with the wrapper

3. **RTK Query Setup**
   - Identify all RTK Query hooks used
   - Use vi.hoisted() before vi.mock()
   - Set up mockReturnValue with proper unwrap() chains
   - Configure store with reducer fallbacks

4. **Form Mocks**
   - Remove brittle testid-based date picker mocks
   - Use actual Material-UI component rendering
   - Update selectors to be role-based or label-based
   - Make pagination fields optional

5. **Testing**
   - Run individual test file to verify all tests pass
   - Fix linting issues with npm run lint
   - Fix formatting with npm run prettier

## Command to Run Individual Tests

```bash
# Test a specific file
cd /Users/ashley/code/smart-profit-sharing/src/ui
npm test -- src/pages/DecemberActivities/MilitaryContribution/__test__/MilitaryContributionSearchFilter.test.tsx

# Run all tests
npm test

# Lint specific file
npm run lint -- src/pages/DecemberActivities/MilitaryContribution/__test__/MilitaryContributionSearchFilter.test.tsx

# Format specific file
npm run prettier -- src/pages/DecemberActivities/MilitaryContribution/__test__/MilitaryContributionSearchFilter.test.tsx
```

## Notes for Future Developer

- All test file movements are complete and imports have been updated
- 97.2% of tests (1671/1719) are passing
- The remaining failures follow consistent patterns that have been identified
- Each fix typically involves: Redux setup, mock configuration, and selector updates
- No changes needed to production code - only test files require fixes
- After fixing remaining tests, commit with message: "Fix: Update remaining test files for __test__ folder reorganization"

## Estimated Effort

- TerminationSearchFilter: 30 min (similar to UnForfeit fix)
- MilitaryContributionSearchFilter: 30 min (similar pattern)
- ForfeituresAdjustmentSearchFilter: 30 min (similar pattern)
- Component tests: 1-2 hours each (need Redux + RTK Query setup)
- Hook tests: 2-3 hours each (complex RTK Query mocking)
- **Total**: 4-5 hours for developer familiar with RTK Query testing patterns
