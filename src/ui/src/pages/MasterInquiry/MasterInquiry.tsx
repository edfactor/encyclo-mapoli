import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import MasterInquirySearchFilter from "./MasterInquirySearchFilter";
import MasterInquiryGrid from "./MasterInquiryGrid";

const MasterInquiry = () => {
  return (
    <Page label="MASTER INQUIRY (008-10)">
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
              <DSMAccordion title="Filter">
                <MasterInquirySearchFilter />
              </DSMAccordion>
             
          </Grid2>

          <Grid2 width="100%">
            <MasterInquiryGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default MasterInquiry;
