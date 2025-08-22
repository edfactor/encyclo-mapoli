import { NavigationDto } from "../types/navigation/navigation";

/**
 * Checks if a path is allowed based on navigation data
 */
export const isPathAllowedInNavigation = (path: string, navigationData: NavigationDto[]): boolean => {
  const allowedPaths = ["/", "/unauthorized", ""];
  if (allowedPaths.includes(path)) {
    return true;
  }

  const pathParts = path.replace(/^\/+|\/+$/g, "").split("/");
  const basePath = pathParts[0];

  const checkPathInNavItems = (items: NavigationDto[]): boolean => {
    if (!items || items.length === 0) return false;

    for (const item of items) {
      if (!item.url) continue;

      const itemUrl = item.url.replace(/^\/+|\/+$/g, "").split("/")[0];

      if (basePath === itemUrl) {
        return true;
      }

      if (item.items && item.items.length > 0) {
        if (checkPathInNavItems(item.items)) {
          return true;
        }
      }
    }

    return false;
  };

  return checkPathInNavItems(navigationData);
};

/**
 * Creates params for redirecting to unauthorized page
 */
export const createUnauthorizedParams = (currentPath: string, reason: string = "navigation_restricted"): string => {
  const searchParams = new URLSearchParams({
    page: currentPath,
    reason: reason
  });
  return searchParams.toString();
};
