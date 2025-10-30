import { Button, CircularProgress } from "@mui/material";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";

interface DeleteBeneficiaryDialogProps {
  open: boolean;
  onConfirm: () => void;
  onCancel: () => void;
  isDeleting: boolean;
}

const DeleteBeneficiaryDialog: React.FC<DeleteBeneficiaryDialogProps> = ({
  open,
  onConfirm,
  onCancel,
  isDeleting
}) => {
  return (
    <Dialog open={open}>
      <DialogTitle>Confirmation</DialogTitle>
      <DialogContent>
        <p>Are you sure you want to delete ?</p>
      </DialogContent>
      <DialogActions>
        <Button autoFocus onClick={onCancel}>
          Cancel
        </Button>
        <Button color="error" onClick={onConfirm}>
          Delete it! &nbsp;
          {isDeleting ? <CircularProgress size="15px" color="error" /> : <></>}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default DeleteBeneficiaryDialog;
