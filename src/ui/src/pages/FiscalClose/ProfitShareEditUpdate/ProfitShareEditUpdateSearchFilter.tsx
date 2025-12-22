import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, TextField, Tooltip } from "@mui/material";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useCallback, useEffect, useMemo, useState } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetProfitShareEditQuery, useLazyGetProfitShareUpdateQuery } from "reduxstore/api/YearsEndApi";
import {
  addBadgeNumberToUpdateAdjustmentSummary,
  clearProfitSharingEdit,
  clearProfitSharingEditQueryParams,
  clearProfitSharingUpdate,
  clearProfitSharingUpdateQueryParams,
  setProfitEditUpdateChangesAvailable,
  setProfitSharingEditQueryParams,
  setProfitSharingUpdateQueryParams,
  setResetYearEndPage,
  setTotalForfeituresGreaterThanZero,
  updateProfitSharingEditQueryParam
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { ProfitShareUpdateRequest } from "reduxstore/types";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { badgeNumberValidator, positiveNumberValidator, profitYearDateValidator } from "../../../utils/FormValidators";

const maxContributionsDefault: number = 76000;

interface ProfitShareEditUpdateSearch {
  profitYear: Date;
  contributionPercent?: number | null | undefined;
  earningsPercent?: number | null | undefined;
  secondaryEarningsPercent?: number | null | undefined;
  incomingForfeitPercent?: number | null | undefined;
  maxAllowedContributions: number | null | undefined;
  badgeToAdjust?: number | null | undefined;
  adjustContributionAmount?: number | null | undefined;
  adjustEarningsAmount?: number | null | undefined;
  adjustIncomingForfeitAmount?: number | null | undefined;
  badgeToAdjust2?: number | null | undefined;
  adjustEarningsSecondaryAmount?: number | null | undefined;
}

const schema = yup.object().shape({
  profitYear: profitYearDateValidator,
  contributionPercent: positiveNumberValidator("Contribution").optional(),
  earningsPercent: positiveNumberValidator("Earnings").optional(),
  secondaryEarningsPercent: positiveNumberValidator("Secondary Earnings").optional(),
  incomingForfeitPercent: positiveNumberValidator("Incoming Forfeiture").optional(),
  maxAllowedContributions: positiveNumberValidator("Max Allowed Contributions")
    .default(maxContributionsDefault)
    .required("Max Contributions is required"),
  badgeToAdjust: badgeNumberValidator.optional(),
  adjustContributionAmount: positiveNumberValidator("Contribution").optional(),
  adjustEarningsAmount: yup.number().typeError("Earnings must be a number").nullable().optional(),
  adjustIncomingForfeitAmount: positiveNumberValidator("Adjusted Incoming Forfeiture").optional(),
  badgeToAdjust2: badgeNumberValidator.optional(),
  adjustEarningsSecondaryAmount: yup.number().typeError("Earnings must be a number").nullable().optional()
});

interface ProfitShareEditUpdateSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
  setPageReset: (reset: boolean) => void;
  setMinimumFieldsEntered?: (entered: boolean) => void;
  setAdjustedBadgeOneValid?: (valid: boolean) => void;
  setAdjustedBadgeTwoValid?: (valid: boolean) => void;
}

