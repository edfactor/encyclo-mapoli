import Link from "@mui/material/Link";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import { formatNumberWithComma, numberToCurrency } from "smart-ui-library";
import { ROUTES } from "../../../../constants";
import { createCurrencyColumn, createHoursColumn, createPointsColumn } from "../../../../utils/gridColumnFactory";

/**
 * Maps lineItemPrefix to PAY426N preset number
 */
const getPresetNumberForLineItem = (lineItemPrefix: string): string | null => {
  const presetMap: Record<string, string> = {
    "1": "1",
    "2": "2",
    "3": "3",
    "4": "4",
    "5": "5",
    "6": "6",
    "7": "7",
    "8": "8",
    "9": "9",
    N: "10"
  };
  return presetMap[lineItemPrefix] || null;
};

/**
 * Cell renderer for line item that renders an actual link for right-click support
 */
const LineItemLinkRenderer = (params: ICellRendererParams) => {
  const { data, value } = params;
  if (!data) return value;

  const displayText = data.lineItemPrefix ? `${data.lineItemPrefix}. ${value}` : value;
  const presetNumber = getPresetNumberForLineItem(data.lineItemPrefix);

  if (!presetNumber) {
    return displayText;
  }

  const href = `/${ROUTES.PAY426N_LIVE}/${presetNumber}`;

  return (
    <Link
      href={href}
      className="text-blue-600 underline decoration-blue-600"
      sx={{ cursor: "pointer" }}>
      {displayText}
    </Link>
  );
};

export const GetProfitSummaryGridColumns = (): ColDef[] => {
  return [
    {
      headerName: "Line Item",
      field: "lineItemTitle",
      colId: "lineItemTitle",
      minWidth: 400,
      sortable: false,
      headerClass: "left-align",
      cellClass: "left-align h-5 normal-case !outline-none !border-none focus:outline-none focus:border-none",
      resizable: true,
      cellRenderer: LineItemLinkRenderer
    },
    createPointsColumn({
      headerName: "EMPS",
      field: "numberOfMembers",
      sortable: false,
      valueFormatter: (params) => formatNumberWithComma(params.value)
    }),
    createCurrencyColumn({
      headerName: "Total Wages",
      field: "totalWages",
      minWidth: 180,
      sortable: false,
      valueFormatter: (params) => {
        if (typeof params.value === "string" && params.value.includes("X")) {
          return params.value;
        }
        return numberToCurrency(params.value);
      }
    }),
    createCurrencyColumn({
      headerName: "Total Balance",
      field: "totalBalance",
      minWidth: 180,
      sortable: false,
      valueFormatter: (params) => {
        if (typeof params.value === "string" && params.value.includes("X")) {
          return params.value;
        }
        return numberToCurrency(params.value);
      }
    }),
    createHoursColumn({
      headerName: "Hours",
      field: "totalHours",
      minWidth: 80,
      sortable: false,
      valueFormatter: (params) => {
        if (params.value === null || params.value === undefined) {
          return "";
        }
        if (typeof params.value === "string" && params.value.includes("X")) {
          return params.value;
        }
        return formatNumberWithComma(params.value);
      }
    }),
    createPointsColumn({
      headerName: "Points",
      field: "totalPoints",
      minWidth: 100,
      sortable: false,
      valueFormatter: (params) => {
        if (params.value === null || params.value === undefined) {
          return "";
        }
        if (typeof params.value === "string" && params.value.includes("X")) {
          return params.value;
        }
        return formatNumberWithComma(params.value);
      }
    })
  ];
};
