import { yupResolver } from "@hookform/resolvers/yup";
import { FormControl, FormLabel, Grid, MenuItem, Select, SelectChangeEvent, TextField } from "@mui/material";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import { ReportPreset } from "reduxstore/types";
import { SearchAndReset } from "smart-ui-library";
import * as yup from "yup";
import { positiveNumberValidator } from "../../../utils/FormValidators";

interface FilterSectionProps {
  presets: ReportPreset[];
  currentPreset: ReportPreset | null;
  onPresetChange: (preset: ReportPreset | null) => void;
  onReset: () => void;
  onStoreNumberChange: (storeNumber: string) => void;
  isLoading?: boolean;
}

interface FilterFormData {
  storeNumber: number | null;
  startDate: Date | null;
  endDate: Date | null;
  vestedPercentage: string;
  age: string;
  employeeStatus: string;
}

const schema = yup.object().shape({
  storeNumber: positiveNumberValidator("Store Number is required").nullable().default(null),
  startDate: yup.date().nullable().default(null),
  endDate: yup.date().nullable().default(null),
  vestedPercentage: yup.string().default(""),
  age: yup.string().default(""),
  employeeStatus: yup.string().default("")
});

const FilterSection: React.FC<FilterSectionProps> = ({
  presets,
  currentPreset,
  onPresetChange,
  onReset,
  onStoreNumberChange,
  isLoading = false
}) => {
  const {
    control,
    handleSubmit,
    reset,
    watch,
    formState: { errors, isValid }
  } = useForm<FilterFormData>({
    resolver: yupResolver(schema),
    defaultValues: {
      storeNumber: null,
      startDate: null,
      endDate: null,
      vestedPercentage: "",
      age: "",
      employeeStatus: ""
    }
  });

  const storeNumber = watch("storeNumber");

  const handlePresetChange = (event: SelectChangeEvent<string>) => {
    const presetId = event.target.value;
    const selected = presets.find((p) => p.id === presetId) || null;
    onPresetChange(selected);
  };

  const handleFilter = (data: unknown) => {
    // @D
    console.log("Filter data:", data);
  };

  const handleResetForm = () => {
    reset();
    onReset();
  };
  /*
  const vestedPercentageOptions = [
    { value: "", label: "All" },
    { value: "<20", label: "< 20%" },
    { value: "20-50", label: "20-50%" },
    { value: "50-80", label: "50-80%" },
    { value: ">80", label: "> 80%" },
    { value: "100", label: "100%" }
  ];
  
  const ageOptions = [
    { value: "", label: "All" },
    { value: "<18", label: "< 18" },
    { value: "18-21", label: "18-21" },
    { value: "21-65", label: "21-65" },
    { value: ">65", label: "> 65" },
    { value: ">70", label: "> 70" }
  ];

  const employeeStatusOptions = [
    { value: "", label: "All" },
    { value: "active", label: "Active" },
    { value: "inactive", label: "Inactive" },
    { value: "terminated", label: "Terminated" }
  ];
*/
  return (
    <form onSubmit={handleSubmit(handleFilter)}>
      <Grid
        container
        paddingX="24px">
        <Grid
          container
          spacing={3}
          width="100%">
          <Grid size={{ xs: 12, sm: 6 }}>
            <Controller
              name="storeNumber"
              control={control}
              render={({ field }) => (
                <>
                  <FormLabel required>Store Number</FormLabel>
                  <TextField
                    {...field}
                    value={field.value ?? ""}
                    fullWidth
                    size="small"
                    required
                    error={!!errors.storeNumber}
                    helperText={errors.storeNumber?.message}
                    onChange={(e) => {
                      const value = e.target.value ? Number(e.target.value) : null;
                      field.onChange(value);
                      onStoreNumberChange(e.target.value);
                    }}
                  />
                </>
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <FormLabel required>QPAY066 Presets</FormLabel>
            <FormControl fullWidth>
              <Select
                value={currentPreset?.id || ""}
                onChange={handlePresetChange}
                displayEmpty
                disabled={!storeNumber}>
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
      </Grid>

      <Grid
        width="100%"
        paddingX="24px"
        paddingY="16px">
        <SearchAndReset
          handleReset={handleResetForm}
          handleSearch={handleSubmit(handleFilter)}
          isFetching={isLoading}
          disabled={!currentPreset || isLoading || !isValid}
          searchButtonText="Search"
        />
      </Grid>
    </form>
  );
};

export default FilterSection;
