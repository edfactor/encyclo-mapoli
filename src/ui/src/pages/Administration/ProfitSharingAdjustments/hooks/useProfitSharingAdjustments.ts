import { useCallback, useReducer } from "react";
import { useDispatch } from "react-redux";
import useFiscalCloseProfitYear from "../../../../hooks/useFiscalCloseProfitYear";
import { useMissiveAlerts } from "../../../../hooks/useMissiveAlerts";
import {
  useLazyGetProfitSharingAdjustmentsQuery,
  useSaveProfitSharingAdjustmentsMutation
} from "../../../../reduxstore/api/ProfitDetailsApi";
import { removeMessage } from "../../../../reduxstore/slices/messageSlice";
import {
  ProfitSharingAdjustmentRowDto,
  ProfitSharingAdjustmentsKey,
  SaveProfitSharingAdjustmentRowRequest
} from "../../../../reduxstore/types";
import { ProfitSharingAdjustmentsSearchParams } from "../ProfitSharingAdjustmentsSearchFilter";

// Constants
const ADJUSTMENTS_MESSAGE_KEY = "ProfitSharingAdjustmentsOperation";

export interface AdjustmentDraft {
  contribution: string;
  earnings: string;
  forfeiture: string;
}

// State Types
interface ProfitSharingAdjustmentsState {
  search: {
    params: ProfitSharingAdjustmentsSearchParams | null;
    isSearching: boolean;
  };
  adjustments: {
    loadedKey: ProfitSharingAdjustmentsKey | null;
    loadedGetAllRows: boolean;
    demographicId: number | null;
    rowData: ProfitSharingAdjustmentRowDto[];
    originalByRowNumber: Record<number, ProfitSharingAdjustmentRowDto>;
    stagedByRowNumber: Record<number, SaveProfitSharingAdjustmentRowRequest>;
    isFetching: boolean;
    isSaving: boolean;
  };
  selection: {
    selectedRow: ProfitSharingAdjustmentRowDto | null;
  };
  adjustmentModal: {
    isOpen: boolean;
    draft: AdjustmentDraft;
  };
  memberDetailsRefreshTrigger: number;
}

// Action Types
type ProfitSharingAdjustmentsAction =
  | { type: "SEARCH_START"; payload: ProfitSharingAdjustmentsSearchParams }
  | {
      type: "SEARCH_SUCCESS";
      payload: {
        key: ProfitSharingAdjustmentsKey;
        getAllRows: boolean;
        demographicId: number | null;
        rowData: ProfitSharingAdjustmentRowDto[];
      };
    }
  | { type: "SEARCH_FAILURE" }
  | { type: "SET_SAVING"; payload: boolean }
  | { type: "STAGE_ROW"; payload: { rowNumber: number; request: SaveProfitSharingAdjustmentRowRequest | null } }
  | { type: "CLEAR_STAGED" }
  | { type: "DISCARD_CHANGES"; payload: ProfitSharingAdjustmentRowDto[] }
  | { type: "SELECT_ROW"; payload: ProfitSharingAdjustmentRowDto | null }
  | { type: "OPEN_ADJUSTMENT_MODAL"; payload: AdjustmentDraft }
  | { type: "CLOSE_ADJUSTMENT_MODAL" }
  | { type: "UPDATE_ADJUSTMENT_DRAFT"; payload: Partial<AdjustmentDraft> }
  | { type: "APPLY_ADJUSTMENT"; payload: ProfitSharingAdjustmentRowDto }
  | { type: "UPDATE_ROW_DATA"; payload: ProfitSharingAdjustmentRowDto[] }
  | { type: "INCREMENT_MEMBER_DETAILS_REFRESH" }
  | { type: "RESET_ALL" };

