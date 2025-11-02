# Frontend Unit Test Repair Work - Remaining Issues

**Analysis Date**: 2025-11-01
**Total Tests**: 1,730
**Passed**: 1,673 (96.7%)
**Failed**: 57 (3.3%)
**Failed Test Files**: 8 of 94

---

## Executive Summary

The test suite has 57 failing tests across 8 test files. The failures fall into four main categories:

1. **RTK Query Configuration Issues** - Missing base URL and API middleware (affects 16 tests)
2. **React Hook Form Mocking** - Forms not properly mocked (affects 17 tests)
3. **Redux State Initialization** - Missing required state properties (affects 8 tests)
4. **Async State Handling** - State updates not wrapped in `act()` (affects 16 tests)

All issues have clear solutions and can be systematically addressed.

---

## Failed Test Files

### 1. ManageExecutiveHoursAndDollars.test.tsx

**Status**: 🔴 8 failures
**Location**: `/src/ui/src/pages/DecemberActivities/ManageExecutiveHoursAndDollars/__test__/ManageExecutiveHoursAndDollars.test.tsx`

#### Error
```
Cannot read properties of undefined (reading 'profitYearSelectorData')
```

#### Failed Tests
1. ✗ should render the page with all components
2. ✗ should render save button in disabled state when no pending changes
3. ✗ should call saveChanges on successful save when button is enabled
4. ✗ should call saveChanges even on save failure to handle error appropriately
5. ✗ should not call saveChanges when button is disabled (no pending changes)
6. ✗ should disable save button in read-only mode even with pending changes
7. ✗ should show read-only tooltip when hovering disabled save button in read-only mode
8. ✗ should show 'no pending changes' tooltip when save button is disabled due to no changes

#### Root Cause
The component expects `profitYearSelectorData` from Redux state (`navigation` slice), but the mock store doesn't provide it.

#### Solution
Update mock state initialization in test setup:

```typescript
const mockState = {
  security: { token: "mock-token" },
  navigation: {
    navigationData: null,
    profitYearSelectorData: {
      profitYears: [
        { profitYear: 2024, fiscalBeginDate: "2024-01-01", fiscalEndDate: "2024-12-31" }
      ],
      currentProfitYear: 2024
    }
  },
  yearsEnd: {
    selectedProfitYearForFiscalClose: 2024,
    executiveHoursAndDollarsGrid: []
  }
};

const { wrapper } = createMockStoreAndWrapper(mockState);
```

#### Priority
🔴 **High** - Blocks all tests in this file

---

### 2. MilitaryContribution.test.tsx

**Status**: 🔴 13 failures
**Location**: `/src/ui/src/pages/DecemberActivities/MilitaryContribution/__test__/MilitaryContribution.test.tsx`

#### Errors
- Timeout errors (component not rendering)
- Missing React Hook Form mocks
- Mock functions not returning proper values

#### Failed Tests
1. ✗ should render the page with all components (1399ms timeout)
2. ✗ should render search filter accordion (1144ms timeout)
3. ✗ should render military contribution grid when member selected (1131ms timeout)
4. ✗ should display frozen year warning when year is frozen (1138ms timeout)
5. ✗ should display read-only info when status is read-only (1114ms timeout)
6. ✗ should disable add contribution button in read-only mode (1129ms timeout)
7. ✗ should enable add contribution button when not read-only (1122ms timeout)
8. ✗ should open dialog when add contribution button is clicked (3473ms timeout)
9. ✗ should close dialog when form is submitted (3590ms timeout)
10. ✗ should close dialog when cancel is clicked (3399ms timeout)
11. ✗ should display success message when contribution is saved (3027ms timeout)
12. ✗ should fetch contributions when member details change (1161ms timeout)
13. ✗ should display missive alerts when present (132ms timeout)

#### Root Causes
1. **React Hook Form not mocked** - Component uses `useForm` and `Controller`
2. **Hook mocks incomplete** - `mockUseMissiveAlerts` doesn't return proper structure
3. **Component dependencies missing** - Missing required props or context

#### Solutions

