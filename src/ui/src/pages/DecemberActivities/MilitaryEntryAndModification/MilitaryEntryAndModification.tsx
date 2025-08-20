import { Dialog, DialogContent, DialogTitle, Divider, Grid } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { InquiryApi } from "../../../reduxstore/api/InquiryApi";
import { useLazyGetMilitaryContributionsQuery } from "../../../reduxstore/api/MilitaryApi";
import MilitaryContributionForm from "./MilitaryContributionForm";
import MilitaryContributionGrid from "./MilitaryContributionFormGrid";
import MilitaryEntryAndModificationSearchFilter from "./MilitaryEntryAndModificationSearchFilter";

const MilitaryEntryAndModification = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [showContributions, setShowContributions] = useState(false);
  const [currentStatus, setCurrentStatus] = useState<string | null>(null);
  const { masterInquiryMemberDetails } = useSelector((state: RootState) => state.inquiry);
  const [fetchContributions, { isFetching }] = useLazyGetMilitaryContributionsQuery();
  const profitYear = useDecemberFlowProfitYear();
  const dispatch = useDispatch();

  const handleStatusChange = (newStatus: string, statusName?: string) => {
    // Only trigger if status is changing TO "Complete" (not already "Complete")
    if (statusName === "Complete" && currentStatus !== "Complete") {
      setCurrentStatus("Complete");
      handleFetchContributions(true); // Call with archive=true
    } else {
      setCurrentStatus(statusName || newStatus);
    }
  };

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={handleStatusChange} />;
  };

  const handleOpenForm = () => {
    setIsDialogOpen(true);
  };

  const handleCloseForm = () => {
    setIsDialogOpen(false);
    handleFetchContributions();
    setInitialSearchLoaded(true); // This will trigger the grid to refresh
  };

  const handleFetchContributions = useCallback(
    (archive?: boolean) => {
      const request = {
        badgeNumber: Number(masterInquiryMemberDetails?.badgeNumber ?? 0),
        profitYear: profitYear,
        contributionAmount: 0,
        contributionDate: "",
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "contributionDate",
          isSortDescending: false
        },
        ...(archive && { archive: true }) // Add archive property if true
      };

      fetchContributions(request);
      setShowContributions(true);
    },
    [fetchContributions, masterInquiryMemberDetails, profitYear]
  );

  useEffect(() => {
    if (masterInquiryMemberDetails) {
      handleFetchContributions();
    }
  }, [handleFetchContributions, masterInquiryMemberDetails]);

  return (
    <Page
      label={CAPTIONS.MILITARY_CONTRIBUTIONS}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <MilitaryEntryAndModificationSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          {masterInquiryMemberDetails ? (
            <MilitaryContributionGrid
              setInitialSearchLoaded={setInitialSearchLoaded}
              initialSearchLoaded={initialSearchLoaded}
              onAddContribution={handleOpenForm}
            />
          ) : (
            <div className="military-contribution-message">
              Please search for and select an employee to view military contributions.
            </div>
          )}
        </Grid>
      </Grid>

      {/* Military Contribution Form Dialog */}
      <Dialog
        open={isDialogOpen}
        onClose={handleCloseForm}
        maxWidth="md"
        fullWidth>
        <DialogTitle>Add Military Contribution</DialogTitle>
        <DialogContent>
          <MilitaryContributionForm
            onSubmit={(rows) => {
              handleCloseForm();
              dispatch(InquiryApi.util.invalidateTags(["memberDetails"]));
            }}
            onCancel={handleCloseForm}
            badgeNumber={Number(masterInquiryMemberDetails?.badgeNumber)}
            profitYear={profitYear}
          />
        </DialogContent>
      </Dialog>
    </Page>
  );
};
export default MilitaryEntryAndModification;
