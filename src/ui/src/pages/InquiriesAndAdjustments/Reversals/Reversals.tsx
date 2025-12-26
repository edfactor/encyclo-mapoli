import { Divider, Grid } from "@mui/material";
import { memo } from "react";
import { ApiMessageAlert, DSMAccordion, Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import { CAPTIONS } from "../../../constants";
import useFiscalCloseProfitYear from "../../../hooks/useFiscalCloseProfitYear";
import StandaloneMemberDetails from "../MasterInquiry/StandaloneMemberDetails";
import ReversalsSearchFilter from "./ReversalsSearchFilter";
import ReversalsGrid, { ProfitDetailRow } from "./ReversalsGrid";
import ReversalConfirmationModal from "./ReversalConfirmationModal";
import { useReversals } from "./hooks/useReversals";

const REVERSALS_MESSAGE_KEY = "ReversalOperation";

const ReversalsContent = memo(() => {
  const profitYear = useFiscalCloseProfitYear();

  const {
    // Search state
    isSearching,

    // Member state
    selectedMember,

    // Profit data state
    profitData,
    isFetchingProfitData,

    // Pagination
    pageNumber,
    pageSize,

    // Confirmation modal state
    isConfirmationOpen,
    confirmationItems,
    confirmationIndex,
    isProcessing,

    // Actions
    executeSearch,
    handlePaginationChange,
    initiateReversal,
    confirmReversal,
    cancelReversal,
    resetAll
  } = useReversals();

  const handleReverse = (selectedRows: ProfitDetailRow[]) => {
    initiateReversal(selectedRows);
  };

  return (
    <Grid
      container
      rowSpacing="24px">
      {/* API Message Alert for success/error/warning */}
      <Grid size={{ xs: 12 }}>
        <ApiMessageAlert commonKey={REVERSALS_MESSAGE_KEY} />
      </Grid>

      {/* Divider */}
      <Grid width="100%">
        <Divider />
      </Grid>

      {/* Search Filter */}
      <Grid width="100%">
        <DSMAccordion title="Filter">
          <ReversalsSearchFilter
            onSearch={executeSearch}
            onReset={resetAll}
            isSearching={isSearching}
          />
        </DSMAccordion>
      </Grid>

      {/* Member Details */}
      {selectedMember && (
        <Grid width="100%">
          <StandaloneMemberDetails
            memberType={selectedMember.memberType}
            id={selectedMember.id}
            profitYear={profitYear}
          />
        </Grid>
      )}

      {/* Profit Details Grid */}
      {profitData && (
        <Grid
          width="100%"
          paddingX="24px">
          <ReversalsGrid
            profitData={profitData}
            isLoading={isFetchingProfitData}
            onReverse={handleReverse}
            pageNumber={pageNumber}
            pageSize={pageSize}
            onPaginationChange={handlePaginationChange}
          />
        </Grid>
      )}

      {/* Confirmation Modal */}
      <ReversalConfirmationModal
        open={isConfirmationOpen}
        selectedItems={confirmationItems}
        currentIndex={confirmationIndex}
        onConfirm={confirmReversal}
        onCancel={cancelReversal}
        isProcessing={isProcessing}
      />
    </Grid>
  );
});

ReversalsContent.displayName = "ReversalsContent";

const Reversals = () => {
  return (
    <PageErrorBoundary pageName="Reversals">
      <Page label={CAPTIONS.REVERSALS}>
        <MissiveAlertProvider>
          <ReversalsContent />
        </MissiveAlertProvider>
      </Page>
    </PageErrorBoundary>
  );
};

export default Reversals;
