import { TerminatedLettersDetail } from "";
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
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import {
  useLazyGetTerminatedLettersDownloadQuery,
  useLazyGetTerminatedLettersReportQuery
} from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, formatNumberWithComma, ISortParams, Pagination } from "smart-ui-library";
import { CAPTIONS } from "../../../constants";
import { GetTerminatedLettersColumns } from "./TerminatedLettersGridColumns";

interface TerminatedLettersRequest {
  profitYear: number;
  pagination: {
    skip: number;
    take: number;
    sortBy: string;
    isSortDescending: boolean;
  };
  beginningDate?: string;
  endingDate?: string;
  badgeNumbers?: number[];
}

interface TerminatedLettersGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
}

const TerminatedLettersGrid: React.FC<TerminatedLettersGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(50);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "fullName",
    isSortDescending: false
  });
  const [selectedRows, setSelectedRows] = useState<TerminatedLettersDetail[]>([]);
  const [isPrintDialogOpen, setIsPrintDialogOpen] = useState(false);
  const [printContent, setPrintContent] = useState<string>("");

  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const { terminatedLetters, terminatedLettersQueryParams } = useSelector((state: RootState) => state.yearsEnd);
  const profitYear = useDecemberFlowProfitYear();
  const [triggerSearch, { isFetching }] = useLazyGetTerminatedLettersReportQuery();
  const [triggerDownload, { isFetching: isDownloading }] = useLazyGetTerminatedLettersDownloadQuery();

  const handlePaginationChange = useCallback(
    async (newPageNumber: number, newPageSize: number, newSortParams?: ISortParams) => {
      if (hasToken && initialSearchLoaded) {
        const request: TerminatedLettersRequest = {
          profitYear: profitYear || 0,
          pagination: {
            skip: newPageNumber * newPageSize,
            take: newPageSize,
            sortBy: newSortParams?.sortBy || sortParams.sortBy,
            isSortDescending: newSortParams?.isSortDescending || sortParams.isSortDescending
          }
        };

        if (terminatedLettersQueryParams?.beginningDate !== undefined) {
          request.beginningDate = terminatedLettersQueryParams.beginningDate;
        }
        if (terminatedLettersQueryParams?.endingDate !== undefined) {
          request.endingDate = terminatedLettersQueryParams.endingDate;
        }

        await triggerSearch(request, false);
      }
    },
    [hasToken, initialSearchLoaded, profitYear, terminatedLettersQueryParams, sortParams, triggerSearch]
  );

  const handlePrint = useCallback(async () => {
    if (selectedRows.length === 0) return;

    const badgeNumbers = selectedRows.map((row) => row.badgeNumber);

    const request: TerminatedLettersRequest = {
      profitYear: profitYear || 0,
      badgeNumbers: badgeNumbers,
      pagination: {
        skip: 0,
        take: 999999,
        sortBy: "fullName",
        isSortDescending: false
      }
    };

    if (terminatedLettersQueryParams?.beginningDate !== undefined) {
      request.beginningDate = terminatedLettersQueryParams.beginningDate;
    }
    if (terminatedLettersQueryParams?.endingDate !== undefined) {
      request.endingDate = terminatedLettersQueryParams.endingDate;
    }

    try {
      const result = await triggerDownload(request, false);
      if (result.data) {
        // Convert blob to string
        const text = await (result.data as Blob).text();
        setPrintContent(text);
        setIsPrintDialogOpen(true);

        // Do not automatically trigger print - let user click Print button in dialog
      }
    } catch (error) {
      console.error("Error downloading terminated letters:", error);
    }
  }, [selectedRows, profitYear, terminatedLettersQueryParams, triggerDownload]);

  const printTerminatedLetters = useCallback((content: string) => {
    const printWindow = window.open("", "_blank");
    if (printWindow) {
      printWindow.document.write(`
        <html>
          <head>
            <title>Terminated Letters</title>
            <style>
              body { 
                font-family: monospace; 
                font-size: 12px; 
                white-space: pre-wrap; 
                margin: 20px;
              }
              @media print {
                body { margin: 0; }
                @page {
                  margin: 0;
                  size: auto;
                }
              }
            </style>
          </head>
          <body>
            ${content.replace(/\n/g, "<br>")}
          </body>
        </html>
      `);
      printWindow.document.close();
      printWindow.focus();
      printWindow.print();
      printWindow.close();
    }
  }, []);

  const handleSelectionChanged = useCallback((event: SelectionChangedEvent) => {
    const selectedData = event.api.getSelectedRows();
    setSelectedRows(selectedData);
  }, []);

  interface TerminatedLettersData {
    response?: {
      results: TerminatedLettersDetail[];
      total: number;
    };
  }

  // Need a useEffect on a change in TerminatedLetters to reset the page number
  const prevTerminatedLetters = useRef<TerminatedLettersData | null>(null);
  useEffect(() => {
    if (
      terminatedLetters !== prevTerminatedLetters.current &&
      terminatedLetters?.response?.results &&
      terminatedLetters.response.results.length !== prevTerminatedLetters.current?.response?.results?.length
    ) {
      setPageNumber(0);
      setSelectedRows([]); // Clear selection when data changes
    }
    prevTerminatedLetters.current = terminatedLetters;
  }, [terminatedLetters]);

  useEffect(() => {
    handlePaginationChange(pageNumber, pageSize, sortParams);
  }, [pageNumber, pageSize, sortParams, handlePaginationChange]);

  const sortEventHandler = (update: ISortParams) => setSortParams(update);
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
      {terminatedLetters?.response && (
        <>
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
            <Typography
              variant="h6"
              component="h2"
              sx={{ marginLeft: "20px", marginRight: "10px" }}>
              EMPLOYEES NEEDING INSTRUCTIONS TO WITHDRAW VESTED SAVINGS (
              {formatNumberWithComma(terminatedLetters.response.total)} Records)
            </Typography>
            {renderPrintButton()}
          </div>
          <DSMGrid
            preferenceKey={CAPTIONS.TERMINATED_LETTERS}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: terminatedLetters?.response.results,
              columnDefs: columnDefs,
              suppressMultiSort: true,
              rowSelection: {
                mode: "multiRow",
                checkboxes: false,
                headerCheckbox: false,
                enableClickSelection: false
              },
              onSelectionChanged: handleSelectionChanged,
              suppressRowClickSelection: true
            }}
          />
        </>
      )}
      {!!terminatedLetters && terminatedLetters.response.results.length > 0 && (
        <Pagination
          pageNumber={pageNumber}
          setPageNumber={(value: number) => {
            setPageNumber(value - 1);
            setInitialSearchLoaded(true);
          }}
          pageSize={pageSize}
          setPageSize={(value: number) => {
            setPageSize(value);
            setPageNumber(1);
            setInitialSearchLoaded(true);
          }}
          recordCount={terminatedLetters.response.total}
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
