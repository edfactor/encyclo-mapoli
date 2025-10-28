import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, Grid } from "@mui/material";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { Controller, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { profitYearValidator } from "../../../utils/FormValidators";

interface ProfitYearSearch {
  profitYear: number;
}

const schema = yup.object().shape({
  profitYear: profitYearValidator()
});

interface ProfitYearSearchFilterProps {
  onSearch: (data: ProfitYearSearch) => void;
  onReset: () => void;
  setPageReset: (reset: boolean) => void;
}

const PayMasterUpdateSearchFilters: React.FC<ProfitYearSearchFilterProps> = ({ onSearch, onReset, setPageReset }) => {
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<ProfitYearSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: fiscalCloseProfitYear
    }
  });

  const validateAndSubmit = handleSubmit((data) => {
    if (isValid) {
      setPageReset(true);
      onSearch(data);
    }
  });

  const handleReset = () => {
    setPageReset(true);
    reset({
      profitYear: fiscalCloseProfitYear
    });
    onReset();
  };

  return (
    <form onSubmit={validateAndSubmit}>
      <Grid
        container
        paddingX="24px"
        alignItems="flex-end"
        gap="24px">
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="profitYear"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="profitYear"
                onChange={(value: Date | null) => field.onChange(value?.getFullYear() || null)}
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
        </Grid>
      </Grid>

      <Grid width="100%" paddingX="24px">
        <SearchAndReset handleReset={handleReset} handleSearch={validateAndSubmit} disabled={!isValid} />
      </Grid>
    </form>
  );
};

export default PayMasterUpdateSearchFilters;
