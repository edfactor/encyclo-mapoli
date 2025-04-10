import { Replay, SaveOutlined } from "@mui/icons-material";
import { Button, Divider, Tooltip, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLazyGetMasterRevertQuery, useLazyGetMasterApplyQuery } from "reduxstore/api/YearsEndApi";
import {
  setProfitEditUpdateChangesAvailable,
  setProfitEditUpdateRevertChangesAvailable
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import { ProfitShareMasterApplyRequest } from "reduxstore/types";
import { ApiMessageAlert, DSMAccordion, MessageUpdate, numberToCurrency, Page, setMessage } from "smart-ui-library";
import { TotalsGrid } from "../../components/TotalsGrid";
import ProfitShareEditUpdateSearchFilter from "./ProfitShareEditUpdateSearchFilter";
import ProfitShareEditUpdateTabs from "./ProfitShareEditUpdateTabs";

enum MessageKeys {
  ProfitShareEditUpdate = "ProfitShareEditUpdate"
}

let beneficiariesAffected = 0;
let employeesAffected = 0;
let etvasAffected = 0;

class Messages {
  static readonly ProfitShareApplySuccess: MessageUpdate = {
    key: MessageKeys.ProfitShareEditUpdate,
    message: {
      type: "success",
      title: "Changes Applied",
      message: `Employees affected: ${employeesAffected} | Beneficiaries: ${beneficiariesAffected}, | ETVAs: ${etvasAffected} `
    }
  };
  static readonly ProfitShareApplyFail: MessageUpdate = {
    key: MessageKeys.ProfitShareEditUpdate,
    message: {
      type: "error",
      title: "Changes Were Not Applied",
      message: `Employees affected: 0 | Beneficiaries: 0, | ETVAs: 0 `
    }
  };
  static readonly ProfitShareRevertSuccess: MessageUpdate = {
    key: MessageKeys.ProfitShareEditUpdate,
    message: {
      type: "success",
      title: "Changes Reverted",
      message: `Employees affected: ${employeesAffected} | Beneficiaries: ${beneficiariesAffected}, | ETVAs: ${etvasAffected} `
    }
  };
  static readonly ProfitShareRevertFail: MessageUpdate = {
    key: MessageKeys.ProfitShareEditUpdate,
    message: {
      type: "error",
      title: "Changes Were Not Reverted",
      message: `Employees affected: 0 | Beneficiaries: 0, | ETVAs: 0 `
    }
  };
}

const RenderSaveButton = () => {
  //const dispatch = useDispatch();

  // This next line the function that makes the HTTP PUT call to
  // update the hours and dollars on the back end
  //const [updateHoursAndDollars] = useUpdateExecutiveHoursAndDollarsMutation();

  // This Grid is the group of pending updates that are changed rows in the grid
  //const { executiveHoursAndDollarsGrid } = useSelector((state: RootState) => state.yearsEnd);

  const pendingChanges = true;

  const { profitEditUpdateChangesAvailable, profitSharingEditQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const [trigger, { isFetching }] = useLazyGetMasterApplyQuery();
  const dispatch = useDispatch();
  const saveButton = (
    <Button
      disabled={!profitEditUpdateChangesAvailable}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<SaveOutlined color={profitEditUpdateChangesAvailable ? "primary" : "disabled"} />}
      onClick={async () => {
        const params: ProfitShareMasterApplyRequest = {
          profitYear: profitSharingEditQueryParams?.profitYear.getFullYear() ?? 0,
          contributionPercent: profitSharingEditQueryParams?.contributionPercent ?? 0,
          earningsPercent: profitSharingEditQueryParams?.earningsPercent ?? 0,
          incomingForfeitPercent: profitSharingEditQueryParams?.incomingForfeitPercent ?? 0,
          secondaryEarningsPercent: profitSharingEditQueryParams?.secondaryEarningsPercent ?? 0,
          maxAllowedContributions: profitSharingEditQueryParams?.maxAllowedContributions ?? 0,
          badgeToAdjust: profitSharingEditQueryParams?.badgeToAdjust ?? 0,
          adjustContributionAmount: profitSharingEditQueryParams?.adjustContributionAmount ?? 0,
          adjustEarningsAmount: profitSharingEditQueryParams?.adjustEarningsAmount ?? 0,
          adjustIncomingForfeitAmount: profitSharingEditQueryParams?.adjustEarningsSecondaryAmount ?? 0,
          badgeToAdjust2: profitSharingEditQueryParams?.badgeToAdjust2 ?? 0,
          adjustEarningsSecondaryAmount: profitSharingEditQueryParams?.adjustEarningsSecondaryAmount ?? 0
        };

        console.log("Applying changes to year end: ", params);
        console.log(params);

        await trigger(params)
          .unwrap()
          .then((payload) => {
            employeesAffected = payload?.employeesAffected || 0;
            beneficiariesAffected = payload?.beneficiariesAffected || 0;
            etvasAffected = payload?.etvasAffected || 0;
            dispatch(setMessage(Messages.ProfitShareApplySuccess));
            console.log("Successfully applied changes to year end: ", payload);
            dispatch(setProfitEditUpdateRevertChangesAvailable(true));
          })
          .catch((error) => {
            console.error("ERROR: Did not apply changes to year end", error);
            dispatch(setMessage(Messages.ProfitShareApplyFail));
          });
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
  const [trigger, { isFetching }] = useLazyGetMasterRevertQuery();

  const dispatch = useDispatch();
  const revertButton = (
    <Button
      disabled={!profitEditUpdateRevertChangesAvailable}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<Replay color={profitEditUpdateRevertChangesAvailable ? "primary" : "disabled"} />}
      onClick={async () => {
        await trigger({ profitYear: fiscalCloseProfitYear }, false)
          .unwrap()
          .then((payload) => {
            employeesAffected = payload?.employeesAffected || 0;
            beneficiariesAffected = payload?.beneficiariesAffected || 0;
            etvasAffected = payload?.etvasAffected || 0;
            dispatch(setMessage(Messages.ProfitShareRevertSuccess));
            console.log("Successfully reverted changes for year end: ", payload);
            dispatch(setProfitEditUpdateChangesAvailable(false));
          })
          .catch((error) => {
            console.error("ERROR: Did not revert changes to year end", error);
            dispatch(setMessage(Messages.ProfitShareRevertFail));
          });
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
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const { profitSharingUpdateAdjustmentSummary, profitSharingUpdate } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const dispatch = useDispatch();
  //console.log("Total results: ", profitSharingUpdate?.response.results.length);
  return (
    <Page
      label="Master Update (PAY444/PAY447)"
      actionNode={
        <div className="flex  justify-end gap-2">
          {RenderRevertButton()}
          {RenderSaveButton()}
        </div>
      }>
      <div>
        <ApiMessageAlert commonKey={MessageKeys.ProfitShareEditUpdate} />
        <div className="h-4"></div>
        <Button onClick={() => dispatch(setMessage(Messages.ProfitShareRevertSuccess))}>Show Success</Button>
      </div>
      <Grid2
        container
        rowSpacing="24px">
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        <Grid2 width={"100%"}>
          <DSMAccordion title="Parameters">
            <ProfitShareEditUpdateSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
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
                  "",
                  numberToCurrency(profitSharingUpdate.totals.allocations || 0),
                  "",
                  "",
                  numberToCurrency(profitSharingUpdate.totals.maxPointsTotal || 0),
                  numberToCurrency(profitSharingUpdate.totals.paidAllocations || 0),
                  numberToCurrency(
                    (profitSharingUpdate.totals.allocations || 0) + (profitSharingUpdate.totals.paidAllocations || 0)
                  )
                ],
                [
                  "",
                  numberToCurrency(profitSharingUpdate.totals.contributionPoints || 0),
                  numberToCurrency(profitSharingUpdate.totals.earningPoints || 0),
                  "",
                  "",
                  "",
                  ""
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
              <ProfitShareEditUpdateTabs
                initialSearchLoaded={initialSearchLoaded}
                setInitialSearchLoaded={setInitialSearchLoaded}
              />
            </Grid2>
          </div>
        )}
      </Grid2>
    </Page>
  );
};

export default ProfitShareEditUpdate;
