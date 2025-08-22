import { ColDef } from "ag-grid-community";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { createCurrencyColumn, createCountColumn, createAgeColumn } from "../../../utils/gridColumnFactory";

export const GetContributionsByAgeColumns = (_reportType: FrozenReportsByAgeRequestType): ColDef[] => {
  return [
    createAgeColumn({
      headerName: "Age",
      field: "age",
      minWidth: 80,
      sortable: false
    }),
    createCountColumn({
      headerName: "EMPS",
      field: "employeeCount",
      minWidth: 100,
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Amount",
      field: "amount",
      minWidth: 150,
      sortable: false
    })
  ];
};
