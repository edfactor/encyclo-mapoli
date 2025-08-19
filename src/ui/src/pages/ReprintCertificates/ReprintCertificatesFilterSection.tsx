import { FormControl, FormLabel, Grid, TextField } from "@mui/material";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import { useLazyGetCertificatesReportQuery } from "reduxstore/api/YearsEndApi";
import { CertificatePrintRequest } from "reduxstore/types";
import { SearchAndReset } from "smart-ui-library";

export interface ReprintCertificatesFilterParams {
  profitYear: number;
  badgeNumber: string;
  socialSecurityNumber: string;
}

interface ReprintCertificatesFilterSectionProps {
  onFilterChange: (params: ReprintCertificatesFilterParams) => void;
  onReset: () => void;
}

const ReprintCertificatesFilterSection: React.FC<ReprintCertificatesFilterSectionProps> = ({
  onFilterChange,
  onReset
}) => {
  const selectedProfitYear = useFiscalCloseProfitYear();
  const [getCertificatesReport, { isFetching }] = useLazyGetCertificatesReportQuery();
  const {
    control,
    handleSubmit,
    reset,
    formState: { errors }
  } = useForm<ReprintCertificatesFilterParams>({
    defaultValues: {
      profitYear: selectedProfitYear,
      badgeNumber: "",
      socialSecurityNumber: ""
    }
  });

  const validateAndSubmit = handleSubmit((data) => {
    onFilterChange(data);
    const request: CertificatePrintRequest = {
      profitYear: data.profitYear,
      skip: 0,
      take: 25,
      sortBy: "badgeNumber",
      isSortDescending: false
    };

    if (data.badgeNumber) {
      const badgeNumbers = data.badgeNumber
        .split(",")
        .map((num) => parseInt(num.trim()))
        .filter((num) => !isNaN(num));
      if (badgeNumbers.length > 0) {
        request.badgeNumbers = badgeNumbers;
      }
    }

    if (data.socialSecurityNumber) {
      const ssns = data.socialSecurityNumber
        .split(",")
        .map((ssn) => parseInt(ssn.replace(/\D/g, "")))
        .filter((ssn) => !isNaN(ssn) && ssn > 0);
      if (ssns.length > 0) {
        request.ssns = ssns;
      }
    }

    getCertificatesReport(request);
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
            <Controller
              name="profitYear"
              control={control}
              rules={{
                required: "Profit Year is required"
              }}
              render={({ field }) => (
                <DsmDatePicker
                  id="profitYear"
                  label="Profit Year *"
                  views={["year"]}
                  value={field.value ? new Date(field.value, 0, 1) : null}
                  onChange={(date: Date | null) => {
                    field.onChange(date ? date.getFullYear() : selectedProfitYear);
                  }}
                  required={true}
                  error={errors.profitYear?.message}
                  minDate={new Date(2020, 0, 1)}
                  maxDate={new Date(2030, 0, 1)}
                />
              )}
            />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <FormLabel>Badge Number</FormLabel>
            <FormControl fullWidth>
              <Controller
                name="badgeNumber"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    size="small"
                    fullWidth
                    placeholder="e.g. 12345 or 12345,23456"
                    helperText="Comma-separated for multiple"
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
                    placeholder="e.g. 123456789 or 123456789,987654321"
                    helperText="Comma-separated for multiple"
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
          isFetching={isFetching}
        />
      </Grid>
    </form>
  );
};

export default ReprintCertificatesFilterSection;
