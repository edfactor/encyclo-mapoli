import { ColDef } from "ag-grid-community";

export const GetMissingCommaInPyNameColumns = (): ColDef[] => {
    return [
      {
        headerName: "Badge",
        field: "employeeBadge",
        colId: "employeeBadge",
        minWidth: 80,
        headerClass: "right-align",
        cellClass: "right-align",
        resizable: true
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