import { AccountBalance, Block } from "@mui/icons-material";
import { Button, IconButton, Paper, Stack, Tab, Tabs, Tooltip } from "@mui/material";
import { CellValueChangedEvent, ColDef, ValueParserParams } from "ag-grid-community";
import { useMemo } from "react";
import { ApiMessageAlert, DSMGrid, Page } from "smart-ui-library";
import { CAPTIONS, GRID_KEYS } from "../../../constants";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import { BankDto } from "../../../types/administration/banks";
import CreateBankDialog from "./components/CreateBankDialog";
import ManageBankAccounts from "./components/ManageBankAccounts";
import { useManageBanks } from "./hooks/useManageBanks";

const ManageBanks = () => {
  const {
    isLoading,
    isSaving,
    hasUnsavedChanges,
    rowData,
    activeTab,
    selectedBank,
    isCreateDialogOpen,
    stageBankEdit,
    handleSave,
    handleDiscard,
    handleCreateBank,
    handleDisableBank,
    setActiveTab,
    openCreateDialog,
    closeCreateDialog
  } = useManageBanks();

  useUnsavedChangesGuard(hasUnsavedChanges);

  const handleCellValueChanged = (params: CellValueChangedEvent) => {
    const bankId = params.data.id;
    const field = params.colDef.field as string;
    const newValue = params.newValue;
    stageBankEdit(bankId, field, newValue);
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
                    setActiveTab(1, { id: bank.id, name: bank.name });
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
    [handleDisableBank, setActiveTab]
  );

  return (
    <Page label={CAPTIONS.MANAGE_BANKS}>
      <Stack spacing={2} sx={{ height: "100%" }}>
        <Paper sx={{ borderBottom: 1, borderColor: "divider" }}>
          <Tabs value={activeTab} onChange={(_, newValue) => setActiveTab(newValue, null)}>
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
                onClick={openCreateDialog}
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
          onClose={closeCreateDialog}
          onCreate={handleCreateBank}
        />
      </Stack>
    </Page>
  );
};

export default ManageBanks;
