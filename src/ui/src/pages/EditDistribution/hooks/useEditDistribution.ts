import { useCallback, useReducer } from "react";
import { useUpdateDistributionMutation } from "../../../reduxstore/api/DistributionApi";
import { EditDistributionRequest } from "../../../types";
import { ServiceErrorResponse } from "../../../types/errors/errors";

// State interface
interface EditDistributionState {
  submission: {
    isSubmitting: boolean;
    error: string | null;
    success: boolean;
  };
}

// Action types
type EditDistributionAction =
  | { type: "SUBMISSION_START" }
  | { type: "SUBMISSION_SUCCESS" }
  | { type: "SUBMISSION_FAILURE"; payload: { error: string } }
  | { type: "CLEAR_SUBMISSION_ERROR" }
  | { type: "RESET" };

// Initial state
const initialState: EditDistributionState = {
  submission: {
    isSubmitting: false,
    error: null,
    success: false
  }
};

// Reducer function
const editDistributionReducer = (
  state: EditDistributionState,
  action: EditDistributionAction
): EditDistributionState => {
  switch (action.type) {
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
export const useEditDistribution = () => {
  const [state, dispatch] = useReducer(editDistributionReducer, initialState);

  // RTK Query hook
  const [updateDistribution] = useUpdateDistributionMutation();

  // Submit distribution update
  const submitDistribution = useCallback(
    async (request: EditDistributionRequest) => {
      try {
        dispatch({ type: "SUBMISSION_START" });

        const response = await updateDistribution(request).unwrap();

        dispatch({ type: "SUBMISSION_SUCCESS" });

        return response;
      } catch (error) {
        const serviceError = error as ServiceErrorResponse;
        console.log("Error in submitDistribution:", error);
        let errorMsg = serviceError?.data?.detail || "Failed to update distribution";

        // Check for specific error messages and customize
        if (errorMsg) {
          if (errorMsg.startsWith("Gross amount")) {
            errorMsg = "Amount Requested exceeds vested balance.";
          } else if (errorMsg.startsWith("Badge number")) {
            errorMsg = "Badge Number not found.";
          } else if (errorMsg.includes("not found")) {
            errorMsg = "Distribution not found.";
          }
        }

        dispatch({ type: "SUBMISSION_FAILURE", payload: { error: errorMsg } });
        throw error;
      }
    },
    [updateDistribution]
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
    isSubmitting: state.submission.isSubmitting,
    submissionError: state.submission.error,
    submissionSuccess: state.submission.success,

    // Methods
    submitDistribution,
    clearSubmissionError,
    reset
  };
};

export default useEditDistribution;
