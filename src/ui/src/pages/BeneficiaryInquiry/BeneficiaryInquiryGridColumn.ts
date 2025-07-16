import { agGridNumberToCurrency } from "smart-ui-library";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import { mmDDYYFormat, mmDDYYYY_HHMMSS_Format } from "utils/dateUtils";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";
import { GRID_COLUMN_WIDTHS } from "../../constants";

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
      field: "psn",
      colId: "psn",
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
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: GRID_COLUMN_WIDTHS.SSN,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.contact.ssn}`;
      }
    },
    {
      headerName: "Date of birth",
      field: "dateOfBirth",
      colId: "dateOfBirth",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${mmDDYYFormat(params.data.contact.dateOfBirth)}`;
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
        return `${params.data.contact.address.street}`;
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
        return `${params.data.contact.address.city}`;
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
        return `${params.data.contact.address.state}`;
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
        return `${params.data.contact.address.postalCode}`;
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
        return `${params.data.contact.address.countryIso}`;
      }
    },
    {
      headerName: "Full Name",
      field: "fullName",
      colId: "fullName",
      minWidth: GRID_COLUMN_WIDTHS.FULL_NAME,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: false,
      valueFormatter: (params) => {
        return `${params.data.contact.contactInfo.lastName}, ${params.data.contact.contactInfo.firstName}`;
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
        return `${params.data.contact.contactInfo.phoneNumber ?? ""}`;
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
        return `${params.data.contact.contactInfo.mobileNumber ?? ""}`;
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
        return `${params.data.contact.contactInfo.emailAddress ?? ""}`;
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
      valueFormatter: (params) => (params.data.contact.createdDate ? mmDDYYFormat(params.data.contact.createdDate) : "")
    }
  ];
};
