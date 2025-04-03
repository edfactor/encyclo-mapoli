import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { Controller, useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useLazyGetProfitShareEditQuery, useLazyGetProfitShareUpdateQuery } from "reduxstore/api/YearsEndApi";
import * as yup from "yup";

import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useState } from "react";
import {
  clearProfitSharingEdit,
  clearProfitSharingEditQueryParams,
  clearProfitSharingUpdate,
  clearProfitSharingUpdateQueryParams,
  setProfitSharingEditQueryParams,
  setProfitSharingUpdateQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { ProfitShareEditUpdateQueryParams, ProfitShareUpdateRequest } from "reduxstore/types";
import { ISortParams } from "smart-ui-library";
import SearchAndReset from "components/SearchAndReset/SearchAndReset";

const schema = yup.object().shape({
  profitYear: yup
    .date()
    .required("Profit Year is required")
    .min(new Date(2020, 0, 1), "Year must be 2020 or later")
    .max(new Date(2100, 11, 31), "Year must be 2100 or earlier")
    .typeError("Invalid date"),
  contributionPercent: yup
    .number()
    .typeError("Contribution must be a number")
    .min(0, "Contribution must be positive")
    .nullable(),
  earningsPercent: yup.number().typeError("Earnings must be a number").min(0, "Earnings must be positive").nullable(),
  secondaryEarningsPercent: yup
    .number()
    .typeError("Secondary Earnings must be a number")
    .min(0, "Secondary Earnings must be positive")
    .nullable(),
  incomingForfeitPercent: yup
    .number()
    .typeError("Incoming Forfeiture must be a number")
    .min(0, "Forfeiture must be positive")
    .nullable(),
  maxAllowedContributions: yup
    .number()
    .typeError("Max Allowed Contributions must be a number")
    .min(0, "Max Allowed Contributions must be positive")
    .nullable(),
  badgeToAdjust: yup.number().typeError("Badge must be a number").integer("Badge must be an integer").nullable(),
  adjustContributionAmount: yup
    .number()
    .typeError("Contribution must be a number")
    .min(0, "Contribution must be positive")
    .nullable(),
  adjustEarningsAmount: yup.number().typeError("Earnings must be a number").nullable(),
  adjustIncomingForfeitureAmount: yup
    .number()
    .typeError("Adjusted Incoming Forfeiture must be a number")
    .min(0, "Adjusted Incoming Forfeiture must be positive")
    .nullable(),
  badgeToAdjust2: yup.number().typeError("Badge must be a number").integer("Badge must be an integer").nullable(),
  adjustEarningsSecondaryAmount: yup.number().typeError("Earnings must be a number").nullable()
});

const ProfitShareEditUpdateSearchFilter = () => {
  const [triggerSearchUpdate, { isFetching: isFetchingUpdate }] = useLazyGetProfitShareUpdateQuery();
  const [triggerSearchEdit, { isFetching: isFetchingEdit }] = useLazyGetProfitShareEditQuery();
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "contributionPercent",
    isSortDescending: false
  });

  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();

  const fiscalCloseProfitYearAsDate = new Date(fiscalCloseProfitYear, 0, 1);

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<ProfitShareEditUpdateQueryParams>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: fiscalCloseProfitYearAsDate,
      contributionPercent: null,
      earningsPercent: null,
      incomingForfeitPercent: null,
      secondaryEarningsPercent: null,
      maxAllowedContributions: null,

      badgeToAdjust: null,
      adjustContributionAmount: null,
      adjustEarningsAmount: null,
      adjustIncomingForfeitAmount: null,

      badgeToAdjust2: null,
      adjustEarningsSecondaryAmount: null
    }
  });

  const validateAndSearch = handleSubmit((data: ProfitShareEditUpdateQueryParams, event?: React.BaseSyntheticEvent) => {
    if (isValid) {
      const updateParams: ProfitShareUpdateRequest = {
        pagination: {
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending,
          skip: 0,
          take: 25
        },
        profitYear: fiscalCloseProfitYear,
        contributionPercent: data.contributionPercent ?? 0,
        earningsPercent: data.earningsPercent ?? 0,
        incomingForfeitPercent: data.incomingForfeitPercent ?? 0,
        secondaryEarningsPercent: data.secondaryEarningsPercent ?? 0,
        maxAllowedContributions: data.maxAllowedContributions ?? 0,
        badgeToAdjust: data.badgeToAdjust ?? 0,
        adjustContributionAmount: data.adjustContributionAmount ?? 0,
        adjustEarningsAmount: data.adjustEarningsAmount ?? 0,
        adjustIncomingForfeitAmount: data.adjustEarningsSecondaryAmount ?? 0,
        badgeToAdjust2: data.badgeToAdjust2 ?? 0,
        adjustEarningsSecondaryAmount: data.adjustEarningsSecondaryAmount ?? 0
      };

      // First we have to do the update calls
      triggerSearchUpdate(updateParams, false).unwrap();
      dispatch(setProfitSharingUpdateQueryParams(data));
      console.log("Successfully did the update");

      // Now we have to do the edit calls
      triggerSearchEdit(updateParams, false).unwrap();
      dispatch(setProfitSharingEditQueryParams(data));
      console.log("Successfully did the edit");
    }
  });

  const handleReset = () => {
    // We need to clear both grids and then both sets of query params
    dispatch(clearProfitSharingEdit());
    dispatch(clearProfitSharingUpdate());
    dispatch(clearProfitSharingEditQueryParams());
    dispatch(clearProfitSharingUpdateQueryParams());

    reset({
      profitYear: fiscalCloseProfitYearAsDate,
      contributionPercent: null,
      earningsPercent: null,
      incomingForfeitPercent: null,
      secondaryEarningsPercent: null,
      maxAllowedContributions: null,

      badgeToAdjust: null,
      adjustContributionAmount: null,
      adjustEarningsAmount: null,
      adjustIncomingForfeitAmount: null,

      badgeToAdjust2: null,
      adjustEarningsSecondaryAmount: null
    });
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Grid2
        container
        paddingX="24px">
        <Grid2
          container
          spacing={3}
          width="100%">
          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <Controller
              name="profitYear"
              control={control}
              render={({ field }) => (
                <DsmDatePicker
                  id="Beginning Year"
                  onChange={(value: Date | null) => field.onChange(value)}
                  value={field.value ?? null}
                  required={true}
                  label="Profit Year"
                  disableFuture
                  views={["year"]}
                  error={errors.profitYear?.message}
                  disabled={true}
                />
              )}
            />
            {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 4, md: 2 }}>
            <FormLabel>Contribution %</FormLabel>
            <Controller
              name="contributionPercent"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.contributionPercent}
                />
              )}
            />
            {errors.contributionPercent && <FormHelperText error>{errors.contributionPercent.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Earnings %</FormLabel>
            <Controller
              name="earningsPercent"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.earningsPercent}
                />
              )}
            />
            {errors.earningsPercent && <FormHelperText error>{errors.earningsPercent.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Forfeiture %</FormLabel>
            <Controller
              name="incomingForfeitPercent"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.incomingForfeitPercent}
                />
              )}
            />
            {errors.incomingForfeitPercent && (
              <FormHelperText error>{errors.incomingForfeitPercent.message}</FormHelperText>
            )}
          </Grid2>
          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Secondary Earnings %</FormLabel>
            <Controller
              name="secondaryEarningsPercent"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.secondaryEarningsPercent}
                />
              )}
            />
            {errors.secondaryEarningsPercent && (
              <FormHelperText error>{errors.secondaryEarningsPercent.message}</FormHelperText>
            )}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Max Allowed Contributions</FormLabel>
            <Controller
              name="maxAllowedContributions"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.maxAllowedContributions}
                />
              )}
            />
            {errors.maxAllowedContributions && (
              <FormHelperText error>{errors.maxAllowedContributions.message}</FormHelperText>
            )}
          </Grid2>
        </Grid2>

        <Grid2
          container
          spacing={3}
          width="100%">
          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Adjustment Badge</FormLabel>
            <Controller
              name="badgeToAdjust"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.badgeToAdjust}
                />
              )}
            />
            {errors.badgeToAdjust && <FormHelperText error>{errors.badgeToAdjust.message}</FormHelperText>}
          </Grid2>
          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Adjust Contribution Amount</FormLabel>
            <Controller
              name="adjustContributionAmount"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.adjustContributionAmount}
                />
              )}
            />
            {errors.adjustContributionAmount && (
              <FormHelperText error>{errors.adjustContributionAmount.message}</FormHelperText>
            )}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Adjust Earnings Amount</FormLabel>
            <Controller
              name="adjustEarningsAmount"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.adjustEarningsAmount}
                />
              )}
            />
            {errors.adjustEarningsAmount && (
              <FormHelperText error>{errors.adjustEarningsAmount.message}</FormHelperText>
            )}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Adjust Forfeiture Amount</FormLabel>
            <Controller
              name="adjustIncomingForfeitAmount"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.adjustIncomingForfeitAmount}
                />
              )}
            />
            {errors.adjustIncomingForfeitAmount && (
              <FormHelperText error>{errors.adjustIncomingForfeitAmount.message}</FormHelperText>
            )}
          </Grid2>
        </Grid2>

        <Grid2
          container
          spacing={3}
          width="100%">
          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Adjust Secondary Badge</FormLabel>
            <Controller
              name="badgeToAdjust2"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.badgeToAdjust2}
                />
              )}
            />
            {errors.badgeToAdjust2 && <FormHelperText error>{errors.badgeToAdjust2.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel>Adjust Secondary Earnings Amount</FormLabel>
            <Controller
              name="adjustEarningsSecondaryAmount"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.adjustEarningsSecondaryAmount}
                />
              )}
            />
            {errors.adjustEarningsSecondaryAmount && (
              <FormHelperText error>{errors.adjustEarningsSecondaryAmount.message}</FormHelperText>
            )}
          </Grid2>
        </Grid2>
        <Grid2 width="100%">
          <SearchAndReset
            handleReset={handleReset}
            searchButtonText="Preview"
            handleSearch={validateAndSearch}
            isFetching={isFetchingUpdate || isFetchingEdit}
          />
        </Grid2>
      </Grid2>
    </form>
  );
};

export default ProfitShareEditUpdateSearchFilter;
