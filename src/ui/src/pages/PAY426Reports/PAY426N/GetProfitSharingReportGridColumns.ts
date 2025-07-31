import { ColDef } from "ag-grid-community";
import { formatNumberWithComma } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { mmDDYYFormat } from "../../../utils/dateUtils";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createSSNColumn,
  createAgeColumn,
  createStoreColumn,
  createNameColumn,
  createHoursColumn,
  createStatusColumn
} from "../../../utils/gridColumnFactory";

export const GetProfitSharingReportGridColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  // Import the date formatter from dateutils

  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "center",
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
    {
      headerName: "Date of Birth",
      field: "dateOfBirth",
      colId: "dateOfBirth",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => mmDDYYFormat(params.value)
    },
    createAgeColumn({}),
    createSSNColumn(),
    createCurrencyColumn({
      headerName: "Wages",
      field: "wages",
      minWidth: 120
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
    {
      headerName: "Years in Plan",
      field: "yearsInPlan",
      colId: "yearsInPlan",
      minWidth: 80,
      type: "rightAligned",
      resizable: true
    },
    {
      headerName: "Inactive date",
      field: "terminationDate",
      colId: "terminationDate",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => mmDDYYFormat(params.value)
    }
  ];
};
