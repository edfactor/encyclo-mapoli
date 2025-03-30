import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";
export const GetDemographicBadgesNotInPayprofitColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => {
        const badgeNumber = params.value;
        return badgeNumber ? badgeNumber.toString().padStart(7, "0") : "";
      }
    },
    {
      headerName: "Demographic SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Name",
      field: "employeeName",
      colId: "employeeName",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Store",
      field: "store",
      colId: "store",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Status",
      field: "status",
      colId: "status",
      minWidth: 80,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        const status = params.data.status; // assuming 'status' is in the row data
        const statusName = params.data.statusName; // assuming 'statusName' is in the row data
        return `[${status}] ${statusName}`;
      }
    }
  ];
};
