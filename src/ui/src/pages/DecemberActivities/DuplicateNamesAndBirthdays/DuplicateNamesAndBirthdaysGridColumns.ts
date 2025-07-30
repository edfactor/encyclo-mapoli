import { ColDef } from "ag-grid-community";
import { yyyyMMDDToMMDDYYYY } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { createBadgeColumn, createCurrencyColumn, createSSNColumn, createDateColumn, createStoreColumn } from "../../../utils/gridColumnFactory";

export const GetDuplicateNamesAndBirthdayColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left"
    }),
    createSSNColumn({ alignment: "left" }),
    {
      headerName: "Name",
      field: "name",
      colId: "name",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
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
    {
      headerName: "City",
      field: "address.city",
      colId: "city",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
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
    {
      headerName: "Years",
      field: "years",
      colId: "years",
      minWidth: 60,
      type: "rightAligned",
      resizable: true
    },
    createStoreColumn({
      headerName: "Store #",
      minWidth: 60
    }),
    {
      headerName: "Hours",
      field: "hoursCurrentYear",
      colId: "hoursCurrentYear",
      minWidth: 60,
      type: "rightAligned",
      resizable: true
    },
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
    {
      headerName: "Employment Status",
      field: "employmentStatusName",
      colId: "employmentStatusName",
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
