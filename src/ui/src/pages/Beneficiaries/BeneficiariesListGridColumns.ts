import { ColDef, ICellRendererParams } from "ag-grid-community";
import {
  createBadgeColumn,
  createCityColumn,
  createDateColumn,
  createNameColumn,
  createSSNColumn,
  createStateColumn,
  createZipColumn
} from "../../utils/gridColumnFactory";

export const GetBeneficiariesListGridColumns = (
  percentageFieldRenderer: (percentage: number, id: number) => React.JSX.Element
): ColDef[] => {
  return [
    createBadgeColumn({ headerName: 'Badge/Psn', psnSuffix: true }),
    createNameColumn({
      field: "fullName",

      valueFormatter: (params) => {
        return `${params.data.lastName}, ${params.data.firstName}`;
      }
    }),
    createSSNColumn({}),
    {
      headerName: "Percentage",
      field: "percentage",
      colId: "percentage",
      maxWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      sortable: false,
      resizable: true,
      cellRenderer: (params: ICellRendererParams) => percentageFieldRenderer(params.data.percent, params.data.id)
    },
    {
      headerName: "Kind",
      field: "kindId",
      colId: "kindId",
      maxWidth: 80,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.kindId}`;
      }
    },
    {
      headerName: "Current Balance",
      field: "currentBalance",
      colId: "currentBalance",
      minWidth: 100,
      maxWidth: 150,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `$${params.data.currentBalance?.toFixed(2) ?? "0.00"}`;
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
    createCityColumn({
      valueFormatter: (params) => {
        return `${params.data.city}`;
      }
    }),
    createStateColumn({}),
    createZipColumn({
      field: "postalCode"
    }),
    createDateColumn({
      headerName: "Date of Birth",
      field: "dateOfBirth",
      colId: "dateOfBirth"
    }),
    {
      headerName: "Relationship",
      field: "relationship",
      colId: "relationship",
      minWidth: 150,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.relationship}`;
      }
    },
    {
      headerName: "Phone",
      field: "phoneNumber",
      colId: "phoneNumber",
      minWidth: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      valueFormatter: (params) => {
        return `${params.data.phoneNumber || ""}`;
      }
    }
  ];
};
