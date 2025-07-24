import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";

export const GetStoreManagementGridColumns = (handleNavigation: (badgeNumber: string) => void): ColDef[] => {
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
      width: 200
    },
    {
      headerName: "Position",
      field: "payClassificationName",
      width: 120
    },
    {
      headerName: "Beginning Balance",
      field: "beginningBalance",
      width: 150,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Earnings",
      field: "earnings",
      width: 120,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Contributions",
      field: "contributions",
      width: 120,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Forfeiture",
      field: "forfeitures",
      width: 120,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Distributions",
      field: "distributions",
      width: 120,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Ending Balance",
      field: "endingBalance",
      width: 150,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Vested Amount",
      field: "vestedAmount",
      width: 150,
      valueFormatter: agGridNumberToCurrency
    }
  ];
};
