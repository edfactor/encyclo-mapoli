import { Button, Typography, Tooltip } from "@mui/material";
import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetMilitaryContributionsQuery } from "reduxstore/api/MilitaryApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import StandaloneMemberDetails from "../../MasterInquiry/StandaloneMemberDetails";
import { MissiveAlertProvider } from "../../MasterInquiry/utils/MissiveAlertContext";
import { GetMilitaryContributionColumns } from "./MilitaryContributionFormGridColumns";

interface MilitaryContributionGridProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  onAddContribution: () => void;
}

const MilitaryContributionGrid: React.FC<MilitaryContributionGridProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  onAddContribution
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "contributionDate",
    isSortDescending: false
  });
  const profitYear = useDecemberFlowProfitYear();
  const { masterInquiryMemberDetails } = useSelector((state: RootState) => state.inquiry);
  const { militaryContributionsData } = useSelector((state: RootState) => state.military);
  const [fetchContributions, { isFetching }] = useLazyGetMilitaryContributionsQuery();

  const onSearch = useCallback(async () => {
    if (masterInquiryMemberDetails) {
      await fetchContributions({
        badgeNumber: Number(masterInquiryMemberDetails.badgeNumber),
        profitYear: profitYear,
        contributionAmount: 0,
        contributionDate: "",
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      });
    }
  }, [pageNumber, pageSize, sortParams, masterInquiryMemberDetails, fetchContributions, profitYear]);

  useEffect(() => {
    if (initialSearchLoaded && masterInquiryMemberDetails) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, masterInquiryMemberDetails, onSearch]);

  // Need a useEffect on a change in militaryContributionsData to reset the page number when total count changes (new search, not pagination)
  const prevMilitaryContributionsData = useRef<any>(null);
  useEffect(() => {
    if (
      militaryContributionsData !== prevMilitaryContributionsData.current &&
      militaryContributionsData?.total !== undefined &&
      militaryContributionsData.total !== prevMilitaryContributionsData.current?.total
    ) {
      setPageNumber(0);
    }
    prevMilitaryContributionsData.current = militaryContributionsData;
  }, [militaryContributionsData]);

  const columnDefs = useMemo(() => GetMilitaryContributionColumns(), []);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  console.log("Master Inquiry Member Details:", masterInquiryMemberDetails);

  return (
    <>
      {masterInquiryMemberDetails && profitYear > 0 && (
        <MissiveAlertProvider>
          <StandaloneMemberDetails
            memberType={masterInquiryMemberDetails.isEmployee ? 1 : 2}
            id={masterInquiryMemberDetails.id}
            profitYear={profitYear}
          />
        </MissiveAlertProvider>
      )}

      {militaryContributionsData && (
        <>
          <div
            style={{
              padding: "0 24px 24px 24px",
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center"
            }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`MILITARY CONTRIBUTIONS (${militaryContributionsData?.total || 0} ${(militaryContributionsData?.total || 0) === 1 ? "Record" : "Records"})`}
            </Typography>

            <Tooltip
              title={masterInquiryMemberDetails?.payFrequencyId == 2 ? "You cannot add a contribution to someone paid monthly." : ""}
              placement="top">
              <span>
                <Button
                  variant="contained"
                  color="primary"
                  disabled={masterInquiryMemberDetails?.payFrequencyId == 2}
                  onClick={onAddContribution}>
                  Add Military Contribution
                </Button>
              </span>
            </Tooltip>
          </div>

          <DSMGrid
            preferenceKey={CAPTIONS.MILITARY_CONTRIBUTIONS}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: militaryContributionsData?.results,
              columnDefs: columnDefs
            }}
          />

          {!!militaryContributionsData && militaryContributionsData?.results?.length > 0 && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={(value: number) => {
                setPageNumber(value - 1);
                setInitialSearchLoaded(true);
              }}
              pageSize={pageSize}
              setPageSize={(value: number) => {
                setPageSize(value);
                setPageNumber(1);
                setInitialSearchLoaded(true);
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
