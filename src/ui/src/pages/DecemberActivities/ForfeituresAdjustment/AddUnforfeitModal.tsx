import { Button, Checkbox, FormControlLabel, Grid, TextField, Typography } from "@mui/material";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState } from "react";
import { useUpdateForfeitureAdjustmentMutation } from "reduxstore/api/YearsEndApi";
import { ForfeitureAdjustmentUpdateRequest, SuggestedForfeitResponse } from "reduxstore/types";
import { SmartModal } from "smart-ui-library";
import { ConfirmationDialog } from "../../../components/ConfirmationDialog";

interface AddUnforfeitModalProps {
  open: boolean;
  onClose: () => void;
  onSave: (formData: { forfeitureAmount: number; classAction: boolean }) => void;
  suggestedForfeitResponse?: SuggestedForfeitResponse | null;
}

interface ErrorResponse {
  data?: {
    title?: string;
  };
}

const getErrorDialogContent = (error: ErrorResponse): { title: string; message: string } => {
  const title = error?.data?.title;

  if (typeof title === "string") {
    if (title.includes("Employee with badge number")) {
      return {
        title: "Badge Number Not Found",
        message: "The specified badge number could not be found in the system."
      };
    } else if (title.includes("Invalid badge number")) {
      return { title: "Invalid Badge Number", message: "The badge number you entered is not valid." };
    } else if (title.includes("Forfeiture amount cannot be zero")) {
      return { title: "Invalid Amount", message: "Unforfeit amount cannot be zero. Please enter a non-zero value." };
    } else if (title.includes("Validation Error")) {
      return {
        title: "Validation Error",
        message: "The submission contains data format errors. Please check your input and try again."
      };
    } else {
      return { title: "Error", message: "An unexpected error occurred. Please try again." };
    }
  }
  return { title: "Error", message: "An unexpected error occurred. Please try again." };
};

const AddUnforfeitModal: React.FC<AddUnforfeitModalProps> = ({ open, onClose, onSave, suggestedForfeitResponse }) => {
  const [formData, setFormData] = useState<{
    badgeNumber: number;
    forfeitureAmount: number;
    suggestedForfeitAmount: number | null;
    classAction: boolean;
  }>({
    badgeNumber: 0,
    forfeitureAmount: 0,
    suggestedForfeitAmount: null,
    classAction: false
  });
  const [errorDialog, setErrorDialog] = useState<{ title: string; message: string } | null>(null);
  const [updateForfeiture, { isLoading }] = useUpdateForfeitureAdjustmentMutation();
  const profitYear = useFiscalCloseProfitYear();

  useEffect(() => {
    if (!open) {
      setFormData({ badgeNumber: 0, forfeitureAmount: 0, suggestedForfeitAmount: null, classAction: false });
      return;
    }

    if (suggestedForfeitResponse) {
      setFormData((prev) => ({
        ...prev,
        badgeNumber: suggestedForfeitResponse.badgeNumber,
        suggestedForfeitAmount: suggestedForfeitResponse.suggestedForfeitAmount,
        forfeitureAmount: Math.abs(suggestedForfeitResponse.suggestedForfeitAmount ?? 0) // Use absolute value for unforfeit
      }));
    }
  }, [suggestedForfeitResponse, open]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;

    if (name === "forfeitureAmount") {
      const numericValue = parseFloat(value) || 0;
      setFormData({
        ...formData,
        forfeitureAmount: numericValue
      });
    } else if (name === "classAction") {
      setFormData({
        ...formData,
        classAction: (e.target as HTMLInputElement).checked
      });
    } else {
      setFormData({
        ...formData,
        [name]: value
      });
    }
  };

  const handleSave = async () => {
    try {
      const request: ForfeitureAdjustmentUpdateRequest & {
        suppressAllToastErrors?: boolean;
        onlyNetworkToastErrors?: boolean;
      } = {
        badgeNumber: formData.badgeNumber,
        forfeitureAmount: -Math.abs(formData.forfeitureAmount), // Make it negative for unforfeit
        classAction: formData.classAction,
        profitYear: profitYear,
        onlyNetworkToastErrors: true
      };

      const result = await updateForfeiture(request);

      if (result.error) {
        const errorContent = getErrorDialogContent(result.error as unknown as ErrorResponse);
        setErrorDialog(errorContent);
        return;
      }

      onSave({
        forfeitureAmount: -Math.abs(formData.forfeitureAmount), // Return negative value
        classAction: formData.classAction
      });

      onClose();
    } catch (error) {
      const errorContent = getErrorDialogContent(error as unknown as ErrorResponse);
      setErrorDialog(errorContent);
    }
  };

  return (
    <SmartModal
      open={open}
      onClose={onClose}
      title="Add Unforfeit"
      actions={[
        <Button
          key="save"
          onClick={handleSave}
          variant="contained"
          color="primary"
          disabled={isLoading}>
          {isLoading ? "SAVING..." : "SAVE"}
        </Button>,
        <Button
          key="cancel"
          onClick={onClose}
          variant="outlined"
          disabled={isLoading}>
          CANCEL
        </Button>
      ]}>
      <Grid
        container
        spacing={2}>
        <Grid size={{ xs: 12 }}>
          Suggested Unforfeit Amount: {formData.suggestedForfeitAmount ? Math.abs(formData.suggestedForfeitAmount) : 0}
        </Grid>
        <Grid size={{ xs: 12 }}>
          <FormControlLabel
            control={
              <Checkbox
                name="classAction"
                checked={formData.classAction}
                onChange={handleChange}
                color="primary"
              />
            }
            label="Class Action"
          />
        </Grid>
        <Grid size={{ xs: 6 }}>
          <Typography
            variant="body2"
            gutterBottom>
            Unforfeit Amount
          </Typography>
          <TextField
            name="forfeitureAmount"
            value={formData.forfeitureAmount}
            onChange={handleChange}
            fullWidth
            size="small"
            variant="outlined"
            type="number"
          />
        </Grid>
      </Grid>

      <ConfirmationDialog
        open={!!errorDialog}
        title={errorDialog?.title || "Error"}
        description={errorDialog?.message || "An error occurred"}
        onClose={() => setErrorDialog(null)}
      />
    </SmartModal>
  );
};

export default AddUnforfeitModal;
