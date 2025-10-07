import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, Grid } from "@mui/material";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { useLazyGetAccountingRangeToCurrent } from "hooks/useFiscalCalendarYear";
import { useEffect } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetRecentlyTerminatedReportQuery } from "reduxstore/api/YearsEndApi";
import {
  clearRecentlyTerminated,
  clearRecentlyTerminatedQueryParams,
  setRecentlyTerminatedQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { SearchAndReset } from "smart-ui-library";
import { mmDDYYFormat, tryddmmyyyyToDate } from "utils/dateUtils";
import * as yup from "yup";
import { dateStringValidator, endDateStringAfterStartDateValidator, profitYearValidator } from "../../../utils/FormValidators";

interface RecentlyTerminatedSearch {
  profitYear: number;
  beginningDate: string;
  endingDate: string;
}

const schema = yup.object().shape({
  profitYear: profitYearValidator,
  beginningDate: dateStringValidator(2000, 2099, "Beginning Date").required("Begin Date is required"),
  endingDate: endDateStringAfterStartDateValidator(
    "beginningDate",
    tryddmmyyyyToDate,
    "Ending date must be the same or after the beginning date"
  ).required("End Date is required")
});

interface RecentlyTerminatedSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const RecentlyTerminatedSearchFilter: React.FC<RecentlyTerminatedSearchFilterProps> = ({ setInitialSearchLoaded }) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerSearch, { isFetching }] = useLazyGetRecentlyTerminatedReportQuery();
  const [fetchAccountingRange, { data: fiscalData }] = useLazyGetAccountingRangeToCurrent(6);
  const dispatch = useDispatch();
  const { recentlyTerminatedQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const profitYear = useDecemberFlowProfitYear();
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<RecentlyTerminatedSearch>({
    resolver: yupResolver(schema) as Resolver<RecentlyTerminatedSearch>,
    defaultValues: {
      profitYear: profitYear || recentlyTerminatedQueryParams?.profitYear || undefined,
      beginningDate:
        recentlyTerminatedQueryParams?.beginningDate || (fiscalData ? fiscalData.fiscalBeginDate : "") || "",
      endingDate: recentlyTerminatedQueryParams?.endingDate || (fiscalData ? fiscalData.fiscalEndDate : "") || ""
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid && hasToken) {
      dispatch(
        setRecentlyTerminatedQueryParams({
          profitYear: data.profitYear,
          beginningDate: data.beginningDate,
          endingDate: data.endingDate,
          pagination: { skip: 0, take: 25, sortBy: "fullName, terminationDate", isSortDescending: false }
        })
      );
      setInitialSearchLoaded(true);
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);

    // Clear the form fields
    reset({
      profitYear: profitYear || undefined,
      beginningDate: fiscalData ? fiscalData.fiscalBeginDate : "",
      endingDate: fiscalData ? fiscalData.fiscalEndDate : ""
    });

    // Clear the data in Redux store
    dispatch(clearRecentlyTerminated());
    dispatch(clearRecentlyTerminatedQueryParams());
    trigger();
  };

  useEffect(() => {
    fetchAccountingRange();
  }, [fetchAccountingRange]);

  useEffect(() => {
    if (fiscalData && fiscalData.fiscalBeginDate && fiscalData.fiscalEndDate) {
      reset({
        profitYear: profitYear || recentlyTerminatedQueryParams?.profitYear || undefined,
        beginningDate: recentlyTerminatedQueryParams?.beginningDate || fiscalData.fiscalBeginDate,
        endingDate: recentlyTerminatedQueryParams?.endingDate || fiscalData.fiscalEndDate
      });
    }
  }, [fiscalData, profitYear, recentlyTerminatedQueryParams, reset]);

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
              <DsmDatePicker
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
              <DsmDatePicker
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
              <DsmDatePicker
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
          isFetching={isFetching}
          disabled={!isValid}
        />
      </Grid>
    </form>
  );
};

export default RecentlyTerminatedSearchFilter;
