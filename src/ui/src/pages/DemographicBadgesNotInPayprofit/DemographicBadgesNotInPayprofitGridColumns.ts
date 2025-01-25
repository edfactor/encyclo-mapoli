import { ColDef } from "ag-grid-community";

export const GetDemographicBadgesNotInPayprofitColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "employeeBadge",
      colId: "employeeBadge",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true
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
      resizable: true
    },
  ];
};