import { Tooltip, Divider, Button } from "@mui/material";
import { Grid } from "@mui/material";
import { DSMAccordion, Page } from "smart-ui-library";
import ManageExecutiveHoursAndDollarsSearchFilter from "./ManageExecutiveHoursAndDollarsSearchFilter";
import ManageExecutiveHoursAndDollarsGrid from "./ManageExecutiveHoursAndDollarsGrid";
import { SaveOutlined } from "@mui/icons-material";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { clearExecutiveHoursAndDollarsGridRows } from "reduxstore/slices/yearsEndSlice";
import {
  useUpdateExecutiveHoursAndDollarsMutation,
  useLazyGetExecutiveHoursAndDollarsQuery
} from "reduxstore/api/YearsEndApi";
import { useState } from "react";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../constants";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";

const RenderSaveButton = () => {
  const dispatch = useDispatch();
  // This next line the function that makes the HTTP PUT call to
  // update the hours and dollars on the back end
  const [updateHoursAndDollars] = useUpdateExecutiveHoursAndDollarsMutation();

  // This Grid is the group of pending updates that are changed rows in the grid
  const { executiveHoursAndDollarsGrid } = useSelector((state: RootState) => state.yearsEnd);

  const pendingChanges =
    executiveHoursAndDollarsGrid !== undefined &&
    executiveHoursAndDollarsGrid?.executiveHoursAndDollars !== undefined &&
    executiveHoursAndDollarsGrid?.executiveHoursAndDollars.length != 0;

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

        updateHoursAndDollars(executiveHoursAndDollarsGrid)
          .unwrap()
          .then((payload) => console.log("Successfully updated hours and dollars. ", payload))
          .catch((error) => console.error("ERROR: Did not update hours and dollars", error));
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
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const [currentStatus, setCurrentStatus] = useState<string | null>(null);
  const [triggerExecutiveSearch] = useLazyGetExecutiveHoursAndDollarsQuery();
  const profitYear = useFiscalCloseProfitYear();

  const handleStatusChange = (newStatus: string, statusName?: string) => {
    // Only trigger if status is changing TO "Complete" (not already "Complete")
    if (statusName === "Complete" && currentStatus !== "Complete") {
      setCurrentStatus("Complete");
      // Trigger archive call with current search parameters
      // Note: This would need the current search parameters from the search filter
      // For now, we'll trigger with minimal parameters and archive=true
      triggerExecutiveSearch({
        profitYear: profitYear,
        hasExecutiveHoursAndDollars: true,
        isMonthlyPayroll: false,
        pagination: {
          skip: 0,
          take: 25,
          sortBy: "",
          isSortDescending: false
        },
        archive: true
      })
        .then((result: any) => {
          console.log("Executive hours and dollars archived successfully", result);
        })
        .catch((error: any) => {
          console.error("Error archiving executive hours and dollars:", error);
        });
    } else {
      setCurrentStatus(statusName || newStatus);
    }
  };

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={handleStatusChange} />;
  };

  return (
    <Page
      label={CAPTIONS.MANAGE_EXECUTIVE_HOURS}
      actionNode={renderActionNode()}>
      <Grid
        container
        rowSpacing="24px">
        <Grid width={"100%"}>
          <Divider />
        </Grid>
        <Grid width={"100%"}>
          <DSMAccordion title="Filter">
            <ManageExecutiveHoursAndDollarsSearchFilter
              setInitialSearchLoaded={setInitialSearchLoaded}
              setPageNumberReset={setPageNumberReset}
            />
          </DSMAccordion>
        </Grid>
        <Grid width="100%">
          <ManageExecutiveHoursAndDollarsGrid
            setInitialSearchLoaded={setInitialSearchLoaded}
            initialSearchLoaded={initialSearchLoaded}
            pageNumberReset={pageNumberReset}
            setPageNumberReset={setPageNumberReset}
          />
        </Grid>
      </Grid>
    </Page>
  );
};

export default ManageExecutiveHoursAndDollars;
