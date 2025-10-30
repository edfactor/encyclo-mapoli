import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createNameColumn,
  createPSNColumn,
  createSSNColumn,
  createStateColumn,
  createZipColumn
} from "../../utils/gridColumnFactory";

export const GetBeneficiaryOfGridColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createPSNColumn({
      headerName: "Psn Suffix",
      field: "psnSuffix",
      enableLinking: false
    }),
    createNameColumn({
      field: "fullName",
      valueFormatter: (params) => {
        return `${params.data.lastName}, ${params.data.firstName}`;
      }
    }),
    createSSNColumn({}),
    createDateColumn({ headerName: "Date of Birth", field: "dateOfBirth" }),
    {
      headerName: "Address",
      field: "street",
      colId: "street",
      flex: 1,
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
      flex: 1,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.city}`;
      }
    },
    createStateColumn({}),
    {
      headerName: "Percentage",
      field: "percent",
      colId: "percent",
      flex: 1,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    createZipColumn({ field: "postalCode" }),
    createCurrencyColumn({ headerName: "Current Balance", field: "currentBalance", colId: "currentBalance" })
  ];
};
