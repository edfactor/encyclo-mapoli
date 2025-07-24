import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";

export const GetAssociatesColumns = (handleNavigation: (badgeNumber: string) => void): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      width: 100,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber, handleNavigation)
    },
    {
      headerName: "Name",
      field: "fullName",
      flex: 1,
    },
    {
      headerName: "Position",
      field: "payClassificationName",
      flex: 1,
    },
    {
      headerName: "Beginning Balance",
      field: "beginningBalance",
      flex: 1,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Earnings",
      field: "earnings",
      flex: 1,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Contributions",
      field: "contributions",
      flex: 1,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Forfeiture",
      field: "forfeitures",
      flex: 1,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Distributions",
      field: "distributions",
      flex: 1,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Ending Balance",
      field: "endingBalance",
      flex: 1,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Vested Amount",
      field: "vestedAmount",
      flex: 1,
      valueFormatter: agGridNumberToCurrency
    }
  ];
};
