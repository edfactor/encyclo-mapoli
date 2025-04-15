import { Replay } from "@mui/icons-material";
import { Button, Divider, Tooltip, Typography } from "@mui/material";
import Grid2 from "@mui/material/Grid2";
import { SetStateAction, useCallback, useEffect, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  useLazyGetMasterApplyQuery,
  useLazyGetMasterRevertQuery,
  useLazyGetProfitMasterStatusQuery
} from "reduxstore/api/YearsEndApi";
import {
  setProfitEditUpdateChangesAvailable,
  setProfitEditUpdateRevertChangesAvailable
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
import useFiscalCloseProfitYear from "hooks/useFiscalCloseProfitYear";
import { set } from "date-fns";

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

const useRevertAction = () => {
  const [trigger] = useLazyGetMasterRevertQuery();
  const dispatch = useDispatch();
  const profitYear = useFiscalCloseProfitYear();
  const [beneficiariesAffected, setBeneficiariesAffected] = useState(0);
  const [employeesAffected, setEmployeesAffected] = useState(0);
  const [etvasAffected, setEtvasAffected] = useState(0);

  const revertAction = async (): Promise<void> => {
    const params: ProfitYearRequest = {
      profitYear: profitYear ?? 0
    };

    console.log("reverting cahnges to year end: ", params);
    console.log(params);

    await trigger(params, false)
      .unwrap()
      .then((payload) => {
        setEmployeesAffected(payload?.employeesAffected || 0);
        setBeneficiariesAffected(payload?.beneficiariesAffected || 0);
        setEtvasAffected(payload?.etvasAffected || 0);
        //dispatch(setMessage(successMessage));
        console.log("Successfully reverted changes for year end: ", payload);
        dispatch(setProfitEditUpdateChangesAvailable(false));
        dispatch(setProfitEditUpdateRevertChangesAvailable(false));
      })
      .catch((error) => {
        console.error("ERROR: Did not revert changes to year end", error);
        //dispatch(setMessage(failMessage));
      });
  };
  return revertAction;
};

const useSaveAction = (setEmployeesAffected: (count: number) => void) => {
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

    await trigger(params)
      .unwrap()
      .then((payload: ProfitShareMasterResponse) => {
        //const eF: number = payload?.employeesAffected || 0;

        //setBeneficiariesAffected(payload?.beneficiariesAffected || 0);
        //setEtvasAffected(payload?.etvasAffected || 0);
        dispatch(setProfitEditUpdateChangesAvailable(false));
        dispatch(setProfitEditUpdateRevertChangesAvailable(true));
        console.log("Successfully applied changes to year end: ", payload);
        console.log("Employees affected: ", payload?.employeesAffected);
        setEmployeesAffected(payload?.employeesAffected ?? 0);
      })
      .catch((error) => {
        console.error("ERROR: Did not apply changes to year end", error);
        //dispatch(setMessage(failMessage));
      });
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
  status: ProfitMasterStatus | null
) => {
  // The incoming status field is about whether or not changes have already been applied
  const { profitEditUpdateChangesAvailable, profitSharingEditQueryParams } = useSelector(
    (state: RootState) => state.yearsEnd
  );
  const saveButton = (
    <Button
      disabled={!profitEditUpdateChangesAvailable && status?.updatedTime !== null}
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

// This really just opens the modal. The modal for this has the function to call
// the back end
const RenderRevertButton = (setOpenRevertModal: (open: boolean) => void, status: ProfitMasterStatus | null) => {
  // The incoming status field is about whether or not changes have already been applied
  const { profitEditUpdateRevertChangesAvailable } = useSelector((state: RootState) => state.yearsEnd);

  const revertButton = (
    <Button
      disabled={!profitEditUpdateRevertChangesAvailable || status?.updatedTime === null}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={
        <Replay
          color={profitEditUpdateRevertChangesAvailable || status?.updatedTime === null ? "primary" : "disabled"}
        />
      }
      onClick={async () => {
        setOpenRevertModal(true);
      }}>
      Revert
    </Button>
  );

  if (!profitEditUpdateRevertChangesAvailable || status?.updatedTime === null) {
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
  const [updatedBy, setUpdatedBy] = useState<string | null>(null);
  const [updatedTime, setUpdatedTime] = useState<string | null>(null);

  const revertAction = useRevertAction();
  const saveAction = useSaveAction(setEmployeesAffected);
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const {
    profitSharingUpdateAdjustmentSummary,
    profitSharingUpdate,
    profitSharingEdit,
    profitSharingEditQueryParams,
    profitMasterStatus
  } = useSelector((state: RootState) => state.yearsEnd);
  const [openSaveModal, setOpenSaveModal] = useState<boolean>(false);
  const [openRevertModal, setOpenRevertModal] = useState<boolean>(false);
  const [openEmptyModal, setOpenEmptyModal] = useState<boolean>(false);

  const [triggerStatusUpdate, { isFetching: isLoading }] = useLazyGetProfitMasterStatusQuery();
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
    if (employeesAffected) {
      dispatch(
        setMessage({
          ...Messages.ProfitShareApplySuccess,
          message: {
            ...Messages.ProfitShareApplySuccess.message,
            message: `Employees affected: ${employeesAffected} | Beneficiaries: ${beneficiariesAffected}, | ETVAs: ${etvasAffected} `
          }
        })
      );
    }
  }, [beneficiariesAffected, employeesAffected, etvasAffected]);

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
          {RenderRevertButton(setOpenRevertModal, profitMasterStatus)}
          {RenderSaveButton(setOpenSaveModal, setOpenEmptyModal, profitMasterStatus)}
        </div>
      }>
      <div>
        <ApiMessageAlert commonKey={MessageKeys.ProfitShareEditUpdate} />
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
        {profitSharingUpdate && profitSharingEdit && (
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
              tablePadding="12px"
            />
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
