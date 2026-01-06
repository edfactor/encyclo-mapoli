import DeleteIcon from "@mui/icons-material/Delete";
import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Typography
} from "@mui/material";
import { numberToCurrency } from "smart-ui-library";
import { DistributionSearchResponse } from "../../../types";

interface DeleteDistributionModalProps {
  open: boolean;
  distribution: DistributionSearchResponse | null;
  onConfirm: () => Promise<void>;
  onCancel: () => void;
  isLoading?: boolean;
}

const DeleteDistributionModal = ({
  open,
  distribution,
  onConfirm,
  onCancel,
  isLoading = false
}: DeleteDistributionModalProps) => {
  const handleConfirm = async () => {
    await onConfirm();
  };

  if (!distribution) {
    return null;
  }

  return (
    <Dialog
      open={open}
      onClose={onCancel}
      maxWidth="sm"
      fullWidth
      PaperProps={{
        sx: {
          borderRadius: "8px"
        }
      }}>
      <DialogTitle
        sx={{
          display: "flex",
          alignItems: "center",
          gap: "8px",
          color: "#d32f2f",
          fontWeight: 600
        }}>
        <DeleteIcon />
        Delete Distribution
      </DialogTitle>

      <DialogContent>
        <Box sx={{ mt: 2 }}>
          <Typography
            variant="body1"
            sx={{ mb: 2 }}>
            Are you sure you want to delete this distribution for
            <strong> {distribution.fullName}?</strong>
            <br />
            <br />
            Frequency: <strong>{distribution.frequencyName}</strong>
            <br />
            Amount: <strong>{numberToCurrency(distribution.grossAmount)}</strong>
          </Typography>
        </Box>
      </DialogContent>

      <DialogActions sx={{ gap: 1, p: 2 }}>
        <Button
          variant="contained"
          color="error"
          onClick={handleConfirm}
          disabled={isLoading}
          startIcon={isLoading ? <CircularProgress size={20} /> : <DeleteIcon />}>
          {isLoading ? "DELETING..." : "DELETE"}
        </Button>
        <Button
          variant="outlined"
          onClick={onCancel}
          disabled={isLoading}>
          CANCEL
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default DeleteDistributionModal;
