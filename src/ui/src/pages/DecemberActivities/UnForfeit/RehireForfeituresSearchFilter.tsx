import { yupResolver } from "@hookform/resolvers/yup";
import { Checkbox, FormControlLabel } from "@mui/material";
import { Grid } from "@mui/material";
import { Controller, useForm, useWatch } from "react-hook-form";
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
import DsmDatePicker from "../../../components/DsmDatePicker/DsmDatePicker";
import { CalendarResponseDto, StartAndEndDateRequest } from "../../../reduxstore/types";
import { mmDDYYFormat, tryddmmyyyyToDate } from "../../../utils/dateUtils";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";

const schema = yup.object().shape({
  beginningDate: yup.string().required("Beginning Date is required"),
  endingDate: yup
    .string()
    .typeError("Invalid date")
    .required("Ending Date is required")
    .test("date-range", "Ending date must be the same or after the beginning date", function (value) {
      const { beginningDate } = this.parent;
      if (!beginningDate || !value) return true;

      const beginDate = tryddmmyyyyToDate(beginningDate);
      const endDate = tryddmmyyyyToDate(value);

      if (!beginDate || !endDate) return true;

      return endDate >= beginDate;
    })
    .test("is-too-early", "Insuffient data for dates before 2024", function (value) {
      return new Date(value) > new Date(2024, 1, 1);
    }),
  pagination: yup
    .object({
      skip: yup.number().required(),
      take: yup.number().required(),
      sortBy: yup.string().required(),
      isSortDescending: yup.boolean().required()
    })
    .required()
  //profitYear: yup.number().required("Profit year is required")
});

interface MilitaryAndRehireForfeituresSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
  fiscalData: CalendarResponseDto;
  onSearch?: () => void;
  hasUnsavedChanges?: boolean;
  setHasUnsavedChanges: (hasChanges: boolean) => void;
}

const RehireForfeituresSearchFilter: React.FC<MilitaryAndRehireForfeituresSearchFilterProps> = ({
  setInitialSearchLoaded,
  fiscalData,
  onSearch,
  hasUnsavedChanges,
  setHasUnsavedChanges
}) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerSearch, { isFetching }] = useLazyGetRehireForfeituresQuery();
  const { rehireForfeituresQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const dispatch = useDispatch();

  const selectedProfitYear = useDecemberFlowProfitYear();

  const validateAndSubmit = (data: StartAndEndDateRequest) => {
    if (hasUnsavedChanges) {
      alert("Please save your changes.");
      return;
    }

    if (isValid && hasToken) {
      const beginDate = data.beginningDate || fiscalData.fiscalBeginDate || "";
      const endDate = data.endingDate || fiscalData.fiscalEndDate || "";

      const updatedData = {
        ...data,
        beginningDate: mmDDYYFormat(beginDate),
        endingDate: mmDDYYFormat(endDate),
        profitYear: selectedProfitYear
      };

      dispatch(setMilitaryAndRehireForfeituresQueryParams(updatedData));
      triggerSearch(updatedData);
      if (onSearch) onSearch(); // Only call if onSearch is provided
    }
  };

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<StartAndEndDateRequest>({
    resolver: yupResolver(schema),
    defaultValues: {
      beginningDate: rehireForfeituresQueryParams?.beginningDate || fiscalData.fiscalBeginDate || undefined,
      endingDate: rehireForfeituresQueryParams?.endingDate || fiscalData.fiscalEndDate || undefined,
      excludeZeroBalance: rehireForfeituresQueryParams?.excludeZeroBalance || false,
      pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
    }
  });

  const beginningDateValue = useWatch({ control, name: "beginningDate" });

  // Effect to fetch fiscal data when profit year changes
  const validateAndSearch = handleSubmit(validateAndSubmit);

  const handleReset = () => {
    setHasUnsavedChanges(false);
    setInitialSearchLoaded(false);
    dispatch(clearRehireForfeituresQueryParams());
    dispatch(clearRehireForfeituresDetails());

    reset({
      beginningDate: fiscalData.fiscalBeginDate,
      endingDate: fiscalData.fiscalEndDate,
      excludeZeroBalance: false,
      pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
    });

    trigger();
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Grid
        container
        paddingX="24px"
        alignItems={"flex-end"}
        gap="24px">
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="beginningDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="beginningDate"
                onChange={(value: Date | null) => {
                  field.onChange(value ?? undefined);
                  trigger(["beginningDate", "endingDate"]);
                }}
                value={field.value ? tryddmmyyyyToDate(field.value) : null}
                required={true}
                label="Rehire Begin Date"
                disableFuture
                error={errors.beginningDate?.message}
                minDate={tryddmmyyyyToDate(fiscalData.fiscalBeginDate) ?? undefined}
                maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate) ?? undefined}
              />
            )}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="endingDate"
            control={control}
            render={({ field }) => {
              const minDateFromBeginning = beginningDateValue ? tryddmmyyyyToDate(beginningDateValue) : null;
              const fiscalMinDate = tryddmmyyyyToDate(fiscalData.fiscalBeginDate);
              const effectiveMinDate =
                minDateFromBeginning && fiscalMinDate
                  ? minDateFromBeginning > fiscalMinDate
                    ? minDateFromBeginning
                    : fiscalMinDate
                  : (minDateFromBeginning ?? fiscalMinDate ?? undefined);

              return (
                <DsmDatePicker
                  id="endingDate"
                  onChange={(value: Date | null) => {
                    field.onChange(value ?? undefined);
                    trigger("endingDate");
                  }}
                  value={field.value ? tryddmmyyyyToDate(field.value) : null}
                  required={true}
                  label="Rehire Ending Date"
                  disableFuture
                  error={errors.endingDate?.message}
                  minDate={effectiveMinDate}
                  maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate) ?? undefined}
                />
              );
            }}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="excludeZeroBalance"
            control={control}
            render={({ field }) => (
              <FormControlLabel
                control={
                  <Checkbox
                    checked={field.value || false}
                    onChange={(e) => field.onChange(e.target.checked)}
                  />
                }
                label="Exclude employees with no current balance and no vested balance"
              />
            )}
          />
        </Grid>
      </Grid>
      <Grid
        width="100%"
        paddingX="24px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSearch}
          isFetching={isFetching}
          disabled={!isValid || isFetching}
        />
      </Grid>
    </form>
  );
};

export default RehireForfeituresSearchFilter;
