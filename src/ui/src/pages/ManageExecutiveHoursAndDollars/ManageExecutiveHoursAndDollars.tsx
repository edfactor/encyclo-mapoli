import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import PauseCircleOutlineOutlinedIcon from "@mui/icons-material/PauseCircleOutlineOutlined";
import { Button, Tooltip  } from "@mui/material";
import ManageExecutiveHoursAndDollarsSearchFilter from "./ManageExecutiveHoursAndDollarsSearchFilter";
import ManageExecutiveHoursAndDollarsGrid from "./ManageExecutiveHoursAndDollarsGrid";
import { FolderOutlined, SaveOutlined } from "@mui/icons-material";


const renderSaveButton = () => {
  const canSave = true;

  const saveButton = (
    <Button
      disabled={!canSave}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<SaveOutlined color={canSave ? "primary" : "disabled"} />}
      onClick={async () => {
        //await saveDeposit({ ardId: currentArdId ?? 0 });
        //navigate("/charges/payments/payment-confirmation");
      }}>
      Save
    </Button>
  );

  if (!canSave) {
    return (
      <Tooltip
        placement="top"
        title="You must change hours or dollars to save.">
        <span>{saveButton}</span>
      </Tooltip>
    );
  } else {
    return saveButton;
  }
}
const ManageExecutiveHoursAndDollars = () => {
  return (
    <Page 
      label="Manage Executive Hours And Dollars"
      actionNode={
        <div style={{ gap: "24px", display: "flex", justifyContent: "end" }}>
          {renderSaveButton()}
        </div>
      }>
        <Grid2
          container
          rowSpacing="24px">
          <Grid2 width={"100%"}>
            <Divider />
          </Grid2>
          <Grid2
            width={"100%"}>
              <DSMAccordion title="Filter">
                <ManageExecutiveHoursAndDollarsSearchFilter />
              </DSMAccordion>
             
          </Grid2>

          <Grid2 width="100%">
            <ManageExecutiveHoursAndDollarsGrid />
          </Grid2>
        </Grid2>
    </Page>
  );
};

export default ManageExecutiveHoursAndDollars;
