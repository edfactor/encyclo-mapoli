import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, TextField } from "@mui/material";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import DuplicateSsnGuard from "../../../components/DuplicateSsnGuard";
import useFiscalCloseProfitYear from "../../../hooks/useFiscalCloseProfitYear";
import { useLazyGetForfeituresAndPointsQuery } from "../../../reduxstore/api/YearsEndApi";
import {
  clearForfeituresAndPoints,
  clearForfeituresAndPointsQueryParams,
  setForfeituresAndPointsQueryParams
} from "../../../reduxstore/slices/yearsEndSlice";
import { RootState } from "../../../reduxstore/store";
import { profitYearValidator } from "../../../utils/FormValidators";

interface ForfeitSearchParams {
  profitYear: number;
}

interface ForfeitSearchParametersProps {
  setInitialSearchLoaded: (loaded: boolean) => void;
  setPageReset: (reset: boolean) => void;
  onSearchClicked?: () => void;
}

const schema = yup.object().shape({
  profitYear: profitYearValidator()
});

const ForfeitSearchParameters: React.FC<ForfeitSearchParametersProps> = ({
  setInitialSearchLoaded,
  setPageReset,
  onSearchClicked
}) => {
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
      profitYear: fiscalCloseProfitYear || forfeituresAndPointsQueryParams?.profitYear || undefined
    }
  });

  const validateAndSearch = handleSubmit((_data) => {
    if (isValid) {
      // Call the parent callback to potentially change status
      if (onSearchClicked) {
        onSearchClicked();
      }

      setPageReset(true);
      triggerSearch(
        {
          profitYear: fiscalCloseProfitYear,
          useFrozenData: true,
          archive: false, // Always use archive=false for search button
          pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
        },
        false
      );
      dispatch(
        setForfeituresAndPointsQueryParams({
          profitYear: fiscalCloseProfitYear,
          useFrozenData: true
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
      profitYear: fiscalCloseProfitYear
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
      </Grid>
      <Grid
        width="100%"
        paddingX="24px">
        <DuplicateSsnGuard>
          {({ prerequisitesComplete }) => (
            <SearchAndReset
              handleReset={handleReset}
              handleSearch={validateAndSearch}
              isFetching={isFetching}
              disabled={!isValid || !prerequisitesComplete}
            />
          )}
        </DuplicateSsnGuard>
      </Grid>
    </form>
  );
};

export default ForfeitSearchParameters;
