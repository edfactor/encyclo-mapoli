import { FilterList } from "@mui/icons-material";
import { Button, Divider, Stack, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import DuplicateNamesAndBirthdaysGrid from "pages/DecemberActivities/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdaysGrid";
import DuplicateSSNsOnDemographicsGrid from "pages/DecemberActivities/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographicsGrid";
import EmployeesOnMilitaryLeave from "pages/DecemberActivities/EmployeesOnMilitaryLeave/EmployeesOnMilitaryLeave";
import MasterInquiryGrid from "pages/MasterInquiry/MasterInquiryGrid";
import MasterInquirySearchFilter from "pages/MasterInquiry/MasterInquirySearchFilter";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { Link } from "react-router-dom";
import {
  useLazyGetDemographicBadgesNotInPayprofitQuery,
  useLazyGetDuplicateNamesAndBirthdaysQuery,
  useLazyGetDuplicateSSNsQuery
} from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMAccordion, Page } from "smart-ui-library";
import NegativeETVA from "./NegativeETVA";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";

const DecemberProcess = () => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  //const [triggerETVASearch, { isFetching: isFetchingETVA }] = useLazyGetNegativeEVTASSNQuery();
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [triggerPayrollDupeSsnsOnDemographics, { isFetching: isFetchingPayRollDupeSsns }] =
    useLazyGetDuplicateSSNsQuery();
  const [triggerDemographicBadgesNotInPayprofit, { isFetching: isfetchingDemographicBadges }] =
    useLazyGetDemographicBadgesNotInPayprofitQuery();
  const [triggerDuplicateNamesAndBirthdays, { isFetching: isFetchingDuplicateNames }] =
    useLazyGetDuplicateNamesAndBirthdaysQuery();

  const profitYear = useDecemberFlowProfitYear();
  
  useEffect(() => {
    if (hasToken) {
      triggerPayrollDupeSsnsOnDemographics({ pagination: { take: 25, skip: 0, sortBy: "ssn",  isSortDescending: false  } });
      triggerDemographicBadgesNotInPayprofit({ pagination: { take: 25, skip: 0, sortBy: "badgeNumber",  isSortDescending: false  } });
      triggerDuplicateNamesAndBirthdays({ profitYear: profitYear, pagination: { take: 25, skip: 0, sortBy: "name",  isSortDescending: false } });
    }
  }, [
    hasToken,
    triggerDemographicBadgesNotInPayprofit,
    triggerDuplicateNamesAndBirthdays,
    triggerPayrollDupeSsnsOnDemographics
  ]);

  return (
    <Page
      label="2024 December"
      actionNode={
        <Button
          variant="outlined"
          startIcon={<FilterList />}>
          Filter List
        </Button>
      }>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2
          size={{ xs: 12 }}
          width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 size={{ xs: 10 }}>
          <Typography
            variant="h2"
            sx={{ color: "#0258A5", paddingLeft: "24px" }}>
            Clean Up Reports
          </Typography>
        </Grid2>
        <Grid2
          size={{ xs: 2 }}
          sx={{ justifyContent: "flex-end", paddingRight: "24px" }}>
          <Link to="/clean-up-summary">
            <Button variant="outlined">View Details</Button>
          </Link>
        </Grid2>

        <Grid2 width="100%">
          <NegativeETVA />
        </Grid2>
        <Grid2 width="100%">
          <DuplicateSSNsOnDemographicsGrid />
        </Grid2>
        <Grid2 width="100%">
          <DuplicateNamesAndBirthdaysGrid
            initialSearchLoaded={initialSearchLoaded}
            setInitialSearchLoaded={setInitialSearchLoaded}
          />
        </Grid2>
        <Grid2 width="100%">
          <Stack paddingX="24px">
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              MISSING COMMA IN FULL NAME
            </Typography>
            <Stack
              sx={{ alignContent: "center" }}
              direction="row">
              <Typography sx={{ margin: 0, padding: 0 }}>This has not been created yet. </Typography>
              <Button>Create Report</Button>
            </Stack>
          </Stack>
        </Grid2>
        <Grid2 width="100%">
          <EmployeesOnMilitaryLeave />
        </Grid2>
        <Grid2 width="100%">
          <Typography
            variant="h2"
            sx={{ color: "#0258A5", paddingBottom: "8px", paddingLeft: "24px" }}>
            Master Inquiry (008-10)
          </Typography>
          <DSMAccordion title="Search">
            <MasterInquirySearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>
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

export default DecemberProcess;
