import { yupResolver } from "@hookform/resolvers/yup";
import { FormHelperText, TextField, Box } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Controller, useForm } from "react-hook-form";
import { useDispatch } from "react-redux";
import { useLazyGetHistoricalFrozenStateResponseQuery } from "reduxstore/api/FrozenApi";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import DsmDatePicker from "../../../components/DsmDatePicker/DsmDatePicker";

// Update the interface to include new fields
interface DemographicFreezeSearch {
  profitYear: number;
  asOfDate: Date | null;
  asOfTime: string | null;
}

// Update the schema to include validation for all fields
const schema = yup.object().shape({
  profitYear: yup
    .number()
    .typeError("Year must be a number")
    .integer("Year must be an integer")
    .min(2020, "Year must be 2020 or later")
    .max(2100, "Year must be 2100 or earlier")
    .required("Year is required"),
  asOfDate: yup
    .date()
    .required("As of Date is required")
    .test(
      "not-too-old",
      "Date cannot be more than 1 year ago",
      (value) => {
        if (!value) return true; // Skip validation if empty (required handles this)
        const oneYearAgo = new Date();
        oneYearAgo.setFullYear(oneYearAgo.getFullYear() - 1);
        return value >= oneYearAgo;
      }
    )
    .test(
      "not-future",
      "Date cannot be in the future",
      (value) => {
        if (!value) return true;
        return value <= new Date();
      }
    ),
  asOfTime: yup.string().required("As of Time is required")
});

interface DemographicFreezeSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
}

const DemographicFreezeManager: React.FC<DemographicFreezeSearchFilterProps> = ({
                                                                                  setInitialSearchLoaded
                                                                                }) => {
  const [triggerSearch, { isFetching }] = useLazyGetHistoricalFrozenStateResponseQuery();
  const profitYear = useDecemberFlowProfitYear();
  const dispatch = useDispatch();

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<DemographicFreezeSearch>({
    resolver: yupResolver(schema),
    defaultValues: {
      profitYear: profitYear || undefined,
      asOfDate: null,
      asOfTime: null
    }
  });

  const validateAndSearch = handleSubmit((data) => {
    if (isValid) {
      triggerSearch({
        skip: 0,
        take: 25,
        sortBy: "createdDateTime",
        isSortDescending: false,
        profitYear: data.profitYear,
        asOfDate: data.asOfDate,
        asOfTime: data.asOfTime
      });
    }
  });

  const handleReset = () => {
    setInitialSearchLoaded(false);
    reset({
      profitYear: undefined,
      asOfDate: null,
      asOfTime: null
    });
  };

  // Style for making input fields smaller
  const fieldStyle = {
    width: '250px',  // Fixed width for all fields to make them smaller
    mr: 2,           // Add some right margin between fields
  };

  return (
    <form onSubmit={validateAndSearch}>
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'row',
          flexWrap: 'wrap',
          alignItems: 'flex-end', // Align items at the bottom for consistent baseline
          paddingX: '24px',
          gap: '24px',
        }}
      >
        {/* Profit Year */}
        <Box sx={fieldStyle}>
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
              />
            )}
          />
          {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
        </Box>

        {/* As Of Date */}
        <Box sx={fieldStyle}>
          <Controller
            name="asOfDate"
            control={control}
            render={({ field }) => (
              <DsmDatePicker
                id="asOfDate"
                onChange={(value: Date | null) => field.onChange(value)}
                value={field.value}
                required={true}
                label="As Of Date"
                disableFuture
                error={errors.asOfDate?.message}
              />
            )}
          />
          {errors.asOfDate && <FormHelperText error>{errors.asOfDate.message}</FormHelperText>}
        </Box>

        {/* As Of Time */}
        <Box sx={fieldStyle}>
          <Controller
            name="asOfTime"
            control={control}
            render={({ field }) => (
              <TextField
                id="asOfTime"
                label="As Of Time"
                type="time"
                size="medium"
                margin="normal"
                sx={{
                  mt: 0,  // Remove top margin
                  mb: 0,  // Remove bottom margin
                  '& .MuiInputBase-root': {
                    height: '56px',  // Match the height of DsmDatePicker
                  }
                }}
                InputLabelProps={{
                  shrink: true,
                }}
                inputProps={{
                  step: 300, // 5-minute steps
                }}
                fullWidth
                required
                onChange={field.onChange}
                value={field.value || ""}
                error={!!errors.asOfTime}
                helperText={errors.asOfTime?.message}
              />
            )}
          />
        </Box>

        {/* Search and Reset Buttons */}
        <Box sx={{ mt: 1 }}>
          <SearchAndReset
            onReset={handleReset}
            isSearching={isFetching}
          />
        </Box>
      </Box>
    </form>
  );
};

export default DemographicFreezeManager;