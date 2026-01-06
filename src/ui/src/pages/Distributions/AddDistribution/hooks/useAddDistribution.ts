import { useCallback, useReducer } from "react";
import {
  useCreateDistributionMutation,
  useLazySearchDistributionsQuery
} from "../../../../reduxstore/api/DistributionApi";
import {
  useLazyGetProfitMasterInquiryMemberQuery,
  useLazySearchProfitMasterInquiryQuery
} from "../../../../reduxstore/api/InquiryApi";
import { useLazyGetStateTaxQuery } from "../../../../reduxstore/api/LookupsApi";
import { CreateDistributionRequest, EmployeeDetails } from "../../../../types";
import { ServiceErrorResponse } from "../../../../types/errors/errors";

// State interface
interface AddDistributionState {
  member: {
    data: EmployeeDetails | null;
    isLoading: boolean;
    error: string | null;
  };
  stateTax: {
    rate: number | null;
    isLoading: boolean;
    error: string | null;
  };
  sequenceNumber: {
    value: number | null;
    isLoading: boolean;
    error: string | null;
    maximumReached: boolean;
  };
  submission: {
    isSubmitting: boolean;
    error: string | null;
    success: boolean;
  };
}

// Action types
type AddDistributionAction =
  | { type: "MEMBER_FETCH_START" }
  | { type: "MEMBER_FETCH_SUCCESS"; payload: { data: EmployeeDetails } }
  | { type: "MEMBER_FETCH_FAILURE"; payload: { error: string } }
  | { type: "STATE_TAX_FETCH_START" }
  | { type: "STATE_TAX_FETCH_SUCCESS"; payload: { rate: number } }
  | { type: "STATE_TAX_FETCH_FAILURE"; payload: { error: string } }
  | { type: "SEQUENCE_NUMBER_FETCH_START" }
  | { type: "SEQUENCE_NUMBER_FETCH_SUCCESS"; payload: { value: number; maximumReached: boolean } }
  | { type: "SEQUENCE_NUMBER_FETCH_FAILURE"; payload: { error: string } }
  | { type: "SUBMISSION_START" }
  | { type: "SUBMISSION_SUCCESS" }
  | { type: "SUBMISSION_FAILURE"; payload: { error: string } }
  | { type: "CLEAR_SUBMISSION_ERROR" }
  | { type: "RESET" };

// Initial state
const initialState: AddDistributionState = {
  member: {
    data: null,
    isLoading: false,
    error: null
  },
  stateTax: {
    rate: null,
    isLoading: false,
    error: null
  },
  sequenceNumber: {
    value: null,
    isLoading: false,
    error: null,
    maximumReached: false
  },
  submission: {
    isSubmitting: false,
    error: null,
    success: false
  }
};

// Reducer function
const addDistributionReducer = (state: AddDistributionState, action: AddDistributionAction): AddDistributionState => {
  switch (action.type) {
    case "MEMBER_FETCH_START":
      return {
        ...state,
        member: {
          ...state.member,
          isLoading: true,
          error: null
        }
      };

    case "MEMBER_FETCH_SUCCESS":
      return {
        ...state,
        member: {
          data: action.payload.data,
          isLoading: false,
          error: null
        }
      };

    case "MEMBER_FETCH_FAILURE":
      return {
        ...state,
        member: {
          ...state.member,
          isLoading: false,
          error: action.payload.error
        }
      };

    case "STATE_TAX_FETCH_START":
      return {
        ...state,
        stateTax: {
          ...state.stateTax,
          isLoading: true,
          error: null
        }
      };

    case "STATE_TAX_FETCH_SUCCESS":
      return {
        ...state,
        stateTax: {
          rate: action.payload.rate,
          isLoading: false,
          error: null
        }
      };

    case "STATE_TAX_FETCH_FAILURE":
      return {
        ...state,
        stateTax: {
          ...state.stateTax,
          isLoading: false,
          error: action.payload.error
        }
      };

    case "SEQUENCE_NUMBER_FETCH_START":
      return {
        ...state,
        sequenceNumber: {
          ...state.sequenceNumber,
          isLoading: true,
          error: null
        }
      };

    case "SEQUENCE_NUMBER_FETCH_SUCCESS":
      return {
        ...state,
        sequenceNumber: {
          value: action.payload.value,
          isLoading: false,
          error: null,
          maximumReached: action.payload.maximumReached
        }
      };

    case "SEQUENCE_NUMBER_FETCH_FAILURE":
      return {
        ...state,
        sequenceNumber: {
          ...state.sequenceNumber,
          isLoading: false,
          error: action.payload.error
        }
      };

    case "SUBMISSION_START":
      return {
        ...state,
        submission: {
          isSubmitting: true,
          error: null,
          success: false
        }
      };

    case "SUBMISSION_SUCCESS":
      return {
        ...state,
        submission: {
          isSubmitting: false,
          error: null,
          success: true
        }
      };

    case "SUBMISSION_FAILURE":
      return {
        ...state,
        submission: {
          isSubmitting: false,
          error: action.payload.error,
          success: false
        }
      };

    case "CLEAR_SUBMISSION_ERROR":
      return {
        ...state,
        submission: {
          ...state.submission,
          error: null
        }
      };

    case "RESET":
      return initialState;

    default:
      return state;
  }
};

