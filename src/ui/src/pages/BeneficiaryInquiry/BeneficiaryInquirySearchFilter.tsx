import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, MenuItem, Select, TextField } from "@mui/material";
import { Paged } from "components/DSMGrid/types";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { memberTypeGetNumberMap } from "pages/MasterInquiry/MasterInquiryFunctions";
import { Controller, Resolver, useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useLazyBeneficiarySearchFilterQuery } from "reduxstore/api/BeneficiariesApi";
import { useLazySearchProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { setMasterInquiryRequestParams } from "reduxstore/slices/inquirySlice";
import { BeneficiarySearchFilterRequest, BeneficiarySearchFilterResponse, BeneficiaryTypeDto, MasterInquiryRequest } from "reduxstore/types";
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
interface bRequest {
  badgePsn?: string;
  name: string;
  socialSecurity: number;
  memberType: string;
}
// Define the type of props
type Props = {
  beneficiaryType: BeneficiaryTypeDto[];
  searchClicked: (badgeNumber: number) => void;
  setInitialSearchLoaded: (include: boolean) => void;
  onSearch: (request: BeneficiarySearchFilterRequest | undefined) => void;
};

const BeneficiaryInquirySearchFilter: React.FC<Props> = ({
  searchClicked,
  beneficiaryType,
  setInitialSearchLoaded,
  onSearch
}) => {
  //const [triggerSearch, { data, isLoading, isError, isFetching }] = useLazyGetBeneficiariesQuery();
  const [triggerSearch, { isFetching }] = useLazyBeneficiarySearchFilterQuery();
  const profitYear = useDecemberFlowProfitYear();
  const dispatch = useDispatch();

  const {
    control,
    register,
    formState: { errors, isValid },
    setValue,
    handleSubmit,
    reset,
    setFocus,
    watch
  } = useForm<bRequest>({
    resolver: yupResolver(schema) as Resolver<bRequest>
  });

  const onSubmit = (data: any) => {
    let { badgePsn, name, ssn, memberType } = data;
    memberType = memberType ?? "2";
    let badge = undefined,
      psn = undefined;
    if (badgePsn && badgePsn.length > 0) {
      if (badgePsn.length == 6) {
        badge = parseInt(badgePsn);
      } else {
        badge = badgePsn ? parseInt(badgePsn.slice(0, -4)) : 0;
        psn = badgePsn ? parseInt(badgePsn.slice(-4)) : 0;
      }
    }

    //searchClicked(badge);
    if (isValid) {
      const beneficiaryRequestDto: BeneficiarySearchFilterRequest = {
          badgeNumber: badge ?? undefined,
          psnSuffix: psn ?? undefined,
          name: name,
          ssn: ssn,
          memberType: Number(memberType),
          skip: 0,
          take: 255,
          isSortDescending: true,
          sortBy: "name"
      };
      onSearch(beneficiaryRequestDto);
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
                  label="Member Type"
                  value={field.value}
                  onChange={(e) => field.onChange(Number(e.target.value))}>
                  <MenuItem value={"1"}>Employees</MenuItem>
                  <MenuItem value={"2"}>Beneficiaries</MenuItem>
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
              isFetching={isFetching}
              disabled={!isValid}
            />
          </Grid>
        </Grid>
      </Grid>
    </form>
  );
};

export default BeneficiaryInquirySearchFilter;
