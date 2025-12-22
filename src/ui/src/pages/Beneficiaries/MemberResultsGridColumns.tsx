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
    createBadgeColumn({ headerName: "Badge/Psn", minWidth: 120, psnSuffix: true }),
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
