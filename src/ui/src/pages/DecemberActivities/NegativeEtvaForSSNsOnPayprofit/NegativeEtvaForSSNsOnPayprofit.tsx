import { Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useRef } from "react";
import { Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import NegativeEtvaForSSNsOnPayprofitGrid from "./NegativeEtvaForSSNsOnPayprofitGrid";
import useNegativeEtvaForSSNsOnPayprofit from "./hooks/useNegativeEtvaForSSNsOnPayprofit";

const NegativeEtvaForSSNsOnPayprofit = () => {
  const componentRef = useRef<HTMLDivElement>(null);
  const { searchResults, isSearching, pagination, showData, hasResults } = useNegativeEtvaForSSNsOnPayprofit();

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  const recordCount = searchResults?.response?.total || 0;

  return (
    <Page
      label={`${CAPTIONS.NEGATIVE_ETVA} (${recordCount} records)`}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>

        <Grid width="100%">
          <NegativeEtvaForSSNsOnPayprofitGrid
            innerRef={componentRef}
            data={searchResults}
            isLoading={isSearching}
            showData={showData}
            hasResults={hasResults ?? false}
            pagination={pagination}
            onSortChange={pagination.handleSortChange}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default NegativeEtvaForSSNsOnPayprofit;
