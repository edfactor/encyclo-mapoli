import { yupResolver } from "@hookform/resolvers/yup";
import Grid2 from "@mui/material/Grid2";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetTerminationReportQuery } from "reduxstore/api/YearsEndApi";
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
import { CalendarResponseDto, TerminationRequest } from "../../../reduxstore/types";
import { tryddmmyyyyToDate } from "../../../utils/dateUtils";

const schema = yup.object().shape({
  profitYear: yup.number().required("Profit Year is required"),
  beginningDate: yup.string().nullable(),
  endingDate: yup.string().nullable(),
  pagination: yup
    .object({
      skip: yup.number().required(),
      take: yup.number().required(),
      sortBy: yup.string().required(),
      isSortDescending: yup.boolean().required()
    })
    .required()
});

interface TerminationSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
  fiscalData: CalendarResponseDto;
  onSearch: () => void;
}

const TerminationSearchFilter: React.FC<TerminationSearchFilterProps> = ({
  setInitialSearchLoaded,
  fiscalData,
  onSearch 
}) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerSearch, { isFetching }] = useLazyGetTerminationReportQuery();
  const { termination } = useSelector((state: RootState) => state.yearsEnd);
  const defaultProfitYear = useDecemberFlowProfitYear();
  const dispatch = useDispatch();

  const validateAndSubmit = (data: TerminationRequest) => {
    if (isValid && hasToken) {
      const updatedData = {
        ...data,
        profitYear: defaultProfitYear || 0,
        beginningDate: data.beginningDate || fiscalData.fiscalBeginDate || '',
        endingDate: data.endingDate || fiscalData.fiscalEndDate || '',
      };

      triggerSearch(updatedData);
      onSearch(); // Call onSearch to trigger page reset
    }
  };

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<TerminationRequest>({
    resolver: yupResolver<TerminationRequest>(schema),
    defaultValues: {
      profitYear: defaultProfitYear || 0,
      beginningDate: termination?.startDate || fiscalData.fiscalBeginDate || undefined,
      endingDate: termination?.endDate || fiscalData.fiscalEndDate || undefined,
      pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
    }
  });

  // Effect to fetch fiscal data when profit year changes
  const validateAndSearch = handleSubmit(validateAndSubmit);

  const handleReset = () => {
    setInitialSearchLoaded(false);

    reset({ 
      profitYear: defaultProfitYear || 0,
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
                required={false}
                label="Begin Date"
                disableFuture
                error={errors.beginningDate?.message}
                minDate={new Date(defaultProfitYear - 5, 0, 1)}
                maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate)}
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
                required={false}
                label="End Date"
                disableFuture
                error={errors.endingDate?.message}
                minDate={new Date(defaultProfitYear - 5, 0, 2)}
                maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate)}
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
          disabled={!isValid || isFetching}
        />
      </Grid2>
    </form>
  );
};

export default TerminationSearchFilter;

