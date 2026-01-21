import { yupResolver } from "@hookform/resolvers/yup";
import {
  Checkbox,
  FormControl,
  FormControlLabel,
  FormHelperText,
  FormLabel,
  Grid,
  ListItemText,
  MenuItem,
  Radio,
  RadioGroup,
  TextField
} from "@mui/material";
import { memo, useCallback, useEffect, useMemo, useState } from "react";
import { Controller, Resolver, useForm, useWatch } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useGetTaxCodesQuery } from "reduxstore/api/LookupsApi";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import {
  clearCurrentDistribution,
  clearCurrentMember,
  clearHistoricalDisbursements,
  clearPendingDisbursements
} from "../../../reduxstore/slices/distributionSlice";
import { DistributionSearchFormData } from "../../../types";
import { badgeNumberOrPSNValidator, mustBeNumberValidator, ssnValidator } from "../../../utils/FormValidators";
import { getDistributionIdLabel } from "../../../utils/lookups";

interface DistributionInquirySearchFilterProps {
  onSearch: (data: DistributionSearchFormData) => void;
  onReset: () => void;
  isLoading: boolean;
}

const schema = yup.object().shape({
  socialSecurity: ssnValidator,
  badgeNumber: badgeNumberOrPSNValidator,
  memberType: yup.string().oneOf(["all", "employees", "beneficiaries"]).default("all").required(),
  frequency: yup.string().nullable(),
  paymentFlag: yup.string().nullable(),
  paymentFlags: yup.array().of(yup.string().required()).nullable(),
  taxCode: yup.string().nullable(),
  minGrossAmount: mustBeNumberValidator(),
  maxGrossAmount: mustBeNumberValidator().test("greater-than-min", "Max must be greater than Min", function (value) {
    const { minGrossAmount } = this.parent;
    if (!value || !minGrossAmount) return true;
    return Number(value) >= Number(minGrossAmount);
  }),
  minCheckAmount: mustBeNumberValidator(),
  maxCheckAmount: mustBeNumberValidator().test("greater-than-min", "Max must be greater than Min", function (value) {
    const { minCheckAmount } = this.parent;
    if (!value || !minCheckAmount) return true;
    return Number(value) >= Number(minCheckAmount);
  })
});

