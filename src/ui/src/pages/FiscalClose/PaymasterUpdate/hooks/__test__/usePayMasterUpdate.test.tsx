import { configureStore } from "@reduxjs/toolkit";
import { act, renderHook } from "@testing-library/react";
import { ReactNode } from "react";
import { Provider } from "react-redux";
import { UpdateSummaryResponse, UpdateSummaryEmployee } from "reduxstore/types";
import { beforeEach, describe, expect, it, vi } from "vitest";
import * as useFiscalCloseProfitYear from "../../../../hooks/useFiscalCloseProfitYear";
import * as useGridPagination from "../../../../hooks/useGridPagination";
import * as useLazyGetUpdateSummaryQuery from "../../../../reduxstore/api/YearsEndApi";
import * as useUpdateEnrollmentMutation from "../../../../reduxstore/api/YearsEndApi";
import { messageSlice } from "../../../../reduxstore/slices/messageSlice";
import navigationReducer, { NavigationState } from "../../../../reduxstore/slices/navigationSlice";
import usePayMasterUpdate from "./usePayMasterUpdate";
import { MessagesState } from "../../../../reduxstore/slices/messageSlice";
import type { NavigationDto } from "../../../../types/navigation/navigation";

type MockRootState = {
  navigation: NavigationState;
  message: MessagesState;
};

// Helper to create minimal navigation item for testing
const createMockNavigationItem = (overrides?: Partial<NavigationDto>): NavigationDto => ({
  id: 1,
  parentId: 0,
  title: "Test",
  subTitle: "",
  url: "/test",
  statusId: 1,
  statusName: "Not Started",
  orderNumber: 1,
  icon: "icon",
  requiredRoles: [],
  disabled: false,
  items: [],
  ...overrides
});

