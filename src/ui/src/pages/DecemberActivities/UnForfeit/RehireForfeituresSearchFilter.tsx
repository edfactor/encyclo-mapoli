import { yupResolver } from "@hookform/resolvers/yup";
import { Checkbox, FormControlLabel } from "@mui/material";
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
import DsmDatePicker from "../../../components/DsmDatePicker/DsmDatePicker";
import { CalendarResponseDto, StartAndEndDateRequest } from "../../../reduxstore/types";
import { mmDDYYFormat, tryddmmyyyyToDate } from "../../../utils/dateUtils";

const schema = yup.object().shape({
  beginningDate: yup.string().required("Beginning Date is required"),
  endingDate: yup.string().typeError("Invalid date").required("Ending Date is required"),
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
  onSearch?: () => void;
  hasUnsavedChanges?: boolean;
}

const RehireForfeituresSearchFilter: React.FC<MilitaryAndRehireForfeituresSearchFilterProps> = ({
  setInitialSearchLoaded,
  fiscalData,
  onSearch,
  hasUnsavedChanges
}) => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerSearch, { isFetching }] = useLazyGetRehireForfeituresQuery();
  const { rehireForfeituresQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const dispatch = useDispatch();

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
        endingDate: mmDDYYFormat(endDate)
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

  // Effect to fetch fiscal data when profit year changes
  const validateAndSearch = handleSubmit(validateAndSubmit);

  const handleReset = () => {
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
      <Grid2
        container
        paddingX="24px"
        alignItems={"flex-end"}
        gap="24px">
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="beginningDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="beginningDate"
                onChange={(value: Date | null) => {
                  field.onChange(value ?? undefined);
                  trigger("beginningDate");
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
        </Grid2>
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="endingDate"
            control={control}
            render={({ field }) => (
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
                minDate={tryddmmyyyyToDate(fiscalData.fiscalBeginDate) ?? undefined}
                maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate) ?? undefined}
              />
            )}
          />
        </Grid2>
        <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
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
                label="Exclude 0 balance"
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

export default RehireForfeituresSearchFilter;
