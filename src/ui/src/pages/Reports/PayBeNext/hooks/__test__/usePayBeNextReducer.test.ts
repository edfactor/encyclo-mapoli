import { describe, expect, it } from "vitest";
import type { adhocBeneficiariesReportResponse, BeneficiaryReportDto } from "@/types/beneficiary/beneficiary";
import {
  initialState,
  payBeNextReducer,
  PayBeNextState,
  selectHasResults,
  selectResults,
  selectShowData,
  selectTotalEndingBalance,
  selectTotalRecords
} from "../usePayBeNextReducer";

// Helper to create a complete mock response with required properties
const createMockResponse = (
  results: BeneficiaryReportDto[] = [],
  total = 0,
  totalEndingBalance = 0
): adhocBeneficiariesReportResponse => ({
  reportName: "Test Report",
  reportDate: "2025-01-01",
  startDate: "2025-01-01",
  endDate: "2025-12-31",
  dataSource: "test",
  response: {
    results,
    total,
    totalPages: Math.ceil(total / 25) || 1,
    pageSize: 25,
    currentPage: 1
  },
  totalEndingBalance
});

// Helper to create a complete mock BeneficiaryReportDto
const createMockBeneficiary = (overrides: Partial<BeneficiaryReportDto> = {}): BeneficiaryReportDto => ({
  beneficiaryId: 1,
  fullName: "Test User",
  ssn: "***-**-1234",
  badgeNumber: 123,
  psnSuffix: 1,
  ...overrides
});

