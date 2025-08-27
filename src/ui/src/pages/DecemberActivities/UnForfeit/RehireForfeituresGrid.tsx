import { Grid } from "@mui/material";
import { ColDef, GridApi, ICellRendererParams } from "ag-grid-community";
import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import {
  useLazyGetRehireForfeituresQuery,
  useUpdateForfeitureAdjustmentBulkMutation,
  useUpdateForfeitureAdjustmentMutation
} from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import ReportSummary from "../../../components/ReportSummary";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import useFiscalCalendarYear from "../../../hooks/useFiscalCalendarYear";
import {
  ForfeitureAdjustmentUpdateRequest,
  RehireForfeituresEditedValues,
  StartAndEndDateRequest
} from "../../../reduxstore/types";
import { GetProfitDetailColumns } from "./RehireForfeituresProfitDetailGridColumns";

import { GetRehireForfeituresGridColumns } from "./RehireForfeituresGridColumns";

interface RehireForfeituresGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  resetPageFlag: boolean;
  onUnsavedChanges: (hasChanges: boolean) => void;
  hasUnsavedChanges: boolean;
  shouldArchive: boolean;
  onArchiveHandled?: () => void;
}

const RehireForfeituresGrid: React.FC<RehireForfeituresGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  resetPageFlag,
  onUnsavedChanges,
  hasUnsavedChanges,
  shouldArchive,
  onArchiveHandled
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "fullName",
    isSortDescending: false
  });
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});
  const [selectedRowIds, setSelectedRowIds] = useState<number[]>([]);
  const [gridApi, setGridApi] = useState<GridApi | null>(null);
  const [editedValues, setEditedValues] = useState<RehireForfeituresEditedValues>({});
  const [loadingRowIds, setLoadingRowIds] = useState<Set<number>>(new Set());
  const fiscalCalendarYear = useFiscalCalendarYear();
  const selectedProfitYear = useDecemberFlowProfitYear();
  const { rehireForfeitures, rehireForfeituresQueryParams } = useSelector((state: RootState) => state.yearsEnd);

  const [triggerSearch, { isFetching }] = useLazyGetRehireForfeituresQuery();
  const [updateForfeitureAdjustmentBulk] = useUpdateForfeitureAdjustmentBulkMutation();
  const [updateForfeitureAdjustment] = useUpdateForfeitureAdjustmentMutation();

  const onGridReady = useCallback((params: { api: GridApi }) => {
    setGridApi(params.api);
  }, []);

  const addRowToSelectedRows = (id: number) => {
    setSelectedRowIds([...selectedRowIds, id]);
  };

  const removeRowFromSelectedRows = (id: number) => {
    setSelectedRowIds(selectedRowIds.filter((rowId) => rowId !== id));
  };

  const updateEditedValue = useCallback((rowKey: string, value: number, hasError: boolean) => {
    setEditedValues((prev) => ({
      ...prev,
      [rowKey]: { value, hasError }
    }));
  }, []);

  // Need a useEffect to reset the page number when total count changes (new search, not pagination)
  const prevRehireForfeitures = useRef<any>(null);
  useEffect(() => {
    if (
      rehireForfeitures !== prevRehireForfeitures.current &&
      rehireForfeitures?.response?.total !== undefined &&
      rehireForfeitures.response.total !== prevRehireForfeitures.current?.response?.total
    ) {
      setPageNumber(0);
    }
    prevRehireForfeitures.current = rehireForfeitures;
  }, [rehireForfeitures]);

  // Create a request object based on current parameters
  const createRequest = useCallback(
    (
      skip: number,
      sortBy: string,
      isSortDescending: boolean
      //profitYear: number
    ): (StartAndEndDateRequest & { archive?: boolean }) | null => {
      // Build request using query params when present, otherwise fall back to fiscal dates
      const baseRequest: StartAndEndDateRequest = {
        beginningDate: rehireForfeituresQueryParams?.beginningDate || fiscalCalendarYear?.fiscalBeginDate || "",
        endingDate: rehireForfeituresQueryParams?.endingDate || fiscalCalendarYear?.fiscalEndDate || "",
        profitYear: selectedProfitYear,
        pagination: { skip, take: pageSize, sortBy, isSortDescending }
      };

      // If we still don't have dates, do not issue a request
      if (!baseRequest.beginningDate || !baseRequest.endingDate) return null;

      // Add archive parameter only when shouldArchive is true
      const finalRequest = shouldArchive ? { ...baseRequest, archive: true } : baseRequest;
      return finalRequest;
    },
    [
      rehireForfeituresQueryParams,
      fiscalCalendarYear?.fiscalBeginDate,
      fiscalCalendarYear?.fiscalEndDate,
      pageSize,
      shouldArchive
    ]
  );

  const handleBulkSave = useCallback(
    async (requests: ForfeitureAdjustmentUpdateRequest[]) => {
      // Add all affected badge numbers to loading state
      const badgeNumbers = requests.map((request) => request.badgeNumber);
      setLoadingRowIds((prev) => {
        const newSet = new Set(Array.from(prev));
        badgeNumbers.forEach((badgeNumber) => newSet.add(badgeNumber));
        return newSet;
      });

      try {
        await updateForfeitureAdjustmentBulk(requests);
        const updatedEditedValues = { ...editedValues };
        requests.forEach((request) => {
          const rowKey = `${request.badgeNumber}-${request.profitYear}`;
          delete updatedEditedValues[rowKey];
        });
        setEditedValues(updatedEditedValues);
        setSelectedRowIds([]);
        onUnsavedChanges(Object.keys(updatedEditedValues).length > 0);
        if (rehireForfeituresQueryParams) {
          const request = createRequest(
            pageNumber * pageSize,
            sortParams.sortBy,
            sortParams.isSortDescending
            //selectedProfitYear
          );
          if (request) {
            await triggerSearch(request, false);
          }
        }
      } catch (error) {
        console.error("Failed to save forfeiture adjustments:", error);
        alert("Failed to save one or more adjustments. Please try again.");
      } finally {
        // Remove all affected badge numbers from loading state
        setLoadingRowIds((prev) => {
          const newSet = new Set(Array.from(prev));
          badgeNumbers.forEach((badgeNumber) => newSet.delete(badgeNumber));
          return newSet;
        });
      }
    },
    [
      updateForfeitureAdjustmentBulk,
      editedValues,
      onUnsavedChanges,
      rehireForfeituresQueryParams,
      pageNumber,
      pageSize,
      sortParams,
      createRequest,
      triggerSearch
    ]
  );

  const handleSave = useCallback(
    async (request: ForfeitureAdjustmentUpdateRequest) => {
      const rowId = request.badgeNumber; // Use badgeNumber as unique identifier
      setLoadingRowIds((prev) => new Set(Array.from(prev).concat(rowId)));

      try {
        const result = await updateForfeitureAdjustment({ ...request, suppressAllToastErrors: true });
        if (result?.error) {
          alert("Save failed. One or more unforfeits were related to a class action forfeit.");
        } else {
          const rowKey = `${request.badgeNumber}-${request.profitYear}`;
          const updatedEditedValues = { ...editedValues };
          delete updatedEditedValues[rowKey];
          setEditedValues(updatedEditedValues);
          onUnsavedChanges(Object.keys(updatedEditedValues).length > 0);
          if (rehireForfeituresQueryParams) {
            const searchRequest = createRequest(pageNumber * pageSize, sortParams.sortBy, sortParams.isSortDescending);
            if (searchRequest) {
              await triggerSearch(searchRequest, false);
            }
          }
        }
      } catch (error) {
        console.error("Failed to save forfeiture adjustment:", error);
        alert("Failed to save adjustment. Please try again.");
      } finally {
        setLoadingRowIds((prev) => {
          const newSet = new Set(Array.from(prev));
          newSet.delete(rowId);
          return newSet;
        });
      }
    },
    [
      updateForfeitureAdjustment,
      editedValues,
      onUnsavedChanges,
      rehireForfeituresQueryParams,
      pageNumber,
      pageSize,
      sortParams,
      createRequest,
      triggerSearch
    ]
  );

  const performSearch = useCallback(
    async (skip: number, sortBy: string, isSortDescending: boolean) => {
      const request = createRequest(skip, sortBy, isSortDescending); //, selectedProfitYear);
      if (request) {
        await triggerSearch(request, false);
      }
    },
    [createRequest, triggerSearch]
  );

  // Effect to handle initial load and pagination changes
  useEffect(() => {
    if (initialSearchLoaded) {
      performSearch(pageNumber * pageSize, sortParams.sortBy, sortParams.isSortDescending);
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, performSearch]);

  // Effect to handle archive mode search - separate from normal search flow
  useEffect(() => {
    if (!shouldArchive) return;

    let cancelled = false;
    const run = async () => {
      await performSearch(pageNumber * pageSize, sortParams.sortBy, sortParams.isSortDescending);
      if (!cancelled) {
        onArchiveHandled?.();
      }
    };

    run();

    // Re-attempt when fiscal dates or selected profit year become available after a refresh
    return () => {
      cancelled = true;
    };
  }, [
    shouldArchive,
    pageNumber,
    pageSize,
    sortParams,
    performSearch,
    fiscalCalendarYear?.fiscalBeginDate,
    fiscalCalendarYear?.fiscalEndDate,
    selectedProfitYear,
    onArchiveHandled
  ]);

  // Reset page number to 0 when resetPageFlag changes
  useEffect(() => {
    setPageNumber(0);
  }, [resetPageFlag]);

  useEffect(() => {
    const hasChanges = selectedRowIds.length > 0;
    onUnsavedChanges(hasChanges);
  }, [selectedRowIds, onUnsavedChanges]);

  // Initialize expandedRows when data is loaded
  useEffect(() => {
    if (rehireForfeitures?.response?.results && rehireForfeitures.response.results.length > 0) {
      const initialExpandState: Record<string, boolean> = {};
      rehireForfeitures.response.results.forEach((row: any) => {
        // In this component, details are under the "details" property
        const hasDetails = row.details && row.details.length > 0;
        if (hasDetails) {
          // Always expand rows with details by default
          initialExpandState[row.badgeNumber.toString()] = true;
        }
      });
      setExpandedRows(initialExpandState);
    }
  }, [rehireForfeitures?.response?.results]);

  // Refresh the grid when loading state changes
  const gridRef = useRef<any>(null);
  useEffect(() => {
    if (gridRef.current?.api) {
      gridRef.current.api.refreshCells({
        force: true,
        suppressFlash: false
      });
    }
  }, [loadingRowIds]);

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
  const mainColumns = useMemo(() => GetRehireForfeituresGridColumns(), []);
  const detailColumns = useMemo(
    () =>
      GetProfitDetailColumns(
        addRowToSelectedRows,
        removeRowFromSelectedRows,
        selectedProfitYear,
        handleSave,
        handleBulkSave
      ),
    [selectedRowIds, selectedProfitYear, handleSave, handleBulkSave]
  );

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
        let index = 0; // essentially we only want to allow UNFORFEIT on the first FORFEIT
        for (const detail of row.details) {
          rows.push({
            ...row,
            ...detail,
            isDetail: true,
            isExpandable: false,
            isExpanded: false,
            parentId: row.badgeNumber,
            index: index
          });
          index++;
        }
      }
    }

    return rows;
  }, [rehireForfeitures, expandedRows]);

  // Create column definitions with expand/collapse functionality and combine main/detail columns
  const columnDefs = useMemo(() => {
    // Add an expansion column
    const expansionColumn = {
      headerName: "",
      field: "isExpandable",
      width: 50,
      cellRenderer: (params: ICellRendererParams) => {
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
    } as ColDef;

    // For main columns, hide content for detail rows unless the same field exists in detail columns
    const visibleColumns = mainColumns.map((column) => {
      return {
        ...column,
        cellRenderer: (params: ICellRendererParams) => {
          if (params.data?.isDetail) {
            const hideInDetails = !detailColumns.some((detailCol) => detailCol.field === (column as any).field);
            if (hideInDetails) {
              return "";
            }
          }

          if ((column as any).cellRenderer) {
            return (column as any).cellRenderer(params);
          }
          return params.valueFormatted ? params.valueFormatted : params.value;
        }
      } as ColDef;
    });

    // Add detail-specific columns that only appear for detail rows
    const detailOnlyColumns = detailColumns
      .filter((detailCol) => !mainColumns.some((mainCol) => mainCol.field === detailCol.field))
      .map(
        (column) =>
          ({
            ...column,
            cellRenderer: (params: ICellRendererParams) => {
              if (!params.data?.isDetail) {
                return "";
              }
              if ((column as any).cellRenderer) {
                return (column as any).cellRenderer(params);
              }
              return params.valueFormatted ? params.valueFormatted : params.value;
            }
          }) as ColDef
      );

    return [expansionColumn, ...visibleColumns, ...detailOnlyColumns];
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
          <Grid
            container
            justifyContent="space-between"
            alignItems="center"
            marginBottom={2}>
            <Grid>
              <ReportSummary report={rehireForfeitures} />
            </Grid>
          </Grid>

          <DSMGrid
            preferenceKey={"REHIRE-FORFEITURES"}
            isLoading={isFetching}
            handleSortChanged={sortEventHandler}
            maxHeight={400}
            providedOptions={{
              rowData: gridData,
              columnDefs: columnDefs,
              getRowClass: (params: any) => (params.data.isDetail ? "detail-row" : ""),
              rowSelection: "multiple",
              suppressRowClickSelection: true,
              rowHeight: 40,
              suppressMultiSort: true,
              defaultColDef: {
                resizable: true
              },
              onGridReady: (params) => {
                gridRef.current = params;
                onGridReady(params);
              },
              context: {
                editedValues,
                updateEditedValue,
                loadingRowIds
              }
            }}
          />

          {!!rehireForfeitures && rehireForfeitures.response.results.length > 0 && (
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
                setPageNumber(1);
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
