import { Checkbox, FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useForm, Controller } from "react-hook-form";
import { useLazyGetExecutiveHoursAndDollarsQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import { useDispatch } from "react-redux";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { setExecutiveHoursAndDollarsGridYear } from "reduxstore/slices/yearsEndSlice";

interface ExecutiveHoursAndDollarsSearch {
  profitYear: number;
  badgeNumber?: number | null;
  socialSecurity?: number | null;
  fullNameContains?: string | null;
  hasExecutiveHoursAndDollars: boolean;
}

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .required("Year is required"),
  badgeNumber: yup
    .number()
    .typeError("Badge Number must be a number")
    .integer("Badge Number must be an integer")
    .nullable(),
  socialSecurity: yup
    .number()
    .typeError("SSN must be a number")
    .integer("SSN must be an integer")
    .min(0, "SSN must be positive")
    .max(999999999, "SSN must be 9 digits or less")
    .nullable(),
  fullNameContains: yup.string().typeError("Full Name must be a string").nullable(),
  hasExecutiveHoursAndDollars: yup.boolean().default(false).required()
});

const ManageExecutiveHoursAndDollarsSearchFilter = () => {
  const [triggerSearch, { isFetching }] = useLazyGetExecutiveHoursAndDollarsQuery();

  const dispatch = useDispatch();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<ExecutiveHoursAndDollarsSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: undefined
      /*
      fullNameContains: "",
      badgeNumber: "",
      socialSecurity: "",
      hasExecutiveHoursAndDollars: false
      */
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      triggerSearch(
        {
          pagination: { skip: 0, take: 25 },
          profitYear: data.profitYear,
          ...(!!data.socialSecurity && { socialSecurity: data.socialSecurity }),
          ...(!!data.badgeNumber && { badgeNumber: data.badgeNumber }),
          hasExecutiveHoursAndDollars: data.hasExecutiveHoursAndDollars ?? false,
          ...(!!data.fullNameContains && { fullNameContains: data.fullNameContains })
        },
        false
      );
      // Now we need to set the Grid pending state's
      // profit year. We have to do it via redux because
      // the grid data has no mention of profit year,
      // but we need the year to submit changes.
      dispatch(setExecutiveHoursAndDollarsGridYear(data.profitYear));
    }
  });

  const handleReset = () => {
    // If we ever decide that the reset button should clear pending changes
    // uncomment this next line...
    // dispatch(clearExecutiveHoursAndDollarsGridRows());
    // ... and then import clearExecutiveHoursAndDollarsGridRows
    // from reduxstore/slices/yearsEndSlice

    reset({
      profitYear: undefined,
      fullNameContains: null,
      badgeNumber: null,
      socialSecurity: null,
      hasExecutiveHoursAndDollars: false
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
          <Grid2
            xs={12}
            sm={6}
            md={3}>
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
                  inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
                />
              )}
            />
            {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
          </Grid2>
          <Grid2
            xs={12}
            sm={6}
            md={3}>
            <FormLabel>Full Name</FormLabel>
            <Controller
              name="fullNameContains"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  variant="outlined"
                  error={!!errors.fullNameContains}
                  onChange={(e) => {
                    field.onChange(e);
                  }}
                  inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
                />
              )}
            />
            {errors.fullNameContains && <FormHelperText error>{errors.fullNameContains.message}</FormHelperText>}
          </Grid2>
          <Grid2
            xs={12}
            sm={6}
            md={3}>
            <FormLabel>SSN</FormLabel>
            <Controller
              name="socialSecurity"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.socialSecurity}
                  onChange={(e) => {
                    if (!isNaN(Number(e.target.value))) {
                      const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                      field.onChange(parsedValue);
                    }
                  }}
                />
              )}
            />
            {errors.socialSecurity && <FormHelperText error>{errors.socialSecurity.message}</FormHelperText>}
          </Grid2>
          <Grid2
            xs={12}
            sm={6}
            md={3}>
            <FormLabel>Badge Number</FormLabel>
            <Controller
              name="badgeNumber"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.badgeNumber}
                  onChange={(e) => {
                    if (!isNaN(Number(e.target.value))) {
                      const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                      field.onChange(parsedValue);
                    }
                  }}
                  //inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
                />
              )}
            />
            {errors.badgeNumber && <FormHelperText error>{errors.badgeNumber.message}</FormHelperText>}
          </Grid2>
          <Grid2
            xs={12}
            sm={6}
            md={3}>
            <FormLabel>Has Executive Hours and Dollars</FormLabel>
            <Controller
              name="hasExecutiveHoursAndDollars"
              control={control}
              render={({ field }) => (
                <Checkbox
                  checked={field.value}
                  onChange={field.onChange}
                />
              )}
            />
            {errors.hasExecutiveHoursAndDollars && (
              <FormHelperText error>{errors.hasExecutiveHoursAndDollars.message}</FormHelperText>
            )}
          </Grid2>
        </Grid2>
      </Grid2>
      <Grid2
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isFetching}
        />
      </Grid2>
    </form>
  );
};

export default ManageExecutiveHoursAndDollarsSearchFilter;
