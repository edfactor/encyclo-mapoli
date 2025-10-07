import { yupResolver } from "@hookform/resolvers/yup";
import { Button, FormHelperText, Grid } from "@mui/material";
import React, { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useDispatch, useSelector } from "react-redux";
import { SearchAndReset, SmartModal } from "smart-ui-library";
import * as yup from "yup";
import DsmDatePicker from "../../../components/DsmDatePicker/DsmDatePicker";
import { profitYearValidator } from "../../../utils/FormValidators";
import DuplicateSsnGuard from "../../../components/DuplicateSsnGuard";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { clearTermination } from "../../../reduxstore/slices/yearsEndSlice";
import { RootState } from "../../../reduxstore/store";
import { CalendarResponseDto } from "../../../reduxstore/types";
import { mmDDYYFormat, tryddmmyyyyToDate } from "../../../utils/dateUtils";
import { TerminationSearchRequest } from "./Termination";

const schema = yup.object().shape({
  beginningDate: yup.string().required("Begin Date is required"),
  endingDate: yup
    .string()
    .required("End Date is required")
    .test("is-after-start", "End Date must be after Begin Date", function (value) {
      const { beginningDate } = this.parent;
      if (!beginningDate || !value) return true;
      const startDate = tryddmmyyyyToDate(beginningDate);
      const endDate = tryddmmyyyyToDate(value);
      if (!startDate || !endDate) return true;
      return endDate > startDate;
    }),
  forfeitureStatus: yup.string().required("Forfeiture Status is required"),
  pagination: yup
    .object({
      skip: yup.number().required(),
      take: yup.number().required(),
      sortBy: yup.string().required(),
      isSortDescending: yup.boolean().required()
    })
    .required(),
  profitYear: profitYearValidator
});

interface TerminationSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
  fiscalData: CalendarResponseDto | null;
  onSearch: (params: TerminationSearchRequest) => void;
  hasUnsavedChanges?: boolean;
  isFetching?: boolean;
}

const TerminationSearchFilter: React.FC<TerminationSearchFilterProps> = ({
  setInitialSearchLoaded,
  fiscalData,
  onSearch,
  hasUnsavedChanges,
  isFetching = false
}) => {
  const [openErrorModal, setOpenErrorModal] = useState(!fiscalData === false);
  const dispatch = useDispatch();
  const selectedProfitYear = useDecemberFlowProfitYear();
  const { termination } = useSelector((state: RootState) => state.yearsEnd);
  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset,
    trigger
  } = useForm<TerminationSearchRequest>({
    resolver: yupResolver(schema),
    defaultValues: {
      beginningDate: termination?.startDate || (fiscalData ? fiscalData.fiscalBeginDate : "") || "",
      endingDate: termination?.endDate || (fiscalData ? fiscalData.fiscalEndDate : "") || "",
      forfeitureStatus: "showAll",
      pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false },
      profitYear: selectedProfitYear
    }
  });

  const validateAndSubmit = async (data: TerminationSearchRequest) => {
    if (hasUnsavedChanges) {
      alert("Please save your changes.");
      return;
    }

    const params = {
      ...data,
      profitYear: selectedProfitYear,
      beginningDate: data.beginningDate
        ? mmDDYYFormat(data.beginningDate)
        : mmDDYYFormat(fiscalData?.fiscalBeginDate || ""),
      endingDate: data.endingDate ? mmDDYYFormat(data.endingDate) : mmDDYYFormat(fiscalData?.fiscalEndDate || "")
    };
    // Only update search params and initial loaded state; let the grid trigger the API
    onSearch(params);
    setInitialSearchLoaded(true);
  };

  const validateAndSearch = handleSubmit(validateAndSubmit);

  const handleReset = async () => {
    setInitialSearchLoaded(false);
    reset({
      beginningDate: fiscalData ? fiscalData.fiscalBeginDate : "",
      endingDate: fiscalData ? fiscalData.fiscalEndDate : "",
      forfeitureStatus: "showAll",
      pagination: { skip: 0, take: 25, sortBy: "name", isSortDescending: false },
      profitYear: selectedProfitYear
    });
    // Trigger validation after reset to ensure form validity is updated
    await trigger();
    dispatch(clearTermination());
  };

  if (!fiscalData) {
    return (
      <SmartModal
        key={"fiscalDataErrorModal"}
        open={openErrorModal}
        maxWidth="sm"
        title="Services Error"
        onClose={() => setOpenErrorModal(false)}
        message={`Fiscal date range not available. Please try again later.\n`}>
        <Button
          onClick={() => setOpenErrorModal(false)}
          variant="outlined"
          sx={{ mt: "15px" }}>
          OK
        </Button>
      </SmartModal>
    );
  }

  return (
    <form onSubmit={validateAndSearch}>
      <Grid
        container
        paddingX="24px"
        gap="24px">
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="beginningDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="beginningDate"
                onChange={(value: Date | null) => {
                  field.onChange(value || undefined);
                  trigger("endingDate");
                }}
                value={field.value ? tryddmmyyyyToDate(field.value) : null}
                required={false}
                label="Begin Date"
                disableFuture
                error={errors.beginningDate?.message}
                minDate={tryddmmyyyyToDate(fiscalData.fiscalBeginDate) || undefined}
                maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate) || undefined}
              />
            )}
          />
          {errors.beginningDate && <FormHelperText error>{errors.beginningDate.message}</FormHelperText>}
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <Controller
            name="endingDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="endingDate"
                onChange={(value: Date | null) => {
                  field.onChange(value || undefined);
                  trigger("endingDate");
                }}
                value={field.value ? tryddmmyyyyToDate(field.value) : null}
                required={false}
                label="End Date"
                disableFuture
                error={errors.endingDate?.message}
                minDate={tryddmmyyyyToDate(fiscalData.fiscalBeginDate) || undefined}
                maxDate={tryddmmyyyyToDate(fiscalData.fiscalEndDate) || undefined}
              />
            )}
          />
          {errors.endingDate && <FormHelperText error>{errors.endingDate.message}</FormHelperText>}
        </Grid>
      </Grid>
      <Grid
        width="100%"
        paddingX="24px">
        <DuplicateSsnGuard>
          {({ prerequisitesComplete }) => (
            <SearchAndReset
              handleReset={handleReset}
              handleSearch={validateAndSearch}
              isFetching={isFetching}
              disabled={!isValid || !prerequisitesComplete}
            />
          )}
        </DuplicateSsnGuard>
      </Grid>
    </form>
  );
};

export default TerminationSearchFilter;
