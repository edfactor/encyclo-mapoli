import { ColDef, ICellRendererParams, IHeaderParams } from "ag-grid-community";
import { agGridNumberToCurrency, formatNumberWithComma } from "smart-ui-library";
import { createBadgeColumn, createAgeColumn, createCurrencyColumn } from "../../../utils/gridColumnFactory";
import { getForfeitedStatus } from "../../../utils/enrollmentUtil";
import { mmDDYYFormat } from "utils/dateUtils";
import { Checkbox, IconButton, CircularProgress } from "@mui/material";
import { SaveOutlined } from "@mui/icons-material";
import useDecemberFlowProfitYear from "../../../hooks/useDecemberFlowProfitYear";
import { SuggestedForfeitEditor, SuggestedForfeitCellRenderer } from "../../../components/SuggestedForfeiture";
import { SelectableGridHeader } from "../../../components/SelectableGridHeader";
import { ForfeitureAdjustmentUpdateRequest } from "../../../reduxstore/types";


export const GetTerminationColumns = (): ColDef[] => {
  return [
    createBadgeColumn({
      headerName: "Badge",
      minWidth: 100,
      alignment: "left",
      psnSuffix: true
    }),
    {
      headerName: "Name",
      field: "name",
      colId: "name",
      width: 200,
      pinned: "left",
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      sortable: true,
      flex: 1
    }
  ];
};

// Separate function for detail columns that will be used for master-detail view
export const GetDetailColumns = (
  addRowToSelectedRows: (id: number) => void,
  removeRowFromSelectedRows: (id: number) => void,
  selectedRowIds: number[],
  selectedProfitYear: number,
  onSave?: (request: ForfeitureAdjustmentUpdateRequest) => Promise<void>,
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[]) => Promise<void>
): ColDef[] => {
  return [
    {
      headerName: "Profit Year",
      field: "profitYear",
      colId: "profitYear",
      width: 100,
      type: "rightAligned",
      resizable: true,
      sortable: false
    },
    createCurrencyColumn({
      headerName: "Beginning Balance",
      field: "beginningBalance",
      minWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Beneficiary Allocation",
      field: "beneficiaryAllocation",
      minWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Distribution Amount",
      field: "distributionAmount",
      minWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Forfeit Amount",
      field: "forfeit",
      minWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Ending Balance",
      field: "endingBalance",
      minWidth: 150
    }),
    createCurrencyColumn({
      headerName: "Vested Balance",
      field: "vestedBalance",
      minWidth: 150
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
    {
      headerName: "Term Date",
      field: "dateTerm",
      colId: "dateTerm",
      width: 150,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: false,
      valueFormatter: (params) => {
        const date = params.value;
        return mmDDYYFormat(date);
      }
    },
    {
      headerName: "YTD PS Hours",
      field: "ytdPsHours",
      colId: "ytdPsHours",
      width: 125,
      type: "rightAligned",
      resizable: true,
      sortable: false,
      valueFormatter: (params) => {
        const hours = params.value;
        return formatNumberWithComma(hours);
      }
    },
    createAgeColumn({
      maxWidth: 70,
      sortable: false
    }),
    {
      headerName: "Forfeit",
      field: "enrollmentCode",
      colId: "enrollmentCode",
      width: 120,
      headerClass: "center-align",
      cellClass: "center-align",
      resizable: true,
      sortable: false,
      // Yes, the enrollmentCode is tied to the Forfeited status. See PS-1279
      valueFormatter: (params) => getForfeitedStatus(params.value)
    },
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
      cellRenderer: (params: ICellRendererParams) => SuggestedForfeitCellRenderer({
        ...params, selectedProfitYear
      }, true, false),
      valueFormatter: agGridNumberToCurrency,
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

        return <div>
          <Checkbox checked={isSelected} onChange={() => {
            if (isSelected) {
              params.removeRowFromSelectedRows(id);
            } else {
              params.addRowToSelectedRows(id);
            }
            params.node?.setSelected(!isSelected);
          }} />
          <IconButton
            onClick={async () => {
              if (params.data.isDetail && params.onSave) {
                const request: ForfeitureAdjustmentUpdateRequest = {
                  badgeNumber: params.data.badgeNumber,
                  profitYear: params.data.profitYear,
                  forfeitureAmount: (currentValue || 0)
                };
                await params.onSave(request);
              }
            }}
            disabled={hasError || isLoading}
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

interface HeaderComponentProps extends IHeaderParams {
  addRowToSelectedRows: (id: number) => void;
  removeRowFromSelectedRows: (id: number) => void;
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[]) => Promise<void>;
  isBulkSaving?: boolean;
}

interface SaveButtonCellParams extends ICellRendererParams {
  removeRowFromSelectedRows: (id: number) => void;
  addRowToSelectedRows: (id: number) => void;
  onSave?: (request: ForfeitureAdjustmentUpdateRequest) => Promise<void>;
}

export const HeaderComponent: React.FC<HeaderComponentProps> = (params: HeaderComponentProps) => {
  const selectedProfitYear = useDecemberFlowProfitYear();

  const isNodeEligible = (nodeData: any, context: any) => {
    if (!nodeData.isDetail || nodeData.profitYear !== selectedProfitYear) return false;
    const rowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}`;
    const currentValue = context?.editedValues?.[rowKey]?.value ?? nodeData.suggestedForfeit;
    return (currentValue || 0) !== 0;
  };

  const createUpdatePayload = (nodeData: any, context: any): ForfeitureAdjustmentUpdateRequest => {
    const rowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}`;
    const currentValue = context?.editedValues?.[rowKey]?.value ?? nodeData.suggestedForfeit;

    return {
      badgeNumber: nodeData.badgeNumber,
      profitYear: nodeData.profitYear,
      forfeitureAmount: -(currentValue || 0),
      classAction: false,
    };
  };

  // Check if any rows are in loading state
  const hasSavingInProgress = params.context?.loadingRowIds?.size > 0;

  return <SelectableGridHeader
    {...params}
    isNodeEligible={isNodeEligible}
    createUpdatePayload={createUpdatePayload}
    isBulkSaving={hasSavingInProgress}
  />;
};
