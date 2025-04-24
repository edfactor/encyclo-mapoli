import { Box,Divider, Typography, CircularProgress } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import DSMCollapsedAccordion from "components/DSMCollapsedAccordion";
import ProfitShareTotalsDisplay from "components/ProfitShareTotalsDisplay";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState, useCallback } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { setYearEndProfitSharingReportQueryParams } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { Page, ISortParams } from "smart-ui-library";
import { CAPTIONS} from "../../constants";
import ProfitShareReportGrid from "./ProfitShareReportGrid";

const ProfitShareReport = () => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: true
  });
    const [initialDataLoaded, setInitialDataLoaded] = useState(false);
  
  const { yearEndProfitSharingReport, yearEndProfitSharingReportQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch, { isFetching }] = useLazyGetYearEndProfitSharingReportQuery();

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

  const renderActionNode = () => {
    return (
      <div className="flex items-center gap-2 h-10">
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
            <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
              <CircularProgress />
            </Box>
          ) : (
            <DSMCollapsedAccordion
              isCollapsedOnRender={true}
              expandable={true}
              title={`${CAPTIONS.PROFIT_SHARE_REPORT}${!isFetching && yearEndProfitSharingReport?.response.total !== undefined ? ` (${yearEndProfitSharingReport.response.total} records)` : ''}`}>
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
            </DSMCollapsedAccordion>
          )}
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ProfitShareReport;
