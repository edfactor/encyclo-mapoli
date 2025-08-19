import { EmployeeDetails, MasterInquiryRequest } from "reduxstore/types";

export interface SelectedMember {
  memberType: number;
  id: number;
  ssn: number;
  badgeNumber: number;
  psnSuffix: number;
}

export interface SearchResponse {
  results: EmployeeDetails[];
  total: number;
}

export interface MemberDetails {
  [key: string]: any;
}

export interface ProfitData {
  results: any[];
  total: number;
}

export type ViewMode = "idle" | "searching" | "multipleMembers" | "memberDetails";

export interface MasterInquiryState {
  search: {
    params: MasterInquiryRequest | null;
    results: SearchResponse | null;
    isSearching: boolean;
    isManuallySearching: boolean;
    noResultsMessage: string | null;
    error: string | null;
  };

  selection: {
    selectedMember: SelectedMember | null;
    memberDetails: MemberDetails | null;
    memberProfitData: ProfitData | null;
    isFetchingMemberDetails: boolean;
    isFetchingProfitData: boolean;
  };

  view: {
    mode: ViewMode;
  };
}

// These are the kinds of state that MasterInquiry can be in
export type MasterInquiryAction =
  | { type: "SEARCH_START"; payload: { params: MasterInquiryRequest; isManual: boolean } }
  | { type: "SEARCH_SUCCESS"; payload: { results: SearchResponse } }
  | { type: "SEARCH_FAILURE"; payload: { error: string } }
  | { type: "SEARCH_RESET" }
  | { type: "SELECT_MEMBER"; payload: { member: SelectedMember | null } }
  | { type: "MEMBER_DETAILS_START" }
  | { type: "MEMBER_DETAILS_SUCCESS"; payload: { details: MemberDetails } }
  | { type: "MEMBER_DETAILS_FAILURE" }
  | { type: "PROFIT_DATA_START" }
  | { type: "PROFIT_DATA_SUCCESS"; payload: { profitData: ProfitData } }
  | { type: "PROFIT_DATA_FAILURE" }
  | { type: "SET_NO_RESULTS_MESSAGE"; payload: { message: string | null } }
  | { type: "SET_VIEW_MODE"; payload: { mode: ViewMode } }
  | { type: "RESET_ALL" };

export const initialState: MasterInquiryState = {
  search: {
    params: null,
    results: null,
    isSearching: false,
    isManuallySearching: false,
    noResultsMessage: null,
    error: null
  },
  selection: {
    selectedMember: null,
    memberDetails: null,
    memberProfitData: null,
    isFetchingMemberDetails: false,
    isFetchingProfitData: false
  },
  view: {
    mode: "idle"
  }
};

export function masterInquiryReducer(state: MasterInquiryState, action: MasterInquiryAction): MasterInquiryState {
  switch (action.type) {
    case "SEARCH_START":
      return {
        ...state,
        search: {
          ...state.search,
          params: action.payload.params,
          isSearching: true,
          isManuallySearching: action.payload.isManual,
          results: null,
          noResultsMessage: null,
          error: null
        },
        selection: {
          ...state.selection,
          selectedMember: null,
          memberDetails: null,
          memberProfitData: null
        },
        view: {
          mode: "searching"
        }
      };

    case "SEARCH_SUCCESS": {
      const hasResults = action.payload.results.results.length > 0;
      const hasSingleResult = action.payload.results.results.length === 1;

      let newViewMode: ViewMode = "idle";
      let selectedMember: SelectedMember | null = null;

      if (hasResults) {
        if (hasSingleResult) {
          newViewMode = "memberDetails";
          const member = action.payload.results.results[0];
          selectedMember = {
            memberType: member.isEmployee ? 1 : 2,
            id: Number(member.id),
            ssn: Number(member.ssn),
            badgeNumber: Number(member.badgeNumber),
            psnSuffix: Number(member.psnSuffix)
          };
        } else {
          newViewMode = "multipleMembers";
        }
      }

      return {
        ...state,
        search: {
          ...state.search,
          results: action.payload.results,
          isSearching: false,
          isManuallySearching: false,
          noResultsMessage: null,
          error: null
        },
        selection: {
          ...state.selection,
          selectedMember
        },
        view: {
          mode: newViewMode
        }
      };
    }

    case "SEARCH_FAILURE":
      return {
        ...state,
        search: {
          ...state.search,
          isSearching: false,
          isManuallySearching: false,
          results: null,
          error: action.payload.error
        },
        view: {
          mode: "idle"
        }
      };

    case "SEARCH_RESET":
      return {
        ...state,
        search: {
          ...initialState.search
        },
        selection: {
          ...initialState.selection
        },
        view: {
          mode: "idle"
        }
      };

    case "SELECT_MEMBER":
      return {
        ...state,
        selection: {
          ...state.selection,
          selectedMember: action.payload.member,
          memberDetails: null,
          memberProfitData: null
        },
        view: {
          mode: action.payload.member ? "memberDetails" : "multipleMembers"
        }
      };

    case "MEMBER_DETAILS_START":
      return {
        ...state,
        selection: {
          ...state.selection,
          isFetchingMemberDetails: true
        }
      };

    case "MEMBER_DETAILS_SUCCESS":
      return {
        ...state,
        selection: {
          ...state.selection,
          memberDetails: action.payload.details,
          isFetchingMemberDetails: false
        }
      };

    case "MEMBER_DETAILS_FAILURE":
      return {
        ...state,
        selection: {
          ...state.selection,
          memberDetails: null,
          isFetchingMemberDetails: false
        }
      };

    case "PROFIT_DATA_START":
      return {
        ...state,
        selection: {
          ...state.selection,
          isFetchingProfitData: true
        }
      };

    case "PROFIT_DATA_SUCCESS":
      return {
        ...state,
        selection: {
          ...state.selection,
          memberProfitData: action.payload.profitData,
          isFetchingProfitData: false
        }
      };

    case "PROFIT_DATA_FAILURE":
      return {
        ...state,
        selection: {
          ...state.selection,
          memberProfitData: null,
          isFetchingProfitData: false
        }
      };

    case "SET_NO_RESULTS_MESSAGE":
      return {
        ...state,
        search: {
          ...state.search,
          noResultsMessage: action.payload.message
        }
      };

    case "SET_VIEW_MODE":
      return {
        ...state,
        view: {
          mode: action.payload.mode
        }
      };

    case "RESET_ALL":
      return initialState;

    default:
      return state;
  }
}

export const selectShowMemberGrid = (state: MasterInquiryState): boolean =>
  state.view.mode === "multipleMembers" && Boolean(state.search.results && state.search.results.results.length > 1);

export const selectShowMemberDetails = (state: MasterInquiryState): boolean =>
  state.view.mode === "memberDetails" && Boolean(state.selection.selectedMember);

export const selectShowProfitDetails = (state: MasterInquiryState): boolean =>
  state.view.mode === "memberDetails" && Boolean(state.selection.selectedMember && state.selection.memberProfitData);
