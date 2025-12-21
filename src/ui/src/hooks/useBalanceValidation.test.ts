import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import type { CrossReferenceValidationGroup } from "../types/validation/cross-reference-validation";
import { useBalanceValidation } from "./useBalanceValidation";

// Create mock functions for RTK Query
const mockTriggerFetch = vi.fn();
const mockUnwrap = vi.fn();

// Mock the RTK Query API
vi.mock("reduxstore/api/ValidationApi", () => ({
  useLazyGetBalanceValidationQuery: () => [
    mockTriggerFetch,
    {
      data: undefined,
      isFetching: false,
      error: undefined,
      isError: false
    }
  ]
}));

describe("useBalanceValidation", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    // Setup default mock behavior
    mockTriggerFetch.mockReturnValue({ unwrap: mockUnwrap });
    mockUnwrap.mockResolvedValue(null);
  });

  describe("initial state", () => {
    it("should have null validation data initially", () => {
      const { result } = renderHook(() => useBalanceValidation(null));

      expect(result.current.validationData).toBeNull();
      expect(result.current.isLoading).toBe(false);
      expect(result.current.error).toBeNull();
    });

    it("should not fetch when profitYear is null", () => {
      renderHook(() => useBalanceValidation(null));

      expect(mockTriggerFetch).not.toHaveBeenCalled();
    });

    it("should not fetch when profitYear is 0 or negative", () => {
      renderHook(() => useBalanceValidation(0));
      renderHook(() => useBalanceValidation(-1));

      expect(mockTriggerFetch).not.toHaveBeenCalled();
    });
  });

  describe("auto-fetch on profitYear change", () => {
    it("should fetch validation data when profitYear is provided", async () => {
      const mockValidationData: CrossReferenceValidationGroup = {
        groupName: "ALLOC/PAID ALLOC Transfers",
        description: null,
        summary: null,
        priority: "High",
        validationRule: null,
        validations: [
          {
            reportCode: "PAY443",
            fieldName: "NetAllocTransfer",
            expectedValue: 1500,
            currentValue: 1500,
            isValid: true,
            variance: 0,
            message: "PAY443.NetAllocTransfer matches archived value",
            archivedAt: null,
            notes: null
          }
        ],
        isValid: true
      };

      mockUnwrap.mockResolvedValueOnce(mockValidationData);

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.validationData).toEqual(mockValidationData);
      });

      expect(mockTriggerFetch).toHaveBeenCalledWith(2024, true);
      expect(result.current.error).toBeNull();
    });

    it("should update validation data when profitYear changes", async () => {
      const mockData2024: CrossReferenceValidationGroup = {
        groupName: "Test 2024",
        description: null,
        summary: null,
        priority: "High",
        validationRule: null,
        validations: [],
        isValid: true
      };

      const mockData2025: CrossReferenceValidationGroup = {
        groupName: "Test 2025",
        description: null,
        summary: null,
        priority: "High",
        validationRule: null,
        validations: [],
        isValid: true
      };

      mockUnwrap.mockResolvedValueOnce(mockData2024);

      const { result, rerender } = renderHook(({ year }: { year: number | null }) => useBalanceValidation(year), {
        initialProps: { year: 2024 as number | null }
      });

      await waitFor(() => {
        expect(result.current.validationData).toEqual(mockData2024);
      });

      // Change profitYear
      mockUnwrap.mockResolvedValueOnce(mockData2025);

      rerender({ year: 2025 });

      await waitFor(() => {
        expect(result.current.validationData).toEqual(mockData2025);
      });

      expect(mockTriggerFetch).toHaveBeenCalledTimes(2);
    });
  });

  describe("404 handling (no validation data)", () => {
    it("should handle 404 gracefully (no validation data available)", async () => {
      mockUnwrap.mockRejectedValueOnce({ status: 404 });

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(mockTriggerFetch).toHaveBeenCalled();
      });

      // Wait for state to settle
      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.validationData).toBeNull();
      expect(result.current.error).toBeNull();
    });
  });

  describe("error handling", () => {
    it("should handle network errors", async () => {
      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});
      mockUnwrap.mockRejectedValueOnce(new Error("Network failure"));

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.error).toBe("Network failure");
      });

      expect(result.current.validationData).toBeNull();
      expect(consoleErrorSpy).toHaveBeenCalled();

      consoleErrorSpy.mockRestore();
    });

    it("should handle HTTP error responses (non-404)", async () => {
      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});
      mockUnwrap.mockRejectedValueOnce({
        status: 500,
        data: { title: "Internal Server Error" }
      });

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.error).toBe("Internal Server Error");
      });

      expect(result.current.validationData).toBeNull();

      consoleErrorSpy.mockRestore();
    });

    it("should handle non-Error objects", async () => {
      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});
      mockUnwrap.mockRejectedValueOnce("String error");

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.error).toBe("Unknown error");
      });

      expect(result.current.validationData).toBeNull();

      consoleErrorSpy.mockRestore();
    });
  });

  describe("manual refetch", () => {
    it("should allow manual refetch via refetch function", async () => {
      const mockValidationData: CrossReferenceValidationGroup = {
        groupName: "Test",
        description: null,
        summary: null,
        priority: "High",
        validationRule: null,
        validations: [],
        isValid: true
      };

      mockUnwrap.mockResolvedValue(mockValidationData);

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.validationData).toEqual(mockValidationData);
      });

      expect(mockTriggerFetch).toHaveBeenCalledTimes(1);

      // Manual refetch
      act(() => {
        result.current.refetch();
      });

      await waitFor(() => {
        expect(mockTriggerFetch).toHaveBeenCalledTimes(2);
      });

      // Refetch should use preferCacheValue = false
      expect(mockTriggerFetch).toHaveBeenLastCalledWith(2024, false);
    });

    it("should not fetch when refetch called with null profitYear", () => {
      const { result } = renderHook(() => useBalanceValidation(null));

      act(() => {
        result.current.refetch();
      });

      expect(mockTriggerFetch).not.toHaveBeenCalled();
    });

    it("should not fetch when refetch called with invalid profitYear", () => {
      const { result } = renderHook(() => useBalanceValidation(0));

      act(() => {
        result.current.refetch();
      });

      expect(mockTriggerFetch).not.toHaveBeenCalled();
    });
  });

  describe("state transitions", () => {
    it("should clear state when profitYear changes to null", async () => {
      const mockValidationData: CrossReferenceValidationGroup = {
        groupName: "Test",
        description: null,
        summary: null,
        priority: "High",
        validationRule: null,
        validations: [],
        isValid: true
      };

      mockUnwrap.mockResolvedValueOnce(mockValidationData);

      const { result, rerender } = renderHook(({ year }: { year: number | null }) => useBalanceValidation(year), {
        initialProps: { year: 2024 as number | null }
      });

      await waitFor(() => {
        expect(result.current.validationData).toEqual(mockValidationData);
      });

      // Change to null
      rerender({ year: null });

      await waitFor(() => {
        expect(result.current.validationData).toBeNull();
      });

      expect(result.current.error).toBeNull();
    });

    it("should handle multiple rapid profitYear changes", async () => {
      mockUnwrap.mockResolvedValue({
        groupName: "Test",
        description: null,
        summary: null,
        priority: "High" as const,
        validationRule: null,
        validations: [],
        isValid: true
      });

      const { rerender } = renderHook(({ year }: { year: number | null }) => useBalanceValidation(year), {
        initialProps: { year: 2024 as number | null }
      });

      rerender({ year: 2025 });
      rerender({ year: 2026 });

      await waitFor(() => {
        expect(mockTriggerFetch).toHaveBeenCalledTimes(3);
      });

      expect(mockTriggerFetch).toHaveBeenCalledWith(2024, true);
      expect(mockTriggerFetch).toHaveBeenCalledWith(2025, true);
      expect(mockTriggerFetch).toHaveBeenCalledWith(2026, true);
    });
  });

  describe("complex validation data", () => {
    it("should handle validation data with multiple validations", async () => {
      const mockValidationData: CrossReferenceValidationGroup = {
        groupName: "ALLOC/PAID ALLOC Transfers",
        description: null,
        summary: null,
        priority: "High",
        validationRule: null,
        validations: [
          {
            reportCode: "PAY443",
            fieldName: "IncomingAllocations",
            expectedValue: 5000,
            currentValue: 5000,
            isValid: true,
            variance: 0,
            message: "Match",
            archivedAt: null,
            notes: null
          },
          {
            reportCode: "PAY443",
            fieldName: "OutgoingAllocations",
            expectedValue: 3000,
            currentValue: 3100,
            isValid: false,
            variance: 100,
            message: "Does not match",
            archivedAt: null,
            notes: null
          },
          {
            reportCode: "PAY443",
            fieldName: "NetAllocTransfer",
            expectedValue: 2000,
            currentValue: 1900,
            isValid: false,
            variance: -100,
            message: "Does not match",
            archivedAt: null,
            notes: null
          }
        ],
        isValid: false
      };

      mockUnwrap.mockResolvedValueOnce(mockValidationData);

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.validationData).toEqual(mockValidationData);
      });

      expect(result.current.validationData?.validations).toHaveLength(3);
      expect(result.current.validationData?.isValid).toBe(false);
    });
  });
});
