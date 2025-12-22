import { ColDef } from "ag-grid-community";
import { createBadgeColumn, createNameColumn } from "../../../utils/gridColumnFactory";

export const GetTerminationColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge/PSN",
      field: "psn",
      psnSuffix: false // No suffix needed - PSN is already combined
    }),
    createNameColumn({
      field: "name"
    })
  ];
};
