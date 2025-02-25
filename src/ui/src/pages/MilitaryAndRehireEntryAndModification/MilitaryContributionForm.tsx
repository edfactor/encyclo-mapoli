import { Button, FormHelperText, FormLabel, TextField } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { Controller, useForm } from "react-hook-form";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import DsmDatePicker from "components/DsmDatePicker/DsmDatePicker";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { useCreateMilitaryContributionMutation } from "reduxstore/api/MilitaryApi";

interface MilitaryContributionFormProps {
  onSubmit: () => void;
  onCancel: () => void;
}

interface ContributionRow {
  contributionDate: Date;
  contributionAmount: string;
}

interface MilitaryContribution {
  rows: ContributionRow[];
}

const rowSchema = yup.object().shape({
  contributionDate: yup.date().required("Contribution date is required"),
  contributionAmount: yup
    .string()
    .required("Amount is required")
    .matches(/^\d*\.?\d{0,2}$/, "Must be a valid dollar amount")
});

const schema = yup.object().shape({
  rows: yup.array().of(rowSchema).required().min(1)
});

const MilitaryContributionForm = ({ onSubmit, onCancel }: MilitaryContributionFormProps) => {
  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.yearsEnd);
  const [createContribution, { isLoading }] = useCreateMilitaryContributionMutation();

  const { control, handleSubmit, formState: { errors } } = useForm<MilitaryContribution>({
    resolver: yupResolver(schema),
    defaultValues: {
      rows: Array(5).fill({
        contributionDate: new Date(),
        contributionAmount: ""
      })
    }
  });

  const handleFormSubmit = async (data: MilitaryContribution) => {
    if (!masterInquiryEmployeeDetails?.badgeNumber) return;

    try {
      // Filter out empty rows
      const filledRows = data.rows.filter(row => row.contributionAmount.trim() !== "");
      
      // Create contributions for each filled row
      await Promise.all(filledRows.map(row => 
        createContribution({
          badgeNumber: parseInt(masterInquiryEmployeeDetails.badgeNumber),
          contributionAmount: parseFloat(row.contributionAmount),
          profitYear: 2024 // Default to 2024
        }).unwrap()
      ));

      onSubmit();
    } catch (error) {
      console.error('Failed to create military contribution:', error);
    }
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
                render={({ field }) => (
                  <DsmDatePicker
                    id={`contributionDate-${index}`}
                    label="Contribution Date"
                    onChange={(value: Date | null) => field.onChange(value)}
                    value={field.value ?? null}
                    error={errors.rows?.[index]?.contributionDate?.message}
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
                render={({ field }) => (
                  <TextField
                    {...field}
                    fullWidth
                    variant="outlined"
                    error={!!errors.rows?.[index]?.contributionAmount}
                    helperText={errors.rows?.[index]?.contributionAmount?.message}
                    inputProps={{
                      inputMode: "decimal",
                      pattern: "^\\d*\\.?\\d{0,2}$"
                    }}
                  />
                )}
              />
            </Grid2>
          </Grid2>
        ))}

        <Grid2 container xs={12} justifyContent="flex-end" spacing={2}>
          <Grid2>
            <Button
              variant="outlined"
              onClick={onCancel}
              disabled={isLoading}>
              Cancel
            </Button>
          </Grid2>
          <Grid2>
            <Button
              variant="contained"
              type="submit"
              disabled={isLoading}>
              Save
            </Button>
          </Grid2>
        </Grid2>
      </Grid2>
    </form>
  );
};

export default MilitaryContributionForm;