import { yupResolver } from "@hookform/resolvers/yup";
import { FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useLazyGetMilitaryAndRehireQuery, useLazyGetProfitMasterInquiryQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

interface SearchFormData {
  socialSecurity?: string;
  employeeNumber?: string;
  badgeNumber?: string;
}

const validationSchema = yup.object().shape({
  socialSecurity: yup.string().optional(),
  employeeNumber: yup.string().optional(),
  badgeNumber: yup.string().optional()
}).test('at-least-one-required', 'At least one field must be provided',
  (values) => Boolean(values.socialSecurity || values.employeeNumber || values.badgeNumber)
);

const MilitaryAndRehireEntryAndModificationSearchFilter = () => {
  const [isFetching, setIsFetching] = useState(false);
  const [triggerSearch, { isLoading }] = useLazyGetProfitMasterInquiryQuery();

  const { register, handleSubmit, reset, formState: { errors } } = useForm<SearchFormData>({
    resolver: yupResolver(validationSchema)
  });

  const onSubmit = (data: SearchFormData) => {
    setIsFetching(true);
    triggerSearch(
      {
        pagination: { skip: 0, take: 25 },
        ...(!!data.socialSecurity && { socialSecurity: Number(data.socialSecurity) }),
        ...(!!data.employeeNumber && { employeeNumber: data.employeeNumber }),
        ...(!!data.badgeNumber && { badgeNumber: data.badgeNumber }),
      },
      false
    );
    setIsFetching(false);
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
        <Grid2
          xs={12}
          sm={6}
          md={3}>
          <FormLabel>socialSecurity</FormLabel>
          <TextField
            fullWidth
            variant="outlined"
            {...register("socialSecurity")}
            error={!!errors.socialSecurity}
            helperText={errors.socialSecurity?.message}
          />
        </Grid2>
        <Grid2
          xs={12}
          sm={6}
          md={3}>
          <FormLabel>Employee Number</FormLabel>
          <TextField
            fullWidth
            variant="outlined"
            {...register("employeeNumber")}
            error={!!errors.employeeNumber}
            helperText={errors.employeeNumber?.message}
          />
        </Grid2>
        <Grid2
          xs={12}
          sm={6}
          md={3}>
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

export default MilitaryAndRehireEntryAndModificationSearchFilter;
