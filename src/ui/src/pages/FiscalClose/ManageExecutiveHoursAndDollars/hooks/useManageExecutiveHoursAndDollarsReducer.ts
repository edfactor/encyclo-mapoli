import { ExecutiveHoursAndDollars, ExecutiveHoursAndDollarsGrid, PagedReportResponse } from "reduxstore/types";
import { ExecutiveHoursAndDollarsRequestDto } from "types/fiscal/executive";

export interface ExecutiveSearchResults {
  results: ExecutiveHoursAndDollars[];
  total: number;
}

export interface SelectedExecutive {
  badgeNumber: number;
  fullName: string;
  storeNumber?: number;
  hoursExecutive: number;
  incomeExecutive: number;
}

export type ViewMode = "idle" | "searching" | "results" | "modal";

export interface ManageExecutiveHoursAndDollarsState {
  search: {
    params: ExecutiveHoursAndDollarsRequestDto | null;
    results: ExecutiveSearchResults | null;
    isSearching: boolean;
    error: string | null;
    initialLoaded: boolean;
  };

  grid: {
    data: PagedReportResponse<ExecutiveHoursAndDollars> | null;
    pendingChanges: ExecutiveHoursAndDollarsGrid | null;
    additionalExecutives: ExecutiveHoursAndDollars[];
    selectedRows: ExecutiveHoursAndDollars[];
  };

  modal: {
    isOpen: boolean;
    results: PagedReportResponse<ExecutiveHoursAndDollars> | null;
    selectedExecutives: ExecutiveHoursAndDollars[];
    isSearching: boolean;
    searchParams: ExecutiveHoursAndDollarsRequestDto | null;
  };

  view: {
    mode: ViewMode;
    pageNumberReset: boolean;
  };
}

export type ManageExecutiveHoursAndDollarsAction =
  | { type: "SEARCH_START"; payload: { params: ExecutiveHoursAndDollarsRequestDto } }
  | { type: "SEARCH_SUCCESS"; payload: { results: PagedReportResponse<ExecutiveHoursAndDollars> } }
  | { type: "SEARCH_FAILURE"; payload: { error: string } }
  | { type: "SEARCH_RESET" }
  | { type: "SET_INITIAL_LOADED"; payload: { loaded: boolean } }
  | { type: "SET_PAGE_RESET"; payload: { reset: boolean } }
  | { type: "ADD_PENDING_CHANGE"; payload: { change: ExecutiveHoursAndDollarsGrid } }
  | { type: "UPDATE_PENDING_CHANGE"; payload: { change: ExecutiveHoursAndDollarsGrid } }
  | { type: "REMOVE_PENDING_CHANGE"; payload: { change: ExecutiveHoursAndDollarsGrid } }
  | { type: "CLEAR_PENDING_CHANGES" }
  | { type: "ADD_ADDITIONAL_EXECUTIVES"; payload: { executives: ExecutiveHoursAndDollars[] } }
  | { type: "CLEAR_ADDITIONAL_EXECUTIVES" }
  | { type: "MODAL_OPEN" }
  | { type: "MODAL_CLOSE" }
  | { type: "MODAL_SEARCH_START"; payload: { params: ExecutiveHoursAndDollarsRequestDto } }
  | { type: "MODAL_SEARCH_SUCCESS"; payload: { results: PagedReportResponse<ExecutiveHoursAndDollars> } }
  | { type: "MODAL_SEARCH_FAILURE"; payload: { error: string } }
  | { type: "MODAL_SELECT_EXECUTIVES"; payload: { executives: ExecutiveHoursAndDollars[] } }
  | { type: "MODAL_CLEAR_SELECTION" }
  | { type: "SET_VIEW_MODE"; payload: { mode: ViewMode } }
  | { type: "RESET_ALL" };

export const initialState: ManageExecutiveHoursAndDollarsState = {
  search: {
    params: null,
    results: null,
    isSearching: false,
    error: null,
    initialLoaded: false
  },
  grid: {
    data: null,
    pendingChanges: null,
    additionalExecutives: [],
    selectedRows: []
  },
  modal: {
    isOpen: false,
    results: null,
    selectedExecutives: [],
    isSearching: false,
    searchParams: null
  },
  view: {
    mode: "idle",
    pageNumberReset: false
  }
};

