import { ColDef, ICellRendererParams } from "ag-grid-community";
import React from "react";
import { ValidationIcon } from "../../../components/ValidationIcon";
import {
  createBadgeOrPSNColumn,
  createColumnFrom,
  createCountColumn,
  createCurrencyColumn,
  createNameColumn,
  createSSNColumn
} from "../../../utils/gridColumnFactory";

export interface ForfeitColumnsOptions {
  navFunction: (badgeNumber: string) => void;
  onValidationClick?: (fieldName: string) => void;
}

export const GetProfitShareForfeitColumns = (options: ForfeitColumnsOptions): ColDef[] => {
  const { navFunction, onValidationClick } = options;
  return [
    createBadgeOrPSNColumn({
      headerName: "Badge/PSN",
      field: "badgeOrPsn",
      badgeField: "badgeNumber",
      psnField: "beneficiaryPsn",
      navigateFunction: navFunction,
      enableLinking: true
    }),
    createNameColumn({
      field: "employeeName"
    }),
    createSSNColumn({}),
    createColumnFrom(
      createCurrencyColumn({
        headerName: "Forfeitures",
        field: "forfeitures"
      }),
      {
        cellRenderer: (params: ICellRendererParams) => {
          return React.createElement('div', {}, 
            React.createElement(ValidationIcon, {
              validationGroup: params.data?.validation || null,
              fieldName: "ForfeitureTotal",
              onClick: onValidationClick ? () => onValidationClick("ForfeitureTotal") : undefined
            }),
            params.valueFormatted,
          );
        },
      }
    ),

    createCountColumn({
      headerName: "Cont/Forfeit Points",
      field: "contForfeitPoints"
    }),
    createCountColumn({
      headerName: "Earnings Points",
      field: "earningPoints"
    })
  ];
};
