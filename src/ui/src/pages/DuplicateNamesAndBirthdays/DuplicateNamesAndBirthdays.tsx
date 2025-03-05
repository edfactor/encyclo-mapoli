import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import DuplicateNamesAndBirthdaysSearchFilter from "./DuplicateNamesAndBirthdaysSearchFilter";
import DuplicateNamesAndBirthdaysGrid from "./DuplicateNamesAndBirthdaysGrid";
import { useState } from "react";

const DuplicateNamesAndBirthdays = () => {
  const [profitYear, setProfitYear] = useState<number | null>(null);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  return (
    <Page label="Duplicate Names and Birthdays">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <DuplicateNamesAndBirthdaysSearchFilter
              setProfitYear={setProfitYear}
              setInitialSearchLoaded={setInitialSearchLoaded}
            />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DuplicateNamesAndBirthdaysGrid
            profitYearCurrent={profitYear}
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default DuplicateNamesAndBirthdays;
