import { yupResolver } from "@hookform/resolvers/yup";
import { FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useLazyGetProfitMasterInquiryQuery } from "reduxstore/api/InquiryApi";
import { clearMasterInquiryData } from "reduxstore/slices/inquirySlice";
import { clearMilitaryContributions } from "reduxstore/slices/militarySlice";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

interface SearchFormData {
  socialSecurity?: string;
  badgeNumber?: string;
}

interface SearchFilterProps {
  setInitialSearchLoaded: (loaded: boolean) => void;
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

const MilitaryEntryAndModificationSearchFilter: React.FC<SearchFilterProps> = ({ setInitialSearchLoaded }) => {
  const [triggerSearch, { isFetching }] = useLazyGetProfitMasterInquiryQuery();
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm<SearchFormData>({
    resolver: yupResolver(validationSchema)
  });
  const dispatch = useDispatch();
  const onSubmit = (data: SearchFormData) => {
    triggerSearch(
      {
        pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: false },
        ...(!!data.socialSecurity && { socialSecurity: Number(data.socialSecurity) }),
        ...(!!data.badgeNumber && { badgeNumber: Number(data.badgeNumber) })
      },
      false
    ).then(() => {
      setInitialSearchLoaded(true); // Set to true after successful search
    });
  };

  const handleReset = () => {
    reset();
    dispatch(clearMasterInquiryData());
    dispatch(clearMilitaryContributions());
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
            required={true}
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
            required={true}
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