**1. Add React Hook Form mock** (at module level, before imports):
```typescript
vi.mock("react-hook-form", () => ({
  useForm: vi.fn(() => ({
    control: {},
    handleSubmit: vi.fn((fn) => (e) => {
      e?.preventDefault();
      return fn();
    }),
    formState: { errors: {}, isValid: true },
    watch: vi.fn(),
    setValue: vi.fn(),
    reset: vi.fn(),
    getValues: vi.fn(() => ({}))
  })),
  Controller: vi.fn(({ render, name }) =>
    render({
      field: {
        onChange: vi.fn(),
        onBlur: vi.fn(),
        value: '',
        name,
        ref: vi.fn()
      },
      fieldState: { error: undefined }
    })
  )
}));
```

**2. Fix hook mock returns in `beforeEach`**:
```typescript
beforeEach(() => {
  vi.clearAllMocks();

  mockUseMissiveAlerts.mockReturnValue({
    missiveAlerts: [],
    addAlert: vi.fn(),
    clearAlerts: vi.fn()
  });

  mockUseMilitaryMemberDetails.mockReturnValue({
    memberDetails: null,
    setMemberDetails: vi.fn()
  });
});
```

**3. Ensure all required context providers are wrapped**:
```typescript
const { wrapper } = createMockStoreAndWrapper({
  security: { token: "test-token" },
  yearsEnd: {
    selectedProfitYearForDecemberActivities: 2024,
    militaryContributions: []
  }
});

render(<MilitaryContribution />, { wrapper });
```

#### Priority
🔴 **High** - Most failing tests in a single file

---

### 3. MilitaryContributionForm.test.tsx

**Status**: 🔴 5 failures
**Location**: `/src/ui/src/pages/DecemberActivities/MilitaryContribution/__test__/MilitaryContributionForm.test.tsx`

#### Error
Form not rendering due to missing React Hook Form mocks

#### Failed Tests
1. ✗ should require contribution amount
2. ✗ should validate contribution amount is positive
3. ✗ should allow selecting contribution type
4. ✗ should accept callback props
5. ✗ should submit correct contribution data

#### Root Cause
Form component uses React Hook Form (`useForm`, `Controller`) which isn't mocked

#### Solution
Same as MilitaryContribution.test.tsx - add React Hook Form mock at module level (see above)

#### Priority
🔴 **High** - All tests failing due to same root cause

---

### 4. Termination.test.tsx

**Status**: 🔴 7 failures
**Location**: `/src/ui/src/pages/DecemberActivities/Termination/__test__/Termination.test.tsx`

#### Errors
```
Failed to parse URL from undefined/api/lookup/duplicate-ssns/exists
An update to DuplicateSsnGuard inside a test was not wrapped in act(...)
```

#### Failed Tests
1. ✗ should render search filter and grid when fiscal data is loaded (1212ms timeout)
2. ✗ should call handleSearch when search button is clicked (1166ms timeout)
3. ✗ should disable search button during search (1167ms timeout)
4. ✗ should disable search button when unsaved changes exist (1173ms timeout)
5. ✗ should pass searchParams to TerminationGrid (253ms)
6. ✗ should pass fiscal data to TerminationSearchFilter (182ms)
7. ✗ should handle archive flag when shouldArchive is true (172ms)

#### Root Causes
1. **Base URL not configured** - RTK Query API calls fail with undefined base URL
2. **DuplicateSsnGuard makes API calls** - Component triggers network requests during render
3. **Act warnings** - Async state updates not properly wrapped

#### Solutions

**1. Mock DuplicateSsnGuard component**:
```typescript
vi.mock("../../../../components/DuplicateSsnGuard", () => ({
  default: vi.fn(({ children }) => <>{children}</>)
}));
```

**2. Configure RTK Query base URL in test**:
```typescript
// Option A: Mock the API endpoint
vi.mock("../../../reduxstore/api/LookupsApi", () => ({
  useLazyCheckDuplicateSsnQuery: vi.fn(() => [
    vi.fn().mockResolvedValue({ data: { exists: false } }),
    { isFetching: false }
  ])
}));

// Option B: Add base URL to test environment
process.env.VITE_API_BASE_URL = "http://localhost:3000";
```

