/**
 * Barrel export for drawer utilities
 */

export {
  containsActivePath,
  filterNavigationTree,
  findNavigationById,
  flattenNavigationTree,
  getNavigationByStatus,
  getNavigationLeafNodes,
  getNavigationParentChain,
  getNavigationTreeDepth,
  hasNavigationAccess
} from "./drawerUtils";

export {
  createNavigationIdMap,
  getDrawerItemsForSection,
  getL0NavigationByTitle,
  getL0NavigationForRoute,
  getMenuBarItems,
  getNavigationAtLevel,
  getNavigationMaxDepth,
  getNavigationPath,
  getNavigationStats
} from "./navigationStructureUtils";

export type { NavigationStats } from "./navigationStructureUtils";
