/**
 * Pure utility functions for percentage allocation validation
 */

interface PercentageSumResult {
  sum: number;
  previousValue: number;
}

interface PercentageValidationResult {
  sum: number;
  valid: boolean;
  error?: string;
}

/**
 * Calculates the total percentage sum when one item's percentage changes
 *
 * Iterates through all items, replacing the target item's percentage with the new value,
 * and returns both the sum and the previous value of the changed item.
 *
 * @example
 * const items = [
 *   { id: 1, percent: 50 },
 *   { id: 2, percent: 50 }
 * ];
 * calculatePercentageSum(items, 1, 60) => { sum: 110, previousValue: 50 }
 *
 * @param items - Array of items with id and percent properties
 * @param updatedId - ID of the item being updated
 * @param newPercentage - New percentage value for the updated item
 * @returns Object with total sum and previous value of updated item
 */
export function calculatePercentageSum(
  items: { id: number; percent: number }[],
  updatedId: number,
  newPercentage: number
): PercentageSumResult {
  let sum = 0;
  let previousValue = 0;

  for (const item of items) {
    if (item.id === updatedId) {
      previousValue = item.percent;
      sum += newPercentage;
    } else {
      sum += item.percent;
    }
  }

  return { sum, previousValue };
}

/**
 * Validates that a percentage allocation does not exceed 100%
 *
 * Used to enforce the constraint that beneficiary percentages must not sum to more than 100%.
 *
 * @example
 * validatePercentageAllocation(100) => { sum: 100, valid: true }
 * validatePercentageAllocation(125) => {
 *   sum: 125,
 *   valid: false,
 *   error: "Total percentage would be 125%. Beneficiary percentages must sum to 100% or less."
 * }
 *
 * @param sum - Total percentage to validate
 * @returns Object with validation result and optional error message
 */
export function validatePercentageAllocation(
  sum: number
): PercentageValidationResult {
  return {
    sum,
    valid: sum <= 100,
    error:
      sum > 100
        ? `Total percentage would be ${sum}%. Beneficiary percentages must sum to 100% or less.`
        : undefined
  };
}
