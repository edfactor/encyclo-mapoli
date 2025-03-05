import { yupResolver } from "@hookform/resolvers/yup";
import { Checkbox, FormControlLabel, FormHelperText } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { Controller, useForm } from "react-hook-form";
import { useLazyGetDistributionsAndForfeituresQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

interface DistributionsAndForfeituresSearch {
  cutoffDate: Date;
  useDemographics: boolean;
  createSummaryReport: boolean;
}

const schema = yup.object().shape({
  cutoffDate: yup.date().required(),
  useDemographics: yup.boolean().default(false).required(),
  createSummaryReport: yup.boolean().default(false).required()
});

const ProfitShareReportEditRunParameters = () => {
  const [triggerSearch, { isFetching }] = useLazyGetDistributionsAndForfeituresQuery();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<DistributionsAndForfeituresSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      cutoffDate: undefined,
      useDemographics: false,
      createSummaryReport: false
    }
  });

  const validateAndSubmit = handleSubmit((data) => {
    if (isValid) {
      // TODO - call the API with the data to generate reports
    }
  });

  const handleReset = () => {
    reset({
      cutoffDate: undefined,
      useDemographics: false,
      createSummaryReport: false
    });
  };

  return (
    <form onSubmit={validateAndSubmit}>
      <Grid2
        container
        paddingX="24px"
        alignItems={"flex-end"}
        gap="24px">
        <Grid2
          xs={12}
          sm={6}
          md={3}>
          <Controller
            name="cutoffDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="cutoffDate"
                onChange={(value: Date | null) => field.onChange(value)}
                value={field.value ?? null}
                required={true}
                label="Cutoff Date"
                disableFuture
                error={errors.cutoffDate?.message}
              />
            )}
          />
          {errors.cutoffDate && <FormHelperText error>{errors.cutoffDate.message}</FormHelperText>}
        </Grid2>

        <Grid2
          xs={12}
          sm={6}
          md={2}>
          <FormControlLabel
            control={
              <Controller
                name="useDemographics"
                control={control}
                render={({ field }) => (
                  <Checkbox
                    checked={field.value}
                    onChange={field.onChange}
                  />
                )}
              />
            }
            label="Use Demographics"
          />
          {errors.useDemographics && <FormHelperText error>{errors.useDemographics.message}</FormHelperText>}
        </Grid2>
        <Grid2
          xs={12}
          sm={6}
          md={3}>
          <FormControlLabel
            control={
              <Controller
                name="createSummaryReport"
                control={control}
                render={({ field }) => (
                  <Checkbox
                    checked={field.value}
                    onChange={field.onChange}
                  />
                )}
              />
            }
            label="Create Summary Report"
          />
          {errors.createSummaryReport && <FormHelperText error>{errors.createSummaryReport.message}</FormHelperText>}
        </Grid2>
      </Grid2>
      <Grid2
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSubmit}
          isFetching={isFetching}
          disabled={!isValid}
        />
      </Grid2>
    </form>
  );
};

export default ProfitShareReportEditRunParameters;
