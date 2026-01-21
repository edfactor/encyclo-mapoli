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
import { useCallback, useEffect, useMemo, useState } from "react";
import { Controller, Resolver, useForm, useWatch } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { MAX_EMPLOYEE_BADGE_LENGTH } from "../../constants";
import { BeneficiarySearchAPIRequest, BeneficiarySearchForm } from "../../types";
import { badgeNumberOrPSNValidator, ssnValidator } from "../../utils/FormValidators";
import { parseBadgeAndPSN } from "./utils/badgeUtils";

const schema = yup.object().shape({
  badgeNumber: badgeNumberOrPSNValidator,
  name: yup.string().nullable(),
  ssn: ssnValidator,
  memberType: yup.number().oneOf([0, 1, 2]).default(0).required()
});

// Define the type of props
type BeneficiaryInquirySearchFilterProps = {
  onSearch: (params: BeneficiarySearchAPIRequest | undefined) => void;
  onReset: () => void;
  isSearching?: boolean;
};

const BeneficiaryInquirySearchFilter: React.FC<BeneficiaryInquirySearchFilterProps> = ({
  onSearch,
  onReset,
  isSearching = false
}) => {
  const {
    control,
    formState: { errors, isValid },
    handleSubmit,
    reset,
    setValue
  } = useForm<BeneficiarySearchForm>({
    resolver: yupResolver(schema) as Resolver<BeneficiarySearchForm>,
    mode: "onBlur",
    defaultValues: {
      badgeNumber: undefined,
      name: undefined,
      ssn: undefined,
      memberType: 0
    }
  });

  // Watch the three mutually exclusive fields
  const ssnValue = useWatch({ control, name: "ssn" });
  const nameValue = useWatch({ control, name: "name" });
  const badgeNumberValue = useWatch({ control, name: "badgeNumber" });
  const memberTypeValue = useWatch({ control, name: "memberType" });

  // Auto-detect member type based on badge number length
  const handleBadgeNumberChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      const badgeStr = e.target.value;

      if (badgeStr.length === 0) {
        setValue("memberType", 0);
      } else if (badgeStr.length > MAX_EMPLOYEE_BADGE_LENGTH) {
        setValue("memberType", 2);
      }
    },
    [setValue]
  );

  // Member type options
  const memberTypeOptions = useMemo(
    () => [
      { value: 0, label: "All" },
      { value: 1, label: "Employees" },
      { value: 2, label: "Beneficiaries" }
    ],
    []
  );

  // Helper function to check if a value is non-empty
  const hasValue = useCallback(
    (value: string | number | null | undefined) => value !== null && value !== undefined && value !== "",
    []
  );

  // Determine which fields should be disabled based on which have values
  const hasSSN = hasValue(ssnValue);
  const hasName = hasValue(nameValue);
  const hasBadgeNumber = hasValue(badgeNumberValue);

  // Disable other fields when one has a value
  const isSSNDisabled = hasName || hasBadgeNumber;
  const isNameDisabled = hasSSN || hasBadgeNumber;
  const isBadgeNumberDisabled = hasSSN || hasName;

  // Helper text for mutual exclusion
  const getExclusionHelperText = useCallback(
    (fieldName: string, isDisabled: boolean) => {
      if (!isDisabled) return undefined;

      if (fieldName === "ssn") {
        if (hasName) return "Disabled while Name is in use. Reset to change.";
        if (hasBadgeNumber) return "Disabled while Badge/PSN is in use. Reset to change.";
      }
      if (fieldName === "name") {
        if (hasSSN) return "Disabled while SSN is in use. Reset to change.";
        if (hasBadgeNumber) return "Disabled while Badge/PSN is in use. Reset to change.";
      }
      if (fieldName === "badgeNumber") {
        if (hasSSN) return "Disabled while SSN is in use. Reset to change.";
        if (hasName) return "Disabled while Name is in use. Reset to change.";
      }

      return undefined;
    },
    [hasSSN, hasName, hasBadgeNumber]
  );

  const isMemberTypeDisabled = badgeNumberValue != null && String(badgeNumberValue).length > MAX_EMPLOYEE_BADGE_LENGTH;

  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (!isSearching) {
      setIsSubmitting(false);
    }
  }, [isSearching]);

  const onSubmit = (data: BeneficiarySearchForm) => {
    const { badgeNumber, name, ssn, memberType } = data;
    const parsedBadge = badgeNumber ? parseBadgeAndPSN(badgeNumber) : undefined;
    const badge = parsedBadge?.badge;
    const psn = parsedBadge?.psn;

    if (isValid && !isSubmitting) {
      setIsSubmitting(true);
      const beneficiarySearchFilterRequest: BeneficiarySearchAPIRequest = {
        badgeNumber: badge,
        psnSuffix: psn,
        memberType: memberType,
        name: name || undefined,
        ssn: ssn || undefined,
        skip: 0,
        take: 10,
        sortBy: "fullName",
        isSortDescending: true
      };
      onSearch(beneficiarySearchFilterRequest);
    }
  };
  const validateAndSubmit = handleSubmit(onSubmit);

  const handleReset = () => {
    reset({
      badgeNumber: undefined,
      name: undefined,
      ssn: undefined,
      memberType: 0
    });
    onReset();
  };

  // Check if search button should be enabled
  const hasSearchCriteria = useMemo(() => {
    return hasSSN || hasName || hasBadgeNumber || memberTypeValue !== 0;
  }, [hasSSN, hasName, hasBadgeNumber, memberTypeValue]);

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
              name="ssn"
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
                  error={!!errors.ssn}
                  disabled={isSSNDisabled}
                  inputProps={{ inputMode: "numeric" }}
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
                    isSSNDisabled
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
            {errors?.ssn && <FormHelperText error>{errors.ssn.message}</FormHelperText>}
            {!errors.ssn && getExclusionHelperText("ssn", isSSNDisabled) && (
              <FormHelperText sx={{ color: "info.main", fontSize: "0.75rem", marginTop: "4px" }}>
                {getExclusionHelperText("ssn", isSSNDisabled)}
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
              name="badgeNumber"
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
                  error={!!errors.badgeNumber}
                  disabled={isBadgeNumberDisabled}
                  inputProps={{ inputMode: "numeric" }}
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
                    // Auto-update memberType when badgeNumber changes
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
            {errors?.badgeNumber && <FormHelperText error>{errors.badgeNumber.message}</FormHelperText>}
            {!errors.badgeNumber && getExclusionHelperText("badgeNumber", isBadgeNumberDisabled) && (
              <FormHelperText sx={{ color: "info.main", fontSize: "0.75rem", marginTop: "4px" }}>
                {getExclusionHelperText("badgeNumber", isBadgeNumberDisabled)}
              </FormHelperText>
            )}
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 6 }}>
            <FormControl
              error={!!errors.memberType}
              disabled={isMemberTypeDisabled}>
              <FormLabel>Member Type</FormLabel>
              <Controller
                name="memberType"
                control={control}
                render={({ field }) => (
                  <RadioGroup
                    {...field}
                    onChange={(event) => {
                      field.onChange(event);
                    }}
                    row>
                    {memberTypeOptions.map((option) => (
                      <FormControlLabel
                        key={option.value}
                        value={option.value}
                        control={<Radio size="small" />}
                        label={option.label}
                      />
                    ))}
                  </RadioGroup>
                )}
              />
            </FormControl>
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
              isFetching={isSearching || isSubmitting}
              disabled={!isValid || isSearching || !hasSearchCriteria || isSubmitting}
            />
          </Grid>
        </Grid>
      </Grid>
    </form>
  );
};

export default BeneficiaryInquirySearchFilter;
