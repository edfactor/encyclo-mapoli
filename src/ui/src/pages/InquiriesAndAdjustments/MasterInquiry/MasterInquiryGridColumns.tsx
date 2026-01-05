import Link from "@mui/material/Link";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import {
  createCurrencyColumn,
  createHoursColumn,
  createStateColumn,
  createTaxCodeColumn,
  createYearColumn,
  createYesOrNoColumn
} from "../../../utils/gridColumnFactory";

/**
 * Cell renderer for Comment Type that creates links for QDRO transfers
 */
const CommentTypeLinkRenderer = (params: ICellRendererParams) => {
  const { data, value } = params;
  if (!data) return value;

  const commentTypeId = data.commentTypeId;
  const commentTypeName = data.commentTypeName || value;
  const xFerQdroId = data.xFerQdroId;

  // Check if this is a QDRO In (3) or QDRO Out (4) entry
  if ((commentTypeId === 3 || commentTypeId === 4) && xFerQdroId) {
    const badgeStr = xFerQdroId.toString();

    // Always link to master inquiry with the badge number
    const route = `/master-inquiry/${badgeStr}`;

    // Return a Link component
    return (
      <Link
        className="solid h-5 normal-case underline"
        href={route}>
        {commentTypeName}
      </Link>
    );
  }

  // For non-QDRO entries, just display the text
  return commentTypeName;
};

export const GetMasterInquiryGridColumns = (): ColDef[] => {
  const partialTransactionColumn = createYesOrNoColumn({
    headerName: "Partial Transaction",
    field: "commentIsPartialTransaction",
    minWidth: 120,
    useWords: true
  });

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
      headerClass: "left-align",
      cellClass: "left-align",
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
      field: "contribution"
    }),
    createCurrencyColumn({
      headerName: "Earnings",
      field: "earnings"
    }),
    createCurrencyColumn({
      headerName: "Forfeiture",
      field: "forfeiture"
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
    createTaxCodeColumn({}),
    {
      headerName: "Comment Type",
      headerTooltip: "Comment Type",
      field: "commentTypeName",
      colId: "commentTypeName",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      cellRenderer: CommentTypeLinkRenderer,
      tooltipValueGetter: (params) => {
        // Show QDRO info for comment types 3 (QDRO In) and 4 (QDRO Out)
        if (params.data?.commentTypeId === 3 || params.data?.commentTypeId === 4) {
          const qdroId = params.data?.xFerQdroId;
          const qdroName = params.data?.xFerQdroName;
          if (qdroId || qdroName) {
            return `Name: ${qdroName || "N/A"}\nBadge:${qdroId || "N/A"}`;
          }
        }
        return "";
      }
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
    { ...partialTransactionColumn, flex: 1 }
  ];
};
