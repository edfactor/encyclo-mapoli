import { yupResolver } from "@hookform/resolvers/yup";
import {
    Box,
    Checkbox,
    FormControl,
    FormControlLabel,
    FormHelperText,
    FormLabel,
    Grid,
    TextField,
    Typography
} from "@mui/material";
import React, { memo, useCallback, useEffect, useMemo, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

export interface ProfitSharingAdjustmentsSearchParams {
  badgeNumber: number;
  getAllRows: boolean;
}

interface ProfitSharingAdjustmentsSearchFormData {
  badgeNumber: number;
  getAllRows: boolean;
}

const schema: yup.ObjectSchema<ProfitSharingAdjustmentsSearchFormData> = yup.object({
  badgeNumber: yup
    .number()
    .required("Badge Number is required")
    .min(1, "Badge Number must be greater than zero")
    .typeError("Badge Number is required"),
  getAllRows: yup.boolean().default(false).required()
});

interface ProfitSharingAdjustmentsSearchFilterProps {
  onSearch: (params: ProfitSharingAdjustmentsSearchParams) => void;
  onReset: () => void;
  isSearching?: boolean;
  hasUnsavedChanges?: boolean;
}

const ProfitSharingAdjustmentsSearchFilter: React.FC<ProfitSharingAdjustmentsSearchFilterProps> = memo(
  ({ onSearch, onReset, isSearching = false, hasUnsavedChanges = false }) => {
    const [isSubmitting, setIsSubmitting] = useState(false);

    const {
      control,
      handleSubmit,
      formState: { errors, isValid },
      reset
    } = useForm<ProfitSharingAdjustmentsSearchFormData>({
      resolver: yupResolver(schema),
      mode: "onBlur",
      defaultValues: {
        badgeNumber: "" as unknown as number,
        getAllRows: false
      }
    });

    useEffect(() => {
      if (!isSearching) {
        setIsSubmitting(false);
      }
    }, [isSearching]);

    const onSubmit = useCallback(
      (data: ProfitSharingAdjustmentsSearchFormData) => {
        if (isValid && !isSubmitting) {
          setIsSubmitting(true);
          onSearch({
            badgeNumber: data.badgeNumber,
            getAllRows: data.getAllRows
          });
        }
      },
      [isValid, onSearch, isSubmitting]
    );

    const validateAndSearch = handleSubmit(onSubmit);

    const handleReset = useCallback(() => {
      setIsSubmitting(false);
      reset({
        badgeNumber: "" as unknown as number,
        getAllRows: false
      });
      onReset();
    }, [reset, onReset]);

    // Enable search when all required fields have valid values
    const hasSearchCriteria = useMemo(() => {
      return isValid;
    }, [isValid]);

    // Disable the form if there are unsaved changes
    const isFormDisabled = hasUnsavedChanges || isSearching || isSubmitting;

    return (
      <form onSubmit={validateAndSearch}>
        <Grid
          container
          paddingX="24px">
          <Grid
            container
            spacing={3}
            width="100%">
            {/* Badge Number */}
            <Grid size={{ xs: 12, sm: 6, md: 4 }}>
              <FormControl fullWidth>
                <FormLabel>Badge Number</FormLabel>
                <Controller
                  name="badgeNumber"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      size="small"
                      type="text"
                      value={field.value === 0 || field.value === null ? "" : field.value}
                      error={!!errors.badgeNumber}
                      disabled={isFormDisabled}
                      onChange={(e) => {
                        const value = e.target.value;
                        // Only allow numeric input
                        if (value !== "" && !/^\d*$/.test(value)) {
                          return;
                        }
                        // Prevent input beyond 11 characters
                        if (value.length > 11) {
                          return;
                        }
                        field.onChange(value === "" ? ("" as unknown as number) : Number(value));
                      }}
                      inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
                    />
                  )}
                />
                {errors.badgeNumber && <FormHelperText error>{errors.badgeNumber.message}</FormHelperText>}
              </FormControl>
            </Grid>

            {/* Show All Rows Checkbox */}
            <Grid size={{ xs: 12, sm: 6, md: 8 }}>
              <FormControl>
                <FormLabel>&nbsp;</FormLabel>
                <Controller
                  name="getAllRows"
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={
                        <Checkbox
                          checked={field.value}
                          onChange={field.onChange}
                          disabled={isFormDisabled}
                        />
                      }
                      label={
                        <Box sx={{ display: "flex", flexDirection: "column" }}>
                          <span>Show all rows (ignore under-21 filter)</span>
                          <Typography
                            variant="caption"
                            color="text.secondary">
                            Default: only rows where the member is under 21 as of today.
                          </Typography>
                        </Box>
                      }
                    />
                  )}
                />
              </FormControl>
            </Grid>

            {/* Search and Reset Buttons */}
            <Grid size={{ xs: 12 }}>
              {hasUnsavedChanges && (
                <FormHelperText
                  error
                  sx={{ mb: 2 }}>
                  Discard changes before loading different adjustments.
                </FormHelperText>
              )}
              <SearchAndReset
                handleSearch={validateAndSearch}
                handleReset={handleReset}
                isFetching={isSearching || isSubmitting}
                disabled={!hasSearchCriteria || isFormDisabled}
              />
            </Grid>
          </Grid>
        </Grid>
      </form>
    );
  }
);

ProfitSharingAdjustmentsSearchFilter.displayName = "ProfitSharingAdjustmentsSearchFilter";

export default ProfitSharingAdjustmentsSearchFilter;
