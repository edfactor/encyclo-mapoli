import { ColDef, ColGroupDef } from "ag-grid-community";
import { createBadgeColumn, createCurrencyColumn, createStoreColumn } from "../../utils/gridColumnFactory";

export const GetPay450GridColumns = (navFunction: (badgeNumber: string) => void): (ColDef | ColGroupDef)[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: 100,
      alignment: "left",
      navigateFunction: navFunction
    }),
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
    createStoreColumn({
      minWidth: 80
    }),
    {
      headerName: "Before",
      children: [
        createCurrencyColumn({
          headerName: "P/S Amount",
          field: "psAmountOriginal",
          minWidth: 120
        }),
        createCurrencyColumn({
          headerName: "P/S Vested",
          field: "psVestedOriginal",
          minWidth: 120
        }),
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
        createCurrencyColumn({
          headerName: "P/S Amount",
          field: "psAmountUpdated",
          minWidth: 120
        }),
        createCurrencyColumn({
          headerName: "P/S Vested",
          field: "psVestedUpdated",
          minWidth: 120
        }),
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
