import { AccountBalance, Block } from "@mui/icons-material";
import { Button, IconButton, Paper, Stack, Tab, Tabs, Tooltip } from "@mui/material";
import { CellValueChangedEvent, ColDef, ValueParserParams } from "ag-grid-community";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useDispatch } from "react-redux";
import { ApiMessageAlert, DSMGrid, Page } from "smart-ui-library";
import { CAPTIONS, GRID_KEYS } from "../../../constants";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import {
    useCreateBankMutation,
    useDisableBankMutation,
    useGetAllBanksQuery,
    useUpdateBankMutation
} from "../../../reduxstore/api/administrationApi";
import { setMessage } from "../../../reduxstore/slices/messageSlice";
import { BankDto, CreateBankRequest, UpdateBankRequest } from "../../../types/administration/banks";
import { Messages } from "../../../utils/messageDictonary";
import CreateBankDialog from "./components/CreateBankDialog";
import ManageBankAccounts from "./components/ManageBankAccounts";

type EditableBank = BankDto & { isEditing?: boolean };

const ManageBanks = () => {
  const dispatch = useDispatch();
  const { data: banks = [], isLoading, refetch } = useGetAllBanksQuery();
  const [createBank] = useCreateBankMutation();
  const [updateBank] = useUpdateBankMutation();
  const [disableBank] = useDisableBankMutation();

  const [originalBanksById, setOriginalBanksById] = useState<Record<number, BankDto>>({});
  const [stagedBanksById, setStagedBanksById] = useState<Record<number, BankDto>>({});
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [activeTab, setActiveTab] = useState(0);
  const [selectedBank, setSelectedBank] = useState<{ id: number; name: string } | null>(null);
  const prevBanksRef = useRef<BankDto[]>([]);

  // Initialize maps when data loads
  useEffect(() => {
    // Only update if banks actually changed (not just re-rendered)
    const banksChanged = banks.length !== prevBanksRef.current.length ||
      banks.some((bank, idx) => bank.id !== prevBanksRef.current[idx]?.id);
    
    if (banksChanged) {
      prevBanksRef.current = banks;
      
      if (banks.length > 0) {
        const banksMap = banks.reduce<Record<number, BankDto>>((acc, bank) => {
          acc[bank.id] = bank;
          return acc;
        }, {});
        setOriginalBanksById(banksMap);
        setStagedBanksById(banksMap);
      }
    }
  }, [banks]);

  const hasUnsavedChanges = useMemo(() => {
    return Object.keys(stagedBanksById).some((id) => {
      const staged = stagedBanksById[Number(id)];
      const original = originalBanksById[Number(id)];
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
  }, [stagedBanksById, originalBanksById]);

  useUnsavedChangesGuard(hasUnsavedChanges);

  const rowData: EditableBank[] = useMemo(() => {
    return banks.map((bank): EditableBank => ({
      ...bank,
      ...(stagedBanksById[bank.id] || {})
    }));
  }, [banks, stagedBanksById]);

  const handleCellValueChanged = useCallback(
    (params: CellValueChangedEvent) => {
      const bankId = params.data.id;
      const field = params.colDef.field as string;
      const newValue = params.newValue;

      setStagedBanksById((prev) => ({
        ...prev,
        [bankId]: {
          ...prev[bankId],
          [field]: newValue
        }
      }));
    },
    []
  );

  const handleSave = async () => {
    setIsSaving(true);
    try {
      const updates = Object.keys(stagedBanksById)
        .filter((id) => {
          const staged = stagedBanksById[Number(id)];
          const original = originalBanksById[Number(id)];
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
        .map((id) => stagedBanksById[Number(id)]);

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
      
      // Reset staged state
      const refreshedBanksMap = banks.reduce<Record<number, BankDto>>((acc, bank) => {
        acc[bank.id] = bank;
        return acc;
      }, {});
      setOriginalBanksById(refreshedBanksMap);
      setStagedBanksById(refreshedBanksMap);

      dispatch(setMessage(Messages.BanksSaveSuccess));
    } catch (error) {
      console.error("Error saving banks:", error);
      dispatch(setMessage(Messages.BanksSaveError));
    } finally {
      setIsSaving(false);
    }
  };

  const handleDiscard = () => {
    setStagedBanksById(originalBanksById);
  };

  const handleCreateBank = async (request: CreateBankRequest) => {
    try {
      await createBank(request).unwrap();
      await refetch();
      setIsCreateDialogOpen(false);
      dispatch(setMessage(Messages.BankCreatedSuccess));
    } catch (error) {
      console.error("Error creating bank:", error);
      dispatch(setMessage(Messages.BankCreateError));
    }
  };

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
        field: "name",
        headerName: "Bank Name",
        width: 200,
        editable: true,
        sortable: true
      },
      {
        field: "officeType",
        headerName: "Office Type",
        width: 150,
        editable: true,
        sortable: true
      },
      {
        field: "city",
        headerName: "City",
        width: 150,
        editable: true,
        sortable: true
      },
      {
        field: "state",
        headerName: "State",
        width: 100,
        editable: true,
        sortable: true,
        valueParser: (params: ValueParserParams) => {
          const value = params.newValue?.trim().toUpperCase();
          return value && value.length <= 2 ? value : params.oldValue;
        }
      },
      {
        field: "phone",
        headerName: "Phone",
        width: 150,
        editable: true,
        sortable: true
      },
      {
        field: "status",
        headerName: "Status",
        width: 150,
        editable: true,
        sortable: true
      },
      {
        field: "accountCount",
        headerName: "Accounts",
        width: 100,
        editable: false,
        sortable: true
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
        cellRenderer: (params: { data: BankDto }) => {
          const bank = params.data;
          return (
            <Stack direction="row" spacing={0.5}>
              <Tooltip title="Manage Accounts">
                <IconButton
                  size="small"
                  color="primary"
                  onClick={() => {
                    setSelectedBank({ id: bank.id, name: bank.name });
                    setActiveTab(1);
                  }}
                >
                  <AccountBalance fontSize="small" />
                </IconButton>
              </Tooltip>
              <Tooltip title="Disable Bank">
                <span>
                  <IconButton
                    size="small"
                    color="error"
                    disabled={bank.isDisabled}
                    onClick={() => handleDisableBank(bank.id)}
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
    [handleDisableBank]
  );

  return (
    <Page label={CAPTIONS.MANAGE_BANKS}>
      <Stack spacing={2} sx={{ height: "100%" }}>
        <Paper sx={{ borderBottom: 1, borderColor: "divider" }}>
          <Tabs value={activeTab} onChange={(_, newValue) => setActiveTab(newValue)}>
            <Tab label="Banks" />
            <Tab label="Bank Accounts" disabled={selectedBank === null} />
          </Tabs>
        </Paper>

        {activeTab === 0 && (
          <Stack spacing={2} sx={{ height: "100%" }}>
            <Stack direction="row" spacing={2}>
              <Button
                variant="contained"
                color="primary"
                onClick={() => setIsCreateDialogOpen(true)}
              >
                Create New Bank
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

            <ApiMessageAlert commonKey="BanksSave" />

            <DSMGrid
              preferenceKey={GRID_KEYS.MANAGE_BANKS}
              isLoading={isLoading || isSaving}
              providedOptions={{
                rowData,
                columnDefs,
                onCellValueChanged: handleCellValueChanged
              }}
            />
          </Stack>
        )}

        {activeTab === 1 && (
          <ManageBankAccounts 
            bankId={selectedBank?.id ?? null} 
            bankName={selectedBank?.name ?? null}
          />
        )}

        <CreateBankDialog
          open={isCreateDialogOpen}
          onClose={() => setIsCreateDialogOpen(false)}
          onCreate={handleCreateBank}
        />
      </Stack>
    </Page>
  );
};

export default ManageBanks;
