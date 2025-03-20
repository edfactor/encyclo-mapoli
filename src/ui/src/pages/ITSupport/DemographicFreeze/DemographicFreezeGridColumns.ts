import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency, yyyyMMDDToMMDDYYYY } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";

export const GetFreezeColumns = (): ColDef[] => {
  return [
    {
      headerName: "Profit Year",
      field: "profitYear",
      colId: "profitYear",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
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
      valueFormatter: (params) => (params.value ? yyyyMMDDToMMDDYYYY(params.value) : "")
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
      resizable: true
    }
  ];
};
