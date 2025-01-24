import { agGridNumberToCurrency } from "smart-ui-library";
import { FrozenReportsByAgeRequestType } from "../../reduxstore/types";
import { ColDef } from "ag-grid-community";

export interface IDistributionByAgeGridColumns {
  headerName: string;
  children?: ColDef[];
}

export const GetDistributionsByAgeColumns = (
  reportType: FrozenReportsByAgeRequestType
): IDistributionByAgeGridColumns => {
  return {
    headerName: reportType,
    children: [
      {
        headerName: "Age",
        field: "age",
        colId: "age",
        minWidth: 80,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true,
        sort: "asc",
        cellDataType: "text"
      },
      {
        headerName: "EMPS",
        field: "employeeCount",
        colId: "employeeCount",
        minWidth: 100,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true,
        valueGetter: (params) => {
          const regular = params.data.regularEmployeeCount || 0; // Fallback to 0 if not provided
          const hardship = params.data.hardshipEmployeeCount || 0; // Fallback to 0 if not provided
          return `${params.data.employeeCount} (R:${regular}, H:${hardship})`;
        },
        cellStyle: (params) => {
          const regular = params.data.regularEmployeeCount || 0;
          const hardship = params.data.hardshipEmployeeCount || 0;
          const total = regular + hardship;
          return total !== params.data.employeeCount ? { backgroundColor: "yellow" } : null;
        }
      },
      {
        headerName: "Amount",
        field: "amount",
        colId: "amount",
        minWidth: 150,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Regular",
        field: "regularAmount",
        colId: "regularAmount",
        minWidth: 150,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true,
        valueFormatter: agGridNumberToCurrency
      },
      {
        headerName: "Hardship",
        field: "hardshipAmount",
        colId: "hardshipAmount",
        minWidth: 150,
        headerClass: "left-align",
        cellClass: "left-align",
        resizable: true,
        valueFormatter: agGridNumberToCurrency
      }
    ]
  };
};