**3. Wrap assertions in waitFor**:
```typescript
await userEvent.click(searchButton);

await waitFor(() => {
  expect(mockHandleSearch).toHaveBeenCalled();
});
```

#### Priority
🔴 **High** - Affects core functionality testing

---

### 5. TerminationSearchFilter.test.tsx

**Status**: 🔴 7 failures
**Location**: `/src/ui/src/pages/DecemberActivities/Termination/__test__/TerminationSearchFilter.test.tsx`

#### Error
```
Failed to parse URL from undefined/api/lookup/duplicate-ssns/exists
```

#### Failed Tests
1. ✗ should render the form with all fields (907ms)
2. ✗ should enforce fiscal year date constraints (215ms)
3. ✗ should validate end date is after begin date (217ms)
4. ✗ should call onSearch when search button is clicked (1933ms timeout)
5. ✗ should call setInitialSearchLoaded when search is executed (1751ms timeout)
6. ✗ should set min/max dates based on fiscal data (188ms)
7. ✗ should include correct default values in search request (1812ms timeout)

#### Root Cause
Same as Termination.test.tsx - `DuplicateSsnGuard` component makes API calls without proper base URL

#### Solution
Same as Termination.test.tsx - mock the `DuplicateSsnGuard` component (see above)

#### Priority
🔴 **High** - Same root cause as parent component

---

### 6. Forfeit.test.tsx

**Status**: 🔴 5 failures
**Location**: `/src/ui/src/pages/FiscalClose/Forfeit/__test__/Forfeit.test.tsx`

#### Errors
```
No data found at `state.navigationStatusApi`. Did you forget to add the reducer to the store?
No data found at `state.lookupsApi`. Did you forget to add the reducer to the store?
No data found at `state.yearsEndApi`. Did you forget to add the reducer to the store?
Warning: Middleware for RTK-Query API at reducerPath "yearsEndApi" has not been added to the store.
```

#### Failed Tests
1. ✗ should render Forfeit page with all components (578ms)
2. ✗ should call handleStatusChange when status changes to Complete (7224ms timeout)
3. ✗ should call handleStatusChange when status changes to In Progress (6123ms timeout)
4. ✗ should call executeSearch when search button is clicked (3218ms timeout)
5. ✗ should call handleReset when reset button is clicked (3189ms timeout)

#### Root Cause
Test store missing RTK Query API reducers and middleware. The component uses multiple RTK Query APIs that aren't registered in the test store.

#### Solution

**Import required APIs**:
```typescript
import { navigationStatusApi } from "../../../../reduxstore/api/NavigationStatusApi";
import { lookupsApi } from "../../../../reduxstore/api/LookupsApi";
import { yearsEndApi } from "../../../../reduxstore/api/YearsEndApi";
```

**Update test store creation**:
```typescript
const store = configureStore({
  reducer: {
    security: securityReducer,
    navigation: navigationReducer,
    fiscalClose: fiscalCloseReducer,
    // Add RTK Query reducers
    [navigationStatusApi.reducerPath]: navigationStatusApi.reducer,
    [lookupsApi.reducerPath]: lookupsApi.reducer,
    [yearsEndApi.reducerPath]: yearsEndApi.reducer
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(
      navigationStatusApi.middleware,
      lookupsApi.middleware,
      yearsEndApi.middleware
    ),
  preloadedState: {
    security: { token: "test-token" },
    fiscalClose: {
      selectedFiscalYear: 2024,
      forfeitStatus: "Not Started"
    }
  }
});
```

