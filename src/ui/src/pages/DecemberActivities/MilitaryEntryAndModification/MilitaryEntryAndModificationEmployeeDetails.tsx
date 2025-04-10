import { Typography } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import LabelValueSection from "components/LabelValueSection";
import React from "react";
import { EmployeeDetails } from "reduxstore/types";
import { mmDDYYFormat } from "smart-ui-library";
import { formatPercentage } from "utils/formatPercentage";

interface MilitaryEntryAndModificationEmployeeDetailsProps {
  details: EmployeeDetails;
}

const MilitaryEntryAndModificationEmployeeDetails: React.FC<MilitaryEntryAndModificationEmployeeDetailsProps> = ({ details }) => {
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

  const infoSection = [
    { label: "", value: `${lastName}, ${firstName}` },
    { label: "", value: `${address}` },
    { label: "", value: `${addressCity}, ${addressState} ${addressZipCode}` },
    { label: "Years In Plan", value: yearsInPlan },
  ];

  const planSection = [
    { label: "Badge", value: badgeNumber },
    { label: "SSN", value: `${ssn}` },
    { label: "DOB", value: mmDDYYFormat(dateOfBirth) }
  ];

  const employeeSection = [
    { label: "Enrolled", value: enrollmentId ? "Yes" : "No" },
    { label: "YTD P/S Hours", value: yearToDateProfitSharingHours },
    { label: "Percentage Vested", value: formatPercentage(percentageVested) },
    { label: "Cont Last Year", value: contributionsLastYear ? "Yes" : "No" }
  ];

  return (
    <Grid2
    container
    paddingX="24px"
    width={"100%"}>
      <Grid2 size={{ xs: 12 }} >
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          Employee Details
        </Typography>
      </Grid2>

      <Grid2 size={{ xs: 12 }} >
        <Grid2
          container
          spacing={3}>
          <Grid2 size={{ xs: 4 }} >
            <LabelValueSection
              data={infoSection}
            />
          </Grid2>
          <Grid2 size={{ xs: 4 }} >
            <LabelValueSection
              data={planSection}
            />
          </Grid2>
          <Grid2 size={{ xs: 4 }} >
            <LabelValueSection
              data={employeeSection}
            />
          </Grid2>
        </Grid2>
      </Grid2>
    </Grid2>
  );
};

export default MilitaryEntryAndModificationEmployeeDetails;
