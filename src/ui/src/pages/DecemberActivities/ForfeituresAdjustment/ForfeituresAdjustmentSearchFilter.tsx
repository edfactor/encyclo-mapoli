import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, TextField, Typography } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { Controller, ControllerRenderProps, Resolver, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { useFakeTimeAwareYear } from "../../../hooks/useFakeTimeAwareDate";
import { VisuallyHidden } from "../../../utils/accessibilityHelpers";
import { generateFieldId, getAriaDescribedBy } from "../../../utils/accessibilityUtils";
import { badgeNumberStringValidator, handleBadgeNumberStringInput, ssnValidator } from "../../../utils/FormValidators";
import {
  ARIA_DESCRIPTIONS,
  formatSSNInput,
  getBadgeOrPSNPlaceholder,
  INPUT_PLACEHOLDERS
} from "../../../utils/inputFormatters";

// Define the search parameters interface
interface ForfeituresAdjustmentSearchParams {
  ssn?: string;
  badge?: string;
}

interface ForfeitureAdjustmentSearchParams {
  ssn?: string;
  badge?: string;
  profitYear: number;
  skip: number;
  take: number;
  sortBy: string;
  isSortDescending: boolean;
}

interface ForfeituresAdjustmentSearchFilterProps {
  onSearch: (params: ForfeitureAdjustmentSearchParams) => void;
  onReset: () => void;
  isSearching: boolean;
}

// Define schema for validation without circular references
const schema = yup
  .object({
    ssn: ssnValidator,
    badge: badgeNumberStringValidator
  })
  .test("at-least-one-required", "Either SSN or Badge is required", (values) => Boolean(values.ssn || values.badge));

const ForfeituresAdjustmentSearchFilter: React.FC<ForfeituresAdjustmentSearchFilterProps> = ({
  onSearch,
  onReset,
  isSearching
}) => {
  const [activeField, setActiveField] = useState<"ssn" | "badge" | null>(null);
  const [badgePlaceholder, setBadgePlaceholder] = useState(INPUT_PLACEHOLDERS.BADGE_OR_PSN);
  const currentYear = useFakeTimeAwareYear();

  // Live SSN formatting handler
  const handleSSNChange = useCallback(
    (
      e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
      field: ControllerRenderProps<ForfeituresAdjustmentSearchParams, "ssn">
    ) => {
      const { display, raw } = formatSSNInput(e.target.value);
      e.target.value = display;
      field.onChange(raw === "" ? "" : raw);
      if (raw) setActiveField("ssn");
    },
    []
  );

  // Badge change handler with dynamic placeholder
  const handleBadgeChange = useCallback(
    (value: string, field: ControllerRenderProps<ForfeituresAdjustmentSearchParams, "badge">) => {
      const validatedValue = handleBadgeNumberStringInput(value);
      if (validatedValue !== null) {
        field.onChange(validatedValue);
        if (value) {
          setActiveField("badge");
          setBadgePlaceholder(getBadgeOrPSNPlaceholder(value.length));
        } else {
          setBadgePlaceholder(INPUT_PLACEHOLDERS.BADGE_OR_PSN);
        }
      }
    },
    []
  );

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    watch,
    reset
  } = useForm<ForfeituresAdjustmentSearchParams>({
    resolver: yupResolver(schema) as Resolver<ForfeituresAdjustmentSearchParams>,
    defaultValues: {
      ssn: "",
      badge: ""
    },
    mode: "onChange"
  });

  const socialSecurity = watch("ssn");
  const badgeNumber = watch("badge");

  // Update active field based on which field has input
  useEffect(() => {
    if (socialSecurity && !badgeNumber) {
      setActiveField("ssn");
    } else if (badgeNumber && !socialSecurity) {
      setActiveField("badge");
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

  const validateAndSearch = handleSubmit((data) => {
    if (!isSubmitting) {
      setIsSubmitting(true);
      const searchParams: ForfeitureAdjustmentSearchParams = {
        ssn: data.ssn,
        badge: data.badge,
        profitYear: currentYear,
        skip: 0,
        take: 255,
        sortBy: "badgeNumber",
        isSortDescending: false
      };

      onSearch(searchParams);
    }
  });

  const handleResetLocal = () => {
    reset({
      ssn: "",
      badge: ""
    });
    setActiveField(null);
    onReset();
  };

  const hasSearchCriteria = Boolean(socialSecurity || badgeNumber);

  const requiredLabel = (
    <Typography
      component="span"
      color="error"
      fontWeight="bold">
      *
    </Typography>
  );

  return (
    <form onSubmit={validateAndSearch}>
      <Grid
        container
        paddingX="24px"
        gap="6px">
        <Grid
          container
          spacing={3}
          width="100%">
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel htmlFor={generateFieldId("ssn")}>SSN {requiredLabel}</FormLabel>
            <Controller
              name="ssn"
              control={control}
              render={({ field }) => (
                <>
                  <TextField
                    {...field}
                    id={generateFieldId("ssn")}
                    fullWidth
                    variant="outlined"
                    disabled={activeField === "badge"}
                    placeholder={INPUT_PLACEHOLDERS.SSN}
                    inputProps={{ inputMode: "numeric" }}
                    aria-invalid={!!errors.ssn || !!errors.root?.message || undefined}
                    aria-describedby={getAriaDescribedBy("ssn", !!errors.ssn || !!errors.root?.message, true)}
                    onChange={(e) => {
                      handleSSNChange(e, field as ControllerRenderProps<ForfeituresAdjustmentSearchParams, "ssn">);
                    }}
                  />
                  <VisuallyHidden id="ssn-hint">{ARIA_DESCRIPTIONS.SSN_FORMAT}</VisuallyHidden>
                  {(errors.ssn || errors.root) && (
                    <div
                      id="ssn-error"
                      aria-live="polite"
                      aria-atomic="true">
                      {errors.ssn && <FormHelperText error>{errors.ssn.message}</FormHelperText>}
                      {errors.root && <FormHelperText error>{errors.root.message}</FormHelperText>}
                    </div>
                  )}
                </>
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel htmlFor={generateFieldId("badge")}>Badge {requiredLabel}</FormLabel>
            <Controller
              name="badge"
              control={control}
              render={({ field }) => (
                <>
                  <TextField
                    {...field}
                    id={generateFieldId("badge")}
                    fullWidth
                    variant="outlined"
                    placeholder={badgePlaceholder}
                    disabled={activeField === "ssn"}
                    inputProps={{ inputMode: "numeric" }}
                    aria-invalid={!!errors.badge || !!errors.root?.message || undefined}
                    aria-describedby={getAriaDescribedBy("badge", !!errors.badge || !!errors.root?.message, true)}
                    onChange={(e) => {
                      handleBadgeChange(
                        e.target.value,
                        field as ControllerRenderProps<ForfeituresAdjustmentSearchParams, "badge">
                      );
                    }}
                  />
                  <VisuallyHidden id="badge-hint">{ARIA_DESCRIPTIONS.BADGE_FORMAT}</VisuallyHidden>
                  {errors.badge && (
                    <div
                      id="badge-error"
                      aria-live="polite"
                      aria-atomic="true">
                      <FormHelperText error>{errors.badge.message}</FormHelperText>
                    </div>
                  )}
                </>
              )}
            />
          </Grid>
        </Grid>
      </Grid>
      <Grid
        width="100%"
        paddingX="24px">
        <SearchAndReset
          disabled={!isValid || !hasSearchCriteria || isSearching || isSubmitting}
          handleReset={handleResetLocal}
          handleSearch={validateAndSearch}
          isFetching={isSearching || isSubmitting}
        />
      </Grid>
    </form>
  );
};

export default ForfeituresAdjustmentSearchFilter;
