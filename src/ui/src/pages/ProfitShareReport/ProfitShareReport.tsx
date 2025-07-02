import { Box, Button, CircularProgress, Divider, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import ProfitShareTotalsDisplay from "components/ProfitShareTotalsDisplay";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useFinalizeReportMutation, useLazyGetYearEndProfitSharingReportQuery } from "reduxstore/api/YearsEndApi";
import { setYearEndProfitSharingReportQueryParams } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { Page, SmartModal, DSMAccordion } from "smart-ui-library";
import { CAPTIONS} from "../../constants";
import ProfitSummary from "../PAY426Reports/PAY426-9/ProfitSummary";
import ProfitShareReportSearchFilters from "./ProfitShareReportSearchFilters";
import ReportGrid from "../PAY426Reports/PAY426N/ReportGrid";
import { FilterParams } from "reduxstore/types";

const ProfitShareReport = () => {
  const [initialDataLoaded, setInitialDataLoaded] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedPresetParams, setSelectedPresetParams] = useState<FilterParams | null>(null);
  
  const { yearEndProfitSharingReport } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const [triggerSearch] = useLazyGetYearEndProfitSharingReportQuery();
  const [finalizeReport, { isLoading: isFinalizing }] = useFinalizeReportMutation();

  useEffect(() => {
    if (hasToken && profitYear && !initialDataLoaded) {
      const request = {
        reportId: 4,
        profitYear: profitYear,
        pagination: {
          skip: 0,
          take: 10,
          sortBy: "badgeNumber",
          isSortDescending: true
        }
      };
      
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
            <ProfitSummary onPresetParamsChange={handlePresetParamsChange} />
          )}
        </Grid2>

        {selectedPresetParams && (
          <Grid2 width="100%">
            <DSMAccordion title="Filter">
              <ProfitShareReportSearchFilters profitYear={profitYear} presetParams={selectedPresetParams} />
            </DSMAccordion>
          </Grid2>
        )}

        {selectedPresetParams && (
          <Grid2 width="100%">
            <ReportGrid params={selectedPresetParams} onLoadingChange={() => {}} />
          </Grid2>
        )}
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
