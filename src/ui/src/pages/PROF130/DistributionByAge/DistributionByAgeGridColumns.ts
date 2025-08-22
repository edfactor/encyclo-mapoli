import { ColDef, ColGroupDef } from "ag-grid-community";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { createCurrencyColumn, createCountColumn } from "../../../utils/gridColumnFactory";

export const GetDistributionsByAgeColumns = (
  _reportType: FrozenReportsByAgeRequestType
): (ColDef | ColGroupDef)[] => {
  // Flattened column set (no header group) to avoid group header render issues
  const columns: ColDef[] = [
    {
      headerName: "Age",
      field: "age",
      colId: "age",
      minWidth: 80,
      type: "rightAligned",
      resizable: true,
      sort: "asc",
      cellDataType: "text"
    },
    createCountColumn({
      headerName: "EMPS",
      field: "regularEmployeeCount",
      minWidth: 100
    }),
    createCurrencyColumn({
      headerName: "Amount",
      field: "regularAmount",
      minWidth: 150
    })
  ];
  return columns;
};
