import { RouteCategory } from "./types/MenuTypes";
import { MENU_LABELS } from "./constants";
import { NavigationDto, NavigationResponseDto } from "./reduxstore/types";
import { RouteData } from "smart-ui-library";

const localStorageImpersonating: string | null = localStorage.getItem("impersonatingRole");

// Navigation menu ID constants
const YEAR_END_MENU_ID = 55;

export const MenuData = (data: NavigationResponseDto | undefined): RouteCategory[] => {
  if (!data || !data.navigation) {
    return [];
  }

  const finalData: RouteCategory[] = [];

  // Get top-level navigation items (parentId is null)
  const topLevelItems = data.navigation
    .filter((m) => m.parentId === null)
    .sort((a, b) => a.orderNumber - b.orderNumber);

  // Process each top-level item
  topLevelItems.forEach((values: NavigationDto) => {
    // Create menu item if:
    // 1. User has the required role OR
    // 2. No roles required for this menu item
    const hasRequiredRole = values.requiredRoles.length > 0 &&
      values.requiredRoles.some(role => role === localStorageImpersonating);
    const noRolesRequired = values.requiredRoles.length === 0;

    if (hasRequiredRole || noRolesRequired) {
      finalData.push(createRouteCategory(values));
    }
  });

  // Commented out special case for IT Operations role
  // if(localStorageImpersonating === ImpersonationRoles.ItOperations) {
  //   finalData.push(it_operations);
  // }

  return finalData;
};

// Helper function to create a RouteCategory from NavigationDto
const createRouteCategory = (navigationItem: NavigationDto): RouteCategory => {
  return {
    menuLabel: navigationItem.title,
    parentRoute: navigationItem.title.toLowerCase(),
    disabled: navigationItem.disabled,
    underlined: false,
    roles: navigationItem.requiredRoles,
    items: navigationItem.items && navigationItem.items.length > 0
      ? getRouteData(navigationItem.items)
      : undefined
  };
};

const getRouteData = (data: NavigationDto[]): RouteData[] => {
  return data.map((value) => ({
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
  statusId? : number;
  statusName?: string;
  topPage: TopPage[]


}

interface TopPage {
  navigationId?: number;
  topTitle: string;
  topRoute?: string;
  statusId?: number;
  statusName?: string;
  disabled?: boolean;
  subPages: SubPages[]
}

interface SubPages {
  navigationId?: number;
  subTitle?: string;
  subRoute?: string;
  statusId?:number;
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

  // Find the Year End navigation item (ID 54)
  const yearEndList = data.navigation.find((m) => m.id === YEAR_END_MENU_ID);
  if (!yearEndList || !yearEndList.items) {
    return [];
  }

export const menuLevels =(data: NavigationResponseDto | undefined): MenuLevel[] =>{
  const yearEndList = data?.navigation.filter(m=>m.id == 54)[0]; //Id 54 is for Year End. It will have the list of December Activities & Fiscal Close. 
  const menuLevel: MenuLevel[] = [];
  yearEndList?.items?.map((value) => {
    menuLevel.push(
      {
        navigationId: value.id,
        mainTitle: value.title + addSubTitle(value.subTitle),
        statusId: value.statusId,
        statusName: value.statusName,
        topPage: value.items && value.items.length>0? poplulateTopPage(value.items): []
      }
    )
  });
  return menuLevel;
}
const poplulateTopPage = (data: NavigationDto[]):TopPage[] =>{
  const topPage:TopPage[] = [];
  data.map((value) => {
    topPage.push(
      {
        navigationId: value.id,
         topTitle: value.title + addSubTitle(value.subTitle),
         statusId: value.statusId,
         statusName: value.statusName,
         disabled: value.disabled,
         topRoute: value.url,
         subPages: value.items && value.items.length>0 ? populateSubPages(value.items): []
      }
    )
  });
  return topPage;
}
const populateSubPages = (data: NavigationDto[]):SubPages[] =>{
  const subPages:SubPages[] = [];
  data.map((value) => {
    subPages.push(
      {
        navigationId: value.id,
        subTitle: value.title + addSubTitle(value.subTitle),
        statusId: value.statusId,
        statusName: value.statusName,
        disabled: value.disabled, 
        subRoute: value.url
      }
    )
  });
  return subPages;
}

export default MenuData;