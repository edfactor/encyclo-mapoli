import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useBalanceValidation } from "./useBalanceValidation";
import type { CrossReferenceValidationGroup } from "../types/validation/cross-reference-validation";

// Mock global fetch
const mockFetch = vi.fn();
global.fetch = mockFetch;

describe("useBalanceValidation", () => {
  beforeEach(() => {
    mockFetch.mockClear();
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

      expect(mockFetch).not.toHaveBeenCalled();
    });

    it("should not fetch when profitYear is 0 or negative", () => {
      renderHook(() => useBalanceValidation(0));
      renderHook(() => useBalanceValidation(-1));

      expect(mockFetch).not.toHaveBeenCalled();
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

      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: async () => mockValidationData
      });

      const { result } = renderHook(() => useBalanceValidation(2024));

      // Initially loading
      expect(result.current.isLoading).toBe(true);

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(mockFetch).toHaveBeenCalledWith("/api/balance-validation/alloc-transfers/2024", {
        method: "GET",
        headers: {
          "Content-Type": "application/json"
        }
      });

      expect(result.current.validationData).toEqual(mockValidationData);
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

      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: async () => mockData2024
      });

      const { result, rerender } = renderHook(({ year }: { year: number | null }) => useBalanceValidation(year), {
        initialProps: { year: 2024 as number | null }
      });

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.validationData).toEqual(mockData2024);

      // Change profitYear
      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: async () => mockData2025
      });

      rerender({ year: 2025 });

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.validationData).toEqual(mockData2025);
      expect(mockFetch).toHaveBeenCalledTimes(2);
    });
  });

  describe("404 handling (no validation data)", () => {
    it("should handle 404 gracefully (no validation data available)", async () => {
      mockFetch.mockResolvedValueOnce({
        ok: false,
        status: 404,
        statusText: "Not Found"
      });

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.validationData).toBeNull();
      expect(result.current.error).toBeNull();
    });
  });

  describe("error handling", () => {
    it("should handle network errors", async () => {
      mockFetch.mockRejectedValueOnce(new Error("Network failure"));

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.validationData).toBeNull();
      expect(result.current.error).toBe("Network failure");
    });

    it("should handle HTTP error responses (non-404)", async () => {
      mockFetch.mockResolvedValueOnce({
        ok: false,
        status: 500,
        statusText: "Internal Server Error"
      });

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.validationData).toBeNull();
      expect(result.current.error).toBe("Failed to fetch balance validation: Internal Server Error");
    });

    it("should handle non-Error objects", async () => {
      mockFetch.mockRejectedValueOnce("String error");

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.validationData).toBeNull();
      expect(result.current.error).toBe("Unknown error");
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

      mockFetch.mockResolvedValue({
        ok: true,
        json: async () => mockValidationData
      });

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(mockFetch).toHaveBeenCalledTimes(1);

      // Manual refetch
      act(() => {
        result.current.refetch();
      });

      await waitFor(() => {
        expect(mockFetch).toHaveBeenCalledTimes(2);
      });

      expect(mockFetch).toHaveBeenCalledWith("/api/balance-validation/alloc-transfers/2024", {
        method: "GET",
        headers: {
          "Content-Type": "application/json"
        }
      });
    });

    it("should not fetch when refetch called with null profitYear", () => {
      const { result } = renderHook(() => useBalanceValidation(null));

      act(() => {
        result.current.refetch();
      });

      expect(mockFetch).not.toHaveBeenCalled();
    });

    it("should not fetch when refetch called with invalid profitYear", () => {
      const { result } = renderHook(() => useBalanceValidation(0));

      act(() => {
        result.current.refetch();
      });

      expect(mockFetch).not.toHaveBeenCalled();
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

      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: async () => mockValidationData
      });

      const { result, rerender } = renderHook(({ year }: { year: number | null }) => useBalanceValidation(year), {
        initialProps: { year: 2024 as number | null }
      });

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.validationData).toEqual(mockValidationData);

      // Change to null
      rerender({ year: null });

      expect(result.current.validationData).toBeNull();
      expect(result.current.error).toBeNull();
    });

    it("should handle multiple rapid profitYear changes", async () => {
      mockFetch.mockResolvedValue({
        ok: true,
        json: async () => ({
          groupName: "Test",
          description: null,
          summary: null,
          priority: "High" as const,
          validationRule: null,
          validations: [],
          isValid: true
        })
      });

      const { rerender } = renderHook(({ year }: { year: number | null }) => useBalanceValidation(year), {
        initialProps: { year: 2024 as number | null }
      });

      rerender({ year: 2025 });
      rerender({ year: 2026 });

      await waitFor(() => {
        expect(mockFetch).toHaveBeenCalledTimes(3);
      });

      expect(mockFetch).toHaveBeenCalledWith("/api/balance-validation/alloc-transfers/2024", expect.any(Object));
      expect(mockFetch).toHaveBeenCalledWith("/api/balance-validation/alloc-transfers/2025", expect.any(Object));
      expect(mockFetch).toHaveBeenCalledWith("/api/balance-validation/alloc-transfers/2026", expect.any(Object));
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

      mockFetch.mockResolvedValueOnce({
        ok: true,
        json: async () => mockValidationData
      });

      const { result } = renderHook(() => useBalanceValidation(2024));

      await waitFor(() => {
        expect(result.current.isLoading).toBe(false);
      });

      expect(result.current.validationData).toEqual(mockValidationData);
      expect(result.current.validationData?.validations).toHaveLength(3);
      expect(result.current.validationData?.isValid).toBe(false);
    });
  });
});
