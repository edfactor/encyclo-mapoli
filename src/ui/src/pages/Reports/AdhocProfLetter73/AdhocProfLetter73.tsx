import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import React, { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { DSMAccordion, Page } from "smart-ui-library";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import { CAPTIONS } from "../../../constants";
import { useCachedPrevious } from "../../../hooks/useCachedPrevious";
import { closeDrawer, openDrawer, setFullscreen } from "../../../reduxstore/slices/generalSlice";
import { RootState } from "../../../reduxstore/store";
import AdhocProfLetter73Grid from "./AdhocProfLetter73Grid.tsx";
import AdhocProfLetter73FilterSection, { AdhocProfLetter73FilterParams } from "./AdhocProfLetter73SearchFilter.tsx";

const AdhocProfLetter73: React.FC = () => {
  const [filterParams, setFilterParams] = useState<AdhocProfLetter73FilterParams | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isGridExpanded, setIsGridExpanded] = useState(false);
  const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = useState(false);
  const [searchTrigger, setSearchTrigger] = useState(0);

  const dispatch = useDispatch();
  const isDrawerOpen = useSelector((state: RootState) => state.general.isDrawerOpen);

  const handleFilterChange = (params: AdhocProfLetter73FilterParams) => {
    setFilterParams(params);
    setSearchTrigger((prev) => prev + 1); // Force a new search even if params are the same
  };

  const handleReset = () => {
    setFilterParams(null);
  };

  const handleLoadingChange = (loading: boolean) => {
    setIsLoading(loading);
  };

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

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  // Cache last non-null filter params so the grid can continue showing
  // previous results while a new search is in-flight.
  const displayFilterParams = useCachedPrevious(filterParams ?? null) ?? null;

  return (
    <PageErrorBoundary pageName="Adhoc Prof Letter 73">
      <Page
        label={isGridExpanded ? "" : CAPTIONS.ADHOC_PROF_LETTER73}
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
                <AdhocProfLetter73FilterSection
                  onSearch={handleFilterChange}
                  onReset={handleReset}
                  isLoading={isLoading}
                />
              </DSMAccordion>
            </Grid>
          )}

          {(filterParams || displayFilterParams) && (
            <Grid width="100%">
              <AdhocProfLetter73Grid
                // Pass trigger to request a fresh search without unmounting
                searchTrigger={searchTrigger}
                filterParams={filterParams ?? displayFilterParams}
                onLoadingChange={handleLoadingChange}
                isGridExpanded={isGridExpanded}
                onToggleExpand={handleToggleGridExpand}
              />
            </Grid>
          )}
        </Grid>
      </Page>
    </PageErrorBoundary>
  );
};

export default AdhocProfLetter73;
