/**
 * Drawer Configuration
 *
 * 100% Dynamic configuration - NO hardcoded navigation items!
 *
 * The drawer can display ANY L0 navigation section from the API.
 * Configuration is created dynamically based on:
 * - L0 (Menu Bar): parentId === null
 * - L1+ (Drawer): parentId !== null
 */

import { NavigationDto } from "../../../reduxstore/types";

export interface DrawerConfig {
  /** Title of the L0 navigation item whose children should be displayed (e.g., "YEAR END") */
  rootNavigationTitle: string;

  /** How many levels to auto-expand (0 = none, 1 = first level, etc.) */
  autoExpandDepth: number;

  /** Whether to show status chips on navigation items */
  showStatus?: boolean;

  /** Custom filter function for items (optional) */
  itemFilter?: (item: NavigationDto) => boolean;
}

/**
 * Default configuration values
 * These are just defaults - not hardcoded navigation items
 * autoExpandDepth: 0 means groups are collapsed by default
 */
export const DEFAULT_CONFIG_VALUES = {
  autoExpandDepth: 0,
  showStatus: true
} as const;

/**
 * Create a drawer configuration for ANY L0 navigation title
 * Completely dynamic - works with any navigation structure from API
 *
 * @param rootNavigationTitle - The title of any L0 item (where parentId === null)
 * @param options - Optional overrides for default config values
 */
export const createDrawerConfig = (
  rootNavigationTitle: string,
  options?: Partial<Omit<DrawerConfig, "rootNavigationTitle">>
): DrawerConfig => {
  return {
    rootNavigationTitle,
    autoExpandDepth: options?.autoExpandDepth ?? DEFAULT_CONFIG_VALUES.autoExpandDepth,
    showStatus: options?.showStatus ?? DEFAULT_CONFIG_VALUES.showStatus,
    itemFilter: options?.itemFilter
  };
};

/**
 * Get default drawer configuration (Year End for backwards compatibility)
 * This is the ONLY place "YEAR END" appears - just as a default
 */
export const getDefaultDrawerConfig = (): DrawerConfig => {
  return createDrawerConfig("YEAR END");
};

/**
 * Dynamic drawer selection based on current route
 * Finds which L0 section contains the current route and creates config automatically
 *
 * @param _pathname - Current route path (reserved for future use)
 * @param l0NavigationItem - The L0 navigation item found for this route
 */
export const getDrawerConfigForRoute = (_pathname: string, l0NavigationItem?: NavigationDto): DrawerConfig => {
  // If we found the L0 item containing this route, create config for it
  if (l0NavigationItem?.title) {
    return createDrawerConfig(l0NavigationItem.title);
  }

  // Fallback to default
  return getDefaultDrawerConfig();
};

/**
 * Create drawer configs for ALL L0 navigation items dynamically
 * This allows the drawer to work with ANY navigation structure from the API
 *
 * @param navigationData - The full navigation response from API
 * @returns Map of L0 title to DrawerConfig
 */
export const createAllDrawerConfigs = (navigationData?: { navigation: NavigationDto[] }): Map<string, DrawerConfig> => {
  const configs = new Map<string, DrawerConfig>();

  if (!navigationData?.navigation) return configs;

  // Find all L0 items (parentId === null) and create configs
  navigationData.navigation
    .filter((item) => item.parentId === null)
    .forEach((l0Item) => {
      configs.set(l0Item.title, createDrawerConfig(l0Item.title));
    });

  return configs;
};
