import { yupResolver } from "@hookform/resolvers/yup";
import { CircularProgress, FormHelperText } from "@mui/material";
import { Grid } from "@mui/material";
import { Controller, Resolver, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useEffect } from "react";
import { useLazyGetDistributionsAndForfeituresQuery } from "reduxstore/api/YearsEndApi";
import {
  clearDistributionsAndForfeitures,
  clearDistributionsAndForfeituresQueryParams,
  setDistributionsAndForfeituresQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import useFiscalCalendarYear from "hooks/useFiscalCalendarYear";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { format } from "date-fns";
import { tryddmmyyyyToDate } from "../../../utils/dateUtils";

const formatDateOnly = (date: Date | null): string | undefined => {
  if (!date) return undefined;
  return format(date, "yyyy-MM-dd");
};

interface DistributionsAndForfeituresSearch {
  startDate: Date | null;
  endDate: Date | null;
}

const schema = yup.object().shape({
  startDate: yup.date().nullable(),
  endDate: yup
    .date()
    .nullable()
    .test("is-after-start", "End Date must be after Start Date", function (value) {
      const { startDate } = this.parent;
      if (!startDate || !value) return true;
      return value > startDate;
    })
});

interface DistributionsAndForfeituresSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const DistributionsAndForfeituresSearchFilter: React.FC<DistributionsAndForfeituresSearchFilterProps> = ({
  setInitialSearchLoaded
}) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerSearch, { isFetching }] = useLazyGetDistributionsAndForfeituresQuery();
  const dispatch = useDispatch();
  const { distributionsAndForfeituresQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const fiscalData = useFiscalCalendarYear();
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
      endDate: null
    }
  });

  // Set form defaults when fiscal data becomes available
  useEffect(() => {
    if (fiscalData) {
      reset({
        startDate: tryddmmyyyyToDate(fiscalData.fiscalBeginDate),
        endDate: tryddmmyyyyToDate(fiscalData.fiscalEndDate)
      });
    }
  }, [fiscalData, reset]);

  const validateAndSearch = handleSubmit((data) => {
    if (isValid && hasToken) {
      triggerSearch(
        {
          ...(data.startDate && { startDate: formatDateOnly(data.startDate) }),
          ...(data.endDate && { endDate: formatDateOnly(data.endDate) }),
          pagination: { skip: 0, take: 25, sortBy: "employeeName, date", isSortDescending: false }
        },
        false
      ).unwrap();
      dispatch(
        setDistributionsAndForfeituresQueryParams({
          startDate: formatDateOnly(data.startDate),
          endDate: formatDateOnly(data.endDate)
        })
      );
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);

    // Clear the form fields
    reset({
      startDate: fiscalData ? tryddmmyyyyToDate(fiscalData.fiscalBeginDate) : null,
      endDate: fiscalData ? tryddmmyyyyToDate(fiscalData.fiscalEndDate) : null
    });

    // Clear the data in Redux store
    dispatch(clearDistributionsAndForfeitures());
    dispatch(clearDistributionsAndForfeituresQueryParams());
  };

  if (!fiscalData) {
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
                label="Start Date (Day is not used)"
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
                label="End Date (Day is not used)"
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

export default DistributionsAndForfeituresSearchFilter;
