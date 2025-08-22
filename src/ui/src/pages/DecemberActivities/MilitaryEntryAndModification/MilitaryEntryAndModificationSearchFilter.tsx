import { yupResolver } from "@hookform/resolvers/yup";
import { FormLabel, TextField, Typography } from "@mui/material";
import { Grid } from "@mui/material";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { useEffect, useState } from "react";
import { useForm, Controller, Resolver } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import useMilitaryEntryAndModification from "./hooks/useMilitaryEntryAndModification";

interface SearchFormData {
  socialSecurity?: string;
  badgeNumber?: string;
}

// Define schema with proper typing for our form
const validationSchema = yup
  .object({
    socialSecurity: yup
      .string()
      .nullable()
      .transform((value) => value || undefined),
    badgeNumber: yup
      .string()
      .nullable()
      .transform((value) => value || undefined)
  })
  .test("at-least-one-required", "At least one field must be provided", (values) =>
    Boolean(values.socialSecurity || values.badgeNumber)
  );

const MilitaryEntryAndModificationSearchFilter: React.FC = () => {
  const [activeField, setActiveField] = useState<"socialSecurity" | "badgeNumber" | null>(null);
  const defaultProfitYear = useDecemberFlowProfitYear();
  const { isSearching, executeSearch, resetSearch } = useMilitaryEntryAndModification();

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

  const onSubmit = async (data: SearchFormData) => {
    await executeSearch(data, defaultProfitYear);
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
          <FormLabel>SSN {requiredLabel}</FormLabel>
          <Controller
            name="socialSecurity"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                value={field.value ?? ""}
                fullWidth
                variant="outlined"
                placeholder="Enter SSN"
                disabled={activeField === "badgeNumber"}
                error={!!errors.socialSecurity}
                helperText={errors.socialSecurity?.message}
                onChange={(e) => {
                  field.onChange(e);
                  if (e.target.value) setActiveField("socialSecurity");
                }}
              />
            )}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>Badge Number {requiredLabel}</FormLabel>
          <Controller
            name="badgeNumber"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                value={field.value ?? ""}
                fullWidth
                variant="outlined"
                placeholder="Enter Badge Number"
                disabled={activeField === "socialSecurity"}
                error={!!errors.badgeNumber}
                helperText={errors.badgeNumber?.message}
                onChange={(e) => {
                  field.onChange(e);
                  if (e.target.value) setActiveField("badgeNumber");
                }}
              />
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
          isFetching={isSearching}
          disabled={!isValid || (!socialSecurity && !badgeNumber)}
        />
      </Grid>
    </form>
  );
};

export default MilitaryEntryAndModificationSearchFilter;
