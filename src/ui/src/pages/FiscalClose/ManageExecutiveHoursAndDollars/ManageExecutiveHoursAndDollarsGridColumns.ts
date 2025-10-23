import { ColDef } from "ag-grid-community";
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
    createBadgeColumn({}),
    createNameColumn({
      field: "fullName"
    }),
    createStoreColumn({}),

    createSSNColumn({
      sortable: true // PS-1829: Enabled - backend now sorts before masking
    }),

    createHoursColumn({
      headerName: "Executive Hours",
      field: "hoursExecutive",

      editable: !mini
    }),
    createCurrencyColumn({
      headerName: "Executive Dollars",
      field: "incomeExecutive"
    }),
    createHoursColumn({
      headerName: "Oracle Hours",
      field: "currentHoursYear"
    }),
    createCurrencyColumn({
      headerName: "Oracle Dollars",
      field: "currentIncomeYear"
    }),

    createStatusColumn({
      headerName: "Pay Frequency",
      field: "payFrequencyId",

      valueFormatter: (params) => {
        return params.data?.payFrequencyName; // assuming 'statusName' is in the row data
      }
    }),
    createStatusColumn({
      headerName: "Employment Status",
      field: "employmentStatusId",

      valueFormatter: (params) => {
        return params.data?.employmentStatusName; // assuming 'statusName' is in the row data
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
