import { ColDef, ICellRendererParams, ValueFormatterParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";
import { agGridNumberToCurrency } from "smart-ui-library";

export const GetMilitaryAndRehireForfeituresColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 50,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      unSortIcon: true,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber)
    },
    {
      headerName: "Full Name",
      field: "fullName",
      colId: "fullName",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: 90,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Rehired Date",
      field: "reHiredDate",
      colId: "reHiredDate",
      minWidth: 90,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Contribution Years",
      field: "companyContributionYears",
      colId: "companyContributionYears",
      minWidth: 90,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Hours Current Year",
      field: "hoursCurrentYear",
      colId: "hoursCurrentYear",
      minWidth: 90,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Enrollment",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      valueGetter: (params) => {
        const id = params.data?.enrollmentId; // assuming 'status' is in the row data
        const name = params.data?.enrollmentName; // assuming 'statusName' is in the row data        
        return `[${id}] ${name}`;
      }
    }
  ];
};

// Separate function for detail columns that will be used for master-detail view
export const GetDetailColumns = (): ColDef[] => {
  return [
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
      headerName: "Forfeiture",
      field: "forfeiture",
      colId: "forfeiture",
      minWidth: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Remark",
      field: "remark",
      colId: "remark",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
};