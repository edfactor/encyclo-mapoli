import { Grid, Typography } from "@mui/material";
import LabelValueSection from "components/LabelValueSection";
import React, { useEffect, useMemo } from "react";
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
import { isSimpleSearch } from "./MasterInquiryFunctions";
import { MASTER_INQUIRY_MESSAGES } from "./MasterInquiryMessages";
import { useMissiveAlerts } from "./MissiveAlertContext";

interface MasterInquiryEmployeeDetailsProps {
  memberType: number;
  id: string | number;
  profitYear?: number | null | undefined;
  noResults?: boolean;
}

const MasterInquiryEmployeeDetails: React.FC<MasterInquiryEmployeeDetailsProps> = ({
  memberType,
  id,
  profitYear,
  noResults
}) => {
  const [trigger, { data: details, isLoading, isError }] = useLazyGetProfitMasterInquiryMemberQuery();
  const { masterInquiryResults } = useSelector((state: RootState) => state.inquiry);

  const missives = useSelector((state: RootState) => state.lookups.missives);
  const { addAlert, addAlerts, hasAlert } = useMissiveAlerts();

  const defaultProfitYear = useDecemberFlowProfitYear();
  const [isMilitary, setIsMilitary] = React.useState(false);

  // We need to get the saved query params
  const { masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);

  useEffect(() => {
    if (memberType && id) {
      trigger({
        memberType,
        id: typeof id === "string" ? parseInt(id) : id,
        profitYear: profitYear ?? defaultProfitYear
      });
    }
  }, [memberType, id, profitYear, defaultProfitYear]);

  useEffect(() => {
    if (masterInquiryResults) {
      // We check transaction comment types to know if this is a military member
      const militaryTransactions = masterInquiryResults.filter((item) => item.commentTypeName === "Military");
      setIsMilitary(militaryTransactions.length > 0);
    } else {
      setIsMilitary(false);
    }
  }, [masterInquiryResults]);

  useEffect(() => {
    if (noResults) {
      if (isSimpleSearch(masterInquiryRequestParams)) {
        if (!hasAlert(MASTER_INQUIRY_MESSAGES.MEMBER_NOT_FOUND.id)) {
          addAlert(MASTER_INQUIRY_MESSAGES.MEMBER_NOT_FOUND);
        }
      } else {
        if (!hasAlert(MASTER_INQUIRY_MESSAGES.NO_RESULTS_FOUND.id)) {
          addAlert(MASTER_INQUIRY_MESSAGES.NO_RESULTS_FOUND);
        }
      }
    }
  }, [noResults, masterInquiryRequestParams, hasAlert, addAlert]);

  useEffect(() => {
    if (!details) return;

    // Handle missives from API response
    if (details.missives && missives) {
      const localMissives: MissiveResponse[] = details.missives
        .map((id: number) => missives.find((m: MissiveResponse) => m.id === id))
        .filter(Boolean) as MissiveResponse[];

      if (localMissives.length > 0) {
        addAlerts(localMissives);
      }
    }

    // Handle beneficiary found warning
    if (!details.isEmployee && masterInquiryRequestParams?.memberType == "all") {
      addAlert(MASTER_INQUIRY_MESSAGES.BENEFICIARY_FOUND(details.ssn));
    }

    // Handle military vested warning
    if (isMilitary && details.percentageVested > 0) {
      addAlert(MASTER_INQUIRY_MESSAGES.MILITARY_VESTED_WARNING(details.percentageVested));
    }
  }, [details, missives, masterInquiryRequestParams?.memberType, isMilitary, addAlert, addAlerts]);

  if (noResults) {
    return null;
  }

  if (isLoading) return <Typography>Loading...</Typography>;
  if (isError || !details) return <Typography>No details found.</Typography>;

  const sectionData = useMemo(() => {
    if (!details) return { summarySection: [], personalSection: [], planSection: [], milestoneSection: [] };

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
      receivedContributionsLastYear,
      enrollmentId,
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
      workLocation,
      allocationToAmount,
      allocationFromAmount
    } = details;

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
      { label: "Forfeited", value: forfeited }
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
      { label: "Allocation To", value: numberToCurrency(allocationToAmount) }
    ];

    // Section 3: Plan/Profit Sharing
    const planSection = [
      { label: "Begin Balance", value: numberToCurrency(beginPSAmount) },
      { label: "Current Balance", value: numberToCurrency(currentPSAmount) },
      { label: "Begin Vested Balance", value: numberToCurrency(beginVestedAmount) },
      { label: "Current Vested Balance", value: numberToCurrency(currentVestedAmount) },
      ...(isEmployee
        ? [{ label: "Profit Sharing Hours", value: formatNumberWithComma(yearToDateProfitSharingHours) }]
        : []),
      ...(isEmployee ? [{ label: "Years In Plan", value: yearsInPlan }] : []),
      { label: "Vested Percent", value: formatPercentage(percentageVested) },
      { label: "Received Contributions Last Year", value: receivedContributionsLastYear ? "Yes" : "No" }
    ];

    // Section 4: Milestones/Status
    const milestoneSection = [
      ...(isEmployee ? [{ label: "Hire Date", value: hireDate ? mmDDYYFormat(hireDate) : "N/A" }] : []),
      ...(isEmployee ? [{ label: "Full Time Date", value: fullTimeDate ? mmDDYYFormat(fullTimeDate) : "N/A" }] : []),
      ...(isEmployee
        ? [{ label: "Termination Date", value: terminationDate ? mmDDYYFormat(terminationDate) : "N/A" }]
        : []),
      ...(isEmployee ? [{ label: "Termination Reason", value: terminationReason || "N/A" }] : []),
      ...(isEmployee ? [{ label: "Re-Hire Date", value: reHireDate ? mmDDYYFormat(reHireDate) : "N/A" }] : []),
      { label: "ETVA", value: numberToCurrency(currentEtva) },
      { label: "Previous ETVA", value: numberToCurrency(previousEtva) },
      { label: "Allocation From", value: numberToCurrency(allocationFromAmount) }
    ];

    return { summarySection, personalSection, planSection, milestoneSection };
  }, [details]);

  return (
    <div
      className="employee-details-lightbox"
      style={{ width: "100%" }}>
      <Grid
        container
        paddingX="24px"
        width={"100%"}>
        <Grid size={{ xs: 12 }}>
          <Typography
            variant="h2"
            sx={{ color: "#0258A5" }}>
            Member Details
          </Typography>
        </Grid>

        <Grid size={{ xs: 12 }}>
          <Grid
            container
            spacing={3}>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <LabelValueSection data={sectionData.summarySection} />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <LabelValueSection data={sectionData.personalSection} />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <LabelValueSection data={sectionData.milestoneSection} />
            </Grid>
            <Grid size={{ xs: 12, sm: 6, md: 3 }}>
              <LabelValueSection data={sectionData.planSection} />
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </div>
  );
};

export default MasterInquiryEmployeeDetails;
