import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Typography,
  CircularProgress,
  Box
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import { DistributionSearchResponse } from "../../types";
import { numberToCurrency } from "smart-ui-library";

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
            Are you sure you want to delete the following distribution for{" "}
            <strong>
              {distribution.fullName}, {distribution.badgeNumber}, {distribution.frequencyName}, and{" "}
              {numberToCurrency(distribution.grossAmount)}
            </strong>
            ?
          </Typography>
          <Typography
            variant="body2"
            color="textSecondary">
            This action cannot be undone.
          </Typography>
        </Box>
      </DialogContent>

      <DialogActions sx={{ gap: 1, p: 2 }}>
        <Button
          variant="outlined"
          onClick={onCancel}
          disabled={isLoading}>
          CANCEL
        </Button>
        <Button
          variant="contained"
          color="error"
          onClick={handleConfirm}
          disabled={isLoading}
          startIcon={isLoading ? <CircularProgress size={20} /> : <DeleteIcon />}>
          {isLoading ? "DELETING..." : "DELETE"}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default DeleteDistributionModal;
