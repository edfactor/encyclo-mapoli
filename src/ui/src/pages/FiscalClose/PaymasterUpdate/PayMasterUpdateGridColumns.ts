import { ColDef, ColGroupDef } from "ag-grid-community";
import {
  createBadgeColumn,
  createCurrencyColumn,
  createNameColumn,
  createStoreColumn
} from "../../../utils/gridColumnFactory";

export const GetPayMasterUpdateGridColumns = (navFunction: (badgeNumber: string) => void): (ColDef | ColGroupDef)[] => {
  return [
    createBadgeColumn({
      navigateFunction: navFunction
    }),
    createNameColumn({}),
    createStoreColumn({
      minWidth: 80
    }),
    {
      headerName: "Before",
      children: [
        createCurrencyColumn({
          headerName: "P/S Amount",
          field: "psAmountOriginal"
        }),
        createCurrencyColumn({
          headerName: "P/S Vested",
          field: "psVestedOriginal"
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
          field: "psAmountUpdated"
        }),
        createCurrencyColumn({
          headerName: "P/S Vested",
          field: "psVestedUpdated"
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
