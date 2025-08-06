import { Grid, Typography } from "@mui/material";
import LabelValueSection from "components/LabelValueSection";
import React, { useEffect, useMemo, memo } from "react";
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
import { useMissiveAlerts } from "./useMissiveAlerts";

interface MasterInquiryEmployeeDetailsProps {
  memberType: number;
  id: string | number;
  profitYear?: number | null | undefined;
  noResults?: boolean;
}

const MasterInquiryEmployeeDetails: React.FC<MasterInquiryEmployeeDetailsProps> = memo(
  ({ memberType, id, profitYear, noResults }) => {
    const [trigger, { data: details, isLoading, isError }] = useLazyGetProfitMasterInquiryMemberQuery();
    // Always call trigger with useEffect dependency, never skip
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

    // Memoized enrollment status
    const enrollmentStatus = useMemo(() => {
      if (!details) return { enrolled: "", forfeited: "" };
      return {
        enrolled: getEnrolledStatus(details.enrollmentId),
        forfeited: getForfeitedStatus(details.enrollmentId)
      };
    }, [details?.enrollmentId]);

    // Memoized summary section
    const summarySection = useMemo(() => {
      if (!details) return [];
      const {
        firstName,
        lastName,
        address,
        addressCity,
        addressState,
        addressZipCode,
        phoneNumber,
        isEmployee,
        workLocation,
        storeNumber
      } = details;

      return [
        { label: "Name", value: `${lastName}, ${firstName}` },
        { label: "Address", value: `${address}` },
        { label: "", value: `${addressCity}, ${addressState} ${addressZipCode}` },
        { label: "Phone #", value: phoneNumber || "N/A" },
        ...(isEmployee ? [{ label: "Work Location", value: workLocation || "N/A" }] : []),
        ...(isEmployee ? [{ label: "Store", value: storeNumber > 0 ? storeNumber : "N/A" }] : []),
        { label: "Enrolled", value: enrollmentStatus.enrolled },
        { label: "Forfeited", value: enrollmentStatus.forfeited }
      ];
    }, [details, enrollmentStatus]);

    // Memoized personal section
    const personalSection = useMemo(() => {
      if (!details) return [];
      const {
        badgeNumber,
        psnSuffix,
        isEmployee,
        department,
        PayClassification,
        employmentStatus,
        gender,
        dateOfBirth,
        ssn: ssnValue,
        allocationToAmount
      } = details;

      return [
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
    }, [details]);

    // Memoized plan section
    const planSection = useMemo(() => {
      if (!details) return [];
      const {
        beginPSAmount,
        currentPSAmount,
        beginVestedAmount,
        currentVestedAmount,
        isEmployee,
        yearToDateProfitSharingHours,
        yearsInPlan,
        percentageVested,
        receivedContributionsLastYear
      } = details;

      return [
        { label: "Begin Balance", value: numberToCurrency(beginPSAmount) },
        { label: "Current Balance", value: numberToCurrency(currentPSAmount) },
        { label: "Begin Vested Balance", value: numberToCurrency(beginVestedAmount) },
        { label: "Current Vested Balance", value: numberToCurrency(currentVestedAmount) },
        ...(isEmployee
          ? [{ label: "Profit Sharing Hours", value: formatNumberWithComma(yearToDateProfitSharingHours) }]
          : []),
        ...(isEmployee ? [{ label: "Years In Plan", value: yearsInPlan }] : []),
        { label: "Vested Percent", value: formatPercentage(percentageVested) },
        { label: "Contributions Last Year", value: (receivedContributionsLastYear ? "Y" : "N") }
      ];
    }, [details]);

    // Memoized milestone section
    const milestoneSection = useMemo(() => {
      if (!details) return [];
      const {
        isEmployee,
        hireDate,
        fullTimeDate,
        terminationDate,
        terminationReason,
        reHireDate,
        currentEtva,
        previousEtva,
        allocationFromAmount
      } = details;

      return [
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
    }, [details]);

    // Early returns after all hooks are called
    if (noResults) {
      return null;
    }

    if (isLoading) return <Typography>Loading...</Typography>;
    if (isError || !details) return <Typography>No details found.</Typography>;

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
                <LabelValueSection data={summarySection} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <LabelValueSection data={personalSection} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <LabelValueSection data={milestoneSection} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6, md: 3 }}>
                <LabelValueSection data={planSection} />
              </Grid>
            </Grid>
          </Grid>
        </Grid>
      </div>
    );
  },
  (prevProps, nextProps) => {
    // Custom comparison function for React.memo
    return (
      prevProps.memberType === nextProps.memberType &&
      prevProps.id === nextProps.id &&
      prevProps.profitYear === nextProps.profitYear &&
      prevProps.noResults === nextProps.noResults
    );
  }
);

export default MasterInquiryEmployeeDetails;
