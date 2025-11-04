import { ColDef } from "ag-grid-community";
import {
  createAgeColumn,
  createBadgeColumn,
  createNameColumn,
  createSSNColumn,
  createStateColumn,
  createZipColumn
} from "../../utils/gridColumnFactory";

export const GetMemberResultsGridColumns = (): ColDef[] => {
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
      field: "name"
    }),
    createSSNColumn({}),
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
    createStateColumn({}),
    createZipColumn({ field: "zip", colId: "zip", headerName: "Zip" }),
    createAgeColumn({})
  ];
};
