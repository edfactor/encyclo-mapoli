import { ColDef, ICellRendererParams } from "ag-grid-community";
import { numberToCurrency } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";
import { GRID_COLUMN_WIDTHS } from "../../../constants";

export const GetProfitShareGrossReportColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "BadgeNumber",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      type: "rightAligned",
      resizable: true,
      sortable: true,
      filter: false,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber, navFunction)
    },
    {
      headerName: "Name",
      field: "employeeName",
      colId: "EmployeeName",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      filter: false
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "Ssn",
      minWidth: GRID_COLUMN_WIDTHS.SSN,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      filter: false
    },
    {
      headerName: "Date of Birth",
      field: "dateOfBirth",
      colId: "DateOfBirth",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      filter: false
    },
    {
      headerName: "P/S Wages",
      field: "grossWages",
      colId: "GrossWages",
      minWidth: 150,
      type: "rightAligned",
      resizable: true,
      sortable: true,
      filter: false,
      valueFormatter: (params) => numberToCurrency(params.value)
    },
    {
      headerName: "P/S Amount",
      field: "profitSharingAmount",
      colId: "ProfitSharingAmount",
      minWidth: 150,
      type: "rightAligned",
      resizable: true,
      sortable: true,
      filter: false,
      valueFormatter: (params) => numberToCurrency(params.value)
    },
    {
      headerName: "Distrubutions",
      field: "loans",
      colId: "Loans",
      minWidth: 120,
      type: "rightAligned",
      resizable: true,
      sortable: true,
      filter: false,
      valueFormatter: (params) => numberToCurrency(params.value)
    },
    {
      headerName: "Forfeitures",
      field: "forfeitures",
      colId: "Forfeitures",
      minWidth: 120,
      type: "rightAligned",
      resizable: true,
      sortable: true,
      filter: false,
      valueFormatter: (params) => numberToCurrency(params.value)
    },
    {
      headerName: "EC",
      field: "enrollmentId",
      colId: "EnrollmentId",
      minWidth: 100,
      type: "rightAligned",
      resizable: true,
      sortable: true,
      filter: false
    }
  ];
};
