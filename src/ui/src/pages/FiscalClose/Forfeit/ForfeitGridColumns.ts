import { ColDef } from "ag-grid-community";
import {
  createBadgeOrPSNColumn,
  createColumnFrom,
  createCountColumn,
  createCurrencyColumn,
  createNameColumn,
  createSSNColumn
} from "../../../utils/gridColumnFactory";
import { ForfeituresCellRenderer } from "./ForfeituresCellRenderer";

export interface ForfeitColumnsOptions {
  navFunction: (badgeNumber: string) => void;
  onValidationClick?: (fieldName: string) => void;
}

export const GetProfitShareForfeitColumns = (options: ForfeitColumnsOptions): ColDef[] => {
  const { navFunction, onValidationClick } = options;
  return [
    createBadgeOrPSNColumn({
      headerName: "Badge/PSN",
      field: "badgeOrPsn",
      badgeField: "badgeNumber",
      psnField: "beneficiaryPsn",
      navigateFunction: navFunction,
      enableLinking: true
    }),
    createNameColumn({
      field: "employeeName"
    }),
    createSSNColumn({}),
    createColumnFrom(
      createCurrencyColumn({
        headerName: "Forfeitures",
        field: "forfeitures"
      }),
      {
        cellRenderer: ForfeituresCellRenderer,
        cellRendererParams: {
          onValidationClick
        }
      }
    ),

    createCountColumn({
      headerName: "Cont/Forfeit Points",
      field: "contForfeitPoints"
    }),
    createCountColumn({
      headerName: "Earnings Points",
      field: "earningPoints"
    })
  ];
};
