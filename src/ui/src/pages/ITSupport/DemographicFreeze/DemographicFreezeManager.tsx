import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetHistoricalFrozenStateResponseQuery } from "reduxstore/api/FrozenApi";
import { RootState } from "reduxstore/store";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import DsmDatePicker from "../../../components/DsmDatePicker/DsmDatePicker";

interface DemographicFreezeSearch {
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

interface DemographicFreezeSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const DemographicFreezeManager: React.FC<DemographicFreezeSearchFilterProps> = ({
                                                                                  setInitialSearchLoaded
                                                                                }) => {
  const [triggerSearch, { isFetching }] = useLazyGetHistoricalFrozenStateResponseQuery();
  const profitYear = useDecemberFlowProfitYear();
  const dispatch = useDispatch();
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<DemographicFreezeSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: profitYear || undefined
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      triggerSearch({
        skip: 0,
        take: 25,
        sortBy: "createdDateTime",
        isSortDescending: false
      });
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);
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

export default DemographicFreezeManager;