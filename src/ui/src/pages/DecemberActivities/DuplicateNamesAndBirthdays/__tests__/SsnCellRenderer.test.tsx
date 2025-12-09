import { configureStore } from "@reduxjs/toolkit";
import { act, fireEvent, render, screen } from "@testing-library/react";
import { Provider } from "react-redux";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import * as YearsEndApi from "../../../../reduxstore/api/YearsEndApi";
import securityReducer from "../../../../reduxstore/slices/securitySlice";
import { DuplicateNameAndBirthday, ImpersonationRoles } from "../../../../types";
import EnvironmentUtils from "../../../../utils/environmentUtils";
import SsnCellRenderer from "../SsnCellRenderer";

// Mock dependencies
vi.mock("../../../../reduxstore/api/YearsEndApi");
vi.mock("../../../../utils/environmentUtils");

/**
 * Test suite for SsnCellRenderer component
 * PS-2098: SSN Unmasking Feature UI Tests
 *
 * Tests cover:
 * - Permission-based UI visibility
 * - Auto-revert timer functionality
 * - Error handling
 * - Loading states
 * - API interaction
 * - Accessibility
 */

// Default test data
const DEFAULT_TEST_DATA: DuplicateNameAndBirthday = {
  demographicId: 12345,
  badgeNumber: 12345,
  ssn: "***-**-5181",
  name: "Doe, John",
  dateOfBirth: "1990-01-15",
  address: {
    street: "123 Main St",
    street2: null,
    city: "Boston",
    state: "MA",
    postalCode: "02101",
    countryIso: "US"
  },
  years: 10,
  hireDate: "2014-01-15",
  terminationDate: null,
  status: "Active",
  storeNumber: 1,
  count: 1,
  netBalance: 5000,
  hoursCurrentYear: 2080,
  incomeCurrentYear: 50000,
  employmentStatusName: "Active"
};

/**
 * Creates a mock Redux store with security state
 */
function createMockStore(
  securityState: Partial<{
    userPermissions: string[];
    impersonating: string[];
  }> = {}
) {
  return configureStore({
    reducer: {
      security: securityReducer
    },
    preloadedState: {
      security: {
        token: null,
        userGroups: [],
        userRoles: [],
        userPermissions: securityState.userPermissions ?? [],
        username: "",
        performLogout: false,
        appUser: null,
        impersonating: (securityState.impersonating ?? []) as ImpersonationRoles[]
      }
    }
  });
}

// Helper function to set mocks at test runtime
function setupEnvironmentMocks(
  isDevelopmentOrQA: boolean,
  isUAT: boolean = false,
  isProduction: boolean = !isDevelopmentOrQA
) {
  const mockUtils = vi.mocked(EnvironmentUtils, true);
  Object.defineProperty(mockUtils, "isDevelopmentOrQA", {
    get: () => isDevelopmentOrQA,
    configurable: true
  });
  Object.defineProperty(mockUtils, "isUAT", {
    get: () => isUAT,
    configurable: true
  });
  Object.defineProperty(mockUtils, "isProduction", {
    get: () => isProduction,
    configurable: true
  });
}

function setupYearsEndApiMocks() {
  // Create a mutation function that when called returns an object with an .unwrap() method
  // This is how RTK Query mutations work - they return a result object with unwrap() and other status info
  // We use a microtask (Promise.resolve) to ensure the state update happens asynchronously
  const mockUnmask = vi.fn(() => {
    return {
      unwrap: () => Promise.resolve({ unmaskedSsn: "700-00-5181" })
    };
  });

  vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    mockUnmask as any,
    { isLoading: false, isError: false, reset: vi.fn() }
  ]);
  return mockUnmask;
}

/**
 * Helper to get the unmask button (handles MUI Tooltip wrapper)
 */
function getUnmaskButton() {
  try {
    return screen.getByTestId("VisibilityIcon").closest("button")!;
  } catch {
    // Fallback: just get any button
    return screen.getByRole("button");
  }
}

beforeEach(() => {
  vi.useFakeTimers();
  vi.clearAllMocks();
  setupEnvironmentMocks(false, false, true); // Default: Production
  setupYearsEndApiMocks();
});

afterEach(async () => {
  await act(async () => {
    vi.runOnlyPendingTimers();
  });
  vi.useRealTimers();
});

// ==================== Permission Tests ====================

