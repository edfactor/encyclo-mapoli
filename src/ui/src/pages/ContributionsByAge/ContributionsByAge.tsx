import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import ContributionsByAgeSearchFilter from "./ContributionsByAgeSearchFilter";
import ContributionsByAgeGrid from "./ContributionsByAgeGrid";

const ContributionsByAge = () => {
  return (
    <Page label="Contributions By Age">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
             <DSMAccordion title="Filter">
              <ContributionsByAgeSearchFilter />
             </DSMAccordion>
          </Grid2>

          <Grid2 width="100%">
            <ContributionsByAgeGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default ContributionsByAge;
