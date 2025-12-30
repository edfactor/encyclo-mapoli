import { Print } from "@mui/icons-material";
import {
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Tooltip,
  Typography
} from "@mui/material";
import { SelectionChangedEvent } from "ag-grid-community";
import React, { useCallback, useMemo } from "react";
import { TerminatedLettersDetail, TerminatedLettersResponse } from "reduxstore/types";
import { formatNumberWithComma, ISortParams } from "smart-ui-library";
import { DSMPaginatedGrid } from "../../../components/DSMPaginatedGrid/DSMPaginatedGrid";
import { GRID_KEYS } from "../../../constants";
import { useGridPagination } from "../../../hooks/useGridPagination";
import { GetTerminatedLettersColumns } from "./TerminatedLettersGridColumns";

interface TerminatedLettersGridProps {
  reportData: TerminatedLettersResponse | null;
  isLoading: boolean;
  gridPagination: ReturnType<typeof useGridPagination>;
  selectedRows: TerminatedLettersDetail[];
  setSelectedRows: (rows: TerminatedLettersDetail[]) => void;
  handlePrint: () => Promise<void>;
  isDownloading: boolean;
  isPrintDialogOpen: boolean;
  setIsPrintDialogOpen: (open: boolean) => void;
  printContent: string;
  printTerminatedLetters: (content: string) => void;
}

const TerminatedLettersGrid: React.FC<TerminatedLettersGridProps> = ({
  reportData,
  isLoading,
  gridPagination,
  selectedRows,
  setSelectedRows,
  handlePrint,
  isDownloading,
  isPrintDialogOpen,
  setIsPrintDialogOpen,
  printContent,
  printTerminatedLetters
}) => {
  const { sortParams, handleSortChange } = gridPagination;

  const handleSelectionChanged = useCallback(
    (event: SelectionChangedEvent) => {
      const selectedData = event.api.getSelectedRows();
      setSelectedRows(selectedData);
    },
    [setSelectedRows]
  );

  const sortEventHandler = (update: ISortParams) => handleSortChange(update);
  const columnDefs = useMemo(() => GetTerminatedLettersColumns(), []);

  const isPrintDisabled = selectedRows.length === 0;

  const paginationProps = {
    pageNumber: gridPagination.pageNumber,
    pageSize: gridPagination.pageSize,
    sortParams,
    handlePageNumberChange: gridPagination.handlePageNumberChange,
    handlePageSizeChange: gridPagination.handlePageSizeChange,
    handleSortChange
  };

  const renderPrintButton = () => {
    const button = (
      <Button
        variant="outlined"
        color="primary"
        size="medium"
        startIcon={isDownloading ? <CircularProgress size={20} /> : <Print />}
        onClick={handlePrint}
        disabled={isPrintDisabled || isDownloading}
        sx={{ marginLeft: 2, marginRight: "20px" }}>
        {isDownloading ? "Generating..." : "Print"}
      </Button>
    );

    if (isPrintDisabled) {
      return (
        <Tooltip
          title="You must check at least one box"
          placement="top">
          <span>{button}</span>
        </Tooltip>
      );
    }

    return button;
  };

  return (
    <div style={{ marginRight: "24px" }}>
      {reportData && reportData.response && (
        <DSMPaginatedGrid
          preferenceKey={GRID_KEYS.TERMINATED_LETTERS}
          data={reportData.response.results}
          columnDefs={columnDefs}
          totalRecords={reportData.response.total}
          isLoading={isLoading}
          pagination={paginationProps}
          onSortChange={sortEventHandler}
          gridOptions={{
            suppressMultiSort: true,
            rowSelection: "multiple",
            onSelectionChanged: handleSelectionChanged
          }}
          showPagination={reportData.response.results && reportData.response.results.length > 0}
          header={
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
              <Typography
                variant="h6"
                component="h2"
                sx={{ marginLeft: "20px", marginRight: "10px" }}>
                EMPLOYEES NEEDING INSTRUCTIONS TO WITHDRAW VESTED SAVINGS (
                {formatNumberWithComma(reportData.response.total)} Records)
              </Typography>
              {renderPrintButton()}
            </div>
          }
        />
      )}

      <Dialog
        open={isPrintDialogOpen}
        onClose={() => setIsPrintDialogOpen(false)}
        maxWidth="lg"
        fullWidth>
        <DialogTitle>Print Preview - Terminated Letters</DialogTitle>
        <DialogContent>
          <pre style={{ whiteSpace: "pre-wrap", fontFamily: "monospace", fontSize: "12px" }}>{printContent}</pre>
        </DialogContent>
        <DialogActions sx={{ paddingRight: "25px" }}>
          <Button onClick={() => setIsPrintDialogOpen(false)}>Close</Button>
          <Button
            onClick={() => printTerminatedLetters(printContent)}
            variant="contained">
            Print
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default TerminatedLettersGrid;
