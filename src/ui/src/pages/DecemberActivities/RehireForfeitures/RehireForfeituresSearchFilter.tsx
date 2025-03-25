import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetRehireForfeituresQuery } from "reduxstore/api/YearsEndApi";
import {
  clearRehireForfeituresDetails,
  clearRehireForfeituresQueryParams,
  setMilitaryAndRehireForfeituresQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import DsmDatePicker from "../../../components/DsmDatePicker/DsmDatePicker";
import { ProfitYearRequest, SortedPaginationRequestDto } from "../../../reduxstore/types";

interface RehireForfeituresSearch extends SortedPaginationRequestDto, ProfitYearRequest {
  beginningDate: Date;
  endingDate: Date;
}

const digitsOnly: (value: string | undefined) => boolean = (value) => (value ? /^\d+$/.test(value) : false);

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Profit Year must be a number")
    .integer("Profit Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Profit Year must be 2100 or earlier")
    .required("Profit Year is required"),
  reportingYear: yup
    .string()
    .test("Digits only", "This field should have digits only", digitsOnly)
    .required("Reporting Year is required")
});

interface MilitaryAndRehireForfeituresSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const RehireForfeituresSearchFilter: React.FC<MilitaryAndRehireForfeituresSearchFilterProps> = ({
  setInitialSearchLoaded
}) => {
  const [triggerSearch, { isFetching }] = useLazyGetRehireForfeituresQuery();
  const { rehireForfeituresQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const profitYear = new Date(useDecemberFlowProfitYear(), 1, 1);
  const dispatch = useDispatch();
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<RehireForfeituresSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: profitYear || rehireForfeituresQueryParams?.profitYear || undefined,
      beginningDate: rehireForfeituresQueryParams?.beginningDate || undefined,
      endingDate: rehireForfeituresQueryParams?.endingDate || undefined
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      triggerSearch(
        {
          profitYear: profitYear,
          beginningDate: data.beginningDate,
          endingDate : data.endingDate,
          pagination: { skip: 0, take: 25, sortBy: "profitYear", isSortDescending: true },
        },
        false
      ).unwrap();
      dispatch(setMilitaryAndRehireForfeituresQueryParams({
        profitYear: profitYear,
        reportingYear: data.endingDate
      }));
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);
    dispatch(clearRehireForfeituresQueryParams());
    dispatch(clearRehireForfeituresDetails());
    reset({
      profitYear: profitYear,
      beginningDate: undefined,
      endingDate: undefined
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
                onChange={(value: Date | null) => field.onChange(value?.getDate() || undefined)}
                value={field.value ? new Date(field.value) : null}
                required={true}
                label="Beginning Date"
                disableFuture
                error={errors.beginningDate?.message}                
              />
            )}
          />
          {errors.beginningDate && <FormHelperText error>{errors.beginningDate.message}</FormHelperText>}
        </Grid2>
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="endingDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="endingDate"
                onChange={(value: Date | null) => field.onChange(value?.getDate() || undefined)}
                value={field.value ? new Date(field.value) : null}
                required={true}
                label="Ening Date"
                disableFuture
                error={errors.endingDate?.message}
              />
            )}
          />
          {errors.endingDate && <FormHelperText error>{errors.endingDate.message}</FormHelperText>}
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

export default RehireForfeituresSearchFilter;
