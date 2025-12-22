import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, Grid } from "@mui/material";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { useLazyGetAccountingRangeToCurrent } from "hooks/useFiscalCalendarYear";
import { useEffect, useState } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import { DSMDatePicker, SearchAndReset } from "smart-ui-library";
import { mmDDYYFormat, tryddmmyyyyToDate } from "utils/dateUtils";
import { getLastYearDateRange } from "utils/dateRangeUtils";
import * as yup from "yup";
import {
  dateStringValidator,
  endDateStringAfterStartDateValidator,
  profitYearValidator
} from "../../../utils/FormValidators";

interface TerminatedLettersSearch {
  profitYear: number;
  beginningDate: string;
  endingDate: string;
}

const schema = yup.object().shape({
  profitYear: profitYearValidator(),
  beginningDate: dateStringValidator(2000, 2099, "Beginning Date").required("Begin Date is required"),
  endingDate: endDateStringAfterStartDateValidator(
    "beginningDate",
    tryddmmyyyyToDate,
    "Date must be equal to or greater than begin date"
  ).required("End Date is required")
});

interface TerminatedLettersSearchFilterProps {
  onSearch: (beginningDate: string, endingDate: string) => Promise<boolean>;
  onReset: () => void;
}

const TerminatedLettersSearchFilter: React.FC<TerminatedLettersSearchFilterProps> = ({ onSearch, onReset }) => {
  const [fetchAccountingRange, { data: fiscalData }] = useLazyGetAccountingRangeToCurrent(6);
  const profitYear = useDecemberFlowProfitYear();
  const [isSearching, setIsSearching] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Get last year date range for default values
  const { beginDate, endDate } = getLastYearDateRange();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<TerminatedLettersSearch>({
    resolver: yupResolver(schema) as Resolver<TerminatedLettersSearch>,
    defaultValues: {
      profitYear: profitYear || undefined,
      beginningDate: mmDDYYFormat(beginDate),
      endingDate: mmDDYYFormat(endDate)
    }
  });

  const validateAndSearch = handleSubmit(async (data) => {
    if (isValid && !isSubmitting) {
      setIsSubmitting(true);
      setIsSearching(true);
      await onSearch(data.beginningDate, data.endingDate);
      setIsSearching(false);
      setIsSubmitting(false);
    }
  });

  const handleReset = () => {
    // Clear the form fields
    reset({
      profitYear: profitYear || undefined,
      beginningDate: mmDDYYFormat(beginDate),
      endingDate: mmDDYYFormat(endDate)
    });

    // Call parent reset
    onReset();
    trigger();
  };

  useEffect(() => {
    fetchAccountingRange();
  }, [fetchAccountingRange]);

  return (
    <form onSubmit={validateAndSearch}>
      <Grid
        container
        paddingX="24px"
        gap="24px">
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="profitYear"
            control={control}
            render={({ field }) => (
              <DSMDatePicker
                id="profitYear"
                onChange={(value: Date | null) => field.onChange(value?.getFullYear() || undefined)}
                value={field.value ? new Date(field.value, 0) : null}
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
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="beginningDate"
            control={control}
            render={({ field }) => (
              <DSMDatePicker
                id="beginningDate"
                onChange={(value: Date | null) => {
                  field.onChange(value ? mmDDYYFormat(value) : undefined);
                  trigger("beginningDate");
                }}
                value={field.value ? tryddmmyyyyToDate(field.value) : null}
                required={false}
                label="Begin Date"
                disableFuture
                error={errors.beginningDate?.message}
                minDate={tryddmmyyyyToDate(fiscalData?.fiscalBeginDate) || undefined}
                maxDate={tryddmmyyyyToDate(fiscalData?.fiscalEndDate) || undefined}
              />
            )}
          />
          {errors.beginningDate && <FormHelperText error>{errors.beginningDate.message}</FormHelperText>}
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="endingDate"
            control={control}
            render={({ field }) => (
              <DSMDatePicker
                id="endingDate"
                onChange={(value: Date | null) => {
                  field.onChange(value ? mmDDYYFormat(value) : undefined);
                  trigger("endingDate");
                }}
                value={field.value ? tryddmmyyyyToDate(field.value) : null}
                required={false}
                label="End Date"
                disableFuture
                error={errors.endingDate?.message}
                minDate={tryddmmyyyyToDate(fiscalData?.fiscalBeginDate) || undefined}
                maxDate={tryddmmyyyyToDate(fiscalData?.fiscalEndDate) || undefined}
              />
            )}
          />
          {errors.endingDate && <FormHelperText error>{errors.endingDate.message}</FormHelperText>}
        </Grid>
      </Grid>
      <Grid
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isSearching || isSubmitting}
          disabled={!isValid || isSearching || isSubmitting}
        />
      </Grid>
    </form>
  );
};

export default TerminatedLettersSearchFilter;
