import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "../../../utils/masterInquiryLink";
import { agGridNumberToCurrency } from "smart-ui-library";
import { dateMMDDYYYY } from "../../../utils/dateUtils";

// The default is to show all columns, but if the mini flag is set to true, only show the
// badge, name, and ssn columns
export const GetMilitaryContributionColumns = (): ColDef[] => {
  const columns: ColDef[] = [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 80,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber)
    },
    {
      headerName: "Contribution Date",
      field: "contributionDate",
      colId: "contributionDate",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => {
        return dateMMDDYYYY(params.data.ContributionDate);
      }
    },
    {
      headerName: "Amount",
      field: "amount",
      colId: "amount",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Supplemental Contribution",
      field: "isSupplementalContribution",
      colId: "isSupplementalContribution",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }]
  return columns;
};
