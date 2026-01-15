import { ColDef, ICellRendererParams } from "ag-grid-community";
import { BeneficiaryDto } from "../../types";
import {
  createCityColumn,
  createDateColumn,
  createNameColumn,
  createSSNColumn,
  createStateColumn,
  createZipColumn
} from "../../utils/gridColumnFactory";
import { BadgeCellRenderer } from "./BadgeCellRenderer";
import { BeneficiaryActionHandlers, BeneficiaryActionsCellRenderer } from "./BeneficiaryActions";

export const GetBeneficiariesListGridColumns = (
  percentageFieldRenderer: (percentage: number, id: number) => React.JSX.Element,
  actionHandlers?: BeneficiaryActionHandlers,
  onBadgeClick?: (beneficiary: BeneficiaryDto) => void
): ColDef[] => {
  const columns: ColDef[] = [
    {
      headerName: "Badge/Psn",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 120,
      maxWidth: 125,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      cellRenderer: BadgeCellRenderer,
      cellRendererParams: {
        onBadgeClick: onBadgeClick
      }
    },
    createNameColumn({
      field: "fullName"
    }),
    createSSNColumn({}),
    {
      headerName: "Percentage",
      field: "percentage",
      colId: "percentage",
      maxWidth: 150,
      headerClass: "center-align",
      cellClass: "center-align",
      sortable: false,
      resizable: false,
      cellRenderer: (params: ICellRendererParams) => percentageFieldRenderer(params.data.percent, params.data.id)
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

  // Add Action column if handlers are provided
  if (actionHandlers) {
    columns.push({
      headerName: "Action",
      field: "actions",
      sortable: false,
      resizable: false,
      headerClass: "center-align",
      cellClass: "center-align",
      cellRenderer: BeneficiaryActionsCellRenderer,
      cellRendererParams: {
        handlers: actionHandlers
      },
      minWidth: 200,
      maxWidth: 250
    });
  }

  return columns;
};
