import { ColDef } from "ag-grid-community";
import { yyyyMMDDToMMDDYYYY } from "smart-ui-library";
import { GRID_COLUMN_WIDTHS } from "../../constants";
import { createBadgeColumn, createCurrencyColumn, createSSNColumn, createAgeColumn, createDateColumn, createStoreColumn } from "../../utils/gridColumnFactory";

export const GetProfitShareReportColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left"
    }),
    {
      headerName: "Name",
      field: "employeeName",
      colId: "employeeName",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
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
    {
      headerName: "Age",
      field: "age",
      colId: "age",
      minWidth: 50,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    createSSNColumn({ alignment: "left" }),
    createCurrencyColumn({
      headerName: "Wages",
      field: "wages",
      minWidth: 150
    }),
    {
      headerName: "Hours",
      field: "hours",
      colId: "hours",
      minWidth: 150,
      type: "rightAligned",
      resizable: true
    },
    {
      headerName: "Points",
      field: "points",
      colId: "points",
      minWidth: 150,
      type: "rightAligned",
      resizable: true
    },
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
