# Frontend Unit Testing Guide

This guide describes how unit tests work in the Smart Profit Sharing frontend application.

## Testing Framework

The frontend uses **Vitest** as the test runner with the following companion libraries:

- **Vitest** - Fast, Vite-native test runner
- **@testing-library/react** - React component testing utilities
- **@testing-library/jest-dom** - Custom matchers for DOM assertions
- **@testing-library/user-event** - User interaction simulation
- **jsdom** - DOM implementation for Node.js environment

### Key Configuration Details

- **Environment**: `jsdom` provides browser-like DOM environment
- **Test Files**: All files matching `**/*.test.tsx` pattern
- **Setup File**: `./src/test/setup.ts` runs before each test suite
- **Coverage**: Generates text and HTML coverage reports
- **JUnit Output**: Test results exported to `./FE_Tests/unit-test.xml` for CI/CD

## Running Tests

### Execute All Unit Tests

```bash
cd src/ui
npm run test
```

### Watch Mode (Development)

```bash
cd src/ui
npx vitest --watch
```

### UI Mode (Interactive)

```bash
cd src/ui
npx vitest --ui
```

### Run Specific Test File

```bash
cd src/ui
npx vitest src/hooks/useFiscalCalendarYear.test.tsx
```

Coverage reports are generated in:

- **Terminal**: Text summary
- **HTML**: `coverage/index.html` (open in browser for detailed view)

# React Unit Testing Strategy Guide

**Based on lessons learned from test refactoring and mock implementation patterns in the Smart Profit Sharing application**

## Table of Contents

