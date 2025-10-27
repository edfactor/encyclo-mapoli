import { ColDef } from "ag-grid-community";
import {
  createCurrencyColumn,
  createNameColumn,
  createSSNColumn,
  createStatusColumn
} from "../../../utils/gridColumnFactory";

export const GetDisbursementGridColumns = (): ColDef[] => {
  const columns: ColDef[] = [
    createSSNColumn({
      maxWidth: 130
    }),
    createNameColumn({
      field: "fullName",
      headerName: "Name",
      maxWidth: 250
    }),
    {
      headerName: "Sequence",
      field: "paymentSequence",
      sortable: true,
      resizable: true,
      headerClass: "center-align",
      cellClass: "center-align",
      maxWidth: 100
    },
    createStatusColumn({
      headerName: "Pay Flag",
      field: "statusName",
      maxWidth: 180
    }),
    createStatusColumn({
      headerName: "Tax Flag",
      field: "taxCodeName",
      maxWidth: 200
    }),
    {
      headerName: "Reason Code",
      field: "frequencyName",
      sortable: true,
      resizable: true,
      headerClass: "left-align",
      cellClass: "left-align",
      maxWidth: 180
    },
    createCurrencyColumn({
      headerName: "Amount Requested",
      field: "grossAmount",
      maxWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Federal Tax",
      field: "federalTax",
      maxWidth: 120
    }),
    createCurrencyColumn({
      headerName: "State Tax",
      field: "stateTax",
      maxWidth: 120
    }),
    createCurrencyColumn({
      headerName: "Check Amount",
      field: "checkAmount",
      maxWidth: 130
    })
  ];

  return columns;
};
