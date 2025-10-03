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

## Test File Location (IMPORTANT)

**Test files MUST be located in the same directory as the file being tested.**

### Naming Convention

Test files follow the pattern: `[ComponentName].test.tsx`


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

## Test Structure Patterns

### Basic Hook Test

Testing custom React hooks using `renderHook`:

```typescript
import { renderHook } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import useDecemberFlowProfitYear from "./useDecemberFlowProfitYear";

describe("useDecemberFlowProfitYear", () => {
  it("should return the selected profit year from Redux store", () => {
    const mockStore = createMockStore(2024);
    const { result } = renderHook(() => useDecemberFlowProfitYear(), {
      wrapper: wrapper(mockStore)
    });

    expect(result.current).toBe(2024);
  });
});
```

### Hook Test with Redux Store

Testing hooks that use Redux state:

```typescript
import { configureStore } from "@reduxjs/toolkit";
import { renderHook } from "@testing-library/react";
import { Provider } from "react-redux";
import { beforeEach, describe, expect, it, vi } from "vitest";

describe("useFiscalCalendarYear", () => {
  let mockStore: ReturnType<typeof configureStore>;

  const createMockStore = (hasToken: boolean, accountingYearData = null) => {
    return configureStore({
      reducer: {
        security: () => ({ token: hasToken ? "mock-token" : null }),
        lookups: () => ({ accountingYearData })
      }
    });
  };

  const wrapper = ({ children }: { children: React.ReactNode }) =>
    <Provider store={mockStore}>{children}</Provider>;

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should return accounting year data from Redux store", () => {
    const mockData = {
      fiscalBeginDate: "2024-01-01",
      fiscalEndDate: "2024-12-31"
    };
    mockStore = createMockStore(true, mockData);

    const { result } = renderHook(() => useFiscalCalendarYear(), { wrapper });

    expect(result.current).toEqual(mockData);
  });
});
```

### Reducer Testing

Testing state management reducers:

```typescript
import { describe, expect, it } from "vitest";
import { initialState, masterInquiryReducer } from "./useMasterInquiryReducer";

describe("useMasterInquiryReducer", () => {
  describe("SEARCH_START", () => {
    it("should set search state to searching", () => {
      const params = {
        pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false }
      };
      const action = {
        type: "SEARCH_START" as const,
        payload: { params, isManual: true }
      };
      const newState = masterInquiryReducer(initialState, action);

      expect(newState.search.params).toEqual(params);
      expect(newState.search.isSearching).toBe(true);
      expect(newState.view.mode).toBe("searching");
    });
  });
});
```

### Selector Testing

Testing selector functions:

```typescript
describe("selectors", () => {
  describe("selectShowMemberGrid", () => {
    it("should return true when in multipleMembers mode with multiple results", () => {
      const state = {
        ...initialState,
        search: {
          ...initialState.search,
          results: { results: [{}, {}], total: 2 }
        },
        view: { mode: "multipleMembers" }
      };

      expect(selectShowMemberGrid(state)).toBe(true);
    });
  });
});
```

## Mocking Patterns

### Mocking External Modules

```typescript
import { vi } from "vitest";

// Mock custom hooks
vi.mock("./useDecemberFlowProfitYear", () => ({
  default: vi.fn()
}));

// Mock RTK Query hooks
vi.mock("reduxstore/api/LookupsApi", () => ({
  useLazyGetAccountingYearQuery: vi.fn(),
  useLazyGetAccountingRangeQuery: vi.fn()
}));
```

### Setting Mock Return Values

```typescript
import { useLazyGetAccountingYearQuery } from "reduxstore/api/LookupsApi";
import useDecemberFlowProfitYear from "./useDecemberFlowProfitYear";

vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);
vi.mocked(useLazyGetAccountingYearQuery).mockReturnValue([
  mockFetchAccountingYear,
  { isLoading: false } as any
]);
```

### Clearing Mocks

```typescript
beforeEach(() => {
  vi.clearAllMocks(); // Clear all mock call history
});
```


### Creating Test Wrappers

Common pattern for providing Redux store to tests:

```typescript
const wrapper = ({ children }: { children: React.ReactNode }) =>
  <Provider store={mockStore}>{children}</Provider>;

const { result } = renderHook(() => useMyHook(), { wrapper });
```
