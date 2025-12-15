import { Divider, Grid } from "@mui/material";
import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { DSMAccordion, Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../constants";
import { closeDrawer, openDrawer, setFullscreen } from "../../../reduxstore/slices/generalSlice";
import { RootState } from "../../../reduxstore/store";
import DistributionsAndForfeituresGrid from "./DistributionsAndForfeituresGrid";
import DistributionsAndForfeituresSearchFilter from "./DistributionsAndForfeituresSearchFilter";
import { useDistributionsAndForfeituresState } from "./useDistributionsAndForfeituresState";

const DistributionsAndForfeitures = () => {
  const { state, actions } = useDistributionsAndForfeituresState();
  const dispatch = useDispatch();
  const [isFetching, setIsFetching] = useState(false);
  const [isGridExpanded, setIsGridExpanded] = useState(false);
  const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = useState(false);
  
  // Get current drawer state from Redux
  const isDrawerOpen = useSelector((state: RootState) => state.general.isDrawerOpen);

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={actions.handleStatusChange} />
  };

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

  return (
    <Page
      label={isGridExpanded ? "" : `${CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES}`}
      actionNode={isGridExpanded ? undefined : renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        {!isGridExpanded && (
          <Grid width={"100%"}>
            <Divider />
          </Grid>
        )}
        {!isGridExpanded && (
          <Grid width={"100%"}>
            <DSMAccordion title="Filter">
              <DistributionsAndForfeituresSearchFilter
                setInitialSearchLoaded={actions.setInitialSearchLoaded}
                isFetching={isFetching}
              />
            </DSMAccordion>
          </Grid>
        )}

        <Grid width="100%">
          <DistributionsAndForfeituresGrid
            setInitialSearchLoaded={actions.setInitialSearchLoaded}
            initialSearchLoaded={state.initialSearchLoaded}
            shouldArchive={state.shouldArchive}
            onArchiveHandled={actions.handleArchiveHandled}
            onLoadingChange={setIsFetching}
            isGridExpanded={isGridExpanded}
            onToggleExpand={handleToggleGridExpand}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default DistributionsAndForfeitures;
