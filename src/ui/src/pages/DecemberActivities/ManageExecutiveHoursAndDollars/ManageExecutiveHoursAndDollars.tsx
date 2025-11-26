import { SaveOutlined } from "@mui/icons-material";
import { Box, Button, CircularProgress, Divider, Grid, Tooltip } from "@mui/material";
import FrozenYearWarning from "components/FrozenYearWarning";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import StatusReadOnlyInfo from "components/StatusReadOnlyInfo";
import { memo, useCallback } from "react";
import { useSelector } from "react-redux";
import { DSMAccordion, Page } from "smart-ui-library";
import MissiveAlerts from "../../../components/MissiveAlerts/MissiveAlerts";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import { EXECUTIVE_HOURS_AND_DOLLARS_MESSAGES } from "../../../components/MissiveAlerts/MissiveMessages";
import { CAPTIONS } from "../../../constants";
import { useIsProfitYearFrozen } from "../../../hooks/useIsProfitYearFrozen";
import { useIsReadOnlyByStatus } from "../../../hooks/useIsReadOnlyByStatus";
import { useMissiveAlerts } from "../../../hooks/useMissiveAlerts";
import { useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
import { RootState } from "../../../reduxstore/store";
import { ImpersonationRoles } from "../../../types/common/enums";
import useManageExecutiveHoursAndDollars from "./hooks/useManageExecutiveHoursAndDollars";
import ManageExecutiveHoursAndDollarsGrid from "./ManageExecutiveHoursAndDollarsGrid";
import ManageExecutiveHoursAndDollarsSearchFilter from "./ManageExecutiveHoursAndDollarsSearchFilter";

interface RenderSaveButtonProps {
  hasPendingChanges: boolean;
  onSave: () => void;
  isReadOnly?: boolean;
}

const RenderSaveButton = memo(({ hasPendingChanges, onSave, isReadOnly = true }: RenderSaveButtonProps) => {
  const isDisabled = !hasPendingChanges || isReadOnly;
  const readOnlyTooltip = "You are in read-only mode and cannot save changes.";
  const noPendingChangesTooltip = "You must change hours or dollars to save.";

  const saveButton = (
    <Button
      disabled={isDisabled}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<SaveOutlined color={isDisabled ? "disabled" : "primary"} />}
      onClick={isReadOnly ? undefined : onSave}>
      Save
    </Button>
  );

  const tooltipTitle = isReadOnly ? readOnlyTooltip : noPendingChangesTooltip;

  if (isDisabled) {
    return (
      <Tooltip
        placement="top"
        title={tooltipTitle}>
        <span>{saveButton}</span>
      </Tooltip>
    );
  } else {
    return saveButton;
  }
});
interface ManageExecutiveHoursAndDollarsContentProps {
  hookData: ReturnType<typeof useManageExecutiveHoursAndDollars>;
}

