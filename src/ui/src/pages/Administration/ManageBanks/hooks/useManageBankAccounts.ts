import { useCallback, useEffect, useMemo, useReducer, useRef } from "react";
import { useDispatch } from "react-redux";
import {
    useCreateBankAccountMutation,
    useDisableBankAccountMutation,
    useGetBankAccountsQuery,
    useSetPrimaryBankAccountMutation,
    useUpdateBankAccountMutation
} from "../../../../reduxstore/api/administrationApi";
import { setMessage } from "../../../../reduxstore/slices/messageSlice";
import { BankAccountDto, CreateBankAccountRequest, UpdateBankAccountRequest } from "../../../../types/administration/banks";
import { validateAccountNumber, validateRoutingNumber } from "../../../../utils/bankValidation";
import { Messages } from "../../../../utils/messageDictonary";

interface ManageBankAccountsState {
  editState: {
    originalAccountsById: Record<number, BankAccountDto>;
    stagedAccountsById: Record<number, BankAccountDto>;
    validationErrors: Record<number, { routingNumber?: string; accountNumber?: string }>;
    isSaving: boolean;
  };
  dialog: {
    isCreateDialogOpen: boolean;
  };
}

type ManageBankAccountsAction =
  | { type: "INITIALIZE_ACCOUNTS"; payload: BankAccountDto[] }
  | { type: "STAGE_ACCOUNT_EDIT"; payload: { id: number; field: string; value: unknown } }
  | { type: "SET_VALIDATION_ERROR"; payload: { id: number; field: string; error: string } }
  | { type: "CLEAR_VALIDATION_ERRORS" }
  | { type: "SAVE_START" }
  | { type: "SAVE_SUCCESS"; payload: BankAccountDto[] }
  | { type: "SAVE_FAILURE" }
  | { type: "DISCARD_CHANGES" }
  | { type: "OPEN_CREATE_DIALOG" }
  | { type: "CLOSE_CREATE_DIALOG" };

const initialState: ManageBankAccountsState = {
  editState: {
    originalAccountsById: {},
    stagedAccountsById: {},
    validationErrors: {},
    isSaving: false
  },
  dialog: {
    isCreateDialogOpen: false
  }
};

