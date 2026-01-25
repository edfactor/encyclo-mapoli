import { yupResolver } from "@hookform/resolvers/yup";
import { FormLabel, Grid, TextField, Typography } from "@mui/material";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { useCallback, useEffect, useState } from "react";
import { Controller, ControllerRenderProps, Resolver, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { VisuallyHidden } from "../../../utils/accessibilityHelpers";
import { generateFieldId, getAriaDescribedBy } from "../../../utils/accessibilityUtils";
import { badgeNumberStringValidator, handleBadgeNumberStringInput, ssnValidator } from "../../../utils/FormValidators";
import {
  ARIA_DESCRIPTIONS,
  formatSSNInput,
  getBadgeOrPSNPlaceholder,
  INPUT_PLACEHOLDERS
} from "../../../utils/inputFormatters";
import useMilitaryContribution from "./hooks/useMilitaryContribution";

interface SearchFormData {
  socialSecurity?: string;
  badgeNumber?: string;
}

// Define schema with proper typing for our form
const validationSchema = yup
  .object({
    socialSecurity: ssnValidator,
    badgeNumber: badgeNumberStringValidator
  })
  .test("at-least-one-required", "At least one field must be provided", (values) =>
    Boolean(values.socialSecurity || values.badgeNumber)
  );

const MilitaryContributionSearchFilter: React.FC = () => {
  const [activeField, setActiveField] = useState<"socialSecurity" | "badgeNumber" | null>(null);
  const [badgePlaceholder, setBadgePlaceholder] = useState(INPUT_PLACEHOLDERS.BADGE_OR_PSN);
  const defaultProfitYear = useDecemberFlowProfitYear();
  const { isSearching, executeSearch, resetSearch } = useMilitaryContribution();

  // Live SSN formatting handler
  const handleSSNChange = useCallback(
    (
      e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
      field: ControllerRenderProps<SearchFormData, "socialSecurity">
    ) => {
      const { display, raw } = formatSSNInput(e.target.value);
      e.target.value = display;
      field.onChange(raw === "" ? undefined : raw);
      if (raw) setActiveField("socialSecurity");
    },
    []
  );

  // Badge change handler with dynamic placeholder
  const handleBadgeChange = useCallback(
    (value: string, field: ControllerRenderProps<SearchFormData, "badgeNumber">) => {
      const validatedValue = handleBadgeNumberStringInput(value);
      if (validatedValue !== null) {
        field.onChange(validatedValue);
        if (value) {
          setActiveField("badgeNumber");
          setBadgePlaceholder(getBadgeOrPSNPlaceholder(value.length) as "Badge or PSN");
        } else {
          setBadgePlaceholder(INPUT_PLACEHOLDERS.BADGE_OR_PSN as "Badge or PSN");
        }
      }
    },
    []
  );

  const {
    control,
    handleSubmit,
    reset,
    watch,
    formState: { errors, isValid }
  } = useForm<SearchFormData>({
    resolver: yupResolver(validationSchema) as Resolver<SearchFormData>,
    mode: "onChange"
  });

  const socialSecurity = watch("socialSecurity");
  const badgeNumber = watch("badgeNumber");

  // Update active field based on which field has input
  useEffect(() => {
    if (socialSecurity && !badgeNumber) {
      setActiveField("socialSecurity");
    } else if (badgeNumber && !socialSecurity) {
      setActiveField("badgeNumber");
    } else if (!socialSecurity && !badgeNumber) {
      setActiveField(null);
    }
  }, [socialSecurity, badgeNumber]);

  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (!isSearching) {
      setIsSubmitting(false);
    }
  }, [isSearching]);

  const onSubmit = async (data: SearchFormData) => {
    if (!isSubmitting) {
      setIsSubmitting(true);
      // executeSearch will handle the conversion internally
      await executeSearch(data, defaultProfitYear);
    }
  };

  const handleReset = () => {
    reset();
    setActiveField(null);
    resetSearch();
  };

  const requiredLabel = (
    <Typography
      component="span"
      color="error"
      fontWeight="bold">
      *
    </Typography>
  );

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Grid
        container
        paddingX="24px"
        gap="24px">
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel htmlFor={generateFieldId("socialSecurity")}>SSN {requiredLabel}</FormLabel>
          <Controller
            name="socialSecurity"
            control={control}
            render={({ field }) => (
              <>
                <TextField
                  {...field}
                  id={generateFieldId("socialSecurity")}
                  value={field.value ?? ""}
                  fullWidth
                  variant="outlined"
                  placeholder={INPUT_PLACEHOLDERS.SSN}
                  disabled={activeField === "badgeNumber"}
                  inputProps={{ inputMode: "numeric" }}
                  aria-invalid={!!errors.socialSecurity || undefined}
                  aria-describedby={getAriaDescribedBy("socialSecurity", !!errors.socialSecurity, true)}
                  onChange={(e) => {
                    handleSSNChange(e, field as ControllerRenderProps<SearchFormData, "socialSecurity">);
                  }}
                />
                <VisuallyHidden id="socialSecurity-hint">{ARIA_DESCRIPTIONS.SSN_FORMAT}</VisuallyHidden>
                {errors.socialSecurity && (
                  <div
                    id="socialSecurity-error"
                    aria-live="polite"
                    aria-atomic="true">
                    <Typography
                      variant="caption"
                      color="error">
                      {errors.socialSecurity.message}
                    </Typography>
                  </div>
                )}
              </>
            )}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel htmlFor={generateFieldId("badgeNumber")}>Badge Number {requiredLabel}</FormLabel>
          <Controller
            name="badgeNumber"
            control={control}
            render={({ field }) => (
              <>
                <TextField
                  {...field}
                  id={generateFieldId("badgeNumber")}
                  value={field.value ?? ""}
                  fullWidth
                  variant="outlined"
                  placeholder={badgePlaceholder}
                  disabled={activeField === "socialSecurity"}
                  inputProps={{ inputMode: "numeric" }}
                  aria-invalid={!!errors.badgeNumber || undefined}
                  aria-describedby={getAriaDescribedBy("badgeNumber", !!errors.badgeNumber, true)}
                  onChange={(e) => {
                    handleBadgeChange(e.target.value, field as ControllerRenderProps<SearchFormData, "badgeNumber">);
                  }}
                />
                <VisuallyHidden id="badgeNumber-hint">{ARIA_DESCRIPTIONS.BADGE_FORMAT}</VisuallyHidden>
                {errors.badgeNumber && (
                  <div
                    id="badgeNumber-error"
                    aria-live="polite"
                    aria-atomic="true">
                    <Typography
                      variant="caption"
                      color="error">
                      {errors.badgeNumber.message}
                    </Typography>
                  </div>
                )}
              </>
            )}
          />
        </Grid>
      </Grid>
      <Grid
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={handleSubmit(onSubmit)}
          isFetching={isSearching || isSubmitting}
          disabled={!isValid || (!socialSecurity && !badgeNumber) || isSearching || isSubmitting}
        />
      </Grid>
    </form>
  );
};

export default MilitaryContributionSearchFilter;
