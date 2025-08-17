import { SaveOutlined } from "@mui/icons-material";
import { Checkbox, CircularProgress, IconButton } from "@mui/material";
import { ColDef, EditableCallbackParams, ICellRendererParams } from "ag-grid-community";
import { SuggestedForfeitCellRenderer, SuggestedForfeitEditor } from "components/SuggestedForfeiture";
import { numberToCurrency } from "smart-ui-library";
import { ForfeitureAdjustmentUpdateRequest, ForfeitureDetail, RehireForfeituresSaveButtonCellParams } from "types";
import {
  createCommentColumn,
  createCountColumn,
  createCurrencyColumn,
  createHoursColumn,
  createYearColumn
} from "utils/gridColumnFactory";
import { HeaderComponent } from "./RehireForfeituresHeaderComponent";

export const GetProfitDetailColumns = (
  addRowToSelectedRows: (id: number) => void,
  removeRowFromSelectedRows: (id: number) => void,
  selectedProfitYear: number,
  onSave?: (request: ForfeitureAdjustmentUpdateRequest) => Promise<void>,
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[]) => Promise<void>
): ColDef[] => {
  return [
    createYearColumn({
      headerName: "Profit Year",
      field: "profitYear"
    }),
    createHoursColumn({
      field: "hoursCurrentYear",
      valueGetter: (params) => {
        // I think showing 0 is visually noisy, so I'm suppressing that.   Especially since we will not have the
        // actual hours for our initial year - 1.
        const value = params.data?.hoursCurrentYear;
        return value == null || value == 0 ? null : value;
      }
    }),
    createCurrencyColumn({
      headerName: "Wages",
      field: "wages"
    }),
    createCountColumn({
      headerName: "Years",
      field: "companyContributionYears"
    }),
    {
      headerName: "Enrollment",
      width: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      valueGetter: (params) => {
        const id = params.data?.enrollmentId;
        const name = params.data?.enrollmentName;
        return `[${id}] ${name}`;
      }
    },
    createCurrencyColumn({
      headerName: "Forfeiture",
      field: "forfeiture"
    }),
    {
      headerName: "Suggested Unforfeiture",
      field: "suggestedForfeit",
      colId: "suggestedForfeit",
      width: 150,
      type: "rightAligned",
      pinned: "right",
      resizable: true,
      sortable: false,
      editable: (params: EditableCallbackParams) => {
        // Figure out if there is a forfeit row to unforfeit. If not, we cannot unforfeit,
        // so the row is not editable
        const hasThisYearForfeitInDetail = params.data.details?.some(
          (detail: ForfeitureDetail) => detail.profitYear === selectedProfitYear && detail.remark === "FORFEIT"
        );

        return params.data.isDetail && params.data.profitYear === selectedProfitYear && hasThisYearForfeitInDetail;
      },
      cellEditor: SuggestedForfeitEditor,
      cellRenderer: (params: ICellRendererParams) => {
        return SuggestedForfeitCellRenderer({ ...params, selectedProfitYear }, false, true);
      },
      valueFormatter: (params) => numberToCurrency(params.value),
      valueGetter: (params) => {
        // So if there are no profit details, do not do anything
        if (!params.data.isDetail) return null;

        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}${params.data.enrollmentId ? `-${params.data.enrollmentId}` : ""}-${params.node?.id || "unknown"}`;

        const editedValue = params.context?.editedValues?.[rowKey]?.value;
        return editedValue !== undefined ? editedValue : params.data.suggestedForfeit;
      }
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
        // We must have a current year forfeit in detail or else we don't need a save button
        const hasThisYearForfeitInDetail = params.data.details?.some(
          (detail: ForfeitureDetail) => detail.profitYear === selectedProfitYear && detail.remark === "FORFEIT"
        );

        if (!params.data.isDetail || params.data.profitYear !== selectedProfitYear || !hasThisYearForfeitInDetail) {
          return "";
        }

        const id = Number(params.node?.id) || -1;
        const isSelected = params.node?.isSelected() || false;
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}${params.data.enrollmentId ? `-${params.data.enrollmentId}` : ""}-${params.node?.id || "unknown"}`;
        const hasError = params.context?.editedValues?.[rowKey]?.hasError;
        const currentValue = params.context?.editedValues?.[rowKey]?.value ?? params.data.suggestedForfeit;
        const isLoading = params.context?.loadingRowIds?.has(params.data.badgeNumber);

        // We need a variable, offsettingProfitDetailId, that is set to the detail record with the remark "FORFEIT"
        const offsettingProfitDetailId = params.data.details?.find(
          (detail: ForfeitureDetail) => detail.profitYear === selectedProfitYear && detail.remark === "FORFEIT"
        )?.profitDetailId;

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
                    profitYear: params.data.profitYear,
                    forfeitureAmount: -(currentValue || 0),
                    offsettingProfitDetailId: offsettingProfitDetailId,
                    classAction: false
                  };
                  await params.onSave(request);
                }
              }}
              disabled={params.data.remark !== "FORFEIT" || hasError || (currentValue || 0) === 0 || isLoading}>
              {isLoading ? <CircularProgress size={20} /> : <SaveOutlined />}
            </IconButton>
          </div>
        );
      }
    }
  ];
};
