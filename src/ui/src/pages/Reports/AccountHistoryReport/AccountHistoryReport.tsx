import { Button, Divider, Grid } from "@mui/material";
import React, { useCallback, useRef, useState } from "react";
import { DSMAccordion, numberToCurrency, Page, TotalsGrid } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import { CAPTIONS, GRID_KEYS } from "../../../constants";
import { SortParams, useGridPagination } from "../../../hooks/useGridPagination";
import { AdhocApi } from "../../../reduxstore/api/AdhocApi";
import { InquiryApi } from "../../../reduxstore/api/InquiryApi";
import { AccountHistoryReportRequest } from "../../../types/reports/AccountHistoryReportTypes";
import MasterInquiryMemberDetails from "../../InquiriesAndAdjustments/MasterInquiry/MasterInquiryMemberDetails";
import AccountHistoryReportFilterSection, {
  AccountHistoryReportFilterParams
} from "./AccountHistoryReportFilterSection";
import AccountHistoryReportTable from "./AccountHistoryReportTable";

const AccountHistoryReport: React.FC = () => {
  const [filterParams, setFilterParams] = useState<AccountHistoryReportFilterParams | null>(null);
  const [isFormDirty, setIsFormDirty] = useState(false);
  const [triggerSearch, { data, isFetching }] = AdhocApi.useLazyGetAccountHistoryReportQuery();
  const [downloadPdf, { isLoading: isDownloadingPdf }] = AdhocApi.useDownloadAccountHistoryReportPdfMutation();

  // Fetch member details when report data changes
  const profitYear = filterParams?.endDate ? filterParams.endDate.getFullYear() : new Date().getFullYear();
  const reportId = data?.response?.results?.[0]?.id ?? 0;
  const { data: memberDetails, isFetching: isFetchingMemberDetails } = InquiryApi.useGetProfitMasterInquiryMemberQuery(
    reportId > 0 ? { memberType: 1, id: reportId, profitYear } : { memberType: 1, id: 0, profitYear },
    { skip: reportId === 0 }
  );

  // Pagination state
  const filterParamsRef = useRef(filterParams);
  filterParamsRef.current = filterParams;

  const handleReportPaginationChange = useCallback(
    async (pageNumber: number, pageSize: number, sortParams: SortParams) => {
      const currentFilterParams = filterParamsRef.current;

      if (!currentFilterParams) return;

      // Trigger the query with pagination parameters
      const queryParams: AccountHistoryReportRequest = {
        badgeNumber: parseInt(currentFilterParams.badgeNumber, 10),
        startDate: currentFilterParams.startDate
          ? currentFilterParams.startDate.toISOString().split("T")[0]
          : undefined,
        endDate: currentFilterParams.endDate ? currentFilterParams.endDate.toISOString().split("T")[0] : undefined,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      };

      await triggerSearch(queryParams);
    },
    [triggerSearch]
  );

  const gridPagination = useGridPagination({
    initialPageSize: 25,
    initialSortBy: "profitYear",
    initialSortDescending: true,
    persistenceKey: GRID_KEYS.ACCOUNT_HISTORY_REPORT,
    onPaginationChange: handleReportPaginationChange
  });

  const paginationRef = useRef(gridPagination);
  paginationRef.current = gridPagination;

  const handleFilterChange = (params: AccountHistoryReportFilterParams) => {
    setFilterParams(params);
    setIsFormDirty(false); // Search was performed, form is no longer dirty

    // Trigger the query immediately with the search params
    const queryParams: AccountHistoryReportRequest = {
      badgeNumber: parseInt(params.badgeNumber, 10),
      startDate: params.startDate ? params.startDate.toISOString().split("T")[0] : undefined,
      endDate: params.endDate ? params.endDate.toISOString().split("T")[0] : undefined,
      pagination: {
        skip: 0,
        take: 25,
        sortBy: "profitYear",
        isSortDescending: true
      }
    };

    triggerSearch(queryParams);

    // Reset pagination to first page on new search
    paginationRef.current.resetPagination();
  };

  const handleReset = () => {
    setFilterParams(null);
    setIsFormDirty(false);
    paginationRef.current.resetPagination();
  };

  const handleFormDirty = useCallback(() => {
    // Mark form as dirty when user modifies filter criteria
    if (filterParams) {
      setIsFormDirty(true);
    }
  }, [filterParams]);

  const handleDownloadPdf = async () => {
    if (!filterParams) return;

    try {
      const blob = await downloadPdf({
        badgeNumber: parseInt(filterParams.badgeNumber, 10),
        startDate: filterParams.startDate ? filterParams.startDate.toISOString().split("T")[0] : undefined,
        endDate: filterParams.endDate ? filterParams.endDate.toISOString().split("T")[0] : undefined
      }).unwrap();

      // Create download link
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = `account-history-${filterParams.badgeNumber}-${new Date().toISOString().split("T")[0]}.pdf`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error("Error downloading PDF:", error);
    }
  };

  return (
    <PageErrorBoundary pageName="Account History Report">
      <Page label={CAPTIONS.DIVORCE_REPORT}>
        <Grid
          container
          rowSpacing="24px">
          <Grid width={"100%"}>
            <Divider />
          </Grid>
          <Grid width={"100%"}>
            <DSMAccordion title="Filter">
              <AccountHistoryReportFilterSection
                onFilterChange={handleFilterChange}
                onReset={handleReset}
                onFormDirty={handleFormDirty}
                isLoading={isFetching}
              />
            </DSMAccordion>
          </Grid>

          {filterParams && data?.response && (
            <>
              <Grid width="100%">
                <MissiveAlertProvider>
                  <MasterInquiryMemberDetails
                    memberType={1}
                    id={reportId.toString()}
                    profitYear={profitYear}
                    memberDetails={memberDetails || undefined}
                    isLoading={isFetchingMemberDetails}
                  />
                </MissiveAlertProvider>
              </Grid>
              <Grid width="100%">
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handleDownloadPdf}
                  disabled={isDownloadingPdf || !filterParams || isFormDirty}
                  title={isFormDirty ? "Click Search to update results before exporting" : undefined}>
                  {isDownloadingPdf ? "Generating PDF..." : "Download PDF Report"}
                </Button>
                {isFormDirty && (
                  <span className="ml-2 text-sm text-amber-600">
                    Search criteria changed. Click Search to update results before exporting.
                  </span>
                )}
              </Grid>
              <Grid width="100%">
                <div className="sticky top-0 z-10 flex items-start gap-2 bg-white py-2 [&_*]:!text-left">
                  {data.cumulativeTotals && (
                    <>
                      <div className="flex-1">
                        <TotalsGrid
                          displayData={[[numberToCurrency(data.cumulativeTotals.totalContributions)]]}
                          leftColumnHeaders={["Contributions"]}
                          topRowHeaders={[]}
                          breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                        />
                      </div>
                      <div className="flex-1">
                        <TotalsGrid
                          displayData={[[numberToCurrency(data.cumulativeTotals.totalEarnings)]]}
                          leftColumnHeaders={["Earnings"]}
                          topRowHeaders={[]}
                          breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                        />
                      </div>
                      <div className="flex-1">
                        <TotalsGrid
                          displayData={[[numberToCurrency(data.cumulativeTotals.totalForfeitures)]]}
                          leftColumnHeaders={["Forfeitures"]}
                          topRowHeaders={[]}
                          breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                        />
                      </div>
                      <div className="flex-1">
                        <TotalsGrid
                          displayData={[[numberToCurrency(data.cumulativeTotals.totalWithdrawals)]]}
                          leftColumnHeaders={["Withdrawals"]}
                          topRowHeaders={[]}
                          breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                        />
                      </div>
                      {data.cumulativeTotals.totalVestedBalance !== undefined && (
                        <div className="flex-1">
                          <TotalsGrid
                            displayData={[[numberToCurrency(data.cumulativeTotals.totalVestedBalance)]]}
                            leftColumnHeaders={["Vested Balance"]}
                            topRowHeaders={[]}
                            breakpoints={{ xs: 12, sm: 12, md: 12, lg: 12, xl: 12 }}
                          />
                        </div>
                      )}
                    </>
                  )}
                </div>
              </Grid>
              <Grid width="100%">
                <AccountHistoryReportTable
                  data={data}
                  isLoading={isFetching}
                  error={undefined}
                  showData={!!filterParams}
                  gridPagination={gridPagination}
                />
              </Grid>
            </>
          )}
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default AccountHistoryReport;
