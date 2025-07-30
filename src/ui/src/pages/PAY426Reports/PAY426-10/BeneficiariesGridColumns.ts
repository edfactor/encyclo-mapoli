import { ColDef } from "ag-grid-community";
import { formatNumberWithComma } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { mmDDYYFormat } from "../../../utils/dateUtils";
import { createBadgeColumn, createCurrencyColumn, createSSNColumn, createAgeColumn, createStoreColumn } from "../../../utils/gridColumnFactory";

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
    createAgeColumn({});
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
    createCurrencyColumn({
      headerName: "Current Balance",
      field: "balance",
      minWidth: 140
    }),
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
