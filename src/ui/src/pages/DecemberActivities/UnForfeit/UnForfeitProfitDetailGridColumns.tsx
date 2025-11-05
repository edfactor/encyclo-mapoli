import { ColDef, EditableCallbackParams, ICellRendererParams } from "ag-grid-community";
import { SuggestedForfeitCellRenderer, SuggestedForfeitEditor } from "components/SuggestedForfeiture";
import { numberToCurrency } from "smart-ui-library";
import { createSaveButtonCellRenderer } from "../../../components/ForfeitActivities";
import { ForfeitureAdjustmentUpdateRequest } from "@/types/december-activities/forfeitures";
import {
  createCommentColumn,
  createCurrencyColumn,
  createHoursColumn,
  createYearColumn
} from "utils/gridColumnFactory";
import { HeaderComponent } from "./UnForfeitHeaderComponent";

function isTransactionEditable(params: EditableCallbackParams, isReadOnly: boolean = false): boolean {
  return params.data.suggestedUnforfeiture != null && !isReadOnly;
}

export const GetProfitDetailColumns = (
  addRowToSelectedRows: (id: number) => void,
  removeRowFromSelectedRows: (id: number) => void,
  selectedProfitYear: number,
  onSave?: (request: ForfeitureAdjustmentUpdateRequest, name: string) => Promise<void>,
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => Promise<void>,
  isReadOnly: boolean = false
): ColDef[] => {
  return [
    createYearColumn({
      headerName: "Profit Year",
      field: "profitYear",
      sortable: false
    }),
    createHoursColumn({
      field: "hoursTransactionYear",
      valueGetter: (params) => {
        // Do not show zeros, for many years we only have zero (aka no data)
        const value = params.data?.hoursTransactionYear;
        return value == null || value == 0 ? null : value;
      },
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Wages",
      field: "wagesTransactionYear",
      valueGetter: (params) => {
        // Do not show zeros, for many years we only have zeros (aka no data)
        const value = params.data?.wagesTransactionYear;
        return value == null || value == 0 ? null : value;
      },
      sortable: false
    }),
    createCurrencyColumn({
      headerName: "Forfeiture",
      field: "forfeiture",
      sortable: false
    }),
    {
      headerName: "Suggested Unforfeiture",
      field: "suggestedUnforfeit",
      colId: "suggestedUnforfeit",
      width: 150,
      type: "rightAligned",
      pinned: "right",
      resizable: true,
      sortable: false,
      editable: (params: EditableCallbackParams) => isTransactionEditable(params, isReadOnly),
      cellEditor: SuggestedForfeitEditor,
      cellRenderer: (params: ICellRendererParams) => {
        return SuggestedForfeitCellRenderer({ ...params, selectedProfitYear }, false, true);
      },
      valueFormatter: (params) => numberToCurrency(params.data.suggestedUnforfeiture)
    },

    createCommentColumn({
      headerName: "Remark",
      field: "remark",
      sortable: false
    }),
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
        activityType: "unforfeit",
        selectedProfitYear,
        isReadOnly
      })
    }
  ];
};
