import { yupResolver } from "@hookform/resolvers/yup";
import { Checkbox, FormControlLabel, FormHelperText, Grid } from "@mui/material";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { useEffect } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetRehireForfeituresQuery } from "reduxstore/api/YearsEndApi";
import {
  clearRehireForfeituresDetails,
  clearRehireForfeituresQueryParams,
  setRehireForfeituresQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import DsmDatePicker from "../../../components/DsmDatePicker/DsmDatePicker";
import { CalendarResponseDto, StartAndEndDateRequest } from "../../../reduxstore/types";
import { mmDDYYFormat, tryddmmyyyyToDate } from "../../../utils/dateUtils";

const schema = yup.object().shape({
  beginningDate: yup
    .string()
    .required("Beginning Date is required")
    .test("is-four-digits", "Beginning Date must be four digits", function (value) {
      return /^\d{1,2}\/\d{1,2}\/\d{4}$/.test(value || "");
    })
    .test("is-valid-year", "Year must be 2000 or later", function (value) {
      if (!value) return true;
      const match = value.match(/^\d{1,2}\/\d{1,2}\/(\d{4})$/);
      if (!match) return true;
      const year = parseInt(match[1]);
      return year >= 2000;
    }),
  endingDate: yup
    .string()
    .typeError("Invalid date")
    .required("Ending Date is required")
    .test("is-valid-year", "Year must be 2000 or later", function (value) {
      if (!value) return true;
      const match = value.match(/^\d{1,2}\/\d{1,2}\/(\d{4})$/);
      if (!match) return true;
      const year = parseInt(match[1]);
      return year >= 2000;
    })
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
    .required(),
  // Hidden field: not shown in search filter, but required in data
  profitYear: yup.number().required("Profit year is required")
});

interface RehireForfeituresSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
  fiscalData: CalendarResponseDto;
  onSearch?: () => void;
  hasUnsavedChanges?: boolean;
  setHasUnsavedChanges: (hasChanges: boolean) => void;
}

const RehireForfeituresSearchFilter: React.FC<RehireForfeituresSearchFilterProps> = ({
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

      dispatch(setRehireForfeituresQueryParams(updatedData));
      triggerSearch(updatedData);
      if (onSearch) onSearch(); // Only call if onSearch is provided
    }
  };

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger,
    clearErrors
  } = useForm<StartAndEndDateRequest>({
    resolver: yupResolver(schema),
    defaultValues: {
      beginningDate:
        rehireForfeituresQueryParams?.beginningDate ||
        (fiscalData.fiscalBeginDate ? mmDDYYFormat(fiscalData.fiscalBeginDate) : undefined),
      endingDate:
        rehireForfeituresQueryParams?.endingDate ||
        (fiscalData.fiscalEndDate ? mmDDYYFormat(fiscalData.fiscalEndDate) : undefined),
      excludeZeroBalance: rehireForfeituresQueryParams?.excludeZeroBalance || false,
      pagination: { skip: 0, take: 25, sortBy: "fullName", isSortDescending: false },
      profitYear: selectedProfitYear
    }
  });

  const beginningDateValue = useWatch({ control, name: "beginningDate" });

  // Trigger validation when fiscal data becomes available
  useEffect(() => {
    if (fiscalData.fiscalBeginDate && fiscalData.fiscalEndDate) {
      trigger();
    }
  }, [fiscalData.fiscalBeginDate, fiscalData.fiscalEndDate, trigger]);

  // Effect to fetch fiscal data when profit year changes
  const validateAndSearch = handleSubmit(validateAndSubmit);

  const handleReset = () => {
    setHasUnsavedChanges(false);
    setInitialSearchLoaded(false);
    dispatch(clearRehireForfeituresQueryParams());
    dispatch(clearRehireForfeituresDetails());

    reset({
      beginningDate: fiscalData.fiscalBeginDate ? mmDDYYFormat(fiscalData.fiscalBeginDate) : undefined,
      endingDate: fiscalData.fiscalEndDate ? mmDDYYFormat(fiscalData.fiscalEndDate) : undefined,
      excludeZeroBalance: false,
      profitYear: selectedProfitYear,
      pagination: { skip: 0, take: 25, sortBy: "badgeNumber", isSortDescending: true }
    });

    clearErrors();
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
              <>
                <DsmDatePicker
                  id="beginningDate"
                  onChange={(value: Date | null) => {
                    field.onChange(value ? mmDDYYFormat(value) : undefined);
                    trigger("beginningDate");
                    if (value) {
                      trigger("endingDate");
                    }
                  }}
                  value={field.value ? tryddmmyyyyToDate(field.value) : null}
                  required={true}
                  label="Rehire Begin Date"
                  disableFuture
                  error={errors.beginningDate?.message}
                  minDate={tryddmmyyyyToDate(fiscalData.fiscalBeginDate) ?? undefined}
                  maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate) ?? undefined}
                />
                <FormHelperText error>{errors.beginningDate?.message || " "}</FormHelperText>
              </>
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
                <>
                  <DsmDatePicker
                    id="endingDate"
                    onChange={(value: Date | null) => {
                      field.onChange(value ? mmDDYYFormat(value) : undefined);
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
                  <FormHelperText error>{errors.endingDate?.message || " "}</FormHelperText>
                </>
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
