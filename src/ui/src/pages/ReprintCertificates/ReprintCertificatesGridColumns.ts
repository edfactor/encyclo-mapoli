import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../constants";
import { createBadgeColumn, createCurrencyColumn } from "../../utils/gridColumnFactory";

export interface ReprintCertificateEmployee {
  badge: number;
  name: string;
  eoyBalance: number;
  forfeitures2024: number;
  withdrawals2024: number;
  balance: number;
  vestedPortion: number;
  singleLifeAnnuity: number;
  qualifiedJoinAndSurvivor: number;
  isSelected?: boolean;
}

export const GetReprintCertificatesGridColumns = (
  selectedRowIds: number[],
  addRowToSelectedRows: (id: number) => void,
  removeRowFromSelectedRows: (id: number) => void
): ColDef[] => {
  return [
    {
      headerName: "",
      field: "selection",
      colId: "selection",
      width: 50,
      checkboxSelection: true,
      headerCheckboxSelection: true,
      pinned: "right",
      sortable: false,
      resizable: false
    },
    createBadgeColumn({
      headerName: "Badge",
      field: "badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "center"
    }),
    {
      headerName: "Name",
      field: "name",
      colId: "name",
      minWidth: 200,
      resizable: true,
      sortable: true
    },

    createCurrencyColumn({
      headerName: "EOY Balance",
      field: "eoyBalance",
      minWidth: 130
    }),
    createCurrencyColumn({
      headerName: "2024 Forfeitures",
      field: "forfeitures2024",
      minWidth: 130
    }),
    createCurrencyColumn({
      headerName: "2024 Withdrawals",
      field: "withdrawals2024",
      minWidth: 130
    }),
    createCurrencyColumn({
      headerName: "Balance",
      field: "balance",
      minWidth: 130
    }),
    createCurrencyColumn({
      headerName: "Vested Portion",
      field: "vestedPortion",
      minWidth: 130
    }),
    createCurrencyColumn({
      headerName: "Single Life Annuity",
      field: "singleLifeAnnuity",
      minWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Qualified Join and 100% Survivor",
      field: "qualifiedJoinAndSurvivor",
      minWidth: 220
    })
  ];
};
