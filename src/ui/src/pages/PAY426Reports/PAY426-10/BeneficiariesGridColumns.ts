import { ColDef } from "ag-grid-community";
import { formatNumberWithComma } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import {
  createAgeColumn,
  createBadgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
  createSSNColumn,
  createStatusColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

export const GetBeneficiariesGridColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
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
    createStatusColumn({
      headerName: "Type",
      field: "employeeTypeCode"
    }),
    createDateColumn({
      headerName: "Date of Birth",
      field: "dateOfBirth"
    }),
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
      minWidth: 80,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => (params.value ? "(<21)" : "")
    },
    createDateColumn({
      headerName: "Term Date",
      field: "terminationDate"
    }),
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
