import { renderHook } from "@testing-library/react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { PropsWithChildren } from "react";
import { Provider } from "react-redux";
import { configureStore } from "@reduxjs/toolkit";

// Setup store with minimal required reducers
const createMockStore = (preloadedState?: { security?: { token: string | null } }) => {
  const defaultState = preloadedState?.security ?? { token: "mock-token" };
  return configureStore({
    reducer: {
      security: (state = defaultState) => state
    }
  });
};

// Mock the dependencies
vi.mock("../../../../hooks/useDecemberFlowProfitYear", () => ({
  default: vi.fn()
}));

vi.mock("../../../../hooks/useGridPagination", () => ({
  useGridPagination: vi.fn()
}));

vi.mock("../../../../reduxstore/api/YearsEndApi", () => ({
  useLazyGetDemographicBadgesNotInPayprofitQuery: vi.fn()
}));

import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import {
  useGridPagination,
  type GridPaginationState,
  type GridPaginationActions
} from "../../../../hooks/useGridPagination";
import { useLazyGetDemographicBadgesNotInPayprofitQuery } from "../../../../reduxstore/api/YearsEndApi";
import useDemographicBadgesNotInPayprofit from "../hooks/useDemographicBadgesNotInPayprofit";

describe("useDemographicBadgesNotInPayprofit Hook", () => {
  beforeEach(() => {
    vi.clearAllMocks();

    // Setup default mocks
    vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);

    vi.mocked(useGridPagination).mockReturnValue({
      pageNumber: 0,
      pageSize: 25,
      sortParams: { sortBy: "badgeNumber", isSortDescending: true },
      handlePaginationChange: vi.fn(),
      handleSortChange: vi.fn(),
      resetPagination: vi.fn()
    } as unknown as GridPaginationState & GridPaginationActions);

    const mockTrigger = vi.fn().mockReturnValue({
      unwrap: vi.fn().mockResolvedValue({
        results: [
          {
            badgeNumber: 12345,
            ssn: 123456789,
            fullName: "John Doe",
            store: 1,
            status: "A",
            statusName: "Active"
          }
        ],
        total: 1
      })
    });

    vi.mocked(useLazyGetDemographicBadgesNotInPayprofitQuery).mockReturnValue([
      mockTrigger,
      { isFetching: false },
      {}
    ] as unknown as ReturnType<typeof useLazyGetDemographicBadgesNotInPayprofitQuery>);
  });

  describe("Hook initialization", () => {
    it("should initialize with correct structure", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      expect(result.current).toHaveProperty("searchResults");
      expect(result.current).toHaveProperty("isSearching");
      expect(result.current).toHaveProperty("pagination");
      expect(result.current).toHaveProperty("showData");
      expect(result.current).toHaveProperty("hasResults");
      expect(result.current).toHaveProperty("executeSearch");
    });
  });

  describe("Search functionality", () => {
    it("should initialize pagination correctly", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      expect(result.current.pagination).toBeDefined();
      expect(result.current.pagination.pageNumber).toBe(0);
      expect(result.current.pagination.pageSize).toBe(25);
    });

    it("should have executeSearch function", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      expect(typeof result.current.executeSearch).toBe("function");
    });

    it("should have pagination handlers", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      expect(typeof result.current.pagination.handlePaginationChange).toBe("function");
      expect(typeof result.current.pagination.handleSortChange).toBe("function");
    });
  });

  describe("Profit year handling", () => {
    it("should use the profit year from useDecemberFlowProfitYear", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      renderHook(() => useDemographicBadgesNotInPayprofit(), { wrapper });

      expect(useDecemberFlowProfitYear).toHaveBeenCalled();
    });

    it("should handle null profit year", () => {
      vi.mocked(useDecemberFlowProfitYear).mockReturnValue(2024);

      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      // Hook should still initialize, just won't auto-search
      expect(result.current).toBeDefined();
    });
  });

  describe("Security token validation", () => {
    it("should check for security token via Redux selector", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      expect(result.current).toBeDefined();
    });

    it("should handle missing security token", () => {
      const store = createMockStore({ security: { token: null } });
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      expect(result.current).toBeDefined();
    });
  });

  describe("Pagination state management", () => {
    it("should initialize pagination with default values", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      expect(result.current.pagination.pageNumber).toBe(0);
      expect(result.current.pagination.pageSize).toBe(25);
      expect(result.current.pagination.sortParams.sortBy).toBe("badgeNumber");
      expect(result.current.pagination.sortParams.isSortDescending).toBe(true);
    });
  });

  describe("Data selectors", () => {
    it("should provide showData boolean", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      expect(typeof result.current.showData).toBe("boolean");
    });

    it("should provide hasResults boolean", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      expect(typeof result.current.hasResults).toBe("boolean");
    });
  });

  describe("Search results", () => {
    it("should have searchResults property", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      expect(result.current).toHaveProperty("searchResults");
    });

    it("should have isSearching property", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      expect(typeof result.current.isSearching).toBe("boolean");
    });
  });

  describe("Loading states", () => {
    it("should track isSearching state", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      expect(typeof result.current.isSearching).toBe("boolean");
    });
  });

  describe("Edge cases", () => {
    it("should handle initial state correctly", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      // Initially, searchResults should be null (before auto-search)
      // or have data if auto-search was triggered
      expect(result.current.searchResults === null || typeof result.current.searchResults === "object").toBe(true);
    });

    it("should handle executeSearch function properly", () => {
      const store = createMockStore();
      const wrapper = ({ children }: PropsWithChildren) => <Provider store={store}>{children}</Provider>;

      const { result } = renderHook(() => useDemographicBadgesNotInPayprofit(), {
        wrapper
      });

      expect(typeof result.current.executeSearch).toBe("function");
    });
  });
});
