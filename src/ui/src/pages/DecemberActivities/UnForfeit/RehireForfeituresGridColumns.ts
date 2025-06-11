import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency, formatNumberWithComma } from "smart-ui-library";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";
import { mmDDYYFormat } from "utils/dateUtils";

export const GetMilitaryAndRehireForfeituresColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      width: 100,
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
      width: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      flex: 1
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      width: 125,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Hire Date",
      field: "hireDate",
      colId: "hireDate",
      width: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
            valueFormatter: (params) => {
              const date = params.value;
              return mmDDYYFormat(date);
            }
    },
    {
      headerName: "Termination Date",
      field: "terminationDate",
      colId: "terminationDate",
      width: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => {
        const date = params.value;
        return mmDDYYFormat(date);
      }
    },
    {
      headerName: "Rehired Date",
      field: "reHiredDate",
      colId: "reHiredDate",
      width: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => {
        const date = params.value;
        return mmDDYYFormat(date);
      }
    },
    
    {
      headerName: "Beginning Balance",
      field: "netBalanceLastYear",
      colId: "netBalanceLastYear",
      width: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Vested Balance",
      field: "vestedBalanceLastYear",
      colId: "vestedBalanceLastYear",
      width: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: agGridNumberToCurrency
    },    
    {
      headerName: "Store",
      field: "storeNumber",
      colId: "storeNumber",
      width: 60,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
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
      width: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Hours Current Year",
      field: "hoursCurrentYear",
      colId: "hoursCurrentYear",
      width: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => {
        const hours = params.value;
        return formatNumberWithComma(hours);
      }
    },    
    {
      headerName: "Enrollment",
      width: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      valueGetter: (params) => {
        const id = params.data?.enrollmentId; // assuming 'status' is in the row data
        const name = params.data?.enrollmentName; // assuming 'statusName' is in the row data        
        return `[${id}] ${name}`;
      }
    },
    {
      headerName: "Forfeiture",
      field: "forfeiture",
      colId: "forfeiture",
      width: 150,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Remark",
      field: "remark",
      colId: "remark",
      width: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: false,
    },
    {
      headerName: "Contribution Years",
      field: "companyContributionYears",
      colId: "companyContributionYears",
      width: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
  ];
};