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
import Grid2 from "@mui/material/Unstable_Grid2";
import { useEffect } from "react";
import { useForm, Controller } from "react-hook-form";
import { useLazyGetProfitMasterInquiryQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { MasterInquiryRequest } from "reduxstore/types";
import {
  clearMasterInquiryData,
  clearMasterInquiryRequestParams,
  setMasterInquiryRequestParams
} from "reduxstore/slices/yearsEndSlice";
import { useDispatch, useSelector } from "react-redux";
import { useParams } from "react-router-dom";
import { RootState } from "reduxstore/store";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { paymentTypeMap, memberTypeMap } from "./MasterInquiryFunctions";

interface MasterInquirySearch {
  startProfitYear?: Date | null;
  endProfitYear?: Date | null;
  startProfitMonth?: number | null;
  endProfitMonth?: number | null;
  socialSecurity?: number | null;
  name?: string | null;
  badgeNumber?: number | null;
  comment?: string | null;
  paymentType: "all" | "hardship" | "payoffs" | "rollovers";
  memberType: "all" | "employees" | "beneficiaries" | "none";
  contribution?: number | null;
  earnings?: number | null;
  forfeiture?: number | null;
  payment?: number | null;
  voids: boolean;
}

const schema = yup.object().shape({
  startProfitYear: yup
    .date()
    .min(new Date(2020, 0, 1), "Year must be 2020 or later")
    .max(new Date(2100, 11, 31), "Year must be 2100 or earlier")
    .typeError("Invalid date")
    .nullable(),
  endProfitYear: yup
    .date()
    .min(new Date(2020, 0, 1), "Year must be 2020 or later")
    .max(new Date(2100, 11, 31), "Year must be 2100 or earlier")
    .typeError("Invalid date")
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
  setStartProfitYear: (year: Date) => void;
  setEndProfitYear: (year: Date) => void;
  setStartProfitMonth: (month: number | null) => void;
  setEndProfitMonth: (month: number | null) => void;
  setSocialSecurity: (ssn: number | null) => void;
  setName: (name: string | null) => void;
  setBadgeNumber: (badgeNumber: number | null) => void;
  setComment: (comment: string | null) => void;
  setPaymentType: (paymentType: "all" | "hardship" | "payoffs" | "rollovers") => void;
  setMemberType: (memberType: "all" | "employees" | "beneficiaries" | "none") => void;
  setContribution: (contribution: number | null) => void;
  setEarnings: (earnings: number | null) => void;
  setForfeiture: (forfeiture: number | null) => void;
  setPayment: (payment: number | null) => void;
  //setVoids: (voids: boolean) => void;
  setInitialSearchLoaded: (include: boolean) => void;
}

const MasterInquirySearchFilter: React.FC<MasterInquirySearchFilterProps> = ({
  setStartProfitYear,
  setEndProfitYear,
  setStartProfitMonth,
  setEndProfitMonth,
  setSocialSecurity,
  setName,
  setBadgeNumber,
  setComment,
  setPaymentType,
  setMemberType,
  setContribution,
  setEarnings,
  setForfeiture,
  setPayment,
  //setVoids,
  setInitialSearchLoaded
}) => {
  const [triggerSearch, { isFetching }] = useLazyGetProfitMasterInquiryQuery();
  const { masterInquiryRequestParams } = useSelector((state: RootState) => state.yearsEnd);

  const dispatch = useDispatch();

  const { badgeNumber } = useParams<{
    badgeNumber: string;
  }>();

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    trigger
  } = useForm<MasterInquirySearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      startProfitYear: masterInquiryRequestParams?.startProfitYear
        ? new Date(masterInquiryRequestParams.startProfitYear, 0, 1)
        : undefined,
      endProfitYear: masterInquiryRequestParams?.endProfitYear
        ? new Date(masterInquiryRequestParams.endProfitYear, 0, 1)
        : undefined,
      startProfitMonth: masterInquiryRequestParams?.startProfitMonth || undefined,
      endProfitMonth: masterInquiryRequestParams?.endProfitMonth || undefined,
      socialSecurity: masterInquiryRequestParams?.socialSecurity || undefined,
      name: masterInquiryRequestParams?.name || undefined,
      badgeNumber: masterInquiryRequestParams?.badgeNumber || undefined,
      comment: masterInquiryRequestParams?.comment || undefined,
      paymentType: masterInquiryRequestParams?.paymentType
        ? (Object.keys(paymentTypeMap).find(
            (key) => paymentTypeMap[key] === masterInquiryRequestParams?.paymentType
          ) as "all" | "hardship" | "payoffs" | "rollovers" | undefined)
        : "all",
      memberType: masterInquiryRequestParams?.memberType
        ? (Object.keys(memberTypeMap).find((key) => memberTypeMap[key] === masterInquiryRequestParams?.memberType) as
            | "all"
            | "employees"
            | "beneficiaries"
            | "none"
            | undefined)
        : "all",
      contribution: masterInquiryRequestParams?.contributionAmount || undefined,
      earnings: masterInquiryRequestParams?.earningsAmount || undefined,
      forfeiture: masterInquiryRequestParams?.forfeitureAmount || undefined,
      payment: masterInquiryRequestParams?.paymentAmount || undefined,
      voids: false // masterInquiryRequestParams?.voids false
    }
  });

  useEffect(() => {
    if (badgeNumber && hasToken) {
      reset({
        ...schema.getDefault(),
        badgeNumber: Number(badgeNumber)
      });

      // Trigger search automatically when badge number is present
      const searchParams: MasterInquiryRequest = {
        pagination: { skip: 0, take: 25 },
        badgeNumber: Number(badgeNumber)
      };

      triggerSearch(searchParams, false);
    }
  }, [badgeNumber, hasToken, reset, triggerSearch]);

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      const searchParams: MasterInquiryRequest = {
        pagination: { skip: 0, take: 25 },
        ...(!!data.startProfitYear && { startProfitYear: data.startProfitYear.getFullYear() }),
        ...(!!data.endProfitYear && { endProfitYear: data.endProfitYear.getFullYear() }),
        ...(!!data.startProfitMonth && { startProfitMonth: data.startProfitMonth }),
        ...(!!data.endProfitMonth && { endProfitMonth: data.endProfitMonth }),
        ...(!!data.socialSecurity && { socialSecurity: data.socialSecurity }),
        ...(!!data.name && { name: data.name }),
        ...(!!data.badgeNumber && { badgeNumber: data.badgeNumber }),
        ...(!!data.comment && { comment: data.comment }),
        ...(!!data.paymentType && { paymentType: paymentTypeMap[data.paymentType] }),
        ...(!!data.memberType && { memberType: memberTypeMap[data.memberType] }),
        ...(!!data.contribution && { contribution: data.contribution }),
        ...(!!data.earnings && { earnings: data.earnings }),
        ...(!!data.forfeiture && { forfeiture: data.forfeiture }),
        ...(!!data.payment && { payment: data.payment })
        //...(!!data.voids && { voids: data.voids })
      };

      triggerSearch(searchParams, false).unwrap();
      dispatch(setMasterInquiryRequestParams(data));
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);
    reset({
      startProfitYear: undefined,
      endProfitYear: undefined,
      startProfitMonth: undefined,
      endProfitMonth: undefined,
      socialSecurity: undefined,
      name: undefined,
      badgeNumber: undefined,
      comment: undefined,
      paymentType: "all",
      memberType: "all",
      contribution: undefined,
      earnings: undefined,
      forfeiture: undefined,
      payment: undefined,
      voids: false
    });
    dispatch(clearMasterInquiryRequestParams());
    dispatch(clearMasterInquiryData());
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
          <Grid2
            xs={12}
            sm={6}
            md={3}>
            <Controller
              name="startProfitYear"
              control={control}
              render={({ field }) => (
                <DsmDatePicker
                  id="Beginning Year"
                  onChange={(value: Date | null) => {
                    if (value) {
                      field.onChange(value);
                      setStartProfitYear(value);
                    }
                  }}
                  value={field.value ?? null}
                  required={true}
                  label="Profit Year"
                  disableFuture
                  views={["year"]}
                  error={errors.startProfitYear?.message}
                />
              )}
            />
            {errors.startProfitYear && <FormHelperText error>{errors.startProfitYear.message}</FormHelperText>}
          </Grid2>

          <Grid2
            xs={12}
            sm={6}
            md={3}>
            <Controller
              name="endProfitYear"
              control={control}
              render={({ field }) => (
                <DsmDatePicker
                  id="End Year"
                  onChange={(value: Date | null) => {
                    if (value) {
                      field.onChange(value);
                      setEndProfitYear(value);
                    }
                  }}
                  value={field.value ?? null}
                  required={true}
                  label="End Year"
                  disableFuture
                  views={["year"]}
                  error={errors.startProfitYear?.message}
                />
              )}
            />
            {errors.endProfitYear && <FormHelperText error>{errors.endProfitYear.message}</FormHelperText>}
          </Grid2>

          <Grid2
            xs={12}
            sm={6}
            md={3}>
            <FormLabel>Beginning Month</FormLabel>
            <Controller
              name="startProfitMonth"
              control={control}
              render={({ field }) => (
                <Select
                  {...field}
                  onChange={(e) => {
                    field.onChange(e.target.value === "" ? null : e.target.value);
                    setStartProfitMonth(e.target.value === "" ? null : Number(e.target.value));
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

          <Grid2
            xs={12}
            sm={6}
            md={3}>
            <FormLabel>Ending Month</FormLabel>
            <Controller
              name="endProfitMonth"
              control={control}
              render={({ field }) => (
                <Select
                  {...field}
                  onChange={(e) => {
                    field.onChange(e.target.value === "" ? null : e.target.value);
                    setEndProfitMonth(e.target.value === "" ? null : Number(e.target.value));
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

          <Grid2
            xs={12}
            sm={6}
            md={3}>
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
                    setSocialSecurity(parsedValue);
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
                    setName(e.target.value);
                  }}
                />
              )}
            />
            {errors.name && <FormHelperText error>{errors.name.message}</FormHelperText>}
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
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.badgeNumber}
                  onChange={(e) => {
                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                    field.onChange(parsedValue);
                    setBadgeNumber(parsedValue);
                  }}
                />
              )}
            />
            {errors.badgeNumber && <FormHelperText error>{errors.badgeNumber.message}</FormHelperText>}
          </Grid2>

          <Grid2
            xs={12}
            sm={6}
            md={3}>
            <FormLabel>Comment</FormLabel>
            <Controller
              name="comment"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.comment}
                  onChange={(e) => {
                    field.onChange(e.target.value);
                    setComment(e.target.value);
                  }}
                />
              )}
            />
            {errors.comment && <FormHelperText error>{errors.comment.message}</FormHelperText>}
          </Grid2>

          <Grid2
            xs={12}
            sm={6}>
            <FormControl error={!!errors.paymentType}>
              <FormLabel>Payment Type</FormLabel>
              <Controller
                name="paymentType"
                control={control}
                render={({ field }) => (
                  <RadioGroup
                    {...field}
                    onChange={(e) => {
                      // We need to translate from the string value to the number value

                      setPaymentType(e.target.value as "all" | "hardship" | "payoffs" | "rollovers");
                    }}
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

          <Grid2
            xs={12}
            sm={6}>
            <FormControl error={!!errors.memberType}>
              <FormLabel>Member Type</FormLabel>
              <Controller
                name="memberType"
                control={control}
                render={({ field }) => (
                  <RadioGroup
                    {...field}
                    onChange={(e) => {
                      // We need to translate from the string value to the number value

                      setMemberType(e.target.value as "all" | "employees" | "beneficiaries" | "none");
                    }}
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
                    <FormControlLabel
                      value="none"
                      control={<Radio size="small" />}
                      label="None"
                    />
                  </RadioGroup>
                )}
              />
            </FormControl>
          </Grid2>

          <Grid2
            xs={12}
            sm={6}
            md={3}>
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
                    setContribution(parsedValue);
                  }}
                />
              )}
            />
            {errors.contribution && <FormHelperText error>{errors.contribution.message}</FormHelperText>}
          </Grid2>

          <Grid2
            xs={12}
            sm={6}
            md={3}>
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
                    setEarnings(parsedValue);
                  }}
                />
              )}
            />
            {errors.earnings && <FormHelperText error>{errors.earnings.message}</FormHelperText>}
          </Grid2>

          <Grid2
            xs={12}
            sm={6}
            md={3}>
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
                    setForfeiture(parsedValue);
                  }}
                />
              )}
            />
            {errors.forfeiture && <FormHelperText error>{errors.forfeiture.message}</FormHelperText>}
          </Grid2>

          <Grid2
            xs={12}
            sm={6}
            md={3}>
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
                    setPayment(parsedValue);
                  }}
                />
              )}
            />
            {errors.payment && <FormHelperText error>{errors.payment.message}</FormHelperText>}
          </Grid2>

          <Grid2 xs={12}>
            <FormControlLabel
              control={
                <Controller
                  name="voids"
                  control={control}
                  render={({ field }) => (
                    <Checkbox
                      {...field}
                      size="small"
                      checked={field.value}
                      onChange={(e) => {
                        field.onChange(e.target.checked);
                        setVoids(e.target.checked);
                      }}
                    />
                  )}
                />
              }
              label="Voids"
            />
          </Grid2>
        </Grid2>

        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isFetching}
          disabled={!isValid}
        />
      </Grid2>
    </form>
  );
};

export default MasterInquirySearchFilter;
