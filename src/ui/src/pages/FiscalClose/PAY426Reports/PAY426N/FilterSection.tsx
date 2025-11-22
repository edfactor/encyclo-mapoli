import { FormControl, FormLabel, Grid, MenuItem, Select, SelectChangeEvent, TextField } from "@mui/material";
import React, { useEffect, useState } from "react";
import { ReportPreset } from "reduxstore/types";
import { SearchAndReset } from "smart-ui-library";

interface FilterSectionProps {
  presets: ReportPreset[];
  currentPreset: ReportPreset | null;
  onPresetChange: (preset: ReportPreset | null) => void;
  onReset: () => void;
  onSearch: () => void;
  isLoading?: boolean;
}

const FilterSection: React.FC<FilterSectionProps> = ({
  presets,
  currentPreset,
  onPresetChange,
  onReset,
  onSearch,
  isLoading = false
}) => {
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (!isLoading) {
      setIsSubmitting(false);
    }
  }, [isLoading]);

  const handlePresetChange = (event: SelectChangeEvent<string>) => {
    const presetId = event.target.value;
    const selected = presets.find((p) => p.id === presetId) || null;
    onPresetChange(selected);
  };

  const handleApply = () => {
    if (!isSubmitting) {
      setIsSubmitting(true);
      // Trigger search in parent component
      onSearch();
    }
  };

  const handleReset = () => {
    onReset();
  };

  const getAgeRangeDisplay = () => {
    if (!currentPreset?.displayCriteria) return "";
    return currentPreset.displayCriteria.ageRange || "";
  };

  const getHoursRangeDisplay = () => {
    if (!currentPreset?.displayCriteria) return "";
    return currentPreset.displayCriteria.hoursRange || "";
  };

  const getEmployeeStatusDisplay = () => {
    if (!currentPreset?.displayCriteria) return "";
    return currentPreset.displayCriteria.employeeStatus || "";
  };

  const getPriorProfitShareDisplay = () => {
    if (!currentPreset?.displayCriteria) return "";
    return currentPreset.displayCriteria.priorProfitShare || "";
  };

  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        handleApply();
      }}>
      <Grid
        container
        paddingX="24px">
        <Grid
          container
          spacing={3}
          width="100%">
          <Grid size={{ xs: 12 }}>
            <FormLabel>PAY426N Presets</FormLabel>
            <FormControl fullWidth>
              <Select
                value={currentPreset?.id || ""}
                onChange={handlePresetChange}
                displayEmpty>
                <MenuItem value="">Select a Report</MenuItem>
                {presets.map((preset) => (
                  <MenuItem
                    key={preset.id}
                    value={preset.id}>
                    {preset.name} - {preset.description}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
        </Grid>

        <Grid
          container
          spacing={3}
          width="100%"
          paddingTop="16px">
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Age</FormLabel>
            <TextField
              value={getAgeRangeDisplay()}
              disabled
              fullWidth
              size="small"
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Profit Share Hours</FormLabel>
            <TextField
              value={getHoursRangeDisplay()}
              disabled
              fullWidth
              size="small"
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Employee Status</FormLabel>
            <TextField
              value={getEmployeeStatusDisplay()}
              disabled
              fullWidth
              size="small"
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <FormLabel>Prior Profit Share Amount</FormLabel>
            <TextField
              value={getPriorProfitShareDisplay()}
              disabled
              fullWidth
              size="small"
            />
          </Grid>
        </Grid>
      </Grid>

      <Grid
        width="100%"
        paddingX="24px"
        paddingY="16px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={handleApply}
          isFetching={isLoading || isSubmitting}
          disabled={!currentPreset || isLoading || isSubmitting}
        />
      </Grid>
    </form>
  );
};

export default FilterSection;
