import { Typography } from "@mui/material";
import React from "react";
import { ProfitMasterStatus, ProfitShareEditUpdateQueryParams } from "../../../reduxstore/types";

interface ChangesListProps {
  params: ProfitShareEditUpdateQueryParams | ProfitMasterStatus | null | undefined;
}

const ChangesList: React.FC<ChangesListProps> = ({ params }) => {
  return (
    <Typography
      component={"span"}
      variant="body2"
      sx={{}}>
      <ul style={{ listStyleType: "none", padding: 0 }}>
        {params && params.contributionPercent != null && params.contributionPercent != 0 && (
          <li
            style={{
              marginBottom: "6px"
            }}>
            {`Contribution Percent: `}
            <strong>{params.contributionPercent}</strong>
          </li>
        )}
        {params && params.earningsPercent != null && params.earningsPercent != 0 && (
          <li style={{ marginBottom: "6px" }}>
            {`Earnings Percent: `}
            <strong>{params.earningsPercent}</strong>
          </li>
        )}
        {params && params.incomingForfeitPercent != null && params.incomingForfeitPercent != 0 && (
          <li style={{ marginBottom: "6px" }}>
            {`Incoming Forfeit Percent: `}
            <strong>{params.incomingForfeitPercent}</strong>
          </li>
        )}
        {params && params?.secondaryEarningsPercent != null && params?.secondaryEarningsPercent != 0 && (
          <li style={{ marginBottom: "6px" }}>
            {`Secondary Earnings Percent: `}
            <strong>{params?.secondaryEarningsPercent}</strong>
          </li>
        )}
        {params && params?.maxAllowedContributions != null && params?.maxAllowedContributions != 0 && (
          <li style={{ marginBottom: "6px" }}>
            {`Max Allowed Contributions: `}
            <strong>{params?.maxAllowedContributions}</strong>
          </li>
        )}
        {params &&
          ((params as ProfitShareEditUpdateQueryParams)?.badgeToAdjust != null &&
          (params as ProfitShareEditUpdateQueryParams)?.badgeToAdjust != 0 ? (
            <li style={{ marginBottom: "6px" }}>
              {`Badge To Adjust: `}
              <strong>{(params as ProfitShareEditUpdateQueryParams)?.badgeToAdjust}</strong>
            </li>
          ) : (params as ProfitMasterStatus)?.badgeAdjusted != null &&
            (params as ProfitMasterStatus)?.badgeAdjusted != 0 ? (
            <li style={{ marginBottom: "6px" }}>
              {`Badge Adjusted: `}
              <strong>{(params as ProfitMasterStatus)?.badgeAdjusted}</strong>
            </li>
          ) : null)}
        {params && params?.adjustContributionAmount != null && params?.adjustContributionAmount != 0 && (
          <li style={{ marginBottom: "6px" }}>
            {`Adjust Contribution Amount: `}
            <strong>{params?.adjustContributionAmount}</strong>
          </li>
        )}
        {params && params?.adjustEarningsAmount != null && params?.adjustEarningsAmount != 0 && (
          <li style={{ marginBottom: "6px" }}>
            {`Adjust Earnings Amount: `}
            <strong>{params?.adjustEarningsAmount}</strong>
          </li>
        )}
        {params && params?.adjustIncomingForfeitAmount != null && params?.adjustIncomingForfeitAmount != 0 && (
          <li
            style={{
              marginBottom: "6px"
            }}>
            {`Adjust Incoming Forfeit Amount: `}
            <strong>{params?.adjustIncomingForfeitAmount}</strong>
          </li>
        )}
        {params &&
          ((params as ProfitShareEditUpdateQueryParams)?.badgeToAdjust2 != null &&
          (params as ProfitShareEditUpdateQueryParams)?.badgeToAdjust2 != 0 ? (
            <li style={{ marginBottom: "6px" }}>
              {`Second Badge to Adjust: `}
              <strong>{(params as ProfitShareEditUpdateQueryParams)?.badgeToAdjust2}</strong>
            </li>
          ) : (params as ProfitMasterStatus)?.badgeAdjusted2 != null &&
            (params as ProfitMasterStatus)?.badgeAdjusted2 != 0 ? (
            <li style={{ marginBottom: "6px" }}>
              {`Second Badge Adjusted: `}
              <strong>{(params as ProfitMasterStatus)?.badgeAdjusted2}</strong>
            </li>
          ) : null)}
        {params && params?.adjustEarningsSecondaryAmount != null && params?.adjustEarningsSecondaryAmount != 0 && (
          <li
            style={{
              marginBottom: "6px"
            }}>
            {`Adjust Secondary Earnings Amount: `}
            <strong>{params?.adjustEarningsSecondaryAmount}</strong>
          </li>
        )}
      </ul>
    </Typography>
  );
};

export default ChangesList;
