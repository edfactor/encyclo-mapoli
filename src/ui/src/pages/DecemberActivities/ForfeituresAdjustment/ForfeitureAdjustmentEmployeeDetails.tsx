import { Grid, Typography } from "@mui/material";
import LabelValueSection from "components/LabelValueSection";
import React from "react";
import { EmployeeDetails } from "reduxstore/types";
import { formatPercentage } from "utils/formatPercentage";

interface ForfeitureAdjustmentEmployeeDetailsProps {
  details: EmployeeDetails;
}

const ForfeitureAdjustmentEmployeeDetails: React.FC<ForfeitureAdjustmentEmployeeDetailsProps> = ({ details }) => {
  const {
    firstName,
    lastName,
    address,
    addressCity,
    addressState,
    addressZipCode,
    dateOfBirth,
    ssn,
    yearToDateProfitSharingHours,
    yearsInPlan,
    percentageVested,
    contributionsLastYear,
    enrollmentId,
    badgeNumber
  } = details;

  const personalInfoSection = [
    { label: "", value: `${lastName}, ${firstName}` },
    { label: "", value: address },
    { label: "", value: `${addressCity}, ${addressState} ${addressZipCode}` },
    { label: "DOB", value: dateOfBirth },
    { label: "SSN", value: ssn }
  ];

  const planInfoSection = [
    { label: "YTD P/S Hours", value: yearToDateProfitSharingHours },
    { label: "Years in plan", value: yearsInPlan },
    { label: "Percentage vested", value: `${formatPercentage(percentageVested)} * 2` },
    { label: "Cont Last Year", value: contributionsLastYear ? "Yes" : "No" }
  ];

  const enrollmentSection = [
    { label: "Enrolled", value: enrollmentId ? "Yes" : "No" },
    { label: "Badge", value: badgeNumber }
  ];

  return (
    <Grid
      container
      paddingX="24px"
      width={"100%"}
      sx={{ marginBottom: "24px" }}>
      <Grid size={{ xs: 12 }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5", marginBottom: "16px" }}>
          Employee Details
        </Typography>
      </Grid>

      <Grid size={{ xs: 12 }}>
        <Grid
          container
          spacing={3}>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <LabelValueSection data={personalInfoSection} />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <LabelValueSection data={planInfoSection} />
          </Grid>
          <Grid size={{ xs: 12, sm: 6, md: 4 }}>
            <LabelValueSection data={enrollmentSection} />
          </Grid>
        </Grid>
      </Grid>
    </Grid>
  );
};

export default ForfeitureAdjustmentEmployeeDetails;
