import { Alert, AlertTitle, Grid, Typography } from "@mui/material";
import { useCallback, useState } from "react";
import { useSelector } from "react-redux";
import { DSMAccordion, numberToCurrency, Page, SmartModal, TotalsGrid } from "smart-ui-library";
import PrerequisiteGuard from "../../../components/PrerequisiteGuard";
import StatusDropdownActionNode from "../../../components/StatusDropdownActionNode";
import { CAPTIONS } from "../../../constants";
import { NavigationStatus, useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
import { RootState } from "../../../reduxstore/store";
import { Messages } from "../../../utils/messageDictonary";
import ChangesList from "./ChangesList";
import { MasterUpdateSummaryTable } from "./MasterUpdateSummaryTable";
import ProfitShareEditConfirmation from "./ProfitShareEditConfirmation";
import ProfitShareEditUpdateSearchFilter from "./ProfitShareEditUpdateSearchFilter";
import ProfitShareEditUpdateTabs from "./ProfitShareEditUpdateTabs";
import ProfitShareRevertButton from "./ProfitShareRevertButton";
import ProfitShareSaveButton from "./ProfitShareSaveButton";
import useProfitShareEditUpdate from "./hooks/useProfitShareEditUpdate";

const ProfitShareEditUpdate = () => {
  // Use custom hook to manage all business logic and state
  const {
    changesApplied,
    openSaveModal,
    openRevertModal,
    openEmptyModal,
    minimumFieldsEntered,
    adjustedBadgeOneValid,
    adjustedBadgeTwoValid,
    updatedBy,
    updatedTime,
    profitEditUpdateRevertChangesAvailable,
    profitShareEditUpdateShowSearch,
    profitSharingEdit,
    profitSharingUpdate,
    profitSharingEditQueryParams,
    profitMasterStatus,
    totalForfeituresGreaterThanZero,
    getFieldValidation,
    saveAction,
    revertAction,
    handleOpenSaveModal,
    handleCloseSaveModal,
    handleOpenRevertModal,
    handleCloseRevertModal,
    handleCloseEmptyModal
  } = useProfitShareEditUpdate();

  // Local state for UI concerns only
  const [initialSearchLoaded, setInitialSearchLoaded] = useState(false);
  const [pageNumberReset, setPageNumberReset] = useState(false);
  const [openValidationField, setOpenValidationField] = useState<string | null>(null);
  // Track current status locally to enable immediate re-evaluation of isReadOnly when status changes
  const [currentStatusId, setCurrentStatusId] = useState<number | null>(null);

  const handleValidationToggle = (fieldName: string) => {
    setOpenValidationField(openValidationField === fieldName ? null : fieldName);
  };

  const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");
  const isReadOnlyByNavigation = useReadOnlyNavigation();
  // Combine navigation-based read-only with status-based read-only for immediate feedback
  // When status changes via dropdown, currentStatusId updates immediately, before Redux refreshes
  const isReadOnlyByCurrentStatus = currentStatusId !== null && currentStatusId !== NavigationStatus.InProgress;
  const isReadOnly = isReadOnlyByNavigation || isReadOnlyByCurrentStatus;
  const { profitSharingUpdateAdjustmentSummary } = useSelector((state: RootState) => state.yearsEnd);

  // Handle status changes from the dropdown to immediately re-evaluate isReadOnly
  const handleStatusChange = useCallback((newStatus: string) => {
    setCurrentStatusId(parseInt(newStatus));
  }, []);

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={handleStatusChange} />;
  };

  return (
    <PrerequisiteGuard
      navigationId={currentNavigationId}
      messageTemplate={Messages.ProfitSharePrerequisiteIncomplete}>
      {({ prerequisitesComplete }) => (
        <Page
          label={`${CAPTIONS.PROFIT_SHARE_UPDATE}`}
          actionNode={
            <div className="flex items-center justify-end gap-2">
              <ProfitShareRevertButton
                setOpenRevertModal={handleOpenRevertModal}
                isLoading={false}
                isReadOnly={isReadOnly}
              />
              <ProfitShareSaveButton
                setOpenSaveModal={handleOpenSaveModal}
                setOpenEmptyModal={handleCloseEmptyModal}
                status={profitMasterStatus}
                isLoading={false}
                minimumFieldsEntered={minimumFieldsEntered}
                adjustedBadgeOneValid={adjustedBadgeOneValid}
                adjustedBadgeTwoValid={adjustedBadgeTwoValid}
                prerequisitesComplete={prerequisitesComplete}
                isReadOnly={isReadOnly}
              />
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
                    {`Employees: ${profitSharingUpdate.profitShareUpdateTotals.totalEmployees} | Beneficiaries: ${profitSharingUpdate.profitShareUpdateTotals.totalBeneficaries}`}
                  </Typography>
                </div>

                {/* Unified Summary Table (PAY444) */}
                <MasterUpdateSummaryTable
                  totals={profitSharingUpdate.profitShareUpdateTotals}
                  getFieldValidation={
                    getFieldValidation as Parameters<typeof MasterUpdateSummaryTable>[0]["getFieldValidation"]
                  }
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
            onClose={handleCloseSaveModal}>
            <ProfitShareEditConfirmation
              key={"saveConfirmation"}
              performLabel="YES, SAVE"
              closeLabel="NO, CANCEL"
              setOpenModal={handleCloseSaveModal}
              actionFunction={async () => {
                await saveAction();
                handleCloseSaveModal();
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
            onClose={handleCloseRevertModal}>
            <ProfitShareEditConfirmation
              key={"revertConfirmation"}
              performLabel="YES, REVERT"
              closeLabel="NO, CANCEL"
              setOpenModal={handleCloseRevertModal}
              actionFunction={async () => {
                await revertAction();
                handleCloseRevertModal();
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
            onClose={handleCloseEmptyModal}>
            <ProfitShareEditConfirmation
              key={"emptyConfirmation"}
              performLabel="OK"
              closeLabel=""
              setOpenModal={handleCloseEmptyModal}
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
