import { useEffect, useState } from "react";
import { CrossReferenceValidationGroup } from "../types/validation/cross-reference-validation";

/**
 * Custom hook to fetch balance validation data (ALLOC/PAID ALLOC transfers)
 * for a specific profit year. Provides validation results that can be used
 * to display validation icons and details in the UI.
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
  const [validationData, setValidationData] = useState<CrossReferenceValidationGroup | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const fetchValidation = async (year: number) => {
    setIsLoading(true);
    setError(null);

    try {
      const response = await fetch(`/api/balance-validation/alloc-transfers/${year}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json"
        }
      });

      if (!response.ok) {
        if (response.status === 404) {
          // No validation data available for this year - this is OK
          setValidationData(null);
          setError(null);
          return;
        }
        throw new Error(`Failed to fetch balance validation: ${response.statusText}`);
      }

      const data: CrossReferenceValidationGroup = await response.json();
      setValidationData(data);
      setError(null);
    } catch (err) {
      console.error("Error fetching balance validation:", err);
      setError(err instanceof Error ? err.message : "Unknown error");
      setValidationData(null);
    } finally {
      setIsLoading(false);
    }
  };

  // Auto-fetch when profitYear changes
  useEffect(() => {
    if (profitYear !== null && profitYear > 0) {
      fetchValidation(profitYear);
    } else {
      setValidationData(null);
      setError(null);
    }
  }, [profitYear]);

  // Manual refetch function
  const refetch = () => {
    if (profitYear !== null && profitYear > 0) {
      fetchValidation(profitYear);
    }
  };

  return {
    /** The validation data returned from the API, or null if not yet loaded or no data available */
    validationData,
    /** Whether the validation data is currently being fetched */
    isLoading,
    /** Error message if the fetch failed, or null if no error */
    error,
    /** Function to manually refetch the validation data */
    refetch
  };
};
