import { MAX_EMPLOYEE_BADGE_LENGTH } from "@/constants";
import { SaveOutlined } from "@mui/icons-material";
import { Checkbox, CircularProgress, IconButton, Tooltip } from "@mui/material";
import { ICellRendererParams } from "ag-grid-community";
import { ForfeitureAdjustmentUpdateRequest } from "../../types";

export type ActivityType = "termination" | "unforfeit";

export interface SaveButtonCellParams extends ICellRendererParams {
  removeRowFromSelectedRows: (id: number) => void;
  addRowToSelectedRows: (id: number) => void;
  onSave?: (request: ForfeitureAdjustmentUpdateRequest, name: string) => Promise<void>;
}

export interface SaveButtonConfig {
  activityType: ActivityType;
  selectedProfitYear: number;
  isReadOnly: boolean;
}

interface RowData {
  badgeNumber: string | number;
  psn: number;
  profitYear: number;
  profitDetailId?: number;
  suggestedForfeit?: number;
  suggestedUnforfeiture?: number;
  isDetail: boolean;
  fullName?: string;
  name?: string;
}

/**
 * Generate row key based on activity type
 * Must match the key used in column definitions (valueGetter)
 */
function generateRowKey(activityType: ActivityType, data: RowData): string {
  if (activityType === "unforfeit") {
    return data.profitDetailId?.toString() || "";
  }
  // For termination: use composite key (badgeNumber-profitYear)
  // badgeNumber is always set in grid data (see useTerminationGrid)
  return `${data.badgeNumber}-${data.profitYear}`;
}

/**
 * Get current value from edited values or original data
 */
function getCurrentValue(activityType: ActivityType, params: SaveButtonCellParams, rowKey: string): number {
  const editedValue = params.context?.editedValues?.[rowKey]?.value;
  if (editedValue !== undefined) {
    return editedValue;
  }

  return activityType === "unforfeit" ? params.data.suggestedUnforfeiture : (params.data.suggestedForfeit ?? 0);
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
 * Check if we should show controls for this row (regardless of read-only status)
 */
function shouldShowControls(activityType: ActivityType, params: SaveButtonCellParams): boolean {
  if (!params.data.isDetail) {
    return false;
  }

  if (activityType === "termination") {
    // Termination: only show if backend gives us a non-null, non-zero value
    return params.data.suggestedForfeit != null && params.data.suggestedForfeit !== 0;
  } else {
    // UnForfeit: all rows with non-null suggestedUnforfeiture get controls
    return params.data.suggestedUnforfeiture != null;
  }
}

/**
 * Shared save button cell renderer for forfeit activities
 * Provides checkbox + save button with activity-specific behavior
 */
export function createSaveButtonCellRenderer(config: SaveButtonConfig) {
  return (params: SaveButtonCellParams) => {
    const { activityType, selectedProfitYear } = config;
    // Read isReadOnly from context for reactivity when status changes
    const isReadOnly = params.context?.isReadOnly ?? config.isReadOnly;

    // If psn is too long (beneficiary) or not editable, return empty
    if (!shouldShowControls(activityType, params) || params?.data?.psn?.length > MAX_EMPLOYEE_BADGE_LENGTH) {
      return "";
    }

    const id = Number(params.node?.id) || -1;
    const isSelected = params.node?.isSelected() || false;
    const rowKey = generateRowKey(activityType, params.data);
    const currentValue = getCurrentValue(activityType, params, rowKey);
    const isLoading =
      activityType === "termination"
        ? params.context?.loadingRowIds?.has(params.data.psn)
        : params.context?.loadingRowIds?.has(params.data.badgeNumber);
    const isZeroValue = currentValue === 0 || currentValue === null || currentValue === undefined;
    // Allow saving even with validation warnings (hasError is just a warning, not blocking)
    const isDisabled = isLoading || isZeroValue || isReadOnly;
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
            // Refresh cells for unforfeit (needed for proper state sync)
            if (activityType === "unforfeit") {
              params.api.refreshCells({ force: true });
            }
          }
        }}
      />
    );

    const saveButtonElement = (
      <IconButton
        onClick={async () => {
          if (!isReadOnly && params.data.isDetail && params.onSave) {
            const transformedValue = transformForfeitureValue(activityType, currentValue);

            const request: ForfeitureAdjustmentUpdateRequest = {
              badgeNumber: activityType === "unforfeit" ? params.data.badgeNumber : params.data.psn,
              profitYear: activityType === "unforfeit" ? selectedProfitYear : params.data.profitYear,
              forfeitureAmount: transformedValue,
              classAction: false
            };

            // UnForfeit requires offsettingProfitDetailId
            if (activityType === "unforfeit") {
              request.offsettingProfitDetailId = params.data.profitDetailId;
            }

            const employeeName = params.data.fullName || params.data.name || "Unknown Employee";
            await params.onSave(request, employeeName);
          }
        }}
        disabled={isDisabled}>
        {isLoading ? <CircularProgress size={20} /> : <SaveOutlined />}
      </IconButton>
    );

    // Render with appropriate tooltips
    return (
      <div>
        {isReadOnly || isZeroValue ? (
          <Tooltip
            title={
              isZeroValue
                ? activityType === "termination"
                  ? "Forfeit cannot be zero."
                  : "Unforfeiture cannot be zero."
                : readOnlyTooltip
            }
            arrow>
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
  };
}
