import { agGridNumberToCurrency } from "smart-ui-library";
import { ColDef } from "ag-grid-community";

export const GetMasterInquiryGridColumns = (): ColDef[] => {
  return [
    {
      headerName: "ID",
      field: "id",
      colId: "id",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true
    },
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
      headerName: "Profit Year",
      field: "profitYear",
      colId: "profitYear",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Iteration",
      field: "profitYearIteration",
      colId: "profitYearIteration", 
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Distribution",
      field: "distributionSequence",
      colId: "distributionSequence",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Code",
      field: "profitCodeId",
      colId: "profitCodeId",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
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
      headerName: "Month",
      field: "monthToDate",
      colId: "monthToDate",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Year",
      field: "yearToDate",
      colId: "yearToDate",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Remark",
      field: "remark",
      colId: "remark",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Zero Contribution Reason",
      field: "zeroContributionReasonId",
      colId: "zeroContributionReasonId",
      minWidth: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
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
      resizable: true
    },
    {
      headerName: "Comment Type",
      field: "commentTypeId",
      colId: "commentTypeId",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Check Number",
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
      minWidth: 80,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Oracle HCM ID",
      field: "commentRelatedOracleHcmId",
      colId: "commentRelatedOracleHcmId",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "PSN Suffix",
      field: "commentRelatedPsnSuffix",
      colId: "commentRelatedPsnSuffix",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
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