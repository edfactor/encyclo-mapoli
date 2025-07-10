import { Typography } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import LabelValueSection from "components/LabelValueSection";
import React, { useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetProfitMasterInquiryMemberQuery } from "reduxstore/api/InquiryApi";
import { RootState } from "reduxstore/store";
import { MissiveResponse } from "reduxstore/types";
import { formatNumberWithComma, numberToCurrency } from "smart-ui-library";
import { formatPercentage } from "utils/formatPercentage";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";
import "../../styles/employee-details-lightbox.css";
import { mmDDYYFormat } from "../../utils/dateUtils";
import { getEnrolledStatus, getForfeitedStatus } from "../../utils/enrollmentUtil";
import { viewBadgeLinkRenderer } from "../../utils/masterInquiryLink";


interface MasterInquiryEmployeeDetailsProps {
  memberType: number;
  id: string | number;
  profitYear?: number | null | undefined;
  noResults?: boolean;
}

const MasterInquiryEmployeeDetails: React.FC<MasterInquiryEmployeeDetailsProps> = ({ memberType, id, profitYear, noResults }) => {
  const [trigger, { data: details, isLoading, isError }] = useLazyGetProfitMasterInquiryMemberQuery();
  const { masterInquiryResults } = useSelector((state: RootState) => state.inquiry);

  const missives = useSelector((state: RootState) => state.lookups.missives);

  const defaultProfitYear = useDecemberFlowProfitYear();
  const [isMilitary, setIsMilitary] = React.useState(false);

  // We need to get the saved query params
  const { masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);
  
  useEffect(() => {
    if (memberType && id) {
      trigger({ memberType, id: typeof id === 'string' ? parseInt(id) : id, profitYear: profitYear ?? defaultProfitYear });
    }
  }, [memberType, id, profitYear, trigger, defaultProfitYear]);

  useEffect(() => {
    if (masterInquiryResults) {
      // We check transaction comment types to know if this is a military member
      const militaryTransactions = masterInquiryResults.filter(item => item.commentTypeName === "Military");
      setIsMilitary(militaryTransactions.length > 0);
    } else {
      setIsMilitary(false);
    }
  }, [masterInquiryResults]);


  if (noResults) {
    return (
      <Grid2 size={{ xs: 12 }}>
        <div className="missive-alerts-box">
          <div className="missive-alert missive-error">
            <Typography sx={{ color: 'error.main' }} variant="body1" fontWeight={600}>No Profit Sharing Records Found</Typography>
            <Typography variant="body2">The Employee Badge Number you have entered has no Profit Sharing Records. Re-enter an Employee Badge Number with Profit Sharing.</Typography>
          </div>
        </div>
      </Grid2>
    );
  }

  if (isLoading) return <Typography>Loading...</Typography>;
  if (isError || !details) return <Typography>No details found.</Typography>;

  // Missive alerts logic (moved from filter/member grid)
  let missiveAlerts: MissiveResponse[] = [];
  if (details && details.missives && missives) {
    missiveAlerts = details.missives.map((id: number) => missives.find((m: MissiveResponse) => m.id === id)).filter(Boolean) as MissiveResponse[];
  }

  const {
    badgeNumber,
    psnSuffix,
    isEmployee,
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
    receivedContributionsLastYear,
    enrollmentId,
    enrollment,
    hireDate,
    fullTimeDate,
    terminationDate,
    terminationReason,
    reHireDate,
    storeNumber,
    beginPSAmount,
    currentPSAmount,
    beginVestedAmount,
    currentVestedAmount,
    currentEtva,
    previousEtva,
    employmentStatus,
    department,
    PayClassification,
    gender,
    phoneNumber,
    workLocation
  } = details;

  if (!isEmployee && masterInquiryRequestParams?.memberType == "all") {
    // Need to add a new missive warning saying a beneficiary was found
    missiveAlerts.push({
      id: 976,
      severity: 'info',
      message: `Beneficiary ${ssnValue} Found`,
      description: 'This member is a beneficiary and not an employee.',
    });

  } 

  // Warning needed if military member has vested money
  if (isMilitary && percentageVested > 0) {
    missiveAlerts.push({
      id: 961, // invented id as this is not from back end
      severity: 'warning',
      message: 'Military entry has affected vested percentage',
      description: `Vested percentage now at ${percentageVested * 100}%.`,
    });
  }

  const enrolled = getEnrolledStatus(enrollmentId);
  const forfeited = getForfeitedStatus(enrollmentId);

  // Section 1: Employee Summary (Name, Address, Contact)
  const summarySection = [
    { label: "Name", value: `${lastName}, ${firstName}` },
    { label: "Address", value: `${address}` },
    { label: "", value: `${addressCity}, ${addressState} ${addressZipCode}` },
    { label: "Phone #", value: phoneNumber || "N/A" },
    ...(isEmployee ? [{ label: "Work Location", value: workLocation || "N/A" }] : []),
    ...(isEmployee ? [{ label: "Store", value: storeNumber > 0 ? storeNumber : "N/A" }] : []),
    { label: "Enrolled", value: enrolled },
    { label: "Forfeited", value: forfeited },
  ];

  // Section 2: Employment/Personal Info
  const personalSection = [
    ...(isEmployee ? [{ label: "Badge", value: viewBadgeLinkRenderer(badgeNumber) }] : []),
    ...(!isEmployee ? [{ label: "PSN", value: viewBadgeLinkRenderer(badgeNumber, psnSuffix) }] : []),
    ...(isEmployee ? [{ label: "Department", value: department || "N/A" }] : []),
    ...(isEmployee ? [{ label: "Class", value: PayClassification || "N/A" }] : []),
    ...(isEmployee ? [{ label: "Status", value: employmentStatus ?? "N/A" }] : []),
    { label: "Gender", value: gender || "N/A" },
    { label: "DOB", value: mmDDYYFormat(dateOfBirth) },
    { label: "SSN", value: `${ssnValue}` },
  ];

  // Section 3: Plan/Profit Sharing
  const planSection = [
    { label: "Begin Balance", value: numberToCurrency(beginPSAmount) },
    { label: "Current Balance", value: numberToCurrency(currentPSAmount) },
    { label: "Begin Vested Balance", value: numberToCurrency(beginVestedAmount) },
    { label: "Current Vested Balance", value: numberToCurrency(currentVestedAmount) },
    ...(isEmployee ? [{ label: "Profit Sharing Hours", value: formatNumberWithComma(yearToDateProfitSharingHours) }] : []),
    ...(isEmployee ? [{ label: "Years In Plan", value: yearsInPlan }] : []),
    { label: "Vested Percent", value: formatPercentage(percentageVested) },
    { label: "Received Contributions Last Year", value: receivedContributionsLastYear ? "Yes" : "No" }
  ];

  // Section 4: Milestones/Status
  const milestoneSection = [
    ...(isEmployee ? [{ label: "Hire Date", value: hireDate ? mmDDYYFormat(hireDate) : 'N/A' }] : []),
    ...(isEmployee ? [{ label: "Full Time Date", value: fullTimeDate ? mmDDYYFormat(fullTimeDate) : 'N/A' }] : []),
    ...(isEmployee ? [{ label: "Termination Date", value: terminationDate ? mmDDYYFormat(terminationDate) : 'N/A' }] : []),
    ...(isEmployee ? [{ label: "Termination Reason", value: terminationReason || 'N/A' }] : []),
    ...(isEmployee ? [{ label: "Re-Hire Date", value: reHireDate ? mmDDYYFormat(reHireDate) : 'N/A' }] : []),
    { label: "ETVA", value: numberToCurrency(currentEtva) },
    { label: "Previous ETVA", value: numberToCurrency(previousEtva) },

  ];

  return (
    <div className="employee-details-lightbox" style={{ width: '100%' }}>
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
            <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
              <LabelValueSection
                data={summarySection}
              />
            </Grid2>
            <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
              <LabelValueSection
                data={personalSection}
              />
            </Grid2>
            <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
              <LabelValueSection
                data={milestoneSection}
              />
            </Grid2>
            <Grid2 size={{ xs: 12, sm: 6, md: 3 }}>
              <LabelValueSection
                data={planSection}
              />
            </Grid2>
          </Grid2>
        </Grid2>
      </Grid2>
      {missiveAlerts.length > 0 && (
        <Grid2 size={{ xs: 12 }}>
          <div className="missive-alerts-box">
            {missiveAlerts.map((alert: MissiveResponse, idx: number) => (
              <div key={alert.id || idx} className={`missive-alert ${alert.severity === 'Error' ? 'missive-error' : 'missive-warning'}`}>
                <Typography sx={{ color: alert.severity === 'Error' ? 'error.main' : 'warning.main' }} variant="body1" fontWeight={600}>{alert.message}</Typography>
                <Typography variant="body2">{alert.description}</Typography>
              </div>
            ))}
          </div>
        </Grid2>
      )}
    </div>
  );
};

export default MasterInquiryEmployeeDetails;
