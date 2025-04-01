import { agGridNumberToCurrency } from "smart-ui-library";
import { ColDef } from "ag-grid-community";

export const ProfitShareEditUpdateGridColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "psn",
      colId: "psn",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Contributions",
      field: "contributionAmount",
      colId: "contributionAmount",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Earnings",
      field: "earningsAmount",
      colId: "earningsAmount",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Incoming Forfeiture",
      field: "forfeitureAmount",
      colId: "forfeitureAmount",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Reason",
      field: "recordChangeSummary",
      colId: "recordChangeSummary",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Code",
      field: "code",
      colId: "code",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Name",
      field: "name",
      colId: "name",
      minWidth: 80,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
};