// Mock data creation
const createMockPagedData = (items: UpdateSummaryEmployee[] = []) => ({
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

const createMockUpdateSummaryResponse = (
  items: UpdateSummaryEmployee[] = [],
  overrides?: Partial<UpdateSummaryResponse>
): UpdateSummaryResponse => ({
  reportName: "Update Summary",
  reportDate: new Date().toISOString(),
  startDate: new Date().toISOString(),
  endDate: new Date().toISOString(),
  dataSource: "Live",
  response: createMockPagedData(items),
  totalNumberOfEmployees: 0,
  totalNumberOfBeneficiaries: 0,
  totalBeforeProfitSharingAmount: 0,
  totalBeforeVestedAmount: 0,
  totalAfterProfitSharingAmount: 0,
  totalAfterVestedAmount: 0,
  ...overrides
});

// Helper to create RTK Query-like promise with unwrap method
interface RTKQueryPromise<T> extends Promise<{ data: T }> {
  unwrap: () => Promise<T>;
}

const createMockRTKQueryPromise = (data: UpdateSummaryResponse | null = null, error: unknown = null): RTKQueryPromise<UpdateSummaryResponse | null> => {
  const promise = error
    ? Promise.reject(error)
    : Promise.resolve({ data });

  if (error) {
    (promise as RTKQueryPromise<UpdateSummaryResponse | null>).unwrap = () => Promise.reject(error);
  } else {
    (promise as RTKQueryPromise<UpdateSummaryResponse | null>).unwrap = () => Promise.resolve(data);
  }

  return promise as RTKQueryPromise<UpdateSummaryResponse | null>;
};

// Mock hooks
const mockTriggerGetSummary = vi.fn();
const mockUpdateEnrollment = vi.fn();
const mockResetPagination = vi.fn();

// Create store wrapper
const createWrapper = (initialState?: Partial<MockRootState>) => {
  const store = configureStore<MockRootState>({
    reducer: {
      navigation: navigationReducer,
      message: messageSlice
    },
    preloadedState: initialState as Partial<MockRootState>
  });

  return ({ children }: { children: ReactNode }) => <Provider store={store}>{children}</Provider>;
};

describe("usePayMasterUpdate", () => {
  beforeEach(() => {
    vi.clearAllMocks();

    // Clear localStorage
    localStorage.clear();

    // Mock useFiscalCloseProfitYear
    vi.spyOn(useFiscalCloseProfitYear, "default").mockReturnValue(2024);

    // Mock useGridPagination
    vi.spyOn(useGridPagination, "useGridPagination").mockReturnValue({
      pageNumber: 0,
      pageSize: 25,
      sortParams: {
        sortBy: "name",
        isSortDescending: false
      },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: mockResetPagination
    } as ReturnType<typeof useGridPagination.useGridPagination>);

    // Mock lazy query - default to returning empty data
    mockTriggerGetSummary.mockImplementation(() =>
      createMockRTKQueryPromise(createMockUpdateSummaryResponse([]))
    );

    // Mock mutation with unwrap method
    const mockMutationPromise = Promise.resolve({
      data: { success: true }
    });
    (mockMutationPromise as Record<string, unknown>).unwrap = () => Promise.resolve({ success: true });
    mockUpdateEnrollment.mockReturnValue(mockMutationPromise as unknown as ReturnType<typeof useUpdateEnrollmentMutation.useUpdateEnrollmentMutation>[0]);

    vi.spyOn(useLazyGetUpdateSummaryQuery, "useLazyGetUpdateSummaryQuery").mockReturnValue([
      mockTriggerGetSummary,
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
        endpointName: "getUpdateSummary",
        startedTimeStamp: undefined,
        fulfilledTimeStamp: undefined
      },
      { lastArg: undefined, requestStatus: "uninitialized" }
    ] as unknown as ReturnType<typeof useLazyGetUpdateSummaryQuery.useLazyGetUpdateSummaryQuery>);

    vi.spyOn(useUpdateEnrollmentMutation, "useUpdateEnrollmentMutation").mockReturnValue([
      mockUpdateEnrollment,
      {
        isLoading: false,
        isSuccess: false,
        isError: false,
        isUninitialized: true,
        endpointName: "updateEnrollment",
        status: "uninitialized",
        originalArgs: undefined,
        startedTimeStamp: undefined,
        fulfilledTimeStamp: undefined,
        requestId: undefined,
        error: null,
        data: undefined
      }
    ] as unknown as ReturnType<typeof useUpdateEnrollmentMutation.useUpdateEnrollmentMutation>);
  });

  describe("Initial State", () => {
    it("should have correct initial state", () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      expect(result.current.isSearching).toBe(false);
      expect(result.current.searchCompleted).toBe(false);
      expect(result.current.searchError).toBeNull();
      expect(result.current.summaryData).toBeNull();
      expect(result.current.isLoadingSummary).toBe(false);
      expect(result.current.summaryError).toBeNull();
      expect(result.current.currentStatus).toBeNull();
      expect(result.current.isModalOpen).toBe(false);
      expect(result.current.isUpdating).toBe(false);
    });

    it("should have grid pagination initialized", () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      expect(result.current.gridPagination).toBeDefined();
      expect(result.current.gridPagination.pageNumber).toBe(0);
      expect(result.current.gridPagination.pageSize).toBe(25);
    });
  });

  describe("executeSearch", () => {
    it("should execute search with profit year", async () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      await act(async () => {
        const searchResult = await result.current.executeSearch(2024, false);
        expect(searchResult).toBe(true);
      });

      expect(mockTriggerGetSummary).toHaveBeenCalledWith(
        expect.objectContaining({
          profitYear: 2024,
          archive: false
        })
      );
      expect(result.current.searchCompleted).toBe(true);
      expect(result.current.isSearching).toBe(false);
    });

    it("should execute search with archive flag", async () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      await act(async () => {
        await result.current.executeSearch(2024, true);
      });

      expect(mockTriggerGetSummary).toHaveBeenCalledWith(
        expect.objectContaining({
          profitYear: 2024,
          archive: true
        })
      );
    });

    it("should handle search errors", async () => {
      const errorMsg = "Network error";
      const errorResponse = {
        data: { detail: errorMsg }
      };

      mockTriggerGetSummary.mockImplementation(() =>
        createMockRTKQueryPromise(null, errorResponse)
      );

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      await act(async () => {
        const searchResult = await result.current.executeSearch(2024, false);
        expect(searchResult).toBe(false);
      });

      expect(result.current.searchError).toBe(errorMsg);
      expect(result.current.isSearching).toBe(false);
    });

    it("should use default error message when error detail is missing", async () => {
      const errorResponse = { data: {} };

      mockTriggerGetSummary.mockImplementation(() =>
        createMockRTKQueryPromise(null, errorResponse)
      );

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      await act(async () => {
        await result.current.executeSearch(2024, false);
      });

      expect(result.current.searchError).toBe("Failed to search pay master update");
    });

    it("should reset pagination on successful search", async () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      await act(async () => {
        await result.current.executeSearch(2024, false);
      });

      expect(mockResetPagination).toHaveBeenCalled();
    });

    it("should populate summary data on successful search", async () => {
      const mockEmployee: UpdateSummaryEmployee = {
        badgeNumber: 12345,
        storeNumber: 1,
        psnSuffix: 0,
        name: "Employee 1",
        isEmployee: true,
        before: {
          profitSharingAmount: 1000,
          vestedProfitSharingAmount: 800,
          yearsInPlan: 5,
          enrollmentId: 1
        },
        after: {
          profitSharingAmount: 1200,
          vestedProfitSharingAmount: 1000,
          yearsInPlan: 5,
          enrollmentId: 1
        }
      };

      const mockData = createMockUpdateSummaryResponse([mockEmployee]);

      mockTriggerGetSummary.mockImplementation(() =>
        createMockRTKQueryPromise(mockData)
      );

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      await act(async () => {
        await result.current.executeSearch(2024, false);
      });

      expect(result.current.summaryData).toEqual(mockData);
    });
  });

  describe("resetSearch", () => {
    it("should reset state to initial", async () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      // First, perform a search
      await act(async () => {
        await result.current.executeSearch(2024, false);
      });

      expect(result.current.searchCompleted).toBe(true);

      // Then reset
      act(() => {
        result.current.resetSearch();
      });

      expect(result.current.searchCompleted).toBe(false);
      expect(result.current.searchError).toBeNull();
      expect(result.current.summaryData).toBeNull();
    });

    it("should reset pagination on reset", () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      act(() => {
        result.current.resetSearch();
      });

      expect(mockResetPagination).toHaveBeenCalled();
    });

    it("should preserve current status on reset", async () => {
      const initialState: Partial<MockRootState> = {
        navigation: {
          navigationData: {
            navigation: [
              createMockNavigationItem({
                id: 123,
                statusName: "In Progress"
              })
            ]
          },
          error: null,
          currentNavigationId: null
        },
        message: {}
      };

      localStorage.setItem("navigationId", "123");

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper(initialState)
      });

      // Wait for useEffect to set status
      await act(async () => {
        await new Promise((resolve) => setTimeout(resolve, 0));
      });

      // Search
      await act(async () => {
        await result.current.executeSearch(2024, false);
      });

      const statusBeforeReset = result.current.currentStatus;

      // Reset
      act(() => {
        result.current.resetSearch();
      });

      // Status should still be present
      expect(result.current.currentStatus).toBe(statusBeforeReset);
    });
  });

  describe("handleStatusChange", () => {
    it("should update status when not transitioning to Complete", () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      act(() => {
        result.current.handleStatusChange("2", "In Progress");
      });

      expect(result.current.currentStatus).toBe("In Progress");
    });

    it("should trigger archive search when status changes to Complete", async () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      mockTriggerGetSummary.mockClear();

      await act(async () => {
        result.current.handleStatusChange("3", "Complete");
      });

      expect(result.current.currentStatus).toBe("Complete");
      // Note: Archive search should be triggered but requires time for async
      expect(mockTriggerGetSummary).toHaveBeenCalled();
    });

    it("should not trigger archive when already Complete", async () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      // First set to Complete
      act(() => {
        result.current.handleStatusChange("3", "Complete");
      });

      mockTriggerGetSummary.mockClear();

      // Try to set to Complete again
      await act(async () => {
        result.current.handleStatusChange("3", "Complete");
      });

      // Should not trigger another search since already Complete
      expect(mockTriggerGetSummary).not.toHaveBeenCalled();
    });
  });

  describe("Update Enrollment", () => {
    it("should handle update success", async () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      await act(async () => {
        await result.current.handleUpdate();
      });

      expect(mockUpdateEnrollment).toHaveBeenCalledWith({
        ProfitYearRequest: 2024
      });
      expect(result.current.isModalOpen).toBe(false);
    });

    it("should close modal on update success", async () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      act(() => {
        result.current.setIsModalOpen(true);
      });

      expect(result.current.isModalOpen).toBe(true);

      await act(async () => {
        await result.current.handleUpdate();
      });

      expect(result.current.isModalOpen).toBe(false);
    });

    it("should handle update errors", async () => {
      mockUpdateEnrollment.mockRejectedValue(new Error("Update failed"));

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      await act(async () => {
        await result.current.handleUpdate();
      });

      // Modal should still close on error
      expect(result.current.isModalOpen).toBe(false);
    });

    it("should close modal on cancel", () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      act(() => {
        result.current.setIsModalOpen(true);
      });

      expect(result.current.isModalOpen).toBe(true);

      act(() => {
        result.current.handleCancel();
      });

      expect(result.current.isModalOpen).toBe(false);
    });
  });

  describe("Modal State", () => {
    it("should toggle modal open state", () => {
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      expect(result.current.isModalOpen).toBe(false);

      act(() => {
        result.current.setIsModalOpen(true);
      });

      expect(result.current.isModalOpen).toBe(true);

      act(() => {
        result.current.setIsModalOpen(false);
      });

      expect(result.current.isModalOpen).toBe(false);
    });
  });

  describe("Navigation Status Initialization", () => {
    it("should initialize current status from navigation", async () => {
      const initialState: Partial<MockRootState> = {
        navigation: {
          navigationData: {
            navigation: [
              createMockNavigationItem({
                id: 123,
                statusName: "In Progress"
              })
            ]
          },
          error: null,
          currentNavigationId: null
        },
        message: {}
      };

      localStorage.setItem("navigationId", "123");

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper(initialState)
      });

      // Wait for useEffect to initialize
      await act(async () => {
        await new Promise((resolve) => setTimeout(resolve, 0));
      });

      expect(result.current.currentStatus).toBe("In Progress");
    });

    it("should find nested navigation items", async () => {
      const initialState: Partial<MockRootState> = {
        navigation: {
          navigationData: {
            navigation: [
              createMockNavigationItem({
                id: 1,
                statusName: "Parent",
                items: [
                  createMockNavigationItem({
                    id: 123,
                    statusName: "Child Status"
                  })
                ]
              })
            ]
          },
          error: null,
          currentNavigationId: null
        },
        message: {}
      };

      localStorage.setItem("navigationId", "123");

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper(initialState)
      });

      await act(async () => {
        await new Promise((resolve) => setTimeout(resolve, 0));
      });

      expect(result.current.currentStatus).toBe("Child Status");
    });

    it("should handle missing navigation id", async () => {
      localStorage.removeItem("navigationId");

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      await act(async () => {
        await new Promise((resolve) => setTimeout(resolve, 0));
      });

      expect(result.current.currentStatus).toBeNull();
    });

    it("should handle missing navigation data", async () => {
      const initialState: Partial<MockRootState> = {
        navigation: {
          navigationData: {
            navigation: []
          },
          error: null,
          currentNavigationId: null
        },
        message: {}
      };

      localStorage.setItem("navigationId", "999");

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper(initialState)
      });

      await act(async () => {
        await new Promise((resolve) => setTimeout(resolve, 0));
      });

      expect(result.current.currentStatus).toBeNull();
    });
  });

  describe("Complex Scenarios", () => {
    it("should handle search -> status change -> update workflow", async () => {
      // Setup mock to return data with searchCompleted flag
      const mockData = createMockUpdateSummaryResponse([
        {
          id: 1,
          badgeNumber: 12345,
          employeeName: "John Doe",
          participationStatus: "Active"
        }
      ]);

      mockTriggerGetSummary.mockImplementation(() =>
        createMockRTKQueryPromise(mockData)
      );

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      // Search
      await act(async () => {
        const searchResult = await result.current.executeSearch(2024, false);
        expect(searchResult).toBe(true);
      });

      expect(result.current.searchCompleted).toBe(true);
      expect(result.current.summaryData).toBeDefined();

      // Change status
      act(() => {
        result.current.handleStatusChange("2", "In Progress");
      });

      expect(result.current.currentStatus).toBe("In Progress");

      // Open modal and update
      act(() => {
        result.current.setIsModalOpen(true);
      });

      await act(async () => {
        await result.current.handleUpdate();
      });

      expect(result.current.isModalOpen).toBe(false);
    });

    it("should handle search -> reset -> search workflow", async () => {
      // Setup mock to return data
      const mockData = createMockUpdateSummaryResponse([
        {
          id: 1,
          badgeNumber: 12345,
          employeeName: "John Doe",
          participationStatus: "Active"
        }
      ]);

      mockTriggerGetSummary.mockImplementation(() =>
        createMockRTKQueryPromise(mockData)
      );

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      // First search
      await act(async () => {
        const searchResult = await result.current.executeSearch(2024, false);
        expect(searchResult).toBe(true);
      });

      expect(result.current.searchCompleted).toBe(true);

      // Reset
      act(() => {
        result.current.resetSearch();
      });

      expect(result.current.searchCompleted).toBe(false);

      // Second search
      await act(async () => {
        const searchResult = await result.current.executeSearch(2023, false);
        expect(searchResult).toBe(true);
      });

      expect(result.current.searchCompleted).toBe(true);
      expect(mockTriggerGetSummary).toHaveBeenCalledTimes(2);
    });
  });

  describe("Error Handling", () => {
    it("should handle pagination errors", async () => {
      // Create a proper error object that mimics RTK Query's rejection
      const errorResponse = {
        data: { detail: "Pagination failed" }
      };

      mockTriggerGetSummary.mockImplementation(() =>
        createMockRTKQueryPromise(null, errorResponse)
      );

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      await act(async () => {
        const searchResult = await result.current.executeSearch(2024, false);
        expect(searchResult).toBe(false);
      });

      expect(result.current.summaryError).toBe("Pagination failed");
    });

    it("should handle enrollment update with unknown error", async () => {
      mockUpdateEnrollment.mockRejectedValue("Unknown error");

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      await act(async () => {
        result.current.setIsModalOpen(true);
        await result.current.handleUpdate();
      });

      expect(result.current.isModalOpen).toBe(false);
    });
  });
});
