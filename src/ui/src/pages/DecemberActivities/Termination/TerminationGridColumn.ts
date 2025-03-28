import { ColDef, ICellRendererParams } from 'ag-grid-community';
import { viewBadgeLinkRenderer } from 'utils/masterInquiryLink';
import { agGridNumberToCurrency } from "smart-ui-library";

export const GetTerminationColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgePSn",
      colId: "badgePSn",
      minWidth: 80,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgePSn, navFunction)
    },
    {
      headerName: "Name",
      field: "name",
      colId: "name",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Beginning Balance",
      field: "beginningBalance",
      colId: "beginningBalance",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Beneficiary Allocation",
      field: "beneficiaryAllocation",
      colId: "beneficiaryAllocation",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Distribution Amount",
      field: "distributionAmount",
      colId: "distributionAmount",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Forfeit",
      field: "forfeit",
      colId: "forfeit",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Ending Balance",
      field: "endingBalance",
      colId: "endingBalance",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Vested Balance",
      field: "vestedBalance",
      colId: "vestedBalance",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Term Date",
      field: "dateTerm",
      colId: "dateTerm",
      minWidth: 100,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "YTD PS Hours",
      field: "ytdPsHours",
      colId: "ytdPsHours",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Vested %",
      field: "vestedPercent",
      colId: "vestedPercent",
      minWidth: 90,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => `${params.value}%`
    },
    {
      headerName: "Age",
      field: "age",
      colId: "age",
      minWidth: 70,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Enrollment Code",
      field: "enrollmentCode",
      colId: "enrollmentCode",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: true
    }
  ];
};
