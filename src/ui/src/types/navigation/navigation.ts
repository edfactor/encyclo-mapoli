export interface NavigationRequestDto {
  navigationId?: number;
}

export interface NavigationResponseDto {
  navigation: NavigationDto[];
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
  items: NavigationDto[];
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
