import { Divider } from "@mui/material";
import Button from "@mui/material/Button";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { useLazyGetDemographicBadgesNotInPayprofitQuery, useLazyGetDuplicateSSNsQuery } from "reduxstore/api/YearsEndApi";
import { DSMAccordion, Page, SectionTitle, SmartModal } from "smart-ui-library";
import DuplicateSSNsOnDemographicsSearchFilter from "./DuplicateSSNsOnDemographicsSearchFilter";
import DuplicateSSNsOnDemographicsGrid from "./DuplicateSSNsOnDemographicsGrid";

const DuplicateSSNsOnDemographics = () => {
  const [openModal, setOpenModal] = useState<boolean>(false);

  const [trigger] = useLazyGetDemographicBadgesNotInPayprofitQuery();
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
