import React from "react";
import { FormControl, Select, MenuItem, SelectChangeEvent, FormLabel } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useForm, Controller } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import SearchAndReset from "components/SearchAndReset/SearchAndReset";
import { ReportPreset } from "reduxstore/types";

interface FilterSectionProps {
  presets: ReportPreset[];
  currentPreset: ReportPreset | null;
  onPresetChange: (preset: ReportPreset | null) => void;
  onReset: () => void;
  isLoading?: boolean;
}

interface FilterFormData {
  startDate: Date | null;
  endDate: Date | null;
  vestedPercentage: string;
  age: string;
  employeeStatus: string;
}

const schema = yup.object().shape({
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
  isLoading = false
}) => {
  const {
    control,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm<FilterFormData>({
    resolver: yupResolver(schema) as any,
    defaultValues: {
      startDate: null,
      endDate: null,
      vestedPercentage: "",
      age: "",
      employeeStatus: ""
    }
  });

  const handlePresetChange = (event: SelectChangeEvent<string>) => {
    const presetId = event.target.value;
    const selected = presets.find((p) => p.id === presetId) || null;
    onPresetChange(selected);
  };

  const handleFilter = (data: FilterFormData) => {
    // @D
    console.log("Filter data:", data);
  };

  const handleResetForm = () => {
    reset();
    onReset();
  };

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

  return (
    <form onSubmit={handleSubmit(handleFilter)}>
      <Grid2
        container
        paddingX="24px">
        <Grid2
          container
          spacing={3}
          width="100%">
          <Grid2 size={{ xs: 12 }}>
            <FormLabel>QPAY066* Presets</FormLabel>
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
          </Grid2>
        </Grid2>

        <Grid2
          container
          spacing={3}
          width="100%"
          paddingTop="16px">
          <Grid2 size={{ xs: 12, sm: 6, md: 2.4 }}>
            <Controller
              name="startDate"
              control={control}
              render={({ field }) => (
                <DsmDatePicker
                  id="startDate"
                  onChange={(value: Date | null) => field.onChange(value)}
                  value={field.value}
                  required={false}
                  label="Start Date"
                  disableFuture
                  error={errors.startDate?.message}
                />
              )}
            />
          </Grid2>
          <Grid2 size={{ xs: 12, sm: 6, md: 2.4 }}>
            <Controller
              name="endDate"
              control={control}
              render={({ field }) => (
                <DsmDatePicker
                  id="endDate"
                  onChange={(value: Date | null) => field.onChange(value)}
                  value={field.value}
                  required={false}
                  label="End Date"
                  disableFuture
                  error={errors.endDate?.message}
                />
              )}
            />
          </Grid2>
          <Grid2 size={{ xs: 12, sm: 6, md: 2.4 }}>
            <FormLabel>Vested Percentage</FormLabel>
            <Controller
              name="vestedPercentage"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth>
                  <Select
                    {...field}
                    displayEmpty
                    size="small">
                    {vestedPercentageOptions.map((option) => (
                      <MenuItem
                        key={option.value}
                        value={option.value}>
                        {option.label}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              )}
            />
          </Grid2>
          <Grid2 size={{ xs: 12, sm: 6, md: 2.4 }}>
            <FormLabel>Age</FormLabel>
            <Controller
              name="age"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth>
                  <Select
                    {...field}
                    displayEmpty
                    size="small">
                    {ageOptions.map((option) => (
                      <MenuItem
                        key={option.value}
                        value={option.value}>
                        {option.label}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              )}
            />
          </Grid2>
          <Grid2 size={{ xs: 12, sm: 6, md: 2.4 }}>
            <FormLabel>Employee Status</FormLabel>
            <Controller
              name="employeeStatus"
              control={control}
              render={({ field }) => (
                <FormControl fullWidth>
                  <Select
                    {...field}
                    displayEmpty
                    size="small">
                    {employeeStatusOptions.map((option) => (
                      <MenuItem
                        key={option.value}
                        value={option.value}>
                        {option.label}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              )}
            />
          </Grid2>
        </Grid2>
      </Grid2>

      <Grid2
        width="100%"
        paddingX="24px"
        paddingY="16px">
        <SearchAndReset
          handleReset={handleResetForm}
          handleSearch={handleSubmit(handleFilter)}
          isFetching={isLoading}
          disabled={!currentPreset || isLoading}
          searchButtonText="Filter"
        />
      </Grid2>
    </form>
  );
};

export default FilterSection;
