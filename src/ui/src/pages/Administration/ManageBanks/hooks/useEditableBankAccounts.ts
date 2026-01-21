import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { BankAccountDto } from "../../../../types/administration/banks";
import { validateAccountNumber, validateRoutingNumber } from "../../../../utils/bankValidation";

interface ValidationErrors {
  routingNumber?: string;
  accountNumber?: string;
}

interface UseEditableBankAccountsReturn {
  originalAccountsById: Record<number, BankAccountDto>;
  stagedAccountsById: Record<number, BankAccountDto>;
  validationErrors: Record<number, ValidationErrors>;
  hasUnsavedChanges: boolean;
  hasValidationErrors: boolean;
  rowData: BankAccountDto[];
  updateAccount: (accountId: number, field: string, value: string) => void;
  resetChanges: () => void;
  getChangedAccounts: () => BankAccountDto[];
  clearValidationErrors: () => void;
  reinitialize: (accounts: BankAccountDto[]) => void;
}

/**
 * Custom hook to manage editable bank accounts with staged edits and validation
 */
export const useEditableBankAccounts = (accounts: BankAccountDto[]): UseEditableBankAccountsReturn => {
  const [originalAccountsById, setOriginalAccountsById] = useState<Record<number, BankAccountDto>>({});
  const [stagedAccountsById, setStagedAccountsById] = useState<Record<number, BankAccountDto>>({});
  const [validationErrors, setValidationErrors] = useState<Record<number, ValidationErrors>>({});
  const prevAccountsRef = useRef<BankAccountDto[]>([]);

  // Initialize maps when data loads
  useEffect(() => {
    // Only update if accounts actually changed (not just re-rendered)
    const accountsChanged =
      accounts.length !== prevAccountsRef.current.length ||
      accounts.some((acc, idx) => acc.id !== prevAccountsRef.current[idx]?.id);

    if (accountsChanged) {
      prevAccountsRef.current = accounts;

      if (accounts.length > 0) {
        const accountsMap = accounts.reduce<Record<number, BankAccountDto>>((acc, account) => {
          acc[account.id] = account;
          return acc;
        }, {});
        setOriginalAccountsById(accountsMap);
        setStagedAccountsById(accountsMap);
      } else {
        setOriginalAccountsById({});
        setStagedAccountsById({});
      }
    }
  }, [accounts]);

  const hasUnsavedChanges = useMemo(() => {
    return Object.keys(stagedAccountsById).some((id) => {
      const staged = stagedAccountsById[Number(id)];
      const original = originalAccountsById[Number(id)];
      if (!original) return false;

      return staged.routingNumber !== original.routingNumber || staged.accountNumber !== original.accountNumber;
    });
  }, [stagedAccountsById, originalAccountsById]);

  const hasValidationErrors = useMemo(() => {
    return Object.keys(validationErrors).some((id) => {
      const errors = validationErrors[Number(id)];
      return errors && (errors.routingNumber || errors.accountNumber);
    });
  }, [validationErrors]);

  const rowData = useMemo(() => {
    return accounts.map((account) => ({
      ...account,
      ...(stagedAccountsById[account.id] || {})
    }));
  }, [accounts, stagedAccountsById]);

  const updateAccount = useCallback((accountId: number, field: string, value: string) => {
    // Validate the changed field
    let error = "";
    if (field === "routingNumber") {
      error = validateRoutingNumber(value);
    } else if (field === "accountNumber") {
      error = validateAccountNumber(value);
    }

    // Update validation errors
    setValidationErrors((prev) => ({
      ...prev,
      [accountId]: {
        ...prev[accountId],
        [field]: error
      }
    }));

    // Update staged data
    setStagedAccountsById((prev) => ({
      ...prev,
      [accountId]: {
        ...prev[accountId],
        [field]: value
      }
    }));
  }, []);

  const resetChanges = useCallback(() => {
    setStagedAccountsById(originalAccountsById);
    setValidationErrors({});
  }, [originalAccountsById]);

  const getChangedAccounts = useCallback(() => {
    return Object.keys(stagedAccountsById)
      .filter((id) => {
        const staged = stagedAccountsById[Number(id)];
        const original = originalAccountsById[Number(id)];
        if (!original) return false;

        return staged.routingNumber !== original.routingNumber || staged.accountNumber !== original.accountNumber;
      })
      .map((id) => stagedAccountsById[Number(id)]);
  }, [stagedAccountsById, originalAccountsById]);

  const clearValidationErrors = useCallback(() => {
    setValidationErrors({});
  }, []);

  const reinitialize = useCallback((accounts: BankAccountDto[]) => {
    const accountsMap = accounts.reduce<Record<number, BankAccountDto>>((acc, account) => {
      acc[account.id] = account;
      return acc;
    }, {});
    setOriginalAccountsById(accountsMap);
    setStagedAccountsById(accountsMap);
    setValidationErrors({});
  }, []);

  return {
    originalAccountsById,
    stagedAccountsById,
    validationErrors,
    hasUnsavedChanges,
    hasValidationErrors,
    rowData,
    updateAccount,
    resetChanges,
    getChangedAccounts,
    clearValidationErrors,
    reinitialize
  };
};
