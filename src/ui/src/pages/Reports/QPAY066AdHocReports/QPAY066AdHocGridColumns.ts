import { ColDef } from "ag-grid-community";
import {
  createAgeColumn,
  createBadgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn
} from "../../../utils/gridColumnFactory";

export const GetQPAY066AdHocGridColumns = (): ColDef[] => [
  createBadgeColumn({}),
  createNameColumn({}),
  createCurrencyColumn({
    headerName: "Beginning Balance",
    field: "beginningBalance"
  }),
  createCurrencyColumn({
    headerName: "Beneficiary Allocation",
    field: "beneficiaryAllocation"
  }),
  createCurrencyColumn({
    headerName: "Distribution Amount",
    field: "distributionAmount"
  }),
  createCurrencyColumn({
    headerName: "Forfeit",
    field: "forfeit"
  }),
  createCurrencyColumn({
    headerName: "Ending Balance",
    field: "endingBalance"
  }),
  createCurrencyColumn({
    headerName: "Vesting Balance",
    field: "vestingBalance"
  }),
  createDateColumn({
    headerName: "Date Term",
    field: "dateTerm",
    minWidth: 100
  }),
  createHoursColumn({
    headerName: "YTD Hours",
    field: "ytdHours",
    minWidth: 90
  }),
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
  createAgeColumn({})
];
