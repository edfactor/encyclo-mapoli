import { ISortParams } from "smart-ui-library";
import { PayBenReportResponse } from "../../../../types";

export interface PaginationState {
  pageNumber: number;
  pageSize: number;
  sortParams: ISortParams;
}

export interface SearchState {
  isLoading: boolean;
  hasSearched: boolean;
}

export interface PayBenReportState {
  data: PayBenReportResponse | null;
  pagination: PaginationState;
  search: SearchState;
}

export type PayBenReportAction =
  | { type: "SEARCH_START" }
  | { type: "SEARCH_SUCCESS"; payload: PayBenReportResponse }
  | { type: "SEARCH_ERROR" }
  | { type: "SET_PAGINATION"; payload: Partial<PaginationState> }
  | { type: "RESET_PAGINATION" }
  | { type: "RESET_ALL" };

export const initialState: PayBenReportState = {
  data: null,
  pagination: {
    pageNumber: 0,
    pageSize: 25,
    sortParams: {
      sortBy: "ssn",
      isSortDescending: true
    }
  },
  search: {
    isLoading: false,
    hasSearched: false
  }
};

export const payBenReportReducer = (state: PayBenReportState, action: PayBenReportAction): PayBenReportState => {
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

    case "RESET_PAGINATION":
      return {
        ...state,
        pagination: {
          pageNumber: 0,
          pageSize: 25,
          sortParams: {
            sortBy: "ssn",
            isSortDescending: true
          }
        }
      };

    case "RESET_ALL":
      return initialState;

    default:
      return state;
  }
};

export const selectShowData = (state: PayBenReportState) => state.data !== null && state.search.hasSearched;

export const selectHasResults = (state: PayBenReportState) => state.data?.results && state.data.results.length > 0;
