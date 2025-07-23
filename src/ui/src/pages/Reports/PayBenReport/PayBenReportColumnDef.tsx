import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";

export const PayBenReportColumnDef = (): ColDef[] => {
    return [
        {
            headerName: "SSN",
            field: "ssn",
            colId: "ssn",
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
            resizable: true,
            cellRenderer: (params: ICellRendererParams) =>
                viewBadgeLinkRenderer(params.data.badge, parseInt(params.data.psn.slice(-4)))
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
            field: "percentage",
            colId: "percentage",
            minWidth: 120,
            headerClass: "center-align",
            cellClass: "center-align",
            resizable: true
        }
    ];
};


