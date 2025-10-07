import { yupResolver } from "@hookform/resolvers/yup";
import { Box, FormLabel, Grid, TextField, Typography } from "@mui/material";
import { useEffect } from "react";
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
import presets from "../PAY426Reports/PAY426N/presets";

interface ProfitShareReportSearch {
  badgeNumber?: number | null;
}

const schema = yup.object().shape({
  badgeNumber: yup
    .number()
    .typeError("Profit Sharing Number must be a number")
    .integer("Profit Sharing Number must be an integer")
    .min(0, "Profit Sharing Number must be positive")
    .max(9999999999, "Profit Sharing Number must be 10 digits or less")
    .nullable()
});

interface ProfitShareReportSearchFilterProps {
  profitYear: number;
  presetParams: FilterParams;
  onSearchParamsUpdate?: (searchParams: any) => void;
}

const ProfitShareReportSearchFilters: React.FC<ProfitShareReportSearchFilterProps> = ({ profitYear, presetParams, onSearchParamsUpdate }) => {
  const [triggerSearch, { isFetching }] = useLazyGetYearEndProfitSharingReportLiveQuery();
  const dispatch = useDispatch();

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

  // Reset form and clear results when report preset changes
  useEffect(() => {
    reset({ badgeNumber: undefined });
    dispatch(clearYearEndProfitSharingReportLive());
  }, [presetParams, reset, dispatch]);

  const validateAndSearch = handleSubmit((data) => {
    const request = {
      ...presetParams,
      badgeNumber: data.badgeNumber,
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
          <FormLabel>Profit Sharing Number</FormLabel>
          <Controller
            name="badgeNumber"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                fullWidth
                value={field.value || ""}
                onChange={(e) => field.onChange(e.target.value === "" ? null : Number(e.target.value))}
                error={!!errors.badgeNumber}
                helperText={errors.badgeNumber?.message}
              />
            )}
          />
        </Grid>
        <Grid size={{ xs: 0, sm: 9, md: 9 }} />
        <Grid size={{ xs: 12, sm: 3, md: 3 }}>
          <SearchAndReset
            handleReset={handleReset}
            handleSearch={validateAndSearch}
            isFetching={isFetching}
          />
        </Grid>
      </Grid>
    </form>
  );
};

export default ProfitShareReportSearchFilters;
