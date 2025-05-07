import { RouteCategory } from "./types/MenuTypes";
import { CAPTIONS, MENU_LABELS, ROUTES } from "./constants";
import { ImpersonationRoles, NavigationDto, NavigationResponseDto } from "./reduxstore/types";
import { RouteData } from "smart-ui-library";

const localStorageImpersonating: string | null = localStorage.getItem("impersonatingRole");

export const MenuData = (data:NavigationResponseDto | undefined): RouteCategory[] => {
  const finalData: RouteCategory[] = [];
  data?.navigation.filter(m=>m.parentId ==null).sort((a, b) => a.orderNumber - b.orderNumber).map((values:NavigationDto) => {
    if(values.requiredRoles.length>0 && values.requiredRoles.filter(m=>m == localStorageImpersonating).length>0)
    {
      finalData.push({
        menuLabel: values.title, 
        parentRoute: values.title.toLocaleLowerCase(),
        disabled: values.disabled, 
        underlined: false, 
        roles: values.requiredRoles, 
        items: values.items && values.items.length>0 ? getRouteData(values.items): undefined
      });

    }
    else if(values.requiredRoles.length ==0) {
      finalData.push({
        menuLabel: values.title, 
        parentRoute: values.title.toLocaleLowerCase(),
        disabled: values.disabled, 
        underlined: false, 
        roles: values.requiredRoles, 
        items: values.items && values.items.length>0 ? getRouteData(values.items): undefined
      });
    }
  });
  // if(localStorageImpersonating == ImpersonationRoles.ItOperations) {
  // finalData.push(it_operations);
  // }
  return finalData;
}
const getRouteData = (data: NavigationDto[]):RouteData[] =>{
  const response: RouteData[] = [];
  data.map((value)  => {
    const obj: RouteData = { // Initialize with an empty object or default values
      caption: value.title,
      route: value.url, 
      disabled: false,
      divider: false,
      requiredPermission: ""
    };
    response.push(obj);
  });
  return response;
}

interface MenuLevel {
  mainTitle: string;
  topPage: TopPage[]
}
interface TopPage {
  topTitle: string;
    topRoute?: string;
    disabled?: boolean;
    subPages: SubPages[]
}
interface SubPages {
  subTitle?: string;
  subRoute?: string;
  disabled?: boolean;
}

export const drawerTitle = MENU_LABELS.YEAR_END;

const addSubTitle = (subTitle?:string):string =>{
  return subTitle? ` (${subTitle})` : "";
}

export const menuLevels =(data: NavigationResponseDto | undefined): MenuLevel[] =>{
  const yearEndList = data?.navigation.filter(m=>m.id == 54)[0]; //Id 54 is for Year End. It will have the list of December Activities & Fiscal Close. 
  const menuLevel: MenuLevel[] = [];
  yearEndList?.items?.map((value) => {
    menuLevel.push(
      {
        mainTitle: value.title + addSubTitle(value.subTitle),
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
         topTitle: value.title + addSubTitle(value.subTitle),
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
        subTitle: value.title + addSubTitle(value.subTitle),
        disabled: value.disabled, 
        subRoute: value.url
      }
    )
  });
  return subPages;
}

export default MenuData;
