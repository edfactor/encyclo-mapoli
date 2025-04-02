import { FC } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import "./MenuBar.css";
import { ICommon } from "../Layout/ICommon";
import NavButton from "./NavButton";
import PopupMenu from "./PopupMenu";
import { RouteCategory } from "smart-ui-library";

export type RouteData = {
  caption: string;
  route: string;
  divider?: boolean;
  disabled?: boolean;
  requiredPermission?: string;
};

export interface MenuBarProps extends ICommon {
  menuInfo: RouteCategory[];
  impersonationMultiSelect?: React.ReactNode;
}

export const MenuBar: FC<MenuBarProps> = ({ menuInfo, impersonationMultiSelect }) => {
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
              key={index}
              menuLabel={current.menuLabel}
              items={current.items}              
              parentRoute={current.parentRoute}
            />
          ) : (
            <NavButton
              key={index}
              isUnderlined={current.underlined ?? false}
              onClick={() => {
                navigate(current.parentRoute);
              }}
              label={current.menuLabel}
            />
          );
        })}
      </div>
      {impersonationMultiSelect}
    </div>
  );
};

export default MenuBar;
