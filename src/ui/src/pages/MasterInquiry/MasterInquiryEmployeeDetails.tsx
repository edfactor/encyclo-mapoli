import { Typography } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import LabelValueSection from "components/LabelValueSection";
import React from "react";
import { EmployeeDetails, MissiveResponse } from "reduxstore/types";
import { mmDDYYFormat, numberToCurrency } from "smart-ui-library";
import { formatPercentage } from "utils/formatPercentage";
import { viewBadgeLinkRenderer } from "../../utils/masterInquiryLink";
import { tryddmmyyyyToDate } from "../../utils/dateUtils";
import { getEnrolledStatus, getForfeitedStatus } from "../../utils/enrollmentUtil";

interface MasterInquiryEmployeeDetailsProps {
  details: EmployeeDetails;
  missives: MissiveResponse[]  | null;
}

const MasterInquiryEmployeeDetails: React.FC<MasterInquiryEmployeeDetailsProps> = ({ details, missives }) => {
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
    enrollment,
    badgeNumber,
    hireDate,
    terminationDate,
    reHireDate,
    storeNumber,
    beginPSAmount,
    currentPSAmount,
    beginVestedAmount,
    currentVestedAmount,
    currentEtva,
    employmentStatus,
  } = details;

  const enrolled = getEnrolledStatus(enrollmentId);
  const forfeited = getForfeitedStatus(enrollmentId);
  
  const infoSection = [
    { label: "", value: `${lastName}, ${firstName}` },
    { label: "", value: `${address}` },
    { label: "", value: `${addressCity}, ${addressState} ${addressZipCode}` },
    { label: "Enrolled", value: enrolled },
    { label: "Forfeited", value: forfeited }
  ];

  const employeeSection = [
    { label: "Badge", value: viewBadgeLinkRenderer(Number(badgeNumber)) },
    { label: "DOB", value: mmDDYYFormat(tryddmmyyyyToDate(dateOfBirth)) },
    { label: "SSN", value: `${ssn}` },
    { label: "ETVA", value: currentEtva },
    { label: "Status", value: employmentStatus },   
    { label: "Enrollment", value: enrollment },
  ].filter(field => field.value !== 0);
  
  const planSection = [
    { label: "YTD P/S Hours", value: yearToDateProfitSharingHours },
    { label: "Years In Plan", value: yearsInPlan },
    { label: "Percentage Vested", value: formatPercentage(percentageVested) },
    { label: "Cont Last Year", value: contributionsLastYear ? "Yes" : "No" }
  ];

  const hireSection = [
    { label: "Hire", value: mmDDYYFormat(tryddmmyyyyToDate(hireDate)) },
    { label: "Term", value: terminationDate ? mmDDYYFormat(tryddmmyyyyToDate(terminationDate)) : 'N/A' },
    { label: "Store", value: storeNumber },
    { label: "Rehire", value: reHireDate ? mmDDYYFormat(tryddmmyyyyToDate(reHireDate)) : 'N/A' },
  ];

  const amountsSection = [
    { label: "Begin PS Amount", value: numberToCurrency(beginPSAmount) },
    { label: "Current PS Amount", value: numberToCurrency(currentPSAmount) },
    { label: "Begin Vested Amount", value: numberToCurrency(beginVestedAmount) },
    { label: "Current Vested Amount", value: numberToCurrency(currentVestedAmount) },
  ];

  const translatedMissives = missives && details && details.missives?.length ? details.missives.map(m=>missives.find(x=>x.id == m)?.message) : []

  const translatedMissivesSection = translatedMissives.map((missive, idx) => (
    <Typography key={idx} variant="body1" sx={{ marginBottom: "8px" }}> {missive}</Typography>
  ));

  return (
    <Grid2
    container
    paddingX="24px"
    width={"100%"}>
      <Grid2 size={{ xs: 12 }}>
        <Typography
          variant="h2"
          sx={{ color: "#0258A5" }}>
          Member Details
        </Typography>
      </Grid2>

      <Grid2 size={{ xs: 12 }}>
        <Grid2
          container
          spacing={3}>
          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <LabelValueSection
              data={infoSection}
            />
          </Grid2>
          <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
            <LabelValueSection
              data={employeeSection}
            />
          </Grid2>
          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <LabelValueSection
              data={planSection}
            />
          </Grid2>
          <Grid2 size={{ xs: 12, sm: 6, md: 2 }}>
            <LabelValueSection
              data={hireSection}
            />
          </Grid2>
          <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
            <LabelValueSection
              data={amountsSection}
            />
          </Grid2>
        </Grid2>
      </Grid2>
      {/* {translatedMissivesSection} */}
    </Grid2>
  );
};

export default MasterInquiryEmployeeDetails;
