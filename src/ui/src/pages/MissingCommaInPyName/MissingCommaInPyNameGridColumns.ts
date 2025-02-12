import { ColDef } from "ag-grid-community";
import { viewBadgeRenderer } from "../../utils/masterInquiryLink";

export const GetMissingCommaInPyNameColumns = (): ColDef[] => {
    return [
      {
        headerName: "Badge",
        field: "badgeNumber",
        colId: "badgeNumber",
        minWidth: 80,
        headerClass: "right-align",
        cellClass: "right-align",
        resizable: true,
        cellRenderer: (params) => viewBadgeRenderer({ value: params.data.badgeNumber }),
      },
      {
        headerName: "SSN",
        field: "ssn",
        colId: "ssn",
        minWidth: 100,
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
    ];
  };