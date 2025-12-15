import { useCallback, useEffect, useReducer, useState } from "react";
import { useDispatch } from "react-redux";
import { formatNumberWithComma } from "smart-ui-library";
import { FORFEITURES_ADJUSTMENT_MESSAGES } from "../../../../components/MissiveAlerts/MissiveMessages";
import useDecemberFlowProfitYear from "../../../../hooks/useDecemberFlowProfitYear";
import { SortParams, useGridPagination } from "../../../../hooks/useGridPagination";
import { useMissiveAlerts } from "../../../../hooks/useMissiveAlerts";
import { useReadOnlyNavigation } from "../../../../hooks/useReadOnlyNavigation";
import {
  InquiryApi,
  useLazyGetProfitMasterInquiryMemberDetailsQuery,
  useLazyGetProfitMasterInquiryMemberQuery
} from "../../../../reduxstore/api/InquiryApi";
import { useLazyGetForfeitureAdjustmentsQuery } from "../../../../reduxstore/api/AdhocApi";
import { MasterInquiryResponseDto } from "../../../../types/master-inquiry/master-inquiry";
import { EmployeeDetails } from "../../../../types/employee/employee";
import {
  ForfeitureAdjustmentSearchParams,
  forfeituresAdjustmentReducer,
  initialState,
  selectShowEmployeeData,
  selectShowMemberDetails,
  selectShowTransactions,
  TransactionData
} from "./useForfeituresAdjustmentReducer";

interface SaveForfeitureFormData {
  forfeitureAmount: number;
  classAction: boolean;
}

