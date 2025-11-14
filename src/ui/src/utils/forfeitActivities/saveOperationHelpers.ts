/**
 * Shared utilities for save operations, message generation, and error handling
 * for Forfeit-related Activities (Termination and UnForfeit).
 */

import { GridApi } from "ag-grid-community";
import { ActivityType, generateRowKey, RowKeyConfig } from "./gridDataHelpers";

/**
 * Base request structure for forfeiture adjustments
 */
export interface ForfeitureAdjustmentUpdateRequest {
  badgeNumber: number;
  profitYear: number;
  forfeitureAmount: number;
  classAction: boolean;
  offsettingProfitDetailId?: number;
}

/**
 * Activity configuration for save operations
 */
export interface ActivityConfig {
  activityType: ActivityType;
  rowKeyConfig: RowKeyConfig;
}

/**
 * Prepare a single save request (pass-through for consistency)
 *
 * Note: Value transformation happens before this function is called.
 * For unforfeit, the sign has already been flipped to negative.
 *
 * @param request - Base request data
 * @returns Request ready for API
 *
 * @example
 * const request = prepareSaveRequest(
 *   { badgeNumber: 123, profitYear: 2025, forfeitureAmount: 1500, classAction: false }
 * );
 * // Returns: { ...request }
 */
export function prepareSaveRequest(
  request: ForfeitureAdjustmentUpdateRequest
): ForfeitureAdjustmentUpdateRequest {
  return {
    ...request,
    forfeitureAmount: request.forfeitureAmount
  };
}

/**
 * Prepare multiple save requests (pass-through for consistency)
 *
 * Note: Value transformation happens before this function is called.
 *
 * @param requests - Array of base request data
 * @returns Array of requests ready for API
 *
 * @example
 * const requests = prepareBulkSaveRequests([
 *   { badgeNumber: 123, profitYear: 2025, forfeitureAmount: 1500, classAction: false },
 *   { badgeNumber: 456, profitYear: 2025, forfeitureAmount: 2000, classAction: false }
 * ]);
 */
export function prepareBulkSaveRequests(
  requests: ForfeitureAdjustmentUpdateRequest[]
): ForfeitureAdjustmentUpdateRequest[] {
  return requests.map((req) => prepareSaveRequest(req));
}

/**
 * Generate success message for individual save operation
 *
 * @param activityType - Type of activity
 * @param memberName - Name of the member
 * @param amount - Amount saved
 * @returns Formatted success message
 *
 * @example
 * generateSaveSuccessMessage("unforfeit", "Doe, John", 1500)
 * // Returns: "Successfully saved unforfeiture of $1,500.00 for Doe, John"
 *
 * generateSaveSuccessMessage("termination", "Smith, Jane", 2000)
 * // Returns: "Successfully saved forfeiture of $2,000.00 for Smith, Jane"
 */
export function generateSaveSuccessMessage(activityType: ActivityType, memberName: string, amount: number): string {
  const operationType = activityType === "unforfeit" ? "unforfeiture" : "forfeiture";
  const formattedAmount = new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD"
  }).format(Math.abs(amount));

  return `Successfully saved ${operationType} of ${formattedAmount} for ${memberName}`;
}

/**
 * Generate success message for bulk save operation
 *
 * @param activityType - Type of activity
 * @param count - Number of records saved
 * @param memberNames - Optional array of member names
 * @returns Formatted success message
 *
 * @example
 * generateBulkSaveSuccessMessage("unforfeit", 5)
 * // Returns: "Successfully saved 5 unforfeitures"
 *
 * generateBulkSaveSuccessMessage("termination", 3, ["Doe, John", "Smith, Jane", "Brown, Bob"])
 * // Returns: "Successfully saved 3 forfeitures for Doe, John, Smith, Jane, Brown, Bob"
 */
export function generateBulkSaveSuccessMessage(
  activityType: ActivityType,
  count: number,
  memberNames?: string[]
): string {
  const operationType = activityType === "unforfeit" ? "unforfeitures" : "forfeitures";
  const baseMessage = `Successfully saved ${count} ${operationType}`;

  if (memberNames && memberNames.length > 0) {
    return `${baseMessage} for ${memberNames.join(", ")}`;
  }

  return baseMessage;
}

