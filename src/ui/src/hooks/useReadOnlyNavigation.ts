import { useSelector } from "react-redux";
import { RootState } from "../reduxstore/store";
import { NavigationDto } from "../types/navigation/navigation";

// Navigation status constants (from backend NavigationStatus.Constants)
export const NavigationStatus = {
  NotStarted: 1,
  InProgress: 2,
  OnHold: 3,
  Complete: 4
} as const;

/**
 * Hook to determine if the current navigation page is read-only
 * Returns true if:
 * - The current user has read-only role access (ITDEVOPS, AUDITOR), OR
 * - The page status is NOT "In Progress" (preventing modifications to completed/not-started pages)
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

  // Read-only if user role restricts access
  const isReadOnlyByRole = currentNavigation?.isReadOnly ?? false;

  // Read-only if page status is not "In Progress"
  // This prevents modifications to completed reports without first reverting to In Progress
  const isReadOnlyByStatus =
    currentNavigation?.statusId !== undefined && currentNavigation?.statusId !== NavigationStatus.InProgress;

  return isReadOnlyByRole || isReadOnlyByStatus;
};
