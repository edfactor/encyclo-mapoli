import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";

export const GetProfitShareGrossReportColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
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
      minWidth: 120,
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
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "P/S Amount",
      field: "profitSharingAmount",
      colId: "profitSharingAmount",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Loans",
      field: "loans",
      colId: "loans",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Forfeitures",
      field: "forfeitures",
      colId: "forfeitures",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "EC",
      field: "enrollmentId",
      colId: "enrollmentId",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
    }
  ];
};
