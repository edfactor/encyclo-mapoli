import { Divider } from "@mui/material";
import Alert from '@mui/material/Alert';
import AlertTitle from '@mui/material/AlertTitle';
import Grid2 from '@mui/material/Grid2';
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { MissiveResponse } from "reduxstore/types";
import { DSMAccordion, Page } from "smart-ui-library";
import MasterInquiryEmployeeDetails from "./MasterInquiryEmployeeDetails";
import MasterInquiryGrid from "./MasterInquiryGrid";
import MasterInquirySearchFilter from "./MasterInquirySearchFilter";


const MasterInquiry = () => {
  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.inquiry);

  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [missiveAlerts, setMissiveAlerts] = useState<MissiveResponse[] | null>(null);

  // This is the front-end cache of back end missives messages
  const { missives } = useSelector((state: RootState) => state.lookups);

  const getAlertColorForSeverity = (severity: string) => {

    switch (severity) {
      case "Error":
        return "error";
      case "Warning":
      case "Information":
        return "warning";
      case "Success":
        return "success";
      default:
        return "info";  
    }
  };

  const removeLeadingAndTrailingChars = (str: string) => {
    return str.replace(/^\*+|\*+$/g, "").trim();
  };

  useEffect(() => {
    if (missives && masterInquiryEmployeeDetails && masterInquiryEmployeeDetails.missives) {
    
    // There could be more than one missive
    if (masterInquiryEmployeeDetails.missives.length > 0) {
      const alerts = masterInquiryEmployeeDetails.missives.map((id: number) => {
      const missiveResponse =  missives.find((missive: MissiveResponse) => missive.id === id);
      return missiveResponse;
      }).filter((alert) => alert !== null) as MissiveResponse[];

      // This will send the list to the screen
      setMissiveAlerts(alerts);
    } else {
      console.log("No missives found to display.");
    }
  }
  }, [masterInquiryEmployeeDetails, missives]);


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
            <Alert severity={getAlertColorForSeverity(alert.severity)}>
              <AlertTitle>{removeLeadingAndTrailingChars(alert.message)}</AlertTitle>
              {alert.description}
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
