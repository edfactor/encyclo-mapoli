import { act, renderHook, waitFor } from "@testing-library/react";
import * as useLazyGetRecentlyTerminatedReportQuery from "reduxstore/api/AdhocApi";
import { RecentlyTerminatedDetail } from "reduxstore/types";
import type { RecentlyTerminatedResponse } from "../../../../../types/reports/recent-termination";
import { Paged } from "smart-ui-library";
import { beforeEach, describe, expect, it, vi } from "vitest";
import * as useDecemberFlowProfitYear from "../../../../../hooks/useDecemberFlowProfitYear";
import * as useGridPagination from "../../../../../hooks/useGridPagination";
import * as useMissiveAlerts from "../../../../../hooks/useMissiveAlerts";
import useRecentlyTerminated from "../useRecentlyTerminated";

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
  overrides?: Partial<RecentlyTerminatedDetail>
): RecentlyTerminatedDetail => ({
  badgeNumber: 12345,
  fullName: "Jane Doe",
  firstName: "Jane",
  lastName: "Doe",
  middleInitial: "M",
  ssn: "123-45-6789",
  terminationDate: "2024-01-15",
  terminationCodeId: "1",
  terminationCode: "RIF",
  address: "123 Main St",
  address2: "",
  city: "Springfield",
  state: "IL",
  postalCode: "62701",
  isExecutive: false,
  ...overrides
});

const createMockPagedData = (items: RecentlyTerminatedDetail[] = []): MockPaged<RecentlyTerminatedDetail> => ({
  items,
  pageNumber: 0,
  pageSize: 25,
  totalCount: items.length,
  hasNextPage: false,
  hasPreviousPage: false,
  total: items.length,
  totalPages: Math.ceil(items.length / 25),
  currentPage: 1,
  results: items
});

// Create mock paged data response (this is what the API returns after unwrap)
const createMockRecentlyTerminatedResponse = (items: RecentlyTerminatedDetail[] = []) => ({
  reportName: "Recently Terminated Report",
  reportDate: "2024-01-15",
  startDate: "2024-01-01",
  endDate: "2024-12-31",
  dataSource: "Test Data",
  response: createMockPagedData(items),
  resultHash: "test-hash"
});

// Helper to create RTK Query-like promise with unwrap method
interface RTKQueryPromise<T> extends Promise<{ data: T }> {
  unwrap: () => Promise<T>;
}

const createMockRTKQueryPromise = (
  data: RecentlyTerminatedResponse | null = null,
  error: unknown = null
): RTKQueryPromise<RecentlyTerminatedResponse | null> => {
  const promise = error ? Promise.reject(error) : Promise.resolve({ data });

  if (error) {
    (promise as RTKQueryPromise<RecentlyTerminatedResponse | null>).unwrap = () => Promise.reject(error);
  } else {
    (promise as RTKQueryPromise<RecentlyTerminatedResponse | null>).unwrap = () => Promise.resolve(data);
  }

  return promise as RTKQueryPromise<RecentlyTerminatedResponse | null>;
};

