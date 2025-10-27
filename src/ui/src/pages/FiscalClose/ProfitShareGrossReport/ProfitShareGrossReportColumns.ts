import { ColDef, ICellRendererParams } from "ag-grid-community";
import { numberToCurrency } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";
import { GRID_COLUMN_WIDTHS } from "../../../constants";

export const GetProfitShareGrossReportColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      type: "rightAligned",
      resizable: true,
      sortable: true,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber, navFunction)
    },
    {
      headerName: "Name",
      field: "employeeName",
      colId: "employeeName",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: GRID_COLUMN_WIDTHS.SSN,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Date of Birth",
      field: "dateOfBirth",
      colId: "dateOfBirth",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "P/S Wages",
      field: "grossWages",
      colId: "grossWages",
      minWidth: 150,
      type: "rightAligned",
      resizable: true,
      valueFormatter: (params) => numberToCurrency(params.value)
    },
    {
      headerName: "P/S Amount",
      field: "profitSharingAmount",
      colId: "profitSharingAmount",
      minWidth: 150,
      type: "rightAligned",
      resizable: true,
      valueFormatter: (params) => numberToCurrency(params.value)
    },
    {
      headerName: "Distrubutions",
      field: "loans",
      colId: "loans",
      minWidth: 120,
      type: "rightAligned",
      resizable: true,
      valueFormatter: (params) => numberToCurrency(params.value)
    },
    {
      headerName: "Forfeitures",
      field: "forfeitures",
      colId: "forfeitures",
      minWidth: 120,
      type: "rightAligned",
      resizable: true,
      valueFormatter: (params) => numberToCurrency(params.value)
    },
    {
      headerName: "EC",
      field: "enrollmentId",
      colId: "enrollmentId",
      minWidth: 100,
      type: "rightAligned",
      resizable: true
    }
  ];
};
