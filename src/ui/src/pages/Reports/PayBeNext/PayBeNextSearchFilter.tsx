import { yupResolver } from "@hookform/resolvers/yup";
import { Checkbox, FormHelperText, FormLabel, Grid, SelectChangeEvent } from "@mui/material";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import ProfitYearSelector from "../../../components/ProfitYearSelector/ProfitYearSelector";
import { PayBeNextFormData } from "./hooks/usePayBeNextReducer";

/**
 * Validation schema for PayBeNext search form
 */
const schema = yup.object().shape({
  profitYear: yup.number().required("Profit Year is required").min(2000).max(2100),
  isAlsoEmployee: yup.boolean().required().default(true)
});

/**
 * Props for PayBeNextSearchFilter component
 */
interface PayBeNextSearchFilterProps {
  onSearch: (data: PayBeNextFormData) => void;
  onReset: () => void;
  isFetching?: boolean;
  initialValues?: PayBeNextFormData;
}

/**
 * Search filter component for PayBeNext report
 * Extracts the form concerns from the main component
 */
const PayBeNextSearchFilter: React.FC<PayBeNextSearchFilterProps> = ({
  onSearch,
  onReset,
  isFetching = false,
  initialValues
}) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    setValue
  } = useForm<PayBeNextFormData>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: initialValues?.profitYear ?? fiscalCloseProfitYear,
      isAlsoEmployee: initialValues?.isAlsoEmployee ?? true
    }
  });

  // Clear submitting state when fetching completes
  useEffect(() => {
    if (!isFetching) {
      setIsSubmitting(false);
    }
  }, [isFetching]);

  /**
   * Handle profit year change from ProfitYearSelector
   */
  const handleProfitYearChange = (event: SelectChangeEvent) => {
    const year = parseInt(event.target.value, 10);
    setValue("profitYear", year);
  };

  /**
   * Validate and submit the form
   */
  const validateAndSubmit = handleSubmit((data) => {
    if (isValid && !isSubmitting) {
      setIsSubmitting(true);
      onSearch(data);
    }
  });

  /**
   * Reset form to initial values
   */
  const handleReset = () => {
    reset({
      profitYear: fiscalCloseProfitYear,
      isAlsoEmployee: true
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
        <Grid size={{ xs: 12, sm: 4, md: 3 }}>
          <Controller
            name="profitYear"
            control={control}
            render={({ field }) => (
              <ProfitYearSelector
                selectedProfitYear={field.value}
                handleChange={handleProfitYearChange}
                showDates={false}
              />
            )}
          />
          {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
        </Grid>

        <Grid size={{ xs: 12, sm: 4, md: 3 }}>
          <div className="flex h-full items-center">
            <Controller
              name="isAlsoEmployee"
              control={control}
              render={({ field }) => (
                <div className="flex items-center">
                  <FormLabel sx={{ marginRight: "8px" }}>Is Also Employee</FormLabel>
                  <Checkbox
                    {...field}
                    size="small"
                    checked={!!field.value}
                    onChange={(e) => field.onChange(e.target.checked)}
                  />
                </div>
              )}
            />
          </div>
        </Grid>
      </Grid>

      <Grid
        width="100%"
        paddingX="24px"
        paddingY="16px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSubmit}
          disabled={!isValid || isFetching || isSubmitting}
          isFetching={isFetching || isSubmitting}
        />
      </Grid>
    </form>
  );
};

export default PayBeNextSearchFilter;
