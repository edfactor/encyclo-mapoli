import { Print } from "@mui/icons-material";
import ArrowDropDownIcon from "@mui/icons-material/ArrowDropDown";
import {
  Button,
  ButtonGroup,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Menu,
  MenuItem,
  Tooltip,
  Typography
} from "@mui/material";
import { SelectionChangedEvent } from "ag-grid-community";
import React, { useCallback, useMemo, useState } from "react";
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
  printTerminatedLetters: (content: string, title: string) => void;
  isXerox: boolean;
  setIsXerox: (value: boolean) => void;
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
  printTerminatedLetters,
  isXerox,
  setIsXerox
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
  const printModeLabel = isXerox ? "Xerox" : "Default";
  const dialogTitle = `Print Preview - Terminated Letters (${printModeLabel})`;
  const [printerMenuAnchorEl, setPrinterMenuAnchorEl] = useState<null | HTMLElement>(null);
  const isPrinterMenuOpen = Boolean(printerMenuAnchorEl);

  const handleOpenPrinterMenu = (event: React.MouseEvent<HTMLButtonElement>) => {
    setPrinterMenuAnchorEl(event.currentTarget);
  };

  const handleClosePrinterMenu = () => {
    setPrinterMenuAnchorEl(null);
  };

  const handleSelectPrinter = (value: "default" | "xerox") => {
    setIsXerox(value === "xerox");
    handleClosePrinterMenu();
  };

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
      <ButtonGroup
        variant="outlined"
        color="primary"
        size="medium"
        className="ml-2 mr-5">
        <Button
          startIcon={isDownloading ? <CircularProgress size={20} /> : <Print />}
          onClick={handlePrint}
          disabled={isPrintDisabled || isDownloading}
          className="whitespace-nowrap">
          {isDownloading ? "Generating..." : "Print"}
        </Button>
        <Button
          onClick={handleOpenPrinterMenu}
          aria-label="Select printer"
          aria-controls={isPrinterMenuOpen ? "terminated-letters-printer-menu" : undefined}
          aria-haspopup="true"
          aria-expanded={isPrinterMenuOpen ? "true" : undefined}
          className="min-w-0 px-2">
          <ArrowDropDownIcon />
        </Button>
      </ButtonGroup>
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
    <div className="mr-6">
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
            <div className="mb-4 flex items-center justify-between">
              <Typography
                variant="h6"
                component="h2"
                sx={{ marginLeft: "20px", marginRight: "10px" }}>
                EMPLOYEES NEEDING INSTRUCTIONS TO WITHDRAW VESTED SAVINGS (
                {formatNumberWithComma(reportData.response.total)} Records)
              </Typography>
              <div className="flex items-center">{renderPrintButton()}</div>
            </div>
          }
        />
      )}

      <Menu
        id="terminated-letters-printer-menu"
        anchorEl={printerMenuAnchorEl}
        open={isPrinterMenuOpen}
        onClose={handleClosePrinterMenu}
        anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
        transformOrigin={{ vertical: "top", horizontal: "right" }}>
        <MenuItem onClick={() => handleSelectPrinter("default")}>Default</MenuItem>
        <MenuItem onClick={() => handleSelectPrinter("xerox")}>Xerox</MenuItem>
      </Menu>

      <Dialog
        open={isPrintDialogOpen}
        onClose={() => setIsPrintDialogOpen(false)}
        maxWidth="lg"
        fullWidth>
        <DialogTitle>{dialogTitle}</DialogTitle>
        <DialogContent>
          <pre className="whitespace-pre-wrap font-mono text-xs">{printContent}</pre>
        </DialogContent>
        <DialogActions sx={{ paddingRight: "25px" }}>
          <Button
            onClick={() => printTerminatedLetters(printContent, dialogTitle)}
            variant="contained">
            Print
          </Button>
          <Button onClick={() => setIsPrintDialogOpen(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default TerminatedLettersGrid;
