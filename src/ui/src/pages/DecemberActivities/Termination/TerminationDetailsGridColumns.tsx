import { SaveOutlined } from "@mui/icons-material";
import { Checkbox, CircularProgress, IconButton, Tooltip } from "@mui/material";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import { numberToCurrency } from "smart-ui-library";
import { SuggestedForfeitCellRenderer, SuggestedForfeitEditor } from "../../../components/SuggestedForfeiture";
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

interface SaveButtonCellParams extends ICellRendererParams {
  removeRowFromSelectedRows: (id: number) => void;
  addRowToSelectedRows: (id: number) => void;
  onSave?: (request: ForfeitureAdjustmentUpdateRequest, name: string) => Promise<void>;
}

// Separate function for detail columns that will be used for master-detail view
export const GetDetailColumns = (
  addRowToSelectedRows: (id: number) => void,
  removeRowFromSelectedRows: (id: number) => void,
  selectedRowIds: number[],
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
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
        const hasError = params.context?.editedValues?.[rowKey]?.hasError;
        return hasError ? "bg-red-50" : "";
      },
      editable: ({ node }) => node.data.isDetail && node.data.profitYear === selectedProfitYear,
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
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
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
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
        const editedValue = params.context?.editedValues?.[rowKey]?.value;
        const currentValue = editedValue ?? params.data.suggestedForfeit ?? 0;
        return `${currentValue}-${params.context?.loadingRowIds?.has(params.data.badgeNumber)}-${params.node?.isSelected()}`;
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
      cellRenderer: (params: SaveButtonCellParams) => {
        if (!params.data.isDetail || params.data.profitYear !== selectedProfitYear) {
          return "";
        }
        const id = Number(params.node?.id) || -1;
        const isSelected = params.node?.isSelected() || false;
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}`;
        const hasError = params.context?.editedValues?.[rowKey]?.hasError;
        const currentValue = params.context?.editedValues?.[rowKey]?.value ?? params.data.suggestedForfeit;
        const isLoading = params.context?.loadingRowIds?.has(params.data.badgeNumber);
        const isZeroValue = currentValue === 0 || currentValue === null || currentValue === undefined;
        const isDisabled = hasError || isLoading || isZeroValue || isReadOnly;
        const readOnlyTooltip = "You are in read-only mode and cannot save changes.";

        const checkboxElement = (
          <Tooltip
            title={isZeroValue ? "Forfeit cannot be zero." : isReadOnly ? readOnlyTooltip : ""}
            arrow>
            <span>
              <Checkbox
                checked={isSelected}
                disabled={isDisabled}
                onChange={() => {
                  if (!isReadOnly) {
                    if (isSelected) {
                      params.removeRowFromSelectedRows(id);
                    } else {
                      params.addRowToSelectedRows(id);
                    }
                    params.node?.setSelected(!isSelected);
                  }
                }}
              />
            </span>
          </Tooltip>
        );

        const saveButtonElement = (
          <IconButton
            onClick={async () => {
              if (!isReadOnly && params.data.isDetail && params.onSave) {
                const request: ForfeitureAdjustmentUpdateRequest = {
                  badgeNumber: params.data.badgeNumber,
                  profitYear: params.data.profitYear,
                  forfeitureAmount: currentValue || 0,
                  classAction: false,
                  offsettingProfitDetailId: undefined
                };

                const employeeName = params.data.fullName || params.data.name || "the selected employee";
                await params.onSave(request, employeeName);
              }
            }}
            disabled={isDisabled}>
            {isLoading ? <CircularProgress size={20} /> : <SaveOutlined />}
          </IconButton>
        );

        return (
          <div>
            {checkboxElement}
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
