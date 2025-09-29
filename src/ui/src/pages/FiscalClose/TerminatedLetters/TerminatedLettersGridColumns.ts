import { ColDef } from "ag-grid-community";
import {
  createAddressColumn,
  createBadgeColumn,
  createNameColumn,
  createSSNColumn,
  createStateColumn
} from "../../../utils/gridColumnFactory";

export const GetTerminatedLettersColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createNameColumn({
      headerName: "Name",
      field: "fullName",
      minWidth: 200
    }),
    createSSNColumn({}),
    createAddressColumn({}),
    {
      headerName: "City",
      field: "city",
      colId: "city",
      minWidth: 80,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    createStateColumn({
      alignment: "center",
      sortable: true
    }),
    {
      headerName: "Print",
      width: 95,
      maxWidth: 95,
      minWidth: 95,
      pinned: "right",
      lockPosition: "right",

      suppressSizeToFit: true,
      suppressAutoSize: true,
      suppressColumnsToolPanel: true,

      suppressMovable: true
    }
  ];
};
