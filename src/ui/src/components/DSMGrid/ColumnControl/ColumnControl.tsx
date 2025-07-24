import LockIcon from "@mui/icons-material/Lock";
import ViewWeekSharpIcon from "@mui/icons-material/ViewWeekSharp";
import { Button, Checkbox } from "@mui/material";
import { ColDef, ColumnState } from "ag-grid-community";
import { FC, useMemo, useState } from "react";
import { ColumnStateWIndex } from "../DSMGrid";
type DropdownProps = {
  readonly columnDefs: ColDef[];
  columnStates: ColumnState[];
  setColumnStateChanged: Function;
  setColumnStates: Function;
  setReset: Function;
};

interface ColumnControlInfo extends ColumnStateWIndex {
  headerName: string;
  locked: boolean;
}

const ColumnControl: FC<DropdownProps> = ({
  columnStates,
  columnDefs,
  setColumnStates,
  setColumnStateChanged,
  setReset
}) => {
  const [columnDialog, setColumnDialog] = useState<boolean>(false);
  //Update local storage and global (relative to page) update value to trigger re-render of ag-grid
  const handleChange = (event: any, column: ColumnControlInfo) => {
    const index = column.index;
    column.hide = !event.target.checked;
    columnStates[index] = column;
    setColumnStates(columnStates);
    const { headerName, ...localObj } = column;
    setColumnStateChanged(localObj);
  };

  const handleReset = () => {
    setReset(true);
  };

  const columnsWithHeaders: ColumnControlInfo[] = useMemo(() => {
    return columnStates.flatMap((columnState, index) => {
      const columnDef = columnDefs.find(
        (column) => column.field === columnState.colId || column.colId === columnState.colId
      );
      if (!columnDef || !columnDef.headerName || columnDef.colId === "action") {
        return [];
      }
      return {
        ...columnState,
        headerName: columnDef.headerName,
        index: index,
        locked: columnDef.hide === false
      };
    });
  }, [columnDefs, columnStates]);

  return (
    <div style={{ position: "relative" }}>
      <div
        style={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          gap: "8px"
        }}>
        {!columnDialog ? (
          <Button
            color="primary"
            startIcon={<ViewWeekSharpIcon />}
            onClick={() => setColumnDialog(!columnDialog)}>
            COLUMNS
          </Button>
        ) : (
          <Button onClick={() => setColumnDialog(!columnDialog)}>Done</Button>
        )}
      </div>

      {columnDialog && (
        <div
          style={{
            width: "200px",
            height: "auto",
            boxShadow: "0 2px 8px rgba(0, 0, 0, 0.15)",
            position: "absolute",
            backgroundColor: "white",
            zIndex: 1000
          }}>
          {columnsWithHeaders.map((column, index) => {
            return (
              <div
                style={{ display: "flex", alignItems: "center" }}
                key={index}>
                {column.locked ? (
                  <LockIcon sx={{ padding: "9px", color: "#0258A5", height: "42px", width: "42px" }} />
                ) : (
                  <Checkbox
                    checked={!column.hide}
                    onChange={(event) => handleChange(event, column)}
                  />
                )}
                <span>{column.headerName}</span>
              </div>
            );
          })}
          <Button
            style={{ width: "100%", display: "flex" }}
            onClick={handleReset}>
            Reset
          </Button>
        </div>
      )}
    </div>
  );
};

export default ColumnControl;
