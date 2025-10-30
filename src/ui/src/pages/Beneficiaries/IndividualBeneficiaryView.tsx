import { BeneficiaryDetail, BeneficiaryDto } from "@/types";
import { Button } from "@mui/material";
import BeneficiaryDetailsPanel from "./BeneficiaryDetailsPanel";
import BeneficiaryRelationships from "./BeneficiaryRelationships";

interface IndividualBeneficiaryViewProps {
  selectedMember: BeneficiaryDetail;
  change: number;
  onAddBeneficiary: () => void;
  onEditBeneficiary: (beneficiary?: BeneficiaryDto) => void;
  onDeleteBeneficiary: (id: number) => void;
}

const IndividualBeneficiaryView: React.FC<IndividualBeneficiaryViewProps> = ({
  selectedMember,
  change,
  onAddBeneficiary,
  onEditBeneficiary,
  onDeleteBeneficiary
}) => {
  return (
    <>
      <BeneficiaryDetailsPanel selectedMember={selectedMember} />
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
          onClick={onAddBeneficiary}>
          Add Beneficiary
        </Button>
      </div>

      <BeneficiaryRelationships
        count={change}
        selectedMember={selectedMember}
        createOrUpdateBeneficiary={onEditBeneficiary}
        deleteBeneficiary={onDeleteBeneficiary}
      />
    </>
  );
};

export default IndividualBeneficiaryView;
