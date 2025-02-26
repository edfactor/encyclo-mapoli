import { ColDef } from "ag-grid-community";

export const GetProfallGridColumns = (viewBadge: Function): ColDef[] => {
    return [
        {
            headerName: "Badge",
            field: "badge",
            colId: "badge",
            minWidth: 80,
            headerClass: "right-align",
            cellClass: "right-align",
            resizable: true,
            sortable: true,
            cellRenderer: viewBadge
        },
        {
            headerName: "Employee Name",
            field: "employeeName",
            colId: "employeeName",
            minWidth: 150,
            headerClass: "left-align",
            cellClass: "left-align",
            resizable: true
        },
        {
            headerName: "Address",
            field: "address",
            colId: "address",
            minWidth: 200,
            headerClass: "left-align",
            cellClass: "left-align",
            resizable: true
        },
        {
            headerName: "City",
            field: "city",
            colId: "city",
            minWidth: 120,
            headerClass: "left-align",
            cellClass: "left-align",
            resizable: true
        },
        {
            headerName: "State",
            field: "state",
            colId: "state",
            minWidth: 80,
            headerClass: "center-align",
            cellClass: "center-align",
            resizable: true
        },
        {
            headerName: "Zip Code",
            field: "zipCode",
            colId: "zipCode",
            minWidth: 100,
            headerClass: "center-align",
            cellClass: "center-align",
            resizable: true
        }
    ];
};