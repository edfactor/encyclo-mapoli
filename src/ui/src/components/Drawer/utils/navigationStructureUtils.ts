/**
 * Navigation Structure Utilities
 *
 * Helper functions for working with multi-dimensional navigation structure.
 *
 * Structure from API (nav.json):
 * - L0 (Menu Bar): Items where parentId === null
 * - L1+ (Drawer): Items where parentId !== null, nested via items[] array
 *
 * Example:
 * {
 *   parentId: null,        // L0 - Goes in menu bar
 *   title: "YEAR END",
 *   items: [               // L1 - First level in drawer
 *     {
 *       parentId: 6,
 *       title: "December Activities",
 *       items: [           // L2 - Second level in drawer
 *         {
 *           parentId: 9,
 *           title: "Clean up Reports",
 *           items: []      // L3 - Third level in drawer, etc.
 *         }
 *       ]
 *     }
 *   ]
 * }
 */

import { NavigationDto, NavigationResponseDto } from "../../../reduxstore/types";

/**
 * Get all L0 navigation items (menu bar items)
 * These are items where parentId === null
 */
export const getMenuBarItems = (navigationData: NavigationResponseDto | undefined): NavigationDto[] => {
  if (!navigationData?.navigation) return [];

  return navigationData.navigation
    .filter((item) => item.parentId === null)
    .sort((a, b) => a.orderNumber - b.orderNumber);
};

/**
 * Get drawer items for a specific L0 navigation title
 * Returns the children (L1+) of the specified L0 item
 */
export const getDrawerItemsForSection = (
  navigationData: NavigationResponseDto | undefined,
  l0Title: string
): NavigationDto[] => {
  if (!navigationData?.navigation) return [];

  const l0Item = navigationData.navigation.find((item) => item.parentId === null && item.title === l0Title);

  return l0Item?.items ?? [];
};

/**
 * Get L0 navigation item by title
 */
export const getL0NavigationByTitle = (
  navigationData: NavigationResponseDto | undefined,
  title: string
): NavigationDto | undefined => {
  if (!navigationData?.navigation) return undefined;

  return navigationData.navigation.find((item) => item.parentId === null && item.title === title);
};

/**
 * Get L0 navigation item that contains a specific route
 * Useful for determining which menu bar item should be active
 */
export const getL0NavigationForRoute = (
  navigationData: NavigationResponseDto | undefined,
  pathname: string
): NavigationDto | undefined => {
  if (!navigationData?.navigation) return undefined;

  const cleanPath = pathname.replace(/^\/+/, "");

  // Search each L0 item's descendants for the matching route
  for (const l0Item of navigationData.navigation.filter((item) => item.parentId === null)) {
    if (containsRoute(l0Item, cleanPath)) {
      return l0Item;
    }
  }

  return undefined;
};

/**
 * Check if a navigation item or its descendants contain a specific route
 */
const containsRoute = (item: NavigationDto, route: string): boolean => {
  const itemRoute = item.url?.replace(/^\/+/, "");

  // Check if this item matches
  if (itemRoute === route) {
    return true;
  }

  // Recursively check children
  if (item.items && item.items.length > 0) {
    return item.items.some((child) => containsRoute(child, route));
  }

  return false;
};

/**
 * Get the full navigation path (breadcrumb) for a specific navigation ID
 * Returns array of navigation items from L0 down to the target
 */
export const getNavigationPath = (
  navigationData: NavigationResponseDto | undefined,
  targetId: number
): NavigationDto[] => {
  if (!navigationData?.navigation) return [];

  // Search from each L0 item
  for (const l0Item of navigationData.navigation.filter((item) => item.parentId === null)) {
    const path = findPathToId(l0Item, targetId, [l0Item]);
    if (path) {
      return path;
    }
  }

  return [];
};

/**
 * Recursively find path to a specific ID
 */
const findPathToId = (
  current: NavigationDto,
  targetId: number,
  currentPath: NavigationDto[]
): NavigationDto[] | null => {
  if (current.id === targetId) {
    return currentPath;
  }

  if (current.items && current.items.length > 0) {
    for (const child of current.items) {
      const path = findPathToId(child, targetId, [...currentPath, child]);
      if (path) {
        return path;
      }
    }
  }

  return null;
};

