import { Button, TextField, Typography } from "@mui/material";
import { useState, useEffect } from "react";
import Grid2 from "@mui/material/Grid2";
import { SmartModal } from "smart-ui-library";
import { useUpdateForfeitureAdjustmentMutation } from "reduxstore/api/YearsEndApi";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { ForfeitureAdjustmentUpdateRequest, EmployeeDetails } from "reduxstore/types";

interface AddForfeitureModalProps {
  open: boolean;
  onClose: () => void;
  onSave: (formData: {
    badgeNumber: string;
    startingBalance: number;
    forfeitureAmount: number;
    netBalance: number;
    netVested: number;
  }) => void;
  employeeDetails?: EmployeeDetails | null;
}


const handleResponseError = (error: any) => {
  const title = error?.data?.title;
  
  if (typeof title === "string") {
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

const AddForfeitureModal: React.FC<AddForfeitureModalProps> = ({ open, onClose, onSave, employeeDetails }) => {
  
  // Need top-level error strings for invalid badge number and badge numbe not found
  

  const [formData, setFormData] = useState({
    badgeNumber: "",
    startingBalance: 0,
    forfeitureAmount: "",
    netBalance: 0,
    netVested: 0
  });
  const [updateForfeiture, { isLoading }] = useUpdateForfeitureAdjustmentMutation();
  const profitYear = useFiscalCloseProfitYear();

  useEffect(() => {
    if (employeeDetails) {
      setFormData((prevState) => ({
        ...prevState,
        badgeNumber: employeeDetails.badgeNumber ? String(employeeDetails.badgeNumber) : "",
        startingBalance: employeeDetails.currentPSAmount || 0,
        forfeitureAmount: "",
        netBalance: employeeDetails.currentPSAmount || 0,
        netVested: employeeDetails.currentVestedAmount || 0
      }));
    }
  }, [employeeDetails, open]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;

    if (name === "startingBalance" || name === "forfeitureAmount") {
      const numericValue = parseFloat(value) || 0;

      const startingBalance = name === "startingBalance" ? numericValue : formData.startingBalance;
      const forfeitureAmount = name === "forfeitureAmount" ? numericValue : parseFloat(formData.forfeitureAmount) || 0;
      const netBalance = startingBalance - forfeitureAmount;

      setFormData({
        ...formData,
        [name]: name === "startingBalance" ? numericValue : value,
        netBalance
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
      const badgeNum = parseInt(formData.badgeNumber) || 0;

      if (!badgeNum) {
        alert("Badge Number is required");
        return;
      }

      const request: ForfeitureAdjustmentUpdateRequest & { suppressAllToastErrors?: boolean, onlyNetworkToastErrors?: boolean } = {
        badgeNumber: badgeNum,
        forfeitureAmount: parseFloat(formData.forfeitureAmount) || 0,
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
        ...formData,
        forfeitureAmount: parseFloat(formData.forfeitureAmount) || 0
      });

      // Close the modal
      onClose();
    } catch (error) {
      // Sometimes 500 errors go down here
      handleResponseError(error);
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
      <Grid2
        container
        spacing={2}>
        <Grid2 size={{ xs: 12 }}>
          <Typography
            variant="body2"
            gutterBottom>
            Badge Number
          </Typography>
          <TextField
            name="badgeNumber"
            value={formData.badgeNumber}
            onChange={handleChange}
            fullWidth
            size="small"
            variant="outlined"
          />
        </Grid2>
        <Grid2 size={{ xs: 6 }}>
          <Typography
            variant="body2"
            gutterBottom>
            Starting Balance
          </Typography>
          <TextField
            name="startingBalance"
            value={formData.startingBalance}
            onChange={handleChange}
            fullWidth
            size="small"
            variant="outlined"
            type="number"
          />
        </Grid2>
        <Grid2 size={{ xs: 6 }}>
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
        </Grid2>
        <Grid2 size={{ xs: 6 }}>
          <Typography
            variant="body2"
            gutterBottom>
            Net Balance
          </Typography>
          <TextField
            name="netBalance"
            value={formData.netBalance}
            onChange={handleChange}
            fullWidth
            size="small"
            variant="outlined"
            type="number"
            disabled
          />
        </Grid2>
        <Grid2 size={{ xs: 6 }}>
          <Typography
            variant="body2"
            gutterBottom>
            Net Vested
          </Typography>
          <TextField
            name="netVested"
            value={formData.netVested}
            onChange={handleChange}
            fullWidth
            size="small"
            variant="outlined"
            type="number"
          />
        </Grid2>
      </Grid2>
    </SmartModal>
  );
};

export default AddForfeitureModal;
