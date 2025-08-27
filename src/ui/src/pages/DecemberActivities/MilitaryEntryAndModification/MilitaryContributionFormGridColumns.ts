import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createYearColumn,
  createYesOrNoColumn
} from "../../../utils/gridColumnFactory";

// The default is to show all columns, but if the mini flag is set to true, only show the
// badge, name, and ssn columns
export const GetMilitaryContributionColumns = (): ColDef[] => {
  const columns: ColDef[] = [
    createBadgeColumn({}),
    createYearColumn({
      headerName: "Contribution Year",
      field: "contributionDate"
    }),
    createCurrencyColumn({
      headerName: "Amount",
      field: "amount"
    }),
    createYesOrNoColumn({
      headerName: "Supplemental Contribution",
      field: "isSupplementalContribution"
    })
  ];
  return columns;
};
