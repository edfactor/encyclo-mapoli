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

const groupHeaders = [
  { name: "Total", className: "group-total" },
  { name: "Full Time", className: "group-full-time" },
  { name: "Part Time", className: "group-part-time" },
];

export const GetDistributionsByAgeColumns = () =>
  groupHeaders.map(({ name, className }) => ({
    headerName: name, // Use the 'name' property for the header name
    headerClass: className, // Apply the class for styling
    children: baseColumnDefs,
  }));

