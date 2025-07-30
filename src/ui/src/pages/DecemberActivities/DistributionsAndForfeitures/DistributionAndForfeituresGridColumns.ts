import { ColDef } from "ag-grid-community";
import { agGridNumberToCurrency, yyyyMMDDToMMDDYYYY } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { getEnrolledStatus } from "../../../utils/enrollmentUtil";
import { createBadgeColumn, createSSNColumn } from "../../../utils/gridColumnFactory";

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
    {
      headerName: "Date",
      field: "date",
      colId: "date",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => (params.value ? yyyyMMDDToMMDDYYYY(params.value) : "")
    },
    {
      headerName: "Distribution",
      field: "distributionAmount",
      colId: "distributionAmount",
      minWidth: 120,
      type: "rightAligned",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "State Tax",
      field: "stateTax",
      colId: "stateTax",
      minWidth: 120,
      type: "rightAligned",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "State",
      field: "state",
      colId: "state",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Federal Tax",
      field: "federalTax",
      colId: "federalTax",
      minWidth: 120,
      type: "rightAligned",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Forfeit Amount",
      field: "forfeitAmount",
      colId: "forfeitAmount",
      minWidth: 120,
      type: "rightAligned",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Age",
      field: "age",
      colId: "age",
      minWidth: 70,
      type: "rightAligned",
      resizable: true
    },
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
