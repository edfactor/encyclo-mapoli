import { ColDef } from "ag-grid-community";
import {
  createAddressColumn,
  createBadgeColumn,
  createCityColumn,
  createDateColumn,
  createNameColumn,
  createSSNColumn,
  createStateColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

export const GetDuplicateSSNsOnDemographicsColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge"
    }),
    createSSNColumn({ alignment: "left" }),
    createNameColumn({
      field: "name"
    }),
    createAddressColumn({}),
    createCityColumn({
      valueGetter: (params) => params.data.address?.city || ""
    }),
    createStateColumn({
      valueGetter: (params) => params.data.address?.state || ""
    }),
    createDateColumn({
      headerName: "Hired Date",
      field: "hireDate"
    }),
    createDateColumn({
      headerName: "Rehired Date",
      field: "rehireDate"
    }),
    createStoreColumn({
      minWidth: 50
    }),
    {
      headerName: "Status",
      field: "status",
      colId: "status",
      minWidth: 60,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => {
        const id = params.data.status; // assuming 'status' is in the row data
        const name = params.data.employmentStatusName; // assuming 'statusName' is in the row data
        //see if one is undefined or null then show other
        return `[${id}] ${name}`;
      }
    }
  ];
};
