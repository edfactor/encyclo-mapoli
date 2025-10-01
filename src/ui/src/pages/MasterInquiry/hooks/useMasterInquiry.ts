import { useCallback, useEffect, useMemo, useReducer, useRef } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import {
  useLazyGetProfitMasterInquiryMemberDetailsQuery,
  useLazyGetProfitMasterInquiryMemberQuery,
  useLazySearchProfitMasterInquiryQuery
} from "reduxstore/api/InquiryApi";
import { RootState } from "reduxstore/store";
import { MasterInquiryRequest, MasterInquirySearch, MissiveResponse } from "reduxstore/types";
import { MASTER_INQUIRY_MESSAGES } from "../../../components/MissiveAlerts/MissiveMessages";
import { ROUTES } from "../../../constants";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { useMissiveAlerts } from "../../../hooks/useMissiveAlerts";
import { isSimpleSearch } from "../utils/MasterInquiryFunctions";
import {
  initialState,
  masterInquiryReducer,
  selectShowMemberDetails,
  selectShowMemberGrid,
  selectShowProfitDetails,
  type SelectedMember
} from "./useMasterInquiryReducer";

const useMasterInquiry = () => {
  const [state, dispatch] = useReducer(masterInquiryReducer, initialState);
  const navigate = useNavigate();

  const [triggerSearch, { isLoading: isSearching }] = useLazySearchProfitMasterInquiryQuery();
  const [triggerMemberDetails] = useLazyGetProfitMasterInquiryMemberQuery();
  const [triggerProfitDetails] = useLazyGetProfitMasterInquiryMemberDetailsQuery();

  const { masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);
  const missives: MissiveResponse[] = useSelector((state: RootState) => state.lookups.missives);

  const { addAlert, addAlerts, clearAlerts } = useMissiveAlerts();

  const searchParamsRef = useRef(state.search.params);
  const selectedMemberRef = useRef(state.selection.selectedMember);

  useEffect(() => {
    searchParamsRef.current = state.search.params;
  }, [state.search.params]);

  useEffect(() => {
    selectedMemberRef.current = state.selection.selectedMember;
  }, [state.selection.selectedMember]);

  const handleMemberGridPaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: any) => {
      const currentSearchParams = searchParamsRef.current;
      if (currentSearchParams) {
        dispatch({ type: "MEMBERS_FETCH_START" });
        triggerSearch({
          ...currentSearchParams,
          pagination: {
            skip: pageNumber * pageSize,
            take: pageSize,
            sortBy: sortParams.sortBy,
            isSortDescending: sortParams.isSortDescending
          }
        })
          .unwrap()
          .then((response) => {
            const results = Array.isArray(response) ? response : response.results;
            const total = Array.isArray(response) ? response.length : response.total;
            dispatch({ type: "MEMBERS_FETCH_SUCCESS", payload: { results: { results, total } } });
          })
          .catch((error) => {
            dispatch({ type: "MEMBERS_FETCH_FAILURE", payload: { error: error?.toString() || "Unknown error" } });
          });
      }
    },
    [triggerSearch]
  );

  const handleProfitGridPaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: any) => {
      const currentSelectedMember = selectedMemberRef.current;
      if (currentSelectedMember?.memberType && currentSelectedMember?.id) {
        triggerProfitDetails({
          memberType: currentSelectedMember.memberType,
          id: currentSelectedMember.id,
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        })
          .unwrap()
          .then((profitData) => {
            dispatch({ type: "PROFIT_DATA_FETCH_SUCCESS", payload: { profitData } });
          });
      }
    },
    [triggerProfitDetails]
  );

  const memberGridPagination = useGridPagination({
    initialPageSize: 5,
    initialSortBy: "badgeNumber",
    initialSortDescending: true,
    onPaginationChange: handleMemberGridPaginationChange
  });

  const profitGridPagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "profitYear",
    initialSortDescending: true,
    onPaginationChange: handleProfitGridPaginationChange
  });

  const executeSearch = useCallback(
    async (params: MasterInquiryRequest) => {
      try {
        dispatch({ type: "SEARCH_START", payload: { params, isManual: true } });
        clearAlerts();

        const [response] = await Promise.all([
          triggerSearch(params).unwrap(),
          new Promise((resolve) => setTimeout(resolve, 300)) // Minimum 300ms loading state to trigger spinner just a bit
        ]);

        if (
          response &&
          (Array.isArray(response) ? response.length > 0 : response.results && response.results.length > 0)
        ) {
          const results = Array.isArray(response) ? response : response.results;
          const total = Array.isArray(response) ? response.length : response.total;

          dispatch({ type: "SEARCH_SUCCESS", payload: { results: { results, total } } });
        } else {
          dispatch({ type: "SEARCH_SUCCESS", payload: { results: { results: [], total: 0 } } });

          // Add appropriate missive alert based on current search parameters
          // Convert API params back to form-ish structure for isSimpleSearch check
          const searchFormData: MasterInquirySearch = {
            endProfitYear: params.endProfitYear,
            startProfitMonth: params.startProfitMonth,
            endProfitMonth: params.endProfitMonth,
            socialSecurity: params.ssn,
            name: params.name,
            badgeNumber: params.badgeNumber,
            paymentType: "all",
            memberType: "all",
            contribution: params.contributionAmount,
            earnings: params.earningsAmount,
            forfeiture: params.forfeitureAmount,
            payment: params.paymentAmount,
            voids: false,
            pagination: params.pagination
          };

          const isSimple = isSimpleSearch(searchFormData);
          const isBeneficiarySearch = masterInquiryRequestParams?.memberType === "beneficiaries";

          let alertMessage;
          if (isSimple && isBeneficiarySearch) {
            alertMessage = MASTER_INQUIRY_MESSAGES.BENEFICIARY_NOT_FOUND;
          } else if (isSimple) {
            alertMessage = MASTER_INQUIRY_MESSAGES.MEMBER_NOT_FOUND;
          } else {
            alertMessage = MASTER_INQUIRY_MESSAGES.NO_RESULTS_FOUND;
          }

          addAlert(alertMessage);
        }
      } catch (error) {
        console.error("Search failed:", error);
        dispatch({ type: "SEARCH_FAILURE", payload: { error: error?.toString() || "Unknown error" } });

        // Add error alert
        addAlert({
          id: 999,
          severity: "Error",
          message: "Search Failed",
          description: "The search request failed. Please try again."
        } as MissiveResponse);
      }
    },
    [triggerSearch, masterInquiryRequestParams, clearAlerts]
  );

  const selectMember = useCallback((member: SelectedMember | null) => {
    dispatch({ type: "SELECT_MEMBER", payload: { member } });
  }, []);

  // Fetch member details when selected member changes
  useEffect(() => {
    if (state.selection.selectedMember?.memberType && state.selection.selectedMember?.id) {
      dispatch({ type: "MEMBER_DETAILS_FETCH_START" });
      triggerMemberDetails({
        memberType: state.selection.selectedMember.memberType,
        id: state.selection.selectedMember.id,
        profitYear: state.search.params?.endProfitYear
      })
        .unwrap()
        .then((details) => {
          dispatch({ type: "MEMBER_DETAILS_FETCH_SUCCESS", payload: { details } });

          // We cannot cross references missives unless we have some in the redux store
          if (details.missives && Array.isArray(missives) && missives.length > 0) {
            const localMissives: MissiveResponse[] = details.missives
              .map((id: number) => missives.find((m: MissiveResponse) => m.id === id))
              .filter(Boolean) as MissiveResponse[];

            if (localMissives.length > 0) {
              addAlerts(localMissives);
            }
          }

          if (!details.isEmployee && masterInquiryRequestParams?.memberType === "all") {
            addAlert(MASTER_INQUIRY_MESSAGES.BENEFICIARY_FOUND(details.ssn));
          }
        })
        .catch(() => {
          dispatch({ type: "MEMBER_DETAILS_FETCH_FAILURE" });
        });
    }
  }, [
    state.selection.selectedMember,
    state.search.params?.endProfitYear,
    triggerMemberDetails,
    missives,
    masterInquiryRequestParams?.memberType,
    addAlert,
    addAlerts
  ]);

  const profitFetchDeps = useMemo(
    () => ({
      memberType: state.selection.selectedMember?.memberType,
      id: state.selection.selectedMember?.id,
      pageNumber: profitGridPagination.pageNumber,
      pageSize: profitGridPagination.pageSize,
      sortBy: profitGridPagination.sortParams.sortBy,
      isSortDescending: profitGridPagination.sortParams.isSortDescending
    }),
    [
      state.selection.selectedMember?.memberType,
      state.selection.selectedMember?.id,
      profitGridPagination.pageNumber,
      profitGridPagination.pageSize,
      profitGridPagination.sortParams.sortBy,
      profitGridPagination.sortParams.isSortDescending
    ]
  );

  useEffect(() => {
    if (profitFetchDeps.memberType && profitFetchDeps.id) {
      dispatch({ type: "PROFIT_DATA_FETCH_START" });
      triggerProfitDetails({
        memberType: profitFetchDeps.memberType,
        id: profitFetchDeps.id,
        skip: profitFetchDeps.pageNumber * profitFetchDeps.pageSize,
        take: profitFetchDeps.pageSize,
        sortBy: profitFetchDeps.sortBy,
        isSortDescending: profitFetchDeps.isSortDescending
      })
        .unwrap()
        .then((profitData) => {
          dispatch({ type: "PROFIT_DATA_FETCH_SUCCESS", payload: { profitData } });
        })
        .catch(() => {
          dispatch({ type: "PROFIT_DATA_FETCH_FAILURE" });
        });
    }
  }, [profitFetchDeps, triggerProfitDetails]);

  const clearSearch = useCallback(() => {
    dispatch({ type: "SEARCH_RESET" });
    clearAlerts();
    // Navigate to base route to clear any badge number URL parameters
    navigate(`/${ROUTES.MASTER_INQUIRY}`, { replace: true });
  }, [clearAlerts, navigate]);

  const resetAll = useCallback(() => {
    dispatch({ type: "RESET_ALL" });
    memberGridPagination.resetPagination();
    profitGridPagination.resetPagination();
    clearSearch();
  }, [memberGridPagination.resetPagination, profitGridPagination.resetPagination, clearSearch]);

  const showMemberGrid = selectShowMemberGrid(state);
  const showMemberDetails = selectShowMemberDetails(state);
  const showProfitDetails = selectShowProfitDetails(state);

  return {
    searchParams: state.search.params,
    searchResults: state.search.results,
    isSearching: isSearching || state.search.isSearching || state.search.isManuallySearching,
    isFetchingMembers: state.search.isFetchingMembers,
    selectedMember: state.selection.selectedMember,
    memberDetails: state.selection.memberDetails,
    memberProfitData: state.selection.memberProfitData,
    isFetchingMemberDetails: state.selection.isFetchingMemberDetails,
    isFetchingProfitData: state.selection.isFetchingProfitData,
    showMemberGrid,
    showMemberDetails,
    showProfitDetails,
    noResultsMessage: state.search.noResultsMessage,
    memberGridPagination,
    profitGridPagination,
    executeSearch,
    selectMember,
    clearSearch,
    resetAll
  };
};

export default useMasterInquiry;