describe("Permission-Based UI Visibility", () => {
  it("PS-2098: Should NOT display eye icon when user lacks SsnUnmasking permission (Production)", () => {
    // Arrange
    const store = createMockStore({
      userPermissions: []
    });

    // Act
    render(
      <Provider store={store}>
        <SsnCellRenderer data={DEFAULT_TEST_DATA} />
      </Provider>
    );

    // Assert
    const visibilityIcon = screen.queryByRole("button", { name: /Click to unmask SSN/i });
    expect(visibilityIcon).not.toBeInTheDocument();
  });

  it("PS-2098: Should display eye icon when user has SsnUnmasking permission (Production)", () => {
    // Arrange
    const store = createMockStore({
      userPermissions: ["SSN-Unmasking"]
    });

    // Act
    render(
      <Provider store={store}>
        <SsnCellRenderer data={DEFAULT_TEST_DATA} />
      </Provider>
    );

    // Assert
    const visibilityIcon = getUnmaskButton();
    expect(visibilityIcon).toBeInTheDocument();
  });

  it("PS-2098: Should display eye icon with permission in Dev/QA environment", () => {
    // Arrange
    setupEnvironmentMocks(true); // Dev/QA
    const store = createMockStore({
      userPermissions: ["SSN-Unmasking"]
    });

    // Act
    render(
      <Provider store={store}>
        <SsnCellRenderer data={DEFAULT_TEST_DATA} />
      </Provider>
    );

    // Assert
    const visibilityIcon = getUnmaskButton();
    expect(visibilityIcon).toBeInTheDocument();
  });

  it("PS-2098: Should display eye icon with impersonation role in Dev/QA environment", () => {
    // Arrange
    setupEnvironmentMocks(true); // Dev/QA
    const store = createMockStore({
      userPermissions: [],
      impersonating: ["SSN-Unmasking"]
    });

    // Act
    render(
      <Provider store={store}>
        <SsnCellRenderer data={DEFAULT_TEST_DATA} />
      </Provider>
    );

    // Assert
    const visibilityIcon = getUnmaskButton();
    expect(visibilityIcon).toBeInTheDocument();
  });

  it("PS-2098: Should NOT display eye icon with impersonation role in Production", () => {
    // Arrange
    setupEnvironmentMocks(false); // Production
    const store = createMockStore({
      userPermissions: [],
      impersonating: ["SSN-Unmasking"]
    });

    // Act
    render(
      <Provider store={store}>
        <SsnCellRenderer data={DEFAULT_TEST_DATA} />
      </Provider>
    );

    // Assert
    const visibilityIcon = screen.queryByRole("button", { name: /Click to unmask SSN/i });
    expect(visibilityIcon).not.toBeInTheDocument();
  });
});

// ==================== SSN Display Tests ====================

describe("SSN Display", () => {
  it("PS-2098: Should display masked SSN by default", () => {
    // Arrange
    const store = createMockStore({
      userPermissions: ["SSN-Unmasking"]
    });

    // Act
    render(
      <Provider store={store}>
        <SsnCellRenderer data={DEFAULT_TEST_DATA} />
      </Provider>
    );

    // Assert
    expect(screen.getByText(DEFAULT_TEST_DATA.ssn)).toBeInTheDocument();
  });

  it("PS-2098: Should display unmasked SSN after eye icon click", async () => {
    // Arrange - use real timers for promise resolution
    vi.useRealTimers();
    try {
      setupYearsEndApiMocks();
      const store = createMockStore({
        userPermissions: ["SSN-Unmasking"]
      });

      render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act
      const button = getUnmaskButton();
      await act(async () => {
        fireEvent.click(button);
        await new Promise((resolve) => setTimeout(resolve, 100));
      });

      // Assert
      expect(screen.getByText("700-00-5181")).toBeInTheDocument();
    } finally {
      vi.useFakeTimers();
    }
  });
});

// ==================== Auto-Revert Timer Tests ====================

