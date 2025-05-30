import { ICellRendererParams } from "ag-grid-community";
import { useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router";
import { useLazyGetTerminationReportQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, numberToCurrency, Pagination } from "smart-ui-library";
import { TotalsGrid } from "../../../components/TotalsGrid/TotalsGrid";
import { ReportSummary } from "../../../components/ReportSummary";
import { StartAndEndDateRequest } from "reduxstore/types";
import useFiscalCalendarYear from "../../../hooks/useFiscalCalendarYear";
import { GetDetailColumns, GetTerminationColumns } from "./TerminationGridColumn";

interface TerminationGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  searchParams: StartAndEndDateRequest | null;
  resetPageFlag: boolean;
}

const TerminationGrid: React.FC<TerminationGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  searchParams,
  resetPageFlag
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
  const fiscalCalendarYear = useFiscalCalendarYear();

  useEffect(() => {
    if (searchParams && hasToken) {
      const request = {
        ...searchParams,
        pagination: {
          skip: pageNumber * pageSize,
          take: pageSize,
          sortBy: sortParams.sortBy,
          isSortDescending: sortParams.isSortDescending
        }
      };
      triggerSearch(request, false);
    }
  }, [searchParams, pageNumber, pageSize, sortParams, hasToken, triggerSearch]);

    // Reset page number to 0 when resetPageFlag changes
  useEffect(() => {
    setPageNumber(0);
  }, [resetPageFlag]);

  // Initialize expandedRows when data is loaded
  useEffect(() => {
    if (termination?.response?.results && termination.response.results.length > 0) {
      // Only reset if badgeNumbers have changed
      const badgeNumbers = termination.response.results.map((row: any) => row.badgeNumber).join(",");
      const prevBadgeNumbers = Object.keys(expandedRows).join(",");
      if (badgeNumbers !== prevBadgeNumbers) {
        const initialExpandState: Record<string, boolean> = {};
        termination.response.results.forEach((row: any) => {
          if (row.yearDetails && row.yearDetails.length > 0) {
            initialExpandState[row.badgeNumber] = false;
          }
        });
        setExpandedRows(initialExpandState);
      }
    }
  }, [termination?.response?.results]);

  const handleRowExpansion = (badgeNumber: string) => {
    setExpandedRows((prev) => ({
      ...prev,
      [badgeNumber]: !prev[badgeNumber]
    }));
  };

  // Get main and detail columns
  const mainColumns = useMemo(() => GetTerminationColumns(), []);
  const detailColumns = useMemo(() => GetDetailColumns(), []);

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

  // Debug: log gridData to verify what is being rendered
  useEffect(() => {
    // eslint-disable-next-line no-console
    console.log("TerminationGrid gridData:", gridData);
  }, [gridData]);

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
      onCellClicked: (params: ICellRendererParams) => {
        if (!params.data.isDetail && params.data.isExpandable) {
          handleRowExpansion(params.data.badgeNumber);
        }
      },
      suppressSizeToFit: true,
      suppressAutoSize: true,
      lockVisible: true,
      lockPosition: true,
      pinned: "left"
    };

    // Add a style column to handle indentation
    const indentationColumn = {
      headerName: "",
      field: "isDetail",
      width: 30,
      cellRenderer: (params: ICellRendererParams) => {
        return params.data.isDetail ? "" : "";
      },
      suppressSizeToFit: true,
      suppressAutoSize: true,
      lockVisible: true,
      lockPosition: true,
      pinned: "left"
    };

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
          return params.value;
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
            return params.value;
          }
        };
      });

    // Combine all columns
    return [expansionColumn, indentationColumn, ...visibleColumns, ...detailOnlyColumns];
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
    <div>
      <style>
        {`
          .detail-row {
            background-color: #f5f5f5;
          }
        `}
      </style>
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
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            providedOptions={{
              rowData: gridData,
              columnDefs: columnDefs,
              getRowClass: getRowClass,
              suppressRowClickSelection: true,
              rowHeight: 40,
              suppressMultiSort: true,
              defaultColDef: {
                resizable: true
              }
            }}
          />

          {!!termination && termination.response.results.length > 0 && (
            <Pagination
              pageNumber={pageNumber}
              setPageNumber={(value: number) => {
                setPageNumber(value - 1);
                setInitialSearchLoaded(true);
              }}
              pageSize={pageSize}
              setPageSize={(value: number) => {
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
