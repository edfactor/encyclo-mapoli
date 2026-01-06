import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createHoursColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

export const GetYTDWagesColumns = (): ColDef[] => {
  const columns: ColDef[] = [
    createBadgeColumn({ flex: 1 }),
    createHoursColumn({
      headerName: "Hours Current Year",
      field: "hoursCurrentYear",
      flex: 1
    }),
    createCurrencyColumn({
      headerName: "Dollars Current Year",
      field: "incomeCurrentYear",
      flex: 1
    }),
    createStoreColumn({ flex: 1 })
  ];
  return columns;
};
