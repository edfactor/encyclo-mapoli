import { agGridNumberToCurrency } from "smart-ui-library";
import { ColDef } from "ag-grid-community";

export const GetPay450GridColumns = (viewBadge: Function): ColDef[] => {
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
      headerName: "P/S Amount",
      field: "psAmountOriginal",
      colId: "psAmountOriginal",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "P/S Vested",
      field: "psVestedOriginal",
      colId: "psVestedOriginal",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Years",
      field: "yearsOriginal",
      colId: "yearsOriginal",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Enroll",
      field: "enrollOriginal",
      colId: "enrollOriginal",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "P/S Amount",
      field: "psAmountUpdated",
      colId: "psAmountUpdated",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "P/S Vested",
      field: "psVestedUpdated",
      colId: "psVestedUpdated",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Years",
      field: "yearsUpdated",
      colId: "yearsUpdated",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Enroll",
      field: "enrollUpdated",
      colId: "enrollUpdated",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    }
  ];
};
