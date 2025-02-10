import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import { Button, Tooltip } from "@mui/material";
import ManageExecutiveHoursAndDollarsSearchFilter from "./ManageExecutiveHoursAndDollarsSearchFilter";
import ManageExecutiveHoursAndDollarsGrid from "./ManageExecutiveHoursAndDollarsGrid";
import { SaveOutlined } from "@mui/icons-material";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { clearExecutiveHoursAndDollarsGridRows } from "reduxstore/slices/yearsEndSlice";

const RenderSaveButton = (pendingChanges: boolean) => {
  const dispatch = useDispatch();
  const saveButton = (
    <Button
      disabled={!pendingChanges}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<SaveOutlined color={pendingChanges ? "primary" : "disabled"} />}
      onClick={async () => {
        // What do we need here?
        // 1. We need to do the submit via a Redux api call
        // 2. If fail, show an error and leave the grid unaltered
        // 3. If success, clear the pending changes of the grid
        // 4. Show some sort of confirmation message?

        // Note that clearing the rows will also disable the save button,
        // which will be notified that there are no pending rows to save
        dispatch(clearExecutiveHoursAndDollarsGridRows());
      }}>
      Save
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
const ManageExecutiveHoursAndDollars = () => {
  const { executiveHoursAndDollarsGrid } = useSelector((state: RootState) => state.yearsEnd);

  const pendingChanges =
    executiveHoursAndDollarsGrid !== undefined &&
    executiveHoursAndDollarsGrid?.executiveHoursAndDollars !== undefined &&
    executiveHoursAndDollarsGrid?.executiveHoursAndDollars.length != 0;

  return (
    <Page
      label="Manage Executive Hours And Dollars"
      actionNode={
        <div style={{ gap: "24px", display: "flex", justifyContent: "end" }}>{RenderSaveButton(pendingChanges)}</div>
      }>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
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