**OR** modify `createMockStoreAndWrapper` to include APIs by default:
```typescript
// In test/utils.tsx or similar
export function createMockStoreAndWrapper(preloadedState = {}, options = {}) {
  const store = configureStore({
    reducer: {
      ...standardReducers,
      // Include all RTK Query APIs by default
      [navigationStatusApi.reducerPath]: navigationStatusApi.reducer,
      [lookupsApi.reducerPath]: lookupsApi.reducer,
      [yearsEndApi.reducerPath]: yearsEndApi.reducer
    },
    middleware: (getDefaultMiddleware) =>
      getDefaultMiddleware().concat(
        navigationStatusApi.middleware,
        lookupsApi.middleware,
        yearsEndApi.middleware
      ),
    preloadedState
  });

  return { store, wrapper: ({ children }) => <Provider store={store}>{children}</Provider> };
}
```

#### Priority
🟡 **Medium** - Architectural issue affecting one component

---

### 7. ForfeituresAdjustment.test.tsx

**Status**: 🔴 4 failures
**Location**: `/src/ui/src/pages/DecemberActivities/ForfeituresAdjustment/__test__/ForfeituresAdjustment.test.tsx`

#### Error
Timeout errors indicate component not rendering properly

#### Failed Tests
1. ✗ should call executeSearch when search button is clicked (1661ms timeout)
2. ✗ should display loading state during search (1175ms timeout)
3. ✗ should display transaction grid when transactions are available (3863ms timeout)
4. ✗ should call handleSaveForfeiture when modal save is clicked (4807ms timeout)

#### Root Cause
Component dependencies not properly mocked or Redux state incomplete

#### Investigation Needed
1. Check component imports and dependencies
2. Verify all Redux state required by component
3. Ensure all RTK Query hooks are mocked
4. Check for any context providers needed

#### Solutions

**1. Add debug logging to identify missing dependencies**:
```typescript
it("should render component", () => {
  const { debug } = render(<ForfeituresAdjustment />, { wrapper });
  debug(); // Will show what actually renders
});
```

**2. Ensure complete Redux state**:
```typescript
const { wrapper } = createMockStoreAndWrapper({
  security: { token: "test-token" },
  yearsEnd: {
    selectedProfitYearForDecemberActivities: 2024,
    forfeituresAdjustmentQueryParams: {},
    // Add any other required state
  }
});
```

**3. Mock any RTK Query hooks used**:
```typescript
vi.mock("../../../reduxstore/api/YearsEndApi", () => ({
  useLazyGetForfeituresQuery: vi.fn(() => [
    vi.fn().mockReturnValue({
      unwrap: vi.fn().mockResolvedValue({ results: [], total: 0 })
    }),
    { isFetching: false }
  ])
}));
```

#### Priority
🟡 **Medium** - Requires investigation to determine exact cause

---

### 8. UnForfeit.test.tsx

**Status**: 🔴 9 failures
**Location**: `/src/ui/src/pages/DecemberActivities/UnForfeit/__test__/UnForfeit.test.tsx`

#### Errors
```
An update to UnForfeitSearchFilter inside a test was not wrapped in act(...)
```

#### Failed Tests
1. ✗ should render ApiMessageAlert (6488ms timeout)
2. ✗ should render StatusDropdownActionNode in action node (5435ms timeout)
3. ✗ should render search filter and grid when fiscal data is loaded (1981ms)
4. ✗ should call handleSearch when search button is clicked (6235ms timeout)
5. ✗ should disable search button when unsaved changes exist (6320ms timeout)
6. ✗ should pass handleStatusChange to StatusDropdownActionNode (6307ms timeout)
7. ✗ should pass initialSearchLoaded to UnForfeitGrid (284ms)
8. ✗ should pass hasUnsavedChanges to grid and filter (4992ms timeout)

#### Root Cause
Async state updates in `UnForfeitSearchFilter` not properly handled in tests. State changes trigger during render or after user interaction without proper act() wrapping.

#### Solutions

**1. Wrap all user interactions and assertions in waitFor**:
```typescript
it("should call handleSearch when search button is clicked", async () => {
  const user = userEvent.setup();
  render(<UnForfeit />, { wrapper });

  const searchButton = screen.getByRole("button", { name: /search/i });

  await user.click(searchButton);

  await waitFor(() => {
    expect(mockHandleSearch).toHaveBeenCalled();
  });
});
```

