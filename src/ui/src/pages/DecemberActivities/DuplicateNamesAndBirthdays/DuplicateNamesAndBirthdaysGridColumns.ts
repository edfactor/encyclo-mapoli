import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency, yyyyMMDDToMMDDYYYY } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { createSSNColumn, createBadgeColumn } from "../../../utils/gridColumnFactory";

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
    {
      headerName: "DOB",
      field: "dateOfBirth",
      colId: "dateOfBirth",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => (params.value ? yyyyMMDDToMMDDYYYY(params.value) : "")
    },
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
    {
      headerName: "Hire",
      field: "hireDate",
      colId: "hireDate",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => (params.value ? yyyyMMDDToMMDDYYYY(params.value) : "")
    },
    {
      headerName: "Termination",
      field: "terminationDate",
      colId: "terminationDate",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => (params.value ? yyyyMMDDToMMDDYYYY(params.value) : "")
    },
    {
      headerName: "Years",
      field: "years",
      colId: "years",
      minWidth: 60,
      type: "rightAligned",
      resizable: true
    },
    {
      headerName: "Store #",
      field: "storeNumber",
      colId: "storeNumber",
      minWidth: 60,
      type: "rightAligned",
      resizable: true
    },
    {
      headerName: "Hours",
      field: "hoursCurrentYear",
      colId: "hoursCurrentYear",
      minWidth: 60,
      type: "rightAligned",
      resizable: true
    },
    {
      headerName: "Balance",
      field: "netBalance",
      colId: "netBalance",
      minWidth: 60,
      type: "rightAligned",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Income",
      field: "incomeCurrentYear",
      colId: "incomeCurrentYear",
      minWidth: 60,
      type: "rightAligned",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
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