const ProfitShareEditUpdateSearchFilter: React.FC<ProfitShareEditUpdateSearchFilterProps> = ({
  setInitialSearchLoaded,
  setPageReset,
  setMinimumFieldsEntered,
  setAdjustedBadgeOneValid,
  setAdjustedBadgeTwoValid
}) => {
  const [triggerSearchUpdate, { isFetching: isFetchingUpdate }] = useLazyGetProfitShareUpdateQuery();
  const [triggerSearchEdit, { isFetching: isFetchingEdit }] = useLazyGetProfitShareEditQuery();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const { profitSharingUpdate, profitSharingEdit, resetYearEndPage } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();

  const fiscalCloseProfitYearAsDate = useMemo(() => new Date(fiscalCloseProfitYear, 0, 1), [fiscalCloseProfitYear]);

  useEffect(() => {
    if (!isFetchingUpdate && !isFetchingEdit) {
      setIsSubmitting(false);
    }
  }, [isFetchingUpdate, isFetchingEdit]);

  useEffect(() => {
    if (fiscalCloseProfitYear && !profitSharingUpdate && !profitSharingEdit) {
      setInitialSearchLoaded(true);
    }
  }, [fiscalCloseProfitYear, profitSharingUpdate, profitSharingEdit, setInitialSearchLoaded]);

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<ProfitShareEditUpdateSearch>({
    resolver: yupResolver(schema) as Resolver<ProfitShareEditUpdateSearch>,
    defaultValues: {
      profitYear: fiscalCloseProfitYearAsDate,
      contributionPercent: 15.0,
      earningsPercent: 9.280136,
      incomingForfeitPercent: 0.876678,
      secondaryEarningsPercent: null,
      maxAllowedContributions: 57000,

      badgeToAdjust: null,
      adjustContributionAmount: null,
      adjustEarningsAmount: null,
      adjustIncomingForfeitAmount: null,

      badgeToAdjust2: null,
      adjustEarningsSecondaryAmount: null
    }
  });

  const [contributionPercentPresent, setContributionPercentPresent] = useState(false);
  const [earningsPercentPresent, setEarningsPercentPresent] = useState(false);
  // This starts as true as it is pre-populated
  const [maxAllowedContributionsPresent, setMaxAllowedContributionsPresent] = useState(true);
  const [badgeToAdjustPresent, setBadgeToAdjustPresent] = useState(false);
  const [adjustContributionAmountPresent, setAdjustContributionAmountPresent] = useState(false);
  const [adjustEarningsAmountPresent, setAdjustEarningsAmountPresent] = useState(false);
  const [adjustIncomingForfeitAmountPresent, setAdjustIncomingForfeitAmountPresent] = useState(false);
  const [badgeToAdjust2Present, setBadgeToAdjust2Present] = useState(false);
  const [adjustEarningsSecondaryAmountPresent, setAdjustEarningsSecondaryAmountPresent] = useState(false);

  const processFieldsEntered = (fieldName: keyof ProfitShareEditUpdateSearch, value: boolean) => {
    // set the value coming in to the field
    switch (fieldName) {
      case "contributionPercent":
        setContributionPercentPresent(value);

        break;
      case "earningsPercent":
        setEarningsPercentPresent(value);
        break;
      case "maxAllowedContributions":
        setMaxAllowedContributionsPresent(value);
        break;
      case "badgeToAdjust":
        setBadgeToAdjustPresent(value);
        break;
      case "adjustContributionAmount":
        setAdjustContributionAmountPresent(value);
        break;
      case "adjustEarningsAmount":
        setAdjustEarningsAmountPresent(value);
        break;
      case "adjustIncomingForfeitAmount":
        setAdjustIncomingForfeitAmountPresent(value);
        break;
      case "badgeToAdjust2":
        setBadgeToAdjust2Present(value);
        break;
      case "adjustEarningsSecondaryAmount":
        setAdjustEarningsSecondaryAmountPresent(value);
        break;
      default:
        break;
    }

    const newBadgeToAdjustPresent = fieldName === "badgeToAdjust" ? value : badgeToAdjustPresent;
    const newAdjustContributionAmountPresent =
      fieldName === "adjustContributionAmount" ? value : adjustContributionAmountPresent;
    const newAdjustEarningsAmountPresent = fieldName === "adjustEarningsAmount" ? value : adjustEarningsAmountPresent;
    const newAdjustIncomingForfeitAmountPresent =
      fieldName === "adjustIncomingForfeitAmount" ? value : adjustIncomingForfeitAmountPresent;

    if (newBadgeToAdjustPresent) {
      const allBadgeOneFieldsPresent =
        newAdjustContributionAmountPresent && newAdjustEarningsAmountPresent && newAdjustIncomingForfeitAmountPresent;
      if (setAdjustedBadgeOneValid) {
        setAdjustedBadgeOneValid(allBadgeOneFieldsPresent);
      }
    }
    // If badgeToAdjust2 is present, we need to check adjustEarningsSecondaryAmount
    // If those two fields are not both present, we need to set adjustedBadgeTwoValid
    const newBadgeToAdjust2Present = fieldName === "badgeToAdjust2" ? value : badgeToAdjust2Present;
    const newAdjustEarningsSecondaryAmountPresent =
      fieldName === "adjustEarningsSecondaryAmount" ? value : adjustEarningsSecondaryAmountPresent;

    if (newBadgeToAdjust2Present) {
      const allBadgeTwoFieldsPresent = newAdjustEarningsSecondaryAmountPresent;
      if (setAdjustedBadgeTwoValid) {
        setAdjustedBadgeTwoValid(allBadgeTwoFieldsPresent);
      }
    }

    const newContributionPercentPresent = fieldName === "contributionPercent" ? value : contributionPercentPresent;
    const newEarningsPercentPresent = fieldName === "earningsPercent" ? value : earningsPercentPresent;
    const newMaxAllowedContributionsPresent =
      fieldName === "maxAllowedContributions" ? value : maxAllowedContributionsPresent;
    const allFieldsPresent =
      newContributionPercentPresent && newEarningsPercentPresent && newMaxAllowedContributionsPresent;

    if (setMinimumFieldsEntered) {
      setMinimumFieldsEntered(allFieldsPresent);
      console.log("Minimum fields entered:", allFieldsPresent);
    }
  };

  const validateAndSearch = handleSubmit((data) => {
    if (isValid && !isSubmitting) {
      setIsSubmitting(true);
      setPageReset(true);
      const updateParams: ProfitShareUpdateRequest = {
        pagination: {
          sortBy: "name",
          isSortDescending: false,
          skip: 0,
          take: 25
        },
        profitYear: fiscalCloseProfitYear,
        contributionPercent: data.contributionPercent ?? 0,
        earningsPercent: data.earningsPercent ?? 0,
        incomingForfeitPercent: data.incomingForfeitPercent ?? 0,
        secondaryEarningsPercent: data.secondaryEarningsPercent ?? 0,
        maxAllowedContributions: data.maxAllowedContributions ?? maxContributionsDefault,
        badgeToAdjust: data.badgeToAdjust ?? 0,
        adjustContributionAmount: data.adjustContributionAmount ?? 0,
        adjustEarningsAmount: data.adjustEarningsAmount ?? 0,
        adjustIncomingForfeitAmount: data.adjustEarningsSecondaryAmount ?? 0,
        badgeToAdjust2: data.badgeToAdjust2 ?? 0,
        adjustEarningsSecondaryAmount: data.adjustEarningsSecondaryAmount ?? 0
      };

      // First we have to do the update calls
      triggerSearchUpdate(updateParams, false)
        .unwrap()
        .then((response: { profitShareUpdateTotals: { maxOverTotal: number } }) => {
          // We need to set the profitSharingUpdate in the store
          if (response.profitShareUpdateTotals && response.profitShareUpdateTotals.maxOverTotal > 0) {
            dispatch(setTotalForfeituresGreaterThanZero(true));
          } else {
            dispatch(setTotalForfeituresGreaterThanZero(false));
          }
        });

      dispatch(setProfitSharingUpdateQueryParams({ ...data, profitYear: fiscalCloseProfitYearAsDate }));

      dispatch(setProfitEditUpdateChangesAvailable(true));

      // Now if we have a badgeToAdjust, we want to save the
      // adjustment summary so that panel shows up
      if (data.badgeToAdjust) {
        dispatch(addBadgeNumberToUpdateAdjustmentSummary(data.badgeToAdjust));
      }

      // Now we have to do the edit calls
      triggerSearchEdit(updateParams, false).unwrap();
      dispatch(setProfitSharingEditQueryParams({ ...data, profitYear: fiscalCloseProfitYearAsDate }));
    }
  });

  /// set maxContributionsAllowed to the default value
  useEffect(() => {
    if (!profitSharingUpdate) {
      dispatch(
        updateProfitSharingEditQueryParam({
          maxAllowedContributions: maxContributionsDefault
        })
      );
    }
  }, [dispatch, profitSharingUpdate]);

  const handleReset = useCallback(() => {
    // We need to clear both grids and then both sets of query params
    if (setMinimumFieldsEntered) {
      setMinimumFieldsEntered(false);
    }
    setPageReset(true);
    dispatch(clearProfitSharingEdit());
    dispatch(clearProfitSharingUpdate());
    dispatch(clearProfitSharingEditQueryParams());
    dispatch(clearProfitSharingUpdateQueryParams());
    setInitialSearchLoaded(false);
    dispatch(setProfitEditUpdateChangesAvailable(false));
    dispatch(setResetYearEndPage(false));
    dispatch(setTotalForfeituresGreaterThanZero(false));

    reset({
      profitYear: fiscalCloseProfitYearAsDate,
      contributionPercent: 15.0,
      earningsPercent: 9.280136,
      incomingForfeitPercent: 0.876678,
      secondaryEarningsPercent: null,
      maxAllowedContributions: 57000,

      badgeToAdjust: null,
      adjustContributionAmount: null,
      adjustEarningsAmount: null,
      adjustIncomingForfeitAmount: null,

      badgeToAdjust2: null,
      adjustEarningsSecondaryAmount: null
    });
  }, [dispatch, reset, fiscalCloseProfitYearAsDate, setMinimumFieldsEntered, setPageReset, setInitialSearchLoaded]);

  // I would like handleReset() to be called whenever resetYearEndPage is true

  useEffect(() => {
    if (resetYearEndPage) {
      handleReset();
    }
  }, [resetYearEndPage, handleReset]);

  return (
    <form onSubmit={validateAndSearch}>
      <Grid
        container
        paddingX="24px">
        <Grid
          container
          spacing={3}
          width="100%">
          <Grid size={{ xs: 12, sm: 4, md: 2 }}>
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
                  onChange={(e) => {
                    field.onChange(e);
                    processFieldsEntered("contributionPercent", e.target.value !== "");
                    dispatch(updateProfitSharingEditQueryParam({ contributionPercent: Number(e.target.value) }));
                  }}
                />
              )}
            />
            {errors.contributionPercent && <FormHelperText error>{errors.contributionPercent.message}</FormHelperText>}
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
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
                  onChange={(e) => {
                    field.onChange(e);
                    processFieldsEntered("earningsPercent", e.target.value !== "");
                    dispatch(updateProfitSharingEditQueryParam({ earningsPercent: Number(e.target.value) }));
                  }}
                />
              )}
            />
            {errors.earningsPercent && <FormHelperText error>{errors.earningsPercent.message}</FormHelperText>}
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
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
                  onChange={(e) => {
                    field.onChange(e);
                    processFieldsEntered("incomingForfeitPercent", e.target.value !== "");
                    dispatch(updateProfitSharingEditQueryParam({ incomingForfeitPercent: Number(e.target.value) }));
                  }}
                />
              )}
            />
            {errors.incomingForfeitPercent && (
              <FormHelperText error>{errors.incomingForfeitPercent.message}</FormHelperText>
            )}
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Secondary Earnings %</FormLabel>
            <Tooltip
              title={
                <a
                  href="https://demoulas.atlassian.net/browse/PS-1419"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "white", textDecoration: "underline" }}>
                  PS-1419
                </a>
              }
              arrow>
              <span>
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
                      disabled={true}
                      sx={{
                        "& .MuiInputBase-root": {
                          backgroundColor: "#e0e0e0",
                          cursor: "not-allowed"
                        }
                      }}
                      onChange={(e) => {
                        field.onChange(e);
                        processFieldsEntered("secondaryEarningsPercent", e.target.value !== "");
                        dispatch(
                          updateProfitSharingEditQueryParam({ secondaryEarningsPercent: Number(e.target.value) })
                        );
                      }}
                    />
                  )}
                />
              </span>
            </Tooltip>
            {errors.secondaryEarningsPercent && (
              <FormHelperText error>{errors.secondaryEarningsPercent.message}</FormHelperText>
            )}
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
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
                  onChange={(e) => {
                    field.onChange(e);
                    processFieldsEntered("maxAllowedContributions", e.target.value !== "");
                    dispatch(updateProfitSharingEditQueryParam({ maxAllowedContributions: Number(e.target.value) }));
                  }}
                />
              )}
            />
            {errors.maxAllowedContributions && (
              <FormHelperText error>{errors.maxAllowedContributions.message}</FormHelperText>
            )}
          </Grid>

          {/* Spacer to push row 2 to a new line */}
          <Grid size={{ xs: 12, sm: 6, md: 2 }} />

          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
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
                  onChange={(e) => {
                    field.onChange(e);
                    processFieldsEntered("badgeToAdjust", e.target.value !== "");
                    dispatch(updateProfitSharingEditQueryParam({ badgeToAdjust: Number(e.target.value) }));
                  }}
                />
              )}
            />
            {errors.badgeToAdjust && <FormHelperText error>{errors.badgeToAdjust.message}</FormHelperText>}
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
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
                  onChange={(e) => {
                    field.onChange(e);
                    processFieldsEntered("adjustContributionAmount", e.target.value !== "");
                    dispatch(updateProfitSharingEditQueryParam({ adjustContributionAmount: Number(e.target.value) }));
                  }}
                />
              )}
            />
            {errors.adjustContributionAmount && (
              <FormHelperText error>{errors.adjustContributionAmount.message}</FormHelperText>
            )}
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
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
                  onChange={(e) => {
                    field.onChange(e);
                    processFieldsEntered("adjustEarningsAmount", e.target.value !== "");
                    dispatch(updateProfitSharingEditQueryParam({ adjustEarningsAmount: Number(e.target.value) }));
                  }}
                />
              )}
            />
            {errors.adjustEarningsAmount && (
              <FormHelperText error>{errors.adjustEarningsAmount.message}</FormHelperText>
            )}
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
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
                  onChange={(e) => {
                    field.onChange(e);
                    processFieldsEntered("adjustIncomingForfeitAmount", e.target.value !== "");
                    dispatch(
                      updateProfitSharingEditQueryParam({ adjustIncomingForfeitAmount: Number(e.target.value) })
                    );
                  }}
                />
              )}
            />
            {errors.adjustIncomingForfeitAmount && (
              <FormHelperText error>{errors.adjustIncomingForfeitAmount.message}</FormHelperText>
            )}
          </Grid>

          {/* Spacer fields to ensure row 3 starts on a new line */}
          <Grid size={{ xs: 12, sm: 6, md: 2 }} />
          <Grid size={{ xs: 12, sm: 6, md: 2 }} />

          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Adjust Secondary Badge</FormLabel>
            <Tooltip
              title={
                <a
                  href="https://demoulas.atlassian.net/browse/PS-1419"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "white", textDecoration: "underline" }}>
                  PS-1419
                </a>
              }
              arrow>
              <span>
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
                      disabled={true}
                      sx={{
                        "& .MuiInputBase-root": {
                          backgroundColor: "#e0e0e0",
                          cursor: "not-allowed"
                        }
                      }}
                      onChange={(e) => {
                        field.onChange(e);
                        processFieldsEntered("badgeToAdjust2", e.target.value !== "");
                        dispatch(updateProfitSharingEditQueryParam({ badgeToAdjust2: Number(e.target.value) }));
                      }}
                    />
                  )}
                />
              </span>
            </Tooltip>
            {errors.badgeToAdjust2 && <FormHelperText error>{errors.badgeToAdjust2.message}</FormHelperText>}
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Adjust Secondary Earnings Amount</FormLabel>
            <Tooltip
              title={
                <a
                  href="https://demoulas.atlassian.net/browse/PS-1419"
                  target="_blank"
                  rel="noopener noreferrer"
                  style={{ color: "white", textDecoration: "underline" }}>
                  PS-1419
                </a>
              }
              arrow>
              <span>
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
                      disabled={true}
                      sx={{
                        "& .MuiInputBase-root": {
                          backgroundColor: "#e0e0e0",
                          cursor: "not-allowed"
                        }
                      }}
                      onChange={(e) => {
                        field.onChange(e);
                        processFieldsEntered("adjustEarningsSecondaryAmount", e.target.value !== "");
                        dispatch(
                          updateProfitSharingEditQueryParam({ adjustEarningsSecondaryAmount: Number(e.target.value) })
                        );
                      }}
                    />
                  )}
                />
              </span>
            </Tooltip>
            {errors.adjustEarningsSecondaryAmount && (
              <FormHelperText error>{errors.adjustEarningsSecondaryAmount.message}</FormHelperText>
            )}
          </Grid>
        </Grid>

        <Grid
          container
          justifyContent="flex-end"
          paddingY="16px">
          <Grid size={{ xs: 12 }}>
            <SearchAndReset
              handleReset={handleReset}
              searchButtonText="Preview"
              handleSearch={validateAndSearch}
              isFetching={isFetchingUpdate || isFetchingEdit || isSubmitting}
              disabled={isFetchingUpdate || isFetchingEdit || isSubmitting}
            />
          </Grid>
        </Grid>
      </Grid>
    </form>
  );
};

export default ProfitShareEditUpdateSearchFilter;
