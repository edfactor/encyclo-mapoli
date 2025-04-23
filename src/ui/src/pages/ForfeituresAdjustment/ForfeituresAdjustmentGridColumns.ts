import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";

export const GetForfeituresAdjustmentColumns = (handleNavigation: (badgeNumber: string) => void): ColDef[] => {
  const getColDefForForfeiture = (): ColDef[] => {
    return [
      {
        headerName: "Client #",
        field: "clientNumber",
        filter: "agTextColumnFilter",
        width: 150
      },
      {
        headerName: "Badge #",
        field: "badgeNumber",
        filter: "agTextColumnFilter",
        width: 150
      },
      {
        headerName: "Starting Balance",
        field: "startingBalance",
        filter: "agTextColumnFilter",
        width: 180,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Forfeiture Amount",
        field: "forfeitureAmount",
        filter: "agTextColumnFilter",
        width: 180,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Net Balance",
        field: "netBalance",
        filter: "agTextColumnFilter",
        width: 180,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Net Vested",
        field: "netVested",
        filter: "agTextColumnFilter",
        width: 180,
        valueFormatter: agGridNumberToCurrency
      }
    ];
  };

  return getColDefForForfeiture();
}; 