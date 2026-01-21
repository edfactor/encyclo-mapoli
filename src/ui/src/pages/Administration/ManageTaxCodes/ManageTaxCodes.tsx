import { Box, Button, Divider, Grid, Typography } from "@mui/material";
import { CellValueChangedEvent, ColDef } from "ag-grid-community";
import { useCallback, useEffect, useMemo, useState } from "react";
import { useDispatch } from "react-redux";
import { ApiMessageAlert, DSMGrid, Page } from "smart-ui-library";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import { CAPTIONS, GRID_KEYS } from "../../../constants";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import {
  useDeleteTaxCodeMutation,
  useGetTaxCodesQuery,
  useUpdateTaxCodeMutation
} from "../../../reduxstore/api/administrationApi";
import { setMessage } from "../../../reduxstore/slices/messageSlice";
import { TaxCodeAdminDto, UpdateTaxCodeRequest } from "../../../types";
import { createNameColumn, createTaxCodeColumn, createYesOrNoColumn } from "../../../utils/gridColumnFactory";
import { Messages } from "../../../utils/messageDictonary";
import { AddTaxCodeDialog } from "./AddTaxCodeDialog";
import { DeleteTaxCodeDialog } from "./DeleteTaxCodeDialog";

const ManageTaxCodes = () => {
  const dispatch = useDispatch();
  const { data, isFetching, refetch } = useGetTaxCodesQuery();
  const [updateTaxCode, { isLoading: isSaving }] = useUpdateTaxCodeMutation();
  const [deleteTaxCode, { isLoading: isDeleting }] = useDeleteTaxCodeMutation();

  const [rowData, setRowData] = useState<TaxCodeAdminDto[]>([]);
  const [originalById, setOriginalById] = useState<Record<string, TaxCodeAdminDto>>({});
  const [stagedUpdatesById, setStagedUpdatesById] = useState<Record<string, Partial<TaxCodeAdminDto>>>({});
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isAddDialogOpen, setIsAddDialogOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<TaxCodeAdminDto | null>(null);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);

  const hasUnsavedChanges = Object.keys(stagedUpdatesById).length > 0;
  useUnsavedChangesGuard(hasUnsavedChanges);

  useEffect(() => {
    if (!data) return;

    setRowData(data.map((r) => ({ ...r })));
    setOriginalById(
      data.reduce<Record<string, TaxCodeAdminDto>>((acc, cur) => {
        acc[cur.id] = cur;
        return acc;
      }, {})
    );

    setStagedUpdatesById({});
    setErrorMessage(null);
  }, [data]);

  const openDeleteDialog = useCallback((taxCode: TaxCodeAdminDto) => {
    setDeleteTarget(taxCode);
    setIsDeleteDialogOpen(true);
  }, []);

  const closeDeleteDialog = useCallback(() => {
    setIsDeleteDialogOpen(false);
    setDeleteTarget(null);
  }, []);

  const updateStagedChange = useCallback(
    (id: string, update: Partial<TaxCodeAdminDto>) => {
      setStagedUpdatesById((prev) => {
        const original = originalById[id];
        if (!original) return prev;

        const next = { ...prev };
        const mergedUpdate = { ...prev[id], ...update };
        const candidate = { ...original, ...mergedUpdate };

        const hasChanges =
          candidate.name !== original.name ||
          candidate.isAvailableForDistribution !== original.isAvailableForDistribution ||
          candidate.isAvailableForForfeiture !== original.isAvailableForForfeiture ||
          candidate.isProtected !== original.isProtected;

        if (hasChanges) {
          next[id] = mergedUpdate;
        } else {
          delete next[id];
        }

        return next;
      });
    },
    [originalById]
  );

  const onCellValueChanged = (event: CellValueChangedEvent) => {
    const row = event.data as TaxCodeAdminDto | undefined;
    const id = row?.id;
    const field = event.colDef.field as keyof TaxCodeAdminDto | undefined;

    if (!id || !field) return;

    const original = originalById[id];
    if (!original) return;

    if (original.isProtected && field !== "isProtected") {
      setErrorMessage("Protected tax codes cannot be modified.");
      event.node.setDataValue(field, original[field]);
      return;
    }

    if (field === "name") {
      const newName = String(event.newValue ?? "").trim();
      if (!newName) {
        setErrorMessage("Name cannot be empty.");
        event.node.setDataValue("name", original.name);
        return;
      }

      if (newName.length > 128) {
        setErrorMessage("Name must be 128 characters or less.");
        event.node.setDataValue("name", original.name);
        return;
      }

      setErrorMessage(null);
      event.node.setDataValue("name", newName);
      updateStagedChange(id, { name: newName });
      return;
    }

    if (field === "isProtected") {
      const newValue = Boolean(event.newValue);
      if (original.isProtected && !newValue) {
        setErrorMessage("Cannot remove protected flag. This must be done via direct database update.");
        event.node.setDataValue("isProtected", true);
        return;
      }

      setErrorMessage(null);
      updateStagedChange(id, { isProtected: newValue });
      return;
    }

    if (field === "isAvailableForDistribution" || field === "isAvailableForForfeiture") {
      const newValue = Boolean(event.newValue);
      setErrorMessage(null);
      updateStagedChange(id, { [field]: newValue });
    }
  };

  const discardChanges = () => {
    if (!data) return;
    setRowData(data.map((r) => ({ ...r })));
    setStagedUpdatesById({});
    setErrorMessage(null);
  };

  const saveChanges = async () => {
    const entries = Object.entries(stagedUpdatesById);
    if (entries.length === 0) return;

    try {
      for (const [id, update] of entries) {
        const original = originalById[id];
        if (!original) continue;

        const payload: UpdateTaxCodeRequest = {
          id: original.id,
          name: update.name ?? original.name,
          isAvailableForDistribution: update.isAvailableForDistribution ?? original.isAvailableForDistribution,
          isAvailableForForfeiture: update.isAvailableForForfeiture ?? original.isAvailableForForfeiture,
          isProtected: update.isProtected ?? original.isProtected
        };

        await updateTaxCode(payload).unwrap();
      }

      setStagedUpdatesById({});
      await refetch();
      dispatch(setMessage(Messages.TaxCodesSaveSuccess));
    } catch (e) {
      console.error("Failed to update tax codes", e);
      dispatch(setMessage(Messages.TaxCodesSaveError));
    }
  };

  const handleCreateSuccess = async () => {
    await refetch();
    setErrorMessage(null);
    dispatch(setMessage(Messages.TaxCodesSaveSuccess));
  };

  const handleDeleteConfirm = async () => {
    if (!deleteTarget) return;

    try {
      await deleteTaxCode(deleteTarget.id).unwrap();
      await refetch();
      dispatch(setMessage(Messages.TaxCodesSaveSuccess));
      closeDeleteDialog();
    } catch (e: unknown) {
      console.error("Failed to delete tax code", e);
      dispatch(setMessage(Messages.TaxCodesSaveError));
    }
  };

  const columnDefs = useMemo<ColDef[]>(() => {
    const idColumn = {
      ...createTaxCodeColumn({ headerName: "ID", field: "id", minWidth: 80, maxWidth: 120 }),
      editable: false
    };

    const nameColumn = {
      ...createNameColumn({ field: "name", minWidth: 220 }),
      editable: (params: { data?: TaxCodeAdminDto }) => !params.data?.isProtected
    };

    const distributionColumn = {
      ...createYesOrNoColumn({
        headerName: "Available for Distribution",
        field: "isAvailableForDistribution",
        minWidth: 210,
        useWords: true
      }),
      editable: (params: { data?: TaxCodeAdminDto }) => !params.data?.isProtected,
      cellEditor: "agCheckboxCellEditor",
      cellRenderer: "agCheckboxCellRenderer"
    };

    const forfeitureColumn = {
      ...createYesOrNoColumn({
        headerName: "Available for Forfeiture",
        field: "isAvailableForForfeiture",
        minWidth: 200,
        useWords: true
      }),
      editable: (params: { data?: TaxCodeAdminDto }) => !params.data?.isProtected,
      cellEditor: "agCheckboxCellEditor",
      cellRenderer: "agCheckboxCellRenderer"
    };

    const protectedColumn = {
      ...createYesOrNoColumn({ headerName: "Protected", field: "isProtected", minWidth: 140, useWords: true }),
      editable: (params: { data?: TaxCodeAdminDto }) => !params.data?.isProtected,
      cellEditor: "agCheckboxCellEditor",
      cellRenderer: "agCheckboxCellRenderer"
    };

    const deleteColumn: ColDef = {
      headerName: "",
      field: "delete",
      sortable: false,
      filter: false,
      editable: false,
      width: 130,
      cellRenderer: (params: { data?: TaxCodeAdminDto }) => {
        const taxCode = params.data;
        if (!taxCode) return null;

        return (
          <Button
            variant="outlined"
            color="error"
            size="small"
            disabled={taxCode.isProtected}
            onClick={() => openDeleteDialog(taxCode)}>
            Delete
          </Button>
        );
      }
    };

    return [idColumn, nameColumn, distributionColumn, forfeitureColumn, protectedColumn, deleteColumn];
  }, [openDeleteDialog]);

  return (
    <PageErrorBoundary pageName="Manage Tax Codes">
      <Page label={CAPTIONS.MANAGE_TAX_CODES}>
        <Grid
          container
          rowSpacing={3}>
          <Grid width="100%">
            <Divider />
          </Grid>

          <Grid width="100%">
            <ApiMessageAlert commonKey="TaxCodesSave" />
          </Grid>

          <Grid width="100%">
            <Box sx={{ px: 1 }}>
              <Button
                variant="contained"
                onClick={() => setIsAddDialogOpen(true)}>
                Add New Tax Code
              </Button>
            </Box>
          </Grid>

          <Grid width="100%">
            <Box
              sx={{
                display: "flex",
                gap: 3,
                alignItems: "center",
                width: "100%",
                px: 1
              }}>
              <Box sx={{ flex: 1 }}>
                {errorMessage && (
                  <Typography
                    variant="body2"
                    color="error">
                    {errorMessage}
                  </Typography>
                )}
              </Box>

              <Box sx={{ display: "flex", gap: 3, justifyContent: "flex-end" }}>
                <Button
                  variant="contained"
                  disabled={!hasUnsavedChanges || isSaving}
                  onClick={saveChanges}>
                  Save
                </Button>
                <Button
                  variant="outlined"
                  disabled={!hasUnsavedChanges || isSaving}
                  onClick={discardChanges}>
                  Discard
                </Button>
              </Box>
            </Box>
          </Grid>

          <Grid width="100%">
            <DSMGrid
              preferenceKey={GRID_KEYS.MANAGE_TAX_CODES}
              isLoading={isFetching || isSaving || isDeleting}
              providedOptions={{
                rowData,
                columnDefs,
                suppressMultiSort: true,
                stopEditingWhenCellsLoseFocus: true,
                enterNavigatesVertically: true,
                enterNavigatesVerticallyAfterEdit: true,
                onCellValueChanged
              }}
            />
          </Grid>
        </Grid>

        <AddTaxCodeDialog
          open={isAddDialogOpen}
          onClose={() => setIsAddDialogOpen(false)}
          onSuccess={handleCreateSuccess}
        />

        <DeleteTaxCodeDialog
          open={isDeleteDialogOpen}
          taxCode={deleteTarget}
          isDeleting={isDeleting}
          onConfirm={handleDeleteConfirm}
          onCancel={closeDeleteDialog}
        />
      </Page>
    </PageErrorBoundary>
  );
};

export default ManageTaxCodes;
