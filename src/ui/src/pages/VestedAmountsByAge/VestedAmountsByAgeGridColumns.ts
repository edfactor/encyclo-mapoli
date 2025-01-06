import { agGridNumberToCurrency } from "smart-ui-library";
import { ColDef, ColGroupDef } from "ag-grid-community";

export type VestedAmountsByAgeColumn = ColDef | ColGroupDef;

export const GetVestedAmountsByAgeColumns = (): VestedAmountsByAgeColumn[] => {
  const createChildColumn = (
    headerName: string,
    field: string,
    isCurrency: boolean = false
  ): ColDef => ({
    headerName,
    field,
    minWidth: 150,
    headerClass: "left-align",
    cellClass: "left-align",
    resizable: true,
    ...(isCurrency && { valueFormatter: agGridNumberToCurrency }),
  });


  return [
    {
      headerName: "Age",
      field: "age",
      minWidth: 80,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sort: "asc",
      cellDataType: "text",
    },
    {
      headerName: "Full-Time 100% Vested",
      children: [
        createChildColumn("Count", "fullTime100PercentCount"),
        createChildColumn("Amount", "fullTime100PercentAmount", true),
      ],
    },
    {
      headerName: "Full-Time Partially Vested",
      children: [
        createChildColumn("Count", "fullTimePartialCount"),
        createChildColumn("Amount", "fullTimePartialAmount", true),
      ],
    },
    {
      headerName: "Full-Time Not Vested",
      children: [
        createChildColumn("Count", "fullTimeNotVestedCount"),
        createChildColumn("Amount", "fullTimeNotVestedAmount", true),
      ],
    },
    {
      headerName: "Part-Time 100% Vested",
      children: [
        createChildColumn("Count", "partTime100PercentCount"),
        createChildColumn("Amount", "partTime100PercentAmount", true),
      ],
    },
    {
      headerName: "Part-Time Partially Vested",
      children: [
        createChildColumn("Count", "partTimePartialCount"),
        createChildColumn("Amount", "partTimePartialAmount", true),
      ],
    },
    {
      headerName: "Part-Time Not Vested",
      children: [
        createChildColumn("Count", "partTimeNotVestedCount"),
        createChildColumn("Amount", "partTimeNotVestedAmount", true),
      ],
    },
    {
      headerName: "Beneficiaries",
      children: [
        createChildColumn("Count", "beneficiaryCount"),
        createChildColumn("Amount", "beneficiaryAmount", true),
      ],
    },
    {
      headerName: "Summary",
      children: [
        createChildColumn("Full-Time Total", "fullTimeCount"),
        createChildColumn("Not Vested Total", "notVestedCount"),
        createChildColumn("Partially Vested Total", "partialVestedCount"),
      ],
    },
  ];
}
