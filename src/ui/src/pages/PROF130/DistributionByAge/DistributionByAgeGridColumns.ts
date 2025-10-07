import { ColDef, ColGroupDef } from "ag-grid-community";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { createCurrencyColumn, createCountColumn, createAgeColumn } from "../../../utils/gridColumnFactory";

export const GetDistributionsByAgeColumns = (_reportType: FrozenReportsByAgeRequestType): (ColDef | ColGroupDef)[] => {
  // Flattened column set (no header group) to avoid group header render issues
  const columns: ColDef[] = [
    createAgeColumn({
      headerName: "Age",
      field: "age",
      minWidth: 80,
      sortable: false
    }),
    createCountColumn({
      headerName: "EMPS",
      field: "regularEmployeeCount",
      minWidth: 100,
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Amount",
      field: "regularAmount",
      minWidth: 150,
      sortable: false
    })
  ];
  return columns;
};