export function manageExecutiveHoursAndDollarsReducer(
  state: ManageExecutiveHoursAndDollarsState,
  action: ManageExecutiveHoursAndDollarsAction
): ManageExecutiveHoursAndDollarsState {
  switch (action.type) {
    case "SEARCH_START":
      return {
        ...state,
        search: {
          ...state.search,
          params: action.payload.params,
          isSearching: true,
          error: null
        },
        view: {
          ...state.view,
          mode: "searching"
        }
      };

    case "SEARCH_SUCCESS":
      return {
        ...state,
        search: {
          ...state.search,
          results: {
            results: action.payload.results.response?.results || [],
            total: action.payload.results.response?.total || 0
          },
          isSearching: false,
          error: null,
          initialLoaded: true
        },
        grid: {
          ...state.grid,
          data: action.payload.results
        },
        view: {
          ...state.view,
          mode: "results"
        }
      };

    case "SEARCH_FAILURE":
      return {
        ...state,
        search: {
          ...state.search,
          isSearching: false,
          error: action.payload.error
        },
        view: {
          ...state.view,
          mode: "idle"
        }
      };

    case "SEARCH_RESET":
      return {
        ...state,
        search: {
          ...initialState.search
        },
        grid: {
          ...initialState.grid
        },
        view: {
          mode: "idle",
          pageNumberReset: false
        }
      };

    case "SET_INITIAL_LOADED":
      return {
        ...state,
        search: {
          ...state.search,
          initialLoaded: action.payload.loaded
        }
      };

    case "SET_PAGE_RESET":
      return {
        ...state,
        view: {
          ...state.view,
          pageNumberReset: action.payload.reset
        }
      };

    case "ADD_PENDING_CHANGE": {
      const existingChanges = state.grid.pendingChanges?.executiveHoursAndDollars || [];
      const newChange = action.payload.change.executiveHoursAndDollars[0];

      return {
        ...state,
        grid: {
          ...state.grid,
          pendingChanges: {
            profitYear: action.payload.change.profitYear,
            executiveHoursAndDollars: [...existingChanges, newChange]
          }
        }
      };
    }

    case "UPDATE_PENDING_CHANGE": {
      const existingChanges = state.grid.pendingChanges?.executiveHoursAndDollars || [];
      const updateChange = action.payload.change.executiveHoursAndDollars[0];
      const updatedChanges = existingChanges.map((change) =>
        change.badgeNumber === updateChange.badgeNumber ? updateChange : change
      );

      return {
        ...state,
        grid: {
          ...state.grid,
          pendingChanges: {
            profitYear: action.payload.change.profitYear,
            executiveHoursAndDollars: updatedChanges
          }
        }
      };
    }

    case "REMOVE_PENDING_CHANGE": {
      const existingChanges = state.grid.pendingChanges?.executiveHoursAndDollars || [];
      const removeChange = action.payload.change.executiveHoursAndDollars[0];
      const filteredChanges = existingChanges.filter((change) => change.badgeNumber !== removeChange.badgeNumber);

      return {
        ...state,
        grid: {
          ...state.grid,
          pendingChanges:
            filteredChanges.length > 0
              ? {
                  profitYear: action.payload.change.profitYear,
                  executiveHoursAndDollars: filteredChanges
                }
              : null
        }
      };
    }

    case "CLEAR_PENDING_CHANGES":
      return {
        ...state,
        grid: {
          ...state.grid,
          pendingChanges: null
        }
      };

    case "ADD_ADDITIONAL_EXECUTIVES":
      console.log("ADD_ADDITIONAL_EXECUTIVES reducer action", {
        currentAdditionalCount: state.grid.additionalExecutives.length,
        incomingExecutivesCount: action.payload.executives.length,
        incomingExecutives: action.payload.executives
      });
      const newState = {
        ...state,
        grid: {
          ...state.grid,
          additionalExecutives: [...state.grid.additionalExecutives, ...action.payload.executives]
        }
      };
      console.log("ADD_ADDITIONAL_EXECUTIVES new state", {
        newAdditionalCount: newState.grid.additionalExecutives.length
      });
      return newState;

    case "CLEAR_ADDITIONAL_EXECUTIVES":
      return {
        ...state,
        grid: {
          ...state.grid,
          additionalExecutives: []
        }
      };

    case "MODAL_OPEN":
      return {
        ...state,
        modal: {
          ...state.modal,
          isOpen: true
        }
      };

    case "MODAL_CLOSE":
      return {
        ...state,
        modal: {
          ...initialState.modal,
          isOpen: false
        }
      };

    case "MODAL_SEARCH_START":
      return {
        ...state,
        modal: {
          ...state.modal,
          searchParams: action.payload.params,
          isSearching: true
        }
      };

    case "MODAL_SEARCH_SUCCESS":
      return {
        ...state,
        modal: {
          ...state.modal,
          results: action.payload.results,
          isSearching: false
        }
      };

    case "MODAL_SEARCH_FAILURE":
      return {
        ...state,
        modal: {
          ...state.modal,
          isSearching: false
        }
      };

    case "MODAL_SELECT_EXECUTIVES":
      return {
        ...state,
        modal: {
          ...state.modal,
          selectedExecutives: action.payload.executives
        }
      };

    case "MODAL_CLEAR_SELECTION":
      return {
        ...state,
        modal: {
          ...state.modal,
          selectedExecutives: []
        }
      };

    case "SET_VIEW_MODE":
      return {
        ...state,
        view: {
          ...state.view,
          mode: action.payload.mode
        }
      };

    case "RESET_ALL":
      return initialState;

    default:
      return state;
  }
}

