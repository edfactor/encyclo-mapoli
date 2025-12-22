import LabelValueSection from "@/components/LabelValueSection";
import { BeneficiaryDetail } from "@/types";
import { Grid, Typography } from "@mui/material";

interface MemberDetailsPanelProps {
  selectedMember: BeneficiaryDetail;
  memberType: number;
}

const MemberDetailsPanel: React.FC<MemberDetailsPanelProps> = ({ selectedMember, memberType }) => {
  const nameAddressSection = [
    { label: "Name", value: selectedMember.fullName },
    { label: "Address", value: selectedMember.street },
    { label: "City", value: selectedMember.city },
    { label: "State", value: selectedMember.state },
    { label: "Zip", value: selectedMember.zip }
  ];

  const secondSection = [
    { label: "SSN", value: selectedMember.ssn },
    { label: "Badge Number", value: selectedMember.badgeNumber },
    { label: "Age", value: selectedMember.age?.toString() ?? "N/A" }
  ];

  return (
    <Grid
      container
      paddingX="24px"
      width={"100%"}>
      <Grid size={{ xs: 12 }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", marginY: "8px" }}>
          {memberType === 1 ? `Employee Details` : `Beneficiary Details`}
        </Typography>
      </Grid>

      <Grid size={{ xs: 12 }}>
        <Grid
          container
          spacing={3}>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <LabelValueSection data={nameAddressSection} />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 3 }}>
            <LabelValueSection data={secondSection} />
          </Grid>
        </Grid>
      </Grid>
    </Grid>
  );
};

export default MemberDetailsPanel;
