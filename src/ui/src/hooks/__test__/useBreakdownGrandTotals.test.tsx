import { renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { createMockStoreAndWrapper } from "../../test";
import { useBreakdownGrandTotals } from "../useBreakdownGrandTotals";

// Hoist mock functions
const {
  mockFetchGrandTotals,
  mockUseDecemberFlowProfitYear
} = vi.hoisted(() => ({
  mockFetchGrandTotals: vi.fn(),
  mockUseDecemberFlowProfitYear: vi.fn(() => 2024)
}));

// Mock the API hook while preserving the rest of the module
vi.mock("../../reduxstore/api/AdhocApi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("../../reduxstore/api/AdhocApi")>();
  return {
    ...actual,
    useLazyGetBreakdownGrandTotalsQuery: () => [
      mockFetchGrandTotals,
      { data: null }
    ]
  };
});

// Mock useDecemberFlowProfitYear
vi.mock("../useDecemberFlowProfitYear", () => ({
  default: mockUseDecemberFlowProfitYear
}));

describe("useBreakdownGrandTotals", () => {
  const mockApiResponse = {
    rows: [
      { category: "100% Vested", storeOther: 100, store700: 50, store701: 30, store800: 20, store801: 10, store802: 5, store900: 40, rowTotal: 255 },
      { category: "Partially Vested", storeOther: 50, store700: 25, store701: 15, store800: 10, store801: 5, store802: 2, store900: 20, rowTotal: 127 },
      { category: "Not Vested", storeOther: 25, store700: 12, store701: 8, store800: 5, store801: 3, store802: 1, store900: 10, rowTotal: 64 },
      { category: "Grand Total", storeOther: 175, store700: 87, store701: 53, store800: 35, store801: 18, store802: 8, store900: 70, rowTotal: 446 }
    ]
  };

  beforeEach(() => {
    vi.clearAllMocks();
    mockFetchGrandTotals.mockReturnValue({
      unwrap: () => Promise.resolve(mockApiResponse)
    });
  });

  it("should initialize with loading state", () => {
    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    const { result } = renderHook(() => useBreakdownGrandTotals(), { wrapper });

    expect(result.current.isLoading).toBe(true);
    expect(result.current.error).toBeNull();
    expect(result.current.rowData).toEqual([]);
  });

  it("should fetch data on mount when user has token", async () => {
    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    renderHook(() => useBreakdownGrandTotals(), { wrapper });

    await waitFor(() => {
      expect(mockFetchGrandTotals).toHaveBeenCalledWith({
        profitYear: 2024
      });
    });
  });

  it("should not fetch data when user has no token", async () => {
    const { wrapper } = createMockStoreAndWrapper({
      security: { token: null }
    });

    renderHook(() => useBreakdownGrandTotals(), { wrapper });

    // Wait a bit to ensure no call is made
    await new Promise(resolve => setTimeout(resolve, 50));
    expect(mockFetchGrandTotals).not.toHaveBeenCalled();
  });

  it("should pass under21Participants when option is true", async () => {
    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    renderHook(() => useBreakdownGrandTotals({ under21Participants: true }), { wrapper });

    await waitFor(() => {
      expect(mockFetchGrandTotals).toHaveBeenCalledWith({
        profitYear: 2024,
        under21Participants: true
      });
    });
  });

  it("should transform API response to row data correctly", async () => {
    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    const { result } = renderHook(() => useBreakdownGrandTotals(), { wrapper });

    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });

    expect(result.current.rowData).toHaveLength(3);
    expect(result.current.rowData[0]).toEqual({
      category: "100% Vested",
      ste1: 100,
      "700": 50,
      "701": 30,
      "800": 20,
      "801": 10,
      "802": 5,
      "900": 40,
      total: 255
    });
  });

  it("should extract grand total row correctly", async () => {
    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    const { result } = renderHook(() => useBreakdownGrandTotals(), { wrapper });

    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });

    expect(result.current.grandTotal).toEqual({
      category: "Grand Total",
      ste1: 175,
      "700": 87,
      "701": 53,
      "800": 35,
      "801": 18,
      "802": 8,
      "900": 70,
      total: 446
    });
  });

  it("should set error state when API call fails", async () => {
    mockFetchGrandTotals.mockReturnValue({
      unwrap: () => Promise.reject(new Error("API Error"))
    });

    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    const { result } = renderHook(() => useBreakdownGrandTotals(), { wrapper });

    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });

    expect(result.current.error).toBe("Failed to load employee data. Please try again.");
  });

  it("should use custom error message when provided", async () => {
    mockFetchGrandTotals.mockReturnValue({
      unwrap: () => Promise.reject(new Error("API Error"))
    });

    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    const { result } = renderHook(
      () => useBreakdownGrandTotals({ errorMessage: "Custom error message" }),
      { wrapper }
    );

    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });

    expect(result.current.error).toBe("Custom error message");
  });

  it("should handle empty rows in API response", async () => {
    mockFetchGrandTotals.mockReturnValue({
      unwrap: () => Promise.resolve({ rows: [] })
    });

    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    const { result } = renderHook(() => useBreakdownGrandTotals(), { wrapper });

    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });

    expect(result.current.rowData).toHaveLength(3); // Still has 3 rows with zero values
    expect(result.current.rowData[0].total).toBe(0);
  });

  it("should handle missing categories gracefully", async () => {
    mockFetchGrandTotals.mockReturnValue({
      unwrap: () => Promise.resolve({
        rows: [
          { category: "100% Vested", storeOther: 10, store700: 5, store701: 3, store800: 2, store801: 1, store802: 0, store900: 4, rowTotal: 25 }
          // Missing Partially Vested, Not Vested, and Grand Total
        ]
      })
    });

    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    const { result } = renderHook(() => useBreakdownGrandTotals(), { wrapper });

    await waitFor(() => {
      expect(result.current.isLoading).toBe(false);
    });

    // Should still return 3 rows, with zeros for missing categories
    expect(result.current.rowData).toHaveLength(3);
    expect(result.current.rowData[0].category).toBe("100% Vested");
    expect(result.current.rowData[0].total).toBe(25);
    expect(result.current.rowData[1].category).toBe("Partially Vested");
    expect(result.current.rowData[1].total).toBe(0); // Default to 0
    expect(result.current.grandTotal.total).toBe(0); // Default to 0
  });

  it("should refetch when profit year changes", async () => {
    const { wrapper } = createMockStoreAndWrapper({
      security: { token: "mock-token" }
    });

    const { rerender } = renderHook(() => useBreakdownGrandTotals(), { wrapper });

    await waitFor(() => {
      expect(mockFetchGrandTotals).toHaveBeenCalledTimes(1);
    });

    // Simulate profit year change
    mockUseDecemberFlowProfitYear.mockReturnValue(2025);
    rerender();

    await waitFor(() => {
      expect(mockFetchGrandTotals).toHaveBeenCalledTimes(2);
    });
  });
});
