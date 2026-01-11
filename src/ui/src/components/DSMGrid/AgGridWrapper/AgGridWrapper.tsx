import {
  ColDef,
  ColGroupDef,
  ColumnState,
  GridApi,
  GridReadyEvent,
  iconSetAlpine,
  SortChangedEvent,
  themeQuartz
} from "ag-grid-community";
import { AgGridReact } from "ag-grid-react";
import { Component, ErrorInfo, FC, ReactNode, useEffect, useRef, useState } from "react";
import colors from "../colors";
import CustomLoadingOverlay from "../CustomLoading/CustomLoadingOverlay";
import { ColumnStateWIndex, DSMGridOptions } from "../DSMGrid";
import { ISortParams, SortDefault } from "../types";

// Suppress known ag-grid column group console errors
const originalConsoleError = console.error;
// eslint-disable-next-line @typescript-eslint/no-explicit-any
console.error = (...args: any[]) => {
  // Check if this is an error object
  const firstArg = args[0];
  const isError = firstArg instanceof Error;
  const errorMessage = isError ? firstArg.message : String(firstArg || "");
  // Ensure stackTrace is always a string before calling string methods
  const stackTrace = isError && typeof firstArg.stack === "string" ? firstArg.stack : "";

  // Join all arguments to check full message
  const fullMessage = args.map((arg) => String(arg)).join(" ");

  // Filter out known ag-grid column group errors
  if (
    errorMessage.includes("getProvidedColumnGroup") ||
    stackTrace.includes("getProvidedColumnGroup") ||
    fullMessage.includes("getProvidedColumnGroup") ||
    fullMessage.includes("AgGridErrorBoundary")
  ) {
    return;
  }

  originalConsoleError.apply(console, args);
};

const warnedInvalidPreferenceKeys = new Set<string>();

/**
 * Error boundary specifically for ag-grid column group rendering issues
 */
class AgGridErrorBoundary extends Component<{ children: ReactNode }, { hasError: boolean; errorCount: number }> {
  constructor(props: { children: ReactNode }) {
    super(props);
    this.state = { hasError: false, errorCount: 0 };
  }

  static getDerivedStateFromError(error: Error) {
    // Check if this is the specific ag-grid column group error
    if (error.message?.includes("getProvidedColumnGroup")) {
      return { hasError: true };
    }
    // Re-throw other errors
    throw error;
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    // Only log if it's not the known column group rendering issue
    if (!error.message?.includes("getProvidedColumnGroup")) {
      console.error("AgGrid Error Boundary caught:", error, errorInfo);
    }

    // Auto-recover after a brief delay
    setTimeout(() => {
      this.setState((prev) => ({
        hasError: false,
        errorCount: prev.errorCount + 1
      }));
    }, 0);
  }

  render() {
    if (this.state.hasError) {
      return (
        <div style={{ padding: "20px", textAlign: "center" }}>
          <p>Loading grid...</p>
        </div>
      );
    }

    return this.props.children;
  }
}

/**
 * DSM-themed AG Grid configuration with custom colors and fonts
 */
const dsmGridTheme = themeQuartz.withPart(iconSetAlpine).withParams({
  cellTextColor: "#231F20",
  fontFamily: {
    googleFont: "Lato"
  },
  fontSize: 14,
  headerBackgroundColor: colors["dsm-grid-header-bg"],
  headerFontSize: 14,
  headerFontWeight: "bold"
  // Note: pinnedRowBackgroundColor removed - not supported in current AG Grid version
  // Use CSS styling instead if pinned row styling is needed
});

/**
 * Extended options for AgGridWrapper including internal state management props
 */
interface AgGridWrapperOptions extends DSMGridOptions {
  /** Column state change from column controls */
  readonly columnStateChanged: ColumnStateWIndex;
  /** Current column states for persistence */
  readonly columnStates: ColumnState[];
  /** Whether preferences should be reset */
  readonly resetPreferences: boolean;
  /** Setter for column states */
  setColumnStates: (states: ColumnState[]) => void;
  /** Optional setter for grid API so that it may be accessed externally */
  setGridApi?: (gridApi: GridApi) => void;
  /** Setter for reset preferences flag */
  setResetPreferences: (value: boolean) => void;
}