**2. Use findBy queries instead of getBy for async elements**:
```typescript
// ❌ BAD - may not be in DOM yet
const element = screen.getByText("Expected Text");

// ✅ GOOD - waits for element to appear
const element = await screen.findByText("Expected Text");
```

**3. Mock form submission to prevent state updates**:
```typescript
vi.mock("react-hook-form", () => ({
  useForm: vi.fn(() => ({
    control: {},
    handleSubmit: vi.fn((fn) => async (e) => {
      e?.preventDefault();
      await act(async () => {
        await fn();
      });
    }),
    // ... other methods
  }))
}));
```

**4. Ensure component fully renders before interactions**:
```typescript
it("should render component", async () => {
  render(<UnForfeit />, { wrapper });

  // Wait for component to fully render
  await waitFor(() => {
    expect(screen.getByRole("button", { name: /search/i })).toBeInTheDocument();
  });

  // Now safe to interact
  await userEvent.click(screen.getByRole("button", { name: /search/i }));
});
```

#### Priority
🟡 **Medium** - Common pattern issue affecting multiple tests

---

## Common Patterns & Root Causes

### Pattern 1: RTK Query Configuration Issues
**Affects**: Termination tests, TerminationSearchFilter tests, Forfeit tests (16 failures)

**Issue**: Components use RTK Query APIs but test environment doesn't configure:
- Base URL for API calls
- API reducers in store
- API middleware

**Global Solution**: Update test setup to include RTK Query configuration by default

```typescript
// In src/ui/src/test/setup.ts or similar
import { beforeAll } from "vitest";

beforeAll(() => {
  // Set base URL for RTK Query
  process.env.VITE_API_BASE_URL = "http://localhost:3000/api";
});

// In createMockStoreAndWrapper helper
import { navigationStatusApi } from "../reduxstore/api/NavigationStatusApi";
import { lookupsApi } from "../reduxstore/api/LookupsApi";
import { yearsEndApi } from "../reduxstore/api/YearsEndApi";

export function createMockStoreAndWrapper(preloadedState = {}) {
  const store = configureStore({
    reducer: {
      ...standardReducers,
      // Add all RTK Query APIs
      [navigationStatusApi.reducerPath]: navigationStatusApi.reducer,
      [lookupsApi.reducerPath]: lookupsApi.reducer,
      [yearsEndApi.reducerPath]: yearsEndApi.reducer
    },
    middleware: (getDefaultMiddleware) =>
      getDefaultMiddleware().concat(
        navigationStatusApi.middleware,
        lookupsApi.middleware,
        yearsEndApi.middleware
      ),
    preloadedState
  });

  const wrapper = ({ children }) => <Provider store={store}>{children}</Provider>;

  return { store, wrapper };
}
```

---

### Pattern 2: React Hook Form Not Mocked
**Affects**: MilitaryContribution tests, MilitaryContributionForm tests (17 failures)

**Issue**: Components use React Hook Form but tests don't mock it

**Global Solution**: Add React Hook Form mock to global test setup

```typescript
// In src/ui/src/test/setup.ts or at top of each affected test file
vi.mock("react-hook-form", () => ({
  useForm: vi.fn(() => ({
    control: {},
    handleSubmit: vi.fn((fn) => async (e) => {
      e?.preventDefault();
      return await fn();
    }),
    formState: { errors: {}, isValid: true, isDirty: false },
    watch: vi.fn((name) => undefined),
    setValue: vi.fn(),
    reset: vi.fn(),
    getValues: vi.fn(() => ({})),
    trigger: vi.fn(() => Promise.resolve(true))
  })),
  Controller: vi.fn(({ render, name, defaultValue }) => {
    return render({
      field: {
        onChange: vi.fn(),
        onBlur: vi.fn(),
        value: defaultValue || '',
        name,
        ref: vi.fn()
      },
      fieldState: {
        invalid: false,
        error: undefined
      },
      formState: {
        errors: {},
        isValid: true
      }
    });
  }),
  useFormContext: vi.fn(() => ({
    control: {},
    formState: { errors: {} }
  }))
}));
```

---

### Pattern 3: Missing Redux State Properties
**Affects**: ManageExecutiveHoursAndDollars tests (8 failures)

