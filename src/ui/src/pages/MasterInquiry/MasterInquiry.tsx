import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { MasterInquiryRequest } from "reduxstore/types";
import { DSMAccordion, Page } from "smart-ui-library";
import MasterInquiryEmployeeDetails from "./MasterInquiryEmployeeDetails";
import MasterInquiryGrid from "./MasterInquiryDetailsGrid";
import MasterInquirySearchFilter from "./MasterInquirySearchFilter";
import MasterInquiryMemberGrid from "./MasterInquiryMemberGrid";


const MasterInquiry = () => {
  const { masterInquiryEmployeeDetails, masterInquiryRequestParams } = useSelector((state: RootState) => state.inquiry);

  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [searchParams, setSearchParams] = useState<MasterInquiryRequest | null>(null);
  const [selectedMember, setSelectedMember] = useState<{ memberType: number; id: number, ssn: number } | null>(null);

  return (
    <Page label="MASTER INQUIRY (008-10)">
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 size={{ xs: 12 }} width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 size={{ xs: 12 }} width={"100%"}>
          <DSMAccordion title="Filter">
            <MasterInquirySearchFilter 
              setInitialSearchLoaded={setInitialSearchLoaded}
              onSearch={setSearchParams}
            />
          </DSMAccordion>
        </Grid2>

        {searchParams && (
          <MasterInquiryMemberGrid {...searchParams} onBadgeClick={setSelectedMember} />
        )}

        {/* Render employee details if identifiers are present in selectedMember */}
        {selectedMember && selectedMember.memberType !== undefined && selectedMember.id && (
          <MasterInquiryEmployeeDetails
            memberType={selectedMember.memberType}
            id={selectedMember.id}
            profitYear={searchParams?.endProfitYear}
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
