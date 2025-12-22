import { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { ApiMessageAlert, DSMAccordion, Page } from "smart-ui-library";
import { ConfirmationDialog } from "../../../components/ConfirmationDialog";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";

import { CircularProgress, Divider, Grid } from "@mui/material";

import { CAPTIONS } from "../../../constants";
import { useLazyGetAccountingRangeToCurrent } from "../../../hooks/useFiscalCalendarYear";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import { closeDrawer, openDrawer, setFullscreen } from "../../../reduxstore/slices/generalSlice";
import { RootState } from "../../../reduxstore/store";
import { StartAndEndDateRequest } from "../../../reduxstore/types";
import { useTerminationState } from "./hooks/useTerminationState";
import TerminationGrid from "./TerminationGrid";
import TerminationSearchFilter from "./TerminationSearchFilter";

export interface TerminationSearchRequest extends StartAndEndDateRequest {
  forfeitureStatus: string;
  archive?: boolean;
  excludeZeroBalance?: boolean;
  excludeZeroAndFullyVested?: boolean;
  vestedBalanceValue?: number | null;
  vestedBalanceOperator?: number | null;
}

const Termination = () => {
  const dispatch = useDispatch();
  const [fetchAccountingRange, { data: fiscalData }] = useLazyGetAccountingRangeToCurrent(6);
  const { state, actions } = useTerminationState();
  const [isFetching, setIsFetching] = useState(false);
  const [showUnsavedChangesDialog, setShowUnsavedChangesDialog] = useState(false);
  const [isGridExpanded, setIsGridExpanded] = useState(false);
  const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = useState(false);

  // Get current drawer state from Redux
  const isDrawerOpen = useSelector((state: RootState) => state.general.isDrawerOpen);

  // Function to scroll to top - only used for error cases
  const scrollToTop = useCallback(() => {
    window.scrollTo({ top: 0, behavior: "smooth" });
  }, []);

  // Use the navigation guard hook
  useUnsavedChangesGuard(state.hasUnsavedChanges);

  // Render action node with status dropdown
  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={actions.handleStatusChange} />;
  };

  useEffect(() => {
    fetchAccountingRange();
  }, [fetchAccountingRange]);

  const isCalendarDataLoaded = !!fiscalData?.fiscalBeginDate && !!fiscalData?.fiscalEndDate;

  // Handler to toggle grid expansion
  const handleToggleGridExpand = () => {
    setIsGridExpanded((prev) => {
      if (!prev) {
        // Expanding: remember drawer state and close it
        setWasDrawerOpenBeforeExpand(isDrawerOpen || false);
        dispatch(closeDrawer());
        dispatch(setFullscreen(true));
      } else {
        // Collapsing: restore previous drawer state
        dispatch(setFullscreen(false));
        if (wasDrawerOpenBeforeExpand) {
          dispatch(openDrawer());
        }
      }
      return !prev;
    });
  };

  // Add listener for error messages to scroll to top
  useEffect(() => {
    const handleMessageEvent = (event: Event) => {
      // Check if the message is an error related to Termination
      const customEvent = event as CustomEvent;
      if (customEvent.detail?.key === "TerminationSave" && customEvent.detail?.message?.type === "error") {
        scrollToTop();
      }
    };

    // These event listeneres are global to the window so that
    // when there is a message for the user, we can scroll to top to show it
    window.addEventListener("dsmMessage", handleMessageEvent);

    return () => {
      window.removeEventListener("dsmMessage", handleMessageEvent);
    };
  }, [scrollToTop]);

  return (
    <Page
      label={isGridExpanded ? "" : CAPTIONS.TERMINATIONS}
      actionNode={isGridExpanded ? undefined : renderActionNode()}>
      <div>
        {!isGridExpanded && <ApiMessageAlert commonKey="TerminationSave" />}
        <Grid
          container
          rowSpacing="24px">
          {!isGridExpanded && (
            <Grid width={"100%"}>
              <Divider />
            </Grid>
          )}
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
              {!isGridExpanded && (
                <Grid width={"100%"}>
                  <DSMAccordion title="Filter">
                    <TerminationSearchFilter
                      fiscalData={fiscalData}
                      onSearch={actions.handleSearch}
                      setInitialSearchLoaded={actions.setInitialSearchLoaded}
                      hasUnsavedChanges={state.hasUnsavedChanges}
                      setHasUnsavedChanges={actions.handleUnsavedChanges}
                      isFetching={isFetching}
                    />
                  </DSMAccordion>
                </Grid>
              )}
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
                  onErrorOccurred={scrollToTop}
                  onLoadingChange={setIsFetching}
                  onShowUnsavedChangesDialog={() => setShowUnsavedChangesDialog(true)}
                  isGridExpanded={isGridExpanded}
                  onToggleExpand={handleToggleGridExpand}
                />
              </Grid>
            </>
          )}
        </Grid>
      </div>

      <ConfirmationDialog
        open={showUnsavedChangesDialog}
        title="Unsaved Changes"
        description="Please save your changes before changing pages."
        onClose={() => setShowUnsavedChangesDialog(false)}
      />
    </Page>
  );
};

export default Termination;
