# Frontend Unit Test Errors Report

**Test Run Date**: 2025-11-01
**Test Framework**: Vitest v3.2.4
**Result**: 83 failed | 1741 passed (1824 total tests)
**Failed Test Files**: 8 of 100

---

## Summary of Error Categories

1. **MUI Grid Migration (Warnings)** - Legacy `xs` prop usage
2. **React `act()` Warnings** - Unhandled state updates in tests
3. **Missing DOM Elements** - Elements not found in rendered output
4. **DOM Method Not Available** - `scrollIntoView` not mocked in jsdom
5. **Async/Timing Issues** - Race conditions in async operations

---

## Detailed Error Breakdown

### 1. MUI Grid `xs` Prop Deprecation (Non-Critical Warnings)

**Affected Files**:
- `src/pages/DecemberActivities/MilitaryContribution/MilitaryContributionForm.test.tsx`
- `src/pages/DecemberActivities/MilitaryContribution/__test__/MilitaryContributionForm.test.tsx`

**Error Message**:
```
MUI Grid: The `xs` prop has been removed. See https://mui.com/material-ui/migration/upgrade-to-grid-v2/
for migration instructions.
```

**Root Cause**: The component uses the deprecated MUI Grid v1 API with the `xs` prop. This prop was removed in Material-UI Grid v2.

**Impact**: These are warnings only—tests still pass, but indicate the component code needs updating.

