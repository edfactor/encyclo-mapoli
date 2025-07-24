export type RouteData = {
  caption: string;
  route: string;
  divider?: boolean;
  disabled?: boolean;
  requiredPermission?: string;
};

export type RouteCategory = {
  menuLabel: string;
  parentRoute: string;
  items?: RouteData[];
  underlined?: boolean;
  disabled?: boolean;
  roles?: string[];
};
