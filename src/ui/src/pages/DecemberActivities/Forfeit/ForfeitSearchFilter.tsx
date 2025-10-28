import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, TextField } from "@mui/material";
import { Controller, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import DuplicateSsnGuard from "../../../components/DuplicateSsnGuard";
import useFiscalCloseProfitYear from "../../../hooks/useFiscalCloseProfitYear";
import { profitYearValidator } from "../../../utils/FormValidators";
import { ForfeitSearchParams } from "./hooks/useForfeit";

interface ForfeitSearchParametersProps {
  onSearch: (params: ForfeitSearchParams) => void;
  onReset: () => void;
  isSearching: boolean;
}

const schema = yup.object().shape({
  profitYear: profitYearValidator()
});

const ForfeitSearchParameters: React.FC<ForfeitSearchParametersProps> = ({ onSearch, onReset, isSearching }) => {
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();

  const {
    control,
    handleSubmit,
    formState: { isValid, errors },
    reset
  } = useForm<ForfeitSearchParams>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: fiscalCloseProfitYear || undefined
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      onSearch({ profitYear: data.profitYear });
    }
  });

  const handleResetClick = () => {
    reset({
      profitYear: fiscalCloseProfitYear
    });
    onReset();
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
              handleReset={handleResetClick}
              handleSearch={validateAndSearch}
              isFetching={isSearching}
              disabled={!isValid || !prerequisitesComplete}
            />
          )}
        </DuplicateSsnGuard>
      </Grid>
    </form>
  );
};

export default ForfeitSearchParameters;
