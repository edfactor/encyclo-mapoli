import { useCallback, useReducer } from "react";
import { useDispatch } from "react-redux";
import { useLazySearchProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { clearMasterInquiryData, setMasterInquiryData } from "reduxstore/slices/inquirySlice";
import { clearMilitaryContributions } from "reduxstore/slices/militarySlice";
import { MasterInquiryRequest, MissiveResponse } from "reduxstore/types";
import { useMissiveAlerts } from "../../../MasterInquiry/hooks/useMissiveAlerts";
import { MASTER_INQUIRY_MESSAGES } from "../../../MasterInquiry/utils/MasterInquiryMessages";

interface SearchFormData {
  socialSecurity?: string;
  badgeNumber?: string;
}

interface MilitaryState {
  search: {
    isSearching: boolean;
    searchCompleted: boolean;
    memberFound: boolean;
    searchParams: SearchFormData | null;
    error: string | null;
  };
}

type MilitaryAction =
  | { type: "SEARCH_START"; payload: { params: SearchFormData } }
  | { type: "SEARCH_SUCCESS"; payload: { memberFound: boolean } }
  | { type: "SEARCH_FAILURE"; payload: { error: string } }
  | { type: "SEARCH_RESET" };

const initialState: MilitaryState = {
  search: {
    isSearching: false,
    searchCompleted: false,
    memberFound: false,
    searchParams: null,
    error: null
  }
};

const militaryReducer = (state: MilitaryState, action: MilitaryAction): MilitaryState => {
  switch (action.type) {
    case "SEARCH_START":
      return {
        ...state,
        search: {
          ...state.search,
          isSearching: true,
          searchCompleted: false,
          memberFound: false,
          searchParams: action.payload.params,
          error: null
        }
      };

    case "SEARCH_SUCCESS":
      return {
        ...state,
        search: {
          ...state.search,
          isSearching: false,
          searchCompleted: true,
          memberFound: action.payload.memberFound,
          error: null
        }
      };

    case "SEARCH_FAILURE":
      return {
        ...state,
        search: {
          ...state.search,
          isSearching: false,
          searchCompleted: true,
          memberFound: false,
          error: action.payload.error
        }
      };

    case "SEARCH_RESET":
      return initialState;

    default:
      return state;
  }
};

export const useMilitaryEntryAndModification = () => {
  const [state, dispatch] = useReducer(militaryReducer, initialState);
  const reduxDispatch = useDispatch();
  const [triggerSearch] = useLazySearchProfitMasterInquiryQuery();
  const { addAlert, clearAlerts } = useMissiveAlerts();

  const executeSearch = useCallback(
    async (params: SearchFormData, profitYear: number) => {
      try {
        dispatch({ type: "SEARCH_START", payload: { params } });
        clearAlerts();

        const searchParams: MasterInquiryRequest = {
          pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false },
          ...(!!params.socialSecurity && { ssn: Number(params.socialSecurity) }),
          ...(!!params.badgeNumber && { badgeNumber: Number(params.badgeNumber) }),
          profitYear: profitYear
        };

        const response = await triggerSearch(searchParams).unwrap();

        if (response?.results && response.results.length > 0) {
          // Member found
          reduxDispatch(setMasterInquiryData(response.results[0]));
          dispatch({ type: "SEARCH_SUCCESS", payload: { memberFound: true } });
        } else {
          // No member found - show the Member Not Found message
          dispatch({ type: "SEARCH_SUCCESS", payload: { memberFound: false } });
          addAlert(MASTER_INQUIRY_MESSAGES.MEMBER_NOT_FOUND);
        }

        return response?.results && response.results.length > 0;
      } catch (error) {
        console.error("Military search failed:", error);
        dispatch({ type: "SEARCH_FAILURE", payload: { error: error?.toString() || "Unknown error" } });

        addAlert({
          id: 999,
          severity: "Error",
          message: "Search Failed",
          description: "The search request failed. Please try again."
        } as MissiveResponse);

        return false;
      }
    },
    [triggerSearch, reduxDispatch, addAlert, clearAlerts]
  );

  const resetSearch = useCallback(() => {
    dispatch({ type: "SEARCH_RESET" });
    reduxDispatch(clearMasterInquiryData());
    reduxDispatch(clearMilitaryContributions());
    clearAlerts();
  }, [reduxDispatch, clearAlerts]);

  return {
    isSearching: state.search.isSearching,
    searchCompleted: state.search.searchCompleted,
    memberFound: state.search.memberFound,
    searchError: state.search.error,
    executeSearch,
    resetSearch
  };
};

export default useMilitaryEntryAndModification;
