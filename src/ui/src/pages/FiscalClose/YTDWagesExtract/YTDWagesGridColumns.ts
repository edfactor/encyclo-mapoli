import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createHoursColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

export const GetYTDWagesColumns = (): ColDef[] => {
  const columns: ColDef[] = [
    createBadgeColumn({}),
    createHoursColumn({
      headerName: "Hours Current Year",
      field: "hoursCurrentYear"
    }),
    createCurrencyColumn({
      headerName: "Dollars Current Year",
      field: "incomeCurrentYear"
    }),
    createStoreColumn({})
  ];
  return columns;
};
