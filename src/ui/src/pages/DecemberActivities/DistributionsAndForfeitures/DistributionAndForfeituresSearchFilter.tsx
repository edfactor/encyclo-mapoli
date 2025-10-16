import { yupResolver } from "@hookform/resolvers/yup";
import { CircularProgress, FormControl, FormHelperText, FormLabel, Grid, MenuItem, TextField } from "@mui/material";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { format } from "date-fns";
import { useEffect } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useGetStatesQuery } from "reduxstore/api/LookupsApi";
import { useLazyGetDistributionsAndForfeituresQuery } from "reduxstore/api/YearsEndApi";
import {
  clearDistributionsAndForfeitures,
  clearDistributionsAndForfeituresQueryParams,
  setDistributionsAndForfeituresQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear.ts";
import { tryddmmyyyyToDate } from "../../../utils/dateUtils";
import { getMonthEndDate, getMonthStartDate } from "../../../utils/dateRangeUtils";
import { endDateAfterStartDateValidator } from "../../../utils/FormValidators";

const formatDateOnly = (date: Date | null): string | undefined => {
  if (!date) return undefined;
  return format(date, "yyyy-MM-dd");
};

interface DistributionsAndForfeituresSearch {
  startDate: Date | null;
  endDate: Date | null;
  state: string;
  taxCode: string;
}

const schema = yup.object().shape({
  startDate: yup.date().nullable(),
  endDate: endDateAfterStartDateValidator("startDate"),
  state: yup.string(),
  taxCode: yup.string()
});

interface DistributionsAndForfeituresSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const DistributionsAndForfeituresSearchFilter: React.FC<DistributionsAndForfeituresSearchFilterProps> = ({
  setInitialSearchLoaded
}) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerSearch, { isFetching }] = useLazyGetDistributionsAndForfeituresQuery();
  const { data: statesData, isLoading: isLoadingStates } = useGetStatesQuery();
  const dispatch = useDispatch();
  //  const fiscalData = useFiscalCalendarYear();
  const profitYear = useDecemberFlowProfitYear();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<DistributionsAndForfeituresSearch>({
    resolver: yupResolver(schema) as Resolver<DistributionsAndForfeituresSearch>,
    defaultValues: {
      startDate: null,
      endDate: null,
      state: "",
      taxCode: ""
    }
  });

  // Set form defaults when fiscal data becomes available
  useEffect(() => {
    if (profitYear) {
      reset({
        // PROFIT_DETAIL transactions historically have month/year - so this date range is the profit_year
        startDate: tryddmmyyyyToDate(new Date(profitYear, 0, 1)),
        endDate: tryddmmyyyyToDate(new Date(profitYear, 11, 31)),
        state: "",
        taxCode: ""
      });
    }
  }, [reset]);

  const validateAndSearch = handleSubmit((data) => {
    if (isValid && hasToken) {
      triggerSearch(
        {
          ...(data.startDate && { startDate: formatDateOnly(data.startDate) }),
          ...(data.endDate && { endDate: formatDateOnly(data.endDate) }),
          ...(data.state && data.state !== "" && { state: data.state }),
          ...(data.taxCode && data.taxCode !== "" && { taxCode: data.taxCode }),
          pagination: { skip: 0, take: 25, sortBy: "employeeName, date", isSortDescending: false }
        },
        false
      ).unwrap();
      dispatch(
        setDistributionsAndForfeituresQueryParams({
          startDate: formatDateOnly(data.startDate),
          endDate: formatDateOnly(data.endDate),
          state: data.state || undefined,
          taxCode: data.taxCode || undefined
        })
      );
      setInitialSearchLoaded(true);
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);

    // Clear the form fields
    reset({
      // PROFIT_DETAIL transactions historically have month/year - so this date range is the profit_year
      startDate: tryddmmyyyyToDate(new Date(profitYear, 0, 1)),
      endDate: tryddmmyyyyToDate(new Date(profitYear, 11, 31)),
      state: "",
      taxCode: ""
    });

    // Clear the data in Redux store
    dispatch(clearDistributionsAndForfeitures());
    dispatch(clearDistributionsAndForfeituresQueryParams());
  };

  if (!profitYear) {
    return (
      <Grid
        container
        justifyContent="center"
        padding="24px">
        <CircularProgress />
      </Grid>
    );
  }

  return (
    <form onSubmit={validateAndSearch}>
      <Grid
        container
        paddingX="24px"
        gap="24px">
        <Grid size={{ xs: 12, sm: 4, md: 2 }}>
          <Controller
            name="startDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="startDate"
                onChange={(value: Date | null) => {
                  // Expand month selection to first day of month
                  const expandedDate = getMonthStartDate(value);
                  field.onChange(expandedDate);
                  trigger("endDate");
                }}
                value={field.value}
                required={false}
                label="Start Date"
                disableFuture
                error={errors.startDate?.message}
                views={["year", "month"]}
              />
            )}
          />
          {errors.startDate && <FormHelperText error>{errors.startDate.message}</FormHelperText>}
        </Grid>
        <Grid size={{ xs: 12, sm: 4, md: 2 }}>
          <Controller
            name="endDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="endDate"
                onChange={(value: Date | null) => {
                  // Expand month selection to last day of month
                  const expandedDate = getMonthEndDate(value);
                  field.onChange(expandedDate);
                  trigger("endDate");
                }}
                value={field.value}
                required={false}
                label="End Date"
                disableFuture
                error={errors.endDate?.message}
                views={["year", "month"]}
              />
            )}
          />
          {errors.endDate && <FormHelperText error>{errors.endDate.message}</FormHelperText>}
        </Grid>
        <Grid size={{ xs: 12, sm: 4, md: 2 }}>
          <FormLabel>State</FormLabel>
          <FormControl fullWidth>
            <Controller
              name="state"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value ?? ""}
                  select
                  fullWidth
                  size="small"
                  variant="outlined"
                  disabled={isLoadingStates}
                  error={!!errors.state}
                  helperText={errors.state?.message}>
                  <MenuItem value="">All</MenuItem>
                  {statesData?.map((state: { abbreviation: string; name: string }) => (
                    <MenuItem
                      key={state.abbreviation}
                      value={state.abbreviation}>
                      {state.abbreviation} - {state.name}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />
          </FormControl>
        </Grid>
        <Grid size={{ xs: 12, sm: 4, md: 2 }}>
          <FormLabel>Tax Code</FormLabel>
          <FormControl fullWidth>
            <Controller
              name="taxCode"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value ?? ""}
                  select
                  fullWidth
                  size="small"
                  variant="outlined"
                  error={!!errors.taxCode}
                  helperText={errors.taxCode?.message}>
                  <MenuItem value="">All</MenuItem>
                  <MenuItem value="1">1</MenuItem>
                  <MenuItem value="3">3</MenuItem>
                  <MenuItem value="7">7</MenuItem>
                </TextField>
              )}
            />
          </FormControl>
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

export default DistributionsAndForfeituresSearchFilter;
