import { useCallback, useState } from "react";
import { useLazyUpdateBeneficiaryQuery } from "reduxstore/api/BeneficiariesApi";
import { BeneficiaryDto } from "../../types";

/**
 * Result of a percentage validation and update attempt
 */
interface ValidateAndUpdateResult {
  success: boolean;
  previousValue?: number;
  error?: string;
}

/**
 * Return type for useBeneficiaryPercentageUpdate hook
 */
interface UseBeneficiaryPercentageUpdateReturn {
  validateAndUpdate: (
    id: number,
    newPercentage: number,
    currentList: BeneficiaryDto[]
  ) => Promise<ValidateAndUpdateResult>;
  isUpdating: boolean;
}

/**
 * useBeneficiaryPercentageUpdate - Validates and updates beneficiary percentage allocation
 *
 * This hook encapsulates the logic for validating and updating beneficiary percentages.
 * Key validation rule: The sum of all beneficiary percentages must equal 100%.
 *
 * The hook:
 * - Validates that the new percentage won't cause the total to exceed 100%
 * - Returns the previous value so UI can restore it on failure
 * - Handles API call to update the percentage
 * - Provides loading state
 *
 * The component using this hook should:
 * - Display error messages from the result
 * - Restore the field value on failure
 * - Trigger a refresh of the grid on success (via parent callback)
 *
 * @param onUpdateSuccess - Optional callback to invoke after successful update
 * @returns Object with validateAndUpdate function and loading state
 *
 * @example
 * const percentageUpdate = useBeneficiaryPercentageUpdate(() => {
 *   relationships.refresh();
 * });
 *
 * // In blur handler:
 * const result = await percentageUpdate.validateAndUpdate(beneficiaryId, 75, beneficiaryList);
 * if (!result.success) {
 *   inputField.value = result.previousValue;
 *   showError(result.error);
 * }
 */
export const useBeneficiaryPercentageUpdate = (
  onUpdateSuccess?: () => void
): UseBeneficiaryPercentageUpdateReturn => {
  const [triggerUpdate] = useLazyUpdateBeneficiaryQuery();
  const [isUpdating, setIsUpdating] = useState(false);

  /**
   * Validates percentage and updates via API if valid
   *
   * @param id - Beneficiary ID to update
   * @param newPercentage - New percentage value
   * @param currentList - Current list of all beneficiaries (needed for sum validation)
   * @returns Result with success flag, error message, and previous value for restoration
   */
  const validateAndUpdate = useCallback(
    async (
      id: number,
      newPercentage: number,
      currentList: BeneficiaryDto[]
    ): Promise<ValidateAndUpdateResult> => {
      // Calculate what the sum would be with the new percentage
      let sum = 0;
      let previousValue = 0;

      for (const item of currentList) {
        if (item.id === id) {
          previousValue = item.percent;
          sum += newPercentage;
        } else {
          sum += item.percent;
        }
      }

      // Validation: Sum must not exceed 100%
      if (sum > 100) {
        return {
          success: false,
          previousValue,
          error: `Total percentage would be ${sum}%. Beneficiary percentages must sum to 100% or less.`
        };
      }

      // Valid - proceed with API update
      setIsUpdating(true);

      try {
        await triggerUpdate({ id, percentage: newPercentage }, false).unwrap();

        // Call success callback if provided (typically to refresh data)
        onUpdateSuccess?.();

        return { success: true };
      } catch (error) {
        console.error("Failed to update beneficiary percentage:", error);
        return {
          success: false,
          previousValue,
          error: "Failed to update percentage. Please try again."
        };
      } finally {
        setIsUpdating(false);
      }
    },
    [triggerUpdate, onUpdateSuccess]
  );

  return {
    validateAndUpdate,
    isUpdating
  };
};
