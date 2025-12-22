import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createNameColumn,
  createStatusColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

export const GetEligibleEmployeesColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createNameColumn({
      field: "fullName"
    }),
    createStatusColumn({
      headerName: "Assignment",
      field: "departmentId",

      valueFormatter: (params) => {
        return params.data.department; // assuming 'statusName' is in the row data
      }
    }),
    createStoreColumn({}),
    {
      headerName: "",
      field: "",
      flex: 1,
      sortable: false,
      filter: false
    }
  ];
};
