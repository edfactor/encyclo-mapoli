import { ColDef } from "ag-grid-community";
import { mmDDYYYY_HHMMSS_Format } from "../../../utils/dateUtils";
import { createDateColumn, createYearColumn } from "../../../utils/gridColumnFactory";

export const GetFreezeColumns = (): ColDef[] => {
  return [
    createYearColumn({
      headerName: "Profit Year",
      field: "profitYear"
    }),
    createDateColumn({
      headerName: "Frozen Date/Time",
      field: "asOfDateTime"
    }),
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
    createDateColumn({
      headerName: "Created Date/Time",
      field: "createdDateTime",
      minWidth: 150,
      alignment: "left",
      valueFormatter: (params) => (params.value ? mmDDYYYY_HHMMSS_Format(params.value) : "")
    })
  ];
};
