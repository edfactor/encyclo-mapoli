import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useState } from "react";
import { MasterInquiryRequest } from "reduxstore/types";
import { DSMAccordion, Page } from "smart-ui-library";
import MasterInquiryGrid from "./MasterInquiryDetailsGrid";
import MasterInquiryEmployeeDetails from "./MasterInquiryEmployeeDetails";
import MasterInquiryGroupingGrid from "./MasterInquiryGroupingGrid";
import MasterInquiryMemberGrid from "./MasterInquiryMemberGrid";
import MasterInquirySearchFilter from "./MasterInquirySearchFilter";

interface SelectedMember {
  memberType: number;
  id: number;
  ssn: number;
  badgeNumber: number;
  psnSuffix: number;
}

const MasterInquiry = () => {
  //const { } = useSelector((state: RootState) => state.inquiry);

  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [searchParams, setSearchParams] = useState<MasterInquiryRequest | null>(null);
  const [selectedMember, setSelectedMember] = useState<SelectedMember | null>(null);
  const [noResults, setNoResults] = useState(false);
  const [searchActive, setSearchActive] = useState(false);

  return (
    <Page label="MASTER INQUIRY (008-10)">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2
          size={{ xs: 12 }}
          width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2
          size={{ xs: 12 }}
          width={"100%"}>
          <DSMAccordion title="Filter">
            <MasterInquirySearchFilter
              setInitialSearchLoaded={setInitialSearchLoaded}
              onSearch={(params) => {
                setSearchParams(params ?? null);
                setSelectedMember(null);
                setNoResults(!params);
              }}
              setSearchActive={setSearchActive}
            />
          </DSMAccordion>
        </Grid2>

        {searchParams && (
          <MasterInquiryMemberGrid
            {...searchParams}
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
            searchActive={searchActive}
          />
        )}

        {/* Render details for selected member if present */}
        {selectedMember && (
          <MasterInquiryGrid
            memberType={selectedMember.memberType}
            id={selectedMember.id}
          />
        )}
      </Grid2>
    </Page>
  );
};

export default MasterInquiry;
