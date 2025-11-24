import { ColDef } from "ag-grid-community";
import {
  createAgeColumn,
  createBadgeColumn,
  createCountColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
  createPointsColumn,
  createSSNColumn,
  createStatusColumn,
  createStoreColumn,
  createYesOrNoColumn
} from "../../../../utils/gridColumnFactory";

export const GetProfitSharingReportGridColumns = (navFunction: (badgeNumber: string) => void): ColDef[] => {
  // Import the date formatter from dateutils

  return [
    createBadgeColumn({
      navigateFunction: navFunction
    }),
    createNameColumn({}),
    createStoreColumn({}),
    {
      headerName: "Type",
      field: "employeeTypeCode",
      colId: "employeeTypeCode",
      minWidth: 80,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    createDateColumn({
      headerName: "Date of Birth",
      field: "dateOfBirth"
    }),
    createAgeColumn({}),
    createSSNColumn(),
    createCurrencyColumn({
      headerName: "Wages",
      field: "wages"
    }),
    createHoursColumn({}),
    createPointsColumn({}),
    createYesOrNoColumn({
      headerName: "New",
      field: "isNew",
      colId: "isNew"
    }),
    createStatusColumn({
      field: "employeeStatus",
      valueFormatter: (params) => {
        const value = typeof params.value === "string" ? params.value.toLowerCase() : params.value;
        if (value === "a") return "Active";
        if (value === "i") return "Inactive";
        if (value === "t") return "Terminated";
        return params.value;
      }
    }),
    createCurrencyColumn({
      headerName: "Balance",
      field: "balance"
    }),
    createCountColumn({
      headerName: "Years in Plan",
      field: "yearsInPlan"
    }),
    createDateColumn({
      headerName: "Inactive date",
      field: "terminationDate"
    })
  ];
};
