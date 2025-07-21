import { yupResolver } from "@hookform/resolvers/yup";
import { Checkbox, FormHelperText, FormLabel, TextField, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import {
  useLazyGetAdditionalExecutivesQuery,
  useLazyGetExecutiveHoursAndDollarsQuery
} from "reduxstore/api/YearsEndApi";
import {
  clearAdditionalExecutivesChosen,
  clearAdditionalExecutivesGrid,
  clearExecutiveHoursAndDollars,
  clearExecutiveHoursAndDollarsAddQueryParams,
  setExecutiveHoursAndDollarsAddQueryParams,
  setExecutiveHoursAndDollarsGridYear,
  setExecutiveHoursAndDollarsQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { ISortParams, SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

interface ExecutiveHoursAndDollarsSearch {
  profitYear: number;
  badgeNumber?: number | null | undefined;
  socialSecurity?: number | undefined;
  fullNameContains?: string | null;
  hasExecutiveHoursAndDollars: NonNullable<boolean>;
  isMonthlyPayroll: NonNullable<boolean>;
}

const validationSchema = yup
  .object({
    profitYear: yup.number().required("Profit Year is required").typeError("Profit Year must be a number"),
    socialSecurity: yup
      .number()
      .typeError("SSN must be a number")
      .integer("SSN must be an integer")
      .min(0, "SSN must be positive")
      .max(999999999, "SSN must be 9 digits or less")
      .transform((value) => value || undefined),
    badgeNumber: yup
      .number()
      .typeError("Badge Number must be a number")
      .integer("Badge Number must be an integer")
      .min(0, "Badge must be positive")
      .max(9999999, "Badge must be 7 digits or less")
      .transform((value) => value || undefined),
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
      values.socialSecurity ||
        values.badgeNumber ||
        values.fullNameContains ||
        values.hasExecutiveHoursAndDollars !== false ||
        values.isMonthlyPayroll !== false
    )
  );

// If we are using a modal window, we want a slimmed down version of the search filter
// and we will
interface ManageExecutiveHoursAndDollarsSearchFilterProps {
  isModal?: boolean;
  setInitialSearchLoaded: (include: boolean) => void;
  setPageNumberReset: (reset: boolean) => void;
}

const ManageExecutiveHoursAndDollarsSearchFilter: React.FC<ManageExecutiveHoursAndDollarsSearchFilterProps> = ({
  isModal,
  setInitialSearchLoaded,
  setPageNumberReset
}) => {
  const dispatch = useDispatch();
  const [activeField, setActiveField] = useState<"socialSecurity" | "badgeNumber" | "fullNameContains" | null>(null);

  useEffect(() => {
    dispatch(clearExecutiveHoursAndDollarsAddQueryParams());
  }, [dispatch]);

  const { executiveHoursAndDollarsQueryParams, executiveHoursAndDollars } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const [oneAddSearchFilterEntered, setOneAddSearchFilterEntered] = useState<boolean>(false);

  let socialSecurityChosen = false;
  let badgeNumberChosen = false;
  let fullNameChosen = false;
  let hasExecutiveHoursAndDollarsChosen = false;
  let isMonthlyPayrollChosen = false;

  const toggleSearchFieldEntered = (value: boolean, fieldType: string) => {
    if (fieldType === "socialSecurity") {
      socialSecurityChosen = value;
    }
    if (fieldType === "badgeNumber") {
      badgeNumberChosen = value;
    }
    if (fieldType === "fullNameContains") {
      fullNameChosen = value;
    }
    if (fieldType === "hasExecutiveHoursAndDollars") {
      hasExecutiveHoursAndDollarsChosen = value;
    }
    if (fieldType === "isMonthlyPayroll") {
      isMonthlyPayrollChosen = value;
    }

    setOneAddSearchFilterEntered(
      socialSecurityChosen ||
        badgeNumberChosen ||
        fullNameChosen ||
        hasExecutiveHoursAndDollarsChosen ||
        isMonthlyPayrollChosen
    );
  };

  const profitYear = useFiscalCloseProfitYear();

  const [triggerSearch, { isFetching }] = useLazyGetExecutiveHoursAndDollarsQuery();
  const [triggerModalSearch, { isFetching: isModalFetching }] = useLazyGetAdditionalExecutivesQuery();

  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "storeNumber",
    isSortDescending: false
  });

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    watch,
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    trigger // need this unused param to prevent console errors. No idea why - EL
  } = useForm<ExecutiveHoursAndDollarsSearch>({
    resolver: yupResolver(validationSchema) as Resolver<ExecutiveHoursAndDollarsSearch>,
    mode: "onChange",
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

  // Update active field based on which field has input
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
      dispatch(clearExecutiveHoursAndDollars());
      dispatch(clearAdditionalExecutivesChosen());

      reset((prevValues) => ({
        ...prevValues,
        profitYear
      }));

      if (executiveHoursAndDollarsQueryParams) {
        dispatch(
          setExecutiveHoursAndDollarsQueryParams({
            ...executiveHoursAndDollarsQueryParams,
            profitYear
          })
        );
      }

      setInitialSearchLoaded(false);
    }
  }, [profitYear]);

  const validateAndSearch = handleSubmit((data) => {
    // If there are any stored additional executives, we
    // should delete them, regardless of modal or not
    //dispatch(clearAdditionalExecutivesChosen());

    if (isValid && !isModal) {
      triggerSearch(
        {
          pagination: { skip: 0, take: 25, sortBy: sortParams.sortBy, isSortDescending: sortParams.isSortDescending },
          profitYear: data.profitYear ?? (profitYear || 0),
          ...(!!data.socialSecurity && { socialSecurity: data.socialSecurity }),
          ...(!!data.badgeNumber && { badgeNumber: data.badgeNumber }),
          hasExecutiveHoursAndDollars: data.hasExecutiveHoursAndDollars ?? false,
          isMonthlyPayroll: data.isMonthlyPayroll !== undefined ? data.isMonthlyPayroll : false,
          ...(!!data.fullNameContains && { fullNameContains: data.fullNameContains })
        },
        false
      ).unwrap();
      dispatch(
        setExecutiveHoursAndDollarsQueryParams({
          profitYear: profitYear || 0,
          badgeNumber: data.badgeNumber ?? 0,
          socialSecurity: data.socialSecurity ?? 0,
          fullNameContains: data.fullNameContains ?? "",
          hasExecutiveHoursAndDollars: data.hasExecutiveHoursAndDollars ?? false,
          isMonthlyPayroll: data.isMonthlyPayroll ?? false
        })
      );

      dispatch(setExecutiveHoursAndDollarsGridYear(profitYear));
      dispatch(clearAdditionalExecutivesChosen());
    }

    // A difference in modal is that we are not filtering for having executive hours
    // and dollars being there
    if (isValid && isModal) {
      triggerModalSearch(
        {
          pagination: { skip: 0, take: 25, sortBy: sortParams.sortBy, isSortDescending: sortParams.isSortDescending },
          profitYear: data.profitYear || 0,
          ...(!!data.socialSecurity && { socialSecurity: data.socialSecurity }),
          ...(!!data.badgeNumber && { badgeNumber: data.badgeNumber }),
          hasExecutiveHoursAndDollars: false,
          isMonthlyPayroll: data.isMonthlyPayroll ?? false,
          ...(!!data.fullNameContains && { fullNameContains: data.fullNameContains })
        },
        false
      ).unwrap();
      dispatch(
        setExecutiveHoursAndDollarsAddQueryParams({
          profitYear: profitYear || 0,
          badgeNumber: data.badgeNumber ?? 0,
          socialSecurity: data.socialSecurity ?? 0,
          fullNameContains: data.fullNameContains ?? "",
          hasExecutiveHoursAndDollars: data.hasExecutiveHoursAndDollars ?? false,
          isMonthlyPayroll: data.isMonthlyPayroll ?? false
        })
      );
    }

    setPageNumberReset(true);
  });

  //if (profitYear && !executiveHoursAndDollars) {
  //  setInitialSearchLoaded(true);
  //}

  const handleReset = async () => {
    // If we ever decide that the reset button should clear pending changes
    // uncomment this next line...
    // dispatch(clearExecutiveHoursAndDollarsGridRows());
    // ... and then import clearExecutiveHoursAndDollarsGridRows
    // from reduxstore/slices/yearsEndSlice

    // Are we in modal

    if (!isModal) {
      // If we are in modal, we want to clear the additional executives
      // and reset the query params
      setInitialSearchLoaded(true);
      
      dispatch(clearExecutiveHoursAndDollars());
      // Need a delay so that the grid sees the empty results set and clears
      await new Promise((resolve) => setTimeout(resolve, 250));
    } else {
      dispatch(clearAdditionalExecutivesGrid());
      setOneAddSearchFilterEntered(false);
    }
    dispatch(clearExecutiveHoursAndDollarsAddQueryParams());
    dispatch(clearAdditionalExecutivesChosen());
    setOneAddSearchFilterEntered(false);
    setActiveField(null);
    socialSecurityChosen = false;
    badgeNumberChosen = false;
    fullNameChosen = false;
    isMonthlyPayrollChosen = false;
    hasExecutiveHoursAndDollarsChosen = false;
    setPageNumberReset(true);

    reset({
      profitYear: profitYear,
      badgeNumber: undefined,
      socialSecurity: undefined,
      fullNameContains: "",
      hasExecutiveHoursAndDollars: false,
      isMonthlyPayroll: false
    });
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
      <Grid2
        container
        paddingX="24px">
        <Grid2
          container
          spacing={3}
          width="100%">
          {!isModal && (
            <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
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
            </Grid2>
          )}
          <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
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
          </Grid2>
          <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
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
                    if (!isNaN(Number(e.target.value))) {
                      const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                      field.onChange(parsedValue);
                      if (e.target.value !== "") {
                        toggleSearchFieldEntered(true, "socialSecurity");
                      } else {
                        toggleSearchFieldEntered(false, "socialSecurity");
                      }
                    }
                  }}
                />
              )}
            />
            {errors.socialSecurity && <FormHelperText error>{errors.socialSecurity.message}</FormHelperText>}
          </Grid2>
          <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
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
                    if (!isNaN(Number(e.target.value))) {
                      const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                      field.onChange(parsedValue);
                      if (e.target.value !== "") {
                        toggleSearchFieldEntered(true, "badgeNumber");
                      } else {
                        toggleSearchFieldEntered(false, "badgeNumber");
                      }
                    }
                  }}
                  type="number"
                />
              )}
            />
            {errors.badgeNumber && <FormHelperText error>{errors.badgeNumber.message}</FormHelperText>}
          </Grid2>
          {!isModal && (
            <>
              <Grid2
                container
                paddingX="8px"
                width={"100%"}>
                <Grid2 size={{ xs: 3, sm: 3, md: 3 }}>
                  <FormLabel>Has Executive Hours and Dollars</FormLabel>
                  <Controller
                    name="hasExecutiveHoursAndDollars"
                    control={control}
                    render={({ field }) => (
                      <Checkbox
                        checked={field.value}
                        onChange={(e) => {
                          console.log("hasExecutiveHoursAndDollars", e.target.checked);
                          field.onChange(e);
                          toggleSearchFieldEntered(e.target.checked, "hasExecutiveHoursAndDollars");
                        }}
                      />
                    )}
                  />
                  {errors.hasExecutiveHoursAndDollars && (
                    <FormHelperText error>{errors.hasExecutiveHoursAndDollars.message}</FormHelperText>
                  )}
                </Grid2>
                <Grid2 size={{ xs: 3, sm: 3, md: 3 }}>
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
                </Grid2>
              </Grid2>
            </>
          )}
        </Grid2>
      </Grid2>
      <Grid2
        width="100%"
        paddingX="24px">
        {!isModal && (
          <SearchAndReset
            disabled={!oneAddSearchFilterEntered}
            handleReset={handleReset}
            handleSearch={validateAndSearch}
            isFetching={isFetching}
          />
        )}
        {isModal && (
          <SearchAndReset
            disabled={!oneAddSearchFilterEntered}
            handleReset={handleReset}
            handleSearch={validateAndSearch}
            isFetching={isModalFetching}
          />
        )}
      </Grid2>
    </form>
  );
};

export default ManageExecutiveHoursAndDollarsSearchFilter;