describe("usePayBeNextReducer", () => {
  describe("initialState", () => {
    it("should have correct default values", () => {
      expect(initialState.data).toBeNull();
      expect(initialState.pagination.pageNumber).toBe(0);
      expect(initialState.pagination.pageSize).toBe(25);
      expect(initialState.pagination.sortParams.sortBy).toBe("psnSuffix");
      expect(initialState.pagination.sortParams.isSortDescending).toBe(true);
      expect(initialState.search.isLoading).toBe(false);
      expect(initialState.search.hasSearched).toBe(false);
      expect(initialState.formData.profitYear).toBe(new Date().getFullYear());
      expect(initialState.formData.isAlsoEmployee).toBe(true);
      expect(initialState.expandedRows).toEqual({});
    });
  });

  describe("SEARCH_START action", () => {
    it("should set isLoading to true", () => {
      const result = payBeNextReducer(initialState, { type: "SEARCH_START" });

      expect(result.search.isLoading).toBe(true);
    });

    it("should set hasSearched to true", () => {
      const result = payBeNextReducer(initialState, { type: "SEARCH_START" });

      expect(result.search.hasSearched).toBe(true);
    });

    it("should preserve other state properties", () => {
      const stateWithData: PayBeNextState = {
        ...initialState,
        formData: { profitYear: 2024, isAlsoEmployee: false }
      };

      const result = payBeNextReducer(stateWithData, { type: "SEARCH_START" });

      expect(result.formData.profitYear).toBe(2024);
      expect(result.formData.isAlsoEmployee).toBe(false);
    });
  });

  describe("SEARCH_SUCCESS action", () => {
    const mockPayload = createMockResponse(
      [createMockBeneficiary({ badgeNumber: 123, beneficiaryId: 1, fullName: "Test User" })],
      1,
      5000
    );

    it("should set data to payload", () => {
      const result = payBeNextReducer(initialState, {
        type: "SEARCH_SUCCESS",
        payload: mockPayload
      });

      expect(result.data).toEqual(mockPayload);
    });

    it("should set isLoading to false", () => {
      const loadingState: PayBeNextState = {
        ...initialState,
        search: { isLoading: true, hasSearched: true }
      };

      const result = payBeNextReducer(loadingState, {
        type: "SEARCH_SUCCESS",
        payload: mockPayload
      });

      expect(result.search.isLoading).toBe(false);
    });

    it("should preserve hasSearched as true", () => {
      const loadingState: PayBeNextState = {
        ...initialState,
        search: { isLoading: true, hasSearched: true }
      };

      const result = payBeNextReducer(loadingState, {
        type: "SEARCH_SUCCESS",
        payload: mockPayload
      });

      expect(result.search.hasSearched).toBe(true);
    });
  });

  describe("SEARCH_ERROR action", () => {
    it("should set isLoading to false", () => {
      const loadingState: PayBeNextState = {
        ...initialState,
        search: { isLoading: true, hasSearched: true }
      };

      const result = payBeNextReducer(loadingState, { type: "SEARCH_ERROR" });

      expect(result.search.isLoading).toBe(false);
    });

    it("should preserve data from previous search", () => {
      const stateWithData: PayBeNextState = {
        ...initialState,
        data: createMockResponse([], 0, 0),
        search: { isLoading: true, hasSearched: true }
      };

      const result = payBeNextReducer(stateWithData, { type: "SEARCH_ERROR" });

      expect(result.data).not.toBeNull();
    });
  });

  describe("SET_PAGINATION action", () => {
    it("should update pageNumber", () => {
      const result = payBeNextReducer(initialState, {
        type: "SET_PAGINATION",
        payload: { pageNumber: 5 }
      });

      expect(result.pagination.pageNumber).toBe(5);
    });

    it("should update pageSize", () => {
      const result = payBeNextReducer(initialState, {
        type: "SET_PAGINATION",
        payload: { pageSize: 50 }
      });

      expect(result.pagination.pageSize).toBe(50);
    });

    it("should update sortParams", () => {
      const result = payBeNextReducer(initialState, {
        type: "SET_PAGINATION",
        payload: { sortParams: { sortBy: "fullName", isSortDescending: false } }
      });

      expect(result.pagination.sortParams.sortBy).toBe("fullName");
      expect(result.pagination.sortParams.isSortDescending).toBe(false);
    });

    it("should preserve other pagination values when updating partially", () => {
      const result = payBeNextReducer(initialState, {
        type: "SET_PAGINATION",
        payload: { pageNumber: 3 }
      });

      expect(result.pagination.pageSize).toBe(25);
      expect(result.pagination.sortParams.sortBy).toBe("psnSuffix");
    });
  });

  describe("SET_FORM_DATA action", () => {
    it("should update profitYear", () => {
      const result = payBeNextReducer(initialState, {
        type: "SET_FORM_DATA",
        payload: { profitYear: 2023 }
      });

      expect(result.formData.profitYear).toBe(2023);
    });

    it("should update isAlsoEmployee", () => {
      const result = payBeNextReducer(initialState, {
        type: "SET_FORM_DATA",
        payload: { isAlsoEmployee: false }
      });

      expect(result.formData.isAlsoEmployee).toBe(false);
    });

    it("should preserve other formData values when updating partially", () => {
      const result = payBeNextReducer(initialState, {
        type: "SET_FORM_DATA",
        payload: { profitYear: 2022 }
      });

      expect(result.formData.isAlsoEmployee).toBe(true);
    });
  });

  describe("TOGGLE_ROW_EXPANSION action", () => {
    it("should expand a collapsed row", () => {
      const result = payBeNextReducer(initialState, {
        type: "TOGGLE_ROW_EXPANSION",
        payload: "123-1"
      });

      expect(result.expandedRows["123-1"]).toBe(true);
    });

    it("should collapse an expanded row", () => {
      const expandedState: PayBeNextState = {
        ...initialState,
        expandedRows: { "123-1": true }
      };

      const result = payBeNextReducer(expandedState, {
        type: "TOGGLE_ROW_EXPANSION",
        payload: "123-1"
      });

      expect(result.expandedRows["123-1"]).toBe(false);
    });

    it("should preserve expansion state of other rows", () => {
      const expandedState: PayBeNextState = {
        ...initialState,
        expandedRows: { "123-1": true, "456-2": false }
      };

      const result = payBeNextReducer(expandedState, {
        type: "TOGGLE_ROW_EXPANSION",
        payload: "789-3"
      });

      expect(result.expandedRows["123-1"]).toBe(true);
      expect(result.expandedRows["456-2"]).toBe(false);
      expect(result.expandedRows["789-3"]).toBe(true);
    });
  });

  describe("RESET_PAGINATION action", () => {
    it("should reset pagination to initial values", () => {
      const modifiedState: PayBeNextState = {
        ...initialState,
        pagination: {
          pageNumber: 5,
          pageSize: 100,
          sortParams: { sortBy: "fullName", isSortDescending: false }
        }
      };

      const result = payBeNextReducer(modifiedState, { type: "RESET_PAGINATION" });

      expect(result.pagination.pageNumber).toBe(0);
      expect(result.pagination.pageSize).toBe(25);
      expect(result.pagination.sortParams.sortBy).toBe("psnSuffix");
      expect(result.pagination.sortParams.isSortDescending).toBe(true);
    });

    it("should preserve other state properties", () => {
      const stateWithData: PayBeNextState = {
        ...initialState,
        data: createMockResponse([], 0, 0),
        formData: { profitYear: 2022, isAlsoEmployee: false },
        pagination: { pageNumber: 10, pageSize: 50, sortParams: { sortBy: "test", isSortDescending: false } }
      };

      const result = payBeNextReducer(stateWithData, { type: "RESET_PAGINATION" });

      expect(result.data).not.toBeNull();
      expect(result.formData.profitYear).toBe(2022);
    });
  });

  describe("RESET_ALL action", () => {
    it("should reset all state to initial values", () => {
      const modifiedState: PayBeNextState = {
        data: createMockResponse([], 5, 1000),
        pagination: {
          pageNumber: 10,
          pageSize: 100,
          sortParams: { sortBy: "fullName", isSortDescending: false }
        },
        search: { isLoading: true, hasSearched: true },
        formData: { profitYear: 2020, isAlsoEmployee: false },
        expandedRows: { "123-1": true, "456-2": true }
      };

      const result = payBeNextReducer(modifiedState, { type: "RESET_ALL" });

      expect(result.data).toBeNull();
      expect(result.pagination).toEqual(initialState.pagination);
      expect(result.search).toEqual(initialState.search);
      expect(result.expandedRows).toEqual({});
    });

    it("should reset formData with current year", () => {
      const result = payBeNextReducer(initialState, { type: "RESET_ALL" });

      expect(result.formData.profitYear).toBe(new Date().getFullYear());
      expect(result.formData.isAlsoEmployee).toBe(true);
    });
  });

  describe("default case", () => {
    it("should return current state for unknown action", () => {
      const state = { ...initialState, formData: { profitYear: 2024, isAlsoEmployee: false } };
      // @ts-expect-error testing unknown action type
      const result = payBeNextReducer(state, { type: "UNKNOWN_ACTION" });

      expect(result).toEqual(state);
    });
  });
});

