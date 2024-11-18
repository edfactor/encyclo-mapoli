import { formattingUtils } from "smart-ui-library";

const baseColumnDefs = [
  {
    headerName: "Age",
    field: "age",
    colId: "age",
    minWidth: 10,
    headerClass: "left-align",
    cellClass: "left-align",
    resizable: true,
    sort: "asc",
  },
  {
    headerName: "EMPS",
    field: "employeeCount",
    colId: "employeeCount",
    minWidth: 10,
    headerClass: "left-align",
    cellClass: "left-align",
    resizable: true,
  },
  {
    headerName: "Amount",
    field: "amount",
    colId: "amount",
    minWidth: 50,
    headerClass: "left-align",
    cellClass: "left-align",
    resizable: true,
    valueFormatter: formattingUtils.agGridNumberToCurrency,
  },
];

const groupHeaders = ["Total", "Full Time", "Part Time"];

export const GetDistributionsByAgeColumns = () =>
  groupHeaders.map((groupName) => ({
    headerName: groupName,
    children: baseColumnDefs,
  }));
