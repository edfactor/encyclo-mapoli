import { ColDef } from "ag-grid-community";
import {
  createAgeColumn,
  createBadgeColumn,
  createCountColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
  createSSNColumn
} from "utils/gridColumnFactory";

export const GetUnder21ReportColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  return [
    createBadgeColumn({
      navigateFunction: navFunction,
      renderAsLink: true
    }),
    createNameColumn({
      field: "fullName"
    }),
    createSSNColumn({}),
    createCountColumn({
      headerName: "P/S Years",
      field: "psYears"
    }),
    {
      headerName: "NE",
      field: "ne",
      colId: "ne",
      minWidth: 70,
      type: "rightAligned",
      resizable: true
    },
    createHoursColumn({
      headerName: "This Year P/S Hours",
      field: "thisYearPSHours"
    }),
    createHoursColumn({
      headerName: "Last Year P/S Hours",
      field: "lastYearPSHours"
    }),
    {
      headerName: "Type",
      field: "type",
      colId: "type",
      minWidth: 80,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    createDateColumn({
      headerName: "Hire Date",
      field: "hireDate"
    }),
    createDateColumn({
      headerName: "Full Date",
      field: "fullDate"
    }),
    createDateColumn({
      headerName: "Term Date",
      field: "termDate"
    }),
    createDateColumn({
      headerName: "Birth Date",
      field: "birthDate"
    }),
    createAgeColumn({})
  ];
};
