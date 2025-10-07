import { ColDef } from "ag-grid-community";
import {
  createAgeColumn,
  createBadgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createNameColumn,
  createSSNColumn,
  createStateColumn,
  createTaxCodeColumn
} from "../../../utils/gridColumnFactory";

export const GetDistributionsAndForfeituresColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createNameColumn({
      field: "employeeName",
      minWidth: 150
    }),
    createSSNColumn({}),
    createDateColumn({
      headerName: "Date",
      field: "date"
    }),
    createCurrencyColumn({
      headerName: "Distribution",
      field: "distributionAmount"
    }),
    createCurrencyColumn({
      headerName: "State Tax",
      field: "stateTax"
    }),
    createStateColumn({}),
    createCurrencyColumn({
      headerName: "Federal Tax",
      field: "federalTax"
    }),
    createCurrencyColumn({
      headerName: "Forfeit Amount",
      field: "forfeitAmount"
    }),
    createAgeColumn({
      headerName: "Age @ Txn",
      headerTooltip: "The age is for the member at the time of the transaction, rather than the current age.",
      tooltip: "The age is for the member at the time of the transaction, rather than the current age."
    }),
    createTaxCodeColumn({
      field: "taxCode",

      alignment: "right",
      showBrackets: false,
      hideZeroValues: false
    }),
    {
      headerName: "Forfeited",
      field: "hasForfeited",
      colId: "hasForfeited",

      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    }
  ];
};
