import { CircularProgress, Divider, Grid, Typography } from "@mui/material";
import { memo, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { DSMAccordion, Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS } from "../../../constants";
import { closeDrawer, openDrawer, setFullscreen } from "../../../reduxstore/slices/generalSlice";
import { RootState } from "../../../reduxstore/store";
import useMasterInquiry from "./hooks/useMasterInquiry";
import MasterInquiryGrid from "./MasterInquiryDetailsGrid";
import MasterInquiryMemberDetails from "./MasterInquiryMemberDetails";
import MasterInquiryMemberGrid from "./MasterInquiryMemberGrid";
import MasterInquirySearchFilter from "./MasterInquirySearchFilter";

interface MasterInquiryContentProps {
  isGridExpanded: boolean;
  setIsGridExpanded: React.Dispatch<React.SetStateAction<boolean>>;
}

const MasterInquiryContent = memo(({ isGridExpanded, setIsGridExpanded }: MasterInquiryContentProps) => {
  const dispatch = useDispatch();
  const [wasDrawerOpenBeforeExpand, setWasDrawerOpenBeforeExpand] = useState(false);

  const isDrawerOpen = useSelector((state: RootState) => state.general.isDrawerOpen) ?? false;

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

  const handleToggleGridExpand = () => {
    if (!isGridExpanded) {
      // Expanding: remember current drawer state and close it
      setWasDrawerOpenBeforeExpand(isDrawerOpen);
      dispatch(closeDrawer());
      dispatch(setFullscreen(true));
      setIsGridExpanded(true);
    } else {
      // Collapsing: restore previous state
      dispatch(setFullscreen(false));
      if (wasDrawerOpenBeforeExpand) {
        dispatch(openDrawer());
      }
      setIsGridExpanded(false);
    }
  };

  return (
    <Grid container>
      {!isGridExpanded && (
        <>
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
        </>
      )}

      {showMemberGrid && searchResults && !isFetchingMembers && (
        <MasterInquiryMemberGrid
          searchResults={searchResults}
          onMemberSelect={selectMember}
          memberGridPagination={memberGridPagination}
          onSortChange={memberGridPagination.handleSortChange}
          isLoading={isFetchingMembers}
          isGridExpanded={isGridExpanded}
          onToggleExpand={handleToggleGridExpand}
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
      {selectedMember && !isGridExpanded && (
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
              onSortChange={profitGridPagination.handleSortChange}
              isGridExpanded={isGridExpanded}
              onToggleExpand={handleToggleGridExpand}
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

MasterInquiryContent.displayName = "MasterInquiryContent";

const MasterInquiry = () => {
  const [isGridExpanded, setIsGridExpanded] = useState(false);

  return (
    <Page label={isGridExpanded ? "" : CAPTIONS.MASTER_INQUIRY}>
      <MissiveAlertProvider>
        <MasterInquiryContent
          isGridExpanded={isGridExpanded}
          setIsGridExpanded={setIsGridExpanded}
        />
      </MissiveAlertProvider>
    </Page>
  );
};

export default MasterInquiry;
