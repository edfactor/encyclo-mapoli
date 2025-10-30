# Unit Testing React Hooks Guide

## Overview

This guide covers best practices for writing unit tests for React hooks in the smart-profit-sharing codebase. It focuses on proper mocking patterns, Redux integration, and common testing scenarios.

## Table of Contents

1. [Setup & Dependencies](#setup--dependencies)
2. [Test Structure](#test-structure)
3. [Mocking Patterns](#mocking-patterns)
4. [Redux Store Setup](#redux-store-setup)
5. [Common Test Scenarios](#common-test-scenarios)
6. [Anti-Patterns to Avoid](#anti-patterns-to-avoid)
7. [Example Tests](#example-tests)

---

## Setup & Dependencies

### Required Imports

```typescript
import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, act, waitFor } from "@testing-library/react";
import React from "react";
import { Provider } from "react-redux";
import { configureStore, type PreloadedState } from "@reduxjs/toolkit";
```

### Installation

These dependencies are already configured in the project:

- **vitest**: Testing framework and mocking utilities
- **@testing-library/react**: Hook testing utilities
- **@reduxjs/toolkit**: Redux store configuration (for tests)
- **react-redux**: Redux provider for tests

---

## Test Structure

### Basic Hook Test Template

```typescript
import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook } from "@testing-library/react";
import { useMyHook } from "./useMyHook";

// Step 1: Create mock variables at module scope
const mockTriggerFetch = vi.fn();

// Step 2: Mock the API/dependencies
vi.mock("reduxstore/api/MyApi", () => ({
  useLazyMyQuery: () => [mockTriggerFetch, {}]
}));

describe("useMyHook", () => {
  beforeEach(() => {
    vi.clearAllMocks(); // Reset mocks before each test
  });

  it("should initialize with correct default state", () => {
    const { result } = renderHook(() => useMyHook());

    expect(result.current.isLoading).toBe(false);
  });
});
```

### Key Principles

1. **Module-scope mock variables**: Create mocks at the module level before `vi.mock()` calls
2. **Clear mocks in beforeEach**: Reset all mocks before each test to ensure isolation
3. **Use renderHook**: Render hooks in isolation, not inside components
4. **Wrap state changes with act()**: When manually triggering state changes, wrap with `act()`

---

## Mocking Patterns

### ✅ Correct Pattern: Direct Mock Variables

Create mock function variables at module scope and return them directly in mock factories:

```typescript
// Step 1: Create mock variables at module scope
const mockTriggerSearch = vi.fn();
const mockTriggerUpdate = vi.fn();

// Step 2: Return them directly in vi.mock() factories
vi.mock("reduxstore/api/MyApi", () => ({
  useLazySearchQuery: () => [mockTriggerSearch, {}],
  useLazyUpdateMutation: () => [mockTriggerUpdate, {}]
}));

// Step 3: Use them directly in tests (no vi.mocked() needed)
it("should call API on search", async () => {
  const mockUnwrap = vi.fn().mockResolvedValue({ results: [] });
  mockTriggerSearch.mockReturnValue({ unwrap: mockUnwrap });

  const { result } = renderHook(() => useMyHook());

  expect(mockTriggerSearch).toHaveBeenCalled();
});
```

**Advantages**:
- No type casting required
- No `vi.mocked()` needed
- Cleaner code and better type safety
- Easier to debug

### ❌ Anti-Pattern: Using vi.mocked()

```typescript
// DON'T DO THIS - requires type casts and is error-prone
vi.mocked(MyApi.useLazySearchQuery).mockReturnValue([mockFn, {}] as unknown as ReturnType<typeof vi.fn>);
```

**Problems**:
- Requires `as unknown as` type casts
- Type system can't verify correctness
- Harder to maintain and debug
- Unnecessary complexity

---

## Redux Store Setup

### For Hooks that Need Redux State

```typescript
import { configureStore, type PreloadedState } from "@reduxjs/toolkit";
import securityReducer, { type SecurityState } from "reduxstore/slices/securitySlice";
import inquiryReducer, { type InquiryState } from "reduxstore/slices/inquirySlice";

// Step 1: Define RootState type
type RootState = {
  security: SecurityState;
  inquiry: InquiryState;
};

// Step 2: Define MockStoreState type (for clarity)
type MockStoreState = PreloadedState<RootState>;

// Step 3: Create store factory function
function createMockStore(preloadedState?: MockStoreState) {
  return configureStore<RootState>({
    reducer: {
      security: securityReducer,
      inquiry: inquiryReducer
    } as const, // Use 'as const' for better type inference
    preloadedState: preloadedState as RootState | undefined
  });
}

// Step 4: Create hook wrapper for provider
function renderHookWithProvider<T>(
  hook: () => T,
  preloadedState?: MockStoreState
) {
  const store = createMockStore(preloadedState || {
    security: { token: "mock-token", user: null },
    inquiry: { /* default inquiry state */ }
  });

  return renderHook(() => hook(), {
    wrapper: ({ children }: { children: React.ReactNode }) =>
      React.createElement(Provider, { store, children })
  });
}

// Step 5: Use in tests
it("should handle missing token", () => {
  const { result } = renderHookWithProvider(
    () => useMyHook(),
    { security: { token: null, user: null }, inquiry: {} }
  );

  expect(result.current.isLoading).toBe(false);
});
```

### Important Notes

- Use `configureStore<RootState>` for proper type inference
- Use `as const` on the reducer object to help TypeScript understand structure
- Cast `preloadedState` to `RootState | undefined` if needed
- Always provide default state in `renderHookWithProvider` calls
- RTK Query's lazy query hook returns different property names (isLoading vs isFetching) depending on the query type, and the mocks needed to match what the hooks actually destructure.

---

## Common Test Scenarios

### Testing Conditional Fetching (Token Required)

```typescript
it("should not fetch when token is missing", () => {
  mockTriggerFetch.mockReturnValue({});

  const { result } = renderHookWithProvider(
    () => useMyHook(),
    { security: { token: null, user: null } }
  );

  expect(result.current.isLoading).toBe(false);
  expect(mockTriggerFetch).not.toHaveBeenCalled();
});

it("should fetch when token is present", async () => {
  const mockUnwrap = vi.fn().mockResolvedValue({ data: "result" });
  mockTriggerFetch.mockReturnValue({ unwrap: mockUnwrap });

  const { result } = renderHookWithProvider(() => useMyHook());

  await waitFor(() => {
    expect(mockTriggerFetch).toHaveBeenCalled();
  });
});
```

### Testing State Changes

```typescript
it("should update state when data arrives", async () => {
  const mockData = { results: [{ id: 1, name: "Item" }] };
  const mockUnwrap = vi.fn().mockResolvedValue(mockData);
  mockTriggerFetch.mockReturnValue({ unwrap: mockUnwrap });

  const { result } = renderHookWithProvider(() => useMyHook());

  await waitFor(() => {
    expect(result.current.data).toEqual(mockData.results);
  });
});
```

### Testing Manual Refresh Functions

```typescript
it("should provide refresh method to manually trigger fetch", async () => {
  const mockUnwrap = vi.fn().mockResolvedValue({ results: [] });
  mockTriggerFetch.mockReturnValue({ unwrap: mockUnwrap });

  const { result } = renderHookWithProvider(() => useMyHook());

  const initialCallCount = mockTriggerFetch.mock.calls.length;

  // Manually trigger refresh
  act(() => {
    result.current.refresh();
  });

  await waitFor(() => {
    expect(mockTriggerFetch.mock.calls.length).toBeGreaterThan(initialCallCount);
  });
});
```

### Testing Error Handling

```typescript
it("should handle API errors gracefully", async () => {
  const mockUnwrap = vi.fn().mockRejectedValue(new Error("API Error"));
  mockTriggerFetch.mockReturnValue({ unwrap: mockUnwrap });

  const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

  const { result } = renderHookWithProvider(() => useMyHook());

  await waitFor(() => {
    expect(consoleErrorSpy).toHaveBeenCalled();
  });

  expect(result.current.data).toBeUndefined();
  expect(result.current.error).not.toBeNull();

  consoleErrorSpy.mockRestore();
});
```

### Testing Dependency Changes

```typescript
it("should re-fetch when dependencies change", async () => {
  const mockUnwrap = vi.fn().mockResolvedValue({ results: [] });
  mockTriggerFetch.mockReturnValue({ unwrap: mockUnwrap });

  const { rerender } = renderHookWithProvider(() =>
    useMyHook({ id: 1 })
  );

  await waitFor(() => {
    expect(mockTriggerFetch).toHaveBeenCalledTimes(1);
  });

  mockTriggerFetch.mockClear();

  rerender();

  // Hook should not refetch if dependencies haven't changed
  expect(mockTriggerFetch).not.toHaveBeenCalled();
});
```

---

## Anti-Patterns to Avoid

### ❌ Using vi.mocked() with Type Casts

```typescript
// DON'T DO THIS
vi.mocked(MyApi.useLazyFetch).mockReturnValue([
  vi.fn(),
  {}
] as unknown as ReturnType<typeof vi.fn>);
```

**Why**: Requires unsafe type casts and defeats the purpose of TypeScript.

### ❌ Forgetting to Clear Mocks

```typescript
// DON'T DO THIS
describe("useMyHook", () => {
  it("first test", () => { /* ... */ });
  it("second test", () => {
    // Mock state from first test may leak here!
  });
});
```

**Why**: Test isolation is critical. Always use `beforeEach(() => vi.clearAllMocks())`.

### ❌ Not Wrapping State Changes with act()

```typescript
// DON'T DO THIS
result.current.refresh(); // React will warn about updates outside act()

// DO THIS
act(() => {
  result.current.refresh();
});
```

### ❌ Using Loose Types with configureStore

```typescript
// DON'T DO THIS
const store = configureStore({
  reducer: { /* ... */ }
  // No type parameter - loses type safety
});

// DO THIS
type RootState = { /* ... */ };
const store = configureStore<RootState>({
  reducer: { /* ... */ } as const
});
```

### ❌ Not Using waitFor for Async Operations

```typescript
// DON'T DO THIS
mockUnwrap.mockResolvedValue(data);
const { result } = renderHook(() => useMyHook());
// Immediately checking result.current.data without waiting
expect(result.current.data).toEqual(data); // Often fails

// DO THIS
mockUnwrap.mockResolvedValue(data);
const { result } = renderHook(() => useMyHook());
await waitFor(() => {
  expect(result.current.data).toEqual(data);
});
```

---

## Example Tests

### Complete Example: useBeneficiaryKinds Hook

```typescript
import { describe, it, expect, vi, beforeEach } from "vitest";
import { renderHook, waitFor } from "@testing-library/react";
import React from "react";
import { Provider } from "react-redux";
import { useBeneficiaryKinds } from "./useBeneficiaryKinds";
import { BeneficiaryKindDto } from "reduxstore/types";
import { configureStore, type PreloadedState } from "@reduxjs/toolkit";
import securityReducer, { type SecurityState } from "reduxstore/slices/securitySlice";

// Mock data
const mockBeneficiaryKinds: BeneficiaryKindDto[] = [
  { id: "spouse", name: "Spouse" },
  { id: "child", name: "Child" }
];

// Step 1: Create mock variable
const mockTriggerGetBeneficiaryKind = vi.fn();

// Step 2: Mock the API
vi.mock("reduxstore/api/BeneficiariesApi", () => ({
  useLazyGetBeneficiaryKindQuery: () => [mockTriggerGetBeneficiaryKind, {}]
}));

// Step 3: Setup Redux types and store
type RootState = {
  security: SecurityState;
};

type MockStoreState = PreloadedState<RootState>;

function createMockStore(preloadedState?: MockStoreState) {
  return configureStore<RootState>({
    reducer: {
      security: securityReducer
    } as const,
    preloadedState: preloadedState as RootState | undefined
  });
}

function renderHookWithProvider<T>(
  hook: () => T,
  preloadedState?: MockStoreState
) {
  const store = createMockStore(preloadedState || {
    security: { token: "mock-token", user: null }
  });
  return renderHook(() => hook(), {
    wrapper: ({ children }: { children: React.ReactNode }) =>
      React.createElement(Provider, { store, children })
  });
}

// Tests
describe("useBeneficiaryKinds", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("should not fetch without token", () => {
    mockTriggerGetBeneficiaryKind.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue({ beneficiaryKindList: mockBeneficiaryKinds })
    });

    const { result } = renderHookWithProvider(
      () => useBeneficiaryKinds(),
      { security: { token: null, user: null } }
    );

    expect(result.current.isLoading).toBe(false);
    expect(result.current.beneficiaryKinds).toEqual([]);
    expect(mockTriggerGetBeneficiaryKind).not.toHaveBeenCalled();
  });

  it("should fetch beneficiary kinds with token", async () => {
    const mockUnwrap = vi.fn().mockResolvedValue({
      beneficiaryKindList: mockBeneficiaryKinds
    });

    mockTriggerGetBeneficiaryKind.mockReturnValue({
      unwrap: mockUnwrap
    });

    const { result } = renderHookWithProvider(() => useBeneficiaryKinds());

    expect(mockTriggerGetBeneficiaryKind).toHaveBeenCalledWith({});

    await waitFor(() => {
      expect(result.current.beneficiaryKinds.length).toBe(2);
    });

    expect(result.current.beneficiaryKinds).toEqual(mockBeneficiaryKinds);
  });

  it("should set error on API failure", async () => {
    const mockUnwrap = vi.fn().mockRejectedValue(new Error("Network error"));
    const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

    mockTriggerGetBeneficiaryKind.mockReturnValue({
      unwrap: mockUnwrap
    });

    const { result } = renderHookWithProvider(() => useBeneficiaryKinds());

    await waitFor(() => {
      expect(result.current.error).not.toBeNull();
    });

    expect(result.current.error).toBe("Failed to load beneficiary types");
    expect(consoleErrorSpy).toHaveBeenCalled();

    consoleErrorSpy.mockRestore();
  });
});
```

---

## Testing Checklist

Before submitting hook tests, verify:

- [ ] All mocks are created at module scope (no vi.mocked() calls)
- [ ] `beforeEach(() => vi.clearAllMocks())` is present
- [ ] Redux store is properly typed with `configureStore<RootState>`
- [ ] Async operations use `await waitFor()` or `await act()`
- [ ] State changes triggered manually are wrapped with `act()`
- [ ] Error handling is tested with spy on console
- [ ] Tests verify both success and failure paths
- [ ] Test names clearly describe what is being tested
- [ ] No TypeScript errors when running `npm run typecheck`
- [ ] Linting passes with `npm run lint`

---

## Resources

- [Vitest Documentation](https://vitest.dev/)
- [React Testing Library Hooks](https://testing-library.com/docs/react-testing-library/api/#renderhook)
- [Redux Testing Patterns](https://redux.js.org/usage/writing-tests)
- [Testing Async Code](https://testing-library.com/docs/dom-testing-library/api-async)

---

**Last Updated**: October 2024
