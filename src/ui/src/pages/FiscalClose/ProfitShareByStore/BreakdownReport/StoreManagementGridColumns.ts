import { ColDef } from "ag-grid-community";
import { createBadgeColumn, createCurrencyColumn, createNameColumn } from "../../../../utils/gridColumnFactory";

export const GetStoreManagementGridColumns = (handleNavigation: (badgeNumber: string) => void): ColDef[] => {
  return [
    createBadgeColumn({
      navigateFunction: handleNavigation
    }),
    createNameColumn({
      field: "fullName"
    }),
    {
      headerName: "Position",
      field: "payClassificationName",
      flex: 1
    },
    createCurrencyColumn({
      headerName: "Beginning Balance",
      field: "beginningBalance"
    }),
    createCurrencyColumn({
      headerName: "Earnings",
      field: "earnings"
    }),
    createCurrencyColumn({
      headerName: "Contributions",
      field: "contributions"
    }),
    createCurrencyColumn({
      headerName: "Forfeiture",
      field: "forfeitures"
    }),
    createCurrencyColumn({
      headerName: "Distributions",
      field: "distributions"
    }),
    createCurrencyColumn({
      headerName: "Ending Balance",
      field: "endingBalance"
    }),
    createCurrencyColumn({
      headerName: "Vested Amount",
      field: "vestedAmount"
    })
  ];
};
