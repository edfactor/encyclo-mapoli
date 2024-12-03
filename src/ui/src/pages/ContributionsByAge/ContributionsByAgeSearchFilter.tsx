import { FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useLazyGetContributionsByAgeQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { ImpersonationRoles, FrozenReportsByAgeRequestType } from "reduxstore/types";

interface ContributionsByAgeSearch {
  profitYear: number;
  reportType?: FrozenReportsByAgeRequestType;
}

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .required("Year is required")
});

const ContributionsByAgeSearchFilter = () => {
  const [isFetching, setIsFetching] = useState(false);

  const [triggerSearch] = useLazyGetContributionsByAgeQuery();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<ContributionsByAgeSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: undefined,
      reportType: undefined
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      setIsFetching(true);
      triggerSearch(
        {
          profitYear: data.profitYear,
          reportType: FrozenReportsByAgeRequestType.Total,
          pagination: { skip: 0, take: 255 },
          impersonation: ImpersonationRoles.ProfitSharingAdministrator
        },
        false
      );
      triggerSearch(
        {
          profitYear: data.profitYear,
          reportType: FrozenReportsByAgeRequestType.FullTime,
          pagination: { skip: 0, take: 255 },
          impersonation: ImpersonationRoles.ProfitSharingAdministrator
        },
        false
      );
      triggerSearch(
        {
          profitYear: data.profitYear,
          reportType: FrozenReportsByAgeRequestType.PartTime,
          pagination: { skip: 0, take: 255 },
          impersonation: ImpersonationRoles.ProfitSharingAdministrator
        },
        false
      );
      setIsFetching(false);
    }
  });

  const handleReset = () => {
    reset({
      profitYear: undefined,
      reportType: undefined
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
          <FormLabel>Year</FormLabel>
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

export default ContributionsByAgeSearchFilter;
