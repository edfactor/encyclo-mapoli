import { Button, Divider, Tooltip } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import ProfitShareEditUpdateSearchFilter from "./ProfitShareEditUpdateSearchFilter";
import { Replay, SaveOutlined } from "@mui/icons-material";
import { useDispatch, useSelector } from "react-redux";
import ProfitShareEditUpdateTabs from "./ProfitShareEditUpdateTabs";
import { RootState } from "reduxstore/store";
import { useLazyGetMasterApplyQuery, useLazyGetMasterRevertQuery } from "reduxstore/api/YearsEndApi";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { clearProfitEdit, setProfitEditUpdateRevertChangesAvailable } from "reduxstore/slices/yearsEndSlice";

const RenderSaveButton = () => {
  //const dispatch = useDispatch();

  // This next line the function that makes the HTTP PUT call to
  // update the hours and dollars on the back end
  //const [updateHoursAndDollars] = useUpdateExecutiveHoursAndDollarsMutation();

  // This Grid is the group of pending updates that are changed rows in the grid
  //const { executiveHoursAndDollarsGrid } = useSelector((state: RootState) => state.yearsEnd);

  const pendingChanges = true;

  const { profitEditUpdateChangesAvailable } = useSelector((state: RootState) => state.yearsEnd);
  const [masterApply] = useLazyGetMasterApplyQuery();

  // This is the condition to check if there are pending changes

  /*
    executiveHoursAndDollarsGrid !== undefined &&
    executiveHoursAndDollarsGrid?.executiveHoursAndDollars !== undefined &&
    executiveHoursAndDollarsGrid?.executiveHoursAndDollars.length != 0;
*/
  const saveButton = (
    <Button
      disabled={!profitEditUpdateChangesAvailable}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<SaveOutlined color={profitEditUpdateChangesAvailable ? "primary" : "disabled"} />}
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

  if (!profitEditUpdateChangesAvailable) {
    return (
      <Tooltip
        placement="top"
        title="You must have previewed data to save.">
        <span>{saveButton}</span>
      </Tooltip>
    );
  } else {
    return saveButton;
  }
};

const RenderRevertButton = () => {
  const { profitEditUpdateRevertChangesAvailable } = useSelector((state: RootState) => state.yearsEnd);
  const fiscalCloseProfitYear = useFiscalCloseProfitYear();
  const [masterRevert] = useLazyGetMasterRevertQuery();

  const dispatch = useDispatch();

  // This next line the function that makes the HTTP PUT call to
  // update the hours and dollars on the back end
  //const [updateHoursAndDollars] = useUpdateExecutiveHoursAndDollarsMutation();

  // This Grid is the group of pending updates that are changed rows in the grid
  //const { executiveHoursAndDollarsGrid } = useSelector((state: RootState) => state.yearsEnd);

  /*
    executiveHoursAndDollarsGrid !== undefined &&
    executiveHoursAndDollarsGrid?.executiveHoursAndDollars !== undefined &&
    executiveHoursAndDollarsGrid?.executiveHoursAndDollars.length != 0;
*/
  const revertButton = (
    <Button
      disabled={!profitEditUpdateRevertChangesAvailable}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<Replay color={profitEditUpdateRevertChangesAvailable ? "primary" : "disabled"} />}
      onClick={async () => {
        masterRevert({
          profitYear: fiscalCloseProfitYear
        })
          .unwrap()
          .then((payload) => console.log("Successfully reverted year end. ", payload))
          .catch((error) => console.error("ERROR: Did not revert year end", error));
        dispatch(setProfitEditUpdateRevertChangesAvailable(false));
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

  if (!profitEditUpdateRevertChangesAvailable) {
    return (
      <Tooltip
        placement="top"
        title="You must have just saved data to revert.">
        <span>{revertButton}</span>
      </Tooltip>
    );
  } else {
    return revertButton;
  }
};

const ProfitShareEditUpdate = () => {
  return (
    <Page
      label="Master Update (PAY444/PAY447/PROFTLD)"
      actionNode={
        <div className="flex  justify-end gap-2">
          {RenderRevertButton()}
          {RenderSaveButton()}
        </div>
      }>
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
          <ProfitShareEditUpdateTabs />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default ProfitShareEditUpdate;
