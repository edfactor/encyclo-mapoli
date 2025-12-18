import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";

export interface UnsavedChangesDialogProps {
  open: boolean;
  onStay: () => void;
  onLeave: () => void;
}

/**
 * Confirmation dialog shown when user tries to navigate away with unsaved changes.
 * Provides "Stay" and "Leave" options.
 */
export const UnsavedChangesDialog: React.FC<UnsavedChangesDialogProps> = ({ open, onStay, onLeave }) => {
  return (
    <Dialog
      open={open}
      onClose={onStay}
      maxWidth="sm"
      fullWidth>
      <DialogTitle>Unsaved Changes</DialogTitle>
      <DialogContent>
        <DialogContentText>You have unsaved changes. Do you want to leave without saving?</DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button
          onClick={onStay}
          variant="outlined"
          color="primary">
          Stay
        </Button>
        <Button
          onClick={onLeave}
          variant="contained"
          color="error">
          Leave Without Saving
        </Button>
      </DialogActions>
    </Dialog>
  );
};
