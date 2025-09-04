import { ColDef } from "ag-grid-community";
import { createCurrencyColumn, createHoursColumn, createStatusColumn, createStateColumn, createYearColumn, createYesOrNoColumn } from "../../utils/gridColumnFactory";

export const GetMasterInquiryGridColumns = (): ColDef[] => {
  return [
    createYearColumn({
      headerName: "Profit Year",
      field: "profitYear",
      minWidth: 100,
      alignment: "right",
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
      type: "leftAligned",
      resizable: true,
      tooltipValueGetter: (params) => {
        return params.data?.profitCodeName;
      },
      valueFormatter: (params) => {
        const id = params.data.profitCodeId; // assuming 'status' is in the row data
        const name = params.data.profitCodeName; // assuming 'statusName' is in the row data
        //see if one is undefined or null then show other
        return `[${id}] ${name}`;
      }
    },
    createCurrencyColumn({
      headerName: "Contribution",
      field: "contribution",
      minWidth: 120
    }),
    createCurrencyColumn({
      headerName: "Earnings",
      field: "earnings",
      minWidth: 120
    }),
    createCurrencyColumn({
      headerName: "Forfeiture",
      field: "forfeiture",
      minWidth: 120
    }),
    createCurrencyColumn({
      headerName: "Payment",
      field: "payment",
      minWidth: 120
    }),
    {
      headerName: "Month/Year",
      headerTooltip: "Month to Date",
      field: "monthToDate",
      colId: "monthToDate",
      minWidth: 100,
      type: "rightAligned",
      sortable: false,
      resizable: true,
      valueFormatter: (params) => {
        const month = params.data.monthToDate; // assuming 'status' is in the row data
        const year = params.data.yearToDate; // assuming 'statusName' is in the row data
        // Format month to always have two digits
        const formattedMonth = month.toString().padStart(2, "0");

        if (month === 0 && year === 0) {
          return "";
        }

        return `${formattedMonth}/${year}`;
      }
    },
    createHoursColumn({
      field: "currentHoursYear",
      minWidth: 120,
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Wages",
      field: "currentIncomeYear",
      minWidth: 120,
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Federal Tax",
      field: "federalTaxes",
      minWidth: 120
    }),
    createCurrencyColumn({
      headerName: "State Tax",
      field: "stateTaxes",
      minWidth: 120
    }),
    {
      headerName: "Tax Code",
      field: "taxCodeId",
      colId: "taxCodeId",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      tooltipValueGetter: (params) => {
        return params.data?.taxCodeName;
      },
      valueFormatter: (params) => {
        const id = params.data?.taxCodeId; // assuming 'status' is in the row data
        const name = params.data?.taxCodeName; // assuming 'statusName' is in the row data

        if (id == 0) return "";
        return `[${id}] ${name}`;
      }
    },
    {
      headerName: "Comment Type",
      headerTooltip: "Comment Type",
      field: "commentTypeName",
      colId: "commentTypeName",
      minWidth: 100,
      type: "leftAligned",
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
    }),
    createStatusColumn({
      field: "employmentStatus",
      minWidth: 60,
      alignment: "right",
      sortable: false
    })
  ];
};
