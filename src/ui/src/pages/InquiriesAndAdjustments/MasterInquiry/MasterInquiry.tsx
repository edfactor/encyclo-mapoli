import { CircularProgress, Divider, Grid, Typography } from "@mui/material";
import { memo } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS } from "../../../constants";
import useMasterInquiry from "./hooks/useMasterInquiry";
import MasterInquiryGrid from "./MasterInquiryDetailsGrid";
import MasterInquiryMemberDetails from "./MasterInquiryMemberDetails";
import MasterInquiryMemberGrid from "./MasterInquiryMemberGrid";
import MasterInquirySearchFilter from "./MasterInquirySearchFilter";

const MasterInquiryContent = memo(() => {
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
      <Grid
        size={{ xs: 12 }}
        width={"100%"}></Grid>
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

      {/* No Results Message */}
      {searchResults && searchResults.total === 0 && !isSearching && !isFetchingMembers && (
        <Grid
          size={{ xs: 12 }}
          sx={{ padding: "24px" }}>
          <Typography variant="body1">No results found.</Typography>
        </Grid>
      )}

      {/* Member Details Section - Always render when we have selection, just control visibility */}
      {selectedMember && (
        <Grid
          size={{ xs: 12 }}
          sx={{
            display: showMemberDetails ? "block" : "none",
            transition: "opacity 0.2s ease-in-out"
          }}>
          {!isFetchingMemberDetails && memberDetails ? (
            <MasterInquiryMemberDetails
              memberType={selectedMember.memberType}
              id={selectedMember.id}
              profitYear={searchParams?.endProfitYear}
              memberDetails={memberDetails}
              isLoading={isFetchingMemberDetails}
            />
          ) : (
            <Grid
              size={{ xs: 12 }}
              sx={{ display: "flex", justifyContent: "center", padding: "24px" }}>
              <CircularProgress />
            </Grid>
          )}
        </Grid>
      )}

      {/* Profit Details Section - Always render when we have selection, just control visibility */}
      {selectedMember && (
        <Grid
          size={{ xs: 12 }}
          sx={{
            display: showProfitDetails ? "block" : "none",
            transition: "opacity 0.2s ease-in-out"
          }}>
          {!isFetchingProfitData && memberProfitData ? (
            <MasterInquiryGrid
              profitData={memberProfitData}
              isLoading={isFetchingProfitData}
              profitGridPagination={profitGridPagination}
              onPaginationChange={profitGridPagination.handlePaginationChange}
              onSortChange={profitGridPagination.handleSortChange}
            />
          ) : (
            <Grid
              size={{ xs: 12 }}
              sx={{ display: "flex", justifyContent: "center", padding: "24px" }}>
              <CircularProgress />
            </Grid>
          )}
        </Grid>
      )}
    </Grid>
  );
});

const MasterInquiry = () => {
  return (
    <Page label={CAPTIONS.MASTER_INQUIRY}>
      <MissiveAlertProvider>
        <MasterInquiryContent />
      </MissiveAlertProvider>
    </Page>
  );
};

export default MasterInquiry;
