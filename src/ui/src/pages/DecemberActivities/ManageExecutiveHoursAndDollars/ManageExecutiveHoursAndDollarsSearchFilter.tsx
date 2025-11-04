import { yupResolver } from "@hookform/resolvers/yup";
import { Checkbox, FormHelperText, FormLabel, Grid, TextField, Typography } from "@mui/material";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import {
  badgeNumberStringValidator,
  handleBadgeNumberStringInput,
  handleSsnInput,
  profitYearValidator,
  ssnValidator
} from "../../../utils/FormValidators";

interface ExecutiveHoursAndDollarsSearch {
  profitYear: number;
  badgeNumber?: string;
  socialSecurity?: string | undefined;
  fullNameContains?: string | null;
  hasExecutiveHoursAndDollars: NonNullable<boolean>;
  isMonthlyPayroll: NonNullable<boolean>;
}

const validationSchema = yup
  .object({
    profitYear: profitYearValidator(),
    socialSecurity: ssnValidator,
    badgeNumber: badgeNumberStringValidator,
    fullNameContains: yup
      .string()
      .typeError("Full Name must be a string")
      .nullable()
      .transform((value) => value || undefined),
    hasExecutiveHoursAndDollars: yup.boolean().default(false).required(),
    isMonthlyPayroll: yup.boolean().default(false).required()
  })
  .test("at-least-one-required", "At least one field must be provided", (values) =>
    Boolean(
      values.profitYear ||
        values.socialSecurity ||
        values.badgeNumber ||
        values.fullNameContains ||
        values.hasExecutiveHoursAndDollars !== false ||
        values.isMonthlyPayroll !== false
    )
  );

interface SearchData {
  profitYear: number;
  badgeNumber?: number;
  socialSecurity?: string | undefined;
  fullNameContains?: string | null;
  hasExecutiveHoursAndDollars: boolean;
  isMonthlyPayroll: boolean;
}

interface ManageExecutiveHoursAndDollarsSearchFilterProps {
  isModal?: boolean;
  onSearch: (searchData: SearchData) => void;
  onReset: () => void;
  isSearching: boolean;
}

