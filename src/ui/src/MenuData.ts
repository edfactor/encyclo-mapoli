import { RouteData } from "smart-ui-library";
import { MENU_LABELS } from "./constants";
import { NavigationDto, NavigationResponseDto } from "./reduxstore/types";
import { RouteCategory } from "./types/MenuTypes";

export const MenuData = (data: NavigationResponseDto | undefined): RouteCategory[] => {
  if (!data || !data.navigation) {
    return [];
  }

  const finalData: RouteCategory[] = [];

  // Get top-level navigation items (parentId is null)
  // Backend has already filtered by user roles, so we don't need to filter again
  const topLevelItems = data.navigation
    .filter((m) => m.parentId === null && (m.isNavigable ?? true))
    .sort((a, b) => a.orderNumber - b.orderNumber);

  // Process each top-level item
  topLevelItems.forEach((values: NavigationDto) => {
    if (values.isNavigable ?? true) {
      finalData.push(createRouteCategory(values));
    }
  });

  // Commented out special case for IT Operations role
  // if(localStorageImpersonating === ImpersonationRoles.ItDevOps) {
  //   finalData.push(it_operations);
  // }

  return finalData;
};

// Helper function to create a RouteCategory from NavigationDto
const createRouteCategory = (navigationItem: NavigationDto): RouteCategory => {
  // Don't show popup menus for sections that use the drawer
  // These sections should open the drawer when clicked, not show a popup
  const drawerOnlySections = ["INQUIRIES", "YEAR END", "DISTRIBUTIONS", "IT DEVOPS"];
  const shouldShowPopup = !drawerOnlySections.includes(navigationItem.title);

  return {
    menuLabel: navigationItem.title,
    parentRoute: navigationItem.url || navigationItem.title.toLowerCase(), // Use URL field for proper routing
    disabled: navigationItem.disabled,
    underlined: false,
    roles: navigationItem.requiredRoles,
    items:
      shouldShowPopup && navigationItem.items && navigationItem.items.length > 0
        ? getRouteData(navigationItem.items)
        : undefined
  };
};

const getRouteData = (data: NavigationDto[]): RouteData[] => {
  const routes: RouteData[] = [];

  data
    .filter((v) => v.isNavigable ?? true)
    .forEach((value) => {
      // If the item has a URL, add it directly to the popup
      if (value.url) {
        routes.push({
          caption: value.title,
          route: value.url,
          disabled: false,
          divider: false,
          requiredPermission: ""
        });
      } else if (value.items && value.items.length > 0) {
        // If no URL but has children (it's a group), recursively get child routes
        routes.push(...getRouteData(value.items));
      }
    });

  return routes;
};

export const drawerTitle = MENU_LABELS.YEAR_END;

// ============================================================================
// REMOVED: menuLevels() and related functions
// ============================================================================
// The hardcoded menuLevels() function has been removed in favor of the new
// 100% data-driven navigation system. The new PSDrawer component uses:
//
// - getL0NavigationByTitle(data, "YEAR END") - Dynamic lookup of L0 items
// - getDrawerItemsForSection(data, "YEAR END") - Get drawer items for any section
// - createDrawerConfig("YEAR END") - Dynamic configuration
//
// These functions are located in:
// - src/ui/src/components/Drawer/utils/navigationStructureUtils.ts
// - src/ui/src/components/Drawer/models/DrawerConfig.ts
//
// The old approach:
// - ❌ Hardcoded "YEAR END" search by string literal
// - ❌ Limited to 3-level depth (MenuLevel → TopPage → SubPages)
// - ❌ Not reusable for other navigation sections
//
// The new approach:
// - ✅ 100% data-driven from API (parentId === null for L0)
// - ✅ Supports unlimited nesting depth via recursive rendering
// - ✅ Works with ANY L0 navigation section
// - ✅ No hardcoded navigation items or IDs
// ============================================================================

export default MenuData;
