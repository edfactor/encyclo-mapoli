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
import { DSMGrid, formatNumberWithComma, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
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
  const { pageNumber, pageSize, handlePaginationChange, handleSortChange } = gridPagination;

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
        <>
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
            <Typography
              variant="h6"
              component="h2"
              sx={{ marginLeft: "20px", marginRight: "10px" }}>
              EMPLOYEES NEEDING INSTRUCTIONS TO WITHDRAW VESTED SAVINGS ({formatNumberWithComma(reportData.response.total)}{" "}
              Records)
            </Typography>
            {renderPrintButton()}
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.TERMINATED_LETTERS}
            isLoading={isLoading}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: reportData.response.results,
              columnDefs: columnDefs,
              suppressMultiSort: true,
              rowSelection: "multiple",
              onSelectionChanged: handleSelectionChanged
            }}
          />
        </>
      )}
      {reportData && reportData.response && reportData.response.results && reportData.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            handlePaginationChange(value - 1, pageSize);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            handlePaginationChange(0, value);
          }}
          recordCount={reportData.response.total}
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
