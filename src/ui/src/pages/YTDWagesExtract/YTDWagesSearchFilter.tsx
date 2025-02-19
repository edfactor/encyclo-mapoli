import { FormHelperText } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { useForm, Controller } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { useLazyGetBalanceByAgeQuery } from "reduxstore/api/YearsEndApi";

interface YTDWagesSearch {
  profitYear: Date;
}

const schema = yup.object().shape({
  profitYear: yup
    .date()
    .required("Year is required")
    .min(new Date(2020, 0, 1), "Year must be 2020 or later")
    .max(new Date(2100, 11, 31), "Year must be 2100 or earlier")
    .typeError("Invalid date")
});

const YTDWagesSearchFilter = () => {
  const [triggerSearch, { isFetching }] = useLazyGetBalanceByAgeQuery();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    setValue
  } = useForm<YTDWagesSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: undefined
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      triggerSearch(
        {
          profitYear: data.profitYear.getFullYear(),
          pagination: { skip: 0, take: 25 }
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
          <Controller
            name="profitYear"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="profitYear"
                onChange={(value: Date | null) => field.onChange(value)}
                value={field.value ?? null}
                required={true}
                label="Profit Year"
                disableFuture
                views={["year"]}
                error={errors.profitYear?.message}
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

export default YTDWagesSearchFilter;
