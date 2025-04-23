import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { useLazyGetForfeitureAdjustmentsQuery } from "reduxstore/api/YearsEndApi";
import { setForfeitureAdjustmentQueryParams, clearForfeitureAdjustmentData, clearForfeitureAdjustmentQueryParams } from "reduxstore/slices/forfeituresAdjustmentSlice";
import { RootState } from "reduxstore/store";

interface ForfeituresAdjustmentSearchParams {
  ssn?: string;
  badge?: string;
  year?: number;
  client?: string;
}

interface ForfeituresAdjustmentSearchParametersProps {
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const schema = yup.object().shape({
  // will make either ssn or badge required, depending on which one is provided
  ssn: yup.string().optional(),
  badge: yup.string().optional(),
  year: yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .optional(),
  client: yup.string().optional()
});

const ForfeituresAdjustmentSearchParameters: React.FC<ForfeituresAdjustmentSearchParametersProps> = ({
  setInitialSearchLoaded
}) => {
  const dispatch = useDispatch();
  const [triggerSearch, { isFetching }] = useLazyGetForfeitureAdjustmentsQuery();
  const { forfeitureAdjustmentQueryParams } = useSelector((state: RootState) => state.forfeituresAdjustment);
  const profitYear = useFiscalCloseProfitYear();

  const {
    control,
    handleSubmit,
    formState: { errors },
    reset
  } = useForm<ForfeituresAdjustmentSearchParams>({
    resolver: yupResolver(schema),
    defaultValues: {
      ssn: forfeitureAdjustmentQueryParams?.ssn || "",
      badge: forfeitureAdjustmentQueryParams?.badge || "",
      year: forfeitureAdjustmentQueryParams?.profitYear || profitYear,
      client: ""
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (!data.ssn && !data.badge) {
      return;
    }

    const searchParams = {
      ssn: data.ssn,
      badge: data.badge,
      profitYear: data.year || profitYear,
      skip: 0,
      take: 255,
      sortBy: "badgeNumber",
      isSortDescending: false
    };

    dispatch(setForfeitureAdjustmentQueryParams(searchParams));
    
    triggerSearch(searchParams)
      .unwrap()
      .then(() => {
        setInitialSearchLoaded(true);
      })
      .catch((error) => {
        console.error("Error fetching forfeiture adjustments:", error);
      });
  });

  const handleReset = () => {
    reset({
      ssn: "",
      badge: "",
      year: profitYear,
      client: ""
    });
    dispatch(clearForfeitureAdjustmentData());
    dispatch(clearForfeitureAdjustmentQueryParams());
    setInitialSearchLoaded(false);
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Grid2
        container
        paddingX="24px"
        gap="6px">
        <Grid2
          container
          spacing={3}
          width="100%">
          <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>SSN</FormLabel>
            <Controller
              name="ssn"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  variant="outlined"
                  error={!!errors.ssn}
                  placeholder="SSN"
                />
              )}
            />
            {errors.ssn && <FormHelperText error>{errors.ssn.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Badge</FormLabel>
            <Controller
              name="badge"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  variant="outlined"
                  error={!!errors.badge}
                  placeholder="Badge"
                />
              )}
            />
            {errors.badge && <FormHelperText error>{errors.badge.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
            <Controller
              name="year"
              control={control}
              render={({ field }) => (
                <DsmDatePicker
                  id="profitYear"
                  onChange={(value: Date | null) => field.onChange(value?.getFullYear() || undefined)}
                  value={field.value ? new Date(field.value, 0) : null}
                  required={true}
                  label="Year"
                  disableFuture
                  views={["year"]}
                  error={errors.year?.message}
                  disabled={true}
                />
              )}
            />
            {errors.year && <FormHelperText error>{errors.year.message}</FormHelperText>}
          </Grid2>

          <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Client</FormLabel>
            <Controller
              name="client"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  fullWidth
                  variant="outlined"
                  error={!!errors.client}
                  placeholder="Client"
                />
              )}
            />
            {errors.client && <FormHelperText error>{errors.client.message}</FormHelperText>}
          </Grid2>
        </Grid2>
      </Grid2>
      <Grid2
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isFetching}
        />
      </Grid2>
    </form>
  );
};

export default ForfeituresAdjustmentSearchParameters; 