import { ColDef } from "ag-grid-community";
import {
  createBadgeOrPSNColumn,
  createCountColumn,
  createCurrencyColumn,
  createNameColumn,
  createSSNColumn
} from "../../../utils/gridColumnFactory";

export const GetProfitShareForfeitColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  return [
    createBadgeOrPSNColumn({
      headerName: "Badge/PSN",
      field: "badgeOrPsn",
      badgeField: "badgeNumber",
      psnField: "beneficiaryPsn",
      navigateFunction: navFunction,
      enableLinking: true
    }),
    createNameColumn({
      field: "employeeName"
    }),
    createSSNColumn({}),
    createCurrencyColumn({
      headerName: "Forfeitures",
      field: "forfeitures"
    }),

    createCountColumn({
      headerName: "Cont/Forfeit Points",
      field: "contForfeitPoints"
    }),
    createCountColumn({
      headerName: "Earnings Points",
      field: "earningPoints"
    })
  ];
};
