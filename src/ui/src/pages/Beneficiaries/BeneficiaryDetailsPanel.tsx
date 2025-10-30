import { Grid, Typography } from "@mui/material";
import { BeneficiaryDetail } from "@/types";
import { mmDDYYFormat } from "@/utils/dateUtils";

interface BeneficiaryDetailsPanelProps {
  selectedMember: BeneficiaryDetail;
}

const BeneficiaryDetailsPanel: React.FC<BeneficiaryDetailsPanelProps> = ({ selectedMember }) => {
  return (
    <>
      <Typography
        variant="h2"
        sx={{ color: "#0258A5", paddingTop: 10 }}>
        {`Beneficiary Details`}
      </Typography>
      <Grid container spacing={5}>
        <Grid size={6}>
          <p>
            <strong>{selectedMember?.name}</strong>
          </p>
          <p>
            {selectedMember?.street}
            <br />
            {selectedMember?.city} {selectedMember?.state} {selectedMember?.zip}
          </p>
        </Grid>
        <Grid size={6}>
          <p>
            <strong>DOB</strong> {mmDDYYFormat(selectedMember.dateOfBirth)}
          </p>
          <p>
            <strong>SSN</strong> {selectedMember.ssn}
          </p>
          <p>
            <strong>Balance</strong> {selectedMember.currentBalance}
          </p>
        </Grid>
      </Grid>
    </>
  );
};

export default BeneficiaryDetailsPanel;
