import { ColDef, ColGroupDef, ColumnState } from "ag-grid-community";
import "ag-grid-community/styles/ag-theme-alpine.css";
import { AgGridReactProps } from "ag-grid-react";
import { FC, ReactNode, useEffect, useMemo, useState } from "react";
import "./DSMGrid.css";
import { ISortParams } from "./types";
import AgGridWrapper from "./AgGridWrapper/AgGridWrapper";
import GridHeader from "./GridHeader/GridHeader";

export interface DSMGridOptions {
  controls?: ReactNode[];
  isLoading: boolean;
  readonly preferenceKey: string;
  handleSortChanged?: (update: ISortParams) => void;
  /** Whether to show the column visibility control dropdown (default: true) */
  showColumnControl?: boolean;
  providedOptions: AgGridReactProps<any>;
}

export interface ColumnStateWIndex extends ColumnState {
  index: number;
}

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

export const DSMGrid: FC<DSMGridOptions> = ({ isLoading, providedOptions, preferenceKey, controls, showColumnControl = true, handleSortChanged }) => {
  const { columnDefs } = providedOptions;
  const [columnStateChanged, setColumnStateChanged] = useState<ColumnStateWIndex>({ index: -1, colId: "" });

  const [resetPreferences, setResetPreferences] = useState<boolean>(false);

  const preferenceArray = useMemo(() => {
    const preferencesPresent = localStorage.getItem(preferenceKey) ?? "[]";
    let preferenceArray: ColumnState[];
    try {
      preferenceArray = JSON.parse(preferencesPresent);
    } catch (e) {
      preferenceArray = [];
    }
    return preferenceArray;
  }, [preferenceKey]);

  const [columnStates, setColumnStates] = useState<ColumnState[]>(preferenceArray);
  const [columnDefinitions, setColumnDefinitions] = useState<(ColDef | ColGroupDef)[]>(columnDefs as (ColDef | ColGroupDef)[]);

  /*
   *   When preferenceArray is updated, a unique set of columns has been loaded.
   *   Update the column state with preferences and column definitions.
   */
  useEffect(() => {
    setColumnStates(preferenceArray);
    setColumnDefinitions(columnDefs as (ColDef | ColGroupDef)[]);
    // eslint-disable-next-line
  }, [preferenceArray]);

  //Add memoized props here
  const props: DSMGridOptions = {
    isLoading,
    preferenceKey,
    controls,
    showColumnControl,
    handleSortChanged,
    providedOptions: {
      ...providedOptions,
      columnDefs: columnDefinitions
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
      <div className="ag-theme-alpine fullWidth fullHeight">
        <AgGridWrapper
          {...props}
          columnStateChanged={columnStateChanged}
          setColumnStates={setColumnStates}
          columnStates={columnStates}
          setResetPreferences={setResetPreferences}
          resetPreferences={resetPreferences}></AgGridWrapper>
      </div>
    </>
  );
};

export default DSMGrid;
