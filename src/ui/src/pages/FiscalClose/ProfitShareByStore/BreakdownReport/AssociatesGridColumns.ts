import { ColDef } from "ag-grid-community";
import { createBadgeColumn, createCurrencyColumn, createNameColumn } from "../../../../utils/gridColumnFactory";

export const GetAssociatesColumns = (handleNavigation: (badgeNumber: string) => void): ColDef[] => {
  return [
    createBadgeColumn({
      navigateFunction: handleNavigation
    }),
    {
      ...createNameColumn({
        field: "fullName"
      }),
      flex: 1
    },
    {
      headerName: "Position",
      field: "payClassificationName",
      flex: 1
    },
    {
      ...createCurrencyColumn({
        headerName: "Beginning Balance",
        field: "beginningBalance"
      }),
      flex: 1
    },
    {
      ...createCurrencyColumn({
        headerName: "Earnings",
        field: "earnings"
      }),
      flex: 1
    },
    {
      ...createCurrencyColumn({
        headerName: "Contributions",
        field: "contributions"
      }),
      flex: 1
    },
    {
      ...createCurrencyColumn({
        headerName: "Forfeiture",
        field: "forfeitures"
      }),
      flex: 1
    },
    {
      ...createCurrencyColumn({
        headerName: "Distributions",
        field: "distributions"
      }),
      flex: 1
    },
    {
      ...createCurrencyColumn({
        headerName: "Ending Balance",
        field: "endingBalance"
      }),
      flex: 1
    },
    {
      ...createCurrencyColumn({
        headerName: "Vested Amount",
        field: "vestedAmount"
      }),
      flex: 1
    }
  ];
};
