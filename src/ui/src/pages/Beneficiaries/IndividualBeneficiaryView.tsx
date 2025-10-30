import { BeneficiaryDetail, BeneficiaryDto } from "@/types";
import { Button } from "@mui/material";
import { useState } from "react";
import BeneficiaryRelationshipsGrids from "./BeneficiaryRelationshipsGrids";
import CreateBeneficiaryDialog from "./CreateBeneficiaryDialog";
import MemberDetailsPanel from "./MemberDetailsPanel";

interface IndividualBeneficiaryViewProps {
  selectedMember: BeneficiaryDetail;
  memberType: number | undefined;
}

const IndividualBeneficiaryView: React.FC<IndividualBeneficiaryViewProps> = ({ selectedMember, memberType }) => {
  const [openCreateDialog, setOpenCreateDialog] = useState(false);
  const [selectedBeneficiary, setSelectedBeneficiary] = useState<BeneficiaryDto | undefined>();
  const [beneficiaryDialogTitle, setBeneficiaryDialogTitle] = useState<string>();
  const [change, setChange] = useState<number>(0);

  const handleClose = () => {
    setOpenCreateDialog(false);
  };

  const createOrUpdateBeneficiary = (data?: BeneficiaryDto) => {
    setSelectedBeneficiary(data);
    setBeneficiaryDialogTitle(data ? "Edit Beneficiary" : "Add Beneficiary");
    setOpenCreateDialog(true);
  };

  const onBeneficiarySaveSuccess = () => {
    setOpenCreateDialog(false);
    setChange((prev) => prev + 1);
  };

  return (
    <>
      <CreateBeneficiaryDialog
        open={openCreateDialog}
        onClose={handleClose}
        title={beneficiaryDialogTitle ?? ""}
        selectedBeneficiary={selectedBeneficiary}
        badgeNumber={selectedMember?.badgeNumber ?? 0}
        psnSuffix={selectedMember?.psnSuffix ?? 0}
        onSaveSuccess={onBeneficiarySaveSuccess}
      />
      <MemberDetailsPanel
        selectedMember={selectedMember}
        memberType={memberType || 0}
      />
      <div
        style={{
          padding: "24px",
          display: "flex",
          justifyContent: "right",
          alignItems: "center"
        }}>
        <Button
          variant="contained"
          color="primary"
          onClick={() => createOrUpdateBeneficiary(undefined)}>
          Add Beneficiary
        </Button>
      </div>

      <BeneficiaryRelationshipsGrids
        count={change}
        selectedMember={selectedMember}
        onEditBeneficiary={createOrUpdateBeneficiary}
      />
    </>
  );
};

export default IndividualBeneficiaryView;
