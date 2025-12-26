import { renderHook, waitFor } from "@testing-library/react";
import { act } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useBalanceValidation } from "./useBalanceValidation";
import { CrossReferenceValidationGroup } from "../types/validation/cross-reference-validation";

// Mock the RTK Query API
const { mockTriggerFetch } = vi.hoisted(() => ({
  mockTriggerFetch: vi.fn()
}));

vi.mock("reduxstore/api/ValidationApi", () => ({
  useLazyGetBalanceValidationQuery: vi.fn(() => [
    mockTriggerFetch,
    { data: undefined, isFetching: false, isError: false }
  ])
}));

describe("useBalanceValidation", () => {
  const mockValidationData: CrossReferenceValidationGroup = {
    groupName: "Balance Validation",
    validations: [
      {
        fieldName: "NetAllocTransfer",
        isValid: true,
        errorMessages: []
      },
      {
        fieldName: "NetPaidAllocTransfer",
        isValid: false,
        errorMessages: ["Transfer amounts do not match"]
      }
    ]
  };

  beforeEach(() => {
    vi.clearAllMocks();

    // Default successful response
    mockTriggerFetch.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue(mockValidationData)
    });
  });

  describe("Initial State", () => {
    it("should return null validation data and no error initially", () => {
      const { result } = renderHook(() => useBalanceValidation(null));

      expect(result.current.validationData).toBeNull();
      expect(result.current.error).toBeNull();
      expect(result.current.isLoading).toBe(false);
    });

    it("should not fetch when profitYear is null", () => {
      renderHook(() => useBalanceValidation(null));

      expect(mockTriggerFetch).not.toHaveBeenCalled();
    });

    it("should not fetch when profitYear is 0", () => {
      renderHook(() => useBalanceValidation(0));

      expect(mockTriggerFetch).not.toHaveBeenCalled();
    });

    it("should not fetch when profitYear is negative", () => {
      renderHook(() => useBalanceValidation(-1));

      expect(mockTriggerFetch).not.toHaveBeenCalled();
    });
  });

  describe("Successful Data Fetching", () => {
    it("should fetch validation data when profitYear is valid", async () => {
      renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(mockTriggerFetch).toHaveBeenCalledWith(2024, true);
      });
    });

    it("should set validation data on successful fetch", async () => {
      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.validationData).toEqual(mockValidationData);
        expect(result.current.error).toBeNull();
      });
    });

    it("should fetch with preferCacheValue=true on initial load", async () => {
      renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(mockTriggerFetch).toHaveBeenCalledWith(2024, true);
      });
    });
  });

  describe("404 Error Handling (Graceful Degradation)", () => {
    it("should handle 404 gracefully by setting null data and no error", async () => {
      mockTriggerFetch.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue({
          status: 404,
          data: { title: "Not Found" }
        })
      });

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.validationData).toBeNull();
        expect(result.current.error).toBeNull();
      });
    });

    it("should not log console error for 404 status", async () => {
      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

      mockTriggerFetch.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue({
          status: 404
        })
      });

      renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(consoleErrorSpy).not.toHaveBeenCalled();
      });

      consoleErrorSpy.mockRestore();
    });
  });

  describe("Error Handling (Non-404 Errors)", () => {
    it("should set error message for API errors with title", async () => {
      const errorTitle = "Validation Failed";
      mockTriggerFetch.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue({
          status: 500,
          data: { title: errorTitle }
        })
      });

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.error).toBe(errorTitle);
        expect(result.current.validationData).toBeNull();
      });
    });

    it("should set error message for API errors with message field", async () => {
      const errorMessage = "Server error occurred";
      mockTriggerFetch.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue({
          status: 500,
          data: { message: errorMessage }
        })
      });

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.error).toBe(errorMessage);
      });
    });

    it("should handle Error instances", async () => {
      const errorMessage = "Network error";
      mockTriggerFetch.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue(new Error(errorMessage))
      });

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.error).toBe(errorMessage);
      });
    });

    it("should handle unknown error types", async () => {
      mockTriggerFetch.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue("string error")
      });

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.error).toBe("Unknown error");
      });
    });

    it("should log console error for non-404 errors", async () => {
      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

      mockTriggerFetch.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue({
          status: 500,
          data: { title: "Server Error" }
        })
      });

      renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(consoleErrorSpy).toHaveBeenCalledWith("Error fetching balance validation:", expect.anything());
      });

      consoleErrorSpy.mockRestore();
    });
  });

  describe("ProfitYear Changes", () => {
    it("should refetch when profitYear changes to a new valid year", async () => {
      const { rerender } = renderHook(({ year }) => useBalanceValidation(year), {
        initialProps: { year: 2024 }
      });

      await waitFor(() => {
        expect(mockTriggerFetch).toHaveBeenCalledWith(2024, true);
      });

      vi.clearAllMocks();

      rerender({ year: 2025 });

      await waitFor(() => {
        expect(mockTriggerFetch).toHaveBeenCalledWith(2025, true);
      });
    });

    it("should clear data when profitYear changes to null", async () => {
      const { result, rerender } = renderHook(({ year }) => useBalanceValidation(year), {
        initialProps: { year: 2024 }
      });

      await waitFor(() => {
        expect(result.current.validationData).toEqual(mockValidationData);
      });

      rerender({ year: null });

      expect(result.current.validationData).toBeNull();
      expect(result.current.error).toBeNull();
    });

    it("should clear data when profitYear changes to 0", async () => {
      const { result, rerender } = renderHook(({ year }) => useBalanceValidation(year), {
        initialProps: { year: 2024 }
      });

      await waitFor(() => {
        expect(result.current.validationData).toEqual(mockValidationData);
      });

      rerender({ year: 0 });

      expect(result.current.validationData).toBeNull();
      expect(result.current.error).toBeNull();
    });
  });

  describe("Manual Refetch", () => {
    it("should refetch data when refetch is called", async () => {
      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.validationData).toEqual(mockValidationData);
      });

      vi.clearAllMocks();

      act(() => {
        result.current.refetch();
      });

      await waitFor(() => {
        expect(mockTriggerFetch).toHaveBeenCalledWith(2024, false);
      });
    });

    it("should use preferCacheValue=false on manual refetch", async () => {
      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.validationData).not.toBeNull();
      });

      act(() => {
        result.current.refetch();
      });

      await waitFor(() => {
        expect(mockTriggerFetch).toHaveBeenCalledWith(2024, false);
      });
    });

    it("should not refetch when profitYear is null", async () => {
      const { result } = renderHook(() => useBalanceValidation(null));

      vi.clearAllMocks();

      act(() => {
        result.current.refetch();
      });

      expect(mockTriggerFetch).not.toHaveBeenCalled();
    });

    it("should handle refetch errors gracefully", async () => {
      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.validationData).toEqual(mockValidationData);
      });

      // Make refetch fail with an Error instance
      mockTriggerFetch.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue(new Error("Refetch failed"))
      });

      act(() => {
        result.current.refetch();
      });

      await waitFor(() => {
        expect(result.current.error).toBe("Refetch failed");
        expect(result.current.validationData).toBeNull();
      });
    });

    it("should handle 404 on refetch gracefully", async () => {
      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.validationData).toEqual(mockValidationData);
      });

      // Make refetch return 404
      mockTriggerFetch.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue({
          status: 404
        })
      });

      act(() => {
        result.current.refetch();
      });

      await waitFor(() => {
        expect(result.current.validationData).toBeNull();
        expect(result.current.error).toBeNull();
      });
    });
  });

  describe("Return Object Stability", () => {
    it("should provide stable refetch function reference", () => {
      const { result, rerender } = renderHook(() => useBalanceValidation(2024));

      const firstRefetch = result.current.refetch;

      rerender();

      expect(result.current.refetch).toBe(firstRefetch);
    });
  });

  describe("Edge Cases", () => {
    it("should handle very large profit years", async () => {
      renderHook(() => useBalanceValidation(9999));

      await waitFor(() => {
        expect(mockTriggerFetch).toHaveBeenCalledWith(9999, true);
      });
    });

    it("should handle rapid profitYear changes", async () => {
      const { rerender } = renderHook(({ year }) => useBalanceValidation(year), {
        initialProps: { year: 2024 }
      });

      rerender({ year: 2025 });
      rerender({ year: 2026 });
      rerender({ year: 2027 });

      await waitFor(() => {
        expect(mockTriggerFetch).toHaveBeenCalledWith(2027, true);
      });
    });

    it("should handle empty validation data array", async () => {
      const emptyData: CrossReferenceValidationGroup = {
        groupName: "Empty",
        validations: []
      };

      mockTriggerFetch.mockReturnValue({
        unwrap: vi.fn().mockResolvedValue(emptyData)
      });

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.validationData).toEqual(emptyData);
        expect(result.current.validationData?.validations).toHaveLength(0);
      });
    });
  });
});
