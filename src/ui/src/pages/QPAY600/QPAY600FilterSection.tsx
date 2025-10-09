import { yupResolver } from "@hookform/resolvers/yup";
import { FormControl, FormHelperText, FormLabel, Grid, MenuItem, Select } from "@mui/material";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import React from "react";
import { Controller, Resolver, useForm, useWatch } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { endDateAfterStartDateValidator } from "../../utils/FormValidators";

export interface QPAY600FilterParams {
  startDate?: Date | null;
  endDate?: Date | null;
  employeeStatus: string;
  employeeType: string;
}

const schema = yup.object().shape({
  startDate: yup.date().nullable(),
  endDate: endDateAfterStartDateValidator("startDate").nullable(),
  employeeStatus: yup.string().oneOf(["Full time", "Part time"]).default("Full time"),
  employeeType: yup.string().oneOf(["", "Hourly", "Salary"]).default("")
});

interface QPAY600FilterSectionProps {
  onFilterChange: (params: QPAY600FilterParams) => void;
  onReset: () => void;
  isLoading?: boolean;
}

const QPAY600FilterSection: React.FC<QPAY600FilterSectionProps> = ({ onFilterChange, onReset, isLoading = false }) => {
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<QPAY600FilterParams>({
    resolver: yupResolver(schema) as Resolver<QPAY600FilterParams>,
    defaultValues: {
      startDate: null,
      endDate: null,
      employeeStatus: "Full time",
      employeeType: ""
    }
  });

  const validateAndSubmit = handleSubmit((data) => {
    onFilterChange(data);
  });

  const watchedValues = useWatch({ control });
  const isSearchEnabled = watchedValues.startDate && watchedValues.endDate && !errors.startDate && !errors.endDate;

  const handleReset = () => {
    reset({
      startDate: null,
      endDate: null,
      employeeStatus: "Full time",
      employeeType: ""
    });
    onReset();
  };

  return (
    <form onSubmit={validateAndSubmit}>
      <Grid
        container
        paddingX="24px">
        <Grid
          container
          spacing={3}
          width="100%">
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Controller
              name="startDate"
              control={control}
              render={({ field }) => (
                <DsmDatePicker
                  id="startDate"
                  onChange={(value: Date | null) => {
                    field.onChange(value);
                    trigger("endDate");
                  }}
                  value={field.value || null}
                  required={false}
                  label="Start Date"
                  disableFuture
                  error={errors.startDate?.message}
                />
              )}
            />
            {errors.startDate && <FormHelperText error>{errors.startDate.message}</FormHelperText>}
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Controller
              name="endDate"
              control={control}
              render={({ field }) => (
                <DsmDatePicker
                  id="endDate"
                  onChange={(value: Date | null) => {
                    field.onChange(value);
                    trigger("endDate");
                  }}
                  value={field.value || null}
                  required={false}
                  label="End Date"
                  disableFuture
                  error={errors.endDate?.message}
                />
              )}
            />
            {errors.endDate && <FormHelperText error>{errors.endDate.message}</FormHelperText>}
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Controller
              name="employeeStatus"
              control={control}
              render={({ field }) => (
                <>
                  <FormLabel>Employee Status</FormLabel>
                  <FormControl fullWidth>
                    <Select
                      {...field}
                      size="small"
                      error={!!errors.employeeStatus}>
                      <MenuItem value="Full time">Full time</MenuItem>
                      <MenuItem value="Part time">Part time</MenuItem>
                    </Select>
                  </FormControl>
                </>
              )}
            />
            {errors.employeeStatus && <FormHelperText error>{errors.employeeStatus.message}</FormHelperText>}
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Controller
              name="employeeType"
              control={control}
              render={({ field }) => (
                <>
                  <FormLabel>Employee Type</FormLabel>
                  <FormControl fullWidth>
                    <Select
                      {...field}
                      size="small"
                      displayEmpty
                      error={!!errors.employeeType}>
                      <MenuItem value="">All</MenuItem>
                      <MenuItem value="Hourly">Hourly</MenuItem>
                      <MenuItem value="Salary">Salary</MenuItem>
                    </Select>
                  </FormControl>
                </>
              )}
            />
            {errors.employeeType && <FormHelperText error>{errors.employeeType.message}</FormHelperText>}
          </Grid>
        </Grid>
      </Grid>

      <Grid
        width="100%"
        paddingX="24px"
        paddingY="16px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSubmit}
          isFetching={isLoading}
          disabled={!isSearchEnabled || isLoading}
        />
      </Grid>
    </form>
  );
};

export default QPAY600FilterSection;
