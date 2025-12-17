import { useCallback, useReducer, useRef } from "react";
import { useDispatch } from "react-redux";
import { numberToCurrency } from "smart-ui-library";
import useFiscalCloseProfitYear from "../../../../hooks/useFiscalCloseProfitYear";
import { useMissiveAlerts } from "../../../../hooks/useMissiveAlerts";
import {
  useLazyGetProfitMasterInquiryFilteredDetailsQuery,
  useLazySearchProfitMasterInquiryQuery
} from "../../../../reduxstore/api/InquiryApi";
import { useReverseProfitDetailsMutation } from "../../../../reduxstore/api/AdjustmentsApi";
import { removeMessage, setMessage } from "../../../../reduxstore/slices/messageSlice";
import { EmployeeDetails, MasterInquiryResponseDto } from "../../../../reduxstore/types";
import { ServiceErrorResponse } from "../../../../types/errors/errors";
import { ReversalsSearchParams } from "../ReversalsSearchFilter";
import { ProfitDetailRow } from "../ReversalsGrid";
import { ReversalItem } from "../ReversalConfirmationModal";

// Constants
const REVERSALS_MESSAGE_KEY = "ReversalOperation";

// State Types
interface ReversalsState {
  search: {
    params: ReversalsSearchParams | null;
    isSearching: boolean;
  };
  member: {
    selectedMember: { memberType: number; id: number } | null;
    memberDetails: EmployeeDetails | null;
    isFetchingDetails: boolean;
  };
  profitData: {
    results: ProfitDetailRow[];
    total: number;
  } | null;
  isFetchingProfitData: boolean;
  pagination: {
    pageNumber: number;
    pageSize: number;
  };
  confirmation: {
    isOpen: boolean;
    selectedItems: ReversalItem[];
    currentIndex: number;
    confirmedIds: number[];
    isProcessing: boolean;
  };
}

// Action Types
type ReversalsAction =
  | { type: "SEARCH_START"; payload: ReversalsSearchParams }
  | { type: "SEARCH_SUCCESS"; payload: { member: { memberType: number; id: number } } }
  | { type: "SEARCH_FAILURE" }
  | { type: "MEMBER_DETAILS_START" }
  | { type: "MEMBER_DETAILS_SUCCESS"; payload: EmployeeDetails }
  | { type: "MEMBER_DETAILS_FAILURE" }
  | { type: "PROFIT_DATA_START" }
  | { type: "PROFIT_DATA_SUCCESS"; payload: { results: ProfitDetailRow[]; total: number } }
  | { type: "PROFIT_DATA_FAILURE" }
  | { type: "SET_PAGINATION"; payload: { pageNumber: number; pageSize: number } }
  | { type: "OPEN_CONFIRMATION"; payload: ReversalItem[] }
  | { type: "ADVANCE_CONFIRMATION" }
  | { type: "CLOSE_CONFIRMATION" }
  | { type: "SET_PROCESSING"; payload: boolean }
  | { type: "RESET_ALL" };

const initialState: ReversalsState = {
  search: {
    params: null,
    isSearching: false
  },
  member: {
    selectedMember: null,
    memberDetails: null,
    isFetchingDetails: false
  },
  profitData: null,
  isFetchingProfitData: false,
  pagination: {
    pageNumber: 0,
    pageSize: 25
  },
  confirmation: {
    isOpen: false,
    selectedItems: [],
    currentIndex: 0,
    confirmedIds: [],
    isProcessing: false
  }
};

function reversalsReducer(state: ReversalsState, action: ReversalsAction): ReversalsState {
  switch (action.type) {
    case "SEARCH_START":
      return {
        ...state,
        search: { params: action.payload, isSearching: true },
        member: { selectedMember: null, memberDetails: null, isFetchingDetails: false },
        profitData: null
      };
    case "SEARCH_SUCCESS":
      return {
        ...state,
        search: { ...state.search, isSearching: false },
        member: { ...state.member, selectedMember: action.payload.member }
      };
    case "SEARCH_FAILURE":
      return {
        ...state,
        search: { ...state.search, isSearching: false }
      };
    case "MEMBER_DETAILS_START":
      return {
        ...state,
        member: { ...state.member, isFetchingDetails: true }
      };
    case "MEMBER_DETAILS_SUCCESS":
      return {
        ...state,
        member: { ...state.member, memberDetails: action.payload, isFetchingDetails: false }
      };
    case "MEMBER_DETAILS_FAILURE":
      return {
        ...state,
        member: { ...state.member, isFetchingDetails: false }
      };
    case "PROFIT_DATA_START":
      return {
        ...state,
        isFetchingProfitData: true
      };
    case "PROFIT_DATA_SUCCESS":
      return {
        ...state,
        profitData: action.payload,
        isFetchingProfitData: false
      };
    case "PROFIT_DATA_FAILURE":
      return {
        ...state,
        isFetchingProfitData: false
      };
    case "SET_PAGINATION":
      return {
        ...state,
        pagination: action.payload
      };
    case "OPEN_CONFIRMATION":
      return {
        ...state,
        confirmation: {
          isOpen: true,
          selectedItems: action.payload,
          currentIndex: 0,
          confirmedIds: [],
          isProcessing: false
        }
      };
    case "ADVANCE_CONFIRMATION":
      return {
        ...state,
        confirmation: {
          ...state.confirmation,
          currentIndex: state.confirmation.currentIndex + 1,
          confirmedIds: [
            ...state.confirmation.confirmedIds,
            state.confirmation.selectedItems[state.confirmation.currentIndex]?.id
          ]
        }
      };
    case "CLOSE_CONFIRMATION":
      return {
        ...state,
        confirmation: {
          isOpen: false,
          selectedItems: [],
          currentIndex: 0,
          confirmedIds: [],
          isProcessing: false
        }
      };
    case "SET_PROCESSING":
      return {
        ...state,
        confirmation: { ...state.confirmation, isProcessing: action.payload }
      };
    case "RESET_ALL":
      return initialState;
    default:
      return state;
  }
}

