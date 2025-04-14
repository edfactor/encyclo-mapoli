import { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { Path, useNavigate } from "react-router";
import { useLazyGetMilitaryContributionsQuery } from "reduxstore/api/MilitaryApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, numberToCurrency, Pagination } from "smart-ui-library";
import { TotalsGrid } from "../../../components/TotalsGrid/TotalsGrid";
import { Typography, Button } from "@mui/material";
import MasterInquiryEmployeeDetails from "../../MasterInquiry/MasterInquiryEmployeeDetails";
import { GetMilitaryContributionColumns } from "./MilitaryContributionFormGridColumns";
import { CAPTIONS } from "../../../constants";

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

  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.inquiry);
  const [fetchContributions, { data: contributionsData, isFetching }] = useLazyGetMilitaryContributionsQuery();
  const navigate = useNavigate();

  const onSearch = useCallback(async () => {
    if (masterInquiryEmployeeDetails) {
      await fetchContributions({
        badgeNumber: Number(masterInquiryEmployeeDetails.badgeNumber),
        profitYear: 2024,
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

  // Calculate totals if data exists
  const totalExecutiveHours = contributionsData?.results?.reduce(
    (sum, item) => sum + (item.hoursExecutive || 0), 0) || 0;
  const totalExecutiveDollars = contributionsData?.results?.reduce(
    (sum, item) => sum + (item.incomeExecutive || 0), 0) || 0;
  const totalOracleHours = contributionsData?.results?.reduce(
    (sum, item) => sum + (item.currentHoursYear || 0), 0) || 0;
  const totalOracleDollars = contributionsData?.results?.reduce(
    (sum, item) => sum + (item.currentIncomeYear || 0), 0) || 0;

  return (
    <>
      {masterInquiryEmployeeDetails && (
        <MasterInquiryEmployeeDetails details={masterInquiryEmployeeDetails} />
      )}

      {contributionsData && (
        <>
          <div className="flex sticky top-0 z-10 bg-white">
            <TotalsGrid
              displayData={[[numberToCurrency(totalExecutiveDollars)]]}
              leftColumnHeaders={["Executive Dollars"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[totalExecutiveHours.toString()]]}
              leftColumnHeaders={["Executive Hours"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(totalOracleDollars)]]}
              leftColumnHeaders={["Oracle Dollars"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[totalOracleHours.toString()]]}
              leftColumnHeaders={["Oracle Hours"]}
              topRowHeaders={[]}></TotalsGrid>
          </div>

          <div style={{ padding: "0 24px 24px 24px", display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              {`MILITARY CONTRIBUTIONS (${contributionsData.total || 0} ${contributionsData.total === 1 ? "Record" : "Records"})`}
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
              rowData: contributionsData.results,
              columnDefs: columnDefs
            }}
          />

          {contributionsData.results.length > 0 && (
            <Pagination
              totalPages={Math.ceil((contributionsData.total || 0) / pageSize)}
              rowsPerPage={pageSize}
              page={pageNumber}
              isLoading={isFetching}
              onPageChange={(page) => setPageNumber(page)}
              onRowsPerPageChange={(rowsPerPage) => {
                setPageSize(rowsPerPage);
                setPageNumber(0);
              }}
            />
          )}
        </>
      )}
    </>
  );
};

export default MilitaryContributionGrid;