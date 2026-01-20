import { FC, useCallback, useMemo } from "react";
import { useDispatch } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import { openDrawer, setActiveSubMenu } from "../../reduxstore/slices/generalSlice";
import { NavigationResponseDto } from "../../reduxstore/types";
import { RouteCategory } from "../../types/MenuTypes";
import EnvironmentUtils from "../../utils/environmentUtils";
import { getFirstNavigableRoute, getL0NavigationForRoute } from "../Drawer/utils/navigationStructureUtils";
import { ICommon } from "../ICommon";
import NavButton from "./NavButton";
import PageSearch from "./PageSearch";

export interface MenuBarProps extends ICommon {
  menuInfo: RouteCategory[];
  impersonationMultiSelect?: React.ReactNode;
  navigationData?: NavigationResponseDto;
}

// Calculate menu bar top position based on environment banner visibility
const getMenuBarTop = (): number => {
  const envMode = EnvironmentUtils.envMode;
  const showsEnvironmentBanner = envMode && envMode !== "production";
  const envBannerHeight = showsEnvironmentBanner ? 52 : 0;
  const appBannerHeight = 52;
  return envBannerHeight + appBannerHeight;
};

export const MenuBar: FC<MenuBarProps> = ({ menuInfo, impersonationMultiSelect, navigationData }) => {
  const location = useLocation();
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const homeTabSelected = location.pathname === "/";

  const drawerOnlySections = useMemo(() => {
    if (!navigationData?.navigation) return [] as string[];

    return navigationData.navigation
      .filter((item) => item.parentId === null && (item.items?.length ?? 0) > 0)
      .map((item) => item.title);
  }, [navigationData]);

  // Determine which L0 section contains the current route
  const activeL0Section = getL0NavigationForRoute(navigationData, location.pathname);

  // Handler for Home button click
  const handleHomeClick = useCallback(() => {
    localStorage.removeItem("navigationId");
    navigate("/");
  }, [navigate]);

  // Handler for menu item clicks - handles both drawer-only and normal navigation
  const handleMenuItemClick = useCallback(
    (current: RouteCategory) => {
      // Clear stored navigation ID when switching L0 sections
      localStorage.removeItem("navigationId");

      // Check if this item has children (from menuInfo) - should open drawer
      const hasMenuChildren = current.items && current.items.length > 0;

      // Check if this is a drawer-only section (from navigationData)
      const isDrawerOnlySection = drawerOnlySections.includes(current.menuLabel);

      if (hasMenuChildren || isDrawerOnlySection) {
        // Set the active submenu so the drawer knows which section to show, then open drawer
        dispatch(setActiveSubMenu(current.menuLabel));
        dispatch(openDrawer());

        // Navigate to the first available route in this section
        const firstRoute = getFirstNavigableRoute(navigationData, current.menuLabel);
        if (firstRoute) {
          const absolutePath = firstRoute.startsWith("/") ? firstRoute : `/${firstRoute}`;
          navigate(absolutePath, { replace: false });
        }
      } else {
        // Navigate normally for sections without children
        const absolutePath = current.parentRoute.startsWith("/") ? current.parentRoute : `/${current.parentRoute}`;
        navigate(absolutePath, { replace: false });
      }
    },
    [dispatch, drawerOnlySections, navigate, navigationData]
  );

  // Check if a menu item should be underlined (active)
  const isMenuItemActive = useCallback(
    (menuLabel: string): boolean => {
      return activeL0Section?.title === menuLabel;
    },
    [activeL0Section]
  );

  const menuBarTop = getMenuBarTop();

  return (
    <div
      className="fixed z-[999] flex w-full items-center justify-between overflow-visible bg-dsm-secondary pr-4 text-white shadow-[0px_2px_4px_-1px_rgba(0,0,0,0.2),0px_4px_5px_0px_rgba(0,0,0,0.14),0px_1px_10px_0px_rgba(0,0,0,0.12)] [&_button]:flex [&_button]:items-center [&_button]:justify-center [&_button]:gap-2 [&_button]:px-4 [&_button]:py-[9px] [&_button]:font-lato [&_button]:text-base [&_button]:font-normal [&_button]:uppercase [&_button]:leading-normal [&_button]:text-white"
      style={{ top: `${menuBarTop}px` }}>
      <div className="ml-2 flex items-center gap-0.5">
        <NavButton
          isUnderlined={homeTabSelected}
          onClick={handleHomeClick}
          label="Home"
        />
        {menuInfo.map((current: RouteCategory, index: number) => (
          <NavButton
            key={index}
            isUnderlined={isMenuItemActive(current.menuLabel)}
            onClick={() => handleMenuItemClick(current)}
            label={current.menuLabel}
            disabled={current.disabled}
          />
        ))}
      </div>
      <div className="mr-4 flex items-center gap-4">
        <PageSearch navigationData={navigationData} />
        {impersonationMultiSelect && (
          <>
            <div className="h-8 w-px bg-white opacity-30" />
            {impersonationMultiSelect}
          </>
        )}
      </div>
    </div>
  );
};

export default MenuBar;
