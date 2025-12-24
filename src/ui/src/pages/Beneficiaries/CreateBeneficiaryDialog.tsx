import { BeneficiaryDetail, BeneficiaryDto } from "@/types";
import { CloseSharp } from "@mui/icons-material";
import { IconButton } from "@mui/material";
import Dialog from "@mui/material/Dialog";
import DialogContent from "@mui/material/DialogContent";
import DialogTitle from "@mui/material/DialogTitle";
import CreateBeneficiary from "./CreateBeneficiary";

interface CreateBeneficiaryDialogProps {
  open: boolean;
  onClose: () => void;
  title: string;
  selectedBeneficiary?: BeneficiaryDto;
  selectedMember: BeneficiaryDetail;
  badgeNumber: number;
  psnSuffix: number;
  onSaveSuccess: () => void;
  existingBeneficiaries?: BeneficiaryDto[];
}

const CreateBeneficiaryDialog: React.FC<CreateBeneficiaryDialogProps> = ({
  open,
  onClose,
  title,
  selectedBeneficiary,
  selectedMember,
  badgeNumber,
  psnSuffix,
  onSaveSuccess,
  existingBeneficiaries
}) => {
  return (
    <Dialog
      open={open}
      onClose={(_event, reason) => {
        if (reason === "backdropClick" || reason === "escapeKeyDown") {
          return;
        }
        onClose();
      }}
      disableEscapeKeyDown>
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
          selectedMember={selectedMember}
          badgeNumber={badgeNumber}
          psnSuffix={psnSuffix}
          onSaveSuccess={onSaveSuccess}
          existingBeneficiaries={existingBeneficiaries}
        />
      </DialogContent>
    </Dialog>
  );
};

export default CreateBeneficiaryDialog;
