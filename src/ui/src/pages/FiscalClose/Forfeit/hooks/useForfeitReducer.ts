import { ISortParams } from "smart-ui-library";
import { ForfeituresAndPointsResponse } from "../../../../types";

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

export interface ArchiveState {
  shouldArchive: boolean;
  isArchiving: boolean;
}

export interface ForfeitState {
  data: ForfeituresAndPointsResponse | null;
  pagination: PaginationState;
  search: SearchState;
  archive: ArchiveState;
}

export type ForfeitAction =
  | { type: "SEARCH_START"; payload: { profitYear: number } }
  | { type: "SEARCH_SUCCESS"; payload: ForfeituresAndPointsResponse }
  | { type: "SEARCH_ERROR" }
  | { type: "TRIGGER_ARCHIVE" }
  | { type: "ARCHIVE_START" }
  | { type: "ARCHIVE_COMPLETE" }
  | { type: "SET_PAGINATION"; payload: Partial<PaginationState> }
  | { type: "RESET_PAGINATION" }
  | { type: "RESET_ALL" };

export const initialState: ForfeitState = {
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
  },
  archive: {
    shouldArchive: false,
    isArchiving: false
  }
};

export const forfeitReducer = (state: ForfeitState, action: ForfeitAction): ForfeitState => {
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

    case "TRIGGER_ARCHIVE":
      return {
        ...state,
        archive: {
          ...state.archive,
          shouldArchive: true
        }
      };

    case "ARCHIVE_START":
      return {
        ...state,
        archive: {
          ...state.archive,
          isArchiving: true
        }
      };

    case "ARCHIVE_COMPLETE":
      return {
        ...state,
        archive: {
          shouldArchive: false,
          isArchiving: false
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

export const selectShowData = (state: ForfeitState) => state.data !== null && state.search.hasSearched;

export const selectHasResults = (state: ForfeitState) =>
  state.data?.response?.results && state.data.response.results.length > 0;
