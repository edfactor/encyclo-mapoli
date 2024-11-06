import { Checkbox, FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { useForm, Controller } from "react-hook-form";
import { useLazyGetExecutiveHoursAndDollarsQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { ImpersonationRoles } from "reduxstore/types";

interface ExecutiveHoursAndDollarsSearch {
  profitYear: number;
  badgeNumber?: number | null;
  fullNameContains?: string | null;
  hasExecutiveHoursAndDollars: boolean;
}

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(1900, "Year must be 1900 or later")
    .max(2100, "Year must be 2100 or earlier")
    .required("Year is required"),
  badgeNumber: yup
    .number()
    .typeError("Badge Number must be a number")
    .integer("Badge Number must be an integer")
    .nullable(),
  fullNameContains: yup.string().typeError("Full Name must be a string").nullable(),
  hasExecutiveHoursAndDollars: yup.boolean().default(false).required()
});

const ManageExecutiveHoursAndDollarsSearchFilter = () => {
  const [isFetching, setIsFetching] = useState(false);

  const [triggerSearch, { isLoading }] = useLazyGetExecutiveHoursAndDollarsQuery();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<ExecutiveHoursAndDollarsSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: undefined,
      fullNameContains: null,
      badgeNumber: null,
      hasExecutiveHoursAndDollars: false
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      setIsFetching(true);
      triggerSearch(
        {
          pagination: { skip: 0, take: 25 },
          profitYear: data.profitYear,
          ...(!!data.badgeNumber && { badgeNumber: data.badgeNumber }),
          hasExecutiveHoursAndDollars: data.hasExecutiveHoursAndDollars ?? false,
          ...(!!data.fullNameContains && { fullNameContains: data.fullNameContains }),
          impersonation: ImpersonationRoles.ProfitSharingAdministrator
        },
        false
      );
      setIsFetching(false);
    }
  });

  const handleReset = () => {
    reset({
      profitYear: undefined
    });
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Grid2
        container
        paddingX="24px"
        gap="24px">
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
              />
            )}
          />
          {errors.fullNameContains && <FormHelperText error>{errors.fullNameContains.message}</FormHelperText>}
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
                error={!!errors.badgeNumber}
                onChange={(e) => {
                  const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                  field.onChange(parsedValue);
                }}
                inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
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
      <Grid2
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={() => {}}
          handleSearch={validateAndSearch}
          isFetching={isFetching}
        />
      </Grid2>
    </form>
  );
};

export default ManageExecutiveHoursAndDollarsSearchFilter;