1. [Philosophy & Principles](#philosophy--principles)
2. [Test Organization](#test-organization)
3. [Mocking Strategies](#mocking-strategies)
4. [Redux & State Management Testing](#redux--state-management-testing)
5. [Form Testing with React Hook Form](#form-testing-with-react-hook-form)
6. [Component Testing Patterns](#component-testing-patterns)
7. [Hook Testing Patterns](#hook-testing-patterns)
8. [Common Pitfalls & Solutions](#common-pitfalls--solutions)
9. [Test File Structure](#test-file-structure)
10. [Real-World Examples](#real-world-examples)

---

## Philosophy & Principles

### 1. Test What Users Do, Not Implementation Details

**Good ❌ → Better ✅**

```typescript
// ❌ BAD: Testing implementation details
it("should call setState with value", () => {
  const setStateSpy = vi.spyOn(React, 'useState');
  // ... tests internals
});

// ✅ GOOD: Testing user behavior
it("should update search results when user enters badge number", async () => {
  const user = userEvent.setup();
  render(<SearchFilter />);

  const input = screen.getByLabelText("Badge Number");
  await user.type(input, "12345");
  await user.click(screen.getByRole("button", { name: /search/i }));

  await waitFor(() => {
    expect(screen.getByText("Employee Found")).toBeInTheDocument();
  });
});
```

**Why**: Implementation details change frequently. User behavior is stable and what actually matters.

### 2. Use Selectors Based on Accessibility

**Priority Order** (best to worst):

1. **Accessible roles**: `screen.getByRole("button", { name: "Search" })`
2. **Labels & text**: `screen.getByLabelText("Email")`, `screen.getByText("Save")`
3. **Placeholders**: `screen.getByPlaceholderText("Enter date")`
4. **Test IDs**: `screen.getByTestId("submit-button")` (last resort)
5. **❌ Never use**: Class names, styles, component implementation details

```typescript
// ✅ Accessible and maintainable
const searchButton = screen.getByRole("button", { name: /search/i });
const dateInput = screen.getByLabelText("Rehire Begin Date");
const checkBox = screen.getByRole("checkbox", {
  name: /exclude zero balance/i,
});

// ❌ Brittle and inaccessible
const searchButton = screen.getByTestId("date-picker-Rehire Begin Date");
const dateInput = document.querySelector(".MuiInput");
```

### 3. Test Behavior, Not State

Always query the DOM for the current state, not internal component state.

```typescript
// ❌ BAD: Checking internal state
const { result } = renderHook(() => useMyHook());
expect(result.current.isLoading).toBe(false);

// ✅ GOOD: Checking DOM state
render(<MyComponent />);
expect(screen.queryByText("Loading...")).not.toBeInTheDocument();
// or
expect(screen.getByRole("button")).not.toBeDisabled();
```

---

## Test Organization

### Directory Structure

```
src/pages/DecemberActivities/UnForfeit/
├── UnForfeit.tsx                              # Component
├── UnForfeitSearchFilter.tsx                  # Sub-component
├── UnForfeitGrid.tsx                          # Sub-component
├── hooks/
│   ├── useUnForfeitGrid.ts                    # Custom hook
│   └── __test__/                              # Tests for hooks
│       └── useUnForfeitGrid.test.ts
└── __test__/                                  # Tests for components
    ├── UnForfeit.test.tsx
    ├── UnForfeitSearchFilter.test.tsx
    └── UnForfeitGrid.test.tsx
```

**Best Practice**: Tests go in `__test__` subdirectories, organized parallel to source structure.

### Test File Naming Convention

- **Component**: `ComponentName.test.tsx`
- **Hook**: `useHookName.test.ts`
- **Utility**: `utilityName.test.ts`
- **Location**: `src/pages/Feature/__test__/FileName.test.tsx`

### Test Grouping with Describe Blocks

```typescript
describe("UnForfeitSearchFilter", () => {
  describe("Rendering", () => {
    it("should render form fields", () => {});
    it("should render action buttons", () => {});
  });

  describe("User Interactions", () => {
    it("should update form when user enters date", () => {});
    it("should call onSearch when search button clicked", () => {});
  });

  describe("Form Validation", () => {
    it("should disable search when form invalid", () => {});
    it("should show error message for invalid date", () => {});
  });

  describe("Redux Integration", () => {
    it("should dispatch search parameters to store", () => {});
  });
});
```

---

## Mocking Strategies

### Core Principle: Mock External Dependencies, Not Your Code

```typescript
// ✅ DO: Mock external libraries and APIs
vi.mock("smart-ui-library"); // Third-party UI library
vi.mock("../../../utils/FormValidators"); // Utility module
vi.mock("../../../reduxstore/api/YearsEndApi"); // API

// ❌ DON'T: Mock your own components/hooks unless necessary
// Don't mock: UnForfeitSearchFilter, useUnForfeitState, etc.
```

### 1. Mocking Library Modules (Utilities)

**Pattern**: Use `async` to import real module, then override specific exports.

```typescript
vi.mock("../../../utils/FormValidators", async () => {
  const yup = await import("yup");

  return {
    dateStringValidator: (minYear, maxYear, fieldName) => {
      return yup.default
        .string()
        .nullable()
        .required(`${fieldName} is required`)
        .test("valid-date", `${fieldName} must be valid`, function (value) {
          if (!value) return false;
          return (
            /^\d{1,2}\/\d{1,2}\/\d{4}$/.test(value) ||
            /^\d{4}-\d{2}-\d{2}$/.test(value)
          );
        });
    },
    mmDDYYFormat: (date) => {
      if (!date) return "";
      // Implementation
      return formattedDate;
    },
  };
});
```

**Why this works**: You're providing real validation behavior (using Yup) while controlling implementation.

### 2. Mocking UI Library Components

**Pattern**: Create a minimal mock component that preserves props.

```typescript
vi.mock("smart-ui-library", () => ({
  SearchAndReset: vi.fn(({
    handleSearch,
    handleReset,
    disabled,
    isFetching
  }) => (
    <div data-testid="search-and-reset">
      <button
        data-testid="search-btn"
        onClick={handleSearch}
        disabled={disabled || isFetching}
      >
        Search
      </button>
      <button
        data-testid="reset-btn"
        onClick={handleReset}
      >
        Reset
      </button>
      {isFetching && <span data-testid="loading">Loading...</span>}
    </div>
  ))
}));
```

**Key Points**:

- Pass through all props your component uses
- Make mock testable (include data-testid attributes)
- Keep it simple - no need to replicate full component
- Preserve event handlers (onClick, onChange, etc.)

### 3. Mocking RTK Query Hooks (Advanced)

**⚠️ Most Complex Pattern - Requires Careful Setup**

RTK Query hooks are async and need proper promise handling.

```typescript
// Step 1: Create mock functions at module scope using vi.hoisted()
const { mockTriggerSearch, mockTriggerStatus } = vi.hoisted(() => ({
  mockTriggerSearch: vi.fn(),
  mockTriggerStatus: vi.fn(),
}));

// Step 2: Mock the entire module
vi.mock("../../../reduxstore/api/YearsEndApi", () => ({
  useLazyGetUnForfeitsQuery: vi.fn(() => [
    mockTriggerSearch, // The trigger function
    { isFetching: false }, // The hook state
  ]),
  useLazyGetProfitMasterStatusQuery: vi.fn(() => [
    mockTriggerStatus,
    { isFetching: false },
  ]),
}));

// Step 3: Set up return values in beforeEach
beforeEach(() => {
  vi.clearAllMocks();

  mockTriggerSearch.mockReturnValue({
    unwrap: vi.fn().mockResolvedValue({
      results: mockData,
      total: 1,
    }),
  });

  mockTriggerStatus.mockReturnValue({
    unwrap: vi.fn().mockResolvedValue({
      updatedBy: "John Doe",
      updatedTime: "2024-01-15",
    }),
  });
});
```

**Critical Points**:

- Use `vi.hoisted()` before `vi.mock()` - this allows using mock variables in the mock definition
- RTK Query lazy hooks return `[triggerFunction, stateObject]` - replicate this structure
- Return value must have `.unwrap()` method that returns a Promise
- Set up different return values for success/error cases

**Testing RTK Query Calls**:

```typescript
it("should call search with correct parameters", async () => {
  render(<UnForfeitSearchFilter {...props} />, { wrapper });

  const searchButton = screen.getByRole("button", { name: /search/i });
  await userEvent.click(searchButton);

  await waitFor(() => {
    expect(mockTriggerSearch).toHaveBeenCalledWith({
      beginningDate: "01/01/2024",
      endingDate: "12/31/2024",
      profitYear: 2024,
      excludeZeroBalance: false
    });
  });
});
```

---

## Redux & State Management Testing

### 1. Creating a Test Store

**Pattern**: Use `createMockStoreAndWrapper` helper or manually configure.

```typescript
// Option 1: Use helper from project
import { createMockStoreAndWrapper } from "../../../../test";

const { wrapper } = createMockStoreAndWrapper({
  yearsEnd: {
    selectedProfitYear: 2024,
  },
  security: {
    token: "test-token",
  },
});

// Option 2: Manual configuration (if helper not available)
function createTestStore(preloadedState) {
  return configureStore({
    reducer: {
      security: securityReducer,
      yearsEnd: yearsEndReducer,
      distribution: distributionReducer,
    },
    preloadedState: {
      security: { token: "test-token", ...preloadedState?.security },
      yearsEnd: { selectedProfitYear: 2024, ...preloadedState?.yearsEnd },
      distribution: preloadedState?.distribution || {},
    },
  });
}
```

### 2. Wrapping Components in Redux Provider

```typescript
// For component rendering
const { wrapper } = createMockStoreAndWrapper({
  yearsEnd: { selectedProfitYear: 2024 }
});

render(<UnForfeitSearchFilter {...props} />, { wrapper });

// For hook testing
const { result } = renderHook(
  () => useProfitShareEditUpdate(),
  { wrapper }
);
```

### 3. Testing Redux Dispatch

```typescript
it("should dispatch setUnForfeitsQueryParams when searching", async () => {
  const { store, wrapper } = createMockStoreAndWrapper({});

  render(<UnForfeitSearchFilter {...props} />, { wrapper });

  const searchButton = screen.getByRole("button", { name: /search/i });
  await userEvent.click(searchButton);

  await waitFor(() => {
    // Access store state after dispatch
    const state = store.getState();
    expect(state.yearsEnd.unForfeitsQueryParams).toEqual(
      expect.objectContaining({
        profitYear: 2024
      })
    );
  });
});
```

### 4. Redux Selectors in Tests

Test that your component correctly consumes Redux state:

```typescript
it("should disable search when hasUnsavedChanges is true", () => {
  const { wrapper } = createMockStoreAndWrapper({
    yearsEnd: {
      hasUnsavedChangesInUnForfeit: true  // Key state for this test
    }
  });

  render(<UnForfeitSearchFilter {...props} />, { wrapper });

  const searchButton = screen.getByRole("button", { name: /search/i });
  expect(searchButton).toBeDisabled();
});
```

---

## Form Testing with React Hook Form

### 1. Testing Form Validation

```typescript
it("should show error when date field is empty", async () => {
  render(<UnForfeitSearchFilter {...props} />, { wrapper });

  const searchButton = screen.getByRole("button", { name: /search/i });

  // Trigger form submission (validation)
  await userEvent.click(searchButton);

  // Check for error message
  await waitFor(() => {
    expect(screen.getByText(/Beginning Date is required/i)).toBeInTheDocument();
  });
});

it("should validate end date is after begin date", async () => {
  const user = userEvent.setup();
  render(<UnForfeitSearchFilter {...props} />, { wrapper });

  const beginInput = screen.getByLabelText("Rehire Begin Date");
  const endInput = screen.getByLabelText("Rehire Ending Date");

  await user.type(beginInput, "06/15/2024");
  await user.type(endInput, "01/01/2024"); // Before begin date!

  const searchButton = screen.getByRole("button", { name: /search/i });
  await user.click(searchButton);

  await waitFor(() => {
    expect(screen.getByText(/Ending date must be.*after.*beginning/i)).toBeInTheDocument();
  });
});
```

### 2. Testing Form Submission

```typescript
it("should call onSearch when form is valid and submitted", async () => {
  const mockOnSearch = vi.fn();
  const user = userEvent.setup();

  render(
    <UnForfeitSearchFilter
      {...props}
      onSearch={mockOnSearch}
    />,
    { wrapper }
  );

  const searchButton = screen.getByRole("button", { name: /search/i });

  // Form should be valid by default (has default dates)
  await user.click(searchButton);

  await waitFor(() => {
    expect(mockOnSearch).toHaveBeenCalled();
  });
});
```

### 3. Testing Form Reset

```typescript
it("should clear form fields when reset button clicked", async () => {
  const user = userEvent.setup();
  render(<UnForfeitSearchFilter {...props} />, { wrapper });

  // Get initial value
  const beginInput = screen.getByDisplayValue("01/01/2024");

  // Clear and type new value
  await user.clear(beginInput);
  await user.type(beginInput, "06/15/2024");
  expect(beginInput).toHaveValue("06/15/2024");

  // Click reset
  const resetButton = screen.getByRole("button", { name: /reset/i });
  await user.click(resetButton);

  // Should revert to default
  await waitFor(() => {
    expect(beginInput).toHaveValue("01/01/2024");
  });
});
```

---

## Component Testing Patterns

### 1. Page/Container Components

Test the integration of child components and state management.

```typescript
describe("UnForfeit Page", () => {
  it("should render search filter and grid", () => {
    render(<UnForfeit />, { wrapper });

    // Check main components are present
    expect(screen.getByRole("textbox", { name: /begin date/i })).toBeInTheDocument();
    expect(screen.getByRole("table")).toBeInTheDocument();
  });

  it("should pass correct props from page to search filter", () => {
    render(<UnForfeit />, { wrapper });

    const searchFilter = screen.getByTestId("search-filter");
    expect(searchFilter).toHaveProperty("fiscalData", mockFiscalData);
  });

  it("should call search when filter emits search event", async () => {
    const user = userEvent.setup();
    render(<UnForfeit />, { wrapper });

    const searchButton = screen.getByRole("button", { name: /search/i });
    await user.click(searchButton);

    // Grid should update/display results
    await waitFor(() => {
      expect(screen.getByRole("table")).toBeInTheDocument();
    });
  });
});
```

### 2. Form Components

Focus on user interactions and validation.

```typescript
describe("MilitaryContributionForm", () => {
  it("should accept user input and submit", async () => {
    const mockOnSubmit = vi.fn();
    const user = userEvent.setup();

    render(
      <MilitaryContributionForm onSubmit={mockOnSubmit} />,
      { wrapper }
    );

    // Fill form
    const amountInput = screen.getByLabelText(/amount/i);
    await user.type(amountInput, "1000");

    const typeSelect = screen.getByLabelText(/contribution type/i);
    await user.selectOption(typeSelect, "hourly");

    // Submit
    const submitButton = screen.getByRole("button", { name: /submit/i });
    await user.click(submitButton);

    expect(mockOnSubmit).toHaveBeenCalledWith(
      expect.objectContaining({
        amount: 1000,
        type: "hourly"
      })
    );
  });

  it("should prevent submission when amount is zero", async () => {
    const mockOnSubmit = vi.fn();
    const user = userEvent.setup();

    render(
      <MilitaryContributionForm onSubmit={mockOnSubmit} />,
      { wrapper }
    );

    const submitButton = screen.getByRole("button", { name: /submit/i });

    // Try to submit with zero amount
    await user.click(submitButton);

    // Should show validation error
    expect(screen.getByText(/amount must be greater than zero/i)).toBeInTheDocument();

    // Should not call submit
    expect(mockOnSubmit).not.toHaveBeenCalled();
  });
});
```

---

## Hook Testing Patterns

### 1. Basic Hook Testing

Use `renderHook` with proper Redux setup.

```typescript
it("should return initial state", () => {
  const { wrapper } = createMockStoreAndWrapper({
    yearsEnd: { selectedProfitYear: 2024 },
  });

  const { result } = renderHook(() => useUnForfeitState(), { wrapper });

  expect(result.current.state.initialSearchLoaded).toBe(false);
  expect(result.current.state.resetPageFlag).toBe(false);
});
```

### 2. Hook State Updates

```typescript
it("should update state when setInitialSearchLoaded called", () => {
  const { wrapper } = createMockStoreAndWrapper({});
  const { result } = renderHook(() => useUnForfeitState(), { wrapper });

  // Initial state
  expect(result.current.state.initialSearchLoaded).toBe(false);

  // Update state
  act(() => {
    result.current.actions.setInitialSearchLoaded(true);
  });

  // Check updated state
  expect(result.current.state.initialSearchLoaded).toBe(true);
});
```

### 3. Hook with Side Effects

```typescript
it("should fetch data on mount", async () => {
  const mockTrigger = vi.fn().mockReturnValue({
    unwrap: vi.fn().mockResolvedValue(mockData),
  });

  vi.mock("../api", () => ({
    useLazyFetchQuery: vi.fn(() => [mockTrigger, { isFetching: false }]),
  }));

  const { wrapper } = createMockStoreAndWrapper({});

  renderHook(() => useMyHook(), { wrapper });

  await waitFor(() => {
    expect(mockTrigger).toHaveBeenCalled();
  });
});
```

### 4. Testing Hook with useCallback/useMemo

```typescript
it("should memoize expensive calculation", () => {
  const { wrapper } = createMockStoreAndWrapper({});
  const { result, rerender } = renderHook(() => useMyHook(), { wrapper });

  const firstValue = result.current.expensiveValue;

  // Re-render with same props
  rerender();

  const secondValue = result.current.expensiveValue;

  // Should be same reference (memoized)
  expect(firstValue).toBe(secondValue);
});
```

---

## Common Pitfalls & Solutions

### Pitfall 1: Not Using Redux Provider

**❌ FAILS**:

```typescript
it("should work", () => {
  render(<MyComponent />); // No Redux provider!
  // Error: "could not find react-redux context value"
});
```

**✅ WORKS**:

```typescript
it("should work", () => {
  const { wrapper } = createMockStoreAndWrapper({});
  render(<MyComponent />, { wrapper });
  // Now Redux context is available
});
```

### Pitfall 2: Brittle Element Selectors

**❌ BRITTLE**:

```typescript
it("should show data", () => {
  // These break if internal structure changes
  const element = document.querySelector(
    ".date-picker-container .MuiInput-root",
  );
  const button = document.querySelectorAll("button")[3];
});
```

**✅ ROBUST**:

```typescript
it("should show data", () => {
  // These are resilient to changes
  const dateInput = screen.getByLabelText("Rehire Begin Date");
  const searchButton = screen.getByRole("button", { name: /search/i });
});
```

### Pitfall 3: Testing Implementation Instead of Behavior

**❌ IMPLEMENTATION**:

```typescript
it("should set state", () => {
  const { result } = renderHook(() => useMyHook());

  // Testing the internal implementation
  expect(result.current.isLoading).toBe(true);
  expect(result.current.error).toBeNull();
});
```

**✅ BEHAVIOR**:

```typescript
it("should show loading state", () => {
  render(<MyComponent />);

  // Testing what the user sees
  expect(screen.getByText("Loading...")).toBeInTheDocument();
  expect(screen.queryByText("Error")).not.toBeInTheDocument();
});
```

### Pitfall 4: Async Code Without Proper Waiting

**❌ FAILS**:

```typescript
it("should load data", async () => {
  render(<MyComponent />);

  await userEvent.click(screen.getByRole("button", { name: /load/i }));

  // ❌ Data not loaded yet!
  expect(screen.getByText("Loaded Data")).toBeInTheDocument();
});
```

**✅ WORKS**:

```typescript
it("should load data", async () => {
  render(<MyComponent />);

  await userEvent.click(screen.getByRole("button", { name: /load/i }));

  // ✅ Wait for data to appear
  await waitFor(() => {
    expect(screen.getByText("Loaded Data")).toBeInTheDocument();
  });
});
```

### Pitfall 5: Mock Not Set Up Correctly

**❌ DOESN'T WORK** (vi.mock must be at module level):

```typescript
it("should use mock", () => {
  // ❌ Too late - mocks must be defined before imports
  vi.mock("../utils");

  const { someFunction } = require("../utils");
});
```

**✅ WORKS**:

```typescript
// ✅ At module level, before any imports
vi.mock("../utils", () => ({
  someFunction: vi.fn(),
}));

// Then import and test
import { someFunction } from "../utils";

it("should use mock", () => {
  expect(someFunction).toBeDefined();
});
```

### Pitfall 6: Not Clearing Mocks Between Tests

**❌ TEST POLLUTION**:

```typescript
describe("Tests", () => {
  const mockFn = vi.fn();

  it("first test", () => {
    mockFn();
  });

  it("second test", () => {
    // ❌ mockFn was called in first test!
    expect(mockFn).toHaveBeenCalledTimes(1); // Fails!
  });
});
```

**✅ CLEAN TESTS**:

```typescript
describe("Tests", () => {
  const mockFn = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks(); // ✅ Clear between tests
  });

  it("first test", () => {
    mockFn();
  });

  it("second test", () => {
    expect(mockFn).toHaveBeenCalledTimes(0); // Passes!
  });
});
```

---

## Test File Structure

### Complete Example: UnForfeitSearchFilter Test

```typescript
// 1. Imports - Testing utilities first, then application code
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../../../test";

// 2. Mocks - Define at module level with vi.hoisted()
const { mockTriggerSearch } = vi.hoisted(() => ({
  mockTriggerSearch: vi.fn()
}));

vi.mock("../../../reduxstore/api/YearsEndApi", () => ({
  useLazyGetUnForfeitsQuery: vi.fn(() => [
    mockTriggerSearch,
    { isFetching: false }
  ])
}));

vi.mock("../../../utils/FormValidators", async () => {
  const yup = await import("yup");
  return {
    dateStringValidator: () => yup.default.string().nullable(),
    // ... other validators
  };
});

// 3. Import component after mocks
import UnForfeitSearchFilter from "../UnForfeitSearchFilter";

// 4. Setup and helper data
const mockFiscalData = {
  fiscalBeginDate: "2024-01-01",
  fiscalEndDate: "2024-12-31"
};

const mockOnSearch = vi.fn();

// 5. Describe block with beforeEach
describe("UnForfeitSearchFilter", () => {
  let wrapper: ReturnType<typeof createMockStoreAndWrapper>["wrapper"];

  beforeEach(() => {
    vi.clearAllMocks();
    const storeAndWrapper = createMockStoreAndWrapper({
      yearsEnd: { selectedProfitYear: 2024 }
    });
    wrapper = storeAndWrapper.wrapper;
  });

  // 6. Test groups by functionality
  describe("Rendering", () => {
    it("should render form fields", () => {
      render(
        <UnForfeitSearchFilter
          fiscalData={mockFiscalData}
          onSearch={mockOnSearch}
          setInitialSearchLoaded={vi.fn()}
          hasUnsavedChanges={false}
          setHasUnsavedChanges={vi.fn()}
        />,
        { wrapper }
      );

      expect(screen.getByLabelText("Rehire Begin Date")).toBeInTheDocument();
      expect(screen.getByLabelText("Rehire Ending Date")).toBeInTheDocument();
    });
  });

  describe("User Interactions", () => {
    it("should call onSearch when search button clicked", async () => {
      const user = userEvent.setup();
      render(
        <UnForfeitSearchFilter {...props} />,
        { wrapper }
      );

      const searchButton = screen.getByRole("button", { name: /search/i });
      await user.click(searchButton);

      await waitFor(() => {
        expect(mockOnSearch).toHaveBeenCalled();
      });
    });
  });

  describe("Form Validation", () => {
    it("should disable search when form invalid", () => {
      // Setup with invalid state
      render(
        <UnForfeitSearchFilter {...props} />,
        { wrapper }
      );

      const searchButton = screen.getByRole("button", { name: /search/i });
      expect(searchButton).toBeDisabled();
    });
  });

  describe("Redux Integration", () => {
    it("should dispatch search params to store", async () => {
      const user = userEvent.setup();
      const { store, wrapper } = createMockStoreAndWrapper({});

      render(
        <UnForfeitSearchFilter {...props} />,
        { wrapper }
      );

      const searchButton = screen.getByRole("button", { name: /search/i });
      await user.click(searchButton);

      await waitFor(() => {
        const state = store.getState();
        expect(state.yearsEnd.unForfeitsQueryParams).toBeDefined();
      });
    });
  });
});
```

---

## Real-World Examples

### Example 1: Testing RTK Query with Hook

From `useProfitShareEditUpdate.test.tsx`:

```typescript
const { mockApplyMaster } = vi.hoisted(() => ({
  mockApplyMaster: vi.fn(),
}));

vi.mock("../../../reduxstore/api/YearsEndApi", () => ({
  useGetMasterApplyMutation: vi.fn(() => [mockApplyMaster]),
}));

beforeEach(() => {
  mockApplyMaster.mockReturnValue({
    unwrap: vi.fn().mockResolvedValue({
      employeesEffected: 100,
      beneficiariesEffected: 50,
      etvasEffected: 150,
    }),
  });
});

it("should handle save success", async () => {
  const { wrapper } = createMockStoreAndWrapper({});
  const { result } = renderHook(() => useProfitShareEditUpdate(), { wrapper });

  expect(result.current.changesApplied).toBe(false);

  await act(async () => {
    await result.current.saveAction();
  });

  expect(result.current.changesApplied).toBe(true);
});
```

### Example 2: Form Validation Testing

From `UnForfeitSearchFilter.test.tsx`:

```typescript
it("should validate end date is after begin date", async () => {
  const user = userEvent.setup();
  render(<UnForfeitSearchFilter {...props} />, { wrapper });

  const beginDateInput = screen.getByLabelText("Rehire Begin Date");
  const endDateInput = screen.getByLabelText("Rehire Ending Date");

  await user.clear(beginDateInput);
  await user.type(beginDateInput, "06/15/2024");

  await user.clear(endDateInput);
  await user.type(endDateInput, "01/01/2024"); // Before begin date!

  const searchButton = screen.getByRole("button", { name: /search/i });
  await user.click(searchButton);

  await waitFor(() => {
    expect(screen.getByText(/Ending date must be.*after.*beginning/i))
      .toBeInTheDocument();
  });
});
```

### Example 3: Redux State Testing

From `useDuplicateNamesAndBirthdays.test.ts`:

```typescript
it("should not fetch when token is missing", () => {
  const { wrapper } = createMockStoreAndWrapper({
    security: { token: null, user: null },
    yearsEnd: {
      selectedProfitYearForDecemberActivities: 2024,
      yearsEndData: null,
    },
  });

  const { result } = renderHook(() => useDuplicateNamesAndBirthdays(), {
    wrapper,
  });

  expect(result.current.isSearching).toBe(false);
});
```

---
