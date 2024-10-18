import { ColDef } from "ag-grid-community";

export const GetMilitaryAndRehireForfeituresColumns = (): ColDef[] => {
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
      headerName: "SSN",
      field: "employeeSsn",
      colId: "employeeSsn",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => params.value ? `***-**-${params.value.toString().slice(-4)}` : ""
    },
    {
      headerName: "ETVA",
      field: "etvaValue",
      colId: "etvaValue",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
};