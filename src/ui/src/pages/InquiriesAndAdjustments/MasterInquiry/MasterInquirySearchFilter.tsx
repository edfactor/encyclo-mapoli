import { yupResolver } from "@hookform/resolvers/yup";
import {
  Checkbox,
  FormControl,
  FormControlLabel,
  FormHelperText,
  FormLabel,
  Grid,
  Radio,
  RadioGroup,
  TextField
} from "@mui/material";
import React, { memo, useCallback, useEffect, useMemo, useRef } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate, useParams } from "react-router-dom";
import {
  clearMasterInquiryData,
  clearMasterInquiryGroupingData,
  clearMasterInquiryRequestParams,
  setMasterInquiryRequestParams
} from "reduxstore/slices/inquirySlice";
import { RootState } from "reduxstore/store";
import { MasterInquiryRequest, MasterInquirySearch } from "reduxstore/types";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { MAX_EMPLOYEE_BADGE_LENGTH, ROUTES } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import {
  badgeNumberOrPSNValidator,
  monthValidator,
  profitYearNullableValidator,
  ssnValidator
} from "../../../utils/FormValidators";
import { transformSearchParams } from "./utils/transformSearchParams";

const schema: yup.ObjectSchema<MasterInquirySearch> = yup.object().shape({
  endProfitYear: profitYearNullableValidator.test(
    "greater-than-start",
    "End year must be after start year",
    function (endYear) {
      const startYear = this.parent.startProfitYear;
      // Only validate if both values are present
      return !startYear || !endYear || endYear >= startYear;
    }
  ),
  startProfitMonth: monthValidator,
  endProfitMonth: monthValidator.min(yup.ref("startProfitMonth"), "End month must be after start month"),
  socialSecurity: ssnValidator,
  name: yup
    .string()
    .nullable()
    .test("min-length", "Name must have at least 2 characters", function (value) {
      if (!value) return true; // Allow empty/null
      return value.length >= 2;
    }),
  badgeNumber: badgeNumberOrPSNValidator,
  comment: yup.string().nullable(),
  paymentType: yup.string().oneOf(["all", "hardship", "payoffs", "rollovers"]).default("all").required(),
  memberType: yup.string().oneOf(["all", "employees", "beneficiaries", "none"]).default("all").required(),
  contribution: yup.number().nullable().typeError("Contribution must be a valid number"),
  earnings: yup.number().nullable().typeError("Earnings must be a valid number"),
  forfeiture: yup.number().nullable().typeError("Forfeiture must be a valid number"),
  payment: yup.number().nullable().typeError("Payment must be a valid number"),
  voids: yup.boolean().default(false).required(),
  pagination: yup
    .object({
      skip: yup.number().required(),
      take: yup.number().required(),
      sortBy: yup.string().required(),
      isSortDescending: yup.boolean().required()
    })
    .required()
});

interface MasterInquirySearchFilterProps {
  onSearch: (params: MasterInquiryRequest) => void;
  onReset: () => void;
  isSearching?: boolean;
}

