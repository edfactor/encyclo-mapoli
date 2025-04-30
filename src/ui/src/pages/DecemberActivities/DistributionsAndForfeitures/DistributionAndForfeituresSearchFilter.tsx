import { yupResolver } from "@hookform/resolvers/yup";
import { Checkbox, FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetDistributionsAndForfeituresQuery } from "reduxstore/api/YearsEndApi";
import {
  clearDistributionsAndForfeitures,
  clearDistributionsAndForfeituresQueryParams,
  setDistributionsAndForfeituresQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";

interface DistributionsAndForfeituresSearch {
  profitYear: number;
  startMonth?: number | null | undefined;
  endMonth?: number | null | undefined;
  includeOutgoingForfeitures: boolean;
}

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .required("Year is required"),
  startMonth: yup
    .number()
    .typeError("Start Month must be a number")
    .integer("Start Month must be an integer")
    .min(1, "Start Month must be 1 or higher")
    .max(12, "Start Month must be 12 or lower")
    .transform((value) => (isNaN(value) ? undefined : value)),
  endMonth: yup
    .number()
    .typeError("End Month must be a number")
    .integer("End Month must be an integer")
    .min(1, "End Month must be 1 or higher")
    .max(12, "End Month must be 12 or lower")
    .nullable(),
  includeOutgoingForfeitures: yup.boolean().default(false).required()
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
  const { distributionsAndForfeituresQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const profitYear = useDecemberFlowProfitYear();
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<DistributionsAndForfeituresSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: profitYear || distributionsAndForfeituresQueryParams?.profitYear || undefined,
      startMonth: distributionsAndForfeituresQueryParams?.startMonth || 1,
      endMonth: distributionsAndForfeituresQueryParams?.endMonth || 12,
      includeOutgoingForfeitures: distributionsAndForfeituresQueryParams?.includeOutgoingForfeitures || false
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid && hasToken) {
      triggerSearch(
        {
          profitYear: data.profitYear,
          ...(data.startMonth && { startMonth: data.startMonth }),
          ...(data.endMonth && { endMonth: data.endMonth }),
          includeOutgoingForfeitures: data.includeOutgoingForfeitures ?? false,
          pagination: { skip: 0, take: 25, sortBy: "employeeName, date", isSortDescending: false }
        },
        false
      ).unwrap();
      dispatch(
        setDistributionsAndForfeituresQueryParams({
          profitYear: data.profitYear,
          startMonth: data.startMonth,
          endMonth: data.endMonth || undefined,
          includeOutgoingForfeitures: data.includeOutgoingForfeitures
        })
      );
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);

    // Clear the form fields
    reset({
      profitYear: profitYear || undefined,
      startMonth: 1,  
      endMonth: 12,
      includeOutgoingForfeitures: false
    });

    // Clear the data in Redux store
    dispatch(clearDistributionsAndForfeitures());
    dispatch(clearDistributionsAndForfeituresQueryParams());
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Grid2
        container
        paddingX="24px"
        gap="24px">
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>Profit Year</FormLabel>
          <Controller
            name="profitYear"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                variant="outlined"
                error={!!errors.profitYear}
                onChange={(e) => {
                  field.onChange(e);
                }}
                type="number"
                disabled={true}
              />
            )}
          />
          {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
        </Grid2>
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>Start Month</FormLabel>
          <Controller
            name="startMonth"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                variant="outlined"
                error={!!errors.startMonth}
                // Convert undefined to empty string for the TextField
                value={field.value === undefined ? '' : field.value}
                // Handle changes to ensure undefined is properly set instead of empty string
                onChange={(e) => {
                  const value = e.target.value;
                  field.onChange(value === '' ? undefined : Number(value));
                }}

                type="number"
              />
            )}
          />
          {errors.startMonth && <FormHelperText error>{errors.startMonth.message}</FormHelperText>}
        </Grid2>
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>End Month</FormLabel>
          <Controller
            name="endMonth"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                variant="outlined"
                error={!!errors.endMonth}
                // Convert undefined to empty string for the TextField
                value={field.value === undefined ? '' : field.value}
                // Handle changes to ensure undefined is properly set instead of empty string
                onChange={(e) => {
                  const value = e.target.value;
                  field.onChange(value === '' ? undefined : Number(value));
                }}

                type="number"
              />
            )}
          />
          {errors.endMonth && <FormHelperText error>{errors.endMonth.message}</FormHelperText>}
        </Grid2>
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>Include Outgoing Forfeitures</FormLabel>
          <Controller
            name="includeOutgoingForfeitures"
            control={control}
            render={({ field }) => (
              <Checkbox
                checked={field.value}
                onChange={(e) => {
                  field.onChange(e.target.checked);
                }}
              />
            )}
          />
          {errors.includeOutgoingForfeitures && (
            <FormHelperText error>{errors.includeOutgoingForfeitures.message}</FormHelperText>
          )}
        </Grid2>
      </Grid2>
      <Grid2
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isFetching}
          disabled={!isValid}
        />
      </Grid2>
    </form>
  );
};

export default DistributionsAndForfeituresSearchFilter;
