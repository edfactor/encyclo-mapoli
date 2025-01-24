import { FilterList } from "@mui/icons-material";
import { Button, Divider, Stack, Typography } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import DuplicateNamesAndBirthdaysGrid from "pages/DuplicateNamesAndBirthdays/DuplicateNamesAndBirthdaysGrid";
import DuplicateSSNsOnDemographicsGrid from "pages/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographicsGrid";
import MasterInquiryGrid from "pages/MasterInquiry/MasterInquiryGrid";
import MasterInquirySearchFilter from "pages/MasterInquiry/MasterInquirySearchFilter";
import MilitaryAndRehire from "pages/MilitaryAndRehire/MilitaryAndRehire";
import { useEffect } from "react";
import { useSelector } from "react-redux";
import {
  useLazyGetDemographicBadgesNotInPayprofitQuery,
  useLazyGetDuplicateNamesAndBirthdaysQuery,
  useLazyGetDuplicateSSNsQuery,
  useLazyGetNegativeEVTASSNQuery
} from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMAccordion, Page } from "smart-ui-library";
import NegativeETVA from "./NegativeETVA";
import { useNavigate } from "react-router";
import MasterInquiryEmployeeDetails from "pages/MasterInquiry/MasterInquiryEmployeeDetails";
import TerminationGrid from "pages/Termination/TerminationGrid";
import TerminationSearchFilter from "pages/Termination/TerminationSearchFilter";
import DistributionsAndForfeituresGrid from "pages/DistributionsAndForfeitures/DistributionAndForfeituresGrid";
import DistributionsAndForfeituresSearchFilter from "pages/DistributionsAndForfeitures/DistributionAndForfeituresSearchFilter";
import DSMCollapsedAccordion from "components/DSMCollapsedAccordion";

const DecemberProcessAccordion = () => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerETVASearch, { isFetching: isFetchingETVA }] = useLazyGetNegativeEVTASSNQuery();
  const [triggerPayrollDupeSsnsOnDemographics, { isFetching: isFetchingPayRollDupeSsns }] =
    useLazyGetDuplicateSSNsQuery();
  const [triggerDemographicBadgesNotInPayprofit, { isFetching: isfetchingDemographicBadges }] =
    useLazyGetDemographicBadgesNotInPayprofitQuery();
  const [triggerDuplicateNamesAndBirthdays, { isFetching: isFetchingDuplicateNames }] =
    useLazyGetDuplicateNamesAndBirthdaysQuery();

  const { negativeEtvaForSSNsOnPayprofit, duplicateSSNsData, demographicBadges, duplicateNamesAndBirthday } =
    useSelector((state: RootState) => state.yearsEnd);

  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.yearsEnd);

  useEffect(() => {
    if (hasToken) {
      triggerPayrollDupeSsnsOnDemographics({ profitYear: 2023, pagination: { take: 25, skip: 0 } });
      triggerDemographicBadgesNotInPayprofit({ pagination: { take: 25, skip: 0 } });
      triggerDuplicateNamesAndBirthdays({ profitYear: 2023, pagination: { take: 25, skip: 0 } });
    }
  }, [hasToken]);

  const navigate = useNavigate();

  return (
    <Page
      label="December Flow">
      <Grid2
        container>
        <Grid2
          xs={12}
          width={"100%"}>
          <Divider />
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Clean Up Reports"
            expandable={false}
            actionButtonText="VIEW DETAILS"
            status={{
              label: "Completed",
              color: "success"
            }}
            onActionClick={() =>
              navigate('/clean-up-summary')
            }
            isCollapsedOnRender={true}
          >
            <Grid2 width="100%">
              <NegativeETVA />
            </Grid2>
            <Grid2 width="100%">
              <DuplicateSSNsOnDemographicsGrid />
            </Grid2>
            <Grid2 width="100%">
              <DuplicateNamesAndBirthdaysGrid />
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
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Military"
            expandable={false}
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/military-and-rehire-entry')}
            actionButtonText="VIEW DETAILS"
          >
            <>
            </>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Rehire"
            expandable={false}
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/military-and-rehire')}
            actionButtonText="VIEW DETAILS"
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Prof Termination"
            expandable={false}
            status={{
              label: "Not Started",
              color: "default"
            }}
            actionButtonText="VIEW DETAILS"
            onActionClick={() => navigate('/prof-term')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Loan Balance"
            expandable={false}
            status={{
              label: "Not Started",
              color: "default"
            }}
            actionButtonText="VIEW DETAILS"
            onActionClick={() => navigate('/distributions-and-forfeitures')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Manage Executives"
            expandable={false}
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => alert("Manage Executives")}
            actionButtonText="VIEW DETAILS"
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Report"
            expandable={false}
            status={{
              label: "In Progress",
              color: "default"
            }}
            onActionClick={() => navigate('/profit-share-report')}
            actionButtonText="VIEW DETAILS"
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

      </Grid2>
    </Page>
  );
};

export default DecemberProcessAccordion;