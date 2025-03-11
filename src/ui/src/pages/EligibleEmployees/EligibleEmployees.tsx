import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import { useState } from "react";
import EligibleEmployeesSearchFilter from "./EligibleEmployeesSearchFilter";
import EligibleEmployeesGrid from "./EligibleEmployeesGrid";

const EligibleEmployees = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  return (
    <Page label="Get Eligible Employees">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <EligibleEmployeesSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <EligibleEmployeesGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default EligibleEmployees;
