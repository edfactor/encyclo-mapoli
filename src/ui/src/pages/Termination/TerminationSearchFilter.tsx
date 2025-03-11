import { yupResolver } from "@hookform/resolvers/yup";
import Grid2 from '@mui/material/Grid2';
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetTerminationReportQuery } from "reduxstore/api/YearsEndApi";
import {
  clearTermination,
  clearTerminationQueryParams,
  setTerminationQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";

interface TerminationSearch {
  profitYear: Date;
}

const schema = yup.object().shape({
  profitYear: yup
    .date()
    .required("Year is required")
    .min(new Date(2020, 0, 1), "Year must be 2020 or later")
    .max(new Date(2100, 11, 31), "Year must be 2100 or earlier")
    .typeError("Invalid date")
});

interface TerminationSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const TerminationSearchFilter: React.FC<TerminationSearchFilterProps> = ({ setInitialSearchLoaded }) => {
  const { terminationQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetTerminationReportQuery();
  const dispatch = useDispatch();
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<TerminationSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: terminationQueryParams?.profitYear ? new Date(terminationQueryParams.profitYear, 0, 1) : undefined
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      triggerSearch(
        {
          profitYear: data.profitYear.getFullYear(),
          pagination: { skip: 0, take: 25 }
        },
        false
      ).unwrap();
      dispatch(setTerminationQueryParams(data.profitYear.getFullYear()));
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);
    dispatch(clearTerminationQueryParams());
    dispatch(clearTermination());
    reset({
      profitYear: undefined
    });
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Grid2
        container
        paddingX="24px"
        gap="24px">
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }} >
          <Controller
            name="profitYear"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="profitYear"
                onChange={(e: Date | null) => {
                  field.onChange(e);
                }}
                value={field.value ?? null}
                required={true}
                label="Profit Year"
                disableFuture
                views={["year"]}
                error={errors.profitYear?.message}
              />
            )}
          />
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

export default TerminationSearchFilter;
