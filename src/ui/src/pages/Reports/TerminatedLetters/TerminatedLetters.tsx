import { Divider, Grid } from "@mui/material";
import MissiveAlerts from "components/MissiveAlerts/MissiveAlerts";
import StatusDropdownActionNode from "components/StatusDropdownActionNode";
import { DSMAccordion, Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../../components/MissiveAlerts/MissiveAlertContext";
import PageErrorBoundary from "../../../components/PageErrorBoundary/PageErrorBoundary";
import { CAPTIONS } from "../../../constants";
import useTerminatedLetters from "./hooks/useTerminatedLetters";
import TerminatedLettersGrid from "./TerminatedLettersGrid";
import TerminatedLettersSearchFilter from "./TerminatedLettersSearchFilter";

const TerminatedLettersContent = () => {
  const {
    reportData,
    isLoadingReport,
    searchCompleted,
    executeSearch,
    resetSearch,
    gridPagination,
    selectedRows,
    setSelectedRows,
    handlePrint,
    isDownloading,
    isPrintDialogOpen,
    setIsPrintDialogOpen,
    printContent,
    printTerminatedLetters,
    isXerox,
    setIsXerox
  } = useTerminatedLetters();

  return (
    <Grid
      container
      rowSpacing="24px">
      <Grid width="100%">
        <Divider />
      </Grid>

      <MissiveAlerts />

      <Grid width="100%">
        <DSMAccordion title="Filter">
          <TerminatedLettersSearchFilter
            onSearch={executeSearch}
            onReset={resetSearch}
          />
        </DSMAccordion>
      </Grid>

      <Grid width="100%">
        {searchCompleted && (
          <TerminatedLettersGrid
            reportData={reportData}
            isLoading={isLoadingReport}
            gridPagination={gridPagination}
            selectedRows={selectedRows}
            setSelectedRows={setSelectedRows}
            handlePrint={handlePrint}
            isDownloading={isDownloading}
            isPrintDialogOpen={isPrintDialogOpen}
            setIsPrintDialogOpen={setIsPrintDialogOpen}
            printContent={printContent}
            printTerminatedLetters={printTerminatedLetters}
            isXerox={isXerox}
            setIsXerox={setIsXerox}
          />
        )}
      </Grid>
    </Grid>
  );
};

const TerminatedLetters = () => {
  const renderActionNode = () => {
    return <StatusDropdownActionNode />;
  };

  return (
    <PageErrorBoundary pageName="Terminated Letters">
      <Page
        label={CAPTIONS.TERMINATED_LETTERS}
        actionNode={renderActionNode()}>
        <MissiveAlertProvider>
          <TerminatedLettersContent />
        </MissiveAlertProvider>
      </Page>
    </PageErrorBoundary>
  );
};

export default TerminatedLetters;
