import { ICellRendererParams, CellClickedEvent, ColDef } from "ag-grid-community";
import { useEffect, useMemo, useState, useCallback } from "react";
import { useSelector } from "react-redux";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, numberToCurrency, Pagination } from "smart-ui-library";
import { TotalsGrid } from "../../../components/TotalsGrid/TotalsGrid";
import { ReportSummary } from "../../../components/ReportSummary";
import { StartAndEndDateRequest } from "reduxstore/types";
import { useLazyGetTerminationReportQuery } from "reduxstore/api/YearsEndApi";
import { GetDetailColumns, GetTerminationColumns } from "./TerminationGridColumn";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { ForfeitureAdjustmentUpdateRequest } from "reduxstore/types";
import { useUpdateForfeitureAdjustmentMutation, useUpdateForfeitureAdjustmentBulkMutation } from "reduxstore/api/YearsEndApi";

interface TerminationGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  searchParams: StartAndEndDateRequest | null;
  resetPageFlag: boolean;
  onUnsavedChanges: (hasChanges: boolean) => void;
  hasUnsavedChanges: boolean;
}

const TerminationGrid: React.FC<TerminationGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  searchParams,
  resetPageFlag,
  onUnsavedChanges,
  hasUnsavedChanges
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});
  const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);
  const { termination } = useSelector((state: RootState) => state.yearsEnd);
  const [triggerSearch, { isFetching }] = useLazyGetTerminationReportQuery();
  const [selectedRowIds, setSelectedRowIds] = useState<number[]>([]);
  const [editedValues, setEditedValues] = useState<Record<string, { value: number; hasError: boolean }>>({});
  const selectedProfitYear = useDecemberFlowProfitYear();
  const [updateForfeitureAdjustmentBulk] = useUpdateForfeitureAdjustmentBulkMutation();
  const [updateForfeitureAdjustment] = useUpdateForfeitureAdjustmentMutation();

  const handleSave = useCallback(async (request: ForfeitureAdjustmentUpdateRequest) => {
    try {
      await updateForfeitureAdjustment(request);
      const rowKey = `${request.badgeNumber}-${request.profitYear}`;
      setEditedValues(prev => {
        const updated = { ...prev };
        delete updated[rowKey];
        return updated;
      });
      onUnsavedChanges(Object.keys(editedValues).length > 1);
      if (searchParams) {
        const params = {
          ...searchParams,
          pagination: {
            skip: pageNumber * pageSize,
            take: pageSize,
            sortBy: sortParams.sortBy,
            isSortDescending: sortParams.isSortDescending
          }
        };
        triggerSearch(params, false);
      }
    } catch (error) {
      console.error('Failed to save forfeiture adjustment:', error);
      alert('Failed to save. Please try again.');
    }
  }, [updateForfeitureAdjustment, editedValues, onUnsavedChanges, searchParams, pageNumber, pageSize, sortParams, triggerSearch]);

  // Reset page number to 0 when resetPageFlag changes
  useEffect(() => {
    setPageNumber(0);
  }, [resetPageFlag]);

  // Track unsaved changes
  useEffect(() => {
    const hasChanges = selectedRowIds.length > 0;
    onUnsavedChanges(hasChanges);
  }, [selectedRowIds, onUnsavedChanges]);

  // Initialize expandedRows when data is loaded
  useEffect(() => {

    setPageNumber(0);
    
    if (termination?.response?.results && termination.response.results.length > 0) {
      // Only reset if badgeNumbers have changed
      const badgeNumbers = termination.response.results.map((row: any) => row.badgeNumber).join(",");
      const prevBadgeNumbers = Object.keys(expandedRows).join(",");
      if (badgeNumbers !== prevBadgeNumbers) {
        const initialExpandState: Record<string, boolean> = {};
        termination.response.results.forEach((row: any) => {
          // Set to TRUE to auto-expand rows with details!
          if (row.yearDetails && row.yearDetails.length > 0) {
            initialExpandState[row.badgeNumber] = true;
          }
        });
        setExpandedRows(initialExpandState);
      }
    }
  }, [termination?.response?.results]);

  // Fetch data when pagination, sort, or searchParams change
  useEffect(() => {
    if (searchParams) {
      const params = {
        ...searchParams,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      };
      triggerSearch(params, false);
    }
  }, [searchParams, pageNumber, pageSize, sortParams, triggerSearch]);

  const handleRowExpansion = (badgeNumber: string) => {
    setExpandedRows((prev) => ({
      ...prev,
      [badgeNumber]: !prev[badgeNumber]
    }));
  };


  const addRowToSelectedRows = (id: number) => {
    setSelectedRowIds([...selectedRowIds, id]);
  };

  const removeRowFromSelectedRows = (id: number) => {
    setSelectedRowIds(selectedRowIds.filter((rowId) => rowId !== id));
  };

  const updateEditedValue = useCallback((rowKey: string, value: number, hasError: boolean) => {
    setEditedValues(prev => ({
      ...prev,
      [rowKey]: { value, hasError }
    }));
  }, []);

  const handleBulkSave = useCallback(async (requests: ForfeitureAdjustmentUpdateRequest[]) => {
    try {
      await updateForfeitureAdjustmentBulk(requests);
      const updatedEditedValues = { ...editedValues };
      requests.forEach(request => {
        const rowKey = `${request.badgeNumber}-${request.profitYear}`;
        delete updatedEditedValues[rowKey];
      });
      setEditedValues(updatedEditedValues);
      setSelectedRowIds([]);
      onUnsavedChanges(Object.keys(updatedEditedValues).length > 0);
      if (searchParams) {
        const params = {
          ...searchParams,
          pagination: {
            skip: pageNumber * pageSize,
            take: pageSize,
            sortBy: sortParams.sortBy,
            isSortDescending: sortParams.isSortDescending
          }
        };
        triggerSearch(params, false);
      }
    } catch (error) {
      console.error('Failed to save forfeiture adjustments:', error);
      alert('Failed to save one or more adjustments. Please try again.');
    }
  }, [updateForfeitureAdjustmentBulk, editedValues, onUnsavedChanges, searchParams, pageNumber, pageSize, sortParams, triggerSearch]);

  // Get main and detail columns
  const mainColumns = useMemo(() => GetTerminationColumns(), []);
  const detailColumns = useMemo(() => GetDetailColumns(addRowToSelectedRows, removeRowFromSelectedRows, selectedRowIds, selectedProfitYear, handleSave, handleBulkSave), [selectedRowIds, selectedProfitYear, handleSave, handleBulkSave]);

  // Build grid data with expandable rows
  const gridData = useMemo(() => {
    if (!termination?.response?.results) return [];
    const rows = [];
    for (const row of termination.response.results) {
      const hasDetails = row.yearDetails && row.yearDetails.length > 0;

      // Add main row
      rows.push({
        ...row,
        isExpandable: hasDetails,
        isExpanded: hasDetails && Boolean(expandedRows[row.badgeNumber])
      });

      // Add detail rows if expanded
      if (hasDetails && expandedRows[row.badgeNumber]) {
        for (const detail of row.yearDetails) {
          // Create a base detail row with all parent properties to prevent undefined values
          // and then override with detail properties
          const detailRow = {
            // Copy all parent row properties first
            ...row,
            // Then add detail properties, which will override any duplicate fields
            ...detail,
            // Add special properties for UI handling
            isDetail: true,
            parentId: row.badgeNumber
          };

          rows.push(detailRow);
        }
      }
    }

    return rows;
  }, [termination, expandedRows]);


  // Compose columns: show main columns for parent, detail columns for detail
  const columnDefs = useMemo(() => {
    // Add an expansion column as the first column
    const expansionColumn = {
      headerName: "",
      field: "isExpandable",
      width: 50,
      cellRenderer: (params: ICellRendererParams) => {
        if (!params.data.isDetail && params.data.isExpandable) {
          return params.data.isExpanded ? "▼" : "►";
        }
        return "";
      },
      onCellClicked: (params: CellClickedEvent) => {
        if (!params.data.isDetail && params.data.isExpandable) {
          handleRowExpansion(params.data.badgeNumber);
        }
      },
      suppressSizeToFit: true,
      suppressAutoSize: true,
      lockVisible: true,
      lockPosition: true,
      pinned: "left"
    } as ColDef;

    // Determine which columns to display based on whether it's a detail row
    const visibleColumns = mainColumns.map((column) => {
      return {
        ...column,
        cellRenderer: (params: ICellRendererParams) => {
          // For detail rows, either hide the column or show a specific value
          if (params.data.isDetail) {
            // Check if this main column should be hidden in detail rows
            const hideInDetails = !detailColumns.some((detailCol) => detailCol.field === column.field);

            if (hideInDetails) {
              return ""; // Hide this column's content for detail rows
            }
          }

          // Use the default renderer for this column if available
          if (column.cellRenderer) {
            return column.cellRenderer(params);
          }

          // Otherwise just return the field value
          return params.valueFormatted ? params.valueFormatted : params.value;
        }
      };
    });

    // Add detail-specific columns that only appear for detail rows
    const detailOnlyColumns = detailColumns
      .filter((detailCol) => !mainColumns.some((mainCol) => mainCol.field === detailCol.field))
      .map((column) => {
        return {
          ...column,
          cellRenderer: (params: ICellRendererParams) => {
            // Only show content for detail rows
            if (!params.data.isDetail) {
              return "";
            }

            // Use the default renderer for this column if available
            if (column.cellRenderer) {
              return column.cellRenderer(params);
            }

            // Otherwise just return the field value
            return params.valueFormatted ? params.valueFormatted : params.value;
          }
        };
      });

    // Combine all columns
    return [expansionColumn, ...visibleColumns, ...detailOnlyColumns];
  }, [mainColumns, detailColumns]);

  // Row class for detail rows
  const getRowClass = (params: { data: { isDetail: boolean } }) => {
    return params.data.isDetail ? "detail-row" : "";
  };

  const sortEventHandler = (update: ISortParams) => {
    setSortParams(update);
    setPageNumber(0);
  };

  return (
    <div className="termination-grid-container">
      <style>
        {`
          .termination-spinner-overlay {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(255,255,255,0.6);
            z-index: 1000;
            display: flex;
            align-items: center;
            justify-content: center;
          }
          .termination-spinner {
            width: 48px;
            height: 48px;
          }
          .detail-row {
            background-color: #f5f5f5;
          }
          .invalid-cell {
            background-color: #fff6f6;
          }
        `}
      </style>
      {isFetching && (
        <div className="termination-spinner-overlay">
          <div className="spinner-border termination-spinner" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      )}
      {termination?.response && (
        <>
          <ReportSummary report={termination} />

          <div className="flex sticky top-0 z-10 bg-white">
            <TotalsGrid
              displayData={[[numberToCurrency(termination.totalEndingBalance || 0)]]}
              leftColumnHeaders={["Amount in Profit Sharing"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(termination.totalVested || 0)]]}
              leftColumnHeaders={["Vested Amount"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(termination.totalForfeit || 0)]]}
              leftColumnHeaders={["Total Forfeitures"]}
              topRowHeaders={[]}></TotalsGrid>
            <TotalsGrid
              displayData={[[numberToCurrency(termination.totalBeneficiaryAllocation || 0)]]}
              leftColumnHeaders={["Total Beneficiary Allocations"]}
              topRowHeaders={[]}></TotalsGrid>
          </div>

          <DSMGrid
            preferenceKey={"QPREV-PROF"}
            handleSortChanged={sortEventHandler}
            maxHeight={800}
            isLoading={isFetching}
            providedOptions={{
              rowData: gridData,
              columnDefs: columnDefs,
              getRowClass: getRowClass,
              rowSelection: 'multiple',
              suppressRowClickSelection: true,
              rowHeight: 40,
              suppressMultiSort: true,
              defaultColDef: {
                resizable: true
              },
              context: {
                editedValues,
                updateEditedValue
              }
            }}
          />

          {!!termination && termination.response.results.length > 0 && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={(value: number) => {
                if (hasUnsavedChanges) {
                  alert("Please save your changes.");
                  return;
                }
                setPageNumber(value - 1);
                setInitialSearchLoaded(true);
              }}
              pageSize={pageSize}
              setPageSize={(value: number) => {
                if (hasUnsavedChanges) {
                  alert("Please save your changes.");
                  return;
                }
                setPageSize(value);
                setPageNumber(0);
                setInitialSearchLoaded(true);
              }}
              recordCount={termination.response.total || 0}
            />
          )}
        </>
      )}
    </div>
  );
};

export default TerminationGrid;
