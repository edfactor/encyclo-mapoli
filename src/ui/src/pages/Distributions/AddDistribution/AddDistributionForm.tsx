import { yupResolver } from "@hookform/resolvers/yup";
import {
  Box,
  Checkbox,
  Divider,
  FormControlLabel,
  FormLabel,
  Grid,
  MenuItem,
  TextField,
  Typography
} from "@mui/material";
import { forwardRef, useCallback, useEffect, useImperativeHandle, useRef, useState } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import { useGetTaxCodesQuery } from "reduxstore/api/LookupsApi";
import * as yup from "yup";
import { CreateDistributionRequest } from "../../../types";

export interface AddDistributionFormRef {
  submit: () => void;
  reset: () => void;
  isFormValid: () => boolean;
  isThirdPartyAddressRequired: () => boolean;
}

interface AddDistributionFormData {
  // Distribution Details
  paymentFlag: string;
  taxCode: string;
  reasonCode: string;
  amountRequested: string;
  fedTaxOverride: boolean;
  fedTaxPct: string;
  fedTax: string;
  stateTaxOverride: boolean;
  stateTaxPct: string;
  stateTax: string;
  sequenceNumber: string;
  memo?: string;
  employeeDeceased: boolean;

  // 3rd Party Details
  thirdPartyName: string;
  thirdPartyAddress1: string;
  thirdPartyAddress2?: string;
  thirdPartyCity: string;
  thirdPartyState: string;
  thirdPartyZip: string;
  checkPayableTo: string;
  fboPayTo: string;
  fboType: string;
  account: string;
  rothIra: boolean;
  thirdPartySsn?: string;
}

interface AddDistributionFormProps {
  stateTaxRate: number | null;
  sequenceNumber: number | null;
  badgeNumber: number;
  onSubmit: (request: CreateDistributionRequest) => Promise<void>;
  onReset: () => void;
  isSubmitting: boolean;
  dateOfBirth?: string | null;
  age?: string | null;
  vestedAmount?: number | null;
}

// Validation schema
const schema = yup.object().shape({
  paymentFlag: yup.string().required("Payment Flag is required"),
  taxCode: yup.string().required("Tax Code is required"),
  reasonCode: yup.string().required("Reason Code is required"),
  amountRequested: yup
    .string()
    .required("Amount Requested is required")
    .test("is-valid-number", "Must be a valid number", (value) => {
      if (!value) return false;
      const num = parseFloat(value);
      return !isNaN(num) && num > 0;
    })
    .test("max-amount", "Amount cannot exceed $999,999.99", (value) => {
      if (!value) return true;
      const num = parseFloat(value);
      return num <= 999999.99;
    }),
  memo: yup.string().max(500, "Memo cannot exceed 500 characters"),
  thirdPartySsn: yup.string().test("valid-ssn", "SSN must be 9 digits", (value) => {
    if (!value || value === "") return true;
    return /^\d{9}$/.test(value);
  })
});

