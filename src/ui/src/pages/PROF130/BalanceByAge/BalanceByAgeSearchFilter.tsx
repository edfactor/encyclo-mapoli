import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetBalanceByAgeQuery } from "reduxstore/api/YearsEndApi";
import {
  clearBalanceByAge,
  clearBalanceByAgeQueryParams,
  setBalanceByAgeQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { FrozenReportsByAgeRequestType } from "reduxstore/types";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import DsmDatePicker from "../../../components/DsmDatePicker/DsmDatePicker";
import { useEffect } from "react";

interface BalanceByAgeSearch {
  profitYear: number;
  reportType?: FrozenReportsByAgeRequestType;
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

interface BalanceByAgeSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const BalanceByAgeSearchFilter: React.FC<BalanceByAgeSearchFilterProps> = ({ setInitialSearchLoaded }) => {
  const [triggerSearch, { isFetching }] = useLazyGetBalanceByAgeQuery();
  const { balanceByAgeQueryParams, balanceByAgeFullTime } = useSelector((state: RootState) => state.yearsEnd);
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<BalanceByAgeSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: fiscalCloseProfitYear || balanceByAgeQueryParams?.profitYear || undefined,
      reportType: undefined
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      // It is necessary to clear for the case where the user clicks
      // search a second time after getting one round of results
      setInitialSearchLoaded(false);
      dispatch(clearBalanceByAgeQueryParams());
      dispatch(clearBalanceByAge());
      triggerSearch(
        {
          profitYear: fiscalCloseProfitYear,
          reportType: FrozenReportsByAgeRequestType.Total,
          pagination: { skip: 0, take: 255 }
        },
        false
      ).unwrap();
      triggerSearch(
        {
          profitYear: fiscalCloseProfitYear,
          reportType: FrozenReportsByAgeRequestType.FullTime,
          pagination: { skip: 0, take: 255 }
        },
        false
      ).unwrap();
      triggerSearch(
        {
          profitYear: fiscalCloseProfitYear,
          reportType: FrozenReportsByAgeRequestType.PartTime,
          pagination: { skip: 0, take: 255 }
        },
        false
      ).unwrap();
      dispatch(setBalanceByAgeQueryParams(fiscalCloseProfitYear));
    }
  });
  useEffect(() => {
    if (fiscalCloseProfitYear && !balanceByAgeFullTime) {
      setInitialSearchLoaded(true);
    }
  }, [fiscalCloseProfitYear, balanceByAgeFullTime, setInitialSearchLoaded]);

  const handleReset = () => {
    setInitialSearchLoaded(false);
    dispatch(clearBalanceByAgeQueryParams());
    dispatch(clearBalanceByAge());
    reset({
      profitYear: fiscalCloseProfitYear,
      reportType: undefined
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
        paddingX="24px">
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

export default BalanceByAgeSearchFilter;
