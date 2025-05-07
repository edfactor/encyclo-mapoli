import { yupResolver } from "@hookform/resolvers/yup";
import Grid2 from "@mui/material/Grid2";
import { useEffect } from "react";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetAccountingYearQuery } from "reduxstore/api/LookupsApi";
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
import { CalendarResponseDto, ProfitYearRequest, SortedPaginationRequestDto } from "../../../reduxstore/types";
import { tryddmmyyyyToDate } from "../../../utils/dateUtils";

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
    .min(2019, "Year must be 2020 or later")
    .max(2100, "Profit Year must be 2100 or earlier")
    .required("Profit Year is required"),
  beginningDate: yup.string().required("Beginning Date is required"),
  endingDate: yup.string()
    .typeError("Invalid date")
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
  fiscalData: CalendarResponseDto;
}

const RehireForfeituresSearchFilter: React.FC<MilitaryAndRehireForfeituresSearchFilterProps> = ({
                                                                                                  setInitialSearchLoaded,
                                                                                                  fiscalData
                                                                                                }) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerSearch, { isFetching }] = useLazyGetRehireForfeituresQuery();
  const [fetchAccountingYear, { isLoading: isLoadingFiscalData }] = useLazyGetAccountingYearQuery();
  const { rehireForfeituresQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const defaultProfitYear = useDecemberFlowProfitYear();
  const dispatch = useDispatch();

  const validateAndSubmit = (data: RehireForfeituresSearch) => {
    if (isValid && hasToken) {
      const beginDate = data.beginningDate || fiscalData.fiscalBeginDate || '';
      const endDate = data.endingDate || fiscalData.fiscalEndDate || '';

      const updatedData = {
        ...data,
        beginningDate: beginDate,
        endingDate: endDate,
      };

      dispatch(setMilitaryAndRehireForfeituresQueryParams(updatedData));
      triggerSearch(updatedData);
    }
  };

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger,
    setValue,
    getValues,
    watch
  } = useForm<RehireForfeituresSearch>({
    resolver: yupResolver<RehireForfeituresSearch>(schema),
    defaultValues: {
      profitYear: defaultProfitYear || rehireForfeituresQueryParams?.profitYear || undefined,
      beginningDate: rehireForfeituresQueryParams?.beginningDate || fiscalData.fiscalBeginDate || undefined,
      endingDate: rehireForfeituresQueryParams?.endingDate || fiscalData.fiscalEndDate || undefined,
      pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
    }
  });

  // Watch for changes to the profitYear field
  const watchProfitYear = watch("profitYear");

  // Effect to fetch fiscal data when profit year changes
  useEffect(() => {
    const fetchFiscalDataForYear = async (year: number) => {
      if (year && hasToken) {
        try {
          const result = await fetchAccountingYear({ profitYear: year }).unwrap();
          if (result) {
            // Update the beginning and ending dates with the new fiscal dates
            setValue("beginningDate", result.fiscalBeginDate);
            setValue("endingDate", result.fiscalEndDate);

            // Trigger validation for updated fields
            trigger(["beginningDate", "endingDate"]);
          }
        } catch (error) {
          console.error("Failed to fetch accounting year data:", error);
        }
      }
    };

    // Only fetch if the profit year has changed from the default
    if (watchProfitYear && watchProfitYear !== defaultProfitYear) {
      fetchFiscalDataForYear(watchProfitYear);
    }
  }, [watchProfitYear, hasToken, fetchAccountingYear, setValue, trigger, defaultProfitYear]);

  const validateAndSearch = handleSubmit(validateAndSubmit);

  const handleReset = () => {
    setInitialSearchLoaded(false);
    dispatch(clearRehireForfeituresQueryParams());
    dispatch(clearRehireForfeituresDetails());

    reset({
      profitYear: defaultProfitYear,
      beginningDate: fiscalData.fiscalBeginDate,
      endingDate: fiscalData.fiscalEndDate,
      pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
    });

    trigger();
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
                minDate={new Date(defaultProfitYear - 5, 0, 1)}
                disabled={isLoadingFiscalData}
              />
            )}
          />
        </Grid2>
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="beginningDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="beginningDate"
                onChange={(value: Date | null) => {
                  field.onChange(value || undefined);
                  trigger('beginningDate');
                }}
                value={field.value ? tryddmmyyyyToDate(field.value) : null}
                required={true}
                label="Rehire Begin Date"
                disableFuture
                error={errors.beginningDate?.message}
                minDate={tryddmmyyyyToDate(fiscalData.fiscalBeginDate)}
                maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate)}
                disabled={isLoadingFiscalData}
              />
            )}
          />
        </Grid2>
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="endingDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="endingDate"
                onChange={(value: Date | null) => {
                  field.onChange(value || undefined);
                  trigger('endingDate');
                }}
                value={field.value ? tryddmmyyyyToDate(field.value) : null}
                required={true}
                label="Rehire Ending Date"
                disableFuture
                error={errors.endingDate?.message}
                minDate={tryddmmyyyyToDate(fiscalData.fiscalBeginDate)}
                maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate)}
                disabled={isLoadingFiscalData}
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
          isFetching={isFetching || isLoadingFiscalData}
          disabled={!isValid || isFetching || isLoadingFiscalData}
        />
      </Grid2>
    </form>
  );
};

export default RehireForfeituresSearchFilter;