import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { Button, Divider } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { Dialog, DialogContent, DialogTitle } from "@mui/material";
import { CAPTIONS } from "../../../constants";
import MilitaryAndRehireEntryAndModificationSearchFilter from "./MilitaryEntryAndModificationSearchFilter";
import MilitaryContributionForm from "./MilitaryContributionForm";
import MilitaryContributionGrid from "./MilitaryContributionFormGrid";

const MilitaryEntryAndModification = () => {
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  const handleOpenForm = () => {
    setIsDialogOpen(true);
  };

  const handleCloseForm = () => {
    setIsDialogOpen(false);
  };

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
            <MilitaryAndRehireEntryAndModificationSearchFilter
              setInitialSearchLoaded={setInitialSearchLoaded}
            />
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
        fullWidth
      >
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