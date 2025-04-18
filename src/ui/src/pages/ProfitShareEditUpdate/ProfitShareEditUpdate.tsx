import { Replay } from "@mui/icons-material";
import { Button, CircularProgress, Divider, Tooltip, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  useLazyGetMasterApplyQuery,
  useLazyGetMasterRevertQuery,
  useLazyGetProfitMasterStatusQuery
} from "reduxstore/api/YearsEndApi";
import {
  clearProfitSharingEdit,
  clearProfitSharingEditQueryParams,
  clearProfitSharingUpdate,
  setProfitEditUpdateChangesAvailable,
  setProfitEditUpdateRevertChangesAvailable,
  setProfitShareApplyOrRevertLoading,
  setProfitShareEditUpdateShowSearch,
  setResetYearEndPage
} from "reduxstore/slices/yearsEndSlice";
import { RootState } from "reduxstore/store";
import {
  ProfitMasterStatus,
  ProfitShareEditUpdateQueryParams,
  ProfitShareMasterApplyRequest,
  ProfitShareMasterResponse,
  ProfitYearRequest
} from "reduxstore/types";
import {
  ApiMessageAlert,
  DSMAccordion,
  MessageUpdate,
  numberToCurrency,
  Page,
  setMessage,
  SmartModal
} from "smart-ui-library";
import { TotalsGrid } from "../../components/TotalsGrid";
import ProfitShareEditConfirmation from "./ProfitShareEditConfirmation";
import ProfitShareEditUpdateSearchFilter from "./ProfitShareEditUpdateSearchFilter";
import ProfitShareEditUpdateTabs from "./ProfitShareEditUpdateTabs";
import ChangesList from "./ChangesList";

enum MessageKeys {
  ProfitShareEditUpdate = "ProfitShareEditUpdate"
}

