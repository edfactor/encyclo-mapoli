import { Button, Tooltip, Typography } from "@mui/material";
import React, { useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import { GRID_KEYS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { GridPaginationActions, GridPaginationState } from "../../../hooks/useGridPagination";
import StandaloneMemberDetails from "../../InquiriesAndAdjustments/MasterInquiry/StandaloneMemberDetails";
import { GetMilitaryContributionColumns } from "./MilitaryContributionFormGridColumns";

interface MilitaryContributionResponse {
  results: unknown[];
  total: number;
}

interface MilitaryContributionGridProps {
  militaryContributionsData: MilitaryContributionResponse | null;
  isLoadingContributions: boolean;
  contributionsGridPagination: GridPaginationState & GridPaginationActions;
  onAddContribution: () => void;
  refreshTrigger?: number;
  isReadOnly?: boolean;
}

const MilitaryContributionGrid: React.FC<MilitaryContributionGridProps> = ({
  militaryContributionsData,
  isLoadingContributions,
  contributionsGridPagination,
  onAddContribution,
  refreshTrigger,
  isReadOnly = false
}) => {
  const profitYear = useDecemberFlowProfitYear();
  const { masterInquiryMemberDetails } = useSelector((state: RootState) => state.inquiry);

  const columnDefs = useMemo(() => GetMilitaryContributionColumns(), []);

  const headerContent = (
    <>
      {masterInquiryMemberDetails && profitYear > 0 && (
        <MissiveAlertProvider>
          <StandaloneMemberDetails
            memberType={masterInquiryMemberDetails.isEmployee ? 1 : 2}
            id={masterInquiryMemberDetails.id}
            profitYear={profitYear}
            refreshTrigger={refreshTrigger}
          />
        </MissiveAlertProvider>
      )}

      {militaryContributionsData && (
        <div
          style={{
            padding: "24px 24px 0px 24px",
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center"
          }}>
          <Typography
            variant="h2"
            sx={{ color: "#0258A5", marginTop: "7px" }}>
            {`Military Contributions (${militaryContributionsData?.total || 0} ${(militaryContributionsData?.total || 0) === 1 ? "Record" : "Records"})`}
          </Typography>

          <Tooltip
            title={
              isReadOnly
                ? "You are in read-only mode and cannot add contributions."
                : masterInquiryMemberDetails?.payFrequencyId == 2
                  ? "You cannot add a contribution to someone paid monthly."
                  : ""
            }
            placement="top">
            <span>
              <Button
                variant="outlined"
                color="primary"
                disabled={isReadOnly || masterInquiryMemberDetails?.payFrequencyId == 2}
                onClick={isReadOnly ? undefined : onAddContribution}
                sx={{ marginTop: "6px" }}>
                Add Military Contribution
              </Button>
            </span>
          </Tooltip>
        </div>
      )}
    </>
  );

  // Don't render grid until we have data
  if (!militaryContributionsData) {
    return headerContent;
  }

  return (
    <DSMPaginatedGrid
      preferenceKey={GRID_KEYS.MILITARY_CONTRIBUTIONS}
      data={militaryContributionsData.results}
      columnDefs={columnDefs}
      totalRecords={militaryContributionsData.total}
      isLoading={isLoadingContributions}
      pagination={contributionsGridPagination}
      header={headerContent}
    />
  );
};

export default MilitaryContributionGrid;
