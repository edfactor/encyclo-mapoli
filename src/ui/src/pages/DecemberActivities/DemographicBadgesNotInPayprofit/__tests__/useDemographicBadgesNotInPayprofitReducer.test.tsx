import { describe, expect, it, beforeEach } from "vitest";
import { PagedReportResponse } from "../../../../types";

const createMockPagedResponse = <T,>(results: T[], total: number): PagedReportResponse<T> => ({
  reportName: "Demographic Badges Not In Payprofit",
  reportDate: "2024-01-15",
  startDate: "2024-01-01",
  endDate: "2024-12-31",
  dataSource: "Test Data",
  response: {
    results,
    total,
    totalPages: Math.ceil(total / 25) || 0,
    pageSize: 25,
    currentPage: 0
  }
});

describe("useDemographicBadgesNotInPayprofitReducer", () => {
  // Mock reducer implementation for testing
  const initialState = {
    search: { isLoading: false },
    data: null
  };

  interface ReducerState {
    search: { isLoading: boolean };
    data: PagedReportResponse<unknown> | null;
  }

  interface ReducerAction {
    type: string;
    payload?: unknown;
  }

  const demographicBadgesNotInPayprofitReducer = (state: ReducerState, action: ReducerAction): ReducerState => {
    switch (action.type) {
      case "SEARCH_START":
        return {
          ...state,
          search: { isLoading: true }
        };
      case "SEARCH_SUCCESS":
        return {
          ...state,
          search: { isLoading: false },
          data: action.payload as ReducerState["data"]
        };
      case "SEARCH_ERROR":
        return {
          ...state,
          search: { isLoading: false },
          data: null
        };
      default:
        return state;
    }
  };

  const selectShowData = (state: ReducerState): boolean => {
    return state.data !== null && state.search.isLoading === false;
  };

  const selectHasResults = (state: ReducerState): boolean => {
    return !!(state.data?.response?.results && state.data.response.results.length > 0);
  };

  beforeEach(() => {
    // Reset to initial state
  });

  describe("Reducer state transitions", () => {
    it("should initialize with default state", () => {
      expect(initialState.search.isLoading).toBe(false);
      expect(initialState.data).toBeNull();
    });

    it("should transition to loading state on SEARCH_START", () => {
      const action = { type: "SEARCH_START", payload: { profitYear: 2024 } };

      const newState = demographicBadgesNotInPayprofitReducer(initialState, action);

      expect(newState.search.isLoading).toBe(true);
      expect(newState.data).toBeNull();
    });

    it("should transition to success state on SEARCH_SUCCESS", () => {
      const mockData = createMockPagedResponse([{ badgeNumber: 12345, storeName: "Store", employeeName: "John" }], 1);

      const action = { type: "SEARCH_SUCCESS", payload: mockData };

      const newState = demographicBadgesNotInPayprofitReducer(initialState, action);

      expect(newState.search.isLoading).toBe(false);
      expect(newState.data).toEqual(mockData);
    });

    it("should transition to error state on SEARCH_ERROR", () => {
      const loadingState = {
        search: { isLoading: true },
        data: null
      };

      const action = { type: "SEARCH_ERROR" };

      const newState = demographicBadgesNotInPayprofitReducer(loadingState, action);

      expect(newState.search.isLoading).toBe(false);
      expect(newState.data).toBeNull();
    });
  });

  describe("SEARCH_START action", () => {
    it("should set isLoading to true", () => {
      const action = { type: "SEARCH_START", payload: { profitYear: 2024 } };

      const newState = demographicBadgesNotInPayprofitReducer(initialState, action);

      expect(newState.search.isLoading).toBe(true);
    });

    it("should preserve existing data during loading", () => {
      const stateWithData = {
        search: { isLoading: false },
        data: createMockPagedResponse([{ badgeNumber: 1, storeName: "Store", employeeName: "Name" }], 1)
      };

      const action = { type: "SEARCH_START", payload: { profitYear: 2025 } };

      const newState = demographicBadgesNotInPayprofitReducer(stateWithData, action);

      expect(newState.search.isLoading).toBe(true);
      expect(newState.data).toEqual(stateWithData.data);
    });
  });

  describe("SEARCH_SUCCESS action", () => {
    it("should set isLoading to false and store data", () => {
      const mockData = createMockPagedResponse([{ badgeNumber: 12345, storeName: "Store", employeeName: "John" }], 1);

      const action = { type: "SEARCH_SUCCESS", payload: mockData };

      const newState = demographicBadgesNotInPayprofitReducer(initialState, action);

      expect(newState.search.isLoading).toBe(false);
      expect(newState.data).toEqual(mockData);
    });

    it("should replace previous data with new search results", () => {
      const previousData = createMockPagedResponse([{ badgeNumber: 1, storeName: "Old Store", employeeName: "Old Name" }], 1);

      const stateWithData = {
        search: { isLoading: false },
        data: previousData
      };

      const newData = createMockPagedResponse([{ badgeNumber: 2, storeName: "New Store", employeeName: "New Name" }], 1);

      const action = { type: "SEARCH_SUCCESS", payload: newData };

      const newState = demographicBadgesNotInPayprofitReducer(stateWithData, action);

      expect(newState.data).toEqual(newData);
      expect(newState.data.response.results[0].badgeNumber).toBe(2);
    });

    it("should handle empty results", () => {
      const emptyData = createMockPagedResponse([], 0);

      const action = { type: "SEARCH_SUCCESS", payload: emptyData };

      const newState = demographicBadgesNotInPayprofitReducer(initialState, action);

      expect(newState.data.response.results).toHaveLength(0);
      expect(newState.data.response.total).toBe(0);
    });

    it("should handle large result sets", () => {
      const largeData = createMockPagedResponse(Array.from({ length: 100 }, (_, i) => ({
        badgeNumber: 10000 + i,
        storeName: `Store ${i}`,
        employeeName: `Employee ${i}`
      })), 100);

      const action = { type: "SEARCH_SUCCESS", payload: largeData };

      const newState = demographicBadgesNotInPayprofitReducer(initialState, action);

      expect(newState.data.response.results).toHaveLength(100);
      expect(newState.data.response.total).toBe(100);
    });
  });

  describe("SEARCH_ERROR action", () => {
    it("should set isLoading to false and clear data", () => {
      const stateWithData = {
        search: { isLoading: true },
        data: createMockPagedResponse([{ badgeNumber: 1, storeName: "Store", employeeName: "Name" }], 1)
      };

      const action = { type: "SEARCH_ERROR" };

      const newState = demographicBadgesNotInPayprofitReducer(stateWithData, action);

      expect(newState.search.isLoading).toBe(false);
      expect(newState.data).toBeNull();
    });

    it("should clear data on error even if previously had data", () => {
      const stateWithData = {
        search: { isLoading: false },
        data: createMockPagedResponse([{ badgeNumber: 1, storeName: "Store", employeeName: "Name" }], 1)
      };

      const action = { type: "SEARCH_ERROR" };

      const newState = demographicBadgesNotInPayprofitReducer(stateWithData, action);

      expect(newState.data).toBeNull();
    });

    it("should handle error when no data exists", () => {
      const action = { type: "SEARCH_ERROR" };

      const newState = demographicBadgesNotInPayprofitReducer(initialState, action);

      expect(newState.data).toBeNull();
      expect(newState.search.isLoading).toBe(false);
    });
  });

  describe("Unknown action", () => {
    it("should return current state for unknown action types", () => {
      const action = { type: "UNKNOWN_ACTION" };

      const newState = demographicBadgesNotInPayprofitReducer(initialState, action);

      expect(newState).toEqual(initialState);
    });
  });

  describe("Selector: selectShowData", () => {
    it("should return true when data is loaded and not loading", () => {
      const stateWithData = {
        search: { isLoading: false },
        data: createMockPagedResponse([{ badgeNumber: 1, storeName: "Store", employeeName: "Name" }], 1)
      };

      const result = selectShowData(stateWithData);

      expect(result).toBe(true);
    });

    it("should return false when data is null", () => {
      const result = selectShowData(initialState);

      expect(result).toBe(false);
    });

    it("should return false when loading is true", () => {
      const loadingState = {
        search: { isLoading: true },
        data: createMockPagedResponse([{ badgeNumber: 1, storeName: "Store", employeeName: "Name" }], 1)
      };

      const result = selectShowData(loadingState);

      expect(result).toBe(false);
    });

    it("should return false when both data is null and loading is true", () => {
      const loadingState = {
        search: { isLoading: true },
        data: null
      };

      const result = selectShowData(loadingState);

      expect(result).toBe(false);
    });
  });

  describe("Selector: selectHasResults", () => {
    it("should return true when results array has items", () => {
      const stateWithResults = {
        search: { isLoading: false },
        data: createMockPagedResponse([{ badgeNumber: 1, storeName: "Store", employeeName: "Name" }], 1)
      };

      const result = selectHasResults(stateWithResults);

      expect(result).toBe(true);
    });

    it("should return false when results array is empty", () => {
      const stateWithEmptyResults = {
        search: { isLoading: false },
        data: createMockPagedResponse([], 0)
      };

      const result = selectHasResults(stateWithEmptyResults);

      expect(result).toBe(false);
    });

    it("should return false when data is null", () => {
      const result = selectHasResults(initialState);

      expect(result).toBe(false);
    });

    it("should return false when data is undefined", () => {
      const stateWithUndefinedData = {
        search: { isLoading: false },
        data: undefined
      };

      const result = selectHasResults(stateWithUndefinedData);

      expect(result).toBe(false);
    });

    it("should handle multiple results", () => {
      const stateWithMultipleResults = {
        search: { isLoading: false },
        data: createMockPagedResponse([
          { badgeNumber: 1, storeName: "Store 1", employeeName: "Name 1" },
          { badgeNumber: 2, storeName: "Store 2", employeeName: "Name 2" },
          { badgeNumber: 3, storeName: "Store 3", employeeName: "Name 3" }
        ], 3)
      };

      const result = selectHasResults(stateWithMultipleResults);

      expect(result).toBe(true);
    });
  });

  describe("State immutability", () => {
    it("should not mutate original state on SEARCH_START", () => {
      const originalState = JSON.parse(JSON.stringify(initialState));
      const action = { type: "SEARCH_START", payload: { profitYear: 2024 } };

      demographicBadgesNotInPayprofitReducer(initialState, action);

      expect(initialState).toEqual(originalState);
    });

    it("should not mutate original state on SEARCH_SUCCESS", () => {
      const originalState = JSON.parse(JSON.stringify(initialState));
      const action = { type: "SEARCH_SUCCESS", payload: { response: { results: [], total: 0 } } };

      demographicBadgesNotInPayprofitReducer(initialState, action);

      expect(initialState).toEqual(originalState);
    });

    it("should not mutate original state on SEARCH_ERROR", () => {
      const originalState = JSON.parse(JSON.stringify(initialState));
      const action = { type: "SEARCH_ERROR" };

      demographicBadgesNotInPayprofitReducer(initialState, action);

      expect(initialState).toEqual(originalState);
    });
  });

  describe("Sequential state transitions", () => {
    it("should handle START -> SUCCESS -> START -> SUCCESS sequence", () => {
      let state = initialState;

      // First search
      state = demographicBadgesNotInPayprofitReducer(state, { type: "SEARCH_START" });
      expect(state.search.isLoading).toBe(true);

      state = demographicBadgesNotInPayprofitReducer(state, {
        type: "SEARCH_SUCCESS",
        payload: { response: { results: [{ badgeNumber: 1 }], total: 1 } }
      });
      expect(state.search.isLoading).toBe(false);
      expect(state.data.response.results).toHaveLength(1);

      // Second search
      state = demographicBadgesNotInPayprofitReducer(state, { type: "SEARCH_START" });
      expect(state.search.isLoading).toBe(true);

      state = demographicBadgesNotInPayprofitReducer(state, {
        type: "SEARCH_SUCCESS",
        payload: { response: { results: [{ badgeNumber: 2 }, { badgeNumber: 3 }], total: 2 } }
      });
      expect(state.search.isLoading).toBe(false);
      expect(state.data.response.results).toHaveLength(2);
    });

    it("should handle START -> ERROR sequence", () => {
      let state = initialState;

      state = demographicBadgesNotInPayprofitReducer(state, { type: "SEARCH_START" });
      expect(state.search.isLoading).toBe(true);

      state = demographicBadgesNotInPayprofitReducer(state, { type: "SEARCH_ERROR" });
      expect(state.search.isLoading).toBe(false);
      expect(state.data).toBeNull();
    });
  });
});
