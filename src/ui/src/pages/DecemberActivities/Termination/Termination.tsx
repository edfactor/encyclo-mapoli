import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useEffect } from "react";
import { ApiMessageAlert, DSMAccordion, Page } from "smart-ui-library";

import { CircularProgress, Divider, Grid } from "@mui/material";

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

  // Use the navigation guard hook
  useUnsavedChangesGuard(state.hasUnsavedChanges);

  const renderActionNode = () => {
    scrollToTop();
    return <StatusDropdownActionNode onStatusChange={actions.handleStatusChange} />;
  };

  const scrollToTop = () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  useEffect(() => {
    fetchAccountingRange();
  }, [fetchAccountingRange]);

  const isCalendarDataLoaded = !!fiscalData?.fiscalBeginDate && !!fiscalData?.fiscalEndDate;

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
              <Grid width="100%">
                <DSMAccordion title="Filter">
                  <TerminationSearchFilter
                    setInitialSearchLoaded={actions.setInitialSearchLoaded}
                    fiscalData={fiscalData}
                    onSearch={actions.handleSearch}
                    hasUnsavedChanges={state.hasUnsavedChanges}
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
