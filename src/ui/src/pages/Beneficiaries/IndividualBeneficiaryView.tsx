import { BeneficiaryDetail, BeneficiaryDetailAPIRequest, BeneficiaryDto } from "@/types";
import { useCallback, useState } from "react";
import { useLazyGetBeneficiaryDetailQuery } from "reduxstore/api/BeneficiariesApi";
import { BENEFICIARY_INQUIRY_MESSAGES } from "../../components/MissiveAlerts/MissiveMessages";
import { useMissiveAlerts } from "../../hooks/useMissiveAlerts";
import type { EmployeeDetails } from "../../types/employee/employee";
import MasterInquiryMemberDetails from "../InquiriesAndAdjustments/MasterInquiry/MasterInquiryMemberDetails";
import BeneficiaryRelationshipsGrids from "./BeneficiaryRelationshipsGrids";
import CreateBeneficiaryDialog from "./CreateBeneficiaryDialog";

interface IndividualBeneficiaryViewProps {
  selectedMember: BeneficiaryDetail;
  memberDetails: EmployeeDetails | null;
  isFetchingMemberDetails: boolean;
  profitYear: number;
  onBeneficiarySelect?: (beneficiary: BeneficiaryDetail) => void;
}

const IndividualBeneficiaryView: React.FC<IndividualBeneficiaryViewProps> = ({
  selectedMember,
  memberDetails,
  isFetchingMemberDetails,
  profitYear,
  onBeneficiarySelect
}) => {
  const [openCreateDialog, setOpenCreateDialog] = useState(false);
  const [selectedBeneficiary, setSelectedBeneficiary] = useState<BeneficiaryDto | undefined>();
  const [beneficiaryDialogTitle, setBeneficiaryDialogTitle] = useState<string>();
  const [change, setChange] = useState<number>(0);
  const [existingBeneficiaries, setExistingBeneficiaries] = useState<BeneficiaryDto[]>([]);
  const [triggerBeneficiaryDetail] = useLazyGetBeneficiaryDetailQuery();
  const { addAlert } = useMissiveAlerts();

  // Calculate memberType based on psnSuffix: 0 = employee (type 1), else beneficiary (type 2)
  const calculatedMemberType = selectedMember.psnSuffix === 0 ? 1 : 2;

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
        id: beneficiary.id,
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
          addAlert(BENEFICIARY_INQUIRY_MESSAGES.MEMBER_NOT_FOUND);
        });
    },
    [triggerBeneficiaryDetail, onBeneficiarySelect, addAlert]
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
      <MasterInquiryMemberDetails
        memberType={calculatedMemberType}
        id={selectedMember.id}
        profitYear={profitYear}
        memberDetails={memberDetails}
        isLoading={isFetchingMemberDetails}
      />

      <BeneficiaryRelationshipsGrids
        count={change}
        selectedMember={selectedMember}
        onEditBeneficiary={createOrUpdateBeneficiary}
        onAddBeneficiary={() => createOrUpdateBeneficiary(undefined)}
        onBeneficiariesChange={handleBeneficiariesChange}
        onBadgeClick={handleBadgeClick}
      />
    </>
  );
};

export default IndividualBeneficiaryView;
