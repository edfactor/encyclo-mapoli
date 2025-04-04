import { FormHelperText } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Controller, useForm } from "react-hook-form";
import { useLazyGetVestingAmountByAgeQuery } from "reduxstore/api/YearsEndApi";
import { SearchAndReset } from "smart-ui-library";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import {
  clearVestedAmountsByAge,
  clearVestedAmountsByAgeQueryParams,
  setVestedAmountsByAgeQueryParams
} from "reduxstore/slices/yearsEndSlice";
import DsmDatePicker from "../../../components/DsmDatePicker/DsmDatePicker";
import { useEffect } from "react";

interface VestingAmountByAgeSearch {
  profitYear: number;
}

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .required("Year is required")
});

const VestedAmountsByAgeSearchFilter: React.FC = () => {
  const [triggerSearch, { isFetching }] = useLazyGetVestingAmountByAgeQuery();
  const { vestedAmountsByAgeQueryParams, vestedAmountsByAge } = useSelector((state: RootState) => state.yearsEnd);
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<VestingAmountByAgeSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: fiscalCloseProfitYear || vestedAmountsByAgeQueryParams?.profitYear || undefined
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      triggerSearch(
        {
          profitYear: fiscalCloseProfitYear,
          acceptHeader: "application/json"
        },
        false
      ).unwrap();
      dispatch(setVestedAmountsByAgeQueryParams(fiscalCloseProfitYear));
    }
  });

  useEffect(() => {
    if (fiscalCloseProfitYear && !vestedAmountsByAge) {
      triggerSearch(
        {
          profitYear: fiscalCloseProfitYear,
          acceptHeader: "application/json"
        },
        false
      ).unwrap();
    }
  }, [fiscalCloseProfitYear, triggerSearch, vestedAmountsByAge]);

  const handleReset = () => {
    dispatch(clearVestedAmountsByAgeQueryParams());
    dispatch(clearVestedAmountsByAge());
    reset({
      profitYear: fiscalCloseProfitYear
    });
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Grid2
        container
        paddingX="24px"
        gap="24px">
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
        display="flex"
        gap="16px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isFetching}
          disabled={!isValid}
        />
      </Grid2>
    </form>
  );
};

export default VestedAmountsByAgeSearchFilter;
