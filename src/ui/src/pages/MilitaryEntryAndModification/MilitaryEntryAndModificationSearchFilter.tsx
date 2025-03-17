import { yupResolver } from "@hookform/resolvers/yup";
import { FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useLazyGetProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

interface SearchFormData {
  socialSecurity?: string;
  badgeNumber?: string;
}

const validationSchema = yup
  .object()
  .shape({
    socialSecurity: yup.string().optional(),
    badgeNumber: yup.string().optional()
  })
  .test("at-least-one-required", "At least one field must be provided", (values) =>
    Boolean(values.socialSecurity || values.badgeNumber)
  );

const MilitaryEntryAndModificationSearchFilter = () => {
  const [triggerSearch, { isFetching }] = useLazyGetProfitMasterInquiryQuery();
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm<SearchFormData>({
    resolver: yupResolver(validationSchema)
  });
  const onSubmit = (data: SearchFormData) => {
    triggerSearch(
      {
        pagination: { skip: 0, take: 25, sortBy: "profitYear", isSortDescending: false },
        ...(!!data.socialSecurity && { socialSecurity: Number(data.socialSecurity) }),
        ...(!!data.badgeNumber && { badgeNumber: Number(data.badgeNumber) })
      },
      false
    );
  };
  const handleReset = () => {
    reset();
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Grid2
        container
        paddingX="24px"
        gap="24px">
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>SSN</FormLabel>
          <TextField
            fullWidth
            variant="outlined"
            {...register("socialSecurity")}
            error={!!errors.socialSecurity}
            helperText={errors.socialSecurity?.message}
          />
        </Grid2>
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>Badge Number</FormLabel>
          <TextField
            fullWidth
            variant="outlined"
            {...register("badgeNumber")}
            error={!!errors.badgeNumber}
            helperText={errors.badgeNumber?.message}
          />
        </Grid2>
      </Grid2>
      <Grid2
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={handleSubmit(onSubmit)}
          isFetching={isFetching}
        />
      </Grid2>
    </form>
  );
};

export default MilitaryEntryAndModificationSearchFilter;
