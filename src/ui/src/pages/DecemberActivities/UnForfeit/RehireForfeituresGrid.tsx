import { GridApi } from "ag-grid-community";
import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetRehireForfeituresQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import useFiscalCalendarYear from "../../../hooks/useFiscalCalendarYear";
import { StartAndEndDateRequest, RehireForfeituresEditedValues, RehireForfeituresSelectedRow } from "../../../reduxstore/types";
import { GetDetailColumns, GetMilitaryAndRehireForfeituresColumns } from "./RehireForfeituresGridColumns";
import ReportSummary from "../../../components/ReportSummary";
import Grid2 from "@mui/material/Grid2";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";

interface MilitaryAndRehireForfeituresGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  resetPageFlag: boolean;
}

const RehireForfeituresGrid: React.FC<MilitaryAndRehireForfeituresGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  resetPageFlag
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: false
  });
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});
  const [selectedRows, setSelectedRows] = useState<RehireForfeituresSelectedRow[]>([]);
  const [gridApi, setGridApi] = useState<GridApi | null>(null);
  const [editedValues, setEditedValues] = useState<RehireForfeituresEditedValues>({});
  const fiscalCalendarYear = useFiscalCalendarYear();
  const selectedProfitYear = useDecemberFlowProfitYear();
  const { rehireForfeitures, rehireForfeituresQueryParams } = useSelector((state: RootState) => state.yearsEnd);

  const [triggerSearch, { isFetching }] = useLazyGetRehireForfeituresQuery();

  const onGridReady = useCallback((params: { api: GridApi }) => {
    setGridApi(params.api);
  }, []);

  const addRowToSelectedRows = useCallback((id: number) => {
    if (gridApi) {
      const node = gridApi.getRowNode(id.toString());
      if (node && node.data.isDetail) {
        const selectedRow: RehireForfeituresSelectedRow = {
          id,
          badgeNumber: node.data.badgeNumber,
          profitYear: node.data.profitYear,
          suggestedForfeit: node.data.suggestedForfeit
        };
        setSelectedRows(prev => [...prev.filter(row => row.id !== id), selectedRow]);
      }
    }
  }, [gridApi]);

  const removeRowFromSelectedRows = useCallback((id: number) => {
    setSelectedRows(prev => prev.filter(row => row.id !== id));
  }, []);

  const updateEditedValue = useCallback((rowKey: string, value: number, hasError: boolean) => {
    setEditedValues(prev => ({
      ...prev,
      [rowKey]: { value, hasError }
    }));
  }, []);

  // Need a useEffect to reset the page number when rehireForfeitures changes
  const prevRehireForfeitures = useRef<any>(null);
  useEffect(() => {
    if (
      rehireForfeitures !== prevRehireForfeitures.current &&
      rehireForfeitures?.response?.results &&
      rehireForfeitures.response.results.length !== prevRehireForfeitures.current?.response?.results?.length
    ) {
      setPageNumber(0);
    }
    prevRehireForfeitures.current = rehireForfeitures;
  }, [rehireForfeitures]);

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
          initialExpandState[row.badgeNumber.toString()] = true;
        }
      });

      setExpandedRows(initialExpandState);
    }
  }, [rehireForfeitures?.response?.results]);

  // Sort handler that immediately triggers a search with the new sort parameters
  const sortEventHandler = (update: ISortParams) => {
    setSortParams(update);
    setPageNumber(0);
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
  const detailColumns = useMemo(() => GetDetailColumns(addRowToSelectedRows, removeRowFromSelectedRows, selectedProfitYear), [addRowToSelectedRows, removeRowFromSelectedRows, selectedProfitYear]);

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
        isExpanded: hasDetails && Boolean(expandedRows[row.badgeNumber.toString()]),
        isDetail: false
      });

      // Add detail rows if expanded
      if (hasDetails && expandedRows[row.badgeNumber.toString()]) {
        for (const detail of row.details) {
          rows.push({
            ...row,
            ...detail,
            isDetail: true,
            isExpandable: false,
            isExpanded: false,
            parentId: row.badgeNumber,
            suggestedForfeit: (detail as any).suggestedForfeit || 0
          });
        }
      }
    }

    return rows;
  }, [rehireForfeitures, expandedRows]);

  // Create column definitions with expand/collapse functionality
  const columnDefs = useMemo(() => {
    // Add an expansion column
    const expansionColumn = {
      headerName: "",
      field: "isExpandable",
      width: 50,
      cellRenderer: (params: any) => {
        if (params.data && !params.data.isDetail && params.data.isExpandable) {
          return params.data.isExpanded ? "▼" : "►";
        }
        return "";
      },
      onCellClicked: (event: any) => {
        if (event.data && !event.data.isDetail && event.data.isExpandable) {
          handleRowExpansion(event.data.badgeNumber.toString());
        }
      },
      suppressSizeToFit: true,
      suppressAutoSize: true,
      lockVisible: true,
      lockPosition: true,
      pinned: "left" as const
    };   

    return [expansionColumn, ...mainColumns, ...detailColumns];
  }, [mainColumns, detailColumns]);

  return (
    <div>
      <style>
        {`
          .detail-row {
            background-color: #f5f5f5;
          }
          .invalid-cell {
            background-color: #fff6f6;
          }
        `}
      </style>

      {rehireForfeitures?.response && (
        <>
          <Grid2 container justifyContent="space-between" alignItems="center" marginBottom={2}>
            <Grid2>
              <ReportSummary report={rehireForfeitures} />
            </Grid2>
          </Grid2>

          <DSMGrid
            preferenceKey={"QPREV-PROF"}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            maxHeight={800}
            providedOptions={{
              rowData: gridData,
              columnDefs: columnDefs,
              getRowClass: (params: any) => params.data.isDetail ? "detail-row" : "",
              rowSelection: "multiple",
              suppressRowClickSelection: true,
              rowHeight: 40,
              suppressMultiSort: true,
              defaultColDef: {
                resizable: true
              },
              onGridReady: onGridReady,
              context: {
                editedValues,
                updateEditedValue
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

