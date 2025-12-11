import { CircularProgress, Divider, Grid } from "@mui/material";
import { useEffect, useState } from "react";
import { ApiMessageAlert, DSMAccordion, Page } from "smart-ui-library";
import { ConfirmationDialog } from "../../../components/ConfirmationDialog";
import FrozenYearWarning from "../../../components/FrozenYearWarning";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useLazyGetAccountingRangeToCurrent } from "../../../hooks/useFiscalCalendarYear";
import { useIsProfitYearFrozen } from "../../../hooks/useIsProfitYearFrozen";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import UnForfeitGrid from "./UnForfeitGrid";
import UnForfeitSearchFilter from "./UnForfeitSearchFilter";
import { useUnForfeitState } from "./useUnForfeitState";
import { useDispatch, useSelector } from "react-redux";
import { closeDrawer, openDrawer, setFullscreen } from "../../../reduxstore/slices/generalSlice";
import { RootState } from "../../../reduxstore/store";

const UnForfeit = () => {
  const { state, actions } = useUnForfeitState();
  const dispatch = useDispatch();
  const [fetchAccountingRange, { data: fiscalCalendarYear }] = useLazyGetAccountingRangeToCurrent(6);
  const profitYear = useDecemberFlowProfitYear();
  const isFrozen = useIsProfitYearFrozen(profitYear);
  const [isGridOnly, setIsGridOnly] = useState(false);
  const [isLeftPaneOpen, setIsLeftPaneOpen] = useState(true);
  const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = useState(false);

  // Get current drawer state from Redux
  const isDrawerOpen = useSelector((state: RootState) => state.general.isDrawerOpen);

  const [showUnsavedChangesDialog, setShowUnsavedChangesDialog] = useState(false);
  const [errorDialog, setErrorDialog] = useState<{ title: string; message: string } | null>(null);

  // Use the navigation guard hook
  useUnsavedChangesGuard(state.hasUnsavedChanges);

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={actions.handleStatusChange} />;
  };

  // Fetch the fiscal calendar year range on mount
  useEffect(() => {
    fetchAccountingRange();
  }, [fetchAccountingRange]);

  const isCalendarDataLoaded = !!fiscalCalendarYear?.fiscalBeginDate && !!fiscalCalendarYear?.fiscalEndDate;

  // Auto-trigger search when archive mode is activated
  useEffect(() => {
    if (state.shouldArchive) {
      actions.handleSearch();
    }
  }, [state.shouldArchive, actions]);

  // Handler to toggle grid-only mode and left pane
  const handleToggleGridExpand = () => {
    setIsGridOnly((prev) => {
      if (!prev) {
        // Expanding: remember drawer state and close it
        setWasDrawerOpenBeforeExpand(isDrawerOpen || false);
        dispatch(closeDrawer());
        dispatch(setFullscreen(true));
        setIsLeftPaneOpen(false);
      } else {
        // Collapsing: restore previous drawer state
        dispatch(setFullscreen(false));
        setIsLeftPaneOpen(true);
        if (wasDrawerOpenBeforeExpand) {
          dispatch(openDrawer());
        }
      }
      return !prev;
    });
  };

  return (
    <Page
      label={isGridOnly ? "" : `${CAPTIONS.REHIRE_FORFEITURES}`}
      actionNode={isGridOnly ? undefined : renderActionNode()}>
      {!isGridOnly && (
        <div>
          <ApiMessageAlert commonKey="UnforfeitSave" />
        </div>
      )}
      <Grid
        container
        rowSpacing="24px">
        {isLeftPaneOpen && !isGridOnly && isFrozen && <FrozenYearWarning profitYear={profitYear} />}
        {isLeftPaneOpen && !isGridOnly && (
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
            {isLeftPaneOpen && !isGridOnly && (
              <Grid width={"100%"}>
                <DSMAccordion title="Filter">
                  <UnForfeitSearchFilter
                    setInitialSearchLoaded={actions.setInitialSearchLoaded}
                    fiscalData={fiscalCalendarYear}
                    onSearch={actions.handleSearch}
                    hasUnsavedChanges={state.hasUnsavedChanges}
                    setHasUnsavedChanges={actions.handleUnsavedChanges}
                  />
                </DSMAccordion>
              </Grid>
            )}
            <Grid width="100%">
              <UnForfeitGrid
                initialSearchLoaded={state.initialSearchLoaded}
                setInitialSearchLoaded={actions.setInitialSearchLoaded}
                resetPageFlag={state.resetPageFlag}
                onUnsavedChanges={actions.handleUnsavedChanges}
                hasUnsavedChanges={state.hasUnsavedChanges}
                shouldArchive={state.shouldArchive}
                onArchiveHandled={actions.handleArchiveHandled}
                setHasUnsavedChanges={actions.handleUnsavedChanges}
                fiscalCalendarYear={fiscalCalendarYear}
                isGridExpanded={isGridOnly}
                onToggleExpand={handleToggleGridExpand}
                onShowUnsavedChangesDialog={() => setShowUnsavedChangesDialog(true)}
                onShowErrorDialog={(title, message) => setErrorDialog({ title, message })}
              />
            </Grid>
          </>
        )}
      </Grid>

      <ConfirmationDialog
        open={showUnsavedChangesDialog}
        title="Unsaved Changes"
        description="Please save your changes before changing pages."
        onClose={() => setShowUnsavedChangesDialog(false)}
      />

      <ConfirmationDialog
        open={!!errorDialog}
        title={errorDialog?.title || "Error"}
        description={errorDialog?.message || "An error occurred"}
        onClose={() => setErrorDialog(null)}
      />
    </Page>
  );
};

export default UnForfeit;
