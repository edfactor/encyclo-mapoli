import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../constants";
import ForfeituresAdjustmentGrid from "./ForfeituresAdjustmentGrid";
import ForfeituresAdjustmentSearchParameters from "./ForfeituresAdjustmentSearchParameters";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import MasterInquiryEmployeeDetails from "pages/MasterInquiry/MasterInquiryEmployeeDetails";
import { EmployeeDetails } from "reduxstore/types";

const ForfeituresAdjustment = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [showEmployeeDetails, setShowEmployeeDetails] = useState(false);

  // Dummy data for employee details - will be replaced with real stuff later
  const dummyEmployeeDetails: EmployeeDetails = {
    firstName: "First",
    lastName: "LastName",
    address: "123 Main Street",
    addressCity: "City Name",
    addressState: "MA",
    addressZipCode: "02134",
    dateOfBirth: "1990-01-01",
    ssn: "***-**-1234",
    yearToDateProfitSharingHours: 0,
    yearsInPlan: 0,
    percentageVested: 0,
    contributionsLastYear: false,
    enrollmentId: 1,
    enrollment: "Yes",
    badgeNumber: "12345",
    hireDate: "2018-01-01",
    terminationDate: null,
    reHireDate: null,
    storeNumber: 0,
    beginPSAmount: 0,
    currentPSAmount: 0,
    beginVestedAmount: 0,
    currentVestedAmount: 0,
    currentEtva: 0,
    previousEtva: 0
  };

  const renderActionNode = () => {
    return (
      <StatusDropdownActionNode />
    );
  };

  const handleSearchComplete = (loaded: boolean) => {
    setInitialSearchLoaded(loaded);
    setShowEmployeeDetails(loaded);
  };

  return (
    <Page label={CAPTIONS.FORFEITURES_ADJUSTMENT} actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <ForfeituresAdjustmentSearchParameters setInitialSearchLoaded={handleSearchComplete} />
          </DSMAccordion>
        </Grid2>

        {showEmployeeDetails && <MasterInquiryEmployeeDetails details={dummyEmployeeDetails} />}

        <Grid2 width="100%">
          <ForfeituresAdjustmentGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ForfeituresAdjustment;
