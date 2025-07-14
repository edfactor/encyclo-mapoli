import { agGridNumberToCurrency } from "smart-ui-library";
import { ColDef, ColGroupDef, ICellRendererParams } from "ag-grid-community";
import { viewBadgeLinkRenderer } from "utils/masterInquiryLink";

export const GetPay450GridColumns = (navFunction: (badgeNumber: string) => void): (ColDef | ColGroupDef)[] => {
  return [
    {
      headerName: "Badge",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      cellRenderer: (params: ICellRendererParams) => viewBadgeLinkRenderer(params.data.badgeNumber, navFunction)
    },
    {
      headerName: "Name",
      field: "employeeName",
      colId: "employeeName",
      minWidth: 180,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
    {
      headerName: "Store",
      field: "storeNumber",
      colId: "storeNumber",
      minWidth: 80,
      type: "rightAligned",
      resizable: true
    },
    {
      headerName: "Before",
      children: [
        {
          headerName: "P/S Amount",
          field: "psAmountOriginal",
          colId: "psAmountOriginal",
          minWidth: 120,
          type: "rightAligned",
          resizable: true,
          valueFormatter: agGridNumberToCurrency
        },
        {
          headerName: "P/S Vested",
          field: "psVestedOriginal",
          colId: "psVestedOriginal",
          minWidth: 120,
          type: "rightAligned",
          resizable: true,
          valueFormatter: agGridNumberToCurrency
        },
        {
          headerName: "Years",
          field: "yearsOriginal",
          colId: "yearsOriginal",
          minWidth: 80,
          type: "rightAligned",
          resizable: true
        },
        {
          headerName: "Enroll",
          field: "enrollOriginal",
          colId: "enrollOriginal",
          minWidth: 80,
          type: "rightAligned",
          resizable: true
        }
      ]
    },
    {
      headerName: "After",
      children: [
        {
          headerName: "P/S Amount",
          field: "psAmountUpdated",
          colId: "psAmountUpdated",
          minWidth: 120,
          type: "rightAligned",
          resizable: true,
          valueFormatter: agGridNumberToCurrency
        },
        {
          headerName: "P/S Vested",
          field: "psVestedUpdated",
          colId: "psVestedUpdated",
          minWidth: 120,
          type: "rightAligned",
          resizable: true,
          valueFormatter: agGridNumberToCurrency
        },
        {
          headerName: "Years",
          field: "yearsUpdated",
          colId: "yearsUpdated",
          minWidth: 80,
          type: "rightAligned",
          resizable: true
        },
        {
          headerName: "Enroll",
          field: "enrollUpdated",
          colId: "enrollUpdated",
          minWidth: 80,
          type: "rightAligned",
          resizable: true
        }
      ]
    }
  ];
};
