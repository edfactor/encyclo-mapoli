import { CircularProgress, Divider, Grid } from "@mui/material";
import { useEffect, useState } from "react";
import { ApiMessageAlert, DSMAccordion, Page } from "smart-ui-library";
import { ConfirmationDialog } from "../../../components/ConfirmationDialog";
import FrozenYearWarning from "../../../components/FrozenYearWarning";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useLazyGetAccountingRangeToCurrent } from "../../../hooks/useFiscalCalendarYear";
import { useGridExpansion } from "../../../hooks/useGridExpansion";
import { useIsProfitYearFrozen } from "../../../hooks/useIsProfitYearFrozen";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import UnForfeitGrid from "./UnForfeitGrid";
import UnForfeitSearchFilter from "./UnForfeitSearchFilter";
import { useUnForfeitState } from "./useUnForfeitState";

const UnForfeit = () => {
  const { state, actions } = useUnForfeitState();
  const [fetchAccountingRange, { data: fiscalCalendarYear }] = useLazyGetAccountingRangeToCurrent(6);
  const profitYear = useDecemberFlowProfitYear();
  const isFrozen = useIsProfitYearFrozen(profitYear);
  const { isGridExpanded, handleToggleGridExpand } = useGridExpansion();

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

  return (
    <Page
      label={isGridExpanded ? "" : `${CAPTIONS.REHIRE_FORFEITURES}`}
      actionNode={isGridExpanded ? undefined : renderActionNode()}>
      {!isGridExpanded && (
        <div>
          <ApiMessageAlert commonKey="UnforfeitSave" />
        </div>
      )}
      <Grid
        container
        rowSpacing="24px">
        {!isGridExpanded && isFrozen && <FrozenYearWarning profitYear={profitYear} />}
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
                isGridExpanded={isGridExpanded}
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