/**
 * Extract row keys from save requests for grid operations
 *
 * @param config - Activity configuration
 * @param requests - Array of save requests
 * @returns Array of row keys
 *
 * @example
 * // UnForfeit: returns profitDetailIds
 * getRowKeysForRequests(
 *   { activityType: "unforfeit", rowKeyConfig: { type: "unforfeit" } },
 *   [{ badgeNumber: 123, profitYear: 2025, offsettingProfitDetailId: 789, ... }]
 * );
 * // Returns: ["789"]
 *
 * @example
 * // Termination: returns composite keys
 * getRowKeysForRequests(
 *   { activityType: "termination", rowKeyConfig: { type: "termination" } },
 *   [{ badgeNumber: 123, profitYear: 2025, ... }]
 * );
 * // Returns: ["123-2025"]
 */
export function getRowKeysForRequests(config: ActivityConfig, requests: ForfeitureAdjustmentUpdateRequest[]): string[] {
  return requests.map((req) =>
    generateRowKey(config.rowKeyConfig, {
      badgeNumber: req.badgeNumber,
      profitYear: req.profitYear,
      profitDetailId: req.offsettingProfitDetailId
    })
  );
}

/**
 * Clear AG Grid selections for saved items by badge number
 *
 * @param gridApi - AG Grid API instance
 * @param badgeNumbers - Array of badge numbers to clear
 *
 * @example
 * clearGridSelectionsForBadges(gridApi, [123456, 789012]);
 */
export function clearGridSelectionsForBadges(gridApi: GridApi | undefined, badgeNumbers: number[]): void {
  if (!gridApi) return;

  const badgeSet = new Set(badgeNumbers);

  gridApi.forEachNode((node) => {
    if (node.data && badgeSet.has(node.data.badgeNumber)) {
      node.setSelected(false);
    }
  });
}

/**
 * Error messages by activity type and operation
 */
const ERROR_MESSAGES = {
  unforfeit: {
    save: "Failed to save unforfeiture adjustment",
    bulkSave: "Failed to save bulk unforfeiture adjustments",
    fetch: "Failed to fetch unforfeiture data"
  },
  termination: {
    save: "Failed to save forfeiture adjustment",
    bulkSave: "Failed to save bulk forfeiture adjustments",
    fetch: "Failed to fetch termination data"
  }
} as const;

/**
 * Get error message for activity type and operation
 *
 * @param activityType - Type of activity
 * @param operation - Operation that failed
 * @returns Error message
 *
 * @example
 * getErrorMessage("unforfeit", "save")
 * // Returns: "Failed to save unforfeiture adjustment"
 */
export function getErrorMessage(activityType: ActivityType, operation: "save" | "bulkSave" | "fetch"): string {
  return ERROR_MESSAGES[activityType][operation];
}

/**
 * Format API error for display
 *
 * @param error - Error from API call
 * @param defaultMessage - Default message if error details not available
 * @returns Formatted error message
 *
 * @example
 * formatApiError(error, "Failed to save adjustment")
 */
export function formatApiError(error: unknown, defaultMessage: string): string {
  if (error && typeof error === "object") {
    if ("message" in error && typeof error.message === "string") {
      return error.message;
    }
    if ("data" in error && error.data && typeof error.data === "object") {
      if ("message" in error.data && typeof error.data.message === "string") {
        return error.data.message;
      }
    }
  }

  return defaultMessage;
}

/**
 * Batch configuration for save operations
 */
export interface BatchConfig {
  /** Maximum number of items to save in a single batch */
  batchSize: number;
  /** Delay between batches in milliseconds */
  delayMs: number;
}

/**
 * Execute save operations in batches with delay
 *
 * @param requests - All requests to save
 * @param saveFn - Function to save a single request
 * @param config - Batch configuration
 * @returns Promise that resolves when all batches complete
 *
 * @example
 * await executeBatchSave(
 *   requests,
 *   async (req) => await updateAdjustmentMutation(req).unwrap(),
 *   { batchSize: 10, delayMs: 100 }
 * );
 */
export async function executeBatchSave<TRequest>(
  requests: TRequest[],
  saveFn: (request: TRequest) => Promise<unknown>,
  config: BatchConfig = { batchSize: 10, delayMs: 100 }
): Promise<void> {
  const batches: TRequest[][] = [];

  // Split into batches
  for (let i = 0; i < requests.length; i += config.batchSize) {
    batches.push(requests.slice(i, i + config.batchSize));
  }

  // Execute batches sequentially with delay
  for (let i = 0; i < batches.length; i++) {
    const batch = batches[i];

    // Execute all requests in this batch in parallel
    await Promise.all(batch.map((req) => saveFn(req)));

    // Delay before next batch (except for last batch)
    if (i < batches.length - 1) {
      await new Promise((resolve) => setTimeout(resolve, config.delayMs));
    }
  }
}
