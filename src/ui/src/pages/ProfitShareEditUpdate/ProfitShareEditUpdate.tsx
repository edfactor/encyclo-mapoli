import { Button, Divider, Tooltip } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import ProfitShareEditUpdateSearchFilter from "./ProfitShareEditUpdateSearchFilter";
import ProfitShareEditUpdateGrid from "./ProfitShareEditUpdateGrid";
import { SaveOutlined } from "@mui/icons-material";
import { useDispatch, useSelector } from "react-redux";

const developmentNoteStyle = {
  backgroundColor: "#FFFFE0", // Light yellow
  padding: "10px",
  margin: "10px"
};

const RenderSaveButton = () => {
  //const dispatch = useDispatch();

  // This next line the function that makes the HTTP PUT call to
  // update the hours and dollars on the back end
  //const [updateHoursAndDollars] = useUpdateExecutiveHoursAndDollarsMutation();

  // This Grid is the group of pending updates that are changed rows in the grid
  //const { executiveHoursAndDollarsGrid } = useSelector((state: RootState) => state.yearsEnd);

  const pendingChanges = true;
  /*
    executiveHoursAndDollarsGrid !== undefined &&
    executiveHoursAndDollarsGrid?.executiveHoursAndDollars !== undefined &&
    executiveHoursAndDollarsGrid?.executiveHoursAndDollars.length != 0;
*/
  const saveButton = (
    <Button
      disabled={!pendingChanges}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<SaveOutlined color={pendingChanges ? "primary" : "disabled"} />}
      onClick={async () => {
        // Note that clearing the rows will also disable the save button,
        // which will be notified that there are no pending rows to save,
        // that happens when we do the clear call below
        /*
        updateHoursAndDollars(executiveHoursAndDollarsGrid)
          .unwrap()
          .then((payload) => console.log("Successfully updated hours and dollars. ", payload))
          .catch((error) => console.error("ERROR: Did not update hours and dollars", error));
        dispatch(clearExecutiveHoursAndDollarsGridRows());
        */
      }}>
      Save Updates
    </Button>
  );

  if (!pendingChanges) {
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
};

const RenderRevertButton = () => {
  //const dispatch = useDispatch();

  // This next line the function that makes the HTTP PUT call to
  // update the hours and dollars on the back end
  //const [updateHoursAndDollars] = useUpdateExecutiveHoursAndDollarsMutation();

  // This Grid is the group of pending updates that are changed rows in the grid
  //const { executiveHoursAndDollarsGrid } = useSelector((state: RootState) => state.yearsEnd);

  const pendingChanges = true;
  /*
    executiveHoursAndDollarsGrid !== undefined &&
    executiveHoursAndDollarsGrid?.executiveHoursAndDollars !== undefined &&
    executiveHoursAndDollarsGrid?.executiveHoursAndDollars.length != 0;
*/
  const saveButton = (
    <Button
      disabled={!pendingChanges}
      variant="outlined"
      color="primary"
      size="medium"
      //startIcon={<SaveOutlined color={pendingChanges ? "primary" : "disabled"} />}
      onClick={async () => {
        // Note that clearing the rows will also disable the save button,
        // which will be notified that there are no pending rows to save,
        // that happens when we do the clear call below
        /*
        updateHoursAndDollars(executiveHoursAndDollarsGrid)
          .unwrap()
          .then((payload) => console.log("Successfully updated hours and dollars. ", payload))
          .catch((error) => console.error("ERROR: Did not update hours and dollars", error));
        dispatch(clearExecutiveHoursAndDollarsGridRows());
        */
      }}>
      Revert
    </Button>
  );

  if (!pendingChanges) {
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
};

const ProfitShareEditUpdate = () => {
  return (
    <Page
      label="Profit Share Update/Edit/Master (PAY444/447/460 PROFTLD)"
      actionNode={
        <div className="flex mr-2 justify-end gap-2">
          {RenderSaveButton()}
          {RenderRevertButton()}
        </div>
      }>
      <div style={developmentNoteStyle}>
        DevNote: This page is functional but incomplete; needs Totals block, needs Adjustment Details, and needs Paging.{" "}
        <a
          style={{ color: "blue" }}
          href="https://demoulas.atlassian.net/browse/PS-945">
          PS-945
        </a>
      </div>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Parameters">
            <ProfitShareEditUpdateSearchFilter />
          </DSMAccordion>
        </Grid2>
        <Grid2 width="100%">
          <ProfitShareEditUpdateGrid />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ProfitShareEditUpdate;
