import { Button, Checkbox, FormControlLabel, Grid, TextField, Typography } from "@mui/material";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState } from "react";
import { useUpdateForfeitureAdjustmentMutation } from "reduxstore/api/YearsEndApi";
import { ForfeitureAdjustmentUpdateRequest, SuggestedForfeitResponse } from "reduxstore/types";
import { SmartModal } from "smart-ui-library";
import { ServiceErrorResponse } from "../../types/errors/errors";

interface AddForfeitureModalProps {
  open: boolean;
  onClose: () => void;
  onSave: (formData: { forfeitureAmount: number; classAction: boolean }) => void;
  suggestedForfeitResponse?: SuggestedForfeitResponse | null;
}

const handleResponseError = (error: ServiceErrorResponse) => {
  const title = error?.data?.title;

  if (title) {
    if (title.includes("Employee with badge number")) {
      alert("Badge Number not found");
    } else if (title.includes("Invalid badge number")) {
      alert("Invalid Badge Number");
    } else if (title.includes("Forfeiture amount cannot be zero")) {
      alert("Forfeiture amount cannot be zero");
    } else if (title.includes("Validation Error")) {
      alert("The submission contains data format errors.");
    } else {
      alert("An unexpected error occurred. Please try again.");
    }
  } else {
    alert("An unexpected error occurred. Please try again.");
  }
};

const AddForfeitureModal: React.FC<AddForfeitureModalProps> = ({ open, onClose, onSave, suggestedForfeitResponse }) => {
  const [formData, setFormData] = useState({
    badgeNumber: 0,
    forfeitureAmount: 0,
    classAction: false
  });
  const [updateForfeiture, { isLoading }] = useUpdateForfeitureAdjustmentMutation();
  const profitYear = useFiscalCloseProfitYear();

  useEffect(() => {
    if (!open) {
      setFormData({ badgeNumber: 0, forfeitureAmount: 0, classAction: false });
      return;
    }

    if (suggestedForfeitResponse) {
      setFormData((prev) => ({
        ...prev,
        badgeNumber: suggestedForfeitResponse.badgeNumber,
        forfeitureAmount: suggestedForfeitResponse.suggestedForfeitAmount
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
        forfeitureAmount: formData.forfeitureAmount,
        classAction: formData.classAction,
        profitYear: profitYear,
        //suppressAllToastErrors: false,
        onlyNetworkToastErrors: true // Suppress validation errors, only show network errors
      };

      const result = await updateForfeiture(request);

      // If the response has an error block, handle it
      if (result.error) {
        handleResponseError(result.error);
        return;
      }

      onSave({
        forfeitureAmount: formData.forfeitureAmount,
        classAction: formData.classAction
      });

      // Close the modal
      onClose();
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (_error: any) {
      // This needs to be called with a blank set of properties to satisfy the type
      handleResponseError({} as ServiceErrorResponse);
    }
  };

  return (
    <SmartModal
      open={open}
      onClose={onClose}
      title="Add Forfeiture"
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
            Forfeiture Amount
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
    </SmartModal>
  );
};

export default AddForfeitureModal;
