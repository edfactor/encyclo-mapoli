import { FormHelperText } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useForm, Controller } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";

interface StoreSearchParams {
  yDate: Date;
  lastDate: Date;
  firstDate: Date;
}

const schema = yup.object().shape({
  yDate: yup.date().required("Year Date is required").typeError("Please enter a valid date"),
  lastDate: yup.date().required("Last Date is required").typeError("Please enter a valid date"),
  firstDate: yup.date().required("First Date is required").typeError("Please enter a valid date")
});

const ProfitShareByStoreParameters = () => {
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<StoreSearchParams>({
    resolver: yupResolver(schema),
    defaultValues: {
      yDate: undefined,
      lastDate: undefined,
      firstDate: undefined
    }
  });

  const validateAndSubmit = handleSubmit((data) => {
    if (isValid) {
      // TODO: Implement search functionality
      console.log("Search data:", data);
    }
  });

  const handleReset = () => {
    reset({
      yDate: undefined,
      lastDate: undefined,
      firstDate: undefined
    });
  };

  return (
    <form onSubmit={validateAndSubmit}>
      <Grid2
        container
        paddingX="24px"
        alignItems="flex-end"
        gap="24px">
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="yDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="yDate"
                onChange={(value: Date | null) => field.onChange(value)}
                value={field.value ?? null}
                required={true}
                label="Year Date"
                disableFuture
                error={errors.yDate?.message}
              />
            )}
          />
          {errors.yDate && <FormHelperText error>{errors.yDate.message}</FormHelperText>}
        </Grid2>

        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="lastDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="lastDate"
                onChange={(value: Date | null) => field.onChange(value)}
                value={field.value ?? null}
                required={true}
                label="Last Date"
                disableFuture
                error={errors.lastDate?.message}
              />
            )}
          />
          {errors.lastDate && <FormHelperText error>{errors.lastDate.message}</FormHelperText>}
        </Grid2>

        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="firstDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="firstDate"
                onChange={(value: Date | null) => field.onChange(value)}
                value={field.value ?? null}
                required={true}
                label="First Date"
                disableFuture
                error={errors.firstDate?.message}
              />
            )}
          />
          {errors.firstDate && <FormHelperText error>{errors.firstDate.message}</FormHelperText>}
        </Grid2>
      </Grid2>

      <Grid2
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSubmit}
          isFetching={false}
          disabled={!isValid}
        />
      </Grid2>
    </form>
  );
};

export default ProfitShareByStoreParameters;