const MasterInquirySearchFilter: React.FC<MasterInquirySearchFilterProps> = memo(
  ({ onSearch, onReset, isSearching = false }) => {
    const { masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);
    const dispatch = useDispatch();
    const navigate = useNavigate();

    const { badgeNumber } = useParams<{
      badgeNumber: string;
    }>();

    // Ref to track if URL search has been processed
    const urlSearchProcessedRef = useRef(false);

    // profitYear should always start with this year
    const profitYear = useDecemberFlowProfitYear();

    const determineCorrectMemberType = (badgeNum: string | undefined) => {
      if (!badgeNum) return "all";
      if (badgeNum.length <= MAX_EMPLOYEE_BADGE_LENGTH) return "employees";
      return "beneficiaries";
    };

    const {
      control,
      handleSubmit,
      formState: { errors, isValid },
      reset,
      setValue
    } = useForm<MasterInquirySearch>({
      resolver: yupResolver(schema),
      mode: "onBlur",
      defaultValues: {
        endProfitYear: profitYear,
        startProfitMonth: masterInquiryRequestParams?.startProfitMonth || undefined,
        endProfitMonth: masterInquiryRequestParams?.endProfitMonth || undefined,
        socialSecurity: masterInquiryRequestParams?.socialSecurity || undefined,
        name: masterInquiryRequestParams?.name || undefined,
        badgeNumber: masterInquiryRequestParams?.badgeNumber || undefined,
        paymentType: masterInquiryRequestParams?.paymentType ? masterInquiryRequestParams?.paymentType : "all",
        memberType: determineCorrectMemberType(badgeNumber),
        contribution: masterInquiryRequestParams?.contribution || undefined,
        earnings: masterInquiryRequestParams?.earnings || undefined,
        forfeiture: masterInquiryRequestParams?.forfeiture || undefined,
        payment: masterInquiryRequestParams?.payment || undefined,
        voids: false,
        pagination: {
          skip: 0,
          take: 5,
          sortBy: "badgeNumber",
          isSortDescending: true
        }
      }
    });

    // Initialize form when badge number is provided via URL
    useEffect(() => {
      if (badgeNumber && !urlSearchProcessedRef.current) {
        urlSearchProcessedRef.current = true;

        const formData = {
          ...schema.getDefault(),
          memberType: determineCorrectMemberType(badgeNumber) as "all" | "employees" | "beneficiaries" | "none",
          badgeNumber: Number(badgeNumber),
          endProfitYear: profitYear,
          pagination: {
            skip: 0,
            take: 5,
            sortBy: "badgeNumber",
            isSortDescending: true
          }
        };

        reset(formData);

        // Transform form data to search params and execute search
        const searchParams = transformSearchParams(formData, profitYear);
        onSearch(searchParams);

        // Remove badge number from URL after consuming it
        navigate(`/${ROUTES.MASTER_INQUIRY}`, { replace: true });
      }
    }, [badgeNumber, reset, profitYear, onSearch, navigate]);

    /*
    const selectSx = useMemo(
      () => ({
        "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
          borderColor: "#0258A5"
        },
        "&:hover .MuiOutlinedInput-notchedOutline": {
          borderColor: "#0258A5"
        }
      }),
      []
    );
    */

    //const months = useMemo(() => Array.from({ length: 12 }, (_, i) => i + 1), []);

    const onSubmit = useCallback(
      (data: MasterInquirySearch) => {
        if (isValid) {
          const searchParams = transformSearchParams(data, profitYear);
          onSearch(searchParams);
          dispatch(setMasterInquiryRequestParams(data));
        }
      },
      [isValid, profitYear, onSearch, dispatch]
    );

    const validateAndSearch = handleSubmit(onSubmit);

    const handleReset = useCallback(() => {
      dispatch(clearMasterInquiryRequestParams());
      dispatch(clearMasterInquiryData());
      dispatch(clearMasterInquiryGroupingData());
      reset({
        endProfitYear: profitYear,
        startProfitMonth: undefined,
        endProfitMonth: undefined,
        socialSecurity: null,
        name: undefined,
        badgeNumber: undefined,
        paymentType: "all",
        memberType: "all",
        contribution: undefined,
        earnings: undefined,
        forfeiture: undefined,
        payment: undefined,
        voids: false,
        pagination: {
          skip: 0,
          take: 5,
          sortBy: "badgeNumber",
          isSortDescending: true
        }
      });
      onReset();
    }, [dispatch, reset, profitYear, onReset]);

    // Memoized form field components
    /*
    const ProfitYearField = memo(() => (
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <Controller
          name="endProfitYear"
          control={control}
          render={({ field }) => (
            <DSMDatePicker
              id="Profit Year"
              onChange={(value: Date | null) => field.onChange(value?.getFullYear() || undefined)}
              value={field.value ? new Date(field.value, 0) : null}
              required={true}
              label="Profit Year"
              disableFuture
              views={["year"]}
              minDate={new Date(2020, 0)}
              error={errors.endProfitYear?.message}
            />
          )}
        />
        {errors.endProfitYear && <FormHelperText error>{errors.endProfitYear.message}</FormHelperText>}
      </Grid>
    ));
    */
    /*
    const MonthSelectField = memo(({ name, label }: { name: "startProfitMonth" | "endProfitMonth"; label: string }) => (
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <FormLabel>{label}</FormLabel>
        <Controller
          name={name}
          control={control}
          render={({ field }) => (
            <Select
              name={field.name}
              ref={field.ref}
              onChange={(e) => {
                field.onChange(
                  typeof e.target.value === "string" && e.target.value === "" ? null : Number(e.target.value)
                );
              }}
              onBlur={field.onBlur}
              sx={selectSx}
              fullWidth
              size="small"
              value={field.value ?? ""}
              error={!!errors[name]}>
              <MenuItem value="">
                <em>None</em>
              </MenuItem>
              {months.map((month) => (
                <MenuItem
                  key={month}
                  value={month}>
                  {month}
                </MenuItem>
              ))}
            </Select>
          )}
        />
        {errors[name] && <FormHelperText error>{errors[name]?.message}</FormHelperText>}
      </Grid>
    ));
*/
    const handleBadgeNumberChange = useCallback(
      (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const badgeStr = e.target.value;
        let memberType: string;

        if (badgeStr.length === 0) {
          memberType = "all";
        } else if (badgeStr.length >= 8) {
          memberType = "beneficiaries";
        } else {
          memberType = "employees";
        }

        setValue("memberType", memberType as "all" | "employees" | "beneficiaries" | "none");
      },
      [setValue]
    );

    const TextInputField = useCallback(
      ({
        name,
        label,
        type = "text",
        disabled = false,
        helperText
      }: {
        name: keyof MasterInquirySearch;
        label: string;
        type?: string;
        disabled?: boolean;
        helperText?: string;
      }) => (
        <Grid
          size={{
            xs: 12,
            sm: 6,
            md:
              type === "number" ||
              name === "contribution" ||
              name === "earnings" ||
              name === "forfeiture" ||
              name === "payment"
                ? 2
                : 4
          }}>
          <FormLabel>{label}</FormLabel>
          <Controller
            name={name}
            control={control}
            render={({ field }) => (
              <TextField
                name={field.name}
                ref={field.ref}
                onBlur={field.onBlur}
                fullWidth
                type={type}
                size="small"
                variant="outlined"
                value={field.value ?? ""}
                error={!!errors[name]}
                disabled={disabled}
                onChange={(e) => {
                  const value = e.target.value;

                  // For SSN and badge fields, only allow numeric input
                  if (name === "socialSecurity" || name === "badgeNumber") {
                    if (value !== "" && !/^\d*$/.test(value)) {
                      return;
                    }
                  }

                  // For dollar amount fields, only allow 0-9, ., and - characters
                  if (name === "contribution" || name === "earnings" || name === "forfeiture" || name === "payment") {
                    if (value !== "" && !/^-?\d*\.?\d*$/.test(value)) {
                      return;
                    }
                  }

                  // Prevent input beyond 11 characters for badgeNumber
                  if (name === "badgeNumber" && value.length > 11) {
                    return;
                  }
                  // Prevent input beyond 9 characters for socialSecurity
                  if (name === "socialSecurity" && value.length > 9) {
                    return;
                  }

                  // Parse value for number fields and dollar amount fields
                  let parsedValue;

                  if (value === "") {
                    parsedValue = null;
                  } else if (name === "contribution" || name === "earnings" || name === "forfeiture" || name === "payment") {
                    // For dollar fields, keep as string while typing (intermediate states)
                    // Only convert to number when it's a complete valid number
                    const endsWithPeriod = value.endsWith(".");
                    const isIntermediateState = value === "-" || value === "." || value === "-." || endsWithPeriod;
                    const isValidNumber = !isIntermediateState && !isNaN(Number(value));
                    parsedValue = isValidNumber ? Number(value) : value;
                  } else if (type === "number") {
                    parsedValue = Number(value);
                  } else {
                    parsedValue = value;
                  }

                  field.onChange(parsedValue);

                  // Auto-update memberType when badgeNumber changes
                  if (name === "badgeNumber") {
                    handleBadgeNumberChange(e);
                  }
                }}
                sx={
                  disabled
                    ? {
                        "& .MuiOutlinedInput-root": {
                          backgroundColor: "#f5f5f5"
                        }
                      }
                    : undefined
                }
              />
            )}
          />
          {errors[name] && <FormHelperText error>{errors[name]?.message}</FormHelperText>}
          {!errors[name] && helperText && (
            <FormHelperText sx={{ color: "info.main", fontSize: "0.75rem", marginTop: "4px" }}>
              {helperText}
            </FormHelperText>
          )}
        </Grid>
      ),
      [control, errors, handleBadgeNumberChange]
    );

    const RadioGroupField = memo(
      ({
        name,
        label,
        options,
        disabled = false
      }: {
        name: "paymentType" | "memberType";
        label: string;
        options: Array<{ value: string; label: string }>;
        disabled?: boolean;
      }) => (
        <Grid size={{ xs: 12, sm: 6, md: 6 }}>
          <FormControl error={!!errors[name]}>
            <FormLabel>{label}</FormLabel>
            <Controller
              name={name}
              control={control}
              render={({ field }) => (
                <RadioGroup
                  {...field}
                  row>
                  {options.map((option) => (
                    <FormControlLabel
                      key={option.value}
                      value={option.value}
                      control={
                        <Radio
                          size="small"
                          disabled={disabled}
                        />
                      }
                      label={option.label}
                      disabled={disabled}
                    />
                  ))}
                </RadioGroup>
              )}
            />
          </FormControl>
        </Grid>
      )
    );

    const VoidsCheckboxField = memo(() => (
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <Controller
          name="voids"
          control={control}
          render={({ field }) => (
            <FormControlLabel
              control={
                <Checkbox
                  name={field.name}
                  ref={field.ref}
                  checked={field.value}
                  onChange={field.onChange}
                  onBlur={field.onBlur}
                  size="small"
                />
              }
              label="Voids"
              sx={{
                marginTop: "20px",
                height: "40px",
                display: "flex",
                alignItems: "center"
              }}
            />
          )}
        />
      </Grid>
    ));

    const paymentTypeOptions = useMemo(
      () => [
        { value: "all", label: "All" },
        { value: "hardship", label: "Hardship/Dist" },
        { value: "payoffs", label: "Payoffs/Forfeit" },
        { value: "rollovers", label: "Rollovers" }
      ],
      []
    );

    const memberTypeOptions = useMemo(
      () => [
        { value: "all", label: "All" },
        { value: "employees", label: "Employees" },
        { value: "beneficiaries", label: "Beneficiaries" }
      ],
      []
    );

    // Watch the badgeNumber field to determine if memberType should be disabled
    const badgeNumberValue = useWatch({
      control,
      name: "badgeNumber"
    });

    // Watch the three mutually exclusive fields
    const socialSecurityValue = useWatch({ control, name: "socialSecurity" });
    const nameValue = useWatch({ control, name: "name" });
    const badgeNumberWatchValue = useWatch({ control, name: "badgeNumber" });

    // Watch all fields that should enable the search button
    const watchedBadgeNumber = useWatch({ control, name: "badgeNumber" });
    const watchedStartProfitMonth = useWatch({ control, name: "startProfitMonth" });
    const watchedEndProfitMonth = useWatch({ control, name: "endProfitMonth" });
    const watchedSocialSecurity = useWatch({ control, name: "socialSecurity" });
    const watchedName = useWatch({ control, name: "name" });
    const watchedContribution = useWatch({ control, name: "contribution" });
    const watchedEarnings = useWatch({ control, name: "earnings" });
    const watchedForfeiture = useWatch({ control, name: "forfeiture" });
    const watchedPayment = useWatch({ control, name: "payment" });
    const watchedMemberType = useWatch({ control, name: "memberType" });
    const watchedPaymentType = useWatch({ control, name: "paymentType" });

    // Helper function to check if a value is non-empty
    const hasValue = useCallback(
      (value: string | number | null | undefined) => value !== null && value !== undefined && value !== "",
      []
    );

    // Determine which fields should be disabled based on which have values
    const hasSocialSecurity = hasValue(socialSecurityValue);
    const hasName = hasValue(nameValue);
    const hasBadgeNumber = hasValue(badgeNumberWatchValue);

    // Disable other fields when one has a value
    const isSocialSecurityDisabled = hasName || hasBadgeNumber;
    const isNameDisabled = hasSocialSecurity || hasBadgeNumber;
    const isBadgeNumberDisabled = hasSocialSecurity || hasName;

    // Helper text for mutual exclusion
    const getExclusionHelperText = useCallback(
      (fieldName: string, isDisabled: boolean) => {
        if (!isDisabled) return undefined;

        if (fieldName === "socialSecurity") {
          if (hasName) return "Disabled: Name field is in use. Press Reset to clear and re-enable.";
          if (hasBadgeNumber) return "Disabled: Badge/PSN field is in use. Press Reset to clear and re-enable.";
        }
        if (fieldName === "name") {
          if (hasSocialSecurity) return "Disabled: SSN field is in use. Press Reset to clear and re-enable.";
          if (hasBadgeNumber) return "Disabled: Badge/PSN field is in use. Press Reset to clear and re-enable.";
        }
        if (fieldName === "badgeNumber") {
          if (hasSocialSecurity) return "Disabled: SSN field is in use. Press Reset to clear and re-enable.";
          if (hasName) return "Disabled: Name field is in use. Press Reset to clear and re-enable.";
        }

        return undefined;
      },
      [hasSocialSecurity, hasName, hasBadgeNumber]
    );

    const isMemberTypeDisabled = badgeNumberValue !== null && badgeNumberValue !== undefined;

    // Determine if search button should be enabled
    const hasSearchCriteria = useMemo(() => {
      // Check if any of the specific fields have values
      const hasFieldValues =
        hasValue(watchedBadgeNumber) ||
        hasValue(watchedStartProfitMonth) ||
        hasValue(watchedEndProfitMonth) ||
        hasValue(watchedSocialSecurity) ||
        hasValue(watchedName) ||
        hasValue(watchedContribution) ||
        hasValue(watchedEarnings) ||
        hasValue(watchedForfeiture) ||
        hasValue(watchedPayment);

      // Check if memberType or paymentType are not at default values
      const hasNonDefaultSelections = watchedMemberType !== "all" || watchedPaymentType !== "all";

      return hasFieldValues || hasNonDefaultSelections;
    }, [
      watchedBadgeNumber,
      watchedStartProfitMonth,
      watchedEndProfitMonth,
      watchedSocialSecurity,
      watchedName,
      watchedContribution,
      watchedEarnings,
      watchedForfeiture,
      watchedPayment,
      watchedMemberType,
      watchedPaymentType,
      hasValue
    ]);

    return (
      <form onSubmit={validateAndSearch}>
        <Grid
          container
          paddingX="24px">
          <Grid
            container
            spacing={3}
            width="100%">
            <TextInputField
              name="socialSecurity"
              label="Social Security Number"
              type="text"
              disabled={isSocialSecurityDisabled}
              helperText={getExclusionHelperText("socialSecurity", isSocialSecurityDisabled)}
            />
            <TextInputField
              name="name"
              label="Name"
              disabled={isNameDisabled}
              helperText={getExclusionHelperText("name", isNameDisabled)}
            />
            <TextInputField
              name="badgeNumber"
              label="Badge/PSN Number"
              type="text"
              disabled={isBadgeNumberDisabled}
              helperText={getExclusionHelperText("badgeNumber", isBadgeNumberDisabled)}
            />
            <RadioGroupField
              name="paymentType"
              label="Payment Type"
              options={paymentTypeOptions}
            />
            <RadioGroupField
              name="memberType"
              label="Member Type"
              options={memberTypeOptions}
              disabled={isMemberTypeDisabled}
            />
            <TextInputField
              name="contribution"
              label="Contribution"
              type="text"
            />
            <TextInputField
              name="earnings"
              label="Earnings"
              type="text"
            />
            <TextInputField
              name="forfeiture"
              label="Forfeiture"
              type="text"
            />
            <TextInputField
              name="payment"
              label="Payment"
              type="text"
            />
            <VoidsCheckboxField />
          </Grid>

          <Grid
            container
            justifyContent="flex-end"
            paddingY="16px">
            <Grid size={{ xs: 12 }}>
              <SearchAndReset
                handleReset={handleReset}
                handleSearch={validateAndSearch}
                isFetching={isSearching}
                disabled={!isValid || isSearching || !hasSearchCriteria}
              />
            </Grid>
          </Grid>
        </Grid>
      </form>
    );
  },
  (prevProps, nextProps) => {
    // Custom comparison function
    // Only re-render if incoming props are different
    return (
      prevProps.isSearching === nextProps.isSearching &&
      prevProps.onSearch === nextProps.onSearch &&
      prevProps.onReset === nextProps.onReset
    );
  }
);

export default MasterInquirySearchFilter;
