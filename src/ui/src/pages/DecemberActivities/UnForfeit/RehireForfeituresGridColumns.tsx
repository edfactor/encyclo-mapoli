import { SaveOutlined } from "@mui/icons-material";
import { Checkbox, CircularProgress, IconButton } from "@mui/material";
import { ColDef, ICellRendererParams } from "ag-grid-community";
import useDecemberFlowProfitYear from "hooks/useDecemberFlowProfitYear";
import { agGridNumberToCurrency } from "smart-ui-library";
import { SelectableGridHeader } from "../../../components/SelectableGridHeader";
import { SuggestedForfeitCellRenderer, SuggestedForfeitEditor } from "../../../components/SuggestedForfeiture";
import {
  ForfeitureAdjustmentUpdateRequest,
  RehireForfeituresHeaderComponentProps,
  RehireForfeituresSaveButtonCellParams
} from "../../../reduxstore/types";
import {
  createBadgeColumn,
  createCountColumn,
  createCurrencyColumn,
  createDateColumn,
  createHoursColumn,
  createNameColumn,
  createSSNColumn,
  createStoreColumn,
  createYearColumn
} from "../../../utils/gridColumnFactory";

export const HeaderComponent: React.FC<RehireForfeituresHeaderComponentProps> = (
  params: RehireForfeituresHeaderComponentProps
) => {
  const selectedProfitYear = useDecemberFlowProfitYear();

  const isNodeEligible = (nodeData: any, context: any) => {
    if (!nodeData.isDetail || nodeData.profitYear !== selectedProfitYear) return false;
    // For bulk operations, we need to check all possible rowKeys for this data
    const baseRowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}${nodeData.enrollmentId ? `-${nodeData.enrollmentId}` : ""}`;
    const editedValues = context?.editedValues || {};
    const matchingKey = Object.keys(editedValues).find((key) => key.startsWith(baseRowKey));
    const currentValue = matchingKey ? editedValues[matchingKey]?.value : nodeData.suggestedForfeit;
    return (currentValue || 0) !== 0;
  };

  const createUpdatePayload = (nodeData: any, context: any): ForfeitureAdjustmentUpdateRequest => {
    // For bulk operations, we need to find the actual edited value
    const baseRowKey = `${nodeData.badgeNumber}-${nodeData.profitYear}${nodeData.enrollmentId ? `-${nodeData.enrollmentId}` : ""}`;
    const editedValues = context?.editedValues || {};
    const matchingKey = Object.keys(editedValues).find((key) => key.startsWith(baseRowKey));
    const currentValue = matchingKey ? editedValues[matchingKey]?.value : nodeData.suggestedForfeit;
    const isClassAction = nodeData.remark === "FORFEIT CA";

    return {
      badgeNumber: nodeData.badgeNumber,
      profitYear: nodeData.profitYear,
      forfeitureAmount: -(currentValue || 0),
      classAction: isClassAction
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

export const GetRehireForfeituresColumns = (): ColDef[] => {
  return [
    createBadgeColumn({}),
    createNameColumn({
      field: "fullName"
    }),
    createSSNColumn({}),
    createDateColumn({
      headerName: "Hire Date",
      field: "hireDate"
    }),
    createDateColumn({
      headerName: "Termination Date",
      field: "terminationDate"
    }),
    createDateColumn({
      headerName: "Rehired Date",
      field: "reHiredDate"
    }),
    createCurrencyColumn({
      headerName: "Current Balance",
      field: "netBalanceLastYear"
    }),
    createCurrencyColumn({
      headerName: "Vested Balance",
      field: "vestedBalanceLastYear"
    }),
    createStoreColumn({})
  ];
};

export const GetDetailColumns = (
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
      headerName: "Hours",
      field: "hoursCurrentYear"
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
      editable: ({ node }) => node.data.isDetail && node.data.profitYear === selectedProfitYear,
      cellEditor: SuggestedForfeitEditor,
      cellRenderer: (params: ICellRendererParams) =>
        SuggestedForfeitCellRenderer({ ...params, selectedProfitYear }, false, true),
      valueFormatter: agGridNumberToCurrency,
      valueGetter: (params) => {
        if (!params.data.isDetail) return null;
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}${params.data.enrollmentId ? `-${params.data.enrollmentId}` : ""}-${params.node?.id || "unknown"}`;
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
        const rowKey = `${params.data.badgeNumber}-${params.data.profitYear}${params.data.enrollmentId ? `-${params.data.enrollmentId}` : ""}-${params.node?.id || "unknown"}`;
        const hasError = params.context?.editedValues?.[rowKey]?.hasError;
        const currentValue = params.context?.editedValues?.[rowKey]?.value ?? params.data.suggestedForfeit;
        const isLoading = params.context?.loadingRowIds?.has(params.data.badgeNumber);

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
                    forfeitureAmount: -(currentValue || 0)
                  };
                  await params.onSave(request);
                }
              }}
              disabled={params.data.remark === "REMARK CA" || hasError || (currentValue || 0) === 0 || isLoading}>
              {isLoading ? <CircularProgress size={20} /> : <SaveOutlined />}
            </IconButton>
          </div>
        );
      }
    }
  ];
};
