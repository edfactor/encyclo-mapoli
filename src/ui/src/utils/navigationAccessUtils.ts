import { NavigationDto } from "../types/navigation/navigation";

/**
 * Checks if a path is allowed based on navigation data
 */
export const isPathAllowedInNavigation = (path: string, navigationData: NavigationDto[]): boolean => {
  const allowedPaths = ["/", "/unauthorized", ""];
  if (allowedPaths.includes(path)) {
    return true;
  }

  if (!navigationData || !Array.isArray(navigationData) || navigationData.length === 0) {
    return false;
  }

  const cleanPath = path.replace(/^\/+|\/+$/g, "");
  const pathParts = cleanPath.split("/");
  const basePath = pathParts[0];

  const extractAllUrls = (items: NavigationDto[]): { url: string }[] => {
    const result: { url: string }[] = [];

    const extractUrlsRecursive = (navItems: NavigationDto[]) => {
      for (const item of navItems) {
        if (item.url) {
          result.push({
            url: item.url.replace(/^\/+|\/+$/g, "")
          });
        }

        if (item.items && item.items.length > 0) {
          extractUrlsRecursive(item.items);
        }
      }
    };

    extractUrlsRecursive(items);
    return result;
  };

  const allUrls = extractAllUrls(navigationData);

  const checkPathInNavItems = (items: NavigationDto[]): boolean => {
    const exactMatch = allUrls.find((item) => item.url === cleanPath);
    if (exactMatch) {
      return true;
    }

    const basePathMatch = allUrls.find((item) => {
      const itemBasePath = item.url.split("/")[0];
      return itemBasePath === basePath;
    });

    if (basePathMatch) {
      return true;
    }

    if (!items || items.length === 0) {
      return false;
    }

    for (const item of items) {
      if (item.url) {
        const cleanItemUrl = item.url.replace(/^\/+|\/+$/g, "");

        if (cleanPath === cleanItemUrl) {
          return true;
        }

        const itemUrlParts = cleanItemUrl.split("/");
        const itemBasePath = itemUrlParts[0];

        if (basePath === itemBasePath) {
          return true;
        }
      }

      if (item.items && item.items.length > 0) {
        const childMatch = checkPathInNavItems(item.items);
        if (childMatch) {
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
