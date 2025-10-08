import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, TextField, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetForfeitureAdjustmentsQuery } from "reduxstore/api/YearsEndApi";
import {
  clearForfeitureAdjustmentData,
  clearForfeitureAdjustmentQueryParams,
  setForfeitureAdjustmentQueryParams
} from "reduxstore/slices/forfeituresAdjustmentSlice";
import { RootState } from "reduxstore/store";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { FORFEITURES_ADJUSTMENT_MESSAGES } from "../../components/MissiveAlerts/MissiveMessages";
import { useMissiveAlerts } from "../../hooks/useMissiveAlerts";
import {
  badgeNumberStringValidator,
  handleBadgeNumberStringInput,
  handleSsnInput,
  ssnValidator
} from "../../utils/FormValidators";

// Define the search parameters interface
interface ForfeituresAdjustmentSearchParams {
  ssn?: string;
  badge?: string;
}

interface ForfeituresAdjustmentSearchFilterProps {
  setInitialSearchLoaded: (loaded: boolean) => void;
  setPageReset: (reset: boolean) => void;
}

// Define schema for validation without circular references
const schema = yup
  .object({
    ssn: ssnValidator,
    badge: badgeNumberStringValidator
  })
  .test("at-least-one-required", "Either SSN or Badge is required", (values) => Boolean(values.ssn || values.badge));

const ForfeituresAdjustmentSearchFilter: React.FC<ForfeituresAdjustmentSearchFilterProps> = ({
  setInitialSearchLoaded,
  setPageReset
}) => {
  const dispatch = useDispatch();
  const [triggerSearch, { isFetching }] = useLazyGetForfeitureAdjustmentsQuery();
  const { forfeitureAdjustmentQueryParams } = useSelector((state: RootState) => state.forfeituresAdjustment);
  //const profitYear = useFiscalCloseProfitYear();
  const { addAlert, clearAlerts } = useMissiveAlerts();

  const [activeField, setActiveField] = useState<"ssn" | "badge" | null>(null);
  const [oneAddSearchFilterEntered, setOneAddSearchFilterEntered] = useState<boolean>(false);

  const {
    control,
    handleSubmit,
    formState: { errors },
    watch,
    reset
  } = useForm<ForfeituresAdjustmentSearchParams>({
    resolver: yupResolver(schema) as Resolver<ForfeituresAdjustmentSearchParams>,
    defaultValues: {
      ssn: forfeitureAdjustmentQueryParams?.ssn || "",
      badge: forfeitureAdjustmentQueryParams?.badge || ""
    },
    mode: "onBlur"
  });

  let socialSecurityChosen = false;
  let badgeNumberChosen = false;

  const toggleSearchFieldEntered = (value: boolean, fieldType: string) => {
    if (fieldType === "ssn") {
      socialSecurityChosen = value;
    }
    if (fieldType === "badge") {
      badgeNumberChosen = value;
    }

    setOneAddSearchFilterEntered(socialSecurityChosen || badgeNumberChosen);
  };

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

  const validateAndSearch = handleSubmit(async (data) => {
    clearAlerts(); // Clear any existing alerts

    const searchParams = {
      ssn: data.ssn ? Number(data.ssn) : undefined,
      badge: data.badge ? Number(data.badge) : undefined,
      profitYear: new Date().getFullYear(), // Use current wall clock year
      skip: 0,
      take: 255,
      sortBy: "badgeNumber",
      isSortDescending: false,
      onlyNetworkToastErrors: true // Suppress validation errors, only show network errors
    };

    setPageReset(true);
    dispatch(setForfeitureAdjustmentQueryParams(searchParams));
    dispatch(clearForfeitureAdjustmentData());

    const result = await triggerSearch(searchParams);

    // If the response has an error block, handle it
    if (result.error) {
      // Check if it's a 500 error with "Employee not found" message
      if (
        result.error &&
        "status" in result.error &&
        result.error.status === 500 &&
        "data" in result.error &&
        (result.error as any).data?.title === "Employee not found."
      ) {
        addAlert(FORFEITURES_ADJUSTMENT_MESSAGES.EMPLOYEE_NOT_FOUND);
      } else {
        // Handle other errors if needed
        console.error("Forfeitures adjustment employee search error:", result.error);
      }
      return;
    }

    setInitialSearchLoaded(true);
  });

  const handleReset = () => {
    clearAlerts(); // Clear missive alerts when resetting
    setPageReset(true);
    reset({
      ssn: "",
      badge: ""
    });
    dispatch(clearForfeitureAdjustmentData());
    dispatch(clearForfeitureAdjustmentQueryParams());
    setInitialSearchLoaded(false);
    setOneAddSearchFilterEntered(false);
    setActiveField(null);
    socialSecurityChosen = false;
    badgeNumberChosen = false;
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
                      toggleSearchFieldEntered(validatedValue !== "", "ssn");
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
                      toggleSearchFieldEntered(e.target.value !== "", "badge");
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
          disabled={!oneAddSearchFilterEntered}
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isFetching}
        />
      </Grid>
    </form>
  );
};

export default ForfeituresAdjustmentSearchFilter;
