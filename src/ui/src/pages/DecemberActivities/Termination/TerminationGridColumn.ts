import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";
import { getEnrolledStatus } from "../../../utils/enrollmentUtil";

export const GetTerminationColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgePSn",
      colId: "badgePSn",
      width: 80,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber, params.data.psnSuffix)
    },
    {
      headerName: "Name",
      field: "name",
      colId: "name",
      width: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    }
  ];
};

// Separate function for detail columns that will be used for master-detail view
export const GetDetailColumns = (): ColDef[] => {
  return [
    {
      headerName: "Year",
      field: "profitYear",
      colId: "profitYear",
      width: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Beginning Balance",
      field: "beginningBalance",
      colId: "beginningBalance",
      width: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Beneficiary Allocation",
      field: "beneficiaryAllocation",
      colId: "beneficiaryAllocation",
      width: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Distribution Amount",
      field: "distributionAmount",
      colId: "distributionAmount",
      width: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Forfeit",
      field: "forfeit",
      colId: "forfeit",
      width: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Ending Balance",
      field: "endingBalance",
      colId: "endingBalance",
      width: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Vested Balance",
      field: "vestedBalance",
      colId: "vestedBalance",
      width: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Term Date",
      field: "dateTerm",
      colId: "dateTerm",
      width: 100,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "YTD PS Hours",
      field: "ytdPsHours",
      colId: "ytdPsHours",
      width: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Vested %",
      field: "vestedPercent",
      colId: "vestedPercent",
      width: 90,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: (params) => `${params.value}%`
    },
    {
      headerName: "Age",
      field: "age",
      colId: "age",
      width: 70,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Enrollment",
      field: "enrollmentCode",
      colId: "enrollmentCode",
      width: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: false,
      valueFormatter: (params) => getEnrolledStatus(params.value)
    }
  ];
};