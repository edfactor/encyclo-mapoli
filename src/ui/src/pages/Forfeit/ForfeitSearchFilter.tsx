import { yupResolver } from "@hookform/resolvers/yup";
import { Checkbox, FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { Controller, useForm } from "react-hook-form";
import { useLazyGetForfeituresAndPointsQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

interface ForfeitSearchParams {
  profitYear: number;
  useFrozenData: boolean;
}

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .required("Year is required"),
  useFrozenData: yup.boolean().default(true).required()
});

const ForfeitSearchParameters = () => {
  const [triggerSearch, { isFetching }] = useLazyGetForfeituresAndPointsQuery();

  const {
    control,
    handleSubmit,
    formState: { isValid, errors },
    reset
  } = useForm<ForfeitSearchParams>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: undefined,
      useFrozenData: true
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      triggerSearch(
        {
          profitYear: data.profitYear,
          useFrozenData: data.useFrozenData,
          pagination: { skip: 0, take: 255 }
        },
        false
      ).unwrap();
    }
  });

  const handleReset = () => {
    reset({
      profitYear: undefined,
      useFrozenData: true
    });
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Grid2
        container
        paddingX="24px"
        gap="6px">
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
        </Grid2>
        <Grid2
          xs={12}
          sm={6}
          md={3}>
          <FormLabel>Use Frozen Data</FormLabel>
          <Controller
            name="useFrozenData"
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

export default ForfeitSearchParameters;
