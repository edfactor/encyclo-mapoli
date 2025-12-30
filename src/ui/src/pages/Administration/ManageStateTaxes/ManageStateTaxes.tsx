import { Box, Button, Divider, Grid } from "@mui/material";
import { CellValueChangedEvent, ColDef, ValueFormatterParams, ValueParserParams } from "ag-grid-community";
import { useEffect, useMemo, useState } from "react";
import { useDispatch } from "react-redux";
import { ApiMessageAlert, DSMGrid, Page } from "smart-ui-library";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import { CAPTIONS, GRID_KEYS } from "../../../constants";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import { useGetStateTaxRatesQuery, useUpdateStateTaxRateMutation } from "../../../reduxstore/api/ItOperationsApi";
import { setMessage } from "../../../reduxstore/slices/messageSlice";
import { StateTaxRateDto } from "../../../reduxstore/types";
import { Messages } from "../../../utils/messageDictonary";

const hasMoreThanTwoDecimals = (value: number): boolean => {
  // Convert to string and count decimal places to avoid floating-point precision issues
  const strValue = String(value);
  const decimalIndex = strValue.indexOf(".");
  if (decimalIndex === -1) return false; // No decimal point, so 0 decimal places
  return strValue.length - decimalIndex - 1 > 2; // Count digits after decimal point
};

const normalizeRateToTwoDecimals = (value: number): number => {
  return Math.round(value * 100) / 100;
};

const ManageStateTaxes = () => {
  const dispatch = useDispatch();
  const { data, isFetching, refetch } = useGetStateTaxRatesQuery();
  const [updateStateTaxRate, { isLoading: isSaving }] = useUpdateStateTaxRateMutation();

  const [rowData, setRowData] = useState<StateTaxRateDto[]>([]);
  const [originalRatesByAbbr, setOriginalRatesByAbbr] = useState<Record<string, number>>({});
  const [stagedRatesByAbbr, setStagedRatesByAbbr] = useState<Record<string, number>>({});

  const hasUnsavedChanges = Object.keys(stagedRatesByAbbr).length > 0;
  useUnsavedChangesGuard(hasUnsavedChanges);

  useEffect(() => {
    if (!data) return;

    // DSMGrid/AG Grid edits mutate row objects; make sure data isn't frozen.
    setRowData(data.map((r) => ({ ...r })));
    setOriginalRatesByAbbr(
      data.reduce<Record<string, number>>((acc, cur) => {
        acc[cur.abbreviation] = cur.rate;
        return acc;
      }, {})
    );

    setStagedRatesByAbbr({});
  }, [data]);

  const columnDefs = useMemo<ColDef[]>(() => {
    return [
      {
        headerName: "State",
        field: "abbreviation",
        sortable: true,
        filter: false,
        editable: false,
        width: 130
      },
      {
        headerName: "Rate (%)",
        field: "rate",
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
          return typeof value === "number" && Number.isFinite(value) ? value.toFixed(2) : "";
        }
      },
      {
        headerName: "",
        field: "",
        sortable: false,
        filter: false,
        editable: false,
        flex: 1,
        minWidth: 100
      }
    ];
  }, []);

  const onCellValueChanged = (event: CellValueChangedEvent) => {
    const row = event.data as StateTaxRateDto | undefined;
    const abbr = row?.abbreviation;
    if (!abbr) return;

    const parsedNewRate = Number.parseFloat(String(event.newValue ?? ""));
    if (!Number.isFinite(parsedNewRate)) return;

    const originalRate = originalRatesByAbbr[abbr];
    if (typeof originalRate !== "number") return;

    const normalizedNewRate = normalizeRateToTwoDecimals(parsedNewRate);
    const normalizedOriginalRate = normalizeRateToTwoDecimals(originalRate);

    setStagedRatesByAbbr((prev) => {
      const next = { ...prev };
      if (normalizedNewRate === normalizedOriginalRate) {
        delete next[abbr];
      } else {
        next[abbr] = normalizedNewRate;
      }
      return next;
    });
  };

  const discardChanges = () => {
    if (!data) return;
    setRowData(data.map((r) => ({ ...r })));
    setStagedRatesByAbbr({});
  };

  const saveChanges = async () => {
    const entries = Object.entries(stagedRatesByAbbr);
    if (entries.length === 0) return;

    for (const [abbreviation, rate] of entries) {
      const normalizedAbbr = abbreviation.trim().toUpperCase();
      if (!/^[A-Z]{2}$/.test(normalizedAbbr)) {
        dispatch(
          setMessage({
            ...Messages.StateTaxRatesSaveError,
            message: {
              ...Messages.StateTaxRatesSaveError.message,
              message: "State abbreviation must be two letters (A-Z)."
            }
          })
        );
        return;
      }

      if (rate < 0 || rate > 100) {
        dispatch(
          setMessage({
            ...Messages.StateTaxRatesSaveError,
            message: {
              ...Messages.StateTaxRatesSaveError.message,
              message: "Rate must be between 0 and 100."
            }
          })
        );
        return;
      }

      if (hasMoreThanTwoDecimals(rate)) {
        dispatch(
          setMessage({
            ...Messages.StateTaxRatesSaveError,
            message: {
              ...Messages.StateTaxRatesSaveError.message,
              message: "Rate can have at most 2 decimal places."
            }
          })
        );
        return;
      }
    }

    try {
      for (const [abbreviation, rate] of entries) {
        await updateStateTaxRate({ abbreviation, rate }).unwrap();
      }

      setStagedRatesByAbbr({});
      await refetch();

      dispatch(setMessage(Messages.StateTaxRatesSaveSuccess));
    } catch (e) {
      console.error("Failed to update state tax rates", e);
      dispatch(setMessage(Messages.StateTaxRatesSaveError));
    }
  };

  return (
    <PageErrorBoundary pageName="Manage State Taxes">
      <Page label={CAPTIONS.MANAGE_STATE_TAX_RATES}>
        <Grid
          container
          rowSpacing={3}>
          <Grid width="100%">
            <Divider />
          </Grid>

          <Grid width="100%">
            <ApiMessageAlert commonKey="StateTaxRatesSave" />
          </Grid>

          <Grid width="100%">
            <Box
              sx={{
                display: "flex",
                gap: 3,
                justifyContent: "flex-end",
                width: "100%",
                px: 1
              }}>
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
          </Grid>

          <Grid width="100%">
            <DSMGrid
              preferenceKey={GRID_KEYS.MANAGE_STATE_TAX_RATES}
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

export default ManageStateTaxes;
