import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, Grid, TextField } from "@mui/material";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { Controller, useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useLazyGetGrossWagesReportQuery } from "reduxstore/api/YearsEndApi";
import { setGrossWagesReportQueryParams } from "reduxstore/slices/yearsEndSlice";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import DsmDatePicker from "../../../components/DsmDatePicker/DsmDatePicker";
import { profitYearValidator } from "../../../utils/FormValidators";

interface GrossReportParams {
  profitYear: number;
  gross?: number;
}

const schema = yup.object().shape({
  profitYear: profitYearValidator(),
  gross: yup.number().optional()
});

interface ProfitShareGrossReportParametersProps {
  setPageReset: (reset: boolean) => void;
}

const ProfitShareGrossReportParameters: React.FC<ProfitShareGrossReportParametersProps> = ({ setPageReset }) => {
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch, { isFetching }] = useLazyGetGrossWagesReportQuery();
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<GrossReportParams>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: fiscalCloseProfitYear,
      gross: 50000
    }
  });

  const validateAndSubmit = handleSubmit((data) => {
    if (isValid) {
      setPageReset(true);
      triggerSearch(
        {
          profitYear: data.profitYear,
          minGrossAmount: data.gross,
          pagination: { skip: 0, take: 25 }
        },
        false
      ).unwrap();
      dispatch(
        setGrossWagesReportQueryParams({
          profitYear: data.profitYear,
          minGrossAmount: data.gross || 0
        })
      );
    }
  });

  const handleReset = () => {
    setPageReset(true);
    reset({
      profitYear: fiscalCloseProfitYear,
      gross: 50000
    });
  };

  return (
    <form onSubmit={validateAndSubmit}>
      <Grid
        container
        paddingX="24px"
        alignItems="flex-start"
        gap="24px">
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
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
              />
            )}
          />
          {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 2 }}>
          <FormLabel>Gross</FormLabel>
          <Controller
            name="gross"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                error={!!errors.gross}
                helperText={errors.gross?.message}
              />
            )}
          />
        </Grid>
      </Grid>

      <Grid
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSubmit}
          isFetching={isFetching}
          disabled={!isValid}
        />
      </Grid>
    </form>
  );
};

export default ProfitShareGrossReportParameters;
