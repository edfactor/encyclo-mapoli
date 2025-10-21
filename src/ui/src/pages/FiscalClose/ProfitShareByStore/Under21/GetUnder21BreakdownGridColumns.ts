import {
  createAgeColumn,
  createBadgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createNameColumn,
  createPercentageColumn
} from "utils/gridColumnFactory";

export const GetUnder21BreakdownColumnDefs = () => [
  createBadgeColumn({
    renderAsLink: true
  }),
  createNameColumn({
    field: "fullName"
  }),
  createAgeColumn({}),
  createDateColumn({
    headerName: "Date of Birth",
    field: "dateOfBirth"
  }),
  createCurrencyColumn({
    headerName: "Beginning Balance",
    field: "beginningBalance"
  }),
  createCurrencyColumn({
    headerName: "Earnings",
    field: "earnings"
  }),
  createCurrencyColumn({
    headerName: "Contributions",
    field: "contributions"
  }),
  createCurrencyColumn({
    headerName: "Forfeitures",
    field: "forfeitures"
  }),
  createCurrencyColumn({
    headerName: "Distributions",
    field: "distributions"
  }),
  createCurrencyColumn({
    headerName: "Ending Balance",
    field: "endingBalance"
  }),
  createCurrencyColumn({
    headerName: "Vested Amount",
    field: "vestedAmount"
  }),
  createPercentageColumn({
    headerName: "Vesting %",
    field: "vestingPercentage"
  })
];

/**
 * Column definitions for the Under 21 Inactive grid
 * @param navFunction Function to handle navigation when clicking on badge numbers
 * @returns Column definitions for the Under 21 Inactive grid
 */
export const under21InactiveColumnDefs = () => [
  createBadgeColumn({
    field: "badgeNumber",

    renderAsLink: true
  }),
  createNameColumn({
    field: "fullName"
  }),
  createDateColumn({
    headerName: "Date of Birth",
    field: "birthDate"
  }),
  createDateColumn({
    headerName: "Hire Date",
    field: "hireDate"
  }),
  createDateColumn({
    headerName: "Termination Date",
    field: "terminationDate"
  }),
  createAgeColumn({}),
  createCurrencyColumn({
    headerName: "Beginning Balance",
    field: "beginningBalance"
  }),
  createCurrencyColumn({
    headerName: "Earnings",
    field: "earnings"
  }),
  createCurrencyColumn({
    headerName: "Contributions",
    field: "contributions"
  }),
  createCurrencyColumn({
    headerName: "Forfeitures",
    field: "forfeitures"
  }),
  createCurrencyColumn({
    headerName: "Distributions",
    field: "distributions"
  }),
  createCurrencyColumn({
    headerName: "Ending Balance",
    field: "endingBalance"
  }),
  createCurrencyColumn({
    headerName: "Vested Amount",
    field: "vestedAmount"
  })
];
