import { ColDef } from "ag-grid-community";
import {
  createAgeColumn,
  createBadgeColumn,
  createCountColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
  createPointsColumn,
  createSSNColumn,
  createStatusColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

export const GetProfitSharingReportGridColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  // Import the date formatter from dateutils

  return [
    createBadgeColumn({
      headerName: "Badge",
      navigateFunction: navFunction
    }),
    createNameColumn({}),
    createStoreColumn({}),
    {
      headerName: "Type",
      field: "employeeTypeCode",
      colId: "employeeTypeCode",
      minWidth: 80,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    createDateColumn({
      headerName: "Date of Birth",
      field: "dateOfBirth"
    }),
    createAgeColumn({}),
    createSSNColumn(),
    createCurrencyColumn({
      headerName: "Wages",
      field: "wages"
    }),
    createHoursColumn({}),
    createPointsColumn({}),
    {
      headerName: "New",
      field: "isNew",
      colId: "isNew",
      minWidth: 100,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    createStatusColumn({
      field: "employeeStatus",
      minWidth: 120,
      alignment: "center"
    }),
    createCurrencyColumn({
      headerName: "Balance",
      field: "balance"
    }),
    createCountColumn({
      headerName: "Years in Plan",
      field: "yearsInPlan"
    }),
    createDateColumn({
      headerName: "Inactive date",
      field: "terminationDate"
    })
  ];
};
