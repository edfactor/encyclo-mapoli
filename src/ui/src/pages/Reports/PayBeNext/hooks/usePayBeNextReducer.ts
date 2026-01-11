import { ISortParams } from "smart-ui-library";
import { adhocBeneficiariesReportResponse } from "../../../../reduxstore/types";

/**
 * Pagination state for the grid
 */
export interface PaginationState {
  pageNumber: number;
  pageSize: number;
  sortParams: ISortParams;
}

/**
 * Search/filter state
 */
export interface SearchState {
  isLoading: boolean;
  hasSearched: boolean;
}

/**
 * Form data for search filters (PascalCase for proper typing)
 */
export interface PayBeNextFormData {
  profitYear: number;
  isAlsoEmployee: boolean;
}

/**
 * Row expansion state for master-detail view
 */
export interface ExpandedRowsState {
  [key: string]: boolean;
}

/**
 * Main state for PayBeNext component
 */
export interface PayBeNextState {
  data: adhocBeneficiariesReportResponse | null;
  pagination: PaginationState;
  search: SearchState;
  formData: PayBeNextFormData;
  expandedRows: ExpandedRowsState;
}

/**
 * Actions for state management
 */
export type PayBeNextAction =
  | { type: "SEARCH_START" }
  | { type: "SEARCH_SUCCESS"; payload: adhocBeneficiariesReportResponse }
  | { type: "SEARCH_ERROR" }
  | { type: "SET_PAGINATION"; payload: Partial<PaginationState> }
  | { type: "SET_FORM_DATA"; payload: Partial<PayBeNextFormData> }
  | { type: "TOGGLE_ROW_EXPANSION"; payload: string }
  | { type: "RESET_PAGINATION" }
  | { type: "RESET_ALL" };

/**
 * Initial state with default values
 */
export const initialState: PayBeNextState = {
  data: null,
  pagination: {
    pageNumber: 0,
    pageSize: 25,
    sortParams: {
      sortBy: "psnSuffix",
      isSortDescending: true
    }
  },
  search: {
    isLoading: false,
    hasSearched: false
  },
  formData: {
    // Note: Initial state uses system time. Component will initialize with fake-time-aware year from hook.
    profitYear: new Date().getFullYear(),
    isAlsoEmployee: true
  },
  expandedRows: {}
};

/**
 * Reducer for PayBeNext state management
 */
export const payBeNextReducer = (state: PayBeNextState, action: PayBeNextAction): PayBeNextState => {
  switch (action.type) {
    case "SEARCH_START":
      return {
        ...state,
        search: {
          ...state.search,
          isLoading: true,
          hasSearched: true
        }
      };

    case "SEARCH_SUCCESS":
      return {
        ...state,
        data: action.payload,
        search: {
          ...state.search,
          isLoading: false
        }
      };

    case "SEARCH_ERROR":
      return {
        ...state,
        search: {
          ...state.search,
          isLoading: false
        }
      };

    case "SET_PAGINATION":
      return {
        ...state,
        pagination: {
          ...state.pagination,
          ...action.payload
        }
      };

    case "SET_FORM_DATA":
      return {
        ...state,
        formData: {
          ...state.formData,
          ...action.payload
        }
      };

    case "TOGGLE_ROW_EXPANSION":
      return {
        ...state,
        expandedRows: {
          ...state.expandedRows,
          [action.payload]: !state.expandedRows[action.payload]
        }
      };

    case "RESET_PAGINATION":
      return {
        ...state,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: {
            sortBy: "psnSuffix",
            isSortDescending: true
          }
        }
      };

    case "RESET_ALL":
      return {
        ...initialState,
        formData: {
          // Note: Component should re-initialize with fake-time-aware year from hook after reset
          profitYear: new Date().getFullYear(),
          isAlsoEmployee: true
        }
      };

    default:
      return state;
  }
};

// Selectors
export const selectShowData = (state: PayBeNextState): boolean => state.data !== null && state.search.hasSearched;

export const selectHasResults = (state: PayBeNextState): boolean =>
  Boolean(state.data?.response?.results && state.data.response.results.length > 0);

export const selectTotalEndingBalance = (state: PayBeNextState): number => state.data?.totalEndingBalance ?? 0;

export const selectTotalRecords = (state: PayBeNextState): number => state.data?.response?.total ?? 0;

export const selectResults = (state: PayBeNextState) => state.data?.response?.results ?? [];
