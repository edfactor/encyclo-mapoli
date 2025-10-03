import { useEffect } from "react";
import { useSelector } from "react-redux";
import { useLazyGetProfitYearSelectorFrozenDataQuery } from "../reduxstore/api/ItOperationsApi";
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
  const frozenStates = useSelector((state: RootState) => state.frozen.profitYearSelectorData);
  const token = useSelector((state: RootState) => state.security.token);
  const [triggerFrozenStateSearch, { isLoading }] = useLazyGetProfitYearSelectorFrozenDataQuery();

  // Fetch frozen state data if not already loaded
  useEffect(() => {
    if (profitYear && token && !frozenStates && !isLoading) {
      triggerFrozenStateSearch({
        skip: 0,
        take: 100, // Get all frozen states
        sortBy: "createdDateTime",
        isSortDescending: true
      });
    }
  }, [profitYear, token, frozenStates, isLoading, triggerFrozenStateSearch]);

  if (!profitYear || !frozenStates?.results) {
    return false;
  }

  // Check if there's an ACTIVE frozen state for this profit year
  return frozenStates.results.some((state) => state.profitYear === profitYear && state.isActive);
};
