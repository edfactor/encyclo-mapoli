import { ColDef } from "ag-grid-community";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import { createBadgeColumn, createCurrencyColumn } from "../../../utils/gridColumnFactory";

// The default is to show all columns, but if the mini flag is set to true, only show the
// badge, name, and ssn columns
export const GetMilitaryContributionColumns = (): ColDef[] => {
  const columns: ColDef[] = [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left"
    }),
    {
      headerName: "Contribution Year",
      field: "contributionDate",
      colId: "contributionDate",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => {
        return new Date(params.value).getFullYear();
      }
    },
    createCurrencyColumn({
      headerName: "Amount",
      field: "amount",
      minWidth: 150
    }),
    {
      headerName: "Supplemental Contribution",
      field: "isSupplementalContribution",
      colId: "isSupplementalContribution",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
  return columns;
};
