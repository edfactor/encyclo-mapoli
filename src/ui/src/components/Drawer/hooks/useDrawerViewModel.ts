/**
 * Drawer ViewModel Hook
 *
 * Encapsulates all business logic, state management, and side effects for the drawer.
 * View components consume this hook to get state and actions without containing logic.
 *
 * This follows the MVVM pattern adapted for React:
 * - Model: NavigationDto, DrawerConfig (data structures)
 * - ViewModel: This hook (business logic and state)
 * - View: PSDrawer component (pure presentation)
 */

import { useCallback, useEffect, useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import { clearActiveSubMenu, closeDrawer, openDrawer, setActiveSubMenu } from "../../../reduxstore/slices/generalSlice";
import { RootState } from "../../../reduxstore/store";
import { NavigationDto, NavigationResponseDto } from "../../../types";
import { DrawerConfig } from "../models";
import { containsActivePath } from "../utils";

export interface DrawerViewModel {
  // State (what the view needs to render)
  isOpen: boolean;
  activeSubmenu: string | null;
  drawerItems: NavigationDto[];
  currentPath: string;
  drawerTitle: string;

  // Actions (what the view can do)
  toggleDrawer: () => void;
  selectMenuItem: (item: NavigationDto) => void;
  navigateToItem: (item: NavigationDto) => void;
  goBackToMainMenu: () => void;

  // Computed values (derived from state)
  activeTopLevelItem: NavigationDto | null;
  visibleItems: NavigationDto[];
  isInSubmenuView: boolean;

  // Helper functions (for view logic)
  isItemActive: (item: NavigationDto) => boolean;
  hasActiveChild: (item: NavigationDto) => boolean;
  shouldAutoExpand: (item: NavigationDto, level: number) => boolean;
}

/**
 * Custom hook that provides all drawer logic and state
 *
 * @param navigationData - Navigation data from API
 * @param config - Drawer configuration (which nav section to display)
 * @returns ViewModel with state and actions
 */
export const useDrawerViewModel = (
  navigationData: NavigationResponseDto | undefined,
  config: DrawerConfig
): DrawerViewModel => {
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const location = useLocation();

  // ====================
  // State from Redux
  // ====================
  const isOpen = useSelector((state: RootState) => state.general.isDrawerOpen);
  const activeSubmenu = useSelector((state: RootState) => state.general.activeSubmenu);

  // ====================
  // Computed Values (Memoized)
  // ====================

  /**
   * Find the root navigation item for this drawer
   * Logic: Find L0 item (parentId === null) matching the config title
   */
  const drawerRootItem = useMemo(() => {
    if (!navigationData?.navigation) return undefined;

    // Find L0 navigation item by title (items where parentId === null)
    return navigationData.navigation.find(
      (item) => item.parentId === null && item.title === config.rootNavigationTitle
    );
  }, [navigationData, config.rootNavigationTitle]);

  /**
   * Get the direct children of the root item
   * These become the top-level items in the drawer
   */
  const drawerItems = useMemo(() => {
    const items = drawerRootItem?.items ?? [];

    // Apply custom filter if provided in config
    if (config.itemFilter) {
      return items.filter(config.itemFilter);
    }

    // Default: show navigable items only
    return items.filter((item) => item.isNavigable ?? true);
  }, [drawerRootItem, config.itemFilter]);

  /**
   * Get the currently selected top-level item (when in submenu view)
   */
  const activeTopLevelItem = useMemo(
    () => (activeSubmenu ? drawerItems.find((item) => item.title === activeSubmenu) : null),
    [activeSubmenu, drawerItems]
  );

  /**
   * Get items to display in current view
   * - Main view: drawerItems (top-level items)
   * - Submenu view: children of activeTopLevelItem
   */
  const visibleItems = useMemo(() => activeTopLevelItem?.items ?? drawerItems, [activeTopLevelItem, drawerItems]);

  /**
   * Clean current path for comparison
   */
  const currentPath = useMemo(() => location.pathname.replace(/^\/+/, ""), [location.pathname]);

  /**
   * Are we in submenu view? (showing children of a top-level item)
   */
  const isInSubmenuView = activeSubmenu !== null;

  // ====================
  // Actions
  // ====================

  /**
   * Toggle drawer open/closed
   * When closing, also clear active submenu
   */
  const toggleDrawer = useCallback(() => {
    if (isOpen) {
      dispatch(closeDrawer());
      dispatch(clearActiveSubMenu());
    } else {
      dispatch(openDrawer());
    }
  }, [isOpen, dispatch]);

  /**
   * Select a top-level menu item (drill into it)
   */
  const selectMenuItem = useCallback(
    (item: NavigationDto) => {
      if (item.disabled) return;

      // Only set active submenu if item has children
      if (item.items && item.items.length > 0) {
        dispatch(setActiveSubMenu(item.title));
      }

      // If item also has a route, navigate to it
      if (item.url) {
        const absolutePath = item.url.startsWith("/") ? item.url : `/${item.url}`;

        // Store navigation ID for read-only checks, etc.
        if (item.id) {
          localStorage.setItem("navigationId", item.id.toString());
        }

        navigate(absolutePath);
      }
    },
    [dispatch, navigate]
  );

  /**
   * Navigate to a specific navigation item
   * Used when clicking leaf nodes or items with routes
   */
  const navigateToItem = useCallback(
    (item: NavigationDto) => {
      if (item.disabled || !item.url) return;

      const absolutePath = item.url.startsWith("/") ? item.url : `/${item.url}`;

      // Store navigation ID
      if (item.id) {
        localStorage.setItem("navigationId", item.id.toString());
      }

      navigate(absolutePath);

      console.log(`Navigating to ${absolutePath} (Navigation ID: ${item.id})`);
    },
    [navigate]
  );

  /**
   * Go back to main menu view from submenu view
   */
  const goBackToMainMenu = useCallback(() => {
    dispatch(clearActiveSubMenu());
  }, [dispatch]);

  // ====================
  // Helper Functions
  // ====================

  /**
   * Check if a navigation item's route is currently active
   */
  const isItemActive = useCallback(
    (item: NavigationDto): boolean => {
      const itemPath = item.url?.replace(/^\/+/, "");
      return currentPath === itemPath;
    },
    [currentPath]
  );

  /**
   * Check if any child of this item contains the active route
   * Used for highlighting parent items when child is active
   */
  const hasActiveChild = useCallback(
    (item: NavigationDto): boolean => {
      if (!item.items || item.items.length === 0) return false;
      return containsActivePath(item.items, location.pathname);
    },
    [location.pathname]
  );

  /**
   * Determine if an item should be auto-expanded based on level and config
   */
  const shouldAutoExpand = useCallback(
    (item: NavigationDto, level: number): boolean => {
      // Auto-expand if within configured depth
      if (level <= config.autoExpandDepth) {
        return true;
      }

      // Auto-expand if this item or a descendant is active
      if (isItemActive(item) || hasActiveChild(item)) {
        return true;
      }

      return false;
    },
    [config.autoExpandDepth, isItemActive, hasActiveChild]
  );

  // ====================
  // Side Effects
  // ====================

  /**
   * Auto-open drawer if navigating to a route within this drawer's section
   * (Optional enhancement - can be disabled if not desired)
   */
  useEffect(() => {
    if (!isOpen && drawerItems.length > 0) {
      // Check if current route is within this drawer's navigation tree
      const isRouteInDrawer = drawerItems.some((item) => isItemActive(item) || hasActiveChild(item));

      if (isRouteInDrawer) {
        // Auto-open drawer when navigating to a route in this section
        // dispatch(openDrawer());
        // Note: Commented out to avoid surprising users, but available if needed
      }
    }
  }, [currentPath, drawerItems, isOpen, isItemActive, hasActiveChild]);

  // ====================
  // Return ViewModel Interface
  // ====================

  return {
    // State
    isOpen,
    activeSubmenu,
    drawerItems,
    currentPath,
    drawerTitle: drawerRootItem?.subTitle || config.rootNavigationTitle, // Use subtitle from L0 item, fallback to title

    // Actions
    toggleDrawer,
    selectMenuItem,
    navigateToItem,
    goBackToMainMenu,

    // Computed
    activeTopLevelItem,
    visibleItems,
    isInSubmenuView,

    // Helpers
    isItemActive,
    hasActiveChild,
    shouldAutoExpand
  };
};

/**
 * Type guard to check if navigation data is loaded
 */
export const hasNavigationData = (data: NavigationResponseDto | undefined): data is NavigationResponseDto => {
  return data !== undefined && data.navigation !== undefined && data.navigation.length > 0;
};

/**
 * Example usage in view component:
 *
 * ```typescript
 * const PSDrawer: FC<PSDrawerProps> = ({ navigationData, drawerConfig }) => {
 *   const vm = useDrawerViewModel(navigationData, drawerConfig);
 *
 *   return (
 *     <Drawer open={vm.isOpen}>
 *       <IconButton onClick={vm.toggleDrawer}>Toggle</IconButton>
 *       <Typography>{vm.drawerTitle}</Typography>
 *       {vm.visibleItems.map(item => (
 *         <MenuItem
 *           key={item.id}
 *           onClick={() => vm.selectMenuItem(item)}
 *           selected={vm.isItemActive(item)}
 *         >
 *           {item.title}
 *         </MenuItem>
 *       ))}
 *     </Drawer>
 *   );
 * };
 * ```
 */
