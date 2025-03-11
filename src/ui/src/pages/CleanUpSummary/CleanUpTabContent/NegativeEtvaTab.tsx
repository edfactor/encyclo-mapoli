import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import NegativeEtvaForSSNsOnPayprofitGrid from "pages/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofitGrid";
import NegativeEtvaForSSNsOnPayprofitSearchFilter from "pages/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofitSearchFilter";
import { useState } from "react";
import { DSMAccordion } from "smart-ui-library";

export const NegativeEtvaTab = () => {
  const [profitYear, setProfitYear] = useState<number | null>(null);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  return (
    <Grid2
      container
      rowSpacing="24px">
      <Grid2 width={"100%"}>
        <Divider />
      </Grid2>
      <Grid2 width={"100%"}>
        <DSMAccordion title="Filter">
          <NegativeEtvaForSSNsOnPayprofitSearchFilter
            setProfitYear={setProfitYear}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </DSMAccordion>
      </Grid2>

      <Grid2 width="100%">
        <NegativeEtvaForSSNsOnPayprofitGrid
          profitYearCurrent={profitYear}
          setInitialSearchLoaded={setInitialSearchLoaded}
          initialSearchLoaded={initialSearchLoaded}
        />
      </Grid2>
    </Grid2>
  );
};

export default NegativeEtvaTab;
