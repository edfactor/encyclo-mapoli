import { Button, Divider, Grid, Typography } from "@mui/material";
import { CellValueChangedEvent, ColDef, ValueFormatterParams, ValueParserParams } from "ag-grid-community";
import { useEffect, useMemo, useState } from "react";
import { DSMGrid, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import { useGetStateTaxRatesQuery, useUpdateStateTaxRateMutation } from "../../../reduxstore/api/ItOperationsApi";
import { StateTaxRateDto } from "../../../reduxstore/types";

const hasMoreThanTwoDecimals = (value: number): boolean => {
  return Math.abs(value * 100 - Math.round(value * 100)) > Number.EPSILON;
};

const ManageStateTaxes = () => {
  const { data, isFetching, refetch } = useGetStateTaxRatesQuery();
  const [updateStateTaxRate, { isLoading: isSaving }] = useUpdateStateTaxRateMutation();

  const [rowData, setRowData] = useState<StateTaxRateDto[]>([]);
  const [originalRatesByAbbr, setOriginalRatesByAbbr] = useState<Record<string, number>>({});
  const [stagedRatesByAbbr, setStagedRatesByAbbr] = useState<Record<string, number>>({});
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const hasUnsavedChanges = Object.keys(stagedRatesByAbbr).length > 0;
  useUnsavedChangesGuard(hasUnsavedChanges);

  useEffect(() => {
    if (!data) return;

    setRowData(data);
    setOriginalRatesByAbbr(
      data.reduce<Record<string, number>>((acc, cur) => {
        acc[cur.abbreviation] = cur.rate;
        return acc;
      }, {})
    );

    setStagedRatesByAbbr({});
    setErrorMessage(null);
  }, [data]);

  const columnDefs = useMemo<ColDef[]>(() => {
    return [
      {
        headerName: "State",
        field: "abbreviation",
        sortable: true,
        filter: false,
        editable: false
      },
      {
        headerName: "Rate (%)",
        field: "rate",
        sortable: true,
        filter: false,
        editable: true,
        valueParser: (params: ValueParserParams) => {
          const parsed = Number.parseFloat(String(params.newValue ?? ""));
          return Number.isFinite(parsed) ? parsed : params.oldValue;
        },
        valueFormatter: (params: ValueFormatterParams) => {
          const value = params.value;
          return typeof value === "number" && Number.isFinite(value) ? value.toFixed(2) : "";
        }
      }
    ];
  }, []);

  const onCellValueChanged = (event: CellValueChangedEvent) => {
    const row = event.data as StateTaxRateDto | undefined;
    const abbr = row?.abbreviation;
    if (!abbr) return;

    setErrorMessage(null);

    const newRate = row?.rate;
    if (typeof newRate !== "number" || !Number.isFinite(newRate)) return;

    const originalRate = originalRatesByAbbr[abbr];
    if (typeof originalRate !== "number") return;

    setStagedRatesByAbbr((prev) => {
      const next = { ...prev };
      if (newRate === originalRate) {
        delete next[abbr];
      } else {
        next[abbr] = newRate;
      }
      return next;
    });
  };

  const discardChanges = () => {
    if (!data) return;
    setRowData(data);
    setStagedRatesByAbbr({});
    setErrorMessage(null);
  };

  const saveChanges = async () => {
    setErrorMessage(null);

    const entries = Object.entries(stagedRatesByAbbr);
    if (entries.length === 0) return;

    for (const [abbreviation, rate] of entries) {
      const normalizedAbbr = abbreviation.trim().toUpperCase();
      if (!/^[A-Z]{2}$/.test(normalizedAbbr)) {
        setErrorMessage("State abbreviation must be two letters (A-Z).");
        return;
      }

      if (rate < 0 || rate > 100) {
        setErrorMessage("Rate must be between 0 and 100.");
        return;
      }

      if (hasMoreThanTwoDecimals(rate)) {
        setErrorMessage("Rate can have at most 2 decimal places.");
        return;
      }
    }

    try {
      for (const [abbreviation, rate] of entries) {
        await updateStateTaxRate({ abbreviation, rate }).unwrap();
      }

      setStagedRatesByAbbr({});
      await refetch();
    } catch (e) {
      console.error("Failed to update state tax rates", e);
      setErrorMessage("Failed to save changes. Please try again.");
    }
  };

  return (
    <Page label={CAPTIONS.MANAGE_STATE_TAX_RATES}>
      <Grid
        container
        rowSpacing={3}>
        <Grid width="100%">
          <Divider />
        </Grid>

        <Grid
          width="100%"
          sx={{ display: "flex", gap: 2, alignItems: "center" }}>
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

          {errorMessage && (
            <Typography
              variant="body2"
              color="error">
              {errorMessage}
            </Typography>
          )}
        </Grid>

        <Grid width="100%">
          <DSMGrid
            preferenceKey={CAPTIONS.MANAGE_STATE_TAX_RATES}
            isLoading={isFetching || isSaving}
            providedOptions={{
              rowData,
              columnDefs,
              suppressMultiSort: true,
              onCellValueChanged
            }}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default ManageStateTaxes;
