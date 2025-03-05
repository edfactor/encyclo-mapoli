import { yupResolver } from "@hookform/resolvers/yup";
import { FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { Controller, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

interface ForfeitSearchParams {
  employeeId?: string;
  name?: string;
  ssn?: string;
  startDate?: Date | null;
  endDate?: Date | null;
}

const schema = yup.object().shape({
  employeeId: yup.string(),
  name: yup.string(),
  ssn: yup.string(),
  startDate: yup.date().nullable().typeError("Please enter a valid date"),
  endDate: yup
    .date()
    .nullable()
    .typeError("Please enter a valid date")
    .min(yup.ref("startDate"), "End Date must be after Start Date")
});

const ForfeitSearchParameters = () => {
  const {
    control,
    handleSubmit,
    formState: { errors },
    reset
  } = useForm<ForfeitSearchParams>({
    resolver: yupResolver(schema),
    defaultValues: {
      employeeId: "",
      name: "",
      ssn: "",
      startDate: null,
      endDate: null
    }
  });

  const validateAndSubmit = handleSubmit((data) => {
    console.log("Search data:", data);
  });

  const handleReset = () => {
    reset({
      employeeId: "",
      name: "",
      ssn: "",
      startDate: null,
      endDate: null
    });
  };

  return (
    <form onSubmit={validateAndSubmit}>
      <Grid2
        container
        paddingX="24px"
        alignItems="flex-end"
        gap="24px">
        <Grid2
          xs={12}
          sm={6}
          md={2}>
          <FormLabel>Badge #</FormLabel>
          <Controller
            name="employeeId"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                error={!!errors.employeeId}
                helperText={errors.employeeId?.message}
              />
            )}
          />
        </Grid2>

        <Grid2
          xs={12}
          sm={6}
          md={2}>
          <FormLabel>Name</FormLabel>
          <Controller
            name="name"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                error={!!errors.name}
                helperText={errors.name?.message}
              />
            )}
          />
        </Grid2>

        <Grid2
          xs={12}
          sm={6}
          md={2}>
          <FormLabel>SSN</FormLabel>
          <Controller
            name="ssn"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                error={!!errors.ssn}
                helperText={errors.ssn?.message}
              />
            )}
          />
        </Grid2>

        <Grid2
          xs={12}
          sm={6}
          md={2}>
          <Controller
            name="startDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="startDate"
                onChange={(value: Date | null) => field.onChange(value)}
                value={field.value ?? null}
                label="Start Date"
                required
                disableFuture
                error={errors.startDate?.message}
              />
            )}
          />
        </Grid2>

        <Grid2
          xs={12}
          sm={6}
          md={2}>
          <Controller
            name="endDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="endDate"
                onChange={(value: Date | null) => field.onChange(value)}
                value={field.value ?? null}
                label="End Date"
                required
                disableFuture
                error={errors.endDate?.message}
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
          handleSearch={validateAndSubmit}
          isFetching={false}
        />
      </Grid2>
    </form>
  );
};

export default ForfeitSearchParameters;
