import { yupResolver } from "@hookform/resolvers/yup";
import { CircularProgress, FormControl, FormHelperText, FormLabel, Grid, MenuItem, TextField } from "@mui/material";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { format } from "date-fns";
import { useEffect } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useGetStatesQuery, useGetTaxCodesQuery } from "reduxstore/api/LookupsApi";
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
import { getMonthEndDate, getMonthStartDate } from "../../../utils/dateRangeUtils";
import { tryddmmyyyyToDate } from "../../../utils/dateUtils";
import { endDateAfterStartDateValidator } from "../../../utils/FormValidators";

const formatDateOnly = (date: Date | null): string | undefined => {
  if (!date) return undefined;
  return format(date, "yyyy-MM-dd");
};

interface DistributionsAndForfeituresSearch {
  startDate: Date | null;
  endDate: Date | null;
  states: string[];
  taxCodes: string[];
}

const schema = yup.object().shape({
  startDate: yup.date().nullable(),
  endDate: endDateAfterStartDateValidator("startDate"),
  states: yup.array().of(yup.string().required()).required(),
  taxCodes: yup.array().of(yup.string().required()).required()
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
  const { data: taxCodesData, isLoading: isLoadingTaxCodes } = useGetTaxCodesQuery();
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
      states: [],
      taxCodes: []
    }
  });

  // Set form defaults when fiscal data becomes available
  useEffect(() => {
    if (profitYear) {
      reset({
        // PROFIT_DETAIL transactions historically have month/year - so this date range is the profit_year
        startDate: tryddmmyyyyToDate(new Date(profitYear, 0, 1)),
        endDate: tryddmmyyyyToDate(new Date(profitYear, 11, 31)),
        states: [],
        taxCodes: []
      });
    }
  }, [reset, profitYear]);

  const validateAndSearch = handleSubmit(async (data) => {
    if (isValid && hasToken) {
      const queryParams = {
        startDate: formatDateOnly(data.startDate),
        endDate: formatDateOnly(data.endDate),
        states: data.states || [],
        taxCodes: data.taxCodes || []
      };

      // Store query params in Redux
      dispatch(setDistributionsAndForfeituresQueryParams(queryParams));

      // Perform the search
      await triggerSearch(queryParams);

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
      states: [],
      taxCodes: []
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
              name="states"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value.length === 0 ? ["ALL"] : field.value}
                  onChange={(e) => {
                    const value = e.target.value as unknown as string[];
                    // If "ALL" is clicked when items are selected, clear to empty (All)
                    if (value.includes("ALL") && value.length > 1) {
                      const lastSelected = value[value.length - 1];
                      if (lastSelected === "ALL") {
                        // User clicked "All" - clear everything
                        field.onChange([]);
                      } else {
                        // User clicked a specific item - remove "ALL"
                        field.onChange(value.filter((v) => v !== "ALL"));
                      }
                    } else {
                      field.onChange(value);
                    }
                  }}
                  select
                  SelectProps={{
                    multiple: true,
                    renderValue: (selected) => {
                      const vals = selected as string[];
                      if (vals.length === 0 || vals.includes("ALL")) {
                        return "All";
                      }
                      return vals.join(", ");
                    }
                  }}
                  fullWidth
                  size="small"
                  variant="outlined"
                  disabled={isLoadingStates}
                  error={!!errors.states}
                  helperText={errors.states?.message}>
                  <MenuItem value="ALL">All</MenuItem>
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
              name="taxCodes"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value.length === 0 ? ["ALL"] : field.value}
                  onChange={(e) => {
                    const value = e.target.value as unknown as string[];
                    // If "ALL" is clicked when items are selected, clear to empty (All)
                    if (value.includes("ALL") && value.length > 1) {
                      const lastSelected = value[value.length - 1];
                      if (lastSelected === "ALL") {
                        // User clicked "All" - clear everything
                        field.onChange([]);
                      } else {
                        // User clicked a specific item - remove "ALL"
                        field.onChange(value.filter((v) => v !== "ALL"));
                      }
                    } else {
                      field.onChange(value);
                    }
                  }}
                  select
                  SelectProps={{
                    multiple: true,
                    renderValue: (selected) => {
                      const vals = selected as string[];
                      if (vals.length === 0 || vals.includes("ALL")) {
                        return "All";
                      }
                      return vals.join(", ");
                    }
                  }}
                  fullWidth
                  size="small"
                  variant="outlined"
                  disabled={isLoadingTaxCodes}
                  error={!!errors.taxCodes}
                  helperText={errors.taxCodes?.message}>
                  <MenuItem value="ALL">All</MenuItem>
                  {taxCodesData?.map((taxCode: { id: string; name: string }) => (
                    <MenuItem
                      key={taxCode.id}
                      value={taxCode.id}>
                      {taxCode.id} - {taxCode.name}
                    </MenuItem>
                  ))}
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
