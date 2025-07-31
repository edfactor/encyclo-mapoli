import { yupResolver } from "@hookform/resolvers/yup";
import { Checkbox, FormHelperText, FormLabel, TextField } from "@mui/material";
import { Grid } from "@mui/material";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetForfeituresAndPointsQuery } from "reduxstore/api/YearsEndApi";
import {
  clearForfeituresAndPoints,
  clearForfeituresAndPointsQueryParams,
  setForfeituresAndPointsQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";

interface ForfeitSearchParams {
  profitYear: number;
  useFrozenData: boolean;
}

interface ForfeitSearchParametersProps {
  setInitialSearchLoaded: (loaded: boolean) => void;
  setPageReset: (reset: boolean) => void;
}

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .required("Year is required"),
  useFrozenData: yup.boolean().default(true).required()
});

const ForfeitSearchParameters: React.FC<ForfeitSearchParametersProps> = ({ setInitialSearchLoaded, setPageReset }) => {
  const [triggerSearch, { isFetching }] = useLazyGetForfeituresAndPointsQuery();
  const { forfeituresAndPointsQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();

  const {
    control,
    handleSubmit,
    formState: { isValid, errors },
    reset
  } = useForm<ForfeitSearchParams>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: fiscalCloseProfitYear || forfeituresAndPointsQueryParams?.profitYear || undefined,
      useFrozenData: forfeituresAndPointsQueryParams?.useFrozenData || true
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      setPageReset(true);
      triggerSearch(
        {
          profitYear: fiscalCloseProfitYear,
          useFrozenData: data.useFrozenData,
          pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
        },
        false
      );
      dispatch(
        setForfeituresAndPointsQueryParams({
          profitYear: fiscalCloseProfitYear,
          useFrozenData: data.useFrozenData
        })
      );
      setInitialSearchLoaded(true);
    }
  });

  const handleReset = () => {
    setPageReset(true);
    dispatch(clearForfeituresAndPoints());
    dispatch(clearForfeituresAndPointsQueryParams());
    reset({
      profitYear: fiscalCloseProfitYear,
      useFrozenData: true
    });
  };

  return (
    <form>
      <Grid
        container
        paddingX="24px"
        gap="6px">
        <Grid
          container
          spacing={3}
          width="100%">
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Profit Year</FormLabel>
            <Controller
              name="profitYear"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  type="number"
                  variant="outlined"
                  error={!!errors.profitYear}
                  onChange={(e) => {
                    field.onChange(e);
                  }}
                  disabled={true}
                />
              )}
            />
            {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
          </Grid>
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <FormLabel>Use Frozen Demographic Data</FormLabel>
          <Controller
            name="useFrozenData"
            control={control}
            render={({ field }) => (
              <Checkbox
                checked={field.value}
                onChange={(e) => {
                  field.onChange(e.target.checked);
                }}
              />
            )}
          />
        </Grid>
      </Grid>
      <Grid
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isFetching}
          disabled={!isValid}
        />
      </Grid>
    </form>
  );
};

export default ForfeitSearchParameters;
