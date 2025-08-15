import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createNameColumn,
  createSSNColumn,
  createDateColumn,
  createCurrencyColumn,
  createStoreColumn
} from "utils/gridColumnFactory";

export const GetRehireForfeituresGridColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createNameColumn({
      field: "fullName"
    }),
    createSSNColumn({}),
    createDateColumn({
      headerName: "Hire Date",
      field: "hireDate"
    }),
    createDateColumn({
      headerName: "Termination Date",
      field: "terminationDate"
    }),
    createDateColumn({
      headerName: "Rehired Date",
      field: "reHiredDate"
    }),
    createCurrencyColumn({
      headerName: "Current Balance",
      field: "netBalanceLastYear"
    }),
    createCurrencyColumn({
      headerName: "Vested Balance",
      field: "vestedBalanceLastYear"
    }),
    createStoreColumn({})
  ];
};
