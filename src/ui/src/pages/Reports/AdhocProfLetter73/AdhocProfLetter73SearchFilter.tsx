import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, TextField } from "@mui/material";
import React, { useEffect, useState } from "react";
import { Controller, Resolver, useForm, useWatch } from "react-hook-form";
import { DSMDatePicker, SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { useFakeTimeAwareDate } from "../../../hooks/useFakeTimeAwareDate";
import { VisuallyHidden } from "../../../utils/accessibilityHelpers";
import { generateFieldId, getAriaDescribedBy } from "../../../utils/accessibilityUtils";
import { ARIA_DESCRIPTIONS, INPUT_PLACEHOLDERS } from "../../../utils/inputFormatters";

export interface AdhocProfLetter73FilterParams {
  profitYear?: Date | null;
  DeMinimusValue?: number | null;
}

const schema = yup.object().shape({
  profitYear: yup.date().nullable().required("Profit Year is required"),
  DeMinimusValue: yup.number().nullable().min(0, "Must be a positive number")
});

interface AdhocProfLetter73FilterSectionProps {
  onSearch: (params: AdhocProfLetter73FilterParams) => void;
  onReset: () => void;
  isLoading?: boolean;
}

const AdhocProfLetter73FilterSection: React.FC<AdhocProfLetter73FilterSectionProps> = ({
  onSearch,
  onReset,
  isLoading = false
}) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const currentDate = useFakeTimeAwareDate();
  const currentYear = currentDate.getFullYear();

  const defaultProfitYear = new Date(currentYear - 1, 0, 1);

  const {
    control,
    handleSubmit,
    formState: { errors },
    reset,
    trigger
  } = useForm<AdhocProfLetter73FilterParams>({
    resolver: yupResolver(schema) as Resolver<AdhocProfLetter73FilterParams>,
    defaultValues: {
      profitYear: defaultProfitYear,
      DeMinimusValue: 1000
    }
  });

  useEffect(() => {
    if (!isLoading) {
      setIsSubmitting(false);
    }
  }, [isLoading]);

  const validateAndSubmit = handleSubmit((data) => {
    if (!isSubmitting) {
      setIsSubmitting(true);
      const dataCopy: AdhocProfLetter73FilterParams = {
        profitYear: data.profitYear ? new Date(data.profitYear.getTime()) : null,
        DeMinimusValue: data.DeMinimusValue
      };

      console.log("Filter params being sent:", dataCopy);
      onSearch(dataCopy);
    }
  });

  const watchedValues = useWatch({ control });
  const isSearchEnabled = watchedValues.profitYear && !errors.profitYear;

  const handleReset = () => {
    reset({
      profitYear: new Date(currentYear - 1, 0, 1),
      DeMinimusValue: 1000
    });
    onReset();
  };

  return (
    <form onSubmit={validateAndSubmit}>
      <Grid
        container
        paddingX="24px">
        <Grid
          container
          spacing={3}
          width="100%">
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Controller
              name="profitYear"
              control={control}
              render={({ field }) => (
                <DSMDatePicker
                  id="profitYear"
                  onChange={(value: Date | null) => {
                    field.onChange(value);
                    trigger("profitYear");
                  }}
                  value={field.value || null}
                  required={true}
                  label="Profit Year"
                  views={["year"]}
                  disableFuture
                  error={errors.profitYear?.message}
                />
              )}
            />
            {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <Controller
              name="DeMinimusValue"
              control={control}
              render={({ field }) => (
                <>
                  <FormLabel htmlFor={generateFieldId("DeMinimusValue")}>De Minimus Value</FormLabel>
                  <TextField
                    {...field}
                    id={generateFieldId("DeMinimusValue")}
                    fullWidth
                    type="number"
                    variant="outlined"
                    size="small"
                    placeholder={INPUT_PLACEHOLDERS.CURRENCY_WITH_SYMBOL}
                    inputMode="decimal"
                    value={field.value ?? ""}
                    error={!!errors.DeMinimusValue}
                    aria-invalid={!!errors.DeMinimusValue}
                    aria-describedby={getAriaDescribedBy("DeMinimusValue", !!errors.DeMinimusValue, true)}
                    onChange={(e) => {
                      const value = e.target.value === "" ? null : parseFloat(e.target.value);
                      field.onChange(value);
                      trigger("DeMinimusValue");
                    }}
                    inputProps={{
                      step: 0.01,
                      min: 0
                    }}
                  />
                  <VisuallyHidden id={generateFieldId("DeMinimusValue-hint")}>
                    {ARIA_DESCRIPTIONS.CURRENCY_FORMAT}
                  </VisuallyHidden>
                  <div
                    id={generateFieldId("DeMinimusValue-error")}
                    aria-live="polite"
                    aria-atomic="true">
                    {errors.DeMinimusValue && <FormHelperText error>{errors.DeMinimusValue.message}</FormHelperText>}
                  </div>
                </>
              )}
            />
          </Grid>
        </Grid>
      </Grid>

      <Grid
        width="100%"
        paddingX="24px"
        paddingY="16px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSubmit}
          isFetching={isLoading || isSubmitting}
          disabled={!isSearchEnabled || isLoading || isSubmitting}
        />
      </Grid>
    </form>
  );
};

export default AdhocProfLetter73FilterSection;
