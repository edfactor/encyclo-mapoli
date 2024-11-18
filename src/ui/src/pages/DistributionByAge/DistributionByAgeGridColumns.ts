import { ColDef } from "ag-grid-community";
import { formattingUtils } from "smart-ui-library";

export const GetDistributionsByAgeColumns = (): ColDef[] => {
  return [
    {
      headerName: "Age",
      field: "age",
      colId: "age",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sort: 'asc'
    },
    {
      headerName: "EMPS",
      field: "employeeCount",
      colId: "employeeCount",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Amount",
      field: "amount",
      colId: "amount",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: formattingUtils.agGridNumberToCurrency
    }
  ];
};