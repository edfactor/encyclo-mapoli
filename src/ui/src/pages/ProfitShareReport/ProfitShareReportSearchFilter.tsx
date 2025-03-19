import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import {
  clearYearEndProfitSharingReport,
  clearYearEndProfitSharingReportQueryParams,
  setYearEndProfitSharingReportQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { YearEndProfitSharingReportRequest } from "reduxstore/types";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import DsmDatePicker from "../../components/DsmDatePicker/DsmDatePicker";

interface ProfitShareReportSearch {
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

interface ProfitShareReportSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const ProfitShareReportSearchFilter: React.FC<ProfitShareReportSearchFilterProps> = ({ setInitialSearchLoaded }) => {
  const [triggerSearch, { isFetching }] = useLazyGetYearEndProfitSharingReportQuery();
  const { yearEndProfitSharingReportQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const profitYear = useDecemberFlowProfitYear();
  const dispatch = useDispatch();
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<ProfitShareReportSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: profitYear || yearEndProfitSharingReportQueryParams?.profitYear || undefined
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      const req: YearEndProfitSharingReportRequest = {
        isYearEnd: false,
        minimumAgeInclusive: 18,
        maximumAgeInclusive: 98,
        minimumHoursInclusive: 1000,
        maximumHoursInclusive: 2000,
        includeActiveEmployees: true,
        includeInactiveEmployees: true,
        includeEmployeesWithPriorProfitSharingAmounts: false,
        includeEmployeesWithNoPriorProfitSharingAmounts: false,
        includeEmployeesTerminatedThisYear: false,
        includeTerminatedEmployees: false,
        includeBeneficiaries: false,
        profitYear: data.profitYear,
        pagination: { skip: 0, take: 25 }
      };
      triggerSearch(req, false).unwrap();
      dispatch(setYearEndProfitSharingReportQueryParams(data.profitYear));
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);
    dispatch(clearYearEndProfitSharingReport());
    reset({
      profitYear: undefined
    });
    dispatch(clearYearEndProfitSharingReportQueryParams());
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
          isFetching={false}
          disabled={!isValid}
        />
      </Grid2>
    </form>
  );
};

export default ProfitShareReportSearchFilter;
