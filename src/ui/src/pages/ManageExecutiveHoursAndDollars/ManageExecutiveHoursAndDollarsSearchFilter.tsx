import { Checkbox, FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useForm, Controller } from "react-hook-form";
import {
  useLazyGetExecutiveHoursAndDollarsQuery,
  useLazyGetAdditionalExecutivesQuery
} from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import { useDispatch, useSelector } from "react-redux";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { RootState } from "reduxstore/store";
import {
  clearAdditionalExecutivesChosen,
  clearEligibleEmployeesQueryParams,
  clearExecutiveHoursAndDollars,
  setExecutiveHoursAndDollarsGridYear,
  setExecutiveHoursAndDollarsQueryParams
} from "reduxstore/slices/yearsEndSlice";

interface ExecutiveHoursAndDollarsSearch {
  profitYear: number;
  badgeNumber?: number | null;
  socialSecurity?: number | null;
  fullNameContains?: string | null;
  hasExecutiveHoursAndDollars: NonNullable<boolean>;
  hasMonthlyPayments: NonNullable<boolean>;
}

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .required("Year is required"),
  badgeNumber: yup
    .number()
    .typeError("Badge Number must be a number")
    .integer("Badge Number must be an integer")
    .min(0, "Badge must be positive")
    .max(9999999, "Badge must be 7 digits or less")
    .nullable(),
  socialSecurity: yup
    .number()
    .typeError("SSN must be a number")
    .integer("SSN must be an integer")
    .min(0, "SSN must be positive")
    .max(999999999, "SSN must be 9 digits or less")
    .nullable(),
  fullNameContains: yup.string().typeError("Full Name must be a string").nullable(),
  hasExecutiveHoursAndDollars: yup.boolean().default(true).required(),
  hasMonthlyPayments: yup.boolean().default(false).required()
});

// If we are using a modal window, we want a slimmed down version of the search filter
// and we will
interface ManageExecutiveHoursAndDollarsSearchFilterProps {
  isModal?: boolean;
  setInitialSearchLoaded: (include: boolean) => void;
}

