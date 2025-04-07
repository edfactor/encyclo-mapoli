import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText } from "@mui/material";
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
import useFiscalCalendarYear from "../../../hooks/useFiscalCalendarYear";
import { useEffect } from "react";
import { dateYYYYMMDD, formatDateString } from "../../../utils/dateUtils";

interface RehireForfeituresSearch extends ProfitYearRequest {
  beginningDate: string;
  endingDate: string;
  pagination: SortedPaginationRequestDto;
}

const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Profit Year must be a number")
    .integer("Profit Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Profit Year must be 2100 or earlier")
    .required("Profit Year is required"),
  beginningDate: yup.string().required("Beginning Date is required"),
  endingDate: yup.string()
    .typeError("Invalid date")
    .test("greater-than-start", "End year must be after start year", function (endYear) {
      const startYear = this.parent.beginningDate;
      // Only validate if both values are present
      return !startYear || !endYear || endYear >= startYear;
    })
    .required("Ending Date is required"),
  pagination: yup
    .object({
      skip: yup.number().required(),
      take: yup.number().required(),
      sortBy: yup.string().required(),
      isSortDescending: yup.boolean().required()
    })
    .required()
});

interface MilitaryAndRehireForfeituresSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const RehireForfeituresSearchFilter: React.FC<MilitaryAndRehireForfeituresSearchFilterProps> = ({
  setInitialSearchLoaded
}) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerSearch, { isFetching }] = useLazyGetRehireForfeituresQuery();
  const { rehireForfeituresQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const profitYear = useDecemberFlowProfitYear();
  const fiscalCalendarYear = useFiscalCalendarYear();
  const today = new Date().toDateString().split('T')[0]; // Simple fallback date
  const dispatch = useDispatch();
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    setValue,
    getValues
  } = useForm<RehireForfeituresSearch>({
    resolver: yupResolver<RehireForfeituresSearch>(schema),
    defaultValues: {
      profitYear: profitYear || rehireForfeituresQueryParams?.profitYear || undefined,
      beginningDate: rehireForfeituresQueryParams?.beginningDate || today,
      endingDate: rehireForfeituresQueryParams?.endingDate || today,
      pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
    }
  });

  // When setting the date in the form, ensure proper timezone handling
  useEffect(() => {
    if (fiscalCalendarYear?.fiscalBeginDate && !rehireForfeituresQueryParams?.beginningDate) {
      // Keep the date as a string without conversion
      setValue('beginningDate', fiscalCalendarYear.fiscalBeginDate);
    }
    if (fiscalCalendarYear?.fiscalEndDate && !rehireForfeituresQueryParams?.endingDate) {
      // Keep the date as a string without conversion
      setValue('endingDate', fiscalCalendarYear.fiscalEndDate);
    }
  }, [fiscalCalendarYear, setValue, rehireForfeituresQueryParams]);

  const validateAndSearch = handleSubmit((data) => {
    if (isValid && hasToken) {
      triggerSearch(
        {
          profitYear: profitYear,
          beginningDate: data.beginningDate,
          endingDate: data.endingDate,
          pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
        },
        false
      ).unwrap();
      dispatch(
        setMilitaryAndRehireForfeituresQueryParams({
          profitYear: profitYear,
          beginningDate: data.beginningDate,
          endingDate: data.endingDate,
          pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
        })
      );
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);
    dispatch(clearRehireForfeituresQueryParams());
    dispatch(clearRehireForfeituresDetails());
    reset({
      profitYear: profitYear,
      beginningDate: fiscalCalendarYear?.fiscalBeginDate || undefined,
      endingDate: fiscalCalendarYear?.fiscalEndDate || undefined,
      pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
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
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="beginningDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="beginningDate"
                onChange={(value: Date | null) => field.onChange(value?.getFullYear() || undefined)}
                value={field.value ? new Date(field.value) : null}
                required={true}
                label="Rehire Begin Date"
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
                onChange={(value: Date | null) => field.onChange(value || undefined)}
                value={field.value ? new Date(field.value) : null}
                required={true}
                label="Rehire Ending Date"
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
          disabled={!isValid || isFetching}
        />
      </Grid2>
    </form>
  );
};

export default RehireForfeituresSearchFilter;
