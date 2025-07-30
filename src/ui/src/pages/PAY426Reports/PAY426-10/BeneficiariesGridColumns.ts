import { agGridNumberToCurrency, formatNumberWithComma } from "smart-ui-library";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { mmDDYYFormat } from "../../../utils/dateUtils";
import { createSSNColumn, createBadgeColumn } from "../../../utils/gridColumnFactory";

export const GetBeneficiariesGridColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
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
    {
      headerName: "Wages",
      field: "wages",
      colId: "wages",
      minWidth: 120,
      type: "rightAligned",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
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
      minWidth: 80,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => (params.value ? "(<21)" : "")
    },
    {
      headerName: "Term Date",
      field: "terminationDate",
      colId: "terminationDate",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => mmDDYYFormat(params.value)
    },
    {
      headerName: "Current Balance",
      field: "balance",
      colId: "balance",
      minWidth: 140,
      type: "rightAligned",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "SVC",
      field: "svc",
      colId: "svc",
      minWidth: 80,
      type: "rightAligned",
      resizable: true
    }
  ];
};
