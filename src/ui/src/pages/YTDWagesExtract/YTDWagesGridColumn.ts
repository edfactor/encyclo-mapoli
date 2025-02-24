import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeRenderer } from "utils/masterInquiryLink";
import { currencyFormat } from "utils/numberUtils";

export const GetYTDWagesColumns = (): ColDef[] => {
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
      cellRenderer: (params: ICellRendererParams) => viewBadgeRenderer(params.data.badgeNumber)
    },
    {
      headerName: "Hours Current Year",
      field: "hoursCurrentYear",
      colId: "hoursCurrentYear",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Dollars Current Year",
      field: "incomeCurrentYear",
      colId: "incomeCurrentYear",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params) => currencyFormat(params.value)
    }
  ];
  return columns;
};
