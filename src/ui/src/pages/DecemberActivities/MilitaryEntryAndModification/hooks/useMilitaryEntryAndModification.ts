import { useCallback, useReducer, useRef } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazySearchProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { useLazyGetMilitaryContributionsQuery } from "reduxstore/api/MilitaryApi";
import { clearMasterInquiryData, setMasterInquiryData } from "reduxstore/slices/inquirySlice";
import { clearMilitaryContributions } from "reduxstore/slices/militarySlice";
import { RootState } from "reduxstore/store";
import { MasterInquiryRequest, MissiveResponse } from "reduxstore/types";
import { MASTER_INQUIRY_MESSAGES } from "../../../../components/MissiveAlerts/MissiveMessages";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { useGridPagination } from "../../../../hooks/useGridPagination";
import { useMissiveAlerts } from "../../../../hooks/useMissiveAlerts";

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
  contributions: {
    data: any | null;
    isLoading: boolean;
    error: string | null;
  };
}

type MilitaryAction =
  | { type: "SEARCH_START"; payload: { params: SearchFormData } }
  | { type: "SEARCH_SUCCESS"; payload: { memberFound: boolean } }
  | { type: "SEARCH_FAILURE"; payload: { error: string } }
  | { type: "SEARCH_RESET" }
  | { type: "CONTRIBUTIONS_FETCH_START" }
  | { type: "CONTRIBUTIONS_FETCH_SUCCESS"; payload: { data: any } }
  | { type: "CONTRIBUTIONS_FETCH_FAILURE"; payload: { error: string } };

const initialState: MilitaryState = {
  search: {
    isSearching: false,
    searchCompleted: false,
    memberFound: false,
    searchParams: null,
    error: null
  },
  contributions: {
    data: null,
    isLoading: false,
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

    case "CONTRIBUTIONS_FETCH_START":
      return {
        ...state,
        contributions: {
          ...state.contributions,
          isLoading: true,
          error: null
        }
      };

    case "CONTRIBUTIONS_FETCH_SUCCESS":
      return {
        ...state,
        contributions: {
          ...state.contributions,
          data: action.payload.data,
          isLoading: false,
          error: null
        }
      };

    case "CONTRIBUTIONS_FETCH_FAILURE":
      return {
        ...state,
        contributions: {
          ...state.contributions,
          isLoading: false,
          error: action.payload.error
        }
      };

    default:
      return state;
  }
};

export const useMilitaryEntryAndModification = () => {
  const [state, dispatch] = useReducer(militaryReducer, initialState);
  const reduxDispatch = useDispatch();
  const [triggerSearch] = useLazySearchProfitMasterInquiryQuery();
  const [fetchContributions] = useLazyGetMilitaryContributionsQuery();
  const { addAlert, clearAlerts } = useMissiveAlerts();
  const { masterInquiryMemberDetails } = useSelector((state: RootState) => state.inquiry);
  const profitYear = useDecemberFlowProfitYear();

  // Use refs to prevent infinite loops in useGridPagination
  const memberDetailsRef = useRef(masterInquiryMemberDetails);
  const profitYearRef = useRef(profitYear);

  // Keep refs updated
  memberDetailsRef.current = masterInquiryMemberDetails;
  profitYearRef.current = profitYear;

  const handleContributionsPaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: any) => {
      const currentMemberDetails = memberDetailsRef.current;
      const currentProfitYear = profitYearRef.current;

      if (!currentMemberDetails) return;

      dispatch({ type: "CONTRIBUTIONS_FETCH_START" });
      fetchContributions({
        badgeNumber: Number(currentMemberDetails.badgeNumber),
        profitYear: currentProfitYear,
        contributionAmount: 0,
        contributionDate: "",
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      })
        .unwrap()
        .then((data) => {
          dispatch({ type: "CONTRIBUTIONS_FETCH_SUCCESS", payload: { data } });
        })
        .catch((error) => {
          dispatch({ type: "CONTRIBUTIONS_FETCH_FAILURE", payload: { error: error?.toString() || "Unknown error" } });
        });
    },
    [fetchContributions]
  );

  const contributionsGridPagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "contributionDate",
    initialSortDescending: false,
    onPaginationChange: handleContributionsPaginationChange
  });

  // Store pagination ref to avoid dependency issues
  const paginationRef = useRef(contributionsGridPagination);
  paginationRef.current = contributionsGridPagination;

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

  const fetchMilitaryContributions = useCallback(() => {
    if (memberDetailsRef.current) {
      handleContributionsPaginationChange(0, 25, { sortBy: "contributionDate", isSortDescending: false });
    }
  }, [handleContributionsPaginationChange]);

  const resetSearch = useCallback(() => {
    dispatch({ type: "SEARCH_RESET" });
    reduxDispatch(clearMasterInquiryData());
    reduxDispatch(clearMilitaryContributions());
    clearAlerts();
    paginationRef.current.resetPagination();
  }, [reduxDispatch, clearAlerts]);

  return {
    isSearching: state.search.isSearching,
    searchCompleted: state.search.searchCompleted,
    memberFound: state.search.memberFound,
    searchError: state.search.error,
    contributionsData: state.contributions.data,
    isLoadingContributions: state.contributions.isLoading,
    contributionsError: state.contributions.error,
    contributionsGridPagination,
    executeSearch,
    fetchMilitaryContributions,
    resetSearch
  };
};

export default useMilitaryEntryAndModification;
