import { Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { useState } from "react";
import { MasterInquiryRequest } from "reduxstore/types";
import { DSMAccordion, Page } from "smart-ui-library";
import MasterInquiryGrid from "./MasterInquiryDetailsGrid";
import MasterInquiryEmployeeDetails from "./MasterInquiryEmployeeDetails";
import MasterInquiryMemberGrid from "./MasterInquiryMemberGrid";
import MasterInquirySearchFilter from "./MasterInquirySearchFilter";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";

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

  const { masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);

  const isSimpleSearch = (): boolean => {
    const simpleFound: boolean =
      !!masterInquiryRequestParams &&
      (!!masterInquiryRequestParams.name ||
        !!masterInquiryRequestParams.socialSecurity ||
        !!masterInquiryRequestParams.badgeNumber) &&
      !(
        !!masterInquiryRequestParams.startProfitMonth ||
        !!masterInquiryRequestParams.endProfitMonth ||
        !!masterInquiryRequestParams.contribution ||
        !!masterInquiryRequestParams.earnings ||
        !!masterInquiryRequestParams.forfeiture ||
        !!masterInquiryRequestParams.payment
      );
    return simpleFound;
  };

  return (
    <Page label="MASTER INQUIRY (008-10)">
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
                // Don't reset noResults when params is undefined (clearing state)
                if (params === undefined) {
                  setNoResults(true);
                } else if (params !== null) {
                  setNoResults(false);
                }
                // Don't change noResults when params is null (form reset)
              }}
            />
          </DSMAccordion>
        </Grid>

        {searchParams && (
          <MasterInquiryMemberGrid
            searchParams={searchParams}
            onBadgeClick={(data) => setSelectedMember(data || null)}
            isSimpleSearch={isSimpleSearch}
          />
        )}

        {/* Render employee details if identifiers are present in selectedMember, or show missive if noResults */}
        {(noResults || (selectedMember && selectedMember.memberType !== undefined && selectedMember.id)) && (
          <MasterInquiryEmployeeDetails
            memberType={selectedMember?.memberType ?? 0}
            id={selectedMember?.id ?? 0}
            profitYear={searchParams?.endProfitYear}
            noResults={noResults}
            isSimpleSearch={isSimpleSearch}
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
    </Page>
  );
};

export default MasterInquiry;
