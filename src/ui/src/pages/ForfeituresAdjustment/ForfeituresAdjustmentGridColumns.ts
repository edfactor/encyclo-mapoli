import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";

export const GetForfeituresAdjustmentColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  return [
    {
      headerName: "CLN NUM",
      field: "clientNumber",
      colId: "clientNumber",
      minWidth: 100,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "EMP NUM",
      field: "employeeNumber",
      colId: "employeeNumber",
      minWidth: 100,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Starting Balance",
      field: "startingBalance",
      colId: "startingBalance",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Forfeiture Amount",
      field: "forfeitureAmount",
      colId: "forfeitureAmount",
      minWidth: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Net Balance",
      field: "netBalance",
      colId: "netBalance",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Net Vested",
      field: "netVested",
      colId: "netVested",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    }
  ];
}; 