**Issue**: Mock store doesn't include all state properties required by components

**Solution**: Create comprehensive default state in test helper

```typescript
// In test utilities
export const DEFAULT_TEST_STATE = {
  security: {
    token: "test-token",
    user: { username: "testuser", roles: ["user"] }
  },
  navigation: {
    navigationData: null,
    profitYearSelectorData: {
      profitYears: [
        { profitYear: 2024, fiscalBeginDate: "2024-01-01", fiscalEndDate: "2024-12-31" }
      ],
      currentProfitYear: 2024
    }
  },
  yearsEnd: {
    selectedProfitYear: 2024,
    selectedProfitYearForDecemberActivities: 2024,
    selectedProfitYearForFiscalClose: 2024
  },
  fiscalClose: {
    selectedFiscalYear: 2024
  }
};

export function createMockStoreAndWrapper(customState = {}) {
  const mergedState = merge(DEFAULT_TEST_STATE, customState);
  // ... create store with mergedState
}
```

---

### Pattern 4: Async State Updates Not Wrapped in act()
**Affects**: UnForfeit tests, Termination tests (16 failures)

**Issue**: Components trigger state updates during or after render, causing React warnings

**Solution**: Always use waitFor for assertions after async operations

```typescript
// ❌ BAD - doesn't wait for async updates
it("should update", async () => {
  render(<Component />);
  await userEvent.click(button);
  expect(screen.getByText("Updated")).toBeInTheDocument(); // May fail
});

// ✅ GOOD - waits for async updates
it("should update", async () => {
  render(<Component />);
  await userEvent.click(button);

  await waitFor(() => {
    expect(screen.getByText("Updated")).toBeInTheDocument();
  });
});
```

---

## Priority Repair Plan

### Phase 1: Global Setup Fixes (High Impact)
**Estimated Impact**: Will fix ~30-40 tests

1. **Add RTK Query configuration to test setup**
   - Update `src/ui/src/test/setup.ts` with base URL
   - Modify `createMockStoreAndWrapper` to include all RTK Query APIs
   - **Fixes**: Termination, TerminationSearchFilter, Forfeit issues

2. **Add React Hook Form global mock**
   - Add to test setup or create reusable mock utility
   - **Fixes**: MilitaryContribution, MilitaryContributionForm issues

3. **Create DEFAULT_TEST_STATE with all common properties**
   - Include navigation.profitYearSelectorData
   - Include all common Redux state
   - **Fixes**: ManageExecutiveHoursAndDollars issues

4. **Mock DuplicateSsnGuard globally**
   - Prevent automatic API calls during tests
   - **Fixes**: Termination, TerminationSearchFilter API issues

**Estimated Time**: 4-6 hours

---

### Phase 2: Component-Specific Fixes (Medium Impact)
**Estimated Impact**: Will fix remaining 17 tests

5. **Fix UnForfeit async handling**
   - Wrap all assertions in waitFor
   - Use findBy queries for async elements
   - Mock form submission properly
   - **Fixes**: 9 UnForfeit tests

6. **Investigate and fix ForfeituresAdjustment**
   - Debug component rendering
   - Identify missing dependencies
   - Add required mocks
   - **Fixes**: 4 ForfeituresAdjustment tests

7. **Fix MilitaryContribution hook mocks**
   - Ensure all hoisted mocks return proper structures
   - Add missing context providers
   - **Fixes**: Remaining MilitaryContribution issues

**Estimated Time**: 3-4 hours

---

### Phase 3: Documentation & Prevention
**Estimated Impact**: Prevents future issues

8. **Update testing documentation**
   - Add RTK Query testing patterns
   - Document React Hook Form mocking
   - Add async testing guidelines

9. **Create test file template**
   - Include all common mocks
   - Show proper setup patterns
   - Include async testing examples

10. **Add ESLint rule for test patterns**
    - Warn on getBy without waitFor after user interaction
    - Require act() or waitFor for async operations

**Estimated Time**: 2-3 hours

---

## Detailed Fix Instructions

