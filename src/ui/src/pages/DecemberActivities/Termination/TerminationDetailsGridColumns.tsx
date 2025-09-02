import { SaveOutlined } from "@mui/icons-material";
import { Checkbox, CircularProgress, IconButton } from "@mui/material";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import { SuggestedForfeitCellRenderer, SuggestedForfeitEditor } from "components/SuggestedForfeiture";
import { numberToCurrency } from "smart-ui-library";
import { ForfeitureAdjustmentUpdateRequest } from "types";
import { getForfeitedStatus } from "utils/enrollmentUtil";
import {
  createAgeColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createStatusColumn,
  createYearColumn
} from "utils/gridColumnFactory";
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
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[]) => Promise<void>
): ColDef[] => {
  return [
    createYearColumn({
      headerName: "Profit Year",
      field: "profitYear"
    }),
    createCurrencyColumn({
      headerName: "Beginning Balance",
      field: "beginningBalance"
    }),
    createCurrencyColumn({
      headerName: "Beneficiary Allocation",
      field: "beneficiaryAllocation"
    }),
    createCurrencyColumn({
      headerName: "Distribution Amount",
      field: "distributionAmount"
    }),
    createCurrencyColumn({
      headerName: "Forfeit Amount",
      field: "forfeit"
    }),
    createCurrencyColumn({
      headerName: "Ending Balance",
      field: "endingBalance"
    }),
    createCurrencyColumn({
      headerName: "Vested Balance",
      field: "vestedBalance"
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
      field: "dateTerm"
    }),
    createHoursColumn({
      headerName: "YTD PS Hours",
      field: "ytdPsHours"
    }),
    createAgeColumn({
      maxWidth: 70,
      sortable: false
    }),
    createStatusColumn({
      headerName: "Forfeit",
      field: "enrollmentCode",
      // Yes, the enrollmentCode is tied to the Forfeited status. See PS-1279
      valueFormatter: (params) => getForfeitedStatus(params.value)
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
        return hasError ? "invalid-cell" : "";
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

        return (
          <div>
            <Checkbox
              checked={isSelected}
              onChange={() => {
                if (isSelected) {
                  params.removeRowFromSelectedRows(id);
                } else {
                  params.addRowToSelectedRows(id);
                }
                params.node?.setSelected(!isSelected);
              }}
            />
            <IconButton
              onClick={async () => {
                if (params.data.isDetail && params.onSave) {
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
              disabled={hasError || isLoading}>
              {isLoading ? <CircularProgress size={20} /> : <SaveOutlined />}
            </IconButton>
          </div>
        );
      }
    }
  ];
};
