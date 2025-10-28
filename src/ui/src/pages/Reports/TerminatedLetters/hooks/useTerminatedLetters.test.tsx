import { act, renderHook } from "@testing-library/react";
import * as useLazyGetTerminatedLettersDownloadQuery from "reduxstore/api/YearsEndApi";
import * as useLazyGetTerminatedLettersReportQuery from "reduxstore/api/YearsEndApi";
import { TerminatedLettersDetail } from "reduxstore/types";
import { Paged } from "smart-ui-library";
import { beforeEach, describe, expect, it, vi } from "vitest";
import * as useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import * as useGridPagination from "../../../../hooks/useGridPagination";
import * as useMissiveAlerts from "../../../../hooks/useMissiveAlerts";
import useTerminatedLetters from "./useTerminatedLetters";

// Mock data types
interface MockPaged<T> extends Paged<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// Create mock data
const createMockTerminatedLetter = (overrides?: Partial<TerminatedLettersDetail>): TerminatedLettersDetail => ({
  badgeNumber: 12345,
  fullName: "John Doe",
  firstName: "John",
  lastName: "Doe",
  middleInitial: "M",
  ssn: "123-45-6789",
  terminationDate: "2024-01-15",
  terminationCodeId: "1",
  address: "123 Main St",
  address2: "",
  city: "Springfield",
  state: "IL",
  postalCode: "62701",
  isExecutive: false,
  ...overrides
});

const createMockPagedData = (items: TerminatedLettersDetail[] = []): MockPaged<TerminatedLettersDetail> => ({
  items,
  pageNumber: 0,
  pageSize: 50,
  totalCount: items.length,
  hasNextPage: false,
  hasPreviousPage: false,
  total: items.length,
  totalPages: Math.ceil(items.length / 50),
  currentPage: 1,
  results: items
});

const createMockTerminatedLettersResponse = (items: TerminatedLettersDetail[] = []) => ({
  reportName: "Terminated Letters Report",
  reportDate: "2024-01-20",
  startDate: "2024-01-01",
  endDate: "2024-12-31",
  dataSource: "Live",
  response: createMockPagedData(items),
  resultHash: "hash123"
});

// Mock hooks
const mockTriggerSearch = vi.fn();
const mockTriggerDownload = vi.fn();
const mockAddAlert = vi.fn();
const mockAddAlerts = vi.fn();
const mockClearAlerts = vi.fn();
const mockRemoveAlert = vi.fn();
const mockHasAlert = vi.fn();
const mockResetPagination = vi.fn();
const mockGridPaginationChange = vi.fn();

