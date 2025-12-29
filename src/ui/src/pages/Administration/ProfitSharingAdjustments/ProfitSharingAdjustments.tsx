import {
  Box,
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  FormControlLabel,
  Grid,
  TextField,
  Typography
} from "@mui/material";
import { CellValueChangedEvent, GridApi, SelectionChangedEvent } from "ag-grid-community";
import StandaloneMemberDetails from "pages/InquiriesAndAdjustments/MasterInquiry/StandaloneMemberDetails";
import { useEffect, useRef, useState } from "react";
import { DSMGrid, Page } from "smart-ui-library";
import { UnsavedChangesDialog } from "../../../components/ConfirmationDialog";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
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
import { GetProfitSharingAdjustmentsGridColumns } from "./ProfitSharingAdjustmentsGridColumns";

const isValidIsoDate = (value: string): boolean => {
  if (!/^\d{4}-\d{2}-\d{2}$/.test(value)) {
    return false;
  }

  const parsed = new Date(`${value}T00:00:00Z`);
  return Number.isFinite(parsed.getTime()) && parsed.toISOString().startsWith(value);
};

const ProfitSharingAdjustmentsContent = () => {
  const [triggerGet, { data, isFetching: isFetchingAdjustments }] = useLazyGetProfitSharingAdjustmentsQuery();
  const [saveAdjustments, { isLoading: isSaving }] = useSaveProfitSharingAdjustmentsMutation();
  const { clearAlerts } = useMissiveAlerts();

  const gridApiRef = useRef<GridApi | null>(null);

  const [profitYear, setProfitYear] = useState<number>(new Date().getFullYear());
  const [badgeNumber, setBadgeNumber] = useState<string>("");
  const [getAllRows, setGetAllRows] = useState<boolean>(false);
  const [loadedGetAllRows, setLoadedGetAllRows] = useState<boolean>(false);

  const [loadedKey, setLoadedKey] = useState<ProfitSharingAdjustmentsKey | null>(null);
  const [rowData, setRowData] = useState<ProfitSharingAdjustmentRowDto[]>([]);
  const [originalByRowNumber, setOriginalByRowNumber] = useState<Record<number, ProfitSharingAdjustmentRowDto>>({});
  const [stagedByRowNumber, setStagedByRowNumber] = useState<Record<number, SaveProfitSharingAdjustmentRowRequest>>({});
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const [selectedRow, setSelectedRow] = useState<ProfitSharingAdjustmentRowDto | null>(null);
  const [isAdjustModalOpen, setIsAdjustModalOpen] = useState<boolean>(false);
  const [adjustmentDraft, setAdjustmentDraft] = useState<{
    contribution: string;
    earnings: string;
    forfeiture: string;
  }>({
    contribution: "0",
    earnings: "0",
    forfeiture: "0"
  });
  const [memberDetailsRefreshTrigger, setMemberDetailsRefreshTrigger] = useState<number>(0);

  const hasUnsavedChanges = Object.keys(stagedByRowNumber).length > 0;
  const { showDialog: showUnsavedDialog, onStay, onLeave } = useUnsavedChangesGuard(hasUnsavedChanges, true);

  useEffect(() => {
    if (!data) {
      return;
    }

    clearAlerts();

    setLoadedKey({ profitYear: data.profitYear, badgeNumber: data.badgeNumber });
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

    // Reset row selection when loading new data.
    setSelectedRow(null);
    gridApiRef.current?.deselectAll();
  }, [data, clearAlerts]);

  const upsertStageForRow = (row: ProfitSharingAdjustmentRowDto) => {
    const original = originalByRowNumber[row.rowNumber];

    const isDraftInsertRow = row.profitDetailId == null;

    const changed = original
      ? row.profitCodeId !== original.profitCodeId ||
        row.contribution !== original.contribution ||
        row.earnings !== original.earnings ||
        row.forfeiture !== original.forfeiture ||
        row.activityDate !== original.activityDate ||
        row.comment !== original.comment
      : isDraftInsertRow &&
        (row.contribution !== 0 ||
          row.earnings !== 0 ||
          row.forfeiture !== 0 ||
          row.activityDate != null ||
          (row.comment ?? "") !== "");

    setStagedByRowNumber((prev) => {
      const next = { ...prev };
      if (!changed) {
        delete next[row.rowNumber];
        return next;
      }

      next[row.rowNumber] = {
        profitDetailId: row.profitDetailId,
        reversedFromProfitDetailId: row.reversedFromProfitDetailId ?? null,
        rowNumber: row.rowNumber,
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

  const columnDefs = GetProfitSharingAdjustmentsGridColumns();

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

    try {
      setLoadedGetAllRows(getAllRows);
      await triggerGet({ profitYear, badgeNumber: parsedBadgeNumber, getAllRows }).unwrap();
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

  const clearSelection = () => {
    setSelectedRow(null);
    gridApiRef.current?.deselectAll();
  };

  const openAdjustModal = () => {
    setErrorMessage(null);

    if (!selectedRow || selectedRow.profitDetailId == null) {
      setErrorMessage("Select an existing row before making an adjustment.");
      return;
    }

    setAdjustmentDraft({
      contribution: String(-selectedRow.contribution),
      earnings: String(-selectedRow.earnings),
      forfeiture: String(-selectedRow.forfeiture)
    });

    setIsAdjustModalOpen(true);
  };

  const applyAdjustmentDraftToInsertRow = () => {
    setErrorMessage(null);

    if (!loadedKey) {
      setErrorMessage("Load adjustments before making an adjustment.");
      return;
    }

    const existingDraft = rowData.find((r) => r.profitDetailId == null);

    const now = new Date();
    const todayIso = now.toISOString().slice(0, 10);

    const maxRowNumber = rowData.reduce((max, r) => Math.max(max, r.rowNumber), 0);
    const rowNumber = existingDraft?.rowNumber ?? maxRowNumber + 1;

    const seedRow = rowData.find((r) => r.profitDetailId != null) ?? selectedRow;

    const draftRow: ProfitSharingAdjustmentRowDto = {
      profitDetailId: null,
      hasBeenReversed: false,
      rowNumber,
      profitYear: loadedKey.profitYear,
      profitYearIteration: 3,
      profitCodeId: seedRow?.profitCodeId ?? 0,
      profitCodeName: seedRow?.profitCodeName ?? "",
      contribution: Number.parseFloat(adjustmentDraft.contribution) || 0,
      earnings: Number.parseFloat(adjustmentDraft.earnings) || 0,
      payment: 0,
      forfeiture: Number.parseFloat(adjustmentDraft.forfeiture) || 0,
      federalTaxes: 0,
      stateTaxes: 0,
      taxCodeId: `${seedRow?.taxCodeId ?? ""}`,
      activityDate: todayIso,
      comment: "ADMINISTRATIVE",
      isEditable: false,
      reversedFromProfitDetailId: selectedRow?.profitDetailId ?? null
    };

    setRowData((prev) => {
      const withoutExistingDraft = prev.filter((r) => r.profitDetailId != null);
      // Put draft row first, then sort remaining rows by profitYear descending
      const next = [draftRow, ...withoutExistingDraft.sort((a, b) => b.profitYear - a.profitYear)];
      return next.map((r, idx) => ({ ...r, rowNumber: idx + 1 }));
    });

    upsertStageForRow({ ...draftRow, rowNumber: 1 }); // Draft is always row 1 now
    setIsAdjustModalOpen(false);

    // Select and scroll to the draft row at the top
    setTimeout(() => {
      const api = gridApiRef.current;
      if (api) {
        const firstNode = api.getDisplayedRowAtIndex(0);
        if (firstNode) {
          firstNode.setSelected(true);
          api.ensureIndexVisible(0, "top");
        }
      }
    }, 0);
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

    upsertStageForRow(row);
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

    const rowsToSave: SaveProfitSharingAdjustmentRowRequest[] = Object.values(stagedByRowNumber);

    if (rowsToSave.length === 0) {
      return;
    }

    for (const row of rowsToSave) {
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

      // Trigger refresh of member details to show updated balance.
      setMemberDetailsRefreshTrigger((prev) => prev + 1);
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

          <FormControlLabel
            control={
              <Checkbox
                checked={getAllRows}
                onChange={(_e, checked) => setGetAllRows(checked)}
                disabled={isFetchingAdjustments || isSaving || hasUnsavedChanges}
              />
            }
            label={
              <Box sx={{ display: "flex", flexDirection: "column" }}>
                <span>Show all rows (ignore under-21 filter)</span>
                <Typography
                  variant="caption"
                  color="text.secondary">
                  Default: only rows where the member is under 21 as of today.
                </Typography>
              </Box>
            }
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
          refreshTrigger={memberDetailsRefreshTrigger}
        />
      )}

      <Grid width="100%">
        <Box
          sx={{
            display: "flex",
            justifyContent: "flex-end",
            gap: 2,
            px: 1,
            flexWrap: "wrap"
          }}>
          <Button
            variant="outlined"
            disabled={!selectedRow || isSaving || isFetchingAdjustments}
            onClick={clearSelection}>
            Clear selection
          </Button>

          <Button
            variant="contained"
            disabled={
              !selectedRow ||
              selectedRow.profitDetailId == null ||
              selectedRow.hasBeenReversed ||
              isSaving ||
              isFetchingAdjustments ||
              rowData.length === 0
            }
            title={selectedRow?.hasBeenReversed ? "This row has already been reversed" : ""}
            onClick={openAdjustModal}>
            Adjust…
          </Button>
        </Box>
      </Grid>

      <Grid width="100%">
        <DSMGrid
          preferenceKey={GRID_KEYS.PROFIT_SHARING_ADJUSTMENTS}
          isLoading={isFetchingAdjustments || isSaving}
          providedOptions={{
            onGridReady: (params) => {
              gridApiRef.current = params.api as GridApi;
            },
            rowData,
            columnDefs,
            suppressMultiSort: true,
            stopEditingWhenCellsLoseFocus: true,
            enterNavigatesVertically: true,
            enterNavigatesVerticallyAfterEdit: true,
            rowSelection: {
              mode: "singleRow",
              checkboxes: true,
              enableClickSelection: false
            },
            onSelectionChanged: ((event: SelectionChangedEvent<ProfitSharingAdjustmentRowDto>) => {
              const selected = event.api.getSelectedNodes().map((n) => n.data ?? null)[0] ?? null;
              setSelectedRow(selected);
            }) as (event: unknown) => void,
            onCellValueChanged,
            getRowStyle: (params) => {
              const data = params.data as ProfitSharingAdjustmentRowDto | undefined;
              if (data?.hasBeenReversed) {
                return { backgroundColor: "#f5f5f5", color: "#9e9e9e" };
              }
              return undefined;
            }
          }}
        />
      </Grid>

      <Dialog
        open={isAdjustModalOpen}
        onClose={() => setIsAdjustModalOpen(false)}
        fullWidth
        maxWidth="sm">
        <DialogTitle>Make adjustment</DialogTitle>
        <DialogContent>
          <Typography
            variant="body2"
            color="text.secondary"
            sx={{ mb: 2 }}>
            Creates an administrative adjustment row (EXT=3) using the values below.
          </Typography>

          <Box
            sx={{
              display: "flex",
              gap: 2,
              flexWrap: "wrap"
            }}>
            <TextField
              label="Contribution"
              size="small"
              type="text"
              value={adjustmentDraft.contribution}
              onChange={(e) =>
                setAdjustmentDraft((prev) => ({
                  ...prev,
                  contribution: e.target.value
                }))
              }
              inputProps={{ inputMode: "decimal" }}
              sx={{ width: 180 }}
            />
            <TextField
              label="Earnings"
              size="small"
              type="text"
              value={adjustmentDraft.earnings}
              onChange={(e) =>
                setAdjustmentDraft((prev) => ({
                  ...prev,
                  earnings: e.target.value
                }))
              }
              inputProps={{ inputMode: "decimal" }}
              sx={{ width: 180 }}
            />
            <TextField
              label="Forfeiture"
              size="small"
              type="text"
              value={adjustmentDraft.forfeiture}
              onChange={(e) =>
                setAdjustmentDraft((prev) => ({
                  ...prev,
                  forfeiture: e.target.value
                }))
              }
              inputProps={{ inputMode: "decimal" }}
              sx={{ width: 180 }}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button
            variant="outlined"
            onClick={() => setIsAdjustModalOpen(false)}>
            Cancel
          </Button>
          <Button
            variant="contained"
            onClick={applyAdjustmentDraftToInsertRow}>
            Apply
          </Button>
        </DialogActions>
      </Dialog>

      {/* Unsaved changes navigation guard dialog */}
      <UnsavedChangesDialog
        open={showUnsavedDialog}
        onStay={onStay}
        onLeave={onLeave}
      />
    </Grid>
  );
};

const ProfitSharingAdjustments = () => {
  return (
    <PageErrorBoundary pageName="Profit Sharing Adjustments">
      <Page label={CAPTIONS.PROFIT_SHARING_ADJUSTMENTS}>
        <MissiveAlertProvider>
          <ProfitSharingAdjustmentsContent />
        </MissiveAlertProvider>
      </Page>
    </PageErrorBoundary>
  );
};

export default ProfitSharingAdjustments;
