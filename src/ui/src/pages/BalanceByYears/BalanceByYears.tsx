import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import BalanceByYearsSearchFilter from "./BalanceByYearsSearchFilter";
import BalanceByYearsGrid from "./BalanceByYearsGrid";

const BalanceByYears = () => {
  return (
    <Page label="Balance By Years">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
             <DSMAccordion title="Filter">
              <BalanceByYearsSearchFilter />
             </DSMAccordion>
          </Grid2>

          <Grid2 width="100%">
            <BalanceByYearsGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default BalanceByYears;
