import { useSelector } from "react-redux";
import { RootState } from "../reduxstore/store";
import { NavigationDto } from "../types/navigation/navigation";

/**
 * Hook to determine if the current navigation page is read-only
 * Returns true if the current user has read-only access to the current page
 */
export const useReadOnlyNavigation = (): boolean => {
  const navigationList = useSelector((state: RootState) => state.navigation.navigationData);
  const currentNavigationId = parseInt(localStorage.getItem("navigationId") ?? "");

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

  const currentNavigation = getNavigationObjectBasedOnId(navigationList?.navigation, currentNavigationId);
  return currentNavigation?.isReadOnly ?? false;
};
