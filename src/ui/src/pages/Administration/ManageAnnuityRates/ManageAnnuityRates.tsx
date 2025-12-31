import { Box, Button, Divider, Grid } from "@mui/material";
import { CellValueChangedEvent, ColDef, ValueFormatterParams, ValueParserParams } from "ag-grid-community";
import { useEffect, useMemo, useState } from "react";
import { useDispatch } from "react-redux";
import { ApiMessageAlert, DSMGrid, Page } from "smart-ui-library";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import { CAPTIONS, GRID_KEYS } from "../../../constants";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import { useGetAnnuityRatesQuery, useUpdateAnnuityRateMutation } from "../../../reduxstore/api/ItOperationsApi";
import { setMessage } from "../../../reduxstore/slices/messageSlice";
import { AnnuityRateDto } from "../../../reduxstore/types";
import { Messages } from "../../../utils/messageDictonary";

type StagedAnnuityRateChange = {
  singleRate: number;
  jointRate: number;
};

const hasMoreThanFourDecimals = (value: number): boolean => {
  // Convert to string and count decimal places
  // This avoids floating-point precision issues
  const valueString = value.toString();
  const decimalIndex = valueString.indexOf(".");

  if (decimalIndex === -1) {
    // No decimal point, so 0 decimal places
    return false;
  }

  const decimalPlaces = valueString.length - decimalIndex - 1;
  return decimalPlaces > 4;
};

const normalizeRateToFourDecimals = (value: number): number => {
  return Math.round(value * 10000) / 10000;
};

const getRowKey = (row: Pick<AnnuityRateDto, "year" | "age">): string => {
  return `${row.year}-${row.age}`;
};

