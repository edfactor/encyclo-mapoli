# Iteration 3 - Test Errors Report

**Test Results**: 86 failed | 1746 passed (1832 total) - UP from 81 (we added 8 tests)

## New Issues Introduced

The agent's fixes for UnForfeitSearchFilter, MilitaryContributionForm, etc. appear to have added new tests or exposed additional issues. Net change is +5 failures, but this might be new tests being discovered.

## Main Issues Still Failing

### 1. ForfeituresAdjustment Tests - Multiple Test ID Issues (11+ tests)
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
- "modal-save-btn"
- "add-forfeiture-btn"

**Solution**: Change all test ID queries to accessible queries:
- getByTestId("search-button") → getByRole("button", { name: /search/i })
- getByTestId("reset-button") → getByRole("button", { name: /reset/i })
- getByTestId("add-forfeiture-btn") → getByRole("button", { name: /add/i })
- getByTestId("search-filter") → look for accordion or search form
- getByTestId("forfeiture-panel") → look for actual panel content
- etc.

---

### 2. MilitaryContribution Tests - Multiple Test ID Issues (8+ tests)
**File**: `src/pages/DecemberActivities/MilitaryContribution/__test__/MilitaryContribution.test.tsx`

**Missing Test IDs Being Queried**:
- "search-filter"
- "military-grid"
- "frozen-warning"
- "readonly-info"
- "add-contribution-btn"

**Issue**: Same as ForfeituresAdjustment - tests are using brittle test IDs instead of accessible queries

**Solution**: Change to accessible queries

---

### 3. ManageExecutiveHoursAndDollars - Component/Hook Issues (8 tests)
**File**: `src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/__test__/ManageExecutiveHoursAndDollars.test.tsx`

**Error**: `TypeError: Cannot read properties of undefined (reading 'profitYearSelectorData')`

**Issue**: The mock store doesn't have required data that hooks expect. The `useIsProfitYearFrozen` hook is trying to access `state.yearsEnd.profitYearSelectorData` but it's undefined.

**Solution**: Ensure mock store includes all required reducer slices with complete data structures

---

### 4. RTK Query Middleware Errors (Multiple files)
**Error**: `Warning: Middleware for RTK-Query API at reducerPath "militaryApi" has not been added to the store`

**Issue**: Some test stores don't include all required RTK Query middleware

**Solution**: Ensure createMockStore() includes all RTK Query middleware from actual store setup

---

## Priority Fixes for Iteration 4

### HIGH PRIORITY (To reduce test failures quickly)
1. **ForfeituresAdjustment (11 tests)** - Replace all getByTestId() with accessible queries
2. **MilitaryContribution (8 tests)** - Replace all getByTestId() with accessible queries

### MEDIUM PRIORITY
3. **ManageExecutiveHoursAndDollars (8 tests)** - Fix mock store data structure
4. **RTK Query middleware errors** - Ensure proper store setup in createMockStore

---

## Observation

The approach of changing from test IDs to accessible queries is the right pattern, but many tests are heavily reliant on test IDs that don't exist in the components. Two options:

**Option A** (Recommended): Update tests to query elements using accessibility-first approach
- Look for text content
- Use role-based queries
- Use labels and placeholders
- Check for rendered component behavior

**Option B**: Add test IDs to components for testing
- Add data-testid attributes to components
- Update tests to use those IDs consistently

Current approach is Option A - continue with accessible queries.
