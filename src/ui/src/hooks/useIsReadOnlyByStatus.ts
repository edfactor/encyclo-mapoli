import { useSelector } from "react-redux";
import { RootState } from "../reduxstore/store";
import { NavigationDto } from "../types/navigation/navigation";

// Navigation status constants (from backend NavigationStatus.Constants)
const NavigationStatus = {
  NotStarted: 1,
  InProgress: 2,
  OnHold: 3,
  Complete: 4
} as const;

/**
 * Hook to determine if the current navigation page is read-only specifically due to page status
 * (not due to user role). Returns true if page status is NOT "In Progress".
 *
 * Use this to show status-specific help messages to users who could modify the page
 * if they changed the status to "In Progress".
 */
export const useIsReadOnlyByStatus = (): boolean => {
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

  // Check if read-only due to status (not role)
  const isReadOnlyByStatus =
    currentNavigation?.statusId !== undefined && currentNavigation?.statusId !== NavigationStatus.InProgress;

  return isReadOnlyByStatus;
};