export const selectHasPendingChanges = (state: ManageExecutiveHoursAndDollarsState): boolean =>
  Boolean(state.grid.pendingChanges?.executiveHoursAndDollars?.length);

export const selectIsRowStagedToSave =
  (state: ManageExecutiveHoursAndDollarsState) =>
  (badge: number): boolean => {
    const pendingChanges = state.grid.pendingChanges?.executiveHoursAndDollars || [];
    return pendingChanges.some((change) => change.badgeNumber === badge);
  };

export const selectCombinedGridData = (
  state: ManageExecutiveHoursAndDollarsState
): PagedReportResponse<ExecutiveHoursAndDollars> | null => {
  const mainData = state.grid.data;
  const additionalExecutives = state.grid.additionalExecutives;

  console.log("selectCombinedGridData called", {
    hasMainData: !!mainData?.response?.results,
    mainDataCount: mainData?.response?.results?.length || 0,
    additionalExecutivesCount: additionalExecutives.length,
    additionalExecutives
  });

  if (!mainData || !mainData.response || !mainData.response.results) {
    return null;
  }

  if (additionalExecutives.length === 0) {
    return mainData;
  }

  const mainGridStructureCopy = structuredClone(mainData);
  const existingBadgeNumbers = new Set(mainGridStructureCopy.response.results.map((item) => item.badgeNumber));
  const filteredAdditionalResults = additionalExecutives.filter((item) => !existingBadgeNumbers.has(item.badgeNumber));

  console.log("Combining data", {
    mainCount: mainGridStructureCopy.response.results.length,
    filteredAdditionalCount: filteredAdditionalResults.length
  });

  mainGridStructureCopy.response.results = mainGridStructureCopy.response.results.concat(filteredAdditionalResults);

  console.log("Combined data result", {
    totalCount: mainGridStructureCopy.response.results.length
  });

  return mainGridStructureCopy;
};

export const selectShowGrid = (state: ManageExecutiveHoursAndDollarsState): boolean =>
  state.view.mode === "results" && Boolean(state.search.results && state.search.results.results.length > 0);

export const selectShowModal = (state: ManageExecutiveHoursAndDollarsState): boolean => state.modal.isOpen;
