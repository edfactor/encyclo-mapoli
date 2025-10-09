import { SaveOutlined } from "@mui/icons-material";
import { Button, Divider, Grid, Tooltip } from "@mui/material";
import FrozenYearWarning from "components/FrozenYearWarning";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import StatusReadOnlyInfo from "components/StatusReadOnlyInfo";
import { memo, useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { useIsProfitYearFrozen } from "../../../hooks/useIsProfitYearFrozen";
import { useIsReadOnlyByStatus } from "../../../hooks/useIsReadOnlyByStatus";
import { useReadOnlyNavigation } from "../../../hooks/useReadOnlyNavigation";
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
    saveExecutiveHoursAndDollars,
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
  const [currentStatus, setCurrentStatus] = useState<string | null>(null);

  const handleStatusChange = (newStatus: string, statusName?: string) => {
    if (statusName === "Complete" && currentStatus !== "Complete") {
      setCurrentStatus("Complete");
      saveExecutiveHoursAndDollars();
    } else {
      setCurrentStatus(statusName || newStatus);
    }
  };

  const renderActionNode = () => {
    return <StatusDropdownActionNode onStatusChange={handleStatusChange} />;
  };

  return (
    <Grid
      container
      rowSpacing="24px">
      {isFrozen && <FrozenYearWarning profitYear={profitYear} />}
      {isReadOnlyByStatus && <StatusReadOnlyInfo />}
      <Grid width={"100%"}>
        <Divider />
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
      {showGrid && (
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
          />
        </Grid>
      )}
    </Grid>
  );
});

const ManageExecutiveHoursAndDollars = () => {
  const hookData = useManageExecutiveHoursAndDollars();
  const { hasPendingChanges, saveChanges } = hookData;
  const isReadOnly = useReadOnlyNavigation();

  return (
    <Page
      label={CAPTIONS.MANAGE_EXECUTIVE_HOURS}
      actionNode={
        <RenderSaveButton
          hasPendingChanges={hasPendingChanges}
          onSave={saveChanges}
          isReadOnly={isReadOnly}
        />
      }>
      <ManageExecutiveHoursAndDollarsContent hookData={hookData} />
    </Page>
  );
};

export default ManageExecutiveHoursAndDollars;
