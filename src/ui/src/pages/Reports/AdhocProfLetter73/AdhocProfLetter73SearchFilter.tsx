import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, Grid } from "@mui/material";
import React, { useEffect, useState } from "react";
import { Controller, Resolver, useForm, useWatch } from "react-hook-form";
import { DSMDatePicker, SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

export interface AdhocProfLetter73FilterParams {
  profitYear?: Date | null;
}

const schema = yup.object().shape({
  profitYear: yup.date().nullable().required("Profit Year is required")
});

interface AdhocProfLetter73FilterSectionProps {
  onSearch: (params: AdhocProfLetter73FilterParams) => void;
  onReset: () => void;
  isLoading?: boolean;
}

const AdhocProfLetter73FilterSection: React.FC<AdhocProfLetter73FilterSectionProps> = ({ onSearch, onReset, isLoading = false }) => {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const {
    control,
    handleSubmit,
    formState: { errors },
    reset,
    trigger
  } = useForm<AdhocProfLetter73FilterParams>({
    resolver: yupResolver(schema) as Resolver<AdhocProfLetter73FilterParams>,
    defaultValues: {
      profitYear: new Date(new Date().getFullYear() - 1, 0, 1)
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
        profitYear: data.profitYear ? new Date(data.profitYear.getTime()) : null
      };

      onSearch(dataCopy);
    }
  });

  const watchedValues = useWatch({ control });
  const isSearchEnabled = watchedValues.profitYear && !errors.profitYear;

  const handleReset = () => {
    reset({
      profitYear: new Date(new Date().getFullYear() - 1, 0, 1)
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
