import { Box, Button, Checkbox, Divider, FormControlLabel, Grid, TextField, Typography } from "@mui/material";
import { CellValueChangedEvent, ColDef, ValueParserParams } from "ag-grid-community";
import StandaloneMemberDetails from "pages/InquiriesAndAdjustments/MasterInquiry/StandaloneMemberDetails";
import { useEffect, useMemo, useState } from "react";
import { DSMGrid, Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS, GRID_KEYS } from "../../../constants";
import { useMissiveAlerts } from "../../../hooks/useMissiveAlerts";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import {
  useLazyGetProfitSharingAdjustmentsQuery,
  useSaveProfitSharingAdjustmentsMutation
} from "../../../reduxstore/api/ProfitDetailsApi";
import {
  ProfitSharingAdjustmentRowDto,
  ProfitSharingAdjustmentsKey,
  SaveProfitSharingAdjustmentRowRequest
} from "../../../reduxstore/types";

const isValidIsoDate = (value: string): boolean => {
  if (!/^\d{4}-\d{2}-\d{2}$/.test(value)) {
    return false;
  }

  const parsed = new Date(`${value}T00:00:00Z`);
  return Number.isFinite(parsed.getTime()) && parsed.toISOString().startsWith(value);
};

const toNumberOrOld = (params: ValueParserParams): number => {
  const parsed = Number.parseFloat(String(params.newValue ?? ""));
  return Number.isFinite(parsed) ? parsed : (params.oldValue as number);
};

