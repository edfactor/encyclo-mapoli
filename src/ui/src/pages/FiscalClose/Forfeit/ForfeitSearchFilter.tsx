import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import DuplicateSsnGuard from "../../../components/DuplicateSsnGuard";
import useFiscalCloseProfitYear from "../../../hooks/useFiscalCloseProfitYear";
import { VisuallyHidden } from "../../../utils/accessibilityHelpers";
import { generateFieldId, getAriaDescribedBy } from "../../../utils/accessibilityUtils";
import { profitYearValidator } from "../../../utils/FormValidators";
import { ARIA_DESCRIPTIONS, INPUT_PLACEHOLDERS } from "../../../utils/inputFormatters";
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
  const [isSubmitting, setIsSubmitting] = useState(false);

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

  useEffect(() => {
    if (!isSearching) {
      setIsSubmitting(false);
    }
  }, [isSearching]);

  const validateAndSearch = handleSubmit((data) => {
    if (isValid && !isSubmitting) {
      setIsSubmitting(true);
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
            <FormLabel htmlFor={generateFieldId("profitYear")}>Profit Year</FormLabel>
            <Controller
              name="profitYear"
              control={control}
              render={({ field }) => (
                <>
                  <TextField
                    {...field}
                    id={generateFieldId("profitYear")}
                    fullWidth
                    type="number"
                    variant="outlined"
                    placeholder={INPUT_PLACEHOLDERS.PROFIT_YEAR}
                    inputMode="numeric"
                    error={!!errors.profitYear}
                    aria-invalid={!!errors.profitYear}
                    aria-describedby={getAriaDescribedBy("profitYear", !!errors.profitYear, true)}
                    onChange={(e) => {
                      field.onChange(e);
                    }}
                    disabled={true}
                  />
                  <VisuallyHidden id={generateFieldId("profitYear-hint")}>
                    {ARIA_DESCRIPTIONS.PROFIT_YEAR}
                  </VisuallyHidden>
                </>
              )}
            />
            <div
              id={generateFieldId("profitYear-error")}
              aria-live="polite"
              aria-atomic="true">
              {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
            </div>
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
              isFetching={isSearching || isSubmitting}
              disabled={!isValid || !prerequisitesComplete || isSearching || isSubmitting}
            />
          )}
        </DuplicateSsnGuard>
      </Grid>
    </form>
  );
};

export default ForfeitSearchParameters;
