import {
  createCurrencyColumn,
  createDateColumn,
  createNameColumn,
  createPSNColumn,
  createSSNColumn,
  createStatusColumn,
  createYearColumn
} from "@/utils/gridColumnFactory";
import { ColDef } from "ag-grid-community";

export const PayBeNextGridColumns = (): ColDef[] => {
  const relationshipColumn: ColDef = {
    headerName: "Relationship",
    field: "relationship",
    colId: "relationship",
    minWidth: 120,
    headerClass: "center-align",
    cellClass: "center-align",
    resizable: true
  };

  return [
    createPSNColumn({
      headerName: "PSN",
      field: "badgeNumber",
      enableLinking: true,
      linkingStyle: "badge-psn"
    }),
    createNameColumn({
      headerName: "Name",
      field: "fullName"
    }),
    createSSNColumn({}),
    { ...relationshipColumn, flex: 1 }
  ];
};

export const ProfitDetailGridColumns = () =>
  //addRowToSelectedRows: (id: number) => void,
  //removeRowFromSelectedRows: (id: number) => void
  {
    return [
      createYearColumn({
        headerName: "Year",
        field: "year"
      }),
      createStatusColumn({
        headerName: "Code",
        field: "code"
      }),
      createCurrencyColumn({
        headerName: "Contributions",
        field: "contributions"
      }),
      createCurrencyColumn({
        headerName: "Earnings",
        field: "earnings"
      }),
      createCurrencyColumn({
        headerName: "Forfeitures",
        field: "forfeitures"
      }),
      createDateColumn({
        field: "date"
      }),
      {
        headerName: "Comments",
        field: "comments",
        colId: "comments",
        width: 100,
        headerClass: "right-align",
        cellClass: "right-align",
        resizable: true,
        sortable: false
      }
    ];
  };
