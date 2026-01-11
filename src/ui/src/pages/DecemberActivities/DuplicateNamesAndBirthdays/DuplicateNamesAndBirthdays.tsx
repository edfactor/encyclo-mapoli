import RefreshIcon from "@mui/icons-material/Refresh";
import { Alert, Box, Button, Chip, CircularProgress, Divider, FormControlLabel, Grid, Switch } from "@mui/material";
import PageErrorBoundary from "components/PageErrorBoundary";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useEffect, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { useRefreshDuplicateNamesAndBirthdaysCacheMutation } from "../../../reduxstore/api/YearsEndApi";
import { closeDrawer, openDrawer, setFullscreen } from "../../../reduxstore/slices/generalSlice";
import { RootState } from "../../../reduxstore/store";
import DuplicateNamesAndBirthdaysGrid from "./DuplicateNamesAndBirthdaysGrid";
import useDuplicateNamesAndBirthdays from "./hooks/useDuplicateNamesAndBirthdays";

const DuplicateNamesAndBirthdays = () => {
  const componentRef = useRef<HTMLDivElement>(null);
  const dispatch = useDispatch();
  const [includeFictionalSsnPairs, setIncludeFictionalSsnPairs] = useState(false);
  const { searchResults, isSearching, pagination, showData, hasResults, executeSearch } =
    useDuplicateNamesAndBirthdays(includeFictionalSsnPairs);
  const profitYear = useDecemberFlowProfitYear();
  const [refreshCache, { isLoading: isRefreshing }] = useRefreshDuplicateNamesAndBirthdaysCacheMutation();
  const [refreshMessage, setRefreshMessage] = useState<{ type: "success" | "error"; text: string } | null>(null);
  const [isGridExpanded, setIsGridExpanded] = useState(false);
  const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = useState(false);

  // Get current drawer state from Redux 
  const isDrawerOpen = useSelector((state: RootState) => state.general.isDrawerOpen);

  const handleRefreshCache = async () => {
    try {
      setRefreshMessage(null);
      await refreshCache().unwrap();
      setRefreshMessage({
        type: "success",
        text: "Cache refresh requested successfully. The cache will be updated in the background."
      });
    } catch {
      setRefreshMessage({
        type: "error",
        text: "Failed to request cache refresh. Please try again later."
      });
    }
  };

  const handleToggleFictionalSsn = (_event: React.ChangeEvent<HTMLInputElement>, checked: boolean) => {
    setIncludeFictionalSsnPairs(checked);
    pagination.resetPagination();
  };

  // When the toggle changes, trigger a new search with the updated filter
  useEffect(() => {
    if (profitYear) {
      executeSearch(
        {
          profitYear,
          includeFictionalSsnPairs
        },
        "toggle"
      );
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [includeFictionalSsnPairs]);

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

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  const recordCount = searchResults?.response?.total || 0;
  const reportDate = searchResults?.reportDate ? new Date(searchResults.reportDate).toLocaleString() : "N/A";

  return (
    <PageErrorBoundary pageName="Duplicate Names and Birthdays">
      <Page
        label={isGridExpanded ? "" : `${CAPTIONS.DUPLICATE_NAMES} (${recordCount} records)`}
        actionNode={isGridExpanded ? undefined : renderActionNode()}>
        <Grid
          container
          rowSpacing="24px">
          {!isGridExpanded && (
            <Grid width={"100%"}>
              <Divider />
            </Grid>
          )}

          {/* Report As Of Date and Refresh Button */}
          {!isGridExpanded && (
            <Grid
              width={"100%"}
              container
              alignItems="center"
              justifyContent="space-between"
              paddingX={2}>
              <Box
                display="flex"
                alignItems="center"
                gap={1}>
                <Chip
                  label={`Report as of: ${reportDate}`}
                  className="bg-dsm-grey-hover"
                />
              </Box>
              <Box
                display="flex"
                alignItems="center"
                gap={2}>
                <FormControlLabel
                  control={
                    <Switch
                      checked={includeFictionalSsnPairs}
                      onChange={handleToggleFictionalSsn}
                    />
                  }
                  label="Include Fictional SSN Pairs"
                />
                <Button
                  variant="outlined"
                  color="primary"
                  startIcon={<RefreshIcon />}
                  onClick={handleRefreshCache}
                  disabled={isRefreshing}>
                  {isRefreshing ? "Refreshing..." : "Refresh Cache"}
                </Button>
              </Box>
            </Grid>
          )}

          {/* Refresh Message */}
          {!isGridExpanded && refreshMessage && (
            <Grid
              width={"100%"}
              paddingX={2}>
              <Alert
                severity={refreshMessage.type}
                onClose={() => setRefreshMessage(null)}>
                {refreshMessage.text}
              </Alert>
            </Grid>
          )}

          {isSearching && !searchResults ? (
            <Grid
              width={"100%"}
              container
              justifyContent="center"
              padding={4}>
              <CircularProgress />
            </Grid>
          ) : (
            <Grid width="100%">
              <DuplicateNamesAndBirthdaysGrid
                innerRef={componentRef}
                data={searchResults}
                isLoading={isSearching}
                showData={showData}
                hasResults={hasResults ?? false}
                pagination={pagination}
                onSortChange={pagination.handleSortChange}
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

export default DuplicateNamesAndBirthdays;
