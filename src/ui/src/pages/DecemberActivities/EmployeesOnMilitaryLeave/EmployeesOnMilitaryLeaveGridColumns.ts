import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { createBadgeColumn, createSSNColumn } from "../../../utils/gridColumnFactory";

export const GetMilitaryAndRehireColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "center"
    }),
    {
      headerName: "Name",
      field: "fullName",
      colId: "fullName",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    createSSNColumn({ alignment: "left" })
  ];
};
