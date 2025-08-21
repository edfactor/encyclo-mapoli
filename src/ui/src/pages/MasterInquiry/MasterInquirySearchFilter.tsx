import { yupResolver } from "@hookform/resolvers/yup";
import {
  Checkbox,
  FormControl,
  FormControlLabel,
  FormHelperText,
  FormLabel,
  Grid,
  MenuItem,
  Radio,
  RadioGroup,
  Select,
  TextField
} from "@mui/material";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import React, { memo, useCallback, useEffect, useMemo } from "react";
import { useWatch } from "react-hook-form";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useParams } from "react-router-dom";
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
import { MAX_EMPLOYEE_BADGE_LENGTH } from "../../constants";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";
import { transformSearchParams } from "./utils/transformSearchParams";

const schema = yup.object().shape({
  endProfitYear: yup
    .number()
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .typeError("Invalid date")
    .test("greater-than-start", "End year must be after start year", function (endYear) {
      const startYear = this.parent.startProfitYear;
      // Only validate if both values are present
      return !startYear || !endYear || endYear >= startYear;
    })
    .nullable(),
  startProfitMonth: yup
    .number()
    .typeError("Beginning Month must be a number")
    .integer("Beginning Month must be an integer")
    .min(1, "Beginning Month must be between 1 and 12")
    .max(12, "Beginning Month must be between 1 and 12")
    .nullable(),
  endProfitMonth: yup
    .number()
    .typeError("Ending Month must be a number")
    .integer("Ending Month must be an integer")
    .min(1, "Ending Month must be between 1 and 12")
    .max(12, "Ending Month must be between 1 and 12")
    .min(yup.ref("startProfitMonth"), "End month must be after start month")
    .nullable(),
  socialSecurity: yup
    .number()
    .typeError("SSN must be a number")
    .integer("SSN must be an integer")
    .min(0, "SSN must be positive")
    .max(999999999, "SSN must be 9 digits or less")
    .nullable(),
  name: yup.string().nullable(),
  badgeNumber: yup
    .number()
    .typeError("Badge number must be a number")
    .integer("Badge number must be an integer")
    .min(0, "Badge number must be positive")
    .max(99999999999, "Badge number must be 11 digits or less")
    .nullable(),
  comment: yup.string().nullable(),
  paymentType: yup.string().oneOf(["all", "hardship", "payoffs", "rollovers"]).default("all").required(),
  memberType: yup.string().oneOf(["all", "employees", "beneficiaries", "none"]).default("all").required(),
  contribution: yup
    .number()
    .typeError("Contribution must be a number")
    .min(0, "Contribution must be positive")
    .nullable(),
  earnings: yup.number().typeError("Earnings must be a number").min(0, "Earnings must be positive").nullable(),
  forfeiture: yup.number().typeError("Forfeiture must be a number").min(0, "Forfeiture must be positive").nullable(),
  payment: yup.number().typeError("Payment must be a number").min(0, "Payment must be positive").nullable(),
  voids: yup.boolean().default(false).required()
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

    const { badgeNumber } = useParams<{
      badgeNumber: string;
    }>();

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
      resolver: yupResolver(schema) as any,
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
      if (badgeNumber) {
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
      }
    }, [badgeNumber, reset, profitYear, onSearch]);

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

    const months = useMemo(() => Array.from({ length: 12 }, (_, i) => i + 1), []);

    const validateAndSearch = useCallback(
      handleSubmit((data) => {
        if (isValid) {
          const searchParams = transformSearchParams(data, profitYear);
          onSearch(searchParams);
          dispatch(setMasterInquiryRequestParams(data));
        }
      }),
      [handleSubmit, isValid, profitYear, onSearch, dispatch]
    );

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
    const ProfitYearField = memo(() => (
      <Grid size={{ xs: 12, sm: 6, md: 4 }}>
        <Controller
          name="endProfitYear"
          control={control}
          render={({ field }) => (
            <DsmDatePicker
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

    const handleBadgeNumberChange = useCallback((e: React.ChangeEvent<HTMLInputElement>) => {
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
    }, [setValue]);

    const TextInputField = useCallback(
      ({ name, label, type = "text" }: { name: keyof MasterInquirySearch; label: string; type?: string }) => (
        <Grid size={{ xs: 12, sm: 6, md: type === "number" ? 2 : 4 }}>
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
                onChange={(e) => {
                  // Prevent input beyond 11 characters for badgeNumber
                  if (name === "badgeNumber" && e.target.value.length > 11) {
                    return;
                  }
                  const parsedValue =
                    type === "number" && e.target.value !== ""
                      ? Number(e.target.value)
                      : e.target.value === ""
                        ? null
                        : e.target.value;
                  field.onChange(parsedValue);

                  // Auto-update memberType when badgeNumber changes
                  if (name === "badgeNumber") {
                    handleBadgeNumberChange(e);
                  }
                }}
              />
            )}
          />
          {errors[name] && <FormHelperText error>{errors[name]?.message}</FormHelperText>}
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
                      control={<Radio size="small" disabled={disabled} />}
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

    const isMemberTypeDisabled = badgeNumberValue !== null && badgeNumberValue !== undefined && badgeNumberValue !== "";

    return (
      <form onSubmit={validateAndSearch}>
        <Grid
          container
          paddingX="24px">
          <Grid
            container
            spacing={3}
            width="100%">
            <ProfitYearField />
            <MonthSelectField
              name="startProfitMonth"
              label="Beginning Month"
            />
            <MonthSelectField
              name="endProfitMonth"
              label="Ending Month"
            />
            <TextInputField
              name="socialSecurity"
              label="Social Security Number"
              type="number"
            />
            <TextInputField
              name="name"
              label="Name"
            />
            <TextInputField
              name="badgeNumber"
              label="Badge/PSN Number"
              type="number"
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
              type="number"
            />
            <TextInputField
              name="earnings"
              label="Earnings"
              type="number"
            />
            <TextInputField
              name="forfeiture"
              label="Forfeiture"
              type="number"
            />
            <TextInputField
              name="payment"
              label="Payment"
              type="number"
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
                disabled={!isValid || isSearching}
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
