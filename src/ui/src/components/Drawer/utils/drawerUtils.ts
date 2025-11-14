/**
 * Drawer Utility Functions
 *
 * Helper functions for working with navigation data in the drawer.
 */

import { NavigationDto } from "../../../types";

/**
 * Find a navigation item by ID in a recursive tree structure
 *
 * @param items Array of navigation items to search
 * @param id ID to search for
 * @returns Found navigation item or undefined
 */
export const findNavigationById = (items: NavigationDto[] | undefined, id: number): NavigationDto | undefined => {
  if (!items) return undefined;

  for (const item of items) {
    if (item.id === id) {
      return item;
    }

    // Recursively search children
    if (item.items && item.items.length > 0) {
      const found = findNavigationById(item.items, id);
      if (found) return found;
    }
  }

  return undefined;
};

/**
 * Check if a navigation tree contains a specific path
 *
 * @param items Array of navigation items to search
 * @param pathname Path to search for (without leading slash)
 * @returns True if path is found in tree
 */
export const containsActivePath = (items: NavigationDto[], pathname: string): boolean => {
  const cleanPath = pathname.replace(/^\/+/, "");

  for (const item of items) {
    const itemPath = item.url?.replace(/^\/+/, "");

    if (cleanPath === itemPath) {
      return true;
    }

    // Recursively check children
    if (item.items && containsActivePath(item.items, pathname)) {
      return true;
    }
  }

  return false;
};

/**
 * Get the parent chain for a navigation item
 * Useful for breadcrumbs or showing current location
 *
 * @param items Array of navigation items to search
 * @param id ID of item to find parent chain for
 * @returns Array of navigation items from root to target (inclusive)
 */
export const getNavigationParentChain = (items: NavigationDto[] | undefined, id: number): NavigationDto[] => {
  if (!items) return [];

  const findChain = (
    currentItems: NavigationDto[],
    targetId: number,
    chain: NavigationDto[]
  ): NavigationDto[] | null => {
    for (const item of currentItems) {
      const newChain = [...chain, item];

      if (item.id === targetId) {
        return newChain;
      }

      if (item.items && item.items.length > 0) {
        const found = findChain(item.items, targetId, newChain);
        if (found) return found;
      }
    }

    return null;
  };

  return findChain(items, id, []) ?? [];
};

/**
 * Flatten navigation tree into a single-level array
 * Useful for searching or creating indexes
 *
 * @param items Array of navigation items to flatten
 * @returns Flattened array of all navigation items
 */
export const flattenNavigationTree = (items: NavigationDto[] | undefined): NavigationDto[] => {
  if (!items) return [];

  const flattened: NavigationDto[] = [];

  const flatten = (currentItems: NavigationDto[]) => {
    for (const item of currentItems) {
      flattened.push(item);

      if (item.items && item.items.length > 0) {
        flatten(item.items);
      }
    }
  };

  flatten(items);
  return flattened;
};

/**
 * Get all leaf nodes (items without children) from navigation tree
 *
 * @param items Array of navigation items
 * @returns Array of leaf navigation items
 */
export const getNavigationLeafNodes = (items: NavigationDto[] | undefined): NavigationDto[] => {
  if (!items) return [];

  const leaves: NavigationDto[] = [];

  const findLeaves = (currentItems: NavigationDto[]) => {
    for (const item of currentItems) {
      if (!item.items || item.items.length === 0) {
        leaves.push(item);
      } else {
        findLeaves(item.items);
      }
    }
  };

  findLeaves(items);
  return leaves;
};

/**
 * Calculate the maximum depth of a navigation tree
 *
 * @param items Array of navigation items
 * @returns Maximum depth (0 for empty, 1 for single level, etc.)
 */
export const getNavigationTreeDepth = (items: NavigationDto[] | undefined): number => {
  if (!items || items.length === 0) return 0;

  let maxDepth = 0;

  const calculateDepth = (currentItems: NavigationDto[], currentDepth: number) => {
    if (currentDepth > maxDepth) {
      maxDepth = currentDepth;
    }

    for (const item of currentItems) {
      if (item.items && item.items.length > 0) {
        calculateDepth(item.items, currentDepth + 1);
      }
    }
  };

  calculateDepth(items, 1);
  return maxDepth;
};

/**
 * Filter navigation tree by a predicate function
 * Maintains tree structure, removing branches that don't match
 *
 * @param items Array of navigation items to filter
 * @param predicate Filter function
 * @returns Filtered navigation tree
 */
export const filterNavigationTree = (
  items: NavigationDto[] | undefined,
  predicate: (item: NavigationDto) => boolean
): NavigationDto[] => {
  if (!items) return [];

  const filtered: NavigationDto[] = [];

  for (const item of items) {
    const matchesFilter = predicate(item);
    const filteredChildren = item.items ? filterNavigationTree(item.items, predicate) : [];

    // Include item if it matches OR has matching descendants
    if (matchesFilter || filteredChildren.length > 0) {
      filtered.push({
        ...item,
        items: filteredChildren
      });
    }
  }

  return filtered;
};

/**
 * Get all navigation items with a specific status
 *
 * @param items Array of navigation items to search
 * @param statusId Status ID to filter by
 * @returns Array of items with matching status
 */
export const getNavigationByStatus = (items: NavigationDto[] | undefined, statusId: number): NavigationDto[] => {
  const flattened = flattenNavigationTree(items);
  return flattened.filter((item) => item.statusId === statusId);
};

/**
 * Check if user has access to a navigation item based on roles
 *
 * @param item Navigation item to check
 * @param userRoles Array of user's roles
 * @returns True if user has required roles
 */
export const hasNavigationAccess = (item: NavigationDto, userRoles: string[]): boolean => {
  // If no roles required, everyone has access
  if (!item.requiredRoles || item.requiredRoles.length === 0) {
    return true;
  }

  // Check if user has any of the required roles
  return item.requiredRoles.some((role) => userRoles.includes(role));
};
