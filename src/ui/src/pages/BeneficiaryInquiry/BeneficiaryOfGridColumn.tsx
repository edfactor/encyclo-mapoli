import { ColDef, ICellRendererParams } from "ag-grid-community";
import { mmDDYYFormat } from "utils/dateUtils";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";
import { GRID_COLUMN_WIDTHS } from "../../constants";
import { createSSNColumn, createNameColumn, createBadgeColumn, createCurrencyColumn, createDateColumn } from "../../utils/gridColumnFactory";

export const BeneficiaryOfGridColumns = (): ColDef[] => {
    return [
        createBadgeColumn({ minWidth: 120 }),
        {
            headerName: "PSN_SUFFIX",
            field: "psnSuffix",
            colId: "psnSuffix",
            flex: 1,
            headerClass: "center-align",
            cellClass: "center-align",
            resizable: true
        },
        createNameColumn({
            field: "fullName",
            minWidth: GRID_COLUMN_WIDTHS.FULL_NAME,
            alignment: "center",
            sortable: false,
            valueFormatter: (params) => {
                return `${params.data.lastName}, ${params.data.firstName}`;
            }
        }),
        createSSNColumn({
            valueFormatter: (params) => {
                return `${params.data.ssn}`;
            }
        }),
        createDateColumn({headerName: "Date of Birth", field: "dateOfBirth", colId: "dateOfBirth"}),
        {
            headerName: "Address",
            field: "street",
            colId: "street",
            flex: 1,
            headerClass: "center-align",
            cellClass: "center-align",
            resizable: true,
            valueFormatter: (params) => {
                return `${params.data.street}`;
            }
        },
        {
            headerName: "City",
            field: "city",
            colId: "city",
            flex: 1,
            headerClass: "center-align",
            cellClass: "center-align",
            resizable: true,
            valueFormatter: (params) => {
                return `${params.data.city}`;
            }
        },
        {
            headerName: "State",
            field: "state",
            colId: "state",
            flex: 1,
            headerClass: "center-align",
            cellClass: "center-align",
            resizable: true,
            valueFormatter: (params) => {
                return `${params.data.state}`;
            }
        },
        {
            headerName: "Percent",
            field: "percent",
            colId: "percent",
            flex: 1,
            headerClass: "center-align",
            cellClass: "center-align",
            resizable: true,
            valueFormatter: (params) => {
                return `${params.data.state}`;
            }
        },
        createCurrencyColumn({headerName: "Current", field: "currentBalance", colId : "currentBalance"})
    ];
};