const ManageExecutiveHoursAndDollarsContent = memo(({ hookData }: ManageExecutiveHoursAndDollarsContentProps) => {
  const {
    profitYear,
    executeSearch,
    resetSearch,
    isSearching,
    showGrid,

    gridData,
    modalResults,
    isModalOpen,
    openModal,
    closeModal,
    selectExecutivesInModal,
    updateExecutiveRow,
    isRowStagedToSave,
    mainGridPagination,
    modalGridPagination,
    executeModalSearch,
    modalSelectedExecutives,
    addExecutivesToMainGrid,
    isModalSearching
  } = hookData;

  const isReadOnly = useReadOnlyNavigation();
  const isReadOnlyByStatus = useIsReadOnlyByStatus();
  const isFrozen = useIsProfitYearFrozen(profitYear);

  // Check if user has ExecutiveAdministrator role
  const impersonatingRoles = useSelector((state: RootState) => state.security.impersonating);
  const hasExecutiveAdminRole = impersonatingRoles.includes(ImpersonationRoles.ExecutiveAdministrator);

  // Editing is only allowed when:
  // 1. Page status is "In Progress" (not read-only by status)
  // 2. User has ExecutiveAdministrator role
  // 3. Year is not frozen
  // 4. Not in general read-only mode
  const canEdit = !isReadOnlyByStatus && hasExecutiveAdminRole && !isFrozen && !isReadOnly;
  //const [currentStatus, setCurrentStatus] = useState<string | null>(null);

  /*
  const handleStatusChange = (newStatus: string, statusName?: string) => {
    if (statusName === "Complete" && currentStatus !== "Complete") {
      setCurrentStatus("Complete");
      saveExecutiveHoursAndDollars();
    } else {
      setCurrentStatus(statusName || newStatus);
    }
  };
  */

  /*
  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={handleStatusChange} />;
  };
  */
  return (
    <Grid
      container
      rowSpacing="24px">
      {isFrozen && <FrozenYearWarning profitYear={profitYear} />}
      {isReadOnlyByStatus && <StatusReadOnlyInfo />}
      <Grid width={"100%"}>
        <Divider />
      </Grid>
      <Grid
        size={{ xs: 12 }}
        width={"100%"}>
        <MissiveAlerts />
      </Grid>
      <Grid width={"100%"}>
        <DSMAccordion title="Filter">
          <ManageExecutiveHoursAndDollarsSearchFilter
            onSearch={executeSearch}
            onReset={resetSearch}
            isSearching={isSearching}
          />
        </DSMAccordion>
      </Grid>

      {isSearching && (
        <Grid
          size={{ xs: 12 }}
          sx={{ display: "flex", justifyContent: "center", padding: "24px" }}>
          <CircularProgress />
        </Grid>
      )}

      {!isSearching && showGrid && (
        <Grid width="100%">
          <ManageExecutiveHoursAndDollarsGrid
            gridData={gridData}
            modalResults={modalResults}
            isSearching={isSearching}
            isModalOpen={isModalOpen}
            openModal={openModal}
            closeModal={closeModal}
            selectExecutivesInModal={selectExecutivesInModal}
            updateExecutiveRow={updateExecutiveRow}
            isRowStagedToSave={isRowStagedToSave}
            mainGridPagination={mainGridPagination}
            modalGridPagination={modalGridPagination}
            executeModalSearch={executeModalSearch}
            modalSelectedExecutives={modalSelectedExecutives}
            addExecutivesToMainGrid={addExecutivesToMainGrid}
            isModalSearching={isModalSearching}
            isReadOnly={isReadOnly}
            canEdit={canEdit}
          />
        </Grid>
      )}
    </Grid>
  );
});

const ManageExecutiveHoursAndDollarsInner = () => {
  const { addAlert, clearAlerts } = useMissiveAlerts();
  const hookData = useManageExecutiveHoursAndDollars({ addAlert, clearAlerts });
  const { hasPendingChanges, saveChanges } = hookData;
  const isReadOnly = useReadOnlyNavigation();

  const handleSave = useCallback(async () => {
    try {
      await saveChanges();
      addAlert(EXECUTIVE_HOURS_AND_DOLLARS_MESSAGES.EXECUTIVE_HOURS_SAVED_SUCCESS);
    } catch (_error) {
      addAlert(EXECUTIVE_HOURS_AND_DOLLARS_MESSAGES.EXECUTIVE_HOURS_SAVE_ERROR);
    }
  }, [saveChanges, addAlert]);

  const renderActionNode = () => {
    return (
      <Box sx={{ display: "flex", gap: 2 }}>
        <RenderSaveButton
          hasPendingChanges={hasPendingChanges}
          onSave={handleSave}
          isReadOnly={isReadOnly}
        />
        <StatusDropdownActionNode />
      </Box>
    );
  };

  return (
    <Page
      label={CAPTIONS.MANAGE_EXECUTIVE_HOURS}
      actionNode={renderActionNode()}>
      <ManageExecutiveHoursAndDollarsContent hookData={hookData} />
    </Page>
  );
};

const ManageExecutiveHoursAndDollars = () => {
  return (
    <MissiveAlertProvider>
      <ManageExecutiveHoursAndDollarsInner />
    </MissiveAlertProvider>
  );
};

export default ManageExecutiveHoursAndDollars;
