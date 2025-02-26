import { ColDef } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";

export const GetProfitShareGrossReportColumns = (viewBadge: Function): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badge",
      colId: "badge",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      cellRenderer: viewBadge
    },
    {
      headerName: "Employee Name",
      field: "employeeName",
      colId: "employeeName",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Date of Birth",
      field: "dateOfBirth",
      colId: "dateOfBirth",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "P/S Wages",
      field: "psWages",
      colId: "psWages",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "P/S Amount",
      field: "psAmount",
      colId: "psAmount",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Loans",
      field: "loans",
      colId: "loans",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Forfeitures",
      field: "forfeitures",
      colId: "forfeitures",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "EC",
      field: "ec",
      colId: "ec",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    }
  ];
};