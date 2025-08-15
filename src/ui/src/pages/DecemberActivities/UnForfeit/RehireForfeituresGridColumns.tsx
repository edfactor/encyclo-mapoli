import { SaveOutlined } from "@mui/icons-material";
import { Checkbox, IconButton, CircularProgress } from "@mui/material";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import { agGridNumberToCurrency, formatNumberWithComma } from "smart-ui-library";
import { mmDDYYFormat } from "utils/dateUtils";
import { SelectableGridHeader } from "../../../components/SelectableGridHeader";
import { SuggestedForfeitCellRenderer, SuggestedForfeitEditor } from "../../../components/SuggestedForfeiture";
import { GRID_COLUMN_WIDTHS } from "../../../constants";
import {
  ForfeitureAdjustmentUpdateRequest,
  RehireForfeituresHeaderComponentProps,
  RehireForfeituresSaveButtonCellParams
} from "../../../reduxstore/types";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { createSSNColumn, createBadgeColumn, createStoreColumn, createCurrencyColumn } from "../../../utils/gridColumnFactory";

export const HeaderComponent: React.FC<RehireForfeituresHeaderComponentProps> = (params: RehireForfeituresHeaderComponentProps) => {
  const selectedProfitYear = useDecemberFlowProfitYear();

  const isNodeEligible = (nodeData: any, context: any) => {
    if (!nodeData.isDetail || nodeData.profitYear !== selectedProfitYear) return false;
    // For bulk operations, we need to check all possible rowKeys for this data
    const baseRowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}${nodeData.enrollmentId ? `-${nodeData.enrollmentId}` : ''}`;
    const editedValues = context?.editedValues || {};
    const matchingKey = Object.keys(editedValues).find(key => key.startsWith(baseRowKey));
    const currentValue = matchingKey ? editedValues[matchingKey]?.value : nodeData.suggestedForfeit;
    return (currentValue || 0) !== 0;
  };

  const createUpdatePayload = (nodeData: any, context: any): ForfeitureAdjustmentUpdateRequest => {
    // For bulk operations, we need to find the actual edited value
    const baseRowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}${nodeData.enrollmentId ? `-${nodeData.enrollmentId}` : ''}`;
    const editedValues = context?.editedValues || {};
    const matchingKey = Object.keys(editedValues).find(key => key.startsWith(baseRowKey));
    const currentValue = matchingKey ? editedValues[matchingKey]?.value : nodeData.suggestedForfeit;
    return {
      badgeNumber: nodeData.badgeNumber,
      profitYear: nodeData.profitYear,
      forfeitureAmount: -(currentValue || 0),
      classAction: false
    };
  };

  // Check if any rows are in loading state
  const hasSavingInProgress = params.context?.loadingRowIds?.size > 0;

  return (
    <SelectableGridHeader
      {...params}
      isNodeEligible={isNodeEligible}
      createUpdatePayload={createUpdatePayload}
      isBulkSaving={hasSavingInProgress}
    />
  );
};

export const GetMilitaryAndRehireForfeituresColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: GRID_COLUMN_WIDTHS.BADGE_NUMBER,
      alignment: "left"
    }),
    {
      headerName: "Name",
      field: "fullName",
      colId: "fullName",
      minWidth: GRID_COLUMN_WIDTHS.FULL_NAME,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      flex: 1,
      pinned: "left",
    },
    createSSNColumn({ alignment: "left" }),
    {
      headerName: "Rehired",
      field: "reHiredDate",
      colId: "reHiredDate",
      minWidth: 110,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      valueFormatter: (params) => {
        const date = params.value;
        return mmDDYYFormat(date);
      }
    },
    createCurrencyColumn({
      headerName: "Current Balance",
      field: "netBalanceLastYear",
      minWidth: 70
    }),
    createCurrencyColumn({
      headerName: "Vested Balance",
      field: "vestedBalanceLastYear",
      minWidth: 150
    }),
    createStoreColumn({
      maxWidth: 80,
      alignment: "left",
      sortable: true
    }),
    {
      headerName: "Years",
      field: "companyContributionYears",
      colId: "companyContributionYears",
      width: 80,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true
    },
  ];
};