/**
 * Get all navigation items at a specific level
 * @param level 0 = L0 (parentId === null), 1 = direct children of L0, etc.
 */
export const getNavigationAtLevel = (
  navigationData: NavigationResponseDto | undefined,
  level: number
): NavigationDto[] => {
  if (!navigationData?.navigation) return [];
  if (level === 0) return getMenuBarItems(navigationData);

  const result: NavigationDto[] = [];

  const collectAtLevel = (items: NavigationDto[], currentLevel: number) => {
    if (currentLevel === level) {
      result.push(...items);
    } else if (currentLevel < level) {
      for (const item of items) {
        if (item.items && item.items.length > 0) {
          collectAtLevel(item.items, currentLevel + 1);
        }
      }
    }
  };

  // Start from L0 items
  for (const l0Item of getMenuBarItems(navigationData)) {
    if (l0Item.items && l0Item.items.length > 0) {
      collectAtLevel(l0Item.items, 1);
    }
  }

  return result;
};

/**
 * Get the maximum depth of the navigation tree
 */
export const getNavigationMaxDepth = (navigationData: NavigationResponseDto | undefined): number => {
  if (!navigationData?.navigation) return 0;

  const getDepth = (items: NavigationDto[], currentDepth: number): number => {
    if (items.length === 0) return currentDepth;

    let maxDepth = currentDepth;

    for (const item of items) {
      if (item.items && item.items.length > 0) {
        const childDepth = getDepth(item.items, currentDepth + 1);
        maxDepth = Math.max(maxDepth, childDepth);
      }
    }

    return maxDepth;
  };

  return getDepth(getMenuBarItems(navigationData), 0);
};

/**
 * Create a flat map of all navigation items by ID for quick lookup
 */
export const createNavigationIdMap = (
  navigationData: NavigationResponseDto | undefined
): Map<number, NavigationDto> => {
  const map = new Map<number, NavigationDto>();

  if (!navigationData?.navigation) return map;

  const addToMap = (items: NavigationDto[]) => {
    for (const item of items) {
      map.set(item.id, item);
      if (item.items && item.items.length > 0) {
        addToMap(item.items);
      }
    }
  };

  addToMap(navigationData.navigation);
  return map;
};

/**
 * Statistics about navigation structure (for debugging/monitoring)
 */
export interface NavigationStats {
  totalItems: number;
  l0Items: number;
  maxDepth: number;
  itemsPerL0: Record<string, number>;
}

export const getNavigationStats = (navigationData: NavigationResponseDto | undefined): NavigationStats => {
  if (!navigationData?.navigation) {
    return {
      totalItems: 0,
      l0Items: 0,
      maxDepth: 0,
      itemsPerL0: {}
    };
  }

  const l0Items = getMenuBarItems(navigationData);
  const idMap = createNavigationIdMap(navigationData);

  const itemsPerL0: Record<string, number> = {};
  for (const l0Item of l0Items) {
    const count = countDescendants(l0Item);
    itemsPerL0[l0Item.title] = count;
  }

  return {
    totalItems: idMap.size,
    l0Items: l0Items.length,
    maxDepth: getNavigationMaxDepth(navigationData),
    itemsPerL0
  };
};

const countDescendants = (item: NavigationDto): number => {
  if (!item.items || item.items.length === 0) return 0;

  let count = item.items.length;
  for (const child of item.items) {
    count += countDescendants(child);
  }

  return count;
};

/**
 * Get the first navigable route (URL) from an L0 section
 * Recursively searches through children to find the first item with a URL
 */
export const getFirstNavigableRoute = (
  navigationData: NavigationResponseDto | undefined,
  l0Title: string
): string | undefined => {
  if (!navigationData?.navigation) return undefined;

  const l0Item = navigationData.navigation.find((item) => item.parentId === null && item.title === l0Title);

  if (!l0Item) return undefined;

  // Recursively find the first item with a URL
  const findFirstUrl = (item: NavigationDto): string | undefined => {
    if (item.url) return item.url;

    if (item.items && item.items.length > 0) {
      for (const child of item.items) {
        const url = findFirstUrl(child);
        if (url) return url;
      }
    }

    return undefined;
  };

  return findFirstUrl(l0Item);
};
