import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCountColumn,
  createCurrencyColumn,
  createNameColumn,
  createSSNColumn
} from "../../../utils/gridColumnFactory";

export const GetProfitShareForfeitColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
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
    }),
    createBadgeColumn({
      headerName: "Beneficiary PSN",
      field: "beneficiaryPsn"
    })
  ];
};