function manageBankAccountsReducer(
  state: ManageBankAccountsState,
  action: ManageBankAccountsAction
): ManageBankAccountsState {
  switch (action.type) {
    case "INITIALIZE_ACCOUNTS": {
      const accountsMap = action.payload.reduce<Record<number, BankAccountDto>>((acc, account) => {
        acc[account.id] = account;
        return acc;
      }, {});
      return {
        ...state,
        editState: {
          ...state.editState,
          originalAccountsById: accountsMap,
          stagedAccountsById: accountsMap,
          validationErrors: {}
        }
      };
    }

    case "STAGE_ACCOUNT_EDIT": {
      const { id, field, value } = action.payload;
      return {
        ...state,
        editState: {
          ...state.editState,
          stagedAccountsById: {
            ...state.editState.stagedAccountsById,
            [id]: {
              ...state.editState.stagedAccountsById[id],
              [field]: value
            }
          }
        }
      };
    }

    case "SET_VALIDATION_ERROR": {
      const { id, field, error } = action.payload;
      return {
        ...state,
        editState: {
          ...state.editState,
          validationErrors: {
            ...state.editState.validationErrors,
            [id]: {
              ...state.editState.validationErrors[id],
              [field]: error
            }
          }
        }
      };
    }

    case "CLEAR_VALIDATION_ERRORS":
      return {
        ...state,
        editState: {
          ...state.editState,
          validationErrors: {}
        }
      };

    case "SAVE_START":
      return {
        ...state,
        editState: {
          ...state.editState,
          isSaving: true
        }
      };

    case "SAVE_SUCCESS": {
      const refreshedAccountsMap = action.payload.reduce<Record<number, BankAccountDto>>((acc, account) => {
        acc[account.id] = account;
        return acc;
      }, {});
      return {
        ...state,
        editState: {
          originalAccountsById: refreshedAccountsMap,
          stagedAccountsById: refreshedAccountsMap,
          validationErrors: {},
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
          stagedAccountsById: state.editState.originalAccountsById,
          validationErrors: {}
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

export function useManageBankAccounts(bankId: number | null) {
  const dispatch = useDispatch();
  const [state, dispatchAction] = useReducer(manageBankAccountsReducer, initialState);
  const { data: accounts = [], isLoading, refetch } = useGetBankAccountsQuery(bankId!, {
    skip: bankId === null
  });
  const [createAccount] = useCreateBankAccountMutation();
  const [updateAccount] = useUpdateBankAccountMutation();
  const [disableAccount] = useDisableBankAccountMutation();
  const [setPrimary] = useSetPrimaryBankAccountMutation();
  const prevAccountsRef = useRef<BankAccountDto[]>([]);

  // Initialize maps when data loads
  useEffect(() => {
    // Only update if accounts actually changed (not just re-rendered)
    const accountsChanged = accounts.length !== prevAccountsRef.current.length ||
      accounts.some((acc, idx) => acc.id !== prevAccountsRef.current[idx]?.id);
    
    if (accountsChanged) {
      prevAccountsRef.current = accounts;
      
      if (accounts.length > 0) {
        dispatchAction({ type: "INITIALIZE_ACCOUNTS", payload: accounts });
      } else {
        dispatchAction({ type: "INITIALIZE_ACCOUNTS", payload: [] });
      }
    }
  }, [accounts]);

  // Selectors
  const hasUnsavedChanges = useMemo(() => {
    return Object.keys(state.editState.stagedAccountsById).some((id) => {
      const staged = state.editState.stagedAccountsById[Number(id)];
      const original = state.editState.originalAccountsById[Number(id)];
      if (!original) return false;

      return (
        staged.routingNumber !== original.routingNumber ||
        staged.accountNumber !== original.accountNumber
      );
    });
  }, [state.editState.stagedAccountsById, state.editState.originalAccountsById]);

  const hasValidationErrors = useMemo(() => {
    return Object.keys(state.editState.validationErrors).some((id) => {
      const errors = state.editState.validationErrors[Number(id)];
      return errors && (errors.routingNumber || errors.accountNumber);
    });
  }, [state.editState.validationErrors]);

  const rowData = useMemo(() => {
    return accounts.map((account) => ({
      ...account,
      ...(state.editState.stagedAccountsById[account.id] || {})
    }));
  }, [accounts, state.editState.stagedAccountsById]);

  // Actions
  const stageAccountEdit = useCallback((id: number, field: string, value: string) => {
    // Validate the changed field
    let error = "";
    if (field === "routingNumber") {
      error = validateRoutingNumber(value);
    } else if (field === "accountNumber") {
      error = validateAccountNumber(value);
    }

    // Update validation errors
    dispatchAction({ type: "SET_VALIDATION_ERROR", payload: { id, field, error } });

    // Update staged data
    dispatchAction({ type: "STAGE_ACCOUNT_EDIT", payload: { id, field, value } });
  }, []);

  const handleSave = useCallback(async () => {
    // Check for validation errors
    const hasErrors = Object.keys(state.editState.stagedAccountsById).some((id) => {
      const errors = state.editState.validationErrors[Number(id)];
      return errors && (errors.routingNumber || errors.accountNumber);
    });

    if (hasErrors) {
      dispatch(setMessage({
        key: "BankAccountsSave",
        message: {
          type: "error",
          title: "Validation Error",
          message: "Please fix validation errors before saving."
        }
      }));
      return;
    }

    dispatchAction({ type: "SAVE_START" });
    try {
      const updates = Object.keys(state.editState.stagedAccountsById)
        .filter((id) => {
          const staged = state.editState.stagedAccountsById[Number(id)];
          const original = state.editState.originalAccountsById[Number(id)];
          if (!original) return false;

          return (
            staged.routingNumber !== original.routingNumber ||
            staged.accountNumber !== original.accountNumber
          );
        })
        .map((id) => state.editState.stagedAccountsById[Number(id)]);

      await Promise.all(
        updates.map((account) =>
          updateAccount({
            id: account.id,
            routingNumber: account.routingNumber,
            accountNumber: account.accountNumber
          } as UpdateBankAccountRequest)
        )
      );

      await refetch();
      dispatchAction({ type: "SAVE_SUCCESS", payload: accounts });
      dispatch(setMessage(Messages.BankAccountsSaveSuccess));
    } catch (error) {
      console.error("Error saving bank accounts:", error);
      dispatchAction({ type: "SAVE_FAILURE" });
      dispatch(setMessage(Messages.BankAccountsSaveError));
    }
  }, [state.editState.stagedAccountsById, state.editState.originalAccountsById, state.editState.validationErrors, updateAccount, refetch, accounts, dispatch]);

  const handleDiscard = useCallback(() => {
    dispatchAction({ type: "DISCARD_CHANGES" });
  }, []);

  const handleCreateAccount = useCallback(async (request: CreateBankAccountRequest) => {
    try {
      await createAccount(request).unwrap();
      await refetch();
      dispatchAction({ type: "CLOSE_CREATE_DIALOG" });
      dispatch(setMessage(Messages.BankAccountCreatedSuccess));
    } catch (error) {
      console.error("Error creating bank account:", error);
      dispatch(setMessage(Messages.BankAccountCreateError));
    }
  }, [createAccount, refetch, dispatch]);

  const handleDisableAccount = useCallback(async (accountId: number) => {
    try {
      await disableAccount(accountId).unwrap();
      await refetch();
      dispatch(setMessage(Messages.BankAccountDisabledSuccess));
    } catch (error) {
      console.error("Error disabling bank account:", error);
      dispatch(setMessage(Messages.BankAccountDisableError));
    }
  }, [disableAccount, refetch, dispatch]);

  const handleSetPrimary = useCallback(async (accountId: number) => {
    try {
      await setPrimary(accountId).unwrap();
      await refetch();
      dispatch(setMessage(Messages.BankAccountSetPrimarySuccess));
    } catch (error) {
      console.error("Error setting primary account:", error);
      dispatch(setMessage(Messages.BankAccountSetPrimaryError));
    }
  }, [setPrimary, refetch, dispatch]);

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
    hasValidationErrors,
    rowData,
    validationErrors: state.editState.validationErrors,
    isCreateDialogOpen: state.dialog.isCreateDialogOpen,

    // Actions
    stageAccountEdit,
    handleSave,
    handleDiscard,
    handleCreateAccount,
    handleDisableAccount,
    handleSetPrimary,
    openCreateDialog,
    closeCreateDialog
  };
}