const initialState: ProfitSharingAdjustmentsState = {
  search: {
    params: null,
    isSearching: false
  },
  adjustments: {
    loadedKey: null,
    loadedGetAllRows: false,
    demographicId: null,
    rowData: [],
    originalByRowNumber: {},
    stagedByRowNumber: {},
    isFetching: false,
    isSaving: false
  },
  selection: {
    selectedRow: null
  },
  adjustmentModal: {
    isOpen: false,
    draft: {
      contribution: "0",
      earnings: "0",
      forfeiture: "0"
    }
  },
  memberDetailsRefreshTrigger: 0
};

function adjustmentsReducer(
  state: ProfitSharingAdjustmentsState,
  action: ProfitSharingAdjustmentsAction
): ProfitSharingAdjustmentsState {
  switch (action.type) {
    case "SEARCH_START":
      return {
        ...state,
        search: { params: action.payload, isSearching: true }
      };
    case "SEARCH_SUCCESS": {
      const { key, getAllRows, demographicId, rowData } = action.payload;
      const originalByRowNumber = rowData.reduce<Record<number, ProfitSharingAdjustmentRowDto>>((acc, cur) => {
        acc[cur.rowNumber] = { ...cur };
        return acc;
      }, {});

      return {
        ...state,
        search: { ...state.search, isSearching: false },
        adjustments: {
          loadedKey: key,
          loadedGetAllRows: getAllRows,
          demographicId,
          rowData,
          originalByRowNumber,
          stagedByRowNumber: {},
          isFetching: false,
          isSaving: false
        },
        selection: {
          selectedRow: null
        }
      };
    }
    case "SEARCH_FAILURE":
      return {
        ...state,
        search: { ...state.search, isSearching: false },
        adjustments: { ...state.adjustments, isFetching: false }
      };
    case "SET_SAVING":
      return {
        ...state,
        adjustments: { ...state.adjustments, isSaving: action.payload }
      };
    case "STAGE_ROW": {
      const { rowNumber, request } = action.payload;
      const next = { ...state.adjustments.stagedByRowNumber };
      if (request === null) {
        delete next[rowNumber];
      } else {
        next[rowNumber] = request;
      }
      return {
        ...state,
        adjustments: { ...state.adjustments, stagedByRowNumber: next }
      };
    }
    case "CLEAR_STAGED":
      return {
        ...state,
        adjustments: { ...state.adjustments, stagedByRowNumber: {} }
      };
    case "DISCARD_CHANGES":
      return {
        ...state,
        adjustments: {
          ...state.adjustments,
          rowData: action.payload,
          stagedByRowNumber: {}
        }
      };
    case "SELECT_ROW":
      return {
        ...state,
        selection: { selectedRow: action.payload }
      };
    case "OPEN_ADJUSTMENT_MODAL":
      return {
        ...state,
        adjustmentModal: {
          isOpen: true,
          draft: action.payload
        }
      };
    case "CLOSE_ADJUSTMENT_MODAL":
      return {
        ...state,
        adjustmentModal: {
          isOpen: false,
          draft: state.adjustmentModal.draft
        }
      };
    case "UPDATE_ADJUSTMENT_DRAFT":
      return {
        ...state,
        adjustmentModal: {
          ...state.adjustmentModal,
          draft: { ...state.adjustmentModal.draft, ...action.payload }
        }
      };
    case "APPLY_ADJUSTMENT":
      return {
        ...state,
        adjustments: {
          ...state.adjustments,
          rowData: [action.payload, ...state.adjustments.rowData.filter((r) => r.profitDetailId != null)]
        },
        adjustmentModal: {
          isOpen: false,
          draft: state.adjustmentModal.draft
        }
      };
    case "UPDATE_ROW_DATA":
      return {
        ...state,
        adjustments: {
          ...state.adjustments,
          rowData: action.payload
        }
      };
    case "INCREMENT_MEMBER_DETAILS_REFRESH":
      return {
        ...state,
        memberDetailsRefreshTrigger: state.memberDetailsRefreshTrigger + 1
      };
    case "RESET_ALL":
      return initialState;
    default:
      return state;
  }
}

