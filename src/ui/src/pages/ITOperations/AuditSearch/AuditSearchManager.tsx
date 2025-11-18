import { yupResolver } from "@hookform/resolvers/yup";
import { Box, Button, Grid, TextField } from "@mui/material";
import { Controller, Resolver, useForm } from "react-hook-form";
import { DSMDatePicker } from "smart-ui-library";
import * as yup from "yup";

interface AuditSearchFilters {
  tableName: string;
  operation: string;
  userName: string;
  startDate: Date | null;
  endDate: Date | null;
}

const schema = yup.object().shape({
  tableName: yup.string(),
  operation: yup.string(),
  userName: yup.string(),
  startDate: yup
    .date()
    .nullable()
    .test("not-future", "Start date cannot be in the future", (value) => {
      if (!value) return true;
      return value <= new Date();
    }),
  endDate: yup
    .date()
    .nullable()
    .test("not-future", "End date cannot be in the future", (value) => {
      if (!value) return true;
      return value <= new Date();
    })
    .test("after-start", "End date must be after start date", function (value) {
      const { startDate } = this.parent;
      if (!value || !startDate) return true;
      return value >= startDate;
    })
});

interface AuditSearchManagerProps {
  onSearch: (filters: AuditSearchFilters) => void;
  isLoading: boolean;
}

const AuditSearchManager: React.FC<AuditSearchManagerProps> = ({ onSearch, isLoading }) => {
  const {
    control,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm<AuditSearchFilters>({
    resolver: yupResolver(schema) as Resolver<AuditSearchFilters>,
    defaultValues: {
      tableName: "",
      operation: "",
      userName: "",
      startDate: null,
      endDate: null
    },
    mode: "onChange"
  });

  const onSubmit = handleSubmit((data) => {
    onSearch(data);
  });

  const handleReset = () => {
    reset({
      tableName: "",
      operation: "",
      userName: "",
      startDate: null,
      endDate: null
    });
    onSearch({
      tableName: "",
      operation: "",
      userName: "",
      startDate: null,
      endDate: null
    });
  };

  return (
    <form onSubmit={onSubmit}>
      <Box sx={{ paddingX: "24px" }}>
        <Grid
          container
          spacing={2}
          alignItems="flex-end">
          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
            <Controller
              name="tableName"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="Table Name"
                  fullWidth
                  error={!!errors.tableName}
                  helperText={errors.tableName?.message}
                />
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
            <Controller
              name="operation"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="Operation"
                  fullWidth
                  error={!!errors.operation}
                  helperText={errors.operation?.message}
                />
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
            <Controller
              name="userName"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="User Name"
                  fullWidth
                  error={!!errors.userName}
                  helperText={errors.userName?.message}
                />
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
            <Controller
              name="startDate"
              control={control}
              render={({ field }) => (
                <DSMDatePicker
                  id="startDate"
                  onChange={(date: Date | null) => field.onChange(date)}
                  value={field.value}
                  label="Begin Date"
                  required={false}
                  disableFuture
                  error={errors.startDate?.message}
                />
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
            <Controller
              name="endDate"
              control={control}
              render={({ field }) => (
                <DSMDatePicker
                  id="endDate"
                  onChange={(date: Date | null) => field.onChange(date)}
                  value={field.value}
                  label="End Date"
                  required={false}
                  disableFuture
                  error={errors.endDate?.message}
                />
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 2 }}>
            <Box sx={{ display: "flex", gap: 1 }}>
              <Button
                type="submit"
                variant="contained"
                color="primary"
                disabled={isLoading}>
                Search
              </Button>
              <Button
                type="button"
                variant="outlined"
                onClick={handleReset}
                disabled={isLoading}>
                Reset
              </Button>
            </Box>
          </Grid>
        </Grid>
      </Box>
    </form>
  );
};

export default AuditSearchManager;
