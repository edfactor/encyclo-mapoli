import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import NegativeEtvaForSSNsOnPayprofitGrid from "pages/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofitGrid";
import NegativeEtvaForSSNsOnPayprofitSearchFilter from "pages/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofitSearchFilter";
import { DSMAccordion } from "smart-ui-library";

export const NegativeEtvaTab = () => {
  return (
    <Grid2
      container
      rowSpacing="24px">
      <Grid2 width={"100%"}>
        <Divider />
      </Grid2>
      <Grid2
        width={"100%"}>
        <DSMAccordion title="Filter">
          <NegativeEtvaForSSNsOnPayprofitSearchFilter />
        </DSMAccordion>

      </Grid2>

      <Grid2 width="100%">
        <NegativeEtvaForSSNsOnPayprofitGrid />
      </Grid2>
    </Grid2>
  );
};

export default NegativeEtvaTab;
