import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";

export const PayBenReportColumnDef = (): ColDef[] => {
  return [
    {
      headerName: "Psn-Num",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: true,
      unSortIcon: true,
      cellRenderer: (params: ICellRendererParams) =>
        viewBadgeLinkRenderer(params.data.badgeNumber, params.data.psnSuffix)
    },
    {
      headerName: "SSN",
      field: "maskedSsn",
      colId: "maskedSsn",
      minWidth: 170,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Beneficiary",
      field: "beneficiaryFullName",
      colId: "beneficiaryFullName",
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    {
      headerName: "PSN",
      field: "psn",
      colId: "psn",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    {
      headerName: "BADGE #",
      field: "badge",
      colId: "badge",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    {
      headerName: "Beneficiary of",
      field: "demographicFullName",
      colId: "demographicFullName",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    {
      headerName: "Percent",
      field: "percent",
      colId: "percent",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    }
  ];
};