const ProfitSharingAdjustmentsContent = () => {
  const [triggerGet, { data, isFetching: isFetchingAdjustments }] = useLazyGetProfitSharingAdjustmentsQuery();
  const [saveAdjustments, { isLoading: isSaving }] = useSaveProfitSharingAdjustmentsMutation();
  const { clearAlerts } = useMissiveAlerts();

  const [profitYear, setProfitYear] = useState<number>(new Date().getFullYear());
  const [badgeNumber, setBadgeNumber] = useState<string>("");
  const [sequenceNumber, setSequenceNumber] = useState<number>(0);
  const [getAllRows, setGetAllRows] = useState<boolean>(false);
  const [loadedGetAllRows, setLoadedGetAllRows] = useState<boolean>(false);

  const [loadedKey, setLoadedKey] = useState<ProfitSharingAdjustmentsKey | null>(null);
  const [rowData, setRowData] = useState<ProfitSharingAdjustmentRowDto[]>([]);
  const [originalByRowNumber, setOriginalByRowNumber] = useState<Record<number, ProfitSharingAdjustmentRowDto>>({});
  const [stagedByRowNumber, setStagedByRowNumber] = useState<Record<number, SaveProfitSharingAdjustmentRowRequest>>({});
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const hasUnsavedChanges = Object.keys(stagedByRowNumber).length > 0;
  useUnsavedChangesGuard(hasUnsavedChanges);

  useEffect(() => {
    if (!data) {
      return;
    }

    clearAlerts();

    setLoadedKey({ profitYear: data.profitYear, badgeNumber: data.badgeNumber, sequenceNumber: data.sequenceNumber });
    setBadgeNumber(String(data.badgeNumber));

    // DSMGrid/AG Grid edits mutate row objects; make sure data isn't frozen.
    const copy = (data.rows ?? []).map((r) => ({ ...r }));
    setRowData(copy);

    setOriginalByRowNumber(
      copy.reduce<Record<number, ProfitSharingAdjustmentRowDto>>((acc, cur) => {
        acc[cur.rowNumber] = { ...cur };
        return acc;
      }, {})
    );

    setStagedByRowNumber({});
    setErrorMessage(null);
  }, [data, clearAlerts]);

  const columnDefs = useMemo<ColDef[]>(() => {
    const isExistingRow = (row?: ProfitSharingAdjustmentRowDto): boolean => row?.profitDetailId != null;
    const isInsertRow = (row?: ProfitSharingAdjustmentRowDto): boolean => row?.profitDetailId == null && row?.activityDate != null;
    const canEditExtOnExistingRow = (row?: ProfitSharingAdjustmentRowDto): boolean =>
      isExistingRow(row) && row?.isEditable === true;

    return [
      {
        headerName: "Row",
        field: "rowNumber",
        sortable: false,
        filter: false,
        editable: false,
        width: 55
      },
      {
        headerName: "Profit Year",
        field: "profitYear",
        sortable: false,
        filter: false,
        editable: false,
        width: 90
      },
      {
        headerName: "EXT",
        field: "profitYearIteration",
        sortable: false,
        filter: false,
        editable: (params) => canEditExtOnExistingRow(params.data as ProfitSharingAdjustmentRowDto | undefined),
        width: 60,
        valueParser: (params: ValueParserParams) => {
          const parsed = Number.parseInt(String(params.newValue ?? ""), 10);
          if (parsed === 0 || parsed === 3) {
            return parsed;
          }
          return params.oldValue;
        }
      },
      {
        headerName: "Profit Code",
        field: "profitCodeId",
        sortable: false,
        filter: false,
        editable: false,
        width: 90
      },
      {
        headerName: "Contribution",
        field: "contribution",
        sortable: false,
        filter: false,
        editable: (params) => isInsertRow(params.data as ProfitSharingAdjustmentRowDto | undefined),
        width: 110,
        valueParser: toNumberOrOld
      },
      {
        headerName: "Earnings",
        field: "earnings",
        sortable: false,
        filter: false,
        editable: (params) => isInsertRow(params.data as ProfitSharingAdjustmentRowDto | undefined),
        width: 110,
        valueParser: toNumberOrOld
      },
      {
        headerName: "Forfeiture",
        field: "forfeiture",
        sortable: false,
        filter: false,
        editable: (params) => isInsertRow(params.data as ProfitSharingAdjustmentRowDto | undefined),
        width: 110,
        valueParser: toNumberOrOld
      },
      {
        headerName: "Activity Date",
        field: "activityDate",
        sortable: false,
        filter: false,
        editable: false,
        width: 120
      },
      {
        headerName: "Comment",
        field: "comment",
        sortable: false,
        filter: false,
        editable: false,
        flex: 1,
        minWidth: 160
      }
    ];
  }, []);

  const loadAdjustments = async () => {
    setErrorMessage(null);
    clearAlerts();

    if (hasUnsavedChanges) {
      setErrorMessage("Discard changes before loading different adjustments.");
      return;
    }

    if (!Number.isFinite(profitYear) || profitYear < 1900 || profitYear > 2500) {
      setErrorMessage("Profit Year must be between 1900 and 2500.");
      return;
    }

    const parsedBadgeNumber = Number.parseInt(badgeNumber, 10);
    if (!Number.isFinite(parsedBadgeNumber) || parsedBadgeNumber <= 0) {
      setErrorMessage("Badge Number must be greater than zero.");
      return;
    }

    if (!Number.isFinite(sequenceNumber) || sequenceNumber < 0) {
      setErrorMessage("Sequence Number must be zero or greater.");
      return;
    }

    try {
      setLoadedGetAllRows(getAllRows);
      await triggerGet({ profitYear, badgeNumber: parsedBadgeNumber, sequenceNumber, getAllRows }).unwrap();
    } catch (e) {
      console.error("Failed to load profit sharing adjustments", e);
      setErrorMessage("Failed to load adjustments. Please try again.");
    }
  };

  const discardChanges = () => {
    setErrorMessage(null);

    const originalRows = Object.values(originalByRowNumber);
    if (originalRows.length === 0) {
      return;
    }

    setRowData(originalRows.map((r) => ({ ...r })));
    setStagedByRowNumber({});
  };

  const onCellValueChanged = (event: CellValueChangedEvent) => {
    const row = event.data as ProfitSharingAdjustmentRowDto | undefined;
    if (!row) {
      return;
    }

    setErrorMessage(null);

    if (event.colDef.field === "activityDate") {
      const next = row.activityDate;
      if (next && !isValidIsoDate(next)) {
        row.activityDate = (event.oldValue as string | null | undefined) ?? null;
        event.api.refreshCells({ force: true });
        setErrorMessage("Activity Date must be in YYYY-MM-DD format.");
        return;
      }
    }

    const original = originalByRowNumber[row.rowNumber];
    if (!original) {
      return;
    }

    const changed =
      row.profitYearIteration !== original.profitYearIteration ||
      row.profitCodeId !== original.profitCodeId ||
      row.contribution !== original.contribution ||
      row.earnings !== original.earnings ||
      row.forfeiture !== original.forfeiture ||
      row.activityDate !== original.activityDate ||
      row.comment !== original.comment;

    setStagedByRowNumber((prev) => {
      const next = { ...prev };

      if (!changed) {
        delete next[row.rowNumber];
        return next;
      }

      next[row.rowNumber] = {
        profitDetailId: row.profitDetailId,
        rowNumber: row.rowNumber,
        profitYearIteration: row.profitYearIteration,
        profitCodeId: row.profitCodeId,
        contribution: row.contribution,
        earnings: row.earnings,
        forfeiture: row.forfeiture,
        activityDate: row.activityDate,
        comment: row.comment
      };

      return next;
    });
  };

  const saveChanges = async () => {
    setErrorMessage(null);

    if (!loadedKey) {
      setErrorMessage("Load adjustments before saving.");
      return;
    }

    if (!hasUnsavedChanges) {
      return;
    }

    const rowsToSave: SaveProfitSharingAdjustmentRowRequest[] = rowData.map((r) => ({
      profitDetailId: r.profitDetailId,
      rowNumber: r.rowNumber,
      profitYearIteration: r.profitYearIteration,
      profitCodeId: r.profitCodeId,
      contribution: r.contribution,
      earnings: r.earnings,
      forfeiture: r.forfeiture,
      activityDate: r.activityDate,
      comment: r.comment
    }));

    for (const row of rowsToSave) {
      if (row.profitYearIteration !== 0 && row.profitYearIteration !== 3) {
        setErrorMessage("EXT must be 0 or 3.");
        return;
      }

      if (row.activityDate && !isValidIsoDate(row.activityDate)) {
        setErrorMessage("Activity Date must be in YYYY-MM-DD format.");
        return;
      }
    }

    try {
      await saveAdjustments({ ...loadedKey, rows: rowsToSave }).unwrap();
      setStagedByRowNumber({});

      // Refresh to ensure server-calculated fields stay in sync.
      await triggerGet({ ...loadedKey, getAllRows: loadedGetAllRows }).unwrap();
    } catch (e) {
      console.error("Failed to save profit sharing adjustments", e);
      setErrorMessage("Failed to save changes. Please try again.");
    }
  };

  return (
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
            gap: 2,
            alignItems: "center",
            width: "100%",
            px: 1,
            flexWrap: "wrap"
          }}>
          <TextField
            label="Profit Year"
            size="small"
            type="text"
            value={profitYear}
            onChange={(e) => setProfitYear(Number.parseInt(e.target.value ?? "", 10) || 0)}
            sx={{ width: 130 }}
            inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
          />
          <TextField
            label="Badge Number"
            size="small"
            type="text"
            value={badgeNumber}
            onChange={(e) => setBadgeNumber(e.target.value ?? "")}
            sx={{ width: 150 }}
            inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
          />
          <TextField
            label="Sequence #"
            size="small"
            type="text"
            value={sequenceNumber}
            onChange={(e) => setSequenceNumber(Number.parseInt(e.target.value ?? "", 10) || 0)}
            sx={{ width: 120 }}
            inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
          />

          <FormControlLabel
            control={
              <Checkbox
                checked={getAllRows}
                onChange={(_e, checked) => setGetAllRows(checked)}
                disabled={isFetchingAdjustments || isSaving || hasUnsavedChanges}
              />
            }
            label="Get all rows"
          />

          <Button
            variant="contained"
            disabled={isFetchingAdjustments || isSaving}
            onClick={loadAdjustments}>
            Load
          </Button>

          <Box sx={{ flex: 1 }}>
            {errorMessage && (
              <Typography
                variant="body2"
                color="error">
                {errorMessage}
              </Typography>
            )}
          </Box>

          <Box sx={{ display: "flex", gap: 2, justifyContent: "flex-end" }}>
            <Button
              variant="contained"
              disabled={!hasUnsavedChanges || isSaving || isFetchingAdjustments}
              onClick={saveChanges}>
              Save
            </Button>
            <Button
              variant="outlined"
              disabled={!hasUnsavedChanges || isSaving || isFetchingAdjustments}
              onClick={discardChanges}>
              Discard
            </Button>
          </Box>
        </Box>
      </Grid>

      {data?.demographicId != null && (
        <StandaloneMemberDetails
          memberType={1}
          id={data.demographicId}
          profitYear={data.profitYear}
        />
      )}

      <Grid width="100%">
        <DSMGrid
          preferenceKey={GRID_KEYS.PROFIT_SHARING_ADJUSTMENTS}
          isLoading={isFetchingAdjustments || isSaving}
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
  );
};

const ProfitSharingAdjustments = () => {
  return (
    <Page label={CAPTIONS.PROFIT_SHARING_ADJUSTMENTS}>
      <MissiveAlertProvider>
        <ProfitSharingAdjustmentsContent />
      </MissiveAlertProvider>
    </Page>
  );
};

export default ProfitSharingAdjustments;
