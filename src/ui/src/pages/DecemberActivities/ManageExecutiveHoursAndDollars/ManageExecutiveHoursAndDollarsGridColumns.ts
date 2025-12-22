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

interface GetColumnsOptions {
  mini?: boolean;
  canEdit?: boolean;
}

// The default is to show all columns, but if the mini flag is set to true, only show the
// badge, name, and ssn columns.
// canEdit controls whether the Executive Hours and Executive Dollars columns are editable.
// Editing requires: page status "In Progress" AND user has ExecutiveAdministrator role.
export const GetManageExecutiveHoursAndDollarsColumns = (options?: GetColumnsOptions | boolean): ColDef[] => {
  // Support legacy boolean argument for backwards compatibility
  const mini = typeof options === "boolean" ? options : (options?.mini ?? false);
  const canEdit = typeof options === "boolean" ? !options : (options?.canEdit ?? false);

  // Columns are editable only when NOT in mini mode AND canEdit is true
  const isEditable = !mini && canEdit;

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

      editable: isEditable
    }),
    {
      ...createCurrencyColumn({
        headerName: "Executive Dollars",
        field: "incomeExecutive"
      }),
      editable: isEditable
    },
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
