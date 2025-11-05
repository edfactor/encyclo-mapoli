import { ISortParams } from "smart-ui-library";
import { SuggestedForfeitResponse } from "@/types/december-activities/forfeitures";

export interface PaginationState {
  pageNumber: number;
  pageSize: number;
  sortParams: ISortParams;
}

export interface SearchState {
  params: ForfeitureAdjustmentSearchParams | null;
  employeeData: SuggestedForfeitResponse | null;
  isSearching: boolean;
  hasSearched: boolean;
  error: string | null;
}

export interface MemberDetailsState {
  details: unknown | null;
  isFetching: boolean;
}

export interface TransactionData {
  results: unknown[];
  total: number;
}

export interface TransactionsState {
  data: TransactionData | null;
  isFetching: boolean;
}

export interface ModalState {
  isAddForfeitureOpen: boolean;
}

export interface ForfeitureAdjustmentSearchParams {
  ssn?: string;
  badge?: string;
  profitYear: number;
  skip: number;
  take: number;
  sortBy: string;
  isSortDescending: boolean;
}

export interface ForfeituresAdjustmentState {
  search: SearchState;
  memberDetails: MemberDetailsState;
  transactions: TransactionsState;
  modal: ModalState;
}

export type ForfeituresAdjustmentAction =
  // Search actions
  | { type: "SEARCH_START"; payload: { params: ForfeitureAdjustmentSearchParams } }
  | { type: "SEARCH_SUCCESS"; payload: { employeeData: SuggestedForfeitResponse | null } }
  | { type: "SEARCH_FAILURE"; payload: { error: string } }
  // Member details actions
  | { type: "MEMBER_DETAILS_FETCH_START" }
  | { type: "MEMBER_DETAILS_FETCH_SUCCESS"; payload: { details: unknown } }
  | { type: "MEMBER_DETAILS_FETCH_FAILURE" }
  // Transaction actions
  | { type: "TRANSACTIONS_FETCH_START" }
  | { type: "TRANSACTIONS_FETCH_SUCCESS"; payload: { data: TransactionData } }
  | { type: "TRANSACTIONS_FETCH_FAILURE" }
  // Modal actions
  | { type: "OPEN_ADD_FORFEITURE_MODAL" }
  | { type: "CLOSE_ADD_FORFEITURE_MODAL" }
  // Utility actions
  | { type: "RESET_ALL" };

export const initialState: ForfeituresAdjustmentState = {
  search: {
    params: null,
    employeeData: null,
    isSearching: false,
    hasSearched: false,
    error: null
  },
  memberDetails: {
    details: null,
    isFetching: false
  },
  transactions: {
    data: null,
    isFetching: false
  },
  modal: {
    isAddForfeitureOpen: false
  }
};

export const forfeituresAdjustmentReducer = (
  state: ForfeituresAdjustmentState,
  action: ForfeituresAdjustmentAction
): ForfeituresAdjustmentState => {
  switch (action.type) {
    case "SEARCH_START":
      return {
        ...state,
        search: {
          ...state.search,
          params: action.payload.params,
          isSearching: true,
          hasSearched: true,
          error: null
        }
      };

    case "SEARCH_SUCCESS":
      return {
        ...state,
        search: {
          ...state.search,
          employeeData: action.payload.employeeData,
          isSearching: false
        }
      };

    case "SEARCH_FAILURE":
      return {
        ...state,
        search: {
          ...state.search,
          isSearching: false,
          error: action.payload.error
        }
      };

    case "MEMBER_DETAILS_FETCH_START":
      return {
        ...state,
        memberDetails: {
          ...state.memberDetails,
          isFetching: true
        }
      };

    case "MEMBER_DETAILS_FETCH_SUCCESS":
      return {
        ...state,
        memberDetails: {
          details: action.payload.details,
          isFetching: false
        }
      };

    case "MEMBER_DETAILS_FETCH_FAILURE":
      return {
        ...state,
        memberDetails: {
          ...state.memberDetails,
          isFetching: false
        }
      };

    case "TRANSACTIONS_FETCH_START":
      return {
        ...state,
        transactions: {
          ...state.transactions,
          isFetching: true
        }
      };

    case "TRANSACTIONS_FETCH_SUCCESS":
      return {
        ...state,
        transactions: {
          data: action.payload.data,
          isFetching: false
        }
      };

    case "TRANSACTIONS_FETCH_FAILURE":
      return {
        ...state,
        transactions: {
          ...state.transactions,
          isFetching: false
        }
      };

    case "OPEN_ADD_FORFEITURE_MODAL":
      return {
        ...state,
        modal: {
          ...state.modal,
          isAddForfeitureOpen: true
        }
      };

    case "CLOSE_ADD_FORFEITURE_MODAL":
      return {
        ...state,
        modal: {
          ...state.modal,
          isAddForfeitureOpen: false
        }
      };

    case "RESET_ALL":
      return initialState;

    default:
      return state;
  }
};

// Selectors
export const selectShowEmployeeData = (state: ForfeituresAdjustmentState): boolean =>
  state.search.employeeData !== null && state.search.hasSearched;

export const selectShowMemberDetails = (state: ForfeituresAdjustmentState): boolean =>
  Boolean(state.search.employeeData && state.memberDetails.details);

export const selectShowTransactions = (state: ForfeituresAdjustmentState): boolean =>
  Boolean(state.search.employeeData && state.transactions.data);
