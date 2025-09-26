import { SaveOutlined } from "@mui/icons-material";
import { Checkbox, CircularProgress, IconButton, Tooltip } from "@mui/material";
import { ColDef, EditableCallbackParams, ICellRendererParams } from "ag-grid-community";
import { SuggestedForfeitCellRenderer, SuggestedForfeitEditor } from "components/SuggestedForfeiture";
import { numberToCurrency } from "smart-ui-library";
import { ForfeitureAdjustmentUpdateRequest, UnForfeitsSaveButtonCellParams } from "types";
import {
  createCommentColumn,
  createCurrencyColumn,
  createHoursColumn,
  createYearColumn
} from "utils/gridColumnFactory";
import { HeaderComponent } from "./UnForfeitHeaderComponent";

function isTransactionEditable(params, isReadOnly: boolean = false): boolean {
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
      editable: (params: EditableCallbackParams) => isTransactionEditable(params, isReadOnly),
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
        onBulkSave,
        isReadOnly
      },
      cellRendererParams: {
        addRowToSelectedRows,
        removeRowFromSelectedRows,
        onSave
      },
      cellRenderer: (params: UnForfeitsSaveButtonCellParams) => {
        if (!isTransactionEditable(params, isReadOnly)) {
          return "";
        }

        const id = Number(params.node?.id) || -1;
        const isSelected = params.node?.isSelected() || false;
        const rowKey = params.data.profitDetailId;
        const currentValue = params.context?.editedValues?.[rowKey]?.value ?? params.data.suggestedUnforfeiture;
        const isLoading = params.context?.loadingRowIds?.has(params.data.badgeNumber);
        const isDisabled = (currentValue || 0) === 0 || isLoading || isReadOnly;
        const readOnlyTooltip = "You are in read-only mode and cannot save changes.";

        const checkboxElement = (
          <Checkbox
            checked={isSelected}
            disabled={isDisabled}
            onChange={() => {
              if (!isReadOnly) {
                if (isSelected) {
                  params.removeRowFromSelectedRows(id);
                  params.node?.setSelected(false);
                } else {
                  params.addRowToSelectedRows(id);
                  params.node?.setSelected(true);
                }
                params.api.refreshCells({ force: true });
              }
            }}
          />
        );

        const saveButtonElement = (
          <IconButton
            onClick={async () => {
              if (!isReadOnly && params.data.isDetail && params.onSave) {
                const request: ForfeitureAdjustmentUpdateRequest = {
                  badgeNumber: params.data.badgeNumber,
                  forfeitureAmount: -(currentValue || 0),
                  profitYear: selectedProfitYear,
                  offsettingProfitDetailId: params.data.profitDetailId,
                  classAction: false
                };
                const employeeName = params.data.fullName || params.data.name || "Unknown Employee";
                await params.onSave(request, employeeName);
              }
            }}
            disabled={isDisabled}>
            {isLoading ? <CircularProgress size={20} /> : <SaveOutlined />}
          </IconButton>
        );

        return (
          <div>
            {isReadOnly ? (
              <Tooltip title={readOnlyTooltip}>
                <span>{checkboxElement}</span>
              </Tooltip>
            ) : (
              checkboxElement
            )}
            {isReadOnly ? (
              <Tooltip title={readOnlyTooltip}>
                <span>{saveButtonElement}</span>
              </Tooltip>
            ) : (
              saveButtonElement
            )}
          </div>
        );
      }
    }
  ];
};
