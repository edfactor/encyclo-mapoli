import { ISortParams } from "smart-ui-library";
import { DuplicateSSNDetail, PagedReportResponse } from "../../../../types";

export interface PaginationState {
  pageNumber: number;
  pageSize: number;
  sortParams: ISortParams;
}

export interface SearchState {
  isLoading: boolean;
  profitYear: number | null;
  hasSearched: boolean;
}

export interface DuplicateSSNsOnDemographicsState {
  data: PagedReportResponse<DuplicateSSNDetail> | null;
  pagination: PaginationState;
  search: SearchState;
}

export type DuplicateSSNsOnDemographicsAction =
  | { type: "SEARCH_START"; payload: { profitYear: number } }
  | { type: "SEARCH_SUCCESS"; payload: PagedReportResponse<DuplicateSSNDetail> }
  | { type: "SEARCH_ERROR" }
  | { type: "SET_PAGINATION"; payload: Partial<PaginationState> }
  | { type: "RESET_PAGINATION" }
  | { type: "RESET_ALL" };

export const initialState: DuplicateSSNsOnDemographicsState = {
  data: null,
  pagination: {
    pageNumber: 0,
    pageSize: 25,
    sortParams: {
      sortBy: "badgeNumber",
      isSortDescending: false
    }
  },
  search: {
    isLoading: false,
    profitYear: null,
    hasSearched: false
  }
};

export const duplicateSSNsOnDemographicsReducer = (
  state: DuplicateSSNsOnDemographicsState,
  action: DuplicateSSNsOnDemographicsAction
): DuplicateSSNsOnDemographicsState => {
  switch (action.type) {
    case "SEARCH_START":
      return {
        ...state,
        search: {
          ...state.search,
          isLoading: true,
          profitYear: action.payload.profitYear,
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
            sortBy: "badgeNumber",
            isSortDescending: false
          }
        }
      };

    case "RESET_ALL":
      return initialState;

    default:
      return state;
  }
};

export const selectShowData = (state: DuplicateSSNsOnDemographicsState) =>
  state.data !== null && state.search.hasSearched;

export const selectHasResults = (state: DuplicateSSNsOnDemographicsState) =>
  state.data?.response?.results && state.data.response.results.length > 0;
