# Iteration 2 - Test Errors Report

**Test Results**: 81 failed | 1743 passed (1824 total)

## Error Categories

### 1. UnForfeitSearchFilter - act() Warnings (20+ warnings)
**File**: `src/pages/DecemberActivities/UnForfeit/UnForfeitSearchFilter.test.tsx`

**Affected Tests**:
- "should render the form with all fields"
- "should render search and reset buttons"
- "should render exclude zero balance checkbox"
- "should enforce fiscal year date constraints"
- "should disable search when hasUnsavedChanges is true"
- "should show alert when trying to search with unsaved changes"
- "should toggle checkbox when reset"
- "should track form validity"
- "should set min/max dates based on fiscal data"
- "should show loading spinner when isFetching is true"

**Issue**: Component renders and passes props but state updates from form interactions aren't wrapped in `act()` or `waitFor()`.

**Solution**: Wrap rendering and assertions in `waitFor()` similar to fixes from iteration 1.

---

### 2. MilitaryContributionForm - Form Tests Failing (3 tests)
**File**: `src/pages/DecemberActivities/MilitaryContribution/MilitaryContributionForm.test.tsx`

**Failing Tests**:
- ✗ "should not call onSubmit when form is invalid"
- ✗ "should call onCancel when cancel button is clicked"
- ✗ "should clear form when cancel is clicked"

**Issue**: Tests are failing but error details truncated in output. Need to check implementation.

---

### 3. Forfeit Tests - Missing Test ID and Function Call Issues (2 tests)
**File**: `src/pages/FiscalClose/Forfeit/__test__/Forfeit.test.tsx`

**Failing Test 1**: "should call executeSearch when search button is clicked"
- Error: `Unable to find an element with testid: "search-button"`
- Should use accessible query instead

**Failing Test 2**: "should call handleReset when reset button is clicked"
- Error: `expected "spy" to be called at least once`
- Mock function not being called properly with `fireEvent.click()`

**Solution**: Use `userEvent` instead of `fireEvent`, wrap in `waitFor()`.

---

### 4. useBalanceValidation Tests - Network Error Handling
**File**: `src/hooks/useBalanceValidation.test.ts`

**Issues**:
- Error handling test cases showing expected errors (these are OK - testing error handling)
- Mostly informational/logging, not actual test failures

---

### 5. usePayMasterUpdate Tests - RTK Query Mock Issue (4 test failures)
**File**: `src/pages/FiscalClose/PayMasterUpdate/hooks/__test__/usePayMasterUpdate.test.tsx`

**Error**: `TypeError: updateEnrollment(...).unwrap is not a function`

**Root Cause**: RTK Query mutation mock doesn't have `.unwrap()` method. The mock should return an object with `.unwrap()`.

**Solution**: Update mock to include `.unwrap()` method:
```typescript
mockMutation.mockReturnValue({
  unwrap: vi.fn().mockResolvedValue({...})
})
```

---

### 6. ManageExecutiveHoursAndDollars - Import Error
**File**: `src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/__test__/ManageExecutiveHoursAndDollars.test.tsx`

**Error**: `Failed to resolve import "../../../hooks/useReadOnlyNavigation"`

**Issue**: The test is trying to import a hook that doesn't exist or has wrong path.

**Solution**: Check if hook exists and fix import path, or remove the test that imports it.

---

### 7. ForfeituresAdjustment Tests - Missing Test IDs (8 tests)
**File**: `src/pages/DecemberActivities/ForfeituresAdjustment/__test__/ForfeituresAdjustment.test.tsx`

**Missing Test IDs Being Queried**:
- "search-filter"
- "search-button"
- "reset-button"
- "searching"
- "forfeiture-panel"
- "transaction-grid"
- "add-forfeiture-modal"
- "modal-close-btn"

**Solution**: Either add these test IDs to the component or change tests to use accessible queries.

---

## Priority Fix Order

1. **HIGH**: UnForfeitSearchFilter (20+ warnings) - Apply same act()/waitFor() pattern
2. **HIGH**: ForfeituresAdjustment (8 failing tests) - Change test IDs to accessible queries
3. **HIGH**: MilitaryContributionForm (3 failing tests) - Debug and fix failing assertions
4. **HIGH**: Forfeit tests (2 failing) - Fix test ID and fireEvent issues
5. **MEDIUM**: usePayMasterUpdate (4 failing) - Fix RTK Query mock with .unwrap()
6. **MEDIUM**: ManageExecutiveHoursAndDollars - Fix import path

---

## Files to Fix (6 test files)

1. `src/pages/DecemberActivities/UnForfeit/UnForfeitSearchFilter.test.tsx` - 20+ act() warnings
2. `src/pages/DecemberActivities/MilitaryContribution/MilitaryContributionForm.test.tsx` - 3 failures
3. `src/pages/FiscalClose/Forfeit/__test__/Forfeit.test.tsx` - 2 failures
4. `src/pages/DecemberActivities/ForfeituresAdjustment/__test__/ForfeituresAdjustment.test.tsx` - 8 failures
5. `src/pages/FiscalClose/PayMasterUpdate/hooks/__test__/usePayMasterUpdate.test.tsx` - 4 failures
6. `src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/__test__/ManageExecutiveHoursAndDollars.test.tsx` - Import error
