import { yupResolver } from "@hookform/resolvers/yup";
import { FormLabel, TextField } from "@mui/material";
import { Grid } from "@mui/material";
import { Controller, useForm, Resolver } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { setYearEndProfitSharingReportQueryParams } from "reduxstore/slices/yearsEndSlice";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { useEffect } from "react";
import { FilterParams } from "reduxstore/types";

interface ProfitShareReportSearch {
  badgeNumber?: number | null;
}

const schema = yup.object().shape({
  badgeNumber: yup
    .number()
    .typeError("Badge Number must be a number")
    .integer("Badge Number must be an integer")
    .min(0, "Badge must be positive")
    .max(9999999, "Badge must be 7 digits or less")
    .nullable()
});

interface ProfitShareReportSearchFilterProps {
  profitYear: number;
  presetParams: FilterParams;
}

const ProfitShareReportSearchFilters: React.FC<ProfitShareReportSearchFilterProps> = ({ profitYear, presetParams }) => {
  const [triggerSearch, { isFetching }] = useLazyGetYearEndProfitSharingReportQuery();
  const dispatch = useDispatch();

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

  // Reset form when report preset changes
  useEffect(() => {
    reset({ badgeNumber: undefined });
  }, [presetParams, reset]);

  const validateAndSearch = handleSubmit((data) => {
    const request = {
      ...presetParams,
      badgeNumber: data.badgeNumber,
      profitYear: profitYear,
      pagination: {
        skip: 0,
        take: 10,
        sortBy: "badgeNumber",
        isSortDescending: true
      }
    };

    triggerSearch(request, false);
    dispatch(setYearEndProfitSharingReportQueryParams(profitYear));
  });

  const handleReset = () => {
    reset({
      badgeNumber: undefined
    });

    const request = {
      ...presetParams,
      profitYear: profitYear,
      pagination: {
        skip: 0,
        take: 10,
        sortBy: "badgeNumber",
        isSortDescending: true
      }
    };

    triggerSearch(request, false);
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Grid
        container
        paddingX="24px">
        <Grid size={{ xs: 12, sm: 3, md: 3 }}>
          <FormLabel>Badge Number</FormLabel>
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
