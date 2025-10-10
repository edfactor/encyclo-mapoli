/**
 * PSDrawer - Refactored with MVVM Pattern
 *
 * Pure presentation component that uses useDrawerViewModel for all business logic.
 * This version replaces hardcoded "YEAR END" logic with configurable recursive approach.
 *
 * Key improvements from original:
 * - Uses ViewModel hook for all logic (testable without rendering)
 * - Supports arbitrary nesting depth via RecursiveNavItem
 * - Configurable via DrawerConfig (not hardcoded to Year End)
 * - Cleaner separation of concerns
 */

import { ChevronLeft } from "@mui/icons-material";
import {
  Box,
  Divider,
  Drawer,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  SvgIcon,
  SvgIconProps,
  Typography
} from "@mui/material";
import { FC } from "react";
import { ICommon } from "smart-ui-library";
import { drawerClosedWidth, drawerOpenWidth } from "../../constants";
import { NavigationDto, NavigationResponseDto } from "../../reduxstore/types";
import { useDrawerViewModel } from "./hooks";
import { DrawerConfig, getDefaultDrawerConfig } from "./models";
import RecursiveNavItem from "./RecursiveNavItem";

// Drawer Icons
const SidebarIcon = (props: SvgIconProps) => (
  <SvgIcon
    {...props}
    viewBox="0 0 16 16"
    sx={{ fontSize: 20 }}>
    <path d="M6.823 7.823a.25.25 0 0 1 0 .354l-2.396 2.396A.25.25 0 0 1 4 10.396V5.604a.25.25 0 0 1 .427-.177Z" />
    <path d="M1.75 0h12.5C15.216 0 16 .784 16 1.75v12.5A1.75 1.75 0 0 1 14.25 16H1.75A1.75 1.75 0 0 1 0 14.25V1.75C0 .784.784 0 1.75 0ZM1.5 1.75v12.5c0 .138.112.25.25.25H9.5v-13H1.75a.25.25 0 0 0-.25.25ZM11 14.5h3.25a.25.25 0 0 0 .25-.25V1.75a.25.25 0 0 0-.25-.25H11Z" />
  </SvgIcon>
);

const SidebarCloseIcon = (props: SvgIconProps) => (
  <SvgIcon
    {...props}
    viewBox="0 0 16 16"
    sx={{ fontSize: 20, transform: "scaleX(-1)" }}>
    <path d="M6.823 7.823a.25.25 0 0 1 0 .354l-2.396 2.396A.25.25 0 0 1 4 10.396V5.604a.25.25 0 0 1 .427-.177Z" />
    <path d="M1.75 0h12.5C15.216 0 16 .784 16 1.75v12.5A1.75 1.75 0 0 1 14.25 16H1.75A1.75 1.75 0 0 1 0 14.25V1.75C0 .784.784 0 1.75 0ZM1.5 1.75v12.5c0 .138.112.25.25.25H9.5v-13H1.75a.25.25 0 0 0-.25.25ZM11 14.5h3.25a.25.25 0 0 0 .25-.25V1.75a.25.25 0 0 0-.25-.25H11Z" />
  </SvgIcon>
);

export interface PSDrawerProps extends ICommon {
  navigationData?: NavigationResponseDto;
  drawerConfig?: DrawerConfig; // Optional: specify which nav section to display
}

/**
 * PSDrawer - Pure Presentation Component
 *
 * All business logic is in useDrawerViewModel hook.
 * This component only handles rendering based on ViewModel state.
 */
