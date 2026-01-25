import { QPAY066xAdHocReportPreset } from "@/types";
import { yupResolver } from "@hookform/resolvers/yup";
import {
  Checkbox,
  FormControl,
  FormControlLabel,
  FormHelperText,
  FormLabel,
  Grid,
  MenuItem,
  Select,
  SelectChangeEvent,
  TextField
} from "@mui/material";
import React, { useCallback, useEffect, useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { DSMDatePicker, SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { VisuallyHidden } from "../../../utils/accessibilityHelpers";
import { generateFieldId, getAriaDescribedBy } from "../../../utils/accessibilityUtils";
import { getLastYearDateRange } from "../../../utils/dateRangeUtils";
import { mmDDYYFormat, tryddmmyyyyToDate } from "../../../utils/dateUtils";
import { dateStringValidator, endDateStringAfterStartDateValidator } from "../../../utils/FormValidators";
import { ARIA_DESCRIPTIONS, getBadgeOrPSNPlaceholder, INPUT_PLACEHOLDERS } from "../../../utils/inputFormatters";

interface QPAY066xAdHocSearchFilterProps {
  presets: QPAY066xAdHocReportPreset[];
  currentPreset: QPAY066xAdHocReportPreset | null;
  onPresetChange: (preset: QPAY066xAdHocReportPreset | null) => void;
  onReset: () => void;
  onStoreNumberChange: (storeNumber: string) => void;
  onBadgeNumberChange: (badgeNumber: string) => void;
  onEmployeeNameChange: (employeeName: string) => void;
  onStoreManagementChange: (storeManagement: boolean) => void;
  onStartDateChange: (startDate: string) => void;
  onEndDateChange: (endDate: string) => void;
  onSearch: () => void;
  isLoading?: boolean;
}

interface QPAY066xAdHocSearchFilterFormData {
  storeNumber: number | null;
  startDate: string;
  endDate: string;
  badgeNumber: string;
  employeeName: string;
  storeManagement: boolean;
}

/*

    getBreakdownByStore: 

    getBreakdownByStoreInactive: 

    getBreakdownByStoreInactiveWithVestedBalance: 

    getBreakdownByStoreTerminatedVestedBalance: 

    getBreakdownByStoreTerminatedBalanceNotVested: 

    getBreakdownByStoreTerminatedWithBenAllocations: 

    getBreakdownByStoreTerminatedWithBalanceActivity: 

    getBreakdownByStoreRetiredWithBalanceActivity

*/
const createSchema = (requiresDateRange: boolean) =>
  yup.object().shape({
    storeNumber: yup
      .number()
      .nullable()
      .default(null)
      .test("is-positive", "Store Number must be a positive number", function (value) {
        if (value === null || value === undefined) return true; // Allow empty
        return value > 0;
      }),
    startDate: requiresDateRange
      ? dateStringValidator(2000, 2099, "Start Date").required("Start Date is required")
      : yup.string().default(""),
    endDate: requiresDateRange
      ? endDateStringAfterStartDateValidator(
          "startDate",
          tryddmmyyyyToDate,
          "End Date must be equal to or greater than Start Date"
        ).required("End Date is required")
      : yup.string().default(""),
    badgeNumber: yup
      .string()
      .default("")
      .test("is-valid-badge", "Badge Number must be between 1 and 11 digits", function (value) {
        if (!value || value === "") return true; // Allow empty
        const numValue = Number(value);
        return !isNaN(numValue) && numValue >= 1 && numValue <= 99999999999;
      }),
    employeeName: yup.string().default(""),
    storeManagement: yup.boolean().default(false)
  });

const QPAY066xAdHocSearchFilter: React.FC<QPAY066xAdHocSearchFilterProps> = ({
  presets,
  currentPreset,
  onPresetChange,
  onReset,
  onStoreNumberChange,
  onBadgeNumberChange,
  onEmployeeNameChange,
  onStoreManagementChange,
  onStartDateChange,
  onEndDateChange,
  onSearch,
  isLoading = false
}) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [badgePlaceholder, setBadgePlaceholder] = useState(INPUT_PLACEHOLDERS.BADGE_OR_PSN);
  const requiresDateRange = currentPreset?.requiresDateRange || false;

  // Get last year date range for default values
  const { beginDate, endDate: defaultEndDate } = getLastYearDateRange();

  useEffect(() => {
    if (!isLoading) {
      setIsSubmitting(false);
    }
  }, [isLoading]);

  const handleBadgeChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>, onChange: (value: string) => void) => {
      const value = e.target.value;
      // Only allow numeric input, max 11 digits
      if (value !== "" && !/^\d*$/.test(value)) {
        return;
      }
      if (value.length > 11) {
        return;
      }
      onChange(value);
      onBadgeNumberChange(value);
      setBadgePlaceholder(getBadgeOrPSNPlaceholder(value.length));
    },
    [onBadgeNumberChange]
  );

  const {
    control,
    handleSubmit,
    reset,
    watch,
    trigger,
    formState: { errors, isValid }
  } = useForm<QPAY066xAdHocSearchFilterFormData>({
    resolver: yupResolver(createSchema(requiresDateRange)),
    defaultValues: {
      storeNumber: null,
      startDate: mmDDYYFormat(beginDate),
      endDate: mmDDYYFormat(defaultEndDate),
      badgeNumber: "",
      employeeName: "",
      storeManagement: false
    }
  });

  const startDateValue = watch("startDate");

  // Watch mutually exclusive fields for badge and name
  const badgeNumberValue = useWatch({ control, name: "badgeNumber" });
  const nameValue = useWatch({ control, name: "employeeName" });

  // Helper function to check if a value exists
  const hasValue = (value: string | number | null | undefined): boolean => {
    if (value === null || value === undefined || value === "") return false;
    if (typeof value === "string" && value.trim() === "") return false;
    return true;
  };

  // Determine which fields should be disabled
  const hasBadgeNumber = hasValue(badgeNumberValue);
  const hasName = hasValue(nameValue);

  const isBadgeNumberDisabled = hasName;
  const isNameDisabled = hasBadgeNumber;

  // Helper text for disabled fields
  const getExclusionHelperText = (fieldName: string, isDisabled: boolean): string => {
    if (!isDisabled) return "";
    if (fieldName === "badgeNumber") {
      return "Disabled: Name field is in use. Press Reset to clear and re-enable.";
    }
    if (fieldName === "employeeName") {
      return "Disabled: Badge field is in use. Press Reset to clear and re-enable.";
    }
    return "";
  };

  const handlePresetChange = (event: SelectChangeEvent<string>) => {
    const presetId = event.target.value;
    const selected = presets.find((p) => p.id === presetId) || null;
    onPresetChange(selected);
  };

  const handleFormSubmit = () => {
    if (!isSubmitting) {
      setIsSubmitting(true);
      onSearch();
    }
  };

  const handleResetForm = () => {
    reset();
    onReset();
  };

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)}>
      <Grid
        container
        paddingX="24px">
        <Grid
          container
          spacing={3}
          width="100%">
          {/* Row 1: Preset and Include Managers */}
          <Grid size={{ xs: 12, sm: 6 }}>
            <FormLabel required>QPAY066 Presets</FormLabel>
            <FormControl fullWidth>
              <Select
                value={currentPreset?.id || ""}
                onChange={handlePresetChange}
                displayEmpty>
                <MenuItem value="">Select a Report</MenuItem>
                {presets.map((preset) => (
                  <MenuItem
                    key={preset.id}
                    value={preset.id}>
                    {preset.name} - {preset.description}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid
            size={{ xs: 12, sm: 6 }}
            sx={{ display: "flex", alignItems: "flex-end" }}>
            <Controller
              name="storeManagement"
              control={control}
              render={({ field }) => (
                <FormControlLabel
                  control={
                    <Checkbox
                      {...field}
                      checked={field.value}
                      onChange={(e) => {
                        field.onChange(e.target.checked);
                        onStoreManagementChange(e.target.checked);
                      }}
                    />
                  }
                  label="Include Managers"
                />
              )}
            />
          </Grid>

          {/* Row 2: Store Number, Badge, Name */}
          <Grid size={{ xs: 12, sm: 2 }}>
            <Controller
              name="storeNumber"
              control={control}
              render={({ field }) => (
                <>
                  <FormLabel htmlFor={generateFieldId("storeNumber")}>Store Number</FormLabel>
                  <TextField
                    {...field}
                    id={generateFieldId("storeNumber")}
                    value={field.value ?? ""}
                    fullWidth
                    size="small"
                    placeholder="Store #"
                    inputMode="numeric"
                    error={!!errors.storeNumber}
                    aria-invalid={!!errors.storeNumber}
                    aria-describedby={getAriaDescribedBy("storeNumber", !!errors.storeNumber, false)}
                    onChange={(e) => {
                      const value = e.target.value ? Number(e.target.value) : null;
                      field.onChange(value);
                      onStoreNumberChange(e.target.value);
                    }}
                  />
                  <div
                    id={generateFieldId("storeNumber-error")}
                    aria-live="polite"
                    aria-atomic="true">
                    {errors.storeNumber && <FormHelperText error>{errors.storeNumber.message}</FormHelperText>}
                  </div>
                </>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 2 }}>
            <Controller
              name="badgeNumber"
              control={control}
              render={({ field }) => (
                <>
                  <FormLabel htmlFor={generateFieldId("badgeNumber")}>Badge</FormLabel>
                  <TextField
                    {...field}
                    id={generateFieldId("badgeNumber")}
                    value={field.value}
                    fullWidth
                    size="small"
                    type="text"
                    placeholder={badgePlaceholder}
                    inputMode="numeric"
                    disabled={isBadgeNumberDisabled}
                    error={!!errors.badgeNumber}
                    aria-invalid={!!errors.badgeNumber}
                    aria-describedby={getAriaDescribedBy("badgeNumber", !!errors.badgeNumber, true)}
                    onChange={(e) => {
                      handleBadgeChange(e, field.onChange);
                    }}
                  />
                  <VisuallyHidden id={generateFieldId("badgeNumber-hint")}>
                    {ARIA_DESCRIPTIONS.BADGE_DYNAMIC}
                  </VisuallyHidden>
                  <div
                    id={generateFieldId("badgeNumber-error")}
                    aria-live="polite"
                    aria-atomic="true">
                    {errors.badgeNumber ? (
                      <FormHelperText error>{errors.badgeNumber.message}</FormHelperText>
                    ) : (
                      getExclusionHelperText("badgeNumber", isBadgeNumberDisabled) && (
                        <FormHelperText>{getExclusionHelperText("badgeNumber", isBadgeNumberDisabled)}</FormHelperText>
                      )
                    )}
                  </div>
                </>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Controller
              name="employeeName"
              control={control}
              render={({ field }) => (
                <>
                  <FormLabel>Name</FormLabel>
                  <TextField
                    {...field}
                    fullWidth
                    size="small"
                    type="text"
                    disabled={isNameDisabled}
                    helperText={getExclusionHelperText("employeeName", isNameDisabled)}
                    onChange={(e) => {
                      field.onChange(e.target.value);
                      onEmployeeNameChange(e.target.value);
                    }}
                  />
                </>
              )}
            />
          </Grid>
          {/* Spacer to force date fields to next row */}
          <Grid size={{ xs: 0, sm: 4 }} />

          {/* Row 3 (conditional): Start Date and End Date */}
          {requiresDateRange && (
            <>
              <Grid size={{ xs: 12, sm: 3 }}>
                <Controller
                  name="startDate"
                  control={control}
                  render={({ field }) => (
                    <>
                      <DSMDatePicker
                        id="startDate"
                        onChange={(value: Date | null) => {
                          const formatted = value ? mmDDYYFormat(value) : "";
                          field.onChange(formatted);
                          onStartDateChange(formatted);
                          trigger("startDate");
                          if (value) {
                            trigger("endDate");
                          }
                        }}
                        value={field.value ? tryddmmyyyyToDate(field.value) : null}
                        required={true}
                        label="Start Date"
                        disableFuture
                        error={errors.startDate?.message}
                      />
                      <FormHelperText
                        error
                        sx={{ minHeight: "20px", visibility: errors.startDate ? "visible" : "hidden" }}>
                        {errors.startDate?.message || "\u00A0"}
                      </FormHelperText>
                    </>
                  )}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 3 }}>
                <Controller
                  name="endDate"
                  control={control}
                  render={({ field }) => {
                    const minDateFromStart = startDateValue ? tryddmmyyyyToDate(startDateValue) : null;

                    return (
                      <>
                        <DSMDatePicker
                          id="endDate"
                          onChange={(value: Date | null) => {
                            const formatted = value ? mmDDYYFormat(value) : "";
                            field.onChange(formatted);
                            onEndDateChange(formatted);
                            trigger("endDate");
                          }}
                          value={field.value ? tryddmmyyyyToDate(field.value) : null}
                          required={true}
                          label="End Date"
                          disableFuture
                          error={errors.endDate?.message}
                          minDate={minDateFromStart ?? undefined}
                        />
                        <FormHelperText
                          error
                          sx={{ minHeight: "20px", visibility: errors.endDate ? "visible" : "hidden" }}>
                          {errors.endDate?.message || "\u00A0"}
                        </FormHelperText>
                      </>
                    );
                  }}
                />
              </Grid>
            </>
          )}
        </Grid>
      </Grid>

      <Grid
        width="100%"
        paddingX="24px"
        paddingY="16px">
        <SearchAndReset
          handleReset={handleResetForm}
          handleSearch={handleSubmit(handleFormSubmit)}
          isFetching={isLoading || isSubmitting}
          disabled={!currentPreset || isLoading || isSubmitting || !isValid}
          searchButtonText="Search"
        />
      </Grid>
    </form>
  );
};

export default QPAY066xAdHocSearchFilter;
