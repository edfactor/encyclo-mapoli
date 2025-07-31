import { FormControl, FormLabel, MenuItem, Select, SelectChangeEvent, TextField } from "@mui/material";
import { Grid } from "@mui/material";
import React from "react";
import { ReportPreset } from "reduxstore/types";
import { SearchAndReset } from "smart-ui-library";

interface FilterSectionProps {
  presets: ReportPreset[];
  currentPreset: ReportPreset | null;
  onPresetChange: (preset: ReportPreset | null) => void;
  onReset: () => void;
  isLoading?: boolean;
}

const FilterSection: React.FC<FilterSectionProps> = ({
  presets,
  currentPreset,
  onPresetChange,
  onReset,
  isLoading = false
}) => {
  const handlePresetChange = (event: SelectChangeEvent<string>) => {
    const presetId = event.target.value;
    const selected = presets.find((p) => p.id === presetId) || null;
    onPresetChange(selected);
  };

  const handleApply = () => {
    // No-op as the actual data fetching is handled by the parent component
  };

  const handleReset = () => {
    onReset();
  };

  const getAgeRangeDisplay = () => {
    if (!currentPreset) return "";
    const { minimumAgeInclusive, maximumAgeInclusive } = currentPreset.params;
    if (minimumAgeInclusive && maximumAgeInclusive) {
      return `${minimumAgeInclusive}-${maximumAgeInclusive}`;
    } else if (minimumAgeInclusive) {
      return `${minimumAgeInclusive}+`;
    } else if (maximumAgeInclusive) {
      return `0-${maximumAgeInclusive}`;
    }
    return "";
  };

  const getHoursRangeDisplay = () => {
    if (!currentPreset) return "";
    const { minimumHoursInclusive, maximumHoursInclusive } = currentPreset.params;
    if (minimumHoursInclusive && maximumHoursInclusive) {
      return `${minimumHoursInclusive}-${maximumHoursInclusive}`;
    } else if (minimumHoursInclusive) {
      return `> ${minimumHoursInclusive}`;
    } else if (maximumHoursInclusive) {
      return `< ${maximumHoursInclusive}`;
    }
    return "";
  };

  const getEmployeeStatusDisplay = () => {
    if (!currentPreset) return "";
    const { includeActiveEmployees, includeInactiveEmployees } = currentPreset.params;

    if (includeActiveEmployees && includeInactiveEmployees) {
      return "Active/Inactive";
    } else if (includeActiveEmployees) {
      return "Active";
    } else if (includeInactiveEmployees) {
      return "Inactive";
    }
    return "";
  };

  const getPriorProfitShareDisplay = () => {
    if (!currentPreset) return "";
    const { includeEmployeesWithPriorProfitSharingAmounts, includeEmployeesWithNoPriorProfitSharingAmounts } =
      currentPreset.params;

    if (includeEmployeesWithPriorProfitSharingAmounts && includeEmployeesWithNoPriorProfitSharingAmounts) {
      return "All";
    } else if (includeEmployeesWithPriorProfitSharingAmounts) {
      return "With Prior Amounts";
    } else if (includeEmployeesWithNoPriorProfitSharingAmounts) {
      return "Without Prior Amounts";
    }
    return "";
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
          isFetching={isLoading}
          disabled={!currentPreset || isLoading}
        />
      </Grid>
    </form>
  );
};

export default FilterSection;
