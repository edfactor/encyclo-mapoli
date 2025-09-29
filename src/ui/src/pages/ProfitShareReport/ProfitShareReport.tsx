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
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ProfitSummary from "../PAY426Reports/ProfitSummary/ProfitSummary";
import CommitModal from "./CommitModal";
import ProfitShareReportGrid from "./ProfitShareReportGrid";
import ProfitShareReportSearchFilters from "./ProfitShareReportSearchFilters";

const ProfitShareReport = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedPresetParams, setSelectedPresetParams] = useState<FilterParams | null>(null);
  const [isLoadingTotals, setIsLoadingTotals] = useState(false);
  const [currentSearchParams, setCurrentSearchParams] = useState<any>(null);
  const [isInitialSearchLoaded, setIsInitialSearchLoaded] = useState(false);

  const { yearEndProfitSharingReportTotals } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch] = useLazyGetYearEndProfitSharingReportTotalsQuery();
  const [finalizeReport, { isLoading: isFinalizing }] = useFinalizeReportMutation();

  // Load both tables when page loads - this is consistent with other pages which only display data and do not take input.
  useEffect(() => {
    if (hasToken && profitYear) {
      setIsLoadingTotals(true);

      const totalsRequest = {
        profitYear: profitYear,
        useFrozenData: false,
        badgeNumber: null
      };

      triggerSearch(totalsRequest, false)
        .then((result) => {
          setIsLoadingTotals(false);
          if (result.data) {
            dispatch(setYearEndProfitSharingReportQueryParams(profitYear));
          }
        })
        .catch((error) => {
          setIsLoadingTotals(false);
        });
    }
  }, [hasToken, profitYear, triggerSearch, dispatch]);

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
    setCurrentSearchParams(null);
    setIsInitialSearchLoaded(false);
  };

  const handleSearchParamsUpdate = (searchParams: any) => {
    setCurrentSearchParams(searchParams);
    setIsInitialSearchLoaded(true);
  };

  const handleStatusChange = (newStatus: string, statusName?: string) => {
    // Check if the status is "Complete" and trigger search with archive=true
    if (statusName === "Complete" && profitYear) {
      setIsLoadingTotals(true);

      const totalsRequest = {
        profitYear: profitYear,
        useFrozenData: true,
        badgeNumber: null,
        archive: true
      };

      triggerSearch(totalsRequest, false)
        .then((result) => {
          setIsLoadingTotals(false);
          if (result.data) {
            dispatch(setYearEndProfitSharingReportQueryParams(profitYear));
          }
        })
        .catch((error) => {
          console.error("Archive search failed:", error);
          setIsLoadingTotals(false);
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
    if (currentSearchParams && isInitialSearchLoaded) {
      setTimeout(() => {
        document.querySelector('[data-testid="results-grid"]')?.scrollIntoView({
          behavior: "smooth",
          block: "start"
        });
      }, 100);
    }
  }, [currentSearchParams, isInitialSearchLoaded]);

  const renderActionNode = () => {
    if (!yearEndProfitSharingReportTotals) return null;

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

            {isLoadingTotals ? (
              <Box sx={{ display: "flex", justifyContent: "center", p: 3 }}>
                <CircularProgress />
              </Box>
            ) : (
              <Box sx={{ px: 3, mt: 2 }}>
                <ProfitShareTotalsDisplay totalsData={yearEndProfitSharingReportTotals} />
              </Box>
            )}
          </Box>
        </Grid>

        <Grid width="100%">
          <ProfitSummary
            onPresetParamsChange={handlePresetParamsChange}
            frozenData={false}
          />
        </Grid>

        {selectedPresetParams && (
          <Grid
            width="100%"
            data-testid="filter-section">
            <DSMAccordion title="Filter">
              <ProfitShareReportSearchFilters
                profitYear={profitYear}
                presetParams={selectedPresetParams}
                onSearchParamsUpdate={handleSearchParamsUpdate}
              />
              {currentSearchParams && (
                <Box
                  sx={{ mt: 3 }}
                  data-testid="results-grid">
                  <ProfitShareReportGrid
                    searchParams={currentSearchParams}
                    isInitialSearchLoaded={isInitialSearchLoaded}
                    profitYear={profitYear}
                  />
                </Box>
              )}
            </DSMAccordion>
          </Grid>
        )}
      </Grid>

      <CommitModal
        open={isModalOpen}
        onClose={handleCancel}
        onCommit={handleCommit}
        isFinalizing={isFinalizing}
      />
    </Page>
  );
};

export default ProfitShareReport;
