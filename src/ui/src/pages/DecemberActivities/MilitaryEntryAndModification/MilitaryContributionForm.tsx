import { Button, TextField } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { useEffect } from "react";
import { useForm, Controller } from "react-hook-form";
import { useCreateMilitaryContributionMutation } from "reduxstore/api/MilitaryApi";
import { CreateMilitaryContributionRequest, MilitaryContribution } from "reduxstore/types";

interface FormData {
  contributionDate: Date | null;
  contributionAmount: number | null;
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

  const { control, handleSubmit, reset, formState } = useForm<FormData>({
    defaultValues: {
      contributionDate: null,
      contributionAmount: null
    }
  });

  useEffect(() => {
    if (initialData) {
      reset({
        contributionDate: initialData.contributionDate,
        contributionAmount: initialData.contributionAmount
      });
    }
  }, [initialData, reset]);

  const handleFormSubmit = async (data: FormData) => {
    console.log("Form submitted with data:", data);

    if (data.contributionDate && data.contributionAmount !== null) {
      console.log("Data validation passed");

      const contribution: MilitaryContribution = {
        contributionDate: data.contributionDate,
        contributionAmount: data.contributionAmount
      };

      try {
        console.log("Creating request with:", { badgeNumber, profitYear, contribution });

        const request: CreateMilitaryContributionRequest = {
          badgeNumber,
          profitYear,
          contributionDate: data.contributionDate,
          contributionAmount: data.contributionAmount
        };

        console.log("Calling API with request:", request);
        const result = await createMilitaryContribution(request).unwrap();
        console.log("API call successful, result:", result);

        onSubmit(contribution);
      } catch (error) {
        console.error("Failed to create military contribution:", error);
      }
    } else {
      console.warn("Form validation failed:", {
        date: data.contributionDate,
        amount: data.contributionAmount
      });
    }
  };

  console.log("Form state:", formState);

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)}>
      <Grid2 container spacing={3}>
        <Grid2 container spacing={2}>
          <Grid2 size={{ xs: 6 }} >
            <Controller
              name="contributionDate"
              control={control}
              rules={{ required: "Date is required" }}
              render={({ field, fieldState: { error } }) => (
                <DsmDatePicker
                  id="contributionDate"
                  label="Contribution Date"
                  onChange={(value: Date | null) => field.onChange(value)}
                  value={field.value ?? null}
                  error={error?.message}
                  required={true}
                  views={["year", "month"]}
                />
              )}
            />
          </Grid2>
          <Grid2 size={{ xs: 6 }} >
            <Controller
              name="contributionAmount"
              control={control}
              rules={{ required: "Amount is required" }}
              render={({ field, fieldState: { error } }) => (
                <TextField
                  {...field}
                  fullWidth
                  label="Contribution Amount"
                  type="number"
                  variant="outlined"
                  error={!!error}
                  helperText={error?.message}
                  onChange={(e) => {
                    const value = e.target.value;
                    field.onChange(value === "" ? null : Number(value));
                  }}
                  value={field.value ?? ""}
                  required
                />
              )}
            />
          </Grid2>
        </Grid2>

        <Grid2 size={{ xs: 12 }} container spacing={2} paddingTop='8px'>
          <Grid2>
            <Button
              variant="contained"
              type="submit"
              disabled={isLoading || isSubmitting}>
              Save
            </Button>
          </Grid2>
          <Grid2>
            <Button
              variant="outlined"
              onClick={onCancel}
              disabled={isLoading || isSubmitting}>
              Cancel
            </Button>
          </Grid2>
        </Grid2>
      </Grid2>
    </form>
  );
};

export default MilitaryContributionForm;