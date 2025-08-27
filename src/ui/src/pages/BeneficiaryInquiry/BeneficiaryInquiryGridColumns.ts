import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";
import { GRID_COLUMN_WIDTHS } from "../../constants";
import {
  createCurrencyColumn,
  createDateColumn,
  createNameColumn,
  createSSNColumn,
  createZipColumn
} from "../../utils/gridColumnFactory";

export const BeneficiaryInquiryGridColumns = (): ColDef[] => {
  return [
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
    createCurrencyColumn({
      headerName: "Current Balance",
      field: "currentBalance"
    }),
    createSSNColumn({}),
    createDateColumn({
      headerName: "Date of birth",
      field: "dateOfBirth"
    }),
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
    createZipColumn({
      field: "postalCode",
      headerName: "Postal Code",
      minWidth: 120
    }),
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
    createNameColumn({
      field: "fullName",
      minWidth: GRID_COLUMN_WIDTHS.FULL_NAME,
      alignment: "center",
      sortable: false,
      valueFormatter: (params) => {
        return `${params.data.lastName}, ${params.data.firstName}`;
      }
    }),
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
    createDateColumn({
      headerName: "Created Date",
      field: "createdDate",
      minWidth: 170
    })
  ];
};
