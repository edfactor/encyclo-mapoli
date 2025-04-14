import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetMilitaryContributionsQuery } from "reduxstore/api/MilitaryApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import { Typography, Button } from "@mui/material";
import MasterInquiryEmployeeDetails from "../../MasterInquiry/MasterInquiryEmployeeDetails";
import { GetMilitaryContributionColumns } from "./MilitaryContributionFormGridColumns";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";

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
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const profitYear = useDecemberFlowProfitYear();
  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.inquiry);
  const [fetchContributions, { data: contributionsData, isFetching }] = useLazyGetMilitaryContributionsQuery();
  
  const onSearch = useCallback(async () => {
    if (masterInquiryEmployeeDetails) {
      await fetchContributions({
        badgeNumber: Number(masterInquiryEmployeeDetails.badgeNumber),
        profitYear: profitYear,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      });
    }
  }, [pageNumber, pageSize, sortParams, masterInquiryEmployeeDetails, fetchContributions]);

  useEffect(() => {
    if (initialSearchLoaded && masterInquiryEmployeeDetails) {
      onSearch();
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, onSearch, masterInquiryEmployeeDetails]);

 
  const columnDefs = useMemo(() => GetMilitaryContributionColumns(), []);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);

  return (
    <>
      {masterInquiryEmployeeDetails && (
        <MasterInquiryEmployeeDetails details={masterInquiryEmployeeDetails} />
      )}

      {contributionsData && (
        <>
          <div style={{ padding: "0 24px 24px 24px", display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`MILITARY CONTRIBUTIONS (${contributionsData?.response?.total || 0} ${(contributionsData?.response?.total || 0) === 1 ? "Record" : "Records"})`}
            </Typography>

            <Button
              variant="contained"
              color="primary"
              onClick={onAddContribution}
            >
              Add Military Contribution
            </Button>
          </div>

          <DSMGrid
            preferenceKey={CAPTIONS.MILITARY_CONTRIBUTIONS}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: contributionsData?.response?.results,
              columnDefs: columnDefs
            }}
          />

          {!!contributionsData && contributionsData.response.results.length > 0 && (          
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
              recordCount={contributionsData.response.total}
            />
          )}
        </>
      )}
    </>
  );
};

export default MilitaryContributionGrid;