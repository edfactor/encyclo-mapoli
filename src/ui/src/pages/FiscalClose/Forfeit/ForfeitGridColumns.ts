import { ColDef } from "ag-grid-community";
import {
  createBadgeOrPSNColumn,
  createCountColumn,
  createCurrencyColumn,
  createNameColumn,
  createSSNColumn
} from "../../../utils/gridColumnFactory";

export const GetProfitShareForfeitColumns = (): ColDef[] => {
  return [
    createBadgeOrPSNColumn({
      headerName: "Badge/PSN",
      field: "badgeOrPsn",
      badgeField: "badgeNumber",
      psnField: "beneficiaryPsn",
      enableLinking: true
    }),
    createNameColumn({
      field: "employeeName"
    }),
    createSSNColumn({}),
    createCurrencyColumn({
      headerName: "Forfeitures",
      field: "forfeitures"
    }),
    createCountColumn({
      headerName: "Cont/Forfeit Points",
      field: "contForfeitPoints"
    }),
    createCountColumn({
      headerName: "Earnings Points",
      field: "earningPoints"
    }),
    {
      headerName: "",
      colId: "spacer",
      field: "spacer",
      sortable: false,
      resizable: false,
      suppressHeaderMenuButton: true,
      filter: false,
      editable: false,
      flex: 1,
      valueGetter: () => ""
    }
  ];
};
