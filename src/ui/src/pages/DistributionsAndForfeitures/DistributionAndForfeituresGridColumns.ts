import { ColDef } from "ag-grid-community";
import { yyyyMMDDToMMDDYYYY } from "smart-ui-library";
import { viewBadgeRenderer } from "../../utils/masterInquiryLink";

export const GetDistributionsAndForfeituresColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sort: "asc",
      cellRenderer: (params) => viewBadgeRenderer({ value: params.data.badgeNumber }),
    },
    {
      headerName: "Name",
      field: "employeeName",
      colId: "employeeName",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "SSN",
      field: "employeeSsn",
      colId: "employeeSsn",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Loan Date",
      field: "loanDate",
      colId: "loanDate",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => (params.value ? yyyyMMDDToMMDDYYYY(params.value) : "")
    },
    {
      headerName: "Distribution",
      field: "distributionAmount",
      colId: "distributionAmount",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "State Tax",
      field: "stateTax",
      colId: "stateTax",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Federal Tax",
      field: "federalTax",
      colId: "federalTax",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Forfeit Amount",
      field: "forfeitAmount",
      colId: "forfeitAmount",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Age",
      field: "age",
      colId: "age",
      minWidth: 70,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    },
    {
      headerName: "Tax Code",
      field: "taxCode",
      colId: "taxCode",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Other Name",
      field: "otherName",
      colId: "otherName",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Other SSN",
      field: "otherSsn",
      colId: "otherSsn",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Enrolled",
      field: "enrolled",
      colId: "enrolled",
      minWidth: 90,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    }
  ];
};
