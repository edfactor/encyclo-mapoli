import { useCallback, useEffect, useMemo, useReducer, useRef } from "react";
import { useDispatch } from "react-redux";
import {
    useCreateBankMutation,
    useDisableBankMutation,
    useGetAllBanksQuery,
    useUpdateBankMutation
} from "../../../../reduxstore/api/administrationApi";
import { setMessage } from "../../../../reduxstore/slices/messageSlice";
import { BankDto, CreateBankRequest, UpdateBankRequest } from "../../../../types/administration/banks";
import { Messages } from "../../../../utils/messageDictonary";

interface ManageBanksState {
  editState: {
    originalBanksById: Record<number, BankDto>;
    stagedBanksById: Record<number, BankDto>;
    isSaving: boolean;
  };
  tabState: {
    activeTab: number;
    selectedBank: { id: number; name: string } | null;
  };
  dialog: {
    isCreateDialogOpen: boolean;
  };
}

type ManageBanksAction =
  | { type: "INITIALIZE_BANKS"; payload: BankDto[] }
  | { type: "STAGE_BANK_EDIT"; payload: { id: number; field: string; value: unknown } }
  | { type: "SAVE_START" }
  | { type: "SAVE_SUCCESS"; payload: BankDto[] }
  | { type: "SAVE_FAILURE" }
  | { type: "DISCARD_CHANGES" }
  | { type: "SET_ACTIVE_TAB"; payload: { tab: number; selectedBank?: { id: number; name: string } | null } }
  | { type: "OPEN_CREATE_DIALOG" }
  | { type: "CLOSE_CREATE_DIALOG" };

const initialState: ManageBanksState = {
  editState: {
    originalBanksById: {},
    stagedBanksById: {},
    isSaving: false
  },
  tabState: {
    activeTab: 0,
    selectedBank: null
  },
  dialog: {
    isCreateDialogOpen: false
  }
};

function manageBanksReducer(state: ManageBanksState, action: ManageBanksAction): ManageBanksState {
  switch (action.type) {
    case "INITIALIZE_BANKS": {
      const banksMap = action.payload.reduce<Record<number, BankDto>>((acc, bank) => {
        acc[bank.id] = bank;
        return acc;
      }, {});
      return {
        ...state,
        editState: {
          ...state.editState,
          originalBanksById: banksMap,
          stagedBanksById: banksMap
        }
      };
    }

    case "STAGE_BANK_EDIT": {
      const { id, field, value } = action.payload;
      return {
        ...state,
        editState: {
          ...state.editState,
          stagedBanksById: {
            ...state.editState.stagedBanksById,
            [id]: {
              ...state.editState.stagedBanksById[id],
              [field]: value
            }
          }
        }
      };
    }

    case "SAVE_START":
      return {
        ...state,
        editState: {
          ...state.editState,
          isSaving: true
        }
      };

    case "SAVE_SUCCESS": {
      const refreshedBanksMap = action.payload.reduce<Record<number, BankDto>>((acc, bank) => {
        acc[bank.id] = bank;
        return acc;
      }, {});
      return {
        ...state,
        editState: {
          originalBanksById: refreshedBanksMap,
          stagedBanksById: refreshedBanksMap,
          isSaving: false
        }
      };
    }

    case "SAVE_FAILURE":
      return {
        ...state,
        editState: {
          ...state.editState,
          isSaving: false
        }
      };

    case "DISCARD_CHANGES":
      return {
        ...state,
        editState: {
          ...state.editState,
          stagedBanksById: state.editState.originalBanksById
        }
      };

    case "SET_ACTIVE_TAB":
      return {
        ...state,
        tabState: {
          activeTab: action.payload.tab,
          selectedBank: action.payload.selectedBank ?? state.tabState.selectedBank
        }
      };

    case "OPEN_CREATE_DIALOG":
      return {
        ...state,
        dialog: {
          ...state.dialog,
          isCreateDialogOpen: true
        }
      };

    case "CLOSE_CREATE_DIALOG":
      return {
        ...state,
        dialog: {
          ...state.dialog,
          isCreateDialogOpen: false
        }
      };

    default:
      return state;
  }
}

