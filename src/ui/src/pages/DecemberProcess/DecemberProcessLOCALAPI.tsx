import React, { useEffect } from "react";
import { Filter, FilterList, CheckCircle, RadioButtonUnchecked, Warning } from "@mui/icons-material";
import { Box, Button, Divider, Stack, Typography, Chip } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import { useSelector } from "react-redux";
import DuplicateSSNsOnDemographicsGrid from "pages/DuplicateSSNsOnDemographics/DuplicateSSNsOnDemographicsGrid";
import MasterInquiryGrid from "pages/MasterInquiry/MasterInquiryGrid";
import MasterInquirySearchFilter from "pages/MasterInquiry/MasterInquirySearchFilter";
import NegativeETVA from "./NegativeETVA";
import {
  useLazyGetNegativeEVTASSNQuery,
  useLazyGetDuplicateSSNsQuery,
  useLazyGetDemographicBadgesNotInPayprofitQuery,
  useLazyGetDuplicateNamesAndBirthdaysQuery
} from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import NegativeEtvaForSSNsOnPayprofitGrid from "pages/NegativeEtvaForSSNsOnPayprofit/NegativeEtvaForSSNsOnPayprofitGrid";

/*

None of the below is set in stone, but is a proposed data structure to formally track related processes. 
The goal is this be general and repeatable as a pattern across projects. All feedback welcome!

Process Persistence Brainstorm
-----------------------------
The process state pattern explores the following goals:
1. Track user-driven process completion (vs the old batch processing model)
2. Enable process validation and user decision tracking
3. Create a reusable pattern for other workflow-based applications

Core Concepts:
-------------
- Flows: High-level groupings of related jobs (e.g. "cleanup-reports")
- Jobs: Individual tasks that need user attention/review
- Status Tracking: Both at flow and job level for granular progress
- Validation States: Separate from job status to track data quality (used to highlight issues that need attention in the UI)
- Audit Trail: Who completed what and when

Future Iterations Could Include:
------------------------------
- Role-based access control for specific jobs
- User decision tracking (approvals, hold)
- Formal dependencies between jobs (informs user of downstream/side effects)
- Historical runs and validation results

DB Needs
------------------------
ProcessState
  - Tracks the state of each process/job by year
  - Core fields:
    • Year            (which year's process)
    • Flow ID         (which group of jobs)
    • Job ID          (specific job/task)
    • Status          (pending/in-progress/completed)
    • Validation      (valid/invalid/pending)
    • Audit fields    (who completed, when)
    • Timestamps      (created/updated)

  - Key relationships:
    • One year has many flows
    • One flow has many jobs
    • Each job has one current state

  - We can index by year and flow for quick lookups

Required API Endpoints:
---------------------
GET /api/process-state/{year}
- Returns the full process state for a given year

PATCH /api/process-state/{year}/{flowId}/{jobId}
Request body: {
    status: string,
    validationStatus?: string,
    completedBy?: string
}
- Updates the status of a specific job

GET /api/process-state/{year}/summary
- Returns a summary of completion status for all flows/jobs for the given year

POST /api/process-state/{year}/initialize
- This will init the process state for a new year with default values - starting fresh

*/


const decemberProcessState: ProcessState = {
  year: 2024,
  flows: {
    "cleanup-reports": {
      id: "cleanup-reports",
      name: "Clean Up Reports",
      description: "Validation reports for year-end cleanup",
      order: 1,
      status: "COMPLETED",
      jobs: {
        "negative-etva": {
          jobId: "negative-etva",
          order: 1,
          status: "COMPLETED",
          validationStatus: "VALID",
          latestRun: {
            completedBy: "SMITH.J",
            completedAt: new Date("2024-12-18T10:00:00")
          }
        },
        "duplicate-ssns": {
          jobId: "duplicate-ssns",
          order: 2,
          status: "COMPLETED",
          validationStatus: "VALID",
          latestRun: {
            completedBy: "JONES.M",
            completedAt: new Date("2024-12-18T11:30:00")
          }
        },
        "duplicate-names-birthdays": {
          jobId: "duplicate-names-birthdays",
          order: 3,
          status: "COMPLETED",
          validationStatus: "VALID",
          latestRun: {
            completedBy: "WILSON.K",
            completedAt: new Date("2024-12-18T14:15:00")
          }
        },
        "missing-comma": {
          jobId: "missing-comma",
          order: 4,
          status: "COMPLETED",
          validationStatus: "VALID",
          latestRun: {
            completedBy: "SMITH.J",
            completedAt: new Date("2024-12-18T15:45:00")
          }
        }
      }
    },
    "military-rehire": {
      id: "military-rehire",
      name: "Military and Rehire",
      description: "Process military and rehire records",
      order: 2,
      status: "COMPLETED",
      jobs: {
        "military-rehire-process": {
          jobId: "military-rehire-process",
          order: 1,
          status: "COMPLETED",
          validationStatus: "VALID",
          latestRun: {
            completedBy: "WILSON.K",
            completedAt: new Date("2024-12-19T09:30:00")
          }
        }
      }
    },
    "master-inquiry": {
      id: "master-inquiry",
      name: "Master Inquiry (008-10)",
      description: "Master record inquiry and validation",
      order: 3,
      status: "IN_PROGRESS",
      jobs: {
        "master-inquiry-report": {
          jobId: "master-inquiry-report",
          order: 1,
          status: "IN_PROGRESS",
          validationStatus: "PENDING",
          latestRun: {
            completedBy: "JONES.M",
            completedAt: new Date("2024-12-19T10:15:00")
          }
        }
      }
    }
  }
};

