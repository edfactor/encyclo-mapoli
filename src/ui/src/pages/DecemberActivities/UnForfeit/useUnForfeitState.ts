import { useCallback, useReducer } from "react";
import { StartAndEndDateRequest } from "../../../reduxstore/types";

export interface UnForfeitSearchRequest extends StartAndEndDateRequest {
  archive?: boolean;
}

interface UnForfeitState {
  initialSearchLoaded: boolean;
  resetPageFlag: boolean;
  hasUnsavedChanges: boolean;
  shouldBlock: boolean;
  previousStatus: string | null;
  shouldArchive: boolean;
}

type UnForfeitAction =
  | { type: "SET_INITIAL_SEARCH_LOADED"; payload: boolean }
  | { type: "TOGGLE_RESET_PAGE_FLAG" }
  | { type: "SET_UNSAVED_CHANGES"; payload: boolean }
  | { type: "SET_SHOULD_BLOCK"; payload: boolean }
  | { type: "SET_STATUS_CHANGE"; payload: { status: string; statusName?: string } }
  | { type: "SET_ARCHIVE_HANDLED" }
  | { type: "RESET_STATE" };

const initialState: UnForfeitState = {
  initialSearchLoaded: false,
  resetPageFlag: false,
  hasUnsavedChanges: false,
  shouldBlock: false,
  previousStatus: null,
  shouldArchive: false
};

function unForfeitReducer(state: UnForfeitState, action: UnForfeitAction): UnForfeitState {
  switch (action.type) {
    case "SET_INITIAL_SEARCH_LOADED":
      return {
        ...state,
        initialSearchLoaded: action.payload
      };

    case "TOGGLE_RESET_PAGE_FLAG":
      return {
        ...state,
        resetPageFlag: !state.resetPageFlag
      };

    case "SET_UNSAVED_CHANGES":
      return {
        ...state,
        hasUnsavedChanges: action.payload,
        shouldBlock: action.payload
      };

    case "SET_SHOULD_BLOCK":
      return {
        ...state,
        shouldBlock: action.payload
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

export const useUnForfeitState = () => {
  const [state, dispatch] = useReducer(unForfeitReducer, initialState);

  const setInitialSearchLoaded = useCallback((loaded: boolean) => {
    dispatch({ type: "SET_INITIAL_SEARCH_LOADED", payload: loaded });
  }, []);

  const handleSearch = useCallback(() => {
    dispatch({ type: "TOGGLE_RESET_PAGE_FLAG" });
  }, []);

  const handleUnsavedChanges = useCallback((hasChanges: boolean) => {
    dispatch({ type: "SET_UNSAVED_CHANGES", payload: hasChanges });
  }, []);

  const setShouldBlock = useCallback((shouldBlock: boolean) => {
    dispatch({ type: "SET_SHOULD_BLOCK", payload: shouldBlock });
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
      handleSearch,
      handleUnsavedChanges,
      setShouldBlock,
      handleStatusChange,
      handleArchiveHandled
    }
  };
};
