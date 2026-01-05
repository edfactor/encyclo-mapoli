import { ColDef } from "ag-grid-community";
import {
    createBadgeColumn,
    createDateColumn,
    createNameColumn,
    createSSNColumn,
    createStatusColumn
} from "../../../utils/gridColumnFactory";

export const GetRecentlyTerminatedColumns = (): ColDef[] => {
  const terminationCodeColumn = createStatusColumn({
    headerName: "Termination Code",
    field: "terminationCode"
  });

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
    { ...terminationCodeColumn, flex: 1 }
  ];
};
