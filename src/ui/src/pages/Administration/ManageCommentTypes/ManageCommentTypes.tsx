import { Box, Button, Divider, Grid, Typography } from "@mui/material";
import { CellValueChangedEvent, ColDef } from "ag-grid-community";
import { useEffect, useMemo, useState } from "react";
import { DSMGrid, Page } from "smart-ui-library";
import { CAPTIONS, GRID_KEYS } from "../../../constants";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import { useGetCommentTypesQuery, useUpdateCommentTypeMutation } from "../../../reduxstore/api/administrationApi";
import { CommentTypeDto } from "../../../types";

const ManageCommentTypes = () => {
  const { data, isFetching, refetch } = useGetCommentTypesQuery();
  const [updateCommentType, { isLoading: isSaving }] = useUpdateCommentTypeMutation();

  const [rowData, setRowData] = useState<CommentTypeDto[]>([]);
  const [originalNamesByID, setOriginalNamesByID] = useState<Record<number, string>>({});
  const [stagedNamesByID, setStagedNamesByID] = useState<Record<number, string>>({});
  const [originalProtectionByID, setOriginalProtectionByID] = useState<Record<number, boolean>>({});
  const [stagedProtectionByID, setStagedProtectionByID] = useState<Record<number, boolean>>({});
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const hasUnsavedChanges = Object.keys(stagedNamesByID).length > 0 || Object.keys(stagedProtectionByID).length > 0;
  useUnsavedChangesGuard(hasUnsavedChanges);

  useEffect(() => {
    if (!data) return;

    // DSMGrid/AG Grid edits mutate row objects; make sure data isn't frozen.
    setRowData(data.map((r) => ({ ...r })));
    setOriginalNamesByID(
      data.reduce<Record<number, string>>((acc, cur) => {
        acc[cur.id] = cur.name;
        return acc;
      }, {})
    );
    setOriginalProtectionByID(
      data.reduce<Record<number, boolean>>((acc, cur) => {
        acc[cur.id] = cur.isProtected;
        return acc;
      }, {})
    );

    setStagedNamesByID({});
    setErrorMessage(null);
  }, [data]);

  const columnDefs = useMemo<ColDef[]>(() => {
    return [
      {
        headerName: "ID",
        field: "id",
        sortable: true,
        filter: false,
        editable: false,
        width: 100
      },
      {
        headerName: "Name",
        field: "name",
        sortable: true,
        filter: false,
        editable: true,
        flex: 1
      },
      {
        headerName: "Protected",
        field: "isProtected",
        sortable: true,
        filter: false,
        editable: true,
        width: 130,
        cellEditor: "agCheckboxCellEditor",
        cellRenderer: "agCheckboxCellRenderer"
      }
    ];
  }, []);

  const onCellValueChanged = (event: CellValueChangedEvent) => {
    const row = event.data as CommentTypeDto | undefined;
    const id = row?.id;
    const field = event.colDef.field;
    if (!id || !field) return;

    // Handle Name changes
    if (field === "name") {
      const newName = String(event.newValue ?? "").trim();
      if (!newName) {
        setErrorMessage("Name cannot be empty.");
        return;
      }

      const originalName = originalNamesByID[id];
      if (!originalName) return;

      // Valid name change - clear any previous errors
      setErrorMessage(null);

      setStagedNamesByID((prev) => {
        const next = { ...prev };
        if (newName === originalName) {
          delete next[id];
        } else {
          next[id] = newName;
        }
        return next;
      });
    }

    // Handle IsProtected changes (one-way: can set to true, cannot unset)
    if (field === "isProtected") {
      const newValue = Boolean(event.newValue);
      const originalProtection = originalProtectionByID[id];

      // Prevent removing protection flag via UI
      if (originalProtection === true && newValue === false) {
        setErrorMessage("Cannot remove protected flag. This must be done via direct database update.");
        // Revert the change in the grid
        event.node.setDataValue("isProtected", true);
        return;
      }

      setStagedProtectionByID((prev) => {
        const next = { ...prev };
        if (newValue === originalProtection) {
          delete next[id];
        } else {
          next[id] = newValue;
          // Only clear error when actually staging a change
          setErrorMessage(null);
        }
        return next;
      });
    }
  };

  const discardChanges = () => {
    if (!data) return;
    setRowData(data.map((r) => ({ ...r })));
    setStagedNamesByID({});
    setStagedProtectionByID({});
    setErrorMessage(null);
  };

  const saveChanges = async () => {
    setErrorMessage(null);

    // Collect all IDs that have changes (names or protection)
    const allChangedIds = new Set([...Object.keys(stagedNamesByID), ...Object.keys(stagedProtectionByID)]);

    if (allChangedIds.size === 0) return;

    // Validate all changes
    for (const idStr of allChangedIds) {
      const id = Number.parseInt(idStr, 10);
      if (!Number.isFinite(id) || id <= 0) {
        setErrorMessage("Invalid comment type ID.");
        return;
      }

      const name = stagedNamesByID[id] || originalNamesByID[id];
      if (!name || name.trim().length === 0) {
        setErrorMessage("Name cannot be empty.");
        return;
      }

      if (name.length > 200) {
        setErrorMessage("Name cannot exceed 200 characters.");
        return;
      }
    }

    try {
      for (const idStr of allChangedIds) {
        const id = Number.parseInt(idStr, 10);
        const name = stagedNamesByID[id] || originalNamesByID[id];
        const isProtected =
          stagedProtectionByID[id] !== undefined ? stagedProtectionByID[id] : originalProtectionByID[id];

        await updateCommentType({ id, name, isProtected }).unwrap();
      }

      setStagedNamesByID({});
      setStagedProtectionByID({});
      await refetch();
    } catch (e) {
      console.error("Failed to update comment types", e);
      setErrorMessage("Failed to save changes. Please try again.");
    }
  };

  return (
    <Page label={CAPTIONS.MANAGE_COMMENT_TYPES}>
      <Grid
        container
        rowSpacing={3}>
        <Grid width="100%">
          <Divider />
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
            preferenceKey={GRID_KEYS.MANAGE_COMMENT_TYPES}
            isLoading={isFetching || isSaving}
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
    </Page>
  );
};

export default ManageCommentTypes;
