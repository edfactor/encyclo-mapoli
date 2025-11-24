import { useCallback, useReducer } from "react";

interface DistributionsAndForfeituresState {
  initialSearchLoaded: boolean;
  previousStatus: string | null;
  shouldArchive: boolean;
}

type DistributionsAndForfeituresAction =
  | { type: "SET_INITIAL_SEARCH_LOADED"; payload: boolean }
  | { type: "SET_STATUS_CHANGE"; payload: { status: string; statusName?: string } }
  | { type: "SET_ARCHIVE_HANDLED" }
  | { type: "RESET_STATE" };

const initialState: DistributionsAndForfeituresState = {
  initialSearchLoaded: false,
  previousStatus: null,
  shouldArchive: false
};

function distributionsAndForfeituresReducer(
  state: DistributionsAndForfeituresState,
  action: DistributionsAndForfeituresAction
): DistributionsAndForfeituresState {
  switch (action.type) {
    case "SET_INITIAL_SEARCH_LOADED":
      return {
        ...state,
        initialSearchLoaded: action.payload
      };

    case "SET_STATUS_CHANGE": {
      const { status, statusName } = action.payload;
      const isCompleteLike = (statusName ?? "").toLowerCase().includes("complete");
      const isChangingToComplete = isCompleteLike && state.previousStatus !== status;

      return {
        ...state,
        previousStatus: status,
        shouldArchive: isChangingToComplete
      };
    }

    case "SET_ARCHIVE_HANDLED":
      return {
        ...state,
        shouldArchive: false
      };

    case "RESET_STATE":
      return initialState;

    default:
      return state;
  }
}

export const useDistributionsAndForfeituresState = () => {
  const [state, dispatch] = useReducer(distributionsAndForfeituresReducer, initialState);

  const setInitialSearchLoaded = useCallback((loaded: boolean) => {
    dispatch({ type: "SET_INITIAL_SEARCH_LOADED", payload: loaded });
  }, []);

  const handleStatusChange = useCallback((newStatus: string, statusName?: string) => {
    dispatch({ type: "SET_STATUS_CHANGE", payload: { status: newStatus, statusName } });
  }, []);

  const handleArchiveHandled = useCallback(() => {
    dispatch({ type: "SET_ARCHIVE_HANDLED" });
  }, []);

  return {
    state,
    actions: {
      setInitialSearchLoaded,
      handleStatusChange,
      handleArchiveHandled
    }
  };
};
