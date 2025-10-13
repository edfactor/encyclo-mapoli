import { useEffect } from "react";
import { useGetMasterUpdateValidationQuery } from "../reduxstore/api/ValidationApi";
import { MasterUpdateCrossReferenceValidationResponse } from "../types/validation/cross-reference-validation";

export interface ChecksumValidationConfig {
  /** The profit year to validate */
  profitYear: number;
  /** Whether to automatically fetch on mount and when profitYear changes */
  autoFetch?: boolean;
  /** Optional callback when validation data is loaded */
  onValidationLoaded?: (data: MasterUpdateCrossReferenceValidationResponse) => void;
  /** Optional callback when an error occurs */
  onError?: (error: string) => void;
}

/**
 * Generic hook to fetch checksum validation data for master updates using RTK Query.
 * This calls the comprehensive validation endpoint that includes all
 * cross-reference checks (contributions, earnings, forfeitures, distributions,
 * and ALLOC/PAID ALLOC transfers).
 *
 * @example
 * ```tsx
 * // Simple usage with auto-fetch
 * const { validationData, isLoading } = useChecksumValidation({
 *   profitYear: 2024,
 *   autoFetch: true
 * });
 *
 * // Manual control with callbacks
 * const { refetch, isLoading } = useChecksumValidation({
 *   profitYear: 2024,
 *   autoFetch: false,
 *   onValidationLoaded: (data) => {
 *     console.log('Validation loaded:', data);
 *   }
 * });
 *
 * // Get specific field validation
 * const validation = getFieldValidation('NetAllocTransfer');
 * ```
 */
export const useChecksumValidation = (config: ChecksumValidationConfig) => {
  const { profitYear, autoFetch = true, onValidationLoaded, onError } = config;

  // Use RTK Query to fetch validation data
  const {
    data: validationData,
    isLoading,
    error: rtkError,
    refetch
  } = useGetMasterUpdateValidationQuery(profitYear, {
    skip: !autoFetch || profitYear <= 0 // Skip if autoFetch is false or profitYear is invalid
  });

  // Convert RTK Query error to string
  const error = rtkError
    ? "error" in rtkError
      ? rtkError.error
      : "status" in rtkError
        ? `HTTP ${rtkError.status}`
        : "Unknown error"
    : null;

  // Call onValidationLoaded when data is successfully fetched
  useEffect(() => {
    if (validationData && onValidationLoaded) {
      onValidationLoaded(validationData);
    }
  }, [validationData, onValidationLoaded]);

  // Call onError when an error occurs
  useEffect(() => {
    if (error && onError) {
      onError(error);
    }
  }, [error, onError]);

  /**
   * Helper function to find a specific field validation by field name.
   * Searches across all validation groups.
   */
  const getFieldValidation = (fieldName: string) => {
    if (!validationData) return null;

    for (const group of validationData.validationGroups) {
      const validation = group.validations.find((v) => v.fieldName === fieldName);
      if (validation) return validation;
    }
    return null;
  };

  /**
   * Helper function to get all validations for a specific group by group name.
   */
  const getValidationGroup = (groupName: string) => {
    if (!validationData) return null;
    return validationData.validationGroups.find((g) => g.groupName === groupName) || null;
  };

  /**
   * Check if all validations are passing (no critical issues).
   */
  const isAllValid = () => {
    if (!validationData) return false;
    return validationData.criticalIssues.length === 0;
  };

  return {
    /** The complete validation response with all groups, or null if not loaded */
    validationData: validationData || null,
    /** Whether validation data is currently being fetched */
    isLoading,
    /** Error message if fetch failed, or null if no error */
    error,
    /** Manually trigger a validation fetch */
    refetch,
    /** Helper to find a specific field validation by name */
    getFieldValidation,
    /** Helper to get all validations for a specific group */
    getValidationGroup,
    /** Check if all validations pass */
    isAllValid
  };
};