### Fix 1: Update Test Setup for RTK Query

**File**: `src/ui/src/test/setup.ts`

```typescript
import { beforeAll, vi } from "vitest";
import "@testing-library/jest-dom";

// Set base URL for RTK Query
beforeAll(() => {
  process.env.VITE_API_BASE_URL = "http://localhost:3000";
});

// Mock common components that make API calls
vi.mock("../components/DuplicateSsnGuard", () => ({
  default: vi.fn(({ children }) => <>{children}</>)
}));
```

---

### Fix 2: Update createMockStoreAndWrapper

**File**: `src/ui/src/test/utils.tsx` (or wherever it's defined)

```typescript
import { configureStore } from "@reduxjs/toolkit";
import { Provider } from "react-redux";
import { navigationStatusApi } from "../reduxstore/api/NavigationStatusApi";
import { lookupsApi } from "../reduxstore/api/LookupsApi";
import { yearsEndApi } from "../reduxstore/api/YearsEndApi";
import securityReducer from "../reduxstore/securitySlice";
import navigationReducer from "../reduxstore/navigationSlice";
import yearsEndReducer from "../reduxstore/yearsEndSlice";
// Import other reducers as needed

const DEFAULT_TEST_STATE = {
  security: {
    token: "test-token",
    user: { username: "testuser", roles: ["user"] }
  },
  navigation: {
    navigationData: null,
    profitYearSelectorData: {
      profitYears: [
        {
          profitYear: 2024,
          fiscalBeginDate: "2024-01-01",
          fiscalEndDate: "2024-12-31",
          isCurrent: true
        }
      ],
      currentProfitYear: 2024
    }
  },
  yearsEnd: {
    selectedProfitYear: 2024,
    selectedProfitYearForDecemberActivities: 2024,
    selectedProfitYearForFiscalClose: 2024
  }
};

export function createMockStoreAndWrapper(customState = {}) {
  // Deep merge custom state with defaults
  const preloadedState = {
    ...DEFAULT_TEST_STATE,
    ...customState,
    security: { ...DEFAULT_TEST_STATE.security, ...customState.security },
    navigation: { ...DEFAULT_TEST_STATE.navigation, ...customState.navigation },
    yearsEnd: { ...DEFAULT_TEST_STATE.yearsEnd, ...customState.yearsEnd }
  };

  const store = configureStore({
    reducer: {
      security: securityReducer,
      navigation: navigationReducer,
      yearsEnd: yearsEndReducer,
      // Add all RTK Query API reducers
      [navigationStatusApi.reducerPath]: navigationStatusApi.reducer,
      [lookupsApi.reducerPath]: lookupsApi.reducer,
      [yearsEndApi.reducerPath]: yearsEndApi.reducer
    },
    middleware: (getDefaultMiddleware) =>
      getDefaultMiddleware().concat(
        navigationStatusApi.middleware,
        lookupsApi.middleware,
        yearsEndApi.middleware
      ),
    preloadedState
  });

  const wrapper = ({ children }) => <Provider store={store}>{children}</Provider>;

  return { store, wrapper };
}
```

---

### Fix 3: Create React Hook Form Mock Utility

**File**: `src/ui/src/test/mocks/reactHookForm.ts`

```typescript
import { vi } from "vitest";

export function createReactHookFormMock() {
  return {
    useForm: vi.fn((config) => ({
      control: {},
      handleSubmit: vi.fn((fn) => async (e) => {
        e?.preventDefault();
        return await fn();
      }),
      formState: {
        errors: {},
        isValid: true,
        isDirty: false,
        isSubmitting: false
      },
      watch: vi.fn((name) => config?.defaultValues?.[name]),
      setValue: vi.fn(),
      reset: vi.fn(),
      getValues: vi.fn(() => config?.defaultValues || {}),
      trigger: vi.fn(() => Promise.resolve(true)),
      setError: vi.fn(),
      clearErrors: vi.fn()
    })),
    Controller: vi.fn(({ render, name, defaultValue, control }) => {
      return render({
        field: {
          onChange: vi.fn(),
          onBlur: vi.fn(),
          value: defaultValue || '',
          name,
          ref: vi.fn()
        },
        fieldState: {
          invalid: false,
          error: undefined,
          isDirty: false
        },
        formState: {
          errors: {},
          isValid: true
        }
      });
    }),
    useFormContext: vi.fn(() => ({
      control: {},
      formState: { errors: {} },
      watch: vi.fn(),
      setValue: vi.fn(),
      getValues: vi.fn(() => ({}))
    })),
    useController: vi.fn(({ name, defaultValue }) => ({
      field: {
        onChange: vi.fn(),
        onBlur: vi.fn(),
        value: defaultValue || '',
        name,
        ref: vi.fn()
      },
      fieldState: {
        invalid: false,
        error: undefined
      }
    }))
  };
}

// Usage in tests:
// vi.mock("react-hook-form", () => createReactHookFormMock());
```

---

### Fix 4: Add to Each Failing Test File

For **ManageExecutiveHoursAndDollars.test.tsx**:
```typescript
// Update mock state
const { wrapper } = createMockStoreAndWrapper({
  navigation: {
    profitYearSelectorData: {
      profitYears: [{ profitYear: 2024, fiscalBeginDate: "2024-01-01", fiscalEndDate: "2024-12-31" }],
      currentProfitYear: 2024
    }
  }
});
```

For **MilitaryContribution.test.tsx** and **MilitaryContributionForm.test.tsx**:
```typescript
// Add at top of file, before imports
vi.mock("react-hook-form", () => createReactHookFormMock());

// In beforeEach
mockUseMissiveAlerts.mockReturnValue({
  missiveAlerts: [],
  addAlert: vi.fn(),
  clearAlerts: vi.fn()
});
```

For **Termination.test.tsx** and **TerminationSearchFilter.test.tsx**:
```typescript
// Already mocked in setup.ts, but ensure tests use waitFor
await waitFor(() => {
  expect(screen.getByText("Expected")).toBeInTheDocument();
});
```

For **Forfeit.test.tsx**:
```typescript
// Use updated createMockStoreAndWrapper (already includes APIs)
const { wrapper } = createMockStoreAndWrapper({
  fiscalClose: {
    selectedFiscalYear: 2024,
    forfeitStatus: "Not Started"
  }
});
```

For **UnForfeit.test.tsx**:
```typescript
// Wrap all assertions in waitFor
it("should do something", async () => {
  render(<UnForfeit />, { wrapper });

  await userEvent.click(button);

  await waitFor(() => {
    expect(mockFn).toHaveBeenCalled();
  });
});
```

---

## Testing the Fixes

After applying each phase of fixes, run tests to verify:

```bash
# Run all tests
npm --prefix /Users/ashley/code/smart-profit-sharing/src/ui run test -- --run --reporter=verbose

# Run specific test file
npm --prefix /Users/ashley/code/smart-profit-sharing/src/ui run test -- --run ManageExecutiveHoursAndDollars.test.tsx

# Run tests in watch mode for debugging
npm --prefix /Users/ashley/code/smart-profit-sharing/src/ui run test -- ManageExecutiveHoursAndDollars.test.tsx
```

---

## Success Criteria

- ✅ All 1,730 tests pass
- ✅ No timeout errors
- ✅ No "act()" warnings in console
- ✅ No RTK Query configuration warnings
- ✅ Test execution time < 5 minutes total

---

## Notes

- **Estimated Total Time**: 9-13 hours to fix all issues
- **Dependencies**: None - can start immediately
- **Risk Level**: Low - all fixes are isolated to test files
- **Impact**: High - will restore 100% test pass rate

---

## Questions / Need Clarification

If you encounter issues while implementing these fixes:

1. **RTK Query APIs not found**: Check import paths match your project structure
2. **Mock not working**: Ensure vi.mock() is at module level, before imports
3. **Still timing out**: Increase timeout or add more waitFor() calls
4. **New errors appear**: May indicate actual component bugs found by better tests

---

**Last Updated**: 2025-11-01
**Next Review**: After Phase 1 completion
