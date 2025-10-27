import { IHeaderParams } from "ag-grid-community";
import { SelectableGridHeader } from "../SelectableGridHeader";
import useDecemberFlowProfitYear from "../../hooks/useDecemberFlowProfitYear";
import { ForfeitureAdjustmentUpdateRequest } from "../../types";

export type ActivityType = "termination" | "unforfeit";

export interface RowKeyConfig {
  type: ActivityType;
}

export interface RowData {
  isDetail: boolean;
  profitYear: number;
  badgeNumber: string;
  enrollmentId?: string;
  suggestedForfeit?: number;
  suggestedUnforfeiture?: number;
  profitDetailId?: number;
  remark?: string;
}

export interface GridContext {
  editedValues?: Record<string, { value?: number }>;
}

export interface SharedHeaderComponentConfig {
  activityType: ActivityType;
  selectedProfitYearOverride?: number;
}

interface HeaderComponentProps extends IHeaderParams {
  addRowToSelectedRows: (id: number) => void;
  removeRowFromSelectedRows: (id: number) => void;
  onBulkSave?: (requests: ForfeitureAdjustmentUpdateRequest[], names: string[]) => Promise<void>;
  isBulkSaving?: boolean;
  isReadOnly?: boolean;
  config: SharedHeaderComponentConfig;
}

/**
 * Generate row key based on activity type
 */
function generateRowKey(activityType: ActivityType, nodeData: RowData): string {
  if (activityType === "unforfeit") {
    return nodeData.profitDetailId?.toString() || "";
  }
  return `${nodeData.badgeNumber}-${nodeData.profitYear}`;
}

/**
 * Get current value from edited values or original data
 */
function getCurrentValue(
  activityType: ActivityType,
  nodeData: RowData,
  context: GridContext
): number | undefined {
  const rowKey = generateRowKey(activityType, nodeData);
  const editedValue = context?.editedValues?.[rowKey]?.value;

  if (editedValue !== undefined) {
    return editedValue;
  }

  return activityType === "unforfeit" ? nodeData.suggestedUnforfeiture : nodeData.suggestedForfeit;
}

/**
 * Transform forfeiture value based on activity type
 */
function transformForfeitureValue(activityType: ActivityType, value: number): number {
  if (activityType === "unforfeit") {
    return -(value || 0); // NEGATE for unforfeit
  }
  return value || 0; // No transformation for termination
}

/**
 * Shared header component for forfeit activities (Termination and UnForfeit)
 * Provides bulk save functionality with activity-specific behavior
 */
export const SharedForfeitHeaderComponent: React.FC<HeaderComponentProps> = (params: HeaderComponentProps) => {
  const defaultProfitYear = useDecemberFlowProfitYear();
  const selectedProfitYear = params.config.selectedProfitYearOverride ?? defaultProfitYear;
  const activityType = params.config.activityType;

  const isNodeEligible = (nodeData: RowData, context: GridContext): boolean => {
    // For termination: only current year detail rows
    // For unforfeit: all detail rows
    if (!nodeData.isDetail) return false;

    if (activityType === "termination" && nodeData.profitYear !== selectedProfitYear) {
      return false;
    }

    const currentValue = getCurrentValue(activityType, nodeData, context);
    return (currentValue || 0) !== 0;
  };

  const createUpdatePayload = (nodeData: RowData, context: GridContext): ForfeitureAdjustmentUpdateRequest => {
    const currentValue = getCurrentValue(activityType, nodeData, context);
    const transformedValue = transformForfeitureValue(activityType, currentValue || 0);

    const basePayload: ForfeitureAdjustmentUpdateRequest = {
      badgeNumber: Number(nodeData.badgeNumber),
      profitYear: activityType === "unforfeit" ? selectedProfitYear : nodeData.profitYear,
      forfeitureAmount: transformedValue,
      classAction: false
    };

    // UnForfeit requires offsettingProfitDetailId
    if (activityType === "unforfeit" && nodeData.profitDetailId) {
      basePayload.offsettingProfitDetailId = nodeData.profitDetailId;
    }

    return basePayload;
  };

  // Check if any rows are in loading state
  const hasSavingInProgress = (): boolean => {
    return params.context?.loadingRowIds?.size > 0;
  };

  return (
    <SelectableGridHeader
      {...params}
      isNodeEligible={isNodeEligible}
      createUpdatePayload={createUpdatePayload}
      onBulkSave={params.onBulkSave}
      isBulkSaving={hasSavingInProgress}
      isReadOnly={params.isReadOnly}
    />
  );
};
