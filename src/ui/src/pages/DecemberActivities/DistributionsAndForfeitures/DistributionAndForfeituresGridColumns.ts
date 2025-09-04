import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { getEnrolledStatus } from "../../../utils/enrollmentUtil";
import {
  createAgeColumn,
  createBadgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createNameColumn,
  createSSNColumn,
  createStateColumn
} from "../../../utils/gridColumnFactory";

export const GetDistributionsAndForfeituresColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left",
      psnSuffix: true
    }),
    createNameColumn({
      field: "employeeName",
      minWidth: 150
    }),
    createSSNColumn({ alignment: "left" }),
    createDateColumn({
      headerName: "Date",
      field: "date",
      minWidth: 100,
      alignment: "left"
    }),
    createCurrencyColumn({
      headerName: "Distribution",
      field: "distributionAmount",
      minWidth: 120
    }),
    createCurrencyColumn({
      headerName: "State Tax",
      field: "stateTax",
      minWidth: 120
    }),
    createStateColumn({
      minWidth: 120,
      alignment: "left"
    }),
    createCurrencyColumn({
      headerName: "Federal Tax",
      field: "federalTax",
      minWidth: 120
    }),
    createCurrencyColumn({
      headerName: "Forfeit Amount",
      field: "forfeitAmount",
      minWidth: 120
    }),
    createAgeColumn({}),
    {
      headerName: "Tax Code",
      field: "taxCode",
      colId: "taxCode",
      minWidth: 80,
      type: "rightAligned",
      resizable: true
    },
    {
      headerName: "Enrolled",
      field: "enrolledId",
      colId: "enrolledId",
      minWidth: 90,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return getEnrolledStatus(params.value);
      }
    }
  ];
};
