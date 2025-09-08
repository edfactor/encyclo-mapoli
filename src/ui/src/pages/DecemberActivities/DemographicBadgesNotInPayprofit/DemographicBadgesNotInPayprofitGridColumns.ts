import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createNameColumn,
  createSSNColumn,
  createStatusColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";
export const GetDemographicBadgesNotInPayprofitColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",

      renderAsLink: false
    }),
    createSSNColumn({}),
    createNameColumn({
      field: "employeeName"
    }),
    createStoreColumn({
      field: "store"
    }),
    createStatusColumn({
      alignment: "right",
      valueFormatter: (params) => {
        const status = params.data.status; // assuming 'status' is in the row data
        const statusName = params.data.statusName; // assuming 'statusName' is in the row data
        return `[${status}] ${statusName}`;
      }
    })
  ];
};
