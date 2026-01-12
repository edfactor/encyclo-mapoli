import { Block, Star, StarBorder } from "@mui/icons-material";
import { Button, IconButton, Stack, Tooltip, Typography } from "@mui/material";
import { ColDef } from "ag-grid-community";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useDispatch } from "react-redux";
import { ApiMessageAlert, DSMGrid } from "smart-ui-library";
import { GRID_KEYS } from "../../../../constants";
import { useUnsavedChangesGuard } from "../../../../hooks/useUnsavedChangesGuard";
import {
    useCreateBankAccountMutation,
    useDisableBankAccountMutation,
    useGetBankAccountsQuery,
    useSetPrimaryBankAccountMutation,
    useUpdateBankAccountMutation
} from "../../../../reduxstore/api/administrationApi";
import { setMessage } from "../../../../reduxstore/slices/messageSlice";
import { BankAccountDto, CreateBankAccountRequest, UpdateBankAccountRequest } from "../../../../types/administration/banks";
import { Messages } from "../../../../utils/messageDictonary";
import CreateBankAccountDialog from "./CreateBankAccountDialog";

type EditableBankAccount = BankAccountDto & { isEditing?: boolean };

interface ManageBankAccountsProps {
    bankId: number | null;
    bankName: string | null;
}

const ManageBankAccounts = ({ bankId, bankName }: ManageBankAccountsProps) => {
    const dispatch = useDispatch();
    const { data: accounts = [], isLoading, refetch } = useGetBankAccountsQuery(bankId!, {
        skip: bankId === null
    });
    const [createAccount] = useCreateBankAccountMutation();
    const [updateAccount] = useUpdateBankAccountMutation();
    const [disableAccount] = useDisableBankAccountMutation();
    const [setPrimary] = useSetPrimaryBankAccountMutation();

    const [originalAccountsById, setOriginalAccountsById] = useState<Record<number, BankAccountDto>>({});
    const [stagedAccountsById, setStagedAccountsById] = useState<Record<number, BankAccountDto>>({});
    const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
    const [isSaving, setIsSaving] = useState(false);
    const prevAccountsRef = useRef<BankAccountDto[]>([]);

    // Initialize maps when data loads
    useEffect(() => {
        // Only update if accounts actually changed (not just re-rendered)
        const accountsChanged = accounts.length !== prevAccountsRef.current.length ||
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

            return (
                staged.routingNumber !== original.routingNumber ||
                staged.accountNumber !== original.accountNumber
            );
        });
    }, [stagedAccountsById, originalAccountsById]);

    useUnsavedChangesGuard(hasUnsavedChanges);

    const rowData: EditableBankAccount[] = useMemo(() => {
        return accounts.map((account): EditableBankAccount => ({
            ...account,
            ...(stagedAccountsById[account.id] || {})
        }));
    }, [accounts, stagedAccountsById]);

    const handleCellValueChanged = useCallback(
        (params: any) => {
            const accountId = params.data.id;
            const field = params.colDef.field;
            const newValue = params.newValue;

            setStagedAccountsById((prev) => ({
                ...prev,
                [accountId]: {
                    ...prev[accountId],
                    [field]: newValue
                }
            }));
        },
        []
    );

    const handleSave = async () => {
        setIsSaving(true);
        try {
            const updates = Object.keys(stagedAccountsById)
                .filter((id) => {
                    const staged = stagedAccountsById[Number(id)];
                    const original = originalAccountsById[Number(id)];
                    if (!original) return false;

                    return (
                        staged.routingNumber !== original.routingNumber ||
                        staged.accountNumber !== original.accountNumber
                    );
                })
                .map((id) => stagedAccountsById[Number(id)]);

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

            // Reset staged state
            const refreshedAccountsMap = accounts.reduce<Record<number, BankAccountDto>>((acc, account) => {
                acc[account.id] = account;
                return acc;
            }, {});
            setOriginalAccountsById(refreshedAccountsMap);
            setStagedAccountsById(refreshedAccountsMap);

            dispatch(setMessage(Messages.BankAccountsSaveSuccess));
        } catch (error) {
            console.error("Error saving bank accounts:", error);
            dispatch(setMessage(Messages.BankAccountsSaveError));
        } finally {
            setIsSaving(false);
        }
    };

    const handleDiscard = () => {
        setStagedAccountsById(originalAccountsById);
    };

    const handleCreateAccount = async (request: CreateBankAccountRequest) => {
        if (bankId === null) return;

        try {
            await createAccount({ ...request, bankId }).unwrap();
            await refetch();
            setIsCreateDialogOpen(false);
            dispatch(setMessage(Messages.BankAccountCreatedSuccess));
        } catch (error) {
            console.error("Error creating bank account:", error);
            dispatch(setMessage(Messages.BankAccountCreateError));
        }
    };

    const handleDisableAccount = async (accountId: number) => {
        try {
            await disableAccount(accountId).unwrap();
            await refetch();
            dispatch(setMessage(Messages.BankAccountDisabledSuccess));
        } catch (error) {
            console.error("Error disabling bank account:", error);
            dispatch(setMessage(Messages.BankAccountDisableError));
        }
    };

    const handleSetPrimary = async (accountId: number) => {
        try {
            await setPrimary(accountId).unwrap();
            await refetch();
            dispatch(setMessage(Messages.BankAccountSetPrimarySuccess));
        } catch (error) {
            console.error("Error setting primary account:", error);
            dispatch(setMessage(Messages.BankAccountSetPrimaryError));
        }
    };

    const columnDefs: ColDef[] = useMemo(
        () => [
            {
                field: "id",
                headerName: "ID",
                width: 80,
                editable: false,
                sortable: true
            } as ColDef,
            {
                field: "routingNumber",
                headerName: "Routing Number",
                width: 150,
                editable: true,
                sortable: true
            },
            {
                field: "accountNumber",
                headerName: "Account Number",
                width: 200,
                editable: true,
                sortable: true,
                valueFormatter: (params) => {
                    const value = params.value as string;
                    if (!value) return "";
                    // Mask all but last 4 digits
                    return value.length > 4 
                        ? "******" + value.slice(-4)
                        : value;
                }
            },
            {
                field: "isPrimary",
                headerName: "Primary",
                width: 100,
                editable: false,
                sortable: true,
                valueFormatter: (params) => (params.value ? "Yes" : "No")
            },
            {
                field: "effectiveDate",
                headerName: "Effective Date",
                width: 150,
                editable: false,
                sortable: true,
                valueFormatter: (params) => params.value ? new Date(params.value).toLocaleDateString() : ""
            },
            {
                field: "discontinuedDate",
                headerName: "Discontinued",
                width: 150,
                editable: false,
                sortable: true,
                valueFormatter: (params) => params.value ? new Date(params.value).toLocaleDateString() : ""
            },
            {
                field: "isDisabled",
                headerName: "Disabled",
                width: 100,
                editable: false,
                sortable: true,
                valueFormatter: (params) => (params.value ? "Yes" : "No")
            },
            {
                headerName: "Actions",
                width: 120,
                editable: false,
                cellRenderer: (params: any) => {
                    const account = params.data as BankAccountDto;
                    return (
                        <Stack direction="row" spacing={0.5}>
                            {!account.isPrimary && !account.isDisabled && (
                                <Tooltip title="Set as Primary">
                                    <IconButton
                                        size="small"
                                        color="primary"
                                        onClick={() => handleSetPrimary(account.id)}
                                    >
                                        <StarBorder fontSize="small" />
                                    </IconButton>
                                </Tooltip>
                            )}
                            {account.isPrimary && (
                                <Tooltip title="Primary Account">
                                    <IconButton size="small" disabled>
                                        <Star fontSize="small" color="primary" />
                                    </IconButton>
                                </Tooltip>
                            )}
                            <Tooltip title="Disable Account">
                                <span>
                                    <IconButton
                                        size="small"
                                        color="error"
                                        disabled={account.isDisabled}
                                        onClick={() => handleDisableAccount(account.id)}
                                    >
                                        <Block fontSize="small" />
                                    </IconButton>
                                </span>
                            </Tooltip>
                        </Stack>
                    );
                }
            }
        ],
        [handleDisableAccount, handleSetPrimary]
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
                    onClick={() => setIsCreateDialogOpen(true)}
                >
                    Add Account
                </Button>
                <Button
                    variant="contained"
                    color="primary"
                    onClick={handleSave}
                    disabled={!hasUnsavedChanges || isSaving}
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
                onClose={() => setIsCreateDialogOpen(false)}
                onCreate={handleCreateAccount}
            />
        </Stack>
    );
};

export default ManageBankAccounts;
