import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";

export const GetForfeituresAdjustmentColumns = (): ColDef[] => {
  const getColDefForForfeiture = (): ColDef[] => {
    return [
      {
        headerName: "Badge",
        field: "badgeNumber",
        width: 150,
        cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber)
      },
      {
        headerName: "Starting Balance",
        field: "startingBalance",
        width: 180,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Forfeiture Amount",
        field: "forfeitureAmount",
        width: 180,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Net Balance",
        field: "netBalance",
        width: 180,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Net Vested",
        field: "netVested",
        width: 180,
        valueFormatter: agGridNumberToCurrency
      }
    ];
  };

  return getColDefForForfeiture();
};
