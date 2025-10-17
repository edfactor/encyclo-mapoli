import { Button, Checkbox, FormControl, FormControlLabel, FormLabel, Grid, TextField, Typography } from "@mui/material";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useCreateMilitaryContributionMutation } from "reduxstore/api/MilitaryApi";
import { CreateMilitaryContributionRequest, MilitaryContribution } from "reduxstore/types";
import { ServiceErrorResponse } from "../../../types/errors/errors";

interface FormData {
  contributionDate: Date | null;
  contributionAmount: number | null;
  isSupplementalContribution: boolean | null;
}

interface MilitaryContributionFormProps {
  onSubmit: (contribution: MilitaryContribution & { contributionYear: number }) => void;
  onCancel: () => void;
  initialData?: MilitaryContribution;
  isLoading?: boolean;
  badgeNumber: number;
  profitYear: number;
}

const MilitaryContributionForm = ({
  onSubmit,
  onCancel,
  initialData,
  isLoading = false,
  badgeNumber,
  profitYear
}: MilitaryContributionFormProps) => {
  const [createMilitaryContribution, { isLoading: isSubmitting }] = useCreateMilitaryContributionMutation();
  const [errorMessages, setErrorMessages] = useState<string[]>([]);

  const { control, handleSubmit, reset } = useForm<FormData>({
    defaultValues: {
      contributionDate: null,
      contributionAmount: null,
      isSupplementalContribution: false
    }
  });

  useEffect(() => {
    if (initialData) {
      reset({
        contributionDate: initialData.contributionDate,
        contributionAmount: initialData.contributionAmount,
        isSupplementalContribution: false
      });
    }
  }, [initialData, reset]);

  const handleFormSubmit = async (data: FormData) => {
    // Clear any existing error messages
    setErrorMessages([]);

    if (data.contributionDate && data.contributionAmount !== null) {
      const contribution: MilitaryContribution = {
        contributionDate: data.contributionDate,
        contributionAmount: data.contributionAmount,
        isSupplementalContribution: data.isSupplementalContribution || false
      };

      try {
        const request: CreateMilitaryContributionRequest & {
          onlyNetworkToastErrors?: boolean;
        } = {
          profitYear,
          badgeNumber,
          contributionDate: data.contributionDate,
          contributionAmount: data.contributionAmount,
          isSupplementalContribution: data.isSupplementalContribution || false,
          onlyNetworkToastErrors: true // Suppress validation errors, only show network errors
        };

        await createMilitaryContribution(request).unwrap();
        onSubmit({
          ...contribution,
          contributionYear: data.contributionDate.getFullYear()
        });
      } catch (error) {
        const serviceError = error as ServiceErrorResponse;
        if (serviceError?.data) {
          const errorMessages: string[] = [];

          if (Array.isArray(serviceError.data.errors)) {
            errorMessages.push("Errors:");
            serviceError.data.errors.forEach((error) => {
              // Map backend error messages to user-friendly messages
              if (error.reason.includes("already exists for this year")) {
                errorMessages.push(
                  "- There is already a contribution for that year. Please check supplemental box and resubmit if applicable."
                );
              } else if (error.reason.includes("profit year differs from contribution year")) {
                errorMessages.push("- When profit year differs from contribution year, it must be supplemental.");
              } else {
                console.warn("Backend error message:", error.reason);
                errorMessages.push(`- ${error.reason}`);
              }
            });
          }

          if (errorMessages.length > 0) {
            setErrorMessages(errorMessages);
          } else {
            setErrorMessages(["An unexpected error occurred. Please try again."]);
          }
        } else {
          setErrorMessages(["An unexpected error occurred. Please try again."]);
        }
      }
    } else {
      console.warn("Form validation failed:", {
        date: data.contributionDate,
        amount: data.contributionAmount,
        isSupplementalContribution: data.isSupplementalContribution
      });
    }
  };
  return (
    <form onSubmit={handleSubmit(handleFormSubmit)}>
      <Grid
        container
        spacing={3}>
        <Grid xs={6}>
          <Controller
            name="contributionDate"
            control={control}
            rules={{ required: "Date is required" }}
            render={({ field, fieldState: { error } }) => (
              <DsmDatePicker
                id="contributionDate"
                label="Contribution Year"
                onChange={(value: Date | null) => field.onChange(value)}
                value={field.value ?? null}
                error={error?.message}
                required={true}
                disableFuture={true}
                minDate={new Date(profitYear - 6, 0, 1)}
                views={["year"]}
              />
            )}
          />
        </Grid>

        <Grid xs={6}>
          <FormLabel>Contribution Amount</FormLabel>
          <Controller
            name="contributionAmount"
            control={control}
            rules={{ required: "Amount is required" }}
            render={({ field, fieldState: { error } }) => (
              <TextField
                {...field}
                value={field.value ?? ""}
                id="contributionAmount"
                type="number"
                error={!!error}
                helperText={error?.message}
                required
                fullWidth
              />
            )}
          />
        </Grid>

        <Grid
          size={{ xs: 12 }}
          container
          spacing={2}>
          <Controller
            name="isSupplementalContribution"
            control={control}
            render={({ field, fieldState: { error } }) => (
              <FormControl error={!!error}>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={!!field.value}
                      onChange={field.onChange}
                      onBlur={field.onBlur}
                      inputRef={field.ref}
                    />
                  }
                  label="Is Supplemental Contribution"
                />
              </FormControl>
            )}
          />
        </Grid>

        {errorMessages.length > 0 && (
          <Grid size={{ xs: 12 }}>
            <Typography
              component="div"
              variant="body1"
              sx={{ color: "#db1532" }}>
              {errorMessages.map((msg, index) => (
                <div key={index}>{msg}</div>
              ))}
            </Typography>
          </Grid>
        )}

        <Grid
          size={{ xs: 12 }}
          container
          spacing={2}
          paddingTop="8px">
          <Grid>
            <Button
              type="submit"
              variant="contained"
              disabled={isLoading || isSubmitting}>
              Submit
            </Button>
          </Grid>
          <Grid>
            <Button
              onClick={onCancel}
              variant="outlined">
              Cancel
            </Button>
          </Grid>
        </Grid>
      </Grid>
    </form>
  );
};

export default MilitaryContributionForm;
