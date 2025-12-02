import LogoutIcon from "@mui/icons-material/Logout";
import { Avatar, MenuList, Paper, Popper } from "@mui/material";
import ClickAwayListener from "@mui/material/ClickAwayListener";
import Divider from "@mui/material/Divider";
import Grow from "@mui/material/Grow";
import MenuItem from "@mui/material/MenuItem";
import React, { useRef, useState } from "react";
import { ICommon } from "../ICommon";

export type MenuItemConfig = {
  icon?: React.ReactNode;
  title: string;
  onClick: React.MouseEventHandler<HTMLLIElement>;
};

export interface WelcomeDisplayProps extends ICommon {
  userName: string;
  oktaEnabled: boolean;
  additionalInfo?: string;
  logout: () => void;
  items?: MenuItemConfig[];
}

export const WelcomeDisplay: React.FC<WelcomeDisplayProps> = ({
  userName,
  oktaEnabled,
  logout,
  items = [],
  additionalInfo = ""
}) => {
  const anchorRef = useRef<HTMLDivElement>(null);
  const [openPopver, setOpenPopover] = useState<boolean>(false);

  if (!userName || !oktaEnabled) {
    return <></>;
  }

  const handleLogout = () => logout();
  const handleListKeyDown = (event: React.KeyboardEvent) => {
    if (event.key === "Tab") {
      event.preventDefault();
      setOpenPopover(false);
    } else if (event.key === "Escape") {
      setOpenPopover(false);
    }
  };

  const avatarText = userName.substring(0, 2).toUpperCase();
  const formattedUsername: string = [
    userName.charAt(0).toUpperCase(),
    userName.charAt(1).toUpperCase() + userName.slice(2)
  ].join(". ");
  return (
    <div
      style={{ position: "fixed", right: "20px" }}
      className="relative z-10 text-2xl font-normal text-dsm-grey"
      onClick={() => setOpenPopover((prev) => !prev)}
      ref={anchorRef}>
      <div id="welcome-user">
        <Avatar
          sx={{
            marginRight: 3,
            backgroundColor: "#0258a5",
            width: "24px",
            height: "24px",
            fontSize: "13px"
          }}>
          {avatarText}
        </Avatar>
        <div className="username">
          {formattedUsername}
          {additionalInfo}
        </div>
      </div>
      <Popper
        open={openPopver}
        anchorEl={anchorRef.current}
        role={undefined}
        placement="bottom-start"
        transition
        disablePortal>
        {({ TransitionProps, placement }) => (
          <Grow
            {...TransitionProps}
            style={{
              transformOrigin: placement === "bottom-start" ? "left top" : "left bottom"
            }}>
            <Paper>
              <ClickAwayListener onClickAway={() => setOpenPopover(false)}>
                <MenuList
                  autoFocusItem={openPopver}
                  id="composition-menu"
                  aria-labelledby="composition-button"
                  onKeyDown={handleListKeyDown}>
                  {items.map((item: MenuItemConfig, index: number) => (
                    <MenuItem
                      key={index}
                      onClick={item.onClick}>
                      {item.icon} {item.title}
                    </MenuItem>
                  ))}
                  <Divider />
                  <MenuItem onClick={handleLogout}>
                    <LogoutIcon /> Logout
                  </MenuItem>
                </MenuList>
              </ClickAwayListener>
            </Paper>
          </Grow>
        )}
      </Popper>
    </div>
  );
};

export default WelcomeDisplay;