describe("Selectors", () => {
  describe("selectShowData", () => {
    it("should return false when data is null", () => {
      expect(selectShowData(initialState)).toBe(false);
    });

    it("should return false when hasSearched is false", () => {
      const state: PayBeNextState = {
        ...initialState,
        data: createMockResponse([], 0, 0),
        search: { isLoading: false, hasSearched: false }
      };

      expect(selectShowData(state)).toBe(false);
    });

    it("should return true when data exists and hasSearched is true", () => {
      const state: PayBeNextState = {
        ...initialState,
        data: createMockResponse([], 0, 0),
        search: { isLoading: false, hasSearched: true }
      };

      expect(selectShowData(state)).toBe(true);
    });
  });

  describe("selectHasResults", () => {
    it("should return false when data is null", () => {
      expect(selectHasResults(initialState)).toBe(false);
    });

    it("should return false when results array is empty", () => {
      const state: PayBeNextState = {
        ...initialState,
        data: createMockResponse([], 0, 0)
      };

      expect(selectHasResults(state)).toBe(false);
    });

    it("should return true when results array has items", () => {
      const state: PayBeNextState = {
        ...initialState,
        data: createMockResponse(
          [createMockBeneficiary({ badgeNumber: 123, beneficiaryId: 1 })],
          1,
          1000
        )
      };

      expect(selectHasResults(state)).toBe(true);
    });
  });

  describe("selectTotalEndingBalance", () => {
    it("should return 0 when data is null", () => {
      expect(selectTotalEndingBalance(initialState)).toBe(0);
    });

    it("should return totalEndingBalance from data", () => {
      const state: PayBeNextState = {
        ...initialState,
        data: createMockResponse([], 0, 5000.50)
      };

      expect(selectTotalEndingBalance(state)).toBe(5000.50);
    });
  });

  describe("selectTotalRecords", () => {
    it("should return 0 when data is null", () => {
      expect(selectTotalRecords(initialState)).toBe(0);
    });

    it("should return total from response", () => {
      const state: PayBeNextState = {
        ...initialState,
        data: createMockResponse([], 42, 0)
      };

      expect(selectTotalRecords(state)).toBe(42);
    });
  });

  describe("selectResults", () => {
    it("should return empty array when data is null", () => {
      expect(selectResults(initialState)).toEqual([]);
    });

    it("should return results array from data", () => {
      const mockResults = [
        createMockBeneficiary({ badgeNumber: 123, beneficiaryId: 1, fullName: "Test User" })
      ];
      const state: PayBeNextState = {
        ...initialState,
        data: createMockResponse(mockResults, 1, 0)
      };

      expect(selectResults(state)).toEqual(mockResults);
    });
  });
});