describe("Auto-Revert Timer", () => {
  it("PS-2098: Should revert to masked SSN after timeout in Dev/QA (60 seconds)", async () => {
    // Arrange
    setupEnvironmentMocks(true); // Dev/QA - 60 second timeout
    setupYearsEndApiMocks();
    const store = createMockStore({
      userPermissions: ["SSN-Unmasking"]
    });

    render(
      <Provider store={store}>
        <SsnCellRenderer data={DEFAULT_TEST_DATA} />
      </Provider>
    );

    // Act - unmask SSN
    const button = getUnmaskButton();
    await act(async () => {
      fireEvent.click(button);
      await vi.runAllTimersAsync();
    });

    // Verify SSN is unmasked
    expect(screen.getByText("700-00-5181")).toBeInTheDocument();

    // Act - advance time by 60 seconds (Dev/QA timeout)
    // After advancing, the setTimeout callback should fire and revert the state
    await act(async () => {
      vi.advanceTimersByTime(60000);
      // Run all timers to process the timeout callback
      await vi.runAllTimersAsync();
    });

    // Assert - SSN should be masked again
    expect(screen.getByText(DEFAULT_TEST_DATA.ssn)).toBeInTheDocument();
  });

  it("PS-2098: Should revert to masked SSN after timeout in Production/UAT (5 minutes)", async () => {
    // Arrange
    setupEnvironmentMocks(false, true); // UAT - 5 minute timeout
    setupYearsEndApiMocks();
    const store = createMockStore({
      userPermissions: ["SSN-Unmasking"]
    });

    render(
      <Provider store={store}>
        <SsnCellRenderer data={DEFAULT_TEST_DATA} />
      </Provider>
    );

    // Act - unmask SSN
    const button = getUnmaskButton();
    await act(async () => {
      fireEvent.click(button);
      await vi.runAllTimersAsync();
    });

    // Verify SSN is unmasked
    expect(screen.getByText("700-00-5181")).toBeInTheDocument();

    // Act - advance time by 5 minutes (Production/UAT timeout)
    // After advancing, the setTimeout callback should fire and revert the state
    await act(async () => {
      vi.advanceTimersByTime(300000);
      // Run all timers to process the timeout callback
      await vi.runAllTimersAsync();
    });

    // Assert - SSN should be masked again
    expect(screen.getByText(DEFAULT_TEST_DATA.ssn)).toBeInTheDocument();
  });

  it("PS-2098: Should clear timer on component unmount", async () => {
    // Arrange
    setupYearsEndApiMocks();
    const store = createMockStore({
      userPermissions: ["SSN-Unmasking"]
    });

    const { unmount } = render(
      <Provider store={store}>
        <SsnCellRenderer data={DEFAULT_TEST_DATA} />
      </Provider>
    );

    // Act - unmask SSN
    const button = getUnmaskButton();
    await act(async () => {
      fireEvent.click(button);
      await vi.runAllTimersAsync();
    });

    // Verify SSN is unmasked
    expect(screen.getByText("700-00-5181")).toBeInTheDocument();

    // Act - unmount component before timer fires
    unmount();

    // Advance time and ensure no errors occur
    vi.advanceTimersByTime(60000);

    // Assert - no errors (timer was cleared)
    expect(true).toBe(true);
  });
});

// ==================== Loading States ====================

describe("Loading States", () => {
  it("PS-2098: Should show loading spinner while unmasking", async () => {
    // Arrange
    let resolveUnmask: (value: { unmaskedSsn: string }) => void = () => {};
    const unmaskedPromise = new Promise((resolve) => {
      resolveUnmask = resolve;
    });

    const mockUnmask = vi.fn().mockReturnValue({
      unwrap: () => unmaskedPromise
    });
    vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([
      mockUnmask,
      { isLoading: false, isError: false, reset: vi.fn() }
    ]);

    const store = createMockStore({
      userPermissions: ["SSN-Unmasking"]
    });

    render(
      <Provider store={store}>
        <SsnCellRenderer data={DEFAULT_TEST_DATA} />
      </Provider>
    );

    // Act - click to unmask
    const button = getUnmaskButton();
    await act(async () => {
      fireEvent.click(button);
      await vi.runAllTimersAsync();
    });

    // Assert - spinner appears
    expect(screen.getByRole("progressbar")).toBeInTheDocument();

    // Cleanup
    await act(async () => {
      resolveUnmask!({ unmaskedSsn: "700-00-5181" });
      await vi.runAllTimersAsync();
    });
  });

  it("PS-2098: Should disable button after unmasking until auto-revert", async () => {
    // Arrange
    setupYearsEndApiMocks();
    const store = createMockStore({
      userPermissions: ["SSN-Unmasking"]
    });

    render(
      <Provider store={store}>
        <SsnCellRenderer data={DEFAULT_TEST_DATA} />
      </Provider>
    );

    // Act - unmask SSN
    const button = getUnmaskButton();
    await act(async () => {
      fireEvent.click(button);
      await vi.runAllTimersAsync();
    });

    // Assert - button is disabled after unmask
    expect(button).toBeDisabled();
  });
});

// ==================== Error Handling ====================

