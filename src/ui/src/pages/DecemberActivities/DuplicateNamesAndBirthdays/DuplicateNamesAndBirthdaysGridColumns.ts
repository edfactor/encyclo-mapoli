import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import {
  createBadgeColumn,
  createCityColumn,
  createCountColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
  createSSNColumn,
  createStatusColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

export const GetDuplicateNamesAndBirthdayColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left"
    }),
    createSSNColumn({ alignment: "left" }),
    createNameColumn({
      field: "name",
      minWidth: 150
    }),
    createDateColumn({
      headerName: "DOB",
      field: "dateOfBirth",
      minWidth: 100,
      alignment: "left"
    }),
    {
      headerName: "Address",
      field: "address",
      colId: "address",
      minWidth: 200,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueGetter: (params) => {
        const addr = params.data.address;
        return addr ? `${addr.street}${addr.street2 ? ", " + addr.street2 : ""}` : "";
      }
    },
    createCityColumn({
      field: "address.city",
      minWidth: 120,
      alignment: "left"
    }),
    {
      headerName: "State",
      field: "address.state",
      colId: "state",
      minWidth: 60,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    createDateColumn({
      headerName: "Hire",
      field: "hireDate",
      minWidth: 100,
      alignment: "left"
    }),
    createDateColumn({
      headerName: "Termination",
      field: "terminationDate",
      minWidth: 100,
      alignment: "left"
    }),
    createCountColumn({
      headerName: "Years",
      field: "years",
      minWidth: 60
    }),
    createStoreColumn({
      headerName: "Store #",
      minWidth: 60
    }),
    createHoursColumn({
      field: "hoursCurrentYear",
      minWidth: 60
    }),
    createCurrencyColumn({
      headerName: "Balance",
      field: "netBalance",
      minWidth: 60
    }),
    createCurrencyColumn({
      headerName: "Income",
      field: "incomeCurrentYear",
      minWidth: 60
    }),
    createStatusColumn({
      headerName: "Employment Status",
      field: "employmentStatusName",
      minWidth: 60,
      valueFormatter: (params) => {
        const id = params.data.status; // assuming 'status' is in the row data
        const name = params.data.employmentStatusName; // assuming 'statusName' is in the row data
        //see if one is undefined or null then show other
        return `[${id}] ${name}`;
      }
    })
  ];
};
