import { Tooltip, Divider, Button } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import { DSMAccordion, Page } from "smart-ui-library";
import ManageExecutiveHoursAndDollarsSearchFilter from "./ManageExecutiveHoursAndDollarsSearchFilter";
import ManageExecutiveHoursAndDollarsGrid from "./ManageExecutiveHoursAndDollarsGrid";
import { AddOutlined } from "@mui/icons-material";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { setAdditionalExecutivesChosen } from "reduxstore/slices/yearsEndSlice";

const RenderAddButton = () => {
  const dispatch = useDispatch();
  const { executiveRowsSelected } = useSelector((state: RootState) => state.yearsEnd);

  const addEnabled = executiveRowsSelected && executiveRowsSelected.length > 0;

  const addButton = (
    <Button
      disabled={!addEnabled}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<AddOutlined color={addEnabled ? "primary" : "disabled"} />}
      onClick={async () => {
        // So what we need to do here is to take the array of selected rows
        // and add them to the additional executives and the main grid will
        // pick them up upon re-render
        if (executiveRowsSelected) {
          dispatch(setAdditionalExecutivesChosen(executiveRowsSelected));
        }
      }}>
      Add to Main Grid
    </Button>
  );

  if (!addEnabled) {
    return (
      <Tooltip
        placement="top"
        title="You must select only one row to add.">
        <span>{addButton}</span>
      </Tooltip>
    );
  } else {
    return addButton;
  }
};
const SearchAndAddExecutive = () => {
  return (
    <Page
      label="Add New Executive"
      actionNode={<div className="flex mr-2 justify-end gap-24">{RenderAddButton()}</div>}>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Filter">
            <ManageExecutiveHoursAndDollarsSearchFilter isModal={true} />
          </DSMAccordion>
        </Grid2>
        <Grid2 width="100%">
          <ManageExecutiveHoursAndDollarsGrid isModal={true} />
        </Grid2>
      </Grid2>
    </Page>
  );
};

export default SearchAndAddExecutive;
