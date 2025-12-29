import { configureStore } from "@reduxjs/toolkit";
import { act, renderHook } from "@testing-library/react";
import React from "react";
import { Provider } from "react-redux";
import { beforeEach, describe, expect, it, vi } from "vitest";
import securityReducer, { type SecurityState } from "../../../../../reduxstore/slices/securitySlice";

// Hoist all mock variables to be accessible in vi.mock() calls
const { mockTriggerReport, mockUseGridPagination } = vi.hoisted(() => ({
  mockTriggerReport: vi.fn(),
  mockUseGridPagination: vi.fn()
}));

// Mock RTK Query hook - this must return [triggerFunction, stateObject]
vi.mock("../../../../../reduxstore/api/AdhocApi", () => ({
  useLazyAdhocBeneficiariesReportQuery: vi.fn(() => [mockTriggerReport, { isFetching: false }])
}));

vi.mock("../../../../../hooks/useGridPagination", () => ({
  useGridPagination: mockUseGridPagination
}));

import usePayBeNext from "../usePayBeNext";

type RootState = {
  security: SecurityState;
};

type MockStoreState = Partial<RootState>;

function createMockStore(preloadedState?: MockStoreState) {
  return configureStore<RootState>({
    reducer: {
      security: securityReducer
    } as const,
    preloadedState: preloadedState as RootState | undefined
  });
}

function renderHookWithProvider<T>(hook: () => T, preloadedState?: MockStoreState) {
  const defaultState: MockStoreState = {
    security: {
      token: "mock-token",
      userGroups: [],
      userRoles: [],
      userPermissions: [],
      username: "test-user",
      performLogout: false,
      appUser: null,
      impersonating: []
    }
  };

  const store = createMockStore(preloadedState || defaultState);
  return renderHook(() => hook(), {
    wrapper: ({ children }: { children: React.ReactNode }) => React.createElement(Provider, { store, children })
  });
}

const mockPaginationObject = {
  pageNumber: 0,
  pageSize: 25,
  sortParams: { sortBy: "psnSuffix", isSortDescending: true },
  handlePageNumberChange: vi.fn(),
  handlePaginationChange: vi.fn(),
  handleSortChange: vi.fn(),
  resetPagination: vi.fn(),
  clearPersistedState: vi.fn()
};

const mockBeneficiaryReportData = {
  response: {
    results: [
      {
        badgeNumber: 123456,
        beneficiaryId: 1,
        fullName: "John Smith",
        endingBalance: 1000.0,
        profitDetails: [
          {
            year: 2024,
            amount: 500.0
          }
        ]
      },
      {
        badgeNumber: 789012,
        beneficiaryId: 2,
        fullName: "Jane Doe",
        endingBalance: 2000.0,
        profitDetails: []
      }
    ],
    total: 2
  },
  totalEndingBalance: 3000.0
};

