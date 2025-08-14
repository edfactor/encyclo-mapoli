import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, MenuItem, Select, TextField } from "@mui/material";
import { MAX_EMPLOYEE_BADGE_LENGTH } from "../../constants";
import { Controller, Resolver, useForm } from "react-hook-form";
import { BeneficiarySearchFilterRequest, BeneficiaryTypeDto, MasterInquiryRequest } from "reduxstore/types";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

const schema = yup.object().shape({
  badgePsn: yup.string().notRequired(),
  name: yup.string().notRequired(),

  socialSecurity: yup
    .number()
    .typeError("SSN must be a number")
    .integer("SSN must be an integer")
    .min(0, "SSN must be positive")
    .max(999999999, "SSN must be 9 digits or less")
    .nullable(),
  memberType: yup.string().notRequired()
});
interface beneficiaryRequest {
  badgePsn?: string;
  name: string;
  socialSecurity: number;
  memberType: string;
}
// Define the type of props
type BeneficiaryInquirySearchFilterProps = {
  onSearch: (params: BeneficiarySearchFilterRequest | undefined) => void;
};

const BeneficiaryInquirySearchFilter: React.FC<BeneficiaryInquirySearchFilterProps> = ({
  onSearch
}) => {

  const {
    control,
    register,
    formState: { errors, isValid },
    setValue,
    handleSubmit,
    reset,
    setFocus,
    watch
  } = useForm<beneficiaryRequest>({
    resolver: yupResolver(schema) as Resolver<beneficiaryRequest>
  });

  const onSubmit = (data: any) => {
    let { badgePsn, name, ssn, memberType } = data;
    memberType = memberType ?? "2";
    let badge = undefined,
      psn = undefined;
    if (badgePsn && badgePsn.length > MAX_EMPLOYEE_BADGE_LENGTH) {
      if (badgePsn.length == MAX_EMPLOYEE_BADGE_LENGTH) {
        badge = parseInt(badgePsn);
      } else {
        badge = badgePsn ? parseInt(badgePsn.slice(0, -4)) : 0;
        psn = badgePsn ? parseInt(badgePsn.slice(-4)) : 0;
      }
    }
    if (isValid) {

      const beneficiarySearchFilterRequest: BeneficiarySearchFilterRequest = {
        badgeNumber: badge,
        psnSuffix: psn,
        memberType: Number(memberType),
        name: name,
        ssn: ssn,
        skip: data.pagination?.skip || 0,
        take: data.pagination?.take || 5,
        sortBy: data.pagination?.sortBy || "name",
        isSortDescending: data.pagination?.isSortDescending || true
      }
      onSearch(beneficiarySearchFilterRequest);
    }
  };
  const validateAndSubmit = handleSubmit(onSubmit);

  const handleReset = () => {
    reset({ badgePsn: undefined, name: undefined, socialSecurity: undefined });
  };

  return (
    <form onSubmit={validateAndSubmit}>
      <Grid
        container
        paddingX="24px">
        <Grid
          container
          spacing={2}
          width="100%">
          <Grid size={{ xs: 12, sm: 3, md: 3 }}>
            <FormLabel>Badge/Psn</FormLabel>
            <Controller
              name="badgePsn"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  size="small"
                  variant="outlined"
                  value={field.value ?? ""}
                  error={!!errors.badgePsn}
                  onChange={(e) => {
                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                    field.onChange(parsedValue);
                  }}
                />
              )}
            />
            {errors?.badgePsn && <FormHelperText error>{errors.badgePsn.message}</FormHelperText>}
          </Grid>

          <Grid size={{ xs: 12, sm: 3, md: 3 }}>
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
            {errors?.name && <FormHelperText error>{errors.name.message}</FormHelperText>}
          </Grid>
          <Grid size={{ xs: 12, sm: 3, md: 3 }}>
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
                    const parsedValue = e.target.value === "" ? null : Number(e.target.value);
                    field.onChange(parsedValue);
                  }}
                />
              )}
            />
            {errors?.socialSecurity && <FormHelperText error>{errors.socialSecurity.message}</FormHelperText>}
          </Grid>
          <Grid size={{ xs: 12, sm: 2, md: 2 }}>
            <FormLabel>Member Type</FormLabel>
            <Controller
              name="memberType"
              control={control}
              render={({ field }) => (
                <Select
                  {...field}
                  defaultValue="2"
                  fullWidth
                  size="small"
                  variant="outlined"
                  labelId="memberType"
                  id="memberType"
                  value={field.value}
                  label="Member Type"
                  onChange={(e) => field.onChange(e.target.value)}>
                  <MenuItem value="1">Employees</MenuItem>
                  <MenuItem value="2">Beneficiaries</MenuItem>
                </Select>
              )}
            />
          </Grid>
        </Grid>

        <Grid
          container
          justifyContent="flex-end"
          paddingY="16px">
          <Grid size={{ xs: 12 }}>
            <SearchAndReset
              handleReset={handleReset}
              handleSearch={validateAndSubmit}
              isFetching={false}
              disabled={!isValid}
            />
          </Grid>
        </Grid>
      </Grid>
    </form>
  );
};

export default BeneficiaryInquirySearchFilter;
