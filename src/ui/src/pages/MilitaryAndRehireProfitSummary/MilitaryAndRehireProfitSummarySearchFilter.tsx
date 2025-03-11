import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetMilitaryAndRehireProfitSummaryQuery } from "reduxstore/api/YearsEndApi";
import {
  clearMilitaryAndRehireProfitSummaryDetails,
  clearMilitaryAndRehireProfitSummaryQueryParams,
  setMilitaryAndRehireProfitSummaryQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

const digitsOnly: (value: string | undefined) => boolean = (value) => (value ? /^\d+$/.test(value) : false);

interface MilitaryAndRehireProfitSummarySearch {
  profitYear: number;
  reportingYear: string;
}

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .required("Year is required"),
  reportingYear: yup
    .string()
    .test("Digits only", "This field should have digits only", digitsOnly)
    .required("Reporting Year is required")
});

interface MilitaryAndRehireProfitSummarySearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const MilitaryAndRehireProfitSummarySearchFilter: React.FC<MilitaryAndRehireProfitSummarySearchFilterProps> = ({
  setInitialSearchLoaded
}) => {
  const [triggerSearch, { isFetching }] = useLazyGetMilitaryAndRehireProfitSummaryQuery();
  const { militaryAndRehireProfitSummaryQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const dispatch = useDispatch();
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<MilitaryAndRehireProfitSummarySearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: militaryAndRehireProfitSummaryQueryParams?.profitYear || undefined,
      reportingYear: militaryAndRehireProfitSummaryQueryParams?.reportingYear || undefined
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      triggerSearch(
        {
          reportingYear: data.reportingYear,
          profitYear: data.profitYear,
          pagination: { skip: 0, take: 25 }
        },
        false
      ).unwrap();
      dispatch(setMilitaryAndRehireProfitSummaryQueryParams(data));
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);
    dispatch(clearMilitaryAndRehireProfitSummaryDetails());

    reset({
      profitYear: undefined,
      reportingYear: undefined
    });
    dispatch(clearMilitaryAndRehireProfitSummaryQueryParams());
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Grid2
        container
        paddingX="24px"
        gap="24px">
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }} >
          <FormLabel>Profit Year</FormLabel>
          <Controller
            name="profitYear"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                variant="outlined"
                error={!!errors.profitYear}
                onChange={(e) => {
                  field.onChange(e);
                }}
                inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
              />
            )}
          />
          {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
        </Grid2>
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }} >
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
                inputProps={{ inputMode: "numeric", pattern: "[0-9]*" }}
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

export default MilitaryAndRehireProfitSummarySearchFilter;
