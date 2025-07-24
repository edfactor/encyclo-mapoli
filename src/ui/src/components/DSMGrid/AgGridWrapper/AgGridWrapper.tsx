import { ColumnState, GridReadyEvent, SortChangedEvent } from "ag-grid-community";
import "ag-grid-community/styles/ag-grid.css";
import { AgGridReact } from "ag-grid-react";
import { FC, useEffect, useRef } from "react";
import CustomLoadingOverlay from "../CustomLoading/CustomLoadingOverlay";
import { ColumnStateWIndex, DSMGridOptions } from "../DSMGrid";
import { ISortParams, SortDefault } from "../types";

interface AgGridWrapperOptions extends DSMGridOptions {
  readonly columnStateChanged: ColumnStateWIndex;
  readonly columnStates: ColumnState[];
  readonly resetPreferences: boolean;
  setColumnStates: Function;
  setResetPreferences: Function;
}

const AgGridWrapper: FC<AgGridWrapperOptions> = ({
  isLoading,
  resetPreferences,
  setResetPreferences,
  columnStateChanged,
  columnStates,
  setColumnStates,
  preferenceKey,
  handleSortChanged,
  providedOptions
}) => {
  const ref = useRef<AgGridReact>(null);

  //The following all have custom defaults to mimic the design behavior.
  const {
    rowData,
    headerHeight,
    rowHeight,
    suppressRowClickSelection,
    groupSelectsChildren,
    domLayout,
    rowSelection,
    enableCellTextSelection,
    overlayNoRowsTemplate,
    onGridReady,
    onSortChanged,
    defaultColDef,
    suppressChangeDetection,
    suppressAutoSize,
    suppressDragLeaveHidesColumns,
    reactiveCustomComponents,
    suppressMultiSort,
    maintainColumnOrder
  } = providedOptions;

  useEffect(() => {
    if (!ref || !ref.current || !ref.current.api) {
      return;
    }

    if (isLoading) {
      ref.current.api.showLoadingOverlay();
    } else if (rowData === null || rowData.length === 0) {
      ref.current.api.showNoRowsOverlay();
    } else {
      ref.current.api.hideOverlay();
    }
  }, [isLoading, rowData]);

  useEffect(() => {
    if (resetPreferences) {
      updateLocalStorage(true);
      setResetPreferences(false);
    }
    // eslint-disable-next-line
  }, [resetPreferences]);

  //The following hook is fired off by the column control that is present in all DSM grids.
  useEffect(() => {
    if (!ref || !ref.current || !ref.current.api || !columnStateChanged.colId) {
      return;
    }

    ref.current.api.showLoadingOverlay();
    let columnState = ref.current.api.getColumnState();
    const { index, ...state } = columnStateChanged;
    columnState[index] = state;
    ref.current.api.applyColumnState({
      state: columnState,
      applyOrder: true
    });
    sizeToFit();
    ref.current.api.hideOverlay();
    updateLocalStorage(false, ref.current.api.getColumnState());
    // eslint-disable-next-line
  }, [columnStateChanged]);

  const updateLocalStorage = (reset: boolean, state?: ColumnState[]) => {
    if (!ref || !ref.current || !ref.current.api) {
      return;
    }

    if (reset) {
      ref.current.api.resetColumnState();
      sizeToFit();
      if (handleSortChanged && columnStates.find((c) => c.sort)) {
        handleSortChanged(SortDefault);
      }
    }

    let columnState = state && state.length > 0 ? state : ref.current.api.getColumnState();

    //Update Column Show/Hide Order
    setColumnStates(columnState);

    if (reset) {
      localStorage.removeItem(preferenceKey);
    } else {
      localStorage.setItem(preferenceKey, JSON.stringify(columnState));
    }
  };

  const handlePreferences = (event: GridReadyEvent) => {
    if (columnStates.length !== 0 && resetPreferences === false) {
      event.api.applyColumnState({
        state: columnStates,
        applyOrder: true
      });
    }
    sizeToFit();
    setColumnStates(event.api.getColumnState());
  };

  const onSortChangedHandler = (event: SortChangedEvent<any, any>) => {
    if (
      event &&
      event.columns &&
      event.source === "uiColumnSorted" &&
      rowData &&
      rowData.length > 0 &&
      handleSortChanged
    ) {
      const column = event.columns.find((c) => c.isSortNone() !== true);
      const updatedSort: ISortParams = column
        ? {
            sortBy: column.getColId(),
            isSortDescending: column.isSortDescending()
          }
        : SortDefault;
      updateLocalStorage(false);
      handleSortChanged(updatedSort);
    }
  };

  const sizeToFit = () => {
    ref?.current?.api?.sizeColumnsToFit();
  };

  const sizeAndSave = () => {
    sizeToFit();
    updateLocalStorage(false);
  };

  useEffect(() => {
    window.addEventListener("resize", () => sizeAndSave());

    return () => {
      window.removeEventListener("resize", () => sizeAndSave());
    };
  }, []);

  const delayedSizeToFit = async () => {
    setTimeout(() => {
      sizeToFit();
    }, 200);
  };

  return (
    <AgGridReact
      {...providedOptions}
      suppressChangeDetection={suppressChangeDetection ?? false}
      suppressAutoSize={suppressAutoSize ?? false}
      suppressDragLeaveHidesColumns={suppressDragLeaveHidesColumns ?? true}
      headerHeight={headerHeight ?? 41}
      rowHeight={rowHeight ?? 41}
      suppressRowClickSelection={suppressRowClickSelection ?? true}
      groupSelectsChildren={groupSelectsChildren ?? true}
      domLayout={domLayout ?? "autoHeight"}
      rowSelection={rowSelection ?? "multiple"}
      enableCellTextSelection={enableCellTextSelection ?? true}
      loadingOverlayComponent={CustomLoadingOverlay}
      overlayNoRowsTemplate={overlayNoRowsTemplate ?? "No records found"}
      reactiveCustomComponents={reactiveCustomComponents ?? true}
      onSortChanged={handleSortChanged ? onSortChangedHandler : onSortChanged}
      suppressMultiSort={suppressMultiSort ?? true}
      maintainColumnOrder={maintainColumnOrder ?? true}
      onRowDataUpdated={sizeToFit}
      onFirstDataRendered={delayedSizeToFit}
      ref={ref}
      onGridReady={
        onGridReady
          ? (e) => {
              onGridReady(e);
              handlePreferences(e);
            }
          : handlePreferences
      }
      onDragStopped={() => updateLocalStorage(false)}
      defaultColDef={
        handleSortChanged
          ? {
              ...defaultColDef,
              comparator: function (valueA, valueB, nodeA, nodeB, isDescending) {
                return 0;
              }
            }
          : defaultColDef
      }></AgGridReact>
  );
};

export default AgGridWrapper;
