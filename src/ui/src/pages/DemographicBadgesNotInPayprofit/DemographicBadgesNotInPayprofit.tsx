import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import DemographicBadgesNotInPayprofitGrid from "./DemographicBadgesNotInPayprofitGrid";
import DemographicBadgesNotInPayprofitSearchFilter from "./DemographicBadgesNotInPayprofitSearchFilter";

const DemographicBadgesNotInPayprofit = () => {
  const [profitYear, setProfitYear] = useState<number | null>(null);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  return (
    <Page label="Demographic Badges Not In Payprofit">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <DemographicBadgesNotInPayprofitSearchFilter
              setProfitYear={setProfitYear}
              setInitialSearchLoaded={setInitialSearchLoaded}
            />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DemographicBadgesNotInPayprofitGrid
            profitYearCurrent={profitYear}
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default DemographicBadgesNotInPayprofit;
