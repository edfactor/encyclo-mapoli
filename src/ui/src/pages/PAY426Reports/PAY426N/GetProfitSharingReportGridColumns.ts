import { ColDef } from "ag-grid-community";
import { formatNumberWithComma } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { mmDDYYFormat } from "../../../utils/dateUtils";
import { createBadgeColumn, createCurrencyColumn, createSSNColumn } from "../../../utils/gridColumnFactory";

export const GetProfitSharingReportGridColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  // Import the date formatter from dateutils

  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "center",
      navigateFunction: navFunction
    }),
    {
      headerName: "Name",
      field: "employeeName",
      colId: "employeeName",
      minWidth: 180,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Store",
      field: "storeNumber",
      colId: "storeNumber",
      minWidth: 80,
      type: "rightAligned",
      resizable: true
    },
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
    {
      headerName: "Age",
      field: "age",
      colId: "age",
      minWidth: 70,
      type: "rightAligned",
      resizable: true
    },
    createSSNColumn(),
    createCurrencyColumn({
      headerName: "Wages",
      field: "wages",
      minWidth: 120
    }),
    {
      headerName: "Hours",
      field: "hours",
      colId: "hours",
      minWidth: 100,
      type: "rightAligned",
      resizable: true,
      valueFormatter: (params) => formatNumberWithComma(params.value)
    },
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
    {
      headerName: "Status",
      field: "employeeStatus",
      colId: "employeeStatus",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
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
