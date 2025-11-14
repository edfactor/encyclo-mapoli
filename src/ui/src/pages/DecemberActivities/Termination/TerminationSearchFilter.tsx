import { yupResolver } from "@hookform/resolvers/yup";
import {
  Box,
  Button,
  Checkbox,
  FormControlLabel,
  FormHelperText,
  Grid,
  InputLabel,
  MenuItem,
  TextField
} from "@mui/material";
import React, { useState } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { DSMDatePicker, SearchAndReset, SmartModal } from "smart-ui-library";
import * as yup from "yup";
import { ConfirmationDialog } from "../../../components/ConfirmationDialog";
import DuplicateSsnGuard from "../../../components/DuplicateSsnGuard";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { clearTermination } from "../../../reduxstore/slices/yearsEndSlice";
import { RootState } from "../../../reduxstore/store";
import { CalendarResponseDto } from "../../../reduxstore/types";
import { mmDDYYFormat, tryddmmyyyyToDate } from "../../../utils/dateUtils";
import {
  dateStringValidator,
  endDateStringAfterStartDateValidator,
  profitYearValidator
} from "../../../utils/FormValidators";
import { TerminationSearchRequest } from "./Termination";

const schema = yup.object().shape({
  beginningDate: dateStringValidator(2000, 2099, "Beginning Date").required("Beginning Date is required"),
  endingDate: endDateStringAfterStartDateValidator(
    "beginningDate",
    tryddmmyyyyToDate,
    "Ending date must be the same or after the beginning date"
  ).required("Ending Date is required"),
  forfeitureStatus: yup.string().required("Forfeiture Status is required"),
  pagination: yup
    .object({
      skip: yup.number().required(),
      take: yup.number().required(),
      sortBy: yup.string().required(),
      isSortDescending: yup.boolean().required()
    })
    .required(),
  profitYear: profitYearValidator(2015, 2099),
  excludeZeroAndFullyVested: yup.boolean(),
  vestedBalanceValue: yup
    .number()
    .nullable()
    .min(0, "Vested Balance must be 0 or greater")
    .transform((value, originalValue) => (originalValue === "" ? null : value)),
  vestedBalanceOperator: yup
    .number()
    .nullable()
    .when("vestedBalanceValue", {
      is: (val: number | null | undefined) => val !== null && val !== undefined,
      then: (schema) => schema.required("Operator is required when Vested Balance is provided"),
      otherwise: (schema) => schema.nullable()
    })
});

interface TerminationSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
  fiscalData: CalendarResponseDto | null;
  onSearch: (params: TerminationSearchRequest) => void;
  hasUnsavedChanges?: boolean;
  setHasUnsavedChanges?: (hasChanges: boolean) => void;
  isFetching?: boolean;
}

