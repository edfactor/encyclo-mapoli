import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";
import {
  createCityColumn,
  createCurrencyColumn,
  createDateColumn,
  createNameColumn,
  createSSNColumn,
  createStateColumn,
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
    createCityColumn({
      valueFormatter: (params) => {
        return `${params.data.city}`;
      }
    }),
    createStateColumn({}),
    createZipColumn({
      field: "postalCode"
    }),
    createZipColumn({
      field: "postalCode",
      headerName: "Postal Code"
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
