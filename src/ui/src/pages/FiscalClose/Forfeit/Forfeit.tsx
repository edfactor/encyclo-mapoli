import { Divider, Grid } from "@mui/material";
import { DSMAccordion, Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../constants";
import ForfeitGrid from "./ForfeitGrid";
import ForfeitSearchFilter from "./ForfeitSearchFilter";
import useForfeit from "./hooks/useForfeit";

const Forfeit = () => {
  const {
    searchResults,
    isSearching,
    showData,
    pagination,
    executeSearch,
    handleStatusChange,
    handleReset
  } = useForfeit();

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={handleStatusChange} />;
  };

  return (
    <Page
      label={CAPTIONS.FORFEIT}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <ForfeitSearchFilter
              onSearch={executeSearch}
              onReset={handleReset}
              isSearching={isSearching}
            />
          </DSMAccordion>
        </Grid>

        {showData && (
          <Grid width="100%">
            <ForfeitGrid
              searchResults={searchResults}
              pagination={pagination}
              isSearching={isSearching}
            />
          </Grid>
        )}
      </Grid>
    </Page>
  );
};

export default Forfeit;
