import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { createBadgeColumn, createNameColumn, createStatusColumn } from "../../../utils/gridColumnFactory";

export const GetEligibleEmployeesColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left"
    }),
    createNameColumn({
      field: "fullName",
      minWidth: GRID_COLUMN_WIDTHS.FULL_NAME
    }),
    createStatusColumn({
      headerName: "Assignment",
      field: "departmentId",
      minWidth: 150,
      valueFormatter: (params) => {
        const name = params.data.department; // assuming 'statusName' is in the row data
        const id = params.data.departmentId; // assuming 'status' is in the row data
        return `[${id}] ${name}`;
      }
    }),
    {
      headerName: "Store",
      field: "storeNumber",
      colId: "storeNumber",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
};
