import {
  Checkbox,
  FormControl,
  FormControlLabel,
  FormHelperText,
  FormLabel,
  MenuItem,
  Radio,
  RadioGroup,
  Select,
  TextField
} from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import React, { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useLazySearchProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { MasterInquiryRequest, MasterInquirySearch } from "reduxstore/types";
import {
  clearMasterInquiryData,
  clearMasterInquiryGroupingData,
  clearMasterInquiryRequestParams,
  setMasterInquiryRequestParams,
  updateMasterInquiryResults
} from "reduxstore/slices/inquirySlice";
import { useDispatch, useSelector } from "react-redux";
import { useParams } from "react-router-dom";
import { RootState } from "reduxstore/store";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { memberTypeGetNumberMap, paymentTypeGetNumberMap } from "./MasterInquiryFunctions";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";

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
  badgeNumber: yup.number().nullable(),
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
  setInitialSearchLoaded: (include: boolean) => void;
  onSearch: (params: MasterInquiryRequest | undefined) => void;
}

const MasterInquirySearchFilter: React.FC<MasterInquirySearchFilterProps> = ({
  setInitialSearchLoaded,
  onSearch
}) => {
  const [triggerSearch, { isFetching }] = useLazySearchProfitMasterInquiryQuery();
  const { masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);

  const dispatch = useDispatch();

  const { badgeNumber } = useParams<{
    badgeNumber: string;
  }>();

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useDecemberFlowProfitYear();

  const determineCorrectMemberType = (badgeNum: string | undefined) => {
    if (!badgeNum) return "all";
    if (badgeNum.length === 6) return "employees";
    if (badgeNum.length > 6) return "beneficiaries";
    return "all";
  };

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    trigger
  } = useForm<MasterInquirySearch>({
    resolver: yupResolver(schema) as any,
    defaultValues: {
      endProfitYear: profitYear, // Always use profitYear const as default
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
  
  

  useEffect(() => {
    if (badgeNumber && hasToken) {
      reset({
        ...schema.getDefault(),
        memberType: determineCorrectMemberType(badgeNumber),
        badgeNumber: Number(badgeNumber)
      });

      const searchParams: MasterInquiryRequest = {
        pagination: { skip: 0, take: 5, sortBy: "badgeNumber", isSortDescending: true },
        badgeNumber: Number(badgeNumber),
        memberType: memberTypeGetNumberMap[determineCorrectMemberType(badgeNumber)],
        endProfitYear: profitYear
      };

      triggerSearch(searchParams, false).unwrap().then((response) => {
        // If data is returned, trigger downstream components
        if (response && Array.isArray(response) ? response.length > 0 : response.results && response.results.length > 0) {
          setInitialSearchLoaded(true);
          onSearch(searchParams);
        } else {
          // Instead of setting missiveAlerts, pass up a signal (to be implemented)
          // setMissiveAlerts([...]);
          setInitialSearchLoaded(false);
          onSearch(undefined);
        }
      });
    }
  }, [badgeNumber, hasToken, reset, triggerSearch, profitYear]);

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      const searchParams: MasterInquiryRequest = {
        pagination: {
          skip: data.pagination?.skip || 0,
          take: data.pagination?.take || 5,
          sortBy: data.pagination?.sortBy || "badgeNumber",
          isSortDescending: data.pagination?.isSortDescending || true
        },
        ...(!!data.endProfitYear && { endProfitYear: data.endProfitYear || profitYear, }),
        ...(!!data.startProfitMonth && { startProfitMonth: data.startProfitMonth }),
        ...(!!data.endProfitMonth && { endProfitMonth: data.endProfitMonth }),
        ...(!!data.socialSecurity && { ssn: data.socialSecurity }),
        ...(!!data.name && { name: data.name }),
        ...(!!data.badgeNumber && { badgeNumber: data.badgeNumber }),
        ...(!!data.paymentType && { paymentType: paymentTypeGetNumberMap[data.paymentType] }),
        ...(!!data.memberType && { memberType: memberTypeGetNumberMap[data.memberType] }),
        ...(!!data.contribution && { contributionAmount: data.contribution }),
        ...(!!data.earnings && { earningsAmount: data.earnings }),
        ...(!!data.forfeiture && { forfeitureAmount: data.forfeiture }),
        ...(!!data.payment && { paymentAmount: data.payment })
      };

      triggerSearch(searchParams, false).unwrap().then((response) => {
        // If data is returned, trigger downstream components
        if (response && Array.isArray(response) ? response.length > 0 : response.results && response.results.length > 0) {
          setInitialSearchLoaded(true);
          onSearch(searchParams);
        } else {
          setInitialSearchLoaded(false);
          onSearch(undefined);
        }
      });
      dispatch(setMasterInquiryRequestParams(data));
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);
    dispatch(clearMasterInquiryRequestParams());
    dispatch(clearMasterInquiryData());
    dispatch(clearMasterInquiryGroupingData());
    reset({
      endProfitYear: profitYear, // Always reset to default profitYear
      startProfitMonth: undefined,
      endProfitMonth: undefined,
      socialSecurity: undefined,
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
    onSearch(undefined);
  };

  const months = Array.from({ length: 12 }, (_, i) => i + 1);

  const selectSx = {
    "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
      borderColor: "#0258A5"
    },
    "&:hover .MuiOutlinedInput-notchedOutline": {
      borderColor: "#0258A5"
    }
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
         
          <Grid2 size={{ xs: 12, sm: 6, md: 4 }}>
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
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel>Beginning Month</FormLabel>
            <Controller
              name="startProfitMonth"
              control={control}
              render={({ field }) => (
                <Select
                  {...field}
                  onChange={(e) => {
                    field.onChange(e.target.value === "" ? null : e.target.value);
                  }}
                  sx={selectSx}
                  fullWidth
                  size="small"
                  value={field.value ?? ""}
                  error={!!errors.startProfitMonth}>
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
            {errors.startProfitMonth && <FormHelperText error>{errors.startProfitMonth.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel>Ending Month</FormLabel>
            <Controller
              name="endProfitMonth"
              control={control}
              render={({ field }) => (
                <Select
                  {...field}
                  onChange={(e) => {
                    field.onChange(e.target.value === "" ? null : e.target.value);
                  }}
                  sx={selectSx}
                  fullWidth
                  size="small"
                  value={field.value ?? ""}
                  error={!!errors.endProfitMonth}>
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
            {errors.endProfitMonth && <FormHelperText error>{errors.endProfitMonth.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel>Social Security Number</FormLabel>
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
                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                    field.onChange(parsedValue);
                  }}
                />
              )}
            />
            {errors.socialSecurity && <FormHelperText error>{errors.socialSecurity.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel>Name</FormLabel>
            <Controller
              name="name"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.name}
                  onChange={(e) => {
                    field.onChange(e.target.value);
                  }}
                />
              )}
            />
            {errors.name && <FormHelperText error>{errors.name.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel>Badge/PSN Number</FormLabel>
            <Controller
              name="badgeNumber"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.badgeNumber}
                  onChange={(e) => {
                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                    field.onChange(parsedValue);
                  }}
                />
              )}
            />
            {errors.badgeNumber && <FormHelperText error>{errors.badgeNumber.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 6 }}>
            <FormControl error={!!errors.paymentType}>
              <FormLabel>Payment Type</FormLabel>
              <Controller
                name="paymentType"
                control={control}
                render={({ field }) => (
                  <RadioGroup
                    {...field}
                    row>
                    <FormControlLabel
                      value="all"
                      control={<Radio size="small" />}
                      label="All"
                    />
                    <FormControlLabel
                      value="hardship"
                      control={<Radio size="small" />}
                      label="Hardship/Dis"
                    />
                    <FormControlLabel
                      value="payoffs"
                      control={<Radio size="small" />}
                      label="Payoffs/Forfeit"
                    />
                    <FormControlLabel
                      value="rollovers"
                      control={<Radio size="small" />}
                      label="Rollovers"
                    />
                  </RadioGroup>
                )}
              />
            </FormControl>
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 6 }}>
            <FormControl error={!!errors.memberType}>
              <FormLabel>Member Type</FormLabel>
              <Controller
                name="memberType"
                control={control}
                render={({ field }) => (
                  <RadioGroup
                    {...field}
                    row>
                    <FormControlLabel
                      value="all"
                      control={<Radio size="small" />}
                      label="All"
                    />
                    <FormControlLabel
                      value="employees"
                      control={<Radio size="small" />}
                      label="Employees"
                    />
                    <FormControlLabel
                      value="beneficiaries"
                      control={<Radio size="small" />}
                      label="Beneficiaries"
                    />
                  </RadioGroup>
                )}
              />
            </FormControl>
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Contribution</FormLabel>
            <Controller
              name="contribution"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.contribution}
                  onChange={(e) => {
                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                    field.onChange(parsedValue);
                  }}
                />
              )}
            />
            {errors.contribution && <FormHelperText error>{errors.contribution.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Earnings</FormLabel>
            <Controller
              name="earnings"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.earnings}
                  onChange={(e) => {
                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                    field.onChange(parsedValue);
                  }}
                />
              )}
            />
            {errors.earnings && <FormHelperText error>{errors.earnings.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Forfeiture</FormLabel>
            <Controller
              name="forfeiture"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.forfeiture}
                  onChange={(e) => {
                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                    field.onChange(parsedValue);
                  }}
                />
              )}
            />
            {errors.forfeiture && <FormHelperText error>{errors.forfeiture.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <FormLabel>Payment</FormLabel>
            <Controller
              name="payment"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.payment}
                  onChange={(e) => {
                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                    field.onChange(parsedValue);
                  }}
                />
              )}
            />
            {errors.payment && <FormHelperText error>{errors.payment.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 4 }}>
            <Controller
              name="voids"
              control={control}
              render={({ field }) => (
                <FormControlLabel
                  control={
                    <Checkbox
                      {...field}
                      checked={field.value}
                      size="small"
                    />
                  }
                  label="Voids"
                  sx={{ 
                    marginTop: '20px', 
                    height: '40px',
                    display: 'flex', 
                    alignItems: 'center' 
                  }}
                />
              )}
            />
          </Grid2>
        </Grid2>

        <Grid2
          container
          justifyContent="flex-end"
          paddingY="16px">
          <Grid2 size={{ xs: 12 }}>
            <SearchAndReset
              handleReset={handleReset}
              handleSearch={validateAndSearch}
              isFetching={isFetching}
              disabled={!isValid}
            />
          </Grid2>
        </Grid2>
      </Grid2>
    </form>
  );
};

export default MasterInquirySearchFilter;