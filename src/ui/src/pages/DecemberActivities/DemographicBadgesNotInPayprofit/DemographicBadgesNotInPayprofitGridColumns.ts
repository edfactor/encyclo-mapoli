import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { createSSNColumn } from "../../../utils/gridColumnFactory";
export const GetDemographicBadgesNotInPayprofitColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      maxWidth: 200,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => {
        const badgeNumber = params.value;
        return badgeNumber ? badgeNumber.toString().padStart(7, "0") : "";
      }
    },
    createSSNColumn({ 
      headerName: "Demographic SSN", 
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
