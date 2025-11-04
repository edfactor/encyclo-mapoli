import { describe, expect, it } from "vitest";
import {
  eligibleEmployeesReducer,
  initialState,
  selectHasResults,
  selectShowData,
  type EligibleEmployeesState
} from "../useEligibleEmployeesReducer";

describe("useEligibleEmployeesReducer", () => {
  describe("initialState", () => {
    it("should have correct initial state", () => {
      expect(initialState.data).toBeNull();
      expect(initialState.pagination.pageNumber).toBe(0);
      expect(initialState.pagination.pageSize).toBe(25);
      expect(initialState.pagination.sortParams.sortBy).toBe("badgeNumber");
      expect(initialState.pagination.sortParams.isSortDescending).toBe(false);
      expect(initialState.search.isLoading).toBe(false);
      expect(initialState.search.profitYear).toBeNull();
      expect(initialState.search.hasSearched).toBe(false);
    });
  });

  describe("SEARCH_START", () => {
    it("should set loading state and profit year", () => {
      const action = { type: "SEARCH_START" as const, payload: { profitYear: 2024 } };
      const newState = eligibleEmployeesReducer(initialState, action);

      expect(newState.search.isLoading).toBe(true);
      expect(newState.search.profitYear).toBe(2024);
      expect(newState.search.hasSearched).toBe(true);
    });

    it("should preserve pagination state", () => {
      const stateWithPagination: EligibleEmployeesState = {
        ...initialState,
        pagination: {
          pageNumber: 5,
          pageSize: 50,
          sortParams: { sortBy: "name", isSortDescending: true }
        }
      };

      const action = { type: "SEARCH_START" as const, payload: { profitYear: 2024 } };
      const newState = eligibleEmployeesReducer(stateWithPagination, action);

      expect(newState.pagination).toEqual(stateWithPagination.pagination);
    });
  });

  describe("SEARCH_SUCCESS", () => {
    it("should set data and stop loading", () => {
      const searchingState: EligibleEmployeesState = {
        ...initialState,
        search: {
          isLoading: true,
          profitYear: 2024,
          hasSearched: true
        }
      };

      const mockData = {
        reportName: "Eligible Employees",
        reportDate: "2024-01-01",
        startDate: "2024-01-01",
        endDate: "2024-12-31",
        dataSource: "test",
        response: {
          results: [{ badgeNumber: 12345, fullName: "John Doe" }],
          total: 1
        }
      };

      const action = { type: "SEARCH_SUCCESS" as const, payload: mockData };
      const newState = eligibleEmployeesReducer(searchingState, action);

      expect(newState.data).toEqual(mockData);
      expect(newState.search.isLoading).toBe(false);
      expect(newState.search.profitYear).toBe(2024);
      expect(newState.search.hasSearched).toBe(true);
    });

    it("should handle empty results", () => {
      const searchingState: EligibleEmployeesState = {
        ...initialState,
        search: {
          isLoading: true,
          profitYear: 2024,
          hasSearched: true
        }
      };

      const mockData = {
        reportName: "Eligible Employees",
        reportDate: "2024-01-01",
        startDate: "2024-01-01",
        endDate: "2024-12-31",
        dataSource: "test",
        response: {
          results: [],
          total: 0
        }
      };

      const action = { type: "SEARCH_SUCCESS" as const, payload: mockData };
      const newState = eligibleEmployeesReducer(searchingState, action);

      expect(newState.data).toEqual(mockData);
      expect(newState.search.isLoading).toBe(false);
    });
  });

  describe("SEARCH_ERROR", () => {
    it("should stop loading on error", () => {
      const searchingState: EligibleEmployeesState = {
        ...initialState,
        search: {
          isLoading: true,
          profitYear: 2024,
          hasSearched: true
        }
      };

      const action = { type: "SEARCH_ERROR" as const };
      const newState = eligibleEmployeesReducer(searchingState, action);

      expect(newState.search.isLoading).toBe(false);
      expect(newState.search.profitYear).toBe(2024);
      expect(newState.search.hasSearched).toBe(true);
    });

    it("should preserve data on error", () => {
      const stateWithData: EligibleEmployeesState = {
        ...initialState,
        data: {
          reportName: "Test",
          reportDate: "2024-01-01",
          startDate: "2024-01-01",
          endDate: "2024-12-31",
          dataSource: "test",
          response: { results: [], total: 0 }
        },
        search: {
          isLoading: true,
          profitYear: 2024,
          hasSearched: true
        }
      };

      const action = { type: "SEARCH_ERROR" as const };
      const newState = eligibleEmployeesReducer(stateWithData, action);

      expect(newState.data).toEqual(stateWithData.data);
    });
  });

  describe("SET_PAGINATION", () => {
    it("should update page number", () => {
      const action = { type: "SET_PAGINATION" as const, payload: { pageNumber: 3 } };
      const newState = eligibleEmployeesReducer(initialState, action);

      expect(newState.pagination.pageNumber).toBe(3);
      expect(newState.pagination.pageSize).toBe(25);
    });

    it("should update page size", () => {
      const action = { type: "SET_PAGINATION" as const, payload: { pageSize: 50 } };
      const newState = eligibleEmployeesReducer(initialState, action);

      expect(newState.pagination.pageSize).toBe(50);
      expect(newState.pagination.pageNumber).toBe(0);
    });

    it("should update sort params", () => {
      const action = {
        type: "SET_PAGINATION" as const,
        payload: {
          sortParams: { sortBy: "fullName", isSortDescending: true }
        }
      };
      const newState = eligibleEmployeesReducer(initialState, action);

      expect(newState.pagination.sortParams.sortBy).toBe("fullName");
      expect(newState.pagination.sortParams.isSortDescending).toBe(true);
    });

    it("should update multiple pagination fields at once", () => {
      const action = {
        type: "SET_PAGINATION" as const,
        payload: {
          pageNumber: 5,
          pageSize: 100,
          sortParams: { sortBy: "name", isSortDescending: false }
        }
      };
      const newState = eligibleEmployeesReducer(initialState, action);

      expect(newState.pagination.pageNumber).toBe(5);
      expect(newState.pagination.pageSize).toBe(100);
      expect(newState.pagination.sortParams).toEqual({
        sortBy: "name",
        isSortDescending: false
      });
    });
  });

  describe("RESET_PAGINATION", () => {
    it("should reset pagination to defaults", () => {
      const stateWithPagination: EligibleEmployeesState = {
        ...initialState,
        pagination: {
          pageNumber: 10,
          pageSize: 100,
          sortParams: { sortBy: "name", isSortDescending: true }
        }
      };

      const action = { type: "RESET_PAGINATION" as const };
      const newState = eligibleEmployeesReducer(stateWithPagination, action);

      expect(newState.pagination).toEqual({
        pageNumber: 0,
        pageSize: 25,
        sortParams: {
          sortBy: "badgeNumber",
          isSortDescending: false
        }
      });
    });

    it("should preserve data and search state", () => {
      const stateWithData: EligibleEmployeesState = {
        ...initialState,
        data: {
          reportName: "Test",
          reportDate: "2024-01-01",
          startDate: "2024-01-01",
          endDate: "2024-12-31",
          dataSource: "test",
          response: { results: [], total: 0 }
        },
        search: {
          isLoading: false,
          profitYear: 2024,
          hasSearched: true
        },
        pagination: {
          pageNumber: 5,
          pageSize: 50,
          sortParams: { sortBy: "name", isSortDescending: true }
        }
      };

      const action = { type: "RESET_PAGINATION" as const };
      const newState = eligibleEmployeesReducer(stateWithData, action);

      expect(newState.data).toEqual(stateWithData.data);
      expect(newState.search).toEqual(stateWithData.search);
    });
  });

  describe("RESET_ALL", () => {
    it("should reset to initial state", () => {
      const stateWithData: EligibleEmployeesState = {
        data: {
          reportName: "Test",
          reportDate: "2024-01-01",
          startDate: "2024-01-01",
          endDate: "2024-12-31",
          dataSource: "test",
          response: {
            results: [{ badgeNumber: 12345, fullName: "John Doe" }],
            total: 1
          }
        },
        pagination: {
          pageNumber: 5,
          pageSize: 100,
          sortParams: { sortBy: "name", isSortDescending: true }
        },
        search: {
          isLoading: false,
          profitYear: 2024,
          hasSearched: true
        }
      };

      const action = { type: "RESET_ALL" as const };
      const newState = eligibleEmployeesReducer(stateWithData, action);

      expect(newState).toEqual(initialState);
    });
  });

  describe("selectors", () => {
    describe("selectShowData", () => {
      it("should return true when data exists and has searched", () => {
        const state: EligibleEmployeesState = {
          ...initialState,
          data: {
            reportName: "Test",
            reportDate: "2024-01-01",
            startDate: "2024-01-01",
            endDate: "2024-12-31",
            dataSource: "test",
            response: { results: [], total: 0 }
          },
          search: { ...initialState.search, hasSearched: true }
        };

        expect(selectShowData(state)).toBe(true);
      });

      it("should return false when data is null", () => {
        const state: EligibleEmployeesState = {
          ...initialState,
          search: { ...initialState.search, hasSearched: true }
        };

        expect(selectShowData(state)).toBe(false);
      });

      it("should return false when has not searched", () => {
        const state: EligibleEmployeesState = {
          ...initialState,
          data: {
            reportName: "Test",
            reportDate: "2024-01-01",
            startDate: "2024-01-01",
            endDate: "2024-12-31",
            dataSource: "test",
            response: { results: [], total: 0 }
          }
        };

        expect(selectShowData(state)).toBe(false);
      });
    });

    describe("selectHasResults", () => {
      it("should return true when results exist", () => {
        const state: EligibleEmployeesState = {
          ...initialState,
          data: {
            reportName: "Test",
            reportDate: "2024-01-01",
            startDate: "2024-01-01",
            endDate: "2024-12-31",
            dataSource: "test",
            response: {
              results: [{ badgeNumber: 12345, fullName: "John Doe" }],
              total: 1
            }
          }
        };

        expect(selectHasResults(state)).toBeTruthy();
      });

      it("should return falsy when results are empty", () => {
        const state: EligibleEmployeesState = {
          ...initialState,
          data: {
            reportName: "Test",
            reportDate: "2024-01-01",
            startDate: "2024-01-01",
            endDate: "2024-12-31",
            dataSource: "test",
            response: {
              results: [],
              total: 0
            }
          }
        };

        expect(selectHasResults(state)).toBeFalsy();
      });

      it("should return falsy when data is null", () => {
        expect(selectHasResults(initialState)).toBeFalsy();
      });
    });
  });
});
