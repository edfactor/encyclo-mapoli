import { Replay } from "@mui/icons-material";
import { Alert, AlertTitle, Button, CircularProgress, Grid, Tooltip, Typography } from "@mui/material";
import { useCallback, useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { DSMAccordion, numberToCurrency, Page, setMessage, SmartModal, TotalsGrid } from "smart-ui-library";
import StatusDropdownActionNode from "../../components/StatusDropdownActionNode";
import { useChecksumValidation } from "../../hooks/useChecksumValidation";
import useFiscalCloseProfitYear from "../../hooks/useFiscalCloseProfitYear";
import { useReadOnlyNavigation } from "../../hooks/useReadOnlyNavigation";
import {
  useGetMasterApplyMutation,
  useLazyGetMasterRevertQuery,
  useLazyGetProfitMasterStatusQuery
} from "../../reduxstore/api/YearsEndApi";
import {
  clearProfitSharingEdit,
  clearProfitSharingEditQueryParams,
  clearProfitSharingUpdate,
  setInvalidProfitShareEditYear,
  setProfitEditUpdateChangesAvailable,
  setProfitEditUpdateRevertChangesAvailable,
  setProfitShareApplyOrRevertLoading,
  setProfitShareEditUpdateShowSearch,
  setResetYearEndPage
} from "../../reduxstore/slices/yearsEndSlice";
import { RootState } from "../../reduxstore/store";
import {
  ProfitMasterStatus,
  ProfitShareEditUpdateQueryParams,
  ProfitShareMasterApplyRequest,
  ProfitShareMasterResponse,
  ProfitYearRequest
} from "../../reduxstore/types";
// usePrerequisiteNavigations now encapsulated by PrerequisiteGuard
import PrerequisiteGuard from "../../components/PrerequisiteGuard";
import { CAPTIONS } from "../../constants";
import { MessageKeys, Messages } from "../../utils/messageDictonary";
import ChangesList from "./ChangesList";
import { MasterUpdateSummaryTable } from "./MasterUpdateSummaryTable";
import ProfitShareEditConfirmation from "./ProfitShareEditConfirmation";
import ProfitShareEditUpdateSearchFilter from "./ProfitShareEditUpdateSearchFilter";
import ProfitShareEditUpdateTabs from "./ProfitShareEditUpdateTabs";

const useRevertAction = (
  //setEmployeesReverted: (count: number) => void,
  //setBeneficiariesReverted: (count: number) => void,
  //setEtvasReverted: (count: number) => void,
  setChangesApplied: (changes: boolean) => void
) => {
  const [trigger] = useLazyGetMasterRevertQuery();
  const dispatch = useDispatch();
  const profitYear = useFiscalCloseProfitYear();
  //const { profitSharingEdit, profitSharingUpdate } = useSelector((state: RootState) => state.yearsEnd);

  const revertAction = async (): Promise<void> => {
    const params: ProfitYearRequest = {
      profitYear: profitYear ?? 0
    };

    await trigger(params, false)
      .unwrap()
      .then((payload) => {
        //setEmployeesReverted(payload?.employeesEffected || 0);
        //setBeneficiariesReverted(payload?.beneficiariesEffected || 0);
        //setEtvasReverted(payload?.etvasEffected || 0);
        //dispatch(setMessage(successMessage));
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
        // Set the changes applied to false
        setChangesApplied(false);
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
        //setEmployeesReverted(0);
        //setBeneficiariesReverted(0);
        //setEtvasReverted(0);
      });
  };
  return revertAction;
};

const useSaveAction = () =>
  //setEmployeesReverted: (count: number) => void,
  //setBeneficiariesReverted: (count: number) => void,
  //setEtvasReverted: (count: number) => void
  // NOTE: Removed setValidationResponse parameter - now using useChecksumValidation hook
  {
    const { profitSharingEditQueryParams } = useSelector((state: RootState) => state.yearsEnd);
    const [applyMaster] = useGetMasterApplyMutation();
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

      dispatch(setProfitShareApplyOrRevertLoading(true));

      await applyMaster(params)
        .unwrap()
        .then((payload: ProfitShareMasterResponse) => {
          dispatch(setProfitEditUpdateChangesAvailable(false));

          console.log("Successfully applied changes to year end: ", payload);
          console.log("Employees affected: ", payload?.employeesEffected);

          // NOTE: Removed setValidationResponse call - useChecksumValidation hook handles this
          // Cross-reference validation will auto-refresh via the hook after save
          if (payload.crossReferenceValidation) {
            console.log("Cross-reference validation:", payload.crossReferenceValidation);
          }

          //setEmployeesReverted(payload?.employeesEffected ?? 0);
          //setBeneficiariesReverted(payload?.beneficiariesEffected ?? 0);
          //setEtvasReverted(payload?.etvasEffected ?? 0);
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
    (profitSharingEditQueryParams?.maxAllowedContributions ?? 0) > 0 ||
    (profitSharingEditQueryParams?.badgeToAdjust ?? 0) > 0 ||
    (profitSharingEditQueryParams?.adjustContributionAmount ?? 0) > 0 ||
    (profitSharingEditQueryParams?.adjustEarningsAmount ?? 0) > 0 ||
    (profitSharingEditQueryParams?.adjustIncomingForfeitAmount ?? 0) > 0 ||
    (profitSharingEditQueryParams?.badgeToAdjust2 ?? 0) > 0 ||
    (profitSharingEditQueryParams?.adjustEarningsSecondaryAmount ?? 0) > 0
  );
};

// This really just opens the modal. The modal for this has the function to call
// the back end
const RenderSaveButton = (
  setOpenSaveModal: (open: boolean) => void,
  setOpenEmptyModal: (open: boolean) => void,
  status: ProfitMasterStatus | null,
  isLoading: boolean,
  minimumFieldsEntered: boolean = false,
  adjustedBadgeOneValid: boolean = true,
  adjustedBadgeTwoValid: boolean = true,
  prerequisitesComplete: boolean = true,
  isReadOnly: boolean = false
) => {
  // The incoming status field is about whether or not changes have already been applied
  const {
    profitEditUpdateChangesAvailable,
    profitSharingEditQueryParams,
    profitShareApplyOrRevertLoading,
    totalForfeituresGreaterThanZero,
    invalidProfitShareEditYear
  } = useSelector((state: RootState) => state.yearsEnd);
  //const navigationList = useSelector((state: RootState) => state.navigation.navigationData);
  //const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");
  // Determine tooltip reason when disabled by prerequisites
  const prereqTooltip = !prerequisitesComplete
    ? "All prerequisite navigations must be complete before saving."
    : undefined;
  const saveButton = (
    <Button
      disabled={
        (!profitEditUpdateChangesAvailable && status?.updatedTime !== null) ||
        isLoading ||
        totalForfeituresGreaterThanZero ||
        invalidProfitShareEditYear ||
        !prerequisitesComplete ||
        isReadOnly
      }
      variant="outlined"
      color="primary"
      size="medium"
      onClick={
        isReadOnly
          ? undefined
          : async () => {
              if (
                profitSharingEditQueryParams &&
                wasFormUsed(profitSharingEditQueryParams) &&
                adjustedBadgeOneValid &&
                adjustedBadgeTwoValid &&
                minimumFieldsEntered &&
                prerequisitesComplete
              ) {
                setOpenSaveModal(true);
              } else {
                setOpenEmptyModal(true);
              }
            }
      }>
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

  if (
    !profitEditUpdateChangesAvailable ||
    invalidProfitShareEditYear ||
    totalForfeituresGreaterThanZero ||
    !prerequisitesComplete ||
    isReadOnly
  ) {
    return (
      <Tooltip
        placement="top"
        title={
          isReadOnly
            ? "You are in read-only mode and cannot apply changes."
            : invalidProfitShareEditYear
              ? "Invalid year for saving changes"
              : totalForfeituresGreaterThanZero == true
                ? "Total forfeitures is greater than zero."
                : !prerequisitesComplete
                  ? prereqTooltip
                  : "You must have previewed data before saving."
        }>
        <span>{saveButton}</span>
      </Tooltip>
    );
  } else {
    return saveButton;
  }
};

// This really just opens the modal. The modal for this has the function to call
// the back end
const RenderRevertButton = (
  setOpenRevertModal: (open: boolean) => void,
  isLoading: boolean,
  isReadOnly: boolean = false
) => {
  // The incoming status field is about whether or not changes have already been applied
  const { profitEditUpdateRevertChangesAvailable, profitShareApplyOrRevertLoading } = useSelector(
    (state: RootState) => state.yearsEnd
  );

  const revertButton = (
    <Button
      disabled={!profitEditUpdateRevertChangesAvailable || isLoading || isReadOnly}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={
        isLoading ? null : (
          <Replay color={profitEditUpdateRevertChangesAvailable && !isReadOnly ? "primary" : "disabled"} />
        )
      }
      onClick={
        isReadOnly
          ? undefined
          : async () => {
              setOpenRevertModal(true);
            }
      }>
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

  if (!profitEditUpdateRevertChangesAvailable || isReadOnly) {
    return (
      <Tooltip
        placement="top"
        title={
          isReadOnly ? "You are in read-only mode and cannot revert changes." : "You must have applied data to revert."
        }>
        <span>{revertButton}</span>
      </Tooltip>
    );
  } else {
    return revertButton;
  }
};

const ProfitShareEditUpdate = () => {
  //const [beneficiariesAffected, setBeneficiariesAffected] = useState(0);
  //const [employeesAffected, setEmployeesAffected] = useState(0);
  //const [etvasAffected, setEtvasAffected] = useState(0);
  //const [beneficiariesReverted, setBeneficiariesReverted] = useState(0);
  //const [employeesReverted, setEmployeesReverted] = useState(0);
  //const [etvasReverted, setEtvasReverted] = useState(0);
  const [updatedBy, setUpdatedBy] = useState<string | null>(null);
  const [updatedTime, setUpdatedTime] = useState<string | null>(null);

  // State for validation popup - track which field popup is open
  const [openValidationField, setOpenValidationField] = useState<string | null>(null);

  const handleValidationToggle = (fieldName: string) => {
    setOpenValidationField(openValidationField === fieldName ? null : fieldName);
  };

  // Helper to render validation icon with popup for a specific field (legacy - replaced by renderValidationIconInGrid)

  // This is a flag used to indicate that the year end change have been made
  // and a banner should be shown indicating this
  const [changesApplied, setChangesApplied] = useState<boolean>(false);

  const revertAction = useRevertAction(
    //setEmployeesReverted,
    // setBeneficiariesReverted,
    // setEtvasReverted,
    setChangesApplied
  );
  const saveAction = useSaveAction();
  //setEmployeesAffected,
  //setBeneficiariesAffected,
  //setEtvasAffected
  // NOTE: Removed setValidationResponse - now using useChecksumValidation hook
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const hasToken = !!useSelector((state: RootState) => state.security.token);

  // These are to track validity of fields in the search filter
  const [minimumFieldsEntered, setMinimumFieldsEntered] = useState(false);
  const [adjustedBadgeOneValid, setAdjustedBadgeOneValid] = useState(true);
  const [adjustedBadgeTwoValid, setAdjustedBadgeTwoValid] = useState(true);

  const {
    profitSharingUpdateAdjustmentSummary,
    profitSharingUpdate,
    profitSharingEdit,
    profitSharingEditQueryParams,
    profitMasterStatus,
    profitShareEditUpdateShowSearch,
    profitEditUpdateRevertChangesAvailable,
    totalForfeituresGreaterThanZero
  } = useSelector((state: RootState) => state.yearsEnd);
  const [openSaveModal, setOpenSaveModal] = useState<boolean>(false);
  const [openRevertModal, setOpenRevertModal] = useState<boolean>(false);
  const [openEmptyModal, setOpenEmptyModal] = useState<boolean>(false);

  const [triggerStatusUpdate, { isLoading }] = useLazyGetProfitMasterStatusQuery();

  const profitYear = useFiscalCloseProfitYear();
  const dispatch = useDispatch();
  const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");
  const isReadOnly = useReadOnlyNavigation();

  // Use checksum validation hook to fetch validation data independently from any page
  const {
    validationData: validationResponse,
    //isLoading: isValidationLoading,
    //error: validationError,
    //refetch: refetchValidation,
    getFieldValidation
  } = useChecksumValidation({
    profitYear: profitYear || 0,
    autoFetch: true,
    // Pass current values from PAY444 for client-side comparison with PAY443 archived values
    currentValues: profitSharingUpdate?.profitShareUpdateTotals
      ? {
          TotalProfitSharingBalance: profitSharingUpdate.profitShareUpdateTotals.beginningBalance,
          DistributionTotals: profitSharingUpdate.profitShareUpdateTotals.distributions,
          ForfeitureTotals: profitSharingUpdate.profitShareUpdateTotals.forfeiture,
          ContributionTotals: profitSharingUpdate.profitShareUpdateTotals.totalContribution,
          EarningsTotals: profitSharingUpdate.profitShareUpdateTotals.earnings,
          IncomingAllocations: profitSharingUpdate.profitShareUpdateTotals.allocations,
          OutgoingAllocations: profitSharingUpdate.profitShareUpdateTotals.paidAllocations,
          // NetAllocTransfer is calculated field: allocations + paidAllocations
          // Note: paidAllocations is already stored as a NEGATIVE value in the database
          NetAllocTransfer:
            (profitSharingUpdate.profitShareUpdateTotals.allocations || 0) +
            (profitSharingUpdate.profitShareUpdateTotals.paidAllocations || 0)
        }
      : undefined
  });

  // Helper to render validation icon positioned absolutely in a TotalsGrid (like State Taxes pattern)
  // IMPORTANT: Must be declared AFTER getFieldValidation helper

  // Extract cross-reference validation from profitSharingUpdate response
  // NOTE: This useEffect is now DISABLED because we're using the useChecksumValidation hook
  // which auto-fetches validation data independently based on profitYear.
  // Keeping this commented for reference in case we need to revert.
  /*
  useEffect(() => {
    if (profitSharingUpdate?.crossReferenceValidation) {
      setValidationResponse(profitSharingUpdate.crossReferenceValidation);
      console.log("Loaded cross-reference validation from GET response:", profitSharingUpdate.crossReferenceValidation);
    }
  }, [profitSharingUpdate]);
  */

  useEffect(() => {
    const currentYear = new Date().getFullYear();
    if (profitYear !== currentYear - 1) {
      dispatch(setInvalidProfitShareEditYear(true));
      dispatch(
        setMessage({
          key: MessageKeys.ProfitShareEditUpdate,
          message: {
            type: "warning",
            title: "Invalid Year Selected",
            message: `Please select a ${currentYear - 1} date in the drawer menu to proceed.`
          }
        })
      );
    } else {
      dispatch(setInvalidProfitShareEditYear(false));
    }
  }, [profitYear, dispatch]);

  const onStatusSearch = useCallback(async () => {
    const request: ProfitYearRequest = {
      profitYear: profitYear ?? 0
    };

    await triggerStatusUpdate(request, false)
      .unwrap()
      .then((payload) => {
        if (payload?.updatedBy) {
          setUpdatedBy(payload.updatedBy);

          // Since we have something to revert, set this to button appears
          dispatch(setProfitEditUpdateRevertChangesAvailable(true));
          // Hide the search filters
          dispatch(setProfitShareEditUpdateShowSearch(false));
        }

        if (payload?.updatedTime) {
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
          dispatch(setProfitEditUpdateRevertChangesAvailable(false));
        }
      })
      .catch((error) => {
        console.error("ERROR: Did not revert changes to year end", error);
      });
  }, [profitYear, triggerStatusUpdate, dispatch]);
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  useEffect(() => {
    if (hasToken) {
      onStatusSearch();
      if (updatedTime) {
        setChangesApplied(true);
        dispatch(setProfitEditUpdateChangesAvailable(false));
        dispatch(setProfitEditUpdateRevertChangesAvailable(true));
      } else {
        setChangesApplied(false);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [onStatusSearch, hasToken, updatedTime, updatedBy]);

  return (
    <PrerequisiteGuard
      navigationId={currentNavigationId}
      messageTemplate={Messages.ProfitSharePrerequisiteIncomplete}>
      {({ prerequisitesComplete }) => (
        <Page
          label={`${CAPTIONS.PROFIT_SHARE_UPDATE}`}
          actionNode={
            <div className="flex items-center justify-end gap-2">
              {RenderRevertButton(setOpenRevertModal, isLoading, isReadOnly)}
              {RenderSaveButton(
                setOpenSaveModal,
                setOpenEmptyModal,
                profitMasterStatus,
                isLoading,
                minimumFieldsEntered,
                adjustedBadgeOneValid,
                adjustedBadgeTwoValid,
                prerequisitesComplete,
                isReadOnly
              )}
              {renderActionNode()}
            </div>
          }>
          {
            // We are using an AlertTitle directly and not a missive because we want this alert message
            // to remain in place, not fade away
            changesApplied && (
              <div className="w-full py-3">
                <Alert severity={Messages.ProfitShareMasterUpdated.message.type}>
                  <AlertTitle sx={{ fontWeight: "bold" }}>{Messages.ProfitShareMasterUpdated.message.title}</AlertTitle>
                  {`Updated By: ${updatedBy} | Date: ${updatedTime} `}
                </Alert>
              </div>
            )
          }

          <Grid
            container
            rowSpacing="24px"
            width={"100%"}>
            <Grid
              width={"100%"}
              hidden={!profitShareEditUpdateShowSearch}>
              <DSMAccordion title="Filter">
                <ProfitShareEditUpdateSearchFilter
                  setInitialSearchLoaded={setInitialSearchLoaded}
                  setPageReset={setPageNumberReset}
                  setMinimumFieldsEntered={setMinimumFieldsEntered}
                  setAdjustedBadgeOneValid={setAdjustedBadgeOneValid}
                  setAdjustedBadgeTwoValid={setAdjustedBadgeTwoValid}
                />
              </DSMAccordion>
            </Grid>
            {profitEditUpdateRevertChangesAvailable && (
              <>
                <Grid
                  width="100%"
                  sx={{ marginLeft: "50px" }}>
                  <Typography
                    component="span"
                    variant="h6"
                    sx={{ fontWeight: "bold" }}>
                    These changes have already been applied:
                  </Typography>
                </Grid>
                <Grid
                  width="100%"
                  sx={{ marginLeft: "50px" }}>
                  <ChangesList params={profitSharingEditQueryParams || profitMasterStatus} />
                </Grid>
              </>
            )}
            {profitSharingUpdate && profitSharingUpdate.profitShareUpdateTotals && profitSharingEdit && (
              <Grid width={"100%"}>
                <div className="px-[24px]">
                  <h2 className="text-dsm-secondary">Summary (PAY444)</h2>
                  <Typography
                    fontWeight="bold"
                    variant="body2">
                    {`Employees: ${profitSharingUpdate.profitShareUpdateTotals.totalEmployees} | Beneficiaries: ${profitSharingUpdate.profitShareUpdateTotals.totalBeneficiaries}`}
                  </Typography>
                </div>

                {/* Unified Summary Table (PAY444) */}
                <MasterUpdateSummaryTable
                  totals={profitSharingUpdate.profitShareUpdateTotals}
                  validationResponse={validationResponse}
                  getFieldValidation={getFieldValidation}
                  openValidationField={openValidationField}
                  onValidationToggle={handleValidationToggle}
                />

                <TotalsGrid
                  tablePadding="12px"
                  displayData={[
                    [
                      numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.maxOverTotal || 0),
                      numberToCurrency(profitSharingUpdate.profitShareUpdateTotals.maxPointsTotal || 0),
                      numberToCurrency(profitSharingEditQueryParams?.maxAllowedContributions || 0)
                    ]
                  ]}
                  leftColumnHeaders={[]}
                  topRowHeaders={["Total Forfeitures", "Total Points", "For Employees Exceeding Max Contribution"]}
                />
                {totalForfeituresGreaterThanZero && (
                  <div className="-mt-2 px-[24px] text-sm text-red-600">
                    <em>
                      * Total Forfeitures value highlighted in red indicates an issue that must be resolved before
                      saving.
                    </em>
                  </div>
                )}
                <div className="h-5" />
                <div className="px-[24px]">
                  <h2 className="text-dsm-secondary">Summary (PAY447)</h2>
                </div>
                <div className="flex gap-2">
                  <TotalsGrid
                    breakpoints={{ xs: 5, sm: 5, md: 5, lg: 5, xl: 5 }}
                    tablePadding="4px"
                    displayData={[
                      [
                        numberToCurrency(profitSharingEdit.beginningBalanceTotal || 0),
                        numberToCurrency(profitSharingEdit.contributionGrandTotal || 0),
                        numberToCurrency(profitSharingEdit.earningsGrandTotal || 0),
                        numberToCurrency(profitSharingEdit.incomingForfeitureGrandTotal || 0)
                      ]
                    ]}
                    leftColumnHeaders={["Grand Totals"]}
                    topRowHeaders={["", "Beginning Balance", "Contributions", "Earnings", "Forfeit"]}
                  />
                </div>
                <br />
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
                          numberToCurrency(
                            profitSharingUpdateAdjustmentSummary?.secondaryEarningsAmountUnadjusted || 0
                          ),
                          numberToCurrency(
                            profitSharingUpdateAdjustmentSummary?.incomingForfeitureAmountUnadjusted || 0
                          )
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
                <Grid width="100%">
                  <ProfitShareEditUpdateTabs
                    initialSearchLoaded={initialSearchLoaded}
                    setInitialSearchLoaded={setInitialSearchLoaded}
                    pageNumberReset={pageNumberReset}
                    setPageNumberReset={setPageNumberReset}
                  />
                </Grid>
              </Grid>
            )}
          </Grid>
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
              messageHeadline={
                !minimumFieldsEntered
                  ? "You must enter at least contribution, earnings, and max allowed contributions."
                  : !adjustedBadgeOneValid
                    ? "If you adjust a badge, you must also enter the contribution, earnings, and incoming forfeiture."
                    : !adjustedBadgeTwoValid
                      ? "If you adjust a secondary badge, you must also enter the earnings amount."
                      : ""
              }
              params={profitSharingEditQueryParams}
              lastWarning=""
            />
          </SmartModal>
        </Page>
      )}
    </PrerequisiteGuard>
  );
};

export default ProfitShareEditUpdate;
