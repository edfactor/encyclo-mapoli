import { ColDef, ICellRendererParams } from "ag-grid-community";
import {
  createAgeColumn,
  createBadgeColumn,
  createCityColumn,
  createNameColumn,
  createPSNColumn,
  createStateColumn,
  createZipColumn
} from "../../utils/gridColumnFactory";

/*

badgeNumber: number;
  psnSuffix: number;
  name?: string | null;
  ssn?: string | null;
  street?: string | null;
  city?: string | null;
  state?: string | null;
  zip?: string | null;
  age?: number | null;

  */

export const GetBeneficiariesListGridColumns = (percentageFieldRenderer: (arg0: any, arg1: any) => any): ColDef[] => {
  return [
    createBadgeColumn({}),
    createPSNColumn({
      headerName: "Psn",
      field: "psnSuffix",

      enableLinking: true,
      linkingStyle: "simple"
    }),
    createNameColumn({
      field: "fullName",

      valueFormatter: (params) => {
        return `${params.data.lastName}, ${params.data.firstName}`;
      }
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
    createAgeColumn({}),
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