// Custom hook
export const useAddDistribution = () => {
  const [state, dispatch] = useReducer(addDistributionReducer, initialState);

  // RTK Query hooks
  const [triggerSearchMember] = useLazySearchProfitMasterInquiryQuery();
  const [triggerGetMember] = useLazyGetProfitMasterInquiryMemberQuery();
  const [triggerSearchDistributions] = useLazySearchDistributionsQuery();
  const [triggerGetStateTax] = useLazyGetStateTaxQuery();
  const [createDistribution] = useCreateDistributionMutation();

  // Fetch state tax rate
  const fetchStateTaxRate = useCallback(
    async (state: string) => {
      try {
        dispatch({ type: "STATE_TAX_FETCH_START" });

        const stateTaxResponse = await triggerGetStateTax(state).unwrap();

        dispatch({
          type: "STATE_TAX_FETCH_SUCCESS",
          payload: { rate: stateTaxResponse.stateTaxRate }
        });

        return stateTaxResponse.stateTaxRate;
      } catch (error) {
        const serviceError = error as ServiceErrorResponse;
        const errorMsg = serviceError?.data?.detail || "Failed to fetch state tax rate";
        dispatch({ type: "STATE_TAX_FETCH_FAILURE", payload: { error: errorMsg } });
        // Don't throw - state tax is optional, set to 0 if not found
        dispatch({
          type: "STATE_TAX_FETCH_SUCCESS",
          payload: { rate: 0 }
        });
        return 0;
      }
    },
    [triggerGetStateTax]
  );

  // Calculate next sequence number
  const calculateSequenceNumber = useCallback(
    async (badgeNumber: number, memberType: number) => {
      try {
        dispatch({ type: "SEQUENCE_NUMBER_FETCH_START" });

        // Search for all distributions for this member
        const searchResponse = await triggerSearchDistributions({
          badgeNumber,
          memberType,
          skip: 0,
          take: 1000 // Get all distributions to find max sequence
        }).unwrap();

        // Find the maximum paymentSequence value and count existing distributions
        // Only count distributions where frequencyId is not 'D'
        let maxSequence = 0;
        let distributionCount = 0;
        if (searchResponse?.results && searchResponse.results.length > 0) {
          maxSequence = Math.max(...searchResponse.results.map((d) => d.paymentSequence));
          distributionCount = searchResponse.results.filter((d) => d.frequencyId !== "D").length;
        }

        const nextSequence = maxSequence + 1;
        const maximumReached = distributionCount >= 9;

        dispatch({
          type: "SEQUENCE_NUMBER_FETCH_SUCCESS",
          payload: { value: nextSequence, maximumReached }
        });

        return nextSequence;
      } catch (error) {
        const serviceError = error as ServiceErrorResponse;
        const errorMsg = serviceError?.data?.detail || "Failed to calculate sequence number";
        dispatch({ type: "SEQUENCE_NUMBER_FETCH_FAILURE", payload: { error: errorMsg } });
        // Default to 1 if we can't calculate
        dispatch({
          type: "SEQUENCE_NUMBER_FETCH_SUCCESS",
          payload: { value: 1, maximumReached: false }
        });
        return 1;
      }
    },
    [triggerSearchDistributions]
  );

  // Fetch member data
  const fetchMemberData = useCallback(
    async (identifier: string, memberType: number, profitYear: number) => {
      try {
        dispatch({ type: "MEMBER_FETCH_START" });

        let identifierNum = parseInt(identifier, 10);
        const isSSN = identifier.length === 9 && /^\d{9}$/.test(identifier);

        let searchResponse;
        let memberId: number;
        let badgeNum = identifierNum;

        // If memberType is 2, we need to create a new variable for psnSuffix that is the last four digits and then use the numbers before that as the badge number
        let psnSuffix = 0;
        if (memberType === 2) {
          const identifierStr = identifier.toString();
          psnSuffix = parseInt(identifierStr.slice(-4), 10);
          identifierNum = parseInt(identifierStr.slice(0, -4), 10);
        }

        // Step 1: Try to search for member using identifier (could be badge number or SSN)
        try {
          searchResponse = await triggerSearchMember({
            // Pass badgeNumber only if not an SSN search
            badgeNumber: isSSN ? undefined : identifierNum,
            // Pass SSN if identifier is a 9-digit SSN (convert to number)
            ssn: isSSN ? parseInt(identifier, 10) : undefined,
            // Only use the psnSuffix parameter if it is not 0 and not an SSN search
            psnSuffix: !isSSN && psnSuffix !== 0 ? psnSuffix : undefined,
            memberType,
            endProfitYear: profitYear,
            pagination: {
              skip: 0,
              take: 1,
              sortBy: "badgeNumber",
              isSortDescending: false
            }
          }).unwrap();

          const results = Array.isArray(searchResponse) ? searchResponse : searchResponse.results;
          if (results && results.length > 0) {
            memberId = results[0].id;
            badgeNum = results[0].badgeNumber; // Get actual badge number from results
          } else {
            throw new Error(isSSN ? "Member not found with SSN" : "Member not found");
          }
        } catch (searchError) {
          if (isSSN) {
            throw new Error(`No member found with SSN ${identifier}`);
          }
          throw searchError;
        }

        // Step 2: Fetch member details using the ID
        const memberResponse = await triggerGetMember({
          id: memberId,
          memberType,
          profitYear
        }).unwrap();

        dispatch({ type: "MEMBER_FETCH_SUCCESS", payload: { data: memberResponse } });

        // After member fetch, get state tax rate
        if (memberResponse?.addressState) {
          await fetchStateTaxRate(memberResponse.addressState);
        }

        // After member fetch, calculate sequence number using the actual badge number
        await calculateSequenceNumber(badgeNum, memberType);

        return memberResponse;
      } catch (error) {
        const serviceError = error as ServiceErrorResponse;
        const errorInstance = error as Error;

        // Check for member not found conditions
        const isMemberNotFound =
          errorInstance?.message?.includes("Member not found") ||
          errorInstance?.message?.includes("No member found") ||
          serviceError?.data?.status === 500 ||
          serviceError?.data?.status === 400 ||
          serviceError?.data?.title === "Badge number not found." ||
          serviceError?.data?.title === "Validation Error";

        if (isMemberNotFound) {
          dispatch({ type: "MEMBER_FETCH_FAILURE", payload: { error: "MEMBER_NOT_FOUND" } });
        } else {
          const errorMsg = serviceError?.data?.detail || "Failed to fetch member data";
          dispatch({ type: "MEMBER_FETCH_FAILURE", payload: { error: errorMsg } });
        }
        // Don't throw - let the component handle the error via state
      }
    },
    [triggerSearchMember, triggerGetMember, calculateSequenceNumber, fetchStateTaxRate]
  );

  // Submit distribution
  const submitDistribution = useCallback(
    async (request: CreateDistributionRequest) => {
      try {
        dispatch({ type: "SUBMISSION_START" });

        // add the onlyNetworkToastErrors flag to the request
        const requestWithFlag = { ...request, onlyNetworkToastErrors: true };

        const response = await createDistribution(requestWithFlag).unwrap();

        dispatch({ type: "SUBMISSION_SUCCESS" });

        return response;
      } catch (error) {
        const serviceError = error as ServiceErrorResponse;
        console.log("Error in submitDistribution:", error);
        let errorMsg = serviceError?.data?.detail || serviceError?.data.detail || "Failed to create distribution";

        // Check for specific error messages and customize
        if (errorMsg) {
          if (errorMsg.startsWith("Gross amount")) {
            errorMsg = "Amount Requested exceeds vested balance.";
          } else if (errorMsg.startsWith("Badge number")) {
            errorMsg = "Badge Number not found.";
          }
        }

        dispatch({ type: "SUBMISSION_FAILURE", payload: { error: errorMsg } });
        throw error;
      }
    },
    [createDistribution]
  );

  // Clear submission error
  const clearSubmissionError = useCallback(() => {
    dispatch({ type: "CLEAR_SUBMISSION_ERROR" });
  }, []);

  // Reset hook state
  const reset = useCallback(() => {
    dispatch({ type: "RESET" });
  }, []);

  return {
    // State
    memberData: state.member.data,
    isMemberLoading: state.member.isLoading,
    memberError: state.member.error,

    stateTaxRate: state.stateTax.rate,
    isStateTaxLoading: state.stateTax.isLoading,
    stateTaxError: state.stateTax.error,

    sequenceNumber: state.sequenceNumber.value,
    isSequenceNumberLoading: state.sequenceNumber.isLoading,
    sequenceNumberError: state.sequenceNumber.error,
    maximumReached: state.sequenceNumber.maximumReached,

    isSubmitting: state.submission.isSubmitting,
    submissionError: state.submission.error,
    submissionSuccess: state.submission.success,

    // Methods
    fetchMemberData,
    fetchStateTaxRate,
    calculateSequenceNumber,
    submitDistribution,
    clearSubmissionError,
    reset
  };
};

export default useAddDistribution;
