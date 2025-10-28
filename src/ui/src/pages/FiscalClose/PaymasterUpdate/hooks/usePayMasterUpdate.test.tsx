import { renderHook, act } from "@testing-library/react";
import { describe, it, expect, beforeEach, vi } from "vitest";
import { Provider } from "react-redux";
import { configureStore, PreloadedState } from "@reduxjs/toolkit";
import usePayMasterUpdate from "./usePayMasterUpdate";
import * as useLazyGetUpdateSummaryQuery from "reduxstore/api/YearsEndApi";
import * as useUpdateEnrollmentMutation from "reduxstore/api/YearsEndApi";
import * as useFiscalCloseProfitYear from "../../../../hooks/useFiscalCloseProfitYear";
import * as useGridPagination from "../../../../hooks/useGridPagination";
import navigationReducer from "reduxstore/slices/navigationSlice";
import messageReducer from "reduxstore/slices/messageSlice";
import { UpdateSummaryResponse } from "reduxstore/types";
import { ReactNode } from "react";

// Mock Redux state types
interface MockNavigationState {
  navigationData: {
    navigation: Array<{
      id: number;
      statusName?: string;
      items?: Array<{
        id: number;
        statusName?: string;
        items?: unknown[];
      }>;
    }>;
  };
}

interface MockMessageState {
  messages: Record<string, unknown>;
}

interface MockRootState {
  navigation: MockNavigationState;
  message: MockMessageState;
}

// Mock data creation
const createMockUpdateSummaryResponse = (overrides?: Partial<UpdateSummaryResponse>): UpdateSummaryResponse => ({
  items: [],
  pageNumber: 0,
  pageSize: 25,
  totalCount: 0,
  hasNextPage: false,
  hasPreviousPage: false,
  ...overrides
});

// Mock hooks
const mockTriggerGetSummary = vi.fn();
const mockUpdateEnrollment = vi.fn();
const mockResetPagination = vi.fn();

// Create store wrapper
const createWrapper = (initialState?: PreloadedState<MockRootState>) => {
  const store = configureStore({
    reducer: {
      navigation: navigationReducer,
      message: messageReducer
    },
    preloadedState: initialState as PreloadedState<any>
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
      sortBy: "name",
      isSortDescending: false,
      resetPagination: mockResetPagination,
      goToPage: vi.fn(),
      onPaginationChange: vi.fn()
    } as ReturnType<typeof useGridPagination.useGridPagination>);

    // Mock lazy query
    mockTriggerGetSummary.mockResolvedValue({
      data: createMockUpdateSummaryResponse({
        items: [],
        totalCount: 0
      })
    });

    // Mock mutation
    mockUpdateEnrollment.mockResolvedValue({
      data: { success: true }
    });

    vi.spyOn(useLazyGetUpdateSummaryQuery, "useLazyGetUpdateSummaryQuery").mockReturnValue([
      mockTriggerGetSummary
    ] as ReturnType<typeof useLazyGetUpdateSummaryQuery.useLazyGetUpdateSummaryQuery>);

    vi.spyOn(useUpdateEnrollmentMutation, "useUpdateEnrollmentMutation").mockReturnValue([
      mockUpdateEnrollment,
      { isLoading: false }
    ] as ReturnType<typeof useUpdateEnrollmentMutation.useUpdateEnrollmentMutation>);
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
      mockTriggerGetSummary.mockRejectedValue({
        data: { detail: errorMsg }
      });

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
      mockTriggerGetSummary.mockRejectedValue({ data: {} });

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
      const mockData = createMockUpdateSummaryResponse({
        items: [{ name: "Employee 1", items: [] }] as any[],
        totalCount: 1
      });

      mockTriggerGetSummary.mockResolvedValue({ data: mockData });

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
      const initialState: PreloadedState<MockRootState> = {
        navigation: {
          navigationData: {
            navigation: [
              {
                id: 123,
                statusName: "In Progress",
                items: []
              }
            ]
          }
        },
        message: { messages: {} }
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
      const initialState: PreloadedState<MockRootState> = {
        navigation: {
          navigationData: {
            navigation: [
              {
                id: 123,
                statusName: "In Progress",
                items: []
              }
            ]
          }
        },
        message: { messages: {} }
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
      const initialState: PreloadedState<MockRootState> = {
        navigation: {
          navigationData: {
            navigation: [
              {
                id: 1,
                statusName: "Parent",
                items: [
                  {
                    id: 123,
                    statusName: "Child Status",
                    items: []
                  }
                ]
              }
            ]
          }
        },
        message: { messages: {} }
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
      const initialState: PreloadedState<MockRootState> = {
        navigation: {
          navigationData: {
            navigation: []
          }
        },
        message: { messages: {} }
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
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      // Search
      await act(async () => {
        await result.current.executeSearch(2024, false);
      });

      expect(result.current.searchCompleted).toBe(true);

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
      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      // First search
      await act(async () => {
        await result.current.executeSearch(2024, false);
      });

      expect(result.current.searchCompleted).toBe(true);

      // Reset
      act(() => {
        result.current.resetSearch();
      });

      expect(result.current.searchCompleted).toBe(false);

      // Second search
      await act(async () => {
        await result.current.executeSearch(2023, false);
      });

      expect(result.current.searchCompleted).toBe(true);
      expect(mockTriggerGetSummary).toHaveBeenCalledTimes(2);
    });
  });

  describe("Error Handling", () => {
    it("should handle pagination errors", async () => {
      mockTriggerGetSummary.mockRejectedValue({
        data: { detail: "Pagination failed" }
      });

      const { result } = renderHook(() => usePayMasterUpdate(), {
        wrapper: createWrapper()
      });

      await act(async () => {
        await result.current.executeSearch(2024, false);
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
