import { ColDef, ICellRendererParams } from "ag-grid-community";
import { numberToCurrency } from "smart-ui-library";
import { createBadgeColumn } from "../../utils/gridColumnFactory";

export const GetQPAY066AdHocGridColumns = (): ColDef[] => [
  createBadgeColumn({ minWidth: 90 }),
  {
    headerName: "Name",
    field: "name",
    minWidth: 120
  },
  {
    headerName: "Beginning Balance",
    field: "beginningBalance",
    minWidth: 140,
    cellRenderer: (params: any) => numberToCurrency(params.value)
  },
  {
    headerName: "Beneficiary Allocation",
    field: "beneficiaryAllocation",
    minWidth: 150,
    cellRenderer: (params: any) => numberToCurrency(params.value)
  },
  {
    headerName: "Distribution Amount",
    field: "distributionAmount",
    minWidth: 140,
    cellRenderer: (params: any) => numberToCurrency(params.value)
  },
  {
    headerName: "Forfeit",
    field: "forfeit",
    minWidth: 90,
    cellRenderer: (params: any) => numberToCurrency(params.value)
  },
  {
    headerName: "Ending Balance",
    field: "endingBalance",
    minWidth: 120,
    cellRenderer: (params: any) => numberToCurrency(params.value)
  },
  {
    headerName: "Vesting Balance",
    field: "vestingBalance",
    minWidth: 120,
    cellRenderer: (params: any) => numberToCurrency(params.value)
  },
  {
    headerName: "Date Term",
    field: "dateTerm",
    minWidth: 100
  },
  {
    headerName: "YTD Hours",
    field: "ytdHours",
    minWidth: 90
  },
  {
    headerName: "Years",
    field: "years",
    minWidth: 70
  },
  {
    headerName: "Vested",
    field: "vested",
    minWidth: 80
  },
  {
    headerName: "Age",
    field: "age",
    minWidth: 70
  }
];
