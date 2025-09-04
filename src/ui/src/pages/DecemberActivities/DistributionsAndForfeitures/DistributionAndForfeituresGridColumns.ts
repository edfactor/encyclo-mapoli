import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { getEnrolledStatus } from "../../../utils/enrollmentUtil";
import {
  createAgeColumn,
  createBadgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createSSNColumn
} from "../../../utils/gridColumnFactory";

export const GetDistributionsAndForfeituresColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left",
      psnSuffix: true
    }),
    {
      headerName: "Name",
      field: "employeeName",
      colId: "employeeName",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
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
    {
      headerName: "State",
      field: "state",
      colId: "state",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
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