const DistributionInquirySearchFilter: React.FC<DistributionInquirySearchFilterProps> = memo(
  ({ onSearch, onReset, isLoading }) => {
    const dispatch = useDispatch();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const { data: taxCodesData, isLoading: isLoadingTaxCodes } = useGetTaxCodesQuery({
      availableForDistribution: true
    });

    const {
      control,
      handleSubmit,
      reset,
      setValue,
      formState: { errors, isValid }
    } = useForm<DistributionSearchFormData>({
      resolver: yupResolver(schema) as Resolver<DistributionSearchFormData>,
      mode: "onChange",
      defaultValues: {
        socialSecurity: undefined,
        badgeNumber: undefined,
        memberType: "all",
        frequency: null,
        paymentFlag: null,
        paymentFlags: [],
        taxCode: null,
        minGrossAmount: "",
        maxGrossAmount: "",
        minCheckAmount: "",
        maxCheckAmount: ""
      }
    });

    useEffect(() => {
      if (!isLoading) {
        setIsSubmitting(false);
      }
    }, [isLoading]);

    const handleFormSubmit = (data: DistributionSearchFormData) => {
      if (!isSubmitting) {
        setIsSubmitting(true);
        onSearch(data);
      }
    };

    const handleFormReset = useCallback(() => {
      reset({
        socialSecurity: undefined,
        badgeNumber: undefined,
        memberType: "all",
        frequency: null,
        paymentFlag: null,
        paymentFlags: [],
        taxCode: null,
        minGrossAmount: "",
        maxGrossAmount: "",
        minCheckAmount: "",
        maxCheckAmount: ""
      });

      // Clear all distribution slice data
      dispatch(clearCurrentMember());
      dispatch(clearCurrentDistribution());
      dispatch(clearPendingDisbursements());
      dispatch(clearHistoricalDisbursements());

      onReset();
    }, [dispatch, reset, onReset]);

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

        setValue("memberType", memberType as "all" | "employees" | "beneficiaries");
      },
      [setValue]
    );

    // Watch the three mutually exclusive fields
    const socialSecurityValue = useWatch({ control, name: "socialSecurity" });
    const badgeNumberValue = useWatch({ control, name: "badgeNumber" });

    // Helper function to check if a value is non-empty
    const hasValue = useCallback(
      (value: string | number | null | undefined) => value !== null && value !== undefined && value !== "",
      []
    );

    // Determine which fields should be disabled based on which have values
    const hasSocialSecurity = hasValue(socialSecurityValue);
    const hasBadgeNumber = hasValue(badgeNumberValue);

    // Disable other field when one has a value
    const isSocialSecurityDisabled = hasBadgeNumber;
    const isBadgeNumberDisabled = hasSocialSecurity;

    // Helper text for mutual exclusion
    const getExclusionHelperText = useCallback(
      (fieldName: string, isDisabled: boolean) => {
        if (!isDisabled) return undefined;

        if (fieldName === "socialSecurity") {
          if (hasBadgeNumber) return "Disabled: Badge/PSN field is in use. Press Reset to clear and re-enable.";
        }
        if (fieldName === "badgeNumber") {
          if (hasSocialSecurity) return "Disabled: SSN field is in use. Press Reset to clear and re-enable.";
        }

        return undefined;
      },
      [hasSocialSecurity, hasBadgeNumber]
    );

    const isMemberTypeDisabled = badgeNumberValue !== null && badgeNumberValue !== undefined;

    const memberTypeOptions = useMemo(
      () => [
        { value: "all", label: "All" },
        { value: "employees", label: "Employees" },
        { value: "beneficiaries", label: "Beneficiaries" }
      ],
      []
    );

    return (
      <form onSubmit={handleSubmit(handleFormSubmit)}>
        <Grid
          container
          paddingX="24px"
          spacing={3}>
          {/* First Row: SSN, Badge, Member Type */}
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Social Security Number</FormLabel>
            <Controller
              name="socialSecurity"
              control={control}
              render={({ field }) => (
                <TextField
                  name={field.name}
                  ref={field.ref}
                  onBlur={field.onBlur}
                  fullWidth
                  size="small"
                  variant="outlined"
                  placeholder="9 digits"
                  value={field.value ?? ""}
                  error={!!errors.socialSecurity}
                  disabled={isSocialSecurityDisabled}
                  onChange={(e) => {
                    const value = e.target.value;
                    if (value !== "" && !/^\d*$/.test(value)) {
                      return;
                    }
                    if (value.length > 9) {
                      return;
                    }
                    field.onChange(value === "" ? null : value);
                  }}
                  sx={
                    isSocialSecurityDisabled
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
            {errors.socialSecurity && <FormHelperText error>{errors.socialSecurity?.message}</FormHelperText>}
            {!errors.socialSecurity && getExclusionHelperText("socialSecurity", isSocialSecurityDisabled) && (
              <FormHelperText sx={{ color: "info.main", fontSize: "0.75rem", marginTop: "4px" }}>
                {getExclusionHelperText("socialSecurity", isSocialSecurityDisabled)}
              </FormHelperText>
            )}
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Badge/PSN Number</FormLabel>
            <Controller
              name="badgeNumber"
              control={control}
              render={({ field }) => (
                <TextField
                  name={field.name}
                  ref={field.ref}
                  onBlur={field.onBlur}
                  fullWidth
                  size="small"
                  variant="outlined"
                  placeholder="5-11 digits"
                  value={field.value ?? ""}
                  error={!!errors.badgeNumber}
                  disabled={isBadgeNumberDisabled}
                  onChange={(e) => {
                    const value = e.target.value;
                    if (value !== "" && !/^\d*$/.test(value)) {
                      return;
                    }
                    if (value.length > 11) {
                      return;
                    }
                    field.onChange(value === "" ? null : value);
                    handleBadgeNumberChange(e);
                  }}
                  sx={
                    isBadgeNumberDisabled
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
            {errors.badgeNumber && <FormHelperText error>{errors.badgeNumber?.message}</FormHelperText>}
            {!errors.badgeNumber && getExclusionHelperText("badgeNumber", isBadgeNumberDisabled) && (
              <FormHelperText sx={{ color: "info.main", fontSize: "0.75rem", marginTop: "4px" }}>
                {getExclusionHelperText("badgeNumber", isBadgeNumberDisabled)}
              </FormHelperText>
            )}
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 6 }}>
            <FormControl error={!!errors.memberType}>
              <FormLabel>Member Type</FormLabel>
              <Controller
                name="memberType"
                control={control}
                render={({ field }) => (
                  <RadioGroup
                    {...field}
                    row>
                    {memberTypeOptions.map((option) => (
                      <FormControlLabel
                        key={option.value}
                        value={option.value}
                        control={
                          <Radio
                            size="small"
                            disabled={isMemberTypeDisabled}
                          />
                        }
                        label={option.label}
                        disabled={isMemberTypeDisabled}
                      />
                    ))}
                  </RadioGroup>
                )}
              />
            </FormControl>
          </Grid>

          {/* Second Row: Frequency, Payment Flag, Tax Code */}
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Frequency</FormLabel>
            <Controller
              name="frequency"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value ?? ""}
                  select
                  fullWidth
                  size="small"
                  variant="outlined"
                  error={!!errors.frequency}
                  helperText={errors.frequency?.message}>
                  <MenuItem value="">All</MenuItem>
                  <MenuItem value="H">Hardship</MenuItem>
                  <MenuItem value="P">Payout</MenuItem>
                  <MenuItem value="M">Monthly</MenuItem>
                  <MenuItem value="Q">Quarterly</MenuItem>
                </TextField>
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 6 }}>
            <FormLabel>Payment Flag</FormLabel>
            <Controller
              name="paymentFlags"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value ?? []}
                  select
                  SelectProps={{
                    multiple: true,
                    renderValue: (selected) => {
                      const selectedArray = selected as string[];
                      return selectedArray.length === 0 ? "All" : selectedArray.join(", ");
                    }
                  }}
                  fullWidth
                  size="small"
                  variant="outlined"
                  error={!!errors.paymentFlags}
                  helperText={errors.paymentFlags?.message}>
                  <MenuItem value="C">
                    <Checkbox checked={(field.value ?? []).indexOf("C") > -1} />
                    <ListItemText primary={`C - ${getDistributionIdLabel("C")}`} />
                  </MenuItem>
                  <MenuItem value="D">
                    <Checkbox checked={(field.value ?? []).indexOf("D") > -1} />
                    <ListItemText primary={`D - ${getDistributionIdLabel("D")}`} />
                  </MenuItem>
                  <MenuItem value="H">
                    <Checkbox checked={(field.value ?? []).indexOf("H") > -1} />
                    <ListItemText primary={`H - ${getDistributionIdLabel("H")}`} />
                  </MenuItem>
                  <MenuItem value="O">
                    <Checkbox checked={(field.value ?? []).indexOf("O") > -1} />
                    <ListItemText primary={`O - ${getDistributionIdLabel("O")}`} />
                  </MenuItem>
                  <MenuItem value="P">
                    <Checkbox checked={(field.value ?? []).indexOf("P") > -1} />
                    <ListItemText primary={`P - ${getDistributionIdLabel("P")}`} />
                  </MenuItem>
                  <MenuItem value="X">
                    <Checkbox checked={(field.value ?? []).indexOf("X") > -1} />
                    <ListItemText primary={`X - ${getDistributionIdLabel("X")}`} />
                  </MenuItem>
                  <MenuItem value="Y">
                    <Checkbox checked={(field.value ?? []).indexOf("Y") > -1} />
                    <ListItemText primary={`Y - ${getDistributionIdLabel("Y")}`} />
                  </MenuItem>
                  <MenuItem value="Z">
                    <Checkbox checked={(field.value ?? []).indexOf("Z") > -1} />
                    <ListItemText primary={`Z - ${getDistributionIdLabel("Z")}`} />
                  </MenuItem>
                </TextField>
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Tax Code</FormLabel>
            <Controller
              name="taxCode"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value ?? ""}
                  select
                  fullWidth
                  size="small"
                  variant="outlined"
                  disabled={isLoadingTaxCodes}
                  error={!!errors.taxCode}
                  helperText={errors.taxCode?.message}>
                  <MenuItem value="">All</MenuItem>
                  {taxCodesData?.map((taxCode) => (
                    <MenuItem
                      key={taxCode.id}
                      value={taxCode.id}>
                      {taxCode.id} - {taxCode.name}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />
          </Grid>

          {/* Third Row: Min Gross Amount, Max Gross Amount, Min Check Amount, Max Check Amount */}
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Min Gross Amount</FormLabel>
            <Controller
              name="minGrossAmount"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value ?? ""}
                  fullWidth
                  size="small"
                  variant="outlined"
                  type="number"
                  placeholder="0.00"
                  error={!!errors.minGrossAmount}
                  helperText={errors.minGrossAmount?.message}
                />
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Max Gross Amount</FormLabel>
            <Controller
              name="maxGrossAmount"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value ?? ""}
                  fullWidth
                  size="small"
                  variant="outlined"
                  type="number"
                  placeholder="0.00"
                  error={!!errors.maxGrossAmount}
                  helperText={errors.maxGrossAmount?.message}
                />
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Min Check Amount</FormLabel>
            <Controller
              name="minCheckAmount"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value ?? ""}
                  fullWidth
                  size="small"
                  variant="outlined"
                  type="number"
                  placeholder="0.00"
                  error={!!errors.minCheckAmount}
                  helperText={errors.minCheckAmount?.message}
                />
              )}
            />
          </Grid>

          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Max Check Amount</FormLabel>
            <Controller
              name="maxCheckAmount"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  value={field.value ?? ""}
                  fullWidth
                  size="small"
                  variant="outlined"
                  type="number"
                  placeholder="0.00"
                  error={!!errors.maxCheckAmount}
                  helperText={errors.maxCheckAmount?.message}
                />
              )}
            />
          </Grid>
        </Grid>

        <Grid
          width="100%"
          paddingX="24px">
          <SearchAndReset
            handleReset={handleFormReset}
            handleSearch={handleSubmit(handleFormSubmit)}
            isFetching={isLoading || isSubmitting}
            disabled={!isValid || isLoading || isSubmitting}
          />
        </Grid>
      </form>
    );
  },
  (prevProps, nextProps) => {
    return (
      prevProps.isLoading === nextProps.isLoading &&
      prevProps.onSearch === nextProps.onSearch &&
      prevProps.onReset === nextProps.onReset
    );
  }
);

export default DistributionInquirySearchFilter;
