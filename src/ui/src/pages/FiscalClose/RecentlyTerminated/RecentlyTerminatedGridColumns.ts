import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createDateColumn,
  createNameColumn,
  createSSNColumn,
  createStatusColumn
} from "../../../utils/gridColumnFactory";

export const GetRecentlyTerminatedColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createSSNColumn({}),
    createNameColumn({
      field: "fullName",
      minWidth: 150
    }),
    createDateColumn({
      headerName: "Termination Date",
      field: "terminationDate"
    }),
    createStatusColumn({
      headerName: "Termination Code",
      field: "terminationCodeId"
    })
  ];
};