const useForfeituresAdjustment = () => {
  const [state, dispatch] = useReducer(forfeituresAdjustmentReducer, initialState);
  const [memberDetailsRefreshTrigger, setMemberDetailsRefreshTrigger] = useState(0);

  const [triggerSearch, { isFetching: isSearching }] = useLazyGetForfeitureAdjustmentsQuery();
  const [triggerMemberDetails] = useLazyGetProfitMasterInquiryMemberQuery();
  const [triggerTransactionDetails] = useLazyGetProfitMasterInquiryMemberDetailsQuery();

  const reduxDispatch = useDispatch();
  const profitYear = useDecemberFlowProfitYear();
  const isReadOnly = useReadOnlyNavigation();
  const { addAlert, clearAlerts } = useMissiveAlerts();

  // Fetch member details
  const fetchMemberDetails = useCallback(
    async (demographicId: number) => {
      dispatch({ type: "MEMBER_DETAILS_FETCH_START" });

      try {
        const details = await triggerMemberDetails({
          memberType: 1,
          id: demographicId,
          profitYear
        }).unwrap();

        dispatch({ type: "MEMBER_DETAILS_FETCH_SUCCESS", payload: { details } });
      } catch (error) {
        console.error("Error fetching member details:", error);
        dispatch({ type: "MEMBER_DETAILS_FETCH_FAILURE" });
      }
    },
    [triggerMemberDetails, profitYear]
  );

  // Fetch transaction details
  const fetchTransactionDetails = useCallback(
    async (demographicId: number, pageNumber: number, pageSize: number, sortParams: SortParams) => {
      dispatch({ type: "TRANSACTIONS_FETCH_START" });

      try {
        const response = await triggerTransactionDetails({
          memberType: 1,
          id: demographicId,
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }).unwrap();

        // Filter for "Outgoing forfeitures" transactions (profit code 2)
        const filteredResults = response.results.filter((transaction: MasterInquiryResponseDto) => {
          return transaction.profitCodeId === 2;
        });

        console.log(
          `Filtered ${filteredResults.length} forfeit transactions from ${response.results.length} total transactions`
        );

        const transactionData: TransactionData = {
          results: filteredResults,
          total: filteredResults.length
        };

        dispatch({ type: "TRANSACTIONS_FETCH_SUCCESS", payload: { data: transactionData } });
      } catch (error) {
        console.error("Error fetching transaction details:", error);
        dispatch({ type: "TRANSACTIONS_FETCH_FAILURE" });
      }
    },
    [triggerTransactionDetails]
  );

  // Pagination for transaction grid
  const handleTransactionPaginationChange = useCallback(
    (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      if (state.search.employeeData?.demographicId) {
        fetchTransactionDetails(state.search.employeeData.demographicId, pageNumber, pageSize, sortParams);
      }
    },
    [state.search.employeeData?.demographicId, fetchTransactionDetails]
  );

  const transactionPagination = useGridPagination({
    initialPageSize: 50,
    initialSortBy: "profitYear",
    initialSortDescending: true,
    onPaginationChange: handleTransactionPaginationChange
  });

  // Execute search (SSN or Badge)
  const executeSearch = useCallback(
    async (searchParams: ForfeitureAdjustmentSearchParams) => {
      clearAlerts();
      dispatch({ type: "SEARCH_START", payload: { params: searchParams } });

      try {
        // Add onlyNetworkToastErrors flag to suppress validation error toasts
        const searchParamsWithFlag = { ...searchParams, onlyNetworkToastErrors: true };
        const employeeData = await triggerSearch(searchParamsWithFlag).unwrap();

        // Check if employee was found
        if (!employeeData || !employeeData.demographicId) {
          // No employee found - show alert
          dispatch({ type: "SEARCH_SUCCESS", payload: { employeeData: null } });
          addAlert(FORFEITURES_ADJUSTMENT_MESSAGES.MEMBER_NOT_FOUND);
          return;
        }

        dispatch({ type: "SEARCH_SUCCESS", payload: { employeeData } });

        // Auto-fetch member details after successful search
        if (profitYear) {
          fetchMemberDetails(employeeData.demographicId);
        }
      } catch (error: unknown) {
        // Check if it's a 404 error (employee not found)
        if (error && typeof error === "object" && "status" in error && error.status === 404) {
          dispatch({ type: "SEARCH_FAILURE", payload: { error: "Member not found" } });
          addAlert(FORFEITURES_ADJUSTMENT_MESSAGES.MEMBER_NOT_FOUND);
        }
        // Check if it's a 500 error with "Employee not found" message
        else if (error && typeof error === "object" && "status" in error && error.status === 500 && "data" in error) {
          const errorData = error as { data?: { title?: string } };
          if (errorData.data?.title === "Employee not found.") {
            dispatch({ type: "SEARCH_FAILURE", payload: { error: "Employee not found" } });
            addAlert(FORFEITURES_ADJUSTMENT_MESSAGES.MEMBER_NOT_FOUND);
          } else {
            console.error("Forfeitures adjustment employee search error:", error);
            dispatch({ type: "SEARCH_FAILURE", payload: { error: error?.toString() || "Unknown error" } });
          }
        } else {
          console.error("Forfeitures adjustment employee search error:", error);
          dispatch({ type: "SEARCH_FAILURE", payload: { error: error?.toString() || "Unknown error" } });
        }
      }
    },
    [triggerSearch, clearAlerts, addAlert, profitYear, fetchMemberDetails]
  );

  // Handle forfeiture save
  const handleSaveForfeiture = useCallback(
    async (formData: SaveForfeitureFormData) => {
      const employeeData = state.search.employeeData;
      const searchParams = state.search.params;

      if (!employeeData || !searchParams) return;

      const demographicId = employeeData.demographicId;

      dispatch({ type: "CLOSE_ADD_FORFEITURE_MODAL" });

      try {
        // Refresh search to get updated data (with onlyNetworkToastErrors flag)
        const searchParamsWithFlag = { ...searchParams, onlyNetworkToastErrors: true };
        const refreshedData = await triggerSearch(searchParamsWithFlag).unwrap();
        dispatch({ type: "SEARCH_SUCCESS", payload: { employeeData: refreshedData } });

        // Invalidate member details cache to force refresh
        reduxDispatch(InquiryApi.util.invalidateTags(["MemberDetails"]));

        // Trigger refresh of member details component
        setMemberDetailsRefreshTrigger((prev) => prev + 1);

        // Fetch member details for name
        const memberDetails = await triggerMemberDetails({
          memberType: 1,
          id: demographicId,
          profitYear
        }).unwrap();

        const employeeName = memberDetails.fullName || "the selected employee";

        // Show success message
        addAlert({
          id: 0,
          severity: "Success",
          message: "Forfeiture Saved Successfully",
          description: `The forfeiture of amount $${formatNumberWithComma(formData.forfeitureAmount)} for ${employeeName} saved successfully`
        });

        // Refresh transactions
        fetchTransactionDetails(
          demographicId,
          transactionPagination.pageNumber,
          transactionPagination.pageSize,
          transactionPagination.sortParams
        );
      } catch (error) {
        console.error("Error saving forfeiture:", error);
      }
    },
    [
      state.search.employeeData,
      state.search.params,
      triggerSearch,
      triggerMemberDetails,
      profitYear,
      fetchTransactionDetails,
      transactionPagination,
      addAlert,
      reduxDispatch
    ]
  );

  // Auto-fetch transactions when employee data loads
  useEffect(() => {
    if (state.search.employeeData?.demographicId && state.search.hasSearched) {
      fetchTransactionDetails(
        state.search.employeeData.demographicId,
        transactionPagination.pageNumber,
        transactionPagination.pageSize,
        transactionPagination.sortParams
      );
    }
  }, [
    state.search.employeeData?.demographicId,
    state.search.hasSearched,
    fetchTransactionDetails,
    transactionPagination.pageNumber,
    transactionPagination.pageSize,
    transactionPagination.sortParams
  ]);

  // Handle reset
  const handleReset = useCallback(() => {
    dispatch({ type: "RESET_ALL" });
    transactionPagination.resetPagination();
    clearAlerts();
  }, [transactionPagination, clearAlerts]);

  const memberDetails = state.memberDetails.details as unknown as EmployeeDetails | null;
  const currentBalance = memberDetails?.currentPSAmount ?? 0;

  return {
    // State
    searchParams: state.search.params,
    employeeData: state.search.employeeData,
    memberDetails: state.memberDetails.details,
    transactionData: state.transactions.data,
    isSearching: isSearching || state.search.isSearching,
    isFetchingMemberDetails: state.memberDetails.isFetching,
    isFetchingTransactions: state.transactions.isFetching,
    isAddForfeitureModalOpen: state.modal.isAddForfeitureOpen,
    currentBalance,

    // Computed state
    showEmployeeData: selectShowEmployeeData(state),
    showMemberDetails: selectShowMemberDetails(state),
    showTransactions: selectShowTransactions(state),

    // Actions
    executeSearch,
    handleReset,
    handleSaveForfeiture,
    openAddForfeitureModal: () => dispatch({ type: "OPEN_ADD_FORFEITURE_MODAL" }),
    closeAddForfeitureModal: () => dispatch({ type: "CLOSE_ADD_FORFEITURE_MODAL" }),

    // Pagination
    transactionPagination,

    // Other
    profitYear,
    isReadOnly,
    memberDetailsRefreshTrigger
  };
};

export default useForfeituresAdjustment;
