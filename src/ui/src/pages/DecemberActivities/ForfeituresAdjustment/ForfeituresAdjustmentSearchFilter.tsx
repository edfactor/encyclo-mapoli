import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, TextField, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { useFakeTimeAwareYear } from "../../../hooks/useFakeTimeAwareDate";
import {
    badgeNumberStringValidator,
    handleBadgeNumberStringInput,
    handleSsnInput,
    ssnValidator
} from "../../../utils/FormValidators";

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
  const currentYear = useFakeTimeAwareYear();

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
            <FormLabel>SSN {requiredLabel}</FormLabel>
            <Controller
              name="ssn"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  variant="outlined"
                  disabled={activeField === "badge"}
                  error={!!errors.ssn || !!errors.root?.message}
                  placeholder="SSN"
                  onChange={(e) => {
                    const validatedValue = handleSsnInput(e.target.value);
                    if (validatedValue !== null) {
                      field.onChange(validatedValue);
                      if (validatedValue) setActiveField("ssn");
                    }
                  }}
                />
              )}
            />
            {errors.ssn && <FormHelperText error>{errors.ssn.message}</FormHelperText>}
            {errors.root && <FormHelperText error>{errors.root.message}</FormHelperText>}
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel>Badge {requiredLabel}</FormLabel>
            <Controller
              name="badge"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  variant="outlined"
                  error={!!errors.badge || !!errors.root?.message}
                  placeholder="Badge"
                  disabled={activeField === "ssn"}
                  onChange={(e) => {
                    const validatedValue = handleBadgeNumberStringInput(e.target.value);
                    if (validatedValue !== null) {
                      field.onChange(validatedValue);
                      if (e.target.value) setActiveField("badge");
                    }
                  }}
                />
              )}
            />
            {errors.badge && <FormHelperText error>{errors.badge.message}</FormHelperText>}
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
