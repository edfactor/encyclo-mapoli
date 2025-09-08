import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createHoursColumn,
  createNameColumn,
  createSSNColumn,
  createStatusColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

// The default is to show all columns, but if the mini flag is set to true, only show the
// badge, name, and ssn columns
export const GetManageExecutiveHoursAndDollarsColumns = (mini?: boolean): ColDef[] => {
  const columns: ColDef[] = [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: mini ? "left" : "center"
    }),
    createNameColumn({
      field: "fullName",
      minWidth: GRID_COLUMN_WIDTHS.FULL_NAME
    }),
    createStoreColumn({
      headerName: "Store",
      minWidth: 60
    }),

    createSSNColumn({ alignment: "left" }),

    createHoursColumn({
      headerName: "Executive Hours",
      field: "hoursExecutive",
      minWidth: 150,
      alignment: "left",
      editable: !mini
    }),
    createCurrencyColumn({
      headerName: "Executive Dollars",
      field: "incomeExecutive"
    }),
    createHoursColumn({
      headerName: "Oracle Hours",
      field: "currentHoursYear",
      minWidth: 150,
      alignment: "left"
    }),
    createCurrencyColumn({
      headerName: "Oracle Dollars",
      field: "currentIncomeYear"
    }),

    createStatusColumn({
      headerName: "Pay Frequency",
      field: "payFrequencyId",
      minWidth: 150,
      valueFormatter: (params) => {
        const id = params.data?.payFrequencyId; // assuming 'status' is in the row data
        const name = params.data?.payFrequencyName; // assuming 'statusName' is in the row data
        return `[${id}] ${name}`;
      }
    }),
    createStatusColumn({
      headerName: "Employment Status",
      field: "employmentStatusId",
      minWidth: 150,
      valueFormatter: (params) => {
        const id = params.data?.employmentStatusId; // assuming 'status' is in the row data
        const name = params.data?.employmentStatusName; // assuming 'statusName' is in the row data
        return `[${id}] ${name}`;
      }
    })
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