const isValidIsoDate = (value: string): boolean => {
  if (!/^\d{4}-\d{2}-\d{2}$/.test(value)) {
    return false;
  }

  const parsed = new Date(`${value}T00:00:00Z`);
  return Number.isFinite(parsed.getTime()) && parsed.toISOString().startsWith(value);
};

export function useProfitSharingAdjustments() {
  const [state, dispatch] = useReducer(adjustmentsReducer, initialState);
  const reduxDispatch = useDispatch();
  const profitYear = useFiscalCloseProfitYear();
  const { addAlert, clearAlerts } = useMissiveAlerts();

  // RTK Query hooks
  const [triggerGet] = useLazyGetProfitSharingAdjustmentsQuery();
  const [saveAdjustments] = useSaveProfitSharingAdjustmentsMutation();

  // Clear messages helper
  const clearMessages = useCallback(() => {
    reduxDispatch(removeMessage(ADJUSTMENTS_MESSAGE_KEY));
  }, [reduxDispatch]);

  const showError = useCallback(
    (message: string) => {
      addAlert({
        id: Date.now(),
        severity: "error",
        message: "Error",
        description: message
      });
    },
    [addAlert]
  );

  // Execute search/load
  const executeSearch = useCallback(
    async (params: ProfitSharingAdjustmentsSearchParams) => {
      clearAlerts();
      clearMessages();

      if (Object.keys(state.adjustments.stagedByRowNumber).length > 0) {
        showError("Discard changes before loading different adjustments.");
        return;
      }

      dispatch({ type: "SEARCH_START", payload: params });

      try {
        const result = await triggerGet({
          profitYear: profitYear,
          badgeNumber: params.badgeNumber,
          getAllRows: params.getAllRows
        }).unwrap();

        // DSMGrid/AG Grid edits mutate row objects; make sure data isn't frozen.
        const copy = (result.rows ?? []).map((r) => ({ ...r }));

        dispatch({
          type: "SEARCH_SUCCESS",
          payload: {
            key: { profitYear: result.profitYear, badgeNumber: result.badgeNumber },
            getAllRows: params.getAllRows,
            demographicId: result.demographicId ?? null,
            rowData: copy
          }
        });
      } catch (_error) {
        dispatch({ type: "SEARCH_FAILURE" });
        showError("Failed to load adjustments. Please try again.");
      }
    },
    [state.adjustments.stagedByRowNumber, profitYear, triggerGet, clearAlerts, clearMessages, showError]
  );

  // Upsert staged row
  const upsertStageForRow = useCallback(
    (row: ProfitSharingAdjustmentRowDto) => {
      const original = state.adjustments.originalByRowNumber[row.rowNumber];
      const isDraftInsertRow = row.profitDetailId == null;

      const changed = original
        ? row.profitCodeId !== original.profitCodeId ||
          row.contribution !== original.contribution ||
          row.earnings !== original.earnings ||
          row.forfeiture !== original.forfeiture ||
          row.activityDate !== original.activityDate ||
          row.comment !== original.comment
        : isDraftInsertRow &&
          (row.contribution !== 0 ||
            row.earnings !== 0 ||
            row.forfeiture !== 0 ||
            row.activityDate != null ||
            (row.comment ?? "") !== "");

      const request = changed
        ? {
            profitDetailId: row.profitDetailId,
            reversedFromProfitDetailId: row.reversedFromProfitDetailId ?? null,
            rowNumber: row.rowNumber,
            profitCodeId: row.profitCodeId,
            contribution: row.contribution,
            earnings: row.earnings,
            forfeiture: row.forfeiture,
            activityDate: row.activityDate,
            comment: row.comment
          }
        : null;

      dispatch({ type: "STAGE_ROW", payload: { rowNumber: row.rowNumber, request } });
    },
    [state.adjustments.originalByRowNumber]
  );

  // Handle cell value changed
  const onCellValueChanged = useCallback(
    (row: ProfitSharingAdjustmentRowDto, field: string, oldValue: unknown) => {
      if (field === "activityDate") {
        const next = row.activityDate;
        if (next && !isValidIsoDate(next)) {
          row.activityDate = (oldValue as string | null | undefined) ?? null;
          showError("Activity Date must be in YYYY-MM-DD format.");
          return false;
        }
      }

      upsertStageForRow(row);
      return true;
    },
    [upsertStageForRow, showError]
  );

  // Discard changes
  const discardChanges = useCallback(() => {
    clearAlerts();
    clearMessages();

    const originalRows = Object.values(state.adjustments.originalByRowNumber);
    if (originalRows.length === 0) {
      return;
    }

    dispatch({ type: "DISCARD_CHANGES", payload: originalRows.map((r) => ({ ...r })) });
  }, [state.adjustments.originalByRowNumber, clearAlerts, clearMessages]);

  // Handle row selection
  const handleRowSelection = useCallback((row: ProfitSharingAdjustmentRowDto | null) => {
    dispatch({ type: "SELECT_ROW", payload: row });
  }, []);

  // Clear selection
  const clearSelection = useCallback(() => {
    dispatch({ type: "SELECT_ROW", payload: null });
  }, []);

  // Open adjustment modal
  const openAdjustModal = useCallback(() => {
    clearAlerts();
    clearMessages();

    if (!state.selection.selectedRow || state.selection.selectedRow.profitDetailId == null) {
      showError("Select an existing row before making an adjustment.");
      return;
    }

    const row = state.selection.selectedRow;
    dispatch({
      type: "OPEN_ADJUSTMENT_MODAL",
      payload: {
        contribution: String(-row.contribution),
        earnings: String(-row.earnings),
        forfeiture: String(-row.forfeiture)
      }
    });
  }, [state.selection.selectedRow, clearAlerts, clearMessages, showError]);

  // Close adjustment modal
  const closeAdjustModal = useCallback(() => {
    dispatch({ type: "CLOSE_ADJUSTMENT_MODAL" });
  }, []);

  // Update adjustment draft
  const updateAdjustmentDraft = useCallback((updates: Partial<AdjustmentDraft>) => {
    dispatch({ type: "UPDATE_ADJUSTMENT_DRAFT", payload: updates });
  }, []);

  // Apply adjustment draft to insert row
  const applyAdjustmentDraft = useCallback(() => {
    clearAlerts();
    clearMessages();

    if (!state.adjustments.loadedKey) {
      showError("Load adjustments before making an adjustment.");
      return;
    }

    const draft = state.adjustmentModal.draft;
    const selectedRow = state.selection.selectedRow;
    const rowData = state.adjustments.rowData;

    const now = new Date();
    const todayIso = now.toISOString().slice(0, 10);

    const existingDraft = rowData.find((r) => r.profitDetailId == null);
    const maxRowNumber = rowData.reduce((max, r) => Math.max(max, r.rowNumber), 0);
    const rowNumber = existingDraft?.rowNumber ?? maxRowNumber + 1;

    const seedRow = rowData.find((r) => r.profitDetailId != null) ?? selectedRow;

    const draftRow: ProfitSharingAdjustmentRowDto = {
      profitDetailId: null,
      hasBeenReversed: false,
      rowNumber,
      profitYear: state.adjustments.loadedKey.profitYear,
      profitYearIteration: 3,
      profitCodeId: seedRow?.profitCodeId ?? 0,
      profitCodeName: seedRow?.profitCodeName ?? "",
      contribution: Number.parseFloat(draft.contribution) || 0,
      earnings: Number.parseFloat(draft.earnings) || 0,
      payment: 0,
      forfeiture: Number.parseFloat(draft.forfeiture) || 0,
      federalTaxes: 0,
      stateTaxes: 0,
      taxCodeId: `${seedRow?.taxCodeId ?? ""}`,
      activityDate: todayIso,
      comment: "ADMINISTRATIVE",
      isEditable: false,
      reversedFromProfitDetailId: selectedRow?.profitDetailId ?? null
    };

    dispatch({ type: "APPLY_ADJUSTMENT", payload: draftRow });
    upsertStageForRow({ ...draftRow, rowNumber: 1 });
  }, [
    state.adjustments.loadedKey,
    state.adjustments.rowData,
    state.adjustmentModal.draft,
    state.selection.selectedRow,
    clearAlerts,
    clearMessages,
    showError,
    upsertStageForRow
  ]);

  // Save changes
  const saveChanges = useCallback(async () => {
    clearAlerts();
    clearMessages();

    if (!state.adjustments.loadedKey) {
      showError("Load adjustments before saving.");
      return;
    }

    const rowsToSave = Object.values(state.adjustments.stagedByRowNumber);

    if (rowsToSave.length === 0) {
      return;
    }

    for (const row of rowsToSave) {
      if (row.activityDate && !isValidIsoDate(row.activityDate)) {
        showError("Activity Date must be in YYYY-MM-DD format.");
        return;
      }
    }

    dispatch({ type: "SET_SAVING", payload: true });

    try {
      await saveAdjustments({ ...state.adjustments.loadedKey, rows: rowsToSave }).unwrap();

      dispatch({ type: "CLEAR_STAGED" });

      // Refresh to ensure server-calculated fields stay in sync.
      const result = await triggerGet({
        ...state.adjustments.loadedKey,
        getAllRows: state.adjustments.loadedGetAllRows
      }).unwrap();

      const copy = (result.rows ?? []).map((r) => ({ ...r }));

      dispatch({
        type: "SEARCH_SUCCESS",
        payload: {
          key: { profitYear: result.profitYear, badgeNumber: result.badgeNumber },
          getAllRows: state.adjustments.loadedGetAllRows,
          demographicId: result.demographicId ?? null,
          rowData: copy
        }
      });

      // Trigger refresh of member details
      dispatch({ type: "INCREMENT_MEMBER_DETAILS_REFRESH" });

      addAlert({
        id: Date.now(),
        severity: "success",
        message: "Success",
        description: "Changes saved successfully."
      });
    } catch (_error) {
      dispatch({ type: "SET_SAVING", payload: false });
      showError("Failed to save changes. Please try again.");
    }
  }, [
    state.adjustments.loadedKey,
    state.adjustments.loadedGetAllRows,
    state.adjustments.stagedByRowNumber,
    saveAdjustments,
    triggerGet,
    clearAlerts,
    clearMessages,
    showError,
    addAlert
  ]);

  // Reset all
  const resetAll = useCallback(() => {
    clearAlerts();
    clearMessages();
    dispatch({ type: "RESET_ALL" });
  }, [clearAlerts, clearMessages]);

  return {
    // Search state
    isSearching: state.search.isSearching,
    searchParams: state.search.params,

    // Adjustments data state
    loadedKey: state.adjustments.loadedKey,
    demographicId: state.adjustments.demographicId,
    rowData: state.adjustments.rowData,
    isFetching: state.adjustments.isFetching,
    isSaving: state.adjustments.isSaving,
    hasUnsavedChanges: Object.keys(state.adjustments.stagedByRowNumber).length > 0,
    stagedByRowNumber: state.adjustments.stagedByRowNumber,

    // Selection state
    selectedRow: state.selection.selectedRow,

    // Adjustment modal state
    isAdjustModalOpen: state.adjustmentModal.isOpen,
    adjustmentDraft: state.adjustmentModal.draft,

    // Member details refresh
    memberDetailsRefreshTrigger: state.memberDetailsRefreshTrigger,

    // Actions
    executeSearch,
    onCellValueChanged,
    discardChanges,
    handleRowSelection,
    clearSelection,
    openAdjustModal,
    closeAdjustModal,
    updateAdjustmentDraft,
    applyAdjustmentDraft,
    saveChanges,
    resetAll
  };
}
