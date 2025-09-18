import { FC } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { ICommon } from "../ICommon";
import NavButton from "./NavButton";
import PopupMenu from "./PopupMenu";
import { RouteCategory } from "../../types/MenuTypes";
import { NavigationResponseDto } from "reduxstore/types";

export interface MenuBarProps extends ICommon {
  menuInfo: RouteCategory[];
  impersonationMultiSelect?: React.ReactNode;
  navigationData?: NavigationResponseDto;
}

export const MenuBar: FC<MenuBarProps> = ({ menuInfo, impersonationMultiSelect, navigationData }) => {
  const location = useLocation();
  const navigate = useNavigate();
  const homeTabSelected = location.pathname === "/";

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
          return current.items ? (
            <PopupMenu
              navigationData={navigationData}
              key={index}
              menuLabel={current.menuLabel}
              items={current.items}
              parentRoute={current.parentRoute}
              disabled={current.disabled}
            />
          ) : (
            <NavButton
              key={index}
              isUnderlined={current.underlined ?? false}
              onClick={() => {
                const absolutePath = current.parentRoute.startsWith("/")
                  ? current.parentRoute
                  : `/${current.parentRoute}`;

                navigate(absolutePath, { replace: false });
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
