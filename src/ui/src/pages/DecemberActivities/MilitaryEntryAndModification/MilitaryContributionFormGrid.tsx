import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetMilitaryContributionsQuery } from "reduxstore/api/MilitaryApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { Typography, Button } from "@mui/material";
import StandaloneMemberDetails from "../../MasterInquiry/StandaloneMemberDetails";
import { GetMilitaryContributionColumns } from "./MilitaryContributionFormGridColumns";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import {MissiveAlertProvider} from "../../MasterInquiry/MissiveAlertContext";

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
  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.inquiry);
  const { militaryContributionsData } = useSelector((state: RootState) => state.military);
  const [fetchContributions, { isFetching }] = useLazyGetMilitaryContributionsQuery();

  const onSearch = useCallback(async () => {
    if (masterInquiryEmployeeDetails) {
      await fetchContributions({
        badgeNumber: Number(masterInquiryEmployeeDetails.badgeNumber),
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
  }, [pageNumber, pageSize, sortParams, masterInquiryEmployeeDetails, fetchContributions, profitYear]);

  useEffect(() => {
    if (initialSearchLoaded && masterInquiryEmployeeDetails) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, masterInquiryEmployeeDetails, onSearch]);

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
  
  return (
    <>
      {masterInquiryEmployeeDetails && profitYear > 0 && (
          <MissiveAlertProvider>
            <StandaloneMemberDetails
              memberType={masterInquiryEmployeeDetails.isEmployee ? 1 : 2}
              id={masterInquiryEmployeeDetails.id}
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

            <Button
              variant="contained"
              color="primary"
              onClick={onAddContribution}>
              Add Military Contribution
            </Button>
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
