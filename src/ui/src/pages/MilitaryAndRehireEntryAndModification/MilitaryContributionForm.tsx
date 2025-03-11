import { Button, FormLabel, TextField } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { useEffect } from "react";
import { useForm, Controller } from "react-hook-form";
import { MilitaryContribution } from "reduxstore/types";



interface FormData {
  rows: MilitaryContribution[];
}

interface MilitaryContributionFormProps {
  onSubmit: (rows: MilitaryContribution[]) => void;
  onCancel: () => void;
  initialData?: MilitaryContribution[];
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
      rows: Array(5).fill({ contributionDate: null, contributionAmount: null })
    }
  });

  useEffect(() => {
    if (initialData) {
      const sortedData = [...initialData]
        .sort((a, b) => {
          if (!a.contributionDate || !b.contributionDate) return 0;
          return b.contributionDate.getTime() - a.contributionDate.getTime();
        })
        .slice(0, 5);

      const paddedData = [
        ...sortedData,
        ...Array(5 - sortedData.length).fill({ contributionDate: null, contributionAmount: null })
      ];

      reset({ rows: paddedData });
    }
  }, [initialData, reset]);

  const handleFormSubmit = (data: FormData) => {
    const validContributions = data.rows.filter(
      row => row.contributionDate && row.contributionAmount !== null
    );
    onSubmit(validContributions);
  };

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)}>
      <Grid2 container spacing={3}>
        {Array.from({ length: 5 }).map((_, index) => (
          <Grid2 container key={index} spacing={2}>
            <Grid2 xs={6}>
              <Controller
                name={`rows.${index}.contributionDate`}
                control={control}
                render={({ field, fieldState: { error } }) => (
                  <DsmDatePicker
                    id={`contributionDate-${index}`}
                    label="Contribution Date"
                    onChange={(value: Date | null) => field.onChange(value)}
                    value={field.value ?? null}
                    error={error?.message}
                    required={false}
                  />
                )}
              />
            </Grid2>
            <Grid2 xs={6}>
              <FormLabel>Contribution Amount</FormLabel>
              <Controller
                name={`rows.${index}.contributionAmount`}
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
        ))}

        <Grid2 container xs={12} spacing={2} paddingTop='8px'>
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