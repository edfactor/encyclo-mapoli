import { Divider, Grid } from "@mui/material";
import PageErrorBoundary from "components/PageErrorBoundary";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import StandaloneMemberDetails from "pages/InquiriesAndAdjustments/MasterInquiry/StandaloneMemberDetails";
import { memo } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import { CAPTIONS } from "../../../constants";
import AddForfeitureModal from "./AddForfeitureModal";
import ForfeituresAdjustmentPanel from "./ForfeituresAdjustmentPanel";
import ForfeituresAdjustmentSearchFilter from "./ForfeituresAdjustmentSearchFilter";
import ForfeituresTransactionGrid from "./ForfeituresTransactionGrid";
import useForfeituresAdjustment from "./hooks/useForfeituresAdjustment";

const ForfeituresAdjustmentContent = memo(() => {
  const {
    employeeData,
    transactionData,
    isSearching,
    isFetchingTransactions,
    isAddForfeitureModalOpen,
    showEmployeeData,
    showTransactions,
    executeSearch,
    handleReset,
    handleSaveForfeiture,
    openAddForfeitureModal,
    closeAddForfeitureModal,
    transactionPagination,
    profitYear,
    isReadOnly,
    memberDetailsRefreshTrigger,
    currentBalance
  } = useForfeituresAdjustment();

  return (
    <Grid
      container
      rowSpacing="24px">
      <Grid width={"100%"}>
        <Divider />
      </Grid>

      <Grid width={"100%"}>
        <DSMAccordion title="Filter">
          <ForfeituresAdjustmentSearchFilter
            onSearch={executeSearch}
            onReset={handleReset}
            isSearching={isSearching}
          />
        </DSMAccordion>
      </Grid>

      {showEmployeeData && employeeData && profitYear && (
        <StandaloneMemberDetails
          memberType={1}
          id={employeeData.demographicId}
          profitYear={profitYear}
          refreshTrigger={memberDetailsRefreshTrigger}
        />
      )}

      {showEmployeeData && (
        <Grid width="100%">
          <ForfeituresAdjustmentPanel
            onAddForfeiture={openAddForfeitureModal}
            isReadOnly={isReadOnly}
            currentBalance={currentBalance}
          />
        </Grid>
      )}

      {showTransactions && (
        <Grid width="100%">
          <ForfeituresTransactionGrid
            transactionData={transactionData}
            isLoading={isFetchingTransactions}
            pagination={transactionPagination}
            onSortChange={transactionPagination.handleSortChange}
          />
        </Grid>
      )}

      <AddForfeitureModal
        open={isAddForfeitureModalOpen}
        onClose={closeAddForfeitureModal}
        onSave={handleSaveForfeiture}
        suggestedForfeitResponse={employeeData}
      />
    </Grid>
  );
});

const ForfeituresAdjustment = () => {
  return (
    <PageErrorBoundary pageName="Forfeitures Adjustment">
      <Page
        label={CAPTIONS.FORFEITURES_ADJUSTMENT}
        actionNode={<StatusDropdownActionNode />}>
        <MissiveAlertProvider>
          <ForfeituresAdjustmentContent />
        </MissiveAlertProvider>
      </Page>
    </PageErrorBoundary>
  );
};

export default ForfeituresAdjustment;
