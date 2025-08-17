import { useState, useCallback, useEffect } from "react";
import { useSelector } from "react-redux";
import {
  useLazySearchProfitMasterInquiryQuery,
  useLazyGetProfitMasterInquiryMemberQuery,
  useLazyGetProfitMasterInquiryMemberDetailsQuery
} from "reduxstore/api/InquiryApi";
import { RootState } from "reduxstore/store";
import { MasterInquiryRequest, EmployeeDetails, MissiveResponse } from "reduxstore/types";
import { isSimpleSearch } from "./MasterInquiryFunctions";
import { MASTER_INQUIRY_MESSAGES } from "./MasterInquiryMessages";
import { useMissiveAlerts } from "./useMissiveAlerts";

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

interface MasterInquiryState {
  // Search state
  searchParams: MasterInquiryRequest | null;
  searchResults: SearchResponse | null;
  isSearching: boolean;
  searchError: string | null;

  // Selected member state
  selectedMember: SelectedMember | null;
  memberDetails: MemberDetails | null;
  memberProfitData: ProfitData | null;
  isFetchingMemberDetails: boolean;
  isFetchingProfitData: boolean;

  // UI state
  showMemberGrid: boolean;
  showMemberDetails: boolean;
  showProfitDetails: boolean;
  noResultsMessage: string | null;
  initialSearchLoaded: boolean;

  // Pagination state
  memberGridPagination: {
    pageNumber: number;
    pageSize: number;
    sortParams: any;
  };
  profitGridPagination: {
    pageNumber: number;
    pageSize: number;
    sortParams: any;
  };
}

interface MasterInquiryActions {
  executeSearch: (params: MasterInquiryRequest) => Promise<void>;
  clearSearch: () => void;
  selectMember: (member: SelectedMember | null) => void;
  clearSelection: () => void;
  resetAll: () => void;
  updateMemberGridPagination: (pageNumber: number, pageSize: number, sortParams: any) => void;
  updateProfitGridPagination: (pageNumber: number, pageSize: number, sortParams: any) => void;
}

const useMasterInquiry = (): MasterInquiryState & MasterInquiryActions => {
  const [searchParams, setSearchParams] = useState<MasterInquiryRequest | null>(null);
  const [searchResults, setSearchResults] = useState<SearchResponse | null>(null);
  const [selectedMember, setSelectedMember] = useState<SelectedMember | null>(null);
  const [memberDetails, setMemberDetails] = useState<MemberDetails | null>(null);
  const [memberProfitData, setMemberProfitData] = useState<ProfitData | null>(null);
  const [noResultsMessage, setNoResultsMessage] = useState<string | null>(null);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [isManuallySearching, setIsManuallySearching] = useState(false);

  const [memberGridPagination, setMemberGridPagination] = useState({
    pageNumber: 0,
    pageSize: 5,
    sortParams: { sortBy: "badgeNumber", isSortDescending: true }
  });

  const [profitGridPagination, setProfitGridPagination] = useState({
    pageNumber: 0,
    pageSize: 25,
    sortParams: { sortBy: "profitYear", isSortDescending: true }
  });

  const [triggerSearch, { isLoading: isSearching, error: searchError }] = useLazySearchProfitMasterInquiryQuery();
  const [triggerMemberDetails, { isFetching: isFetchingMemberDetails }] = useLazyGetProfitMasterInquiryMemberQuery();
  const [triggerProfitDetails, { isFetching: isFetchingProfitData }] =
    useLazyGetProfitMasterInquiryMemberDetailsQuery();

  const { masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);
  const missives = useSelector((state: RootState) => state.lookups.missives);

  const { addAlert, addAlerts, clearAlerts } = useMissiveAlerts();

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

        const response = await triggerSearch(params).unwrap();

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
            setSelectedMember(selectedMemberData);
          }
        } else {
          setSearchResults(null);
          setInitialSearchLoaded(false);
          setNoResultsMessage(
            isSimpleSearch(masterInquiryRequestParams)
              ? MASTER_INQUIRY_MESSAGES.MEMBER_NOT_FOUND.message
              : MASTER_INQUIRY_MESSAGES.NO_RESULTS_FOUND.message
          );
        }
      } catch (error) {
        console.error("Search failed:", error);
        setSearchResults(null);
        setInitialSearchLoaded(false);
        setNoResultsMessage("Search failed. Please try again.");
      } finally {
        // Always clear loading state when search completes
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

  useEffect(() => {
    if (selectedMember?.memberType && selectedMember?.id) {
      const { pageNumber, pageSize, sortParams } = profitGridPagination;

      triggerProfitDetails({
        memberType: selectedMember.memberType,
        id: selectedMember.id,
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
  }, [selectedMember, profitGridPagination, triggerProfitDetails]);

  const updateMemberGridPagination = useCallback(
    (pageNumber: number, pageSize: number, sortParams: any) => {
      setMemberGridPagination({ pageNumber, pageSize, sortParams });

      if (searchParams) {
        triggerSearch({
          ...searchParams,
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
    [searchParams, triggerSearch]
  );

  const updateProfitGridPagination = useCallback(
    (pageNumber: number, pageSize: number, sortParams: any) => {
      setProfitGridPagination({ pageNumber, pageSize, sortParams });

      if (selectedMember?.memberType && selectedMember?.id) {
        triggerProfitDetails({
          memberType: selectedMember.memberType,
          id: selectedMember.id,
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
    [selectedMember, triggerProfitDetails]
  );

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
    setMemberGridPagination({
      pageNumber: 0,
      pageSize: 5,
      sortParams: { sortBy: "badgeNumber", isSortDescending: true }
    });
    setProfitGridPagination({
      pageNumber: 0,
      pageSize: 25,
      sortParams: { sortBy: "profitYear", isSortDescending: true }
    });
  }, [clearSearch]);

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
    resetAll,
    updateMemberGridPagination,
    updateProfitGridPagination
  };
};

export default useMasterInquiry;
