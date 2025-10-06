import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createNameColumn,
  createSSNColumn
} from "../../utils/gridColumnFactory";
import { ActionsCellRenderer } from "./DistributionActions";

export const GetDistributionInquiryColumns = (): ColDef[] => {
  const columns: ColDef[] = [
    createSSNColumn({}),
    createNameColumn({
      field: "fullName"
    }),
    createBadgeColumn({}),
    {
      headerName: "Frequency",
      field: "frequencyName",
      sortable: true,
      resizable: true,
      headerClass: "left-align",
      cellClass: "left-align"
    },
    {
      headerName: "Pay Flag",
      field: "statusName",
      sortable: true,
      resizable: true,
      headerClass: "center-align",
      cellClass: "center-align"
    },
    {
      headerName: "Tax Flag",
      field: "taxCodeName",
      sortable: true,
      resizable: true,
      headerClass: "center-align",
      cellClass: "center-align"
    },
    createCurrencyColumn({
      headerName: "Gross Amount",
      field: "grossAmount"
    }),
    createCurrencyColumn({
      headerName: "Federal Tax",
      field: "federalTax"
    }),
    createCurrencyColumn({
      headerName: "State Tax",
      field: "stateTax"
    }),
    createCurrencyColumn({
      headerName: "Check Amount",
      field: "checkAmount"
    }),
    {
      headerName: "Action",
      field: "actions",
      sortable: false,
      resizable: false,
      headerClass: "center-align",
      cellClass: "center-align",
      cellRenderer: ActionsCellRenderer,
      minWidth: 200
    }
  ];

  return columns;
};
