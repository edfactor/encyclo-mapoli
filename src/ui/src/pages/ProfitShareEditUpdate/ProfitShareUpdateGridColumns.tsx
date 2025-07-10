import {agGridNumberToCurrency} from "smart-ui-library";
import {ColDef} from "ag-grid-community";

export const ProfitShareUpdateGridColumns = (): ColDef[] => {
    return [
        {
            headerName: "Number",
            field: "psn",
            colId: "psn",
            minWidth: 100,
            type: "rightAligned",
            resizable: true
        },
        {
            headerName: "Name",
            field: "name",
            colId: "name",
            minWidth: 80,
            headerClass: "left-align",
            cellClass: "left-align",
            resizable: true
        },
        {
            headerName: "Beginning Balance",
            field: "beginningAmount",
            colId: "beginningAmount",
            minWidth: 120,
            type: "rightAligned",
            resizable: true,
            valueFormatter: agGridNumberToCurrency
        },
        {
            headerName: "Contributions",
            field: "contributions",
            colId: "contributions",
            minWidth: 120,
            type: "rightAligned",
            resizable: true,
            valueFormatter: agGridNumberToCurrency
        },
        {
            headerName: "Earnings",
            field: "allEarnings",
            colId: "allEarnings",
            minWidth: 120,
            type: "rightAligned",
            resizable: true,
            valueFormatter: agGridNumberToCurrency
        }, {
            headerName: "Earnings2",
            field: "allSecondaryEarnings",
            colId: "allSecondaryEarnings",
            minWidth: 120,
            type: "rightAligned",
            resizable: true,
            valueFormatter: agGridNumberToCurrency
        },
        {
            headerName: "Forfeiture",
            field: "incomingForfeitures",
            colId: "incomingForfeitures",
            minWidth: 120,
            type: "rightAligned",
            resizable: true,
            valueFormatter: agGridNumberToCurrency
        },
        {
            headerName: "Distributions",
            field: "distributions",
            colId: "distributions",
            minWidth: 100,
            type: "rightAligned",
            resizable: true,
            valueFormatter: agGridNumberToCurrency
        },
        {
            headerName: "Military/Paid Allocation",
            field: "pxfer",
            colId: "pxfer",
            minWidth: 100,
            type: "rightAligned",
            resizable: true,
            valueFormatter: agGridNumberToCurrency
        },
        {
            headerName: "Ending Balance",
            field: "endingBalance",
            colId: "endingBalance",
            minWidth: 150,
            type: "rightAligned",
            resizable: true,
            valueFormatter: agGridNumberToCurrency
        },
    ];
};