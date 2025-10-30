import { ColDef, ICellRendererParams } from "ag-grid-community";
import { numberToCurrency } from "smart-ui-library";
import { SuggestedForfeitCellRenderer, SuggestedForfeitEditor } from "../../../components/SuggestedForfeiture";
import { createSaveButtonCellRenderer } from "../../../components/ForfeitActivities";
import { ForfeitureAdjustmentUpdateRequest } from "../../../types";
import {
  createAgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createYearColumn,
  createYesOrNoColumn
} from "../../../utils/gridColumnFactory";
import { HeaderComponent } from "./TerminationHeaderComponent";

// Separate function for detail columns that will be used for master-detail view
export const GetDetailColumns = (
  addRowToSelectedRows: (id: number) => void,
  removeRowFromSelectedRows: (id: number) => void,
  selectedProfitYear: number,
  onSave?: (request: ForfeitureAdjustmentUpdateRequest, name: string) => Promise<void>,
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => Promise<void>,
  isReadOnly = true
): ColDef[] => {
  return [
    createYearColumn({
      headerName: "Profit Year",
      field: "profitYear",
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Beginning Balance",
      field: "beginningBalance",
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Beneficiary Allocation",
      field: "beneficiaryAllocation",
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Distribution Amount",
      field: "distributionAmount",
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Forfeit Amount",
      field: "forfeit",
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Ending Balance",
      field: "endingBalance",
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Vested Balance",
      field: "vestedBalance",
      sortable: false
    }),
    {
      headerName: "Vested %",
      field: "vestedPercent",
      colId: "vestedPercent",
      width: 100,
      type: "rightAligned",
      resizable: true,
      sortable: false,
      valueFormatter: (params) => `${params.value}%`
    },
    createDateColumn({
      headerName: "Term Date",
      field: "dateTerm",
      sortable: false
    }),
    createHoursColumn({
      headerName: "YTD PS Hours",
      field: "ytdPsHours",
      sortable: false
    }),
    createAgeColumn({
      maxWidth: 70,
      sortable: false
    }),
    createYesOrNoColumn({
      headerName: "Forfeited",
      field: "hasForfeited",
      colId: "hasForfeited",
      sortable: false
    }),
    {
      headerName: "Suggested Forfeit",
      field: "suggestedForfeit",
      colId: "suggestedForfeit",
      minWidth: 150,
      pinned: "right",
      type: "rightAligned",
      resizable: true,
      sortable: false,
      cellClass: (params) => {
        if (!params.data.isDetail) return "";
        const rowKey = String(params.data.psn);
        const hasError = params.context?.editedValues?.[rowKey]?.hasError;
        return hasError ? "bg-red-50" : "";
      },
      editable: ({ node }) => node.data.isDetail,
      flex: 1,
      cellEditor: SuggestedForfeitEditor,
      cellRenderer: (params: ICellRendererParams) =>
        SuggestedForfeitCellRenderer(
          {
            ...params,
            selectedProfitYear
          },
          true,
          false
        ),
      valueFormatter: (params) => numberToCurrency(params.value),
      valueGetter: (params) => {
        if (!params.data.isDetail) return params.data.suggestedForfeit;
        const rowKey = String(params.data.psn);
        const editedValue = params.context?.editedValues?.[rowKey]?.value;
        return editedValue ?? params.data.suggestedForfeit ?? 0;
      }
    },
    {
      headerName: "Save Button",
      field: "saveButton",
      colId: "saveButton",
      minWidth: 100,
      pinned: "right",
      lockPinned: true,
      resizable: false,
      sortable: false,
      cellStyle: { backgroundColor: "#E8E8E8" },
      headerComponent: HeaderComponent,
      valueGetter: (params) => {
        if (!params.data.isDetail) return "";
        const rowKey = String(params.data.psn);
        const editedValue = params.context?.editedValues?.[rowKey]?.value;
        const currentValue = editedValue ?? params.data.suggestedForfeit ?? 0;
        return `${currentValue}-${params.context?.loadingRowIds?.has(params.data.psn)}-${params.node?.isSelected()}`;
      },
      headerComponentParams: {
        addRowToSelectedRows,
        removeRowFromSelectedRows,
        onBulkSave,
        isReadOnly
      },
      cellRendererParams: {
        addRowToSelectedRows,
        removeRowFromSelectedRows,
        onSave
      },
      cellRenderer: createSaveButtonCellRenderer({
        activityType: "termination",
        selectedProfitYear,
        isReadOnly
      })
    }
  ];
};
