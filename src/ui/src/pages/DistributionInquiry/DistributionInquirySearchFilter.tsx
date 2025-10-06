import { yupResolver } from "@hookform/resolvers/yup";
import { FormLabel, Grid, MenuItem, TextField } from "@mui/material";
import { Controller, Resolver, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { DistributionSearchFormData } from "../../types";

interface DistributionInquirySearchFilterProps {
  onSearch: (data: DistributionSearchFormData) => void;
  onReset: () => void;
  isLoading: boolean;
}

const schema = yup.object().shape({
  ssnOrMemberNumber: yup
    .string()
    .nullable()
    .test("valid-id", "Invalid id number", function (value) {
      if (!value || value.trim() === "") return true;
      return /^\d{5,11}$/.test(value.trim());
    }),
  frequency: yup.string().nullable(),
  paymentFlag: yup.string().nullable(),
  taxCode: yup.string().nullable(),
  minGrossAmount: yup
    .string()
    .nullable()
    .test("is-number", "Must be a valid number", function (value) {
      if (!value) return true;
      return !isNaN(Number(value));
    }),
  maxGrossAmount: yup
    .string()
    .nullable()
    .test("is-number", "Must be a valid number", function (value) {
      if (!value) return true;
      return !isNaN(Number(value));
    })
    .test("greater-than-min", "Max must be greater than Min", function (value) {
      const { minGrossAmount } = this.parent;
      if (!value || !minGrossAmount) return true;
      return Number(value) >= Number(minGrossAmount);
    }),
  minCheckAmount: yup
    .string()
    .nullable()
    .test("is-number", "Must be a valid number", function (value) {
      if (!value) return true;
      return !isNaN(Number(value));
    }),
  maxCheckAmount: yup
    .string()
    .nullable()
    .test("is-number", "Must be a valid number", function (value) {
      if (!value) return true;
      return !isNaN(Number(value));
    })
    .test("greater-than-min", "Max must be greater than Min", function (value) {
      const { minCheckAmount } = this.parent;
      if (!value || !minCheckAmount) return true;
      return Number(value) >= Number(minCheckAmount);
    })
});

const DistributionInquirySearchFilter: React.FC<DistributionInquirySearchFilterProps> = ({
  onSearch,
  onReset,
  isLoading
}) => {
  const {
    control,
    handleSubmit,
    reset,
    formState: { errors, isValid }
  } = useForm<DistributionSearchFormData>({
    resolver: yupResolver(schema) as Resolver<DistributionSearchFormData>,
    mode: "onChange",
    defaultValues: {
      ssnOrMemberNumber: "",
      frequency: null,
      paymentFlag: null,
      taxCode: null,
      minGrossAmount: "",
      maxGrossAmount: "",
      minCheckAmount: "",
      maxCheckAmount: ""
    }
  });

  const handleFormSubmit = (data: DistributionSearchFormData) => {
    onSearch(data);
  };

  const handleFormReset = () => {
    reset({
      ssnOrMemberNumber: "",
      frequency: null,
      paymentFlag: null,
      taxCode: null,
      minGrossAmount: "",
      maxGrossAmount: "",
      minCheckAmount: "",
      maxCheckAmount: ""
    });
    onReset();
  };

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)}>
      <Grid container paddingX="24px" gap="24px">
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>SSN/Member #</FormLabel>
          <Controller
            name="ssnOrMemberNumber"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                value={field.value ?? ""}
                fullWidth
                variant="outlined"
                placeholder="Enter SSN or Member #"
                error={!!errors.ssnOrMemberNumber}
                helperText={errors.ssnOrMemberNumber?.message}
              />
            )}
          />
        </Grid>

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

        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>Payment Flag</FormLabel>
          <Controller
            name="paymentFlag"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                value={field.value ?? ""}
                select
                fullWidth
                variant="outlined"
                error={!!errors.paymentFlag}
                helperText={errors.paymentFlag?.message}>
                <MenuItem value="">All</MenuItem>
                <MenuItem value="Y">Yes</MenuItem>
                <MenuItem value="C">Check</MenuItem>
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
                variant="outlined"
                error={!!errors.taxCode}
                helperText={errors.taxCode?.message}>
                <MenuItem value="">All</MenuItem>
                <MenuItem value="1">1</MenuItem>
                <MenuItem value="3">3</MenuItem>
                <MenuItem value="7">7</MenuItem>
              </TextField>
            )}
          />
        </Grid>

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

      <Grid width="100%" paddingX="24px">
        <SearchAndReset
          handleReset={handleFormReset}
          handleSearch={handleSubmit(handleFormSubmit)}
          isFetching={isLoading}
          disabled={!isValid}
        />
      </Grid>
    </form>
  );
};

export default DistributionInquirySearchFilter;
