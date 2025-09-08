import { CircularProgress, Divider, Grid } from "@mui/material";
import MissiveAlerts from "components/MissiveAlerts/MissiveAlerts";
import { memo } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../components/MissiveAlerts/MissiveAlertContext";
import { useMissiveAlerts } from "../../hooks/useMissiveAlerts";
import useMasterInquiry from "./hooks/useMasterInquiry";
import MasterInquiryGrid from "./MasterInquiryDetailsGrid";
import MasterInquiryMemberDetails from "./MasterInquiryMemberDetails";
import MasterInquiryMemberGrid from "./MasterInquiryMemberGrid";
import MasterInquirySearchFilter from "./MasterInquirySearchFilter";

const MasterInquiryContent = memo(() => {
  const { missiveAlerts } = useMissiveAlerts();
  const {
    searchParams,
    searchResults,
    isSearching,
    isFetchingMembers,
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
    <Grid container>
      <Grid
        size={{ xs: 12 }}
        width={"100%"}>
        <Divider />
      </Grid>
      {missiveAlerts.length > 0 && <MissiveAlerts missiveAlerts={missiveAlerts} />}
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

      {showMemberGrid && searchResults && !isFetchingMembers && (
        <MasterInquiryMemberGrid
          key={searchParams?._timestamp || Date.now()}
          searchResults={searchResults}
          onMemberSelect={selectMember}
          memberGridPagination={memberGridPagination}
          onPaginationChange={memberGridPagination.handlePaginationChange}
          onSortChange={memberGridPagination.handleSortChange}
          isLoading={isFetchingMembers}
        />
      )}

      {showMemberGrid && isFetchingMembers && (
        <Grid
          size={{ xs: 12 }}
          sx={{ display: "flex", justifyContent: "center", padding: "24px" }}>
          <CircularProgress />
        </Grid>
      )}

      {showMemberDetails && selectedMember && !isFetchingMemberDetails && (
        <MasterInquiryMemberDetails
          memberType={selectedMember.memberType}
          id={selectedMember.id}
          profitYear={searchParams?.endProfitYear}
          memberDetails={memberDetails}
          isLoading={isFetchingMemberDetails}
        />
      )}

      {showMemberDetails && selectedMember && isFetchingMemberDetails && (
        <Grid
          size={{ xs: 12 }}
          sx={{ display: "flex", justifyContent: "center", padding: "24px" }}>
          <CircularProgress />
        </Grid>
      )}

      {showProfitDetails && selectedMember && !isFetchingProfitData && (
        <MasterInquiryGrid
          profitData={memberProfitData}
          isLoading={isFetchingProfitData}
          profitGridPagination={profitGridPagination}
          onPaginationChange={profitGridPagination.handlePaginationChange}
          onSortChange={profitGridPagination.handleSortChange}
        />
      )}

      {showProfitDetails && selectedMember && isFetchingProfitData && (
        <Grid
          size={{ xs: 12 }}
          sx={{ display: "flex", justifyContent: "center", padding: "24px" }}>
          <CircularProgress />
        </Grid>
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
});

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
