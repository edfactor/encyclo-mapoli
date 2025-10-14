import { FC } from "react";
import { useDispatch } from "react-redux";
import { useLocation, useNavigate } from "react-router-dom";
import { openDrawer, setActiveSubMenu } from "../../reduxstore/slices/generalSlice";
import { NavigationResponseDto } from "../../reduxstore/types";
import { RouteCategory } from "../../types/MenuTypes";
import { getFirstNavigableRoute, getL0NavigationForRoute } from "../Drawer/utils/navigationStructureUtils";
import { ICommon } from "../ICommon";
import NavButton from "./NavButton";
import PopupMenu from "./PopupMenu";

export interface MenuBarProps extends ICommon {
  menuInfo: RouteCategory[];
  impersonationMultiSelect?: React.ReactNode;
  navigationData?: NavigationResponseDto;
}

export const MenuBar: FC<MenuBarProps> = ({ menuInfo, impersonationMultiSelect, navigationData }) => {
  const location = useLocation();
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const homeTabSelected = location.pathname === "/";

  // Sections that should open the drawer instead of navigating
  const drawerOnlySections = ["INQUIRIES AND ADJUSTMENTS", "YEAR END", "DISTRIBUTIONS", "IT DEVOPS"];

  // Determine which L0 section contains the current route
  const activeL0Section = getL0NavigationForRoute(navigationData, location.pathname);

  const handleMenuClick = (current: RouteCategory) => {
    if (drawerOnlySections.includes(current.menuLabel)) {
      // Set the active submenu so the drawer knows which section to show, then open drawer
      dispatch(setActiveSubMenu(current.menuLabel));
      // Open drawer for drawer-only sections and navigate to first child route
      dispatch(openDrawer());

      // Navigate to the first available route in this section
      const firstRoute = getFirstNavigableRoute(navigationData, current.menuLabel);
      if (firstRoute) {
        const absolutePath = firstRoute.startsWith("/") ? firstRoute : `/${firstRoute}`;
        navigate(absolutePath, { replace: false });
      }
    } else {
      // Navigate normally for other sections
      const absolutePath = current.parentRoute.startsWith("/") ? current.parentRoute : `/${current.parentRoute}`;
      navigate(absolutePath, { replace: false });
    }
  };

  // Check if a menu item should be underlined (active)
  const isMenuItemActive = (menuLabel: string): boolean => {
    return activeL0Section?.title === menuLabel;
  };

  return (
    <div
      className="menubar"
      style={{ position: "fixed", width: "100%", overflow: "visible", zIndex: 2 }}>
      <div className="navbuttons ml-2">
        <NavButton
          isUnderlined={homeTabSelected}
          onClick={() => {
            navigate("/");
          }}
          label="Home"
        />
        {menuInfo.map((current: RouteCategory, index: number) => {
          // Render top-level items as NavButton only. Previously some items opened a PopupMenu.
          // Clicking a top-level item should either open the drawer (and set the active submenu)
          // or navigate normally.
          return (
            <NavButton
              key={index}
              isUnderlined={isMenuItemActive(current.menuLabel)}
              onClick={() => {
                // If this top-level has child items, set them as the active submenu and open drawer
                if (current.items && current.items.length > 0) {
                  dispatch(setActiveSubMenu(current.menuLabel));
                  dispatch(openDrawer());

                  // Navigate to the first available route in this section so
                  // SmartPSDrawer (which auto-detects the drawer root from route)
                  // will select the correct L0 section to display.
                  const firstRoute = getFirstNavigableRoute(navigationData, current.menuLabel);
                  if (firstRoute) {
                    const absolutePath = firstRoute.startsWith("/") ? firstRoute : `/${firstRoute}`;
                    navigate(absolutePath, { replace: false });
                  }

                  return;
                }

                // Otherwise, fall back to existing handler (which handles drawer-only sections)
                handleMenuClick(current);
              }}
              label={current.menuLabel}
              disabled={current.disabled}
            />
          );
        })}
      </div>
      {impersonationMultiSelect}
    </div>
  );
};

export default MenuBar;
