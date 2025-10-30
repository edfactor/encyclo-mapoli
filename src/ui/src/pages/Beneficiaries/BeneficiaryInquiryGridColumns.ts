import { ColDef, ICellRendererParams } from "ag-grid-community";
import {
  createCityColumn,
  createCurrencyColumn,
  createDateColumn,
  createNameColumn,
  createPSNColumn,
  createSSNColumn,
  createStateColumn,
  createZipColumn
} from "../../utils/gridColumnFactory";

export const BeneficiaryInquiryGridColumns = (percentageFieldRenderer: (arg0: any, arg1: any) => any): ColDef[] => {
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
    {
      headerName: "Percentage",
      field: "percentage",
      colId: "percentage",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      sortable: false,
      resizable: true,
      cellRenderer: (params: ICellRendererParams) => percentageFieldRenderer(params.data.percent, params.data.id)
    }
  ];
};
