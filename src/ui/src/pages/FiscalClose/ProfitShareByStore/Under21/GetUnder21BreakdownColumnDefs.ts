import { ICellRendererParams, ValueFormatterParams } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";


export const GetUnder21BreakdownColumnDefs = (navFunction?: (path: string) => void) => [
  {
    headerName: "Badge",
    field: "badgeNumber",
    width: 100,
    cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber, navFunction)
  },
  {
    headerName: "Name",
    field: "fullName",
    width: 200
  },
  {
    headerName: "Age",
    field: "age",
    width: 70
  },
  {
    headerName: "Date of Birth",
    field: "dateOfBirth",
    width: 120
  },
  {
    headerName: "Beginning Balance",
    field: "beginningBalance",
    width: 150,
    valueFormatter: agGridNumberToCurrency
  },
  {
    headerName: "Earnings",
    field: "earnings",
    width: 120,
    valueFormatter: agGridNumberToCurrency
  },
  {
    headerName: "Contributions",
    field: "contributions",
    width: 150,
    valueFormatter: agGridNumberToCurrency
  },
  {
    headerName: "Forfeitures",
    field: "forfeitures",
    width: 120,
    valueFormatter: agGridNumberToCurrency
  },
  {
    headerName: "Distributions",
    field: "distributions",
    width: 120,
    valueFormatter: agGridNumberToCurrency
  },
  {
    headerName: "Ending Balance",
    field: "endingBalance",
    width: 150,
    valueFormatter: agGridNumberToCurrency
  },
  {
    headerName: "Vested Amount",
    field: "vestedAmount",
    width: 150,
    valueFormatter: agGridNumberToCurrency
  },
  {
    headerName: "Vesting %",
    field: "vestingPercentage",
    width: 100,
    valueFormatter: (params: ValueFormatterParams) => `${params.value}%`
  }
];

/**
 * Column definitions for the Under 21 Inactive grid
 * @param navFunction Function to handle navigation when clicking on badge numbers
 * @returns Column definitions for the Under 21 Inactive grid
 */
export const under21InactiveColumnDefs = (navFunction?: (path: string) => void) => [
  {
    headerName: "Badge",
    field: "badgeNumber",
    width: 100,
    cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber, navFunction)
  },
  {
    headerName: "Name",
    field: "fullName",
    width: 200
  },
  {
    headerName: "Date of Birth",
    field: "birthDate",
    width: 120
  },
  {
    headerName: "Hire Date",
    field: "hireDate",
    width: 120
  },
  {
    headerName: "Termination Date",
    field: "terminationDate",
    width: 150
  },
  {
    headerName: "Age",
    field: "age",
    width: 70
  },
  {
    headerName: "Beginning Balance",
    field: "beginningBalance",
    width: 150,
    valueFormatter: agGridNumberToCurrency
  },
  {
    headerName: "Earnings",
    field: "earnings",
    width: 120,
    valueFormatter: agGridNumberToCurrency
  },
  {
    headerName: "Contributions",
    field: "contributions",
    width: 120,
    valueFormatter: agGridNumberToCurrency
  },
  {
    headerName: "Forfeitures",
    field: "forfeitures",
    width: 120,
    valueFormatter: agGridNumberToCurrency
  },
  {
    headerName: "Distributions",
    field: "distributions",
    width: 120,
    valueFormatter: agGridNumberToCurrency
  },
  {
    headerName: "Ending Balance",
    field: "endingBalance",
    width: 150,
    valueFormatter: agGridNumberToCurrency
  },
  {
    headerName: "Vested Amount",
    field: "vestedAmount",
    width: 150,
    valueFormatter: agGridNumberToCurrency
  }
]; 