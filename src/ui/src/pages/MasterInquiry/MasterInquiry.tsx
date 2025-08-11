import { Divider, Grid } from "@mui/material";
import MissiveAlerts from "components/MissiveAlerts/MissiveAlerts";
import { useState } from "react";
import { MasterInquiryRequest } from "reduxstore/types";
import { DSMAccordion, Page } from "smart-ui-library";
import MasterInquiryGrid from "./MasterInquiryDetailsGrid";
import MasterInquiryEmployeeDetails from "./MasterInquiryEmployeeDetails";
import MasterInquiryMemberGrid from "./MasterInquiryMemberGrid";
import MasterInquirySearchFilter from "./MasterInquirySearchFilter";
import { MissiveAlertProvider } from "./MissiveAlertContext";
import { useMissiveAlerts } from "./useMissiveAlerts";

interface SelectedMember {
  memberType: number;
  id: number;
  ssn: number;
  badgeNumber: number;
  psnSuffix: number;
}

const MasterInquiryContent = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [searchParams, setSearchParams] = useState<MasterInquiryRequest | null>(null);
  const [selectedMember, setSelectedMember] = useState<SelectedMember | null>(null);
  const [noResults, setNoResults] = useState(false);
  const { missiveAlerts } = useMissiveAlerts();

  return (
      <Grid
        container
        rowSpacing="24px">
        <Grid
          size={{ xs: 12 }}
          width={"100%"}>
          <Divider />
        </Grid>
        <Grid
          size={{ xs: 12 }}
          width={"100%"}>
          <DSMAccordion title="Filter">
            <MasterInquirySearchFilter
              setInitialSearchLoaded={setInitialSearchLoaded}
              onSearch={(params) => {
                setSearchParams(params ?? null);
                setSelectedMember(null);
                // Only set noResults to true if params is undefined (not found)
                // Reset noResults when params is null (form reset)
                if (params === undefined) {
                  setNoResults(true);
                } else {
                  setNoResults(false);
                }
              }}
            />
          </DSMAccordion>
        </Grid>
        {missiveAlerts.length > 0 && <MissiveAlerts missiveAlerts={missiveAlerts} />}

        {searchParams && (
          <MasterInquiryMemberGrid
            key={searchParams._timestamp || Date.now()}
            searchParams={searchParams}
            onBadgeClick={(data) => setSelectedMember(data || null)}
          />
        )}

        {/* Render employee details if identifiers are present in selectedMember, or show missive if noResults */}
        {(noResults || (selectedMember && selectedMember.memberType !== undefined && selectedMember.id)) && (
          <MasterInquiryEmployeeDetails
            memberType={selectedMember?.memberType ?? 0}
            id={selectedMember?.id ?? 0}
            profitYear={searchParams?.endProfitYear}
            noResults={noResults}
          />
        )}

        {/* Render details for selected member if present */}
        {!noResults && selectedMember && (
          <MasterInquiryGrid
            memberType={selectedMember.memberType}
            id={selectedMember.id}
          />
        )}
      </Grid>
  );
};

const MasterInquiry = () => {
  return (
    <Page label="MASTER INQUIRY (008-10)">
      <MissiveAlertProvider>
        <MasterInquiryContent />
      </MissiveAlertProvider>
    </Page>
  );
};

export default MasterInquiry;
