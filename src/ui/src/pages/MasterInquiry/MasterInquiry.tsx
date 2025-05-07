import { Divider } from "@mui/material";
import Grid2 from '@mui/material/Grid2';
import { DSMAccordion, Page } from "smart-ui-library";
import MasterInquirySearchFilter from "./MasterInquirySearchFilter";
import MasterInquiryGrid from "./MasterInquiryGrid";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import MasterInquiryEmployeeDetails from "./MasterInquiryEmployeeDetails";
import { useCallback, useEffect, useState } from "react";
import Alert from '@mui/material/Alert';
import AlertTitle from '@mui/material/AlertTitle';
import { MissiveResponse, MissiveAlert } from "reduxstore/types";


const MasterInquiry = () => {
  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.inquiry);

  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [missiveAlerts, setMissiveAlerts] = useState<MissiveAlert[] | null>(null);

  // This is the front-end cache of back end missives messages
  const { missives } = useSelector((state: RootState) => state.lookups);

  console.log("Missives", missives);

 
  const getMissiveReponseForId = useCallback((id: number): MissiveResponse | undefined => {
    let missive;
    if (missives && missives.length !== 0) {
      missive = missives.find((missive: MissiveResponse) => missive.id === id);
    }
    
    return missive;
  }, [missives]);


  useEffect(() => {
    if (masterInquiryEmployeeDetails && masterInquiryEmployeeDetails.missives) {

    if (masterInquiryEmployeeDetails.missives.length > 0) {
      const alerts = masterInquiryEmployeeDetails.missives.map((id: number) => {
      const missiveResponse = getMissiveReponseForId(id);
      if (missiveResponse && missiveResponse.severity) {
        return {
        severity: missiveResponse.severity,
        title: missiveResponse.severity.charAt(0).toUpperCase() + missiveResponse.severity.slice(1),
        message: missiveResponse.message
        };
      }
      return null;
      }).filter((alert) => alert !== null) as MissiveAlert[];

      setMissiveAlerts(alerts);
    } else {
      console.log("No missives found");
    }
  }
  }, [getMissiveReponseForId, masterInquiryEmployeeDetails, missiveAlerts]);


  return (
    <Page label="MASTER INQUIRY (008-10)">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 size={{ xs: 12 }} width={"100%"}>
          <Divider />
        </Grid2>
        {missiveAlerts && missiveAlerts.map((alert, index) => (
          <Grid2 key={index} size={{ xs: 12 }} width={"100%"}>
            <Alert severity={alert.severity}>
              <AlertTitle>{alert.title}</AlertTitle>
              {alert.message}
            </Alert>
          </Grid2>
        ))}
        <Grid2 size={{ xs: 12 }} width={"100%"}>
          <DSMAccordion title="Filter">
            <MasterInquirySearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        {masterInquiryEmployeeDetails && <MasterInquiryEmployeeDetails details={masterInquiryEmployeeDetails} missives={missives}/>}

        <Grid2 size={{ xs: 12 }} width="100%">
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
