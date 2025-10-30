import { CloseSharp } from "@mui/icons-material";
import { IconButton } from "@mui/material";
import Dialog from "@mui/material/Dialog";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import { BeneficiaryDto } from "@/types";
import CreateBeneficiary from "./CreateBeneficiary";

interface CreateBeneficiaryDialogProps {
  open: boolean;
  onClose: () => void;
  title: string;
  selectedBeneficiary?: BeneficiaryDto;
  badgeNumber: number;
  psnSuffix: number;
  onSaveSuccess: () => void;
}

const CreateBeneficiaryDialog: React.FC<CreateBeneficiaryDialogProps> = ({
  open,
  onClose,
  title,
  selectedBeneficiary,
  badgeNumber,
  psnSuffix,
  onSaveSuccess
}) => {
  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>{title}</DialogTitle>
      <IconButton
        aria-label="close"
        onClick={onClose}
        sx={(theme) => ({
          position: "absolute",
          right: 8,
          top: 8,
          color: theme.palette.grey[500]
        })}>
        <CloseSharp />
      </IconButton>
      <DialogContent>
        <CreateBeneficiary
          selectedBeneficiary={selectedBeneficiary}
          badgeNumber={badgeNumber}
          psnSuffix={psnSuffix}
          onSaveSuccess={onSaveSuccess}
        />
      </DialogContent>
    </Dialog>
  );
};

export default CreateBeneficiaryDialog;
