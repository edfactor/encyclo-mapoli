import { ColDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createYearColumn,
  createYesOrNoColumn
} from "../../../utils/gridColumnFactory";

export const GetMilitaryContributionColumns = (): ColDef[] => {
  const columns: ColDef[] = [
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
