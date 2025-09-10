import { Grid, Typography } from "@mui/material";
import LabelValueSection from "components/LabelValueSection";
import React, { memo, useMemo } from "react";
import { formatNumberWithComma, numberToCurrency } from "smart-ui-library";
import { formatPercentage } from "utils/formatPercentage";
import { mmDDYYFormat } from "../../utils/dateUtils";
import { getEnrolledStatus, getForfeitedStatus } from "../../utils/enrollmentUtil";
import { viewBadgeLinkRenderer } from "../../utils/masterInquiryLink";

// Sometimes we get back end zip codes that are 1907 rather than 01907
const formatZipCode = (zipCode: string): string => {
  if (/^\d{4}$/.test(zipCode)) {
    return `0${zipCode}`;
  }
  return zipCode;
};

interface MasterInquiryMemberDetailsProps {
  memberType: number;
  id: string | number;
  profitYear?: number | null | undefined;
  memberDetails?: any | null;
  isLoading?: boolean;
}

/*
Note, the first three parameters appear to be unused, but.... they are used
in the React memo stuff at the bottom and are important for edge cases like these:
 - If you switch from Member A to Member B, but memberDetails is still loading (null), the
  component wouldn't rerender
  - If you change the profit year but memberDetails hasn't updated yet, the component wouldn't
  know to rerender
  - The component could show stale data for wrong member
*/
const MasterInquiryMemberDetails: React.FC<MasterInquiryMemberDetailsProps> = memo(
  ({ memberType, id, profitYear, memberDetails, isLoading }) => {
    // Memoized enrollment status
    const enrollmentStatus = useMemo(() => {
      if (!memberDetails) return { enrolled: "", forfeited: "" };
      return {
        enrolled: getEnrolledStatus(memberDetails.enrollmentId),
        forfeited: getForfeitedStatus(memberDetails.enrollmentId)
      };
    }, [memberDetails?.enrollmentId]);

    // Memoized summary section
    const summarySection = useMemo(() => {
      if (!memberDetails) return [];
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
      } = memberDetails;

      return [
        { label: "Name", value: `${lastName}, ${firstName}` },
        { label: "Address", value: `${address}` },
        { label: "", value: `${addressCity}, ${addressState} ${formatZipCode(addressZipCode)}` },
        { label: "Phone #", value: phoneNumber || "N/A" },
        ...(isEmployee ? [{ label: "Work Location", value: workLocation || "N/A" }] : []),
        ...(isEmployee ? [{ label: "Store", value: storeNumber > 0 ? storeNumber : "N/A" }] : []),
        { label: "Enrolled", value: enrollmentStatus.enrolled },
        { label: "Forfeited", value: enrollmentStatus.forfeited }
      ];
    }, [memberDetails, enrollmentStatus]);

    // Memoized personal section
    const personalSection = useMemo(() => {
      if (!memberDetails) return [];
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
      } = memberDetails;

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
    }, [memberDetails]);

    // Memoized plan section
    const planSection = useMemo(() => {
      if (!memberDetails) return [];
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
      } = memberDetails;

      var yearLabel = profitYear == new Date().getFullYear() ? "Current" : `End ${profitYear}`;

      return [
        { label: "Begin Balance", value: beginPSAmount == null ? "N/A" : numberToCurrency(beginPSAmount) },
        { label: `${yearLabel} Balance`, value: currentPSAmount == null ? "N/A" : numberToCurrency(currentPSAmount) },
        {
          label: "Begin Vested Balance",
          value: beginVestedAmount == null ? "N/A" : numberToCurrency(beginVestedAmount)
        },
        {
          label: `${yearLabel} Vested Balance`,
          value: currentVestedAmount == null ? "N/A" : numberToCurrency(currentVestedAmount)
        },
        ...(isEmployee
          ? [{ label: "Profit Sharing Hours", value: formatNumberWithComma(yearToDateProfitSharingHours) }]
          : []),
        ...(isEmployee ? [{ label: "Years In Plan", value: yearsInPlan }] : []),
        { label: "Vested Percent", value: formatPercentage(percentageVested) },
        {
          label: "Contributions in Last Year",
          value: receivedContributionsLastYear == null ? "N/A" : receivedContributionsLastYear ? "Y" : "N"
        }
      ];
    }, [memberDetails]);

    // Memoized milestone section
    const milestoneSection = useMemo(() => {
      if (!memberDetails) return [];
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
      } = memberDetails;

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
    }, [memberDetails]);

    // Early returns after all hooks are called
    if (isLoading) return <Typography>Loading...</Typography>;
    if (!memberDetails) return <Typography>No details found.</Typography>;

    return (
      <div
        className="m-[1px] box-border p-[1px]"
        style={{ width: "100%" }}>
        <Grid
          container
          paddingX="24px"
          width={"100%"}>
          <Grid size={{ xs: 12 }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5", marginY: "8px" }}>
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
      prevProps.memberDetails === nextProps.memberDetails &&
      prevProps.isLoading === nextProps.isLoading
    );
  }
);

export default MasterInquiryMemberDetails;
