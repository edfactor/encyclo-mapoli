import { style, textTransform } from "@mui/system";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import { Link } from '@mui/material';
import { NegativeEtvaForSSNsOnPayProfit } from "reduxstore/types";

export const GetNegativeEtvaForSSNsOnPayProfitColumns = (viewBadge: Function): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "employeeBadge",
      colId: "employeeBadge",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      cellRenderer: viewBadge
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
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