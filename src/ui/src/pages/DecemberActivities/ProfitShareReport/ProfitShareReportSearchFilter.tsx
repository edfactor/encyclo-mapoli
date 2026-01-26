import { yupResolver } from "@hookform/resolvers/yup";
import { Box, FormLabel, Grid, TextField, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { Controller, Resolver, useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useLazyGetYearEndProfitSharingReportLiveQuery } from "reduxstore/api/YearsEndApi";
import {
  clearYearEndProfitSharingReportLive,
  setYearEndProfitSharingReportQueryParams
} from "reduxstore/slices/yearsEndSlice";
import { FilterParams } from "reduxstore/types";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { VisuallyHidden } from "../../../utils/accessibilityHelpers";
import { generateFieldId, getAriaDescribedBy } from "../../../utils/accessibilityUtils";
import { psnValidator } from "../../../utils/FormValidators";
import { ARIA_DESCRIPTIONS, INPUT_PLACEHOLDERS } from "../../../utils/inputFormatters";
import presets from "../../FiscalClose/PAY426Reports/PAY426N/presets";

interface ProfitShareReportSearch {
  badgeNumber?: number | null;
}

const schema = yup.object().shape({
  badgeNumber: psnValidator
});

interface SearchParams {
  [key: string]: unknown;
  reportId: number;
  badgeNumber?: number;
  profitYear?: number;
  pagination?: {
    skip: number;
    take: number;
    sortBy: string;
    isSortDescending: boolean;
  };
}

interface ProfitShareReportSearchFilterProps {
  profitYear: number;
  presetParams: FilterParams;
  onSearchParamsUpdate?: (searchParams: SearchParams) => void;
}

const ProfitShareReportSearchFilter: React.FC<ProfitShareReportSearchFilterProps> = ({
  profitYear,
  presetParams,
  onSearchParamsUpdate
}) => {
  const [triggerSearch, { isFetching }] = useLazyGetYearEndProfitSharingReportLiveQuery();
  const dispatch = useDispatch();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const currentPreset = presets.find((preset) => preset.params.reportId === presetParams.reportId);

  const {
    control,
    handleSubmit,
    formState: { errors },
    reset
  } = useForm<ProfitShareReportSearch>({
    resolver: yupResolver(schema) as Resolver<ProfitShareReportSearch>,
    defaultValues: {
      badgeNumber: undefined
    }
  });

  useEffect(() => {
    if (!isFetching) {
      setIsSubmitting(false);
    }
  }, [isFetching]);

  // Reset form and clear results when report preset changes
  useEffect(() => {
    reset({ badgeNumber: undefined });
    dispatch(clearYearEndProfitSharingReportLive());
  }, [presetParams, reset, dispatch]);

  const validateAndSearch = handleSubmit((data) => {
    if (!isSubmitting) {
      setIsSubmitting(true);
      const request = {
        ...presetParams,
        badgeNumber: data.badgeNumber ?? undefined,
        profitYear: profitYear,
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "badgeNumber",
          isSortDescending: false
        }
      };

      triggerSearch(request, false);
      dispatch(setYearEndProfitSharingReportQueryParams(profitYear));

      // Notify parent component of search parameters
      if (onSearchParamsUpdate) {
        onSearchParamsUpdate(request);
      }
    }
  });

  const handleReset = () => {
    reset({
      badgeNumber: undefined
    });
    dispatch(clearYearEndProfitSharingReportLive());
  };

  return (
    <form onSubmit={validateAndSearch}>
      {currentPreset && (
        <Box sx={{ px: "24px", pb: 2 }}>
          <Typography
            variant="body2"
            color="text.secondary">
            Searching: <strong>{currentPreset.name}</strong> - {currentPreset.description}
          </Typography>
        </Box>
      )}
      <Grid
        container
        paddingX="24px">
        <Grid size={{ xs: 12, sm: 3, md: 3 }}>
          <FormLabel htmlFor={generateFieldId("badgeNumber")}>Profit Sharing Number</FormLabel>
          <Controller
            name="badgeNumber"
            control={control}
            render={({ field }) => (
              <>
                <TextField
                  {...field}
                  id={generateFieldId("badgeNumber")}
                  fullWidth
                  value={field.value || ""}
                  placeholder={INPUT_PLACEHOLDERS.BADGE_OR_PSN}
                  inputProps={{ inputMode: "numeric" }}
                  aria-invalid={!!errors.badgeNumber || undefined}
                  aria-describedby={getAriaDescribedBy("badgeNumber", !!errors.badgeNumber, true)}
                  onChange={(e) => field.onChange(e.target.value === "" ? null : Number(e.target.value))}
                />
                <VisuallyHidden id="badgeNumber-hint">{ARIA_DESCRIPTIONS.BADGE_FORMAT}</VisuallyHidden>
                {errors.badgeNumber && (
                  <div
                    id="badgeNumber-error"
                    aria-live="polite"
                    aria-atomic="true">
                    <Typography
                      variant="caption"
                      color="error">
                      {errors.badgeNumber.message}
                    </Typography>
                  </div>
                )}
              </>
            )}
          />
        </Grid>
        <Grid size={{ xs: 0, sm: 9, md: 9 }} />
        <Grid size={{ xs: 12, sm: 3, md: 3 }}>
          <SearchAndReset
            handleReset={handleReset}
            handleSearch={validateAndSearch}
            isFetching={isFetching || isSubmitting}
            disabled={isFetching || isSubmitting}
          />
        </Grid>
      </Grid>
    </form>
  );
};

export default ProfitShareReportSearchFilter;
