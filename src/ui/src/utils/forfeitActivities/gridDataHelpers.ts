/**
 * Shared utilities for Forfeit-related Activities master-detail grid screens.
 * Supports both Termination and UnForfeit pages with their unique requirements.
 */

/**
 * Activity type discriminator for configuration
 */
export type ActivityType = "termination" | "unforfeit";

/**
 * Row key configuration for different activity types
 */
export interface RowKeyConfig {
  type: ActivityType;
}

/**
 * Parameters for generating row keys
 */
export interface RowKeyParams {
  badgeNumber: number;
  profitYear: number;
  profitDetailId?: number;
}

/**
 * Generate unique row key for tracking edited values
 *
 * @param config - Configuration specifying activity type
 * @param params - Parameters needed to construct the key
 * @returns Unique string key for the row
 *
 * @example
 * // UnForfeit: uses profitDetailId (multiple forfeitures per badge/year possible)
 * generateRowKey({ type: "unforfeit" }, { badgeNumber: 123, profitYear: 2025, profitDetailId: 789 })
 * // Returns: "789"
 *
 * @example
 * // Termination: uses composite key
 * generateRowKey({ type: "termination" }, { badgeNumber: 123, profitYear: 2025 })
 * // Returns: "123-2025"
 */
export function generateRowKey(config: RowKeyConfig, params: RowKeyParams): string {
  if (config.type === "unforfeit") {
    if (params.profitDetailId === undefined) {
      throw new Error("profitDetailId is required for unforfeit row keys");
    }
    return params.profitDetailId.toString();
  }

  // Termination uses composite key
  return `${params.badgeNumber}-${params.profitYear}`;
}

/**
 * Transform forfeiture value before saving
 *
 * @param activityType - Type of activity
 * @param value - The original value
 * @returns Transformed value
 *
 * @example
 * // UnForfeit: negates value (positive UI → negative backend)
 * transformForfeitureValue("unforfeit", 1500) // Returns: -1500
 *
 * @example
 * // Termination: no transformation
 * transformForfeitureValue("termination", 1500) // Returns: 1500
 */
export function transformForfeitureValue(activityType: ActivityType, value: number): number {
  if (activityType === "unforfeit") {
    return -(value || 0);
  }
  return value || 0;
}

/**
 * Get the field name for the editable column
 *
 * @param activityType - Type of activity
 * @returns Field name
 *
 * @example
 * getEditableFieldName("unforfeit") // Returns: "suggestedUnforfeiture"
 * getEditableFieldName("termination") // Returns: "suggestedForfeit"
 */
export function getEditableFieldName(activityType: ActivityType): string {
  return activityType === "unforfeit" ? "suggestedUnforfeiture" : "suggestedForfeit";
}

/**
 * Determine if a detail row is editable based on activity-specific rules
 *
 * @param activityType - Type of activity
 * @param detailRow - The detail row data
 * @param selectedProfitYear - The currently selected profit year (for termination)
 * @returns Whether the row is editable
 *
 * @example
 * // UnForfeit: editable if suggested value is not null
 * isDetailRowEditable("unforfeit", { suggestedUnforfeiture: 1500 }, 2025)
 * // Returns: true
 *
 * @example
 * // Termination: editable only for current profit year
 * isDetailRowEditable("termination", { profitYear: 2025, suggestedForfeit: 100 }, 2025)
 * // Returns: true
 *
 * isDetailRowEditable("termination", { profitYear: 2024, suggestedForfeit: 100 }, 2025)
 * // Returns: false
 */
export function isDetailRowEditable(
  activityType: ActivityType,
  detailRow: {
    profitYear?: number;
    suggestedUnforfeiture?: number | null;
    suggestedForfeit?: number | null;
  },
  selectedProfitYear: number
): boolean {
  if (activityType === "unforfeit") {
    // UnForfeit: editable if suggested value is not null
    return detailRow.suggestedUnforfeiture != null;
  }

  // Termination: editable only for current profit year
  return detailRow.profitYear === selectedProfitYear;
}

/**
 * Configuration for flattening master-detail data
 */
export interface FlattenConfig<TMaster> {
  /** Get unique key for master row */
  getKey: (master: TMaster) => string;
  /** Get detail rows for a master */
  getDetails: (master: TMaster) => unknown[];
  /** Check if master has details */
  hasDetails: (master: TMaster) => boolean;
}

/**
 * Flatten master-detail data structure for AG Grid
 *
 * @param masterData - Array of master records
 * @param expandedRows - Set of expanded row keys
 * @param config - Configuration for accessing master and detail data
 * @returns Flattened array with master rows and interspersed detail rows
 *
 * @example
 * const gridData = flattenMasterDetailData(
 *   employees,
 *   expandedRows,
 *   {
 *     getKey: (row) => row.badgeNumber.toString(),
 *     getDetails: (row) => row.profitDetails,
 *     hasDetails: (row) => Boolean(row.profitDetails && row.profitDetails.length > 0)
 *   }
 * );
 */
export function flattenMasterDetailData<TMaster>(
  masterData: TMaster[],
  expandedRows: Set<string>,
  config: FlattenConfig<TMaster>
): unknown[] {
  if (!masterData) return [];

  return masterData.flatMap((master) => {
    const key = config.getKey(master);
    const isExpanded = expandedRows.has(key);
    const hasDetails = config.hasDetails(master);

    const mainRow = {
      ...master,
      isExpandable: hasDetails,
      isExpanded,
      isDetail: false
    };

    // If expanded and has details, include detail rows
    if (isExpanded && hasDetails) {
      const details = config.getDetails(master);
      const detailRows = details.map((detail) => ({
        ...detail,
        isDetail: true
      }));
      return [mainRow, ...detailRows];
    }

    // Otherwise just the main row
    return [mainRow];
  });
}

/**
 * Get current edited value with fallback to original
 *
 * @param editedValues - Map of edited values by row key
 * @param rowKey - The row key to look up
 * @param originalValue - The original value to fall back to
 * @returns Current value (edited or original)
 *
 * @example
 * const editedValues = { "123-2025": { value: 1500 } };
 * getEditedValue(editedValues, "123-2025", 1000) // Returns: 1500
 * getEditedValue(editedValues, "456-2025", 1000) // Returns: 1000
 */
export function getEditedValue(
  editedValues: Record<string, { value?: number; hasError?: boolean }>,
  rowKey: string,
  originalValue?: number | null
): number {
  const edited = editedValues?.[rowKey]?.value;
  return edited ?? originalValue ?? 0;
}

/**
 * Check if a row has validation errors
 *
 * @param editedValues - Map of edited values by row key
 * @param rowKey - The row key to check
 * @returns Whether the row has errors
 *
 * @example
 * const editedValues = {
 *   "123-2025": { value: 1500, hasError: false },
 *   "456-2025": { value: -100, hasError: true }
 * };
 * hasRowError(editedValues, "456-2025") // Returns: true
 */
export function hasRowError(
  editedValues: Record<string, { value?: number; hasError?: boolean }>,
  rowKey: string
): boolean {
  return editedValues?.[rowKey]?.hasError ?? false;
}
