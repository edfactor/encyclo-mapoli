import { ColDef } from "ag-grid-community";
import { agGridNumberToCurrency } from "smart-ui-library";

export const GetMasterInquiryGridColumns = (): ColDef[] => {
  return [
    {
      headerName: "ID",
      field: "id",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "SSN",
      field: "ssn",
      minWidth: 120,
      headerClass: "right-align", 
      cellClass: "right-align",
      valueFormatter: params => params.value ? `***-**-${params.value.toString().slice(-4)}` : "",
      resizable: true
    },
    {
      headerName: "Profit Year",
      field: "profitYear",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Iteration",
      field: "profitYearIteration",
      minWidth: 90,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Distribution Seq",
      field: "distributionSequence",
      minWidth: 130,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Profit Code",
      field: "profitCodeId",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Contribution",
      field: "contribution",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      valueFormatter: agGridNumberToCurrency,
      resizable: true
    },
    {
      headerName: "Earnings",
      field: "earnings",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      valueFormatter: agGridNumberToCurrency,
      resizable: true
    },
    {
      headerName: "Forfeiture",
      field: "forfeiture",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      valueFormatter: agGridNumberToCurrency,
      resizable: true
    },
    {
      headerName: "Federal Tax",
      field: "federalTaxes",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      valueFormatter: agGridNumberToCurrency,
      resizable: true
    },
    {
      headerName: "State Tax",
      field: "stateTaxes",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      valueFormatter: agGridNumberToCurrency,
      resizable: true
    },
    {
      headerName: "Tax Code",
      field: "taxCodeId",
      minWidth: 90,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    {
      headerName: "Remarks",
      field: "remark",
      minWidth: 200,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
};