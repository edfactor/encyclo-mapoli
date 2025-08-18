import { ColDef, ICellRendererParams } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../constants";
import { createSSNColumn, createNameColumn, createBadgeColumn, createZipColumn } from "../../utils/gridColumnFactory";

export const BeneficiarySearchFilterColumns = (): ColDef[] => {
  return [
    createBadgeColumn({ minWidth: 120 }),
    {
      headerName: "PSN Suffix",
      field: "psnSuffix",
      colId: "psnSuffix",
      flex: 1,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.psnSuffix}`;
      }
    },
    createNameColumn({
      field: "name",
      minWidth: GRID_COLUMN_WIDTHS.FULL_NAME,
      alignment: "center",
      sortable: false
    }),
    createSSNColumn({
      valueFormatter: (params) => {
        return `${params.data.ssn}`;
      }
    }),
    {
      headerName: "City",
      field: "city",
      colId: "city",
      flex: 1,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.city}`;
      }
    },
    {
      headerName: "State",
      field: "state",
      colId: "state",
      flex: 1,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.state}`;
      }
    },
    createZipColumn({ field: "zip", colId: "zip", headerName: "Zip" }),
    {
      headerName: "Age",
      field: "age",
      colId: "age",
      flex: 1,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    }
  ];
};
