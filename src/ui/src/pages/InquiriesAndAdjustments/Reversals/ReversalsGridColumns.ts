import { ColDef } from "ag-grid-community";
import {
  createCurrencyColumn,
  createHoursColumn,
  createStatusColumn,
  createTaxCodeColumn,
  createYearColumn
} from "../../../utils/gridColumnFactory";
import { ReversalCheckboxCellRenderer } from "./ReversalCheckboxCellRenderer";

// Profit codes that are allowed to be reversed
export const REVERSIBLE_PROFIT_CODES = [1, 3, 5, 6, 9];

/** Possible statuses for a row's reversal eligibility */
export type ReversalEligibilityStatus = "reversible" | "already-reversed" | "ineligible";

interface ReversalCheckData {
  profitCodeId: number;
  monthToDate: number;
  yearToDate: number;
  isAlreadyReversed?: boolean;
}

/**
 * Determines the reversal eligibility status of a row.
 * @returns "reversible" if can be reversed, "already-reversed" if already has a reversal, "ineligible" otherwise
 */
export const getReversalEligibilityStatus = (data: ReversalCheckData): ReversalEligibilityStatus => {
  if (!data) return "ineligible";

  // Check if this record has already been reversed (double-reversal protection)
  if (data.isAlreadyReversed) {
    return "already-reversed";
  }

  // Check if profit code is in the allowed list
  if (!REVERSIBLE_PROFIT_CODES.includes(data.profitCodeId)) {
    return "ineligible";
  }

  // Check if Month/Year is not more than 2 months ago
  const { monthToDate, yearToDate } = data;
  if (yearToDate && monthToDate) {
    const rowDate = new Date(yearToDate, monthToDate - 1); // monthToDate is 1-based
    const twoMonthsAgo = new Date();
    twoMonthsAgo.setMonth(twoMonthsAgo.getMonth() - 2);

    if (rowDate < twoMonthsAgo) {
      return "ineligible";
    }

    // January rule: if current month is January, transaction month must be > 1 and < 12
    const currentMonth = new Date().getMonth() + 1; // getMonth() is 0-based
    if (currentMonth === 1) {
      if (!(monthToDate > 1 && monthToDate < 12)) {
        return "ineligible";
      }
    }
  }

  return "reversible";
};

// Check if a row is selectable for reversal (backwards compatible wrapper)
export const isRowReversible = (data: ReversalCheckData): boolean => {
  return getReversalEligibilityStatus(data) === "reversible";
};

export const GetReversalsGridColumns = (): ColDef[] => {
  return [
    {
      headerName: "",
      field: "selectable",
      colId: "selectable",
      width: 50,
      minWidth: 50,
      maxWidth: 50,
      pinned: "left",
      cellRenderer: ReversalCheckboxCellRenderer,
      suppressHeaderMenuButton: true,
      sortable: false,
      resizable: false
    },
    {
      headerName: "ID",
      field: "id",
      colId: "id",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      sortable: true,
      resizable: true
    },
    createYearColumn({
      headerName: "Profit Year",
      field: "profitYear",
      maxWidth: 120,
      alignment: "right",
      sortable: true,
      valueFormatter: (params) => {
        const year = params.data?.profitYear;
        const iter = params.data?.profitYearIteration;
        if (year === undefined || year === null) return "";
        return `${year}.${iter ?? 0}`;
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
        const id = params.data?.profitCodeId;
        const name = params.data?.profitCodeName;
        if (id === undefined || id === null) return "";
        return `[${id}] ${name ?? ""}`;
      }
    },
    createCurrencyColumn({
      headerName: "Contribution",
      field: "contribution"
    }),
    createCurrencyColumn({
      headerName: "Earnings",
      field: "earnings"
    }),
    createCurrencyColumn({
      headerName: "Forfeiture",
      field: "forfeiture",
      maxWidth: 100
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
      sortable: true,
      resizable: true,
      valueFormatter: (params) => {
        const month = params.data?.monthToDate;
        const year = params.data?.yearToDate;
        if (month === undefined || month === null) return "";
        const formattedMonth = month.toString().padStart(2, "0");
        if (month === 0 && year === 0) {
          return "";
        }
        return `${formattedMonth}/${year ?? ""}`;
      }
    },
    createHoursColumn({
      field: "currentHoursYear",
      maxWidth: 80,
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
    createTaxCodeColumn({}),
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
    createStatusColumn({
      field: "employmentStatus"
    })
  ];
};
