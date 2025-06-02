import { Typography } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import LabelValueSection from "components/LabelValueSection";
import React, { useEffect } from "react";
import { EmployeeDetails, MissiveResponse } from "reduxstore/types";
import { numberToCurrency } from "smart-ui-library";
import { formatPercentage } from "utils/formatPercentage";
import { viewBadgeLinkRenderer } from "../../utils/masterInquiryLink";
import { mmDDYYFormat } from "../../utils/dateUtils";
import { getEnrolledStatus, getForfeitedStatus } from "../../utils/enrollmentUtil";
import { useLazyGetProfitMasterInquiryMemberQuery } from "reduxstore/api/InquiryApi";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";


interface MasterInquiryEmployeeDetailsProps {
  memberType: number;
  id: string | number;
  profitYear?: number | null | undefined;
}

const MasterInquiryEmployeeDetails: React.FC<MasterInquiryEmployeeDetailsProps> = ({ memberType, id, profitYear }) => {
  const [trigger, { data: details, isLoading, isError }] = useLazyGetProfitMasterInquiryMemberQuery();
  const missives = useSelector((state: RootState) => state.lookups.missives);

  const defaultProfitYear = useDecemberFlowProfitYear();

  useEffect(() => {
    if (memberType && id) {
      trigger({ memberType, id: typeof id === 'string' ? parseInt(id) : id, profitYear:  profitYear ?? defaultProfitYear });
    }
  }, [memberType, id, profitYear, trigger, defaultProfitYear]);

  if (isLoading) return <Typography>Loading...</Typography>;
  if (isError || !details) return <Typography>No details found.</Typography>;

  // Missive alerts logic (moved from filter/member grid)
  let missiveAlerts: MissiveResponse[] = [];
  if (details && details.missives && missives) {
    missiveAlerts = details.missives.map((id: number) => missives.find((m: MissiveResponse) => m.id === id)).filter(Boolean) as MissiveResponse[];
  }

  const {
    firstName,
    lastName,
    address,
    addressCity,
    addressState,
    addressZipCode,
    dateOfBirth,
    ssn: ssnValue,
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
    { label: "DOB", value: mmDDYYFormat(dateOfBirth) },
    { label: "SSN", value: `${ssnValue}` },
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
    { label: "Hire", value: mmDDYYFormat(hireDate) },
    { label: "Term", value: terminationDate ? mmDDYYFormat(terminationDate) : 'N/A' },
    { label: "Store", value: storeNumber },
    { label: "Rehire", value: reHireDate ? mmDDYYFormat(reHireDate) : 'N/A' },
  ];

  const amountsSection = [
    { label: "Begin PS Amount", value: numberToCurrency(beginPSAmount) },
    { label: "Current PS Amount", value: numberToCurrency(currentPSAmount) },
    { label: "Begin Vested Amount", value: numberToCurrency(beginVestedAmount) },
    { label: "Current Vested Amount", value: numberToCurrency(currentVestedAmount) },
  ];

  return (
    <>
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
      </Grid2>
      {missiveAlerts.length > 0 && (
        <Grid2 size={{ xs: 12 }}>
          <div className="missive-alerts-box">
            {missiveAlerts.map((alert, idx) => (
              <div key={alert.id || idx} className="missive-alert">
                <Typography color={alert.severity === 'Error' ? 'error' : 'warning'} variant="body1" fontWeight={600}>{alert.message}</Typography>
                <Typography variant="body2">{alert.description}</Typography>
              </div>
            ))}
          </div>
        </Grid2>
      )}
    </>
  );
};

export default MasterInquiryEmployeeDetails;
