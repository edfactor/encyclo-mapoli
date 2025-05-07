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
    clnNum: string;
    empNum: string;
    startingBalance: number;
    forfeitureAmount: number;
    netBalance: number;
    netVested: number;
  }) => void;
  employeeDetails?: EmployeeDetails | null;
}

const AddForfeitureModal: React.FC<AddForfeitureModalProps> = ({
  open,
  onClose,
  onSave,
  employeeDetails
}) => {
  const [formData, setFormData] = useState({
    clnNum: "",
    empNum: "",
    startingBalance: 0,
    forfeitureAmount: undefined,
    netBalance: 0,
    netVested: 0
  });
  const [updateForfeiture, { isLoading }] = useUpdateForfeitureAdjustmentMutation();
  const profitYear = useFiscalCloseProfitYear();

  useEffect(() => {
    if (employeeDetails) {
      setFormData(prevState => ({
        ...prevState,
        clnNum: employeeDetails.storeNumber.toString() || '',
        empNum: employeeDetails.badgeNumber || '',
        startingBalance: employeeDetails.currentPSAmount || 0,
        forfeitureAmount: undefined,
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
      const forfeitureAmount = name === "forfeitureAmount" ? numericValue : (formData.forfeitureAmount || 0);
      const netBalance = startingBalance - forfeitureAmount;

      setFormData({
        ...formData,
        [name]: numericValue,
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
      const clientNum = parseInt(formData.clnNum) || 0;
      const badgeNum = parseInt(formData.empNum) || 0;

      if (!badgeNum) {
        alert("Employee number is required");
        return;
      }

      const request: ForfeitureAdjustmentUpdateRequest = {
        clientNumber: clientNum,
        badgeNumber: badgeNum,
        forfeitureAmount: formData.forfeitureAmount || 0,
        profitYear: profitYear
      };

      await updateForfeiture(request).unwrap();

      onSave({
        ...formData,
        forfeitureAmount: formData.forfeitureAmount || 0
      });

      // Close the modal
      onClose();
    } catch (error) {
      console.error("Error updating forfeiture:", error);
      alert("Failed to update forfeiture adjustment. Please try again.");
    }
  };

  return (
    <SmartModal
      open={open}
      onClose={onClose}
      title="Add Forfeiture"
      actions={[
        <Button key="save" onClick={handleSave} variant="contained" color="primary" disabled={isLoading}>
          {isLoading ? "SAVING..." : "SAVE"}
        </Button>,
        <Button key="cancel" onClick={onClose} variant="outlined" disabled={isLoading}>
          CANCEL
        </Button>
      ]}
    >
      <Grid2 container spacing={2}>
        <Grid2 size={{ xs: 6 }}>
          <Typography variant="body2" gutterBottom>CLN NUM</Typography>
          <TextField
            name="clnNum"
            value={formData.clnNum}
            onChange={handleChange}
            fullWidth
            size="small"
            variant="outlined"
          />
        </Grid2>
        <Grid2 size={{ xs: 6 }}>
          <Typography variant="body2" gutterBottom>EMP NUM</Typography>
          <TextField
            name="empNum"
            value={formData.empNum}
            onChange={handleChange}
            fullWidth
            size="small"
            variant="outlined"
          />
        </Grid2>
        <Grid2 size={{ xs: 6 }}>
          <Typography variant="body2" gutterBottom>Starting Balance</Typography>
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
          <Typography variant="body2" gutterBottom>Forfeiture Amount</Typography>
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
          <Typography variant="body2" gutterBottom>Net Balance</Typography>
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
          <Typography variant="body2" gutterBottom>Net Vested</Typography>
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