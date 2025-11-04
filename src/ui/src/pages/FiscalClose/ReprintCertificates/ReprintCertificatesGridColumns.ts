import { ColDef } from "ag-grid-community";
import { createBadgeColumn, createCurrencyColumn, createNameColumn } from "../../../utils/gridColumnFactory";

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

export const GetReprintCertificatesGridColumns = () //selectedRowIds: number[],
//addRowToSelectedRows: (id: number) => void,
//removeRowFromSelectedRows: (id: number) => void
: ColDef[] => {
  return [
    {
      headerName: "",
      field: "selection",
      colId: "selection",
      width: 50,
      pinned: "right",
      sortable: false,
      resizable: false
    },
    createBadgeColumn({
      field: "badge"
    }),
    createNameColumn({
      field: "name"
    }),

    createCurrencyColumn({
      headerName: "EOY Balance",
      field: "eoyBalance"
    }),
    createCurrencyColumn({
      headerName: "2024 Forfeitures",
      field: "forfeitures2024"
    }),
    createCurrencyColumn({
      headerName: "2024 Withdrawals",
      field: "withdrawals2024"
    }),
    createCurrencyColumn({
      headerName: "Balance",
      field: "balance"
    }),
    createCurrencyColumn({
      headerName: "Vested Portion",
      field: "vestedPortion"
    }),
    createCurrencyColumn({
      headerName: "Single Life Annuity",
      field: "singleLifeAnnuity"
    }),
    createCurrencyColumn({
      headerName: "Qualified Join and 100% Survivor",
      field: "qualifiedJoinAndSurvivor"
    })
  ];
};
