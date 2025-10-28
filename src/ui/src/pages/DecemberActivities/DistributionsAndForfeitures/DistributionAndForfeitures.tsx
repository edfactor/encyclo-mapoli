import { Divider, Grid } from "@mui/material";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../constants";
import DistributionsAndForfeituresGrid from "./DistributionAndForfeituresGrid";
import DistributionsAndForfeituresSearchFilter from "./DistributionAndForfeituresSearchFilter";

const DistributionsAndForfeitures = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [isFetching, setIsFetching] = useState(false);

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <Page
      label={`${CAPTIONS.DISTRIBUTIONS_AND_FORFEITURES}`}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <DistributionsAndForfeituresSearchFilter
              setInitialSearchLoaded={setInitialSearchLoaded}
              isFetching={isFetching}
            />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          <DistributionsAndForfeituresGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
            onLoadingChange={setIsFetching}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default DistributionsAndForfeitures;
