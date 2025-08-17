import { useState, useCallback, useEffect, useRef, useMemo } from "react";
import { useSelector } from "react-redux";
import {
  useLazySearchProfitMasterInquiryQuery,
  useLazyGetProfitMasterInquiryMemberQuery,
  useLazyGetProfitMasterInquiryMemberDetailsQuery
} from "reduxstore/api/InquiryApi";
import { RootState } from "reduxstore/store";
import { MasterInquiryRequest, EmployeeDetails, MissiveResponse, MasterInquirySearch } from "reduxstore/types";
import { isSimpleSearch } from "./MasterInquiryFunctions";
import { MASTER_INQUIRY_MESSAGES } from "./MasterInquiryMessages";
import { useMissiveAlerts } from "./useMissiveAlerts";
import { useGridPagination } from "./useGridPagination";

interface SelectedMember {
  memberType: number;
  id: number;
  ssn: number;
  badgeNumber: number;
  psnSuffix: number;
}

interface SearchResponse {
  results: EmployeeDetails[];
  total: number;
}

interface MemberDetails {
  [key: string]: any;
}

interface ProfitData {
  results: any[];
  total: number;
}

const useMasterInquiry = () => {
  const [searchParams, setSearchParams] = useState<MasterInquiryRequest | null>(null);
  const [searchResults, setSearchResults] = useState<SearchResponse | null>(null);
  const [selectedMember, setSelectedMember] = useState<SelectedMember | null>(null);
  const [memberDetails, setMemberDetails] = useState<MemberDetails | null>(null);
  const [memberProfitData, setMemberProfitData] = useState<ProfitData | null>(null);
  const [noResultsMessage, setNoResultsMessage] = useState<string | null>(null);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [isManuallySearching, setIsManuallySearching] = useState(false);

  const [triggerSearch, { isLoading: isSearching, error: searchError }] = useLazySearchProfitMasterInquiryQuery();
  const [triggerMemberDetails, { isFetching: isFetchingMemberDetails }] = useLazyGetProfitMasterInquiryMemberQuery();
  const [triggerProfitDetails, { isFetching: isFetchingProfitData }] =
    useLazyGetProfitMasterInquiryMemberDetailsQuery();

  const { masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);
  const missives = useSelector((state: RootState) => state.lookups.missives);

  const { addAlert, addAlerts, clearAlerts } = useMissiveAlerts();

  const searchParamsRef = useRef(searchParams);
  const selectedMemberRef = useRef(selectedMember);

  useEffect(() => {
    searchParamsRef.current = searchParams;
  }, [searchParams]);

  useEffect(() => {
    selectedMemberRef.current = selectedMember;
  }, [selectedMember]);

  const handleMemberGridPaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: any) => {
      const currentSearchParams = searchParamsRef.current;
      if (currentSearchParams) {
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
            setSearchResults({ results, total });
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
            setMemberProfitData(profitData);
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
        setIsManuallySearching(true);
        clearAlerts();
        setSearchParams(params);
        setSearchResults(null);
        setSelectedMember(null);
        setMemberDetails(null);
        setMemberProfitData(null);
        setNoResultsMessage(null);
        setInitialSearchLoaded(false);

        const [response] = await Promise.all([
          triggerSearch(params).unwrap(),
          new Promise((resolve) => setTimeout(resolve, 300)) // Minimum 300ms loading state
        ]);

        if (
          response &&
          (Array.isArray(response) ? response.length > 0 : response.results && response.results.length > 0)
        ) {
          const results = Array.isArray(response) ? response : response.results;
          const total = Array.isArray(response) ? response.length : response.total;

          setSearchResults({ results, total });
          setInitialSearchLoaded(true);

          if (results.length === 1) {
            const member = results[0];
            const selectedMemberData: SelectedMember = {
              memberType: member.isEmployee ? 1 : 2,
              id: Number(member.id),
              ssn: Number(member.ssn),
              badgeNumber: Number(member.badgeNumber),
              psnSuffix: Number(member.psnSuffix)
            };

            // Only set selected member if it's different from current one
            setSelectedMember((prev) => {
              if (prev?.id === selectedMemberData.id && prev?.memberType === selectedMemberData.memberType) {
                return prev; // No change needed
              }
              return selectedMemberData;
            });
          }
        } else {
          setSearchResults(null);
          setInitialSearchLoaded(false);
          setNoResultsMessage(null);

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

          const alertMessage = isSimpleSearch(searchFormData)
            ? MASTER_INQUIRY_MESSAGES.MEMBER_NOT_FOUND
            : MASTER_INQUIRY_MESSAGES.NO_RESULTS_FOUND;
          addAlert(alertMessage);
        }
      } catch (error) {
        console.error("Search failed:", error);
        setSearchResults(null);
        setInitialSearchLoaded(false);
        setNoResultsMessage(null);

        // Add error alert
        addAlert({
          id: 999,
          severity: "Error",
          message: "Search Failed",
          description: "The search request failed. Please try again."
        } as MissiveResponse);
      } finally {
        setIsManuallySearching(false);
      }
    },
    [triggerSearch, masterInquiryRequestParams, clearAlerts]
  );

  const selectMember = useCallback((member: SelectedMember | null) => {
    setSelectedMember(member);
    setMemberDetails(null);
    setMemberProfitData(null);

    if (!member) {
      setNoResultsMessage(null);
      return;
    }
  }, []);

  // Fetch memb details when selected member changes
  useEffect(() => {
    if (selectedMember?.memberType && selectedMember?.id) {
      triggerMemberDetails({
        memberType: selectedMember.memberType,
        id: selectedMember.id,
        profitYear: searchParams?.endProfitYear
      })
        .unwrap()
        .then((details) => {
          setMemberDetails(details);

          if (details.missives && missives) {
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
        });
    }
  }, [
    selectedMember,
    searchParams?.endProfitYear,
    triggerMemberDetails,
    missives,
    masterInquiryRequestParams?.memberType,
    addAlert,
    addAlerts
  ]);

  const profitFetchDeps = useMemo(
    () => ({
      memberType: selectedMember?.memberType,
      id: selectedMember?.id,
      pageNumber: profitGridPagination.pageNumber,
      pageSize: profitGridPagination.pageSize,
      sortBy: profitGridPagination.sortParams.sortBy,
      isSortDescending: profitGridPagination.sortParams.isSortDescending
    }),
    [
      selectedMember?.memberType,
      selectedMember?.id,
      profitGridPagination.pageNumber,
      profitGridPagination.pageSize,
      profitGridPagination.sortParams.sortBy,
      profitGridPagination.sortParams.isSortDescending
    ]
  );

  useEffect(() => {
    if (profitFetchDeps.memberType && profitFetchDeps.id) {
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
          setMemberProfitData(profitData);
        });
    }
  }, [profitFetchDeps, triggerProfitDetails]);

  const clearSearch = useCallback(() => {
    setSearchParams(null);
    setSearchResults(null);
    setSelectedMember(null);
    setMemberDetails(null);
    setMemberProfitData(null);
    setNoResultsMessage(null);
    setInitialSearchLoaded(false);
    setIsManuallySearching(false);
    clearAlerts();
  }, [clearAlerts]);

  const clearSelection = useCallback(() => {
    setSelectedMember(null);
    setMemberDetails(null);
    setMemberProfitData(null);
  }, []);

  const resetAll = useCallback(() => {
    clearSearch();
    memberGridPagination.resetPagination();
    profitGridPagination.resetPagination();
  }, [clearSearch, memberGridPagination.resetPagination, profitGridPagination.resetPagination]);

  const showMemberGrid = Boolean(searchResults && searchResults.results.length > 1);
  const showMemberDetails = Boolean(selectedMember && memberDetails);
  const showProfitDetails = Boolean(selectedMember && memberProfitData);

  return {
    searchParams,
    searchResults,
    isSearching: isSearching || isManuallySearching,
    searchError: searchError?.toString() || null,
    selectedMember,
    memberDetails,
    memberProfitData,
    isFetchingMemberDetails,
    isFetchingProfitData,
    showMemberGrid,
    showMemberDetails,
    showProfitDetails,
    noResultsMessage,
    initialSearchLoaded,
    memberGridPagination,
    profitGridPagination,
    executeSearch,
    clearSearch,
    selectMember,
    clearSelection,
    resetAll
  };
};

export default useMasterInquiry;
