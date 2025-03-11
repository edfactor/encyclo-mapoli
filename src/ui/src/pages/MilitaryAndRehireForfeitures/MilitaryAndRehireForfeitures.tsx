import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import MilitaryAndRehireForfeituresSearchFilter from "./MilitaryAndRehireForfeituresSearchFilter";
import MilitaryAndRehireForfeituresGrid from "./MilitaryAndRehireForfeituresGrid";
import { useState } from "react";

const MilitaryAndRehireForfeitures = () => {
  const [profitYear, setProfitYear] = useState<number | null>(null);
  const [reportingYear, setReportingYear] = useState<string | null>(null);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  return (
    <Page label="Military and Rehire Forfeitures">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <MilitaryAndRehireForfeituresSearchFilter
              setProfitYear={setProfitYear}
              setReportingYear={setReportingYear}
              setInitialSearchLoaded={setInitialSearchLoaded}
            />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <MilitaryAndRehireForfeituresGrid
            profitYearCurrent={profitYear}
            reportingYearCurrent={reportingYear}
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default MilitaryAndRehireForfeitures;
