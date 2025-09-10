import { ColDef } from "ag-grid-community";
import {
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
    {
      headerName: "Address",
      field: "address",
      colId: "address",
      minWidth: 200,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueGetter: (params) => {
        const address1 = params.data.address || "";
        const address2 = params.data.address2 || "";
        return address2 && address2.trim() ? `${address1}, ${address2}` : address1;
      }
    },
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
      checkboxSelection: true,
      headerCheckboxSelection: true,
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