export function useManageBanks() {
  const dispatch = useDispatch();
  const [state, dispatchAction] = useReducer(manageBanksReducer, initialState);
  const { data: banks = [], isLoading, refetch } = useGetAllBanksQuery();
  const [createBank] = useCreateBankMutation();
  const [updateBank] = useUpdateBankMutation();
  const [disableBank] = useDisableBankMutation();
  const prevBanksRef = useRef<BankDto[]>([]);

  // Initialize maps when data loads
  useEffect(() => {
    // Only update if banks actually changed (not just re-rendered)
    const banksChanged = banks.length !== prevBanksRef.current.length ||
      banks.some((bank, idx) => bank.id !== prevBanksRef.current[idx]?.id);
    
    if (banksChanged) {
      prevBanksRef.current = banks;
      
      if (banks.length > 0) {
        dispatchAction({ type: "INITIALIZE_BANKS", payload: banks });
      }
    }
  }, [banks]);

  // Selectors
  const hasUnsavedChanges = useMemo(() => {
    return Object.keys(state.editState.stagedBanksById).some((id) => {
      const staged = state.editState.stagedBanksById[Number(id)];
      const original = state.editState.originalBanksById[Number(id)];
      if (!original) return false;
      
      return (
        staged.name !== original.name ||
        staged.officeType !== original.officeType ||
        staged.city !== original.city ||
        staged.state !== original.state ||
        staged.phone !== original.phone ||
        staged.status !== original.status
      );
    });
  }, [state.editState.stagedBanksById, state.editState.originalBanksById]);

  const rowData = useMemo(() => {
    return banks.map((bank) => ({
      ...bank,
      ...(state.editState.stagedBanksById[bank.id] || {})
    }));
  }, [banks, state.editState.stagedBanksById]);

  // Actions
  const stageBankEdit = useCallback((id: number, field: string, value: unknown) => {
    dispatchAction({ type: "STAGE_BANK_EDIT", payload: { id, field, value } });
  }, []);

  const handleSave = useCallback(async () => {
    dispatchAction({ type: "SAVE_START" });
    try {
      const updates = Object.keys(state.editState.stagedBanksById)
        .filter((id) => {
          const staged = state.editState.stagedBanksById[Number(id)];
          const original = state.editState.originalBanksById[Number(id)];
          if (!original) return false;
          
          return (
            staged.name !== original.name ||
            staged.officeType !== original.officeType ||
            staged.city !== original.city ||
            staged.state !== original.state ||
            staged.phone !== original.phone ||
            staged.status !== original.status
          );
        })
        .map((id) => state.editState.stagedBanksById[Number(id)]);

      await Promise.all(
        updates.map((bank) =>
          updateBank({
            id: bank.id,
            name: bank.name,
            officeType: bank.officeType,
            city: bank.city,
            state: bank.state,
            phone: bank.phone,
            status: bank.status
          } as UpdateBankRequest)
        )
      );

      await refetch();
      dispatchAction({ type: "SAVE_SUCCESS", payload: banks });
      dispatch(setMessage(Messages.BanksSaveSuccess));
    } catch (error) {
      console.error("Error saving banks:", error);
      dispatchAction({ type: "SAVE_FAILURE" });
      dispatch(setMessage(Messages.BanksSaveError));
    }
  }, [state.editState.stagedBanksById, state.editState.originalBanksById, updateBank, refetch, banks, dispatch]);

  const handleDiscard = useCallback(() => {
    dispatchAction({ type: "DISCARD_CHANGES" });
  }, []);

  const handleCreateBank = useCallback(async (request: CreateBankRequest) => {
    try {
      await createBank(request).unwrap();
      await refetch();
      dispatchAction({ type: "CLOSE_CREATE_DIALOG" });
      dispatch(setMessage(Messages.BankCreatedSuccess));
    } catch (error) {
      console.error("Error creating bank:", error);
      dispatch(setMessage(Messages.BankCreateError));
    }
  }, [createBank, refetch, dispatch]);

  const handleDisableBank = useCallback(async (bankId: number) => {
    try {
      await disableBank(bankId).unwrap();
      await refetch();
      dispatch(setMessage(Messages.BankDisabledSuccess));
    } catch (error) {
      console.error("Error disabling bank:", error);
      dispatch(setMessage(Messages.BankDisableError));
    }
  }, [disableBank, refetch, dispatch]);

  const setActiveTab = useCallback((tab: number, selectedBank?: { id: number; name: string } | null) => {
    dispatchAction({ type: "SET_ACTIVE_TAB", payload: { tab, selectedBank } });
  }, []);

  const openCreateDialog = useCallback(() => {
    dispatchAction({ type: "OPEN_CREATE_DIALOG" });
  }, []);

  const closeCreateDialog = useCallback(() => {
    dispatchAction({ type: "CLOSE_CREATE_DIALOG" });
  }, []);

  return {
    // State
    isLoading,
    isSaving: state.editState.isSaving,
    hasUnsavedChanges,
    rowData,
    activeTab: state.tabState.activeTab,
    selectedBank: state.tabState.selectedBank,
    isCreateDialogOpen: state.dialog.isCreateDialogOpen,

    // Actions
    stageBankEdit,
    handleSave,
    handleDiscard,
    handleCreateBank,
    handleDisableBank,
    setActiveTab,
    openCreateDialog,
    closeCreateDialog
  };
}
