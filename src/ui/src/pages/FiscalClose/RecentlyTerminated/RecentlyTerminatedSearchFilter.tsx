import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, Grid } from "@mui/material";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { format } from "date-fns";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
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
import * as yup from "yup";

const formatDateOnly = (date: Date | null): string | undefined => {
  if (!date) return undefined;
  return format(date, "yyyy-MM-dd");
};

interface RecentlyTerminatedSearch {
  profitYear: number;
  startDate: Date | null;
  endDate: Date | null;
}

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .required("Year is required"),
  startDate: yup.date().nullable(),
  endDate: yup
    .date()
    .nullable()
    .test("is-after-start", "End Date must be after Start Date", function (value) {
      const { startDate } = this.parent;
      if (!startDate || !value) return true;
      return value > startDate;
    })
    .test("is-too-early", "Insuffient data for dates before 2024", function (value) {
      if (!value) return true;
      return value > new Date(2024, 1, 1);
    })
});

interface RecentlyTerminatedSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const RecentlyTerminatedSearchFilter: React.FC<RecentlyTerminatedSearchFilterProps> = ({ setInitialSearchLoaded }) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerSearch, { isFetching }] = useLazyGetRecentlyTerminatedReportQuery();
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
      startDate: null,
      endDate: null
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid && hasToken) {
      triggerSearch(
        {
          profitYear: data.profitYear,
          beginningDate: formatDateOnly(data.startDate) ?? "",
          endingDate: formatDateOnly(data.endDate) ?? "",
          pagination: { skip: 0, take: 25, sortBy: "fullName, terminationDate", isSortDescending: false }
        },
        false
      ).unwrap();
      dispatch(
        setRecentlyTerminatedQueryParams({
          profitYear: data.profitYear,
          beginningDate: formatDateOnly(data.startDate) ?? "",
          endingDate: formatDateOnly(data.endDate) ?? "",
          pagination: { skip: 0, take: 25, sortBy: "fullName, terminationDate", isSortDescending: false }
        })
      );
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);

    // Clear the form fields
    reset({
      profitYear: profitYear || undefined,
      startDate: null,
      endDate: null
    });

    // Clear the data in Redux store
    dispatch(clearRecentlyTerminated());
    dispatch(clearRecentlyTerminatedQueryParams());
  };

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
            name="startDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="startDate"
                onChange={(value: Date | null) => {
                  field.onChange(value);
                  trigger("endDate");
                }}
                value={field.value}
                required={false}
                label="Start Date"
                disableFuture
                error={errors.startDate?.message}
              />
            )}
          />
          {errors.startDate && <FormHelperText error>{errors.startDate.message}</FormHelperText>}
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="endDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="endDate"
                onChange={(value: Date | null) => {
                  field.onChange(value);
                  trigger("endDate");
                }}
                value={field.value}
                required={false}
                label="End Date"
                disableFuture
                error={errors.endDate?.message}
              />
            )}
          />
          {errors.endDate && <FormHelperText error>{errors.endDate.message}</FormHelperText>}
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
