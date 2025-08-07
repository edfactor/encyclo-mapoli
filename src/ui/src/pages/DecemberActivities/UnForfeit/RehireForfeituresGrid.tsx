import { GridApi } from "ag-grid-community";
import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useSelector } from "react-redux";
import { useLazyGetRehireForfeituresQuery } from "reduxstore/api/YearsEndApi";
import { RootState } from "reduxstore/store";
import { DSMGrid, ISortParams, Pagination } from "smart-ui-library";
import useFiscalCalendarYear from "../../../hooks/useFiscalCalendarYear";
import {
  StartAndEndDateRequest,
  RehireForfeituresEditedValues,
  RehireForfeituresSelectedRow
} from "../../../reduxstore/types";
import { GetDetailColumns, GetMilitaryAndRehireForfeituresColumns } from "./RehireForfeituresGridColumns";
import ReportSummary from "../../../components/ReportSummary";
import { Grid } from "@mui/material";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { ForfeitureAdjustmentUpdateRequest } from "../../../reduxstore/types";
import { useUpdateForfeitureAdjustmentBulkMutation } from "reduxstore/api/YearsEndApi";
import { useUpdateForfeitureAdjustmentMutation } from "reduxstore/api/YearsEndApi";

interface MilitaryAndRehireForfeituresGridSearchProps {
  initialSearchLoaded: boolean;
  setInitialSearchLoaded: (loaded: boolean) => void;
  resetPageFlag: boolean;
  onUnsavedChanges: (hasChanges: boolean) => void;
  hasUnsavedChanges: boolean;
  shouldArchive: boolean;
}

const RehireForfeituresGrid: React.FC<MilitaryAndRehireForfeituresGridSearchProps> = ({
  initialSearchLoaded,
  setInitialSearchLoaded,
  resetPageFlag,
  onUnsavedChanges,
  hasUnsavedChanges,
  shouldArchive
}) => {
  const [pageNumber, setPageNumber] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortParams, setSortParams] = useState<ISortParams>({
    sortBy: "badgeNumber",
    isSortDescending: true
  });
  const [expandedRows, setExpandedRows] = useState<Record<string, boolean>>({});
  const [selectedRowIds, setSelectedRowIds] = useState<number[]>([]);
  const [gridApi, setGridApi] = useState<GridApi | null>(null);
  const [editedValues, setEditedValues] = useState<RehireForfeituresEditedValues>({});
  const [loadingRowIds, setLoadingRowIds] = useState<Set<number>>(new Set());
  const fiscalCalendarYear = useFiscalCalendarYear();
  const selectedProfitYear = useDecemberFlowProfitYear();
  const { rehireForfeitures, rehireForfeituresQueryParams } = useSelector((state: RootState) => state.yearsEnd);

  // Debug logging
  useEffect(() => {
    console.log('Grid props changed:', { shouldArchive, rehireForfeituresQueryParams });
  }, [shouldArchive, rehireForfeituresQueryParams]);

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
    (skip: number, sortBy: string, isSortDescending: boolean): (StartAndEndDateRequest & { archive?: boolean }) | null => {
      if (!rehireForfeituresQueryParams) return null;

      const baseRequest: StartAndEndDateRequest = {
        beginningDate: rehireForfeituresQueryParams.beginningDate || fiscalCalendarYear?.fiscalBeginDate || "",
        endingDate: rehireForfeituresQueryParams.endingDate || fiscalCalendarYear?.fiscalEndDate || "",
        pagination: { skip, take: pageSize, sortBy, isSortDescending }
      };

      // Add archive parameter only when shouldArchive is true
      const finalRequest = shouldArchive ? { ...baseRequest, archive: true } : baseRequest;
      console.log('createRequest called:', { shouldArchive, hasArchive: !!finalRequest.archive, finalRequest });
      return finalRequest;
    },
    [rehireForfeituresQueryParams, fiscalCalendarYear?.fiscalBeginDate, fiscalCalendarYear?.fiscalEndDate, pageSize, shouldArchive]
  );

  const handleBulkSave = useCallback(async (requests: ForfeitureAdjustmentUpdateRequest[]) => {
    // Add all affected badge numbers to loading state
    const badgeNumbers = requests.map(request => request.badgeNumber);
    setLoadingRowIds(prev => {
      const newSet = new Set(Array.from(prev));
      badgeNumbers.forEach(badgeNumber => newSet.add(badgeNumber));
      return newSet;
    });
    
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
      if (rehireForfeituresQueryParams) {
        const request = createRequest(pageNumber * pageSize, sortParams.sortBy, sortParams.isSortDescending);
        if (request) {
          await triggerSearch(request, false);
        }
      }
    } catch (error) {
      console.error('Failed to save forfeiture adjustments:', error);
      alert('Failed to save one or more adjustments. Please try again.');
    } finally {
      // Remove all affected badge numbers from loading state
      setLoadingRowIds(prev => {
        const newSet = new Set(Array.from(prev));
        badgeNumbers.forEach(badgeNumber => newSet.delete(badgeNumber));
        return newSet;
      });
    }
  }, [updateForfeitureAdjustmentBulk, editedValues, onUnsavedChanges, rehireForfeituresQueryParams, pageNumber, pageSize, sortParams, createRequest, triggerSearch]);

  const handleSave = useCallback(async (request: ForfeitureAdjustmentUpdateRequest) => {
    const rowId = request.badgeNumber; // Use badgeNumber as unique identifier
    setLoadingRowIds(prev => new Set(Array.from(prev).concat(rowId)));
    
    try {
      await updateForfeitureAdjustment(request);
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
    } catch (error) {
      console.error('Failed to save forfeiture adjustment:', error);
      alert('Failed to save adjustment. Please try again.');
    } finally {
      setLoadingRowIds(prev => {
        const newSet = new Set(Array.from(prev));
        newSet.delete(rowId);
        return newSet;
      });
    }
  }, [updateForfeitureAdjustment, editedValues, onUnsavedChanges, rehireForfeituresQueryParams, pageNumber, pageSize, sortParams, createRequest, triggerSearch]);

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
    console.log('Main search effect triggered:', { initialSearchLoaded, pageNumber, shouldArchive });
    if (initialSearchLoaded) {
      console.log('Calling performSearch with archive flag:', shouldArchive);
      performSearch(pageNumber * pageSize, sortParams.sortBy, sortParams.isSortDescending);
    }
  }, [initialSearchLoaded, pageNumber, pageSize, sortParams, performSearch]);

  // Effect to handle archive mode search - separate from normal search flow
  useEffect(() => {
    if (shouldArchive && rehireForfeituresQueryParams) {
      console.log('Archive mode triggered - performing search with archive=true');
      performSearch(pageNumber * pageSize, sortParams.sortBy, sortParams.isSortDescending);
    }
  }, [shouldArchive, rehireForfeituresQueryParams, pageNumber, pageSize, sortParams, performSearch]);

  // Reset page number to 0 when resetPageFlag changes
  useEffect(() => {
    console.log('resetPageFlag changed, setting page to 0');
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
        const hasDetails = (row.details && row.details.length > 0);
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
  const mainColumns = useMemo(() => GetMilitaryAndRehireForfeituresColumns(), []);
  const detailColumns = useMemo(() => GetDetailColumns(addRowToSelectedRows, removeRowFromSelectedRows, selectedProfitYear, handleSave, handleBulkSave), [selectedRowIds, selectedProfitYear, handleSave, handleBulkSave]);

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
            maxHeight={800}
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
