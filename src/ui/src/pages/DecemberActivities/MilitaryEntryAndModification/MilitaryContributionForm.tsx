import { Button, FormLabel, TextField } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { useEffect } from "react";
import { useForm, Controller } from "react-hook-form";
import { MilitaryContribution } from "reduxstore/types";

interface FormData {
  contributionDate: Date | null;
  contributionAmount: number | null;
}

interface MilitaryContributionFormProps {
  onSubmit: (contribution: MilitaryContribution) => void;
  onCancel: () => void;
  initialData?: MilitaryContribution;
  isLoading?: boolean;
}

const MilitaryContributionForm = ({
                                    onSubmit,
                                    onCancel,
                                    initialData,
                                    isLoading = false
                                  }: MilitaryContributionFormProps) => {
  const { control, handleSubmit, reset } = useForm<FormData>({
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

  const handleFormSubmit = (data: FormData) => {
    if (data.contributionDate && data.contributionAmount !== null) {
      onSubmit(data);
    }
  };

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)}>
      <Grid2 container spacing={3}>
        <Grid2 container spacing={2}>
          <Grid2 size={{ xs: 6 }} >
            <Controller
              name="contributionDate"
              control={control}
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
            <FormLabel>Contribution Amount</FormLabel>
            <Controller
              name="contributionAmount"
              control={control}
              render={({ field, fieldState: { error } }) => (
                <TextField
                  {...field}
                  fullWidth
                  type="number"
                  variant="outlined"
                  error={!!error}
                  helperText={error?.message}
                  onChange={(e) => {
                    const value = e.target.value;
                    field.onChange(value === "" ? null : Number(value));
                  }}
                  value={field.value ?? ""}
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
              disabled={isLoading}>
              Save
            </Button>
          </Grid2>
          <Grid2>
            <Button
              variant="outlined"
              onClick={onCancel}
              disabled={isLoading}>
              Cancel
            </Button>
          </Grid2>
        </Grid2>
      </Grid2>
    </form>
  );
};

export default MilitaryContributionForm;