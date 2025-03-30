import { ColDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";
import { agGridNumberToCurrency } from "smart-ui-library";

export const GetYTDWagesColumns = (): ColDef[] => {
  const columns: ColDef[] = [
    {
      headerName: "Badge Number",
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
      valueFormatter: agGridNumberToCurrency
    },
    {
      headerName: "Store",
      field: "storeNumber",
      colId: "storeNumber",
      minWidth: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    }
  ];
  return columns;
};
