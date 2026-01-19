import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  Grid,
  TextField,
  Typography
} from "@mui/material";
import { CellValueChangedEvent, ColDef, ValueFormatterParams, ValueParserParams } from "ag-grid-community";
import { useEffect, useMemo, useState } from "react";
import { useDispatch } from "react-redux";
import { ApiMessageAlert, DSMGrid, Page } from "smart-ui-library";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import { CAPTIONS, GRID_KEYS } from "../../../constants";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import {
  useCreateAnnuityRatesMutation,
  useGetAnnuityRatesQuery,
  useUpdateAnnuityRateMutation
} from "../../../reduxstore/api/ItOperationsApi";
import { setMessage } from "../../../reduxstore/slices/messageSlice";
import { AnnuityRateDto, AnnuityRateInputRequest } from "../../../reduxstore/types";
import { mmDDYYFormat } from "../../../utils/dateUtils";
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
  const [createAnnuityRates, { isLoading: isCreating }] = useCreateAnnuityRatesMutation();

  const [rowData, setRowData] = useState<AnnuityRateDto[]>([]);
  const [originalRatesByKey, setOriginalRatesByKey] = useState<Record<string, StagedAnnuityRateChange>>({});
  const [stagedRatesByKey, setStagedRatesByKey] = useState<Record<string, StagedAnnuityRateChange>>({});
  const [isCopyModalOpen, setIsCopyModalOpen] = useState(false);
  const [copyYear, setCopyYear] = useState<number | "">("");
  const [copySourceYear, setCopySourceYear] = useState<number | null>(null);
  const [copyRates, setCopyRates] = useState<AnnuityRateInputRequest[]>([]);

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
      },
      {
        headerName: "User Modified",
        field: "userModified",
        sortable: true,
        filter: false,
        editable: false,
        width: 150
      },
      {
        headerName: "Date Modified",
        field: "dateModified",
        sortable: true,
        filter: false,
        editable: false,
        width: 150,
        valueFormatter: (params: ValueFormatterParams) => {
          return params.value ? mmDDYYFormat(params.value) : "";
        }
      }
    ];
  }, []);

  const copyColumnDefs = useMemo<ColDef[]>(() => {
    return [
      {
        headerName: "Age",
        field: "age",
        sortable: true,
        filter: false,
        editable: false,
        width: 70
      },
      {
        headerName: "Single Rate",
        field: "singleRate",
        sortable: false,
        filter: false,
        editable: true,
        width: 120,
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
        sortable: false,
        filter: false,
        editable: true,
        width: 120,
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

  const openCopyModal = () => {
    if (!data || data.length === 0) {
      dispatch(
        setMessage({
          ...Messages.AnnuityRatesSaveError,
          message: {
            ...Messages.AnnuityRatesSaveError.message,
            message: "No prior year annuity rates are available to copy."
          }
        })
      );
      return;
    }

    const latestYear = Math.max(...data.map((rate) => rate.year));
    const lastYearRates = data
      .filter((rate) => rate.year === latestYear)
      .sort((a, b) => a.age - b.age)
      .map((rate) => ({
        age: rate.age,
        singleRate: rate.singleRate,
        jointRate: rate.jointRate
      }));

    if (lastYearRates.length === 0) {
      dispatch(
        setMessage({
          ...Messages.AnnuityRatesSaveError,
          message: {
            ...Messages.AnnuityRatesSaveError.message,
            message: `No annuity rates were found for ${latestYear}.`
          }
        })
      );
      return;
    }

    setCopySourceYear(latestYear);
    setCopyYear(latestYear + 1);
    setCopyRates(lastYearRates.map((rate) => ({ ...rate })));
    setIsCopyModalOpen(true);
  };

  const closeCopyModal = () => {
    if (isCreating) return;
    setIsCopyModalOpen(false);
  };

  const onCopyCellValueChanged = (event: CellValueChangedEvent) => {
    const row = event.data as AnnuityRateInputRequest | undefined;
    if (!row) return;

    setCopyRates((prev) =>
      prev.map((rate) =>
        rate.age === row.age
          ? {
              ...rate,
              singleRate: row.singleRate,
              jointRate: row.jointRate
            }
          : rate
      )
    );
  };

  const saveCopiedRates = async () => {
    const yearValue = typeof copyYear === "number" ? copyYear : Number.parseInt(String(copyYear), 10);

    if (!Number.isFinite(yearValue) || yearValue < 1900 || yearValue > 2100) {
      dispatch(
        setMessage({
          ...Messages.AnnuityRatesCreateError,
          message: {
            ...Messages.AnnuityRatesCreateError.message,
            message: "Year must be between 1900 and 2100."
          }
        })
      );
      return;
    }

    if (!copyRates.length) {
      dispatch(
        setMessage({
          ...Messages.AnnuityRatesCreateError,
          message: {
            ...Messages.AnnuityRatesCreateError.message,
            message: "Rates are required."
          }
        })
      );
      return;
    }

    for (const rate of copyRates) {
      if (rate.singleRate < 0 || rate.singleRate > 99.9999) {
        dispatch(
          setMessage({
            ...Messages.AnnuityRatesCreateError,
            message: {
              ...Messages.AnnuityRatesCreateError.message,
              message: `Single Rate must be between 0 and 99.9999 (age ${rate.age}).`
            }
          })
        );
        return;
      }

      if (hasMoreThanFourDecimals(rate.singleRate)) {
        dispatch(
          setMessage({
            ...Messages.AnnuityRatesCreateError,
            message: {
              ...Messages.AnnuityRatesCreateError.message,
              message: `Single Rate can have at most 4 decimal places (age ${rate.age}).`
            }
          })
        );
        return;
      }

      if (rate.jointRate < 0 || rate.jointRate > 99.9999) {
        dispatch(
          setMessage({
            ...Messages.AnnuityRatesCreateError,
            message: {
              ...Messages.AnnuityRatesCreateError.message,
              message: `Joint Rate must be between 0 and 99.9999 (age ${rate.age}).`
            }
          })
        );
        return;
      }

      if (hasMoreThanFourDecimals(rate.jointRate)) {
        dispatch(
          setMessage({
            ...Messages.AnnuityRatesCreateError,
            message: {
              ...Messages.AnnuityRatesCreateError.message,
              message: `Joint Rate can have at most 4 decimal places (age ${rate.age}).`
            }
          })
        );
        return;
      }
    }

    try {
      await createAnnuityRates({
        year: yearValue,
        rates: copyRates.map((rate) => ({
          age: rate.age,
          singleRate: normalizeRateToFourDecimals(rate.singleRate),
          jointRate: normalizeRateToFourDecimals(rate.jointRate)
        }))
      }).unwrap();

      setIsCopyModalOpen(false);
      setCopyRates([]);
      setCopySourceYear(null);
      setCopyYear("");
      await refetch();

      dispatch(setMessage(Messages.AnnuityRatesCreateSuccess));
    } catch (e) {
      console.error("Failed to create annuity rates", e);
      dispatch(setMessage(Messages.AnnuityRatesCreateError));
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
                  variant="outlined"
                  disabled={isSaving || isCreating}
                  onClick={openCopyModal}>
                  Copy From Last Year
                </Button>
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
      <Dialog
        open={isCopyModalOpen}
        onClose={closeCopyModal}
        maxWidth="md"
        fullWidth>
        <DialogTitle sx={{ fontWeight: "bold" }}>Copy Annuity Rates</DialogTitle>
        <DialogContent>
          <Typography sx={{ mb: 2 }}>
            {copySourceYear ? `Copying rates from ${copySourceYear}.` : "Select a year to copy rates from."}
          </Typography>
          <Box sx={{ display: "flex", gap: 2, alignItems: "center", mb: 2 }}>
            <TextField
              label="New Year"
              value={copyYear}
              onChange={(event) => {
                const rawValue = event.target.value;
                if (rawValue === "") {
                  setCopyYear("");
                  return;
                }

                if (/^\d+$/.test(rawValue)) {
                  setCopyYear(Number.parseInt(rawValue, 10));
                }
              }}
              inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
              size="small"
            />
          </Box>
          <DSMGrid
            preferenceKey={`${GRID_KEYS.MANAGE_ANNUITY_RATES}-copy`}
            isLoading={isCreating}
            providedOptions={{
              rowData: copyRates,
              columnDefs: copyColumnDefs,
              suppressMultiSort: true,
              stopEditingWhenCellsLoseFocus: true,
              enterNavigatesVertically: true,
              enterNavigatesVerticallyAfterEdit: true,
              onCellValueChanged: onCopyCellValueChanged
            }}
          />
        </DialogContent>
        <DialogActions sx={{ padding: "16px 24px" }}>
          <Button
            variant="outlined"
            onClick={closeCopyModal}
            disabled={isCreating}>
            Cancel
          </Button>
          <Button
            variant="contained"
            onClick={saveCopiedRates}
            disabled={isCreating}>
            Create
          </Button>
        </DialogActions>
      </Dialog>
    </PageErrorBoundary>
  );
};

export default ManageAnnuityRates;
