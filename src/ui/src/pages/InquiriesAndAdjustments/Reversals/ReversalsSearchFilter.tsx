import { yupResolver } from "@hookform/resolvers/yup";
import {
  FormControl,
  FormControlLabel,
  FormHelperText,
  FormLabel,
  Grid,
  Radio,
  RadioGroup,
  TextField
} from "@mui/material";
import React, { memo, useCallback, useEffect, useMemo, useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { MAX_EMPLOYEE_BADGE_LENGTH } from "../../../constants";

export interface ReversalsSearchParams {
  socialSecurity?: string | null;
  badgeNumber?: number | null;
  memberType: "all" | "employees" | "beneficiaries";
}

interface ReversalsSearchFormData {
  socialSecurity: string | null;
  badgeNumber: number | null;
  memberType: "all" | "employees" | "beneficiaries";
}

const schema: yup.ObjectSchema<ReversalsSearchFormData> = yup.object({
  socialSecurity: yup.string().nullable().defined(),
  badgeNumber: yup.number().nullable().defined(),
  memberType: yup
    .string()
    .oneOf(["all", "employees", "beneficiaries"] as const)
    .default("all")
    .required()
});

interface ReversalsSearchFilterProps {
  onSearch: (params: ReversalsSearchParams) => void;
  onReset: () => void;
  isSearching?: boolean;
}

const ReversalsSearchFilter: React.FC<ReversalsSearchFilterProps> = memo(
  ({ onSearch, onReset, isSearching = false }) => {
    const [isSubmitting, setIsSubmitting] = useState(false);

    const {
      control,
      handleSubmit,
      formState: { errors, isValid },
      reset,
      setValue
    } = useForm<ReversalsSearchFormData>({
      resolver: yupResolver(schema),
      mode: "onBlur",
      defaultValues: {
        socialSecurity: null,
        badgeNumber: null,
        memberType: "all"
      }
    });

    useEffect(() => {
      if (!isSearching) {
        setIsSubmitting(false);
      }
    }, [isSearching]);

    const onSubmit = useCallback(
      (data: ReversalsSearchFormData) => {
        if (isValid && !isSubmitting) {
          setIsSubmitting(true);
          onSearch({
            socialSecurity: data.socialSecurity ?? undefined,
            badgeNumber: data.badgeNumber ?? undefined,
            memberType: data.memberType
          });
        }
      },
      [isValid, onSearch, isSubmitting]
    );

    const validateAndSearch = handleSubmit(onSubmit);

    const handleReset = useCallback(() => {
      setIsSubmitting(false);
      reset({
        socialSecurity: null,
        badgeNumber: null,
        memberType: "all"
      });
      onReset();
    }, [reset, onReset]);

    // Auto-switch memberType when badge length indicates beneficiary
    const handleBadgeNumberChange = useCallback(
      (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const badgeStr = e.target.value;

        if (badgeStr.length === 0) {
          setValue("memberType", "all");
        } else if (badgeStr.length > MAX_EMPLOYEE_BADGE_LENGTH) {
          setValue("memberType", "beneficiaries");
        }
      },
      [setValue]
    );

    // Watch the two mutually exclusive fields
    const socialSecurityValue = useWatch({ control, name: "socialSecurity" });
    const badgeNumberValue = useWatch({ control, name: "badgeNumber" });

    // Helper function to check if a value is non-empty
    const hasValue = useCallback(
      (value: string | number | null | undefined) => value !== null && value !== undefined && value !== "",
      []
    );

    const hasSocialSecurity = hasValue(socialSecurityValue);
    const hasBadgeNumber = hasValue(badgeNumberValue);

    // Mutual exclusion: disable one when other has value
    const isSocialSecurityDisabled = hasBadgeNumber;
    const isBadgeNumberDisabled = hasSocialSecurity;

    // Lock memberType radio when badge length indicates beneficiary
    const isMemberTypeDisabled =
      badgeNumberValue != null && String(badgeNumberValue).length > MAX_EMPLOYEE_BADGE_LENGTH;

    // Helper text for mutual exclusion
    const getExclusionHelperText = useCallback(
      (fieldName: string, isDisabled: boolean) => {
        if (!isDisabled) return undefined;

        if (fieldName === "socialSecurity") {
          if (hasBadgeNumber) return "Disabled: Badge/PSN field is in use. Press Reset to clear and re-enable.";
        }
        if (fieldName === "badgeNumber") {
          if (hasSocialSecurity) return "Disabled: SSN field is in use. Press Reset to clear and re-enable.";
        }

        return undefined;
      },
      [hasSocialSecurity, hasBadgeNumber]
    );

    // Enable search when at least one identifier is provided
    const hasSearchCriteria = useMemo(() => {
      return hasSocialSecurity || hasBadgeNumber;
    }, [hasSocialSecurity, hasBadgeNumber]);

    const memberTypeOptions = useMemo(
      () => [
        { value: "all", label: "All" },
        { value: "employees", label: "Employees" },
        { value: "beneficiaries", label: "Beneficiaries" }
      ],
      []
    );

    return (
      <form onSubmit={validateAndSearch}>
        <Grid
          container
          paddingX="24px">
          <Grid
            container
            spacing={3}
            width="100%">
            {/* Social Security Number */}
            <Grid size={{ xs: 12, sm: 6, md: 4 }}>
              <FormLabel>Social Security Number</FormLabel>
              <Controller
                name="socialSecurity"
                control={control}
                render={({ field }) => (
                  <TextField
                    name={field.name}
                    ref={field.ref}
                    onBlur={field.onBlur}
                    fullWidth
                    size="small"
                    variant="outlined"
                    value={field.value ?? ""}
                    error={!!errors.socialSecurity}
                    disabled={isSocialSecurityDisabled}
                    onChange={(e) => {
                      const value = e.target.value;
                      // Only allow numeric input
                      if (value !== "" && !/^\d*$/.test(value)) {
                        return;
                      }
                      // Prevent input beyond 9 characters
                      if (value.length > 9) {
                        return;
                      }
                      field.onChange(value === "" ? null : Number(value));
                    }}
                    sx={
                      isSocialSecurityDisabled
                        ? {
                            "& .MuiOutlinedInput-root": {
                              backgroundColor: "#f5f5f5"
                            }
                          }
                        : undefined
                    }
                  />
                )}
              />
              {errors.socialSecurity && <FormHelperText error>{errors.socialSecurity.message}</FormHelperText>}
              {!errors.socialSecurity && getExclusionHelperText("socialSecurity", isSocialSecurityDisabled) && (
                <FormHelperText sx={{ color: "info.main", fontSize: "0.75rem", marginTop: "4px" }}>
                  {getExclusionHelperText("socialSecurity", isSocialSecurityDisabled)}
                </FormHelperText>
              )}
            </Grid>

            {/* Badge/PSN Number */}
            <Grid size={{ xs: 12, sm: 6, md: 4 }}>
              <FormLabel>Badge/PSN Number</FormLabel>
              <Controller
                name="badgeNumber"
                control={control}
                render={({ field }) => (
                  <TextField
                    name={field.name}
                    ref={field.ref}
                    onBlur={field.onBlur}
                    fullWidth
                    size="small"
                    variant="outlined"
                    value={field.value ?? ""}
                    error={!!errors.badgeNumber}
                    disabled={isBadgeNumberDisabled}
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
                      field.onChange(value === "" ? null : Number(value));
                      handleBadgeNumberChange(e);
                    }}
                    sx={
                      isBadgeNumberDisabled
                        ? {
                            "& .MuiOutlinedInput-root": {
                              backgroundColor: "#f5f5f5"
                            }
                          }
                        : undefined
                    }
                  />
                )}
              />
              {errors.badgeNumber && <FormHelperText error>{errors.badgeNumber.message}</FormHelperText>}
              {!errors.badgeNumber && getExclusionHelperText("badgeNumber", isBadgeNumberDisabled) && (
                <FormHelperText sx={{ color: "info.main", fontSize: "0.75rem", marginTop: "4px" }}>
                  {getExclusionHelperText("badgeNumber", isBadgeNumberDisabled)}
                </FormHelperText>
              )}
            </Grid>

            {/* Member Type */}
            <Grid size={{ xs: 12, sm: 6, md: 4 }}>
              <FormControl error={!!errors.memberType}>
                <FormLabel>Member Type</FormLabel>
                <Controller
                  name="memberType"
                  control={control}
                  render={({ field }) => (
                    <RadioGroup
                      {...field}
                      row>
                      {memberTypeOptions.map((option) => (
                        <FormControlLabel
                          key={option.value}
                          value={option.value}
                          control={
                            <Radio
                              size="small"
                              disabled={isMemberTypeDisabled}
                            />
                          }
                          label={option.label}
                          disabled={isMemberTypeDisabled}
                        />
                      ))}
                    </RadioGroup>
                  )}
                />
              </FormControl>
            </Grid>

            {/* Search and Reset Buttons */}
            <Grid size={{ xs: 12 }}>
              <SearchAndReset
                handleSearch={validateAndSearch}
                handleReset={handleReset}
                isFetching={isSearching || isSubmitting}
                disabled={!isValid || isSearching || isSubmitting || !hasSearchCriteria}
              />
            </Grid>
          </Grid>
        </Grid>
      </form>
    );
  }
);

ReversalsSearchFilter.displayName = "ReversalsSearchFilter";

export default ReversalsSearchFilter;
