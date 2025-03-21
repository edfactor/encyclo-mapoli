import { ColDef } from "ag-grid-community";
import { mmDDYYYY_HHMMSS_Format } from "../../../utils/dateUtils";

export const GetFreezeColumns = (): ColDef[] => {
  return [
    {
      headerName: "Profit Year",
      field: "profitYear",
      colId: "profitYear",
      minWidth: 80,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sort: "asc"
    },
    {
      headerName: "As of Date/Time",
      field: "asOfDateTime",
      colId: "asOfDateTime",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => (params.value ? mmDDYYYY_HHMMSS_Format(params.value) : "")
    },
    {
      headerName: "IsActive Freeze",
      field: "isActive",
      colId: "isActive",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true      
    },
    {
      headerName: "Frozen By",
      field: "frozenBy",
      colId: "frozenBy",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Created Date/Time",
      field: "createdDateTime",
      colId: "createdDateTime",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => (params.value ? mmDDYYYY_HHMMSS_Format(params.value) : "")
    }
  ];
};