const ManageExecutiveHoursAndDollarsSearchFilter: React.FC<ManageExecutiveHoursAndDollarsSearchFilterProps> = ({
  isModal,
  setInitialSearchLoaded
}) => {
  const { executiveHoursAndDollarsQueryParams } = useSelector((state: RootState) => state.yearsEnd);

  const [triggerSearch, { isFetching }] = useLazyGetExecutiveHoursAndDollarsQuery();
  const [triggerModalSearch, { isFetching: isModalFetching }] = useLazyGetAdditionalExecutivesQuery();

  const dispatch = useDispatch();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    trigger // need this unused param to prevent console errors. No idea why - EL
  } = useForm<ExecutiveHoursAndDollarsSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: executiveHoursAndDollarsQueryParams?.profitYear ?? undefined,
      badgeNumber: executiveHoursAndDollarsQueryParams?.badgeNumber ?? undefined,
      socialSecurity: executiveHoursAndDollarsQueryParams?.socialSecurity ?? undefined,
      fullNameContains: executiveHoursAndDollarsQueryParams?.fullNameContains ?? undefined,
      hasExecutiveHoursAndDollars: executiveHoursAndDollarsQueryParams?.hasExecutiveHoursAndDollars ?? true,
      hasMonthlyPayments: executiveHoursAndDollarsQueryParams?.hasMonthlyPayments ?? false
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    // If there are any stored additional executives, we
    // should delete them, regardless of modal or not
    //dispatch(clearAdditionalExecutivesChosen());

    if (isValid && !isModal) {
      triggerSearch(
        {
          pagination: { skip: 0, take: 25 },
          profitYear: data.profitYear,
          ...(!!data.socialSecurity && { socialSecurity: data.socialSecurity }),
          ...(!!data.badgeNumber && { badgeNumber: data.badgeNumber }),
          hasExecutiveHoursAndDollars: data.hasExecutiveHoursAndDollars ?? false,
          hasMonthlyPayments: data.hasMonthlyPayments !== undefined ? data.hasMonthlyPayments : false,
          ...(!!data.fullNameContains && { fullNameContains: data.fullNameContains })
        },
        false
      ).unwrap();
      dispatch(
        setExecutiveHoursAndDollarsQueryParams({
          ...data,
          badgeNumber: data.badgeNumber ?? 0,
          socialSecurity: data.socialSecurity ?? 0,
          fullNameContains: data.fullNameContains ?? "",
          hasExecutiveHoursAndDollars: data.hasExecutiveHoursAndDollars ?? false,
          hasMonthlyPayments: data.hasMonthlyPayments ?? false
        })
      );

      dispatch(setExecutiveHoursAndDollarsGridYear(data.profitYear));

      dispatch(clearAdditionalExecutivesChosen());
    }

    // A difference in modal is that we are not filtering for having executive hours
    // and dollars being there
    if (isValid && isModal) {
      triggerModalSearch(
        {
          pagination: { skip: 0, take: 25 },
          profitYear: data.profitYear,
          ...(!!data.socialSecurity && { socialSecurity: data.socialSecurity }),
          ...(!!data.badgeNumber && { badgeNumber: data.badgeNumber }),
          hasExecutiveHoursAndDollars: false,
          hasMonthlyPayments: data.hasMonthlyPayments ?? false,
          ...(!!data.fullNameContains && { fullNameContains: data.fullNameContains })
        },
        false
      );
    }
  });

  const handleReset = () => {
    // If we ever decide that the reset button should clear pending changes
    // uncomment this next line...
    // dispatch(clearExecutiveHoursAndDollarsGridRows());
    // ... and then import clearExecutiveHoursAndDollarsGridRows
    // from reduxstore/slices/yearsEndSlice
    setInitialSearchLoaded(false);
    dispatch(clearExecutiveHoursAndDollars());
    dispatch(clearEligibleEmployeesQueryParams());
    reset({
      profitYear: undefined
    });
  };

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
            <Grid2
              xs={12}
              sm={6}
              md={3}>
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
                    inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
                  />
                )}
              />
              {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
            </Grid2>
          )}
          <Grid2
            xs={12}
            sm={6}
            md={3}>
            <FormLabel>Full Name</FormLabel>
            <Controller
              name="fullNameContains"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  variant="outlined"
                  error={!!errors.fullNameContains}
                  onChange={(e) => {
                    field.onChange(e);
                  }}
                />
              )}
            />
            {errors.fullNameContains && <FormHelperText error>{errors.fullNameContains.message}</FormHelperText>}
          </Grid2>
          <Grid2
            xs={12}
            sm={6}
            md={3}>
            <FormLabel>SSN</FormLabel>
            <Controller
              name="socialSecurity"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.socialSecurity}
                  onChange={(e) => {
                    if (!isNaN(Number(e.target.value))) {
                      const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                      field.onChange(parsedValue);
                    }
                  }}
                />
              )}
            />
            {errors.socialSecurity && <FormHelperText error>{errors.socialSecurity.message}</FormHelperText>}
          </Grid2>
          <Grid2
            xs={12}
            sm={6}
            md={3}>
            <FormLabel>Badge Number</FormLabel>
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
                  onChange={(e) => {
                    if (!isNaN(Number(e.target.value))) {
                      const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                      field.onChange(parsedValue);
                    }
                  }}
                  //inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
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
                <Grid2
                  xs={3}
                  sm={3}
                  md={3}>
                  <FormLabel>Has Executive Hours and Dollars</FormLabel>
                  <Controller
                    name="hasExecutiveHoursAndDollars"
                    control={control}
                    render={({ field }) => (
                      <Checkbox
                        checked={field.value}
                        onChange={(e) => {
                          field.onChange(e);
                        }}
                      />
                    )}
                  />
                  {errors.hasExecutiveHoursAndDollars && (
                    <FormHelperText error>{errors.hasExecutiveHoursAndDollars.message}</FormHelperText>
                  )}
                </Grid2>
                <Grid2
                  xs={3}
                  sm={3}
                  md={3}>
                  <FormLabel>Has Monthly Payments</FormLabel>
                  <Controller
                    name="hasMonthlyPayments"
                    control={control}
                    render={({ field }) => (
                      <Checkbox
                        checked={field.value}
                        onChange={(e) => {
                          field.onChange(e);
                        }}
                      />
                    )}
                  />
                  {errors.hasMonthlyPayments && (
                    <FormHelperText error>{errors.hasMonthlyPayments.message}</FormHelperText>
                  )}
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
            handleReset={handleReset}
            handleSearch={validateAndSearch}
            isFetching={isFetching}
          />
        )}
        {isModal && (
          <SearchAndReset
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
