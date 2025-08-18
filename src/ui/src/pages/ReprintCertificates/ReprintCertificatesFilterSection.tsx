import { TextField, Grid, FormControl, FormLabel } from "@mui/material";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import { SearchAndReset } from "smart-ui-library";

export interface ReprintCertificatesFilterParams {
  employeeNumber: string;
  name: string;
  socialSecurityNumber: string;
}

interface ReprintCertificatesFilterSectionProps {
  onFilterChange: (params: ReprintCertificatesFilterParams) => void;
  onReset: () => void;
  isLoading?: boolean;
}

const ReprintCertificatesFilterSection: React.FC<ReprintCertificatesFilterSectionProps> = ({
  onFilterChange,
  onReset,
  isLoading = false
}) => {
  const { control, handleSubmit, reset } = useForm<ReprintCertificatesFilterParams>({
    defaultValues: {
      employeeNumber: "",
      name: "",
      socialSecurityNumber: ""
    }
  });

  const validateAndSubmit = handleSubmit((data) => {
    onFilterChange(data);
  });

  const handleReset = () => {
    reset();
    onReset();
  };

  return (
    <form onSubmit={validateAndSubmit}>
      <Grid
        container
        paddingX="24px">
        <Grid
          container
          spacing={3}
          width="100%"
          paddingTop="16px">
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel>Employee Number / PSN</FormLabel>
            <FormControl fullWidth>
              <Controller
                name="employeeNumber"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    size="small"
                    fullWidth
                  />
                )}
              />
            </FormControl>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel>Name</FormLabel>
            <FormControl fullWidth>
              <Controller
                name="name"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    size="small"
                    fullWidth
                  />
                )}
              />
            </FormControl>
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel>Social Security Number</FormLabel>
            <FormControl fullWidth>
              <Controller
                name="socialSecurityNumber"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    size="small"
                    fullWidth
                  />
                )}
              />
            </FormControl>
          </Grid>
        </Grid>
      </Grid>

      <Grid
        width="100%"
        paddingX="24px"
        paddingY="16px">
        <SearchAndReset
          handleReset={handleReset}
          handleSearch={validateAndSubmit}
          isFetching={isLoading}
        />
      </Grid>
    </form>
  );
};

export default ReprintCertificatesFilterSection;
