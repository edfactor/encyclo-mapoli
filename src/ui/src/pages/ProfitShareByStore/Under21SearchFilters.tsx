import { FormHelperText, FormControlLabel, Checkbox } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { useForm, Controller } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { useDispatch } from "react-redux";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { 
  setUnder21BreakdownByStoreQueryParams, 
  setUnder21InactiveQueryParams,
  setUnder21TotalsQueryParams 
} from "reduxstore/slices/yearsEndSlice";

interface Under21SearchParams {
  profitYear: number;
  isSortDescending: boolean;
}

interface Under21SearchFiltersProps {
  isLoading?: boolean;
  onSearch?: (profitYear: number, isSortDescending: boolean) => void;
}

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .required("Year is required"),
  isSortDescending: yup.boolean().required()
});

const Under21SearchFilters: React.FC<Under21SearchFiltersProps> = ({ isLoading = false, onSearch }) => {
  const dispatch = useDispatch();
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<Under21SearchParams>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: fiscalCloseProfitYear || 2024,
      isSortDescending: true
    }
  });

  const validateAndSubmit = handleSubmit((data) => {
    if (isValid) {
      const queryParams = {
        profitYear: data.profitYear,
        isSortDescending: data.isSortDescending,
        pagination: {
          take: 255,
          skip: 0
        }
      };
      
      dispatch(setUnder21BreakdownByStoreQueryParams(queryParams));
      dispatch(setUnder21InactiveQueryParams(queryParams));
      dispatch(setUnder21TotalsQueryParams(queryParams));
      
      if (onSearch) {
        onSearch(data.profitYear, data.isSortDescending);
      }
    }
  });

  const handleReset = () => {
    reset({
      profitYear: fiscalCloseProfitYear || 2024,
      isSortDescending: true
    });
  };

  return (
    <form onSubmit={validateAndSubmit}>
      <Grid2
        container
        paddingX="24px"
        alignItems="flex-end"
        spacing={2}>
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="profitYear"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="profitYear"
                onChange={(value: Date | null) => field.onChange(value?.getFullYear() || undefined)}
                value={field.value ? new Date(field.value, 0) : null}
                required={true}
                label="Profit Year"
                disableFuture
                views={["year"]}
                error={errors.profitYear?.message}
                disabled={true}
              />
            )}
          />
          {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
        </Grid2>

      </Grid2>

      <Grid2
        width="100%"
        paddingX="24px"
        marginTop={2}>
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSubmit}
          isFetching={isLoading}
          disabled={!isValid}
        />
      </Grid2>
    </form>
  );
};

export default Under21SearchFilters;