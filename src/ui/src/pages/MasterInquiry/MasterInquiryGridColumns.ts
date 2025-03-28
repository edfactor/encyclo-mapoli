import { agGridNumberToCurrency } from "smart-ui-library";
import { ColDef } from "ag-grid-community";

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
      headerName: "Badge Number",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,    
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
      minWidth: 120,
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
      headerName: "Distribution Sequence",
      headerTooltip: "Distribution Sequence",
      field: "distributionSequence",
      colId: "distributionSequence",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Profit Code",
      field: "profitCodeName",
      colId: "profitCodeName",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: (params) => {
        const id = params.data.profitCodeId; // assuming 'status' is in the row data
        const name = params.data.profitCodeName; // assuming 'statusName' is in the row data
        //see if one is undefined or null then show other
        return id && name? `${name} (${id})`: ``;
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
      headerName: "Month to Date",
      headerTooltip: "Month to Date",
      field: "monthToDate",
      colId: "monthToDate",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Year To Date",
      headerTooltip: "Year To Date",
      field: "yearToDate",
      colId: "yearToDate",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Zero Contribution Reason",
      headerTooltip: "Zero Contribution Reason",
      field: "zeroContributionReasonName",
      colId: "zeroContributionReasonName",
      minWidth: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: (params) => {
        const id = params.data?.zeroContributionReasonId; // assuming 'status' is in the row data
        const name = params.data?.zeroContributionReasonName; // assuming 'statusName' is in the row data
        // If both are null/undefined, just return an empty string
        if (!id && !name) {
          return '';
        }
        // If one of them is missing, show only the one that exists
        if (!id) {
          return `${name}`;
        }
        if (!name) {
          return `${id}`;
        }

        // If both exist, format as "name (id)"
        return `${name} (${id})`;
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
      valueFormatter: (params) => {
        const id = params.data?.taxCodeId; // assuming 'status' is in the row data
        const name = params.data?.taxCodeName; // assuming 'statusName' is in the row data
        // If both are null/undefined, just return an empty string
        if (!id && !name) {
          return '';
        }
        // If one of them is missing, show only the one that exists
        if (!id) {
          return `${name}`;
        }
        if (!name) {
          return `${id}`;
        }

        // If both exist, format as "name (id)"
        return `${name} (${id})`;
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
      resizable: true
    }
  ];
};