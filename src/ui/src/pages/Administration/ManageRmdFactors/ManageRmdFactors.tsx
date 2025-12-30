import { Box, Button, Divider, Grid, Typography } from "@mui/material";
import { CellValueChangedEvent, ColDef, ValueFormatterParams, ValueParserParams } from "ag-grid-community";
import { useEffect, useMemo, useState } from "react";
import { useDispatch } from "react-redux";
import { ApiMessageAlert, DSMGrid, Page, setMessage } from "smart-ui-library";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import { CAPTIONS, GRID_KEYS } from "../../../constants";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import { useGetRmdFactorsQuery, useUpdateRmdFactorMutation } from "../../../reduxstore/api/administrationApi";
import { RmdFactorDto } from "../../../reduxstore/types";
import { Messages } from "../../../utils/messageDictonary";

const hasMoreThanFourDecimals = (value: number): boolean => {
  return Math.abs(value * 10000 - Math.round(value * 10000)) > Number.EPSILON;
};

const normalizePercentageToFourDecimals = (value: number): number => {
  return Math.round(value * 10000) / 10000;
};

const ManageRmdFactors = () => {
  const dispatch = useDispatch();
  const { data, isFetching, refetch, error, isError } = useGetRmdFactorsQuery();
  const [updateRmdFactor, { isLoading: isSaving }] = useUpdateRmdFactorMutation();

  const [rowData, setRowData] = useState<RmdFactorDto[]>([]);
  const [originalFactorsByAge, setOriginalFactorsByAge] = useState<Record<number, number>>({});
  const [stagedFactorsByAge, setStagedFactorsByAge] = useState<Record<number, number>>({});
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const hasUnsavedChanges = Object.keys(stagedFactorsByAge).length > 0;
  useUnsavedChangesGuard(hasUnsavedChanges);

  // Log any API errors
  useEffect(() => {
    if (isError && error) {
      console.error("RMD Factors API Error:", error);
    }
  }, [isError, error]);

  useEffect(() => {
    if (!data) return;

    // DSMGrid/AG Grid edits mutate row objects; make sure data isn't frozen.
    setRowData(data.map((r) => ({ ...r })));
    setOriginalFactorsByAge(
      data.reduce<Record<number, number>>((acc, cur) => {
        acc[cur.age] = cur.factor;
        return acc;
      }, {})
    );

    setStagedFactorsByAge({});
    setErrorMessage(null);
  }, [data]);

  const columnDefs = useMemo<ColDef[]>(() => {
    return [
      {
        headerName: "Age",
        field: "age",
        sortable: true,
        filter: false,
        editable: false,
        width: 130
      },
      {
        headerName: "Factor",
        field: "factor",
        sortable: true,
        filter: false,
        editable: true,
        width: 170,
        valueParser: (params: ValueParserParams) => {
          const parsed = Number.parseFloat(String(params.newValue ?? ""));
          return Number.isFinite(parsed) ? parsed : params.oldValue;
        },
        valueFormatter: (params: ValueFormatterParams) => {
          const value = params.value;
          return typeof value === "number" && Number.isFinite(value) ? value.toFixed(4) : "N/A";
        }
      }
    ];
  }, []);

  const onCellValueChanged = (event: CellValueChangedEvent) => {
    const row = event.data as RmdFactorDto | undefined;
    const age = row?.age;
    if (age === undefined) return;

    setErrorMessage(null);

    const parsedNewFactor = Number.parseFloat(String(event.newValue ?? ""));
    if (!Number.isFinite(parsedNewFactor)) return;

    const originalFactor = originalFactorsByAge[age];
    if (typeof originalFactor !== "number") return;

    const normalizedNewFactor = normalizePercentageToFourDecimals(parsedNewFactor);
    const normalizedOriginalFactor = normalizePercentageToFourDecimals(originalFactor);

    setStagedFactorsByAge((prev) => {
      const next = { ...prev };
      if (normalizedNewFactor === normalizedOriginalFactor) {
        delete next[age];
      } else {
        next[age] = normalizedNewFactor;
      }
      return next;
    });
  };

  const discardChanges = () => {
    if (!data) return;
    setRowData(data.map((r) => ({ ...r })));
    setStagedFactorsByAge({});
    setErrorMessage(null);
  };

  const saveChanges = async () => {
    setErrorMessage(null);

    const entries = Object.entries(stagedFactorsByAge);
    if (entries.length === 0) return;

    for (const [ageStr, factor] of entries) {
      const age = Number.parseInt(ageStr, 10);
      
      if (!Number.isInteger(age) || age < 0 || age > 150) {
        setErrorMessage("Age must be a valid integer between 0 and 150.");
        return;
      }

      if (factor < 0 || factor > 100) {
        setErrorMessage("Factor must be between 0 and 100.");
        return;
      }

      if (hasMoreThanFourDecimals(factor)) {
        setErrorMessage("Factor can have at most 4 decimal places.");
        return;
      }
    }

    try {
      for (const [ageStr, factor] of entries) {
        const age = Number.parseInt(ageStr, 10);
        await updateRmdFactor({ age, factor }).unwrap();
      }

      setStagedFactorsByAge({});
      await refetch();
      
      // Dispatch success message
      dispatch(
        setMessage({
          ...Messages.RmdFactorsSaveSuccess,
          message: {
            ...Messages.RmdFactorsSaveSuccess.message,
            message: `Successfully updated ${entries.length} RMD factor${entries.length > 1 ? 's' : ''}.`
          }
        })
      );
    } catch (e) {
      console.error("Failed to update RMD factors", e);
      setErrorMessage("Failed to save changes. Please try again.");
    }
  };

  return (
    <PageErrorBoundary pageName="Manage RMD Factors">
      <Page label={CAPTIONS.MANAGE_RMD_FACTORS}>
        <div>
          <ApiMessageAlert commonKey="RmdFactorsSave" />
        </div>
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
            preferenceKey={GRID_KEYS.MANAGE_RMD_FACTORS}
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
    </PageErrorBoundary>
  );
};

export default ManageRmdFactors;