describe("useTerminatedLetters", () => {
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

    // Mock useGridPagination
    vi.spyOn(useGridPagination, "useGridPagination").mockReturnValue({
      pageNumber: 0,
      pageSize: 50,
      sortParams: {
        sortBy: "fullName",
        isSortDescending: false
      },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: mockResetPagination
    } as ReturnType<typeof useGridPagination.useGridPagination>);

    // Mock lazy query
    mockTriggerSearch.mockResolvedValue({
      data: createMockTerminatedLettersResponse([
        createMockTerminatedLetter({ badgeNumber: 12345, fullName: "John Doe" }),
        createMockTerminatedLetter({ badgeNumber: 12346, fullName: "Jane Smith" })
      ])
    });

    mockTriggerDownload.mockResolvedValue({
      data: new Blob(["Terminated Letters Content"])
    });

    vi.spyOn(useLazyGetTerminatedLettersReportQuery, "useLazyGetTerminatedLettersReportQuery").mockReturnValue([
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
        endpointName: "getTerminatedLettersReport",
        startedTimeStamp: undefined,
        fulfilledTimeStamp: undefined
      },
      { lastArg: undefined, requestStatus: "uninitialized" }
    ] as unknown as ReturnType<typeof useLazyGetTerminatedLettersReportQuery.useLazyGetTerminatedLettersReportQuery>);

    vi.spyOn(useLazyGetTerminatedLettersDownloadQuery, "useLazyGetTerminatedLettersDownloadQuery").mockReturnValue([
      mockTriggerDownload,
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
        endpointName: "getTerminatedLettersDownload",
        startedTimeStamp: undefined,
        fulfilledTimeStamp: undefined
      },
      { lastArg: undefined, requestStatus: "uninitialized" }
    ] as unknown as ReturnType<
      typeof useLazyGetTerminatedLettersDownloadQuery.useLazyGetTerminatedLettersDownloadQuery
    >);
  });

  describe("Initial State", () => {
    it("should have correct initial state", () => {
      const { result } = renderHook(() => useTerminatedLetters());

      expect(result.current.isSearching).toBe(false);
      expect(result.current.searchCompleted).toBe(false);
      expect(result.current.searchParams).toBeNull();
      expect(result.current.searchError).toBeNull();
      expect(result.current.reportData).toBeNull();
      expect(result.current.isLoadingReport).toBe(false);
      expect(result.current.reportError).toBeNull();
      expect(result.current.selectedRows).toEqual([]);
      expect(result.current.isPrintDialogOpen).toBe(false);
      expect(result.current.printContent).toBe("");
    });

    it("should have grid pagination initialized", () => {
      const { result } = renderHook(() => useTerminatedLetters());

      expect(result.current.gridPagination).toBeDefined();
      expect(result.current.gridPagination.pageNumber).toBe(0);
      expect(result.current.gridPagination.pageSize).toBe(50);
    });
  });

  describe("executeSearch", () => {
    it("should execute search with valid dates", async () => {
      const { result } = renderHook(() => useTerminatedLetters());
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
      expect(result.current.reportData?.items).toHaveLength(2);
    });

    it("should set isSearching to true during search", async () => {
      mockTriggerSearch.mockImplementation(
        () => new Promise((resolve) => setTimeout(() => resolve({ data: createMockTerminatedLettersResponse() }), 100))
      );

      const { result } = renderHook(() => useTerminatedLetters());

      act(() => {
        result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      // Initially searching
      expect(result.current.isSearching).toBe(true);
    });

    it("should handle search errors", async () => {
      const errorMsg = "Network error";
      mockTriggerSearch.mockRejectedValue({
        data: { detail: errorMsg }
      });

      const { result } = renderHook(() => useTerminatedLetters());

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

      const { result } = renderHook(() => useTerminatedLetters());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(result.current.searchError).toBe("Failed to search terminated letters");
    });

    it("should clear alerts on successful search", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(mockClearAlerts).toHaveBeenCalled();
    });

    it("should reset pagination on new search", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(mockResetPagination).toHaveBeenCalled();
    });
  });

  describe("resetSearch", () => {
    it("should reset state to initial", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

      // First, perform a search
      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(result.current.searchCompleted).toBe(true);

      // Then reset
      act(() => {
        result.current.resetSearch();
      });

      expect(result.current.searchCompleted).toBe(false);
      expect(result.current.searchParams).toBeNull();
      expect(result.current.searchError).toBeNull();
      expect(result.current.reportData).toBeNull();
      expect(result.current.selectedRows).toEqual([]);
    });

    it("should clear alerts on reset", () => {
      const { result } = renderHook(() => useTerminatedLetters());

      act(() => {
        result.current.resetSearch();
      });

      expect(mockClearAlerts).toHaveBeenCalled();
    });

    it("should reset pagination on reset", () => {
      const { result } = renderHook(() => useTerminatedLetters());

      act(() => {
        result.current.resetSearch();
      });

      expect(mockResetPagination).toHaveBeenCalled();
    });
  });

  describe("Row Selection", () => {
    it("should set selected rows", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

      const mockRows: TerminatedLettersDetail[] = [
        createMockTerminatedLetter({ badgeNumber: 12345 }),
        createMockTerminatedLetter({ badgeNumber: 12346 })
      ];

      act(() => {
        result.current.setSelectedRows(mockRows);
      });

      expect(result.current.selectedRows).toEqual(mockRows);
      expect(result.current.selectedRows).toHaveLength(2);
    });

    it("should clear selected rows", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

      const mockRows: TerminatedLettersDetail[] = [createMockTerminatedLetter()];

      act(() => {
        result.current.setSelectedRows(mockRows);
      });

      expect(result.current.selectedRows).toHaveLength(1);

      act(() => {
        result.current.clearSelectedRows();
      });

      expect(result.current.selectedRows).toEqual([]);
    });

    it("should clear selected rows on successful search", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

      act(() => {
        result.current.setSelectedRows([createMockTerminatedLetter()]);
      });

      expect(result.current.selectedRows).toHaveLength(1);

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(result.current.selectedRows).toEqual([]);
    });
  });

  describe("Pagination", () => {
    it("should handle pagination change with search params", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

      // First perform search
      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      // Reset mock to track pagination calls
      mockTriggerSearch.mockClear();

      // Simulate pagination change using the mock's handlePaginationChange
      const mockSpyResult = vi.spyOn(useGridPagination, "useGridPagination");
      if (mockSpyResult.mock.results.length > 0) {
        const paginationCallback = mockSpyResult.mock.results[0].value.handlePaginationChange as unknown as (
          pageNumber: number,
          pageSize: number,
          sortParams: unknown
        ) => void;

        await act(async () => {
          await paginationCallback(1, 50, { sortBy: "fullName", isSortDescending: false });
        });
      }

      expect(mockTriggerSearch).toHaveBeenCalledWith(
        expect.objectContaining({
          pagination: expect.objectContaining({
            skip: 50,
            take: 50
          })
        })
      );
    });

    it("should not paginate if no search params exist", async () => {
      //const { result } = renderHook(() => useTerminatedLetters());

      mockTriggerSearch.mockClear();

      const mockSpyResult = vi.spyOn(useGridPagination, "useGridPagination");
      if (mockSpyResult.mock.results.length > 0) {
        const paginationCallback = mockSpyResult.mock.results[0].value.handlePaginationChange as unknown as (
          pageNumber: number,
          pageSize: number,
          sortParams: unknown
        ) => void;

        await act(async () => {
          await paginationCallback(1, 50, { sortBy: "fullName", isSortDescending: false });
        });
      }

      expect(mockTriggerSearch).not.toHaveBeenCalled();
    });
  });

  describe("Print Functionality", () => {
    it("should handle print with selected rows", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

      const mockRows: TerminatedLettersDetail[] = [
        createMockTerminatedLetter({ badgeNumber: 12345 }),
        createMockTerminatedLetter({ badgeNumber: 12346 })
      ];

      // Set search params and rows
      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      act(() => {
        result.current.setSelectedRows(mockRows);
      });

      // Handle print
      await act(async () => {
        await result.current.handlePrint();
      });

      expect(mockTriggerDownload).toHaveBeenCalledWith(
        expect.objectContaining({
          badgeNumbers: [12345, 12346],
          beginningDate: "2024-01-01",
          endingDate: "2024-12-31"
        })
      );

      expect(result.current.isPrintDialogOpen).toBe(true);
    });

    it("should not handle print without selected rows", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

      mockTriggerDownload.mockClear();

      await act(async () => {
        await result.current.handlePrint();
      });

      expect(mockTriggerDownload).not.toHaveBeenCalled();
    });

    it("should handle print errors", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

      mockTriggerDownload.mockRejectedValue(new Error("Download failed"));

      const mockRows: TerminatedLettersDetail[] = [createMockTerminatedLetter()];

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      act(() => {
        result.current.setSelectedRows(mockRows);
      });

      await act(async () => {
        await result.current.handlePrint();
      });

      expect(mockAddAlert).toHaveBeenCalledWith(
        expect.objectContaining({
          severity: "Error",
          message: "Print Failed"
        })
      );
    });

    it("should set print dialog state", () => {
      const { result } = renderHook(() => useTerminatedLetters());

      expect(result.current.isPrintDialogOpen).toBe(false);

      act(() => {
        result.current.setIsPrintDialogOpen(true);
      });

      expect(result.current.isPrintDialogOpen).toBe(true);

      act(() => {
        result.current.setIsPrintDialogOpen(false);
      });

      expect(result.current.isPrintDialogOpen).toBe(false);
    });

    it("should print content correctly", () => {
      const { result } = renderHook(() => useTerminatedLetters());

      const printContent = "Test print content\nLine 2";
      const windowOpenSpy = vi.spyOn(window, "open").mockReturnValue({
        document: {
          write: vi.fn(),
          close: vi.fn()
        },
        focus: vi.fn(),
        print: vi.fn(),
        close: vi.fn()
      } as unknown as Window);

      act(() => {
        result.current.printTerminatedLetters(printContent);
      });

      expect(windowOpenSpy).toHaveBeenCalledWith("", "_blank");

      windowOpenSpy.mockRestore();
    });
  });

  describe("Error Handling", () => {
    it("should handle pagination fetch errors", async () => {
      const errorMsg = "Pagination fetch failed";
      mockTriggerSearch.mockRejectedValue({
        data: { detail: errorMsg }
      });

      const { result } = renderHook(() => useTerminatedLetters());

      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(result.current.reportError).toBe(errorMsg);
      expect(mockAddAlert).toHaveBeenCalledWith(
        expect.objectContaining({
          severity: "Error",
          message: "Search Failed"
        })
      );
    });

    it("should handle missing search params in pagination", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

      mockTriggerSearch.mockClear();
      mockAddAlert.mockClear();

      const paginationCallback = (useGridPagination.useGridPagination as ReturnType<typeof vi.spyOn>).mock.results[0]
        .value.onPaginationChange;

      await act(async () => {
        await paginationCallback(0, 50, { sortBy: "fullName", isSortDescending: false });
      });

      expect(mockTriggerSearch).not.toHaveBeenCalled();
      expect(mockAddAlert).not.toHaveBeenCalled();
    });
  });

  describe("Complex Scenarios", () => {
    it("should handle multiple searches sequentially", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

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

    it("should handle search -> select -> print workflow", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

      // Search
      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      // Select rows
      const rows = [
        createMockTerminatedLetter({ badgeNumber: 12345 }),
        createMockTerminatedLetter({ badgeNumber: 12346 })
      ];

      act(() => {
        result.current.setSelectedRows(rows);
      });

      expect(result.current.selectedRows).toHaveLength(2);

      // Print
      await act(async () => {
        await result.current.handlePrint();
      });

      expect(mockTriggerDownload).toHaveBeenCalled();
      expect(result.current.isPrintDialogOpen).toBe(true);
    });

    it("should handle search -> reset workflow", async () => {
      const { result } = renderHook(() => useTerminatedLetters());

      // Search
      await act(async () => {
        await result.current.executeSearch("2024-01-01", "2024-12-31");
      });

      expect(result.current.searchCompleted).toBe(true);
      expect(result.current.reportData).not.toBeNull();

      // Reset
      act(() => {
        result.current.resetSearch();
      });

      expect(result.current.searchCompleted).toBe(false);
      expect(result.current.reportData).toBeNull();
      expect(mockResetPagination).toHaveBeenCalled();
    });
  });
});
