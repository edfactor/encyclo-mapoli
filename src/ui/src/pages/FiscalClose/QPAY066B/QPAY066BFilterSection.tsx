import { FormControl, FormLabel, MenuItem, Select } from "@mui/material";
import { Grid } from "@mui/material";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import { DSMDatePicker, SearchAndReset } from "smart-ui-library";

export interface QPAY066BFilterParams {
  qpay066Presets: string;
  startDate?: Date | null;
  endDate?: Date | null;
  vestedPercentage: string;
  age: string;
  employeeStatus: string;
}

interface QPAY066BFilterSectionProps {
  onFilterChange: (params: QPAY066BFilterParams) => void;
  onReset: () => void;
  isLoading?: boolean;
}

const QPAY066BFilterSection: React.FC<QPAY066BFilterSectionProps> = ({
  onFilterChange,
  onReset,
  isLoading = false
}) => {
  const { control, handleSubmit, reset } = useForm<QPAY066BFilterParams>({
    defaultValues: {
      qpay066Presets: "QPay066B",
      startDate: null,
      endDate: null,
      vestedPercentage: "< 20%",
      age: "",
      employeeStatus: ""
    }
  });

  const validateAndSubmit = handleSubmit((data) => {
    onFilterChange(data);
  });

  const handleReset = () => {
    reset();
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
          <Grid size={{ xs: 12 }}>
            <Controller
              name="qpay066Presets"
              control={control}
              render={({ field }) => (
                <>
                  <FormLabel>QPay066 Presets</FormLabel>
                  <FormControl fullWidth>
                    <Select
                      {...field}
                      size="small">
                      <MenuItem value="QPay066B">QPay066B – Less than 20% Vested</MenuItem>
                    </Select>
                  </FormControl>
                </>
              )}
            />
          </Grid>
        </Grid>

        <Grid
          container
          spacing={3}
          width="100%"
          paddingTop="16px">
          <Grid size={{ xs: 12, sm: 6, md: 2.4 }}>
            <Controller
              name="startDate"
              control={control}
              render={({ field }) => (
                <DSMDatePicker
                  id="startDate"
                  onChange={field.onChange}
                  value={field.value || null}
                  label="Start Date"
                  disableFuture
                  required={false}
                />
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 2.4 }}>
            <Controller
              name="endDate"
              control={control}
              render={({ field }) => (
                <DSMDatePicker
                  id="endDate"
                  onChange={field.onChange}
                  value={field.value || null}
                  label="End Date"
                  disableFuture
                  required={false}
                />
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 2.4 }}>
            <Controller
              name="vestedPercentage"
              control={control}
              render={({ field }) => (
                <>
                  <FormLabel>Vested Percentage</FormLabel>
                  <FormControl fullWidth>
                    <Select
                      {...field}
                      size="small">
                      <MenuItem value="< 20%">Less than 20%</MenuItem>
                    </Select>
                  </FormControl>
                </>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 2.4 }}>
            <Controller
              name="age"
              control={control}
              render={({ field }) => (
                <>
                  <FormLabel>Age</FormLabel>
                  <FormControl fullWidth>
                    <Select
                      {...field}
                      size="small"
                      displayEmpty>
                      <MenuItem value=""></MenuItem>
                    </Select>
                  </FormControl>
                </>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 2.4 }}>
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
                      displayEmpty>
                      <MenuItem value=""></MenuItem>
                    </Select>
                  </FormControl>
                </>
              )}
            />
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
        />
      </Grid>
    </form>
  );
};

export default QPAY066BFilterSection;
