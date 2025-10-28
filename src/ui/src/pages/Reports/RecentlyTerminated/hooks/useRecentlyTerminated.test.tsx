import { renderHook, act } from "@testing-library/react";
import { describe, it, expect, beforeEach, vi } from "vitest";
import useRecentlyTerminated from "./useRecentlyTerminated";
import * as useLazyGetRecentlyTerminatedReportQuery from "reduxstore/api/YearsEndApi";
import * as useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import * as useGridPagination from "../../../../hooks/useGridPagination";
import * as useMissiveAlerts from "../../../../hooks/useMissiveAlerts";
import { RecentlyTerminatedRecord, Paged } from "reduxstore/types";

// Mock data types
interface MockRecentlyTerminatedRecord extends RecentlyTerminatedRecord {
  fullName: string;
  terminationDate: string;
  badgeNumber: number;
  [key: string]: unknown;
}

interface MockPaged<T> extends Paged<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// Create mock data
const createMockRecentlyTerminatedRecord = (
  overrides?: Partial<MockRecentlyTerminatedRecord>
): MockRecentlyTerminatedRecord => ({
  fullName: "Jane Doe",
  terminationDate: "2024-01-15",
  badgeNumber: 12345,
  ...overrides
});

const createMockPagedData = (
  items: MockRecentlyTerminatedRecord[] = []
): MockPaged<RecentlyTerminatedRecord> => ({
  items,
  pageNumber: 0,
  pageSize: 25,
  totalCount: items.length,
  hasNextPage: false,
  hasPreviousPage: false
});

const createMockRecentlyTerminatedResponse = (items: MockRecentlyTerminatedRecord[] = []) => ({
  reportName: "Recently Terminated Report",
  reportDate: "2024-01-20",
  startDate: "2024-01-01",
  endDate: "2024-12-31",
  dataSource: "Live",
  response: createMockPagedData(items),
  resultHash: "hash123"
});

// Mock hooks
const mockTriggerSearch = vi.fn();
const mockAddAlert = vi.fn();
const mockClearAlerts = vi.fn();
const mockResetPagination = vi.fn();