/**
 * AG Grid integration layer that handles grid lifecycle, events, and state persistence.
 *
 * Key responsibilities:
 * - Grid initialization and state restoration
 * - Column state persistence to localStorage
 * - Event handling (resize, move, sort)
 * - Height management (auto vs constrained)
 * - Loading overlay management
 *
 * @param options - Combined DSMGrid options and internal state props
 */
const AgGridWrapper: FC<AgGridWrapperOptions> = ({
  isLoading,
  resetPreferences,
  setResetPreferences,
  columnStateChanged,
  columnStates,
  setColumnStates,
  preferenceKey,
  handleSortChanged,
  providedOptions,
  maxHeight,
  setGridApi
}) => {
  const ref = useRef<AgGridReact>(null);
  const [gridReady, setGridReady] = useState(false);

  const isValidPreferenceKey =
    typeof preferenceKey === "string" &&
    Boolean(preferenceKey) &&
    preferenceKey !== "undefined" &&
    preferenceKey !== "null";

  useEffect(() => {
    if (process.env.NODE_ENV !== "development" || isValidPreferenceKey) {
      return;
    }

    const keyLabel = String(preferenceKey);
    if (warnedInvalidPreferenceKeys.has(keyLabel)) {
      return;
    }

    warnedInvalidPreferenceKeys.add(keyLabel);
    console.warn(
      `[DSMGrid] Invalid preferenceKey detected (will skip persistence): "${keyLabel}". Investigate the call site passing preferenceKey.`,
      new Error("Invalid preferenceKey stack trace")
    );
  }, [isValidPreferenceKey, preferenceKey]);

  // Clear saved state for grids with column groups on mount to prevent corruption
  useEffect(() => {
    const hasColumnGroups = providedOptions.columnDefs?.some(
      (col: ColDef | ColGroupDef) => "children" in col && Array.isArray(col.children)
    );

    if (hasColumnGroups) {
      if (isValidPreferenceKey) {
        localStorage.removeItem(preferenceKey);
      }
      setColumnStates([]);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // Only run on mount

  // Extract provided options with custom defaults
  const {
    rowData,
    headerHeight,
    rowHeight,
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
    suppressMultiSort,
    maintainColumnOrder
  } = providedOptions;

  /**
   * Saves current column state to localStorage without triggering React re-renders.
   * Critical: Does not update React state to prevent infinite re-render loops.
   */
  const saveGridStateToStorage = () => {
    if (!ref?.current?.api || !gridReady) {
      return;
    }

    if (!isValidPreferenceKey) {
      return;
    }

    // Check if column definitions contain column groups
    const hasColumnGroups = providedOptions.columnDefs?.some(
      (col: ColDef | ColGroupDef) => "children" in col && Array.isArray(col.children)
    );

    // Skip state saving for grids with column groups to avoid state corruption
    if (hasColumnGroups) {
      return;
    }

    try {
      const columnState = ref.current.api.getColumnState();
      localStorage.setItem(preferenceKey, JSON.stringify(columnState));

      // DEVNOTE: Do NOT update React state during save operations!
      // This was causing component re-mounting after every drag operation
    } catch (err) {
      console.error("Error saving grid state:", err);
    }
  };

  // Note: Overlay states are now handled via the "loading" grid option (AG Grid v32+)
  // instead of using deprecated showLoadingOverlay/hideOverlay API methods

  /**
   * Handles resetting grid preferences - clears column state and resets grid
   */
  useEffect(() => {
    if (resetPreferences && gridReady && ref?.current?.api) {
      if (isValidPreferenceKey) {
        localStorage.removeItem(preferenceKey);
      }
      ref.current.api.resetColumnState();

      // Check if grid is not destroyed before sizing columns
      if (!ref.current.api.isDestroyed()) {
        ref.current.api.sizeColumnsToFit();
      }

      setColumnStates([]);
      setResetPreferences(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [resetPreferences, gridReady]);

  /**
   * Handles column control changes (show/hide columns from dropdown)
   */
  useEffect(() => {
    if (!ref?.current?.api || !columnStateChanged.colId || !gridReady) {
      return;
    }

    try {
      const columnState = ref.current.api.getColumnState();
      const { ...state } = columnStateChanged;

      // Find the actual column index by colId instead of using the filtered index, was giving issues in a custom master-detail grid
      const actualColumnIndex = columnState.findIndex((cs) => cs.colId === state.colId);

      if (actualColumnIndex !== -1) {
        columnState[actualColumnIndex] = {
          ...columnState[actualColumnIndex],
          ...state
        };

        ref.current.api.applyColumnState({
          state: columnState,
          applyOrder: true
        });

        saveGridStateToStorage();
      } else {
        console.warn(`Column with colId "${state.colId}" not found in grid column state`);
      }
    } catch (err) {
      console.error("Error updating column state:", err);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [columnStateChanged, gridReady]);

  /**
   * Handles the grid API setter
   */
  useEffect(() => {
    if (!ref?.current?.api || !gridReady || !setGridApi) {
      return;
    }
    setGridApi(ref.current.api);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [gridReady]);

  /**
   * Grid ready handler - initializes grid and restores saved column preferences.
   * Uses setTimeout to ensure grid is fully initialized before applying state.
   */
  const handleGridReady = (event: GridReadyEvent) => {
    setGridReady(true);

    // Call original onGridReady if provided
    if (onGridReady) {
      onGridReady(event);
    }

    // Check if column definitions contain column groups
    const hasColumnGroups = providedOptions.columnDefs?.some(
      (col: ColDef | ColGroupDef) => "children" in col && Array.isArray(col.children)
    );

    // Apply saved column states or size to fit if none exist
    // Skip state restoration for grids with column groups to avoid rendering issues
    if (columnStates.length > 0 && event?.api && !hasColumnGroups) {
      setTimeout(() => {
        if (event?.api && !event.api.isDestroyed()) {
          event.api.applyColumnState({
            state: columnStates,
            applyOrder: true
          });
        }
      }, 50);
    } else if (event?.api) {
      setTimeout(() => {
        if (event?.api && !event.api.isDestroyed()) {
          event.api.sizeColumnsToFit();
        }
      }, 50);
    }
  };

  /**
   * Sort change handler - manages both internal state persistence and external callbacks..
   */
  const onSortChangedHandler = (event: SortChangedEvent<unknown, unknown>) => {
    if (
      event &&
      event.columns &&
      event.source === "uiColumnSorted" &&
      rowData &&
      rowData.length > 0 &&
      handleSortChanged &&
      gridReady
    ) {
      const column = event.columns.find((c) => c.isSortNone() !== true);
      const updatedSort: ISortParams = column
        ? {
            sortBy: column.getColId(),
            isSortDescending: column.isSortDescending()
          }
        : SortDefault;

      saveGridStateToStorage();
      handleSortChanged(updatedSort);
    }

    if (onSortChanged) {
      onSortChanged(event);
    }
  };

  return (
    <AgGridErrorBoundary>
      <div
        style={{
          width: "100%",
          height: maxHeight ? (typeof maxHeight === "string" ? maxHeight : `${maxHeight}px`) : "100%",
          position: "relative",
          overflow: "hidden"
        }}>
        <AgGridReact
          {...providedOptions}
          theme={dsmGridTheme}
          loading={isLoading}
          suppressChangeDetection={suppressChangeDetection ?? false}
          suppressAutoSize={suppressAutoSize ?? false}
          suppressDragLeaveHidesColumns={suppressDragLeaveHidesColumns ?? true}
          headerHeight={headerHeight ?? 41}
          rowHeight={rowHeight ?? 41}
          domLayout={domLayout ?? (maxHeight ? "normal" : "autoHeight")}
          rowSelection={rowSelection ?? undefined}
          enableCellTextSelection={enableCellTextSelection ?? true}
          loadingOverlayComponent={CustomLoadingOverlay}
          overlayNoRowsTemplate={overlayNoRowsTemplate ?? "No records found"}
          onSortChanged={onSortChangedHandler}
          suppressMultiSort={suppressMultiSort ?? true}
          maintainColumnOrder={maintainColumnOrder ?? true}
          ref={ref}
          onGridReady={handleGridReady}
          onDragStopped={() => {
            saveGridStateToStorage();
          }}
          onColumnMoved={() => {
            saveGridStateToStorage();
          }}
          defaultColDef={
            handleSortChanged
              ? ({
                  ...defaultColDef,
                  comparator: function (_valueA, _valueB, _nodeA, _nodeB, _isDescending) {
                    return 0;
                  }
                } as typeof defaultColDef)
              : defaultColDef
          }
        />
      </div>
    </AgGridErrorBoundary>
  );
};

export default AgGridWrapper;
