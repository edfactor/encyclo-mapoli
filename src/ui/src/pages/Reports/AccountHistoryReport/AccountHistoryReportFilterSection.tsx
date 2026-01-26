import { AccountHistoryReportTotals } from "@/types/reports/AccountHistoryReportTypes";
import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, TextField } from "@mui/material";
import React, { useCallback, useEffect, useState } from "react";
import { Controller, Resolver, useForm, useWatch } from "react-hook-form";
import { DSMDatePicker, SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { useFakeTimeAwareYear } from "../../../hooks/useFakeTimeAwareDate";
import { VisuallyHidden } from "../../../utils/accessibilityHelpers";
import { generateFieldId, getAriaDescribedBy } from "../../../utils/accessibilityUtils";
import { ARIA_DESCRIPTIONS, getBadgeOrPSNPlaceholder, INPUT_PLACEHOLDERS } from "../../../utils/inputFormatters";

export interface AccountHistoryReportFilterParams {
  badgeNumber: string;
  startDate: Date | null;
  endDate: Date | null;
  cumulativeTotals?: AccountHistoryReportTotals;
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

interface AccountHistoryReportFilterSectionProps {
  onFilterChange: (params: AccountHistoryReportFilterParams) => void;
  onReset: () => void;
  onFormDirty?: () => void;
  isLoading?: boolean;
}

const AccountHistoryReportFilterSection: React.FC<AccountHistoryReportFilterSectionProps> = ({
  onFilterChange,
  onReset,
  onFormDirty,
  isLoading = false
}) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [badgePlaceholder, setBadgePlaceholder] = useState(INPUT_PLACEHOLDERS.BADGE_OR_PSN);
  const currentYear = useFakeTimeAwareYear();
  // Default start date goes back 5 years from current year
  const defaultStartDate = new Date(currentYear - 5, 0, 1);
  // Minimum allowed date goes back 75 years from current year
  const minStartDate = new Date(currentYear - 75, 0, 1);

  const {
    control,
    handleSubmit,
    formState: { errors },
    reset,
    trigger
  } = useForm<AccountHistoryReportFilterParams>({
    resolver: yupResolver(schema) as Resolver<AccountHistoryReportFilterParams>,
    defaultValues: {
      badgeNumber: "",
      startDate: defaultStartDate,
      endDate: new Date(currentYear, 11, 31) // December 31 (month is 0-indexed)
    }
  });

  useEffect(() => {
    if (!isLoading) {
      setIsSubmitting(false);
    }
  }, [isLoading]);

  const handleBadgeChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>, onChange: (value: string) => void) => {
      const value = e.target.value;
      onChange(value);
      trigger("badgeNumber");
      setBadgePlaceholder(getBadgeOrPSNPlaceholder(value.length) as "Badge or PSN");
    },
    [trigger]
  );

  const validateAndSubmit = handleSubmit((data) => {
    if (!isSubmitting) {
      setIsSubmitting(true);
      const dataCopy: AccountHistoryReportFilterParams = {
        badgeNumber: data.badgeNumber,
        startDate: data.startDate ? new Date(data.startDate.getTime()) : null,
        endDate: data.endDate ? new Date(data.endDate.getTime()) : null
      };

      onFilterChange(dataCopy);
    }
  });

  const watchedValues = useWatch({ control });
  const isSearchEnabled =
    watchedValues.badgeNumber &&
    watchedValues.startDate &&
    watchedValues.endDate &&
    !errors.badgeNumber &&
    !errors.startDate &&
    !errors.endDate;

  // Notify parent when form values change (user is modifying criteria)
  useEffect(() => {
    onFormDirty?.();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [watchedValues.badgeNumber, watchedValues.startDate?.getTime(), watchedValues.endDate?.getTime()]);

  const handleReset = () => {
    reset({
      badgeNumber: "",
      startDate: defaultStartDate,
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
        <Grid size={{ xs: 12, sm: 4, md: 2 }}>
          <FormLabel
            htmlFor={generateFieldId("badgeNumber")}
            required={true}>
            Badge Number
          </FormLabel>
          <Controller
            name="badgeNumber"
            control={control}
            render={({ field }) => (
              <>
                <TextField
                  {...field}
                  id={generateFieldId("badgeNumber")}
                  type="text"
                  fullWidth
                  size="small"
                  variant="outlined"
                  placeholder={badgePlaceholder}
                  inputMode="numeric"
                  error={!!errors.badgeNumber}
                  aria-invalid={!!errors.badgeNumber}
                  aria-describedby={getAriaDescribedBy("badgeNumber", !!errors.badgeNumber, true)}
                  inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
                  onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
                    handleBadgeChange(e, field.onChange);
                  }}
                />
                <VisuallyHidden id={generateFieldId("badgeNumber-hint")}>
                  {ARIA_DESCRIPTIONS.BADGE_DYNAMIC}
                </VisuallyHidden>
              </>
            )}
          />
          <div
            id={generateFieldId("badgeNumber-error")}
            aria-live="polite"
            aria-atomic="true">
            {errors.badgeNumber && <FormHelperText error>{errors.badgeNumber.message}</FormHelperText>}
          </div>
        </Grid>
        <Grid size={{ xs: 12, sm: 4, md: 2 }}>
          <Controller
            name="startDate"
            control={control}
            render={({ field }) => (
              <DSMDatePicker
                id="startDate"
                onChange={(value: Date | null) => {
                  field.onChange(value);
                  trigger("startDate");
                }}
                value={field.value || null}
                required={true}
                label="Start Date"
                disableFuture
                minDate={minStartDate}
                error={errors.startDate?.message}
              />
            )}
          />
          {errors.startDate && <FormHelperText error={true}>{errors.startDate.message}</FormHelperText>}
        </Grid>
        <Grid size={{ xs: 12, sm: 4, md: 2 }}>
          <Controller
            name="endDate"
            control={control}
            render={({ field }) => (
              <DSMDatePicker
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
          isFetching={isLoading || isSubmitting}
          disabled={!isSearchEnabled || isLoading || isSubmitting}
        />
      </Grid>
    </form>
  );
};

export default AccountHistoryReportFilterSection;
