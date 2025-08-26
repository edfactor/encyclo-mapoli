import { ColDef } from "ag-grid-community";
import { formatNumberWithComma } from "smart-ui-library";
import {
  createAgeColumn,
  createBadgeColumn,
  createCountColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
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
    createNameColumn({
      field: "employeeName",
      minWidth: 180,
      sortable: true
    }),
    createStoreColumn({
      minWidth: 80
    }),
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
    createHoursColumn({
      minWidth: 100
    }),
    {
      headerName: "Points",
      field: "points",
      colId: "points",
      minWidth: 100,
      type: "rightAligned",
      resizable: true,
      valueFormatter: (params) => formatNumberWithComma(params.value)
    },
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
      field: "balance",
      minWidth: 140
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
