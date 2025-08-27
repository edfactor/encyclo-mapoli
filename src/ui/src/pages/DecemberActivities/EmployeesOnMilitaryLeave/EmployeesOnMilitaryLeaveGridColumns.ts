import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { createBadgeColumn, createSSNColumn, createNameColumn } from "../../../utils/gridColumnFactory";

export const GetRehireColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "center"
    }),
    createNameColumn({
      field: "fullName",
      minWidth: 150
    }),
    createSSNColumn({ alignment: "left" })
  ];
};
