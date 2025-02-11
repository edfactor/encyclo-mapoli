import { agGridNumberToCurrency } from "smart-ui-library";
import { ColDef } from "ag-grid-community";

export const GetEighteenToTwentyGridColumns = (viewBadge: Function): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badge",
      colId: "badge",
      minWidth: 100,
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
      minWidth: 180,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Store",
      field: "store",
      colId: "store",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Type",
      field: "type",
      colId: "type",
      minWidth: 80,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    {
      headerName: "Date of Birth",
      field: "dateOfBirth",
      colId: "dateOfBirth",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    {
      headerName: "Age",
      field: "age",
      colId: "age",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    {
      headerName: "Wages",
      field: "wages",
      colId: "wages",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Hours",
      field: "hours",
      colId: "hours",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Points",
      field: "points",
      colId: "points",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "New",
      field: "new",
      colId: "new",
      minWidth: 100,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    {
      headerName: "Term Date",
      field: "termDate",
      colId: "termDate",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    {
      headerName: "Current Balance",
      field: "currentBalance",
      colId: "currentBalance",
      minWidth: 140,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "SVC",
      field: "svc",
      colId: "svc",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    }
  ];
};