export class Messages {
  static readonly ProfitShareApplySuccess: MessageUpdate = {
    key: MessageKeys.ProfitShareEditUpdate,
    message: {
      type: "success",
      title: "Changes Applied",
      message: `Employees affected: x | Beneficiaries: x, | ETVAs: x `
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
      message: `Employees affected: x | Beneficiaries: x, | ETVAs: x `
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
  static readonly ProfitShareMasterUpdated: MessageUpdate = {
    key: MessageKeys.ProfitShareEditUpdate,
    message: {
      type: "success",
      title: "Changes Already Applied",
      message: `Updated By: x | Date: x `
    }
  };
}

const useRevertAction = (
  setEmployeesReverted: (count: number) => void,
  setBeneficiariesReverted: (count: number) => void,
  setEtvasReverted: (count: number) => void
) => {
  const [trigger] = useLazyGetMasterRevertQuery();
  const dispatch = useDispatch();
  const profitYear = useFiscalCloseProfitYear();
  //const { profitSharingEdit, profitSharingUpdate } = useSelector((state: RootState) => state.yearsEnd);

  const revertAction = async (): Promise<void> => {
    const params: ProfitYearRequest = {
      profitYear: profitYear ?? 0
    };

    console.log("reverting changes to year end: ", params);
    console.log(params);

    await trigger(params, false)
      .unwrap()
      .then((payload) => {
        setEmployeesReverted(payload?.employeesEffected || 0);
        setBeneficiariesReverted(payload?.beneficiariesEffected || 0);
        setEtvasReverted(payload?.etvasEffected || 0);
        //dispatch(setMessage(successMessage));
        console.log("Successfully reverted changes for year end: ", payload);
        dispatch(setProfitEditUpdateChangesAvailable(false));
        dispatch(setProfitEditUpdateRevertChangesAvailable(false));
        dispatch(clearProfitSharingEditQueryParams());
        dispatch(
          setMessage({
            ...Messages.ProfitShareRevertSuccess,
            message: {
              ...Messages.ProfitShareRevertSuccess.message,
              message: `Employees affected: ${payload?.employeesEffected} | Beneficiaries: ${payload?.beneficiariesEffected} | ETVAs: ${payload?.etvasEffected} `
            }
          })
        );
        // Clear form and grids (we need to call reset on the search filter page)
        dispatch(setResetYearEndPage(true));
        // Bring search filters back
        dispatch(setProfitShareEditUpdateShowSearch(true));
        dispatch(clearProfitSharingEdit());
        dispatch(clearProfitSharingUpdate());
      })
      .catch((error) => {
        console.error("ERROR: Did not revert changes to year end", error);
        //dispatch(setMessage(failMessage));
        dispatch(
          setMessage({
            ...Messages.ProfitShareRevertFail,
            message: {
              ...Messages.ProfitShareRevertFail.message,
              message: `Employees affected: 0 | Beneficiaries: 0, | ETVAs: 0 `
            }
          })
        );
        setEmployeesReverted(0);
        setBeneficiariesReverted(0);
        setEtvasReverted(0);
      });
  };
  return revertAction;
};

const useSaveAction = (
  setEmployeesReverted: (count: number) => void,
  setBeneficiariesReverted: (count: number) => void,
  setEtvasReverted: (count: number) => void
) => {
  const { profitSharingEditQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const [trigger] = useLazyGetMasterApplyQuery();
  const dispatch = useDispatch();
  const profitYear = useFiscalCloseProfitYear();

  const saveAction = async (): Promise<void> => {
    const params: ProfitShareMasterApplyRequest = {
      profitYear: profitYear ?? 0,
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

    dispatch(setProfitShareApplyOrRevertLoading(true));

    await trigger(params)
      .unwrap()
      .then((payload: ProfitShareMasterResponse) => {
        dispatch(setProfitEditUpdateChangesAvailable(false));

        console.log("Successfully applied changes to year end: ", payload);
        console.log("Employees affected: ", payload?.employeesEffected);
        setEmployeesReverted(payload?.employeesEffected ?? 0);
        setBeneficiariesReverted(payload?.beneficiariesEffected ?? 0);
        setEtvasReverted(payload?.etvasEffected ?? 0);
        dispatch(
          setMessage({
            ...Messages.ProfitShareApplySuccess,
            message: {
              ...Messages.ProfitShareApplySuccess.message,
              message: `Employees affected: ${payload?.employeesEffected} | Beneficiaries: ${payload?.beneficiariesEffected} | ETVAs: ${payload?.etvasEffected} `
            }
          })
        );
        dispatch(setResetYearEndPage(true));
        dispatch(setProfitEditUpdateRevertChangesAvailable(true));
        dispatch(setProfitShareEditUpdateShowSearch(false));
        // Clear the grids
        dispatch(clearProfitSharingUpdate());
      })
      .catch((error) => {
        console.error("ERROR: Did not apply changes to year end", error);
        dispatch(
          setMessage({
            ...Messages.ProfitShareApplyFail,
            message: {
              ...Messages.ProfitShareApplyFail.message,
              message: `Employees affected: 0 | Beneficiaries: 0, | ETVAs: 0 `
            }
          })
        );
      });
    dispatch(setProfitShareApplyOrRevertLoading(false));
  };

  return saveAction;
};

const wasFormUsed = (profitSharingEditQueryParams: ProfitShareEditUpdateQueryParams) => {
  return (
    (profitSharingEditQueryParams?.contributionPercent ?? 0) > 0 ||
    (profitSharingEditQueryParams?.earningsPercent ?? 0) > 0 ||
    (profitSharingEditQueryParams?.incomingForfeitPercent ?? 0) > 0 ||
    (profitSharingEditQueryParams?.maxAllowedContributions ?? 0) > 0
  );
};

// This really just opens the modal. The modal for this has the function to call
// the back end
const RenderSaveButton = (
  setOpenSaveModal: (open: boolean) => void,
  setOpenEmptyModal: (open: boolean) => void,
  status: ProfitMasterStatus | null,
  isLoading: boolean
) => {
  // The incoming status field is about whether or not changes have already been applied
  const { profitEditUpdateChangesAvailable, profitSharingEditQueryParams, profitShareApplyOrRevertLoading } =
    useSelector((state: RootState) => state.yearsEnd);
  const saveButton = (
    <Button
      disabled={(!profitEditUpdateChangesAvailable && status?.updatedTime !== null) || isLoading}
      variant="outlined"
      color="primary"
      size="medium"
      onClick={async () => {
        if (profitSharingEditQueryParams && wasFormUsed(profitSharingEditQueryParams)) {
          setOpenSaveModal(true);
        } else {
          setOpenEmptyModal(true);
        }
      }}>
      {isLoading || profitShareApplyOrRevertLoading ? (
        //Prevent loading spinner from shrinking button
        <div className="spinner">
          <CircularProgress
            color="inherit"
            size="20px"
          />
        </div>
      ) : (
        "Save Updates"
      )}
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

// This really just opens the modal. The modal for this has the function to call
// the back end
const RenderRevertButton = (setOpenRevertModal: (open: boolean) => void, isLoading: boolean) => {
  // The incoming status field is about whether or not changes have already been applied
  const { profitEditUpdateRevertChangesAvailable, profitShareApplyOrRevertLoading } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const revertButton = (
    <Button
      disabled={!profitEditUpdateRevertChangesAvailable || isLoading}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={isLoading ? null : <Replay color={profitEditUpdateRevertChangesAvailable ? "primary" : "disabled"} />}
      onClick={async () => {
        setOpenRevertModal(true);
      }}>
      {isLoading || profitShareApplyOrRevertLoading ? (
        //Prevent loading spinner from shrinking button
        <div className="spinner">
          <CircularProgress
            color="inherit"
            size="20px"
          />
        </div>
      ) : (
        "Revert"
      )}
    </Button>
  );

  if (!profitEditUpdateRevertChangesAvailable) {
    return (
      <Tooltip
        placement="top"
        title="You must have applied data to revert.">
        <span>{revertButton}</span>
      </Tooltip>
    );
  } else {
    return revertButton;
  }
};

const ProfitShareEditUpdate = () => {
  const [beneficiariesAffected, setBeneficiariesAffected] = useState(0);
  const [employeesAffected, setEmployeesAffected] = useState(0);
  const [etvasAffected, setEtvasAffected] = useState(0);
  const [beneficiariesReverted, setBeneficiariesReverted] = useState(0);
  const [employeesReverted, setEmployeesReverted] = useState(0);
  const [etvasReverted, setEtvasReverted] = useState(0);
  const [updatedBy, setUpdatedBy] = useState<string | null>(null);
  const [updatedTime, setUpdatedTime] = useState<string | null>(null);
  const revertAction = useRevertAction(setEmployeesReverted, setBeneficiariesReverted, setEtvasReverted);
  const saveAction = useSaveAction(setEmployeesAffected, setBeneficiariesAffected, setEtvasAffected);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const {
    profitSharingUpdateAdjustmentSummary,
    profitSharingUpdate,
    profitSharingEdit,
    profitSharingEditQueryParams,
    profitMasterStatus,
    profitShareEditUpdateShowSearch,
    profitEditUpdateRevertChangesAvailable
  } = useSelector((state: RootState) => state.yearsEnd);
  const [openSaveModal, setOpenSaveModal] = useState<boolean>(false);
  const [openRevertModal, setOpenRevertModal] = useState<boolean>(false);
  const [openEmptyModal, setOpenEmptyModal] = useState<boolean>(false);

  const [triggerStatusUpdate, { isLoading }] = useLazyGetProfitMasterStatusQuery();

  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();

  const onStatusSearch = useCallback(async () => {
    const request: ProfitYearRequest = {
      profitYear: profitYear ?? 0
    };

    console.log("Getting status...");

    await triggerStatusUpdate(request, false)
      .unwrap()
      .then((payload) => {
        if (payload?.updatedBy) {
          console.log("Status updated by: ", payload?.updatedBy);
          setUpdatedBy(payload.updatedBy);

          // Since we have something to revert, set this to button appears
          dispatch(setProfitEditUpdateRevertChangesAvailable(true));
          // Hide the search filters
          dispatch(setProfitShareEditUpdateShowSearch(false));
        }

        if (payload?.updatedTime) {
          console.log("Status updated time: ", payload?.updatedTime);

          setUpdatedTime(
            new Date(payload.updatedTime).toLocaleString("en-US", {
              month: "long",
              day: "numeric",
              year: "numeric",
              hour: "2-digit",
              minute: "2-digit"
            })
          );
        } else {
          console.log("We did not get a status");
          dispatch(setProfitEditUpdateRevertChangesAvailable(false));
        }
      })
      .catch((error) => {
        console.error("ERROR: Did not revert changes to year end", error);
      });
  }, [profitYear, triggerStatusUpdate, dispatch]);

  useEffect(() => {
    if (hasToken) {
      onStatusSearch();
      if (updatedTime) {
        dispatch(
          setMessage({
            ...Messages.ProfitShareMasterUpdated,
            message: {
              ...Messages.ProfitShareMasterUpdated.message,
              message: `Updated By: ${updatedBy} | Date: ${updatedTime} `
            }
          })
        );
        dispatch(setProfitEditUpdateChangesAvailable(false));
        dispatch(setProfitEditUpdateRevertChangesAvailable(true));
      }
    }
  }, [onStatusSearch, hasToken, updatedTime, updatedBy]);

  return (
    <Page
      label="Master Update (PAY444|PAY447)"
      actionNode={
        <div className="flex  justify-end gap-2">
          {RenderRevertButton(setOpenRevertModal, isLoading)}
          {RenderSaveButton(setOpenSaveModal, setOpenEmptyModal, profitMasterStatus, isLoading)}
        </div>
      }>
      <div>
        <ApiMessageAlert commonKey={MessageKeys.ProfitShareEditUpdate} />
      </div>
      <Grid2
        container
        rowSpacing="24px"
        width={"100%"}>
        <Grid2 width={"100%"}>
          <Divider />
        </Grid2>
        {profitShareEditUpdateShowSearch && (
          <Grid2 width={"100%"}>
            <DSMAccordion title="Parameters">
              <ProfitShareEditUpdateSearchFilter setInitialSearchLoaded={setInitialSearchLoaded} />
            </DSMAccordion>
          </Grid2>
        )}
        {(profitEditUpdateRevertChangesAvailable || profitMasterStatus) && profitEditUpdateRevertChangesAvailable && (
          <>
            <Grid2
              width={"100%"}
              sx={{ marginLeft: "50px" }}>
              <Typography
                component={"span"}
                variant="h6"
                sx={{ fontWeight: "bold" }}>
                {`These changes have already been applied: `}
              </Typography>
            </Grid2>
            <Grid2
              width={"100%"}
              sx={{ marginLeft: "50px" }}>
              {profitSharingEditQueryParams && !profitMasterStatus && (
                <ChangesList params={profitSharingEditQueryParams} />
              )}
              {profitMasterStatus && !profitSharingEditQueryParams && <ChangesList params={profitMasterStatus} />}
            </Grid2>
          </>
        )}
        {profitSharingUpdate && profitSharingEdit && (
          <Grid2 width={"100%"}>
            <div className="px-[24px]">
              <h2 className="text-dsm-secondary">Summary (PAY444)</h2>
              <Typography
                fontWeight="bold"
                variant="body2">
                {`Employees: ${profitSharingUpdate.totals.totalEmployees} | Beneficiaries: ${profitSharingUpdate.totals.totalBeneficaries}`}
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
              tablePadding="12px"
            />
            <div className="px-[24px]">
              <div style={{ display: "flex", gap: "75px" }}>
                <span>
                  <strong>Total Forfeitures</strong>:{" "}
                  {numberToCurrency(profitSharingUpdate.totals.maxOverTotal || 0) + "      "}{" "}
                </span>
                <span>
                  <strong>Total Points</strong>:{" "}
                  {numberToCurrency(profitSharingUpdate.totals.maxPointsTotal || 0) + " "}{" "}
                </span>
                <span>
                  <strong>For Employees Exceeding Max Contribution</strong> :{" "}
                  {numberToCurrency(profitSharingEditQueryParams?.maxAllowedContributions || 0)}
                </span>
              </div>
            </div>
            <div style={{ height: "20px" }}></div>
            <div className="px-[24px]">
              <h2 className="text-dsm-secondary">Summary (PAY447)</h2>
            </div>
            <div style={{ display: "flex", gap: "8px" }}>
              <TotalsGrid
                breakPoints={{ xs: 5, sm: 5, md: 5, lg: 5, xl: 5 }}
                tablePadding="4px"
                displayData={[
                  [
                    numberToCurrency(profitSharingEdit.contributionGrandTotal || 0),
                    numberToCurrency(profitSharingEdit.earningsGrandTotal || 0),
                    numberToCurrency(profitSharingEdit.incomingForfeitureGrandTotal || 0)
                  ]
                ]}
                leftColumnHeaders={["Grand Totals"]}
                topRowHeaders={["", "Contributions", "Earnings", "Forfeit"]}
                headerCellStyle={{}}
              />
              <div style={{ marginTop: "20px" }}>
                <TotalsGrid
                  tablePadding="8px"
                  displayData={[[numberToCurrency(profitSharingEdit.beginningBalanceTotal || 0), "", ""]]}
                  leftColumnHeaders={["Beginning Balance"]}
                  topRowHeaders={["", ""]}
                  headerCellStyle={{}}
                />
              </div>
            </div>

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
                      "", // need the requested contribution adjustment (from the request)
                      "", // need the requested earnings adjustment amount
                      "", // need the requested secondary earnings
                      "" // need the requested incoming forfeiture adjustment
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
          </Grid2>
        )}
      </Grid2>
      <SmartModal
        key={"saveModal"}
        maxWidth="sm"
        open={openSaveModal}
        onClose={() => setOpenSaveModal(false)}>
        <ProfitShareEditConfirmation
          key={"saveConfirmation"}
          performLabel="YES, SAVE"
          closeLabel="NO, CANCEL"
          setOpenModal={setOpenSaveModal}
          actionFunction={() => {
            saveAction();
            setOpenSaveModal(false);
          }}
          messageType="confirmation"
          messageHeadline="You are about to apply the following changes:"
          params={profitSharingEditQueryParams}
          lastWarning="Ready to save? It may take a few minutes to process."
        />
      </SmartModal>
      <SmartModal
        key={"revertModal"}
        maxWidth="sm"
        open={openRevertModal}
        onClose={() => setOpenRevertModal(false)}>
        <ProfitShareEditConfirmation
          key={"revertConfirmation"}
          performLabel="YES, REVERT"
          closeLabel="NO, CANCEL"
          setOpenModal={setOpenRevertModal}
          actionFunction={() => {
            revertAction();
            setOpenRevertModal(false);
          }}
          messageType="warning"
          messageHeadline="Reverting to the last update will modify the following:"
          params={profitSharingEditQueryParams || profitMasterStatus}
          lastWarning="Do you still wish to revert?"
        />
      </SmartModal>

      <SmartModal
        key={"emptyModal"}
        open={openEmptyModal}
        maxWidth="sm"
        onClose={() => setOpenEmptyModal(false)}>
        <ProfitShareEditConfirmation
          key={"emptyConfirmation"}
          performLabel="OK"
          closeLabel=""
          setOpenModal={setOpenEmptyModal}
          actionFunction={() => {}}
          messageType="info"
          messageHeadline="You must fill out  at least one of these: contribution, earnings, or forfeiture."
          params={profitSharingEditQueryParams}
          lastWarning=""
        />
      </SmartModal>
    </Page>
  );
};

export default ProfitShareEditUpdate;
