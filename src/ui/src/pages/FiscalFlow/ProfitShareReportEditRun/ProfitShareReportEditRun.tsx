import { Button, Divider } from "@mui/material";
import { Grid } from "@mui/material";
import { DSMAccordion, Page, SmartModal } from "smart-ui-library";
import ProfitShareReportEditRunParameters from "./ProfitShareReportEditRunParameters";
import { CAPTIONS } from "../../../constants";
import ProfitShareReportEditRunResults from "./ProfitShareReportEditRunResults";
import { useState } from "react";

const ProfitShareReportEditRun = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);

  const handleConfirm = () => {
    console.log("Commit Updates");
    setIsModalOpen(false);
  };

  const handleCancel = () => {
    setIsModalOpen(false);
  };

  const renderActionNode = () => {
    return (
      <Button
        onClick={() => setIsModalOpen(true)}
        variant="outlined"
        className="h-10 min-w-fit whitespace-nowrap">
        Commit Updates
      </Button>
    );
  };

  return (
    <Page
      label={CAPTIONS.PROFIT_SHARE_REPORT_EDIT_RUN}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <ProfitShareReportEditRunParameters />
          </DSMAccordion>
        </Grid>

        <Grid width="100%">
          <ProfitShareReportEditRunResults />
        </Grid>
      </Grid>

      <SmartModal
        open={isModalOpen}
        onClose={handleCancel}
        actions={[
          <Button
            onClick={handleConfirm}
            variant="contained"
            color="primary"
            className="mr-2">
            Commit Updates
          </Button>,
          <Button
            onClick={handleCancel}
            variant="outlined">
            Cancel
          </Button>
        ]}
        title="Confirm Updates">
        Are you sure you want to commit these updates?
      </SmartModal>
    </Page>
  );
};

export default ProfitShareReportEditRun;
