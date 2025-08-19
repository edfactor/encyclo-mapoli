import { Divider, Grid } from "@mui/material";
import MissiveAlerts from "components/MissiveAlerts/MissiveAlerts";
import { DSMAccordion, Page } from "smart-ui-library";
import MasterInquiryGrid from "./MasterInquiryDetailsGrid";
import MasterInquiryEmployeeDetails from "./MasterInquiryMemberDetails";
import MasterInquiryMemberGrid from "./MasterInquiryMemberGrid";
import MasterInquirySearchFilter from "./MasterInquirySearchFilter";
import { MissiveAlertProvider } from "./MissiveAlertContext";
import useMasterInquiry from "./useMasterInquiry";
import { useMissiveAlerts } from "./useMissiveAlerts";

const MasterInquiryContent = () => {
  const { missiveAlerts } = useMissiveAlerts();
  const {
    searchParams,
    searchResults,
    isSearching,
    selectedMember,
    memberDetails,
    memberProfitData,
    isFetchingMemberDetails,
    isFetchingProfitData,
    showMemberGrid,
    showMemberDetails,
    showProfitDetails,
    noResultsMessage,
    memberGridPagination,
    profitGridPagination,
    executeSearch,
    selectMember,
    resetAll
  } = useMasterInquiry();

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
            onSearch={executeSearch}
            onReset={resetAll}
            isSearching={isSearching}
          />
        </DSMAccordion>
      </Grid>
      {missiveAlerts.length > 0 && <MissiveAlerts missiveAlerts={missiveAlerts} />}

      {showMemberGrid && searchResults && (
        <MasterInquiryMemberGrid
          key={searchParams?._timestamp || Date.now()}
          searchResults={searchResults}
          onMemberSelect={selectMember}
          memberGridPagination={memberGridPagination}
          onPaginationChange={memberGridPagination.handlePaginationChange}
          onSortChange={memberGridPagination.handleSortChange}
        />
      )}

      {showMemberDetails && selectedMember && (
        <MasterInquiryEmployeeDetails
          memberType={selectedMember.memberType}
          id={selectedMember.id}
          profitYear={searchParams?.endProfitYear}
          memberDetails={memberDetails}
          isLoading={isFetchingMemberDetails}
        />
      )}

      {showProfitDetails && selectedMember && (
        <MasterInquiryGrid
          profitData={memberProfitData}
          isLoading={isFetchingProfitData}
          profitGridPagination={profitGridPagination}
          onPaginationChange={profitGridPagination.handlePaginationChange}
          onSortChange={profitGridPagination.handleSortChange}
        />
      )}

      {noResultsMessage && !showMemberGrid && !showMemberDetails && (
        <Grid
          size={{ xs: 12 }}
          sx={{ padding: "24px" }}>
          <div>{noResultsMessage}</div>
        </Grid>
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
