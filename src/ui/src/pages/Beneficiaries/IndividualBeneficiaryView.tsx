import { BeneficiaryDetail, BeneficiaryDetailAPIRequest, BeneficiaryDto } from "@/types";
import { Button } from "@mui/material";
import { useCallback, useState } from "react";
import { useLazyGetBeneficiaryDetailQuery } from "reduxstore/api/BeneficiariesApi";
import BeneficiaryRelationshipsGrids from "./BeneficiaryRelationshipsGrids";
import CreateBeneficiaryDialog from "./CreateBeneficiaryDialog";
import MemberDetailsPanel from "./MemberDetailsPanel";

interface IndividualBeneficiaryViewProps {
  selectedMember: BeneficiaryDetail;
  memberType: number | undefined;
  onBeneficiarySelect?: (beneficiary: BeneficiaryDetail) => void;
}

const IndividualBeneficiaryView: React.FC<IndividualBeneficiaryViewProps> = ({
  selectedMember,
  memberType,
  onBeneficiarySelect
}) => {
  const [openCreateDialog, setOpenCreateDialog] = useState(false);
  const [selectedBeneficiary, setSelectedBeneficiary] = useState<BeneficiaryDto | undefined>();
  const [beneficiaryDialogTitle, setBeneficiaryDialogTitle] = useState<string>();
  const [change, setChange] = useState<number>(0);
  const [existingBeneficiaries, setExistingBeneficiaries] = useState<BeneficiaryDto[]>([]);
  const [triggerBeneficiaryDetail] = useLazyGetBeneficiaryDetailQuery();

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

  const handleBeneficiariesChange = (beneficiaries: BeneficiaryDto[]) => {
    setExistingBeneficiaries(beneficiaries);
  };

  const handleBadgeClick = useCallback(
    (beneficiary: BeneficiaryDto) => {
      const request: BeneficiaryDetailAPIRequest = {
        badgeNumber: beneficiary.badgeNumber,
        psnSuffix: beneficiary.psnSuffix,
        isSortDescending: true,
        skip: 0,
        sortBy: "psnSuffix",
        take: 25
      };

      triggerBeneficiaryDetail(request)
        .unwrap()
        .then((res) => {
          if (onBeneficiarySelect) {
            onBeneficiarySelect(res);
          }
        })
        .catch((error) => {
          console.error("Failed to fetch beneficiary details:", error);
        });
    },
    [triggerBeneficiaryDetail, onBeneficiarySelect]
  );

  return (
    <>
      <CreateBeneficiaryDialog
        open={openCreateDialog}
        onClose={handleClose}
        title={beneficiaryDialogTitle ?? ""}
        selectedBeneficiary={selectedBeneficiary}
        selectedMember={selectedMember}
        badgeNumber={selectedMember?.badgeNumber ?? 0}
        psnSuffix={selectedMember?.psnSuffix ?? 0}
        onSaveSuccess={onBeneficiarySaveSuccess}
        existingBeneficiaries={existingBeneficiaries}
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
        onBeneficiariesChange={handleBeneficiariesChange}
        onBadgeClick={handleBadgeClick}
      />
    </>
  );
};

export default IndividualBeneficiaryView;
