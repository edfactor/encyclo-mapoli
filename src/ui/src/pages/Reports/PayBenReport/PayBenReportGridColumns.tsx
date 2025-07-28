import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";
import { createSSNColumn, createBadgeColumn } from "../../../utils/gridColumnFactory";

export const PayBenReportGridColumn = (): ColDef[] => {
  return [
    createSSNColumn({ minWidth: 170, sortable: false }),
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
      resizable: true,
      cellRenderer: (params: ICellRendererParams) =>
        viewBadgeLinkRenderer(params.data.badgeNumber, parseInt(params.data.psn.slice(-4)))
    },
    createBadgeColumn({ minWidth: 120 }),
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
      field: "percentage",
      colId: "percentage",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    }
  ];
};