describe("usePayBeNext", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    // Reset RTK Query hook to return default state
    mockTriggerReport.mockReturnValue({
      unwrap: vi.fn().mockResolvedValue(mockBeneficiaryReportData)
    });
    mockUseGridPagination.mockReturnValue(mockPaginationObject);
  });

  describe("Initial State", () => {
    it("should initialize with default state", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      expect(result.current.searchResults).toBeNull();
      expect(result.current.isSearching).toBe(false);
      expect(result.current.showData).toBe(false);
      expect(result.current.hasResults).toBe(false);
      expect(result.current.totalEndingBalance).toBe(0);
      expect(result.current.totalRecords).toBe(0);
    });

    it("should initialize formData with current year and isAlsoEmployee true", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      expect(result.current.formData.profitYear).toBe(new Date().getFullYear());
      expect(result.current.formData.isAlsoEmployee).toBe(true);
    });

    it("should expose pagination object from useGridPagination", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      expect(result.current.pagination).toBeDefined();
      expect(result.current.pagination.pageNumber).toBe(0);
      expect(result.current.pagination.pageSize).toBe(25);
    });

    it("should expose gridData as empty array initially", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      expect(result.current.gridData).toEqual([]);
    });

    it("should expose expandedRows as empty object initially", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      expect(result.current.expandedRows).toEqual({});
    });
  });

  describe("Authentication", () => {
    it("should not execute search when token is missing", async () => {
      const { result } = renderHookWithProvider(() => usePayBeNext(), {
        security: {
          token: null,
          userGroups: [],
          userRoles: [],
          userPermissions: [],
          username: "",
          performLogout: false,
          appUser: null,
          impersonating: []
        }
      });

      await act(async () => {
        await result.current.executeSearch();
      });

      expect(mockTriggerReport).not.toHaveBeenCalled();
    });

    it("should execute search when token is present", async () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      await act(async () => {
        await result.current.executeSearch();
      });

      expect(mockTriggerReport).toHaveBeenCalled();
    });
  });

  describe("Actions", () => {
    it("should expose executeSearch function", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      expect(typeof result.current.executeSearch).toBe("function");
    });

    it("should expose resetSearch function", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      expect(typeof result.current.resetSearch).toBe("function");
    });

    it("should expose toggleRowExpansion function", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      expect(typeof result.current.toggleRowExpansion).toBe("function");
    });

    it("should call handlePageNumberChange when resetSearch is called", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      act(() => {
        result.current.resetSearch();
      });

      expect(mockPaginationObject.handlePageNumberChange).toHaveBeenCalledWith(0);
    });
  });

  describe("executeSearch", () => {
    it("should trigger report with correct parameters", async () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      await act(async () => {
        await result.current.executeSearch({ profitYear: 2024, isAlsoEmployee: false });
      });

      expect(mockTriggerReport).toHaveBeenCalledWith(
        expect.objectContaining({
          profitYear: 2024,
          isAlsoEmployee: false,
          skip: 0,
          take: 25,
          sortBy: "psnSuffix",
          isSortDescending: true
        })
      );
    });

    it("should reset pagination to page 0 on new search", async () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      await act(async () => {
        await result.current.executeSearch();
      });

      expect(mockPaginationObject.handlePageNumberChange).toHaveBeenCalledWith(0);
    });

    it("should use current formData when no formData parameter provided", async () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());
      const currentYear = new Date().getFullYear();

      await act(async () => {
        await result.current.executeSearch();
      });

      expect(mockTriggerReport).toHaveBeenCalledWith(
        expect.objectContaining({
          profitYear: currentYear,
          isAlsoEmployee: true
        })
      );
    });

    it("should handle search error gracefully", async () => {
      const consoleSpy = vi.spyOn(console, "error").mockImplementation(() => {});
      mockTriggerReport.mockReturnValue({
        unwrap: vi.fn().mockRejectedValue(new Error("Search failed"))
      });

      const { result } = renderHookWithProvider(() => usePayBeNext());

      await act(async () => {
        await result.current.executeSearch();
      });

      expect(consoleSpy).toHaveBeenCalledWith("Search failed:", expect.any(Error));
      consoleSpy.mockRestore();
    });
  });

  describe("Selectors", () => {
    it("should return showData as false when data is null", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      expect(result.current.showData).toBe(false);
    });

    it("should return hasResults as false when no results", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      expect(result.current.hasResults).toBe(false);
    });

    it("should return totalEndingBalance as 0 when no data", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      expect(result.current.totalEndingBalance).toBe(0);
    });

    it("should return totalRecords as 0 when no data", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      expect(result.current.totalRecords).toBe(0);
    });
  });

  describe("toggleRowExpansion", () => {
    it("should toggle row expansion state", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());
      const rowKey = "123456-1";

      // Initially not expanded
      expect(result.current.expandedRows[rowKey]).toBeUndefined();

      // Toggle to expanded
      act(() => {
        result.current.toggleRowExpansion(rowKey);
      });

      expect(result.current.expandedRows[rowKey]).toBe(true);

      // Toggle back to collapsed
      act(() => {
        result.current.toggleRowExpansion(rowKey);
      });

      expect(result.current.expandedRows[rowKey]).toBe(false);
    });

    it("should handle multiple row expansions independently", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());
      const rowKey1 = "123456-1";
      const rowKey2 = "789012-2";

      act(() => {
        result.current.toggleRowExpansion(rowKey1);
      });

      act(() => {
        result.current.toggleRowExpansion(rowKey2);
      });

      expect(result.current.expandedRows[rowKey1]).toBe(true);
      expect(result.current.expandedRows[rowKey2]).toBe(true);

      // Collapse first row
      act(() => {
        result.current.toggleRowExpansion(rowKey1);
      });

      expect(result.current.expandedRows[rowKey1]).toBe(false);
      expect(result.current.expandedRows[rowKey2]).toBe(true);
    });
  });

  describe("isSearching state", () => {
    it("should expose isSearching state from RTK Query isFetching", () => {
      const { result } = renderHookWithProvider(() => usePayBeNext());

      expect(typeof result.current.isSearching).toBe("boolean");
    });
  });

  describe("useGridPagination integration", () => {
    it("should call useGridPagination with correct initial parameters", () => {
      renderHookWithProvider(() => usePayBeNext());

      expect(mockUseGridPagination).toHaveBeenCalledWith(
        expect.objectContaining({
          initialPageSize: 25,
          initialSortBy: "psnSuffix",
          initialSortDescending: true,
          persistenceKey: expect.any(String)
        })
      );
    });

    it("should pass onPaginationChange callback to useGridPagination", () => {
      renderHookWithProvider(() => usePayBeNext());

      expect(mockUseGridPagination).toHaveBeenCalledWith(
        expect.objectContaining({
          onPaginationChange: expect.any(Function)
        })
      );
    });
  });
});
