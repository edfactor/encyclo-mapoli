import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { DSMAccordion, Page } from "smart-ui-library";
import MasterInquirySearchFilter from "./MasterInquirySearchFilter";
import MasterInquiryGrid from "./MasterInquiryGrid";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import MasterInquiryEmployeeDetails from "./MasterInquiryEmployeeDetails";
import { useState } from "react";

const MasterInquiry = () => {
  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.yearsEnd);

  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);

  return (
    <Page label="MASTER INQUIRY (008-10)">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <MasterInquirySearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        {masterInquiryEmployeeDetails && <MasterInquiryEmployeeDetails details={masterInquiryEmployeeDetails} />}

        <Grid2 width="100%">
          <MasterInquiryGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default MasterInquiry;
