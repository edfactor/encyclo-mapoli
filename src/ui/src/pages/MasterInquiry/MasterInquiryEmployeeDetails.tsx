import { Typography } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import LabelValueSection from "components/LabelValueSection";
import React from "react";
import { EmployeeDetails } from "reduxstore/types";
import { mmDDYYFormat, numberToCurrency } from "smart-ui-library";
import { formatPercentage } from "utils/formatPercentage";

interface MasterInquiryEmployeeDetailsProps {
  details: EmployeeDetails;
}

const MasterInquiryEmployeeDetails: React.FC<MasterInquiryEmployeeDetailsProps> = ({ details }) => {
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
    badgeNumber,
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
    { label: "Enrolled", value: enrolled ? "Yes" : "No" },
  ];

  const employeeSection = [
    { label: "Badge Number", value: badgeNumber },
    { label: "DOB", value: mmDDYYFormat(dateOfBirth) },
    { label: "SSN", value: `${ssn}` }
  ];
  
  const planSection = [
    { label: "YTD P/S Hours", value: yearToDateProfitSharingHours },
    { label: "Years In Plan", value: yearsInPlan },
    { label: "Percentage Vested", value: formatPercentage(percentageVested) },
    { label: "Cont Last Year", value: contributionsLastYear ? "Yes" : "No" }
  ];

  const hireSection = [
    { label: "Hire", value: mmDDYYFormat(hireDate) },
    { label: "Term", value: mmDDYYFormat(terminationDate) ?? 'N/A' },
    { label: "Store", value: storeNumber },
    { label: "Rehire", value: mmDDYYFormat(reHireDate) ?? 'N/A' },
  ];

  const amountsSection = [
    { label: "Begin PS Amount", value: numberToCurrency(beginPSAmount) },
    { label: "Current PS Amount", value: numberToCurrency(currentPSAmount) },
    { label: "Begin Vested Amount", value: numberToCurrency(beginVestedAmount) },
    { label: "Current Vested Amount", value: numberToCurrency(currentVestedAmount) },
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
          <Grid2 xs={2}>
            <LabelValueSection
              data={infoSection}
            />
          </Grid2>
          <Grid2 xs={3}>
            <LabelValueSection
              data={employeeSection}
            />
          </Grid2>
          <Grid2 xs={2}>
            <LabelValueSection
              data={planSection}
            />
          </Grid2>
          <Grid2 xs={2}>
            <LabelValueSection
              data={hireSection}
            />
          </Grid2>
          <Grid2 xs={3}>
            <LabelValueSection
              data={amountsSection}
            />
          </Grid2>
        </Grid2>
      </Grid2>
    </Grid2>
  );
};

export default MasterInquiryEmployeeDetails;
