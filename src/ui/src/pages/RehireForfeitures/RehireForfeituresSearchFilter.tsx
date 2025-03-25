import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetMilitaryAndRehireForfeituresQuery } from "reduxstore/api/YearsEndApi";
import {
  clearMilitaryAndRehireForfeituresDetails,
  clearMilitaryAndRehireForfeituresQueryParams,
  setMilitaryAndRehireForfeituresQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";

interface MilitaryAndRehireForfeituresSearch {
  profitYear: number;
  reportingYear: string;
}

const digitsOnly: (value: string | undefined) => boolean = (value) => (value ? /^\d+$/.test(value) : false);

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Profit Year must be a number")
    .integer("Profit Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Profit Year must be 2100 or earlier")
    .required("Profit Year is required"),
  reportingYear: yup
    .string()
    .test("Digits only", "This field should have digits only", digitsOnly)
    .required("Reporting Year is required")
});

interface MilitaryAndRehireForfeituresSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const MilitaryAndRehireForfeituresSearchFilter: React.FC<MilitaryAndRehireForfeituresSearchFilterProps> = ({
  setInitialSearchLoaded
}) => {
  const [triggerSearch, { isFetching }] = useLazyGetMilitaryAndRehireForfeituresQuery();
  const { militaryAndRehireForfeituresQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const profitYear = useDecemberFlowProfitYear();
  const dispatch = useDispatch();
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<MilitaryAndRehireForfeituresSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: profitYear || militaryAndRehireForfeituresQueryParams?.profitYear || undefined,
      reportingYear: militaryAndRehireForfeituresQueryParams?.reportingYear || undefined
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      triggerSearch(
        {
          profitYear: profitYear,
          reportingYear: data.reportingYear,
          pagination: { skip: 0, take: 25 }
        },
        false
      ).unwrap();
      dispatch(setMilitaryAndRehireForfeituresQueryParams({
        profitYear: profitYear,
        reportingYear: data.reportingYear
      }));
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);
    dispatch(clearMilitaryAndRehireForfeituresQueryParams());
    dispatch(clearMilitaryAndRehireForfeituresDetails());
    reset({
      profitYear: profitYear,
      reportingYear: undefined
    });
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Grid2
        container
        paddingX="24px"
        gap="24px">
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>Profit Year</FormLabel>
          <Controller
            name="profitYear"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                variant="outlined"
                type="number"
                error={!!errors.profitYear}
                onChange={(e) => {
                  field.onChange(e);
                }}
                disabled={true}
              />
            )}
          />
          {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
        </Grid2>
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>Reporting Year</FormLabel>
          <Controller
            name="reportingYear"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                variant="outlined"
                error={!!errors.reportingYear}
                onChange={(e) => {
                  field.onChange(e);
                }}
                type="number"
              />
            )}
          />
          {errors.reportingYear && <FormHelperText error>{errors.reportingYear.message}</FormHelperText>}
        </Grid2>
      </Grid2>
      <Grid2
        width="100%"
        paddingX="24px">
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

export default MilitaryAndRehireForfeituresSearchFilter;
