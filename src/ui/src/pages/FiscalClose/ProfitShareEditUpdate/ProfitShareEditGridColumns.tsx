import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCountColumn,
  createCurrencyColumn,
  createNameColumn,
  createPSNColumn,
  createStatusColumn
} from "../../../utils/gridColumnFactory";

export const ProfitShareEditUpdateGridColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createNameColumn({ field: "name" }),
    createCountColumn({
      headerName: "Code",
      field: "code"
    }),
    createPSNColumn({}),
    createCurrencyColumn({
      headerName: "Contributions",
      field: "contributionAmount"
    }),
    createCurrencyColumn({
      headerName: "Earnings",
      field: "earningsAmount"
    }),
    createCurrencyColumn({
      headerName: "Forfeiture",
      field: "forfeitureAmount"
    }),

    {
      headerName: "Remark",
      field: "remark",
      colId: "remark",
      minWidth: 100,
      type: "rightAligned",
      resizable: true
    },
    createCountColumn({
      headerName: "Comment Code",
      field: "commentTypeId"
    }),

    {
      headerName: "Record Change Summary",
      field: "recordChangeSummary",
      colId: "recordChangeSummary",
      minWidth: 80,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    createStatusColumn({
      headerName: "Zero Contribution Status",
      field: "zeroContStatus"
    }),
    createCountColumn({
      headerName: "Year Extension",
      field: "yearExtension"
    })
  ];
};
