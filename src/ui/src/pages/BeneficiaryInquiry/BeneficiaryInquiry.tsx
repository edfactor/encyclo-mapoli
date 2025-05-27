import { Divider } from "@mui/material";
import Alert from '@mui/material/Alert';
import AlertTitle from '@mui/material/AlertTitle';
import Grid2 from '@mui/material/Grid2';
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { MissiveResponse } from "reduxstore/types";
import { DSMAccordion, Page } from "smart-ui-library";
import BeneficiaryInquirySearchFilter from "./BeneficiaryInquirySearchFilter";
import BeneficiaryInquiryGrid from "./BeneficiaryInquiryGrid";


const BeneficiaryInquiry = () => {
    const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
 
  return (
    <Page label="BENEFICIARY INQUIRY">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 size={{ xs: 12 }} width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 size={{ xs: 12 }} width={"100%"}>
          <DSMAccordion title="Filter">
            {/* <MasterInquirySearchFilter setInitialSearchLoaded={setInitialSearchLoaded} 
            setMissiveAlerts={setMissiveAlerts}
            /> */}
            <BeneficiaryInquirySearchFilter></BeneficiaryInquirySearchFilter>
          </DSMAccordion>
        </Grid2>

        <Grid2 size={{ xs: 12 }} width="100%">
          <BeneficiaryInquiryGrid initialSearchLoaded = {initialSearchLoaded} setInitialSearchLoaded={setInitialSearchLoaded}  />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default BeneficiaryInquiry;
