import { useSelector } from "react-redux";
import { RootState } from "../reduxstore/store";

/**
 * Hook to check if a given profit year is frozen (has frozen demographics)
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

  if (!profitYear || !frozenStates?.results) {
    return false;
  }

  // Check if there's a frozen state for this profit year
  return frozenStates.results.some((state) => state.profitYear === profitYear);
};
