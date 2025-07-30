import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { createBadgeColumn, createCurrencyColumn, createSSNColumn } from "../../../utils/gridColumnFactory";

// The default is to show all columns, but if the mini flag is set to true, only show the
// badge, name, and ssn columns
export const GetManageExecutiveHoursAndDollarsColumns = (mini?: boolean): ColDef[] => {
  const columns: ColDef[] = [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: mini ? "left" : "center"
    }),
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
    createSSNColumn({ alignment: "left" }),
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
      ...createCurrencyColumn({
        headerName: "Executive Dollars",
        field: "incomeExecutive"
      }),
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      editable: !mini
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
      ...createCurrencyColumn({
        headerName: "Oracle Dollars",
        field: "currentIncomeYear"
      }),
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align"
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
