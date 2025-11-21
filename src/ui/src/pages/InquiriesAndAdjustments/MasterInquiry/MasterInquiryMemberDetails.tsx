import { Grid, Typography } from "@mui/material";
import React, { memo, useMemo } from "react";
import { formatNumberWithComma, numberToCurrency } from "smart-ui-library";
import LabelValueSection from "../../../components/LabelValueSection";
import MissiveAlerts from "../../../components/MissiveAlerts/MissiveAlerts";
import { useMissiveAlerts } from "../../../hooks/useMissiveAlerts";
import type { EmployeeDetails } from "../../../types/employee/employee";
import { mmDDYYFormat } from "../../../utils/dateUtils";
import { getEnrolledStatus, getForfeitedStatus } from "../../../utils/enrollmentUtil";
import { formatPercentage } from "../../../utils/formatPercentage";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";
import { formatPhoneNumber } from "../../../utils/phoneUtils";

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
  memberDetails?: EmployeeDetails | null;
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
  ({ memberType: _memberType, id: _id, profitYear, memberDetails, isLoading }) => {
    const { missiveAlerts } = useMissiveAlerts();

    // Memoized enrollment status
    const enrollmentStatus = useMemo(() => {
      if (!memberDetails) return { enrolled: "", forfeited: "" };
      const enrollmentId = memberDetails.enrollmentId ?? 0;
      return {
        enrolled: getEnrolledStatus(enrollmentId),
        forfeited: getForfeitedStatus(enrollmentId)
      };
    }, [memberDetails]);

    // Memoized summary section
    const summarySection = useMemo(() => {
      if (!memberDetails) return [];
      const {
        fullName,
        address,
        addressCity,
        addressState,
        addressZipCode,
        phoneNumber,
        isEmployee,
        workLocation,
        storeNumber
      } = memberDetails;

      const formattedCity = addressCity || "";
      const formattedState = addressState || "";
      const formattedZip = addressZipCode ? formatZipCode(addressZipCode) : "";
      const cityStateZip =
        [formattedCity, formattedState].filter(Boolean).join(", ") + (formattedZip ? ` ${formattedZip}` : "");

      return [
        { label: "Name", value: fullName },
        { label: "Address", value: `${address}` },
        { label: "", value: cityStateZip },
        { label: "Phone #", value: formatPhoneNumber(phoneNumber) },
        ...(isEmployee ? [{ label: "Work Location", value: workLocation || "N/A" }] : []),
        ...(isEmployee
          ? [{ label: "Store", value: typeof storeNumber === "number" && storeNumber > 0 ? storeNumber : "N/A" }]
          : []),
        { label: "Enrolled", value: enrollmentStatus.enrolled.replace(/\s*\(\d+\)/, "") }, // Remove code like "(1)"
        { label: "Forfeited", value: enrollmentStatus.forfeited.replace(/\s*\(\d+\)/, "") } // Remove code like "(1)"
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
        payClassification,
        employmentStatus,
        gender,
        dateOfBirth,
        ssn: ssnValue,
        allocationToAmount,
        badgesOfDuplicateSsns
      } = memberDetails;

      const duplicateBadgeLink = [];
      if (badgesOfDuplicateSsns && badgesOfDuplicateSsns.length) {
        for (const badge of badgesOfDuplicateSsns) {
          duplicateBadgeLink.push({
            label: "Duplicate SSN with",
            value: viewBadgeLinkRenderer(badge),
            labelColor: "error" as const
          });
        }
      }

      const age = dateOfBirth
        ? Math.floor((Date.now() - new Date(dateOfBirth).getTime()) / (1000 * 60 * 60 * 24 * 365.25))
        : 0;
      const dobDisplay = dateOfBirth ? `${mmDDYYFormat(dateOfBirth)} (${age})` : "N/A";

      return [
        ...(isEmployee ? [{ label: "Badge", value: viewBadgeLinkRenderer(badgeNumber) }] : []),
        ...(!isEmployee ? [{ label: "PSN", value: viewBadgeLinkRenderer(badgeNumber, psnSuffix) }] : []),
        ...(isEmployee ? [{ label: "Department", value: department || "N/A" }] : []),
        ...(isEmployee ? [{ label: "Class", value: payClassification || "N/A" }] : []),
        ...(isEmployee ? [{ label: "Status", value: employmentStatus ?? "N/A" }] : []),
        { label: "Gender", value: gender || "N/A" },
        { label: "DOB", value: dobDisplay },
        { label: "SSN", value: `${ssnValue}` },
        ...duplicateBadgeLink,
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
        percentageVested
      } = memberDetails;

      const yearLabel = profitYear == new Date().getFullYear() ? "Current" : `End ${profitYear}`;

      // Format current vested balance with bold blue styling per PS-1897
      const formattedCurrentVested =
        currentVestedAmount == null ? (
          "N/A"
        ) : (
          <Typography
            component="span"
            variant="body2"
            sx={{ fontWeight: "bold", color: "#0258A5" }}>
            {numberToCurrency(currentVestedAmount)}
          </Typography>
        );

      return [
        // Group 1: Beginning balances
        { label: "Begin Balance", value: beginPSAmount == null ? "N/A" : numberToCurrency(beginPSAmount) },
        {
          label: "Begin Vested Balance",
          value: beginVestedAmount == null ? "N/A" : numberToCurrency(beginVestedAmount)
        },
        // Group 2: Current balances (vested balance highlighted)
        { label: `${yearLabel} Balance`, value: currentPSAmount == null ? "N/A" : numberToCurrency(currentPSAmount) },
        {
          label: `${yearLabel} Vested Balance`,
          value: formattedCurrentVested
        },
        ...(isEmployee
          ? [{ label: "Profit Sharing Hours", value: formatNumberWithComma(yearToDateProfitSharingHours) }]
          : []),
        ...(isEmployee ? [{ label: "Years In Plan", value: yearsInPlan }] : []),
        { label: "Vested Percent", value: formatPercentage(percentageVested) }
      ];
    }, [memberDetails, profitYear]);

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
        // Previous ETVA removed per PS-1897
        { label: "Allocation From", value: numberToCurrency(allocationFromAmount) }
      ];
    }, [memberDetails]);

    // Early returns after all hooks are called
    if (isLoading) return null;
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

          {/* Missive Alerts - Display at bottom of member details */}
          {missiveAlerts.length > 0 && (
            <Grid
              size={{ xs: 12 }}
              sx={{ marginTop: "14px" }}>
              <MissiveAlerts />
            </Grid>
          )}
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
