import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, TextField, Typography } from "@mui/material";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
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

// Define the search parameters interface
interface ForfeituresAdjustmentSearchParams {
  ssn?: string;
  badge?: string;
  year?: number;
}

interface ForfeituresAdjustmentSearchParametersProps {
  setInitialSearchLoaded: (loaded: boolean) => void;
  setPageReset: (reset: boolean) => void;
}

// Define schema for validation without circular references
const schema = yup
  .object({
    ssn: yup
      .number()
      .typeError("SSN must be a number")
      .integer("SSN must be an integer")
      .min(0, "SSN must be positive")
      .max(999999999, "SSN must be 9 digits or less")
      .transform((value) => value || undefined),
    badge: yup
      .number()
      .typeError("Badge Number must be a number")
      .integer("Badge Number must be an integer")
      .min(0, "Badge must be positive")
      .max(9999999, "Badge must be 7 digits or less")
      .transform((value) => value || undefined),
    year: yup
      .number()
      .typeError("Year must be a number")
      .integer("Year must be an integer")
      .min(2020, "Year must be 2020 or later")
      .max(2100, "Year must be 2100 or earlier")
      .optional()
  })
  .test("at-least-one-required", "Either SSN or Badge is required", (values) => Boolean(values.ssn || values.badge));

const ForfeituresAdjustmentSearchParameters: React.FC<ForfeituresAdjustmentSearchParametersProps> = ({
  setInitialSearchLoaded,
  setPageReset
}) => {
  const dispatch = useDispatch();
  const [triggerSearch, { isFetching }] = useLazyGetForfeitureAdjustmentsQuery();
  const { forfeitureAdjustmentQueryParams } = useSelector((state: RootState) => state.forfeituresAdjustment);
  const profitYear = useFiscalCloseProfitYear();
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
      badge: forfeitureAdjustmentQueryParams?.badge || "",
      year: forfeitureAdjustmentQueryParams?.profitYear || profitYear
    },
    mode: "onSubmit"
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
      ssn: data.ssn,
      badge: data.badge,
      profitYear: data.year || profitYear,
      skip: 0,
      take: 255,
      sortBy: "badgeNumber",
      isSortDescending: false,
      onlyNetworkToastErrors: true // Suppress validation errors, only show network errors
    };

    setPageReset(true);
    dispatch(setForfeitureAdjustmentQueryParams(searchParams));

    const result = await triggerSearch(searchParams);

    console.log("Search result:", result);

    // If the response has an error block, handle it
    if (result.error) {
      console.log("Error detected:", result.error);
      console.log("Error status:", result.error?.status);
      console.log("Error data:", result.error?.data);
      console.log("Error title:", result.error?.data?.title);

      // Check if it's a 500 error with "Employee not found" message
      if (result.error?.status === 500 && result.error?.data?.title === "Employee not found.") {
        console.log("Triggering Employee not found alert");
        addAlert(FORFEITURES_ADJUSTMENT_MESSAGES.EMPLOYEE_NOT_FOUND);
      } else {
        // Handle other errors if needed
        console.error("Search error:", result.error);
      }
      return;
    }

    console.log("Search successful, setting initial search loaded");

    setInitialSearchLoaded(true);
  });

  const handleReset = () => {
    setPageReset(true);
    reset({
      ssn: "",
      badge: "",
      year: profitYear
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
                    if (!isNaN(Number(e.target.value))) {
                      const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                      field.onChange(parsedValue);
                      if (e.target.value !== "") {
                        toggleSearchFieldEntered(true, "ssn");
                      } else {
                        toggleSearchFieldEntered(false, "ssn");
                      }
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
                    if (!isNaN(Number(e.target.value))) {
                      const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                      field.onChange(parsedValue);
                      if (e.target.value !== "") {
                        toggleSearchFieldEntered(true, "badge");
                      } else {
                        toggleSearchFieldEntered(false, "badge");
                      }
                    }
                  }}
                />
              )}
            />
            {errors.badge && <FormHelperText error>{errors.badge.message}</FormHelperText>}
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <Controller
              name="year"
              control={control}
              render={({ field }) => (
                <DsmDatePicker
                  id="profitYear"
                  onChange={(value: Date | null) => field.onChange(value?.getFullYear() || undefined)}
                  value={field.value ? new Date(field.value, 0) : null}
                  required={true}
                  label="Year"
                  disableFuture
                  views={["year"]}
                  error={errors.year?.message}
                  disabled={true}
                />
              )}
            />
            {errors.year && <FormHelperText error>{errors.year.message}</FormHelperText>}
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

export default ForfeituresAdjustmentSearchParameters;