// Mock hooks
const mockTriggerSearch = vi.fn();
const mockAddAlert = vi.fn();
const mockAddAlerts = vi.fn();
const mockClearAlerts = vi.fn();
const mockRemoveAlert = vi.fn();
const mockHasAlert = vi.fn();
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
      addAlerts: mockAddAlerts,
      clearAlerts: mockClearAlerts,
      removeAlert: mockRemoveAlert,
      hasAlert: mockHasAlert
    } as ReturnType<typeof useMissiveAlerts.useMissiveAlerts>);

    // Mock useGridPagination - capture the callback
    const mockHandlePaginationChange = vi.fn();
    const mockSortParams = {
      sortBy: "fullName, terminationDate",
      isSortDescending: false
    };
    vi.spyOn(useGridPagination, "useGridPagination").mockImplementation((config) => {
      // When the hook calls useGridPagination with a callback, capture it and wrap it
      if (config?.onPaginationChange) {
        mockHandlePaginationChange.mockImplementation((pageNumber: number, pageSize: number) => {
          // Simulate real useGridPagination behavior: call callback with sortParams
          return config.onPaginationChange!(pageNumber, pageSize, mockSortParams);
        });
      }
      return {
        pageNumber: 0,
        pageSize: 25,
        sortParams: mockSortParams,
        handlePaginationChange: mockHandlePaginationChange,
        handleSortChange: vi.fn(),
        resetPagination: mockResetPagination
      } as unknown as ReturnType<typeof useGridPagination.useGridPagination>;
    });

    // Mock lazy query - default to returning data
    mockTriggerSearch.mockImplementation(() =>
      createMockRTKQueryPromise(
        createMockRecentlyTerminatedResponse([
          createMockRecentlyTerminatedRecord({ badgeNumber: 12345, fullName: "Jane Doe" }),
          createMockRecentlyTerminatedRecord({ badgeNumber: 12346, fullName: "Bob Smith" })
        ])
      )
    );

    vi.spyOn(useLazyGetRecentlyTerminatedReportQuery, "useLazyGetRecentlyTerminatedReportQuery").mockReturnValue([
      mockTriggerSearch,
      {
        data: undefined,
        isLoading: false,
        isSuccess: false,
        isError: false,
        error: null,
        isFetching: false,
        isUninitialized: true,
        currentData: undefined,
        requestId: undefined,
        endpointName: "getRecentlyTerminatedReport",
        startedTimeStamp: undefined,
        fulfilledTimeStamp: undefined
      },
      { lastArg: undefined, requestStatus: "uninitialized" }
    ] as unknown as ReturnType<typeof useLazyGetRecentlyTerminatedReportQuery.useLazyGetRecentlyTerminatedReportQuery>);
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

      expect(result.current.gridPagination.sortParams.sortBy).toBe("fullName, terminationDate");
      expect(result.current.gridPagination.sortParams.isSortDescending).toBe(false);
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
      expect(
        (result.current.reportData?.response as unknown as MockPaged<RecentlyTerminatedDetail>)?.items
      ).toHaveLength(2);
    });

    it("should set isSearching to true during search", async () => {
      mockTriggerSearch.mockImplementation(() => {
        const delayedPromise = new Promise<RecentlyTerminatedResponse>((resolve) =>
          setTimeout(() => resolve(createMockRecentlyTerminatedResponse()), 100)
        ) as unknown as RTKQueryPromise<RecentlyTerminatedResponse>;
        (delayedPromise as RTKQueryPromise<RecentlyTerminatedResponse>).unwrap = () =>
          new Promise<RecentlyTerminatedResponse>((resolve) =>
            setTimeout(() => resolve(createMockRecentlyTerminatedResponse()), 100)
          );
        return delayedPromise;
      });

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
      const errorResponse = {
        data: { detail: errorMsg }
      };

      mockTriggerSearch.mockImplementation(() => createMockRTKQueryPromise(null, errorResponse));

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
      const errorResponse = { data: {} };

      mockTriggerSearch.mockImplementation(() => createMockRTKQueryPromise(null, errorResponse));

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

      mockTriggerSearch.mockImplementation(() => createMockRTKQueryPromise(mockData));

      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(result.current.reportData).toEqual(mockData);
      expect(
        (result.current.reportData?.response as unknown as MockPaged<RecentlyTerminatedDetail>)?.items
      ).toHaveLength(2);
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
      await act(async () => {
        await result.current.gridPagination.handlePaginationChange(1, 25);
      });

      // Wait for the deferred callback to execute (setTimeout in useGridPagination)
      await waitFor(() => {
        expect(mockTriggerSearch).toHaveBeenCalledWith(
          expect.objectContaining({
            pagination: expect.objectContaining({
              skip: 25,
              take: 25
            })
          })
        );
      });
    });

    it("should not paginate if no search params exist", async () => {
      mockTriggerSearch.mockClear();

      const mockSpyResult = vi.spyOn(useGridPagination, "useGridPagination");
      if (mockSpyResult.mock.results.length > 0) {
        const paginationCallback = mockSpyResult.mock.results[0].value.handlePaginationChange as unknown as (
          pageNumber: number,
          pageSize: number,
          sortParams: unknown
        ) => void;

        await act(async () => {
          await paginationCallback(1, 25, { sortBy: "fullName, terminationDate", isSortDescending: false });
        });
      }

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
      const mockSpyResult = vi.spyOn(useGridPagination, "useGridPagination");
      if (mockSpyResult.mock.results.length > 0) {
        const paginationCallback = mockSpyResult.mock.results[0].value.handlePaginationChange as unknown as (
          pageNumber: number,
          pageSize: number,
          sortParams: unknown
        ) => void;

        mockTriggerSearch.mockClear();

        await act(async () => {
          await paginationCallback(1, 25, { sortBy: "fullName, terminationDate", isSortDescending: false });
        });
      }

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
      const errorResponse = {
        data: { detail: errorMsg }
      };

      const { result } = renderHook(() => useRecentlyTerminated());

      // First search succeeds
      mockTriggerSearch.mockImplementationOnce(() => createMockRTKQueryPromise(createMockRecentlyTerminatedResponse()));

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      // Next call (pagination) fails
      mockTriggerSearch.mockImplementation(() => createMockRTKQueryPromise(null, errorResponse));

      // Simulate pagination that fails
      await act(async () => {
        await result.current.gridPagination.handlePaginationChange(1, 25);
      });

      // Wait for the deferred callback and error handling to complete
      await waitFor(() => {
        expect(result.current.reportError).toBe(errorMsg);
      });
    });
  });

  describe("Error Handling", () => {
    it("should handle network errors during search", async () => {
      mockTriggerSearch.mockImplementation(() => createMockRTKQueryPromise(null, new Error("Network error")));

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

      // Try to paginate without having searched first
      await act(async () => {
        await result.current.gridPagination.handlePaginationChange(0, 25);
      });

      expect(mockTriggerSearch).not.toHaveBeenCalled();
      expect(mockAddAlert).not.toHaveBeenCalled();
    });

    it("should show alert on search error", async () => {
      const errorMsg = "Service unavailable";
      const errorResponse = {
        data: { detail: errorMsg }
      };

      mockTriggerSearch.mockImplementation(() => createMockRTKQueryPromise(null, errorResponse));

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
      const errorResponse = {
        data: { detail: errorMsg }
      };

      const { result } = renderHook(() => useRecentlyTerminated());

      // Initial successful search
      mockTriggerSearch.mockImplementationOnce(() => createMockRTKQueryPromise(createMockRecentlyTerminatedResponse()));

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      // Next call (pagination) fails
      mockTriggerSearch.mockImplementation(() => createMockRTKQueryPromise(null, errorResponse));
      mockAddAlert.mockClear();

      await act(async () => {
        await result.current.gridPagination.handlePaginationChange(1, 25);
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

      await act(async () => {
        await result.current.gridPagination.handlePaginationChange(1, 25);
      });

      // Wait for the deferred callback to execute
      await waitFor(() => {
        expect(mockTriggerSearch).toHaveBeenCalledTimes(1);
      });

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

      mockTriggerSearch.mockImplementation(() =>
        createMockRTKQueryPromise(createMockRecentlyTerminatedResponse(mockRecords))
      );

      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(
        (result.current.reportData?.response as unknown as MockPaged<RecentlyTerminatedDetail>)?.items
      ).toHaveLength(3);
      expect((result.current.reportData?.response as unknown as MockPaged<RecentlyTerminatedDetail>)?.total).toBe(3);
    });

    it("should handle empty search results", async () => {
      mockTriggerSearch.mockImplementation(() => createMockRTKQueryPromise(createMockRecentlyTerminatedResponse([])));

      const { result } = renderHook(() => useRecentlyTerminated());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect((result.current.reportData?.response as unknown as MockPaged<RecentlyTerminatedDetail>)?.items).toEqual(
        []
      );
      expect((result.current.reportData?.response as unknown as MockPaged<RecentlyTerminatedDetail>)?.total).toBe(0);
      expect(result.current.searchCompleted).toBe(true);
    });
  });
});
