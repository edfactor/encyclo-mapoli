import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createNameColumn,
  createSSNColumn,
  createStatusColumn
} from "../../../utils/gridColumnFactory";
import { ActionsCellRenderer } from "./DistributionActions";

export const GetDistributionInquiryColumns = (): ColDef[] => {
  const columns: ColDef[] = [
    createSSNColumn({
      maxWidth: 130
    }),
    createNameColumn({
      field: "fullName",
      maxWidth: 250
    }),
    createBadgeColumn({}),
    {
      headerName: "Frequency",
      field: "frequencyName",
      sortable: true,
      resizable: true,
      headerClass: "left-align",
      cellClass: "left-align",
      maxWidth: 130
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
    createCurrencyColumn({
      headerName: "Gross Amount",
      field: "grossAmount",
      maxWidth: 130
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
    }),
    {
      headerName: "Action",
      field: "actions",
      sortable: false,
      resizable: false,
      headerClass: "center-align",
      cellClass: "center-align",
      cellRenderer: ActionsCellRenderer,
      minWidth: 200,
      maxWidth: 250
    }
  ];

  return columns;
};
