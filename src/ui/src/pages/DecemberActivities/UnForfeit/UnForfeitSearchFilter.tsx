import { yupResolver } from "@hookform/resolvers/yup";
import { Checkbox, FormControlLabel, FormHelperText, Grid } from "@mui/material";
import { useEffect, useState } from "react";
import { Controller, Resolver, useForm, useWatch } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { DSMDatePicker, SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { ConfirmationDialog } from "../../../components/ConfirmationDialog";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useLazyGetUnForfeitsQuery } from "../../../reduxstore/api/YearsEndApi";
import {
  clearUnForfeitsDetails,
  clearUnForfeitsQueryParams,
  setUnForfeitsQueryParams
} from "../../../reduxstore/slices/yearsEndSlice";
import { RootState } from "../../../reduxstore/store";
import { CalendarResponseDto, StartAndEndDateRequest } from "../../../reduxstore/types";
import { mmDDYYFormat, tryddmmyyyyToDate } from "../../../utils/dateUtils";
import {
  dateStringValidator,
  endDateStringAfterStartDateValidator,
  profitYearValidator
} from "../../../utils/FormValidators";

const schema = yup.object().shape({
  beginningDate: dateStringValidator(2000, 2099, "Beginning Date").required("Beginning Date is required"),
  endingDate: endDateStringAfterStartDateValidator(
    "beginningDate",
    tryddmmyyyyToDate,
    "Ending date must be the same or after the beginning date"
  ).required("Ending Date is required"),
  // Pagination is handled separately, not part of form validation
  pagination: yup
    .object({
      skip: yup.number(),
      take: yup.number(),
      sortBy: yup.string(),
      isSortDescending: yup.boolean()
    })
    .nullable(),
  // Hidden field: not shown in search filter, but required in data
  profitYear: profitYearValidator()
});

interface UnForfeitSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
  fiscalData: CalendarResponseDto;
  onSearch?: () => void;
  hasUnsavedChanges?: boolean;
  setHasUnsavedChanges: (hasChanges: boolean) => void;
}

const UnForfeitSearchFilter: React.FC<UnForfeitSearchFilterProps> = ({
  setInitialSearchLoaded,
  fiscalData,
  onSearch,
  hasUnsavedChanges,
  setHasUnsavedChanges
}) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerSearch, { isFetching }] = useLazyGetUnForfeitsQuery();
  const { unForfeitsQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const dispatch = useDispatch();
  const [showUnsavedChangesDialog, setShowUnsavedChangesDialog] = useState(false);

  const selectedProfitYear = useDecemberFlowProfitYear();

  const validateAndSubmit = (data: StartAndEndDateRequest) => {
    if (hasUnsavedChanges) {
      setShowUnsavedChangesDialog(true);
      return;
    }

    if (isValid && hasToken) {
      const beginDate = data.beginningDate || fiscalData.fiscalBeginDate || "";
      const endDate = data.endingDate || fiscalData.fiscalEndDate || "";

      const updatedData = {
        ...data,
        beginningDate: mmDDYYFormat(beginDate),
        endingDate: mmDDYYFormat(endDate),
        profitYear: selectedProfitYear
      };

      dispatch(setUnForfeitsQueryParams(updatedData));
      triggerSearch(updatedData);
      if (onSearch) onSearch(); // Only call if onSearch is provided
    }
  };

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger,
    clearErrors
  } = useForm<StartAndEndDateRequest>({
    resolver: yupResolver(schema) as Resolver<StartAndEndDateRequest>,
    defaultValues: {
      beginningDate:
        unForfeitsQueryParams?.beginningDate ||
        (fiscalData.fiscalBeginDate ? mmDDYYFormat(fiscalData.fiscalBeginDate) : undefined),
      endingDate:
        unForfeitsQueryParams?.endingDate ||
        (fiscalData.fiscalEndDate ? mmDDYYFormat(fiscalData.fiscalEndDate) : undefined),
      excludeZeroBalance: unForfeitsQueryParams?.excludeZeroBalance || false,
      pagination: { skip: 0, take: 25, sortBy: "fullName", isSortDescending: false },
      profitYear: selectedProfitYear
    }
  });

  const beginningDateValue = useWatch({ control, name: "beginningDate" });

  // Trigger validation when fiscal data becomes available
  useEffect(() => {
    if (fiscalData.fiscalBeginDate && fiscalData.fiscalEndDate) {
      trigger();
    }
  }, [fiscalData.fiscalBeginDate, fiscalData.fiscalEndDate, trigger]);

  // Effect to fetch fiscal data when profit year changes
  const validateAndSearch = handleSubmit(validateAndSubmit as (data: StartAndEndDateRequest) => void);

  const handleReset = () => {
    setHasUnsavedChanges(false);
    setInitialSearchLoaded(false);
    dispatch(clearUnForfeitsQueryParams());
    dispatch(clearUnForfeitsDetails());

    reset({
      beginningDate: fiscalData.fiscalBeginDate ? mmDDYYFormat(fiscalData.fiscalBeginDate) : undefined,
      endingDate: fiscalData.fiscalEndDate ? mmDDYYFormat(fiscalData.fiscalEndDate) : undefined,
      excludeZeroBalance: false,
      profitYear: selectedProfitYear,
      pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
    });

    clearErrors();
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Grid
        container
        paddingX="24px"
        alignItems={"flex-end"}
        gap="24px">
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="beginningDate"
            control={control}
            render={({ field }) => (
              <>
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
                  required={true}
                  label="Rehire Begin Date"
                  disableFuture
                  error={errors.beginningDate?.message}
                  minDate={tryddmmyyyyToDate(fiscalData.fiscalBeginDate) ?? undefined}
                  maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate) ?? undefined}
                />
                <FormHelperText error>{errors.beginningDate?.message || " "}</FormHelperText>
              </>
            )}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="endingDate"
            control={control}
            render={({ field }) => {
              const minDateFromBeginning = beginningDateValue ? tryddmmyyyyToDate(beginningDateValue) : null;
              const fiscalMinDate = tryddmmyyyyToDate(fiscalData.fiscalBeginDate);
              const effectiveMinDate =
                minDateFromBeginning && fiscalMinDate
                  ? minDateFromBeginning > fiscalMinDate
                    ? minDateFromBeginning
                    : fiscalMinDate
                  : (minDateFromBeginning ?? fiscalMinDate ?? undefined);

              return (
                <>
                  <DSMDatePicker
                    id="endingDate"
                    onChange={(value: Date | null) => {
                      field.onChange(value ? mmDDYYFormat(value) : undefined);
                      trigger("endingDate");
                    }}
                    value={field.value ? tryddmmyyyyToDate(field.value) : null}
                    required={true}
                    label="Rehire Ending Date"
                    disableFuture
                    error={errors.endingDate?.message}
                    minDate={effectiveMinDate}
                    maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate) ?? undefined}
                  />
                  <FormHelperText error>{errors.endingDate?.message || " "}</FormHelperText>
                </>
              );
            }}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="excludeZeroBalance"
            control={control}
            render={({ field }) => (
              <>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={field.value || false}
                      onChange={(e) => field.onChange(e.target.checked)}
                    />
                  }
                  label="Exclude employees with no current or vested balance"
                />
                <FormHelperText> </FormHelperText>
              </>
            )}
          />
        </Grid>
      </Grid>
      <Grid
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isFetching}
          disabled={!isValid || isFetching}
        />
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

export default UnForfeitSearchFilter;
