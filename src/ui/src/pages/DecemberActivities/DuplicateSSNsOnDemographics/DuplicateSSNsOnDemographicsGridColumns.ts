import { ColDef } from "ag-grid-community";
import { yyyyMMDDToMMDDYYYY } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import {
  createBadgeColumn,
  createSSNColumn,
  createDateColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

export const GetDuplicateSSNsOnDemographicsColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left"
    }),
    createSSNColumn({ alignment: "left" }),
    {
      headerName: "Name",
      field: "name",
      colId: "name",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Address",
      field: "address",
      colId: "address",
      minWidth: 200,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueGetter: (params) => {
        const addr = params.data.address;
        return addr ? `${addr.street}${addr.street2 ? ", " + addr.street2 : ""}` : "";
      }
    },
    {
      headerName: "City",
      field: "city",
      colId: "city",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueGetter: (params) => params.data.address?.city || ""
    },
    {
      headerName: "State",
      field: "state",
      colId: "state",
      minWidth: 60,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueGetter: (params) => params.data.address?.state || ""
    },
    createDateColumn({
      headerName: "Hire",
      field: "hireDate",
      minWidth: 100,
      alignment: "left"
    }),
    createDateColumn({
      headerName: "Rehire",
      field: "rehireDate",
      minWidth: 100,
      alignment: "left"
    }),
    createStoreColumn({
      minWidth: 50
    }),
    {
      headerName: "Status",
      field: "status",
      colId: "status",
      minWidth: 60,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => {
        const id = params.data.status; // assuming 'status' is in the row data
        const name = params.data.employmentStatusName; // assuming 'statusName' is in the row data
        //see if one is undefined or null then show other
        return `[${id}] ${name}`;
      }
    }
  ];
};
