import {
    CellKeyDownEvent,
    ColDef,
    ColGroupDef,
    ColumnState,
    FullWidthCellKeyDownEvent,
    GridApi
} from "ag-grid-community";
import { AgGridReactProps } from "ag-grid-react";
import { FC, ReactNode, useEffect, useMemo, useState } from "react";
import AgGridWrapper from "./AgGridWrapper/AgGridWrapper";
import GridHeader from "./GridHeader/GridHeader";
import { ISortParams } from "./types";
import { onCellKeyDownEventHandler } from "./useEnterAsTab";

/**
 * Configuration options for DSMGrid component
 */
export interface DSMGridOptions {
  /** Custom controls to display in the grid header */
  controls?: ReactNode[];
  /** Loading state - shows loading overlay when true */
  isLoading: boolean;
  /** Unique key for localStorage persistence - must be unique per grid instance */
  readonly preferenceKey: string;
  /** Callback fired when user changes sort - used for server-side sorting */
  handleSortChanged?: (update: ISortParams) => void;
  /** Optional setter for grid API so that it may be accessed externally */
  setGridApi?: (gridApi: GridApi) => void;
  /** AG Grid configuration object - pass all AG Grid props here */
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  providedOptions: AgGridReactProps<any>;
  /** Whether to show the column visibility control dropdown (default: true) */
  showColumnControl?: boolean;
  /** Height constraint - omit for auto-height, provide for constrained height with scrolling */
  maxHeight?: string | number;
  /** Whether to enable the number pad enter key as an alternative tab (requires global app configuration to function) **/
  enableEnterToTab?: boolean;
}

/**
 * Extended ColumnState with index for internal state management
 */
export interface ColumnStateWIndex extends ColumnState {
  index: number;
}

/**
 * Default column definition for action columns (buttons, icons, etc.)
 */
// eslint-disable-next-line react-refresh/only-export-components
export const ActionColumnDefault: ColDef = {
  headerName: "Action",
  colId: "action",
  pinned: true,
  sortable: false,
  resizable: false,
  lockPosition: true,
  suppressMovable: true,
  suppressAutoSize: true,
  suppressSizeToFit: true,
  cellStyle: { textAlign: "center" },
  width: 100
};

/**
 * Enterprise-ready data grid component built on AG Grid Community.
 *
 * Features:
 * - Automatic column state persistence (width, order, visibility)
 * - Flexible height management (auto-height vs constrained)
 * - Built-in column controls and custom header controls
 * - Loading states and empty state handling
 * - Custom theming and DSM design system integration
 *
 * @example
 * ```tsx
 * <DSMGrid
 *   preferenceKey="user-grid"
 *   isLoading={false}
 *   maxHeight={400}
 *   providedOptions={{
 *     rowData: users,
 *     columnDefs: columnDefs,
 *     defaultColDef: { resizable: true, sortable: true }
 *   }}
 * />
 * ```
 */
export const DSMGrid: FC<DSMGridOptions> = (props) => {
  const { preferenceKey, controls, showColumnControl = true, enableEnterToTab = false } = props;
  const { columnDefs } = props.providedOptions;
  const [columnStateChanged, setColumnStateChanged] = useState<ColumnStateWIndex>({ index: -1, colId: "" });

  const [resetPreferences, setResetPreferences] = useState<boolean>(false);

  /**
   * Memoized column preferences from localStorage
   */
  const preferenceArray = useMemo(() => {
    // Defensive: if a caller accidentally passes an undefined preference key at runtime,
    // localStorage would coerce it to the string "undefined".
    if (typeof preferenceKey !== "string" || !preferenceKey || preferenceKey === "undefined" || preferenceKey === "null") {
      return [];
    }
    try {
      const stored = localStorage.getItem(preferenceKey);
      if (stored) {
        const parsed = JSON.parse(stored);
        if (Array.isArray(parsed)) {
          return parsed as ColumnState[];
        }
      }
    } catch {
      // Ignore parse errors
    }
    return [];
  }, [preferenceKey]);

  const [columnStates, setColumnStates] = useState<ColumnState[]>(preferenceArray);
  const [columnDefinitions, setColumnDefinitions] = useState<(ColDef | ColGroupDef)[]>(
    columnDefs as (ColDef | ColGroupDef)[]
  );

  //Add memoized coldefs here
  const providedOptions = {
    ...props.providedOptions,
    columnDefs: columnDefinitions
  };

  /**
   * Sync column states and definitions when preferences change
   */
  useEffect(() => {
    setColumnStates(preferenceArray);
    setColumnDefinitions(columnDefs as (ColDef | ColGroupDef)[]);
    // eslint-disable-next-line
  }, [preferenceArray]);

  // Memoize a combined onCellKeyDown handler for enter to tab functionality
  const onCellKeyDownCombined = useMemo(() => {
    // If enter to tab is not enabled, return the provided onCellKeyDown handler directly
    if (!enableEnterToTab) {
      return providedOptions.onCellKeyDown;
    }

    if (providedOptions.onCellKeyDown) {
      // Combine caller's handler and grid specific handler
      return (event: CellKeyDownEvent | FullWidthCellKeyDownEvent) => {
        providedOptions.onCellKeyDown?.(event);
        onCellKeyDownEventHandler(event);
      };
    }
    // Only grid specific handler
    return onCellKeyDownEventHandler;
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [enableEnterToTab, providedOptions.onCellKeyDown]);

  const updatedProps: DSMGridOptions = {
    ...props,
    providedOptions: {
      ...providedOptions,
      onCellKeyDown: onCellKeyDownCombined
    }
  };

  return (
    <>
      <GridHeader
        columnStates={columnStates}
        setColumnStates={setColumnStates}
        setColumnStateChanged={setColumnStateChanged}
        columnDefs={columnDefinitions}
        setReset={setResetPreferences}
        controls={controls}
        showColumnControl={showColumnControl}
      />
      <div className="w-full">
        <AgGridWrapper
          {...updatedProps}
          columnStateChanged={columnStateChanged}
          setColumnStates={setColumnStates}
          columnStates={columnStates}
          setResetPreferences={setResetPreferences}
          resetPreferences={resetPreferences}
        />
      </div>
    </>
  );
};

export default DSMGrid;
