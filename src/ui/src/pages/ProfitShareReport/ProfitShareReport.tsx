import { Box, Button, CircularProgress, Divider, Grid, Typography } from "@mui/material";
import ProfitShareTotalsDisplay from "components/ProfitShareTotalsDisplay";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useFinalizeReportMutation, useLazyGetYearEndProfitSharingReportTotalsQuery } from "reduxstore/api/YearsEndApi";
import { setYearEndProfitSharingReportQueryParams } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { FilterParams } from "reduxstore/types";
import { DSMAccordion, Page, SmartModal } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ProfitSummary from "../PAY426Reports/ProfitSummary/ProfitSummary";
import ProfitShareReportGrid from "./ProfitShareReportGrid";
import ProfitShareReportSearchFilters from "./ProfitShareReportSearchFilters";

const ProfitShareReport = () => {
  const [initialDataLoaded, setInitialDataLoaded] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedPresetParams, setSelectedPresetParams] = useState<FilterParams | null>(null);

  const { yearEndProfitSharingReportTotals, yearEndProfitSharingReport } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch] = useLazyGetYearEndProfitSharingReportTotalsQuery();
  const [finalizeReport, { isLoading: isFinalizing }] = useFinalizeReportMutation();

  useEffect(() => {
    if (hasToken && profitYear && !initialDataLoaded) {
      const totalsRequest = {
        profitYear: profitYear,
        useFrozenData: true,
        badgeNumber: null
      };

      triggerSearch(totalsRequest, false)
        .then((result) => {
          if (result.data) {
            dispatch(setYearEndProfitSharingReportQueryParams(profitYear));
            setInitialDataLoaded(true);
          }
        })
        .catch((error) => {
          console.error("Initial totals search failed:", error);
        });
    }
  }, [hasToken, profitYear, initialDataLoaded, triggerSearch, dispatch]);

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

  const handlePresetParamsChange = (params: FilterParams | null) => {
    setSelectedPresetParams(params);
  };

  const handleStatusChange = (newStatus: string, statusName?: string) => {
    // Check if the status is "Complete" and trigger search with archive=true
    if (statusName === "Complete" && profitYear) {
      const totalsRequest = {
        profitYear: profitYear,
        useFrozenData: true,
        badgeNumber: null,
        archive: true
      };

      triggerSearch(totalsRequest, false)
        .then((result) => {
          if (result.data) {
            dispatch(setYearEndProfitSharingReportQueryParams(profitYear));
          }
        })
        .catch((error) => {
          console.error("Archive search failed:", error);
        });
    }
  };

  useEffect(() => {
    if (selectedPresetParams) {
      setTimeout(() => {
        document.querySelector('[data-testid="filter-section"]')?.scrollIntoView({
          behavior: "smooth",
          block: "start"
        });
      }, 100);
    }
  }, [selectedPresetParams]);

  useEffect(() => {
    if (yearEndProfitSharingReport?.response.results?.length) {
      setTimeout(() => {
        document.querySelector('[data-testid="results-grid"]')?.scrollIntoView({
          behavior: "smooth",
          block: "start"
        });
      }, 100);
    }
  }, [yearEndProfitSharingReport?.response.results]);

  const renderActionNode = () => {
    if (!initialDataLoaded || !yearEndProfitSharingReportTotals) return null;

    return (
      <div className="flex h-10 items-center gap-2">
        <Button
          onClick={() => setIsModalOpen(true)}
          variant="outlined"
          className="h-10 min-w-fit whitespace-nowrap">
          Commit
        </Button>
        <StatusDropdownActionNode onStatusChange={handleStatusChange} />
      </div>
    );
  };

  return (
    <Page
      label={CAPTIONS.PROFIT_SHARE_REPORT}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width="100%">
          <Box sx={{ mb: 3 }}>
            <div style={{ padding: "0 24px 0 24px" }}>
              <Typography
                variant="h2"
                sx={{ color: "#0258A5" }}>
                {`${CAPTIONS.PROFIT_SHARE_TOTALS}`}
              </Typography>
            </div>

            {yearEndProfitSharingReportTotals && (
              <Box sx={{ px: 3, mt: 2 }}>
                <ProfitShareTotalsDisplay totalsData={yearEndProfitSharingReportTotals} />
              </Box>
            )}
          </Box>
        </Grid>

        <Grid width="100%">
          {!initialDataLoaded ? (
            <Box sx={{ display: "flex", justifyContent: "center", p: 3 }}>
              <CircularProgress />
            </Box>
          ) : (
            <ProfitSummary onPresetParamsChange={handlePresetParamsChange} />
          )}
        </Grid>

        {selectedPresetParams && (
          <Grid
            width="100%"
            data-testid="filter-section">
            <DSMAccordion title="Filter">
              <ProfitShareReportSearchFilters
                profitYear={profitYear}
                presetParams={selectedPresetParams}
              />
              {yearEndProfitSharingReport?.response.results &&
                yearEndProfitSharingReport.response.results.length > 0 && (
                  <Box
                    sx={{ mt: 3 }}
                    data-testid="results-grid">
                    <ProfitShareReportGrid
                      data={yearEndProfitSharingReport.response.results}
                      isLoading={false}
                      pageNumber={1}
                      pageSize={yearEndProfitSharingReport.response.results.length}
                      sortParams={{ sortBy: "badgeNumber", isSortDescending: true }}
                      recordCount={yearEndProfitSharingReport.response.results.length}
                      onPageChange={() => {}}
                      onPageSizeChange={() => {}}
                      onSortChange={() => {}}
                    />
                  </Box>
                )}
            </DSMAccordion>
          </Grid>
        )}
      </Grid>

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
        <div>
          <table
            cellPadding={20}
            style={{ width: "100%" }}>
            <tr>
              <td>Earn Points</td>
              <td>How much money goes towards allocating a contribution</td>
            </tr>
            <tr>
              <td>ZeroContributionReason</td>
              <td>
                {" "}
                Why did an employee get a zero contribution? Normal, Under21, Terminated (Vest Only), Retired, Soon to
                be Retired
              </td>
            </tr>

            <tr>
              <td>EmployeeType</td>
              <td>
                {" "}
                Is this a "new employee in the plan" - aka this is your first year &gt;21 and &gt;1000 hours - employee
                may already have V-ONLY records
              </td>
            </tr>
            <tr>
              <td>PsCertificateIssuedDate</td>
              <td>
                {" "}
                indicates that this employee should get a physically printed certificate. It is a proxy for Earn Points
                &gt; 0.
              </td>
            </tr>
          </table>
        </div>
        <div>
          This "COMMIT" is safe to run multiple times, until the "Master Update" is saved. Running "COMMIT" after Master
          Update means that the Master Update will potentially not have computed the correct earnings and contribution
          amounts.
        </div>
      </SmartModal>
    </Page>
  );
};

export default ProfitShareReport;
