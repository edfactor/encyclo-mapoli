import { agGridNumberToCurrency } from "smart-ui-library";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import { mmDDYYFormat, mmDDYYYY_HHMMSS_Format } from "utils/dateUtils";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";
import { GRID_COLUMN_WIDTHS } from "../../constants";
import { createSSNColumn } from "../../utils/ssnColumnFactory";

export const BeneficiaryInquiryGridColumns = (): ColDef[] => {
  return [
    // {
    //   headerName: "Badge Number",
    //   field: "badgeNumber",
    //   colId: "badgeNumber",
    //   minWidth: 130,
    //   headerClass: "center-align",
    //   cellClass: "center-align",
    //   resizable: true
    // },
    // {
    //   headerName: "Psn Suffix",
    //   field: "psnSuffix",
    //   colId: "psnSuffix",
    //   minWidth: 100,
    //   headerClass: "left-align",
    //   cellClass: "left-align",
    //   resizable: true
    // },
    {
      headerName: "Psn",
      field: "psnSuffix",
      colId: "psnSuffix",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: true,
      unSortIcon: true,
      cellRenderer: (params: ICellRendererParams) =>
        viewBadgeLinkRenderer(params.data.badgeNumber, params.data.psnSuffix)
    },
    {
      headerName: "Current Balance",
      field: "currentBalance",
      colId: "currentBalance",
      minWidth: 170,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: false
    },
    createSSNColumn({
      valueFormatter: (params) => {
        return `${params.data.ssn}`;
      }
    }),
    {
      headerName: "Date of birth",
      field: "dateOfBirth",
      colId: "dateOfBirth",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${mmDDYYFormat(params.data.dateOfBirth)}`;
      }
    },
    {
      headerName: "Street",
      field: "street",
      colId: "street",
      minWidth: 150,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.street}`;
      }
    },
    {
      headerName: "City",
      field: "city",
      colId: "city",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.city}`;
      }
    },
    {
      headerName: "State",
      field: "state",
      colId: "state",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.state}`;
      }
    },
    {
      headerName: "Postal Code",
      field: "postalCode",
      colId: "postalCode",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      sortable: false,
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.postalCode}`;
      }
    },
    {
      headerName: "Country Iso",
      field: "countryIso",
      colId: "countryIso",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: false,
      valueFormatter: (params) => {
        return `${params.data.countryIso}`;
      }
    },
    {
      headerName: "Name",
      field: "fullName",
      colId: "fullName",
      minWidth: GRID_COLUMN_WIDTHS.FULL_NAME,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: false,
      valueFormatter: (params) => {
        return `${params.data.lastName}, ${params.data.firstName}`;
      }
    },
    {
      headerName: "Phone Number",
      field: "phoneNumber",
      colId: "phoneNumber",
      minWidth: 130,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.phoneNumber ?? ""}`;
      }
    },
    {
      headerName: "Mobile Number",
      field: "mobileNumber",
      colId: "mobileNumber",
      minWidth: 130,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.mobileNumber ?? ""}`;
      }
    },
    {
      headerName: "Email Address",
      field: "emailAddress",
      colId: "emailAddress",
      minWidth: 130,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.emailAddress ?? ""}`;
      }
    },
    {
      headerName: "Created Date",
      field: "createdDate",
      colId: "createdDate",
      minWidth: 170,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => (params.data.createdDate ? mmDDYYFormat(params.data.createdDate) : "")
    }
  ];
};
