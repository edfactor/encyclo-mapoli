import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import BalanceByYearsSearchFilter from "./BalanceByYearsSearchFilter";
import BalanceByYearsGrid from "./BalanceByYearsGrid";
import { useState } from "react";

const BalanceByYears = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  return (
    <Page label="Balance By Years">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <BalanceByYearsSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <BalanceByYearsGrid initialSearchLoaded={initialSearchLoaded} />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default BalanceByYears;
