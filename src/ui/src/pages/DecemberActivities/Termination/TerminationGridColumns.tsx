import { ColDef } from "ag-grid-community";
import { createBadgeColumn, createNameColumn } from "../../../utils/gridColumnFactory";

export const GetTerminationColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      psnSuffix: true
    }),
    createNameColumn({})
  ];
};
