import { ColDef } from "ag-grid-community";
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
} from "../../../utils/gridColumnFactory";

export const GetProfitShareReportColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createNameColumn({
      field: "employeeName"
    }),
    createStoreColumn({}),
    {
      headerName: "Employee Type",
      field: "employeeTypeCode",
      colId: "employeeTypeCode",

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
      field: "dateOfBirth"
    }),
    createAgeColumn({}),

    createSSNColumn({ alignment: "left" }),
    createCurrencyColumn({
      headerName: "Wages",
      field: "wages"
    }),
    createHoursColumn({}),
    createPointsColumn({}),
    {
      headerName: "Notes",
      colId: "isNew",

      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return params.data.isUnder21 ? "< 21" : params.data.isNew ? "New" : "";
      }
    }
  ];
};
