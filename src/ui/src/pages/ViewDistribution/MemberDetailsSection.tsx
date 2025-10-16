import { Grid, Paper, Typography } from "@mui/material";
import { numberToCurrency } from "smart-ui-library";
import { EmployeeDetails } from "../../types";

interface MemberDetailsSectionProps {
  member: EmployeeDetails;
}

const MemberDetailsSection: React.FC<MemberDetailsSectionProps> = ({ member }) => {
  // Format full name
  const fullName = `${member.lastName}, ${member.firstName}`;

  // Format address
  const addressLine1 = member.address || "";
  const addressLine2 = `${member.addressCity}, ${member.addressState} ${member.addressZipCode}`;

  // Calculate vested percentage display
  const vestedPercentageDisplay = `${(member.percentageVested * 100).toFixed(0)}%`;

  return (
    <Grid
      width="100%"
      paddingX="24px">
      <Paper
        elevation={2}
        sx={{ padding: "16px" }}>
        {/* Section Header */}
        <Typography
          variant="h5"
          sx={{
            color: "#0258A5",
            marginBottom: "24px",
            fontWeight: "600"
          }}>
          Member Details
        </Typography>

        {/* Member Details Grid - 3 columns */}
        <Grid
          container
          spacing={3}>
          {/* Column 1 */}
          <Grid size={{ xs: 12, md: 4 }}>
            {/* Member Number */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body1"
                sx={{ color: "#666", fontWeight: "500" }}>
                Member # {member.badgeNumber}
                {member.psnSuffix > 0 && `${member.psnSuffix.toString().padStart(5, "0")}`}
              </Typography>
            </Grid>

            {/* Full Name */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography variant="body1">{fullName}</Typography>
            </Grid>

            {/* Address Line 1 */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography variant="body1">{addressLine1}</Typography>
            </Grid>

            {/* Address Line 2 */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography variant="body1">{addressLine2}</Typography>
            </Grid>

            {/* Date of Birth */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography variant="body1">DOB {member.dateOfBirth}</Typography>
            </Grid>

            {/* SSN */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography variant="body1">SSN {member.ssn}</Typography>
            </Grid>
          </Grid>

          {/* Column 2 */}
          <Grid size={{ xs: 12, md: 4 }}>
            {/* YTD P/S Hours */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                YTD P/S Hours
              </Typography>
              <Typography variant="body1">{member.yearToDateProfitSharingHours}</Typography>
            </Grid>

            {/* Years in Plan */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Years in Plan
              </Typography>
              <Typography variant="body1">{member.yearsInPlan}</Typography>
            </Grid>

            {/* Percentage Vested */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Percentage Vested
              </Typography>
              <Typography variant="body1">{vestedPercentageDisplay}</Typography>
            </Grid>

            {/* Enrolled */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Enrolled
              </Typography>
              <Typography variant="body1">{member.enrollment}</Typography>
            </Grid>
          </Grid>

          {/* Column 3 */}
          <Grid size={{ xs: 12, md: 4 }}>
            {/* Begin Amount */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Begin Amount
              </Typography>
              <Typography variant="body1">{numberToCurrency(member.beginPSAmount)}</Typography>
            </Grid>

            {/* Current Amount */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Current Amount
              </Typography>
              <Typography variant="body1">{numberToCurrency(member.currentPSAmount)}</Typography>
            </Grid>

            {/* Begin Vested */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Begin Vested
              </Typography>
              <Typography variant="body1">{numberToCurrency(member.beginVestedAmount)}</Typography>
            </Grid>

            {/* Current Vested */}
            <Grid sx={{ marginBottom: "16px" }}>
              <Typography
                variant="body2"
                sx={{ color: "#666", fontWeight: "bold" }}>
                Current Vested
              </Typography>
              <Typography variant="body1">{numberToCurrency(member.currentVestedAmount)}</Typography>
            </Grid>
          </Grid>
        </Grid>
      </Paper>
    </Grid>
  );
};

export default MemberDetailsSection;
