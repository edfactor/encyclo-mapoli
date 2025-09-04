import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../constants";
import {
  createAgeColumn,
  createBadgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
  createPointsColumn,
  createSSNColumn,
  createStoreColumn
} from "../../utils/gridColumnFactory";

export const GetProfitShareReportColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left"
    }),
    createNameColumn({
      field: "employeeName",
      minWidth: 120
    }),
    createStoreColumn({
      minWidth: 60
    }),
    {
      headerName: "Employee Type",
      field: "employeeTypeCode",
      colId: "employeeTypeCode",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => {
        const id = params.data.employeeTypeCode;
        const name = params.data.employeeTypeName;
        return `${id} - ${name}`;
      }
    },
    createDateColumn({
      headerName: "Date of Birth",
      field: "dateOfBirth",
      minWidth: 150,
      alignment: "left"
    }),
    createAgeColumn({}),

    createSSNColumn({ alignment: "left" }),
    createCurrencyColumn({
      headerName: "Wages",
      field: "wages",
      minWidth: 150
    }),
    createHoursColumn({
      minWidth: 150
    }),
    createPointsColumn({
      
    }),
    {
      headerName: "",
      colId: "isNew",
      minWidth: 150,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return params.data.isUnder21 ? "< 21" : params.data.isNew ? "New" : "";
      }
    }
  ];
};
