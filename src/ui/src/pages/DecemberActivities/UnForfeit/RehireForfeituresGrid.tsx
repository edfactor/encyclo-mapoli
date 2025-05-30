import { ICellRendererParams } from "ag-grid-community";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetRehireForfeituresQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import useFiscalCalendarYear from "../../../hooks/useFiscalCalendarYear";
import { StartAndEndDateRequest } from "../../../reduxstore/types";
import { GetDetailColumns, GetMilitaryAndRehireForfeituresColumns } from "./RehireForfeituresGridColumns";
import ReportSummary from "../../../components/ReportSummary";

interface MilitaryAndRehireForfeituresGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  resetPageFlag: boolean;
}

const RehireForfeituresGrid: React.FC<MilitaryAndRehireForfeituresGridSearchProps> = ({
                                                                                        initialSearchLoaded,
                                                                                        setInitialSearchLoaded,
                                                                                        resetPageFlag // Destructure the new prop
                                                                                      }) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});
  const fiscalCalendarYear = useFiscalCalendarYear();
  const { rehireForfeitures, rehireForfeituresQueryParams } = useSelector((state: RootState) => state.yearsEnd);

  const [triggerSearch, { isFetching }] = useLazyGetRehireForfeituresQuery();

  // Create a request object based on current parameters
  const createRequest = useCallback(
    (skip: number, sortBy: string, isSortDescending: boolean): StartAndEndDateRequest | null => {
      if (!rehireForfeituresQueryParams) return null;

      return {
        beginningDate: rehireForfeituresQueryParams.beginningDate || fiscalCalendarYear?.fiscalBeginDate || "",
        endingDate: rehireForfeituresQueryParams.endingDate || fiscalCalendarYear?.fiscalEndDate || "",
        pagination: { skip, take: pageSize, sortBy, isSortDescending }
      };
    },
    [rehireForfeituresQueryParams, fiscalCalendarYear?.fiscalBeginDate, fiscalCalendarYear?.fiscalEndDate, pageSize]
  );

  const performSearch = useCallback(
    async (skip: number, sortBy: string, isSortDescending: boolean) => {
      if (rehireForfeituresQueryParams) {
        const request = createRequest(skip, sortBy, isSortDescending);
        if (request) {
          await triggerSearch(request, false);
        }
      }
    },
    [createRequest, rehireForfeituresQueryParams, triggerSearch]
  );

  // Effect to handle initial load and pagination changes
  useEffect(() => {
    if (initialSearchLoaded) {
      performSearch(pageNumber * pageSize, sortParams.sortBy, sortParams.isSortDescending);
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, performSearch]);

  // Reset page number to 0 when resetPageFlag changes
  useEffect(() => {
    setPageNumber(0);
  }, [resetPageFlag]);

  // Initialize expandedRows when data is loaded
  useEffect(() => {
    if (rehireForfeitures?.response?.results) {
      const initialExpandState: Record<string, boolean> = {};

      // Set all rows with details to be expanded by default
      rehireForfeitures.response.results.forEach((row) => {
        if (row.details && row.details.length > 0) {
          initialExpandState[row.badgeNumber] = true;
        }
      });

      setExpandedRows(initialExpandState);
    }
  }, [rehireForfeitures?.response?.results]);

  // Sort handler that immediately triggers a search with the new sort parameters
  const sortEventHandler = (update: ISortParams) => {
    setSortParams(update); // Update state for future reference
    setPageNumber(0); // Reset to first page when sorting

    // Immediately perform search with the new sort params
    performSearch(0, update.sortBy, update.isSortDescending);
  };

  // Handle row expansion toggle
  const handleRowExpansion = (badgeNumber: string) => {
    setExpandedRows((prev) => ({
      ...prev,
      [badgeNumber]: !prev[badgeNumber]
    }));
  };

  // Get the main and detail columns
  const mainColumns = useMemo(() => GetMilitaryAndRehireForfeituresColumns(), []);
  const detailColumns = useMemo(() => GetDetailColumns(), []);

  // Create the grid data with expandable rows
  const gridData = useMemo(() => {
    if (!rehireForfeitures?.response?.results) return [];

    const rows = [];

    for (const row of rehireForfeitures.response.results) {
      const hasDetails = row.details && row.details.length > 0;

      // Add main row
      rows.push({
        ...row,
        isExpandable: hasDetails,
        isExpanded: hasDetails && Boolean(expandedRows[row.badgeNumber])
      });

      // Add detail rows if expanded
      if (hasDetails && expandedRows[row.badgeNumber]) {
        for (const detail of row.details) {
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
  }, [rehireForfeitures, expandedRows]);

  // Create column definitions with expand/collapse functionality
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
    return [expansionColumn, ...visibleColumns, ...detailOnlyColumns];
  }, [mainColumns, detailColumns]);

  // Custom CSS classes for rows
  const getRowClass = (params: { data: { isDetail: boolean } }) => {
    return params.data.isDetail ? "detail-row" : "";
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

      {rehireForfeitures?.response && (
        <>
          <ReportSummary report={rehireForfeitures} />
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

          {!!rehireForfeitures && rehireForfeitures.response.results.length > 0 && (
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
              recordCount={rehireForfeitures.response.total || 0}
            />
          )}
        </>
      )}
    </div>
  );
};

export default RehireForfeituresGrid;

