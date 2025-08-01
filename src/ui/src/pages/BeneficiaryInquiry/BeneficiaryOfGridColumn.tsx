import { ColDef, ICellRendererParams } from "ag-grid-community";
import { mmDDYYFormat } from "utils/dateUtils";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";
import { GRID_COLUMN_WIDTHS } from "../../constants";
import { createSSNColumn, createNameColumn, createBadgeColumn } from "../../utils/gridColumnFactory";

export const BeneficiaryOfGridColumns = (): ColDef[] => {
    return [
        createBadgeColumn({ minWidth: 120 }),
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
        {
            headerName: "Date of birth",
            field: "dateOfBirth",
            colId: "dateOfBirth",
            flex: 1,
            headerClass: "center-align",
            cellClass: "center-align",
            resizable: true,
            valueFormatter: (params) => {
                return `${mmDDYYFormat(params.data.dateOfBirth)}`;
            }
        },
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
        {
            headerName: "Current            ",
            field: "currentBalance",
            colId: "currentBalance",
            flex: 1,
            headerClass: "center-align",
            cellClass: "center-align",
            resizable: true,
            sortable: false
        }
    ];
};
