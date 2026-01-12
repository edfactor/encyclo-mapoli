import PageErrorBoundary from "@/components/PageErrorBoundary";
import { Divider, Grid } from "@mui/material";
import { DSMAccordion, Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../constants";
import { useGridExpansion } from "../../../hooks/useGridExpansion";
import ForfeitGrid from "./ForfeitGrid";
import ForfeitSearchFilter from "./ForfeitSearchFilter";
import useForfeit from "./hooks/useForfeit";

const Forfeit = () => {
  const { searchResults, isSearching, showData, pagination, executeSearch, handleStatusChange, handleReset } =
    useForfeit();
  const { isGridExpanded, handleToggleGridExpand } = useGridExpansion();

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={handleStatusChange} />;
  };

  return (
    <PageErrorBoundary pageName="Forfeit">
      <Page
        label={isGridExpanded ? "" : CAPTIONS.FORFEIT}
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
                <ForfeitSearchFilter
                  onSearch={executeSearch}
                  onReset={handleReset}
                  isSearching={isSearching}
                />
              </DSMAccordion>
            </Grid>
          )}

          {showData && (
            <Grid width="100%">
              <ForfeitGrid
                searchResults={searchResults}
                pagination={pagination}
                isSearching={isSearching}
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

export default Forfeit;
