import { SaveOutlined } from "@mui/icons-material";
import { Checkbox, CircularProgress, IconButton } from "@mui/material";
import { ColDef, EditableCallbackParams, ICellRendererParams } from "ag-grid-community";
import { SuggestedForfeitEditor, SuggestedForfeitCellRenderer } from "components/SuggestedForfeiture";
import { numberToCurrency } from "smart-ui-library";
import { ForfeitureAdjustmentUpdateRequest, ForfeitureDetail, RehireForfeituresSaveButtonCellParams } from "types";
import {
  createYearColumn,
  createCurrencyColumn,
  createCommentColumn,
  createHoursColumn
} from "utils/gridColumnFactory";
import { HeaderComponent } from "./RehireForfeituresHeaderComponent";
import useFiscalCloseProfitYear from "../../../hooks/useFiscalCloseProfitYear";

function isTransactionEditiabe(params) {
  return params.data.isDetail && params.data.suggestedUnforfeiture != null;
}

export const GetProfitDetailColumns = (
  addRowToSelectedRows: (id: number) => void,
  removeRowFromSelectedRows: (id: number) => void,
  selectedProfitYear: number,
  onSave?: (request: ForfeitureAdjustmentUpdateRequest, name: string) => Promise<void>,
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => Promise<void>
): ColDef[] => {
  return [
    createYearColumn({
      headerName: "Profit Year",
      field: "profitYear"
    }),
    createHoursColumn({
      field: "hoursTransactionYear",
      valueGetter: (params) => {
        // Do not show zeros, for many years we only have zero (aka no data)
        const value = params.data?.hoursTransactionYear;
        return value == null || value == 0 ? null : value;
      }
    }),
    createCurrencyColumn({
      headerName: "Wages",
      field: "wagesTransactionYear",
      valueGetter: (params) => {
        // Do not show zeros, for many years we only have zeros (aka no data)
        const value = params.data?.wagesTransactionYear;
        return value == null || value == 0 ? null : value;
      }
    }),
    createCurrencyColumn({
      headerName: "Forfeiture",
      field: "forfeiture"
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
      editable: (params: EditableCallbackParams) => isTransactionEditiabe(params),
      cellEditor: SuggestedForfeitEditor,
      cellRenderer: (params: ICellRendererParams) => {
        return SuggestedForfeitCellRenderer({ ...params, selectedProfitYear }, false, true);
      },
      valueFormatter: (params) => numberToCurrency(params.data.suggestedUnforfeiture)
    },

    createCommentColumn({
      headerName: "Remark",
      field: "remark"
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
        onBulkSave
      },
      cellRendererParams: {
        addRowToSelectedRows,
        removeRowFromSelectedRows,
        onSave
      },
      cellRenderer: (params: RehireForfeituresSaveButtonCellParams) => {
        if (!isTransactionEditiabe(params)) {
          return "";
        }

        const id = Number(params.node?.id) || -1;
        const isSelected = params.node?.isSelected() || false;
        const rowKey = params.data.profitDetailId;
        const currentValue = params.context?.editedValues?.[rowKey]?.value ?? params.data.suggestedUnforfeiture;
        const isLoading = params.context?.loadingRowIds?.has(params.data.badgeNumber);
        const profitYear = useFiscalCloseProfitYear();

        return (
          <div>
            <Checkbox
              checked={isSelected}
              disabled={(currentValue || 0) === 0}
              onChange={() => {
                if (isSelected) {
                  params.removeRowFromSelectedRows(id);
                  params.node?.setSelected(false);
                } else {
                  params.addRowToSelectedRows(id);
                  params.node?.setSelected(true);
                }
                params.api.refreshCells({ force: true });
              }}
            />
            <IconButton
              onClick={async () => {
                if (params.data.isDetail && params.onSave) {
                  const request: ForfeitureAdjustmentUpdateRequest = {
                    badgeNumber: params.data.badgeNumber,
                    forfeitureAmount: -(currentValue || 0),
                    profitYear: profitYear,
                    offsettingProfitDetailId: params.data.profitDetailId,
                    classAction: false
                  };
                  const employeeName = params.data.fullName || params.data.name || "Unknown Employee";
                  await params.onSave(request, employeeName);
                }
              }}
              disabled={(currentValue || 0) === 0 || isLoading}>
              {isLoading ? <CircularProgress size={20} /> : <SaveOutlined />}
            </IconButton>
          </div>
        );
      }
    }
  ];
};
