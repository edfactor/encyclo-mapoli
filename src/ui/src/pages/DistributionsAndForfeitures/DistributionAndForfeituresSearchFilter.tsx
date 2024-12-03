import { Checkbox, FormHelperText, FormLabel, TextField, Typography } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { useForm, Controller } from "react-hook-form";
import { useLazyGetDistributionsAndForfeituresQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { ImpersonationRoles } from "reduxstore/types";

interface DistributionsAndForfeituresSearch {
  profitYear: number;
  startMonth?: number | null;
  endMonth?: number | null;
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
    .nullable(),
  endMonth: yup
    .number()
    .typeError("End Month must be a number")
    .integer("End Month must be an integer")
    .min(1, "End Month must be 1 or higher")
    .max(12, "End Month must be 12 or lower")
    .nullable(),
  includeOutgoingForfeitures: yup.boolean().default(false).required()
});

const DistributionsAndForfeituresSearchFilter = () => {
  const [isFetching, setIsFetching] = useState(false);

  const [triggerSearch, { isLoading }] = useLazyGetDistributionsAndForfeituresQuery();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<DistributionsAndForfeituresSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: undefined,
      startMonth: null,
      endMonth: null,
      includeOutgoingForfeitures: false
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      setIsFetching(true);
      triggerSearch(
        {
          profitYear: data.profitYear,
          ...(data.startMonth && { startMonth: data.startMonth }),
          ...(data.endMonth && { endMonth: data.endMonth }),
          includeOutgoingForfeitures: data.includeOutgoingForfeitures ?? false,
          pagination: { skip: 0, take: 25 },
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
                onChange={(e) => {
                  const parsedValue = e.target.value === "" ? null : Number(e.target.value); 
                  field.onChange(parsedValue);
                }}
                inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
              />
            )}
          />
          {errors.startMonth && <FormHelperText error>{errors.startMonth.message}</FormHelperText>}
        </Grid2>
        <Grid2
          xs={12}
          sm={6}
          md={3}>
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
                onChange={(e) => {
                  const parsedValue = e.target.value === "" ? null : Number(e.target.value); 
                  field.onChange(parsedValue);
                }}
                inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
              />
            )}
          />
          {errors.endMonth && <FormHelperText error>{errors.endMonth.message}</FormHelperText>}
        </Grid2>
        <Grid2
          xs={12}
          sm={6}
          md={3}>
          <FormLabel>Include Outgoing Forfeitures</FormLabel>
          <Controller
            name="includeOutgoingForfeitures"
            control={control}
            render={({ field }) => (
              <Checkbox
                checked={field.value}
                onChange={field.onChange}
              />
            )}
          />
          {errors.includeOutgoingForfeitures && <FormHelperText error>{errors.includeOutgoingForfeitures.message}</FormHelperText>}
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
