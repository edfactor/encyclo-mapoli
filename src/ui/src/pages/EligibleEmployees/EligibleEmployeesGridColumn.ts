import { ColDef } from "ag-grid-community";
import { viewBadgeRenderer } from "../../utils/masterInquiryLink";

export const GetEligibleEmployeesColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 80,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      cellRenderer: (params) => viewBadgeRenderer({ value: params.data.badgeNumber }),
    },
    {
      headerName: "Full Name",
      field: "fullName",
      colId: "fullName",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
    },
    {
      headerName: "Assignment ID",
      field: "oracleHcmId",
      colId: "oracleHcmId",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
};