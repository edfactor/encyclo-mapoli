export interface NavigationRequestDto {
  navigationId?: number;
}

export const NavigationCustomSettingsKeys = {
  trackPageStatus: "trackPageStatus",
  useFrozenYear: "useFrozenYear"
} as const;

export interface NavigationCustomSettings {
  trackPageStatus?: boolean;
  useFrozenYear?: boolean;
  [key: string]: string | boolean | undefined;
}

export interface NavigationResponseDto {
  navigation: NavigationDto[];
  customSettings?: NavigationCustomSettings;
}

export interface NavigationDto {
  id: number;
  parentId: number;
  title: string;
  subTitle: string;
  url: string;
  statusId?: number;
  statusName?: string;
  orderNumber: number;
  icon: string;
  requiredRoles: string[];
  disabled: boolean;
  isNavigable?: boolean;
  isReadOnly?: boolean;
  // Prerequisite navigation elements that are currently completed.
  prerequisiteNavigations?: NavigationDto[];
  items: NavigationDto[];
  // Custom settings specific to this navigation item (e.g., trackPageStatus, useFrozenYear)
  customSettings?: NavigationCustomSettings;
}

export interface NavigationStatusDto {
  id: number;
  name?: string;
}

export interface GetNavigationStatusRequestDto {
  id?: number;
}

export interface GetNavigationStatusResponseDto {
  navigationStatusList?: NavigationStatusDto[];
}

export interface UpdateNavigationRequestDto {
  navigationId?: number;
  statusId?: number;
}

export interface UpdateNavigationResponseDto {
  isSuccessful?: boolean;
}

export interface CurrentNavigation {
  navigationId?: number;
  statusId?: number;
  statusName?: string;
}
