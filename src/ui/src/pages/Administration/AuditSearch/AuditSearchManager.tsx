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
  startTime: string;
  endDate: Date | null;
  endTime: string;
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
  startTime: yup.string(),
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
    }),
  endTime: yup.string()
});

interface AuditSearchManagerProps {
  onSearch: (filters: AuditSearchFilters) => void;
  onReset: () => void;
  isLoading: boolean;
}

const AuditSearchManager: React.FC<AuditSearchManagerProps> = ({ onSearch, onReset, isLoading }) => {
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
      startTime: "00:00",
      endDate: null,
      endTime: "23:59"
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
      startTime: "00:00",
      endDate: null,
      endTime: "23:59"
    });
    onReset();
  };

  return (
    <form onSubmit={onSubmit}>
      <Box sx={{ paddingX: "24px" }}>
        <Grid
          container
          spacing={2}
          alignItems="flex-end">
          {/* First row: Text filters and buttons */}
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
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

          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
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

          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
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

          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
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

          {/* Second row: Date and time filters */}
          <Grid size={{ xs: 12, sm: 6, md: 2.5 }}>
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

          <Grid size={{ xs: 12, sm: 6, md: 1.5 }}>
            <Controller
              name="startTime"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="Start Time"
                  type="time"
                  fullWidth
                  InputLabelProps={{
                    shrink: true
                  }}
                  inputProps={{
                    step: 60
                  }}
                  error={!!errors.startTime}
                  helperText={errors.startTime?.message}
                />
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 2.5 }}>
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

          <Grid size={{ xs: 12, sm: 6, md: 1.5 }}>
            <Controller
              name="endTime"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  label="End Time"
                  type="time"
                  fullWidth
                  InputLabelProps={{
                    shrink: true
                  }}
                  inputProps={{
                    step: 60
                  }}
                  error={!!errors.endTime}
                  helperText={errors.endTime?.message}
                />
              )}
            />
          </Grid>
        </Grid>
      </Box>
    </form>
  );
};

export default AuditSearchManager;
