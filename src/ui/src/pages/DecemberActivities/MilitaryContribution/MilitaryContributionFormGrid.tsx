import { Button, Tooltip, Typography } from "@mui/material";
import React, { useCallback, useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, Pagination } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import StandaloneMemberDetails from "../../MasterInquiry/StandaloneMemberDetails";
import { GetMilitaryContributionColumns } from "./MilitaryContributionFormGridColumns";

interface MilitaryContributionGridProps {
  militaryContributionsData: any;
  isLoadingContributions: boolean;
  contributionsGridPagination: any;
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

  const handlePaginationChange = useCallback(
    (pageNumber: number, pageSize: number) => {
      contributionsGridPagination.handlePaginationChange(pageNumber, pageSize);
    },
    [contributionsGridPagination]
  );

  const handleSortChange = useCallback(
    (sortParams: any) => {
      contributionsGridPagination.handleSortChange(sortParams);
    },
    [contributionsGridPagination]
  );

  return (
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
        <>
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

          <DSMGrid
            preferenceKey={CAPTIONS.MILITARY_CONTRIBUTIONS}
            isLoading={isLoadingContributions}
            handleSortChanged={handleSortChange}
            providedOptions={{
              rowData: militaryContributionsData?.results,
              columnDefs: columnDefs
            }}
          />

          {!!militaryContributionsData && militaryContributionsData?.results?.length > 0 && (
            <Pagination
              pageNumber={contributionsGridPagination.pageNumber}
              setPageNumber={(value: number) => {
                handlePaginationChange(value - 1, contributionsGridPagination.pageSize);
              }}
              pageSize={contributionsGridPagination.pageSize}
              setPageSize={(value: number) => {
                handlePaginationChange(0, value);
              }}
              recordCount={militaryContributionsData?.total}
            />
          )}
        </>
      )}
    </>
  );
};

export default MilitaryContributionGrid;
