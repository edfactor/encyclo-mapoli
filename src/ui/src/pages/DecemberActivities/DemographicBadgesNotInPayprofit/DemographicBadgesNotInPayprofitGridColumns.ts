import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { createSSNColumn, createBadgeColumn } from "../../../utils/gridColumnFactory";
export const GetDemographicBadgesNotInPayprofitColumns = (): ColDef[] => {
  return [
    createBadgeColumn({ 
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      maxWidth: 200,
      alignment: "left",
      renderAsLink: false
    }),
    createSSNColumn({ 
      headerName: "SSN", 
      alignment: "left", 
      maxWidth: 250 
    }),
    {
      headerName: "Name",
      field: "employeeName",
      colId: "employeeName",
      minWidth: 150,
      maxWidth: 300,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Store",
      field: "store",
      colId: "store",
      minWidth: 50,
      maxWidth: 170,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Status",
      field: "status",
      colId: "status",
      minWidth: 80,
      maxWidth: 250,
      type: "rightAligned",
      resizable: true,
      valueFormatter: (params) => {
        const status = params.data.status; // assuming 'status' is in the row data
        const statusName = params.data.statusName; // assuming 'statusName' is in the row data
        return `[${status}] ${statusName}`;
      }
    }
  ];
};