const PSDrawer: FC<PSDrawerProps> = ({
  navigationData,
  drawerConfig = getDefaultDrawerConfig() // Defaults to Year End for backwards compatibility
}) => {
  // ViewModel provides all logic, state, and actions
  const vm = useDrawerViewModel(navigationData, drawerConfig);

  return (
    <>
      {/* Toggle Button */}
      <Box
        key="TitleBarAboveDrawer"
        sx={{
          position: "fixed",
          left: vm.isOpen ? "16px" : "12px",
          top: "179px",
          zIndex: (theme) => theme.zIndex.drawer + 100,
          display: "flex",
          alignItems: "center",
          gap: 1,
          justifyContent: "space-between",
          width: vm.isOpen ? "338px" : "auto",
          transition: (theme) =>
            theme.transitions.create("left", {
              easing: theme.transitions.easing.sharp,
              duration: theme.transitions.duration.enteringScreen
            })
        }}>
        {vm.isOpen && (
          <Typography
            variant="h6"
            sx={{
              color: (theme) => theme.palette.text.primary,
              whiteSpace: "nowrap",
              fontWeight: "bold"
            }}>
            {vm.drawerTitle}
          </Typography>
        )}
        <IconButton
          onClick={vm.toggleDrawer}
          sx={{
            backgroundColor: "transparent",
            "&:hover": {
              backgroundColor: (theme) => theme.palette.action.hover
            }
          }}>
          {vm.isOpen ? <SidebarCloseIcon /> : <SidebarIcon />}
        </IconButton>
      </Box>

      {/* Drawer */}
      <Drawer
        variant="permanent"
        id="DrawerItself"
        sx={{
          width: vm.isOpen ? drawerOpenWidth : drawerClosedWidth,
          display: vm.isOpen ? "block" : "none",
          flexShrink: 0,
          "& .MuiDrawer-paper": {
            width: vm.isOpen ? drawerOpenWidth : drawerClosedWidth,
            borderRight: "1px solid #BDBDBD",
            boxSizing: "border-box",
            overflowX: "hidden",
            "& > *": {
              overflowX: "hidden"
            },
            transition: "all 225ms"
          }
        }}>
        <Box
          id="DrawerContent"
          sx={{
            mt: "215px",
            overflowY: "hidden",
            "&:hover": {
              overflowY: "auto"
            },
            "&::-webkit-scrollbar": {
              width: "8px"
            },
            "&::-webkit-scrollbar-thumb": {
              backgroundColor: "transparent",
              borderRadius: "4px"
            },
            "&:hover::-webkit-scrollbar-thumb": {
              backgroundColor: (theme) => theme.palette.grey[300]
            }
          }}>
          {vm.isInSubmenuView && vm.activeTopLevelItem ? (
            /* Submenu View: Show children of selected top-level item */
            <>
              <ListItemButton
                onClick={vm.goBackToMainMenu}
                sx={{
                  display: "flex",
                  alignItems: "center",
                  gap: 1,
                  borderBottom: 1,
                  borderColor: "divider",
                  position: "sticky",
                  top: 0,
                  backgroundColor: "background.paper",
                  zIndex: 1,
                  "&:hover": {
                    backgroundColor: (theme) => theme.palette.action.hover
                  }
                }}>
                <ChevronLeft />
                <Typography
                  variant="body2"
                  sx={{ fontWeight: "bold" }}>
                  {vm.activeTopLevelItem.title}
                </Typography>
              </ListItemButton>

              <List>
                {vm.activeTopLevelItem.items
                  ?.filter((item: NavigationDto) => !item.disabled && (item.isNavigable ?? true))
                  .map((item: NavigationDto) => (
                    <RecursiveNavItem
                      key={item.id}
                      item={item}
                      level={0}
                      maxAutoExpandDepth={drawerConfig.autoExpandDepth}
                      onNavigate={vm.navigateToItem}
                    />
                  ))}
              </List>
            </>
          ) : (
            /* Main Menu View: Show top-level items */
            <>
              <Divider sx={{ mb: 1 }} />
              <List>
                {vm.drawerItems
                  .filter((item) => item.isNavigable ?? true)
                  .map((item) => {
                    const hasChildren = item.items && item.items.length > 0;
                    const isActive = vm.isItemActive(item);
                    const hasActiveDescendant = vm.hasActiveChild(item);

                    return (
                      <ListItem
                        key={item.id}
                        disablePadding>
                        <ListItemButton
                          onClick={() => (hasChildren ? vm.selectMenuItem(item) : vm.navigateToItem(item))}
                          disabled={item.disabled}
                          sx={{
                            backgroundColor: isActive ? "#0258A515" : "transparent",
                            borderLeft: isActive || hasActiveDescendant ? "4px solid #0258A5" : "4px solid transparent",
                            "&:hover": {
                              backgroundColor: (theme) => theme.palette.action.hover
                            },
                            "& .MuiListItemText-primary": {
                              color: (theme) => theme.palette.text.primary,
                              fontWeight: isActive || hasActiveDescendant ? 600 : "normal"
                            },
                            display: "flex",
                            justifyContent: "space-between",
                            alignItems: "center",
                            opacity: item.disabled ? 0.5 : 1
                          }}>
                          <Box sx={{ display: "flex", flexDirection: "column", flex: 1 }}>
                            <Typography variant="body2">{item.title}</Typography>
                            {item.subTitle && (
                              <Typography
                                variant="caption"
                                sx={{ fontStyle: "italic", color: "text.secondary" }}>
                                {item.subTitle}
                              </Typography>
                            )}
                          </Box>
                          {hasChildren && <ChevronLeft sx={{ transform: "rotate(180deg)" }} />}
                        </ListItemButton>
                      </ListItem>
                    );
                  })}
              </List>
            </>
          )}
        </Box>
      </Drawer>
    </>
  );
};

export default PSDrawer;
