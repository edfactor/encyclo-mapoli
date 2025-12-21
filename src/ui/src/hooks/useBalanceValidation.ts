import { useCallback, useEffect, useMemo, useState } from "react";
import { useLazyGetBalanceValidationQuery } from "reduxstore/api/ValidationApi";
import { CrossReferenceValidationGroup } from "../types/validation/cross-reference-validation";

/**
 * Custom hook to fetch balance validation data (ALLOC/PAID ALLOC transfers)
 * for a specific profit year. Provides validation results that can be used
 * to display validation icons and details in the UI.
 *
 * This hook wraps the RTK Query endpoint to provide:
 * - Automatic fetching when profitYear changes
 * - Graceful 404 handling (returns null data, no error)
 * - Manual refetch capability
 * - Backward-compatible return interface
 *
 * @example
 * ```tsx
 * const { validationData, isLoading, error, refetch } = useBalanceValidation(2024);
 *
 * if (validationData) {
 *   const allocValidation = validationData.validations.find(v => v.fieldName === 'NetAllocTransfer');
 *   // Display validation icon based on allocValidation.isValid
 * }
 * ```
 */
export const useBalanceValidation = (profitYear: number | null) => {
  const [triggerFetch, { data, isFetching, isError }] = useLazyGetBalanceValidationQuery();
  const [validationData, setValidationData] = useState<CrossReferenceValidationGroup | null>(null);
  const [error, setError] = useState<string | null>(null);

  // Determine if profitYear is valid for fetching
  const isValidYear = profitYear !== null && profitYear > 0;

  // Handle the fetch when profitYear changes
  useEffect(() => {
    if (isValidYear) {
      triggerFetch(profitYear, true) // preferCacheValue = true
        .unwrap()
        .then((result) => {
          setValidationData(result);
          setError(null);
        })
        .catch((err: unknown) => {
          // Handle 404 gracefully - no validation data available for this year
          if (err && typeof err === "object" && "status" in err && err.status === 404) {
            setValidationData(null);
            setError(null);
            return;
          }

          // Handle other errors
          console.error("Error fetching balance validation:", err);
          if (err && typeof err === "object" && "data" in err) {
            const errorData = err as { data?: { title?: string; message?: string } };
            setError(errorData.data?.title || errorData.data?.message || "Failed to fetch balance validation");
          } else if (err instanceof Error) {
            setError(err.message);
          } else {
            setError("Unknown error");
          }
          setValidationData(null);
        });
    } else {
      // Clear state when profitYear is invalid
      setValidationData(null);
      setError(null);
    }
  }, [profitYear, isValidYear, triggerFetch]);

  // Update from RTK Query data when available (for cache hits)
  useEffect(() => {
    if (data !== undefined && !isError) {
      setValidationData(data);
    }
  }, [data, isError]);

  // Manual refetch function
  const refetch = useCallback(() => {
    if (isValidYear) {
      triggerFetch(profitYear, false) // preferCacheValue = false to force refetch
        .unwrap()
        .then((result) => {
          setValidationData(result);
          setError(null);
        })
        .catch((err: unknown) => {
          if (err && typeof err === "object" && "status" in err && err.status === 404) {
            setValidationData(null);
            setError(null);
            return;
          }
          console.error("Error fetching balance validation:", err);
          setError(err instanceof Error ? err.message : "Unknown error");
          setValidationData(null);
        });
    }
  }, [profitYear, isValidYear, triggerFetch]);

  // Memoize return object for stable reference
  return useMemo(
    () => ({
      /** The validation data returned from the API, or null if not yet loaded or no data available */
      validationData,
      /** Whether the validation data is currently being fetched */
      isLoading: isFetching,
      /** Error message if the fetch failed, or null if no error */
      error,
      /** Function to manually refetch the validation data */
      refetch
    }),
    [validationData, isFetching, error, refetch]
  );
};
