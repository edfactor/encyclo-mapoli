import { SaveOutlined } from "@mui/icons-material";
import { Button, Divider, Grid, Tooltip } from "@mui/material";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { memo, useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import useManageExecutiveHoursAndDollars from "./hooks/useManageExecutiveHoursAndDollars";
import ManageExecutiveHoursAndDollarsGrid from "./ManageExecutiveHoursAndDollarsGrid";
import ManageExecutiveHoursAndDollarsSearchFilter from "./ManageExecutiveHoursAndDollarsSearchFilter";

interface RenderSaveButtonProps {
  hasPendingChanges: boolean;
  onSave: () => void;
}

const RenderSaveButton = memo(({ hasPendingChanges, onSave }: RenderSaveButtonProps) => {
  const saveButton = (
    <Button
      disabled={!hasPendingChanges}
      variant="outlined"
      color="primary"
      size="medium"
      startIcon={<SaveOutlined color={hasPendingChanges ? "primary" : "disabled"} />}
      onClick={onSave}>
      Save
    </Button>
  );

  if (!hasPendingChanges) {
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
});

interface ManageExecutiveHoursAndDollarsContentProps {
  hookData: ReturnType<typeof useManageExecutiveHoursAndDollars>;
}

const ManageExecutiveHoursAndDollarsContent = memo(({ hookData }: ManageExecutiveHoursAndDollarsContentProps) => {
  const {
    executeSearch,
    resetSearch,
    isSearching,
    showGrid,
    archiveExecutiveHoursAndDollars,
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

  const [currentStatus, setCurrentStatus] = useState<string | null>(null);

  const handleStatusChange = (newStatus: string, statusName?: string) => {
    if (statusName === "Complete" && currentStatus !== "Complete") {
      setCurrentStatus("Complete");
      archiveExecutiveHoursAndDollars();
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
          />
        </Grid>
      )}
    </Grid>
  );
});

const ManageExecutiveHoursAndDollars = () => {
  const hookData = useManageExecutiveHoursAndDollars();
  const { hasPendingChanges, saveChanges } = hookData;

  return (
    <Page
      label={CAPTIONS.MANAGE_EXECUTIVE_HOURS}
      actionNode={
        <RenderSaveButton
          hasPendingChanges={hasPendingChanges}
          onSave={saveChanges}
        />
      }>
      <ManageExecutiveHoursAndDollarsContent hookData={hookData} />
    </Page>
  );
};

export default ManageExecutiveHoursAndDollars;
