import { configureStore, PreloadedState } from "@reduxjs/toolkit";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { Provider } from "react-redux";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import * as YearsEndApi from "../../../../reduxstore/api/YearsEndApi";
import securityReducer from "../../../../reduxstore/slices/securitySlice";
import { RootState } from "../../../../reduxstore/store";
import { DuplicateNameAndBirthday } from "../../../../types";
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
  ssn: "***-**-5181",
  firstName: "John",
  lastName: "Doe",
  birthDate: new Date("1990-01-15"),
  duplicateCount: 2,
  lastModified: new Date()
};

/**
 * Helper function to create a mock store with optional preloaded state
 */
function createMockStore(preloadedState?: PreloadedState<RootState>) {
  return configureStore({
    reducer: {
      security: securityReducer
    },
    preloadedState: preloadedState || {
      security: {
        userPermissions: [],
        impersonating: [],
        user: null,
        roles: [],
        isLoading: false,
        error: null
      }
    }
  });
}

describe("SsnCellRenderer Component", () => {
  // ==================== Setup & Cleanup ====================

  beforeEach(() => {
    vi.useFakeTimers();
    vi.clearAllMocks();
    
    // Setup default mocks for EnvironmentUtils
    vi.spyOn(EnvironmentUtils, "isDevelopmentOrQA", "get").mockReturnValue(false);
    vi.spyOn(EnvironmentUtils, "isUAT", "get").mockReturnValue(false);
    vi.spyOn(EnvironmentUtils, "isProduction", "get").mockReturnValue(true);
  });

  afterEach(() => {
    vi.runOnlyPendingTimers();
    vi.useRealTimers();
  });

  // ==================== Permission Tests ====================

  describe("Permission-Based UI Visibility", () => {
    it("PS-2098: Should NOT display eye icon when user lacks SsnUnmasking permission (Production)", () => {
      // Arrange
      const store = createMockStore({
        security: {
          userPermissions: [],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([
        vi.fn().mockResolvedValue({ unmaskedSsn: "700-00-5181" }),
        { isLoading: false, isError: false }
      ]);

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
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([
        vi.fn().mockResolvedValue({ unmaskedSsn: "700-00-5181" }),
        { isLoading: false, isError: false }
      ]);

      // Act
      render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Assert
      const visibilityIcon = screen.getByRole("button", { name: /Click to unmask SSN/i });
      expect(visibilityIcon).toBeInTheDocument();
    });

    it("PS-2098: Should display eye icon with permission in Dev/QA environment", () => {
      // Arrange
      const store = createMockStore({
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(true);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([
        vi.fn().mockResolvedValue({ unmaskedSsn: "700-00-5181" }),
        { isLoading: false, isError: false }
      ]);

      // Act
      render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Assert
      const visibilityIcon = screen.getByRole("button", { name: /Click to unmask SSN/i });
      expect(visibilityIcon).toBeInTheDocument();
    });

    it("PS-2098: Should display eye icon with impersonation role in Dev/QA environment", () => {
      // Arrange
      const store = createMockStore({
        security: {
          userPermissions: [],
          impersonating: ["SSN-Unmasking"],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(true);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([
        vi.fn().mockResolvedValue({ unmaskedSsn: "700-00-5181" }),
        { isLoading: false, isError: false }
      ]);

      // Act
      render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Assert
      const visibilityIcon = screen.getByRole("button", { name: /Click to unmask SSN/i });
      expect(visibilityIcon).toBeInTheDocument();
    });

    it("PS-2098: Should NOT display eye icon with impersonation role in Production", () => {
      // Arrange
      const store = createMockStore({
        security: {
          userPermissions: [],
          impersonating: ["SSN-Unmasking"],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([
        vi.fn().mockResolvedValue({ unmaskedSsn: "700-00-5181" }),
        { isLoading: false, isError: false }
      ]);

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

  // ==================== Display Tests ====================

  describe("SSN Display", () => {
    it("PS-2098: Should display masked SSN by default", () => {
      // Arrange
      const store = createMockStore({
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([vi.fn(), { isLoading: false, isError: false }]);

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
      // Arrange
      const mockUnmask = vi.fn().mockResolvedValue({ unmaskedSsn: "700-00-5181" });
      const store = createMockStore({
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([mockUnmask, { isLoading: false, isError: false }]);

      const { rerender } = render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act
      const button = screen.getByRole("button", { name: /Click to unmask SSN/i });
      fireEvent.click(button);

      // Wait for async operation
      await waitFor(() => {
        expect(mockUnmask).toHaveBeenCalledWith({ demographicId: DEFAULT_TEST_DATA.demographicId });
      });

      rerender(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Assert - unmasked SSN should be visible
      await waitFor(() => {
        expect(screen.getByText("700-00-5181")).toBeInTheDocument();
      });
    });
  });

  // ==================== Auto-Revert Timer Tests ====================

  describe("Auto-Revert Timer (60 seconds)", () => {
    it("PS-2098: Should revert to masked SSN after 60 seconds", async () => {
      // Arrange
      const mockUnmask = vi.fn().mockResolvedValue({ unmaskedSsn: "700-00-5181" });
      const store = createMockStore({
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([mockUnmask, { isLoading: false, isError: false }]);

      const { rerender } = render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act - click eye icon
      const button = screen.getByRole("button", { name: /Click to unmask SSN/i });
      fireEvent.click(button);

      await waitFor(() => {
        expect(mockUnmask).toHaveBeenCalled();
      });

      rerender(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Wait for unmasked SSN to appear
      await waitFor(() => {
        expect(screen.getByText("700-00-5181")).toBeInTheDocument();
      });

      // Advance time by 60 seconds
      vi.advanceTimersByTime(60000);

      // Assert - should revert to masked
      await waitFor(() => {
        expect(screen.getByText(DEFAULT_TEST_DATA.ssn)).toBeInTheDocument();
        expect(screen.queryByText("700-00-5181")).not.toBeInTheDocument();
      });
    });

    it("PS-2098: Should clear timer on component unmount", async () => {
      // Arrange
      const mockUnmask = vi.fn().mockResolvedValue({ unmaskedSsn: "700-00-5181" });
      const store = createMockStore({
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([mockUnmask, { isLoading: false, isError: false }]);

      const { unmount, rerender } = render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act
      const button = screen.getByRole("button", { name: /Click to unmask SSN/i });
      fireEvent.click(button);

      await waitFor(() => {
        expect(mockUnmask).toHaveBeenCalled();
      });

      rerender(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Unmount component before timer fires
      unmount();

      // Advance timer - should not throw error
      expect(() => {
        vi.advanceTimersByTime(60000);
      }).not.toThrow();
    });
  });

  // ==================== Loading State Tests ====================

  describe("Loading States", () => {
    it("PS-2098: Should show loading spinner while unmasking", async () => {
      // Arrange
      const mockUnmask = vi.fn();
      mockUnmask.mockImplementation(
        () =>
          new Promise((resolve) => {
            setTimeout(() => resolve({ unmaskedSsn: "700-00-5181" }), 100);
          })
      );

      const store = createMockStore({
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([mockUnmask, { isLoading: false, isError: false }]);

      render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act
      const button = screen.getByRole("button");
      fireEvent.click(button);

      // Assert - button should be disabled during loading
      await waitFor(() => {
        expect(button).toBeDisabled();
      });

      // Clean up
      vi.advanceTimersByTime(100);
    });

    it("PS-2098: Should disable button after unmasking until auto-revert", async () => {
      // Arrange
      const mockUnmask = vi.fn().mockResolvedValue({ unmaskedSsn: "700-00-5181" });
      const store = createMockStore({
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([mockUnmask, { isLoading: false, isError: false }]);

      const { rerender } = render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act
      const button = screen.getByRole("button", { name: /Click to unmask SSN/i });
      fireEvent.click(button);

      await waitFor(() => {
        expect(mockUnmask).toHaveBeenCalled();
      });

      rerender(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Assert - button should remain disabled while unmasked
      await waitFor(() => {
        expect(button).toBeDisabled();
      });

      // Advance time
      vi.advanceTimersByTime(60000);

      // Button should be enabled again after revert
      await waitFor(() => {
        expect(button).not.toBeDisabled();
      });
    });
  });

  // ==================== Error Handling Tests ====================

  describe("Error Handling", () => {
    it("PS-2098: Should display error message on API failure", async () => {
      // Arrange
      const mockUnmask = vi.fn().mockRejectedValue(new Error("Network error"));
      const store = createMockStore({
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([mockUnmask, { isLoading: false, isError: false }]);

      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

      const { rerender } = render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act
      const button = screen.getByRole("button", { name: /Click to unmask SSN/i });
      fireEvent.click(button);

      // Assert
      await waitFor(() => {
        expect(screen.getByText("Network error")).toBeInTheDocument();
      });

      consoleErrorSpy.mockRestore();
    });

    it("PS-2098: Should clear error after successful unmask", async () => {
      // Arrange
      const mockUnmask = vi
        .fn()
        .mockRejectedValueOnce(new Error("First attempt failed"))
        .mockResolvedValueOnce({ unmaskedSsn: "700-00-5181" });

      const store = createMockStore({
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([mockUnmask, { isLoading: false, isError: false }]);

      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

      const { rerender } = render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act - First attempt (fails)
      let button = screen.getByRole("button", { name: /Click to unmask SSN/i });
      fireEvent.click(button);

      await waitFor(() => {
        expect(screen.getByText("First attempt failed")).toBeInTheDocument();
      });

      // Revert to masked and re-render to reset state
      vi.advanceTimersByTime(60000);

      rerender(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act - Second attempt (succeeds)
      button = screen.getByRole("button", { name: /Click to unmask SSN/i });
      fireEvent.click(button);

      // Assert - error should be cleared
      await waitFor(() => {
        expect(screen.queryByText("First attempt failed")).not.toBeInTheDocument();
      });

      consoleErrorSpy.mockRestore();
    });
  });

  // ==================== Accessibility Tests ====================

  describe("Accessibility", () => {
    it("PS-2098: Should have accessible button with proper tooltip", () => {
      // Arrange
      const store = createMockStore({
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([
        vi.fn().mockResolvedValue({ unmaskedSsn: "700-00-5181" }),
        { isLoading: false, isError: false }
      ]);

      // Act
      render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Assert
      const button = screen.getByRole("button", { name: /Click to unmask SSN/i });
      expect(button).toBeInTheDocument();
      expect(button).toHaveAttribute("title", "Click to unmask SSN");
    });

    it("PS-2098: Should update tooltip after unmasking", async () => {
      // Arrange
      const mockUnmask = vi.fn().mockResolvedValue({ unmaskedSsn: "700-00-5181" });
      const store = createMockStore({
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([mockUnmask, { isLoading: false, isError: false }]);

      const { rerender } = render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act
      const button = screen.getByRole("button", { name: /Click to unmask SSN/i });
      fireEvent.click(button);

      await waitFor(() => {
        expect(mockUnmask).toHaveBeenCalled();
      });

      rerender(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Assert
      await waitFor(() => {
        expect(button).toHaveAttribute("title", "SSN unmasked for 60 seconds");
      });
    });
  });

  // ==================== Integration Tests ====================

  describe("Component Integration", () => {
    it("PS-2098: Should correctly render with different demographic IDs", async () => {
      // Arrange
      const mockUnmask = vi.fn().mockResolvedValue({ unmaskedSsn: "555-55-5555" });
      const store = createMockStore({
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([mockUnmask, { isLoading: false, isError: false }]);

      const testData = { ...DEFAULT_TEST_DATA, demographicId: 99999 };

      // Act
      render(
        <Provider store={store}>
          <SsnCellRenderer data={testData} />
        </Provider>
      );

      const button = screen.getByRole("button");
      fireEvent.click(button);

      // Assert
      await waitFor(() => {
        expect(mockUnmask).toHaveBeenCalledWith({ demographicId: 99999 });
      });
    });

    it("PS-2098: Should handle rapid clicks gracefully", async () => {
      // Arrange
      const mockUnmask = vi.fn().mockResolvedValue({ unmaskedSsn: "700-00-5181" });
      const store = createMockStore({
        security: {
          userPermissions: ["SSN-Unmasking"],
          impersonating: [],
          user: null,
          roles: [],
          isLoading: false,
          error: null
        }
      });

      vi.mocked(EnvironmentUtils.isDevelopmentOrQA).mockReturnValue(false);
      vi.mocked(YearsEndApi.useUnmaskSsnMutation).mockReturnValue([mockUnmask, { isLoading: false, isError: false }]);

      const { rerender } = render(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );

      // Act - Click multiple times rapidly
      const button = screen.getByRole("button");
      fireEvent.click(button);
      fireEvent.click(button);
      fireEvent.click(button);

      await waitFor(() => {
        // Should only call API once due to button being disabled after first click
        expect(mockUnmask.mock.calls.length).toBeLessThanOrEqual(1);
      });

      rerender(
        <Provider store={store}>
          <SsnCellRenderer data={DEFAULT_TEST_DATA} />
        </Provider>
      );
    });
  });
});