describe("useRecentlyTerminated", () => {
  beforeEach(() => {
    vi.clearAllMocks();

    // Mock useDecemberFlowProfitYear
    vi.spyOn(useDecemberFlowProfitYear, "default").mockReturnValue(2024);

    // Mock useMissiveAlerts
    vi.spyOn(useMissiveAlerts, "useMissiveAlerts").mockReturnValue({
      missiveAlerts: [],
      addAlert: mockAddAlert,
      clearAlerts: mockClearAlerts
    } as ReturnType<typeof useMissiveAlerts.useMissiveAlerts>);

    // Mock useGridPagination
    vi.spyOn(useGridPagination, "useGridPagination").mockReturnValue({
      pageNumber: 0,
      pageSize: 25,
      sortBy: "fullName, terminationDate",
      isSortDescending: false,
      resetPagination: mockResetPagination,
      goToPage: vi.fn(),
      onPaginationChange: vi.fn()
    } as ReturnType<typeof useGridPagination.useGridPagination>);

    // Mock lazy query
    mockTriggerSearch.mockResolvedValue({
      data: createMockRecentlyTerminatedResponse([
        createMockRecentlyTerminatedRecord({ badgeNumber: 12345, fullName: "Jane Doe" }),
        createMockRecentlyTerminatedRecord({ badgeNumber: 12346, fullName: "Bob Smith" })
      ])
    });

    vi.spyOn(useLazyGetRecentlyTerminatedReportQuery, "useLazyGetRecentlyTerminatedReportQuery").mockReturnValue([
      mockTriggerSearch,
      { isFetching: false }
    ] as ReturnType<typeof useLazyGetRecentlyTerminatedReportQuery.useLazyGetRecentlyTerminatedReportQuery>);
  });

  describe("Initial State", () => {
    it("should have correct initial state", () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      expect(result.current.isSearching).toBe(false);
      expect(result.current.searchCompleted).toBe(false);
      expect(result.current.searchParams).toBeNull();
      expect(result.current.searchError).toBeNull();
      expect(result.current.reportData).toBeNull();
      expect(result.current.isLoadingReport).toBe(false);
      expect(result.current.reportError).toBeNull();
    });

    it("should have grid pagination initialized", () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      expect(result.current.gridPagination).toBeDefined();
      expect(result.current.gridPagination.pageNumber).toBe(0);
      expect(result.current.gridPagination.pageSize).toBe(25);
    });

    it("should initialize with correct sort parameters", () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      expect(result.current.gridPagination.sortBy).toBe("fullName, terminationDate");
      expect(result.current.gridPagination.isSortDescending).toBe(false);
    });
  });

  describe("executeSearch", () => {
    it("should execute search with valid dates", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());
      const beginningDate = "2024-01-01";
      const endingDate = "2024-12-31";

      await act(async () => {
        const searchResult = await result.current.executeSearch(beginningDate, endingDate);
        expect(searchResult).toBe(true);
      });

      expect(mockTriggerSearch).toHaveBeenCalledWith(
        expect.objectContaining({
          profitYear: 2024,
          beginningDate,
          endingDate
        })
      );
      expect(result.current.searchParams).toEqual({ beginningDate, endingDate });
      expect(result.current.searchCompleted).toBe(true);
      expect(result.current.reportData).toBeDefined();
      expect(result.current.reportData?.response.items).toHaveLength(2);
    });

    it("should set isSearching to true during search", async () => {
      mockTriggerSearch.mockImplementation(
        () => new Promise((resolve) => setTimeout(() => resolve({ data: createMockRecentlyTerminatedResponse() }), 100))
      );

      const { result } = renderHook(() => useRecentlyTerminated());

      act(() => {
        result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      // Initially searching
      expect(result.current.isSearching).toBe(true);
    });

    it("should pass correct pagination parameters", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(mockTriggerSearch).toHaveBeenCalledWith(
        expect.objectContaining({
          pagination: {
            skip: 0,
            take: 25,
            sortBy: "fullName, terminationDate",
            isSortDescending: false
          }
        })
      );
    });

    it("should handle search errors", async () => {
      const errorMsg = "Network error";
      mockTriggerSearch.mockRejectedValue({
        data: { detail: errorMsg }
      });

      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        const searchResult = await result.current.executeSearch("2024-01-01", "2024-12-31");
        expect(searchResult).toBe(false);
      });

      expect(result.current.searchError).toBe(errorMsg);
      expect(result.current.isSearching).toBe(false);
      expect(mockAddAlert).toHaveBeenCalledWith(
        expect.objectContaining({
          severity: "Error",
          message: "Search Failed"
        })
      );
    });

    it("should use default error message when error detail is missing", async () => {
      mockTriggerSearch.mockRejectedValue({ data: {} });

      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(result.current.searchError).toBe("Failed to search recently terminated employees");
    });

    it("should clear alerts on successful search", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(mockClearAlerts).toHaveBeenCalled();
    });

    it("should reset pagination on new search", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(mockResetPagination).toHaveBeenCalled();
    });

    it("should populate report data on successful search", async () => {
      const mockRecords = [
        createMockRecentlyTerminatedRecord({ badgeNumber: 12345, fullName: "Jane Doe", terminationDate: "2024-01-15" }),
        createMockRecentlyTerminatedRecord({ badgeNumber: 12346, fullName: "Bob Smith", terminationDate: "2024-01-20" })
      ];
      const mockData = createMockRecentlyTerminatedResponse(mockRecords);

      mockTriggerSearch.mockResolvedValue({ data: mockData });

      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(result.current.reportData).toEqual(mockData);
      expect(result.current.reportData?.response.items).toHaveLength(2);
    });
  });

  describe("resetSearch", () => {
    it("should reset state to initial", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      // First, perform a search
      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(result.current.searchCompleted).toBe(true);
      expect(result.current.reportData).not.toBeNull();

      // Then reset
      act(() => {
        result.current.resetSearch();
      });

      expect(result.current.searchCompleted).toBe(false);
      expect(result.current.searchParams).toBeNull();
      expect(result.current.searchError).toBeNull();
      expect(result.current.reportData).toBeNull();
      expect(result.current.reportError).toBeNull();
    });

    it("should clear alerts on reset", () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      act(() => {
        result.current.resetSearch();
      });

      expect(mockClearAlerts).toHaveBeenCalled();
    });

    it("should reset pagination on reset", () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      act(() => {
        result.current.resetSearch();
      });

      expect(mockResetPagination).toHaveBeenCalled();
    });

    it("should set searchCompleted to false after reset", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      // Search first
      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(result.current.searchCompleted).toBe(true);

      // Reset
      act(() => {
        result.current.resetSearch();
      });

      expect(result.current.searchCompleted).toBe(false);
    });
  });

  describe("Pagination", () => {
    it("should handle pagination change with search params", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      // First perform search
      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      // Reset mock to track pagination calls
      mockTriggerSearch.mockClear();

      // Simulate pagination change
      const paginationCallback = (useGridPagination.useGridPagination as ReturnType<typeof vi.spyOn>).mock
        .results[0].value.onPaginationChange;

      await act(async () => {
        await paginationCallback(1, 25, { sortBy: "fullName, terminationDate", isSortDescending: false });
      });

      expect(mockTriggerSearch).toHaveBeenCalledWith(
        expect.objectContaining({
          pagination: expect.objectContaining({
            skip: 25,
            take: 25
          })
        })
      );
    });

    it("should not paginate if no search params exist", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      mockTriggerSearch.mockClear();

      const paginationCallback = (useGridPagination.useGridPagination as ReturnType<typeof vi.spyOn>).mock
        .results[0].value.onPaginationChange;

      await act(async () => {
        await paginationCallback(1, 25, { sortBy: "fullName, terminationDate", isSortDescending: false });
      });

      expect(mockTriggerSearch).not.toHaveBeenCalled();
    });

    it("should maintain search params during pagination", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      const beginningDate = "2024-01-01";
      const endingDate = "2024-12-31";

      // Search
      await act(async () => {
        await result.current.executeSearch(beginningDate, endingDate);
      });

      // Verify search params are stored
      expect(result.current.searchParams).toEqual({ beginningDate, endingDate });

      // Simulate pagination to next page
      const paginationCallback = (useGridPagination.useGridPagination as ReturnType<typeof vi.spyOn>).mock
        .results[0].value.onPaginationChange;

      mockTriggerSearch.mockClear();

      await act(async () => {
        await paginationCallback(1, 25, { sortBy: "fullName, terminationDate", isSortDescending: false });
      });

      // Verify the same search params were used in pagination
      expect(mockTriggerSearch).toHaveBeenCalledWith(
        expect.objectContaining({
          profitYear: 2024,
          beginningDate,
          endingDate
        })
      );
    });

    it("should handle pagination errors", async () => {
      const errorMsg = "Pagination failed";
      mockTriggerSearch.mockRejectedValue({
        data: { detail: errorMsg }
      });

      const { result } = renderHook(() => useRecentlyTerminated());

      // First search succeeds
      mockTriggerSearch.mockResolvedValueOnce({ data: createMockPagedData() });

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      // Reset pagination mock for next call
      mockTriggerSearch.mockRejectedValue({
        data: { detail: errorMsg }
      });

      // Simulate pagination that fails
      const paginationCallback = (useGridPagination.useGridPagination as ReturnType<typeof vi.spyOn>).mock
        .results[0].value.onPaginationChange;

      await act(async () => {
        await paginationCallback(1, 25, { sortBy: "fullName, terminationDate", isSortDescending: false });
      });

      expect(result.current.reportError).toBe(errorMsg);
    });
  });

  describe("Error Handling", () => {
    it("should handle network errors during search", async () => {
      mockTriggerSearch.mockRejectedValue(new Error("Network error"));

      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        const searchResult = await result.current.executeSearch("2024-01-01", "2024-12-31");
        expect(searchResult).toBe(false);
      });

      expect(result.current.searchError).toBe("Failed to search recently terminated employees");
      expect(result.current.isSearching).toBe(false);
    });

    it("should handle missing search params in pagination", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      mockTriggerSearch.mockClear();
      mockAddAlert.mockClear();

      const paginationCallback = (useGridPagination.useGridPagination as ReturnType<typeof vi.spyOn>).mock
        .results[0].value.onPaginationChange;

      await act(async () => {
        await paginationCallback(0, 25, { sortBy: "fullName, terminationDate", isSortDescending: false });
      });

      expect(mockTriggerSearch).not.toHaveBeenCalled();
      expect(mockAddAlert).not.toHaveBeenCalled();
    });

    it("should show alert on search error", async () => {
      const errorMsg = "Service unavailable";
      mockTriggerSearch.mockRejectedValue({
        data: { detail: errorMsg }
      });

      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(mockAddAlert).toHaveBeenCalledWith(
        expect.objectContaining({
          id: 999,
          severity: "Error",
          message: "Search Failed",
          description: errorMsg
        })
      );
    });

    it("should show alert on pagination error", async () => {
      const errorMsg = "Pagination failed";
      mockTriggerSearch.mockRejectedValue({
        data: { detail: errorMsg }
      });

      const { result } = renderHook(() => useRecentlyTerminated());

      // Initial successful search
      mockTriggerSearch.mockResolvedValueOnce({ data: createMockPagedData() });

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      // Reset for pagination failure
      mockTriggerSearch.mockRejectedValue({
        data: { detail: errorMsg }
      });
      mockAddAlert.mockClear();

      const paginationCallback = (useGridPagination.useGridPagination as ReturnType<typeof vi.spyOn>).mock
        .results[0].value.onPaginationChange;

      await act(async () => {
        await paginationCallback(1, 25, { sortBy: "fullName, terminationDate", isSortDescending: false });
      });

      expect(mockAddAlert).toHaveBeenCalledWith(
        expect.objectContaining({
          severity: "Error",
          message: "Report Failed"
        })
      );
    });
  });

  describe("Complex Scenarios", () => {
    it("should handle multiple searches sequentially", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      // First search
      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-06-30");
      });

      expect(result.current.searchParams).toEqual({
        beginningDate: "2024-01-01",
        endingDate: "2024-06-30"
      });

      // Second search
      await act(async () => {
        await result.current.executeSearch("2024-07-01", "2024-12-31");
      });

      expect(result.current.searchParams).toEqual({
        beginningDate: "2024-07-01",
        endingDate: "2024-12-31"
      });

      expect(mockTriggerSearch).toHaveBeenCalledTimes(2);
    });

    it("should handle search -> paginate -> reset workflow", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      // Search
      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(result.current.searchCompleted).toBe(true);
      expect(result.current.reportData).not.toBeNull();

      // Paginate
      mockTriggerSearch.mockClear();

      const paginationCallback = (useGridPagination.useGridPagination as ReturnType<typeof vi.spyOn>).mock
        .results[0].value.onPaginationChange;

      await act(async () => {
        await paginationCallback(1, 25, { sortBy: "fullName, terminationDate", isSortDescending: false });
      });

      expect(mockTriggerSearch).toHaveBeenCalledTimes(1);

      // Reset
      act(() => {
        result.current.resetSearch();
      });

      expect(result.current.searchCompleted).toBe(false);
      expect(result.current.reportData).toBeNull();
    });

    it("should handle search with large date range", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        await result.current.executeSearch("2020-01-01", "2024-12-31");
      });

      expect(mockTriggerSearch).toHaveBeenCalledWith(
        expect.objectContaining({
          beginningDate: "2020-01-01",
          endingDate: "2024-12-31"
        })
      );
      expect(result.current.searchParams).toEqual({
        beginningDate: "2020-01-01",
        endingDate: "2024-12-31"
      });
    });

    it("should handle search with same beginning and ending dates", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        await result.current.executeSearch("2024-01-15", "2024-01-15");
      });

      expect(mockTriggerSearch).toHaveBeenCalledWith(
        expect.objectContaining({
          beginningDate: "2024-01-15",
          endingDate: "2024-01-15"
        })
      );
    });

    it("should clear alerts and reset pagination together on reset", async () => {
      const { result } = renderHook(() => useRecentlyTerminated());

      // Search first
      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      mockClearAlerts.mockClear();
      mockResetPagination.mockClear();

      // Reset
      act(() => {
        result.current.resetSearch();
      });

      expect(mockClearAlerts).toHaveBeenCalled();
      expect(mockResetPagination).toHaveBeenCalled();
    });
  });

  describe("Data Population", () => {
    it("should populate report data with multiple records", async () => {
      const mockRecords = [
        createMockRecentlyTerminatedRecord({
          badgeNumber: 12345,
          fullName: "Jane Doe",
          terminationDate: "2024-01-15"
        }),
        createMockRecentlyTerminatedRecord({
          badgeNumber: 12346,
          fullName: "Bob Smith",
          terminationDate: "2024-01-20"
        }),
        createMockRecentlyTerminatedRecord({
          badgeNumber: 12347,
          fullName: "Alice Johnson",
          terminationDate: "2024-02-10"
        })
      ];

      mockTriggerSearch.mockResolvedValue({
        data: createMockRecentlyTerminatedResponse(mockRecords)
      });

      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(result.current.reportData?.response.items).toHaveLength(3);
      expect(result.current.reportData?.response.totalCount).toBe(3);
    });

    it("should handle empty search results", async () => {
      mockTriggerSearch.mockResolvedValue({
        data: createMockRecentlyTerminatedResponse([])
      });

      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(result.current.reportData?.response.items).toEqual([]);
      expect(result.current.reportData?.response.totalCount).toBe(0);
      expect(result.current.searchCompleted).toBe(true);
    });
  });
});
