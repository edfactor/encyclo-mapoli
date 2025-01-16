import { Typography } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import LabelValueSection from "components/LabelValueSection";
import React from "react";
import { EmployeeDetails } from "reduxstore/types";
import { mmDDYYFormat, numberToCurrency } from "smart-ui-library";
import { formatPercentage } from "utils/formatPercentage";

interface MilitaryAndRehireEntryAndModificationEmployeeDetailsProps {
  details: EmployeeDetails;
}

const MilitaryAndRehireEntryAndModificationEmployeeDetails: React.FC<MilitaryAndRehireEntryAndModificationEmployeeDetailsProps> = ({ details }) => {
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
    enrolled,
    employeeId,
    hireDate,
    terminationDate,
    reHireDate,
    storeNumber,
    beginPSAmount,
    currentPSAmount,
    beginVestedAmount,
    currentVestedAmount
  } = details;

  const infoSection = [
    { label: "", value: `${lastName}, ${firstName}` },
    { label: "", value: `${address}` },
    { label: "", value: `${addressCity}, ${addressState} ${addressZipCode}` },
    { label: "DOB", value: mmDDYYFormat(dateOfBirth) },
    { label: "SSN", value: `${ssn}` }
  ];

  const planSection = [
    { label: "YTD P/S Hours", value: yearToDateProfitSharingHours },
    { label: "Years In Plan", value: yearsInPlan },
    { label: "Percentage Vested", value: formatPercentage(percentageVested) },
    { label: "Cont Last Year", value: contributionsLastYear ? "Yes" : "No" }
  ];

  const employeeSection = [
    { label: "Enrolled", value: enrolled ? "Yes" : "No" },
    { label: "Emp", value: employeeId },
  ];

  return (
    <Grid2
    container
    paddingX="24px"
    width={"100%"}>
      <Grid2 xs={12}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          Employee Details
        </Typography>
      </Grid2>

      <Grid2 xs={12}>
        <Grid2
          container
          spacing={3}>
          <Grid2 xs={4}>
            <LabelValueSection
              data={infoSection}
            />
          </Grid2>
          <Grid2 xs={4}>
            <LabelValueSection
              data={planSection}
            />
          </Grid2>
          <Grid2 xs={4}>
            <LabelValueSection
              data={employeeSection}
            />
          </Grid2>
        </Grid2>
      </Grid2>
    </Grid2>
  );
};

export default MilitaryAndRehireEntryAndModificationEmployeeDetails;
