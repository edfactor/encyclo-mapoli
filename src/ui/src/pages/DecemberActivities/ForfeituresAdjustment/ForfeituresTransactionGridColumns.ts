import { ColDef } from "ag-grid-community";
import {
  createCurrencyColumn,
  createHoursColumn,
  createStateColumn,
  createYearColumn,
  createYesOrNoColumn
} from "../../../utils/gridColumnFactory";

export const GetForfeituresTransactionGridColumns = (): ColDef[] => {
  return [
    createYearColumn({
      headerName: "Profit Year",
      field: "profitYear",

      sortable: true,
      valueFormatter: (params) => {
        const year = params.data.profitYear;
        const iter = params.data.profitYearIteration;
        return `${year}.${iter}`;
      }
    }),
    {
      headerName: "Profit Code",
      field: "profitCodeId",
      colId: "profitCodeId",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      tooltipValueGetter: (params) => {
        return params.data?.profitCodeName;
      },
      valueFormatter: (params) => {
        const id = params.data.profitCodeId;
        const name = params.data.profitCodeName;
        return `[${id}] ${name}`;
      }
    },
    createCurrencyColumn({
      headerName: "Forfeiture",
      field: "forfeiture",
      cellStyle: (params) => {
        // Highlight forfeitures/unforfeitures
        if (params.value !== 0) {
          return { backgroundColor: "#fff3cd", fontWeight: "bold" };
        }
        return null;
      }
    }),
    createCurrencyColumn({
      headerName: "Contribution",
      field: "contribution"
    }),
    createCurrencyColumn({
      headerName: "Earnings",
      field: "earnings"
    }),
    createCurrencyColumn({
      headerName: "Payment",
      field: "payment"
    }),
    {
      headerName: "Month/Year",
      headerTooltip: "Month to Date",
      field: "monthToDate",
      colId: "monthToDate",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      sortable: false,
      resizable: true,
      valueFormatter: (params) => {
        const month = params.data.monthToDate;
        const year = params.data.yearToDate;
        const formattedMonth = month.toString().padStart(2, "0");

        if (month === 0 && year === 0) {
          return "";
        }

        return `${formattedMonth}/${year}`;
      }
    },
    createHoursColumn({
      field: "currentHoursYear"
    }),
    createCurrencyColumn({
      headerName: "Wages",
      field: "currentIncomeYear"
    }),
    {
      headerName: "Comment Type",
      headerTooltip: "Comment Type",
      field: "commentTypeName",
      colId: "commentTypeName",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Check Number",
      headerTooltip: "Check Number",
      field: "commentRelatedCheckNumber",
      colId: "commentRelatedCheckNumber",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: false
    },
    createStateColumn({
      field: "commentRelatedState",
      minWidth: 60,
      alignment: "left"
    }),
    createYesOrNoColumn({
      headerName: "Partial Transaction",
      field: "commentIsPartialTransaction",
      minWidth: 120,
      useWords: true
    })
  ];
};
