import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useCallback, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import useFiscalCloseProfitYear from "../../../hooks/useFiscalCloseProfitYear";
import { closeDrawer, openDrawer, setFullscreen } from "../../../reduxstore/slices/generalSlice";
import { RootState } from "../../../reduxstore/store";
import useYTDWages from "./hooks/useYTDWages";
import YTDWagesGrid from "./YTDWagesGrid";
import YTDWagesSearchFilter from "./YTDWagesSearchFilter";

interface YTDWagesProps {
  useFrozenData?: boolean;
}

const YTDWages: React.FC<YTDWagesProps> = ({ useFrozenData = true }) => {
  const componentRef = useRef<HTMLDivElement>(null);
  const dispatch = useDispatch();
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const { searchResults, isSearching, pagination, showData, hasResults, executeSearch } = useYTDWages({
    defaultUseFrozenData: useFrozenData
  });
  const [isGridExpanded, setIsGridExpanded] = useState(false);
  const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = useState(false);

  // Get current drawer state from Redux
  const isDrawerOpen = useSelector((state: RootState) => state.general.isDrawerOpen);

  // Handle status change - refresh grid with archive=true when status changes to Complete
  const handleStatusChange = useCallback(
    (_newStatus: string, statusName?: string) => {
      if (statusName === "Complete" && fiscalCloseProfitYear) {
        // Refresh the grid with archive=true to archive the results
        executeSearch({ profitYear: fiscalCloseProfitYear, useFrozenData, archive: true }, "status-complete");
      }
    },
    [fiscalCloseProfitYear, useFrozenData, executeSearch]
  );

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={handleStatusChange} />;
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

  //const recordCount = searchResults?.response?.total || 0;

  return (
    <Page
      label={isGridExpanded ? "" : `${CAPTIONS.YTD_WAGES_EXTRACT}`}
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
          <Grid
            width={"100%"}
            hidden={true}>
            <DSMAccordion title="Filter">
              <YTDWagesSearchFilter
                onSearch={executeSearch}
                isSearching={isSearching}
                defaultUseFrozenData={useFrozenData}
              />
            </DSMAccordion>
          </Grid>
        )}

        <Grid width="100%">
          <YTDWagesGrid
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
      </Grid>
    </Page>
  );
};

export default YTDWages;