const AddDistributionForm = forwardRef<AddDistributionFormRef, AddDistributionFormProps>(
  ({ stateTaxRate, sequenceNumber, badgeNumber, onSubmit, age, vestedAmount }, ref) => {
    const { data: taxCodesData, isLoading: isLoadingTaxCodes } = useGetTaxCodesQuery({
      availableForDistribution: true
    });
    const {
      control,
      handleSubmit,
      reset,
      setValue,
      watch,
      formState: { errors, isValid }
    } = useForm<AddDistributionFormData>({
      resolver: yupResolver(schema) as unknown as Resolver<AddDistributionFormData>,
      mode: "onChange",
      defaultValues: {
        paymentFlag: "",
        taxCode: "",
        reasonCode: "",
        amountRequested: "",
        fedTaxOverride: false,
        fedTaxPct: "20.0",
        fedTax: "0.00",
        stateTaxOverride: false,
        stateTaxPct: stateTaxRate?.toFixed(2) || "0.00",
        stateTax: "0.00",
        sequenceNumber: sequenceNumber?.toString() || "1",
        memo: "",
        employeeDeceased: false,
        thirdPartyName: "",
        thirdPartyAddress1: "",
        thirdPartyAddress2: "",
        thirdPartyCity: "",
        thirdPartyState: "",
        thirdPartyZip: "",
        checkPayableTo: "",
        fboPayTo: "",
        fboType: "",
        account: "",
        rothIra: false,
        thirdPartySsn: ""
      }
    });

    // Watch amount requested for tax calculations
    const amountRequested = watch("amountRequested");
    const fedTaxOverride = watch("fedTaxOverride");
    const stateTaxOverride = watch("stateTaxOverride");
    const reasonCode = watch("reasonCode");
    const taxCode = watch("taxCode");

    // Watch 3rd party fields
    const thirdPartyName = watch("thirdPartyName");
    const thirdPartyAddress1 = watch("thirdPartyAddress1");
    const thirdPartyCity = watch("thirdPartyCity");
    const thirdPartyState = watch("thirdPartyState");
    const thirdPartyZip = watch("thirdPartyZip");

    // State for 3rd party validation
    const [thirdPartyTouched, setThirdPartyTouched] = useState(false);
    const [thirdPartyAddressValid, setThirdPartyAddressValid] = useState(false);

    // Update state tax PCT when rate changes
    useEffect(() => {
      if (stateTaxRate !== null) {
        setValue("stateTaxPct", stateTaxRate.toFixed(2));
      }
    }, [stateTaxRate, setValue]);

    // Update sequence number when it changes
    useEffect(() => {
      if (sequenceNumber !== null) {
        setValue("sequenceNumber", sequenceNumber.toString());
      }
    }, [sequenceNumber, setValue]);

    // Track previous amount to detect changes
    const prevAmountRef = useRef(amountRequested);

    useEffect(() => {
      if (prevAmountRef.current !== amountRequested && amountRequested) {
        // Amount has changed, uncheck overrides and recalculate
        if (fedTaxOverride) {
          setValue("fedTaxOverride", false);
        }
        if (stateTaxOverride) {
          setValue("stateTaxOverride", false);
        }
      }
      prevAmountRef.current = amountRequested;
    }, [amountRequested, fedTaxOverride, stateTaxOverride, setValue]);

    // Check 3rd party address validation when touched
    useEffect(() => {
      if (thirdPartyTouched) {
        const isValid = !!(
          thirdPartyAddress1?.trim() &&
          thirdPartyCity?.trim() &&
          thirdPartyState?.trim() &&
          thirdPartyZip?.trim()
        );
        setThirdPartyAddressValid(isValid);
      }
    }, [thirdPartyTouched, thirdPartyAddress1, thirdPartyCity, thirdPartyState, thirdPartyZip]);

    // Clear 3rd party validation when name is cleared
    useEffect(() => {
      if (!thirdPartyName) {
        setThirdPartyTouched(false);
        setThirdPartyAddressValid(false);
      }
    }, [thirdPartyName]);

    // Auto-set tax code and reason code when 3rd party name is entered
    useEffect(() => {
      if (thirdPartyName?.trim()) {
        setValue("taxCode", "3"); // Direct rollover
        setValue("reasonCode", "R"); // Rollover
      }
    }, [thirdPartyName, setValue]);

    // Clear tax overrides when Rollover is selected
    useEffect(() => {
      if (reasonCode === "R") {
        setValue("fedTaxOverride", false);
        setValue("stateTaxOverride", false);
      }
    }, [reasonCode, setValue]);

    // Auto-set memo when age > 64, tax code is 7, and amount differs from vested amount
    useEffect(() => {
      const memberAge = age != null ? Number(age) : 0;
      const requestedAmount = parseFloat(amountRequested) || 0;
      const vested = vestedAmount ?? 0;

      if (memberAge > 64 && taxCode === "7" && requestedAmount !== vested && requestedAmount > 0) {
        setValue("memo", "AGE > 64 OVERRIDE");
      }
    }, [age, taxCode, amountRequested, vestedAmount, setValue]);

    // Calculate taxes on amount change
    const calculateTaxes = useCallback(() => {
      const amount = parseFloat(amountRequested);
      if (!isNaN(amount) && amount > 0) {
        // Federal tax = 20% unless overridden
        if (!fedTaxOverride) {
          const fedTax = amount * 0.2;
          setValue("fedTax", fedTax.toFixed(2));
        }

        // State tax = state tax rate unless overridden
        if (!stateTaxOverride && stateTaxRate !== null) {
          const stateTax = amount * (stateTaxRate / 100);
          setValue("stateTax", stateTax.toFixed(2));
        }
      } else {
        setValue("fedTax", "0.00");
        setValue("stateTax", "0.00");
      }
    }, [amountRequested, fedTaxOverride, stateTaxOverride, stateTaxRate, setValue]);

    // Trigger calculation on amount blur
    useEffect(() => {
      calculateTaxes();
    }, [amountRequested, calculateTaxes]);

    const handleFormSubmit = async (data: AddDistributionFormData) => {
      // Build CreateDistributionRequest
      const request: CreateDistributionRequest = {
        badgeNumber,
        statusId: data.paymentFlag || "C",
        frequencyId: "M", // Default to Monthly
        payeeId: null,
        thirdPartyPayee: data.thirdPartyName
          ? {
              payee: data.checkPayableTo || null,
              name: data.thirdPartyName || null,
              account: data.account || null,
              address: {
                street: data.thirdPartyAddress1 || "",
                street2: data.thirdPartyAddress2 || null,
                city: data.thirdPartyCity || null,
                state: data.thirdPartyState || null,
                postalCode: data.thirdPartyZip || null,
                countryIso: "US"
              },
              memo: data.memo || null
            }
          : null,
        forTheBenefitOfPayee: data.fboPayTo || null,
        forTheBenefitOfAccountType: data.fboType || null,
        tax1099ForEmployee: false,
        tax1099ForBeneficiary: false,
        federalTaxPercentage: 20.0,
        stateTaxPercentage: stateTaxRate || 0,
        grossAmount: parseFloat(data.amountRequested),
        federalTaxAmount: parseFloat(data.fedTax),
        stateTaxAmount: parseFloat(data.stateTax),
        checkAmount: parseFloat(data.amountRequested) - parseFloat(data.fedTax) - parseFloat(data.stateTax),
        taxCodeId: data.taxCode || "1",
        isDeceased: data.employeeDeceased,
        genderId: null,
        isQdro: false,
        memo: data.memo || null,
        isRothIra: data.rothIra
      };

      await onSubmit(request);
    };

    const handleFormReset = () => {
      reset();
    };

    // Expose submit and reset methods to parent via ref
    useImperativeHandle(ref, () => ({
      submit: () => {
        handleSubmit(handleFormSubmit as (data: AddDistributionFormData) => Promise<void>)();
      },
      reset: () => {
        handleFormReset();
      },
      isFormValid: () => isValid,
      isThirdPartyAddressRequired: () => thirdPartyTouched && !thirdPartyAddressValid
    }));

    return (
      <form onSubmit={handleSubmit(handleFormSubmit as (data: AddDistributionFormData) => Promise<void>)}>
        <Grid
          container
          spacing={3}>
          {/* Distribution Details Section */}
          <Grid size={{ xs: 12 }}>
            <Box sx={{ mb: 1 }}>
              <Typography
                variant="h2"
                sx={{ color: "#0258A5", mb: 0.5 }}>
                Distribution Details
              </Typography>
              <Divider />
            </Box>
          </Grid>

          {/* Form Fields Grid - 4 columns */}
          <Grid size={{ xs: 12 }}>
            <Grid
              container
              spacing={1.5}>
              {/* Row 1: Payment Flag, Tax Code, Reason Code, Amount Requested */}
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <FormLabel>Payment Flag *</FormLabel>
                <Controller
                  name="paymentFlag"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      value={field.value || ""}
                      select
                      fullWidth
                      size="small"
                      variant="outlined"
                      error={!!errors.paymentFlag}
                      helperText={errors.paymentFlag?.message}>
                      <MenuItem value="C">C - Ok to pay</MenuItem>
                      <MenuItem value="D">D - Deferred</MenuItem>
                      <MenuItem value="H">H - Hold</MenuItem>
                      <MenuItem value="O">O - Other</MenuItem>
                      <MenuItem value="P">P - Processed</MenuItem>
                      <MenuItem value="X">X - Cancelled</MenuItem>
                      <MenuItem value="Y">Y - Yes</MenuItem>
                      <MenuItem value="Z">Z - Zero</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <FormLabel>Tax Code *</FormLabel>
                <Controller
                  name="taxCode"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      value={field.value || ""}
                      select
                      fullWidth
                      size="small"
                      variant="outlined"
                      disabled={isLoadingTaxCodes}
                      error={!!errors.taxCode}
                      helperText={errors.taxCode?.message}>
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
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <FormLabel>Reason Code *</FormLabel>
                <Controller
                  name="reasonCode"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      value={field.value || ""}
                      select
                      fullWidth
                      size="small"
                      variant="outlined"
                      error={!!errors.reasonCode}
                      helperText={errors.reasonCode?.message}>
                      <MenuItem value="R">R - Rollover</MenuItem>
                      <MenuItem value="H">H - Hardship</MenuItem>
                      <MenuItem value="T">T - Termination</MenuItem>
                      <MenuItem value="D">D - Death</MenuItem>
                      <MenuItem value="O">O - Other</MenuItem>
                    </TextField>
                  )}
                />
              </Grid>

              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <FormLabel>Amount Requested *</FormLabel>
                <Controller
                  name="amountRequested"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      type="text"
                      inputMode="decimal"
                      placeholder="0.00"
                      error={!!errors.amountRequested}
                      helperText={errors.amountRequested?.message}
                      onBlur={() => {
                        field.onBlur();
                        calculateTaxes();
                      }}
                    />
                  )}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <Controller
                  name="fedTaxOverride"
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={
                        <Checkbox
                          checked={field.value}
                          onChange={field.onChange}
                        />
                      }
                      label="Fed Tax Override"
                      sx={{ mt: 3 }}
                    />
                  )}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <FormLabel>Fed Tax PCT</FormLabel>
                <Controller
                  name="fedTaxPct"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      disabled
                      sx={{
                        "& .MuiOutlinedInput-root": {
                          backgroundColor: "#f5f5f5"
                        }
                      }}
                    />
                  )}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <FormLabel>Fed Tax</FormLabel>
                <Controller
                  name="fedTax"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      disabled={!fedTaxOverride}
                      sx={
                        !fedTaxOverride
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
              </Grid>

              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <Controller
                  name="stateTaxOverride"
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={
                        <Checkbox
                          checked={field.value}
                          onChange={field.onChange}
                        />
                      }
                      label="State Tax Override"
                      sx={{ mt: 3 }}
                    />
                  )}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <FormLabel>State Tax PCT</FormLabel>
                <Controller
                  name="stateTaxPct"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      disabled
                      sx={{
                        "& .MuiOutlinedInput-root": {
                          backgroundColor: "#f5f5f5"
                        }
                      }}
                    />
                  )}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <FormLabel>State Tax</FormLabel>
                <Controller
                  name="stateTax"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      disabled={!stateTaxOverride}
                      sx={
                        !stateTaxOverride
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
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }} />

              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <FormLabel>Sequence Number</FormLabel>
                <Controller
                  name="sequenceNumber"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      disabled
                      sx={{
                        "& .MuiOutlinedInput-root": {
                          backgroundColor: "#f5f5f5"
                        }
                      }}
                    />
                  )}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 6 }}>
                <FormLabel>Memo</FormLabel>
                <Controller
                  name="memo"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      error={!!errors.memo}
                      helperText={errors.memo?.message}
                    />
                  )}
                />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <Controller
                  name="employeeDeceased"
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={
                        <Checkbox
                          checked={field.value}
                          onChange={field.onChange}
                        />
                      }
                      label="Employee Deceased"
                      sx={{ mt: 3 }}
                    />
                  )}
                />
              </Grid>
            </Grid>
          </Grid>

          {/* 3rd Party Details Section - Only show when Reason Code is "R - Rollover" */}
          {reasonCode === "R" && (
            <>
              <Grid size={{ xs: 12 }}>
                <Typography
                  variant="h2"
                  sx={{ color: "#0258A5", marginTop: "24px", marginBottom: "16px" }}>
                  3rd Party Details
                </Typography>
                <Divider />
              </Grid>

              {/* Name */}
              <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                <FormLabel>Name</FormLabel>
                <Controller
                  name="thirdPartyName"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      onBlur={() => {
                        field.onBlur();
                        if (field.value?.trim()) {
                          setThirdPartyTouched(true);
                        }
                      }}
                    />
                  )}
                />
              </Grid>

              {/* Address 1 */}
              <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                <FormLabel>Address 1</FormLabel>
                <Controller
                  name="thirdPartyAddress1"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      sx={
                        thirdPartyTouched && !field.value?.trim()
                          ? { "& .MuiOutlinedInput-root": { borderColor: "#d32f2f" } }
                          : undefined
                      }
                      error={thirdPartyTouched && !field.value?.trim()}
                    />
                  )}
                />
              </Grid>

              {/* Address 2 */}
              <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                <FormLabel>Address 2</FormLabel>
                <Controller
                  name="thirdPartyAddress2"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                    />
                  )}
                />
              </Grid>

              {/* City */}
              <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                <FormLabel>City</FormLabel>
                <Controller
                  name="thirdPartyCity"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      sx={
                        thirdPartyTouched && !field.value?.trim()
                          ? { "& .MuiOutlinedInput-root": { borderColor: "#d32f2f" } }
                          : undefined
                      }
                      error={thirdPartyTouched && !field.value?.trim()}
                    />
                  )}
                />
              </Grid>

              {/* State */}
              <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                <FormLabel>State</FormLabel>
                <Controller
                  name="thirdPartyState"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      sx={
                        thirdPartyTouched && !field.value?.trim()
                          ? { "& .MuiOutlinedInput-root": { borderColor: "#d32f2f" } }
                          : undefined
                      }
                      error={thirdPartyTouched && !field.value?.trim()}
                    />
                  )}
                />
              </Grid>

              {/* Zip */}
              <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                <FormLabel>Zip</FormLabel>
                <Controller
                  name="thirdPartyZip"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      sx={
                        thirdPartyTouched && !field.value?.trim()
                          ? { "& .MuiOutlinedInput-root": { borderColor: "#d32f2f" } }
                          : undefined
                      }
                      error={thirdPartyTouched && !field.value?.trim()}
                    />
                  )}
                />
              </Grid>

              {/* Check Payable To */}
              <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                <FormLabel>Check Payable To</FormLabel>
                <Controller
                  name="checkPayableTo"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                    />
                  )}
                />
              </Grid>

              {/* FBO Pay To */}
              <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                <FormLabel>FBO Pay To</FormLabel>
                <Controller
                  name="fboPayTo"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                    />
                  )}
                />
              </Grid>

              {/* FBO Type */}
              <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                <FormLabel>FBO Type</FormLabel>
                <Controller
                  name="fboType"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                    />
                  )}
                />
              </Grid>

              {/* Account */}
              <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                <FormLabel>Account</FormLabel>
                <Controller
                  name="account"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                    />
                  )}
                />
              </Grid>

              {/* Roth IRA */}
              <Grid
                size={{ xs: 12, sm: 6, md: 4 }}
                sx={{ display: "flex", alignItems: "center" }}>
                <Controller
                  name="rothIra"
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={
                        <Checkbox
                          checked={field.value}
                          onChange={field.onChange}
                        />
                      }
                      labelPlacement="start"
                      label="Roth IRA"
                    />
                  )}
                />
              </Grid>

              {/* SSN */}
              <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                <FormLabel>SSN</FormLabel>
                <Controller
                  name="thirdPartySsn"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      fullWidth
                      size="small"
                      variant="outlined"
                      placeholder="9 digits"
                      error={!!errors.thirdPartySsn}
                      helperText={errors.thirdPartySsn?.message}
                      onChange={(e) => {
                        const value = e.target.value;
                        // Only allow digits
                        if (value === "" || /^\d*$/.test(value)) {
                          // Limit to 9 digits
                          if (value.length <= 9) {
                            field.onChange(value);
                          }
                        }
                      }}
                    />
                  )}
                />
              </Grid>
            </>
          )}
        </Grid>
      </form>
    );
  }
);

AddDistributionForm.displayName = "AddDistributionForm";

export default AddDistributionForm;
