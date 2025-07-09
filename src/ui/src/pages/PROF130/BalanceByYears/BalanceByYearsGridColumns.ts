import { agGridNumberToCurrency } from "smart-ui-library";
import { FrozenReportsByAgeRequestType } from "../../../reduxstore/types";
import { ColDef, ColGroupDef } from "ag-grid-community";

export const GetBalanceByYearsGridColumns = (reportType: FrozenReportsByAgeRequestType): ColDef[] => {
  const columns: (ColDef | ColGroupDef)[] = [
    {
      headerName: reportType,
      children: [
        {
          headerName: "Years",
          field: "years",
          colId: "years",
          minWidth: 80,
          type: "rightAligned",
          resizable: true,
          sort: "asc",
          cellDataType: "text"
        },
        {
          headerName: "Count",
          field: "employeeCount",
          colId: "employeeCount",
          minWidth: 100,
          type: "rightAligned",
          resizable: true
        },
        {
          headerName: "Balance",
          field: "currentBalance",
          colId: "currentBalance",
          minWidth: 150,
          type: "rightAligned",
          resizable: true,
          valueFormatter: agGridNumberToCurrency
        },
        {
          headerName: "Vested",
          field: "vestedBalance",
          colId: "vestedBalance",
          minWidth: 150,
          type: "rightAligned",
          resizable: true,
          valueFormatter: agGridNumberToCurrency
        }
      ]
    }
  ];
  return columns;
};
