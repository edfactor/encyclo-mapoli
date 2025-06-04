import { yupResolver } from "@hookform/resolvers/yup";
import Grid2 from "@mui/material/Grid2";
import { Controller, useForm } from "react-hook-form";
import { useSelector, useDispatch } from "react-redux";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import DsmDatePicker from "../../../components/DsmDatePicker/DsmDatePicker";
import { CalendarResponseDto, StartAndEndDateRequest } from "../../../reduxstore/types";
import { tryddmmyyyyToDate } from "../../../utils/dateUtils";
import { RootState } from "reduxstore/store";
import { useLazyGetTerminationReportQuery } from "reduxstore/api/YearsEndApi";

const schema = yup.object().shape({
  beginningDate: yup.string().required("Begin Date is required"),
  endingDate: yup.string().required("End Date is required"),
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
  fiscalData: CalendarResponseDto | null;
  onSearch: (params: StartAndEndDateRequest) => void;
}

const TerminationSearchFilter: React.FC<TerminationSearchFilterProps> = ({
  setInitialSearchLoaded,
  fiscalData,
  onSearch
}) => {
  if (!fiscalData) return null;

  const dispatch = useDispatch();
  const { termination } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetTerminationReportQuery();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<StartAndEndDateRequest>({
    resolver: yupResolver(schema),
    defaultValues: {
      beginningDate: termination?.startDate || fiscalData.fiscalBeginDate || '',
      endingDate: termination?.endDate || fiscalData.fiscalEndDate || '',
      pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
    }
  });

  const validateAndSubmit = async (data: StartAndEndDateRequest) => {
    const params = {
      ...data,
      beginningDate: data.beginningDate || fiscalData.fiscalBeginDate || '',
      endingDate: data.endingDate || fiscalData.fiscalEndDate || '',
    };
    // Call the API and only call onSearch after success
    try {
      await triggerSearch(params, false).unwrap();
      onSearch(params);
      setInitialSearchLoaded(true);
    } catch (e) {
      setInitialSearchLoaded(false);
    }
  };

  const validateAndSearch = handleSubmit(validateAndSubmit);

  const handleReset = () => {
    setInitialSearchLoaded(false);
    reset({
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
                minDate={tryddmmyyyyToDate(fiscalData.fiscalBeginDate)}
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
                minDate={tryddmmyyyyToDate(fiscalData.fiscalBeginDate)}
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

