import { Button, Paper, Stack, Tab, Tabs } from "@mui/material";
import { CellValueChangedEvent } from "ag-grid-community";
import { useMemo } from "react";
import { ApiMessageAlert, DSMGrid, Page } from "smart-ui-library";
import { CAPTIONS, GRID_KEYS } from "../../../constants";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import CreateBankDialog from "./components/CreateBankDialog";
import ManageBankAccounts from "./components/ManageBankAccounts";
import { useManageBanks } from "./hooks/useManageBanks";
import { GetManageBanksColumns } from "./ManageBanksColumns";

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

  const columnDefs = useMemo(
    () => GetManageBanksColumns({ handleDisableBank, setActiveTab }),
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
