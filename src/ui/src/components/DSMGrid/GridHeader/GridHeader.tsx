import React, { FC } from "react";
import ColumnControl from "../ColumnControl/ColumnControl";
import { ColDef, ColumnState } from "ag-grid-community";
import Grid from "@mui/material/Grid";

type GridHeaderProps = {
  columnStates: ColumnState[];
  columnDefs: ColDef[];
  setColumnStates: (value: ColumnState[]) => void;
  setSkipExport?: (value: boolean) => void;
  setSkipReport?: (value: boolean) => void;
  setColumnStateChanged: (value: ColumnState & { index: number }) => void;
  setReset: (value: boolean) => void;
  controls?: React.ReactNode[];
  showColumnControl?: boolean;
};

const GridHeader: FC<GridHeaderProps> = ({
  columnStates,
  columnDefs,
  setColumnStates,
  controls,
  setColumnStateChanged,
  setReset,
  showColumnControl = true
}) => {
  return (
    <>
      <Grid
        container
        padding={"0 24px 8px 24px"}
        gap={"8px"}
        justifyContent={"space-between"}>
        {showColumnControl && (
          <ColumnControl
            columnStates={columnStates}
            columnDefs={columnDefs}
            setColumnStates={setColumnStates}
            setColumnStateChanged={setColumnStateChanged}
            setReset={setReset}
          />
        )}

        {controls?.map((control, index) => {
          return <React.Fragment key={index}>{control}</React.Fragment>;
        })}
      </Grid>
    </>
  );
};

export default GridHeader;