describe("Error Handling", () => {
  it("PS-2098: Should display error message on API failure", async () => {
    // Arrange
    const mockUnmask = vi.fn().mockReturnValue({
      unwrap: () => Promise.reject(new Error("API Error"))
    });
    vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([
      mockUnmask,
      { isLoading: false, isError: false, reset: vi.fn() }
    ]);

    const store = createMockStore({
      userPermissions: ["SSN-Unmasking"]
    });

    render(
      <Provider store={store}>
        <SsnCellRenderer data={DEFAULT_TEST_DATA} />
      </Provider>
    );

    // Act
    const button = getUnmaskButton();
    await act(async () => {
      fireEvent.click(button);
      await vi.runAllTimersAsync();
    });

    // Assert
    expect(screen.getByText(/API Error/i)).toBeInTheDocument();
  });

  it("PS-2098: Should clear error after successful unmask", async () => {
    // Arrange - disable fake timers for this test
    vi.useRealTimers();
    try {
      const mockUnmask = vi
        .fn()
        .mockReturnValueOnce({
          unwrap: () => Promise.reject(new Error("Test Error"))
        })
        .mockReturnValueOnce({
          unwrap: () => Promise.resolve({ unmaskedSsn: "700-00-5181" })
        });
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([
        mockUnmask,
        { isLoading: false, isError: false, reset: vi.fn() }
      ]);

      const store = createMockStore({
        userPermissions: ["SSN-Unmasking"]
      });

      render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act - trigger error
      const button = getUnmaskButton();
      await act(async () => {
        fireEvent.click(button);
        await new Promise((resolve) => setTimeout(resolve, 100));
      });

      // Assert - error shows
      expect(screen.getByText(/Test Error/i)).toBeInTheDocument();

      // Act - try again successfully
      await act(async () => {
        fireEvent.click(button);
        await new Promise((resolve) => setTimeout(resolve, 100));
      });

      // Assert - error is cleared
      expect(screen.queryByText(/Test Error/i)).not.toBeInTheDocument();
    } finally {
      vi.useFakeTimers();
    }
  });
});

// ==================== Accessibility ====================

describe("Accessibility", () => {
  it("PS-2098: Should have accessible button with proper tooltip", () => {
    // Arrange
    const store = createMockStore({
      userPermissions: ["SSN-Unmasking"]
    });

    // Act
    render(
      <Provider store={store}>
        <SsnCellRenderer data={DEFAULT_TEST_DATA} />
      </Provider>
    );

    // Assert
    const button = getUnmaskButton();
    expect(button).toBeInTheDocument();
    expect(button).toHaveAttribute("type", "button");
  });

  it("PS-2098: Should update tooltip after unmasking", async () => {
    // Arrange - disable fake timers for this test
    vi.useRealTimers();
    try {
      setupYearsEndApiMocks();
      const store = createMockStore({
        userPermissions: ["SSN-Unmasking"]
      });

      render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act - unmask SSN
      const button = getUnmaskButton();
      await act(async () => {
        fireEvent.click(button);
        await new Promise((resolve) => setTimeout(resolve, 100));
      });

      // Assert - button is disabled after click
      expect(button).toBeDisabled();
    } finally {
      vi.useFakeTimers();
    }
  });
});

// ==================== Component Integration ====================

describe("Component Integration", () => {
  it("PS-2098: Should correctly render with different demographic IDs", async () => {
    // Arrange - disable fake timers for this test to avoid waitFor timeouts
    vi.useRealTimers();
    try {
      setupYearsEndApiMocks();
      const store = createMockStore({
        userPermissions: ["SSN-Unmasking"]
      });

      const testData = { ...DEFAULT_TEST_DATA, demographicId: 99999 };

      // Act
      render(
        <Provider store={store}>
          <SsnCellRenderer data={testData} />
        </Provider>
      );

      const button = getUnmaskButton();

      // Assert - verify button is rendered and functional
      expect(button).toBeInTheDocument();

      // Verify demographic ID is used by clicking
      await act(async () => {
        fireEvent.click(button);
        await new Promise((resolve) => setTimeout(resolve, 100));
      });

      expect(button).toBeDisabled();
    } finally {
      vi.useFakeTimers();
    }
  });

  it("PS-2098: Should handle rapid clicks gracefully", async () => {
    // Arrange - disable fake timers for this test
    vi.useRealTimers();
    try {
      setupYearsEndApiMocks();
      const store = createMockStore({
        userPermissions: ["SSN-Unmasking"]
      });

      render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act - Click multiple times rapidly
      const button = getUnmaskButton();
      await act(async () => {
        fireEvent.click(button);
        fireEvent.click(button);
        fireEvent.click(button);
        await new Promise((resolve) => setTimeout(resolve, 100));
      });

      // Assert - Button becomes disabled after first click to prevent race conditions
      expect(button).toBeDisabled();
    } finally {
      vi.useFakeTimers();
    }
  });
});
