import { useEffect, useMemo } from "react";
import { useSelector } from "react-redux";
import { useLazyGetFrozenStateResponseQuery } from "../reduxstore/api/ItOperationsApi";
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
  const hasToken = !!useSelector((state: RootState) => state.security.token);
  const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");
  const [fetchActiveFrozenState] = useLazyGetFrozenStateResponseQuery();

  const currentNavigation = useMemo(() => {
    return getNavigationObjectBasedOnId(navigationList?.navigation, currentNavigationId);
  }, [navigationList, currentNavigationId]);

  // Check if this navigation requires frozen year
  const useFrozenYear = currentNavigation?.customSettings?.[NavigationCustomSettingsKeys.useFrozenYear] === true;

  // Fetch frozen state if this page requires it and we don't have it yet
  useEffect(() => {
    if (hasToken && useFrozenYear && !frozenStateResponse) {
      console.log('[useNavigationYear] Fetching frozen state...');
      fetchActiveFrozenState();
    }
  }, [hasToken, useFrozenYear, frozenStateResponse, fetchActiveFrozenState]);

  // Calculate the appropriate year
  const profitYear = useMemo(() => {
    const currentYear = new Date().getFullYear();
    
    // If we don't have navigation data yet, we need to wait to know if useFrozenYear is true
    if (!navigationList?.navigation) {
      console.log('[useNavigationYear] Navigation data not loaded yet, waiting...');
      return undefined;
    }

    // If navigation not found, we can't determine useFrozenYear, so use current year as safe fallback
    if (!currentNavigation) {
      console.log('[useNavigationYear] Navigation item not found for ID:', currentNavigationId, 'using current year as fallback');
      return currentYear;
    }

    console.log('[useNavigationYear] Current navigation:', currentNavigation.label, 'useFrozenYear:', useFrozenYear);

    // If this page requires frozen year, wait for frozen state to load
    if (useFrozenYear) {
      if (!frozenStateResponse) {
        console.log('[useNavigationYear] Waiting for frozen state to load...');
        return undefined;
      }
      console.log('[useNavigationYear] Using frozen year:', frozenStateResponse.profitYear);
      return frozenStateResponse.profitYear;
    }

    // Otherwise use current calendar year immediately
    console.log('[useNavigationYear] Using current calendar year:', currentYear);
    return currentYear;
  }, [navigationList, currentNavigation, currentNavigationId, useFrozenYear, frozenStateResponse]);

  return profitYear;
};

export default useNavigationYear;
