import { useCallback, useReducer } from "react";
import { TerminationSearchRequest } from "../pages/DecemberActivities/Termination/Termination";

interface TerminationState {
  searchParams: TerminationSearchRequest | null;
  initialSearchLoaded: boolean;
  hasUnsavedChanges: boolean;
  resetPageFlag: boolean;
  currentStatus: string | null;
  archiveMode: boolean;
  shouldArchive: boolean;
}

type TerminationAction =
  | { type: "SET_SEARCH_PARAMS"; payload: TerminationSearchRequest }
  | { type: "SET_INITIAL_SEARCH_LOADED"; payload: boolean }
  | { type: "SET_UNSAVED_CHANGES"; payload: boolean }
  | { type: "TOGGLE_RESET_PAGE_FLAG" }
  | { type: "SET_STATUS_CHANGE"; payload: { status: string; statusName?: string } }
  | { type: "SET_ARCHIVE_HANDLED" }
  | { type: "RESET_STATE" };

const initialState: TerminationState = {
  searchParams: null,
  initialSearchLoaded: false,
  hasUnsavedChanges: false,
  resetPageFlag: false,
  currentStatus: null,
  archiveMode: false,
  shouldArchive: false
};

function terminationReducer(state: TerminationState, action: TerminationAction): TerminationState {
  switch (action.type) {
    case "SET_SEARCH_PARAMS":
      return {
        ...state,
        searchParams: action.payload,
        initialSearchLoaded: true,
        resetPageFlag: !state.resetPageFlag
      };

    case "SET_INITIAL_SEARCH_LOADED":
      return {
        ...state,
        initialSearchLoaded: action.payload
      };

    case "SET_UNSAVED_CHANGES":
      return {
        ...state,
        hasUnsavedChanges: action.payload
      };

    case "TOGGLE_RESET_PAGE_FLAG":
      return {
        ...state,
        resetPageFlag: !state.resetPageFlag
      };

    case "SET_STATUS_CHANGE": {
      const { status, statusName } = action.payload;
      const isCompleteLike = (statusName ?? "").toLowerCase().includes("complete");
      const isChangingToComplete = isCompleteLike && state.currentStatus !== statusName;

      if (isChangingToComplete) {
        const updatedSearchParams = state.searchParams ? { ...state.searchParams, archive: true } : null;

        return {
          ...state,
          currentStatus: statusName || null,
          archiveMode: true,
          shouldArchive: true,
          searchParams: updatedSearchParams,
          resetPageFlag: !state.resetPageFlag
        };
      } else {
        const shouldResetArchive = !isCompleteLike;
        let updatedSearchParams = state.searchParams;

        if (shouldResetArchive && state.searchParams) {
          const { archive, ...paramsWithoutArchive } = state.searchParams as TerminationSearchRequest & { archive?: boolean };
          updatedSearchParams = paramsWithoutArchive as TerminationSearchRequest;
        }

        return {
          ...state,
          currentStatus: statusName || null,
          archiveMode: shouldResetArchive ? false : state.archiveMode,
          searchParams: shouldResetArchive ? updatedSearchParams : state.searchParams,
          resetPageFlag: shouldResetArchive ? !state.resetPageFlag : state.resetPageFlag
        };
      }
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

export const useTerminationState = () => {
  const [state, dispatch] = useReducer(terminationReducer, initialState);

  const handleSearch = useCallback(
    (params: TerminationSearchRequest) => {
      const searchParamsWithArchive = {
        ...params,
        ...(state.archiveMode && { archive: true })
      };
      dispatch({ type: "SET_SEARCH_PARAMS", payload: searchParamsWithArchive });
    },
    [state.archiveMode]
  );

  const handleUnsavedChanges = useCallback((hasChanges: boolean) => {
    dispatch({ type: "SET_UNSAVED_CHANGES", payload: hasChanges });
  }, []);

  const handleStatusChange = useCallback((newStatus: string, statusName?: string) => {
    dispatch({ type: "SET_STATUS_CHANGE", payload: { status: newStatus, statusName } });
  }, []);

  const handleArchiveHandled = useCallback(() => {
    dispatch({ type: "SET_ARCHIVE_HANDLED" });
  }, []);

  const setInitialSearchLoaded = useCallback((loaded: boolean) => {
    dispatch({ type: "SET_INITIAL_SEARCH_LOADED", payload: loaded });
  }, []);

  return {
    state,
    actions: {
      handleSearch,
      handleUnsavedChanges,
      handleStatusChange,
      handleArchiveHandled,
      setInitialSearchLoaded
    }
  };
};
