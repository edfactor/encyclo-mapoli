import { Button, Stack, Typography } from "@mui/material";
import { CellValueChangedEvent } from "ag-grid-community";
import { useMemo } from "react";
import { ApiMessageAlert, DSMGrid } from "smart-ui-library";
import { GRID_KEYS } from "../../../../constants";
import { useUnsavedChangesGuard } from "../../../../hooks/useUnsavedChangesGuard";
import CreateBankAccountDialog from "./CreateBankAccountDialog";
import { useManageBankAccounts } from "../hooks/useManageBankAccounts";
import { GetManageBankAccountsColumns } from "./ManageBankAccountsColumns";

interface ManageBankAccountsProps {
    bankId: number | null;
    bankName: string | null;
}

const ManageBankAccounts = ({ bankId, bankName }: ManageBankAccountsProps) => {
    const {
        isLoading,
        isSaving,
        hasUnsavedChanges,
        hasValidationErrors,
        rowData,
        validationErrors,
        isCreateDialogOpen,
        stageAccountEdit,
        handleSave,
        handleDiscard,
        handleCreateAccount,
        handleDisableAccount,
        handleSetPrimary,
        openCreateDialog,
        closeCreateDialog
    } = useManageBankAccounts(bankId);

    useUnsavedChangesGuard(hasUnsavedChanges);

    const handleCellValueChanged = (params: CellValueChangedEvent) => {
        const accountId = params.data.id;
        const field = params.colDef.field as string;
        const newValue = params.newValue;
        stageAccountEdit(accountId, field, newValue);
    };

    const columnDefs = useMemo(
        () => GetManageBankAccountsColumns(validationErrors, handleDisableAccount, handleSetPrimary),
        [validationErrors, handleDisableAccount, handleSetPrimary]
    );

    if (bankId === null) {
        return (
            <Stack spacing={2} sx={{ height: "100%", p: 2 }}>
                <Typography variant="h6" color="text.secondary">
                    Select a bank to manage its accounts
                </Typography>
            </Stack>
        );
    }

    return (
        <Stack spacing={2} sx={{ height: "100%", p: 2 }}>
            <Typography variant="h6">
                Accounts for {bankName}
            </Typography>

            <Stack direction="row" spacing={2}>
                <Button
                    variant="contained"
                    color="primary"
                    onClick={openCreateDialog}
                >
                    Add Account
                </Button>
                <Button
                    variant="contained"
                    color="primary"
                    onClick={handleSave}
                    disabled={!hasUnsavedChanges || hasValidationErrors || isSaving}
                >
                    {isSaving ? "Saving..." : "Save Changes"}
                </Button>
                <Button
                    variant="outlined"
                    color="secondary"
                    onClick={handleDiscard}
                    disabled={!hasUnsavedChanges || isSaving}
                >
                    Discard Changes
                </Button>
            </Stack>

            <ApiMessageAlert commonKey="BankAccountsSave" />

            <DSMGrid
                preferenceKey={GRID_KEYS.MANAGE_BANK_ACCOUNTS}
                isLoading={isLoading || isSaving}
                providedOptions={{
                    rowData,
                    columnDefs,
                    onCellValueChanged: handleCellValueChanged
                }}
            />

            <CreateBankAccountDialog
                open={isCreateDialogOpen}
                onClose={closeCreateDialog}
                onCreate={handleCreateAccount}
                bankId={bankId!}
            />
        </Stack>
    );
};

export default ManageBankAccounts;