export const GetDetailColumns = (addRowToSelectedRows: (id: number) => void, removeRowFromSelectedRows: (id: number) => void, selectedProfitYear: number, onSave?: (request: ForfeitureAdjustmentUpdateRequest) => Promise<void>, onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[]) => Promise<void>): ColDef[] => {
  return [
    {
      headerName: "Profit Year",
      field: "profitYear",
      colId: "profitYear",
      width: 100,
      minWidth: 30,
      type: "rightAligned",
      resizable: true,
      sortable: false
    },
    {
      headerName: "Hours",
      field: "hoursCurrentYear",
      colId: "hoursCurrentYear",
      width: 120,
      type: "rightAligned",
      resizable: true,
      sortable: true,
      valueGetter: (params) => {
        // I think showing 0 is visually noisy, so I'm suppressing that.   Especially since we will not have the
        // actual hours for our initial year - 1.
        const value = params.data?.hoursCurrentYear;
        return (value == null || value == 0) ? null : value;
      },
      valueFormatter: (params) => {
        const hours = params.value;
        if (hours == null) return "";
        return formatNumberWithComma(hours);
      }
    },
    createCurrencyColumn({
      headerName: "Wages",
      field: "wages",
      minWidth: 150
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
        if (id == null)
          return "";
        const name = params.data?.enrollmentName;
        return `[${id}] ${name}`;
      }
    },
    createCurrencyColumn({
      headerName: "Forfeiture",
      field: "forfeiture",
      minWidth: 150
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
      editable: ({ node }) => node.data.isDetail && node.data.profitYear === selectedProfitYear,
      cellEditor: SuggestedForfeitEditor,
      cellRenderer: (params: ICellRendererParams) => SuggestedForfeitCellRenderer({ ...params, selectedProfitYear }, false, true),
      valueFormatter: agGridNumberToCurrency,
      valueGetter: (params) => {
        if (!params.data.isDetail) return null;
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}${params.data.enrollmentId ? `-${params.data.enrollmentId}` : ''}-${params.node?.id || 'unknown'}`;
        const editedValue = params.context?.editedValues?.[rowKey]?.value;
        return editedValue !== undefined ? editedValue : params.data.suggestedForfeit;
      }
    },
    {
      headerName: "Remark",
      field: "remark",
      colId: "remark",
      width: 150,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: false
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
      cellRenderer: (params: RehireForfeituresSaveButtonCellParams) => {
        if (!params.data.isDetail || params.data.profitYear !== selectedProfitYear) {
          return "";
        }
        const id = Number(params.node?.id) || -1;
        const isSelected = params.node?.isSelected() || false;
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}${params.data.enrollmentId ? `-${params.data.enrollmentId}` : ''}-${params.node?.id || 'unknown'}`;
        const hasError = params.context?.editedValues?.[rowKey]?.hasError;
        const currentValue = params.context?.editedValues?.[rowKey]?.value ?? params.data.suggestedForfeit;
        const isLoading = params.context?.loadingRowIds?.has(params.data.badgeNumber);

        return <div>
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
            }} />
          <IconButton
            onClick={async () => {
              if (params.data.isDetail && params.onSave) {
                const request: ForfeitureAdjustmentUpdateRequest = {
                  badgeNumber: params.data.badgeNumber,
                  profitYear: params.data.profitYear,
                  forfeitureAmount: -(currentValue || 0),
                  classAction: params.data.classAction
                };
                await params.onSave(request);
              }
            }}
            disabled={hasError || (currentValue || 0) === 0 || isLoading}
          >
            {isLoading ? (
              <CircularProgress size={20} />
            ) : (
              <SaveOutlined />
            )}
          </IconButton>
        </div>;
      }
    }
  ];
};
