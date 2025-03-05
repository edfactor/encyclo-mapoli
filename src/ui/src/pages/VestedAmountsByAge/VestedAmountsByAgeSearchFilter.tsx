import { FormHelperText, FormLabel, TextField, Button } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { Controller, useForm } from "react-hook-form";
import { useLazyGetVestingAmountByAgeQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import { downloadFileFromResponse } from "utils/fileDownload"; // Import utility function
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { useDispatch } from "react-redux";

interface VestingAmountByAgeSearch {
  profitYear: number;
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

const VestedAmountsByAgeSearchFilter = () => {
  const [triggerSearch, { isFetching }] = useLazyGetVestingAmountByAgeQuery();
  const dispatch = useDispatch();
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<VestingAmountByAgeSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: undefined
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      triggerSearch(
        {
          profitYear: data.profitYear,
          acceptHeader: "application/json"
        },
        false
      );
    }
  });

  const handleReset = () => {
    reset({
      profitYear: undefined
    });
  };

  const handleDownloadCSV = handleSubmit(async (data) => {
    if (isValid) {
      try {
        const fetchPromise = triggerSearch({
          profitYear: data.profitYear,
          acceptHeader: "text/csv"
        });
        await downloadFileFromResponse(fetchPromise, `vesting-amounts-${data.profitYear}.csv`);
      } catch (error) {
        console.error("Download failed:", error);
        // Do we want to throw a formal Error for the react-error-boundary to catch?
      }
    }
  });

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
        paddingX="24px"
        display="flex"
        gap="16px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isFetching}
          disabled={!isValid}
        />
        <Button
          variant="contained"
          color="primary"
          onClick={handleDownloadCSV}
          disabled={!isValid}>
          Download CSV
        </Button>
      </Grid2>
    </form>
  );
};

export default VestedAmountsByAgeSearchFilter;
