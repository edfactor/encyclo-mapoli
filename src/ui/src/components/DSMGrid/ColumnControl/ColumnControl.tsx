import LockIcon from "@mui/icons-material/Lock";
import ViewWeekSharpIcon from "@mui/icons-material/ViewWeekSharp";
import { Button, Checkbox } from "@mui/material";
import { ColDef, ColumnState } from "ag-grid-community";
import { FC, useMemo, useState, useRef, useEffect } from "react";

type DropdownProps = {
  readonly columnDefs: ColDef[];
  columnStates: ColumnState[];
  setColumnStateChanged: (value: ColumnState & { index: number }) => void;
  setColumnStates: (value: ColumnState[]) => void;
  setReset: (value: boolean) => void;
};

interface ColumnControlInfo extends ColumnState {
  headerName: string;
  locked: boolean;
  originalIndex: number; // The original position in columnDefs array
}

const ColumnControl: FC<DropdownProps> = ({
  columnStates,
  columnDefs,
  setColumnStates,
  setColumnStateChanged,
  setReset
}) => {
  const [columnDialog, setColumnDialog] = useState<boolean>(false);
  const dialogRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dialogRef.current && !dialogRef.current.contains(event.target as Node)) {
        setColumnDialog(false);
      }
    };

    if (columnDialog) {
      // Small timeout to avoid conflicts with button clicks
      const timeoutId = setTimeout(() => {
        document.addEventListener("click", handleClickOutside);
      }, 0);

      return () => {
        clearTimeout(timeoutId);
        document.removeEventListener("click", handleClickOutside);
      };
    }
  }, [columnDialog]);

  //Update local storage and global (relative to page) update value to trigger re-render of ag-grid
  const handleChange = (event: React.ChangeEvent<HTMLInputElement>, column: ColumnControlInfo) => {
    // Find the corresponding column state or create new one
    const existingStateIndex = columnStates.findIndex((state) => state.colId === column.colId);

    const updatedColumn = {
      ...column,
      hide: !event.target.checked
    };

    const newColumnStates = [...columnStates];

    if (existingStateIndex >= 0) {
      newColumnStates[existingStateIndex] = updatedColumn;
    } else {
      newColumnStates.push(updatedColumn);
    }

    setColumnStates(newColumnStates);
    const { originalIndex, ...localObj } = updatedColumn;

    // Pass the originalIndex as index to match the expected ColumnStateWIndex
    setColumnStateChanged({ ...localObj, index: originalIndex });
  };

  const handleReset = () => {
    setReset(true);
  };

  const columnsWithHeaders: ColumnControlInfo[] = useMemo(() => {
    // First we create an array with original indices, THEN filter
    return columnDefs
      .map((columnDef, originalIndex) => {
        if (!columnDef.headerName || columnDef.colId === "action") {
          return null;
        }

        const existingState = columnStates.find((state) => state.colId === (columnDef.field || columnDef.colId));

        return {
          colId: columnDef.field || columnDef.colId || "",
          hide: existingState?.hide ?? false,
          headerName: columnDef.headerName!,
          locked: columnDef.hide === false,
          originalIndex,
          ...existingState
        };
      })
      .filter((item): item is NonNullable<typeof item> => item !== null);
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
            onClick={() => setColumnDialog(true)}>
            COLUMNS
          </Button>
        ) : (
          <Button onClick={() => setColumnDialog(false)}>Done</Button>
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
          }}
          ref={dialogRef}>
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
