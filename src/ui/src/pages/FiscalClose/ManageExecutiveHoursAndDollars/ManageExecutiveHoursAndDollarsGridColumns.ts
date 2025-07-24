import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";
import { agGridNumberToCurrency } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../../../constants";

// The default is to show all columns, but if the mini flag is set to true, only show the
// badge, name, and ssn columns
export const GetManageExecutiveHoursAndDollarsColumns = (mini?: boolean): ColDef[] => {
  const columns: ColDef[] = [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      headerClass: mini ? "left-align" : "right-align",
      cellClass: mini ? "left-align" : "right-align",
      resizable: true,
      sortable: true,
      checkboxSelection: mini,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber)
    },
    {
      headerName: "Name",
      field: "fullName",
      colId: "fullName",
      minWidth: GRID_COLUMN_WIDTHS.FULL_NAME,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Store",
      field: "storeNumber",
      colId: "storeNumber",
      minWidth: 80,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: GRID_COLUMN_WIDTHS.SSN,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Executive Hours",
      field: "hoursExecutive",
      colId: "hoursExecutive",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      editable: !mini
    },
    {
      headerName: "Executive Dollars",
      field: "incomeExecutive",
      colId: "incomeExecutive",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      editable: !mini,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Oracle Hours",
      field: "currentHoursYear",
      colId: "currentHoursYear",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Oracle Dollars",
      field: "currentIncomeYear",
      colId: "currentIncomeYear",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Pay Frequency",
      field: "payFrequencyId",
      colId: "payFrequencyId",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => {
        const id = params.data?.payFrequencyId; // assuming 'status' is in the row data
        const name = params.data?.payFrequencyName; // assuming 'statusName' is in the row data
        return `[${id}] ${name}`;
      }
    },
    {
      headerName: "Employment Status",
      field: "employmentStatusId",
      colId: "employmentStatusId",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => {
        const id = params.data?.employmentStatusId; // assuming 'status' is in the row data
        const name = params.data?.employmentStatusName; // assuming 'statusName' is in the row data
        return `[${id}] ${name}`;
      }
    }
  ];

  // We could have a hide property in elements to be hidden and not filter this way,
  // but in the modal, the column selection panel would show them
  if (mini) {
    return columns.filter(
      (column) =>
        column.colId === "badgeNumber" ||
        column.colId === "fullName" ||
        column.colId === "ssn" ||
        column.colId === "hoursExecutive" ||
        column.colId === "incomeExecutive"
    );
  }
  return columns;
};
