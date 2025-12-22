import { Box, Button, Divider, Grid, Typography } from "@mui/material";
import { CellValueChangedEvent, ColDef, ValueFormatterParams } from "ag-grid-community";
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
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const hasUnsavedChanges = Object.keys(stagedNamesByID).length > 0;
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
      }
    ];
  }, []);

  const onCellValueChanged = (event: CellValueChangedEvent) => {
    const row = event.data as CommentTypeDto | undefined;
    const id = row?.id;
    if (!id) return;

    setErrorMessage(null);

    const newName = String(event.newValue ?? "").trim();
    if (!newName) {
      setErrorMessage("Name cannot be empty.");
      return;
    }

    const originalName = originalNamesByID[id];
    if (!originalName) return;

    setStagedNamesByID((prev) => {
      const next = { ...prev };
      if (newName === originalName) {
        delete next[id];
      } else {
        next[id] = newName;
      }
      return next;
    });
  };

  const discardChanges = () => {
    if (!data) return;
    setRowData(data.map((r) => ({ ...r })));
    setStagedNamesByID({});
    setErrorMessage(null);
  };

  const saveChanges = async () => {
    setErrorMessage(null);

    const entries = Object.entries(stagedNamesByID);
    if (entries.length === 0) return;

    for (const [idStr, name] of entries) {
      const id = Number.parseInt(idStr, 10);
      if (!Number.isFinite(id) || id <= 0) {
        setErrorMessage("Invalid comment type ID.");
        return;
      }

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
      for (const [idStr, name] of entries) {
        const id = Number.parseInt(idStr, 10);
        await updateCommentType({ id, name }).unwrap();
      }

      setStagedNamesByID({});
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
