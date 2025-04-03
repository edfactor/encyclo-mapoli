import { Replay, SaveOutlined } from "@mui/icons-material";
import { Button, Divider, Tooltip, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { useDispatch, useSelector } from "react-redux";
import { DSMAccordion, numberToCurrency, Page } from "smart-ui-library";
import ProfitShareEditUpdateSearchFilter from "./ProfitShareEditUpdateSearchFilter";
import { TotalsGrid } from "../../components/TotalsGrid";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useLazyGetMasterApplyQuery, useLazyGetMasterRevertQuery } from "reduxstore/api/YearsEndApi";
import { setProfitEditUpdateRevertChangesAvailable } from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import ProfitShareEditUpdateTabs from "./ProfitShareEditUpdateTabs";

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
  const { profitSharingUpdateAdjustmentSummary, profitSharingUpdate } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  //console.log("Total results: ", profitSharingUpdate?.response.results.length);
  return (
    <Page
      label="Master Update (PAY444/PAY447/PROFTLD)"
      actionNode={
        <div className="flex  justify-end gap-2">
          {RenderRevertButton()}
          {RenderSaveButton()}
        </div>
      }>
      <div style={developmentNoteStyle}>
        Note: Much of this page works, but is incomplete. Needs adjustment summary panel, apply update, and revert.{" "}
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
        {profitSharingUpdate && (
          <div>
            <div className="px-[24px]">
              <h2 className="text-dsm-secondary">Summary</h2>
              <Typography
                fontWeight="bold"
                variant="body2">
                {`Employees: ${profitSharingUpdate.totals.totalEmployees} | Beneficiaries: ${profitSharingUpdate.totals.totalBeneficiaries || 0}`}
              </Typography>
            </div>

            <TotalsGrid
              displayData={[
                [
                  numberToCurrency(profitSharingUpdate.totals.beginningBalance || 0),
                  numberToCurrency(profitSharingUpdate.totals.totalContribution || 0),
                  numberToCurrency(profitSharingUpdate.totals.earnings || 0),
                  numberToCurrency(profitSharingUpdate.totals.earnings2 || 0),
                  numberToCurrency(profitSharingUpdate.totals.forfeiture || 0),
                  numberToCurrency(profitSharingUpdate.totals.military || 0),
                  numberToCurrency(profitSharingUpdate.totals.endingBalance || 0)
                ],
                [
                  numberToCurrency(0),
                  numberToCurrency(profitSharingUpdate.totals.allocations || 0),
                  numberToCurrency(0),
                  numberToCurrency(0),
                  numberToCurrency(profitSharingUpdate.totals.maxPointsTotal || 0),
                  numberToCurrency(profitSharingUpdate.totals.paidAllocations || 0),
                  numberToCurrency(0)
                ],
                [
                  numberToCurrency(0),
                  numberToCurrency(profitSharingUpdate.totals.contributionPoints || 0),
                  numberToCurrency(profitSharingUpdate.totals.earningPoints || 0),
                  numberToCurrency(0),
                  numberToCurrency(0),
                  numberToCurrency(0),
                  numberToCurrency(0)
                ]
              ]}
              leftColumnHeaders={["Total", "Allocation", "Point"]}
              topRowHeaders={[
                "",
                "Beginning Balance",
                "Contributions",
                "Earnings",
                "Earnings2",
                "Forfeitures",
                "Distributions Military/Paid Allocation",
                "Ending Balance"
              ]}
            />
            {profitSharingUpdateAdjustmentSummary?.badgeNumber && (
              <>
                <div className="px-[24px]">
                  <h2 className="text-dsm-secondary">Adjustments Entered</h2>
                </div>

                <TotalsGrid
                  displayData={[
                    [
                      profitSharingUpdateAdjustmentSummary?.badgeNumber,
                      numberToCurrency(profitSharingUpdateAdjustmentSummary?.contributionAmountUnadjusted || 0),
                      numberToCurrency(profitSharingUpdateAdjustmentSummary?.earningsAmountUnadjusted || 0),
                      numberToCurrency(profitSharingUpdateAdjustmentSummary?.secondaryEarningsAmountUnadjusted || 0),
                      numberToCurrency(profitSharingUpdateAdjustmentSummary?.incomingForfeitureAmountUnadjusted || 0)
                    ],
                    [
                      "",
                      numberToCurrency(profitSharingUpdateAdjustmentSummary?.contributionAmountAdjusted || 0),
                      numberToCurrency(profitSharingUpdateAdjustmentSummary?.earningsAmountAdjusted || 0),
                      numberToCurrency(profitSharingUpdateAdjustmentSummary?.secondaryEarningsAmountAdjusted || 0),
                      numberToCurrency(profitSharingUpdateAdjustmentSummary?.incomingForfeitureAmountAdjusted || 0)
                    ],
                    [
                      "",
                      numberToCurrency(profitSharingUpdateAdjustmentSummary?.contributionAmountAdjusted || 0),
                      numberToCurrency(profitSharingUpdateAdjustmentSummary?.earningsAmountAdjusted || 0),
                      numberToCurrency(profitSharingUpdateAdjustmentSummary?.secondaryEarningsAmountAdjusted || 0),
                      numberToCurrency(profitSharingUpdateAdjustmentSummary?.incomingForfeitureAmountAdjusted || 0)
                    ]
                  ]}
                  leftColumnHeaders={["Initial", "Adjustment", "Totals"]}
                  topRowHeaders={["", "Badge", "Contributions", "Earnings", "Earnings2", "Forfeitures"]}
                  headerCellStyle={{}}
                />
              </>
            )}
            <Grid2 width="100%">
              <ProfitShareEditUpdateTabs />
            </Grid2>
          </div>
        )}
      </Grid2>
    </Page>
  );
};

export default ProfitShareEditUpdate;