const StatusIndicator: React.FC<{ status: string; validationStatus?: string }> = ({ status, validationStatus }) => {
  if (status === "COMPLETED" && validationStatus === "VALID") {
    return <CheckCircle color="success" />;
  }
  if (status === "IN_PROGRESS") {
    return <Warning color="warning" />;
  }
  return <RadioButtonUnchecked color="disabled" />;
};

const JobSection: React.FC<{
  title: string;
  status: string;
  validationStatus?: string;
  order: number;
  children: React.ReactNode;
}> = ({ title, status, validationStatus, order, children }) => (
  <Grid2 width="100%">
    <Stack
      direction="row"
      spacing={2}
      alignItems="center"
      sx={{ padding: "24px" }}>
      <Typography
        variant="h2"
        sx={{ color: "#0258A5" }}>
        {title}
      </Typography>
      <Chip
        label={`Step ${order}`}
        size="small"
      />
      <StatusIndicator
        status={status}
        validationStatus={validationStatus}
      />
      {status === "COMPLETED" && (
        <Typography
          variant="caption"
          color="textSecondary">
          Completed by {decemberProcessState.flows["cleanup-reports"].jobs["negative-etva"].latestRun?.completedBy}
        </Typography>
      )}
    </Stack>
    {children}
  </Grid2>
);

const DecemberProcessLocalApi: React.FC = () => {
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const [triggerETVASearch, { isFetching: isFetchingETVA }] = useLazyGetNegativeEVTASSNQuery();
  const [triggerPayrollDupeSsnsOnDemographics, { isFetching: isFetchingPayRollDupeSsns }] =
    useLazyGetDuplicateSSNsQuery();
  const [triggerDemographicBadgesNotInPayprofit, { isFetching: isfetchingDemographicBadges }] =
    useLazyGetDemographicBadgesNotInPayprofitQuery();
  const [triggerDuplicateNamesAndBirthdays, { isFetching: isFetchingDuplicateNames }] =
    useLazyGetDuplicateNamesAndBirthdaysQuery();

  const { _ } = useSelector((state: RootState) => state.yearsEnd);

  useEffect(() => {
    if (hasToken) {
      triggerETVASearch({ profitYear: 2024, pagination: { take: 25, skip: 0 } });
      triggerPayrollDupeSsnsOnDemographics({ pagination: { take: 25, skip: 0 } });
      triggerDemographicBadgesNotInPayprofit({ pagination: { take: 25, skip: 0 } });
      triggerDuplicateNamesAndBirthdays({ profitYear: 2024, pagination: { take: 25, skip: 0 } });
    }
  }, [hasToken]);
  return (
    <Page
      label="2024 December">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2
          xs={12}
          width="100%">
          <Divider />
        </Grid2>

        <Grid2 xs={10}>
          <Stack
            direction="row"
            spacing={2}
            alignItems="center"
            sx={{ paddingLeft: "24px" }}>
            <Typography
              variant="h2"
              sx={{ color: "#0258A5" }}>
              Clean Up Reports
            </Typography>
            <Chip
              label={`Flow ${decemberProcessState.flows["cleanup-reports"].order}`}
              color="primary"
              variant="outlined"
            />
            <StatusIndicator status={decemberProcessState.flows["cleanup-reports"].status} />
          </Stack>
        </Grid2>

        <JobSection
          title="NEGATIVE ETVA FOR SSNs ON PAYPROFIT"
          status={decemberProcessState.flows["cleanup-reports"].jobs["negative-etva"].status}
          validationStatus={decemberProcessState.flows["cleanup-reports"].jobs["negative-etva"].validationStatus}
          order={decemberProcessState.flows["cleanup-reports"].jobs["negative-etva"].order}>
          <NegativeEtvaForSSNsOnPayprofitGrid />
        </JobSection>

        <JobSection
          title="DUPLICATE SSNs ON DEMOGRAPHICS"
          status={decemberProcessState.flows["cleanup-reports"].jobs["duplicate-ssns"].status}
          validationStatus={decemberProcessState.flows["cleanup-reports"].jobs["duplicate-ssns"].validationStatus}
          order={decemberProcessState.flows["cleanup-reports"].jobs["duplicate-ssns"].order}>
          <DuplicateSSNsOnDemographicsGrid />
        </JobSection>

        <JobSection
          title="MASTER INQUIRY (008-10)"
          status={decemberProcessState.flows["master-inquiry"].jobs["master-inquiry-report"].status}
          validationStatus={decemberProcessState.flows["master-inquiry"].jobs["master-inquiry-report"].validationStatus}
          order={decemberProcessState.flows["master-inquiry"].jobs["master-inquiry-report"].order}>
          <DSMAccordion title="Search">
            <MasterInquirySearchFilter />
          </DSMAccordion>
          <MasterInquiryGrid />
        </JobSection>
      </Grid2>
    </Page>
  );
};

export default DecemberProcessLocalApi;
