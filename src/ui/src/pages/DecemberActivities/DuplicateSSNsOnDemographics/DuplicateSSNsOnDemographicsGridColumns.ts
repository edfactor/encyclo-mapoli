import { ColDef } from "ag-grid-community";
import {
  createAddressColumn,
  createBadgeColumn,
  createCityColumn,
  createDateColumn,
  createNameColumn,
  createSSNColumn,
  createStateColumn,
  createStatusColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

export const GetDuplicateSSNsOnDemographicsColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createSSNColumn({}),
    createNameColumn({
      field: "name"
    }),
    createAddressColumn({
      field1: "street",
      field2: "street2"
    }),
    createCityColumn({
      valueGetter: (params) => params.data.address?.city || ""
    }),
    createStateColumn({
      valueGetter: (params) => params.data.address?.state || ""
    }),
    {
      headerName: "Current Hour Year",
      field: "currentHourYear",
      colId: "currentHourYear",
      maxWidth: 150, 
    },
    createDateColumn({
      headerName: "Termination Date",
      field: "terminationDate"
    }),
    createDateColumn({
      headerName: "Hired Date",
      field: "hireDate"
    }),
    createDateColumn({
      headerName: "Rehired Date",
      field: "rehireDate"
    }),
    createStoreColumn({}),
    createStatusColumn({
      valueFormatter: (params) => {
        const id = params.data.status; // assuming 'status' is in the row data
        const name = params.data.employmentStatusName; // assuming 'statusName' is in the row data
        //see if one is undefined or null then show other
        return `[${id}] ${name}`;
      }
    })
  ];
};
