import { ColDef } from "ag-grid-community";
import {
  createCityColumn,
  createCurrencyColumn,
  createDateColumn,
  createNameColumn,
  createPhoneColumn,
  createPSNColumn,
  createSSNColumn,
  createStateColumn,
  createZipColumn
} from "../../utils/gridColumnFactory";

export const BeneficiaryInquiryGridColumns = (): ColDef[] => {
  return [
    createPSNColumn({
      headerName: "Psn",
      field: "psnSuffix",

      enableLinking: true,
      linkingStyle: "simple"
    }),
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
    createPhoneColumn({
      headerName: "Phone Number",
      field: "phoneNumber"
    }),
    createPhoneColumn({
      headerName: "Mobile Number",
      field: "mobileNumber"
    }),
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
