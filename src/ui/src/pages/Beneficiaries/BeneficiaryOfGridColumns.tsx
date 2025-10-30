import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createNameColumn,
  createSSNColumn,
  createStateColumn
} from "../../utils/gridColumnFactory";

export const GetBeneficiaryOfGridColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    {
      headerName: "PSN_SUFFIX",
      field: "psnSuffix",
      colId: "psnSuffix",
      flex: 1,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
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
      headerName: "Percent",
      field: "percent",
      colId: "percent",
      flex: 1,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true
    },
    createCurrencyColumn({ headerName: "Current", field: "currentBalance", colId: "currentBalance" })
  ];
};
