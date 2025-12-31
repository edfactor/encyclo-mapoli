import { Divider, Grid } from "@mui/material";
import StandaloneMemberDetails from "pages/InquiriesAndAdjustments/MasterInquiry/StandaloneMemberDetails";
import { memo } from "react";
import { ApiMessageAlert, DSMAccordion, Page } from "smart-ui-library";
import { UnsavedChangesDialog } from "../../../components/ConfirmationDialog";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import { CAPTIONS } from "../../../constants";
import { useUnsavedChangesGuard } from "../../../hooks/useUnsavedChangesGuard";
import ProfitSharingAdjustmentModal from "./ProfitSharingAdjustmentModal";
import ProfitSharingAdjustmentsGrid from "./ProfitSharingAdjustmentsGrid";
import ProfitSharingAdjustmentsSearchFilter from "./ProfitSharingAdjustmentsSearchFilter";
import { useProfitSharingAdjustments } from "./hooks/useProfitSharingAdjustments";

const ADJUSTMENTS_MESSAGE_KEY = "ProfitSharingAdjustmentsOperation";

const ProfitSharingAdjustmentsContent = memo(() => {
  const {
    // Search state
    isSearching,
    searchParams,

    // Adjustments data state
    loadedKey,
    demographicId,
    rowData,
    isFetching,
    isSaving,
    hasUnsavedChanges,

    // Selection state
    selectedRow,

    // Adjustment modal state
    isAdjustModalOpen,
    adjustmentDraft,

    // Member details refresh
    memberDetailsRefreshTrigger,

    // Actions
    executeSearch,
    onCellValueChanged,
    discardChanges,
    handleRowSelection,
    clearSelection,
    openAdjustModal,
    closeAdjustModal,
    updateAdjustmentDraft,
    applyAdjustmentDraft,
    saveChanges,
    resetAll
  } = useProfitSharingAdjustments();

  const { showDialog: showUnsavedDialog, onStay, onLeave } = useUnsavedChangesGuard(hasUnsavedChanges, true);

  return (
    <Grid
      container
      rowSpacing="24px">
      {/* API Message Alert for success/error/warning */}
      <Grid size={{ xs: 12 }}>
        <ApiMessageAlert commonKey={ADJUSTMENTS_MESSAGE_KEY} />
      </Grid>

      {/* Divider */}
      <Grid width="100%">
        <Divider />
      </Grid>

      {/* Search Filter */}
      <Grid width="100%">
        <DSMAccordion title="Filter">
          <ProfitSharingAdjustmentsSearchFilter
            onSearch={executeSearch}
            onReset={resetAll}
            isSearching={isSearching || isFetching}
            hasUnsavedChanges={hasUnsavedChanges}
          />
        </DSMAccordion>
      </Grid>

      {/* Member Details */}
      {demographicId != null && loadedKey && (
        <Grid width="100%">
          <StandaloneMemberDetails
            memberType={1}
            id={demographicId}
            profitYear={loadedKey.profitYear}
            refreshTrigger={memberDetailsRefreshTrigger}
          />
        </Grid>
      )}

      {/* Profit Adjustments Grid */}
      {loadedKey && (
        <Grid
          width="100%"
          paddingX="24px">
          <ProfitSharingAdjustmentsGrid
            rowData={rowData}
            isLoading={isFetching || isSaving}
            selectedRow={selectedRow}
            hasUnsavedChanges={hasUnsavedChanges}
            onRowSelection={handleRowSelection}
            onClearSelection={clearSelection}
            onAdjust={openAdjustModal}
            onSave={saveChanges}
            onDiscard={discardChanges}
            onCellValueChanged={onCellValueChanged}
          />
        </Grid>
      )}

      {/* Adjustment Modal */}
      <ProfitSharingAdjustmentModal
        open={isAdjustModalOpen}
        draft={adjustmentDraft}
        onUpdateDraft={updateAdjustmentDraft}
        onApply={applyAdjustmentDraft}
        onCancel={closeAdjustModal}
      />

      {/* Unsaved changes navigation guard dialog */}
      <UnsavedChangesDialog
        open={showUnsavedDialog}
        onStay={onStay}
        onLeave={onLeave}
      />
    </Grid>
  );
});

ProfitSharingAdjustmentsContent.displayName = "ProfitSharingAdjustmentsContent";

const ProfitSharingAdjustments = () => {
  return (
    <PageErrorBoundary pageName="Profit Sharing Adjustments">
      <Page label={CAPTIONS.PROFIT_SHARING_ADJUSTMENTS}>
        <MissiveAlertProvider>
          <ProfitSharingAdjustmentsContent />
        </MissiveAlertProvider>
      </Page>
    </PageErrorBoundary>
  );
};

export default ProfitSharingAdjustments;
