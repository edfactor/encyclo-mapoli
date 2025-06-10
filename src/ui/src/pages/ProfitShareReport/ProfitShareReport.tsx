import { Box, Button, CircularProgress, Divider, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import ProfitShareTotalsDisplay from "components/ProfitShareTotalsDisplay";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useFinalizeReportMutation, useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { setYearEndProfitSharingReportQueryParams } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { ISortParams, Page, SmartModal } from "smart-ui-library";
import ReportSummary from "../../components/ReportSummary";
import { CAPTIONS } from "../../constants";
import ProfitShareReportGrid from "./ProfitShareReportGrid";

const ProfitShareReport = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: true
  });
  const [initialDataLoaded, setInitialDataLoaded] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  
  const { yearEndProfitSharingReport, yearEndProfitSharingReportQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch, { isFetching }] = useLazyGetYearEndProfitSharingReportQuery();
  const [finalizeReport, { isLoading: isFinalizing }] = useFinalizeReportMutation();

  const createSearchRequest = useCallback((pYear: number) => {
    return {
      isYearEnd: false,
      includeActiveEmployees: true,
      includeInactiveEmployees: true,
      includeEmployeesTerminatedThisYear: false,
      includeTerminatedEmployees: true,
      includeBeneficiaries: false,
      includeEmployeesWithPriorProfitSharingAmounts: true,
      includeEmployeesWithNoPriorProfitSharingAmounts: true,
      profitYear: pYear,
      pagination: {
        skip: pageNumber * pageSize,
        take: pageSize,
        sortBy: sortParams.sortBy,
        isSortDescending: sortParams.isSortDescending
      }
    };
  }, [pageNumber, pageSize, sortParams]);

  // initial load on page mount
  useEffect(() => {
    if (hasToken && profitYear && !initialDataLoaded) {
      const request = createSearchRequest(profitYear);
      
      triggerSearch(request, false)
        .then((result) => {
          if (result.data) {
            dispatch(setYearEndProfitSharingReportQueryParams(profitYear));
            setInitialDataLoaded(true);
          }
        })
        .catch((error) => {
          console.error("Initial search failed:", error);
        });
    }
  }, [hasToken, profitYear, initialDataLoaded, createSearchRequest, triggerSearch, dispatch]);

  
  const handlePaginationOrSortChange = useCallback(() => {
    if (initialDataLoaded && yearEndProfitSharingReportQueryParams?.profitYear) {
      const request = createSearchRequest(yearEndProfitSharingReportQueryParams.profitYear);
      triggerSearch(request, false).catch((error) => {
        console.error("Pagination search failed:", error);
      });
    }
  }, [initialDataLoaded, yearEndProfitSharingReportQueryParams, createSearchRequest, triggerSearch]);

  const handlePageChange = (value: number) => {
    setPageNumber(value - 1);
    setTimeout(() => handlePaginationOrSortChange(), 0);
  };

  const handlePageSizeChange = (value: number) => {
    setPageSize(value);
    setPageNumber(0);
    setTimeout(() => handlePaginationOrSortChange(), 0);
  };

  const handleSortChange = (update: ISortParams) => {
    setSortParams(update);
    setTimeout(() => handlePaginationOrSortChange(), 0);
  };

  const handleCommit = async () => {
    if (profitYear) {
      try {
        await finalizeReport({ profitYear });
        setIsModalOpen(false);
      } catch (error) {
        console.error("Failed to finalize report:", error);
      }
    }
  };

  const handleCancel = () => {
    setIsModalOpen(false);
  };

  const renderActionNode = () => {
    if (!initialDataLoaded || !yearEndProfitSharingReport) return null;

    return (
      <div className="flex items-center gap-2 h-10">
        <Button 
          onClick={() => setIsModalOpen(true)}
          variant="outlined"
          className="h-10 whitespace-nowrap min-w-fit">
          Commit
        </Button>
        <StatusDropdownActionNode />
      </div>
    );
  };

  return (
    <Page
      label={CAPTIONS.PROFIT_SHARE_REPORT}
      actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width="100%">
          <Box sx={{ mb: 3 }}>
            <div style={{ padding: "0 24px 0 24px" }}>
              <Typography
                variant="h2"
                sx={{ color: "#0258A5" }}>
                {`${CAPTIONS.PROFIT_SHARE_TOTALS}`}
              </Typography>
            </div>

            {yearEndProfitSharingReport && (
              <Box sx={{ px: 3, mt: 2 }}>
                <ProfitShareTotalsDisplay data={yearEndProfitSharingReport} />
              </Box>
            )}
          </Box>
        </Grid2>

        <Grid2 width="100%">
          {!initialDataLoaded ? (
            <Box sx={{ display: "flex", justifyContent: "center", p: 3 }}>
              <CircularProgress />
            </Box>
          ) : (
            <>
              <ReportSummary report={yearEndProfitSharingReport} />
              <ProfitShareReportGrid
                data={yearEndProfitSharingReport?.response.results || []}
                isLoading={isFetching}
                pageNumber={pageNumber}
                pageSize={pageSize}
                sortParams={sortParams}
                recordCount={yearEndProfitSharingReport?.response.total || 0}
                onPageChange={handlePageChange}
                onPageSizeChange={handlePageSizeChange}
                onSortChange={handleSortChange}
              />
            </>
          )}
        </Grid2>
      </Grid2>

      <SmartModal
        open={isModalOpen}
        onClose={handleCancel}
        actions={[
          <Button
            onClick={handleCommit}
            variant="contained"
            color="primary"
            disabled={isFinalizing}
            className="mr-2">
            {isFinalizing ? (
              <CircularProgress
                size={24}
                color="inherit"
              />
            ) : (
              "Yes, Commit"
            )}
          </Button>,
          <Button
            onClick={handleCancel}
            variant="outlined">
            No, Cancel
          </Button>
        ]}
        title="Are you ready to Commit?">
        Committing this change will update and save:
      </SmartModal>
    </Page>
  );
};

export default ProfitShareReport;