export function useReversals() {
  const [state, dispatch] = useReducer(reversalsReducer, initialState);
  const reduxDispatch = useDispatch();
  const profitYear = useFiscalCloseProfitYear();
  const { addAlert, clearAlerts } = useMissiveAlerts();

  // RTK Query hooks
  const [triggerSearch] = useLazySearchProfitMasterInquiryQuery();
  const [triggerProfitDetails] = useLazyGetProfitMasterInquiryFilteredDetailsQuery();
  const [reverseProfitDetails] = useReverseProfitDetailsMutation();

  // Refs for tracking
  const lastSearchRef = useRef<string | null>(null);

  // Clear messages helper
  const clearMessages = useCallback(() => {
    reduxDispatch(removeMessage(REVERSALS_MESSAGE_KEY));
  }, [reduxDispatch]);

  // Execute search
  const executeSearch = useCallback(
    async (params: ReversalsSearchParams) => {
      const searchKey = JSON.stringify(params);
      if (lastSearchRef.current === searchKey && state.search.isSearching) {
        return;
      }
      lastSearchRef.current = searchKey;

      clearAlerts();
      clearMessages();
      dispatch({ type: "SEARCH_START", payload: params });

      try {
        // Build search request
        const memberTypeMap: Record<string, number> = { all: 0, employees: 1, beneficiaries: 2 };
        const searchRequest = {
          ssn: params.socialSecurity ? Number(params.socialSecurity) : undefined,
          badgeNumber: params.badgeNumber ?? undefined,
          memberType: memberTypeMap[params.memberType] ?? 0,
          endProfitYear: profitYear,
          pagination: { skip: 0, take: 1, sortBy: "badgeNumber", isSortDescending: true }
        };

        const result = await triggerSearch(searchRequest).unwrap();
        const members = result.results;

        if (!members || members.length === 0) {
          dispatch({ type: "SEARCH_FAILURE" });
          addAlert({
            id: 404,
            severity: "warning",
            message: "Member Not Found",
            description: "No member found with the provided identifier."
          });
          return;
        }

        // Select the first member
        const member = members[0];
        const memberType = member.isEmployee ? 1 : 2;
        const memberId = Number(member.id);

        dispatch({ type: "SEARCH_SUCCESS", payload: { member: { memberType, id: memberId } } });

        // Note: Member details are fetched by StandaloneMemberDetails component

        // Fetch profit details
        dispatch({ type: "PROFIT_DATA_START" });
        try {
          const profitResult = await triggerProfitDetails({
            memberType,
            id: memberId,
            skip: 0,
            take: state.pagination.pageSize,
            sortBy: "profitYear",
            isSortDescending: true
          }).unwrap();

          const transformedResults: ProfitDetailRow[] = profitResult.results.map((item: MasterInquiryResponseDto) => ({
            id: item.id,
            profitYear: item.profitYear,
            profitYearIteration: item.profitYearIteration,
            profitCodeId: item.profitCodeId,
            profitCodeName: item.profitCodeName ?? "",
            contribution: typeof item.contribution === "number" ? item.contribution : 0,
            earnings: item.earnings,
            forfeiture: item.forfeiture,
            payment: item.payment ?? 0,
            monthToDate: item.monthToDate,
            yearToDate: item.yearToDate,
            currentHoursYear: item.currentHoursYear,
            currentIncomeYear: item.currentIncomeYear,
            federalTaxes: item.federalTaxes,
            stateTaxes: item.stateTaxes,
            taxCode: (item.taxCode as string) ?? "",
            commentTypeName: item.commentTypeName ?? "",
            commentRelatedCheckNumber: item.commentRelatedCheckNumber ?? "",
            employmentStatus: item.employmentStatus ?? ""
          }));

          dispatch({
            type: "PROFIT_DATA_SUCCESS",
            payload: { results: transformedResults, total: profitResult.total }
          });
        } catch {
          dispatch({ type: "PROFIT_DATA_FAILURE" });
        }
      } catch {
        dispatch({ type: "SEARCH_FAILURE" });
        addAlert({
          id: 500,
          severity: "error",
          message: "Search Failed",
          description: "An error occurred while searching. Please try again."
        });
      }
    },
    [
      state.search.isSearching,
      state.pagination.pageSize,
      profitYear,
      triggerSearch,
      triggerProfitDetails,
      addAlert,
      clearAlerts,
      clearMessages
    ]
  );

  // Handle pagination change
  const handlePaginationChange = useCallback(
    async (pageNumber: number, pageSize: number) => {
      dispatch({ type: "SET_PAGINATION", payload: { pageNumber, pageSize } });

      if (!state.member.selectedMember) return;

      dispatch({ type: "PROFIT_DATA_START" });
      try {
        const { memberType, id } = state.member.selectedMember;
        const profitResult = await triggerProfitDetails({
          memberType,
          id,
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: "profitYear",
          isSortDescending: true
        }).unwrap();

        const transformedResults: ProfitDetailRow[] = profitResult.results.map((item: MasterInquiryResponseDto) => ({
          id: item.id,
          profitYear: item.profitYear,
          profitYearIteration: item.profitYearIteration,
          profitCodeId: item.profitCodeId,
          profitCodeName: item.profitCodeName ?? "",
          contribution: typeof item.contribution === "number" ? item.contribution : 0,
          earnings: item.earnings,
          forfeiture: item.forfeiture,
          payment: item.payment ?? 0,
          monthToDate: item.monthToDate,
          yearToDate: item.yearToDate,
          currentHoursYear: item.currentHoursYear,
          currentIncomeYear: item.currentIncomeYear,
          federalTaxes: item.federalTaxes,
          stateTaxes: item.stateTaxes,
          taxCode: (item.taxCode as string) ?? "",
          commentTypeName: item.commentTypeName ?? "",
          commentRelatedCheckNumber: item.commentRelatedCheckNumber ?? "",
          employmentStatus: item.employmentStatus ?? ""
        }));

        dispatch({
          type: "PROFIT_DATA_SUCCESS",
          payload: { results: transformedResults, total: profitResult.total }
        });
      } catch {
        dispatch({ type: "PROFIT_DATA_FAILURE" });
      }
    },
    [state.member.selectedMember, triggerProfitDetails]
  );

  // Initiate reversal (open confirmation modal)
  const initiateReversal = useCallback((selectedRows: ProfitDetailRow[]) => {
    const items: ReversalItem[] = selectedRows.map((row) => ({
      id: row.id,
      payment: row.payment
    }));
    dispatch({ type: "OPEN_CONFIRMATION", payload: items });
  }, []);

  // Confirm current reversal and advance to next or execute
  const confirmReversal = useCallback(async () => {
    const { selectedItems, currentIndex, confirmedIds } = state.confirmation;
    const isLastItem = currentIndex === selectedItems.length - 1;

    if (isLastItem) {
      // All items confirmed, execute the reversal
      dispatch({ type: "SET_PROCESSING", payload: true });

      const allIds = [...confirmedIds, selectedItems[currentIndex].id];

      try {
        await reverseProfitDetails({ ids: allIds, onlyNetworkToastErrors: true }).unwrap();

        dispatch({ type: "CLOSE_CONFIRMATION" });

        // Build success message with profit detail IDs and payment amounts
        const reversalDetails = selectedItems
          .map((item) => `Profit Detail ${item.id}: ${numberToCurrency(item.payment)}`)
          .join("\n");

        reduxDispatch(
          setMessage({
            key: REVERSALS_MESSAGE_KEY,
            message: {
              type: "success",
              title: `Successfully reversed ${allIds.length} transaction${allIds.length > 1 ? "s" : ""}.`,
              message: reversalDetails
            }
          })
        );

        // Refresh profit data
        if (state.member.selectedMember) {
          dispatch({ type: "PROFIT_DATA_START" });
          try {
            const { memberType, id } = state.member.selectedMember;
            const profitResult = await triggerProfitDetails({
              memberType,
              id,
              skip: state.pagination.pageNumber * state.pagination.pageSize,
              take: state.pagination.pageSize,
              sortBy: "profitYear",
              isSortDescending: true
            }).unwrap();

            const transformedResults: ProfitDetailRow[] = profitResult.results.map(
              (item: MasterInquiryResponseDto) => ({
                id: item.id,
                profitYear: item.profitYear,
                profitYearIteration: item.profitYearIteration,
                profitCodeId: item.profitCodeId,
                profitCodeName: item.profitCodeName ?? "",
                contribution: typeof item.contribution === "number" ? item.contribution : 0,
                earnings: item.earnings,
                forfeiture: item.forfeiture,
                payment: item.payment ?? 0,
                monthToDate: item.monthToDate,
                yearToDate: item.yearToDate,
                currentHoursYear: item.currentHoursYear,
                currentIncomeYear: item.currentIncomeYear,
                federalTaxes: item.federalTaxes,
                stateTaxes: item.stateTaxes,
                taxCode: (item.taxCode as string) ?? "",
                commentTypeName: item.commentTypeName ?? "",
                commentRelatedCheckNumber: item.commentRelatedCheckNumber ?? "",
                employmentStatus: item.employmentStatus ?? ""
              })
            );

            dispatch({
              type: "PROFIT_DATA_SUCCESS",
              payload: { results: transformedResults, total: profitResult.total }
            });
          } catch {
            dispatch({ type: "PROFIT_DATA_FAILURE" });
          }
        }
      } catch (error) {
        dispatch({ type: "CLOSE_CONFIRMATION" });

        // Check for 400 validation errors
        const serviceError = error as ServiceErrorResponse;
        if (serviceError?.data?.status === 400 && serviceError?.data?.errors) {
          // Build detailed error message from validation errors
          const validationErrors = serviceError.data.errors;
          const errorMessages: string[] = [];

          for (const [key, messages] of Object.entries(validationErrors)) {
            // Extract profit detail ID from key (e.g., "profitDetail_111315" -> "111315")
            const idMatch = key.match(/profitDetail_(\d+)/);
            const profitDetailId = idMatch ? idMatch[1] : key;

            if (Array.isArray(messages) && messages.length > 0) {
              errorMessages.push(`Profit Detail ${profitDetailId}: ${messages.join("; ")}`);
            }
          }

          reduxDispatch(
            setMessage({
              key: REVERSALS_MESSAGE_KEY,
              message: {
                type: "error",
                title: "Reversal Failed",
                message: errorMessages.length > 0 ? errorMessages.join("\n") : "Validation error occurred."
              }
            })
          );
        } else {
          // Generic error message for non-400 errors
          reduxDispatch(
            setMessage({
              key: REVERSALS_MESSAGE_KEY,
              message: {
                type: "error",
                title: "Reversal Failed",
                message: "An error occurred while processing the reversals. None of the transactions were saved."
              }
            })
          );
        }
      }
    } else {
      // Advance to next item
      dispatch({ type: "ADVANCE_CONFIRMATION" });
    }
  }, [
    state.confirmation,
    state.member.selectedMember,
    state.pagination,
    reverseProfitDetails,
    triggerProfitDetails,
    reduxDispatch
  ]);

  // Cancel reversal
  const cancelReversal = useCallback(() => {
    const { selectedItems } = state.confirmation;
    const cancelledIds = selectedItems.map((item) => item.id);

    dispatch({ type: "CLOSE_CONFIRMATION" });

    if (cancelledIds.length > 0) {
      reduxDispatch(
        setMessage({
          key: REVERSALS_MESSAGE_KEY,
          message: {
            type: "error", // Using error type for yellow/warning appearance
            title: "Reversals Cancelled",
            message: `Cancelled reversal for profit detail IDs: ${cancelledIds.join(", ")}`
          }
        })
      );
    }
  }, [state.confirmation, reduxDispatch]);

  // Reset all state
  const resetAll = useCallback(() => {
    lastSearchRef.current = null;
    clearAlerts();
    clearMessages();
    dispatch({ type: "RESET_ALL" });
  }, [clearAlerts, clearMessages]);

  return {
    // Search state
    isSearching: state.search.isSearching,

    // Member state
    selectedMember: state.member.selectedMember,
    memberDetails: state.member.memberDetails,
    isFetchingMemberDetails: state.member.isFetchingDetails,

    // Profit data state
    profitData: state.profitData,
    isFetchingProfitData: state.isFetchingProfitData,

    // Pagination
    pageNumber: state.pagination.pageNumber,
    pageSize: state.pagination.pageSize,

    // Confirmation modal state
    isConfirmationOpen: state.confirmation.isOpen,
    confirmationItems: state.confirmation.selectedItems,
    confirmationIndex: state.confirmation.currentIndex,
    isProcessing: state.confirmation.isProcessing,

    // Actions
    executeSearch,
    handlePaginationChange,
    initiateReversal,
    confirmReversal,
    cancelReversal,
    resetAll
  };
}
