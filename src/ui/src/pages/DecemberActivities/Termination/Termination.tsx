import { useCallback, useEffect, useState } from "react";
import { ApiMessageAlert, DSMAccordion, Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";

import { HelpOutline } from "@mui/icons-material";
import { Box, CircularProgress, Divider, Grid, IconButton, Tooltip } from "@mui/material";
import { useNavigate } from "react-router-dom";

import { CAPTIONS } from "../../../constants";
import { useLazyGetAccountingRangeToCurrent } from "../../../hooks/useFiscalCalendarYear";
import { useTerminationState } from "../../../hooks/useTerminationState";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import { StartAndEndDateRequest } from "../../../reduxstore/types";
import TerminationGrid from "./TerminationGrid";
import TerminationSearchFilter from "./TerminationSearchFilter";

export interface TerminationSearchRequest extends StartAndEndDateRequest {
  forfeitureStatus: string;
  archive?: boolean;
}

const Termination = () => {
  const [fetchAccountingRange, { data: fiscalData, isLoading: isRangeLoading }] = useLazyGetAccountingRangeToCurrent(6);
  const { state, actions } = useTerminationState();
  const navigate = useNavigate();
  const [isDataFetching, setIsDataFetching] = useState(false);

  // Function to scroll to top - only used for error cases
  const scrollToTop = useCallback(() => {
    window.scrollTo({ top: 0, behavior: "smooth" });
  }, []);

  // Handle loading state changes from TerminationGrid
  const handleLoadingChange = useCallback((isLoading: boolean) => {
    setIsDataFetching(isLoading);
  }, []);

  // Use the navigation guard hook
  useUnsavedChangesGuard(state.hasUnsavedChanges);

  // Navigate to documentation page with terminations guide selected
  const handleHelpClick = () => {
    navigate("/documentation?doc=terminations-business-guide");
  };

  // Modify renderActionNode to include both status dropdown and help button
  const renderActionNode = () => {
    return (
      <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
        <Tooltip title="View Terminations Documentation">
          <IconButton
            onClick={handleHelpClick}
            color="primary"
            size="medium"
            aria-label="help documentation">
            <HelpOutline />
          </IconButton>
        </Tooltip>
        <StatusDropdownActionNode onStatusChange={actions.handleStatusChange} />
      </Box>
    );
  };

  useEffect(() => {
    fetchAccountingRange();
  }, [fetchAccountingRange]);

  const isCalendarDataLoaded = !!fiscalData?.fiscalBeginDate && !!fiscalData?.fiscalEndDate;

  // Add listener for error messages to scroll to top
  useEffect(() => {
    const handleMessageEvent = (event: CustomEvent) => {
      // Check if the message is an error related to Termination
      if (event.detail?.key === "TerminationSave" && event.detail?.message?.type === "error") {
        scrollToTop();
      }
    };

    window.addEventListener("dsmMessage" as any, handleMessageEvent);

    return () => {
      window.removeEventListener("dsmMessage" as any, handleMessageEvent);
    };
  }, [scrollToTop]);

  return (
    <Page
      label={CAPTIONS.TERMINATIONS}
      actionNode={renderActionNode()}>
      <div>
        <ApiMessageAlert commonKey="TerminationSave" />
        <Grid
          container
          rowSpacing="24px">
          <Grid width={"100%"}>
            <Divider />
          </Grid>
          {!isCalendarDataLoaded ? (
            <Grid
              width={"100%"}
              container
              justifyContent="center"
              padding={4}>
              <CircularProgress />
            </Grid>
          ) : (
            <>
              <Grid width={"100%"}>
                <DSMAccordion title="Filter">
                  <TerminationSearchFilter
                    fiscalData={fiscalData}
                    onSearch={actions.handleSearch}
                    setInitialSearchLoaded={actions.setInitialSearchLoaded}
                    hasUnsavedChanges={state.hasUnsavedChanges}
                    isFetching={isDataFetching}
                  />
                </DSMAccordion>
              </Grid>
              <Grid width="100%">
                <TerminationGrid
                  setInitialSearchLoaded={actions.setInitialSearchLoaded}
                  initialSearchLoaded={state.initialSearchLoaded}
                  searchParams={state.searchParams}
                  resetPageFlag={state.resetPageFlag}
                  onUnsavedChanges={actions.handleUnsavedChanges}
                  hasUnsavedChanges={state.hasUnsavedChanges}
                  fiscalData={fiscalData}
                  shouldArchive={state.shouldArchive}
                  onArchiveHandled={actions.handleArchiveHandled}
                  onErrorOccurred={scrollToTop} // Pass down the error handler
                  onLoadingChange={handleLoadingChange}
                />
              </Grid>
            </>
          )}
        </Grid>
      </div>
    </Page>
  );
};

export default Termination;
