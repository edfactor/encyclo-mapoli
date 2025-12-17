import { useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetFrozenStateResponseQuery } from "../reduxstore/api/ItOperationsApi";
import { RootState } from "../reduxstore/store";

/**
 * Hook to check if a given profit year is frozen (has frozen demographics)
 *
 * Automatically fetches frozen state data if not already loaded in Redux.
 * This ensures the frozen warning displays correctly on all pages.
 *
 * @param profitYear - The profit year to check. If not provided, returns false.
 * @returns boolean - True if the profit year has frozen demographics, false otherwise
 *
 * @example
 * ```tsx
 * const isFrozen = useIsProfitYearFrozen(2024);
 * ```
 */
export const useIsProfitYearFrozen = (profitYear?: number): boolean => {
  const activeFrozenState = useSelector((state: RootState) => state.frozen.frozenStateResponseData);
  const token = useSelector((state: RootState) => state.security.token);
  const [triggerFetchActiveFrozenState, { isLoading }] = useLazyGetFrozenStateResponseQuery();

  // Fetch frozen state data if not already loaded
  useEffect(() => {
    if (profitYear && token && !activeFrozenState && !isLoading) {
      triggerFetchActiveFrozenState();
    }
  }, [profitYear, token, activeFrozenState, isLoading, triggerFetchActiveFrozenState]);

  if (!profitYear || !activeFrozenState) {
    return false;
  }

  // Check if the active frozen state corresponds to this profit year
  return activeFrozenState.profitYear === profitYear && !!activeFrozenState.isActive;
};