**How to Fix**:
1. Update `MilitaryContributionForm.tsx` to use MUI Grid v2 syntax
2. Replace `xs={12}` with `sx={{ xs: 12 }}` or use the new `size` prop
3. See [MUI Grid v2 Migration Guide](https://mui.com/material-ui/migration/upgrade-to-grid-v2/)

**Example**:
```typescript
// OLD (Grid v1)
<Grid xs={12} sm={6} md={4}>

// NEW (Grid v2)
<Grid size={{ xs: 12, sm: 6, md: 4 }}>
```

---

### 2. React `act()` Warnings in MilitaryContribution Tests

**Affected File**: `src/pages/DecemberActivities/MilitaryContribution/__test__/MilitaryContribution.test.tsx`

**Affected Tests** (8+ warnings from):
- "should render search filter accordion"
- "should render military contribution grid when member selected"
- "should display frozen year warning when year is frozen"
- "should not display frozen year warning when year is not frozen"
- "should display read-only info when status is read-only"
- "should disable add contribution button in read-only mode"
- "should enable add contribution button when not read-only"

**Error Message**:
```
An update to MilitaryContributionContent inside a test was not wrapped in act(...).

When testing, code that causes React state updates should be wrapped into act(...):

act(() => {
  /* fire events that update state */
});
/* assert on the output */
```

**Root Cause**: The test renders a component that updates state asynchronously (e.g., from Redux, API calls, or effects) without wrapping those updates in `act()`. This is usually caused by:
- Async state updates from Redux that aren't awaited
- `useEffect` hooks firing after render
- Delayed state updates via `setTimeout`

**Impact**: Tests still pass, but React/Testing Library warns about potential race conditions and unpredictable test behavior.

**How to Fix**:
1. **Wrap async operations in `act()`**:
   ```typescript
   await act(async () => {
     await user.click(searchButton);
   });
   ```

2. **Use `waitFor()` for async state updates**:
   ```typescript
   await waitFor(() => {
     expect(screen.getByText("Expected Text")).toBeInTheDocument();
   });
   ```

3. **Ensure RTK Query operations are mocked properly** with resolved promises

4. **Check `beforeEach` setup** - Mock RTK Query mutations/queries that fire on component mount

**Example Fix**:
```typescript
// BEFORE (causes act warning)
it("should render grid when member selected", () => {
  render(<MilitaryContribution />, { wrapper });
  // State updates happen here without awaiting
  expect(screen.getByRole("table")).toBeInTheDocument();
});

// AFTER (proper handling)
it("should render grid when member selected", async () => {
  const user = userEvent.setup();
  render(<MilitaryContribution />, { wrapper });

  await waitFor(() => {
    expect(screen.getByRole("table")).toBeInTheDocument();
  });
});
```

---

### 3. Missing Test Data Element Error

**Affected File**: `src/pages/FiscalClose/Forfeit/__test__/Forfeit.test.tsx`

**Error Message**:
```
Unable to find an element with the text: false. This could be because the text is broken up
by multiple elements. In this case, you can provide a function for your text matcher to make
your matcher more flexible.

Ignored nodes: comments, script, style
<body>
  <div>
    <!-- Full rendered HTML shown here -->
  </body>
```

**Failing Test**:
```typescript
const searchFilterIsSearching = screen.getByTestId("filter-is-searching");
expect(searchFilterIsSearching).toHaveTextContent("false");
```

**Root Cause**: The test expects to find an element with `data-testid="filter-is-searching"`, but:
1. The element doesn't exist in the rendered output
2. The element may not be rendering the expected text "false"
3. The data-testid may have changed or the component structure changed

**Impact**: Test fails completely - cannot find the element being queried.

**How to Fix**:
1. **Verify the element exists** in the actual component being tested:
   ```typescript
   // In Forfeit.tsx or its child components, check if this element is rendered:
   <div data-testid="filter-is-searching">{isSearching ? "true" : "false"}</div>
   ```

2. **If the element was renamed or removed**, update the test to query the correct element:
   ```typescript
   // Option 1: Use a different query if element is still there
   const searchButton = screen.getByRole("button", { name: /search/i });

   // Option 2: Query by accessible role instead
   const status = screen.getByText(/searching/i);
   ```

3. **If the component structure changed**, refactor the test to query the component output differently:
   ```typescript
   // Instead of checking a status element, verify behavior:
   expect(screen.getByRole("button")).toBeDisabled(); // Loading state
   await waitFor(() => {
     expect(screen.getByRole("table")).toBeInTheDocument(); // Results loaded
   });
   ```

4. **Add the missing element to the component** if it should exist:
   ```typescript
   // In Forfeit.tsx or SearchFilter component:
   <div data-testid="filter-is-searching">
     {isSearching ? "true" : "false"}
   </div>
   ```

---

### 4. DOM Method Not Available: `scrollIntoView`

**Affected File**: `src/pages/DecemberActivities/ProfitShareReport/__tests__/ProfitShareReport.test.tsx`

**Error Message**:
```
TypeError: document.querySelector(...)?.scrollIntoView is not a function
 ❯ Timeout._onTimeout src/pages/DecemberActivities/ProfitShareReport/ProfitShareReport.tsx:100:67
    100|     document.querySelector('[data-testid="filter-section"]')?.scrollIntoView({
    101|       behavior: "smooth",
    102|       block: "start"
    103|     });
```

**Root Cause**: The component code calls `scrollIntoView()` on a DOM element, but jsdom (the testing environment) doesn't implement this DOM method by default.

**Impact**: Tests fail with uncaught exceptions when the component tries to scroll during the test.

**How to Fix**:

**Option 1: Mock `scrollIntoView` in test setup** (Recommended)
```typescript
// Add to src/test/setup.ts or at the top of the test file:
Element.prototype.scrollIntoView = vi.fn();
```

**Option 2: Mock `scrollIntoView` per test file**
```typescript
beforeEach(() => {
  Element.prototype.scrollIntoView = vi.fn();
});
```

**Option 3: Prevent scroll in tests by mocking the condition**
```typescript
// In ProfitShareReport.tsx, guard against missing methods:
document.querySelector('[data-testid="filter-section"]')?.scrollIntoView?.({
  behavior: "smooth",
  block: "start"
});
```

**Option 4: Avoid scrolling in test environment**
```typescript
// In ProfitShareReport.tsx:
if (typeof document !== 'undefined' && 'scrollIntoView' in Element.prototype) {
  document.querySelector('[data-testid="filter-section"]')?.scrollIntoView({
    behavior: "smooth",
    block: "start"
  });
}
```

**Recommended Solution**: Add mock to `src/test/setup.ts`:
```typescript
// src/test/setup.ts
Element.prototype.scrollIntoView = vi.fn();
HTMLElement.prototype.scrollIntoView = vi.fn();
```

---

### 5. Test Timing and Async Race Conditions

**Affected Area**: Multiple tests across several files

**Root Cause**: Tests may be completing before async operations finish, or async operations are firing after test assertions.

**Common Scenarios**:
- Redux queries/mutations not properly mocked
- `useEffect` hooks firing state updates after render
- `setTimeout` callbacks executing during test cleanup
- RTK Query infinite queries or polling not being stopped

**How to Fix**:

1. **Ensure RTK Query hooks are mocked correctly**:
   ```typescript
   const { mockTrigger } = vi.hoisted(() => ({
     mockTrigger: vi.fn()
   }));

   vi.mock("../api", () => ({
     useMyQuery: vi.fn(() => ({
       data: mockData,
       isLoading: false,
       error: null
     }))
   }));
   ```

2. **Mock infinite queries/polling**:
   ```typescript
   vi.mock("../api", () => ({
     usePollQuery: vi.fn(() => ({
       data: mockData,
       isLoading: false,
       originalArgs: undefined,
       startPolling: vi.fn(),
       stopPolling: vi.fn()
     }))
   }));
   ```

3. **Use `waitFor()` for async assertions**:
   ```typescript
   await waitFor(() => {
     expect(screen.getByRole("table")).toBeInTheDocument();
   }, { timeout: 3000 });
   ```

4. **Clean up timers in `afterEach`**:
   ```typescript
   afterEach(() => {
     vi.clearAllTimers();
     vi.clearAllMocks();
   });
   ```

5. **Flush async operations with `vi.runAllTimersAsync()`**:
   ```typescript
   it("should load data", async () => {
     render(<MyComponent />);
     await vi.runAllTimersAsync();
     expect(screen.getByText("Loaded")).toBeInTheDocument();
   });
   ```

---

## Remediation Priority

### High Priority (Fixes 83 Failed Tests)
1. **Fix `scrollIntoView` mock** - Prevents uncaught exceptions in ProfitShareReport tests
2. **Wrap state updates in `act()`** - Fixes MilitaryContribution test warnings that may cause instability
3. **Find missing `filter-is-searching` element** - Fixes Forfeit test failures

### Medium Priority (Quality Improvements)
4. **Update MUI Grid v1 → v2** - Eliminate deprecation warnings, prepare for future MUI upgrades
5. **Review async test patterns** - Ensure proper use of `waitFor()` and mocking

### Low Priority (Future Modernization)
6. **Update all test selectors to use accessibility queries** - Better test maintainability

---

## Files That Need Attention

| File | Issue Type | Severity | Fix Category |
|------|-----------|----------|--------------|
| `src/pages/DecemberActivities/MilitaryContribution/__test__/MilitaryContribution.test.tsx` | `act()` warnings | High | Async handling |
| `src/pages/DecemberActivities/ProfitShareReport/__tests__/ProfitShareReport.test.tsx` | `scrollIntoView` not available | High | DOM mocking |
| `src/pages/FiscalClose/Forfeit/__test__/Forfeit.test.tsx` | Missing element | High | Test data/selectors |
| `src/pages/DecemberActivities/MilitaryContribution/MilitaryContributionForm.test.tsx` | MUI Grid v1 deprecation | Low | Component update |
| `src/test/setup.ts` | Missing DOM mocks | High | Test setup |

---

## Recommended Next Steps

1. **Immediately**: Add `scrollIntoView` mock to test setup to prevent uncaught exceptions
2. **Week 1**: Fix the missing `filter-is-searching` element in Forfeit tests
3. **Week 1**: Wrap async state updates in MilitaryContribution tests with `act()` or `waitFor()`
4. **Week 2**: Update MUI Grid components to v2 API
5. **Ongoing**: Review test patterns for compliance with React Testing Library best practices

---

## Testing Best Practices to Apply

Based on the errors found, ensure all new tests follow these patterns:

✅ **Always use `waitFor()` for async assertions**
✅ **Mock RTK Query hooks at module level with `vi.hoisted()`**
✅ **Wrap user interactions in `act()` when needed**
✅ **Mock DOM methods like `scrollIntoView` in test setup**
✅ **Query elements by accessible roles, not test IDs**
✅ **Clean up mocks and timers in `afterEach`**

See `ai-templates/front-end/fe-unit-tests.md` for comprehensive testing patterns.
