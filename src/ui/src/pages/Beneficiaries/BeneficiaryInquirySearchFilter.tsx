import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, TextField } from "@mui/material";
import { useCallback, useMemo } from "react";
import { Controller, Resolver, useForm, useWatch } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { MAX_EMPLOYEE_BADGE_LENGTH } from "../../constants";
import { BeneficiarySearchAPIRequest } from "../../types";
import { badgeNumberOrPSNValidator, ssnValidator } from "../../utils/FormValidators";

const schema = yup.object().shape({
  badgePsn: badgeNumberOrPSNValidator,
  name: yup.string().nullable(),
  socialSecurity: ssnValidator,
  memberType: yup.string().oneOf(["beneficiaries"]).default("beneficiaries").required()
});
interface beneficiaryRequest {
  badgePsn?: number | null;
  name?: string | null;
  socialSecurity?: string | null;
  memberType: string;
}
// Define the type of props
type BeneficiaryInquirySearchFilterProps = {
  onSearch: (params: BeneficiarySearchAPIRequest | undefined) => void;
};

const BeneficiaryInquirySearchFilter: React.FC<BeneficiaryInquirySearchFilterProps> = ({ onSearch }) => {
  const {
    control,
    formState: { errors, isValid },
    handleSubmit,
    reset
  } = useForm<beneficiaryRequest>({
    resolver: yupResolver(schema) as Resolver<beneficiaryRequest>,
    mode: "onBlur",
    defaultValues: {
      badgePsn: undefined,
      name: undefined,
      socialSecurity: undefined,
      memberType: "beneficiaries"
    }
  });

  // Watch the three mutually exclusive fields
  const socialSecurityValue = useWatch({ control, name: "socialSecurity" });
  const nameValue = useWatch({ control, name: "name" });
  const badgeNumberValue = useWatch({ control, name: "badgePsn" });

  // Helper function to check if a value is non-empty
  const hasValue = useCallback(
    (value: string | number | null | undefined) => value !== null && value !== undefined && value !== "",
    []
  );

  // Determine which fields should be disabled based on which have values
  const hasSocialSecurity = hasValue(socialSecurityValue);
  const hasName = hasValue(nameValue);
  const hasBadgeNumber = hasValue(badgeNumberValue);

  // Disable other fields when one has a value
  const isSocialSecurityDisabled = hasName || hasBadgeNumber;
  const isNameDisabled = hasSocialSecurity || hasBadgeNumber;
  const isBadgeNumberDisabled = hasSocialSecurity || hasName;

  // Helper text for mutual exclusion
  const getExclusionHelperText = useCallback(
    (fieldName: string, isDisabled: boolean) => {
      if (!isDisabled) return undefined;

      if (fieldName === "socialSecurity") {
        if (hasName) return "Disabled: Name field is in use. Press Reset to clear and re-enable.";
        if (hasBadgeNumber) return "Disabled: Badge/PSN field is in use. Press Reset to clear and re-enable.";
      }
      if (fieldName === "name") {
        if (hasSocialSecurity) return "Disabled: SSN field is in use. Press Reset to clear and re-enable.";
        if (hasBadgeNumber) return "Disabled: Badge/PSN field is in use. Press Reset to clear and re-enable.";
      }
      if (fieldName === "badgePsn") {
        if (hasSocialSecurity) return "Disabled: SSN field is in use. Press Reset to clear and re-enable.";
        if (hasName) return "Disabled: Name field is in use. Press Reset to clear and re-enable.";
      }

      return undefined;
    },
    [hasSocialSecurity, hasName, hasBadgeNumber]
  );

  const onSubmit = (data: beneficiaryRequest) => {
    const { badgePsn, name, socialSecurity: ssn } = data;
    let badge: number | undefined = undefined;
    let psn: number | undefined = undefined;

    if (badgePsn) {
      const badgeStr = badgePsn.toString();
      if (badgeStr.length <= MAX_EMPLOYEE_BADGE_LENGTH) {
        // Badge only (7 digits or less)
        badge = Number(badgePsn);
      } else {
        // Badge + PSN (more than 7 digits)
        badge = parseInt(badgeStr.slice(0, MAX_EMPLOYEE_BADGE_LENGTH));
        psn = parseInt(badgeStr.slice(MAX_EMPLOYEE_BADGE_LENGTH));
      }
    }

    if (isValid) {
      const beneficiarySearchFilterRequest: BeneficiarySearchAPIRequest = {
        badgeNumber: badge,
        psn: psn,
        memberType: 2, // Always beneficiaries
        name: name || undefined,
        ssn: ssn || undefined,
        skip: 0,
        take: 5,
        sortBy: "name",
        isSortDescending: true
      };
      onSearch(beneficiarySearchFilterRequest);
    }
  };
  const validateAndSubmit = handleSubmit(onSubmit);

  const handleReset = () => {
    reset({
      badgePsn: undefined,
      name: undefined,
      socialSecurity: undefined,
      memberType: "beneficiaries"
    });
  };

  // Check if search button should be enabled
  const hasSearchCriteria = useMemo(() => {
    return hasSocialSecurity || hasName || hasBadgeNumber;
  }, [hasSocialSecurity, hasName, hasBadgeNumber]);

  return (
    <form onSubmit={validateAndSubmit}>
      <Grid
        container
        paddingX="24px">
        <Grid
          container
          spacing={2}
          width="100%">
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
                  type="text"
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
                    const parsedValue = value === "" ? null : value;
                    field.onChange(parsedValue);
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
            {errors?.socialSecurity && <FormHelperText error>{errors.socialSecurity.message}</FormHelperText>}
            {!errors.socialSecurity && getExclusionHelperText("socialSecurity", isSocialSecurityDisabled) && (
              <FormHelperText sx={{ color: "info.main", fontSize: "0.75rem", marginTop: "4px" }}>
                {getExclusionHelperText("socialSecurity", isSocialSecurityDisabled)}
              </FormHelperText>
            )}
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel>Name</FormLabel>
            <Controller
              name="name"
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
                  error={!!errors.name}
                  disabled={isNameDisabled}
                  onChange={(e) => {
                    const value = e.target.value === "" ? null : e.target.value;
                    field.onChange(value);
                  }}
                  sx={
                    isNameDisabled
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
            {errors?.name && <FormHelperText error>{errors.name.message}</FormHelperText>}
            {!errors.name && getExclusionHelperText("name", isNameDisabled) && (
              <FormHelperText sx={{ color: "info.main", fontSize: "0.75rem", marginTop: "4px" }}>
                {getExclusionHelperText("name", isNameDisabled)}
              </FormHelperText>
            )}
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel>Badge/PSN Number</FormLabel>
            <Controller
              name="badgePsn"
              control={control}
              render={({ field }) => (
                <TextField
                  name={field.name}
                  ref={field.ref}
                  onBlur={field.onBlur}
                  fullWidth
                  type="text"
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.badgePsn}
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
                    const parsedValue = value === "" ? null : Number(value);
                    field.onChange(parsedValue);
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
            {errors?.badgePsn && <FormHelperText error>{errors.badgePsn.message}</FormHelperText>}
            {!errors.badgePsn && getExclusionHelperText("badgePsn", isBadgeNumberDisabled) && (
              <FormHelperText sx={{ color: "info.main", fontSize: "0.75rem", marginTop: "4px" }}>
                {getExclusionHelperText("badgePsn", isBadgeNumberDisabled)}
              </FormHelperText>
            )}
          </Grid>
        </Grid>

        <Grid
          container
          justifyContent="flex-end"
          paddingY="16px">
          <Grid size={{ xs: 12 }}>
            <SearchAndReset
              handleReset={handleReset}
              handleSearch={validateAndSubmit}
              isFetching={false}
              disabled={!isValid || !hasSearchCriteria}
            />
          </Grid>
        </Grid>
      </Grid>
    </form>
  );
};

export default BeneficiaryInquirySearchFilter;
