import { yupResolver } from "@hookform/resolvers/yup";
import { FormControl, FormHelperText, FormLabel, Grid, MenuItem, Select } from "@mui/material";
import React, { useEffect, useState } from "react";
import { Controller, Resolver, useForm, useWatch } from "react-hook-form";
import { DSMDatePicker, SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

export interface QPAY600FilterParams {
  profitYear?: Date | null;
  employeeType: string;
}

const schema = yup.object().shape({
  profitYear: yup.date().nullable().required("Profit Year is required"),
  employeeType: yup
    .string()
    .oneOf(["parttime", "fulltimesalaried", "fulltimehourlyearned", "fulltimehourlyaccrued"])
    .default("parttime")
});

interface QPAY600FilterSectionProps {
  onFilterChange: (params: QPAY600FilterParams) => void;
  onReset: () => void;
  isLoading?: boolean;
}

const QPAY600FilterSection: React.FC<QPAY600FilterSectionProps> = ({ onFilterChange, onReset, isLoading = false }) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const {
    control,
    handleSubmit,
    formState: { errors },
    reset,
    trigger
  } = useForm<QPAY600FilterParams>({
    resolver: yupResolver(schema) as Resolver<QPAY600FilterParams>,
    defaultValues: {
      profitYear: new Date(new Date().getFullYear() - 1, 0, 1),
      employeeType: "parttime"
    }
  });

  useEffect(() => {
    if (!isLoading) {
      setIsSubmitting(false);
    }
  }, [isLoading]);

  const validateAndSubmit = handleSubmit((data) => {
    if (!isSubmitting) {
      setIsSubmitting(true);
      const dataCopy: QPAY600FilterParams = {
        profitYear: data.profitYear ? new Date(data.profitYear.getTime()) : null,
        employeeType: data.employeeType
      };

      onFilterChange(dataCopy);
    }
  });

  const watchedValues = useWatch({ control });
  const isSearchEnabled = watchedValues.profitYear && !errors.profitYear;

  const handleReset = () => {
    reset({
      profitYear: new Date(new Date().getFullYear() - 1, 0, 1),
      employeeType: "parttime"
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
              name="profitYear"
              control={control}
              render={({ field }) => (
                <DSMDatePicker
                  id="profitYear"
                  onChange={(value: Date | null) => {
                    field.onChange(value);
                    trigger("profitYear");
                  }}
                  value={field.value || null}
                  required={true}
                  label="Profit Year"
                  views={["year"]}
                  disableFuture
                  error={errors.profitYear?.message}
                />
              )}
            />
            {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
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
                      error={!!errors.employeeType}>
                      <MenuItem value="parttime">Part Time Hourly</MenuItem>
                      <MenuItem value="fulltimesalaried">Full Time Salaried</MenuItem>
                      <MenuItem value="fulltimehourlyearned">Full Time Hourly Earned Holidays</MenuItem>
                      <MenuItem value="fulltimehourlyaccrued">Full Time Hourly Accrued Holidays</MenuItem>
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
          isFetching={isLoading || isSubmitting}
          disabled={!isSearchEnabled || isLoading || isSubmitting}
        />
      </Grid>
    </form>
  );
};

export default QPAY600FilterSection;
