import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { DSMAccordion, Page } from "smart-ui-library";
import ForfeituresByAgeSearchFilter from "./ForfeituresByAgeSearchFilter";
import ForfeituresByAgeGrid from "./ForfeituresByAgeGrid";

const ForfeituresByAge = () => {
  return (
    <Page label="Forfeitures By Age">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
             <DSMAccordion title="Filter">
              <ForfeituresByAgeSearchFilter />
             </DSMAccordion>
          </Grid2>

          <Grid2 width="100%">
            <ForfeituresByAgeGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default ForfeituresByAge;
