import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeRenderer } from "../../utils/masterInquiryLink";

export const GetMilitaryAndRehireProfitSummaryColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      cellRenderer: (params: ICellRendererParams) => viewBadgeRenderer(params.data.badgeNumber)
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Net Balance",
      field: "netBalanceLastYear",
      colId: "netBalanceLastYear",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
};
