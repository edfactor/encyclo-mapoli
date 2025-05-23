import { yupResolver } from "@hookform/resolvers/yup";
import { FormLabel, TextField, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useEffect, useState } from "react";
import { useForm, Controller, Resolver } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useLazyGetProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { clearMasterInquiryData } from "reduxstore/slices/inquirySlice";
import { clearMilitaryContributions } from "reduxstore/slices/militarySlice";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

interface SearchFormData {
  socialSecurity?: string;
  badgeNumber?: string;
}

interface SearchFilterProps {
  setInitialSearchLoaded: (loaded: boolean) => void;
}

// Define schema with proper typing for our form
const validationSchema = yup.object({
  socialSecurity: yup.string().nullable().transform((value) => value || undefined),
  badgeNumber: yup.string().nullable().transform((value) => value || undefined)
}).test(
  "at-least-one-required",
  "At least one field must be provided",
  (values) => Boolean(values.socialSecurity || values.badgeNumber)
);

const MilitaryEntryAndModificationSearchFilter: React.FC<SearchFilterProps> = ({ setInitialSearchLoaded }) => {
  const [triggerSearch, { isFetching }] = useLazyGetProfitMasterInquiryQuery();
  const [activeField, setActiveField] = useState<"socialSecurity" | "badgeNumber" | null>(null);
  
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

  const dispatch = useDispatch();
  
  const onSubmit = (data: SearchFormData) => {
    triggerSearch(
      {
        pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false },
        ...(!!data.socialSecurity && { socialSecurity: Number(data.socialSecurity) }),
        ...(!!data.badgeNumber && { badgeNumber: Number(data.badgeNumber) })
      },
      false
    ).then(() => {
      setInitialSearchLoaded(true); // Set to true after successful search
    });
  };

  const handleReset = () => {
    reset();
    setActiveField(null);
    dispatch(clearMasterInquiryData());
    dispatch(clearMilitaryContributions());
  };

  const requiredLabel = (
    <Typography component="span" color="error" fontWeight="bold">
      *
    </Typography>
  );

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Grid2
        container
        paddingX="24px"
        gap="24px">
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>
            SSN {requiredLabel}
          </FormLabel>
          <Controller
            name="socialSecurity"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
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
        </Grid2>
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>
            Badge Number {requiredLabel}
          </FormLabel>
          <Controller
            name="badgeNumber"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
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
        </Grid2>
      </Grid2>
      <Grid2
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={handleSubmit(onSubmit)}
          isFetching={isFetching}
          disabled={!isValid || (!socialSecurity && !badgeNumber)}
        />
      </Grid2>
    </form>
  );
};

export default MilitaryEntryAndModificationSearchFilter;
