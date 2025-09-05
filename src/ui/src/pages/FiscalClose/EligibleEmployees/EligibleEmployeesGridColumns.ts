import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createNameColumn,
  createStatusColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

export const GetEligibleEmployeesColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge"
    }),
    createNameColumn({
      field: "fullName"
    }),
    createStatusColumn({
      headerName: "Assignment",
      field: "departmentId",

      valueFormatter: (params) => {
        const name = params.data.department; // assuming 'statusName' is in the row data
        const id = params.data.departmentId; // assuming 'status' is in the row data
        return `[${id}] ${name}`;
      }
    }),
    createStoreColumn({})
  ];
};
