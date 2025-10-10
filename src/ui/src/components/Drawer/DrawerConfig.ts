/**
 * Drawer Configuration
 *
 * Defines which navigation sections can be displayed in the drawer.
 * This replaces the hardcoded "YEAR END" lookup with a flexible configuration system.
 */

import { NavigationDto } from "../../../reduxstore/types";

// Navigation menu ID constants (from backend database)
export const NAVIGATION_IDS = {
  YEAR_END: 55,
  REPORTS: 60, // Example - adjust based on actual IDs
  BENEFICIARIES: 45, // Example - adjust based on actual IDs
  DISTRIBUTIONS: 50 // Example - adjust based on actual IDs
} as const;

export interface DrawerConfig {
  /** Navigation item ID whose children should be displayed in drawer */
  rootNavigationId: number;

  /** Title to display in drawer header */
  title: string;

  /** How many levels to auto-expand (0 = none, 1 = first level, etc.) */
  autoExpandDepth: number;

  /** Whether to show status chips on navigation items */
  showStatus?: boolean;

  /** Custom filter function for items (optional) */
  itemFilter?: (item: NavigationDto) => boolean;
}

/**
 * Predefined drawer configurations
 * Add new configurations here as needed
 */
export const DRAWER_CONFIGS: Record<string, DrawerConfig> = {
  YEAR_END: {
    rootNavigationId: NAVIGATION_IDS.YEAR_END,
    title: "Year End",
    autoExpandDepth: 1,
    showStatus: true
  },

  // Example: Reports drawer configuration
  REPORTS: {
    rootNavigationId: NAVIGATION_IDS.REPORTS,
    title: "Reports",
    autoExpandDepth: 0,
    showStatus: false
  },

  // Example: Beneficiaries drawer configuration
  BENEFICIARIES: {
    rootNavigationId: NAVIGATION_IDS.BENEFICIARIES,
    title: "Beneficiaries",
    autoExpandDepth: 1,
    showStatus: true
  }

  // Add more drawer configurations as needed...
};

/**
 * Get drawer configuration by name
 */
export const getDrawerConfig = (configName: string): DrawerConfig | undefined => {
  return DRAWER_CONFIGS[configName];
};

/**
 * Get default drawer configuration (Year End for backwards compatibility)
 */
export const getDefaultDrawerConfig = (): DrawerConfig => {
  return DRAWER_CONFIGS.YEAR_END;
};

/**
 * Example: Dynamic drawer selection based on current route
 * This could be used to automatically switch drawer content based on which section the user is in
 */
export const getDrawerConfigForRoute = (pathname: string): DrawerConfig => {
  // Remove leading slash and get first path segment
  const firstSegment = pathname.replace(/^\//, "").split("/")[0];

  // Map route segments to drawer configs
  const routeConfigMap: Record<string, string> = {
    "year-end": "YEAR_END",
    reports: "REPORTS",
    beneficiaries: "BENEFICIARIES",
    distributions: "DISTRIBUTIONS"
  };

  const configName = routeConfigMap[firstSegment];
  return configName ? DRAWER_CONFIGS[configName] : getDefaultDrawerConfig();
};
