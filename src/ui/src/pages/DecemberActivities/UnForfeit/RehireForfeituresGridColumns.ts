import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCountColumn,
  createCurrencyColumn,
  createDateColumn,
  createNameColumn,
  createSSNColumn,
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
    createStoreColumn({}),
    createCountColumn({
      headerName: "Years",
      field: "companyContributionYears"
    }),
    {
      headerName: "Enrollment",
      width: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      valueGetter: (params) => {
        const id = params.data?.enrollmentId;
        const name = params.data?.enrollmentName;
        if (!id || !name) return "-";
        return `[${id}] ${name}`;
      }
    }
  ];
};
