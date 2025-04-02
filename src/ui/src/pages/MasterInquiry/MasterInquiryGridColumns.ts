import { agGridNumberToCurrency } from "smart-ui-library";
import {ColDef, ICellRendererParams} from "ag-grid-community";
import {viewBadgeLinkRenderer} from "../../utils/masterInquiryLink";

export const GetMasterInquiryGridColumns = (): ColDef[] => {
  return [
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Badge/Psn",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,   
      rowGroup: true,
      showRowGroup: 'always',
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber),
      valueFormatter: (params) => {
        const badgeNumber = params.data?.badgeNumber; 
        const psnSuffix = params.data?.psnSuffix; 
        // If both are null/undefined, just return an empty string
        if (!badgeNumber && !psnSuffix) {
          return '';
        }
        
        if (psnSuffix > 0) {
          // If both exist, format as "name (id)"
          return `${badgeNumber}-${psnSuffix}`;
        }
        
        return badgeNumber
      }
    },
    {
      headerName: "Profit Year",
      field: "profitYear",
      colId: "profitYear",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      unSortIcon: true,
      valueFormatter: (params) => {
        const year = params.data.profitYear; // assuming 'status' is in the row data
        const iter = params.data.profitYearIteration; // assuming 'statusName' is in the row data
        return `${year}.${iter}`;
      }
    },  
    {
      headerName: "Profit Code",
      field: "profitCodeId",
      colId: "profitCodeId",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
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
    {
      headerName: "Contribution",
      field: "contribution",
      colId: "contribution",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Earnings",
      field: "earnings",
      colId: "earnings",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Forfeiture",
      field: "forfeiture",
      colId: "forfeiture",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Payment",
      field: "payment",
      colId: "payment",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Month/Year",
      headerTooltip: "Month to Date",
      field: "monthToDate",
      colId: "monthToDate",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: (params) => {
        const month = params.data.monthToDate; // assuming 'status' is in the row data
        const year = params.data.yearToDate; // assuming 'statusName' is in the row data
        // Format month to always have two digits
        const formattedMonth = month.toString().padStart(2, '0');

        return `${formattedMonth}/${year}`;
      }
    },      
    {
      headerName: "Federal Tax",
      field: "federalTaxes",
      colId: "federalTaxes",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "State Tax",
      field: "stateTaxes",
      colId: "stateTaxes",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
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
        return `[${id}] ${name}`;
      }
    },
    {
      headerName: "Comment Type",
      headerTooltip: "Comment Type",
      field: "commentTypeName",
      colId: "commentTypeName",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
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
      resizable: true
    },
    {
      headerName: "State",
      field: "commentRelatedState",
      colId: "commentRelatedState",
      minWidth: 60,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Partial Transaction",
      field: "commentIsPartialTransaction",
      colId: "commentIsPartialTransaction",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      cellRenderer: 'agAnimateShowChangeCellRenderer', // Use text renderer instead of checkbox
      valueFormatter: (params) => {
        // Return "Yes" only if the value is explicitly true
        // Return "No" for false, null, undefined, or any other falsy value
        return params.value === true ? "Yes" : "No";
      }
    }
  ];
};