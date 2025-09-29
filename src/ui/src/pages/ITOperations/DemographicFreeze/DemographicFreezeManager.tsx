import { yupResolver } from "@hookform/resolvers/yup";
import { Box, Button, FormHelperText, FormLabel, Grid, TextField } from "@mui/material";
import { Controller, Resolver, useForm } from "react-hook-form";
import * as yup from "yup";
import DsmDatePicker from "../../../components/DsmDatePicker/DsmDatePicker";
import DuplicateSsnGuard from "../../../components/DuplicateSsnGuard";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useFreezeDemographicsMutation } from "../../../reduxstore/api/ItOperationsApi";

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
    .test("valid-year", "Year must be current or previous year", (value) => {
      if (!value) return false;
      const currentYear = new Date().getFullYear();
      return value === currentYear || value === currentYear - 1;
    })
    .required("Year is required")
    .defined(),
  asOfDate: yup
    .date()
    .nullable()
    .required("As of Date is required")
    .test("not-future", "Date cannot be in the future", (value) => {
      if (!value) return true;
      return value <= new Date();
    })
    .test("valid-year", "Date must be from current or previous year", (value) => {
      if (!value) return true;
      const currentYear = new Date().getFullYear();
      const dateYear = value.getFullYear();
      return dateYear === currentYear || dateYear === currentYear - 1;
    }),
  asOfTime: yup.string().nullable().required("As of Time is required")
});

interface DemographicFreezeSearchFilterProps {
  setInitialSearchLoaded: (include: boolean) => void;
  setPageReset: (reset: boolean) => void;
}

const DemographicFreezeManager: React.FC<DemographicFreezeSearchFilterProps> = ({
  setInitialSearchLoaded,
  setPageReset
}) => {
  const [freezeDemographics, { isLoading }] = useFreezeDemographicsMutation();
  const profitYear = useDecemberFlowProfitYear();

  const currentYear = new Date().getFullYear();
  const previousYear = currentYear - 1;

  const {
    control,
    handleSubmit,
    formState: { errors, isValid },
    reset
  } = useForm<DemographicFreezeSearch>({
    resolver: yupResolver(schema) as Resolver<DemographicFreezeSearch>,
    defaultValues: {
      profitYear: profitYear || currentYear,
      asOfDate: null,
      asOfTime: null
    },
    mode: "onChange"
  });

  const onSubmit = handleSubmit(async (data) => {
    if (isValid) {
      try {
        // Format date and time together into a single ISO string
        const dateObj = data.asOfDate;
        const timeStr = data.asOfTime;

        // Create a date with combined date and time components
        if (dateObj && timeStr) {
          const [hours, minutes] = timeStr.split(":").map(Number);
          const combinedDate = new Date(dateObj);
          combinedDate.setHours(hours, minutes, 0, 0);

          // Format as ISO string with timezone offset
          // The format should match: "2025-03-19T00:00:00-04:00"
          const asOfDateTime = combinedDate.toISOString();

          setPageReset(true);
          await freezeDemographics({
            asOfDateTime,
            profitYear: data.profitYear
          });

          // Clear the form fields after successful freeze so the As-of date/time
          // are removed and the Create button becomes disabled (form invalid).
          reset({ profitYear: profitYear || currentYear, asOfDate: null, asOfTime: null });

          setInitialSearchLoaded(true);
          // Could add a success notification here
        }
      } catch (error) {
        console.error("Error freezing demographics:", error);
        // Could add an error notification here
      }
    }
  });

  // Style for making input fields smaller
  const fieldStyle = {
    width: "250px", // Fixed width for all fields to make them smaller
    mr: 2 // Add some right margin between fields
  };

  return (
    <DuplicateSsnGuard>
      {({ prerequisitesComplete }) => (
        <form onSubmit={onSubmit}>
          <Box
            sx={{
              display: "flex",
              flexDirection: "row",
              flexWrap: "wrap",
              alignItems: "flex-end", // Align items at the bottom for consistent baseline
              paddingX: "24px",
              gap: "24px"
            }}>
            {/* Profit Year */}
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
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
                    minDate={new Date(previousYear, 0)}
                    maxDate={new Date(currentYear, 11)}
                    error={errors.profitYear?.message}
                  />
                )}
              />
              {errors.profitYear && <FormHelperText error>{errors.profitYear.message}</FormHelperText>}
            </Grid>

            {/* As of Date */}
            <Box sx={fieldStyle}>
              <Controller
                name="asOfDate"
                control={control}
                render={({ field }) => (
                  <DsmDatePicker
                    id="asOfDate"
                    onChange={(date) => field.onChange(date)}
                    value={field.value}
                    required={true}
                    label="As of Date"
                    minDate={new Date(previousYear, 0, 1)}
                    maxDate={new Date()}
                    error={errors.asOfDate?.message}
                  />
                )}
              />
            </Box>

            {/* As of Time */}
            <Box sx={fieldStyle}>
              <Controller
                name="asOfTime"
                control={control}
                render={({ field }) => (
                  <>
                    <FormLabel>As of Time (HH:MM)</FormLabel>
                    <TextField
                      id="asOfTime"
                      type="time"
                      required
                      fullWidth
                      onChange={field.onChange}
                      value={field.value || ""}
                      error={!!errors.asOfTime}
                    />
                  </>
                )}
              />
              {errors.asOfTime && <FormHelperText error>{errors.asOfTime.message}</FormHelperText>}
            </Box>

            {/* Submit Button */}
            <Box sx={{ mt: 2, mb: 2 }}>
              <Button
                type="submit"
                variant="contained"
                color="primary"
                disabled={isLoading || !isValid || !prerequisitesComplete}>
                {isLoading ? "Submitting..." : "Create Freeze Point"}
              </Button>
            </Box>
          </Box>
        </form>
      )}
    </DuplicateSsnGuard>
  );
};

export default DemographicFreezeManager;
