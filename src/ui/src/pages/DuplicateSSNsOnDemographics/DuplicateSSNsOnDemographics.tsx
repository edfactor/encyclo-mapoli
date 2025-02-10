import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { useLazyGetDemographicBadgesNotInPayprofitQuery } from "reduxstore/api/YearsEndApi";
import { DSMAccordion, Page } from "smart-ui-library";
import DuplicateSSNsOnDemographicsSearchFilter from "./DuplicateSSNsOnDemographicsSearchFilter";
import DuplicateSSNsOnDemographicsGrid from "./DuplicateSSNsOnDemographicsGrid";

const DuplicateSSNsOnDemographics = () => {
  const [, ] = useState<boolean>(false);

  const [] = useLazyGetDemographicBadgesNotInPayprofitQuery();
  return (
    <Page label="Duplicate SSNs on Demographics">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
                <DSMAccordion title="Filter">
                    <DuplicateSSNsOnDemographicsSearchFilter />
                </DSMAccordion>
          </Grid2>

          <Grid2 width="100%">
            <DuplicateSSNsOnDemographicsGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default DuplicateSSNsOnDemographics;