const TerminationSearchFilter: React.FC<TerminationSearchFilterProps> = ({
  setInitialSearchLoaded,
  fiscalData,
  onSearch,
  hasUnsavedChanges,
  setHasUnsavedChanges,
  isFetching = false
}) => {
  const [openErrorModal, setOpenErrorModal] = useState(!fiscalData === false);
  const [showUnsavedChangesDialog, setShowUnsavedChangesDialog] = useState(false);
  const dispatch = useDispatch();
  const selectedProfitYear = useDecemberFlowProfitYear();
  const { termination } = useSelector((state: RootState) => state.yearsEnd);
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<TerminationSearchRequest>({
    resolver: yupResolver(schema) as Resolver<TerminationSearchRequest>,
    defaultValues: {
      beginningDate: termination?.startDate || (fiscalData ? mmDDYYFormat(fiscalData.fiscalBeginDate) : "") || "",
      endingDate: termination?.endDate || (fiscalData ? mmDDYYFormat(fiscalData.fiscalEndDate) : "") || "",
      forfeitureStatus: "showAll",
      pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false },
      profitYear: selectedProfitYear,
      excludeZeroAndFullyVested: false,
      vestedBalanceValue: null,
      vestedBalanceOperator: 0
    }
  });

  const validateAndSubmit = async (data: TerminationSearchRequest) => {
    if (hasUnsavedChanges) {
      setShowUnsavedChangesDialog(true);
      return;
    }

    const params = {
      ...data,
      profitYear: selectedProfitYear,
      beginningDate: data.beginningDate
        ? mmDDYYFormat(data.beginningDate)
        : mmDDYYFormat(fiscalData?.fiscalBeginDate || ""),
      endingDate: data.endingDate ? mmDDYYFormat(data.endingDate) : mmDDYYFormat(fiscalData?.fiscalEndDate || "")
    };

    // Only include vested balance fields if both are provided
    if (data.vestedBalanceValue === null || data.vestedBalanceOperator === null) {
      delete params.vestedBalanceValue;
      delete params.vestedBalanceOperator;
    }

    // Only update search params and initial loaded state; let the grid trigger the API
    onSearch(params);
    setInitialSearchLoaded(true);
  };

  const validateAndSearch = handleSubmit(validateAndSubmit as (data: TerminationSearchRequest) => Promise<void>);

  const handleReset = async () => {
    setHasUnsavedChanges?.(false);
    setInitialSearchLoaded(false);
    reset({
      beginningDate: fiscalData ? mmDDYYFormat(fiscalData.fiscalBeginDate) : "",
      endingDate: fiscalData ? mmDDYYFormat(fiscalData.fiscalEndDate) : "",
      forfeitureStatus: "showAll",
      pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false },
      profitYear: selectedProfitYear,
      excludeZeroAndFullyVested: false,
      vestedBalanceValue: null,
      vestedBalanceOperator: 0
    });
    // Trigger validation after reset to ensure form validity is updated
    await trigger();
    dispatch(clearTermination());
  };

  if (!fiscalData) {
    return (
      <SmartModal
        key={"fiscalDataErrorModal"}
        open={openErrorModal}
        maxWidth="sm"
        title="Services Error"
        onClose={() => setOpenErrorModal(false)}
        message={`Fiscal date range not available. Please try again later.\n`}>
        <Button
          onClick={() => setOpenErrorModal(false)}
          variant="outlined"
          sx={{ mt: "15px" }}>
          OK
        </Button>
      </SmartModal>
    );
  }

  return (
    <form onSubmit={validateAndSearch}>
      <Grid
        container
        paddingX="24px"
        columnSpacing={2}
        rowSpacing={1}
        alignItems="flex-end">
        <Grid size={{ xs: 12, sm: 6, md: 2.5 }}>
          <Controller
            name="beginningDate"
            control={control}
            render={({ field }) => (
              <DSMDatePicker
                id="beginningDate"
                onChange={(value: Date | null) => {
                  field.onChange(value ? mmDDYYFormat(value) : undefined);
                  trigger("beginningDate");
                  if (value) {
                    trigger("endingDate");
                  }
                }}
                value={field.value ? tryddmmyyyyToDate(field.value) : null}
                required={false}
                label="Begin Date"
                disableFuture
                error={errors.beginningDate?.message}
                minDate={tryddmmyyyyToDate(fiscalData.fiscalBeginDate) || undefined}
                maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate) || undefined}
              />
            )}
          />
          {errors.beginningDate && <FormHelperText error>{errors.beginningDate.message}</FormHelperText>}
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 2.5 }}>
          <Controller
            name="endingDate"
            control={control}
            render={({ field }) => (
              <DSMDatePicker
                id="endingDate"
                onChange={(value: Date | null) => {
                  field.onChange(value || undefined);
                  trigger("endingDate");
                }}
                value={field.value ? tryddmmyyyyToDate(field.value) : null}
                required={false}
                label="End Date"
                disableFuture
                error={errors.endingDate?.message}
                minDate={tryddmmyyyyToDate(fiscalData.fiscalBeginDate) || undefined}
                maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate) || undefined}
              />
            )}
          />
          {errors.endingDate && <FormHelperText error>{errors.endingDate.message}</FormHelperText>}
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 2.5 }}>
          <InputLabel sx={{ mb: 1 }}>Vested Balance</InputLabel>
          <Box sx={{ display: "flex", gap: 1 }}>
            <Controller
              name="vestedBalanceOperator"
              control={control}
              render={({ field }) => (
                <TextField
                  select
                  fullWidth
                  value={field.value ?? ""}
                  onChange={(e) => {
                    const value = e.target.value === "" ? null : Number(e.target.value);
                    field.onChange(value);
                  }}
                  onBlur={field.onBlur}
                  name={field.name}
                  inputRef={field.ref}>
                  <MenuItem value=""></MenuItem>
                  <MenuItem value={0}>=</MenuItem>
                  <MenuItem value={1}>&lt;</MenuItem>
                  <MenuItem value={2}>&lt;=</MenuItem>
                  <MenuItem value={3}>&gt;</MenuItem>
                  <MenuItem value={4}>&gt;=</MenuItem>
                </TextField>
              )}
            />
            <Controller
              name="vestedBalanceValue"
              control={control}
              render={({ field }) => (
                <TextField
                  fullWidth
                  type="number"
                  placeholder="Amount"
                  value={field.value ?? ""}
                  onChange={(e) => {
                    const value = e.target.value === "" ? null : Number(e.target.value);
                    field.onChange(value);
                  }}
                  onBlur={field.onBlur}
                  name={field.name}
                  inputRef={field.ref}
                  inputProps={{
                    min: 0,
                    step: 1
                  }}
                  error={!!errors.vestedBalanceValue}
                  helperText={errors.vestedBalanceValue?.message}
                />
              )}
            />
          </Box>
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 6 }}>
          <Controller
            name="excludeZeroAndFullyVested"
            control={control}
            render={({ field }) => (
              <FormControlLabel
                control={
                  <Checkbox
                    checked={field.value || false}
                    onChange={(e) => field.onChange(e.target.checked)}
                  />
                }
                label="Exclude members with: $0 Ending Balance, 100% Vested, or Forfeited"
              />
            )}
          />
        </Grid>
      </Grid>
      <Grid
        width="100%"
        paddingX="24px">
        <DuplicateSsnGuard mode="warning">
          {({ prerequisitesComplete }) => (
            <SearchAndReset
              handleReset={handleReset}
              handleSearch={validateAndSearch}
              isFetching={isFetching}
              disabled={!isValid || !prerequisitesComplete || isFetching}
            />
          )}
        </DuplicateSsnGuard>
      </Grid>

      <ConfirmationDialog
        open={showUnsavedChangesDialog}
        title="Unsaved Changes"
        description="Please save your changes before performing a new search."
        onClose={() => setShowUnsavedChangesDialog(false)}
      />
    </form>
  );
};

export default TerminationSearchFilter;