const ManageExecutiveHoursAndDollarsSearchFilter: React.FC<ManageExecutiveHoursAndDollarsSearchFilterProps> = ({
  isModal,
  onSearch,
  onReset,
  isSearching
}) => {
  const [activeField, setActiveField] = useState<"socialSecurity" | "badgeNumber" | "fullNameContains" | null>(null);

  const [fieldStates, setFieldStates] = useState({
    socialSecurity: false,
    badgeNumber: false,
    fullNameContains: false,
    hasExecutiveHoursAndDollars: false,
    isMonthlyPayroll: false
  });

  const oneAddSearchFilterEntered =
    fieldStates.socialSecurity ||
    fieldStates.badgeNumber ||
    fieldStates.fullNameContains ||
    fieldStates.hasExecutiveHoursAndDollars ||
    fieldStates.isMonthlyPayroll;

  const toggleSearchFieldEntered = (value: boolean, fieldType: keyof typeof fieldStates) => {
    setFieldStates((prev) => ({
      ...prev,
      [fieldType]: value
    }));
  };

  const profitYear = useFiscalCloseProfitYear();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    watch,
    trigger: _trigger // need this unused param to prevent console errors. No idea why - EL
  } = useForm<ExecutiveHoursAndDollarsSearch>({
    resolver: yupResolver(validationSchema) as Resolver<ExecutiveHoursAndDollarsSearch>,
    mode: "onBlur",
    defaultValues: {
      profitYear: profitYear,
      badgeNumber: undefined,
      socialSecurity: undefined,
      fullNameContains: "",
      hasExecutiveHoursAndDollars: false,
      isMonthlyPayroll: false
    }
  });

  const socialSecurity = watch("socialSecurity");
  const badgeNumber = watch("badgeNumber");
  const fullNameContains = watch("fullNameContains");

  useEffect(() => {
    if (socialSecurity && !badgeNumber) {
      setActiveField("socialSecurity");
    } else if (badgeNumber && !socialSecurity) {
      setActiveField("badgeNumber");
    } else if (fullNameContains && fullNameContains !== "") {
      setActiveField("fullNameContains");
    } else if (!socialSecurity && !badgeNumber && !fullNameContains) {
      setActiveField(null);
    }
  }, [socialSecurity, badgeNumber, fullNameContains]);

  useEffect(() => {
    if (profitYear) {
      reset((prevValues) => ({
        ...prevValues,
        profitYear
      }));
    }
  }, [profitYear, reset]);

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      const searchData = {
        ...data,
        badgeNumber: data.badgeNumber ? Number(data.badgeNumber) : undefined
      };
      onSearch(searchData);
    }
  });

  const handleReset = () => {
    setActiveField(null);
    setFieldStates({
      socialSecurity: false,
      badgeNumber: false,
      fullNameContains: false,
      hasExecutiveHoursAndDollars: false,
      isMonthlyPayroll: false
    });

    reset({
      profitYear: profitYear,
      badgeNumber: undefined,
      socialSecurity: undefined,
      fullNameContains: "",
      hasExecutiveHoursAndDollars: false,
      isMonthlyPayroll: false
    });

    onReset();
  };

  // The thing is that on the main page search filter, we want the user to be able to leave
  // the name, social security, and badge number fields empty and just search by hasExecutiveHoursAndDollars
  // or isMonthlyPayroll. But on the modal, we want to require at least one of the
  // name, social security, or badge number fields to be filled out as the other fields
  // are not relevant to the modal search, as this is about adding one specific person that is known.
  const requiredLabel = isModal && (
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
        paddingX="24px">
        <Grid
          container
          spacing={3}
          width="100%">
          {!isModal && (
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <FormLabel>Profit Year</FormLabel>
              <Controller
                name="profitYear"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    variant="outlined"
                    error={!!errors.profitYear}
                    onChange={(e) => {
                      field.onChange(e);
                    }}
                    type="number"
                    disabled={true}
                  />
                )}
              />
              {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
            </Grid>
          )}
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Full Name {requiredLabel}</FormLabel>
            <Controller
              name="fullNameContains"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  variant="outlined"
                  disabled={activeField === "socialSecurity" || activeField === "badgeNumber"}
                  error={!!errors.fullNameContains}
                  onChange={(e) => {
                    field.onChange(e);
                    if (e.target.value !== "") {
                      toggleSearchFieldEntered(true, "fullNameContains");
                    } else {
                      toggleSearchFieldEntered(false, "fullNameContains");
                    }
                  }}
                />
              )}
            />
            {errors.fullNameContains && <FormHelperText error>{errors.fullNameContains.message}</FormHelperText>}
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>SSN {requiredLabel}</FormLabel>
            <Controller
              name="socialSecurity"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  disabled={activeField === "badgeNumber" || activeField === "fullNameContains"}
                  value={field.value ?? ""}
                  error={!!errors.socialSecurity}
                  onChange={(e) => {
                    const validatedValue = handleSsnInput(e.target.value);
                    if (validatedValue !== null) {
                      const parsedValue = validatedValue === "" ? null : validatedValue;
                      field.onChange(parsedValue);
                      toggleSearchFieldEntered(validatedValue !== "", "socialSecurity");
                    }
                  }}
                />
              )}
            />
            {errors.socialSecurity && <FormHelperText error>{errors.socialSecurity.message}</FormHelperText>}
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Badge Number {requiredLabel}</FormLabel>
            <Controller
              name="badgeNumber"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.badgeNumber}
                  disabled={activeField === "socialSecurity" || activeField === "fullNameContains"}
                  onChange={(e) => {
                    const validatedValue = handleBadgeNumberStringInput(e.target.value);
                    if (validatedValue !== null) {
                      field.onChange(validatedValue);
                      toggleSearchFieldEntered(validatedValue !== "", "badgeNumber");
                    }
                  }}
                />
              )}
            />
            {errors.badgeNumber && <FormHelperText error>{errors.badgeNumber.message}</FormHelperText>}
          </Grid>
          {!isModal && (
            <>
              <Grid
                container
                paddingX="8px"
                width={"100%"}>
                <Grid size={{ xs: 3, sm: 3, md: 3 }}>
                  <FormLabel>Has Executive Hours and Dollars</FormLabel>
                  <Controller
                    name="hasExecutiveHoursAndDollars"
                    control={control}
                    render={({ field }) => (
                      <Checkbox
                        checked={field.value}
                        onChange={(e) => {
                          field.onChange(e);
                          toggleSearchFieldEntered(e.target.checked, "hasExecutiveHoursAndDollars");
                        }}
                      />
                    )}
                  />
                  {errors.hasExecutiveHoursAndDollars && (
                    <FormHelperText error>{errors.hasExecutiveHoursAndDollars.message}</FormHelperText>
                  )}
                </Grid>
                <Grid size={{ xs: 3, sm: 3, md: 3 }}>
                  <FormLabel>Monthly Payroll</FormLabel>
                  <Controller
                    name="isMonthlyPayroll"
                    control={control}
                    render={({ field }) => (
                      <Checkbox
                        checked={field.value}
                        onChange={(e) => {
                          field.onChange(e);
                          toggleSearchFieldEntered(e.target.checked, "isMonthlyPayroll");
                        }}
                      />
                    )}
                  />
                  {errors.isMonthlyPayroll && <FormHelperText error>{errors.isMonthlyPayroll.message}</FormHelperText>}
                </Grid>
              </Grid>
            </>
          )}
        </Grid>
      </Grid>
      <Grid
        width="100%"
        paddingX="24px">
        <SearchAndReset
          disabled={!oneAddSearchFilterEntered}
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isSearching}
        />
      </Grid>
    </form>
  );
};

export default ManageExecutiveHoursAndDollarsSearchFilter;
