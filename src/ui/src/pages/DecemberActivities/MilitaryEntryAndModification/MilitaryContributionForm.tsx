import { Button, Checkbox, FormControl, FormControlLabel, FormLabel, Grid, TextField, Typography } from "@mui/material";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useCreateMilitaryContributionMutation } from "reduxstore/api/MilitaryApi";
import { CreateMilitaryContributionRequest, MilitaryContribution } from "reduxstore/types";

interface FormData {
  contributionDate: Date | null;
  contributionAmount: number | null;
  isSupplementalContribution: boolean | null;
}

interface MilitaryContributionFormProps {
  onSubmit: (contribution: MilitaryContribution) => void;
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
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

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
    console.log("Form submitted with data:", data);

    // Clear any existing error message
    setErrorMessage(null);

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

        onSubmit(contribution);
      } catch (error) {
        if (
          typeof error === "object" &&
          error !== null &&
          "data" in error &&
          (error as any).data?.errors?.[0]?.reason
        ) {
          if (
            (error as any).data.errors[0].reason ===
            "Regular Contribution already recorded for Year. Duplicates are not allowed."
          ) {
            setErrorMessage("There is already a contribution for that year.");
          }
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

        {errorMessage && (
          <Grid size={{ xs: 12 }}>
            <Typography
              variant="body1"
              sx={{ color: "#db1532" }}>
              {errorMessage}
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
