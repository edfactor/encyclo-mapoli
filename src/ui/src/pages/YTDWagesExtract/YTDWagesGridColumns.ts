import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../constants";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createHoursColumn,
  createStoreColumn
} from "../../utils/gridColumnFactory";

export const GetYTDWagesColumns = (): ColDef[] => {
  const columns: ColDef[] = [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left"
    }),
    createHoursColumn({
      headerName: "Hours Current Year",
      field: "hoursCurrentYear",
      minWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Dollars Current Year",
      field: "incomeCurrentYear",
      minWidth: 150
    }),
    createStoreColumn({})
  ];
  return columns;
};