const ManageAnnuityRates = () => {
  const dispatch = useDispatch();
  const { data, isFetching, refetch } = useGetAnnuityRatesQuery({ sortBy: "Year", isSortDescending: true });
  const [updateAnnuityRate, { isLoading: isSaving }] = useUpdateAnnuityRateMutation();

  const [rowData, setRowData] = useState<AnnuityRateDto[]>([]);
  const [originalRatesByKey, setOriginalRatesByKey] = useState<Record<string, StagedAnnuityRateChange>>({});
  const [stagedRatesByKey, setStagedRatesByKey] = useState<Record<string, StagedAnnuityRateChange>>({});

  const hasUnsavedChanges = Object.keys(stagedRatesByKey).length > 0;
  useUnsavedChangesGuard(hasUnsavedChanges);

  useEffect(() => {
    if (!data) return;

    // DSMGrid/AG Grid edits mutate row objects; make sure data isn't frozen.
    setRowData(data.map((r) => ({ ...r })));

    setOriginalRatesByKey(
      data.reduce<Record<string, StagedAnnuityRateChange>>((acc, cur) => {
        acc[getRowKey(cur)] = { singleRate: cur.singleRate, jointRate: cur.jointRate };
        return acc;
      }, {})
    );

    setStagedRatesByKey({});
  }, [data]);

  const columnDefs = useMemo<ColDef[]>(() => {
    return [
      {
        headerName: "Year",
        field: "year",
        sortable: true,
        filter: false,
        editable: false,
        width: 60
      },
      {
        headerName: "Age",
        field: "age",
        sortable: true,
        filter: false,
        editable: false,
        width: 55
      },
      {
        headerName: "Single Rate",
        field: "singleRate",
        sortable: true,
        filter: false,
        editable: true,
        width: 80,
        valueParser: (params: ValueParserParams) => {
          const parsed = Number.parseFloat(String(params.newValue ?? ""));
          return Number.isFinite(parsed) ? parsed : params.oldValue;
        },
        valueFormatter: (params: ValueFormatterParams) => {
          const value = params.value;
          return typeof value === "number" && Number.isFinite(value) ? value.toFixed(4) : "";
        }
      },
      {
        headerName: "Joint Rate",
        field: "jointRate",
        sortable: true,
        filter: false,
        editable: true,
        width: 80,
        valueParser: (params: ValueParserParams) => {
          const parsed = Number.parseFloat(String(params.newValue ?? ""));
          return Number.isFinite(parsed) ? parsed : params.oldValue;
        },
        valueFormatter: (params: ValueFormatterParams) => {
          const value = params.value;
          return typeof value === "number" && Number.isFinite(value) ? value.toFixed(4) : "";
        }
      }
    ];
  }, []);

  const onCellValueChanged = (event: CellValueChangedEvent) => {
    const row = event.data as AnnuityRateDto | undefined;
    if (!row) return;

    const key = getRowKey(row);
    const original = originalRatesByKey[key];
    if (!original) return;

    const normalizedSingleRate = normalizeRateToFourDecimals(row.singleRate);
    const normalizedJointRate = normalizeRateToFourDecimals(row.jointRate);

    setStagedRatesByKey((prev) => {
      const next = { ...prev };

      const normalizedOriginalSingle = normalizeRateToFourDecimals(original.singleRate);
      const normalizedOriginalJoint = normalizeRateToFourDecimals(original.jointRate);

      if (normalizedSingleRate === normalizedOriginalSingle && normalizedJointRate === normalizedOriginalJoint) {
        delete next[key];
      } else {
        next[key] = { singleRate: normalizedSingleRate, jointRate: normalizedJointRate };
      }

      return next;
    });
  };

  const discardChanges = () => {
    if (!data) return;
    setRowData(data.map((r) => ({ ...r })));
    setStagedRatesByKey({});
  };

  const saveChanges = async () => {
    const entries = Object.entries(stagedRatesByKey);
    if (entries.length === 0) return;

    for (const [key, rates] of entries) {
      const [yearStr, ageStr] = key.split("-");
      const year = Number.parseInt(yearStr ?? "", 10);
      const age = Number.parseInt(ageStr ?? "", 10);

      if (!Number.isFinite(year) || year < 1900 || year > 2100) {
        dispatch(
          setMessage({
            ...Messages.AnnuityRatesSaveError,
            message: {
              ...Messages.AnnuityRatesSaveError.message,
              message: "Year must be between 1900 and 2100."
            }
          })
        );
        return;
      }

      if (!Number.isFinite(age) || age < 0 || age > 120) {
        dispatch(
          setMessage({
            ...Messages.AnnuityRatesSaveError,
            message: {
              ...Messages.AnnuityRatesSaveError.message,
              message: "Age must be between 0 and 120."
            }
          })
        );
        return;
      }

      if (rates.singleRate < 0 || rates.singleRate > 99.9999) {
        dispatch(
          setMessage({
            ...Messages.AnnuityRatesSaveError,
            message: {
              ...Messages.AnnuityRatesSaveError.message,
              message: "Single Rate must be between 0 and 99.9999."
            }
          })
        );
        return;
      }

      if (hasMoreThanFourDecimals(rates.singleRate)) {
        dispatch(
          setMessage({
            ...Messages.AnnuityRatesSaveError,
            message: {
              ...Messages.AnnuityRatesSaveError.message,
              message: "Single Rate can have at most 4 decimal places."
            }
          })
        );
        return;
      }

      if (rates.jointRate < 0 || rates.jointRate > 99.9999) {
        dispatch(
          setMessage({
            ...Messages.AnnuityRatesSaveError,
            message: {
              ...Messages.AnnuityRatesSaveError.message,
              message: "Joint Rate must be between 0 and 99.9999."
            }
          })
        );
        return;
      }

      if (hasMoreThanFourDecimals(rates.jointRate)) {
        dispatch(
          setMessage({
            ...Messages.AnnuityRatesSaveError,
            message: {
              ...Messages.AnnuityRatesSaveError.message,
              message: "Joint Rate can have at most 4 decimal places."
            }
          })
        );
        return;
      }
    }

    try {
      for (const [key, rates] of entries) {
        const [yearStr, ageStr] = key.split("-");
        const year = Number.parseInt(yearStr ?? "", 10);
        const age = Number.parseInt(ageStr ?? "", 10);

        await updateAnnuityRate({ year, age, singleRate: rates.singleRate, jointRate: rates.jointRate }).unwrap();
      }

      setStagedRatesByKey({});
      await refetch();

      dispatch(setMessage(Messages.AnnuityRatesSaveSuccess));
    } catch (e) {
      console.error("Failed to update annuity rates", e);
      dispatch(setMessage(Messages.AnnuityRatesSaveError));
    }
  };

  return (
    <PageErrorBoundary pageName="Manage Annuity Rates">
      <Page label={CAPTIONS.MANAGE_ANNUITY_RATES}>
        <Grid
          container
          rowSpacing={3}>
          <Grid width="100%">
            <Divider />
          </Grid>

          <Grid width="100%">
            <ApiMessageAlert commonKey="AnnuityRatesSave" />
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
              <Box sx={{ flex: 1 }} />

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
              preferenceKey={GRID_KEYS.MANAGE_ANNUITY_RATES}
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

export default ManageAnnuityRates;
