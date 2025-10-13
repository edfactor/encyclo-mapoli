import { useEffect, useMemo } from "react";
import { useGetMasterUpdateValidationQuery } from "../reduxstore/api/ValidationApi";
import { MasterUpdateCrossReferenceValidationResponse, CrossReferenceValidation } from "../types/validation/cross-reference-validation";

export interface ChecksumValidationConfig {
  /** The profit year to validate */
  profitYear: number;
  /** Whether to automatically fetch on mount and when profitYear changes */
  autoFetch?: boolean;
  /** Current values from PAY444 to compare against archived PAY443 values */
  currentValues?: {
    TotalProfitSharingBalance?: number;
    DistributionTotals?: number;
    ForfeitureTotals?: number;
    ContributionTotals?: number;
    EarningsTotals?: number;
    IncomingAllocations?: number;
    OutgoingAllocations?: number;
    NetAllocTransfer?: number;
  };
  /** Optional callback when validation data is loaded */
  onValidationLoaded?: (data: MasterUpdateCrossReferenceValidationResponse) => void;
  /** Optional callback when an error occurs */
  onError?: (error: string) => void;
}

/**
 * Enriches validation data from API (which has expectedValue but null currentValue)
 * with actual current values from PAY444 and performs client-side comparison.
 */
const enrichValidationWithCurrentValues = (
  apiValidation: CrossReferenceValidation,
  currentValues?: ChecksumValidationConfig["currentValues"]
): CrossReferenceValidation => {
  if (!currentValues) {
    // No current values provided, return API data as-is
    return apiValidation;
  }

  // Map field names to current values
  const currentValue =
    apiValidation.fieldName === "TotalProfitSharingBalance"
      ? currentValues.TotalProfitSharingBalance
      : apiValidation.fieldName === "DistributionTotals"
        ? currentValues.DistributionTotals
        : apiValidation.fieldName === "ForfeitureTotals"
          ? currentValues.ForfeitureTotals
          : apiValidation.fieldName === "ContributionTotals"
            ? currentValues.ContributionTotals
            : apiValidation.fieldName === "EarningsTotals"
              ? currentValues.EarningsTotals
              : apiValidation.fieldName === "IncomingAllocations"
                ? currentValues.IncomingAllocations
                : apiValidation.fieldName === "OutgoingAllocations"
                  ? currentValues.OutgoingAllocations
                  : apiValidation.fieldName === "NetAllocTransfer"
                    ? currentValues.NetAllocTransfer
                    : null;

  if (currentValue === null || currentValue === undefined) {
    // Current value not available for this field
    return apiValidation;
  }

  const expectedValue = apiValidation.expectedValue;
  if (expectedValue === null || expectedValue === undefined) {
    // No archived value to compare against
    return {
      ...apiValidation,
      currentValue,
      isValid: false,
      message: `No archived value found for comparison`
    };
  }

  // Perform comparison (allow small floating point tolerance)
  const variance = currentValue - expectedValue;
  const isValid = Math.abs(variance) < 0.01; // Tolerance for floating point precision

  return {
    ...apiValidation,
    currentValue,
    isValid,
    variance,
    message: isValid
      ? `${apiValidation.reportCode}.${apiValidation.fieldName} matches archived value`
      : `${apiValidation.reportCode}.${apiValidation.fieldName} does NOT match archived value`
  };
};

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
  const { profitYear, autoFetch = true, currentValues, onValidationLoaded, onError } = config;

  // Use RTK Query to fetch validation data
  const {
    data: apiValidationData,
    isLoading,
    error: rtkError,
    refetch
  } = useGetMasterUpdateValidationQuery(profitYear, {
    skip: !autoFetch || profitYear <= 0 // Skip if autoFetch is false or profitYear is invalid
  });

  // Enrich API validation data with current values and perform client-side comparison
  const validationData = useMemo(() => {
    if (!apiValidationData) return null;

    return {
      ...apiValidationData,
      validationGroups: apiValidationData.validationGroups.map((group) => ({
        ...group,
        validations: group.validations.map((v) => enrichValidationWithCurrentValues(v, currentValues)),
        // Recalculate group-level isValid based on enriched validations
        isValid: group.validations.every((v) => enrichValidationWithCurrentValues(v, currentValues).isValid)
      }))
    };
  }, [apiValidationData, currentValues]);

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
