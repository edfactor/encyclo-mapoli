import { Dialog, DialogContent, DialogTitle, Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useCallback, useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useLazyGetMilitaryContributionsQuery } from "../../../reduxstore/api/MilitaryApi";
import MilitaryContributionForm from "./MilitaryContributionForm";
import MilitaryContributionGrid from "./MilitaryContributionFormGrid";
import MilitaryAndRehireEntryAndModificationSearchFilter from "./MilitaryEntryAndModificationSearchFilter";

const MilitaryEntryAndModification = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [showContributions, setShowContributions] = useState(false);
  const { masterInquiryEmployeeDetails } = useSelector((state: RootState) => state.inquiry);
  const [fetchContributions, { isFetching }] = useLazyGetMilitaryContributionsQuery();

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  const handleOpenForm = () => {
    setIsDialogOpen(true);
  };

  const handleCloseForm = () => {
    setIsDialogOpen(false);
  };

  const handleFetchContributions = useCallback(() => {
    if (masterInquiryEmployeeDetails) {
      fetchContributions({
        badgeNumber: Number(masterInquiryEmployeeDetails.badgeNumber),
        profitYear: 2024,
        pagination: { skip: 0, take: 25 }
      });
      setShowContributions(true);
    }
  }, [masterInquiryEmployeeDetails, fetchContributions]);

  useEffect(() => {
    if (masterInquiryEmployeeDetails) {
      handleFetchContributions();
    }
  }, [handleFetchContributions, masterInquiryEmployeeDetails]);

  return (
    <Page
      label={CAPTIONS.MILITARY_CONTRIBUTIONS}
      actionNode={renderActionNode()}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <MilitaryAndRehireEntryAndModificationSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
          </DSMAccordion>
        </Grid2>

        <Grid2 width="100%">
          <MilitaryContributionGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
            onAddContribution={handleOpenForm}
          />
        </Grid2>
      </Grid2>

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
              // Handle submit logic
              handleCloseForm();
              setInitialSearchLoaded(true); // Trigger a refresh
            }}
            onCancel={handleCloseForm}
          />
        </DialogContent>
      </Dialog>
    </Page>
  );
};
export default MilitaryEntryAndModification;
