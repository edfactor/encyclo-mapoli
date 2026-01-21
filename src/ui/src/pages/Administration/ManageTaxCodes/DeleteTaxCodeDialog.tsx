import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";
import { TaxCodeAdminDto } from "../../../types";

interface DeleteTaxCodeDialogProps {
  open: boolean;
  taxCode: TaxCodeAdminDto | null;
  isDeleting: boolean;
  onConfirm: () => void;
  onCancel: () => void;
}

export const DeleteTaxCodeDialog = ({ open, taxCode, isDeleting, onConfirm, onCancel }: DeleteTaxCodeDialogProps) => {
  return (
    <Dialog
      open={open}
      onClose={onCancel}
      maxWidth="sm"
      fullWidth>
      <DialogTitle>Delete Tax Code</DialogTitle>
      <DialogContent>
        <DialogContentText>
          {taxCode
            ? `Are you sure you want to delete tax code ${taxCode.id} - ${taxCode.name}?`
            : "Are you sure you want to delete this tax code?"}
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button
          onClick={onCancel}
          disabled={isDeleting}>
          Cancel
        </Button>
        <Button
          onClick={onConfirm}
          variant="contained"
          color="error"
          disabled={isDeleting}>
          {isDeleting ? "Deleting..." : "Delete"}
        </Button>
      </DialogActions>
    </Dialog>
  );
};
