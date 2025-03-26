import { ColDef } from "ag-grid-community";

export const GetUnder21BreakdownColumnDefs = (): ColDef[] => [
  {
    headerName: "Store",
    field: "storeNumber",
    width: 80
  },
  {
    headerName: "Badge",
    field: "badgeNumber",
    width: 100
  },
  {
    headerName: "Employee Name",
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
    width: 150
  },
  {
    headerName: "Earnings",
    field: "earnings",
    width: 120
  },
  {
    headerName: "Contributions",
    field: "contributions",
    width: 150
  },
  {
    headerName: "Forfeitures",
    field: "forfeitures",
    width: 120
  },
  {
    headerName: "Distributions",
    field: "distributions",
    width: 120
  },
  {
    headerName: "Ending Balance",
    field: "endingBalance",
    width: 150
  },
  {
    headerName: "Vested Amount",
    field: "vestedAmount",
    width: 150
  },
  {
    headerName: "Vesting %",
    field: "vestingPercentage",
    width: 100
  }
];

export const under21InactiveColumnDefs = [
  {
    headerName: "Badge",
    field: "badgeNumber",
    width: 100
  },
  {
    headerName: "Employee Name", 
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
  }
]; 