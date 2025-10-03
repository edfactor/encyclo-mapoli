import { RouteData } from "smart-ui-library";
import { MENU_LABELS } from "./constants";
import { NavigationDto, NavigationResponseDto } from "./reduxstore/types";
import { RouteCategory } from "./types/MenuTypes";

const localStorageImpersonating: string[] = JSON.parse(localStorage.getItem("impersonatingRoles") || "[]");

// Navigation menu ID constants
const YEAR_END_MENU_ID = 55;

export const MenuData = (data: NavigationResponseDto | undefined): RouteCategory[] => {
  if (!data || !data.navigation) {
    return [];
  }

  const finalData: RouteCategory[] = [];

  // Get top-level navigation items (parentId is null)
  const topLevelItems = data.navigation
    .filter((m) => m.parentId === null && (m.isNavigable ?? true))
    .sort((a, b) => a.orderNumber - b.orderNumber);

  // Process each top-level item
  topLevelItems.forEach((values: NavigationDto) => {
    // Create menu item if:
    // 1. User has the required role OR
    // 2. No roles required for this menu item
    const hasRequiredRole =
      values.requiredRoles.length > 0 && values.requiredRoles.some((role) => localStorageImpersonating.includes(role));
    const noRolesRequired = values.requiredRoles.length === 0;

    if ((hasRequiredRole || noRolesRequired) && (values.isNavigable ?? true)) {
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
  return {
    menuLabel: navigationItem.title,
    parentRoute: navigationItem.url || navigationItem.title.toLowerCase(), // Use URL field for proper routing
    disabled: navigationItem.disabled,
    underlined: false,
    roles: navigationItem.requiredRoles,
    items: navigationItem.items && navigationItem.items.length > 0 ? getRouteData(navigationItem.items) : undefined
  };
};

const getRouteData = (data: NavigationDto[]): RouteData[] => {
  return data
    .filter((v) => v.isNavigable ?? true)
    .map((value) => ({
      caption: value.title,
      route: value.url,
      disabled: false,
      divider: false,
      requiredPermission: ""
    }));
};

interface MenuLevel {
  navigationId?: number;
  mainTitle: string;
  statusId?: number;
  statusName?: string;
  topPage: TopPage[];
}

interface TopPage {
  navigationId?: number;
  topTitle: string;
  topRoute?: string;
  statusId?: number;
  statusName?: string;
  disabled?: boolean;
  subPages: SubPages[];
}

interface SubPages {
  navigationId?: number;
  subTitle?: string;
  subRoute?: string;
  statusId?: number;
  statusName?: string;
  disabled?: boolean;
}

export const drawerTitle = MENU_LABELS.YEAR_END;

const addSubTitle = (subTitle?: string): string => {
  return subTitle ? ` (${subTitle})` : "";
};

export const menuLevels = (data: NavigationResponseDto | undefined): MenuLevel[] => {
  if (!data || !data.navigation) {
    return [];
  }

  // Find the Year End navigation item
  const yearEndList = data.navigation.find((m) => m.title === "YEAR END");
  if (!yearEndList || !yearEndList.items) {
    return [];
  }

  return yearEndList.items
    .filter((v) => v.isNavigable ?? true)
    .map((value) => ({
      navigationId: value.id,
      statusId: value.statusId,
      statusName: value.statusName,
      mainTitle: value.title + addSubTitle(value.subTitle),
      topPage: value.items && value.items.length > 0 ? populateTopPage(value.items) : []
    }));
};

const populateTopPage = (data: NavigationDto[]): TopPage[] => {
  return data
    .filter((v) => v.isNavigable ?? true)
    .map((value) => ({
      navigationId: value.id,
      statusId: value.statusId,
      statusName: value.statusName,
      topTitle: value.title + addSubTitle(value.subTitle),
      disabled: value.disabled,
      topRoute: value.url,
      subPages: value.items && value.items.length > 0 ? populateSubPages(value.items) : []
    }));
};

const populateSubPages = (data: NavigationDto[]): SubPages[] => {
  return data
    .filter((v) => v.isNavigable ?? true)
    .map((value) => ({
      navigationId: value.id,
      statusId: value.statusId,
      statusName: value.statusName,
      subTitle: value.title + addSubTitle(value.subTitle),
      disabled: value.disabled,
      subRoute: value.url
    }));
};

export default MenuData;
