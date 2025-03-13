import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { DSMAccordion, Page } from "smart-ui-library";
import BalanceByAgeSearchFilter from "./BalanceByAgeSearchFilter";
import BalanceByAgeGrid from "./BalanceByAgeGrid";

const BalanceByAge = () => {
  return (
    <Page label="Balance By Age">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
             <DSMAccordion title="Filter">
              <BalanceByAgeSearchFilter />
             </DSMAccordion>
          </Grid2>

          <Grid2 width="100%">
            <BalanceByAgeGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default BalanceByAge;
