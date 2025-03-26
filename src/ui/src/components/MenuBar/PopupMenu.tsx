import { Link } from "@mui/material";
import Button from "@mui/material/Button";
import ClickAwayListener from "@mui/material/ClickAwayListener";
import MenuDivider from "@mui/material/Divider";
import Grow from "@mui/material/Grow";
import MenuItem from "@mui/material/MenuItem";
import MenuList from "@mui/material/MenuList";
import Paper from "@mui/material/Paper";
import Popper from "@mui/material/Popper";
import { FC, useEffect, useRef, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { RouteData } from "./MenuBar";
import { useDispatch } from "react-redux";
import { openDrawer, setActiveSubMenu } from "reduxstore/slices/generalSlice";
import { menuLevels } from "../../MenuData";

type myProps = {
  menuLabel: string;
  items: RouteData[];
  parentRoute: string;
};
const PopupMenu: FC<myProps> = ({ menuLabel, items, parentRoute }) => {
  const location = useLocation();
  const elemRef = useRef<HTMLButtonElement>(null);
  const [open, setOpen] = useState(false);
  const [buttonWidth, setButtonWidth] = useState(0);
  const [onParentActive, setOnParentActive] = useState(false);
  const prevOpen = useRef(open);
  const navigate = useNavigate();
  const dispatch = useDispatch();

  useEffect(() => {
    if (prevOpen.current === true && open === false) {
      elemRef.current!.focus();
    }

    prevOpen.current = open;
  }, [open]);

  useEffect(() => {
    if (elemRef && elemRef.current) {
      setButtonWidth(elemRef.current?.offsetWidth || 0);
    }
    setOnParentActive(location.pathname.indexOf(parentRoute) === 0);
    // eslint-disable-next-line
  }, [location.pathname]);

  const handleToggle = () => {
    setOpen((prevOpen) => !prevOpen);
  };

  const isTitleInMainMenuLevels = (caption: string) => {
    return menuLevels.some((level) => level.mainTitle === caption);
  };

  const handleClose = (event: { target: EventTarget & HTMLElement }, route?: string, caption?: string) => {
    if (elemRef.current && elemRef.current.contains(event.target)) {
      return;
    }

    // Now we need to see if we need to open the drawer and set a menu level inside
    if (caption && isTitleInMainMenuLevels(caption)) {
      dispatch(openDrawer());
      dispatch(setActiveSubMenu(caption));
    }

    if (route) {
      console.log("Navigating to: ", route);
      navigate(route);
    }

    setOpen(false);
  };

  function handleListKeyDown(event: { key: string; preventDefault: () => void }) {
    if (event.key === "Tab") {
      event.preventDefault();
      setOpen(false);
    }
  }
  return (
    <>
      <span
        className={`${
          open || onParentActive
            ? "border-0 border-b-2 border-solid border-gray-300"
            : "border-0 border-b-2 border-solid border-transparent"
        }`}>
        <Button
          className="h-full pl-3"
          ref={elemRef}
          aria-controls={open ? "menu-list-grow" : undefined}
          aria-haspopup="true"
          onClick={handleToggle}>
          {menuLabel}
        </Button>
      </span>
      <Popper
        open={open}
        sx={{ zIndex: 11 }}
        anchorEl={elemRef.current}
        role={undefined}
        transition
        disablePortal>
        {({ TransitionProps, placement }) => (
          <Grow
            {...TransitionProps}
            style={{
              transformOrigin: placement === "bottom" ? "center top" : "center bottom"
            }}>
            <Paper>
              <ClickAwayListener onClickAway={(e) => handleClose(e)}>
                <MenuList
                  autoFocusItem={open}
                  id="menu-list-grow"
                  onKeyDown={handleListKeyDown}>
                  {items.map((item: RouteData, index: number) => {
                    const { route, caption, divider, disabled } = item;

                    return (
                      <div key={index}>
                        <Link
                          href={route}
                          underline="none"
                          color="inherit"
                          onClick={(e) => {
                            e.preventDefault();
                          }}>
                          <MenuItem
                            disabled={disabled}
                            sx={{ minWidth: `${buttonWidth}px` }}
                            onClick={(e) => handleClose(e, route ?? "#", caption)}>
                            {caption}
                          </MenuItem>
                        </Link>
                        {divider && <MenuDivider />}
                      </div>
                    );
                  })}
                </MenuList>
              </ClickAwayListener>
            </Paper>
          </Grow>
        )}
      </Popper>
    </>
  );
};

export default PopupMenu;
