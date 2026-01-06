import { useMemo } from "react";
import { useSelector } from "react-redux";
import { RootState } from "../reduxstore/store";
import { NavigationCustomSettingsKeys, NavigationDto } from "../types/navigation/navigation";

/**
 * Helper function to find a navigation item by ID in the navigation tree
 */
const getNavigationObjectBasedOnId = (navigationArray?: NavigationDto[], id?: number): NavigationDto | undefined => {
  if (navigationArray) {
    for (const item of navigationArray) {
      if (item.id === id) {
        return item;
      }
      if (item.items && item.items.length > 0) {
        const found = getNavigationObjectBasedOnId(item.items, id);
        if (found) {
          return found;
        }
      }
    }
  }
  return undefined;
};

/**
 * Hook to provide the appropriate profit year based on current navigation's settings
 * 
 * - If the current navigation has `useFrozenYear: true` in customSettings,
 *   returns the frozen profit year from Fiscal Close flow (waits for it to load)
 * - Otherwise, returns the current calendar year immediately
 * 
 * Returns undefined while waiting for required data to load (navigation or frozen state)
 * 
 * @returns Profit year number or undefined if still loading
 */
const useNavigationYear = (): number | undefined => {
  const navigationList = useSelector((state: RootState) => state.navigation.navigationData);
  const frozenStateResponse = useSelector((state: RootState) => state.frozen.frozenStateResponseData);
  const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");

  const currentNavigation = useMemo(() => {
    return getNavigationObjectBasedOnId(navigationList?.navigation, currentNavigationId);
  }, [navigationList, currentNavigationId]);

  // Check if this navigation requires frozen year
  const useFrozenYear = currentNavigation?.customSettings?.[NavigationCustomSettingsKeys.useFrozenYear] === true;

  // Calculate the appropriate year
  const profitYear = useMemo(() => {
    // If we don't have navigation data yet, wait
    if (!navigationList?.navigation) {
      return undefined;
    }

    // If this page requires frozen year, wait for frozen state to load
    if (useFrozenYear) {
      if (!frozenStateResponse) {
        return undefined;
      }
      return frozenStateResponse.profitYear;
    }

    // Otherwise use current calendar year immediately
    return new Date().getFullYear();
  }, [navigationList, useFrozenYear, frozenStateResponse]);

  return profitYear;
};

export default useNavigationYear;
