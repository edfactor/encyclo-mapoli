import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";

export interface ConfirmationDialogProps {
  open: boolean;
  title: string;
  description: string;
  onClose: () => void;
}

/**
 * Reusable confirmation dialog component
 * Displays a message with an OK button
 */
export const ConfirmationDialog: React.FC<ConfirmationDialogProps> = ({ open, title, description, onClose }) => {
  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="sm"
      fullWidth>
      <DialogTitle>{title}</DialogTitle>
      <DialogContent>
        <DialogContentText>{description}</DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button
          onClick={onClose}
          variant="contained"
          color="primary">
          OK
        </Button>
      </DialogActions>
    </Dialog>
  );
};
