import { ColDef } from "ag-grid-community";
import {
  createAddressColumn,
  createBadgeColumn,
  createCityColumn,
  createCountColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
  createSSNColumn,
  createStateColumn,
  createStatusColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

export const GetDuplicateNamesAndBirthdayColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createSSNColumn({}),
    createNameColumn({
      field: "name"
    }),
    createDateColumn({
      headerName: "DOB",
      field: "dateOfBirth"
    }),
    createAddressColumn({}),
    createCityColumn({
      field: "address.city",
      colId: "address.city"
    }),
    createStateColumn({
      headerName: "State",
      field: "address.state"
    }),
    createDateColumn({
      headerName: "Hire",
      field: "hireDate"
    }),
    createDateColumn({
      headerName: "Termination",
      field: "terminationDate"
    }),
    createCountColumn({
      headerName: "Years",
      field: "years"
    }),
    createStoreColumn({
      headerName: "Store #"
    }),
    createHoursColumn({
      field: "hoursCurrentYear"
    }),
    createCurrencyColumn({
      headerName: "Balance",
      field: "netBalance"
    }),
    createCurrencyColumn({
      headerName: "Income",
      field: "incomeCurrentYear"
    }),
    createStatusColumn({
      headerName: "Employment Status",
      field: "employmentStatusName",
      valueFormatter: (params) => {
        const id = params.data.status; // assuming 'status' is in the row data
        const name = params.data.employmentStatusName; // assuming 'statusName' is in the row data
        //see if one is undefined or null then show other
        return `[${id}] ${name}`;
      }
    })
  ];
};
