import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, Grid, TextField } from "@mui/material";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import React from "react";
import { Controller, Resolver, useForm, useWatch } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

export interface DivorceReportFilterParams {
  badgeNumber: string;
  startDate: Date | null;
  endDate: Date | null;
}

const schema = yup.object().shape({
  badgeNumber: yup
    .string()
    .required("Badge Number is required")
    .matches(/^\d+$/, "Badge Number must be a valid integer")
    .min(1, "Badge Number must be greater than 0")
    .max(9999999, "Badge Number must not exceed 7 digits"),
  startDate: yup.date().nullable().required("Start Date is required"),
  endDate: yup.date().nullable().required("End Date is required")
});

interface DivorceReportFilterSectionProps {
  onFilterChange: (params: DivorceReportFilterParams) => void;
  onReset: () => void;
  isLoading?: boolean;
}

const DivorceReportFilterSection: React.FC<DivorceReportFilterSectionProps> = ({
  onFilterChange,
  onReset,
  isLoading = false
}) => {
  const currentYear = new Date().getFullYear();

  const {
    control,
    handleSubmit,
    formState: { errors },
    reset,
    trigger
  } = useForm<DivorceReportFilterParams>({
    resolver: yupResolver(schema) as Resolver<DivorceReportFilterParams>,
    defaultValues: {
      badgeNumber: "",
      startDate: new Date(currentYear - 1, 0, 1),
      endDate: new Date(currentYear, 11, 31)
    }
  });

  const validateAndSubmit = handleSubmit((data) => {
    const dataCopy: DivorceReportFilterParams = {
      badgeNumber: data.badgeNumber,
      startDate: data.startDate ? new Date(data.startDate.getTime()) : null,
      endDate: data.endDate ? new Date(data.endDate.getTime()) : null
    };

    onFilterChange(dataCopy);
  });

  const watchedValues = useWatch({ control });
  const isSearchEnabled =
    watchedValues.badgeNumber &&
    watchedValues.startDate &&
    watchedValues.endDate &&
    !errors.badgeNumber &&
    !errors.startDate &&
    !errors.endDate;

  const handleReset = () => {
    reset({
      badgeNumber: "",
      startDate: new Date(currentYear - 1, 0, 1),
      endDate: new Date(currentYear, 11, 31)
    });
    onReset();
  };

  return (
    <form onSubmit={validateAndSubmit}>
      <Grid
        container
        paddingX="24px"
        gap="24px">
        <Grid size={{ xs: 12, sm: 4, md: 3 }}>
          <Controller
            name="badgeNumber"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                label="Badge Number"
                type="number"
                placeholder="Enter badge number"
                required={true}
                fullWidth
                size="small"
                error={!!errors.badgeNumber}
                helperText={errors.badgeNumber?.message}
                inputProps={{ min: "1", max: "9999999" }}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
                  field.onChange(e.target.value);
                  trigger("badgeNumber");
                }}
              />
            )}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 4, md: 3 }}>
          <Controller
            name="startDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="startDate"
                onChange={(value: Date | null) => {
                  field.onChange(value);
                  trigger("startDate");
                }}
                value={field.value || null}
                required={true}
                label="Start Date"
                disableFuture
                error={errors.startDate?.message}
              />
            )}
          />
          {errors.startDate && <FormHelperText error={true}>{errors.startDate.message}</FormHelperText>}
        </Grid>
        <Grid size={{ xs: 12, sm: 4, md: 3 }}>
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
                required={true}
                label="End Date"
                disableFuture
                error={errors.endDate?.message}
              />
            )}
          />
          {errors.endDate && <FormHelperText error={true}>{errors.endDate.message}</FormHelperText>}
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

export default DivorceReportFilterSection;
