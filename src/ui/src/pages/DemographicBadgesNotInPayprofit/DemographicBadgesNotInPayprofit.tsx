import { Divider } from "@mui/material";
import Button from "@mui/material/Button";
import Grid2 from "@mui/material/Unstable_Grid2";
import { useState } from "react";
import { useLazyGetDemographicBadgesNotInPayprofitQuery, useLazyGetDuplicateSSNsQuery } from "reduxstore/api/YearsEndApi";
import { DSMAccordion, Page, SectionTitle, SmartModal } from "smart-ui-library";
import DemographicBadgesNotInPayprofitSearchFilter from "./DemographicBadgesNotInPayprofitSearchFilter";
import DemographicBadgesNotInPayprofitGrid from "./DemographicBadgesNotInPayprofitGrid";

const DemographicBadgesNotInPayprofit = () => {
  const [openModal, setOpenModal] = useState<boolean>(false);

  const [trigger] = useLazyGetDemographicBadgesNotInPayprofitQuery();
  return (
    <Page label="Demographic Badges Not In Payprofit">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
              <DSMAccordion title="Filter">
              <DemographicBadgesNotInPayprofitSearchFilter />
              </DSMAccordion>
              
          </Grid2>

          <Grid2 width="100%">
            <DemographicBadgesNotInPayprofitGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default DemographicBadgesNotInPayprofit;
